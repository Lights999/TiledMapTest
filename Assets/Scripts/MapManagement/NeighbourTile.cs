using UnityEngine;
using System.Collections;

namespace MapManagement
{
  public enum TILE_DIRECTION
  {
    UP = 0,
    DOWN,
    LEFT,
    RIGHT,
    LEFT_UP,
    RIGHT_UP,
    LEFT_DOWN,
    RIGHT_DOWN
  }

  [System.Serializable]
  public class NeighbourTile 
  {
    public TILE_DIRECTION Direction;
    public GameObject TileObject;

    public NeighbourTile(){}

    public NeighbourTile(TILE_DIRECTION direction, GameObject tileObject)
    {
      Direction = direction;
      TileObject = tileObject;
    }

    public NeighbourTile(NeighbourTile tile)
    {
      Direction = tile.Direction;
      TileObject = tile.TileObject;
    }
  }
}

