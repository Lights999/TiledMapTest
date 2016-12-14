using UnityEngine;
using System.Collections;
using ConstCollections.PJEnums;
using System.Collections.Generic;
using UnityEngine.Events;

namespace MapManagement
{
  public class TerrainCellsGenerator : MonoBehaviour 
  {
    public MAP_CELL_TYPE MapCellType;

    public GameObject TerrainCellPrefab;
    public List<GameObject> TerrainCellList;
    public int TerrainDumpStartPoint = 10;
    public int TerrainDumpMin = 1;
    public int TerrainDumpMax = 3;

    public void Clear()
    {
      if (this.TerrainCellList != null) {
        this.TerrainCellList.ForEach (item => DestroyImmediate (item));
        this.TerrainCellList.Clear ();
        //this.TerrainCellList = null;
      }
    }
      

    public void GenerateTerrainCell(GameObject parentObj, int dumpStart)
    {
      this.Clear ();

      GameObject _root = parentObj;
      Queue<GameObject> _openQueue = new Queue<GameObject> ();
      HashSet<GameObject> _closedList = new HashSet<GameObject> ();
      _root.GetComponent<BasicCell> ().DumpNumber = dumpStart;
      _openQueue.Enqueue (_root);

      while (_openQueue.Count > 0) {
        CheckQueue (_openQueue, _closedList);
      }
    }

    public void CheckQueue (Queue<GameObject> openQueue, HashSet<GameObject> closedList)
    {
      GameObject _baseObj = openQueue.Dequeue ();
      int _baseDump = _baseObj.GetComponent<BasicCell> ().DumpNumber;
      if (_baseDump <= 0)
        return;


      if (_baseObj.transform.childCount > 0) 
      {
        Transform _terrainTrans =  _baseObj.transform.GetChild (0);
        //MAP_TILE_CONFLIC_SOLUTION = REPLACE
        DestroyImmediate (_terrainTrans.gameObject);
        GameObject _terrainObj = GameObject.Instantiate (this.TerrainCellPrefab, _baseObj.transform, false) as GameObject;
        this.TerrainCellList.Add (_terrainObj);
      } 
      else 
      {
        GameObject _terrainObj = GameObject.Instantiate (this.TerrainCellPrefab, _baseObj.transform, false) as GameObject;
        this.TerrainCellList.Add (_terrainObj);
      }

      if (!closedList.Contains (_baseObj))
        closedList.Add (_baseObj);

      BasicCell _parentScript = _baseObj.GetComponent<BasicCell> ();
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
          _neighbourObj.GetComponent<BasicCell> ().DumpNumber = 0;

          if (!closedList.Contains (_neighbourObj))
            closedList.Add (_neighbourObj);

          continue;
        }

        if (openQueue.Contains (_neighbourObj))
          continue;

        _neighbourObj.GetComponent<BasicCell> ().DumpNumber = _nextDumper;
        openQueue.Enqueue (_neighbourObj);
      }
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

  public enum MAP_TILE_CONFLICT_SOLUTION
  {
    REPLACE,
    SKIP
  }
}
