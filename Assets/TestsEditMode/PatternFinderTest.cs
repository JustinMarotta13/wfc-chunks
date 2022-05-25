using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.Tilemaps;
using WaveFunctionCollapse;

namespace Tests
{
    public class PatternFinderTest
    {
        GameObject tileMapPrefab;
        Tilemap tilemap;
        InputReader inputReader;
        ValuesManager<TileBase> valueManager;
        PatternDataResults patterndataResults;
        int patternSize = 1;
        IFindNeighborStrategy strategy = new NeighborStartegySize1Default();
        [OneTimeSetUp]
        public void Init()
        {
            tileMapPrefab = Resources.Load("TestTilemap Variant") as GameObject; //"TestTilemap"
            tilemap = tileMapPrefab.GetComponent<Tilemap>();
            inputReader = new InputReader(tilemap);
            valueManager = new ValuesManager<TileBase>(inputReader.ReadInputToGrid());
            patterndataResults = PatternFinder.GetPatternDataFromGrid(valueManager, patternSize,false);
            if (patternSize > 1)
            {
                strategy = new NeighborStartegySize2andMore();
            }
        }
        // A Test behaves as an ordinary method
        [Test]
        public void PatternFinderHashCalculationTest()
        {
            Hashtable table = new Hashtable();
            int[][] arr1 = new int[2][];
            arr1[0] = new int[] { 1, 2 };
            arr1[1] = new int[] { 1, 2 };

            int[][] arr2 = new int[2][];
            arr2[0] = new int[] { 1, 2 };
            arr2[1] = new int[] { 1, 2 };

            int[][] arr3 = new int[2][];
            arr3[0] = new int[] { 2, 2 };
            arr3[1] = new int[] { 1, 2 };

            Assert.That(PatternFinder.AreArraysTheSame(arr1, arr2));
        }

        [Test]
        public void PatternFinderResultTest()
        {
            //arrange
            //act

            StringBuilder b;
            for (int y = 0; y < patterndataResults.GetGridLengthInY(); y++)
            {
                b = new StringBuilder();
                for (int x = 0; x < patterndataResults.GetGridLengthInX(); x++)
                {
                    b.Append(patterndataResults.GetIndexAt(x, y) + " ");
                }
                Debug.Log(b.ToString());
            }
            //assert

            Assert.That(patterndataResults.GetGridLengthInY() == valueManager.GetGridSize().x + 2 - (patternSize - 1));
        }

        [Test]
        public void PatternFinderCheckNeighbours2Test()
        {
            //arrange
            //act
            var neighbours = PatternFinder.CheckNeighboursInEachDirection(0, 0, patterndataResults);
            foreach (var item in neighbours.directionNeighborPatternDictionary)
            {
                Debug.Log("Direction " + item.Key.ToString());
                foreach (var item2 in item.Value)
                {
                    Debug.Log("Pattern: " + item2);
                }
            }

            Assert.That(neighbours.directionNeighborPatternDictionary.Count == 2);
        }

        [Test]
        public void PatternFinderCheckNeighbours3Test()
        {
            //arrange
            //act
            var neighbours = PatternFinder.CheckNeighboursInEachDirection(0, 1, patterndataResults);
            foreach (var item in neighbours.directionNeighborPatternDictionary)
            {
                Debug.Log("Direction " + item.Key.ToString());
                foreach (var item2 in item.Value)
                {
                    Debug.Log("Pattern: " + item2);
                }
            }

            Assert.That(neighbours.directionNeighborPatternDictionary.Count == 3);
        }

        [Test]
        public void PatternFinderCheckNeighbours4Test()
        {
            //arrange
            //act
            var neighbours = PatternFinder.CheckNeighboursInEachDirection(3, 3, patterndataResults);
            Debug.Log("Pattern " + patterndataResults.GetIndexAt(3, 3));
            foreach (var item in neighbours.directionNeighborPatternDictionary)
            {

                Debug.Log("Direction " + item.Key.ToString());
                foreach (var item2 in item.Value)
                {
                    Debug.Log("Pattern: " + item2);
                }
            }

            Assert.That(neighbours.directionNeighborPatternDictionary.Count == 4);
        }

