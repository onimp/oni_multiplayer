// Decompiled with JetBrains decompiler
// Type: NavTable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

public class NavTable
{
  public Action<int, NavType> OnValidCellChanged;
  private short[] NavTypeMasks;
  private short[] ValidCells;

  public NavTable(int cell_count)
  {
    this.ValidCells = new short[cell_count];
    this.NavTypeMasks = new short[11];
    for (short index = 0; index < (short) 11; ++index)
      this.NavTypeMasks[(int) index] = (short) (1 << (int) index);
  }

  public bool IsValid(int cell, NavType nav_type = NavType.Floor) => Grid.IsValidCell(cell) && ((uint) this.NavTypeMasks[(int) nav_type] & (uint) this.ValidCells[cell]) > 0U;

  public void SetValid(int cell, NavType nav_type, bool is_valid)
  {
    short navTypeMask = this.NavTypeMasks[(int) nav_type];
    short validCell = this.ValidCells[cell];
    if (((uint) validCell & (uint) navTypeMask) > 0U == is_valid)
      return;
    this.ValidCells[cell] = !is_valid ? (short) ((int) ~navTypeMask & (int) validCell) : (short) ((int) navTypeMask | (int) validCell);
    if (this.OnValidCellChanged == null)
      return;
    this.OnValidCellChanged(cell, nav_type);
  }
}
