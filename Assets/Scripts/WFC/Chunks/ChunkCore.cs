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

                int innerIteration = 500;
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
            Conditions conditions = new Conditions(0, 0, outputGrid.height, outputGrid.width);
            conditions.SetConditions(chunk);
            int i = conditions.i;
            int j = conditions.j;
            int rows = conditions.rows;
            int columns = conditions.columns;

            for (int y = j; y < rows; y++)
            {
                for (int x = i; x < columns; x++)
                {
                    int patternIndex = outputGrid.GetPossibleValuesForPositon(new Vector2Int(x, y)).First();

                    int leftOffset = chunk.left != null ? 1 : 0;
                    int bottomOffset = chunk.below != null ? 1 : 0;

                    int virtualX = x - leftOffset;
                    int virtualY = y - bottomOffset;

                    Debug.Log("Iteration (" + virtualX + "," + virtualY + ") has pattern index " + patternIndex);

                    if (virtualX == 0)
                    {
                        chunk.leftBorder[virtualY] = patternIndex;
                        Debug.Log("Left border at " + virtualY + ": " + patternIndex);
                    }
                    if (virtualX + leftOffset == columns - 1)
                    {
                        chunk.rightBorder[virtualY] = patternIndex;
                        Debug.Log("Right border at " + virtualY + ": " + patternIndex);
                    }
                    if (virtualY + bottomOffset == rows - 1)
                    {
                        chunk.topBorder[virtualX] = patternIndex;
                        Debug.Log("Top border at " + virtualX + ": " + patternIndex);
                    }
                    if (virtualY == 0)
                    {
                        chunk.bottomBorder[virtualX] = patternIndex;
                        Debug.Log("Bottom border at " + virtualX + ": " + patternIndex);
                    }
                }
            }
        }

        // private void SetBorderIndex(Chunk chunk, int x, int y, int patternIndex)
        // {
        //     if (x == 0)
        //     {
        //         chunk.leftBorder[y] = patternIndex;
        //     }
        //     if (x == chunk.width - 1)
        //     {
        //         chunk.rightBorder[y] = patternIndex;
        //     }
        //     if (y == chunk.height - 1)
        //     {
        //         chunk.topBorder[x] = patternIndex;
        //     }
        //     if (y == 0)
        //     {
        //         chunk.bottomBorder[x] = patternIndex;
        //     }
        // }

        struct Conditions
        {
            public int i;
            public int j;
            public int rows;
            public int columns;

            public Conditions(int i, int j, int rows, int columns)
            {
                this.i = i;
                this.j = j;
                this.rows = rows;
                this.columns = columns;
            }

            public void SetConditions(Chunk chunk)
            {
                if (chunk.left != null) {Debug.Log("Storing and found left neighbor true"); i++;}
                if (chunk.right != null) columns--;
                if (chunk.above != null) rows--;
                if (chunk.below != null) j++;
            }
        }
    }
}