// Decompiled with JetBrains decompiler
// Type: EscapePodConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class EscapePodConfig : IEntityConfig
{
  public const string ID = "EscapePod";
  public const float MASS = 100f;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public GameObject CreatePrefab()
  {
    string name = (string) STRINGS.BUILDINGS.PREFABS.ESCAPEPOD.NAME;
    string desc = (string) STRINGS.BUILDINGS.PREFABS.ESCAPEPOD.DESC;
    EffectorValues tieR0_1 = TUNING.BUILDINGS.DECOR.BONUS.TIER0;
    EffectorValues tieR0_2 = NOISE_POLLUTION.NOISY.TIER0;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit("escape_pod_kanim"));
    EffectorValues decor = tieR0_1;
    EffectorValues noise = tieR0_2;
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity("EscapePod", name, desc, 100f, anim, "grounded", Grid.SceneLayer.Building, 1, 2, decor, noise);
    placedEntity.AddOrGet<KBatchedAnimController>().fgLayer = Grid.SceneLayer.BuildingFront;
    TravellingCargoLander.Def def1 = placedEntity.AddOrGetDef<TravellingCargoLander.Def>();
    def1.landerWidth = 1;
    def1.landingSpeed = 15f;
    def1.deployOnLanding = true;
    CargoDropperMinion.Def def2 = placedEntity.AddOrGetDef<CargoDropperMinion.Def>();
    def2.kAnimName = "anim_interacts_escape_pod_kanim";
    def2.animName = "deploying";
    def2.animLayer = Grid.SceneLayer.BuildingUse;
    def2.notifyOnJettison = true;
    BallisticClusterGridEntity clusterGridEntity = placedEntity.AddOrGet<BallisticClusterGridEntity>();
    clusterGridEntity.clusterAnimName = "escape_pod01_kanim";
    clusterGridEntity.isWorldEntity = true;
    clusterGridEntity.nameKey = new StringKey("STRINGS.BUILDINGS.PREFABS.ESCAPEPOD.NAME");
    ClusterDestinationSelector destinationSelector = placedEntity.AddOrGet<ClusterDestinationSelector>();
    destinationSelector.assignable = false;
    destinationSelector.shouldPointTowardsPath = true;
    destinationSelector.requireAsteroidDestination = true;
    destinationSelector.canNavigateFogOfWar = true;
    placedEntity.AddOrGet<ClusterTraveler>();
    placedEntity.AddOrGet<MinionStorage>();
    placedEntity.AddOrGet<Prioritizable>();
    Prioritizable.AddRef(placedEntity);
    placedEntity.AddOrGet<Operational>();
    placedEntity.AddOrGet<Deconstructable>().audioSize = "large";
    return placedEntity;
  }

  public void OnPrefabInit(GameObject inst)
  {
    OccupyArea component = inst.GetComponent<OccupyArea>();
    component.ApplyToCells = false;
    component.objectLayers = new ObjectLayer[1]
    {
      ObjectLayer.Building
    };
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
