// Decompiled with JetBrains decompiler
// Type: LogicGateBase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/LogicGateBase")]
public class LogicGateBase : KMonoBehaviour
{
  public static LogicModeUI uiSrcData;
  public static readonly HashedString OUTPUT_TWO_PORT_ID = new HashedString("LogicGateOutputTwo");
  public static readonly HashedString OUTPUT_THREE_PORT_ID = new HashedString("LogicGateOutputThree");
  public static readonly HashedString OUTPUT_FOUR_PORT_ID = new HashedString("LogicGateOutputFour");
  [SerializeField]
  public LogicGateBase.Op op;
  public static CellOffset[] portOffsets = new CellOffset[3]
  {
    CellOffset.none,
    new CellOffset(0, 1),
    new CellOffset(1, 0)
  };
  public CellOffset[] inputPortOffsets;
  public CellOffset[] outputPortOffsets;
  public CellOffset[] controlPortOffsets;

  private int GetActualCell(CellOffset offset)
  {
    Rotatable component = ((Component) this).GetComponent<Rotatable>();
    if (Object.op_Inequality((Object) component, (Object) null))
      offset = component.GetRotatedCellOffset(offset);
    return Grid.OffsetCell(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)), offset);
  }

  public int InputCellOne => this.GetActualCell(this.inputPortOffsets[0]);

  public int InputCellTwo => this.GetActualCell(this.inputPortOffsets[1]);

  public int InputCellThree => this.GetActualCell(this.inputPortOffsets[2]);

  public int InputCellFour => this.GetActualCell(this.inputPortOffsets[3]);

  public int OutputCellOne => this.GetActualCell(this.outputPortOffsets[0]);

  public int OutputCellTwo => this.GetActualCell(this.outputPortOffsets[1]);

  public int OutputCellThree => this.GetActualCell(this.outputPortOffsets[2]);

  public int OutputCellFour => this.GetActualCell(this.outputPortOffsets[3]);

  public int ControlCellOne => this.GetActualCell(this.controlPortOffsets[0]);

  public int ControlCellTwo => this.GetActualCell(this.controlPortOffsets[1]);

  public int PortCell(LogicGateBase.PortId port)
  {
    switch (port)
    {
      case LogicGateBase.PortId.InputOne:
        return this.InputCellOne;
      case LogicGateBase.PortId.InputTwo:
        return this.InputCellTwo;
      case LogicGateBase.PortId.InputThree:
        return this.InputCellThree;
      case LogicGateBase.PortId.InputFour:
        return this.InputCellFour;
      case LogicGateBase.PortId.OutputOne:
        return this.OutputCellOne;
      case LogicGateBase.PortId.OutputTwo:
        return this.OutputCellTwo;
      case LogicGateBase.PortId.OutputThree:
        return this.OutputCellThree;
      case LogicGateBase.PortId.OutputFour:
        return this.OutputCellFour;
      case LogicGateBase.PortId.ControlOne:
        return this.ControlCellOne;
      case LogicGateBase.PortId.ControlTwo:
        return this.ControlCellTwo;
      default:
        return this.OutputCellOne;
    }
  }

  public bool TryGetPortAtCell(int cell, out LogicGateBase.PortId port)
  {
    if (cell == this.InputCellOne)
    {
      port = LogicGateBase.PortId.InputOne;
      return true;
    }
    if ((this.RequiresTwoInputs || this.RequiresFourInputs) && cell == this.InputCellTwo)
    {
      port = LogicGateBase.PortId.InputTwo;
      return true;
    }
    if (this.RequiresFourInputs && cell == this.InputCellThree)
    {
      port = LogicGateBase.PortId.InputThree;
      return true;
    }
    if (this.RequiresFourInputs && cell == this.InputCellFour)
    {
      port = LogicGateBase.PortId.InputFour;
      return true;
    }
    if (cell == this.OutputCellOne)
    {
      port = LogicGateBase.PortId.OutputOne;
      return true;
    }
    if (this.RequiresFourOutputs && cell == this.OutputCellTwo)
    {
      port = LogicGateBase.PortId.OutputTwo;
      return true;
    }
    if (this.RequiresFourOutputs && cell == this.OutputCellThree)
    {
      port = LogicGateBase.PortId.OutputThree;
      return true;
    }
    if (this.RequiresFourOutputs && cell == this.OutputCellFour)
    {
      port = LogicGateBase.PortId.OutputFour;
      return true;
    }
    if (this.RequiresControlInputs && cell == this.ControlCellOne)
    {
      port = LogicGateBase.PortId.ControlOne;
      return true;
    }
    if (this.RequiresControlInputs && cell == this.ControlCellTwo)
    {
      port = LogicGateBase.PortId.ControlTwo;
      return true;
    }
    port = LogicGateBase.PortId.InputOne;
    return false;
  }

  public bool RequiresTwoInputs => LogicGateBase.OpRequiresTwoInputs(this.op);

  public bool RequiresFourInputs => LogicGateBase.OpRequiresFourInputs(this.op);

  public bool RequiresFourOutputs => LogicGateBase.OpRequiresFourOutputs(this.op);

  public bool RequiresControlInputs => LogicGateBase.OpRequiresControlInputs(this.op);

  public static bool OpRequiresTwoInputs(LogicGateBase.Op op)
  {
    switch (op)
    {
      case LogicGateBase.Op.Not:
      case LogicGateBase.Op.CustomSingle:
      case LogicGateBase.Op.Multiplexer:
      case LogicGateBase.Op.Demultiplexer:
        return false;
      default:
        return true;
    }
  }

  public static bool OpRequiresFourInputs(LogicGateBase.Op op) => op == LogicGateBase.Op.Multiplexer;

  public static bool OpRequiresFourOutputs(LogicGateBase.Op op) => op == LogicGateBase.Op.Demultiplexer;

  public static bool OpRequiresControlInputs(LogicGateBase.Op op)
  {
    switch (op)
    {
      case LogicGateBase.Op.Multiplexer:
      case LogicGateBase.Op.Demultiplexer:
        return true;
      default:
        return false;
    }
  }

  public enum PortId
  {
    InputOne,
    InputTwo,
    InputThree,
    InputFour,
    OutputOne,
    OutputTwo,
    OutputThree,
    OutputFour,
    ControlOne,
    ControlTwo,
  }

  public enum Op
  {
    And,
    Or,
    Not,
    Xor,
    CustomSingle,
    Multiplexer,
    Demultiplexer,
  }
}
