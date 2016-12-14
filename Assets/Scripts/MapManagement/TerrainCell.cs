using UnityEngine;
using System.Collections;
using ConstCollections.PJEnums;

namespace MapManagement
{
  public class TerrainCell : MonoBehaviour 
  {
    public MAP_CELL_TYPE MapCellType = MAP_CELL_TYPE.BASIC;
    public MAP_CELL_ORDER Order = MAP_CELL_ORDER.TERRAIN;

    public void Start()
    {
      GetComponent<SpriteRenderer> ().sortingOrder = (int)Order;
    }
  }
}

