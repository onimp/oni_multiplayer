// Decompiled with JetBrains decompiler
// Type: LogicGateMultiplexerConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

public class LogicGateMultiplexerConfig : LogicGateBaseConfig
{
  public const string ID = "LogicGateMultiplexer";

  protected override LogicGateBase.Op GetLogicOp() => LogicGateBase.Op.Multiplexer;

  protected override CellOffset[] InputPortOffsets => new CellOffset[4]
  {
    new CellOffset(-1, 3),
    new CellOffset(-1, 2),
    new CellOffset(-1, 1),
    new CellOffset(-1, 0)
  };

  protected override CellOffset[] OutputPortOffsets => new CellOffset[1]
  {
    new CellOffset(1, 3)
  };

  protected override CellOffset[] ControlPortOffsets => new CellOffset[2]
  {
    new CellOffset(0, 0),
    new CellOffset(1, 0)
  };

  protected override LogicGate.LogicGateDescriptions GetDescriptions() => new LogicGate.LogicGateDescriptions()
  {
    outputOne = new LogicGate.LogicGateDescriptions.Description()
    {
      name = (string) BUILDINGS.PREFABS.LOGICGATEMULTIPLEXER.OUTPUT_NAME,
      active = (string) BUILDINGS.PREFABS.LOGICGATEMULTIPLEXER.OUTPUT_ACTIVE,
      inactive = (string) BUILDINGS.PREFABS.LOGICGATEMULTIPLEXER.OUTPUT_INACTIVE
    }
  };

  public override BuildingDef CreateBuildingDef() => this.CreateBuildingDef("LogicGateMultiplexer", "logic_multiplexer_kanim", 3, 4);
}
