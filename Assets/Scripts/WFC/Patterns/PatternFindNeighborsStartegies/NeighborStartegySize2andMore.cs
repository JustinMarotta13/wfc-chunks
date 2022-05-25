using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaveFunctionCollapse
{
    public class NeighborStartegySize2andMore : IFindNeighborStrategy
    {
        public Dictionary<int, PatternNeighbors> FindNeighbors(PatternDataResults patterndataResults)
        {
            Dictionary<int, PatternNeighbors> patternDictionary = new Dictionary<int, PatternNeighbors>();
            foreach (var patternDataToCheck in patterndataResults.PatternIndexDictionary)
            {
                foreach (var possibleNeighborPatternData in patterndataResults.PatternIndexDictionary)
                {
                    FindNeighborsInAllDirections(patternDictionary, patternDataToCheck, possibleNeighborPatternData);
                }
            }
            return patternDictionary;
        }

        private static void FindNeighborsInAllDirections(Dictionary<int, PatternNeighbors> patternDictionary, KeyValuePair<int, PatternData> patternDataToCheck, KeyValuePair<int, PatternData> possibleNeighborPatternData)
        {
            foreach (Direction dir in Enum.GetValues(typeof(Direction)))
            {

                if (patternDataToCheck.Value.CompareGrid(dir, possibleNeighborPatternData.Value))
                {
                    if (patternDictionary.ContainsKey(patternDataToCheck.Key) == false)
                    {
                        patternDictionary.Add(patternDataToCheck.Key, new PatternNeighbors());
                    }
                    patternDictionary[patternDataToCheck.Key].AddPatternToDirection(dir, possibleNeighborPatternData.Key);
                }
            }
        }
    }
}

