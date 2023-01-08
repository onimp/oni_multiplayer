// Decompiled with JetBrains decompiler
// Type: SolidConduitInboxConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class SolidConduitInboxConfig : IBuildingConfig
{
  public const string ID = "SolidConduitInbox";

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR3 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
    string[] refinedMetals = MATERIALS.REFINED_METALS;
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues tieR1 = BUILDINGS.DECOR.PENALTY.TIER1;
    EffectorValues noise = none;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("SolidConduitInbox", 1, 2, "conveyorin_kanim", 100, 60f, tieR3, refinedMetals, 1600f, BuildLocationRule.Anywhere, tieR1, noise);
    buildingDef.RequiresPowerInput = true;
    buildingDef.EnergyConsumptionWhenActive = 120f;
    buildingDef.ExhaustKilowattsWhenActive = 0.0f;
    buildingDef.SelfHeatKilowattsWhenActive = 2f;
    buildingDef.Floodable = false;
    buildingDef.ViewMode = OverlayModes.SolidConveyor.ID;
    buildingDef.AudioCategory = "Metal";
    buildingDef.OutputConduitType = ConduitType.Solid;
    buildingDef.PowerInputOffset = new CellOffset(0, 1);
    buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
    buildingDef.PermittedRotations = PermittedRotations.R360;
    buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 1));
    GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, "SolidConduitInbox");
    return buildingDef;
  }

  public override void DoPostConfigureUnderConstruction(GameObject go) => go.GetComponent<Constructable>().requiredSkillPerk = Db.Get().SkillPerks.ConveyorBuild.Id;

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGet<LogicOperationalController>();
    Prioritizable.AddRef(go);
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.IndustrialMachinery, false);
    go.AddOrGet<EnergyConsumer>();
    go.AddOrGet<Automatable>();
    List<Tag> tagList = new List<Tag>();
    tagList.AddRange((IEnumerable<Tag>) STORAGEFILTERS.NOT_EDIBLE_SOLIDS);
    tagList.AddRange((IEnumerable<Tag>) STORAGEFILTERS.FOOD);
    Storage storage = go.AddOrGet<Storage>();
    storage.capacityKg = 1000f;
    storage.showInUI = true;
    storage.showDescriptor = true;
    storage.storageFilters = tagList;
    storage.allowItemRemoval = false;
    storage.onlyTransferFromLowerPriority = true;
    storage.showCapacityStatusItem = true;
    storage.showCapacityAsMainStatus = true;
    go.AddOrGet<TreeFilterable>();
    go.AddOrGet<SolidConduitInbox>();
    go.AddOrGet<SolidConduitDispenser>();
  }
}
