// Decompiled with JetBrains decompiler
// Type: LogicPortVisualizer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class LogicPortVisualizer : ILogicUIElement, IUniformGridObject
{
  private int cell;
  private LogicPortSpriteType spriteType;

  public LogicPortVisualizer(int cell, LogicPortSpriteType sprite_type)
  {
    this.cell = cell;
    this.spriteType = sprite_type;
  }

  public int GetLogicUICell() => this.cell;

  public Vector2 PosMin() => Vector2.op_Implicit(Grid.CellToPos2D(this.cell));

  public Vector2 PosMax() => Vector2.op_Implicit(Grid.CellToPos2D(this.cell));

  public LogicPortSpriteType GetLogicPortSpriteType() => this.spriteType;
}
