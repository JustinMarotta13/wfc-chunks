using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WaveFunctionCollapse
{
    public class PropagationHelper
    {
        OutputGrid outputGrid;
        CoreHelper coreHelper;
        bool cellWithNoSolutionPresent = false;

        SortedSet<LowEntropyCell> lowestEntropySet = new SortedSet<LowEntropyCell>();
        public SortedSet<LowEntropyCell> LowestEntropySet { get => lowestEntropySet;}

        Queue<VectorPair> pairsToPropagate = new Queue<VectorPair>();
        public Queue<VectorPair> PairsToPropagate { get => pairsToPropagate; }

        public PropagationHelper(OutputGrid outputGrid, CoreHelper coreHelper)
        {
            this.outputGrid = outputGrid;
            this.coreHelper = coreHelper;
        }

        public bool ShouldPairBeProcessed(VectorPair propagatePair)
        {
            return outputGrid.IsPositionValid(propagatePair.CellToPropagatePosition) && propagatePair.AreWeCheckingPreviousCellAgain() == false;
        }

        public void AnalyzePropagatonResults(VectorPair propagatePair, int startCount, int newPossiblePatternCount)
        {
            if (newPossiblePatternCount > 1 && startCount > newPossiblePatternCount)
            {
                AddNewPairsToPropagateQueue(propagatePair.CellToPropagatePosition, propagatePair.BaseCellPosition);
                AddToLowestEntropySet(propagatePair.CellToPropagatePosition, coreHelper);
            }
            if (newPossiblePatternCount == 0)
            {
                cellWithNoSolutionPresent = true;
            }
            if (newPossiblePatternCount == 1)
            {
                cellWithNoSolutionPresent = coreHelper.HasSolutionForCollisions(propagatePair.CellToPropagatePosition, outputGrid);
            }
        }

        public void AddToLowestEntropySet(Vector2Int cellToPropagatePosition, CoreHelper coreHelper)
        {
            LowEntropyCell elementIdLowEntropySet = lowestEntropySet.Where(cell => cell.Position == cellToPropagatePosition).FirstOrDefault();

            if (elementIdLowEntropySet == null && outputGrid.IsCellCollapsed(cellToPropagatePosition) == false)
            {
                float entropy = coreHelper.CalculateEntropy(cellToPropagatePosition, outputGrid);
                lowestEntropySet.Add(new LowEntropyCell(cellToPropagatePosition, entropy));
                return;
            }
            lowestEntropySet.Remove(elementIdLowEntropySet);
            elementIdLowEntropySet.Entropy = coreHelper.CalculateEntropy(cellToPropagatePosition, outputGrid);
            lowestEntropySet.Add(elementIdLowEntropySet);
        }

        public void RemoveCellFromEntropySet(Vector2Int cellToPropagatePosition)
        {
            LowEntropyCell elementIdLowEntropySet = lowestEntropySet.Where(cell => cell.Position == cellToPropagatePosition).FirstOrDefault();
            lowestEntropySet.Remove(elementIdLowEntropySet);
        }

        public void AddNewPairsToPropagateQueue(Vector2Int cellCoordinates, Vector2Int previousCell)
        {
            List<VectorPair> list = coreHelper.Create4DirectionNeighbors(cellCoordinates, previousCell);

            foreach (var item in list)
            {
                pairsToPropagate.Enqueue(item);
            }
        }

        public void EnqueueUncollapsedNeighbors(VectorPair propagatePair)
        {
            var uncollapsedNeighbors = coreHelper.CheckIfNeighborsAreCollapsed(propagatePair, outputGrid);
            foreach (VectorPair uncollapsed in uncollapsedNeighbors)
            {
                pairsToPropagate.Enqueue(uncollapsed);
            }
        }

        public bool HasConflicts()
        {
            return cellWithNoSolutionPresent;
        }

        public void SetConflictFlag()
        {
            cellWithNoSolutionPresent=true;
        }
    }
}