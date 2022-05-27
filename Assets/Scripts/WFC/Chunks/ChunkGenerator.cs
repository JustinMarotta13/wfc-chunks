using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WaveFunctionCollapse
{
    public class ChunkGenerator : MonoBehaviour
    {
        public Tilemap inputImage;
        public Grid gridObject;

        [Tooltip("For tiles usualy set to 1. If tile contain just a color can set to higher value")]
        public int patternSize;

        [Tooltip("How many times algorithm will try creating the output before quiting")]
        public int maxIterations;

        [Tooltip("Don't use tile frequency - each tile has equal weight")]
        public bool equalWeights = false;

        [Tooltip("Chunk will be NxN tiles")]
        public int chunkSize = 5;

        [Tooltip("NxM Chunks to generate initially")]
        public Vector2 dimensionOfChunks = new Vector2(3, 6);

        [Tooltip("Coordinates of the chunk to be generated")]
        public Vector2Int chunkCoordinateToAdd = new Vector2Int(0, 0);

        private ChunkMap chunkMap;

        public void GenerateChunks()
        {
            Debug.Log("Generating chunks");

            chunkMap = new ChunkMap();

            for (int x = 0; x < dimensionOfChunks.x; x++)
            {
                for (int y = 0; y < dimensionOfChunks.y; y++)
                {
                    WFChunks wfc = new WFChunks(this.inputImage, patternSize, this.chunkSize, this.maxIterations, this.equalWeights, chunkMap, new ChunkCoordinate(x, y));
                    wfc.CreateNewTilemap(gridObject);
                    Chunk chunk = wfc.GetOutputTileMap();
                    chunkMap.AddChunk(new ChunkCoordinate(x, y), chunk);
                    chunk.gameObject.name = "Chunk_" + x + "_" + y;
                    TranslateChunk(chunk.gameObject, x, y);
                    Debug.Log("Generated chunk (" + x + ", " + y + ")");
                }
            }
        }

        public void AddChunk()
        {
            if (chunkMap == null) chunkMap = new ChunkMap();
            if (chunkMap.ContainsChunk(chunkCoordinateToAdd.x, chunkCoordinateToAdd.y))
            {
                chunkMap.RemoveChunk(chunkCoordinateToAdd.x, chunkCoordinateToAdd.y);
            }
            WFChunks wfc = new WFChunks(this.inputImage, 1, this.chunkSize, this.maxIterations, this.equalWeights, chunkMap, new ChunkCoordinate(chunkCoordinateToAdd.x, chunkCoordinateToAdd.y));
            wfc.CreateNewTilemap(gridObject);
            Chunk chunk = wfc.GetOutputTileMap();
            chunkMap.AddChunk(new ChunkCoordinate(chunkCoordinateToAdd.x, chunkCoordinateToAdd.y), chunk);
            chunk.gameObject.name = "Chunk_" + chunkCoordinateToAdd.x + "_" + chunkCoordinateToAdd.y;
            TranslateChunk(chunk.gameObject, chunkCoordinateToAdd.x, chunkCoordinateToAdd.y);
            Debug.Log("Generated chunk (" + chunkCoordinateToAdd.x + ", " + chunkCoordinateToAdd.y + ")");
        }

        public void SaveTilemap()
        {
            // var output = wfchunks.GetOutputTileMap();
            // if (output != null)
            // {
            //     outputImage = output;
            //     GameObject objectToSave = outputImage.gameObject;

            //     PrefabUtility.SaveAsPrefabAsset(objectToSave, outputPath);
            // }
        }

        private void TranslateChunk(GameObject chunk, int x, int y)
        {
            chunk.transform.Translate(new Vector3(x * (chunkSize), y * (chunkSize), 0));
        }
    }
}