using UnityEngine;
using System.Collections;
using UnityEditor;
using ConstCollections;

namespace MapManagement.Editor
{
  [UnityEditor.CanEditMultipleObjects]
  [UnityEditor.CustomEditor(typeof(TerrainTilesGenerator))]
  public class TerrainTilesGeneratorEditor : UnityEditor.Editor
  {
    void OnEnable()
    {
      script = (TerrainTilesGenerator)target;
    }


    public override void OnInspectorGUI() 
    {

      DrawDefaultInspector();

//      if (script.ReplaceTileTypes.Length == 0) {
//        Debug.LogError ("SpawnableTileTypes is needed !");
//      }

      if (Application.isPlaying)
        return;

    }


    TerrainTilesGenerator script;
  }
}

