using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Helpers;
using UnityEngine;

namespace WaveFunctionCollapse
{
    public class ValuesManager<T>
    {
        int[][] _grid;
        Dictionary<int, IValue<T>> valueIndexDictionary = new Dictionary<int, IValue<T>>();
        int index = 0;

        public ValuesManager(IValue<T>[][] gridOfValues)
        {
            CreateGridOfIndices(gridOfValues);
        }
        private void CreateGridOfIndices(IValue<T>[][] gridOfValues)
        {
            _grid = MyCollectionExtension.CreateJaggedArray<int[][]>(gridOfValues.Length, gridOfValues[0].Length);
            for (int i = 0; i < gridOfValues.Length; i++)
            {
                for (int j = 0; j < gridOfValues[i].Length; j++)
                {
                    SetIndexToGridPosition(gridOfValues, i, j);
                }
            }
        }

        private void SetIndexToGridPosition(IValue<T>[][] gridOfValues, int i, int j)
        {
            if (valueIndexDictionary.ContainsValue(gridOfValues[i][j]))
            {
                var key = valueIndexDictionary.FirstOrDefault(x => x.Value.Equals(gridOfValues[i][j]));

                _grid[i][j] = key.Key;
                return;
            }
            _grid[i][j] = index;
            valueIndexDictionary.Add(_grid[i][j], gridOfValues[i][j]);
            index++;
        }

        public int GetGridValue(int x, int y)
        {
            if (x >= _grid[0].Length || y >= _grid.Length)
            {
                throw new System.IndexOutOfRangeException("Grid of values doesn't contain x: " + x + " y: " + y);
            }
            return _grid[y][x];
        }

        public IValue<T> GetValueFromIndex(int index)
        {
            if (valueIndexDictionary.ContainsKey(index))
            {
                return valueIndexDictionary[index];
            }
            throw new System.Exception("ValueIndexDictionary doesn't contain index: " + index);
        }

        public Vector2 GetGridSize()
        {
            if (_grid == null)
            {
                return Vector2.zero;
            }
            return new Vector2(_grid[0].Length, _grid.Length);
        }

        public int[][] GetPatternValuesFromGridAt(int x, int y, int patternSize)
        {
            int[][] arrayToReturn = MyCollectionExtension.CreateJaggedArray<int[][]>(patternSize, patternSize);
            for (int row = 0; row < patternSize; row++)
            {
                for (int col = 0; col < patternSize; col++)
                {
                    arrayToReturn[row][col] = GetGridValueIncludingOffset(x + col, y + row);
                }
            }
            return arrayToReturn;
        }

        public int GetGridValueIncludingOffset(int x, int y)
        {
            int yMax = _grid.Length;
            int xMax = _grid[0].Length;
            if (x < 0 && y < 0)
            {
                return GetGridValue(xMax+x, yMax+y);
            }
            if (x < 0 && y >= yMax)
            {
                return GetGridValue(xMax + x, y - yMax);
            }
            if (x >= xMax && y < 0)
            {
                return GetGridValue(x-xMax, yMax + y);
            }
            if (x >= xMax && y >= yMax)
            {
                return GetGridValue(x - xMax, y - yMax);
            }
            if (x < 0)
            {
                return GetGridValue(xMax + x, y);
            }
            if (x >= xMax)
            {
                return GetGridValue(x - xMax, y);
            }
            if (y >= yMax)
            {
                return GetGridValue(x, y-yMax);
            }
            if (y < 0)
            {
                return GetGridValue(x, yMax + y);
            }
            return GetGridValue(x, y);
        }
    }
}