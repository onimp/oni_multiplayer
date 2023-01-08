// Decompiled with JetBrains decompiler
// Type: LogicGateNotConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

public class LogicGateNotConfig : LogicGateBaseConfig
{
  public const string ID = "LogicGateNOT";

  protected override LogicGateBase.Op GetLogicOp() => LogicGateBase.Op.Not;

  protected override CellOffset[] InputPortOffsets => new CellOffset[1]
  {
    CellOffset.none
  };

  protected override CellOffset[] OutputPortOffsets => new CellOffset[1]
  {
    new CellOffset(1, 0)
  };

  protected override CellOffset[] ControlPortOffsets => (CellOffset[]) null;

  protected override LogicGate.LogicGateDescriptions GetDescriptions() => new LogicGate.LogicGateDescriptions()
  {
    outputOne = new LogicGate.LogicGateDescriptions.Description()
    {
      name = (string) BUILDINGS.PREFABS.LOGICGATENOT.OUTPUT_NAME,
      active = (string) BUILDINGS.PREFABS.LOGICGATENOT.OUTPUT_ACTIVE,
      inactive = (string) BUILDINGS.PREFABS.LOGICGATENOT.OUTPUT_INACTIVE
    }
  };

  public override BuildingDef CreateBuildingDef() => this.CreateBuildingDef("LogicGateNOT", "logic_not_kanim", height: 1);
}
