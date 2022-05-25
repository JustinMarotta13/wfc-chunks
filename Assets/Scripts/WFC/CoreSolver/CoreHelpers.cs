using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WaveFunctionCollapse
{
    public class CoreHelper
    {
        float totalFrequency = 0;
        float totalFrequencyLog = 0;
        PatternManager patternManager;
        public CoreHelper(PatternManager patternManager)
        {
            this.patternManager = patternManager;
            for (int i = 0; i < this.patternManager.GetNumberOfPatterns(); i++)
            {
                totalFrequency += this.patternManager.GetPatternFrequency(i);
            }
            totalFrequencyLog = Mathf.Log(totalFrequency, 2);
        }
        public int SelectSolutionPatternFromFrequency(List<int> possibleValues)
        {
            List<float> valueFrequenciesFractions = GetListOfWeightsFromIndices(possibleValues);
            float randomValue = Random.Range(0, valueFrequenciesFractions.Sum());
            float sum = 0;
            int index = 0;
            foreach (var item in valueFrequenciesFractions)
            {
                sum += item;
                if (randomValue <= sum)
                {
                    return index;
                }
                index++;
            }
            return index;
        }

        private List<float> GetListOfWeightsFromIndices(List<int> possibleValues)
        {
            var valueFrequencies = possibleValues.Aggregate(new List<float>(), (acc, val) =>
            {
                acc.Add(patternManager.GetPatternFrequency(val));
                return acc;
            },
                                acc => acc).ToList();
            return valueFrequencies;
        }

        public List<VectorPair> Create4DirectionNeighbors(Vector2Int cellCoordinates, Vector2Int previousCell)
        {
            List<VectorPair> list = new List<VectorPair>()
            {
                new VectorPair(cellCoordinates, cellCoordinates + new Vector2Int(1, 0), Direction.Right,previousCell),
                new VectorPair(cellCoordinates, cellCoordinates + new Vector2Int(-1, 0), Direction.Left, previousCell),
                new VectorPair(cellCoordinates, cellCoordinates + new Vector2Int(0, 1), Direction.Up, previousCell),
                new VectorPair(cellCoordinates, cellCoordinates + new Vector2Int(0, -1), Direction.Down, previousCell)
            };
            return list;
        }

        public List<VectorPair> Create4DirectionNeighbors(Vector2Int cellCoordinates)
        {
            return Create4DirectionNeighbors(cellCoordinates, cellCoordinates);
        }

        public float CalculateEntropy(Vector2Int position, OutputGrid outputGrid)
        {
            float sum = 0;
            foreach (var possibleIndex in outputGrid.GetPossibleValuesForPositon(position))
            {
                sum += patternManager.GetPatternFrequencyLog2(possibleIndex);
            }
            return totalFrequencyLog - (sum / totalFrequency);
        }

        public List<VectorPair> CheckIfNeighborsAreCollapsed(VectorPair pairToCheck, OutputGrid outputGrid)
        {
            return Create4DirectionNeighbors(pairToCheck.CellToPropagatePosition, pairToCheck.BaseCellPosition)
                .Where(x => outputGrid.IsPositionValid(x.CellToPropagatePosition) && outputGrid.IsCellCollapsed(x.CellToPropagatePosition) == false)
                .ToList();
        }

        public bool HasSolutionForCollisions(Vector2Int cellCoordinates, OutputGrid outputGrid)
        {
            foreach (VectorPair neighbor in Create4DirectionNeighbors(cellCoordinates))
            {
                if (outputGrid.IsPositionValid(neighbor.CellToPropagatePosition) == false) continue;

                HashSet<int> possibleIndices = new HashSet<int>();
                foreach (var patternIndexAtNeighbour in outputGrid.GetPossibleValuesForPositon(neighbor.CellToPropagatePosition))
                {
                    var possibleNeighborusForBase = patternManager.GetPossibleNeighborsForPatternInDirection(patternIndexAtNeighbour, neighbor.DiectionFromBase.GetOppositeDirectionTo());
                    possibleIndices.UnionWith(possibleNeighborusForBase);
                }
                if (possibleIndices.Contains(outputGrid.GetPossibleValuesForPositon(cellCoordinates).First()) == false)
                {
                    return true;
                }
            }
            return false;
        }
    }
}