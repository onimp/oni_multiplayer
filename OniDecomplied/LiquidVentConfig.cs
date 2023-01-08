// Decompiled with JetBrains decompiler
// Type: LiquidVentConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class LiquidVentConfig : IBuildingConfig
{
  public const string ID = "LiquidVent";
  public const float OVERPRESSURE_MASS = 1000f;
  private const ConduitType CONDUIT_TYPE = ConduitType.Liquid;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR4 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues tieR1 = BUILDINGS.DECOR.PENALTY.TIER1;
    EffectorValues noise = none;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("LiquidVent", 1, 1, "ventliquid_kanim", 30, 30f, tieR4, allMetals, 1600f, BuildLocationRule.Anywhere, tieR1, noise);
    buildingDef.InputConduitType = ConduitType.Liquid;
    buildingDef.Floodable = false;
    buildingDef.Overheatable = false;
    buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
    buildingDef.AudioCategory = "Metal";
    buildingDef.UtilityInputOffset = new CellOffset(0, 0);
    buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
    buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
    GeneratedBuildings.RegisterWithOverlay(OverlayScreen.LiquidVentIDs, "LiquidVent");
    SoundEventVolumeCache.instance.AddVolume("ventliquid_kanim", "LiquidVent_squirt", NOISE_POLLUTION.NOISY.TIER0);
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.AddOrGet<LoopingSounds>();
    go.AddOrGet<Exhaust>();
    go.AddOrGet<LogicOperationalController>();
    Vent vent = go.AddOrGet<Vent>();
    vent.conduitType = ConduitType.Liquid;
    vent.endpointType = Endpoint.Sink;
    vent.overpressureMass = 1000f;
    ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
    conduitConsumer.conduitType = ConduitType.Liquid;
    conduitConsumer.ignoreMinMassCheck = true;
    BuildingTemplates.CreateDefaultStorage(go).showInUI = true;
    go.AddOrGet<SimpleVent>();
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGetDef<VentController.Def>();
    go.GetComponent<KPrefabID>().AddTag(GameTags.OverlayInFrontOfConduits, false);
  }
}
