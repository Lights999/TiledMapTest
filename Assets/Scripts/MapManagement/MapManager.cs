using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
using ConstCollections.PJEnums;


public class MapManager : MonoBehaviour {
  public Vector3 OffsetOrigin;
  public MAP_ALIGN_MODE AlignMode;
  MAP_ALIGN_MODE alignModePrev;
  UnityEvent OnGUIChanged;

  public int CellRowNumber;
  public int CellColNumber;
  public int GridSideLength = 1;
  public GameObject BasicObj;
  public GameObject SeaObj;
  public GameObject[,] CellList;
  public List<GameObject> SeaList;

  public int SeaDumpStartPoint = 10;
  public int SeaDumpMin = 1;
  public int SeaDumpMax = 3;
  [Range(0.0F, 2.0F)]
  public float GenerateSeaProcedureSeconds = 0.5F;


	// Use this for initialization
	void Start () {
    InitWithPlain ();
    GenerateSea ();

  }
	
	// Update is called once per frame
	void Update () {
	}

  public void InitWithPlain()
  {
    this.Clear ();
    CellList = new GameObject[CellRowNumber,CellColNumber];
    SeaList = new List<GameObject> ();
    this.AdjustAlign ();

    this.SetBasicCells ();
    this.SetBasicCellsNeighbours ();

    this.SetCellsOffsetPos ();
  }

  public void Clear()
  {
    if (this.SeaList != null) {
      this.SeaList.ForEach (item => DestroyImmediate (item));
      this.SeaList.Clear ();
      this.SeaList = null;
    }

    if (CellList != null) {
      foreach (var cell in CellList) {
        DestroyImmediate (cell);
      }

      CellList = null;
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

  public void SetBasicCells()
  {
    for (int _row = 0; _row < this.CellRowNumber; _row++) {
      for (int _col = 0; _col < this.CellColNumber; _col++) {

        GameObject _basicInstance =  GameObject.Instantiate (this.BasicObj, this.transform) as GameObject;
        this.CellList [_row, _col] = _basicInstance;

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
          GameObject _Top = this.CellList [_row + 1, _col];
          this.CellList [_row, _col].GetComponent<BasicCell>().AddNeighboursCross (_Top);
        }
          
        if (_row - 1 >= 0) {
          GameObject _Bottom = this.CellList [_row - 1, _col];
          this.CellList [_row, _col].GetComponent<BasicCell>().AddNeighboursCross (_Bottom);
        }

        if (_col + 1 < this.CellColNumber) {
          GameObject _Right = this.CellList [_row, _col + 1];
          this.CellList [_row, _col].GetComponent<BasicCell>().AddNeighboursCross (_Right);
        }

        if (_col - 1 >= 0) {
          GameObject _Left = this.CellList [_row, _col - 1];
          this.CellList [_row, _col].GetComponent<BasicCell>().AddNeighboursCross (_Left);
        }
      }
    }
  }

  public void SetCellsOffsetPos()
  {
    foreach (var cell in CellList) {
      cell.GetComponent<BasicCell>().Offset(this.OffsetOrigin);
    }
  }

  public void GenerateSea()
  {
    StopAllCoroutines ();

    this.SeaList.ForEach (obj => {
      GameObject.DestroyImmediate(obj);
    });
    this.SeaList.Clear ();

    foreach (var cell in CellList) {
      cell.GetComponent<BasicCell>().DumpNumber = 0;
    }

    float _startTime = Time.realtimeSinceStartup;

    // Decide a point
    int _row = Random.Range(0, this.CellRowNumber);
    int _col = Random.Range(0, this.CellColNumber);
    //int _dumpStep = 2;
    int _dumpStart = this.SeaDumpStartPoint;
    Debug.LogFormat ("Sea row = {0}, col = {1}", _row, _col);

    GameObject _baseObj = this.CellList [_row, _col];
    int _orderInlayer = _baseObj.GetComponent<BasicCell> ().GetOrderInLayer () + 1;

//    StartCoroutine (GenerateSeaCellCoroutine (_baseObj, _dumpStart, _orderInlayer,
//      this.OutputCostTime, _startTime, "GenerateSea Cost time:"));

    GenerateSeaCell (_baseObj, _dumpStart, _orderInlayer);
    float _time = Time.realtimeSinceStartup - _startTime;
    Debug.LogFormat ("GenerateSea Cost time: {0:f6}", _time);
  }

  public void OutputCostTime(float startTime, string text)
  {
    float _time = Time.realtimeSinceStartup - startTime;
    Debug.LogFormat (text + "{0:f6}", _time);
  }

  public IEnumerator GenerateSeaCellCoroutine(GameObject parentObj, int dumpStart, int orderInlayer, 
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

  public void GenerateSeaCell(GameObject parentObj, int dumpStart, int orderInlayer)
  {
    GameObject _root = parentObj;
    Queue<GameObject> _openQueue = new Queue<GameObject> ();
    HashSet<GameObject> _closedList = new HashSet<GameObject> ();
    _root.GetComponent<BasicCell> ().DumpNumber = dumpStart;
    _openQueue.Enqueue (_root);

    while (_openQueue.Count > 0) {
      CheckQueue (_openQueue, _closedList, orderInlayer);
    }
  }

  void CheckQueue (Queue<GameObject> openQueue, HashSet<GameObject> closedList, int orderInlayer)
  {
    GameObject _baseObj = openQueue.Dequeue ();
    int _baseDump = _baseObj.GetComponent<BasicCell> ().DumpNumber;
    if (_baseDump <= 0)
      return;
    
    GameObject _seaObj =  GameObject.Instantiate (this.SeaObj, _baseObj.transform, false) as GameObject;
    _seaObj.GetComponent<BasicCell> ().SetOrderInLayer (orderInlayer);
    this.SeaList.Add (_seaObj);

    if (!closedList.Contains (_baseObj))
      closedList.Add (_baseObj);

    BasicCell _parentScript = _baseObj.GetComponent<BasicCell> ();
    if (!_parentScript.HasNeighboursCross ()) {
      return;
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

      if (openQueue.Contains (_neighbourObj))
        continue;

      _neighbourObj.GetComponent<BasicCell> ().DumpNumber = _nextDumper;
      openQueue.Enqueue (_neighbourObj);
    }
  }

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
