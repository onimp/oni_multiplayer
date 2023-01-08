// Decompiled with JetBrains decompiler
// Type: EggCrackerConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

[EntityConfigOrder(2)]
public class EggCrackerConfig : IBuildingConfig
{
  public const string ID = "EggCracker";

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR1 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER1;
    string[] rawMetals = MATERIALS.RAW_METALS;
    EffectorValues none = NOISE_POLLUTION.NONE;
    EffectorValues tieR0 = BUILDINGS.DECOR.BONUS.TIER0;
    EffectorValues noise = none;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("EggCracker", 2, 2, "egg_cracker_kanim", 30, 10f, tieR1, rawMetals, 1600f, BuildLocationRule.OnFloor, tieR0, noise);
    buildingDef.AudioCategory = "Metal";
    buildingDef.SceneLayer = Grid.SceneLayer.Building;
    buildingDef.ForegroundLayer = Grid.SceneLayer.BuildingFront;
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.AddOrGet<DropAllWorkable>();
    go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
    go.AddOrGet<KBatchedAnimController>().SetSymbolVisiblity(KAnimHashedString.op_Implicit("snapto_egg"), false);
    ComplexFabricator fabricator = go.AddOrGet<ComplexFabricator>();
    fabricator.labelByResult = false;
    fabricator.sideScreenStyle = ComplexFabricatorSideScreen.StyleSetting.ListQueueHybrid;
    fabricator.duplicantOperated = true;
    go.AddOrGet<FabricatorIngredientStatusManager>();
    go.AddOrGet<CopyBuildingSettings>();
    ComplexFabricatorWorkable fabricatorWorkable = go.AddOrGet<ComplexFabricatorWorkable>();
    BuildingTemplates.CreateComplexFabricatorStorage(go, fabricator);
    KAnimFile[] kanimFileArray = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_egg_cracker_kanim"))
    };
    fabricatorWorkable.overrideAnims = kanimFileArray;
    fabricator.outputOffset = new Vector3(1f, 1f, 0.0f);
    Prioritizable.AddRef(go);
    go.AddOrGet<EggCracker>();
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
  }
}
