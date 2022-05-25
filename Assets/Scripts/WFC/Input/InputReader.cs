using UnityEngine.Tilemaps;
using Helpers;

namespace WaveFunctionCollapse
{
    public class InputReader : IInputReader<TileBase>
    {
        private Tilemap _inputTileMap;

        public InputReader(Tilemap inputTileMap)
        {
            _inputTileMap = inputTileMap;
        }

        public IValue<TileBase>[][] ReadInputToGrid()
        {
            TileBase[][] grid = ReadInputTileMap();
            TileBaseValue[][] gridOValues = null;

            if (grid != null)
            {
                gridOValues = MyCollectionExtension.CreateJaggedArray<TileBaseValue[][]>(grid.Length, grid[0].Length);
                for (int row = 0; row < grid.Length; row++)
                {
                    for (int col = 0; col < grid[row].Length; col++)
                    {
                        gridOValues[row][col] = new TileBaseValue(grid[row][col]);
                    }
                }
            }
            return gridOValues;
        }

        public TileBase[][] ReadInputTileMap()
        {
            InputImageParameters imageParameters = new InputImageParameters(_inputTileMap);
            return CreateTileBaseGrid(imageParameters);
        }

        private TileBase[][] CreateTileBaseGrid(InputImageParameters inputImageParameters)
        {
            TileBase[][] gridOfInputTiles = null;

            gridOfInputTiles = MyCollectionExtension.CreateJaggedArray<TileBase[][]>(inputImageParameters.Height, inputImageParameters.Width);
            for (int row = 0; row < inputImageParameters.Height; row++)
            {
                for (int col = 0; col < inputImageParameters.Width; col++)
                {
                    gridOfInputTiles[row][col] = inputImageParameters.StackOfTiles.Dequeue().Tile;
                }
            }
            return gridOfInputTiles;
        }
    }
}