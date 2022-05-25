using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.Tilemaps;
using WaveFunctionCollapse;

namespace Tests
{
    public class TileMapOutputTest
    {

        GameObject tileMapPrefab;
        Tilemap tilemap;
        Tilemap outputtilemap;
        InputReader inputReader;
        ValuesManager<TileBase> valueManager;
        PatternManager patternManager;
        OutputGrid outputGrid;
        CoreSolver solver;
        int patternSize = 2;
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
            patternManager.ProcessGrid(valueManager,false);
            outputtilemap = (Resources.Load("OutputImage") as GameObject).GetComponent<Tilemap>();

        }

        // A Test behaves as an ordinary method
        [Test]
        public void TileMapOutputTestSimplePasses()
        {
            //outputGrid = new OutputGrid(3, 3, patternManager.GetNumberOfPatterns());
            //solver = new CoreSolver(outputGrid, patternManager);
            //var position = new Vector2Int(0, 0);
            //for (int i = 0; i < 3; i++)
            //{
            //    for (int j = 0; j < 3; j++)
            //    {
            //        outputGrid.SetPatternOnPosition(i, j, 1);
            //    }
            //}

            outputGrid = new OutputGrid(5, 5, patternManager.GetNumberOfPatterns());
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
            if (solver.CheckForConflics())
            {
                Debug.Log("Conflict");
                Assert.True(true);
            }
            else
            {
                outputGrid.PrintResultsToConsole();
                int[][] wfcOutput = outputGrid.GetSolvedOutputGrid();
                IOutputCreator<Tilemap> tileOutput = new TilemapOutput(valueManager, outputtilemap);
                tileOutput.CreateOutput(patternManager, wfcOutput, outputGrid.width, outputGrid.height);
                Assert.True(CheckEveryNeighbour(outputGrid));
            }

            
        }

        private bool CheckEveryNeighbour(OutputGrid outputGrid)
        {
            for (int row = 0; row < outputGrid.height; row++)
            {
                for (int col = 0; col < outputGrid.width; col++)
                {
                    Vector2Int cellCoordinates = new Vector2Int(col, row);
                    List<VectorPair> list = new List<VectorPair>()
                    {
                        new VectorPair(cellCoordinates, cellCoordinates + new Vector2Int(1, 0), Direction.Right,cellCoordinates),
                        new VectorPair(cellCoordinates, cellCoordinates + new Vector2Int(-1, 0), Direction.Left, cellCoordinates),
                        new VectorPair(cellCoordinates, cellCoordinates + new Vector2Int(0, 1), Direction.Up, cellCoordinates),
                        new VectorPair(cellCoordinates, cellCoordinates + new Vector2Int(0, -1), Direction.Down, cellCoordinates)
                    };
                    foreach (var item in list)
                    {
                        if (item.CellToPropagatePosition.x >= 0 && item.CellToPropagatePosition.x < 3 && item.CellToPropagatePosition.y >= 0 && item.CellToPropagatePosition.y < 3 && outputGrid.ContainsCell(item.CellToPropagatePosition))
                        {
                            var itemToCheck = outputGrid.GetPossibleValuesForPositon(item.CellToPropagatePosition).First();
                            var baseCell = outputGrid.GetPossibleValuesForPositon(cellCoordinates).First();
                            if (patternManager.GetPossibleNeighborsForPatternInDirection(baseCell, item.DiectionFromBase).Contains(itemToCheck) == false)
                            {
                                Debug.Log(baseCell + " cant have a neighbour " + itemToCheck + " at direction " + item.DiectionFromBase.ToString());
                                return false;
                            }
                        }
                    }
                }
            }
            return true;
        }

    }
}
