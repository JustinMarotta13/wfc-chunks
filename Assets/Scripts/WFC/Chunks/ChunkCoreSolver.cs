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
                int yOffset = chunk.below != null ? 1 : 0;
                for (int i = yOffset; i < outputGrid.height; i++)
                {
                    Vector2Int cellCoordinates = new Vector2Int(0, i);
                    SetPatternAndPropogate(cellCoordinates, solver, left.rightBorder[i]);
                }
            }

            Chunk right = chunkMap.GetRightNeighbor(chunkLocation);
            if (right != null)
            {
                right.hasLeftNeighbor = true;
                int yOffset = chunk.below != null ? 1 : 0;
                for (int i = yOffset; i < outputGrid.height; i++)
                {
                    Vector2Int cellCoordinates = new Vector2Int(outputGrid.width - 1, i);
                    SetPatternAndPropogate(cellCoordinates, solver, right.leftBorder[i]);
                }
            }

            Chunk top = chunkMap.GetTopNeighbor(chunkLocation);
            if (top != null)
            {
                top.hasBottomNeighbor = true;
                int xOffset = chunk.left != null ? 1 : 0;
                for (int i = xOffset; i < outputGrid.width; i++)
                {
                    Vector2Int cellCoordinates = new Vector2Int(i, outputGrid.height - 1);
                    SetPatternAndPropogate(cellCoordinates, solver, top.bottomBorder[i]);
                }
            }

            Chunk bottom = chunkMap.GetBottomNeighbor(chunkLocation);
            if (bottom != null)
            {
                bottom.hasTopNeighbor = true;
                int xOffset = chunk.left != null ? 1 : 0;
                for (int i = xOffset; i < outputGrid.width; i++)
                {
                    Vector2Int cellCoordinates = new Vector2Int(i, 0);
                    SetPatternAndPropogate(cellCoordinates, solver, bottom.topBorder[i]);
                }
            }
        }

        private void SetPatternAndPropogate(Vector2Int cellCoordinates, ChunkCoreSolver solver, int patternIndex)
        {
            if (solver.outputGrid.IsCellCollapsed(cellCoordinates)) return;
            outputGrid.SetPatternOnPosition(cellCoordinates.x, cellCoordinates.y, patternIndex);
            propagationHelper.RemoveCellFromEntropySet(cellCoordinates);
            if (coreHelper.HasSolutionForCollisions(cellCoordinates, outputGrid) == false)
            {
                propagationHelper.AddNewPairsToPropagateQueue(cellCoordinates, cellCoordinates);
                return;
            }
            else propagationHelper.SetConflictFlag();
            Propagate();
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