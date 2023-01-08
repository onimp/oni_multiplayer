// Decompiled with JetBrains decompiler
// Type: Rendering.World.Tile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace Rendering.World
{
  public struct Tile
  {
    public int Idx;
    public TileCells TileCells;
    public int MaskCount;

    public Tile(int idx, int tile_x, int tile_y, int mask_count)
    {
      this.Idx = idx;
      this.TileCells = new TileCells(tile_x, tile_y);
      this.MaskCount = mask_count;
    }
  }
}
