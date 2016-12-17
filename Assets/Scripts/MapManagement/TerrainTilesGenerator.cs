using UnityEngine;
using System.Collections;
using ConstCollections.PJEnums;
using System.Collections.Generic;
using UnityEngine.Events;
using Common.PJMath;

namespace MapManagement
{
  public class TerrainTilesGenerator : MonoBehaviour 
  {
    public MAP_TILE_TYPE MapTileType;
    public FILL_MODE FillMode;

    public GameObject TerrainTilePrefab;
    public List<GameObject> TerrainTileList;
    public MAP_TILE_TYPE[] SpawnReplaceableTypes = {MAP_TILE_TYPE.BASIC, MAP_TILE_TYPE.PLAIN};
    public MAP_TILE_TYPE[] FillReplaceableTypes = {MAP_TILE_TYPE.BASIC, MAP_TILE_TYPE.PLAIN};

    public int TerrainDumpStartPoint = 10;
    public int TerrainDumpMin = 1;
    public int TerrainDumpMax = 3;

    public void Clear()
    {
      if (this.TerrainTileList != null) {
        this.TerrainTileList.ForEach (item => DestroyImmediate (item));
        this.TerrainTileList.Clear ();
        //this.TerrainCellList = null;
      }
    }
      
    public void GenerateTerrain(MapManager mapManager)
    {
      float _startTime = Time.realtimeSinceStartup;

      foreach (var tile in mapManager.BasicTileList) 
      {
        tile.GetComponent<BasicTile>().DumpNumber = 0;
      }
        
      switch (this.FillMode) 
      {
      case FILL_MODE.FULL_FILL:
        GenerateTilesFullFill (mapManager.BasicTileList);
        break;

      case FILL_MODE.RANDOMLY_EXPAND:
        GameObject _baseObj = FindSpawnObject (mapManager);

        GenerateTilesRandomExpandly (_baseObj, this.TerrainDumpStartPoint);
        break;
      }

      float _time = Time.realtimeSinceStartup - _startTime;
      Debug.LogFormat ("Generate {0} Cost time: {1:f6}", this.MapTileType, _time);
    }

    public GameObject FindSpawnObject(MapManager mapManager)
    {
      // Decide a point
      List<Index2D> _spawnIndexList = new List<Index2D> ();
      foreach (var tile in mapManager.BasicTileList) 
      {
        // Only Basic Tile
        if (tile.transform.childCount == 0) 
        {
          BasicTile _basicTile = tile.GetComponent<BasicTile> ();
          _spawnIndexList.Add (new Index2D (_basicTile.Index));
          continue;
        }

        // Has Terrain
        TerrainTile _terrainTile = tile.GetComponentInChildren<TerrainTile> ();
        bool _fondSpawnableType = false;
        foreach (var spawnableType in this.SpawnReplaceableTypes) 
        {
          if (_terrainTile.MapCellType == spawnableType)
            _fondSpawnableType = true;
        }

        if (_fondSpawnableType) 
        {
          BasicTile _basicTile = tile.GetComponent<BasicTile> ();
          _spawnIndexList.Add (new Index2D (_basicTile.Index));
        }

      }

      // FIXME: yang-zhang
      //if(_spawnIndexList.Count == 0)

      int _spawnIndex = Random.Range (0, _spawnIndexList.Count);
      Index2D _mapIndex = _spawnIndexList [_spawnIndex];
      Debug.LogFormat ("{0} : row = {1}, col = {2}", this.MapTileType, _mapIndex.IndexRow, _mapIndex.IndexCol);


      GameObject _baseObj = mapManager.BasicTileList [_mapIndex.IndexRow, _mapIndex.IndexCol];
      return _baseObj;
    }

    public void GenerateTilesFullFill(GameObject[,] basicTileList)
    {
      foreach (var basicTile in basicTileList) 
      {
        GenerateTile (basicTile);
      }
    }

    public void GenerateTilesRandomExpandly(GameObject parentObj, int dumpStart)
    {
      this.Clear ();

      GameObject _root = parentObj;
      Queue<GameObject> _openQueue = new Queue<GameObject> ();
      HashSet<GameObject> _closedList = new HashSet<GameObject> ();
      _root.GetComponent<BasicTile> ().DumpNumber = dumpStart;
      _openQueue.Enqueue (_root);

      while (_openQueue.Count > 0) {
        CheckQueueExpandly (_openQueue, _closedList);
      }
    }

    void CheckQueueExpandly (Queue<GameObject> openQueue, HashSet<GameObject> closedList)
    {
      GameObject _baseObj = openQueue.Dequeue ();
      int _baseDump = _baseObj.GetComponent<BasicTile> ().DumpNumber;
      if (_baseDump <= 0)
        return;


      GenerateTile (_baseObj);

      if (!closedList.Contains (_baseObj))
        closedList.Add (_baseObj);

      BasicTile _parentScript = _baseObj.GetComponent<BasicTile> ();
      if (!_parentScript.HasNeighboursCross ()) 
      {
        return;
      }

      foreach (var _neighbourObj in _parentScript.NeighboursCross) 
      {
        int _dumpStep = Random.Range (this.TerrainDumpMin, this.TerrainDumpMax);
        if (closedList.Contains (_neighbourObj))
          continue;

        int _nextDumper = _baseDump - _dumpStep;
        if (_nextDumper <= 0) {
          _neighbourObj.GetComponent<BasicTile> ().DumpNumber = 0;

          if (!closedList.Contains (_neighbourObj))
            closedList.Add (_neighbourObj);

          continue;
        }

        if (openQueue.Contains (_neighbourObj))
          continue;

        _neighbourObj.GetComponent<BasicTile> ().DumpNumber = _nextDumper;
        openQueue.Enqueue (_neighbourObj);
      }
    }

