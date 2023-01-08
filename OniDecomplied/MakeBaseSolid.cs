// Decompiled with JetBrains decompiler
// Type: MakeBaseSolid
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class MakeBaseSolid : 
  GameStateMachine<MakeBaseSolid, MakeBaseSolid.Instance, IStateMachineTarget, MakeBaseSolid.Def>
{
  private const Sim.Cell.Properties floorCellProperties = Sim.Cell.Properties.GasImpermeable | Sim.Cell.Properties.LiquidImpermeable | Sim.Cell.Properties.SolidImpermeable | Sim.Cell.Properties.Opaque | Sim.Cell.Properties.NotifyOnMelt;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.root;
    this.root.Enter(new StateMachine<MakeBaseSolid, MakeBaseSolid.Instance, IStateMachineTarget, MakeBaseSolid.Def>.State.Callback(MakeBaseSolid.ConvertToSolid)).Exit(new StateMachine<MakeBaseSolid, MakeBaseSolid.Instance, IStateMachineTarget, MakeBaseSolid.Def>.State.Callback(MakeBaseSolid.ConvertToVacuum));
  }

  private static void ConvertToSolid(MakeBaseSolid.Instance smi)
  {
    if (Object.op_Equality((Object) smi.buildingComplete, (Object) null))
      return;
    int cell = Grid.PosToCell(smi.gameObject);
    PrimaryElement component1 = smi.GetComponent<PrimaryElement>();
    Building component2 = smi.GetComponent<Building>();
    foreach (CellOffset solidOffset in smi.def.solidOffsets)
    {
      CellOffset rotatedOffset = component2.GetRotatedOffset(solidOffset);
      int num = Grid.OffsetCell(cell, rotatedOffset);
      if (smi.def.occupyFoundationLayer)
      {
        SimMessages.ReplaceAndDisplaceElement(num, component1.ElementID, CellEventLogger.Instance.SimCellOccupierOnSpawn, component1.Mass, component1.Temperature);
        Grid.Objects[num, 9] = smi.gameObject;
      }
      else
        SimMessages.ReplaceAndDisplaceElement(num, SimHashes.Vacuum, CellEventLogger.Instance.SimCellOccupierOnSpawn, 0.0f, 0.0f);
      Grid.Foundation[num] = true;
      Grid.SetSolid(num, true, CellEventLogger.Instance.SimCellOccupierForceSolid);
      SimMessages.SetCellProperties(num, (byte) 103);
      Grid.RenderedByWorld[num] = false;
      World.Instance.OnSolidChanged(num);
      GameScenePartitioner.Instance.TriggerEvent(num, GameScenePartitioner.Instance.solidChangedLayer, (object) null);
    }
  }

  private static void ConvertToVacuum(MakeBaseSolid.Instance smi)
  {
    if (Object.op_Equality((Object) smi.buildingComplete, (Object) null))
      return;
    int cell = Grid.PosToCell(smi.gameObject);
    Building component = smi.GetComponent<Building>();
    foreach (CellOffset solidOffset in smi.def.solidOffsets)
    {
      CellOffset rotatedOffset = component.GetRotatedOffset(solidOffset);
      int num = Grid.OffsetCell(cell, rotatedOffset);
      SimMessages.ReplaceAndDisplaceElement(num, SimHashes.Vacuum, CellEventLogger.Instance.SimCellOccupierOnSpawn, 0.0f);
      Grid.Objects[num, 9] = (GameObject) null;
      Grid.Foundation[num] = false;
      Grid.SetSolid(num, false, CellEventLogger.Instance.SimCellOccupierDestroy);
      SimMessages.ClearCellProperties(num, (byte) 103);
      Grid.RenderedByWorld[num] = true;
      World.Instance.OnSolidChanged(num);
      GameScenePartitioner.Instance.TriggerEvent(num, GameScenePartitioner.Instance.solidChangedLayer, (object) null);
    }
  }

  public class Def : StateMachine.BaseDef
  {
    public CellOffset[] solidOffsets;
    public bool occupyFoundationLayer = true;
  }

  public new class Instance : 
    GameStateMachine<MakeBaseSolid, MakeBaseSolid.Instance, IStateMachineTarget, MakeBaseSolid.Def>.GameInstance
  {
    [MyCmpGet]
    public BuildingComplete buildingComplete;

    public Instance(IStateMachineTarget master, MakeBaseSolid.Def def)
      : base(master, def)
    {
    }
  }
}
