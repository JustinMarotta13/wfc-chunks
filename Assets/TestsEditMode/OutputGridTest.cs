using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using WaveFunctionCollapse;

namespace Tests
{
    public class OutputGridTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void OutputGridTestCOnstructorPasses()
        {
            //arrange
            OutputGrid outputGrid = new OutputGrid(5, 5, 10);
            //act
            var list = outputGrid.GetPossibleValuesForPositon(new Vector2Int(4,4));
            //assert
            Assert.True(list.Count == 10 && list.ElementAt(0)==0 && list.ElementAt(9)==9);
        }

        [Test]
        public void OutputGridTestCOnstructorFail()
        {
            //arrange
            OutputGrid outputGrid = new OutputGrid(5, 5, 10);
            //act
            //assert
            Assert.True(outputGrid.GetPossibleValuesForPositon(new Vector2Int(5,5)).Count == 0);
        }

        [Test]
        public void OutputGridTestOutputSolution()
        {
            //arrange
            OutputGrid outputGrid = new OutputGrid(5, 5, 10);
            //act
            int[][] output = outputGrid.GetSolvedOutputGrid();
            //assert
            Assert.True(output.Length==0);
        }
    }
}
