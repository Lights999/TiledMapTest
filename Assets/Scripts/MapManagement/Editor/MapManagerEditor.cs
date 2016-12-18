using UnityEngine;
using System.Collections;
using UnityEditor;
using ConstCollections;
using ConstCollections.PJEnums;

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

    [DrawGizmo (GizmoType.Selected | GizmoType.Active)]
    public static void DrawGizmoGrid(MapManager mapManager, GizmoType gizmoType)
    {
      Color _oldColor = Gizmos.color;
      Gizmos.color = Color.yellow;
      float gridSideLength = mapManager.GridSideLength;

      float _startTime = Time.realtimeSinceStartup;
      float _coastTime = 0.0F;

      //Draw row 
      for (int _row = 0; _row < mapManager.TileRowNumber + 1; _row++) {
        Vector3 _start = new Vector3 (0, _row * gridSideLength, 0);
        Vector3 _end = new Vector3 (mapManager.TileColNumber * gridSideLength, _row * gridSideLength, 0);
        Vector3 _offsetStart = _start + mapManager.Offset; // Convert to offset coordinate
        Vector3 _offsetEnd = _end + mapManager.Offset; // Convert to offset coordinate
        Gizmos.DrawLine (mapManager.transform.position + _offsetStart, mapManager.transform.position + _offsetEnd);

        _coastTime = Time.realtimeSinceStartup - _startTime;
        if (_coastTime > 10.0F)
          throw new System.Exception ("Time out @OnDrawGizmos - Draw row ");
      }

      //Draw col 
      for (int _col = 0; _col < mapManager.TileColNumber + 1; _col++) {
        Vector3 _start = new Vector3 (_col * gridSideLength, 0, 0);
        Vector3 _end = new Vector3 ( _col * gridSideLength, mapManager.TileRowNumber * gridSideLength, 0);
        Vector3 _offsetStart = _start + mapManager.Offset; // Convert to offset coordinate
        Vector3 _offsetEnd = _end + mapManager.Offset; // Convert to offset coordinate
        Gizmos.DrawLine (mapManager.transform.position + _offsetStart, mapManager.transform.position + _offsetEnd);

        _coastTime = Time.realtimeSinceStartup - _startTime;
        if (_coastTime > 10.0F)
          throw new System.Exception ("Time out @OnDrawGizmos - Draw col ");
      }


      _coastTime = Time.realtimeSinceStartup - _startTime;
      if(showGizmoInfo)
        Debug.LogFormat ("DrawGizmoGrid Cost time: {0:f6}", _coastTime);

      Gizmos.color = _oldColor;

    }

    public override void OnInspectorGUI() 
    {

      DrawDefaultInspector();

      if (GUI.changed) 
      {
        Debug.LogWarning ("GUI.changed");
        script.AdjustAlign ();
      }

      if (Application.isPlaying)
        return;
      
      script.GetTerrainCellsGenerators ();

      EditorGUILayout.Separator ();
      GUILayout.TextArea("",GUI.skin.horizontalSlider);

      using(new EditorGUILayout.HorizontalScope())
      {
        if (GUILayout.Button ("Init Basic Tiles")) 
        {
          script.InitBasicCell ();
        }

        if (GUILayout.Button("Clear")) 
        {
          script.Clear();
        }
      }
        
      if (GUILayout.Button("Generate Terrain")) 
      {
        script.GenerateTerrains();
      }

//      this.AlignMode = (MAP_ALIGN_MODE)EditorGUILayout.EnumPopup ("AlignMode",AlignMode);

      showGizmoInfo = GUILayout.Toggle (showGizmoInfo, "Show Gizmo Info");

    }

    void OnDestroy()
    {
      script.Clear();
    }

    MapManager script;

    static bool showGizmoInfo = false;
  }

}
