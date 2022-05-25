using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaveFunctionCollapse
{
    public class PatternNeighbors
    {
        public Dictionary<Direction, HashSet<int>> directionNeighborPatternDictionary = new Dictionary<Direction, HashSet<int>>();

        public void AddPatternToDirection(Direction dir, int patternIndex)
        {
            if (directionNeighborPatternDictionary.ContainsKey(dir))
            {
                directionNeighborPatternDictionary[dir].Add(patternIndex);
                return;
            }
            directionNeighborPatternDictionary.Add(dir, new HashSet<int>() { patternIndex });
        }

        public HashSet<int> GetNeighborsInDirection(Direction dir)
        {
            if (directionNeighborPatternDictionary.ContainsKey(dir))
            {
                return directionNeighborPatternDictionary[dir];
            }
            return new HashSet<int>();
        }

        public void AddNeighbors(PatternNeighbors neighbors)
        {
            foreach (KeyValuePair<Direction, HashSet<int>> item in neighbors.directionNeighborPatternDictionary)
            {
                if (directionNeighborPatternDictionary.ContainsKey(item.Key) == false)
                {
                    directionNeighborPatternDictionary.Add(item.Key, new HashSet<int>());
                }
                directionNeighborPatternDictionary[item.Key].UnionWith(item.Value);
            }
        }
    }
}