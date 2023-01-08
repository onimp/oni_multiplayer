// Decompiled with JetBrains decompiler
// Type: Extents
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

public struct Extents
{
  public int x;
  public int y;
  public int width;
  public int height;

  public static Extents OneCell(int cell)
  {
    int x;
    int y;
    Grid.CellToXY(cell, out x, out y);
    return new Extents(x, y, 1, 1);
  }

  public Extents(int x, int y, int width, int height)
  {
    this.x = x;
    this.y = y;
    this.width = width;
    this.height = height;
  }

  public Extents(int cell, int radius)
  {
    int x = 0;
    int y = 0;
    Grid.CellToXY(cell, out x, out y);
    this.x = x - radius;
    this.y = y - radius;
    this.width = radius * 2 + 1;
    this.height = radius * 2 + 1;
  }

  public Extents(int center_x, int center_y, int radius)
  {
    this.x = center_x - radius;
    this.y = center_y - radius;
    this.width = radius * 2 + 1;
    this.height = radius * 2 + 1;
  }

  public Extents(int cell, CellOffset[] offsets)
  {
    int x1 = 0;
    int y1 = 0;
    Grid.CellToXY(cell, out x1, out y1);
    int val1_1 = x1;
    int val1_2 = y1;
    for (int index = 0; index < offsets.Length; ++index)
    {
      CellOffset offset = offsets[index];
      int x2 = 0;
      int y2 = 0;
      Grid.CellToXY(Grid.OffsetCell(cell, offset), out x2, out y2);
      x1 = Math.Min(x1, x2);
      y1 = Math.Min(y1, y2);
      val1_1 = Math.Max(val1_1, x2);
      val1_2 = Math.Max(val1_2, y2);
    }
    this.x = x1;
    this.y = y1;
    this.width = val1_1 - x1 + 1;
    this.height = val1_2 - y1 + 1;
  }

  public Extents(int cell, CellOffset[] offsets, Orientation orientation)
  {
    int x1 = 0;
    int y1 = 0;
    Grid.CellToXY(cell, out x1, out y1);
    int val1_1 = x1;
    int val1_2 = y1;
    for (int index = 0; index < offsets.Length; ++index)
    {
      CellOffset rotatedCellOffset = Rotatable.GetRotatedCellOffset(offsets[index], orientation);
      int x2 = 0;
      int y2 = 0;
      Grid.CellToXY(Grid.OffsetCell(cell, rotatedCellOffset), out x2, out y2);
      x1 = Math.Min(x1, x2);
      y1 = Math.Min(y1, y2);
      val1_1 = Math.Max(val1_1, x2);
      val1_2 = Math.Max(val1_2, y2);
    }
    this.x = x1;
    this.y = y1;
    this.width = val1_1 - x1 + 1;
    this.height = val1_2 - y1 + 1;
  }

  public Extents(int cell, CellOffset[][] offset_table)
  {
    int x1 = 0;
    int y1 = 0;
    Grid.CellToXY(cell, out x1, out y1);
    int val1_1 = x1;
    int val1_2 = y1;
    for (int index = 0; index < offset_table.Length; ++index)
    {
      CellOffset[] cellOffsetArray = offset_table[index];
      int x2 = 0;
      int y2 = 0;
      Grid.CellToXY(Grid.OffsetCell(cell, cellOffsetArray[0]), out x2, out y2);
      x1 = Math.Min(x1, x2);
      y1 = Math.Min(y1, y2);
      val1_1 = Math.Max(val1_1, x2);
      val1_2 = Math.Max(val1_2, y2);
    }
    this.x = x1;
    this.y = y1;
    this.width = val1_1 - x1 + 1;
    this.height = val1_2 - y1 + 1;
  }

  public bool Contains(Vector2I pos) => this.x <= pos.x && pos.x < this.x + this.width && this.y <= pos.y && pos.y < this.y + this.height;
}
