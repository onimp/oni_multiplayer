// Decompiled with JetBrains decompiler
// Type: ContactConductivePipeBridgeConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class ContactConductivePipeBridgeConfig : IBuildingConfig
{
  public const float LIQUID_CAPACITY_KG = 10f;
  public const float GAS_CAPACITY_KG = 0.5f;
  public const float TEMPERATURE_EXCHANGE_WITH_STORAGE_MODIFIER = 50f;
  public const float BUILDING_TO_BUILDING_TEMPERATURE_SCALE = 0.001f;
  public const string ID = "ContactConductivePipeBridge";
  private const ConduitType CONDUIT_TYPE = ConduitType.Liquid;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR2 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER2;
    string[] refinedMetals = MATERIALS.REFINED_METALS;
    EffectorValues none1 = NOISE_POLLUTION.NONE;
    EffectorValues none2 = BUILDINGS.DECOR.NONE;
    EffectorValues noise = none1;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("ContactConductivePipeBridge", 3, 1, "contactConductivePipeBridge_kanim", 30, 10f, tieR2, refinedMetals, 2400f, BuildLocationRule.NoLiquidConduitAtOrigin, none2, noise);
    buildingDef.Floodable = false;
    buildingDef.Entombable = false;
    buildingDef.Overheatable = false;
    buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
    buildingDef.ObjectLayer = ObjectLayer.LiquidConduitConnection;
    buildingDef.SceneLayer = Grid.SceneLayer.LiquidConduitBridges;
    buildingDef.AudioCategory = "Metal";
    buildingDef.AudioSize = "small";
    buildingDef.BaseTimeUntilRepair = -1f;
    buildingDef.PermittedRotations = PermittedRotations.R360;
    buildingDef.InputConduitType = ConduitType.Liquid;
    buildingDef.OutputConduitType = ConduitType.Liquid;
    buildingDef.UtilityInputOffset = new CellOffset(-1, 0);
    buildingDef.UtilityOutputOffset = new CellOffset(1, 0);
    buildingDef.UseStructureTemperature = true;
    buildingDef.ReplacementTags = new List<Tag>();
    buildingDef.ReplacementTags.Add(GameTags.Pipes);
    GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, "ContactConductivePipeBridge");
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    BuildingConfigManager.Instance.IgnoreDefaultKComponent(typeof (RequiresFoundation), prefab_tag);
    go.AddOrGet<StructureToStructureTemperature>();
    Storage storage = go.AddOrGet<Storage>();
    storage.allowItemRemoval = false;
    storage.storageFilters = STORAGEFILTERS.LIQUIDS;
    storage.capacityKg = 10f;
    storage.showDescriptor = true;
    List<Storage.StoredItemModifier> modifiers = new List<Storage.StoredItemModifier>()
    {
      Storage.StoredItemModifier.Hide,
      Storage.StoredItemModifier.Seal,
      Storage.StoredItemModifier.Insulate,
      Storage.StoredItemModifier.Preserve
    };
    storage.SetDefaultStoredItemModifiers(modifiers);
    ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
    conduitConsumer.conduitType = ConduitType.Liquid;
    conduitConsumer.capacityKG = storage.capacityKg;
    conduitConsumer.alwaysConsume = true;
    ContactConductivePipeBridge.Def def = go.AddOrGetDef<ContactConductivePipeBridge.Def>();
    def.pumpKGRate = 10f;
    def.type = ConduitType.Liquid;
  }

  public override void DoPostConfigureComplete(GameObject go) => Object.DestroyImmediate((Object) go.GetComponent<RequireOutputs>());
}
