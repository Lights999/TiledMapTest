using UnityEngine;
using System.Collections;
using ConstCollections.PJEnums;
using System.Collections.Generic;
using System.Linq;

namespace MapManagement
{
  public class TerrainTile : MonoBehaviour 
  {
    public MAP_TILE_TYPE MapCellType = MAP_TILE_TYPE.BASIC;
    public MAP_TILE_ORDER Order = MAP_TILE_ORDER.TERRAIN;
    public List<GameObject> NeighboursCross;

    public void Start()
    {
      this.basicCell = this.GetComponentInParent<BasicTile> ();
      GetComponent<SpriteRenderer> ().sortingOrder = (int)Order;
      this.NeighboursCross = new List<GameObject> ();
    }

    public void RefreshNeighboursCross()
    {
      this.NeighboursCross.Clear ();
      this.basicCell.Neighbours.ForEach (cell => {
        TerrainTile _terrainCell = cell.TileObject.GetComponentInChildren<TerrainTile>();

        if(_terrainCell == null)
          return;
        
        if(_terrainCell.MapCellType == this.MapCellType)
        {
          this.NeighboursCross.Add(_terrainCell.gameObject);
        }
      });
    }

    BasicTile basicCell;
  }
}

