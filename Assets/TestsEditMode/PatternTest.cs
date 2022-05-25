using System.Collections;
using System.Collections.Generic;
using NSubstitute;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.Tilemaps;
using WaveFunctionCollapse;
using Helpers;

namespace Tests
{
    public class PatternTest
    {
        Pattern pattern;
        Pattern pattern2;

        [OneTimeSetUp]
        public void Init()
        {
            int[][] test = MyCollectionExtension.CreateJaggedArray<int[][]>(1, 1);
            string hashCode = HashCodeCalculator.CalculateHashCode(test);
            pattern = new Pattern(test, hashCode,0);
            pattern2 = new Pattern(test, hashCode,1);
        }

        // A Test behaves as an ordinary method
        [Test]
        public void CreatePatternSize2Test()
        {
            //arrange
            //act
            int[][] test = new int[1][];
            test[0] = new int[] { 0 };
            string hashCode = HashCodeCalculator.CalculateHashCode(test);
            Pattern patternTEst = new Pattern(test, hashCode,3);
            Debug.Log("Value at (0, 0) " + patternTEst.GetGridValue(0, 0));
            //assert
            Assert.True(patternTEst != null && patternTEst.GetGridValue(0, 0) == 0);
        }

        [Test]
        public void CreatePatternSize2TestFail()
        {
            //arrange
            //IValue<TileBase> value = Substitute.For<TileBaseValue>();
            //act

            //assert
            Assert.Throws<System.IndexOutOfRangeException>(()=>pattern.GetGridValue(2, 2));
        }


        [Test]
        public void CreatePatternSetValueTest()
        {
            //arrange
            //IValue<TileBase> value = Substitute.For<TileBaseValue>();
            //act
            int tileBaseValue = 1;
            pattern.SetGridValue(0, 0, tileBaseValue);
            //assert
            Assert.True(pattern.CheckValueAtPosition(0, 0, tileBaseValue));
        }

        [Test]
        public void CreatePatternSetValueFailTest()
        {
            //arrange
            //IValue<TileBase> value = Substitute.For<TileBaseValue>();
            //act
            int tileBaseValue = 1;
            pattern.SetGridValue(0, 0, tileBaseValue);

            int tileBaseValue2 = 2;
            //assert
            Assert.False(pattern.CheckValueAtPosition(0, 0, tileBaseValue2));
        }


        [Test]
        public void CreatePatternIndexTest()
        {
            //arrange

            //IValue<TileBase> value = Substitute.For<TileBaseValue>();
            //act

            //assert
            //Debug.Log(pattern.Index + " " + pattern2.Index);
            Assert.True(pattern2.Index != pattern.Index && pattern.Index == 0 && pattern2.Index == 1);
        }

        [Test]
        public void CreatePatternIndexFailTest()
        {
            //arrange
            //IValue<TileBase> value = Substitute.For<TileBaseValue>();
            //act

            //assert
            Assert.False(pattern2.Index == 2);
        }
    }
}