        [Test]
        public void PatternFinderCheckNeighboursLastTest()
        {
            //arrange
            //act
            var neighbours = PatternFinder.CheckNeighboursInEachDirection(patterndataResults.GetGridLengthInX() - 1, patterndataResults.GetGridLengthInY() - 1, patterndataResults);
            foreach (var item in neighbours.directionNeighborPatternDictionary)
            {
                Debug.Log("Direction " + item.Key.ToString());
                foreach (var item2 in item.Value)
                {
                    Debug.Log("Pattern: " + item2);
                }
            }

            Assert.That(neighbours.directionNeighborPatternDictionary.Count == 2);
        }

        [Test]
        public void PatternFinderCheckNeighboursDictionaryTest()
        {
            //arrange
            //act
            var neighbours = PatternFinder.FindPossibleNeighborsForAllPatterns(strategy, patterndataResults);
            StringBuilder b;
            foreach (var item in neighbours)
            {
                b = new StringBuilder();
                b.Append("Pattern " + item.Key + " ");
                foreach (var item2 in item.Value.directionNeighborPatternDictionary)
                {
                    b.Append("Direction " + item2.Key.ToString() + " ");
                    foreach (var item3 in item2.Value)
                    {
                        b.Append(item3 + ",");
                    }

                }
                Debug.Log(b.ToString());
            }
            if (patternSize == 1)
                Assert.That(neighbours.Count == 5);
            else if (patternSize == 2)
                Assert.That(neighbours.Count == 15);
            else
                Assert.False(true);
        }

        [Test]
        public void PatternFinderCheckNeighboursPattern0Down()
        {

            //arrange
            //act
            var neighbours = PatternFinder.FindPossibleNeighborsForAllPatterns(strategy, patterndataResults);

            StringBuilder b;
            Debug.Log("Pattern 0:");
            for (int i = 0; i < patternSize; i++)
            {
                b = new StringBuilder();
                for (int j = 0; j < patternSize; j++)
                {
                    b.Append(patterndataResults.PatternIndexDictionary[0].Pattern.GetGridValue(j, i) + " ");
                }
                Debug.Log(b.ToString());
            }
            Debug.Log("Neighbours:");
            foreach (var index in neighbours[0].GetNeighborsInDirection(Direction.Down))
            {

                Debug.Log("Pattern " + index + " ");
                for (int i = 0; i < patternSize; i++)
                {
                    b = new StringBuilder();
                    for (int j = 0; j < patternSize; j++)
                    {
                        b.Append(patterndataResults.PatternIndexDictionary[index].Pattern.GetGridValue(j, i) + " ");
                    }
                    Debug.Log(b.ToString());
                }
            }



            if (patternSize == 1)
                Assert.That(patterndataResults.PatternIndexDictionary[0].Pattern.GetGridValue(0, 0) == patterndataResults.PatternIndexDictionary[neighbours[0].GetNeighborsInDirection(Direction.Down).ElementAt(0)].Pattern.GetGridValue(0, 0));
            else
            {
                Assert.That(
                    patterndataResults.PatternIndexDictionary[0].Pattern.GetGridValue(0, 0) == patterndataResults.PatternIndexDictionary[neighbours[0].GetNeighborsInDirection(Direction.Down).ElementAt(0)].Pattern.GetGridValue(0, patternSize - 1)
                    && patterndataResults.PatternIndexDictionary[0].Pattern.GetGridValue(patternSize - 1, 0) == patterndataResults.PatternIndexDictionary[neighbours[0].GetNeighborsInDirection(Direction.Down).ElementAt(0)].Pattern.GetGridValue(patternSize - 1, patternSize - 1)
                    );
            };


        }

