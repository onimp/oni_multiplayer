// Decompiled with JetBrains decompiler
// Type: CellOffsetQuery
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class CellOffsetQuery : CellArrayQuery
{
  public CellArrayQuery Reset(int cell, CellOffset[] offsets)
  {
    int[] target_cells = new int[offsets.Length];
    for (int index = 0; index < offsets.Length; ++index)
      target_cells[index] = Grid.OffsetCell(cell, offsets[index]);
    this.Reset(target_cells);
    return (CellArrayQuery) this;
  }
}
