using UnityEngine;
using System.Collections;

namespace Common.PJMath
{
  [System.Serializable]
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

  [System.Serializable]
  public class IntRandomRange
  {
    public int Min;
    public int Max;

    public IntRandomRange(int min, int max)
    {
      this.Min = min;
      this.Max = max;
    }

    public IntRandomRange(IntRandomRange range)
    {
      this.Min = range.Min;
      this.Max = range.Max;
    }

    public int RandomValue()
    {
      int _value = Random.Range (this.Min, this.Max);
      return _value;
    }
  }

  [System.Serializable]
  public class TerrainTileDumpper
  {
    public int StartValue;
    public IntRandomRange DumpRange;

    public TerrainTileDumpper()
    {
      
    }

    public TerrainTileDumpper(int start, IntRandomRange range)
    {
      this.StartValue = start;
      this.DumpRange = range;
    }

    public TerrainTileDumpper(TerrainTileDumpper dummper)
    {
      this.StartValue = dummper.StartValue;
      this.DumpRange = dummper.DumpRange;
    }
  }
    
}