        [Test]
        public void PatternFinderCheckNeighboursPattern0UP()
        {

            //arrange
            //act
            var neighbours = PatternFinder.FindPossibleNeighborsForAllPatterns(strategy, patterndataResults);
            if (patternSize == 1)
                Assert.That(patterndataResults.PatternIndexDictionary[0].Pattern.GetGridValue(0, 0) == patterndataResults.PatternIndexDictionary[neighbours[0].GetNeighborsInDirection(Direction.Up).ElementAt(0)].Pattern.GetGridValue(0, 0));
            else
            {
                Assert.That(
                    patterndataResults.PatternIndexDictionary[0].Pattern.GetGridValue(0, patternSize - 1) == patterndataResults.PatternIndexDictionary[neighbours[0].GetNeighborsInDirection(Direction.Up).ElementAt(0)].Pattern.GetGridValue(0, 0)
                    && patterndataResults.PatternIndexDictionary[0].Pattern.GetGridValue(patternSize - 1, patternSize - 1) == patterndataResults.PatternIndexDictionary[neighbours[0].GetNeighborsInDirection(Direction.Up).ElementAt(0)].Pattern.GetGridValue(patternSize - 1, 0)
                    );
            };


        }

        [Test]
        public void PatternFinderCheckNeighboursPattern0RIGHT()
        {

            //arrange
            //act
            var neighbours = PatternFinder.FindPossibleNeighborsForAllPatterns(strategy, patterndataResults);
            if (patternSize == 1)
                Assert.That(patterndataResults.PatternIndexDictionary[0].Pattern.GetGridValue(0, 0) == patterndataResults.PatternIndexDictionary[neighbours[0].GetNeighborsInDirection(Direction.Right).ElementAt(0)].Pattern.GetGridValue(0, 0));
            else
            {
                Assert.That(
                    patterndataResults.PatternIndexDictionary[0].Pattern.GetGridValue(patternSize - 1, 0) == patterndataResults.PatternIndexDictionary[neighbours[0].GetNeighborsInDirection(Direction.Right).ElementAt(0)].Pattern.GetGridValue(0, 0)
                    && patterndataResults.PatternIndexDictionary[0].Pattern.GetGridValue(patternSize - 1, patternSize - 1) == patterndataResults.PatternIndexDictionary[neighbours[0].GetNeighborsInDirection(Direction.Right).ElementAt(0)].Pattern.GetGridValue(0, patternSize - 1)
                    );
            };

        }

        [Test]
        public void PatternFinderCheckNeighboursPattern0LEFT()
        {

            //arrange
            //act
            var neighbours = PatternFinder.FindPossibleNeighborsForAllPatterns(strategy, patterndataResults);
            if (patternSize == 1)
                Assert.That(patterndataResults.PatternIndexDictionary[0].Pattern.GetGridValue(0, 0) == patterndataResults.PatternIndexDictionary[neighbours[0].GetNeighborsInDirection(Direction.Left).ElementAt(0)].Pattern.GetGridValue(0, 0));
            else 
            {
                Assert.That(
                    patterndataResults.PatternIndexDictionary[0].Pattern.GetGridValue(0, patternSize - 1) == patterndataResults.PatternIndexDictionary[neighbours[0].GetNeighborsInDirection(Direction.Left).ElementAt(0)].Pattern.GetGridValue(patternSize - 1, patternSize - 1)
                    && patterndataResults.PatternIndexDictionary[0].Pattern.GetGridValue(0, 0) == patterndataResults.PatternIndexDictionary[neighbours[0].GetNeighborsInDirection(Direction.Left).ElementAt(0)].Pattern.GetGridValue(1, 0)
                    );
            };

        }


        [Test]
        public void PatternFinderCheckFrequencyTestPass()
        {
            //arrange
            //act
            StringBuilder b;
            foreach (var item in patterndataResults.PatternIndexDictionary)
            {
                b = new StringBuilder();
                b.Append("Pattern " + item.Key + " frequency " + item.Value.FrequencyRelative);
                Debug.Log(b.ToString());
            }
            if (patternSize == 1)
            {
                Assert.That(patterndataResults.PatternIndexDictionary[0].FrequencyRelative == 1.25f);
            }
            else if (patternSize == 2)
            {
                Assert.That(patterndataResults.PatternIndexDictionary[0].FrequencyRelative == 0.16f);
            }
            else
            {
                Assert.False(true);
            }


        }
    }


}
