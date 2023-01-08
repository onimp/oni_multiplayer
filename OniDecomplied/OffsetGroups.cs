// Decompiled with JetBrains decompiler
// Type: OffsetGroups
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

public static class OffsetGroups
{
  public static CellOffset[] Use = new CellOffset[1];
  public static CellOffset[] Chat = new CellOffset[6]
  {
    new CellOffset(1, 0),
    new CellOffset(-1, 0),
    new CellOffset(1, 1),
    new CellOffset(1, -1),
    new CellOffset(-1, 1),
    new CellOffset(-1, -1)
  };
  public static CellOffset[] LeftOnly = new CellOffset[1]
  {
    new CellOffset(-1, 0)
  };
  public static CellOffset[] RightOnly = new CellOffset[1]
  {
    new CellOffset(1, 0)
  };
  public static CellOffset[] LeftOrRight = new CellOffset[2]
  {
    new CellOffset(-1, 0),
    new CellOffset(1, 0)
  };
  public static CellOffset[] Standard = OffsetGroups.InitGrid(-2, 2, -3, 3);
  public static CellOffset[] LiquidSource = new CellOffset[11]
  {
    new CellOffset(0, 0),
    new CellOffset(1, 0),
    new CellOffset(-1, 0),
    new CellOffset(0, 1),
    new CellOffset(0, -1),
    new CellOffset(1, 1),
    new CellOffset(1, -1),
    new CellOffset(-1, 1),
    new CellOffset(-1, -1),
    new CellOffset(2, 0),
    new CellOffset(-2, 0)
  };
  public static CellOffset[][] InvertedStandardTable = OffsetTable.Mirror(new CellOffset[28][]
  {
    new CellOffset[1]{ new CellOffset(0, 0) },
    new CellOffset[1]{ new CellOffset(0, 1) },
    new CellOffset[2]
    {
      new CellOffset(0, 2),
      new CellOffset(0, 1)
    },
    new CellOffset[3]
    {
      new CellOffset(0, 3),
      new CellOffset(0, 2),
      new CellOffset(0, 1)
    },
    new CellOffset[1]{ new CellOffset(0, -1) },
    new CellOffset[1]{ new CellOffset(0, -2) },
    new CellOffset[3]
    {
      new CellOffset(0, -3),
      new CellOffset(0, -2),
      new CellOffset(0, -1)
    },
    new CellOffset[1]{ new CellOffset(1, 0) },
    new CellOffset[2]
    {
      new CellOffset(1, 1),
      new CellOffset(0, 1)
    },
    new CellOffset[2]
    {
      new CellOffset(1, 1),
      new CellOffset(1, 0)
    },
    new CellOffset[3]
    {
      new CellOffset(1, 2),
      new CellOffset(1, 1),
      new CellOffset(1, 0)
    },
    new CellOffset[3]
    {
      new CellOffset(1, 2),
      new CellOffset(0, 2),
      new CellOffset(0, 1)
    },
    new CellOffset[4]
    {
      new CellOffset(1, 3),
      new CellOffset(1, 2),
      new CellOffset(1, 1),
      new CellOffset(0, 1)
    },
    new CellOffset[4]
    {
      new CellOffset(1, 3),
      new CellOffset(0, 3),
      new CellOffset(0, 2),
      new CellOffset(0, 1)
    },
    new CellOffset[1]{ new CellOffset(1, -1) },
    new CellOffset[3]
    {
      new CellOffset(1, -2),
      new CellOffset(1, -1),
      new CellOffset(1, 0)
    },
    new CellOffset[3]
    {
      new CellOffset(1, -2),
      new CellOffset(1, -1),
      new CellOffset(0, -1)
    },
    new CellOffset[4]
    {
      new CellOffset(1, -3),
      new CellOffset(1, -2),
      new CellOffset(1, -1),
      new CellOffset(1, 0)
    },
    new CellOffset[4]
    {
      new CellOffset(1, -3),
      new CellOffset(1, -2),
      new CellOffset(0, -2),
      new CellOffset(0, -1)
    },
    new CellOffset[2]
    {
      new CellOffset(2, 0),
      new CellOffset(1, 0)
    },
    new CellOffset[3]
    {
      new CellOffset(2, 1),
      new CellOffset(1, 1),
      new CellOffset(0, 1)
    },
    new CellOffset[3]
    {
      new CellOffset(2, 1),
      new CellOffset(1, 1),
      new CellOffset(1, 0)
    },
    new CellOffset[4]
    {
      new CellOffset(2, 2),
      new CellOffset(1, 2),
      new CellOffset(1, 1),
      new CellOffset(0, 1)
    },
    new CellOffset[4]
    {
      new CellOffset(2, 2),
      new CellOffset(1, 2),
      new CellOffset(1, 1),
      new CellOffset(1, 0)
    },
    new CellOffset[5]
    {
      new CellOffset(2, 3),
      new CellOffset(1, 3),
      new CellOffset(1, 2),
      new CellOffset(1, 1),
      new CellOffset(0, 1)
    },
    new CellOffset[3]
    {
      new CellOffset(2, -1),
      new CellOffset(2, 0),
      new CellOffset(1, 0)
    },
    new CellOffset[4]
    {
      new CellOffset(2, -2),
      new CellOffset(2, -1),
      new CellOffset(1, -1),
      new CellOffset(1, 0)
    },
    new CellOffset[4]
    {
      new CellOffset(2, -3),
      new CellOffset(1, -2),
      new CellOffset(1, -1),
      new CellOffset(1, 0)
    }
  });
  public static CellOffset[][] InvertedStandardTableWithCorners = OffsetTable.Mirror(new CellOffset[24][]
  {
    new CellOffset[1]{ new CellOffset(0, 0) },
    new CellOffset[1]{ new CellOffset(0, 1) },
    new CellOffset[2]
    {
      new CellOffset(0, 2),
      new CellOffset(0, 1)
    },
    new CellOffset[3]
    {
      new CellOffset(0, 3),
      new CellOffset(0, 2),
      new CellOffset(0, 1)
    },
    new CellOffset[1]{ new CellOffset(0, -1) },
    new CellOffset[1]{ new CellOffset(0, -2) },
    new CellOffset[3]
    {
      new CellOffset(0, -3),
      new CellOffset(0, -2),
      new CellOffset(0, -1)
    },
    new CellOffset[1]{ new CellOffset(1, 0) },
    new CellOffset[1]{ new CellOffset(1, 1) },
    new CellOffset[2]
    {
      new CellOffset(1, 2),
      new CellOffset(1, 1)
    },
    new CellOffset[3]
    {
      new CellOffset(1, 2),
      new CellOffset(0, 2),
      new CellOffset(0, 1)
    },
    new CellOffset[3]
    {
      new CellOffset(1, 3),
      new CellOffset(1, 2),
      new CellOffset(1, 1)
    },
    new CellOffset[4]
    {
      new CellOffset(1, 3),
      new CellOffset(0, 3),
      new CellOffset(0, 2),
      new CellOffset(0, 1)
    },
    new CellOffset[1]{ new CellOffset(1, -1) },
    new CellOffset[2]
    {
      new CellOffset(1, -2),
      new CellOffset(1, -1)
    },
    new CellOffset[4]
    {
      new CellOffset(1, -3),
      new CellOffset(1, -2),
      new CellOffset(0, -2),
      new CellOffset(0, -1)
    },
    new CellOffset[3]
    {
      new CellOffset(1, -3),
      new CellOffset(1, -2),
      new CellOffset(1, -1)
    },
    new CellOffset[2]
    {
      new CellOffset(2, 0),
      new CellOffset(1, 0)
    },
    new CellOffset[2]
    {
      new CellOffset(2, 1),
      new CellOffset(1, 1)
    },
    new CellOffset[3]
    {
      new CellOffset(2, 2),
      new CellOffset(1, 2),
      new CellOffset(1, 1)
    },
    new CellOffset[4]
    {
      new CellOffset(2, 3),
      new CellOffset(1, 3),
      new CellOffset(1, 2),
      new CellOffset(1, 1)
    },
    new CellOffset[3]
    {
      new CellOffset(2, -1),
      new CellOffset(2, 0),
      new CellOffset(1, 0)
    },
    new CellOffset[3]
    {
      new CellOffset(2, -2),
      new CellOffset(2, -1),
      new CellOffset(1, -1)
    },
    new CellOffset[3]
    {
      new CellOffset(2, -3),
      new CellOffset(1, -2),
      new CellOffset(1, -1)
    }
  });
  public static CellOffset[][] InvertedWideTable = OffsetTable.Mirror(new CellOffset[33][]
  {
    new CellOffset[1]{ new CellOffset(0, 0) },
    new CellOffset[1]{ new CellOffset(0, 1) },
    new CellOffset[2]
    {
      new CellOffset(0, 2),
      new CellOffset(0, 1)
    },
    new CellOffset[3]
    {
      new CellOffset(0, 3),
      new CellOffset(0, 2),
      new CellOffset(0, 1)
    },
    new CellOffset[1]{ new CellOffset(0, -1) },
    new CellOffset[1]{ new CellOffset(0, -2) },
    new CellOffset[3]
    {
      new CellOffset(0, -3),
      new CellOffset(0, -2),
      new CellOffset(0, -1)
    },
    new CellOffset[1]{ new CellOffset(1, 0) },
    new CellOffset[2]
    {
      new CellOffset(1, 1),
      new CellOffset(0, 1)
    },
    new CellOffset[2]
    {
      new CellOffset(1, 1),
      new CellOffset(1, 0)
    },
    new CellOffset[3]
    {
      new CellOffset(1, 2),
      new CellOffset(1, 1),
      new CellOffset(1, 0)
    },
    new CellOffset[3]
    {
      new CellOffset(1, 2),
      new CellOffset(0, 2),
      new CellOffset(0, 1)
    },
    new CellOffset[4]
    {
      new CellOffset(1, 3),
      new CellOffset(1, 2),
      new CellOffset(1, 1),
      new CellOffset(0, 1)
    },
    new CellOffset[4]
    {
      new CellOffset(1, 3),
      new CellOffset(0, 3),
      new CellOffset(0, 2),
      new CellOffset(0, 1)
    },
    new CellOffset[1]{ new CellOffset(1, -1) },
    new CellOffset[3]
    {
      new CellOffset(1, -2),
      new CellOffset(1, -1),
      new CellOffset(1, 0)
    },
    new CellOffset[3]
    {
      new CellOffset(1, -2),
      new CellOffset(1, -1),
      new CellOffset(0, -1)
    },
    new CellOffset[4]
    {
      new CellOffset(1, -3),
      new CellOffset(1, -2),
      new CellOffset(1, -1),
      new CellOffset(1, 0)
    },
    new CellOffset[4]
    {
      new CellOffset(1, -3),
      new CellOffset(1, -2),
      new CellOffset(0, -2),
      new CellOffset(0, -1)
    },
    new CellOffset[2]
    {
      new CellOffset(2, 0),
      new CellOffset(1, 0)
    },
    new CellOffset[3]
    {
      new CellOffset(2, 1),
      new CellOffset(1, 1),
      new CellOffset(0, 1)
    },
    new CellOffset[3]
    {
      new CellOffset(2, 1),
      new CellOffset(1, 1),
      new CellOffset(1, 0)
    },
    new CellOffset[4]
    {
      new CellOffset(2, 2),
      new CellOffset(1, 2),
      new CellOffset(1, 1),
      new CellOffset(0, 1)
    },
    new CellOffset[4]
    {
      new CellOffset(2, 2),
      new CellOffset(1, 2),
      new CellOffset(1, 1),
      new CellOffset(1, 0)
    },
    new CellOffset[5]
    {
      new CellOffset(2, 3),
      new CellOffset(1, 3),
      new CellOffset(1, 2),
      new CellOffset(1, 1),
      new CellOffset(0, 1)
    },
    new CellOffset[3]
    {
      new CellOffset(2, -1),
      new CellOffset(2, 0),
      new CellOffset(1, 0)
    },
    new CellOffset[4]
    {
      new CellOffset(2, -2),
      new CellOffset(2, -1),
      new CellOffset(1, -1),
      new CellOffset(1, 0)
    },
    new CellOffset[4]
    {
      new CellOffset(2, -3),
      new CellOffset(1, -2),
      new CellOffset(1, -1),
      new CellOffset(1, 0)
    },
    new CellOffset[3]
    {
      new CellOffset(3, 0),
      new CellOffset(2, 0),
      new CellOffset(1, 0)
    },
    new CellOffset[4]
    {
      new CellOffset(3, 1),
      new CellOffset(2, 1),
      new CellOffset(1, 1),
      new CellOffset(0, 1)
    },
    new CellOffset[4]
    {
      new CellOffset(3, 1),
      new CellOffset(2, 1),
      new CellOffset(1, 1),
      new CellOffset(1, 0)
    },
    new CellOffset[4]
    {
      new CellOffset(3, -1),
      new CellOffset(2, -1),
      new CellOffset(1, -1),
      new CellOffset(0, -1)
    },
    new CellOffset[4]
    {
      new CellOffset(3, -1),
      new CellOffset(2, -1),
      new CellOffset(1, -1),
      new CellOffset(1, 0)
    }
  });
  private static Dictionary<CellOffset[], Dictionary<CellOffset[][], Dictionary<CellOffset[], CellOffset[][]>>> reachabilityTableCache = new Dictionary<CellOffset[], Dictionary<CellOffset[][], Dictionary<CellOffset[], CellOffset[][]>>>();
  private static readonly CellOffset[] nullFilter = new CellOffset[0];

