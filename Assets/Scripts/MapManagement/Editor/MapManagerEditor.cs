using UnityEngine;
using System.Collections;
using UnityEditor;
using ConstCollections;

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
      for (int _row = 0; _row < mapManager.CellRowNumber + 1; _row++) {
        Vector3 _start = new Vector3 (0, _row * gridSideLength, 0);
        Vector3 _end = new Vector3 (mapManager.CellColNumber * gridSideLength, _row * gridSideLength, 0);
        Vector3 _offsetStart = _start + mapManager.OffsetOrigin; // Convert to offset coordinate
        Vector3 _offsetEnd = _end + mapManager.OffsetOrigin; // Convert to offset coordinate
        Gizmos.DrawLine (mapManager.transform.position + _offsetStart, mapManager.transform.position + _offsetEnd);

        _coastTime = Time.realtimeSinceStartup - _startTime;
        if (_coastTime > 10.0F)
          throw new System.Exception ("Time out @OnDrawGizmos - Draw row ");
      }

      for (int _col = 0; _col < mapManager.CellColNumber + 1; _col++) {
        Vector3 _start = new Vector3 (_col * gridSideLength, 0, 0);
        Vector3 _end = new Vector3 ( _col * gridSideLength, mapManager.CellRowNumber * gridSideLength, 0);
        Vector3 _offsetStart = _start + mapManager.OffsetOrigin; // Convert to offset coordinate
        Vector3 _offsetEnd = _end + mapManager.OffsetOrigin; // Convert to offset coordinate
        Gizmos.DrawLine (mapManager.transform.position + _offsetStart, mapManager.transform.position + _offsetEnd);

        _coastTime = Time.realtimeSinceStartup - _startTime;
        if (_coastTime > 10.0F)
          throw new System.Exception ("Time out @OnDrawGizmos - Draw col ");
      }


      _coastTime = Time.realtimeSinceStartup - _startTime;
      if(showGizmoInfo)
        Debug.LogFormat ("DrawGizmoGrid Cost time: {0:f6}", _coastTime);

      Gizmos.color = _oldColor;

      //EditorUtility.SetDirty (mapManager);

    }

    public override void OnInspectorGUI() 
    {

      DrawDefaultInspector();

      if (GUI.changed) 
      {
        script.AdjustAlign ();
      }

      if (Application.isPlaying)
        return;
      
      GUILayout.TextArea("",GUI.skin.horizontalSlider);

      using(new GUILayout.HorizontalScope())
      {
        if (GUILayout.Button ("Init")) 
        {
          script.InitBasicCell ();
        }

        if (GUILayout.Button("Clear")) 
        {
          script.Clear();
        }
      }
        

      if (GUILayout.Button("Generate Sub-Cell")) 
      {
        script.TCG = script.GetComponent<TerrainCellsGenerator> ();
          script.GenerateTerrain();
      }

      showGizmoInfo = GUILayout.Toggle (showGizmoInfo, "Show Gizmo Info");
      //EditorGUILayout.SelectableLabel ("Select");
    }

    MapManager script;
    static bool showGizmoInfo = false;
  }

}
