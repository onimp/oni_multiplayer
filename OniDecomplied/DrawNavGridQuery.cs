// Decompiled with JetBrains decompiler
// Type: DrawNavGridQuery
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class DrawNavGridQuery : PathFinderQuery
{
  public DrawNavGridQuery Reset(MinionBrain brain) => this;

  public override bool IsMatch(int cell, int parent_cell, int cost)
  {
    if (parent_cell == Grid.InvalidCell || (int) Grid.WorldIdx[parent_cell] != ClusterManager.Instance.activeWorldId || (int) Grid.WorldIdx[cell] != ClusterManager.Instance.activeWorldId)
      return false;
    GL.Color(Color.white);
    GL.Vertex(Grid.CellToPosCCC(parent_cell, Grid.SceneLayer.Move));
    GL.Vertex(Grid.CellToPosCCC(cell, Grid.SceneLayer.Move));
    return false;
  }
}
