// Decompiled with JetBrains decompiler
// Type: ArtifactFinder
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/ArtifactFinder")]
public class ArtifactFinder : KMonoBehaviour
{
  public const string ID = "ArtifactFinder";
  [MyCmpReq]
  private MinionStorage minionStorage;
  private static readonly EventSystem.IntraObjectHandler<ArtifactFinder> OnLandDelegate = new EventSystem.IntraObjectHandler<ArtifactFinder>((Action<ArtifactFinder, object>) ((component, data) => component.OnLand(data)));

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.Subscribe<ArtifactFinder>(-887025858, ArtifactFinder.OnLandDelegate);
  }

  public ArtifactTier GetArtifactDropTier(
    StoredMinionIdentity minionID,
    SpaceDestination destination)
  {
    ArtifactDropRate artifactDropTable = destination.GetDestinationType().artifactDropTable;
    bool flag = minionID.traitIDs.Contains("Archaeologist");
    if (artifactDropTable != null)
    {
      float totalWeight = artifactDropTable.totalWeight;
      if (flag)
        totalWeight -= artifactDropTable.GetTierWeight(DECOR.SPACEARTIFACT.TIER_NONE);
      float num = Random.value * totalWeight;
      foreach (Tuple<ArtifactTier, float> rate in artifactDropTable.rates)
      {
        if (!flag || flag && rate.first != DECOR.SPACEARTIFACT.TIER_NONE)
          num -= rate.second;
        if ((double) num <= 0.0)
          return rate.first;
      }
    }
    return DECOR.SPACEARTIFACT.TIER0;
  }

  public List<string> GetArtifactsOfTier(ArtifactTier tier)
  {
    List<string> artifactsOfTier = new List<string>();
    foreach (KeyValuePair<ArtifactType, List<string>> artifactItem in ArtifactConfig.artifactItems)
    {
      foreach (string str in artifactItem.Value)
      {
        if (Assets.GetPrefab(TagExtensions.ToTag(str)).GetComponent<SpaceArtifact>().GetArtifactTier() == tier)
          artifactsOfTier.Add(str);
      }
    }
    return artifactsOfTier;
  }

  public string SearchForArtifact(StoredMinionIdentity minionID, SpaceDestination destination)
  {
    ArtifactTier artifactDropTier = this.GetArtifactDropTier(minionID, destination);
    if (artifactDropTier == DECOR.SPACEARTIFACT.TIER_NONE)
      return (string) null;
    List<string> artifactsOfTier = this.GetArtifactsOfTier(artifactDropTier);
    return artifactsOfTier[Random.Range(0, artifactsOfTier.Count)];
  }

  public void OnLand(object data)
  {
    SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(SpacecraftManager.instance.GetSpacecraftID(((Component) ((Component) this).GetComponent<RocketModule>().conditionManager).GetComponent<ILaunchableRocket>()));
    foreach (MinionStorage.Info info in this.minionStorage.GetStoredMinionInfo())
    {
      string str = this.SearchForArtifact(info.serializedMinion.Get<StoredMinionIdentity>(), spacecraftDestination);
      if (str != null)
        GameUtil.KInstantiate(Assets.GetPrefab(TagExtensions.ToTag(str)), TransformExtensions.GetPosition(((Component) this).gameObject.transform), Grid.SceneLayer.Ore).SetActive(true);
    }
  }
}
