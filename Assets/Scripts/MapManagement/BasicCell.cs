using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstCollections.PJEnums;

namespace MapManagement
{
  public class BasicCell : MonoBehaviour {

    public MAP_CELL_TYPE MapCellType = MAP_CELL_TYPE.BASIC;
    public List<GameObject> NeighboursCross;
    public Vector3 OriginPosition;
    public Vector3 OffsetPosition;
    public int DumpNumber;

    void Awake()
    {
      this.NeighboursCross = new List<GameObject> ();
    }

    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update () {

    }

    void OnDrawGizmos() {

      DrawNeighboursCross ();
    }


    public void Init(int indexRow, int indexCol, int length)
    {
      Vector3 _pos = Vector3.zero;
      _pos.x = indexCol * length + (float)length/2.0F;// anchor is sprite's center
      _pos.y = indexRow * length + (float)length/2.0F;// anchor is sprite's center

      OriginPosition = _pos;
      this.transform.localPosition = _pos;
    }

    public void Offset(Vector3 offsetPos)
    {
      this.OffsetPosition = offsetPos;
      this.transform.localPosition = this.OriginPosition + this.OffsetPosition;
    }

    public void AddNeighboursCross(GameObject obj)
    {
      this.NeighboursCross.Add (obj);
    }

    public bool HasNeighboursCross()
    {
      return this.NeighboursCross != null && this.NeighboursCross.Count > 0;
    }

    public int GetOrderInLayer()
    {
      return this.GetComponent<SpriteRenderer> ().sortingOrder;
    }

    public int SetOrderInLayer(int order)
    {
      return this.GetComponent<SpriteRenderer> ().sortingOrder = order;
    }

    void DrawNeighboursCross()
    {
      Color _oldColor = Gizmos.color;
      Gizmos.color = Color.red;

      foreach (var item in NeighboursCross) {
        if (item == null)
          continue;
        Vector3 _line = item.transform.position - this.transform.position;
        Vector3 _to = this.transform.position + _line.normalized * _line.magnitude * 0.2f;
        Gizmos.DrawLine (this.transform.position, _to);
      }

      Gizmos.color = _oldColor;
    }

  }
}
