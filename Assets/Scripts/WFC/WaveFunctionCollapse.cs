using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WaveFunctionCollapse
{
    public class WaveFunctionCollapse 
    {
        Tilemap inputTilemap, outputImage;
        IInputReader<TileBase> reader;
        ValuesManager<TileBase> valuesManager;
        PatternManager patternManager;
        WFCCore core;
        int outputWidth;
        int outputHeight;
        int patternSize;
        int maxIterations;
        bool equalWeights;
        string strategyName = null;
        IOutputCreator<Tilemap> tileOutput = null;

        public WaveFunctionCollapse(Tilemap inputTilemap, Tilemap outputImage, int patternSize, int outputWidth, int outputHeight, int maxIterations, bool equalWeights, string strategyName=null)
        {
            this.patternSize = patternSize;
            this.inputTilemap = inputTilemap;
            this.outputHeight = outputHeight;
            this.outputWidth = outputWidth;
            this.maxIterations = maxIterations;
            this.outputImage = outputImage;
            this.equalWeights = equalWeights;
            
            if (strategyName != null)
            {
                this.strategyName = strategyName;
            }
        }
        
        public void CreateNewTileMap()
        {
            reader = new InputReader(this.inputTilemap);
            patternManager = new PatternManager(this.patternSize);
            var gridOfValues = reader.ReadInputToGrid();
            if (gridOfValues != null)
            {
                valuesManager = new ValuesManager<TileBase>(gridOfValues);
                patternManager.ProcessGrid(valuesManager, this.equalWeights, this.strategyName);
                core = new WFCCore(this.outputWidth, this.outputHeight, patternManager,this.maxIterations);
                int[][] wfcOutput = core.CreateOutputGrid();
                tileOutput = new TilemapOutput(valuesManager, this.outputImage);
                tileOutput.CreateOutput(patternManager,wfcOutput, outputWidth, outputHeight);
                return;
            }
            throw new System.Exception("Tilemap must contain only 1 square of non-empty tiles. See the example prefab.");
        }

        public void CreateNewTileMap(Tilemap inputTilemap)
        {
            // Get the tiles as jagged array of values
            reader = new InputReader(inputTilemap);
            var gridOfValues = reader.ReadInputToGrid();
            
            if (gridOfValues == null) throw new System.Exception("Tilemap must contain only 1 square of non-empty tiles. See the example prefab.");

            valuesManager = new ValuesManager<TileBase>(gridOfValues);
            patternManager = new PatternManager(this.patternSize);
            patternManager.ProcessGrid(valuesManager, this.equalWeights, this.strategyName);
            core = new WFCCore(this.outputWidth, this.outputHeight, patternManager, this.maxIterations);

            int[][] wfcOutput = core.CreateOutputGrid();
            tileOutput = new TilemapOutput(valuesManager, this.outputImage);
            tileOutput.CreateOutput(patternManager, wfcOutput, outputWidth, outputHeight);
        }

        public Tilemap GetOutputTileMap()
        {
            if (tileOutput == null)
            {
                return null;
            }
            return tileOutput.OutputImage;
        }
    }
}
