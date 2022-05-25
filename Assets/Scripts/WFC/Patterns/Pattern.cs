using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helpers;
using System.Linq;

namespace WaveFunctionCollapse
{
    public class Pattern
    {
        private int _index;
        private int[][] _grid;

        public string HashIndex { get; set; }
        public int Index { get => _index; }

        public Pattern(int[][] gridValues, string hashcode, int index)
        {
            _grid = gridValues;
            HashIndex = hashcode;
            this._index = index;
        }

        public void SetGridValue(int x, int y, int value)
        {
            _grid[x][y] = value;
        }

        public int GetGridValue(int x, int y)
        {
            return _grid[y][x];
        }

        public bool CheckValueAtPosition(int x, int y, int value)
        {
            return value.Equals(GetGridValue(x, y));
        }

        internal bool ComparePatternToAnotherPattern(Direction dir, Pattern pattern)
        {
            int[][] myGrid = GetGridValuesInDIrection(dir);
            int[][] otherGrid = pattern.GetGridValuesInDIrection(dir.GetOppositeDirectionTo());

            for (int i = 0; i < myGrid.Length; i++)
            {
                for (int j = 0; j < myGrid[0].Length; j++)
                {
                    if(myGrid[i][j] != otherGrid[i][j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private int[][] GetGridValuesInDIrection(Direction dir)
        {
            int[][] gridPartToCompare;
            switch (dir)
            {
                case Direction.Up:
                    //gridPartToCompare = MyCollectableExtension.CreateJaggedArray<int[][]>(grid.Length - 1, grid.Length);
                    gridPartToCompare = MyCollectionExtension.CreateJaggedArray<int[][]>(_grid.Length-1, _grid.Length);
                    CreatePartOfGrid(0, _grid.Length, 1, _grid.Length, gridPartToCompare);
                    break;
                case Direction.Down:
                    gridPartToCompare = MyCollectionExtension.CreateJaggedArray<int[][]>(_grid.Length-1, _grid.Length);
                    CreatePartOfGrid(0, _grid.Length, 0, _grid.Length-1, gridPartToCompare);
                    break;
                case Direction.Left:
                    gridPartToCompare = MyCollectionExtension.CreateJaggedArray<int[][]>(_grid.Length, _grid.Length-1);
                    CreatePartOfGrid(0, _grid.Length-1, 0, _grid.Length, gridPartToCompare);
                    break;
                case Direction.Right:
                    gridPartToCompare = MyCollectionExtension.CreateJaggedArray<int[][]>(_grid.Length, _grid.Length-1);
                    CreatePartOfGrid(1, _grid.Length, 0, _grid.Length, gridPartToCompare);
                    break;
                default:
                    return _grid;
            }

            return gridPartToCompare;
        }

        private void CreatePartOfGrid(int xmin, int xmax, int ymin, int ymax, int[][] gridPartToCompare)
        {
            List<int> tempList = new List<int>();
            for (int i = ymin; i < ymax; i++)
            {
                for (int j = xmin; j < xmax; j++)
                {
                    tempList.Add(this._grid[i][j]);

                }
            }

            for (int i = 0; i < tempList.Count; i++)
            {
                int x = i % gridPartToCompare.Length;
                int y = i / gridPartToCompare.Length;
                gridPartToCompare[x][y] = tempList[i];


            }

        }
    }
}

