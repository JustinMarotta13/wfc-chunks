using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WaveFunctionCollapse
{
    public class WFChunks 
    {
        Tilemap inputTilemap;
        ValuesManager<TileBase> valuesManager;
        PatternManager patternManager;
        ChunkCore core;
        int outputWidth;
        int outputHeight;
        int patternSize;
        int maxIterations;
        bool equalWeights;
        TilemapChunkOutput tileOutput;

        Chunk chunk;

        ChunkCoordinate chunkLocation;
        ChunkMap chunkMap;

        public WFChunks(Tilemap inputTilemap, int patternSize, int chunkSize, int maxIterations, bool equalWeights, ChunkMap chunkMap, ChunkCoordinate chunkLocation)
        {
            this.patternSize = patternSize;
            this.inputTilemap = inputTilemap;
            this.maxIterations = maxIterations;
            this.equalWeights = equalWeights;
            this.chunkMap = chunkMap;
            this.outputWidth = chunkSize;
            this.outputHeight = chunkSize;
            this.chunkLocation = chunkLocation;
            this.chunk = new Chunk(chunkSize, chunkLocation);

            if (chunkLocation.Equals(0, 0) == false) AdjustOutputHeightWidthAccordingToNeighboringChunks();
        }

        public void CreateNewTilemap(Grid gridObject)
        {
            Debug.Log("Creating New Tilemap");
            // Get the tiles as jagged array of values
            IInputReader<TileBase> reader = new InputReader(inputTilemap);
            IValue<TileBase>[][] gridOfValues = reader.ReadInputToGrid();
            
            if (gridOfValues == null) throw new System.Exception("Tilemap must contain only 1 square of non-empty tiles. See the example prefab.");

            valuesManager = new ValuesManager<TileBase>(gridOfValues);
            patternManager = new PatternManager(this.patternSize);
            patternManager.ProcessGrid(valuesManager, this.equalWeights);

            ChunkCore core = new ChunkCore(this.outputWidth, this.outputHeight, patternManager, this.maxIterations);
            int[][] outputValues = core.CreateOutputGrid(chunkMap, chunk);
            tileOutput = new TilemapChunkOutput(valuesManager, gridObject);
            tileOutput.CreateOutput(patternManager, outputValues, outputWidth, outputHeight, chunk);
        }

        public Chunk GetOutputTileMap()
        {
            if (tileOutput == null) return null;
            return chunk;
        }

        private void AdjustOutputHeightWidthAccordingToNeighboringChunks()
        {
            bool hasLeftNeighbor = chunkMap.ChunkHasLeftNeighbor(chunkLocation);
            if (hasLeftNeighbor) 
            {
                this.outputWidth++;
                this.chunk.hasLeftNeighbor = true;
                this.chunk.left = chunkMap.GetChunk(new ChunkCoordinate(chunkLocation.x - 1, chunkLocation.y));
            }

            bool hasRightNeighbor = chunkMap.ChunkHasRightNeighbor(chunkLocation);
            if (hasRightNeighbor) 
            {
                this.outputWidth++;
                this.chunk.hasRightNeighbor = true;
                this.chunk.right = chunkMap.GetChunk(new ChunkCoordinate(chunkLocation.x + 1, chunkLocation.y));
            }

            bool hasTopNeighbor = chunkMap.ChunkHasTopNeighbor(chunkLocation);
            if (hasTopNeighbor) 
            {
                this.outputHeight++;
                this.chunk.hasTopNeighbor = true;
                this.chunk.above = chunkMap.GetChunk(new ChunkCoordinate(chunkLocation.x, chunkLocation.y + 1));
            }

            bool hasBottomNeighbor = chunkMap.ChunkHasBottomNeighbor(chunkLocation);
            if (hasBottomNeighbor) 
            {
                this.outputHeight++;
                this.chunk.hasBottomNeighbor = true;
                this.chunk.below = chunkMap.GetChunk(new ChunkCoordinate(chunkLocation.x, chunkLocation.y - 1));
            }
        }
    }
}