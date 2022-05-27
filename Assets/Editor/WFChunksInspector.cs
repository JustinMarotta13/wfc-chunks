using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace WaveFunctionCollapse
{
    [CustomEditor(typeof(ChunkGenerator))]
    public class WFChunkInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            ChunkGenerator generator = (ChunkGenerator)target;
            
            if (GUILayout.Button("Generate Grid of Chunks"))
            {
                generator.GenerateChunks();
            }

            if (GUILayout.Button("Generate Chunk"))
            {
                generator.AddChunk();
            }

            //if (GUILayout.Button("Save tilemap")) generator.SaveTilemap();
        }
    }
}