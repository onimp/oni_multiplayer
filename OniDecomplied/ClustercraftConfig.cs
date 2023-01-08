// Decompiled with JetBrains decompiler
// Type: ClustercraftConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class ClustercraftConfig : IEntityConfig
{
  public const string ID = "Clustercraft";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public GameObject CreatePrefab()
  {
    GameObject entity = EntityTemplates.CreateEntity("Clustercraft", "Clustercraft");
    SaveLoadRoot saveLoadRoot = entity.AddOrGet<SaveLoadRoot>();
    saveLoadRoot.DeclareOptionalComponent<WorldInventory>();
    saveLoadRoot.DeclareOptionalComponent<WorldContainer>();
    saveLoadRoot.DeclareOptionalComponent<OrbitalMechanics>();
    entity.AddOrGet<Clustercraft>();
    entity.AddOrGet<CraftModuleInterface>();
    entity.AddOrGet<UserNameable>();
    RocketClusterDestinationSelector destinationSelector = entity.AddOrGet<RocketClusterDestinationSelector>();
    destinationSelector.requireLaunchPadOnAsteroidDestination = true;
    destinationSelector.assignable = true;
    destinationSelector.shouldPointTowardsPath = true;
    entity.AddOrGet<ClusterTraveler>().stopAndNotifyWhenPathChanges = true;
    entity.AddOrGetDef<AlertStateManager.Def>();
    entity.AddOrGet<Notifier>();
    entity.AddOrGetDef<RocketSelfDestructMonitor.Def>();
    return entity;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
