using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WaveFunctionCollapse
{
    public class ChunkCoreSolver
    {
        OutputGrid outputGrid;
        PatternManager patternManager;
        ChunkCoreHelper coreHelper;
        ChunkPropagationHelper propagationHelper;

        public ChunkCoreSolver(OutputGrid outputGrid, PatternManager patternManager)
        {
            this.outputGrid = outputGrid;
            this.patternManager = patternManager;
            coreHelper = new ChunkCoreHelper(this.patternManager);
            this.propagationHelper = new ChunkPropagationHelper(this.outputGrid, this.coreHelper);
        }

        public void Propagate()
        {
            //Debug.Log("Propagation result:");
            while (propagationHelper.PairsToPropagate.Count > 0)
            {
                VectorPair propagatePair = propagationHelper.PairsToPropagate.Dequeue();

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
            Vector2Int returnVector = lowestEntropyElement.Position;
            propagationHelper.LowestEntropySet.Remove(lowestEntropyElement);
            return returnVector;
        }

        public void CollapseCell(Vector2Int cellCoordinates, Chunk chunk)
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

        public void SetBorderingCellsToOutputGrid(ChunkMap chunkMap, Chunk chunk, ChunkCoreSolver solver)
        {
            ChunkCoordinate chunkLocation = chunk.chunkLocation;

            Chunk left = chunkMap.GetLeftNeighbor(chunkLocation);
            if (left != null)
            {
                left.hasRightNeighbor = true;
                for (int i = 0; i < outputGrid.height; i++)
                {
                    Vector2Int cellCoordinates = new Vector2Int(0, i);
                    Debug.Log("Left neighbor found is: " + "(" + left.chunkLocation.x + "," + left.chunkLocation.y + ")"
                                + " for chunk: " + "(" + chunkLocation.x + "," + chunkLocation.y + ")" + " and has index " + left.rightBorder[i] + " at row " + i);
                    outputGrid.SetPatternOnPosition(cellCoordinates.x, cellCoordinates.y, left.rightBorder[i]);
                    propagationHelper.RemoveCellFromEntropySet(cellCoordinates);
                    if (coreHelper.HasSolutionForCollisions(cellCoordinates, outputGrid) == false)
                    {
                        propagationHelper.AddNewPairsToPropagateQueueLeft(cellCoordinates);
                        return;
                    }
                    else propagationHelper.SetConflictFlag();
                    solver.Propagate();
                }
            }

            Chunk right = chunkMap.GetRightNeighbor(chunkLocation);
            if (right != null)
            {
                right.hasLeftNeighbor = true;
                for (int i = 0; i < outputGrid.height; i++)
                {
                    Vector2Int cellCoordinates = new Vector2Int(outputGrid.width - 1, i);
                    outputGrid.SetPatternOnPosition(cellCoordinates.x, cellCoordinates.y, right.leftBorder[i]);
                    propagationHelper.RemoveCellFromEntropySet(cellCoordinates);
                    if (coreHelper.HasSolutionForCollisions(cellCoordinates, outputGrid) == false)
                    {
                        propagationHelper.AddNewPairsToPropagateQueueRight(cellCoordinates);
                        return;
                    }
                    else propagationHelper.SetConflictFlag();
                    solver.Propagate();
                }
            }

            Chunk top = chunkMap.GetTopNeighbor(chunkLocation);
            if (top != null)
            {
                top.hasBottomNeighbor = true;
                for (int i = 0; i < outputGrid.width; i++)
                {
                    Vector2Int cellCoordinates = new Vector2Int(i, outputGrid.height - 1);
                    outputGrid.SetPatternOnPosition(cellCoordinates.x, cellCoordinates.y, top.bottomBorder[i]);
                    propagationHelper.RemoveCellFromEntropySet(cellCoordinates);
                    if (coreHelper.HasSolutionForCollisions(cellCoordinates, outputGrid) == false)
                    {
                        propagationHelper.AddNewPairsToPropagateQueueTop(cellCoordinates);
                        return;
                    }
                    else propagationHelper.SetConflictFlag();
                    solver.Propagate();
                }
            }

            Chunk bottom = chunkMap.GetBottomNeighbor(chunkLocation);
            if (bottom != null)
            {
                bottom.hasTopNeighbor = true;
                for (int i = 0; i < outputGrid.width; i++)
                {
                    Vector2Int cellCoordinates = new Vector2Int(i, 0);
                    outputGrid.SetPatternOnPosition(cellCoordinates.x, cellCoordinates.y, bottom.topBorder[i]);
                    propagationHelper.RemoveCellFromEntropySet(cellCoordinates);
                    if (coreHelper.HasSolutionForCollisions(cellCoordinates, outputGrid) == false)
                    {
                        propagationHelper.AddNewPairsToPropagateQueueBottom(cellCoordinates);
                        return;
                    }
                    else propagationHelper.SetConflictFlag();
                    solver.Propagate();
                }
            }
        }

        public bool IsSolved()
        {
            return outputGrid.IsGridSolved();
        }

        public bool HasConflicts()
        {
            return propagationHelper.HasConflicts();
        }
    }
}