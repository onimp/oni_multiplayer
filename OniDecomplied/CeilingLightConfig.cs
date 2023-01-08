// Decompiled with JetBrains decompiler
// Type: CeilingLightConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class CeilingLightConfig : IBuildingConfig
{
  public const string ID = "CeilingLight";

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR1 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues none1 = NOISE_POLLUTION.NONE;
    EffectorValues none2 = BUILDINGS.DECOR.NONE;
    EffectorValues noise = none1;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("CeilingLight", 1, 1, "ceilinglight_kanim", 10, 10f, tieR1, allMetals, 800f, BuildLocationRule.OnCeiling, none2, noise);
    buildingDef.RequiresPowerInput = true;
    buildingDef.EnergyConsumptionWhenActive = 10f;
    buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
    buildingDef.ViewMode = OverlayModes.Light.ID;
    buildingDef.AudioCategory = "Metal";
    return buildingDef;
  }

  public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
  {
    LightShapePreview lightShapePreview = go.AddComponent<LightShapePreview>();
    lightShapePreview.lux = 1800;
    lightShapePreview.radius = 8f;
    lightShapePreview.shape = LightShape.Cone;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) => go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.LightSource, false);

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGet<LoopingSounds>();
    Light2D light2D = go.AddOrGet<Light2D>();
    light2D.overlayColour = LIGHT2D.CEILINGLIGHT_OVERLAYCOLOR;
    light2D.Color = LIGHT2D.CEILINGLIGHT_COLOR;
    light2D.Range = 8f;
    light2D.Angle = 2.6f;
    light2D.Direction = LIGHT2D.CEILINGLIGHT_DIRECTION;
    light2D.Offset = LIGHT2D.CEILINGLIGHT_OFFSET;
    light2D.shape = LightShape.Cone;
    light2D.drawOverlay = true;
    light2D.Lux = 1800;
    go.AddOrGetDef<LightController.Def>();
  }
}
