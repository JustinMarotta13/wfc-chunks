using Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace WaveFunctionCollapse
{
    public class ChunkCore
    {
        OutputGrid outputGrid;

        PatternManager patternManager;

        private int maxIterations = 0;

        /// <summary> Initializes a new instance of the <see cref="T:WaveFunctionCollapse.WFCCore"/>
        /// class that creates a new <see cref="T:WaveFunctionCollapse.Output"
        /// </summary>
        public ChunkCore(int outputWidth, int outputHeight, PatternManager patternManager, int maxIterations)
        {
            this.outputGrid = new OutputGrid(outputWidth, outputHeight, patternManager.GetNumberOfPatterns());
            this.patternManager = patternManager;
            this.maxIterations = maxIterations;
        }

        public int[][] CreateOutputGrid(ChunkMap chunkMap, Chunk chunk)
        {
            ChunkCoreSolver solver = new ChunkCoreSolver(this.outputGrid, this.patternManager);

            int iteration = 0;
            while (iteration < this.maxIterations)
            {
                solver.SetBorderingCellsToOutputGrid(chunkMap, chunk, solver);

                int innerIteration = 100;
                while (!solver.HasConflicts() && !solver.IsSolved())
                {
                    Vector2Int position = solver.GetLowestEntropyCell();
                    solver.CollapseCell(position, chunk);
                    solver.Propagate();
                    innerIteration--;
                    if (innerIteration <= 0)
                    {
                        Debug.Log("Propagation is taking too long");
                        return new int[0][];
                    }
                }
                
                if (solver.HasConflicts())
                {
                    //Debug.Log("\nConflict occured. Iteration: " + iteration);
                    iteration++;
                    outputGrid.ResetAllPossibilities();
                    solver = new ChunkCoreSolver(this.outputGrid, this.patternManager);
                    continue;
                }

                StorePatternIndicesInChunk(chunk);

                Debug.Log("Solved in " + iteration + " iteration(s)");
                outputGrid.PrintResultsToConsole();
                break;
            }
            if (iteration >= this.maxIterations)
            {
                Debug.Log("Couldn't solve in " + this.maxIterations + " iterations");
            }
            return outputGrid.GetSolvedOutputGrid();
        }

        private void StorePatternIndicesInChunk(Chunk chunk)
        {
            for (int j = 0; j < outputGrid.height; j++)
            {
                for (int i = 0; i < outputGrid.width; i++)
                {
                    int patternIndex = outputGrid.GetPossibleValuesForPositon(new Vector2Int(i, j)).First();
                    if (i == 0)
                    {
                        chunk.leftBorder[j] = patternIndex;
                        Debug.Log("Left border at " + j + ": " + patternIndex);
                    }
                    if (i == outputGrid.width - 1)
                    {
                        chunk.rightBorder[j] = patternIndex;
                        Debug.Log("Right border at " + j + ": " + patternIndex);
                    }
                    if (j == outputGrid.height - 1)
                    {
                        chunk.topBorder[i] = patternIndex;
                        Debug.Log("Top border at " + i + ": " + patternIndex);
                    }
                    if (j == 0)
                    {
                        chunk.bottomBorder[i] = patternIndex;
                        Debug.Log("Bottom border at " + i + ": " + patternIndex);
                    }
                }
            }
        }
    }
}