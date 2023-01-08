// Decompiled with JetBrains decompiler
// Type: OffsetTable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

public static class OffsetTable
{
  public static CellOffset[][] Mirror(CellOffset[][] table)
  {
    List<CellOffset[]> cellOffsetArrayList = new List<CellOffset[]>();
    foreach (CellOffset[] cellOffsetArray1 in table)
    {
      cellOffsetArrayList.Add(cellOffsetArray1);
      if (cellOffsetArray1[0].x != 0)
      {
        CellOffset[] cellOffsetArray2 = new CellOffset[cellOffsetArray1.Length];
        for (int index = 0; index < cellOffsetArray2.Length; ++index)
        {
          cellOffsetArray2[index] = cellOffsetArray1[index];
          cellOffsetArray2[index].x = -cellOffsetArray2[index].x;
        }
        cellOffsetArrayList.Add(cellOffsetArray2);
      }
    }
    return cellOffsetArrayList.ToArray();
  }
}
