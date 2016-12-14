using UnityEngine;
using System.Collections;

namespace ConstCollections.PJEnums
{
  public enum MAP_ALIGN_MODE {
    LEFT_BOTTOM,
    CENTER,
    CUSTOM,
  }

  public enum MAP_CELL_TYPE
  {
    BASIC = 0,
    PLAIN,
    SEA,
    MOUNTAIN,
    SNOW_MOUNTAIN,
    FOREST,
    DESERT
  }

  public enum MAP_CELL_ORDER
  {
    BASIC = 0,
    TERRAIN,
    TOWN
  }
}

