// Decompiled with JetBrains decompiler
// Type: HEPEngineConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class HEPEngineConfig : IBuildingConfig
{
  private const int PARTICLES_PER_HEX = 200;
  private const int RANGE = 20;
  private const int PARTICLE_STORAGE_CAPACITY = 4000;
  private const int PORT_OFFSET_Y = 3;
  public const string ID = "HEPEngine";
  public const string PORT_ID = "HEP_STORAGE";

  public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public override BuildingDef CreateBuildingDef()
  {
    float[] engineMassLarge = TUNING.BUILDINGS.ROCKETRY_MASS_KG.ENGINE_MASS_LARGE;
    string[] construction_materials = new string[1]
    {
      SimHashes.Steel.ToString()
    };
    EffectorValues tieR2 = NOISE_POLLUTION.NOISY.TIER2;
    EffectorValues none = TUNING.BUILDINGS.DECOR.NONE;
    EffectorValues noise = tieR2;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("HEPEngine", 5, 5, "rocket_hep_engine_kanim", 1000, 60f, engineMassLarge, construction_materials, 9999f, BuildLocationRule.Anywhere, none, noise);
    BuildingTemplates.CreateRocketBuildingDef(buildingDef);
    buildingDef.SceneLayer = Grid.SceneLayer.Building;
    buildingDef.OverheatTemperature = 2273.15f;
    buildingDef.Floodable = false;
    buildingDef.AttachmentSlotTag = GameTags.Rocket;
    buildingDef.ObjectLayer = ObjectLayer.Building;
    buildingDef.attachablePosition = new CellOffset(0, 0);
    buildingDef.RequiresPowerInput = false;
    buildingDef.RequiresPowerOutput = false;
    buildingDef.CanMove = true;
    buildingDef.Cancellable = false;
    buildingDef.ShowInBuildMenu = false;
    buildingDef.UseHighEnergyParticleInputPort = true;
    buildingDef.HighEnergyParticleInputOffset = new CellOffset(0, 3);
    buildingDef.LogicOutputPorts = new List<LogicPorts.Port>()
    {
      LogicPorts.Port.OutputPort(HashedString.op_Implicit("HEP_STORAGE"), new CellOffset(0, 2), (string) STRINGS.BUILDINGS.PREFABS.HEPENGINE.LOGIC_PORT_STORAGE, (string) STRINGS.BUILDINGS.PREFABS.HEPENGINE.LOGIC_PORT_STORAGE_ACTIVE, (string) STRINGS.BUILDINGS.PREFABS.HEPENGINE.LOGIC_PORT_STORAGE_INACTIVE)
    };
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof (RequiresFoundation), prefab_tag);
    go.AddOrGet<LoopingSounds>();
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
    go.AddOrGet<BuildingAttachPoint>().points = new BuildingAttachPoint.HardPoint[1]
    {
      new BuildingAttachPoint.HardPoint(new CellOffset(0, 5), GameTags.Rocket, (AttachableBuilding) null)
    };
  }

  public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
  {
  }

  public override void DoPostConfigureUnderConstruction(GameObject go)
  {
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    RadiationEmitter radiationEmitter = go.AddOrGet<RadiationEmitter>();
    radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Constant;
    radiationEmitter.emitRadiusX = (short) 10;
    radiationEmitter.emitRadiusY = (short) 10;
    radiationEmitter.emitRads = (float) (8400.0 / ((double) radiationEmitter.emitRadiusX / 6.0));
    radiationEmitter.emissionOffset = new Vector3(0.0f, 3f, 0.0f);
    HighEnergyParticleStorage energyParticleStorage = go.AddOrGet<HighEnergyParticleStorage>();
    energyParticleStorage.capacity = 4000f;
    energyParticleStorage.autoStore = true;
    energyParticleStorage.PORT_ID = "HEP_STORAGE";
    energyParticleStorage.showCapacityStatusItem = true;
    go.AddOrGet<HEPFuelTank>().physicalFuelCapacity = 4000f;
    RocketEngineCluster rocketEngineCluster = go.AddOrGet<RocketEngineCluster>();
    rocketEngineCluster.maxModules = 4;
    rocketEngineCluster.maxHeight = TUNING.ROCKETRY.ROCKET_HEIGHT.MEDIUM;
    rocketEngineCluster.efficiency = TUNING.ROCKETRY.ENGINE_EFFICIENCY.STRONG;
    rocketEngineCluster.explosionEffectHash = SpawnFXHashes.MeteorImpactDust;
    rocketEngineCluster.requireOxidizer = false;
    rocketEngineCluster.exhaustElement = SimHashes.Fallout;
    rocketEngineCluster.exhaustTemperature = 873.15f;
    rocketEngineCluster.exhaustEmitRate = 25f;
    rocketEngineCluster.exhaustDiseaseIdx = Db.Get().Diseases.GetIndex(HashedString.op_Implicit(Db.Get().Diseases.RadiationPoisoning.Id));
    rocketEngineCluster.exhaustDiseaseCount = 100000;
    rocketEngineCluster.emitRadiation = true;
    rocketEngineCluster.fuelTag = GameTags.HighEnergyParticle;
    BuildingTemplates.ExtendBuildingToRocketModuleCluster(go, (string) null, TUNING.ROCKETRY.BURDEN.MODERATE_PLUS, (float) TUNING.ROCKETRY.ENGINE_POWER.LATE_STRONG, TUNING.ROCKETRY.FUEL_COST_PER_DISTANCE.PARTICLES);
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    go.GetComponent<KPrefabID>().prefabInitFn += HEPEngineConfig.\u003C\u003Ec.\u003C\u003E9__11_0 ?? (HEPEngineConfig.\u003C\u003Ec.\u003C\u003E9__11_0 = new KPrefabID.PrefabFn((object) HEPEngineConfig.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003CDoPostConfigureComplete\u003Eb__11_0)));
  }
}
