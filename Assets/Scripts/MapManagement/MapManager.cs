using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using ConstCollections.PJEnums;

namespace MapManagement
{
  public class MapManager : MonoBehaviour {
    public Vector3 Offset;
    public MAP_ALIGN_MODE AlignMode;

    public int CellRowNumber;
    public int CellColNumber;
    public int GridSideLength = 1;

    public GameObject BasicCellPrefab;
    public GameObject[,] BasicCellList;

    public GameObject MapRootObject;
    public string MapRootName = "MapRootObject";

    public MAP_CELL_TYPE[] TerrainGenerateOrder = {
      MAP_CELL_TYPE.PLAIN,
      MAP_CELL_TYPE.SEA
    };
      
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

   

      this.GenerateBasicCells ();
      this.SetBasicCellsNeighbours ();

      this.AdjustAlign ();
      this.SetBasicCellsPosition ();
    }

    public void Clear()
    {
      this.TCG.Clear ();

      if (this.MapRootObject != null) 
      {
        DestroyImmediate (this.MapRootObject);
      }

      this.BasicCellList = null;

      /*
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
      */

    }

    public void AdjustAlign()
    {

      switch (this.AlignMode) {
      case MAP_ALIGN_MODE.LEFT_BOTTOM:
        Offset = Vector3.zero;
        break;
      case MAP_ALIGN_MODE.CENTER:
        Offset = new Vector3 ((((float)-CellColNumber) / 2.0F) * (float)GridSideLength, ((float)-CellRowNumber / 2.0F) * (float)GridSideLength, 0);
        break;
      case MAP_ALIGN_MODE.CUSTOM:
        break;
      }

      this.SetBasicCellsPosition ();
    }

    public void GenerateBasicCells()
    {
      if (this.MapRootObject == null) {
        this.MapRootObject = new GameObject (this.MapRootName);//, this.transform, false) as GameObject;
        this.MapRootObject.transform.SetParent(this.transform);
        this.MapRootObject.transform.localPosition = Vector3.zero;
        this.MapRootObject.transform.localRotation = Quaternion.identity;
      }

      for (int _row = 0; _row < this.CellRowNumber; _row++) {
        for (int _col = 0; _col < this.CellColNumber; _col++) {

          GameObject _basicInstance =  GameObject.Instantiate (this.BasicCellPrefab, this.MapRootObject.transform, false) as GameObject;
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

    public void SetBasicCellsPosition()
    {
      this.MapRootObject.transform.localPosition = this.Offset;
      /*
      foreach (var cell in BasicCellList) {
        cell.GetComponent<BasicCell>().Offset(this.Offset);
      }
      */
    }

    public void GenerateTerrain()
    {
      foreach (var cell in BasicCellList) {
        cell.GetComponent<BasicCell>().DumpNumber = 0;
      }

      // Decide a point
      int _row = Random.Range(0, this.CellRowNumber);
      int _col = Random.Range(0, this.CellColNumber);

      int _dumpStart = this.TCG.TerrainDumpStartPoint;
      Debug.LogFormat ("Sea row = {0}, col = {1}", _row, _col);

      GameObject _baseObj = this.BasicCellList [_row, _col];

      float _startTime = Time.realtimeSinceStartup;

      TCG.GenerateTerrainCell(_baseObj, _dumpStart);

      float _time = Time.realtimeSinceStartup - _startTime;
      Debug.LogFormat ("GenerateSea Cost time: {0:f6}", _time);
    }

    /*
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
    */

  }

}

