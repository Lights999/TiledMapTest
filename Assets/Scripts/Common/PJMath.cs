using UnityEngine;
using System.Collections;

namespace Common.PJMath
{
  public struct Index2D 
  {
    public int IndexRow;
    public int IndexCol;

    public Index2D(int row, int col)
    {
      this.IndexRow = row;
      this.IndexCol = col;
    }

    public Index2D(Index2D index)
    {
      this.IndexRow = index.IndexRow;
      this.IndexCol = index.IndexCol;
    }
  }
    
}

