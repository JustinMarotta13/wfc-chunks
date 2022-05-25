using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace WaveFunctionCollapse
{
    public class CoreSolver
    {
        OutputGrid outputGrid;
        PatternManager patternManager;
        CoreHelper coreHelper;
        PropagationHelper propagationHelper;

        public CoreSolver(OutputGrid outputGrid, PatternManager patternManager)
        {
            this.outputGrid = outputGrid;
            this.patternManager = patternManager;
            coreHelper = new CoreHelper(this.patternManager);
            this.propagationHelper = new PropagationHelper(this.outputGrid, this.coreHelper);
        }

        public void Propagate()
        {
            //Debug.Log("Propagation result:");
            while (propagationHelper.PairsToPropagate.Count > 0)
            {
                var propagatePair = propagationHelper.PairsToPropagate.Dequeue();

                if (propagationHelper.ShouldPairBeProcessed(propagatePair)) ProcessCells(propagatePair);

                if (propagationHelper.HasConflicts() || outputGrid.IsGridSolved()) return;
            }
            if (propagationHelper.HasConflicts() && propagationHelper.PairsToPropagate.Count == 0 && propagationHelper.LowestEntropySet.Count == 0)
            {
                propagationHelper.SetConflictFlag();
            }
            //outputGrid.PrintResultsToConsol();
        }
        private void ProcessCells(VectorPair propagatePair)
        {
            if (outputGrid.IsCellCollapsed(propagatePair.CellToPropagatePosition))
            {
                propagationHelper.EnqueueUncollapsedNeighbors(propagatePair);
                return;
            }
            PropagateNeighbors(propagatePair);
        }
        private void PropagateNeighbors(VectorPair propagatePair)
        {
            HashSet<int> possibleValuesAtNeighbor = outputGrid.GetPossibleValuesForPositon(propagatePair.CellToPropagatePosition);
            int startCount = possibleValuesAtNeighbor.Count();

            RemoveImpossibleNeighbors(propagatePair, possibleValuesAtNeighbor);

            int newPossiblePatternCount = possibleValuesAtNeighbor.Count;
            propagationHelper.AnalyzePropagatonResults(propagatePair, startCount, newPossiblePatternCount);
        }
        private void RemoveImpossibleNeighbors(VectorPair propagatePair, HashSet<int> possibleValuesAtNeighbor)
        {
            HashSet<int> possibleIndices = new HashSet<int>();

            foreach (var patternIndexAtBase in outputGrid.GetPossibleValuesForPositon(propagatePair.BaseCellPosition))
            {
                HashSet<int> possibleNeighborusForBase = patternManager.GetPossibleNeighborsForPatternInDirection(patternIndexAtBase, propagatePair.DiectionFromBase);

                possibleIndices.UnionWith(possibleNeighborusForBase);
            }
            possibleValuesAtNeighbor.IntersectWith(possibleIndices);
        }

        public Vector2Int GetLowestEntropyCell()
        {
            if (propagationHelper.LowestEntropySet.Count <= 0) return outputGrid.GetRandomCell();

            LowEntropyCell lowestEntropyElement = propagationHelper.LowestEntropySet.First();
            Vector2Int returnVEctor = lowestEntropyElement.Position;
            propagationHelper.LowestEntropySet.Remove(lowestEntropyElement);
            return returnVEctor;
        }

        public void CollapseCell(Vector2Int cellCoordinates)
        {
            List<int> possibleValues = outputGrid.GetPossibleValuesForPositon(cellCoordinates).ToList();

            if (possibleValues.Count == 0 || possibleValues.Count == 1) return;

            int index = coreHelper.SelectSolutionPatternFromFrequency(possibleValues);
            outputGrid.SetPatternOnPosition(cellCoordinates.x, cellCoordinates.y, possibleValues[index]);

            if (coreHelper.HasSolutionForCollisions(cellCoordinates, outputGrid) == false)
            {
                propagationHelper.AddNewPairsToPropagateQueue(cellCoordinates, cellCoordinates);
                return;
            }
            propagationHelper.SetConflictFlag();
        }

        public bool CheckIfSolved()
        {
            return outputGrid.IsGridSolved();
        }

        public bool CheckForConflics()
        {
            return propagationHelper.HasConflicts();
        }
    }
}