    void GenerateTile(GameObject basicTile)
    {
      if (basicTile.transform.childCount == 0) 
      {
        GameObject _terrainObj = GameObject.Instantiate (this.TerrainTilePrefab, basicTile.transform, false) as GameObject;
        this.TerrainTileList.Add (_terrainObj);
        return;
      }

      // basicTile.transform.childCount > 0
      Transform _childTrans = basicTile.transform.GetChild (0);
      TerrainTile _terrainTile = _childTrans.GetComponent<TerrainTile> ();
      bool _fondReplaceTarget = false;
      foreach (var replaceType in this.FillReplaceableTypes) 
      {
        if (_terrainTile.MapCellType == replaceType)
          _fondReplaceTarget = true;
      }

      if (_fondReplaceTarget) 
      {
        DestroyImmediate (_childTrans.gameObject);
        GameObject _terrainObj = GameObject.Instantiate (this.TerrainTilePrefab, basicTile.transform, false) as GameObject;
        this.TerrainTileList.Add (_terrainObj);
      }
      /*
      switch (this.TileConflictMode) 
      {
      case TILE_CONFLICT_MODE.REPLACE:


        if (_fondReplaceTarget) 
        {
          DestroyImmediate (_childTrans.gameObject);
          GameObject _terrainObj = GameObject.Instantiate (this.TerrainTilePrefab, basicTile.transform, false) as GameObject;
          this.TerrainTileList.Add (_terrainObj);
        }
        break;

      case TILE_CONFLICT_MODE.SKIP:
        bool _fondTarget = false;
        foreach (var replaceType in this.ReplaceTileTypes) 
        {
          if (_terrainTile.MapCellType == replaceType)
            _fondTarget = true;
        }

        TerrainTile _terrainTile = _childTrans.GetComponent<TerrainTile> ();
        if (_terrainTile.MapCellType == this.MapTileType)
          break;
        
        break;
      }*/
    }

    /*
    public IEnumerator GenerateTerrainCellCoroutine(GameObject parentObj, int dumpStart, int orderInlayer, 
      UnityAction<float, string> onFinished = null , float p0 = 0, string p1 = null)
    {
      GameObject _root = parentObj;
      Queue<GameObject> _openQueue = new Queue<GameObject> ();
      HashSet<GameObject> _closedList = new HashSet<GameObject> ();
      _root.GetComponent<BasicCell> ().DumpNumber = dumpStart;
      _openQueue.Enqueue (_root);

      while (_openQueue.Count > 0) {
        CheckQueue (_openQueue, _closedList, orderInlayer);
        yield return null;
      }

      if(onFinished !=null)
        onFinished.Invoke(p0, p1);

      yield break;
    }
    */

    /*
  IEnumerator CheckQueueCoroutine (Queue<GameObject> openQueue, HashSet<GameObject> closedList, int orderInlayer)
  {
    GameObject _baseObj = openQueue.Dequeue ();
    int _baseDump = _baseObj.GetComponent<BasicCell> ().DumpNumber;
    if (_baseDump <= 0)
      yield break;

    BasicCell _parentScript = _baseObj.GetComponent<BasicCell> ();
    GameObject _seaObj =  GameObject.Instantiate (this.SeaObj, _baseObj.transform, false);
    _seaObj.GetComponent<BasicCell> ().SetOrderInLayer (orderInlayer);

    if (!closedList.Contains (_baseObj))
      closedList.Add (_baseObj);

    this.SeaList.Add (_seaObj);

    if (!_parentScript.HasNeighboursCross ()) {
      yield break;
    }

    foreach (var _neighbourObj in _parentScript.NeighboursCross) 
    {
      int _dumpStep = Random.Range (this.SeaDumpMin, this.SeaDumpMax);
      if (closedList.Contains (_neighbourObj))
        continue;

      int _nextDumper = _baseDump - _dumpStep;
      if (_nextDumper <= 0) {
        _neighbourObj.GetComponent<BasicCell> ().DumpNumber = 0;

        if (!closedList.Contains (_neighbourObj))
          closedList.Add (_neighbourObj);

        continue;
      }

      _neighbourObj.GetComponent<BasicCell> ().DumpNumber = _nextDumper;
      openQueue.Enqueue (_neighbourObj);
    }

    yield break;
  }
  */

  }

  public enum FILL_MODE
  {
    FULL_FILL,
    RANDOMLY_EXPAND
  }
  /*
  public enum SPAWN_CONFLICT_MODE
  {
    REPLACE,
    FIND_BLANK
  }

  public enum FILL_CONFLICT_MODE
  {
    REPLACE,
    SKIP
  }*/




}