  public static CellOffset[] InitGrid(int x0, int x1, int y0, int y1)
  {
    List<CellOffset> cellOffsetList = new List<CellOffset>();
    for (int index1 = y0; index1 <= y1; ++index1)
    {
      for (int index2 = x0; index2 <= x1; ++index2)
        cellOffsetList.Add(new CellOffset(index2, index1));
    }
    CellOffset[] array = cellOffsetList.ToArray();
    Array.Sort<CellOffset>(array, 0, array.Length, (IComparer<CellOffset>) new OffsetGroups.CellOffsetComparer());
    return array;
  }

  public static CellOffset[][] BuildReachabilityTable(
    CellOffset[] area_offsets,
    CellOffset[][] table,
    CellOffset[] filter)
  {
    Dictionary<CellOffset[][], Dictionary<CellOffset[], CellOffset[][]>> dictionary1 = (Dictionary<CellOffset[][], Dictionary<CellOffset[], CellOffset[][]>>) null;
    Dictionary<CellOffset[], CellOffset[][]> dictionary2 = (Dictionary<CellOffset[], CellOffset[][]>) null;
    CellOffset[][] cellOffsetArray1 = (CellOffset[][]) null;
    if (OffsetGroups.reachabilityTableCache.TryGetValue(area_offsets, out dictionary1) && dictionary1.TryGetValue(table, out dictionary2) && dictionary2.TryGetValue(filter == null ? OffsetGroups.nullFilter : filter, out cellOffsetArray1))
      return cellOffsetArray1;
    HashSet<CellOffset> cellOffsetSet = new HashSet<CellOffset>();
    foreach (CellOffset areaOffset in area_offsets)
    {
      foreach (CellOffset[] cellOffsetArray2 in table)
      {
        if (filter == null || Array.IndexOf<CellOffset>(filter, cellOffsetArray2[0]) == -1)
        {
          CellOffset cellOffset = CellOffset.op_Addition(areaOffset, cellOffsetArray2[0]);
          cellOffsetSet.Add(cellOffset);
        }
      }
    }
    List<CellOffset[]> cellOffsetArrayList = new List<CellOffset[]>();
    foreach (CellOffset cellOffset1 in cellOffsetSet)
    {
      CellOffset cellOffset2 = area_offsets[0];
      foreach (CellOffset areaOffset in area_offsets)
      {
        CellOffset cellOffset3 = CellOffset.op_Subtraction(cellOffset1, cellOffset2);
        int offsetDistance1 = ((CellOffset) ref cellOffset3).GetOffsetDistance();
        CellOffset cellOffset4 = CellOffset.op_Subtraction(cellOffset1, areaOffset);
        int offsetDistance2 = ((CellOffset) ref cellOffset4).GetOffsetDistance();
        if (offsetDistance1 > offsetDistance2)
          cellOffset2 = areaOffset;
      }
      foreach (CellOffset[] cellOffsetArray3 in table)
      {
        if ((filter == null || Array.IndexOf<CellOffset>(filter, cellOffsetArray3[0]) == -1) && CellOffset.op_Equality(CellOffset.op_Addition(cellOffsetArray3[0], cellOffset2), cellOffset1))
        {
          CellOffset[] cellOffsetArray4 = new CellOffset[cellOffsetArray3.Length];
          for (int index = 0; index < cellOffsetArray3.Length; ++index)
            cellOffsetArray4[index] = CellOffset.op_Addition(cellOffsetArray3[index], cellOffset2);
          cellOffsetArrayList.Add(cellOffsetArray4);
        }
      }
    }
    CellOffset[][] array = cellOffsetArrayList.ToArray();
    Array.Sort<CellOffset[]>(array, (Comparison<CellOffset[]>) ((x, y) => ((CellOffset) ref x[0]).GetOffsetDistance().CompareTo(((CellOffset) ref y[0]).GetOffsetDistance())));
    if (dictionary1 == null)
    {
      dictionary1 = new Dictionary<CellOffset[][], Dictionary<CellOffset[], CellOffset[][]>>();
      OffsetGroups.reachabilityTableCache.Add(area_offsets, dictionary1);
    }
    if (dictionary2 == null)
    {
      dictionary2 = new Dictionary<CellOffset[], CellOffset[][]>();
      dictionary1.Add(table, dictionary2);
    }
    dictionary2.Add(filter == null ? OffsetGroups.nullFilter : filter, array);
    return array;
  }

  private class CellOffsetComparer : IComparer<CellOffset>
  {
    public int Compare(CellOffset a, CellOffset b) => (Math.Abs(a.x) + Math.Abs(a.y)).CompareTo(Math.Abs(b.x) + Math.Abs(b.y));
  }
}
