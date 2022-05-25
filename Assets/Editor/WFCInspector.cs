using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace WaveFunctionCollapse
{
    [CustomEditor(typeof(WaveFunctionCollapseGenerator))]
    public class WFCInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            WaveFunctionCollapseGenerator myScript = (WaveFunctionCollapseGenerator)target;
            
            if (GUILayout.Button("Create tilemap"))
            {
                myScript.CreateWFC();
                myScript.CreateTilemap();
            }

            if (GUILayout.Button("Save tilemap")) myScript.SaveTilemap();
        }
    }
}