// Decompiled with JetBrains decompiler
// Type: FloodTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class FloodTool : InterfaceTool
{
  public Func<int, bool> floodCriteria;
  public Action<HashSet<int>> paintArea;
  protected Color32 areaColour = Color32.op_Implicit(new Color(0.5f, 0.7f, 0.5f, 0.2f));
  protected int mouseCell = -1;

  public HashSet<int> Flood(int startCell)
  {
    HashSet<int> visited_cells = new HashSet<int>();
    HashSet<int> valid_cells = new HashSet<int>();
    GameUtil.FloodFillConditional(startCell, this.floodCriteria, (ICollection<int>) visited_cells, (ICollection<int>) valid_cells);
    return valid_cells;
  }

  public override void OnLeftClickDown(Vector3 cursor_pos)
  {
    base.OnLeftClickDown(cursor_pos);
    this.paintArea(this.Flood(Grid.PosToCell(cursor_pos)));
  }

  public override void OnMouseMove(Vector3 cursor_pos)
  {
    base.OnMouseMove(cursor_pos);
    this.mouseCell = Grid.PosToCell(cursor_pos);
  }
}
