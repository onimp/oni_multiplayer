// Decompiled with JetBrains decompiler
// Type: LogicGateVisualizer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
public class LogicGateVisualizer : LogicGateBase
{
  private List<LogicGateVisualizer.IOVisualizer> visChildren = new List<LogicGateVisualizer.IOVisualizer>();

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.Register();
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    this.Unregister();
  }

  private void Register()
  {
    this.Unregister();
    this.visChildren.Add(new LogicGateVisualizer.IOVisualizer(this.OutputCellOne, false));
    if (this.RequiresFourOutputs)
    {
      this.visChildren.Add(new LogicGateVisualizer.IOVisualizer(this.OutputCellTwo, false));
      this.visChildren.Add(new LogicGateVisualizer.IOVisualizer(this.OutputCellThree, false));
      this.visChildren.Add(new LogicGateVisualizer.IOVisualizer(this.OutputCellFour, false));
    }
    this.visChildren.Add(new LogicGateVisualizer.IOVisualizer(this.InputCellOne, true));
    if (this.RequiresTwoInputs)
      this.visChildren.Add(new LogicGateVisualizer.IOVisualizer(this.InputCellTwo, true));
    else if (this.RequiresFourInputs)
    {
      this.visChildren.Add(new LogicGateVisualizer.IOVisualizer(this.InputCellTwo, true));
      this.visChildren.Add(new LogicGateVisualizer.IOVisualizer(this.InputCellThree, true));
      this.visChildren.Add(new LogicGateVisualizer.IOVisualizer(this.InputCellFour, true));
    }
    if (this.RequiresControlInputs)
    {
      this.visChildren.Add(new LogicGateVisualizer.IOVisualizer(this.ControlCellOne, true));
      this.visChildren.Add(new LogicGateVisualizer.IOVisualizer(this.ControlCellTwo, true));
    }
    LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
    foreach (LogicGateVisualizer.IOVisualizer visChild in this.visChildren)
      logicCircuitManager.AddVisElem((ILogicUIElement) visChild);
  }

  private void Unregister()
  {
    LogicCircuitManager logicCircuitManager = Game.Instance.logicCircuitManager;
    foreach (LogicGateVisualizer.IOVisualizer visChild in this.visChildren)
      logicCircuitManager.RemoveVisElem((ILogicUIElement) visChild);
    this.visChildren.Clear();
  }

  private class IOVisualizer : ILogicUIElement, IUniformGridObject
  {
    private int cell;
    private bool input;

    public IOVisualizer(int cell, bool input)
    {
      this.cell = cell;
      this.input = input;
    }

    public int GetLogicUICell() => this.cell;

    public LogicPortSpriteType GetLogicPortSpriteType() => !this.input ? LogicPortSpriteType.Output : LogicPortSpriteType.Input;

    public Vector2 PosMin() => Vector2.op_Implicit(Grid.CellToPos2D(this.cell));

    public Vector2 PosMax() => this.PosMin();
  }
}
