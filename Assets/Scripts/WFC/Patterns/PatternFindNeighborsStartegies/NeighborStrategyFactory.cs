using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace WaveFunctionCollapse
{
    public class NeighborStrategyFactory
    {
        Dictionary<string, Type> strategies;
        public NeighborStrategyFactory()
        {
            LoadTypesIFindNeighboutStrategy();
        }

        private void LoadTypesIFindNeighboutStrategy()
        {
            strategies = new Dictionary<string, Type>();
            Type[] typesInThisAssembly = Assembly.GetExecutingAssembly().GetTypes();

            foreach (var type in typesInThisAssembly)
            {
                if (type.GetInterface(typeof(IFindNeighborStrategy).ToString()) != null)
                {
                    strategies.Add(type.Name.ToLower(), type);
                }
            }
        }

        public IFindNeighborStrategy CreateInstance(string strategyName)
        {
            Type t = GetTypeToCreate(strategyName);
            if (t == null)
            {
                t = GetTypeToCreate("more");
            }
            return Activator.CreateInstance(t) as IFindNeighborStrategy;

        }

        private Type GetTypeToCreate(string patternSizeName)
        {
            foreach (var possibleStrategy in strategies)
            {
                if (possibleStrategy.Key.Contains(patternSizeName))
                {
                    return strategies[possibleStrategy.Key];
                }
            }
            return null;
        }
    }

}
