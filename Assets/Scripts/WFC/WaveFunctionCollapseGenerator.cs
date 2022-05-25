using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace WaveFunctionCollapse
{
    public class WaveFunctionCollapseGenerator : MonoBehaviour
    {
        public Tilemap inputImage;
        public Tilemap outputImage;
        [Tooltip("For tiles usualy set to 1. If tile contain just a color can set to higher value")]
        public int patternSize;
        [Tooltip("How many times algorithm will try creating the output before quiting")]
        public int maxIterations;
        [Tooltip("Output image width")]
        public int outputWidth = 5;
        [Tooltip("Output image height")]
        public int outputHeight = 5;
        [Tooltip("Don't use tile frequency - each tile has equal weight")]
        public bool equalWeights = false;
        WaveFunctionCollapse wfc;

        public string outputPath;

        // Start is called before the first frame update
        void Start()
        {
            // CreateWFC();
            // CreateTilemap();
            // SaveTilemap();
        }

        public void CreateWFC()
        {
            wfc = new WaveFunctionCollapse(this.inputImage, this.outputImage, patternSize, this.outputWidth, this.outputHeight, this.maxIterations, this.equalWeights);
        }
        public void CreateTilemap()
        {
            wfc.CreateNewTileMap();
        }

        public void CreateTilemap(Tilemap inputTilemap)
        {
            wfc.CreateNewTileMap(inputTilemap);
        }

        public void SaveTilemap()
        {
            var output = wfc.GetOutputTileMap();
            if (output != null)
            {
                outputImage = output;
                GameObject objectToSave = outputImage.gameObject;

                PrefabUtility.SaveAsPrefabAsset(objectToSave, outputPath);
            }
        }
    }
}