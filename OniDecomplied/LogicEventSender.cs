// Decompiled with JetBrains decompiler
// Type: LogicEventSender
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

internal class LogicEventSender : 
  ILogicEventSender,
  ILogicNetworkConnection,
  ILogicUIElement,
  IUniformGridObject
{
  private HashedString id;
  private int cell;
  private int logicValue;
  private Action<int> onValueChanged;
  private Action<int, bool> onConnectionChanged;
  private LogicPortSpriteType spriteType;

  public LogicEventSender(
    HashedString id,
    int cell,
    Action<int> on_value_changed,
    Action<int, bool> on_connection_changed,
    LogicPortSpriteType sprite_type)
  {
    this.id = id;
    this.cell = cell;
    this.onValueChanged = on_value_changed;
    this.onConnectionChanged = on_connection_changed;
    this.spriteType = sprite_type;
  }

  public HashedString ID => this.id;

  public int GetLogicCell() => this.cell;

  public int GetLogicValue() => this.logicValue;

  public int GetLogicUICell() => this.GetLogicCell();

  public LogicPortSpriteType GetLogicPortSpriteType() => this.spriteType;

  public Vector2 PosMin() => Vector2.op_Implicit(Grid.CellToPos2D(this.cell));

  public Vector2 PosMax() => Vector2.op_Implicit(Grid.CellToPos2D(this.cell));

  public void SetValue(int value)
  {
    this.logicValue = value;
    this.onValueChanged(value);
  }

  public void LogicTick()
  {
  }

  public void OnLogicNetworkConnectionChanged(bool connected)
  {
    if (this.onConnectionChanged == null)
      return;
    this.onConnectionChanged(this.cell, connected);
  }
}
