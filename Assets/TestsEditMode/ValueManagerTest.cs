using System.Collections;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.Tilemaps;
using WaveFunctionCollapse;

namespace Tests
{
    public class ValueManagerTest
    {
        GameObject tileMapPrefab;
        Tilemap tilemap;
        InputReader inputReader;

        [OneTimeSetUp]
        public void Init()
        {
            tileMapPrefab = Resources.Load("TestTilemap") as GameObject; //TestTilemap Variant
            tilemap = tileMapPrefab.GetComponent<Tilemap>();
            inputReader = new InputReader(tilemap);
        }

        // A Test behaves as an ordinary method
        [Test]
        public void ValueManagerReadValuesTest()
        {
            ValuesManager<TileBase> valueManager = new ValuesManager<TileBase>(inputReader.ReadInputToGrid());
            Assert.True(valueManager != null && valueManager.GetGridValue(0, 0) == 0);
        }

        // A Test behaves as an ordinary method
        [Test]
        public void ValueManagerIndexTestTrue()
        {

            ValuesManager<TileBase> valueManager = new ValuesManager<TileBase>(inputReader.ReadInputToGrid());
            StringBuilder b;
            for (int row = 0; row < valueManager.GetGridSize().y; row++)
            {
                b = new StringBuilder();
                for (int col = 0; col < valueManager.GetGridSize().x; col++)
                {
                    b.Append(valueManager.GetGridValue(col, row) + " ");
                }
                Debug.Log(b.ToString());
            }
            Assert.True(valueManager.GetGridValue(0,1)== valueManager.GetGridValue(0, 2));
        }

        [Test]
        public void ValueManagerIndexTestFalse()
        {

            ValuesManager<TileBase> valueManager = new ValuesManager<TileBase>(inputReader.ReadInputToGrid());

            Assert.False(valueManager.GetGridValue(0, 1) == valueManager.GetGridValue(0,0));
        }

        //[Test]
        //public void ValueManagerFreqncyTest()
        //{

        //    ValuesManager<TileBase> valueManager = new ValuesManager<TileBase>(inputReader.ReadInputToGrid());
        //    Debug.Log(valueManager.GetFrequencyFromIndex(1));
        //    Assert.True(valueManager.GetFrequencyFromIndex(1)==2);
        //}

        [Test]
        public void ValueManagerGetGridValueIncludingOffsetSpecialCaseTest()
        {

            ValuesManager<TileBase> valueManager = new ValuesManager<TileBase>(inputReader.ReadInputToGrid());
            //Debug.Log(valueManager.GetFrequencyFromIndex(2));
            Assert.True(
                valueManager.GetGridValueIncludingOffset(-1,-1)==valueManager.GetGridValue((int)valueManager.GetGridSize().x-1, (int)valueManager.GetGridSize().y-1) 
                && valueManager.GetGridValueIncludingOffset((int)valueManager.GetGridSize().x, (int)valueManager.GetGridSize().y) == valueManager.GetGridValue(0, 0)
                && valueManager.GetGridValueIncludingOffset(-1, (int)valueManager.GetGridSize().y) == valueManager.GetGridValue((int)valueManager.GetGridSize().x-1, 0)
                &&valueManager.GetGridValueIncludingOffset((int)valueManager.GetGridSize().x, -1) == valueManager.GetGridValue(0, (int)valueManager.GetGridSize().y-1)
                );
        }

        [Test]
        public void ValueManagerGetGridValueIncludingOffsetSpecialCase3x3Test()
        {

            ValuesManager<TileBase> valueManager = new ValuesManager<TileBase>(inputReader.ReadInputToGrid());
            //Debug.Log(valueManager.GetFrequencyFromIndex(2));
            Assert.True(
                valueManager.GetGridValueIncludingOffset(-2, -2) == valueManager.GetGridValue((int)valueManager.GetGridSize().x - 2, (int)valueManager.GetGridSize().y - 2)
                && valueManager.GetGridValueIncludingOffset((int)valueManager.GetGridSize().x+2, (int)valueManager.GetGridSize().y+2) == valueManager.GetGridValue(2, 2)
                && valueManager.GetGridValueIncludingOffset(-2, (int)valueManager.GetGridSize().y+1) == valueManager.GetGridValue((int)valueManager.GetGridSize().x - 2, 1)
                && valueManager.GetGridValueIncludingOffset((int)valueManager.GetGridSize().x+1, -2) == valueManager.GetGridValue(1, (int)valueManager.GetGridSize().y - 2)
                );
        }

        [Test]
        public void ValueManagerGetGridValueIncludingOffsetUsualCaseTest()
        {

            ValuesManager<TileBase> valueManager = new ValuesManager<TileBase>(inputReader.ReadInputToGrid());
            //Debug.Log(valueManager.GetFrequencyFromIndex(2));
            Assert.True(
                valueManager.GetGridValueIncludingOffset(-1, 1) == valueManager.GetGridValue((int)valueManager.GetGridSize().x-1, 1)
                && valueManager.GetGridValueIncludingOffset(1, (int)valueManager.GetGridSize().y) == valueManager.GetGridValue(1, 0)
                && valueManager.GetGridValueIncludingOffset(-1, (int)valueManager.GetGridSize().y-1) == valueManager.GetGridValue((int)valueManager.GetGridSize().x-1, (int)valueManager.GetGridSize().y-1)
                &&valueManager.GetGridValueIncludingOffset((int)valueManager.GetGridSize().x-1, -1) == valueManager.GetGridValue((int)valueManager.GetGridSize().x-1, (int)valueManager.GetGridSize().y-1)
                && valueManager.GetGridValueIncludingOffset((int)valueManager.GetGridSize().x-1, (int)valueManager.GetGridSize().y-1) == valueManager.GetGridValue((int)valueManager.GetGridSize().x-1, (int)valueManager.GetGridSize().y-1)
                );
        }

    }
}
