using UnityEngine;
using System.Collections;
using ConstCollections.PJEnums;
using System.Collections.Generic;
using System.Linq;

namespace MapManagement
{
  public class TerrainCell : MonoBehaviour 
  {
    public MAP_CELL_TYPE MapCellType = MAP_CELL_TYPE.BASIC;
    public MAP_CELL_ORDER Order = MAP_CELL_ORDER.TERRAIN;
    public List<GameObject> NeighboursCross;

    public void Start()
    {
      this.basicCell = this.GetComponentInParent<BasicCell> ();
      GetComponent<SpriteRenderer> ().sortingOrder = (int)Order;
      this.NeighboursCross = new List<GameObject> ();
    }

    public void RefreshNeighboursCross()
    {
      this.NeighboursCross.Clear ();
      this.basicCell.NeighboursCross.ForEach (cell => {
        TerrainCell _terrainCell = cell.GetComponentInChildren<TerrainCell>();

        if(_terrainCell == null)
          return;
        
        if(_terrainCell.MapCellType == this.MapCellType)
        {
          this.NeighboursCross.Add(_terrainCell.gameObject);
        }
      });
    }

    BasicCell basicCell;
  }
}

