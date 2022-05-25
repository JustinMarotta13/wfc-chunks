using Helpers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaveFunctionCollapse
{
    public class PatternDataResults
    {
        public int[][] PatternIndicesGrid { get; set; } = new int[0][];
        public Dictionary<int, PatternData> PatternIndexDictionary { get; set; }

        public PatternDataResults(int[][] patternIndicesGrid, Dictionary<int, PatternData> patternIndexDictionary)
        {
            PatternIndexDictionary = patternIndexDictionary;
            PatternIndicesGrid = patternIndicesGrid;
        }

        public int GetGridLengthInX()
        {
            return PatternIndicesGrid[0].Length;
        }
        public int GetGridLengthInY()
        {
            return PatternIndicesGrid.Length;
        }
        public int GetIndexAt(int x, int y)
        {
            //jagged array is NOT a grid. We access first columnt and second rows
            return PatternIndicesGrid[y][x];
        }

        public int GetNeighbourInDirection(int x, int y, Direction dir)
        {
            //y and x swapped because int[columns][rows] as 
            if (PatternIndicesGrid.CheckJaggedArray2IfIndexIsValid(x, y) == false)
            {
                return -1;
            }
            switch (dir)
            {
                case Direction.Up:
                    if (PatternIndicesGrid.CheckJaggedArray2IfIndexIsValid(x, y + 1))
                    {
                        return GetIndexAt(x, y + 1);
                    }
                    return -1;
                case Direction.Down:
                    if (PatternIndicesGrid.CheckJaggedArray2IfIndexIsValid(x, y - 1))
                    {
                        return GetIndexAt(x, y - 1);
                    }
                    return -1;
                case Direction.Left:
                    if (PatternIndicesGrid.CheckJaggedArray2IfIndexIsValid(x - 1, y))
                    {
                        return GetIndexAt(x - 1, y);
                    }
                    return -1;
                case Direction.Right:
                    if (PatternIndicesGrid.CheckJaggedArray2IfIndexIsValid(x + 1, y))
                    {
                        return GetIndexAt(x + 1, y);
                    }
                    return -1;
                default:
                    return -1;
            }
        }
    }
}
