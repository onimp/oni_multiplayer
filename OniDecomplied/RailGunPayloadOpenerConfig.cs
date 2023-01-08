// Decompiled with JetBrains decompiler
// Type: RailGunPayloadOpenerConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using System.Linq;
using TUNING;
using UnityEngine;

public class RailGunPayloadOpenerConfig : IBuildingConfig
{
  public const string ID = "RailGunPayloadOpener";
  private ConduitPortInfo liquidOutputPort = new ConduitPortInfo(ConduitType.Liquid, new CellOffset(0, 0));
  private ConduitPortInfo gasOutputPort = new ConduitPortInfo(ConduitType.Gas, new CellOffset(1, 0));
  private ConduitPortInfo solidOutputPort = new ConduitPortInfo(ConduitType.Solid, new CellOffset(-1, 0));

  public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR2 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues tieR5 = NOISE_POLLUTION.NOISY.TIER5;
    EffectorValues none = BUILDINGS.DECOR.NONE;
    EffectorValues noise = tieR5;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("RailGunPayloadOpener", 3, 3, "railgun_emptier_kanim", 250, 10f, tieR2, allMetals, 1600f, BuildLocationRule.OnFloor, none, noise);
    buildingDef.Floodable = false;
    buildingDef.Overheatable = false;
    buildingDef.AudioCategory = "Metal";
    buildingDef.DefaultAnimState = "on";
    buildingDef.RequiresPowerInput = true;
    buildingDef.PowerInputOffset = new CellOffset(0, 0);
    buildingDef.EnergyConsumptionWhenActive = 120f;
    buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
    return buildingDef;
  }

  private void AttachPorts(GameObject go)
  {
    go.AddComponent<ConduitSecondaryOutput>().portInfo = this.liquidOutputPort;
    go.AddComponent<ConduitSecondaryOutput>().portInfo = this.gasOutputPort;
    go.AddComponent<ConduitSecondaryOutput>().portInfo = this.solidOutputPort;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    RailGunPayloadOpener gunPayloadOpener = go.AddOrGet<RailGunPayloadOpener>();
    gunPayloadOpener.liquidPortInfo = this.liquidOutputPort;
    gunPayloadOpener.gasPortInfo = this.gasOutputPort;
    gunPayloadOpener.solidPortInfo = this.solidOutputPort;
    gunPayloadOpener.payloadStorage = go.AddComponent<Storage>();
    gunPayloadOpener.payloadStorage.showInUI = true;
    gunPayloadOpener.payloadStorage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
    Storage payloadStorage = gunPayloadOpener.payloadStorage;
    List<Tag> tagList = new List<Tag>();
    tagList.Add(GameTags.RailGunPayloadEmptyable);
    payloadStorage.storageFilters = tagList;
    gunPayloadOpener.payloadStorage.capacityKg = 10f;
    gunPayloadOpener.resourceStorage = go.AddComponent<Storage>();
    gunPayloadOpener.resourceStorage.showInUI = true;
    gunPayloadOpener.resourceStorage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
    List<Tag> list = ((IEnumerable<Tag>) ((IEnumerable<Tag>) STORAGEFILTERS.NOT_EDIBLE_SOLIDS).Concat<Tag>((IEnumerable<Tag>) STORAGEFILTERS.GASES).ToList<Tag>()).Concat<Tag>((IEnumerable<Tag>) STORAGEFILTERS.LIQUIDS).ToList<Tag>();
    gunPayloadOpener.resourceStorage.storageFilters = list;
    gunPayloadOpener.resourceStorage.capacityKg = 20000f;
    ManualDeliveryKG manualDeliveryKg = go.AddComponent<ManualDeliveryKG>();
    manualDeliveryKg.SetStorage(gunPayloadOpener.payloadStorage);
    manualDeliveryKg.RequestedItemTag = GameTags.RailGunPayloadEmptyable;
    manualDeliveryKg.capacity = 10f;
    manualDeliveryKg.refillMass = 1f;
    manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
    manualDeliveryKg.operationalRequirement = Operational.State.None;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGet<BuildingCellVisualizer>();
    DropAllWorkable dropAllWorkable = go.AddOrGet<DropAllWorkable>();
    dropAllWorkable.dropWorkTime = 90f;
    dropAllWorkable.choreTypeID = Db.Get().ChoreTypes.Fetch.Id;
    dropAllWorkable.ConfigureMultitoolContext(HashedString.op_Implicit("build"), Tag.op_Implicit(EffectConfigs.BuildSplashId));
    RequireInputs component = go.GetComponent<RequireInputs>();
    component.SetRequirements(true, false);
    component.requireConduitHasMass = false;
  }

  public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
  {
    base.DoPostConfigurePreview(def, go);
    this.AttachPorts(go);
    go.AddOrGet<BuildingCellVisualizer>();
  }

  public override void DoPostConfigureUnderConstruction(GameObject go)
  {
    base.DoPostConfigureUnderConstruction(go);
    this.AttachPorts(go);
    go.AddOrGet<BuildingCellVisualizer>();
  }
}
