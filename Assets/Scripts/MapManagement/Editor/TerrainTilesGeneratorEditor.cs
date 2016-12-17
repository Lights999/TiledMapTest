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

    /*
    public override void OnInspectorGUI() 
    {

      DrawDefaultInspector();

      if (Application.isPlaying)
        return;

    }*/


    TerrainTilesGenerator script;
  }
}

