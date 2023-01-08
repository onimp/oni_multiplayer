// Decompiled with JetBrains decompiler
// Type: SaunaConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class SaunaConfig : IBuildingConfig
{
  public const string ID = "Sauna";
  private const float STEAM_PER_USE_KG = 25f;
  private const float WATER_OUTPUT_TEMP = 353.15f;

  public override BuildingDef CreateBuildingDef()
  {
    float[] construction_mass = new float[2]{ 100f, 100f };
    string[] construction_materials = new string[2]
    {
      "Metal",
      "BuildingWood"
    };
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues tieR2 = BUILDINGS.DECOR.BONUS.TIER2;
    EffectorValues noise = none;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("Sauna", 3, 3, "sauna_kanim", 30, 60f, construction_mass, construction_materials, 1600f, BuildLocationRule.OnFloor, tieR2, noise);
    buildingDef.ViewMode = OverlayModes.GasConduits.ID;
    buildingDef.Floodable = true;
    buildingDef.AudioCategory = "Metal";
    buildingDef.Overheatable = true;
    buildingDef.InputConduitType = ConduitType.Gas;
    buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
    buildingDef.OutputConduitType = ConduitType.Liquid;
    buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
    buildingDef.RequiresPowerInput = true;
    buildingDef.PowerInputOffset = new CellOffset(0, 2);
    buildingDef.EnergyConsumptionWhenActive = 60f;
    buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.RecBuilding, false);
    go.AddOrGet<Storage>().SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
    ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
    conduitConsumer.conduitType = ConduitType.Gas;
    conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.Steam).tag;
    conduitConsumer.capacityKG = 50f;
    conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
    conduitConsumer.alwaysConsume = true;
    ConduitDispenser conduitDispenser = go.AddOrGet<ConduitDispenser>();
    conduitDispenser.conduitType = ConduitType.Liquid;
    conduitDispenser.elementFilter = new SimHashes[1]
    {
      SimHashes.Water
    };
    go.AddOrGet<SaunaWorkable>().basePriority = RELAXATION.PRIORITY.TIER3;
    Sauna sauna = go.AddOrGet<Sauna>();
    sauna.steamPerUseKG = 25f;
    sauna.waterOutputTemp = 353.15f;
    sauna.specificEffect = "Sauna";
    sauna.trackingEffect = "RecentlySauna";
    RoomTracker roomTracker = go.AddOrGet<RoomTracker>();
    roomTracker.requiredRoomType = Db.Get().RoomTypes.RecRoom.Id;
    roomTracker.requirement = RoomTracker.Requirement.Recommended;
    go.AddOrGetDef<RocketUsageRestriction.Def>();
  }

  public override void DoPostConfigureComplete(GameObject go) => go.GetComponent<RequireInputs>().requireConduitHasMass = false;
}
