using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using ConstCollections.PJEnums;

namespace MapManagement
{
  public class MapManager : MonoBehaviour {
    public Vector3 OffsetOrigin;
    public MAP_ALIGN_MODE AlignMode;
    MAP_ALIGN_MODE alignModePrev;
    UnityEvent OnGUIChanged;

    public int CellRowNumber;
    public int CellColNumber;
    public int GridSideLength = 1;

    public GameObject BasicCellPrefab;
    public GameObject[,] BasicCellList;

    public MAP_CELL_TYPE[] TerrainGenerateOrder = {
      MAP_CELL_TYPE.PLAIN,
      MAP_CELL_TYPE.SEA
    };

    [Space(10)]
    public GameObject TerrainCellPrefab;
    public List<GameObject> TerrainCellList;
    public int TerrainDumpStartPoint = 10;
    public int TerrainDumpMin = 1;
    public int TerrainDumpMax = 3;

    [Range(0.0F, 2.0F)]
    public float GenerateTerrainProcedureSeconds = 0.5F;

    public TerrainCellsGenerator TCG;


    // Use this for initialization
    void Start () {
      TCG = GetComponent<TerrainCellsGenerator> ();

      InitBasicCell ();
      GenerateTerrain ();
     
    }

    public void InitBasicCell()
    {
      this.Clear ();
      this.BasicCellList = new GameObject[CellRowNumber,CellColNumber];
      this.TerrainCellList = new List<GameObject> ();

      this.AdjustAlign ();

      this.GenerateBasicCells ();
      this.SetBasicCellsNeighbours ();
      this.SetBasicCellsOffsetPos ();
    }

    public void Clear()
    {
      if (this.TerrainCellList != null) {
        this.TerrainCellList.ForEach (item => DestroyImmediate (item));
        this.TerrainCellList.Clear ();
        this.TerrainCellList = null;
      }

      if (BasicCellList != null) {
        foreach (var cell in BasicCellList) {
          DestroyImmediate (cell);
        }

        BasicCellList = null;
      }

      int _count = this.transform.childCount;
      List<GameObject> _checkList = new List<GameObject> ();
      for (int i = 0; i < _count; i++) {
        _checkList.Add (this.transform.GetChild (i).gameObject);
      }

      for (int i = 0; i < _checkList.Count; i++) {
        DestroyImmediate (_checkList [i]);
      }

    }

    public void RoundRowCol()
    {
      if (CellRowNumber < 0)
        CellRowNumber = 0;

      if (CellColNumber < 0)
        CellColNumber = 0;

      if ((CellRowNumber % 2) != 0)
        CellRowNumber--;

      if ((CellColNumber % 2) != 0)
        CellColNumber--;
    }

    public void AdjustAlign()
    {

      switch (this.AlignMode) {
      case MAP_ALIGN_MODE.LEFT_BOTTOM:
        OffsetOrigin = Vector3.zero;
        break;
      case MAP_ALIGN_MODE.CENTER:
        OffsetOrigin = new Vector3 ((((float)-CellColNumber) / 2.0F) * (float)GridSideLength, ((float)-CellRowNumber / 2.0F) * (float)GridSideLength, 0);
        break;
      case MAP_ALIGN_MODE.CUSTOM:
        break;
      }

      this.alignModePrev = this.AlignMode;
    }

    public void GenerateBasicCells()
    {
      for (int _row = 0; _row < this.CellRowNumber; _row++) {
        for (int _col = 0; _col < this.CellColNumber; _col++) {

          GameObject _basicInstance =  GameObject.Instantiate (this.BasicCellPrefab, this.transform) as GameObject;
          this.BasicCellList [_row, _col] = _basicInstance;

          BasicCell _basicCell = _basicInstance.GetComponent<BasicCell> ();
          _basicCell.Init (_row, _col, this.GridSideLength); 

        }
      }
    }

    public void SetBasicCellsNeighbours()
    {
      for (int _row = 0; _row < this.CellRowNumber; _row++) {
        for (int _col = 0; _col < this.CellColNumber; _col++) {

          if (_row + 1 < this.CellRowNumber) {
            GameObject _Top = this.BasicCellList [_row + 1, _col];
            this.BasicCellList [_row, _col].GetComponent<BasicCell>().AddNeighboursCross (_Top);
          }

          if (_row - 1 >= 0) {
            GameObject _Bottom = this.BasicCellList [_row - 1, _col];
            this.BasicCellList [_row, _col].GetComponent<BasicCell>().AddNeighboursCross (_Bottom);
          }

          if (_col + 1 < this.CellColNumber) {
            GameObject _Right = this.BasicCellList [_row, _col + 1];
            this.BasicCellList [_row, _col].GetComponent<BasicCell>().AddNeighboursCross (_Right);
          }

          if (_col - 1 >= 0) {
            GameObject _Left = this.BasicCellList [_row, _col - 1];
            this.BasicCellList [_row, _col].GetComponent<BasicCell>().AddNeighboursCross (_Left);
          }
        }
      }
    }

    public void SetBasicCellsOffsetPos()
    {
      foreach (var cell in BasicCellList) {
        cell.GetComponent<BasicCell>().Offset(this.OffsetOrigin);
      }
    }

    public void GenerateTerrain()
    {
      foreach (var cell in BasicCellList) {
        cell.GetComponent<BasicCell>().DumpNumber = 0;
      }

      // Decide a point
      int _row = Random.Range(0, this.CellRowNumber);
      int _col = Random.Range(0, this.CellColNumber);

      int _dumpStart = this.TerrainDumpStartPoint;
      Debug.LogFormat ("Sea row = {0}, col = {1}", _row, _col);

      GameObject _baseObj = this.BasicCellList [_row, _col];

      float _startTime = Time.realtimeSinceStartup;

      TCG.GenerateTerrainCell(_baseObj, _dumpStart);

      float _time = Time.realtimeSinceStartup - _startTime;
      Debug.LogFormat ("GenerateSea Cost time: {0:f6}", _time);
    }
  }

}

