using UnityEngine;
using System.Collections;

namespace MapManagement.Editor
{
  //using UnityEditor;

  [UnityEditor.CanEditMultipleObjects]
  [UnityEditor.CustomEditor(typeof(MapManager))]
  public class MapManagerEditor : UnityEditor.Editor 
  {
    void OnEnable()
    {
      script = (MapManager)target;
    }

    public override void OnInspectorGUI() 
    {

      DrawDefaultInspector();

      if (GUI.changed) 
      {
        script.AdjustAlign ();
      }

      using(new GUILayout.HorizontalScope())
      {
        if (GUILayout.Button ("Init")) 
        {
          script.InitWithPlain ();
        }

        if (GUILayout.Button("Clear")) 
        {
          script.Clear();
        }
      }


      if (GUILayout.Button("Generate Sub-Cell")) 
      {
          script.GenerateSea();
      }


    }

    MapManager script;
  }

}
