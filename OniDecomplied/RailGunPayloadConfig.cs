// Decompiled with JetBrains decompiler
// Type: RailGunPayloadConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class RailGunPayloadConfig : IEntityConfig
{
  public const string ID = "RailGunPayload";
  public const float MASS = 200f;
  public const int LANDING_EDGE_PADDING = 3;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public GameObject CreatePrefab()
  {
    string name = (string) ITEMS.RAILGUNPAYLOAD.NAME;
    string desc = (string) ITEMS.RAILGUNPAYLOAD.DESC;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit("railgun_capsule_kanim"));
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.IgnoreMaterialCategory);
    additionalTags.Add(GameTags.Experimental);
    GameObject looseEntity = EntityTemplates.CreateLooseEntity("RailGunPayload", name, desc, 200f, true, anim, "object", Grid.SceneLayer.Front, EntityTemplates.CollisionShape.RECTANGLE, 0.75f, isPickupable: true, additionalTags: additionalTags);
    looseEntity.AddOrGetDef<RailGunPayload.Def>().attractToBeacons = true;
    looseEntity.AddComponent<LoopingSounds>();
    Storage defaultStorage = BuildingTemplates.CreateDefaultStorage(looseEntity);
    defaultStorage.showInUI = true;
    defaultStorage.SetDefaultStoredItemModifiers(Storage.StandardSealedStorage);
    defaultStorage.allowSettingOnlyFetchMarkedItems = false;
    defaultStorage.allowItemRemoval = false;
    defaultStorage.capacityKg = 200f;
    DropAllWorkable dropAllWorkable = looseEntity.AddOrGet<DropAllWorkable>();
    dropAllWorkable.dropWorkTime = 30f;
    dropAllWorkable.choreTypeID = Db.Get().ChoreTypes.Fetch.Id;
    dropAllWorkable.ConfigureMultitoolContext(HashedString.op_Implicit("build"), Tag.op_Implicit(EffectConfigs.BuildSplashId));
    ClusterDestinationSelector destinationSelector = looseEntity.AddOrGet<ClusterDestinationSelector>();
    destinationSelector.assignable = false;
    destinationSelector.shouldPointTowardsPath = true;
    destinationSelector.requireAsteroidDestination = true;
    BallisticClusterGridEntity clusterGridEntity = looseEntity.AddOrGet<BallisticClusterGridEntity>();
    clusterGridEntity.clusterAnimName = "payload01_kanim";
    clusterGridEntity.isWorldEntity = true;
    clusterGridEntity.nameKey = new StringKey("STRINGS.ITEMS.RAILGUNPAYLOAD.NAME");
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
