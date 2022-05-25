using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaveFunctionCollapse
{
    public interface IFindNeighborStrategy
    {
        Dictionary<int, PatternNeighbors> FindNeighbors(PatternDataResults patterndataResults);
    }
}
