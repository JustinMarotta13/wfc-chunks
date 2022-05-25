using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helpers;

namespace WaveFunctionCollapse
{
    public class PatternManager
    {
        Dictionary<int, PatternData> patternDataIndexDictionary;
        Dictionary<int, PatternNeighbors> patternPossibleNeighborsDictionary;
        int _patternSize = -1;
        IFindNeighborStrategy strategy;
        
        public PatternManager(int patternSize)
        {
            _patternSize = patternSize;
        }

        public void ProcessGrid<T>(ValuesManager<T> valuesManager, bool equalWeights, string strategyName = null)
        {
            NeighborStrategyFactory strategyFactory = new NeighborStrategyFactory();
            strategy = strategyFactory.CreateInstance(strategyName == null? _patternSize+"" : strategyName);
            CreatePatterns(valuesManager, strategy, equalWeights);
        }

        private void CreatePatterns<T>(ValuesManager<T> valuesManager, IFindNeighborStrategy startegy, bool equalWeights)
        {
            PatternDataResults patternFinderResults = PatternFinder.GetPatternDataFromGrid(valuesManager, _patternSize, equalWeights);
            patternDataIndexDictionary = patternFinderResults.PatternIndexDictionary;
            GetPatternNeighbors(patternFinderResults, startegy);
        }

        private void GetPatternNeighbors(PatternDataResults patternFinderResults, IFindNeighborStrategy strategy)
        {
            patternPossibleNeighborsDictionary = PatternFinder.FindPossibleNeighborsForAllPatterns(strategy, patternFinderResults);
        }

        public int[][] ConvertPatternToValues<T>(int[][] patternOutputGrid)
        {
            int patternOutputWidth = patternOutputGrid[0].Length;
            int patternOutputHeight = patternOutputGrid.Length;
            int valueGridWidth = patternOutputWidth + (_patternSize - 1);
            int valueGridHeight = patternOutputHeight + (_patternSize - 1);
            int[][] valuesGrid = MyCollectionExtension.CreateJaggedArray<int[][]>(valueGridHeight, valueGridWidth);

            for (int row = 0; row < patternOutputHeight; row++)
            {
                for (int col = 0; col < patternOutputWidth; col++)
                {
                    Pattern pattern = GetPatternDataFromIndex(patternOutputGrid[row][col]).Pattern;
                    GetPatternValues(patternOutputWidth, patternOutputHeight, valuesGrid, row, col, pattern);
                }
            }
            return valuesGrid;
        }

        private void GetPatternValues(int patternOutputWidth, int patternOutputHeight, int[][] valuesGrid, int row, int col, Pattern pattern)
        {
            if (row == patternOutputHeight - 1 && col == patternOutputWidth - 1)
            {
                for (int row_1 = 0; row_1 < _patternSize; row_1++)
                {
                    for (int col_1 = 0; col_1 < _patternSize; col_1++)
                    {
                        valuesGrid[row + row_1][col + col_1] = pattern.GetGridValue(col_1, row_1);
                    }
                }
                return;
            }
            if (row == patternOutputHeight - 1)
            {
                for (int row_1 = 0; row_1 < _patternSize; row_1++)
                {
                    valuesGrid[row + row_1][col] = pattern.GetGridValue(0, row_1);
                }
                return;
            }
            if (col == patternOutputWidth - 1)
            {
                for (int col_1 = 0; col_1 < _patternSize; col_1++)
                {
                    valuesGrid[row][col + col_1] = pattern.GetGridValue(col_1, 0);
                }
                return;
            }
            valuesGrid[row][col] = pattern.GetGridValue(0, 0);
        }

        public PatternData GetPatternDataFromIndex(int index)
        {
            return patternDataIndexDictionary[index];
        }

        public HashSet<int> GetPossibleNeighborsForPatternInDirection(int patternIndex, Direction dir)
        {
            return patternPossibleNeighborsDictionary[patternIndex].GetNeighborsInDirection(dir);
        }

        public float GetPatternFrequency(int index)
        {
            return GetPatternDataFromIndex(index).FrequencyRelative;
        }

        public float GetPatternFrequencyLog2(int index)
        {
            return GetPatternDataFromIndex(index).Frequencylog2;
        }

        public int GetNumberOfPatterns()
        {
            return patternDataIndexDictionary.Count;
        }
    }
}