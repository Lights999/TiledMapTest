using UnityEngine;
using System.Collections;

namespace ConstCollections
{
  public class PJEnums 
  {
    public enum MAP_ALIGN_MODE {
      LEFT_BOTTOM,
      CENTER,
      CUSTOM,
    }

    public enum MAP_CELL_TYPE
    {
      PLAIN = 0,
      SEA,
      MOUNTAIN,
      SNOW_MOUNTAIN,
      FOREST,
      DESERT
    }
  }
}

