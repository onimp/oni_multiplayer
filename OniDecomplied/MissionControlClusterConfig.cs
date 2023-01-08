// Decompiled with JetBrains decompiler
// Type: MissionControlClusterConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class MissionControlClusterConfig : IBuildingConfig
{
  public const string ID = "MissionControlCluster";
  public const int WORK_RANGE_RADIUS = 2;
  public const float EFFECT_DURATION = 600f;
  public const float SPEED_MULTIPLIER = 1.2f;

  public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR4 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
    string[] refinedMetals = MATERIALS.REFINED_METALS;
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues tieR0 = BUILDINGS.DECOR.PENALTY.TIER0;
    EffectorValues noise = none;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("MissionControlCluster", 3, 3, "mission_control_station_kanim", 100, 30f, tieR4, refinedMetals, 1600f, BuildLocationRule.OnFloor, tieR0, noise);
    buildingDef.RequiresPowerInput = true;
    buildingDef.EnergyConsumptionWhenActive = 960f;
    buildingDef.ExhaustKilowattsWhenActive = 0.5f;
    buildingDef.SelfHeatKilowattsWhenActive = 2f;
    buildingDef.ViewMode = OverlayModes.Power.ID;
    buildingDef.AudioCategory = "Metal";
    buildingDef.AudioSize = "large";
    buildingDef.DefaultAnimState = "off";
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.ScienceBuilding, false);
    BuildingDef def1 = go.GetComponent<BuildingComplete>().Def;
    Prioritizable.AddRef(go);
    go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
    go.AddOrGetDef<PoweredController.Def>();
    SkyVisibilityMonitor.Def def2 = go.AddOrGetDef<SkyVisibilityMonitor.Def>();
    def2.ScanRadius = 1;
    def2.ScanOriginOffset = new CellOffset(0, def1.HeightInCells);
    go.AddOrGetDef<MissionControlCluster.Def>();
    MissionControlClusterWorkable controlClusterWorkable = go.AddOrGet<MissionControlClusterWorkable>();
    controlClusterWorkable.requiredSkillPerk = Db.Get().SkillPerks.CanMissionControl.Id;
    controlClusterWorkable.workLayer = Grid.SceneLayer.BuildingUse;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
    roomTracker.requiredRoomType = Db.Get().RoomTypes.Laboratory.Id;
    roomTracker.requirement = RoomTracker.Requirement.Required;
  }
}
