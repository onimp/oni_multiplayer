// Decompiled with JetBrains decompiler
// Type: GravitasLabLightConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class GravitasLabLightConfig : IBuildingConfig
{
  public const string ID = "GravitasLabLight";

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR0 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER0;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues none1 = NOISE_POLLUTION.NONE;
    EffectorValues none2 = BUILDINGS.DECOR.NONE;
    EffectorValues noise = none1;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("GravitasLabLight", 1, 1, "gravitas_lab_light_kanim", 30, 10f, tieR0, allMetals, 2400f, BuildLocationRule.OnCeiling, none2, noise);
    buildingDef.ShowInBuildMenu = false;
    buildingDef.Entombable = false;
    buildingDef.Floodable = false;
    buildingDef.Invincible = true;
    buildingDef.AudioCategory = "Metal";
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag) => go.AddTag(GameTags.Gravitas);

  public override void DoPostConfigureComplete(GameObject go)
  {
  }
}
