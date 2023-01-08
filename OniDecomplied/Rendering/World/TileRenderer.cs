// Decompiled with JetBrains decompiler
// Type: Rendering.World.TileRenderer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

namespace Rendering.World
{
  public abstract class TileRenderer : KMonoBehaviour
  {
    private Tile[] TileGrid;
    private int[] BrushGrid;
    protected int TileGridWidth;
    protected int TileGridHeight;
    private int[] CellTiles = new int[4];
    protected Brush[] Brushes;
    protected Mask[] Masks;
    protected List<Brush> DirtyBrushes = new List<Brush>();
    protected List<Brush> ActiveBrushes = new List<Brush>();
    private VisibleAreaUpdater VisibleAreaUpdater;
    private HashSet<int> ClearTiles = new HashSet<int>();
    private HashSet<int> DirtyTiles = new HashSet<int>();
    public TextureAtlas Atlas;

    protected virtual void OnSpawn()
    {
      this.Masks = this.GetMasks();
      this.TileGridWidth = Grid.WidthInCells + 1;
      this.TileGridHeight = Grid.HeightInCells + 1;
      this.BrushGrid = new int[this.TileGridWidth * this.TileGridHeight * 4];
      for (int index = 0; index < this.BrushGrid.Length; ++index)
        this.BrushGrid[index] = -1;
      this.TileGrid = new Tile[this.TileGridWidth * this.TileGridHeight];
      for (int idx = 0; idx < this.TileGrid.Length; ++idx)
      {
        int tile_x = idx % this.TileGridWidth;
        int tile_y = idx / this.TileGridWidth;
        this.TileGrid[idx] = new Tile(idx, tile_x, tile_y, this.Masks.Length);
      }
      this.LoadBrushes();
      this.VisibleAreaUpdater = new VisibleAreaUpdater(new Action<int>(this.UpdateOutsideView), new Action<int>(this.UpdateInsideView), nameof (TileRenderer));
    }

    protected virtual Mask[] GetMasks() => new Mask[16]
    {
      new Mask(this.Atlas, 0, false, false, false, false),
      new Mask(this.Atlas, 2, false, false, true, false),
      new Mask(this.Atlas, 2, false, true, true, false),
      new Mask(this.Atlas, 1, false, false, true, false),
      new Mask(this.Atlas, 2, false, false, false, false),
      new Mask(this.Atlas, 1, true, false, false, false),
      new Mask(this.Atlas, 3, false, false, false, false),
      new Mask(this.Atlas, 4, false, false, true, false),
      new Mask(this.Atlas, 2, false, true, false, false),
      new Mask(this.Atlas, 3, true, false, false, false),
      new Mask(this.Atlas, 1, true, false, true, false),
      new Mask(this.Atlas, 4, false, true, true, false),
      new Mask(this.Atlas, 1, false, false, false, false),
      new Mask(this.Atlas, 4, false, false, false, false),
      new Mask(this.Atlas, 4, false, true, false, false),
      new Mask(this.Atlas, 0, false, false, false, true)
    };

    private void UpdateInsideView(int cell)
    {
      foreach (int cellTile in this.GetCellTiles(cell))
      {
        this.ClearTiles.Add(cellTile);
        this.DirtyTiles.Add(cellTile);
      }
    }

    private void UpdateOutsideView(int cell)
    {
      foreach (int cellTile in this.GetCellTiles(cell))
        this.ClearTiles.Add(cellTile);
    }

    private int[] GetCellTiles(int cell)
    {
      int x = 0;
      int y = 0;
      Grid.CellToXY(cell, out x, out y);
      this.CellTiles[0] = y * this.TileGridWidth + x;
      this.CellTiles[1] = y * this.TileGridWidth + (x + 1);
      this.CellTiles[2] = (y + 1) * this.TileGridWidth + x;
      this.CellTiles[3] = (y + 1) * this.TileGridWidth + (x + 1);
      return this.CellTiles;
    }

    public abstract void LoadBrushes();

    public void MarkDirty(int cell) => this.VisibleAreaUpdater.UpdateCell(cell);

    private void LateUpdate()
    {
      foreach (int clearTile in this.ClearTiles)
        this.Clear(ref this.TileGrid[clearTile], this.Brushes, this.BrushGrid);
      this.ClearTiles.Clear();
      foreach (int dirtyTile in this.DirtyTiles)
        this.MarkDirty(ref this.TileGrid[dirtyTile], this.Brushes, this.BrushGrid);
      this.DirtyTiles.Clear();
      this.VisibleAreaUpdater.Update();
      foreach (Brush dirtyBrush in this.DirtyBrushes)
        dirtyBrush.Refresh();
      this.DirtyBrushes.Clear();
      foreach (Brush activeBrush in this.ActiveBrushes)
        activeBrush.Render();
    }

    public abstract void MarkDirty(ref Tile tile, Brush[] brush_array, int[] brush_grid);

    public void Clear(ref Tile tile, Brush[] brush_array, int[] brush_grid)
    {
      for (int index1 = 0; index1 < 4; ++index1)
      {
        int index2 = tile.Idx * 4 + index1;
        if (brush_grid[index2] != -1)
          brush_array[brush_grid[index2]].Remove(tile.Idx);
      }
    }
  }
}
