// Decompiled with JetBrains decompiler
// Type: GravitasContainerConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class GravitasContainerConfig : IBuildingConfig
{
  public const string ID = "GravitasContainer";
  private const float WORK_TIME = 1.5f;

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR3 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER3;
    string[] allMetals = MATERIALS.ALL_METALS;
    EffectorValues none1 = NOISE_POLLUTION.NONE;
    EffectorValues none2 = BUILDINGS.DECOR.NONE;
    EffectorValues noise = none1;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("GravitasContainer", 2, 2, "gravitas_container_kanim", 30, 10f, tieR3, allMetals, 2400f, BuildLocationRule.OnFloor, none2, noise);
    buildingDef.ShowInBuildMenu = false;
    buildingDef.Entombable = false;
    buildingDef.Floodable = false;
    buildingDef.Invincible = true;
    buildingDef.AudioCategory = "Metal";
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.AddTag(GameTags.Gravitas);
    go.AddOrGet<KBatchedAnimController>().sceneLayer = Grid.SceneLayer.Building;
    Prioritizable.AddRef(go);
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    PajamaDispenser pajamaDispenser = go.AddComponent<PajamaDispenser>();
    pajamaDispenser.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_gravitas_container_kanim"))
    };
    pajamaDispenser.SetWorkTime(30f);
  }
}
