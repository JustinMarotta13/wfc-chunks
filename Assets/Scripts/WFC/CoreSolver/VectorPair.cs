using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WaveFunctionCollapse
{
    public class VectorPair
    {
        public Vector2Int BaseCellPosition { get; set; }
        public Vector2Int CellToPropagatePosition { get; set; }
        private Vector2Int PreviousCellPosition { get; set; }
        public Direction DiectionFromBase { get; set; }

        public VectorPair(Vector2Int baseCellPosition, Vector2Int cellToPropagatePosition, Direction directionFromBase, Vector2Int previousCellPosition)
        {
            this.BaseCellPosition = baseCellPosition;
            this.CellToPropagatePosition = cellToPropagatePosition;
            this.DiectionFromBase = directionFromBase;
            this.PreviousCellPosition = previousCellPosition;
        }

        public bool AreWeCheckingPreviousCellAgain()
        {
            return PreviousCellPosition == CellToPropagatePosition;
        }
    }
}