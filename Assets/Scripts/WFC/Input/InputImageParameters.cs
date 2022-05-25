using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WaveFunctionCollapse
{
    public class InputImageParameters
    {
        Vector2Int? bottomRightTileCoords = null;
        Vector2Int? topLeftoTileCoords = null;
        BoundsInt inputTileMapBounds;
        TileBase[] inputTilemapTilesArray;
        Queue<TileContainer> stackOfTiles = new Queue<TileContainer>();
        private int width = 0, height = 0;


        public Queue<TileContainer> StackOfTiles { get => stackOfTiles; set => stackOfTiles = value; }
        public int Height { get => height; }
        public int Width { get => width; }

        public InputImageParameters(Tilemap _inputTileMap)
        {
            this.inputTileMapBounds = _inputTileMap.cellBounds;
            this.inputTilemapTilesArray = _inputTileMap.GetTilesBlock(inputTileMapBounds);
            ExtractNonEmptyTIles();
            VerifyInputTiles();
        }

        private void ExtractNonEmptyTIles()
        {
            for (int row = 0; row < inputTileMapBounds.size.y; row++)
            {
                for (int col = 0; col < inputTileMapBounds.size.x; col++)
                {
                    int index = col + (row * inputTileMapBounds.size.x);
                    TileBase tile = inputTilemapTilesArray[index];
                    if (bottomRightTileCoords == null && tile != null)
                    {
                        bottomRightTileCoords = new Vector2Int(col, row);
                    }
                    if (tile != null)
                    {
                        stackOfTiles.Enqueue(new TileContainer(tile, col, row));
                        topLeftoTileCoords = new Vector2Int(col, row);
                    }
                }
            }
        }

        private void VerifyInputTiles()
        {
            if (topLeftoTileCoords.HasValue == false || bottomRightTileCoords.HasValue == false)
            {
                throw new System.Exception("WFC: Input tIlemap is empty");
            }
            int minX = bottomRightTileCoords.Value.x;
            int maxX = topLeftoTileCoords.Value.x;
            int minY = bottomRightTileCoords.Value.y;
            int maxY = topLeftoTileCoords.Value.y;
            width = Mathf.Abs(maxX - minX) + 1;
            height = Mathf.Abs(maxY - minY) + 1;

            int tileCount = width * height;
            if (stackOfTiles.Count != tileCount)
            {
                throw new System.Exception("WFC: Tilemap has empty fields");
            }
            if (stackOfTiles.Any(tile => tile.X > maxX || tile.X < minX || tile.Y > maxY || tile.Y < minY))
            {
                throw new System.Exception("WFC: Tilemap image should be a filled rectangle");
            }
        }


    }

}
