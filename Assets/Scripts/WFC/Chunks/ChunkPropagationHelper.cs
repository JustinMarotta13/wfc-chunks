using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace WaveFunctionCollapse
{
    public class ChunkPropagationHelper
    {
        OutputGrid outputGrid;
        ChunkCoreHelper coreHelper;
        bool cellWithNoSolutionPresent = false;

        SortedSet<LowEntropyCell> lowestEntropySet = new SortedSet<LowEntropyCell>();
        public SortedSet<LowEntropyCell> LowestEntropySet { get => lowestEntropySet;}

        Queue<VectorPair> pairsToPropagate = new Queue<VectorPair>();
        public Queue<VectorPair> PairsToPropagate { get => pairsToPropagate; }

        public ChunkPropagationHelper(OutputGrid outputGrid, ChunkCoreHelper coreHelper)
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

        public void AddToLowestEntropySet(Vector2Int cellToPropagatePosition, ChunkCoreHelper coreHelper)
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

            foreach (VectorPair item in list)
            {
                pairsToPropagate.Enqueue(item);
            }
        }
        public void AddNewPairsToPropagateQueueLeft(Vector2Int cellCoordinates)
        {
            pairsToPropagate.Enqueue(new VectorPair(cellCoordinates, cellCoordinates + new Vector2Int(1, 0), Direction.Right, cellCoordinates));
        }
        public void AddNewPairsToPropagateQueueRight(Vector2Int cellCoordinates)
        {
            pairsToPropagate.Enqueue(new VectorPair(cellCoordinates, cellCoordinates + new Vector2Int(-1, 0), Direction.Left, cellCoordinates));
        }
        public void AddNewPairsToPropagateQueueTop(Vector2Int cellCoordinates)
        {
            pairsToPropagate.Enqueue(new VectorPair(cellCoordinates, cellCoordinates + new Vector2Int(0, -1), Direction.Down, cellCoordinates));
        }
        public void AddNewPairsToPropagateQueueBottom(Vector2Int cellCoordinates)
        {
            pairsToPropagate.Enqueue(new VectorPair(cellCoordinates, cellCoordinates + new Vector2Int(0, 1), Direction.Up, cellCoordinates));
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