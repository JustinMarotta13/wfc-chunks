using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WaveFunctionCollapse
{
    public class TilemapOutput : IOutputCreator<Tilemap>
    {
        private ValuesManager<TileBase> valuesManager;
        private Tilemap outputImage;

        public TilemapOutput(ValuesManager<TileBase> valuesManager, Tilemap outputImage)
        {
            this.valuesManager = valuesManager;
            this.outputImage = outputImage;
        }

        public Tilemap OutputImage => outputImage;

        public void CreateOutput(PatternManager patternManager, int[][] outputValues, int width, int height)
        {
            if (outputValues.Length == 0)
            {
                return;
            }
            this.outputImage.ClearAllTiles();

            int[][] valuesGrid;

            valuesGrid = patternManager.ConvertPatternToValues<TileBase>(outputValues);

            for (int row = 0; row < height; row++)
            {
                for (int col = 0; col < width; col++)
                {
                    TileBase tile = (TileBase)this.valuesManager.GetValueFromIndex(valuesGrid[row][col]).Value;
                    this.outputImage.SetTile(new Vector3Int(col, row, 0), tile);
                }
            }
        }
    }
}

