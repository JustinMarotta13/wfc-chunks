using Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WaveFunctionCollapse
{
    public class OutputGrid
    {
        Dictionary<int, HashSet<int>> indexPossiblePatternsDict = new Dictionary<int, HashSet<int>>();

        public int width { get; }
        public int height { get; }
        private int maxNumberOfPatterns = 0;

        public OutputGrid(int width, int height, int numberOfPatterns)
        {
            this.width = width;
            this.height = height;
            this.maxNumberOfPatterns = numberOfPatterns;
            ResetAllPossibilities();
        }

        public void ResetAllPossibilities()
        {
            HashSet<int> allPossiblePatternsList = new HashSet<int>();
            allPossiblePatternsList.UnionWith(Enumerable.Range(0, this.maxNumberOfPatterns).ToList());
            indexPossiblePatternsDict.Clear();
            for (int i = 0; i < width * height; i++)
            {
                indexPossiblePatternsDict.Add(i, new HashSet<int>(allPossiblePatternsList));
            }
        }

        public bool ContainsCell(Vector2Int position)
        {
            int index = GetIndexFromCoords(position.x, position.y);
            return indexPossiblePatternsDict.ContainsKey(index);
        }

        public bool IsCellCollapsed(Vector2Int position)
        {
            return GetPossibleValuesForPositon(position).Count <= 1;
        }

        public bool IsGridSolved()
        {
            return !indexPossiblePatternsDict.Any(x => x.Value.Count > 1);
        }

        internal bool IsPositionValid(Vector2Int position)
        {
            return MyCollectionExtension.ValidateCoordinates(position.x, position.y, this.width, this.height);
        }

        public HashSet<int> GetPossibleValuesForPositon(Vector2Int position)
        {
            int index = GetIndexFromCoords(position.x, position.y);
            if (indexPossiblePatternsDict.ContainsKey(index))
                return indexPossiblePatternsDict[index];
            return new HashSet<int>();
        }

        public Vector2Int GetRandomCell()
        {
            int randomIndex = UnityEngine.Random.Range(0, indexPossiblePatternsDict.Count);
            return GetCoordsFromIndex(randomIndex);
        }

        public Vector2Int GetCoordsFromIndex(int index)
        {
            Vector2Int coordsVector = Vector2Int.zero;
            coordsVector.x = index / this.width;
            coordsVector.y = index % this.height;
            return coordsVector;
        }

        public void SetPatternOnPosition(int x, int y, int patternIndex)
        {
            int index = GetIndexFromCoords(x, y);
            indexPossiblePatternsDict[index] = new HashSet<int>() { patternIndex };
        }

        public int GetIndexFromCoords(int x, int y)
        {
            return x + width * y;
        }

        public void PrintResultsToConsole()
        {
            List<String> textToPrint = new List<string>();
            StringBuilder b;
            for (int i = 0; i < this.height; i++)
            {
                b = new StringBuilder();
                for (int j = 0; j < this.width; j++)
                {
                    var result = GetPossibleValuesForPositon(new Vector2Int(j, i));
                    if (result.Count == 1)
                        b.Append(result.First() + "   ");
                    else
                    {
                        //string newString = "C" + result.Count;
                        string newString = "";
                        foreach (var item in result)
                        {
                            newString += item + ",";
                        }
                        b.Append(newString + " ");
                    }
                }
                textToPrint.Add(b.ToString());
            }
            textToPrint.Reverse();
            foreach (var item in textToPrint)
            {
                //Debug.Log(item);
            }
            //Debug.Log("---");
        }

        public int[][] GetSolvedOutputGrid()
        {
            int[][] returnGrid = MyCollectionExtension.CreateJaggedArray<int[][]>(this.height, this.width);

            if (IsGridSolved() == false)
            {
                return MyCollectionExtension.CreateJaggedArray<int[][]>(0, 0);
            }
            for (int i = 0; i < returnGrid.Length; i++)
            {
                for (int j = 0; j < returnGrid[0].Length; j++)
                {
                    int index = GetIndexFromCoords(j, i);
                    returnGrid[i][j] = indexPossiblePatternsDict[index].First();
                }
            }
            return returnGrid;
        }
    }
}