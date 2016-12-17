using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using ConstCollections.PJEnums;

namespace MapManagement
{
  public class MapManager : MonoBehaviour {
    public GameObject MapRootObject;
    public string MapRootName = "MapRootObject";
    public Vector3 Offset;
    public MAP_ALIGN_MODE AlignMode;

    public GameObject BasicTilePrefab;
    public GameObject[,] BasicTileList;
    public int TileRowNumber;
    public int TileColNumber;
    public int GridSideLength = 1;

    public TerrainTilesGenerator[] TCGArray;


    // Use this for initialization
    void Start () {


      InitBasicCell ();
      GenerateTerrains ();
     
    }

    public void GetTerrainCellsGenerators()
    {
      this.TCGArray = GetComponentsInChildren<TerrainTilesGenerator> ();
    }

    public void InitBasicCell()
    {
      this.Clear ();
      this.BasicTileList = new GameObject[TileRowNumber,TileColNumber];

      this.GenerateBasicTiles ();
      this.SetBasicTilesNeighbours ();

      this.AdjustAlign ();
      //this.SetBasicTilesPosition ();
    }

    public void Clear()
    {
      if (this.TCGArray != null) {
        for (int i = 0; i < this.TCGArray.Length; i++) {
          this.TCGArray [i].Clear();
          //this.GenerateTerrain (this.TCGArray [i]);
        }
      }




      if (this.MapRootObject != null) 
      {
        DestroyImmediate (this.MapRootObject);
      }

      this.BasicTileList = null;
    }

    public void AdjustAlign()
    {

      switch (this.AlignMode) {
      case MAP_ALIGN_MODE.LEFT_BOTTOM:
        Offset = Vector3.zero;
        break;
      case MAP_ALIGN_MODE.CENTER:
        Offset = new Vector3 ((((float)-TileColNumber) / 2.0F) * (float)GridSideLength, ((float)-TileRowNumber / 2.0F) * (float)GridSideLength, 0);
        break;
      case MAP_ALIGN_MODE.CUSTOM:
        break;
      }

      this.SetBasicTilesPosition ();
    }

    public void GenerateBasicTiles()
    {
      if (this.MapRootObject == null) {
        this.MapRootObject = new GameObject (this.MapRootName);//, this.transform, false) as GameObject;
        this.MapRootObject.transform.SetParent(this.transform);
        this.MapRootObject.transform.localPosition = Vector3.zero;
        this.MapRootObject.transform.localRotation = Quaternion.identity;
      }

      for (int _row = 0; _row < this.TileRowNumber; _row++) {
        for (int _col = 0; _col < this.TileColNumber; _col++) {

          GameObject _basicInstance =  GameObject.Instantiate (this.BasicTilePrefab, this.MapRootObject.transform, false) as GameObject;
          this.BasicTileList [_row, _col] = _basicInstance;

          BasicTile _basicCell = _basicInstance.GetComponent<BasicTile> ();
          _basicCell.Init (_row, _col, this.GridSideLength); 

        }
      }

    }

    public void SetBasicTilesNeighbours()
    {
      for (int _row = 0; _row < this.TileRowNumber; _row++) {
        for (int _col = 0; _col < this.TileColNumber; _col++) {

          if (_row + 1 < this.TileRowNumber) {
            GameObject _Top = this.BasicTileList [_row + 1, _col];
            this.BasicTileList [_row, _col].GetComponent<BasicTile>().AddNeighboursCross (_Top);
          }

          if (_row - 1 >= 0) {
            GameObject _Bottom = this.BasicTileList [_row - 1, _col];
            this.BasicTileList [_row, _col].GetComponent<BasicTile>().AddNeighboursCross (_Bottom);
          }

          if (_col + 1 < this.TileColNumber) {
            GameObject _Right = this.BasicTileList [_row, _col + 1];
            this.BasicTileList [_row, _col].GetComponent<BasicTile>().AddNeighboursCross (_Right);
          }

          if (_col - 1 >= 0) {
            GameObject _Left = this.BasicTileList [_row, _col - 1];
            this.BasicTileList [_row, _col].GetComponent<BasicTile>().AddNeighboursCross (_Left);
          }
        }
      }
    }

    public void SetBasicTilesPosition()
    {
      if(this.MapRootObject != null)
        this.MapRootObject.transform.localPosition = this.Offset;
    }

    public void GenerateTerrains()
    {
      if (this.TCGArray == null)
        return;
      
      for (int i = 0; i < this.TCGArray.Length; i++) {
        this.TCGArray [i].GenerateTerrain (this);
        //this.GenerateTerrain (this.TCGArray [i]);
      }
    }

    public void GenerateTerrain(TerrainTilesGenerator tcg)
    {
      foreach (var cell in BasicTileList) {
        cell.GetComponent<BasicTile>().DumpNumber = 0;
      }

      // Decide a point
      int _row = Random.Range(0, this.TileRowNumber);
      int _col = Random.Range(0, this.TileColNumber);

      int _dumpStart = tcg.TerrainDumpStartPoint;
      Debug.LogFormat ("Sea row = {0}, col = {1}", _row, _col);

      GameObject _baseObj = this.BasicTileList [_row, _col];

      float _startTime = Time.realtimeSinceStartup;

      tcg.GenerateTilesRandomExpandly(_baseObj, _dumpStart);

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

