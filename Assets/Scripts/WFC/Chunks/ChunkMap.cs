using System.Collections.Generic;
using UnityEngine;

namespace WaveFunctionCollapse
{
    public class ChunkMap
    {
        private Dictionary<ChunkCoordinate, Chunk> chunkMap = new Dictionary<ChunkCoordinate, Chunk>();

        public bool ContainsChunk(int x, int y)
        {
            return chunkMap.ContainsKey(new ChunkCoordinate(x, y));
        }

        public void AddChunk(ChunkCoordinate chunkCoordinate, Chunk chunk)
        {
            chunkMap.Add(chunkCoordinate, chunk);
        }

        public Chunk GetChunk(ChunkCoordinate chunkCoordinate)
        {
            if (chunkMap.ContainsKey(chunkCoordinate))
            {
                return chunkMap[chunkCoordinate];
            }
            return null;
        }

        public Chunk GetChunk(int x, int y)
        {
            return GetChunk(new ChunkCoordinate(x, y));
        }

        public Chunk RemoveChunk(int x, int y)
        {
            if (chunkMap.ContainsKey(new ChunkCoordinate(x, y)))
            {
                Chunk chunk = chunkMap[new ChunkCoordinate(x, y)];
                chunkMap.Remove(new ChunkCoordinate(x, y));
                return chunk;
            }
            return null;
        }

        public bool ChunkHasLeftNeighbor(ChunkCoordinate chunkCoordinate)
        {
            Debug.Log("Has left neighbor: " + chunkMap.ContainsKey(new ChunkCoordinate(chunkCoordinate.x - 1, chunkCoordinate.y)));
            return chunkMap.ContainsKey(new ChunkCoordinate(chunkCoordinate.x - 1, chunkCoordinate.y));
        }

        public bool ChunkHasRightNeighbor(ChunkCoordinate chunkCoordinate)
        {
            //Debug.Log("Has right neighbor: " + chunkMap.ContainsKey(new ChunkCoordinate(chunkCoordinate.x + 1, chunkCoordinate.y)));
            return chunkMap.ContainsKey(new ChunkCoordinate(chunkCoordinate.x + 1, chunkCoordinate.y));
        }

        public bool ChunkHasTopNeighbor(ChunkCoordinate chunkCoordinate)
        {
            //Debug.Log("Has top neighbor: " + chunkMap.ContainsKey(new ChunkCoordinate(chunkCoordinate.x, chunkCoordinate.y + 1)));
            return chunkMap.ContainsKey(new ChunkCoordinate(chunkCoordinate.x, chunkCoordinate.y + 1));
        }

        public bool ChunkHasBottomNeighbor(ChunkCoordinate chunkCoordinate)
        {
            //Debug.Log("Has bottom neighbor: " + chunkMap.ContainsKey(new ChunkCoordinate(chunkCoordinate.x, chunkCoordinate.y - 1)));
            return chunkMap.ContainsKey(new ChunkCoordinate(chunkCoordinate.x, chunkCoordinate.y - 1));
        }

        public Chunk GetLeftNeighbor(ChunkCoordinate chunkCoordinate)
        {
            return GetChunk(new ChunkCoordinate(chunkCoordinate.x - 1, chunkCoordinate.y));
        }

        public Chunk GetRightNeighbor(ChunkCoordinate chunkCoordinate)
        {
            return GetChunk(new ChunkCoordinate(chunkCoordinate.x + 1, chunkCoordinate.y));
        }

        public Chunk GetTopNeighbor(ChunkCoordinate chunkCoordinate)
        {
            return GetChunk(new ChunkCoordinate(chunkCoordinate.x, chunkCoordinate.y + 1));
        }

        public Chunk GetBottomNeighbor(ChunkCoordinate chunkCoordinate)
        {
            return GetChunk(new ChunkCoordinate(chunkCoordinate.x, chunkCoordinate.y - 1));
        }
    }
}