using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Linq;

namespace WaveFunctionCollapse
{
    public class TilemapChunkOutput
    {
        private ValuesManager<TileBase> valuesManager;
        private Tilemap outputImage;

        public TilemapChunkOutput(ValuesManager<TileBase> valuesManager)
        {
            this.valuesManager = valuesManager;
        }

        public Tilemap OutputImage => outputImage;

        public void CreateOutput(PatternManager patternManager, int[][] outputValues, int width, int height, Chunk chunk)
        {
            chunk.gameObject = CreateGameObject();

            if (outputValues.Length == 0) return;

            outputImage.ClearAllTiles();

            int[][] valuesGrid = patternManager.ConvertPatternToValues<TileBase>(outputValues);
            valuesGrid = RemoveBordersFromChunk(valuesGrid, chunk);

            for (int row = 0; row < valuesGrid.Length; row++)
            {
                for (int col = 0; col < valuesGrid[0].Length; col++)
                {
                    TileBase tile = (TileBase)this.valuesManager.GetValueFromIndex(valuesGrid[row][col]).Value;
                    this.outputImage.SetTile(new Vector3Int(col, row, 0), tile);
                }
            }
        }

        private GameObject CreateGameObject()
        {
            GameObject gameObject = new GameObject("Chunk");
            this.outputImage = gameObject.AddComponent<Tilemap>();
            gameObject.AddComponent<TilemapRenderer>();
            return gameObject;
        }

        private int[][] RemoveBordersFromChunk(int[][] inputOutputValues, Chunk chunk)
        {
            int[][] outputValues = inputOutputValues;

            if (chunk.hasLeftNeighbor)
            {
                outputValues = outputValues.Where((arr, i) => true)
                          .Select(arr => arr.Where((item, i) => i != 0)
                                          .ToArray()).ToArray();
            }
            if (chunk.hasRightNeighbor)
            {
                outputValues = outputValues.Where((arr, i) => true)
                          .Select(arr => arr.Where((item, i) => i != outputValues[0].Length - 1)
                                          .ToArray()).ToArray();
            }
            if (chunk.hasTopNeighbor)
            {
                outputValues = outputValues.Where((arr, i) => i != outputValues.Length - 1)
                          .Select(arr => arr.Where((item, i) => true)
                                          .ToArray()).ToArray();
            }
            if (chunk.hasBottomNeighbor)
            {
                outputValues = outputValues.Where((arr, i) => i != 0)
                          .Select(arr => arr.Where((item, i) => true)
                                          .ToArray()).ToArray();
            }
            return outputValues;
        }
    }
}