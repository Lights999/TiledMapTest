using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ConstCollections.PJEnums;
using Common.PJMath;

namespace MapManagement
{
  public class BasicTile : MonoBehaviour {
    public MAP_TILE_ORDER Order = MAP_TILE_ORDER.BASIC;
    public MAP_TILE_TYPE MapCellType = MAP_TILE_TYPE.BASIC;
    public List<NeighbourTile> Neighbours;
    public Vector3 OriginPosition;
    public Vector3 OffsetPosition;
    public Index2D Index;
    public int DumpNumber;

    void Awake()
    {
      this.Neighbours = new List<NeighbourTile> ();
    }

    void OnDrawGizmos() {

      DrawNeighbours ();
    }


    public void Init(int indexRow, int indexCol, int length)
    {
      Index = new Index2D (indexRow, indexCol);
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

    public void AddNeighbour(NeighbourTile neighbourTile)
    {
      this.Neighbours.Add (neighbourTile);
    }

    public bool HasNeighbour()
    {
      return this.Neighbours != null && this.Neighbours.Count > 0;
    }

    void DrawNeighbours()
    {
      Color _oldColor = Gizmos.color;
      Gizmos.color = Color.red;

      foreach (var neighbour in Neighbours) {
        if (neighbour == null)
          continue;

        if (neighbour.TileObject.transform.childCount != 0)
          continue;
        
        Vector3 _line = neighbour.TileObject.transform.position - this.transform.position;
        Vector3 _to = this.transform.position + _line.normalized * _line.magnitude * 0.2f;
        Gizmos.DrawLine (this.transform.position, _to);
      }

      Gizmos.color = _oldColor;
    }

  }
}
