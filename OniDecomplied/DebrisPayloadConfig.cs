// Decompiled with JetBrains decompiler
// Type: DebrisPayloadConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class DebrisPayloadConfig : IEntityConfig
{
  public const string ID = "DebrisPayload";
  public const float MASS = 100f;
  public const float MAX_STORAGE_KG_MASS = 5000f;
  public const float STARMAP_SPEED = 10f;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public GameObject CreatePrefab()
  {
    string name = (string) ITEMS.DEBRISPAYLOAD.NAME;
    string desc = (string) ITEMS.DEBRISPAYLOAD.DESC;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit("rocket_debris_combined_kanim"));
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.IgnoreMaterialCategory);
    additionalTags.Add(GameTags.Experimental);
    GameObject looseEntity = EntityTemplates.CreateLooseEntity("DebrisPayload", name, desc, 100f, true, anim, "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, isPickupable: true, additionalTags: additionalTags);
    RailGunPayload.Def def = looseEntity.AddOrGetDef<RailGunPayload.Def>();
    def.attractToBeacons = false;
    def.clusterAnimSymbolSwapTarget = "debris1";
    def.randomClusterSymbolSwaps = new List<string>()
    {
      "debris1",
      "debris2",
      "debris3"
    };
    def.worldAnimSymbolSwapTarget = "debris";
    def.randomWorldSymbolSwaps = new List<string>()
    {
      "debris",
      "2_debris",
      "3_debris"
    };
    SymbolOverrideControllerUtil.AddToPrefab(looseEntity);
    looseEntity.AddOrGet<LoopingSounds>();
    Storage defaultStorage = BuildingTemplates.CreateDefaultStorage(looseEntity);
    defaultStorage.showInUI = true;
    defaultStorage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
    defaultStorage.allowSettingOnlyFetchMarkedItems = false;
    defaultStorage.allowItemRemoval = false;
    defaultStorage.capacityKg = 5000f;
    DropAllWorkable dropAllWorkable = looseEntity.AddOrGet<DropAllWorkable>();
    dropAllWorkable.dropWorkTime = 30f;
    dropAllWorkable.choreTypeID = Db.Get().ChoreTypes.Fetch.Id;
    dropAllWorkable.ConfigureMultitoolContext(HashedString.op_Implicit("build"), Tag.op_Implicit(EffectConfigs.BuildSplashId));
    ClusterDestinationSelector destinationSelector = looseEntity.AddOrGet<ClusterDestinationSelector>();
    destinationSelector.assignable = false;
    destinationSelector.shouldPointTowardsPath = true;
    destinationSelector.requireAsteroidDestination = true;
    destinationSelector.canNavigateFogOfWar = true;
    BallisticClusterGridEntity clusterGridEntity = looseEntity.AddOrGet<BallisticClusterGridEntity>();
    clusterGridEntity.clusterAnimName = "rocket_debris_kanim";
    clusterGridEntity.isWorldEntity = true;
    clusterGridEntity.nameKey = new StringKey("STRINGS.ITEMS.DEBRISPAYLOAD.NAME");
    looseEntity.AddOrGet<ClusterTraveler>();
    return looseEntity;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
