using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.Tilemaps;
using WaveFunctionCollapse;

namespace Tests
{
    public class InputReaderTest
    {
        GameObject tileMapPrefab;
        Tilemap tilemap;
        
        [OneTimeSetUp]
        public void Init()
        {
            tileMapPrefab = Resources.Load("TestTilemap Variant") as GameObject;
            tilemap = tileMapPrefab.GetComponent<Tilemap>();
        }

        // A Test behaves as an ordinary method/
        [Test]
        public void InputReaderLoadResourceTest()
        {
            Assert.True(tileMapPrefab != null);
        }

        [Test]
        public void InputReaderGetTilemapFromPrefabTest()
        {
            Assert.True(tilemap != null && tilemap is Tilemap);
        }

        [Test]
        public void InputReaderReadTilesFromTilemap()
        {
            //Arrange
            InputReader inputReader = new InputReader(tilemap);
            //Act
            var output = inputReader.ReadInputTileMap();

            //Assert
            Assert.True(output != null && output.Length == 4 && output[0].Length==4);
        }

        [Test]
        public void InputReaderCompareTilesTest()
        {
            //Arrange
            InputReader inputReader = new InputReader(tilemap);
            //Act
            var output = inputReader.ReadInputTileMap();
            for (int i = 0; i < output.Length; i++)
            {
                for (int j = 0; j < output[i].Length; j++)
                {
                    Debug.Log("X: " + i + " Y: " + j + " tile: " + output[i][j].name);
                }
            }
            //Assert
            Assert.True(output[1][0] == output[2][0]);
        }

        [Test]
        public void InputReaderReadGridFromTilemap()
        {
            //Arrange
            IInputReader<TileBase> inputReader = new InputReader(tilemap);
            //Act
            var output = inputReader.ReadInputToGrid();

            //Assert
            Assert.True(output != null && output.Length == 4 && output[0].Length == 4 && output[0][0] is IValue<TileBase>);
        }


        [Test]
        public void InputReaderReadWrongGridLackTilesFromTilemap()
        {
            tileMapPrefab = Resources.Load("TestTilemapWrong") as GameObject;
            tilemap = tileMapPrefab.GetComponent<Tilemap>();
            //Arrange
            IInputReader<TileBase> inputReader = new InputReader(tilemap);
            //Act
            //var output = inputReader.ReadInputToGrid();

            //Assert
            Assert.That(() => inputReader.ReadInputToGrid(),
                  Throws.TypeOf<System.Exception>());
        }

        [Test]
        public void InputReaderReadWrongGridNotRectangleFromTilemap()
        {
            tileMapPrefab = Resources.Load("TestTilemapWrong2") as GameObject;
            tilemap = tileMapPrefab.GetComponent<Tilemap>();
            //Arrange
            IInputReader<TileBase> inputReader = new InputReader(tilemap);
            //Act
            //var output = inputReader.ReadInputToGrid();

            //Assert
            Assert.That(() => inputReader.ReadInputToGrid(),
                  Throws.TypeOf<System.Exception>());
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        //[UnityTest]
        //public IEnumerator InputReaderTestWithEnumeratorPasses()
        //{
        //    // Use the Assert class to test conditions.
        //    // Use yield to skip a frame.
        //    yield return null;
        //}
    }
}
