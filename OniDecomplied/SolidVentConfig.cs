// Decompiled with JetBrains decompiler
// Type: SolidVentConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class SolidVentConfig : IBuildingConfig
{
  public const string ID = "SolidVent";
  private const ConduitType CONDUIT_TYPE = ConduitType.Solid;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR3 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues tieR1 = BUILDINGS.DECOR.PENALTY.TIER1;
    EffectorValues noise = none;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("SolidVent", 1, 1, "conveyer_dropper_kanim", 30, 30f, tieR3, allMetals, 1600f, BuildLocationRule.Anywhere, tieR1, noise);
    buildingDef.InputConduitType = ConduitType.Solid;
    buildingDef.Floodable = false;
    buildingDef.Overheatable = false;
    buildingDef.ViewMode = OverlayModes.SolidConveyor.ID;
    buildingDef.AudioCategory = "Metal";
    buildingDef.UtilityInputOffset = new CellOffset(0, 0);
    buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
    buildingDef.LogicInputPorts = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0));
    GeneratedBuildings.RegisterWithOverlay(OverlayScreen.SolidConveyorIDs, "SolidVent");
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) => go.AddOrGet<LogicOperationalController>();

  public override void DoPostConfigureUnderConstruction(GameObject go)
  {
    base.DoPostConfigureUnderConstruction(go);
    go.GetComponent<Constructable>().requiredSkillPerk = Db.Get().SkillPerks.ConveyorBuild.Id;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGet<SimpleVent>();
    go.AddOrGet<SolidConduitConsumer>();
    go.AddOrGet<SolidConduitDropper>();
    Storage defaultStorage = BuildingTemplates.CreateDefaultStorage(go);
    defaultStorage.capacityKg = 100f;
    defaultStorage.showInUI = true;
  }
}
