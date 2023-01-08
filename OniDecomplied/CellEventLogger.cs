// Decompiled with JetBrains decompiler
// Type: CellEventLogger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using System.Diagnostics;

public class CellEventLogger : EventLogger<CellEventInstance, CellEvent>
{
  public static CellEventLogger Instance;
  public CellSolidEvent SimMessagesSolid;
  public CellSolidEvent SimCellOccupierDestroy;
  public CellSolidEvent SimCellOccupierForceSolid;
  public CellSolidEvent SimCellOccupierSolidChanged;
  public CellElementEvent DoorOpen;
  public CellElementEvent DoorClose;
  public CellElementEvent Excavator;
  public CellElementEvent DebugTool;
  public CellElementEvent SandBoxTool;
  public CellElementEvent TemplateLoader;
  public CellElementEvent Scenario;
  public CellElementEvent SimCellOccupierOnSpawn;
  public CellElementEvent SimCellOccupierDestroySelf;
  public CellElementEvent WorldGapManager;
  public CellElementEvent ReceiveElementChanged;
  public CellElementEvent ObjectSetSimOnSpawn;
  public CellElementEvent DecompositionDirtyWater;
  public CellElementEvent LaunchpadDesolidify;
  public CellCallbackEvent SendCallback;
  public CellCallbackEvent ReceiveCallback;
  public CellDigEvent Dig;
  public CellAddRemoveSubstanceEvent WorldDamageDelayedSpawnFX;
  public CellAddRemoveSubstanceEvent SublimatesEmit;
  public CellAddRemoveSubstanceEvent OxygenModifierSimUpdate;
  public CellAddRemoveSubstanceEvent LiquidChunkOnStore;
  public CellAddRemoveSubstanceEvent FallingWaterAddToSim;
  public CellAddRemoveSubstanceEvent ExploderOnSpawn;
  public CellAddRemoveSubstanceEvent ExhaustSimUpdate;
  public CellAddRemoveSubstanceEvent ElementConsumerSimUpdate;
  public CellAddRemoveSubstanceEvent ElementChunkTransition;
  public CellAddRemoveSubstanceEvent OxyrockEmit;
  public CellAddRemoveSubstanceEvent BleachstoneEmit;
  public CellAddRemoveSubstanceEvent UnstableGround;
  public CellAddRemoveSubstanceEvent ConduitFlowEmptyConduit;
  public CellAddRemoveSubstanceEvent ConduitConsumerWrongElement;
  public CellAddRemoveSubstanceEvent OverheatableMeltingDown;
  public CellAddRemoveSubstanceEvent FabricatorProduceMelted;
  public CellAddRemoveSubstanceEvent PumpSimUpdate;
  public CellAddRemoveSubstanceEvent WallPumpSimUpdate;
  public CellAddRemoveSubstanceEvent Vomit;
  public CellAddRemoveSubstanceEvent Tears;
  public CellAddRemoveSubstanceEvent Pee;
  public CellAddRemoveSubstanceEvent AlgaeHabitat;
  public CellAddRemoveSubstanceEvent CO2FilterOxygen;
  public CellAddRemoveSubstanceEvent ToiletEmit;
  public CellAddRemoveSubstanceEvent ElementEmitted;
  public CellAddRemoveSubstanceEvent Mop;
  public CellAddRemoveSubstanceEvent OreMelted;
  public CellAddRemoveSubstanceEvent ConstructTile;
  public CellAddRemoveSubstanceEvent Dumpable;
  public CellAddRemoveSubstanceEvent Cough;
  public CellAddRemoveSubstanceEvent Meteor;
  public CellModifyMassEvent CO2ManagerFixedUpdate;
  public CellModifyMassEvent EnvironmentConsumerFixedUpdate;
  public CellModifyMassEvent ExcavatorShockwave;
  public CellModifyMassEvent OxygenBreatherSimUpdate;
  public CellModifyMassEvent CO2ScrubberSimUpdate;
  public CellModifyMassEvent RiverSourceSimUpdate;
  public CellModifyMassEvent RiverTerminusSimUpdate;
  public CellModifyMassEvent DebugToolModifyMass;
  public CellModifyMassEvent EnergyGeneratorModifyMass;
  public CellSolidFilterEvent SolidFilterEvent;
  public Dictionary<int, int> CallbackToCellMap = new Dictionary<int, int>();

