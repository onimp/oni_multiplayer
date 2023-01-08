// Decompiled with JetBrains decompiler
// Type: FishDeliveryPointConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class FishDeliveryPointConfig : IBuildingConfig
{
  public const string ID = "FishDeliveryPoint";

  public override BuildingDef CreateBuildingDef()
  {
    BuildingDef buildingDef = BuildingTemplates.CreateBuildingDef("FishDeliveryPoint", 1, 3, "fishrelocator_kanim", 10, 10f, BUILDINGS.CONSTRUCTION_MASS_KG.TIER1, MATERIALS.RAW_METALS, 1600f, BuildLocationRule.Anywhere, BUILDINGS.DECOR.PENALTY.TIER2, NOISE_POLLUTION.NOISY.TIER0);
    buildingDef.AudioCategory = "Metal";
    buildingDef.Entombable = true;
    buildingDef.Floodable = true;
    buildingDef.ForegroundLayer = Grid.SceneLayer.TileMain;
    buildingDef.ViewMode = OverlayModes.Rooms.ID;
    return buildingDef;
  }

  public override void ConfigureBuildingTemplate(GameObject go, Tag prefab_tag)
  {
    go.GetComponent<KPrefabID>().AddTag(RoomConstraints.ConstraintTags.CreatureRelocator, false);
    Storage storage = go.AddOrGet<Storage>();
    storage.allowItemRemoval = false;
    storage.showDescriptor = true;
    storage.storageFilters = STORAGEFILTERS.SWIMMING_CREATURES;
    storage.workAnims = new HashedString[1]
    {
      new HashedString("working_pre")
    };
    storage.workAnimPlayMode = (KAnim.PlayMode) 1;
    storage.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_fishrelocator_kanim"))
    };
    storage.synchronizeAnims = false;
    storage.useGunForDelivery = false;
    storage.allowSettingOnlyFetchMarkedItems = false;
    storage.faceTargetWhenWorking = false;
    CreatureDeliveryPoint creatureDeliveryPoint = go.AddOrGet<CreatureDeliveryPoint>();
    creatureDeliveryPoint.deliveryOffsets = new CellOffset[1]
    {
      new CellOffset(0, 1)
    };
    creatureDeliveryPoint.spawnOffset = new CellOffset(0, -1);
    creatureDeliveryPoint.playAnimsOnFetch = true;
    go.AddOrGet<TreeFilterable>();
  }

  public override void DoPostConfigureComplete(GameObject go) => go.AddOrGetDef<MakeBaseSolid.Def>().solidOffsets = new CellOffset[1]
  {
    new CellOffset(0, 0)
  };
}
