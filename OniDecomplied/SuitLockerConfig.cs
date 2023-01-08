// Decompiled with JetBrains decompiler
// Type: SuitLockerConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class SuitLockerConfig : IBuildingConfig
{
  public const string ID = "SuitLocker";

  public override BuildingDef CreateBuildingDef()
  {
    string[] refinedMetals = MATERIALS.REFINED_METALS;
    float[] construction_mass = new float[2]
    {
      BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0],
      BUILDINGS.CONSTRUCTION_MASS_KG.TIER1[0]
    };
    string[] construction_materials = refinedMetals;
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues tieR1 = BUILDINGS.DECOR.BONUS.TIER1;
    EffectorValues noise = none;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("SuitLocker", 1, 3, "changingarea_kanim", 30, 30f, construction_mass, construction_materials, 1600f, BuildLocationRule.OnFloor, tieR1, noise);
    buildingDef.RequiresPowerInput = true;
    buildingDef.EnergyConsumptionWhenActive = 120f;
    buildingDef.PreventIdleTraversalPastBuilding = true;
    buildingDef.InputConduitType = ConduitType.Gas;
    buildingDef.UtilityInputOffset = new CellOffset(0, 0);
    GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SuitIDs, "SuitLocker");
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.AddOrGet<SuitLocker>().OutfitTags = new Tag[1]
    {
      GameTags.AtmoSuit
    };
    ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
    conduitConsumer.conduitType = ConduitType.Gas;
    conduitConsumer.consumptionRate = 1f;
    conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.Oxygen).tag;
    conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Dump;
    conduitConsumer.forceAlwaysSatisfied = true;
    conduitConsumer.capacityKG = 200f;
    go.AddOrGet<AnimTileable>().tags = new Tag[2]
    {
      new Tag("SuitLocker"),
      new Tag("SuitMarker")
    };
    go.AddOrGet<Storage>();
    Prioritizable.AddRef(go);
  }

  public override void DoPostConfigureComplete(GameObject go) => SymbolOverrideControllerUtil.AddToPrefab(go);
}
