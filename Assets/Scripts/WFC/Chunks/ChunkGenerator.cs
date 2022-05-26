using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WaveFunctionCollapse
{
    public class ChunkGenerator : MonoBehaviour
    {
        public Tilemap inputImage;
        public Grid gridObject;

        // [Tooltip("For tiles usualy set to 1. If tile contain just a color can set to higher value")]
        // public int patternSize;

        [Tooltip("How many times algorithm will try creating the output before quiting")]
        public int maxIterations;

        [Tooltip("Chunk will be NxN tiles")]
        public int chunkSize = 5;

        [Tooltip("NxM Chunks to generate initially")]
        public Vector2 dimensionOfChunks = new Vector2(3, 6);

        [Tooltip("Don't use tile frequency - each tile has equal weight")]
        public bool equalWeights = false;

        private ChunkMap chunkMap;

        public void GenerateChunks()
        {
            ChunkMap chunkMap = new ChunkMap();

            Debug.Log("Generating chunks");

            for (int x = 0; x < dimensionOfChunks.x; x++)
            {
                for (int y = 0; y < dimensionOfChunks.y; y++)
                {
                    WFChunks wfc = new WFChunks(this.inputImage, 1, this.chunkSize, this.maxIterations, this.equalWeights, chunkMap, new ChunkCoordinate(x, y));
                    wfc.CreateNewTilemap(gridObject);
                    Chunk chunk = wfc.GetOutputTileMap();
                    chunkMap.AddChunk(new ChunkCoordinate(x, y), chunk);
                    chunk.gameObject.name = "Chunk_" + x + "_" + y;
                    TranslateChunk(chunk.gameObject, x, y);
                    Debug.Log("Generated chunk (" + x + ", " + y + ")");
                }
            }
        }

        public void SaveTilemap()
        {
            // var output = wfchunks.GetOutputTileMap();
            // if (output != null)
            // {
            //     outputImage = output;
            //     GameObject objectToSave = outputImage.gameObject;

            //     PrefabUtility.SaveAsPrefabAsset(objectToSave, outputPath);
            // }
        }

        private void TranslateChunk(GameObject chunk, int x, int y)
        {
            chunk.transform.Translate(new Vector3(x * (chunkSize), y * (chunkSize), 0));
        }
    }
}