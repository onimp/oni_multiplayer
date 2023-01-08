// Decompiled with JetBrains decompiler
// Type: LiquidPumpingStationConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class LiquidPumpingStationConfig : IBuildingConfig
{
  public const string ID = "LiquidPumpingStation";

  public override BuildingDef CreateBuildingDef()
  {
    float[] tieR4 = BUILDINGS.CONSTRUCTION_MASS_KG.TIER4;
    string[] rawMinerals = MATERIALS.RAW_MINERALS;
    EffectorValues none1 = NOISE_POLLUTION.NONE;
    EffectorValues none2 = BUILDINGS.DECOR.NONE;
    EffectorValues noise = none1;
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("LiquidPumpingStation", 2, 4, "waterpump_kanim", 100, 10f, tieR4, rawMinerals, 1600f, BuildLocationRule.Anywhere, none2, noise);
    buildingDef.Floodable = false;
    buildingDef.Entombable = true;
    buildingDef.AudioCategory = "Metal";
    buildingDef.AudioSize = "large";
    buildingDef.UtilityInputOffset = new CellOffset(0, 0);
    buildingDef.UtilityOutputOffset = new CellOffset(0, 0);
    buildingDef.DefaultAnimState = "on";
    buildingDef.ShowInBuildMenu = true;
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.AddOrGet<LoopingSounds>();
    go.AddOrGet<BuildingComplete>().isManuallyOperated = true;
    go.AddOrGet<LiquidPumpingStation>().overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_waterpump_kanim"))
    };
    Storage storage = go.AddOrGet<Storage>();
    storage.showInUI = false;
    storage.allowItemRemoval = true;
    storage.showDescriptor = true;
    storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
    go.AddTag(GameTags.CorrosionProof);
  }

  private static void AddGuide(GameObject go, bool occupy_tiles)
  {
    GameObject gameObject = new GameObject();
    gameObject.transform.parent = go.transform;
    TransformExtensions.SetLocalPosition(gameObject.transform, Vector3.zero);
    KBatchedAnimController kbatchedAnimController = gameObject.AddComponent<KBatchedAnimController>();
    kbatchedAnimController.Offset = go.GetComponent<Building>().Def.GetVisualizerOffset();
    kbatchedAnimController.AnimFiles = new KAnimFile[1]
    {
      Assets.GetAnim(new HashedString("waterpump_kanim"))
    };
    kbatchedAnimController.initialAnim = "place_guide";
    kbatchedAnimController.visibilityType = KAnimControllerBase.VisibilityType.OffscreenUpdate;
    kbatchedAnimController.isMovable = true;
    PumpingStationGuide pumpingStationGuide = gameObject.AddComponent<PumpingStationGuide>();
    pumpingStationGuide.parent = go;
    pumpingStationGuide.occupyTiles = occupy_tiles;
  }

  public override void DoPostConfigureComplete(GameObject go)
  {
    LiquidPumpingStationConfig.AddGuide(go.GetComponent<Building>().Def.BuildingPreview, false);
    LiquidPumpingStationConfig.AddGuide(go.GetComponent<Building>().Def.BuildingUnderConstruction, true);
    go.AddOrGet<FakeFloorAdder>().floorOffsets = new CellOffset[2]
    {
      new CellOffset(0, 0),
      new CellOffset(1, 0)
    };
  }
}
