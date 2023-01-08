// Decompiled with JetBrains decompiler
// Type: TileVisualizer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class TileVisualizer
{
  private static void RefreshCellInternal(int cell, ObjectLayer tile_layer)
  {
    if (Game.IsQuitting() || !Grid.IsValidCell(cell))
      return;
    GameObject gameObject = Grid.Objects[cell, (int) tile_layer];
    if (!Object.op_Inequality((Object) gameObject, (Object) null))
      return;
    World.Instance.blockTileRenderer.Rebuild(tile_layer, cell);
    KAnimGraphTileVisualizer componentInChildren = gameObject.GetComponentInChildren<KAnimGraphTileVisualizer>();
    if (!Object.op_Inequality((Object) componentInChildren, (Object) null))
      return;
    componentInChildren.Refresh();
  }

  private static void RefreshCell(int cell, ObjectLayer tile_layer)
  {
    if (tile_layer == ObjectLayer.NumLayers)
      return;
    TileVisualizer.RefreshCellInternal(cell, tile_layer);
    TileVisualizer.RefreshCellInternal(Grid.CellAbove(cell), tile_layer);
    TileVisualizer.RefreshCellInternal(Grid.CellBelow(cell), tile_layer);
    TileVisualizer.RefreshCellInternal(Grid.CellLeft(cell), tile_layer);
    TileVisualizer.RefreshCellInternal(Grid.CellRight(cell), tile_layer);
  }

  public static void RefreshCell(int cell, ObjectLayer tile_layer, ObjectLayer replacement_layer)
  {
    TileVisualizer.RefreshCell(cell, tile_layer);
    TileVisualizer.RefreshCell(cell, replacement_layer);
  }
}
