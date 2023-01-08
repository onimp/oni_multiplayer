// Decompiled with JetBrains decompiler
// Type: RailGunConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class RailGunConfig : IBuildingConfig
{
  public const string ID = "RailGun";
  public const string PORT_ID = "HEP_STORAGE";
  public const int RANGE = 20;
  public const float BASE_PARTICLE_COST = 0.0f;
  public const float HEX_PARTICLE_COST = 10f;
  public const float HEP_CAPACITY = 200f;
  public const float TAKEOFF_VELOCITY = 35f;
  public const int MAINTENANCE_AFTER_NUM_PAYLOADS = 6;
  public const int MAINTENANCE_COOLDOWN = 30;
  public const float CAPACITY = 1200f;
  private ConduitPortInfo solidInputPort = new ConduitPortInfo(ConduitType.Solid, new CellOffset(-1, 0));
  private ConduitPortInfo liquidInputPort = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(0, 0));
  private ConduitPortInfo gasInputPort = new ConduitPortInfo(ConduitType.Gas, new CellOffset(1, 0));

  public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR4 = TUNING.BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues tieR5 = NOISE_POLLUTION.NOISY.TIER5;
    EffectorValues none = TUNING.BUILDINGS.DECOR.NONE;
    EffectorValues noise = tieR5;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("RailGun", 5, 6, "rail_gun_kanim", 250, 30f, tieR4, allMetals, 1600f, BuildLocationRule.OnFloor, none, noise);
    buildingDef.Floodable = false;
    buildingDef.Overheatable = false;
    buildingDef.AudioCategory = "Metal";
    buildingDef.BaseTimeUntilRepair = 400f;
    buildingDef.DefaultAnimState = "off";
    buildingDef.RequiresPowerInput = true;
    buildingDef.PowerInputOffset = new CellOffset(-2, 0);
    buildingDef.EnergyConsumptionWhenActive = 240f;
    buildingDef.ViewMode = OverlayModes.Power.ID;
    buildingDef.ExhaustKilowattsWhenActive = 0.5f;
    buildingDef.SelfHeatKilowattsWhenActive = 2f;
    buildingDef.UseHighEnergyParticleInputPort = true;
    buildingDef.HighEnergyParticleInputOffset = new CellOffset(-2, 1);
    buildingDef.LogicInputPorts = new List<LogicPorts.Port>()
    {
      LogicPorts.Port.InputPort(RailGun.PORT_ID, new CellOffset(-2, 2), (string) STRINGS.BUILDINGS.PREFABS.RAILGUN.LOGIC_PORT, (string) STRINGS.BUILDINGS.PREFABS.RAILGUN.LOGIC_PORT_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.RAILGUN.LOGIC_PORT_INACTIVE)
    };
    buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
    {
      LogicPorts.Port.OutputPort(HashedString.op_Implicit("HEP_STORAGE"), new CellOffset(2, 0), (string) STRINGS.BUILDINGS.PREFABS.HEPENGINE.LOGIC_PORT_STORAGE, (string) STRINGS.BUILDINGS.PREFABS.HEPENGINE.LOGIC_PORT_STORAGE_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.HEPENGINE.LOGIC_PORT_STORAGE_INACTIVE)
    };
    return buildingDef;
  }

  private void AttachPorts(GameObject go)
  {
    go.AddComponent<ConduitSecondaryInput>().portInfo = this.liquidInputPort;
    go.AddComponent<ConduitSecondaryInput>().portInfo = this.gasInputPort;
    go.AddComponent<ConduitSecondaryInput>().portInfo = this.solidInputPort;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    RailGun railGun = go.AddOrGet<RailGun>();
    go.AddOrGet<LoopingSounds>();
    ClusterDestinationSelector destinationSelector = go.AddOrGet<ClusterDestinationSelector>();
    destinationSelector.assignable = true;
    destinationSelector.requireAsteroidDestination = true;
    railGun.liquidPortInfo = this.liquidInputPort;
    railGun.gasPortInfo = this.gasInputPort;
    railGun.solidPortInfo = this.solidInputPort;
    HighEnergyParticleStorage energyParticleStorage = go.AddOrGet<HighEnergyParticleStorage>();
    energyParticleStorage.capacity = 200f;
    energyParticleStorage.autoStore = true;
    energyParticleStorage.showInUI = false;
    energyParticleStorage.PORT_ID = "HEP_STORAGE";
    energyParticleStorage.showCapacityStatusItem = true;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    List<Tag> tagList = new List<Tag>();
    tagList.AddRange((IEnumerable<Tag>) STORAGEFILTERS.NOT_EDIBLE_SOLIDS);
    tagList.AddRange((IEnumerable<Tag>) STORAGEFILTERS.GASES);
    tagList.AddRange((IEnumerable<Tag>) STORAGEFILTERS.FOOD);
    Storage defaultStorage = BuildingTemplates.CreateDefaultStorage(go);
    defaultStorage.showInUI = true;
    defaultStorage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
    defaultStorage.storageFilters = tagList;
    defaultStorage.allowSettingOnlyFetchMarkedItems = false;
    defaultStorage.fetchCategory = Storage.FetchCategory.GeneralStorage;
    defaultStorage.capacityKg = 1200f;
  }

  public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
  {
    base.DoPostConfigurePreview(def, go);
    this.AttachPorts(go);
  }

  public override void DoPostConfigureUnderConstruction(GameObject go)
  {
    base.DoPostConfigureUnderConstruction(go);
    this.AttachPorts(go);
  }
}
