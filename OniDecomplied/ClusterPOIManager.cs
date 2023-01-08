// Decompiled with JetBrains decompiler
// Type: ClusterPOIManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using ProcGenGame;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class ClusterPOIManager : KMonoBehaviour
{
  [Serialize]
  private List<Ref<ResearchDestination>> m_researchDestinations = new List<Ref<ResearchDestination>>();
  [Serialize]
  private Ref<TemporalTear> m_temporalTear = new Ref<TemporalTear>();
  private ClusterFogOfWarManager.Instance m_fowManager;

  private ClusterFogOfWarManager.Instance GetFOWManager()
  {
    if (this.m_fowManager == null)
      this.m_fowManager = ((Component) SaveGame.Instance).GetSMI<ClusterFogOfWarManager.Instance>();
    return this.m_fowManager;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (!DlcManager.FeatureClusterSpaceEnabled())
      return;
    UIScheduler.Instance.ScheduleNextFrame("UpgradeOldSaves", (Action<object>) (o => this.UpgradeOldSaves()));
  }

  public void RegisterTemporalTear(TemporalTear temporalTear) => this.m_temporalTear.Set(temporalTear);

  public bool HasTemporalTear() => Object.op_Inequality((Object) this.m_temporalTear.Get(), (Object) null);

  public TemporalTear GetTemporalTear() => this.m_temporalTear.Get();

  private void UpgradeOldSaves()
  {
    bool flag = false;
    foreach (KeyValuePair<AxialI, List<ClusterGridEntity>> cellContent in ClusterGrid.Instance.cellContents)
    {
      foreach (ClusterGridEntity clusterGridEntity in cellContent.Value)
      {
        if (Object.op_Implicit((Object) ((Component) clusterGridEntity).GetComponent<HarvestablePOIClusterGridEntity>()) || Object.op_Implicit((Object) ((Component) clusterGridEntity).GetComponent<ArtifactPOIClusterGridEntity>()))
        {
          flag = true;
          break;
        }
      }
      if (flag)
        break;
    }
    if (flag)
      return;
    ClusterManager.Instance.GetClusterPOIManager().SpawnSpacePOIsInLegacySave();
  }

  public void SpawnSpacePOIsInLegacySave()
  {
    Dictionary<int[], string[]> dictionary = new Dictionary<int[], string[]>();
    dictionary.Add(new int[2]{ 2, 3 }, new string[1]
    {
      "HarvestableSpacePOI_SandyOreField"
    });
    dictionary.Add(new int[2]{ 5, 7 }, new string[1]
    {
      "HarvestableSpacePOI_OrganicMassField"
    });
    dictionary.Add(new int[2]{ 8, 11 }, new string[5]
    {
      "HarvestableSpacePOI_GildedAsteroidField",
      "HarvestableSpacePOI_GlimmeringAsteroidField",
      "HarvestableSpacePOI_HeliumCloud",
      "HarvestableSpacePOI_OilyAsteroidField",
      "HarvestableSpacePOI_FrozenOreField"
    });
    dictionary.Add(new int[2]{ 10, 11 }, new string[2]
    {
      "HarvestableSpacePOI_RadioactiveGasCloud",
      "HarvestableSpacePOI_RadioactiveAsteroidField"
    });
    dictionary.Add(new int[2]{ 5, 7 }, new string[5]
    {
      "HarvestableSpacePOI_RockyAsteroidField",
      "HarvestableSpacePOI_InterstellarIceField",
      "HarvestableSpacePOI_InterstellarOcean",
      "HarvestableSpacePOI_SandyOreField",
      "HarvestableSpacePOI_SwampyOreField"
    });
    dictionary.Add(new int[2]{ 7, 11 }, new string[10]
    {
      "HarvestableSpacePOI_MetallicAsteroidField",
      "HarvestableSpacePOI_SatelliteField",
      "HarvestableSpacePOI_ChlorineCloud",
      "HarvestableSpacePOI_OxidizedAsteroidField",
      "HarvestableSpacePOI_OxygenRichAsteroidField",
      "HarvestableSpacePOI_GildedAsteroidField",
      "HarvestableSpacePOI_HeliumCloud",
      "HarvestableSpacePOI_OilyAsteroidField",
      "HarvestableSpacePOI_FrozenOreField",
      "HarvestableSpacePOI_RadioactiveAsteroidField"
    });
    List<AxialI> axialIList1 = new List<AxialI>();
    foreach (KeyValuePair<int[], string[]> keyValuePair in dictionary)
    {
      int[] key = keyValuePair.Key;
      string[] strArray = keyValuePair.Value;
      int num1 = Mathf.Min(key[0], ClusterGrid.Instance.numRings - 1);
      int num2 = Mathf.Min(key[1], ClusterGrid.Instance.numRings - 1);
      List<AxialI> rings = AxialUtil.GetRings(AxialI.ZERO, num1, num2);
      List<AxialI> axialIList2 = new List<AxialI>();
      foreach (AxialI axialI in rings)
      {
        ClusterGrid instance = ClusterGrid.Instance;
        Dictionary<AxialI, List<ClusterGridEntity>> cellContents = ClusterGrid.Instance.cellContents;
        List<ClusterGridEntity> cellContent = ClusterGrid.Instance.cellContents[axialI];
        if (ClusterGrid.Instance.cellContents[axialI].Count == 0 && Object.op_Equality((Object) ClusterGrid.Instance.GetVisibleEntityOfLayerAtAdjacentCell(axialI, EntityLayer.Asteroid), (Object) null))
          axialIList2.Add(axialI);
      }
      foreach (string str in strArray)
      {
        GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(str)), (GameObject) null, (string) null);
        AxialI axialI = axialIList2[Random.Range(0, axialIList2.Count - 1)];
        axialIList2.Remove(axialI);
        axialIList1.Add(axialI);
        gameObject.GetComponent<ClusterGridEntity>().Location = axialI;
        gameObject.SetActive(true);
      }
    }
    string[] strArray1 = new string[6]
    {
      "ArtifactSpacePOI_GravitasSpaceStation1",
      "ArtifactSpacePOI_GravitasSpaceStation4",
      "ArtifactSpacePOI_GravitasSpaceStation5",
      "ArtifactSpacePOI_GravitasSpaceStation6",
      "ArtifactSpacePOI_GravitasSpaceStation8",
      "ArtifactSpacePOI_RussellsTeapot"
    };
    int num3 = Mathf.Min(2, ClusterGrid.Instance.numRings - 1);
    int num4 = Mathf.Min(11, ClusterGrid.Instance.numRings - 1);
    List<AxialI> rings1 = AxialUtil.GetRings(AxialI.ZERO, num3, num4);
    List<AxialI> axialIList3 = new List<AxialI>();
    foreach (AxialI axialI in rings1)
    {
      if (ClusterGrid.Instance.cellContents[axialI].Count == 0 && Object.op_Equality((Object) ClusterGrid.Instance.GetVisibleEntityOfLayerAtAdjacentCell(axialI, EntityLayer.Asteroid), (Object) null) && !axialIList1.Contains(axialI))
        axialIList3.Add(axialI);
    }
    foreach (string str in strArray1)
    {
      GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(str)), (GameObject) null, (string) null);
      AxialI location = axialIList3[Random.Range(0, axialIList3.Count - 1)];
      axialIList3.Remove(location);
      HarvestablePOIClusterGridEntity component1 = gameObject.GetComponent<HarvestablePOIClusterGridEntity>();
      if (Object.op_Inequality((Object) component1, (Object) null))
        component1.Init(location);
      ArtifactPOIClusterGridEntity component2 = gameObject.GetComponent<ArtifactPOIClusterGridEntity>();
      if (Object.op_Inequality((Object) component2, (Object) null))
        component2.Init(location);
      gameObject.SetActive(true);
    }
  }

  public void PopulatePOIsFromWorldGen(Cluster clusterLayout)
  {
    foreach (KeyValuePair<AxialI, string> poiPlacement in clusterLayout.poiPlacements)
    {
      GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(poiPlacement.Value)), (GameObject) null, (string) null);
      gameObject.GetComponent<ClusterGridEntity>().Location = poiPlacement.Key;
      gameObject.SetActive(true);
    }
  }

  public void RevealTemporalTear()
  {
    if (Object.op_Equality((Object) this.m_temporalTear.Get(), (Object) null))
    {
      Debug.LogWarning((object) "This cluster has no temporal tear, but has the poi mechanism to reveal it");
    }
    else
    {
      AxialI location = this.m_temporalTear.Get().Location;
      this.GetFOWManager().RevealLocation(location, 1);
    }
  }

  public bool IsTemporalTearRevealed()
  {
    if (!Object.op_Equality((Object) this.m_temporalTear.Get(), (Object) null))
      return this.GetFOWManager().IsLocationRevealed(this.m_temporalTear.Get().Location);
    Debug.LogWarning((object) "This cluster has no temporal tear, but has the poi mechanism to reveal it");
    return false;
  }

  public void OpenTemporalTear(int openerWorldId)
  {
    if (Object.op_Equality((Object) this.m_temporalTear.Get(), (Object) null))
    {
      Debug.LogWarning((object) "This cluster has no temporal tear, but has the poi mechanism to open it");
    }
    else
    {
      if (this.m_temporalTear.Get().IsOpen())
        return;
      this.m_temporalTear.Get().Open();
      ((Component) ClusterManager.Instance.GetWorld(openerWorldId)).GetSMI<GameplaySeasonManager.Instance>().StartNewSeason(Db.Get().GameplaySeasons.TemporalTearMeteorShowers);
    }
  }

  public bool HasTemporalTearConsumedCraft() => !Object.op_Equality((Object) this.m_temporalTear.Get(), (Object) null) && this.m_temporalTear.Get().HasConsumedCraft();

  public bool IsTemporalTearOpen() => !Object.op_Equality((Object) this.m_temporalTear.Get(), (Object) null) && this.m_temporalTear.Get().IsOpen();
}
