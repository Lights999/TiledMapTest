using UnityEngine;
using System.Collections;
using UnityEditor;
using ConstCollections;
using ConstCollections.PJEnums;
using System.Collections.Generic;

namespace MapManagement.Editor
{
  [UnityEditor.CanEditMultipleObjects]
  [UnityEditor.CustomEditor(typeof(TerrainTilesGenerator))]
  public class TerrainTilesGeneratorEditor : UnityEditor.Editor
  {
    void OnEnable()
    {
      //script = (TerrainTilesGenerator)target;
    }


    public override void OnInspectorGUI() 
    {
      DrawDefaultInspector();

      if (Application.isPlaying)
        return;

      //spawnReplaceableTypes = (MAP_TILE_TYPE)EditorGUILayout.EnumMaskField("ReplaceableTypes ",spawnReplaceableTypes);

    }


    //TerrainTilesGenerator script;
    //MAP_TILE_TYPE spawnReplaceableTypes;
  }
}

