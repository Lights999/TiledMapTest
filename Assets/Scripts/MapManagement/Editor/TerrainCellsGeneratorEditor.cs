using UnityEngine;
using System.Collections;
using UnityEditor;
using ConstCollections;

namespace MapManagement.Editor
{
  [UnityEditor.CanEditMultipleObjects]
  [UnityEditor.CustomEditor(typeof(TerrainCellsGenerator))]
  public class TerrainCellsGeneratorEditor : UnityEditor.Editor
  {
    void OnEnable()
    {
      script = (TerrainCellsGenerator)target;
    }

    public override void OnInspectorGUI() 
    {

      DrawDefaultInspector();

      if (Application.isPlaying)
        return;

    }


    TerrainCellsGenerator script;
  }
}