  public static void DestroyInstance() => CellEventLogger.Instance = (CellEventLogger) null;

  [Conditional("ENABLE_CELL_EVENT_LOGGER")]
  public void LogCallbackSend(int cell, int callback_id)
  {
    if (callback_id == -1)
      return;
    this.CallbackToCellMap[callback_id] = cell;
  }

  [Conditional("ENABLE_CELL_EVENT_LOGGER")]
  public void LogCallbackReceive(int callback_id)
  {
    int invalidCell = Grid.InvalidCell;
    this.CallbackToCellMap.TryGetValue(callback_id, out invalidCell);
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    CellEventLogger.Instance = this;
    this.SimMessagesSolid = this.AddEvent((CellEvent) new CellSolidEvent("SimMessageSolid", "Sim Message", false)) as CellSolidEvent;
    this.SimCellOccupierDestroy = this.AddEvent((CellEvent) new CellSolidEvent("SimCellOccupierClearSolid", "Sim Cell Occupier Destroy", false)) as CellSolidEvent;
    this.SimCellOccupierForceSolid = this.AddEvent((CellEvent) new CellSolidEvent("SimCellOccupierForceSolid", "Sim Cell Occupier Force Solid", false)) as CellSolidEvent;
    this.SimCellOccupierSolidChanged = this.AddEvent((CellEvent) new CellSolidEvent("SimCellOccupierSolidChanged", "Sim Cell Occupier Solid Changed", false)) as CellSolidEvent;
    this.DoorOpen = this.AddEvent((CellEvent) new CellElementEvent("DoorOpen", "Door Open", true)) as CellElementEvent;
    this.DoorClose = this.AddEvent((CellEvent) new CellElementEvent("DoorClose", "Door Close", true)) as CellElementEvent;
    this.Excavator = this.AddEvent((CellEvent) new CellElementEvent("Excavator", "Excavator", true)) as CellElementEvent;
    this.DebugTool = this.AddEvent((CellEvent) new CellElementEvent("DebugTool", "Debug Tool", true)) as CellElementEvent;
    this.SandBoxTool = this.AddEvent((CellEvent) new CellElementEvent("SandBoxTool", "Sandbox Tool", true)) as CellElementEvent;
    this.TemplateLoader = this.AddEvent((CellEvent) new CellElementEvent("TemplateLoader", "Template Loader", true)) as CellElementEvent;
    this.Scenario = this.AddEvent((CellEvent) new CellElementEvent("Scenario", "Scenario", true)) as CellElementEvent;
    this.SimCellOccupierOnSpawn = this.AddEvent((CellEvent) new CellElementEvent("SimCellOccupierOnSpawn", "Sim Cell Occupier OnSpawn", true)) as CellElementEvent;
    this.SimCellOccupierDestroySelf = this.AddEvent((CellEvent) new CellElementEvent("SimCellOccupierDestroySelf", "Sim Cell Occupier Destroy Self", true)) as CellElementEvent;
    this.WorldGapManager = this.AddEvent((CellEvent) new CellElementEvent("WorldGapManager", "World Gap Manager", true)) as CellElementEvent;
    this.ReceiveElementChanged = this.AddEvent((CellEvent) new CellElementEvent("ReceiveElementChanged", "Sim Message", false, false)) as CellElementEvent;
    this.ObjectSetSimOnSpawn = this.AddEvent((CellEvent) new CellElementEvent("ObjectSetSimOnSpawn", "Object set sim on spawn", true)) as CellElementEvent;
    this.DecompositionDirtyWater = this.AddEvent((CellEvent) new CellElementEvent("DecompositionDirtyWater", "Decomposition dirty water", true)) as CellElementEvent;
    this.SendCallback = this.AddEvent((CellEvent) new CellCallbackEvent("SendCallback", true)) as CellCallbackEvent;
    this.ReceiveCallback = this.AddEvent((CellEvent) new CellCallbackEvent("ReceiveCallback", false)) as CellCallbackEvent;
    this.Dig = this.AddEvent((CellEvent) new CellDigEvent()) as CellDigEvent;
    this.WorldDamageDelayedSpawnFX = this.AddEvent((CellEvent) new CellAddRemoveSubstanceEvent("WorldDamageDelayedSpawnFX", "World Damage Delayed Spawn FX")) as CellAddRemoveSubstanceEvent;
    this.OxygenModifierSimUpdate = this.AddEvent((CellEvent) new CellAddRemoveSubstanceEvent("OxygenModifierSimUpdate", "Oxygen Modifier SimUpdate")) as CellAddRemoveSubstanceEvent;
    this.LiquidChunkOnStore = this.AddEvent((CellEvent) new CellAddRemoveSubstanceEvent("LiquidChunkOnStore", "Liquid Chunk On Store")) as CellAddRemoveSubstanceEvent;
    this.FallingWaterAddToSim = this.AddEvent((CellEvent) new CellAddRemoveSubstanceEvent("FallingWaterAddToSim", "Falling Water Add To Sim")) as CellAddRemoveSubstanceEvent;
    this.ExploderOnSpawn = this.AddEvent((CellEvent) new CellAddRemoveSubstanceEvent("ExploderOnSpawn", "Exploder OnSpawn")) as CellAddRemoveSubstanceEvent;
    this.ExhaustSimUpdate = this.AddEvent((CellEvent) new CellAddRemoveSubstanceEvent("ExhaustSimUpdate", "Exhaust SimUpdate")) as CellAddRemoveSubstanceEvent;
    this.ElementConsumerSimUpdate = this.AddEvent((CellEvent) new CellAddRemoveSubstanceEvent("ElementConsumerSimUpdate", "Element Consumer SimUpdate")) as CellAddRemoveSubstanceEvent;
    this.SublimatesEmit = this.AddEvent((CellEvent) new CellAddRemoveSubstanceEvent("SublimatesEmit", "Sublimates Emit")) as CellAddRemoveSubstanceEvent;
    this.Mop = this.AddEvent((CellEvent) new CellAddRemoveSubstanceEvent("Mop", "Mop")) as CellAddRemoveSubstanceEvent;
    this.OreMelted = this.AddEvent((CellEvent) new CellAddRemoveSubstanceEvent("OreMelted", "Ore Melted")) as CellAddRemoveSubstanceEvent;
    this.ConstructTile = this.AddEvent((CellEvent) new CellAddRemoveSubstanceEvent("ConstructTile", "ConstructTile")) as CellAddRemoveSubstanceEvent;
    this.Dumpable = this.AddEvent((CellEvent) new CellAddRemoveSubstanceEvent("Dympable", "Dumpable")) as CellAddRemoveSubstanceEvent;
    this.Cough = this.AddEvent((CellEvent) new CellAddRemoveSubstanceEvent("Cough", "Cough")) as CellAddRemoveSubstanceEvent;
    this.Meteor = this.AddEvent((CellEvent) new CellAddRemoveSubstanceEvent("Meteor", "Meteor")) as CellAddRemoveSubstanceEvent;
    this.ElementChunkTransition = this.AddEvent((CellEvent) new CellAddRemoveSubstanceEvent("ElementChunkTransition", "Element Chunk Transition")) as CellAddRemoveSubstanceEvent;
    this.OxyrockEmit = this.AddEvent((CellEvent) new CellAddRemoveSubstanceEvent("OxyrockEmit", "Oxyrock Emit")) as CellAddRemoveSubstanceEvent;
    this.BleachstoneEmit = this.AddEvent((CellEvent) new CellAddRemoveSubstanceEvent("BleachstoneEmit", "Bleachstone Emit")) as CellAddRemoveSubstanceEvent;
    this.UnstableGround = this.AddEvent((CellEvent) new CellAddRemoveSubstanceEvent("UnstableGround", "Unstable Ground")) as CellAddRemoveSubstanceEvent;
    this.ConduitFlowEmptyConduit = this.AddEvent((CellEvent) new CellAddRemoveSubstanceEvent("ConduitFlowEmptyConduit", "Conduit Flow Empty Conduit")) as CellAddRemoveSubstanceEvent;
    this.ConduitConsumerWrongElement = this.AddEvent((CellEvent) new CellAddRemoveSubstanceEvent("ConduitConsumerWrongElement", "Conduit Consumer Wrong Element")) as CellAddRemoveSubstanceEvent;
    this.OverheatableMeltingDown = this.AddEvent((CellEvent) new CellAddRemoveSubstanceEvent("OverheatableMeltingDown", "Overheatable MeltingDown")) as CellAddRemoveSubstanceEvent;
    this.FabricatorProduceMelted = this.AddEvent((CellEvent) new CellAddRemoveSubstanceEvent("FabricatorProduceMelted", "Fabricator Produce Melted")) as CellAddRemoveSubstanceEvent;
    this.PumpSimUpdate = this.AddEvent((CellEvent) new CellAddRemoveSubstanceEvent("PumpSimUpdate", "Pump SimUpdate")) as CellAddRemoveSubstanceEvent;
    this.WallPumpSimUpdate = this.AddEvent((CellEvent) new CellAddRemoveSubstanceEvent("WallPumpSimUpdate", "Wall Pump SimUpdate")) as CellAddRemoveSubstanceEvent;
    this.Vomit = this.AddEvent((CellEvent) new CellAddRemoveSubstanceEvent("Vomit", "Vomit")) as CellAddRemoveSubstanceEvent;
    this.Tears = this.AddEvent((CellEvent) new CellAddRemoveSubstanceEvent("Tears", "Tears")) as CellAddRemoveSubstanceEvent;
    this.Pee = this.AddEvent((CellEvent) new CellAddRemoveSubstanceEvent("Pee", "Pee")) as CellAddRemoveSubstanceEvent;
    this.AlgaeHabitat = this.AddEvent((CellEvent) new CellAddRemoveSubstanceEvent("AlgaeHabitat", "AlgaeHabitat")) as CellAddRemoveSubstanceEvent;
    this.CO2FilterOxygen = this.AddEvent((CellEvent) new CellAddRemoveSubstanceEvent("CO2FilterOxygen", "CO2FilterOxygen")) as CellAddRemoveSubstanceEvent;
    this.ToiletEmit = this.AddEvent((CellEvent) new CellAddRemoveSubstanceEvent("ToiletEmit", "ToiletEmit")) as CellAddRemoveSubstanceEvent;
    this.ElementEmitted = this.AddEvent((CellEvent) new CellAddRemoveSubstanceEvent("ElementEmitted", "Element Emitted")) as CellAddRemoveSubstanceEvent;
    this.CO2ManagerFixedUpdate = this.AddEvent((CellEvent) new CellModifyMassEvent("CO2ManagerFixedUpdate", "CO2Manager FixedUpdate")) as CellModifyMassEvent;
    this.EnvironmentConsumerFixedUpdate = this.AddEvent((CellEvent) new CellModifyMassEvent("EnvironmentConsumerFixedUpdate", "EnvironmentConsumer FixedUpdate")) as CellModifyMassEvent;
    this.ExcavatorShockwave = this.AddEvent((CellEvent) new CellModifyMassEvent("ExcavatorShockwave", "Excavator Shockwave")) as CellModifyMassEvent;
    this.OxygenBreatherSimUpdate = this.AddEvent((CellEvent) new CellModifyMassEvent("OxygenBreatherSimUpdate", "Oxygen Breather SimUpdate")) as CellModifyMassEvent;
    this.CO2ScrubberSimUpdate = this.AddEvent((CellEvent) new CellModifyMassEvent("CO2ScrubberSimUpdate", "CO2Scrubber SimUpdate")) as CellModifyMassEvent;
    this.RiverSourceSimUpdate = this.AddEvent((CellEvent) new CellModifyMassEvent("RiverSourceSimUpdate", "RiverSource SimUpdate")) as CellModifyMassEvent;
    this.RiverTerminusSimUpdate = this.AddEvent((CellEvent) new CellModifyMassEvent("RiverTerminusSimUpdate", "RiverTerminus SimUpdate")) as CellModifyMassEvent;
    this.DebugToolModifyMass = this.AddEvent((CellEvent) new CellModifyMassEvent("DebugToolModifyMass", "DebugTool ModifyMass")) as CellModifyMassEvent;
    this.EnergyGeneratorModifyMass = this.AddEvent((CellEvent) new CellModifyMassEvent("EnergyGeneratorModifyMass", "EnergyGenerator ModifyMass")) as CellModifyMassEvent;
    this.SolidFilterEvent = this.AddEvent((CellEvent) new CellSolidFilterEvent("SolidFilterEvent")) as CellSolidFilterEvent;
  }
}
