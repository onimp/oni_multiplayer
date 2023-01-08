// Decompiled with JetBrains decompiler
// Type: DecontaminationShowerConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class DecontaminationShowerConfig : IBuildingConfig
{
  public const string ID = "DecontaminationShower";
  private const float MASS_PER_USE = 100f;
  private const int DISEASE_REMOVAL_COUNT = 1000000;
  private const float WATER_PER_USE = 100f;
  private const int USES_PER_FLUSH = 1;
  private const float WORK_TIME = 15f;
  private const SimHashes CONSUMED_ELEMENT = SimHashes.Water;
  private const SimHashes PRODUCED_ELEMENT = SimHashes.DirtyWater;

  public override string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public override BuildingDef CreateBuildingDef()
  {
    string[] radiationContainment = MATERIALS.RADIATION_CONTAINMENT;
    float[] construction_mass = new float[2]
    {
      BUILDINGS.CONSTRUCTION_MASS_KG.TIER5[0],
      BUILDINGS.CONSTRUCTION_MASS_KG.TIER2[0]
    };
    string[] construction_materials = radiationContainment;
    EffectorValues tieR0 = NOISE_POLLUTION.NOISY.TIER0;
    EffectorValues tieR3 = BUILDINGS.DECOR.PENALTY.TIER3;
    EffectorValues noise = tieR0;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("DecontaminationShower", 2, 4, "decontamination_shower_kanim", 250, 120f, construction_mass, construction_materials, 1600f, BuildLocationRule.OnFloor, tieR3, noise);
    buildingDef.InputConduitType = ConduitType.Liquid;
    buildingDef.ViewMode = OverlayModes.LiquidConduits.ID;
    buildingDef.AudioCategory = "Metal";
    buildingDef.UtilityInputOffset = new CellOffset(1, 2);
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    KBatchedAnimController kbatchedAnimController = go.AddOrGet<KBatchedAnimController>();
    kbatchedAnimController.sceneLayer = Grid.SceneLayer.BuildingBack;
    kbatchedAnimController.fgLayer = Grid.SceneLayer.BuildingFront;
    HandSanitizer handSanitizer = go.AddOrGet<HandSanitizer>();
    handSanitizer.massConsumedPerUse = 100f;
    handSanitizer.consumedElement = SimHashes.Water;
    handSanitizer.outputElement = SimHashes.DirtyWater;
    handSanitizer.diseaseRemovalCount = 1000000;
    handSanitizer.maxUses = 1;
    handSanitizer.canSanitizeSuit = true;
    handSanitizer.canSanitizeStorage = true;
    go.AddOrGet<DirectionControl>();
    HandSanitizer.Work work = go.AddOrGet<HandSanitizer.Work>();
    work.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_decontamination_shower_kanim"))
    };
    work.workLayer = Grid.SceneLayer.BuildingUse;
    work.workTime = 15f;
    work.trackUses = true;
    work.removeIrritation = true;
    ConduitConsumer conduitConsumer = go.AddOrGet<ConduitConsumer>();
    conduitConsumer.conduitType = ConduitType.Liquid;
    conduitConsumer.capacityTag = ElementLoader.FindElementByHash(SimHashes.Water).tag;
    conduitConsumer.capacityKG = 100f;
    conduitConsumer.wrongElementResult = ConduitConsumer.WrongElementResult.Store;
    AutoStorageDropper.Def def = go.AddOrGetDef<AutoStorageDropper.Def>();
    def.elementFilter = new SimHashes[1]
    {
      SimHashes.DirtyWater
    };
    def.dropOffset = new CellOffset(1, 0);
    go.AddOrGet<Storage>().SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
  }
}
