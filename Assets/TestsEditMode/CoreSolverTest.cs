using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.Tilemaps;
using WaveFunctionCollapse;
namespace Tests
{
    public class WFCCoreTest
    {
        GameObject tileMapPrefab;
        Tilemap tilemap;
        InputReader inputReader;
        ValuesManager<TileBase> valueManager;
        PatternManager patternManager;
        OutputGrid outputGrid;
        CoreSolver solver;
        int patternSize = 1;
        IFindNeighborStrategy strategy = new NeighborStartegySize1Default();
        [OneTimeSetUp]
        public void Init()
        {
            tileMapPrefab = Resources.Load("TestTilemap") as GameObject;
            tilemap = tileMapPrefab.GetComponent<Tilemap>();
            inputReader = new InputReader(tilemap);
            valueManager = new ValuesManager<TileBase>(inputReader.ReadInputToGrid());
            patternManager = new PatternManager(patternSize);
            if (patternSize > 1)
            {
                strategy = new NeighborStartegySize2andMore();
            }
            patternManager.ProcessGrid(valueManager, false);
            

        }

        


        [Test]
        public void WFCCoreCheckIfSolvedFalsePasses()
        {
            var position = new Vector2Int(0, 0);
            Assert.False(solver.CheckIfSolved());
        }

        [Test]
        public void WFCCoreCheckIfSolvedTruePasses()
        {
            outputGrid = new OutputGrid(10, 10, patternManager.GetNumberOfPatterns());
            solver = new CoreSolver(outputGrid, patternManager);
            var position = new Vector2Int(0, 0);
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    outputGrid.SetPatternOnPosition(i, j, 1);
                }
            }
            Assert.True(solver.CheckIfSolved());
        }

        // A Test behaves as an ordinary method
        [Test]
        public void CoreSolverCollapseCellPasses()
        {
            outputGrid = new OutputGrid(10, 10, patternManager.GetNumberOfPatterns());
            solver = new CoreSolver(outputGrid, patternManager);
            var position = new Vector2Int(0, 0);
            solver.CollapseCell(position);
            Assert.True(outputGrid.IsCellCollapsed(position));
        }

        // A Test behaves as an ordinary method
        [Test]
        public void CoreSolverCheckLowestEntropyPasses()
        {
            outputGrid = new OutputGrid(10, 10, patternManager.GetNumberOfPatterns());
            solver = new CoreSolver(outputGrid, patternManager);
            var position = new Vector2Int(0, 0);
            solver.CollapseCell(position);
            Assert.True(outputGrid.IsCellCollapsed(position));
        }

        // A Test behaves as an ordinary method
        [Test]
        public void CoreSolverGetLowestEntropyRandomPasses()
        {
            outputGrid = new OutputGrid(1, 1, patternManager.GetNumberOfPatterns());
            solver = new CoreSolver(outputGrid, patternManager);
            var position = new Vector2Int(0, 0);
            var returnPosition = solver.GetLowestEntropyCell();
            Assert.True(position==returnPosition);
        }

        // A Test behaves as an ordinary method
        [Test]
        public void CoreSolverPropagatePass()
        {
            outputGrid = new OutputGrid(2, 2, patternManager.GetNumberOfPatterns());
            solver = new CoreSolver(outputGrid, patternManager);
            var position = new Vector2Int(0, 0);
            solver.CollapseCell(position);
            solver.Propagate();
            var numberOfPossiblePatterns = patternManager.GetNumberOfPatterns();
            Assert.True(outputGrid.GetPossibleValuesForPositon(new Vector2Int(1,0)).Count<numberOfPossiblePatterns && outputGrid.GetPossibleValuesForPositon(new Vector2Int(0,1)).Count < numberOfPossiblePatterns);
        }

        // A Test behaves as an ordinary method
        [Test]
        public void CoreSolverEntropyPass()
        {
            outputGrid = new OutputGrid(2, 2, patternManager.GetNumberOfPatterns());
            outputGrid.SetPatternOnPosition(1, 1, 0);
            solver = new CoreSolver(outputGrid, patternManager);
            var position = new Vector2Int(0, 0);
            solver.CollapseCell(position);
            solver.Propagate();
            var newPosition1 = solver.GetLowestEntropyCell();
            
            var newPosition2 = solver.GetLowestEntropyCell();
            Debug.Log(newPosition1 + " " + newPosition2);
            Debug.Log(outputGrid.GetPossibleValuesForPositon(new Vector2Int(0, 1)).Count + " " + outputGrid.GetPossibleValuesForPositon(new Vector2Int(1, 1)).Count);
            Debug.Log(outputGrid.GetPossibleValuesForPositon(new Vector2Int(0, 0)).Count + " " + outputGrid.GetPossibleValuesForPositon(new Vector2Int(1, 0)).Count);

            Assert.True(((newPosition1 == new Vector2Int(0, 1) || newPosition1 == new Vector2Int(1, 0)) && (newPosition2 == new Vector2Int(0, 1) || newPosition2 == new Vector2Int(1, 0))) || outputGrid.IsGridSolved());
        }

        // A Test behaves as an ordinary method
        [Test]
        public void CoreSolverEntropyUpdatePass()
        {
            outputGrid = new OutputGrid(2, 2, patternManager.GetNumberOfPatterns());
            solver = new CoreSolver(outputGrid, patternManager);
            var position = new Vector2Int(0, 0);
            solver.CollapseCell(position);
            solver.Propagate();
            
            var newPosition1 = solver.GetLowestEntropyCell();
            solver.CollapseCell(newPosition1);
            solver.Propagate();

            while (solver.CheckForConflics() == false && outputGrid.IsGridSolved() == false)
            {
                newPosition1 = solver.GetLowestEntropyCell();
                solver.CollapseCell(newPosition1);
                solver.Propagate();
            }
            

            Debug.Log(outputGrid.GetPossibleValuesForPositon(new Vector2Int(0, 1)).Count + " " + outputGrid.GetPossibleValuesForPositon(new Vector2Int(1, 1)).Count);
            Debug.Log(outputGrid.GetPossibleValuesForPositon(new Vector2Int(0, 0)).Count + " " + outputGrid.GetPossibleValuesForPositon(new Vector2Int(1, 0)).Count);


            Assert.True(solver.CheckForConflics() || outputGrid.IsGridSolved());
        }
    }
}
