using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaveFunctionCollapse
{
    public class NeighborStartegySize1Default : IFindNeighborStrategy
    {
        public Dictionary<int, PatternNeighbors> FindNeighbors(PatternDataResults patterndataResults)
        {
            Dictionary<int, PatternNeighbors> result = new Dictionary<int, PatternNeighbors>();
            FindNeighborsForEachPattern(patterndataResults, result);
            return result;
        }

        private static void FindNeighborsForEachPattern(PatternDataResults patterndataResults, Dictionary<int, PatternNeighbors> result)
        {
            for (int y = 0; y < patterndataResults.GetGridLengthInY(); y++)
            {
                for (int x = 0; x < patterndataResults.GetGridLengthInX(); x++)
                {
                    PatternNeighbors neighbors = PatternFinder.CheckNeighboursInEachDirection(x, y, patterndataResults);
                    PatternFinder.AddNeighboursToDictionary(result, patterndataResults.GetIndexAt(x, y), neighbors);
                }
            }
        }
    }
}

