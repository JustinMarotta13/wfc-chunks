using UnityEngine.Tilemaps;
using UnityEngine;
using System.Collections.Generic;

namespace WaveFunctionCollapse
{
    public class ChunkCoordinate
    {
        public int x { get; set; }
        public int y { get; set; }

        public ChunkCoordinate(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;

            ChunkCoordinate other = obj as ChunkCoordinate;

            if (other == null) return false;

            return this.x == other.x && this.y == other.y;
        }

        public bool Equals(int x, int y)
        {
            return this.x == x && this.y == y;
        }

        public override int GetHashCode()
        {
            return x.GetHashCode() ^ y.GetHashCode();
        }
    }

    public class Chunk
    {
        public GameObject gameObject { get; set; }
        public ChunkCoordinate chunkLocation;
        public int[] topBorder;
        public int[] bottomBorder;
        public int[] leftBorder;
        public int[] rightBorder;

        public bool hasLeftNeighbor;
        public bool hasRightNeighbor;
        public bool hasTopNeighbor;
        public bool hasBottomNeighbor;

        public Chunk(int chunkSize, ChunkCoordinate chunkLocation)
        {
            topBorder = new int[chunkSize+2];
            bottomBorder = new int[chunkSize+2];
            leftBorder = new int[chunkSize+2];
            rightBorder = new int[chunkSize+2];
            this.chunkLocation = chunkLocation;
        }
    }
}