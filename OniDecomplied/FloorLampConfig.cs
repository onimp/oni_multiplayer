// Decompiled with JetBrains decompiler
// Type: FloorLampConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class FloorLampConfig : IBuildingConfig
{
  public const string ID = "FloorLamp";

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR1_1 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues tieR1_2 = BUILDINGS.DECOR.BONUS.TIER1;
    EffectorValues noise = none;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("FloorLamp", 1, 2, "floorlamp_kanim", 10, 10f, tieR1_1, allMetals, 800f, BuildLocationRule.OnFloor, tieR1_2, noise);
    buildingDef.RequiresPowerInput = true;
    buildingDef.EnergyConsumptionWhenActive = 8f;
    buildingDef.SelfHeatKilowattsWhenActive = 0.5f;
    buildingDef.ViewMode = OverlayModes.Light.ID;
    buildingDef.AudioCategory = "Metal";
    return buildingDef;
  }

  public override void DoPostConfigurePreview(BuildingDef def, GameObject go)
  {
    LightShapePreview lightShapePreview = go.AddComponent<LightShapePreview>();
    lightShapePreview.lux = 1000;
    lightShapePreview.radius = 4f;
    lightShapePreview.shape = LightShape.Circle;
    lightShapePreview.offset = new CellOffset((int) def.BuildingComplete.GetComponent<Light2D>().Offset.x, (int) def.BuildingComplete.GetComponent<Light2D>().Offset.y);
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) => go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.LightSource, false);

  public override void DoPostConfigureComplete(GameObject go)
  {
    go.AddOrGet<EnergyConsumer>();
    go.AddOrGet<LoopingSounds>();
    Light2D light2D = go.AddOrGet<Light2D>();
    light2D.overlayColour = LIGHT2D.FLOORLAMP_OVERLAYCOLOR;
    light2D.Color = LIGHT2D.FLOORLAMP_COLOR;
    light2D.Range = 4f;
    light2D.Angle = 0.0f;
    light2D.Direction = LIGHT2D.FLOORLAMP_DIRECTION;
    light2D.Offset = LIGHT2D.FLOORLAMP_OFFSET;
    light2D.shape = LightShape.Circle;
    light2D.drawOverlay = true;
    go.AddOrGetDef<LightController.Def>();
  }
}
