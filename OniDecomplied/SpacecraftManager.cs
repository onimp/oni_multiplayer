// Decompiled with JetBrains decompiler
// Type: SpacecraftManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/SpacecraftManager")]
public class SpacecraftManager : KMonoBehaviour, ISim1000ms
{
  public static SpacecraftManager instance;
  [Serialize]
  private List<Spacecraft> spacecraft = new List<Spacecraft>();
  [Serialize]
  private int nextSpacecraftID;
  public const int INVALID_DESTINATION_ID = -1;
  [Serialize]
  private int analyzeDestinationID = -1;
  [Serialize]
  public bool hasVisitedWormHole;
  [Serialize]
  public List<SpaceDestination> destinations;
  [Serialize]
  public Dictionary<int, int> savedSpacecraftDestinations;
  [Serialize]
  public bool destinationsGenerated;
  [Serialize]
  public Dictionary<int, float> destinationAnalysisScores = new Dictionary<int, float>();

  public static void DestroyInstance() => SpacecraftManager.instance = (SpacecraftManager) null;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    SpacecraftManager.instance = this;
    if (this.savedSpacecraftDestinations != null)
      return;
    this.savedSpacecraftDestinations = new Dictionary<int, int>();
  }

  private void GenerateFixedDestinations()
  {
    SpaceDestinationTypes destinationTypes = Db.Get().SpaceDestinationTypes;
    if (this.destinations != null)
      return;
    this.destinations = new List<SpaceDestination>()
    {
      new SpaceDestination(0, destinationTypes.CarbonaceousAsteroid.Id, 0),
      new SpaceDestination(1, destinationTypes.CarbonaceousAsteroid.Id, 0),
      new SpaceDestination(2, destinationTypes.MetallicAsteroid.Id, 1),
      new SpaceDestination(3, destinationTypes.RockyAsteroid.Id, 2),
      new SpaceDestination(4, destinationTypes.IcyDwarf.Id, 3),
      new SpaceDestination(5, destinationTypes.OrganicDwarf.Id, 4)
    };
  }

  private void GenerateRandomDestinations()
  {
    KRandom krandom = new KRandom(SaveLoader.Instance.clusterDetailSave.globalWorldSeed);
    SpaceDestinationTypes destinationTypes = Db.Get().SpaceDestinationTypes;
    List<List<string>> stringListList = new List<List<string>>()
    {
      new List<string>(),
      new List<string>() { destinationTypes.OilyAsteroid.Id },
      new List<string>() { destinationTypes.Satellite.Id },
      new List<string>()
      {
        destinationTypes.Satellite.Id,
        destinationTypes.RockyAsteroid.Id,
        destinationTypes.CarbonaceousAsteroid.Id,
        destinationTypes.ForestPlanet.Id
      },
      new List<string>()
      {
        destinationTypes.MetallicAsteroid.Id,
        destinationTypes.RockyAsteroid.Id,
        destinationTypes.CarbonaceousAsteroid.Id,
        destinationTypes.SaltDwarf.Id
      },
      new List<string>()
      {
        destinationTypes.MetallicAsteroid.Id,
        destinationTypes.RockyAsteroid.Id,
        destinationTypes.CarbonaceousAsteroid.Id,
        destinationTypes.IcyDwarf.Id,
        destinationTypes.OrganicDwarf.Id
      },
      new List<string>()
      {
        destinationTypes.IcyDwarf.Id,
        destinationTypes.OrganicDwarf.Id,
        destinationTypes.DustyMoon.Id,
        destinationTypes.ChlorinePlanet.Id,
        destinationTypes.RedDwarf.Id
      },
      new List<string>()
      {
        destinationTypes.DustyMoon.Id,
        destinationTypes.TerraPlanet.Id,
        destinationTypes.VolcanoPlanet.Id
      },
      new List<string>()
      {
        destinationTypes.TerraPlanet.Id,
        destinationTypes.GasGiant.Id,
        destinationTypes.IceGiant.Id,
        destinationTypes.RustPlanet.Id
      },
      new List<string>()
      {
        destinationTypes.GasGiant.Id,
        destinationTypes.IceGiant.Id,
        destinationTypes.HydrogenGiant.Id
      },
      new List<string>()
      {
        destinationTypes.RustPlanet.Id,
        destinationTypes.VolcanoPlanet.Id,
        destinationTypes.RockyAsteroid.Id,
        destinationTypes.TerraPlanet.Id,
        destinationTypes.MetallicAsteroid.Id
      },
      new List<string>()
      {
        destinationTypes.ShinyPlanet.Id,
        destinationTypes.MetallicAsteroid.Id,
        destinationTypes.RockyAsteroid.Id
      },
      new List<string>()
      {
        destinationTypes.GoldAsteroid.Id,
        destinationTypes.OrganicDwarf.Id,
        destinationTypes.ForestPlanet.Id,
        destinationTypes.ChlorinePlanet.Id
      },
      new List<string>()
      {
        destinationTypes.IcyDwarf.Id,
        destinationTypes.MetallicAsteroid.Id,
        destinationTypes.DustyMoon.Id,
        destinationTypes.VolcanoPlanet.Id,
        destinationTypes.IceGiant.Id
      },
      new List<string>()
      {
        destinationTypes.ShinyPlanet.Id,
        destinationTypes.RedDwarf.Id,
        destinationTypes.RockyAsteroid.Id,
        destinationTypes.GasGiant.Id
      },
      new List<string>()
      {
        destinationTypes.HydrogenGiant.Id,
        destinationTypes.ForestPlanet.Id,
        destinationTypes.OilyAsteroid.Id
      },
      new List<string>()
      {
        destinationTypes.GoldAsteroid.Id,
        destinationTypes.SaltDwarf.Id,
        destinationTypes.TerraPlanet.Id,
        destinationTypes.VolcanoPlanet.Id
      }
    };
    List<int> intList = new List<int>();
    int num1 = 3;
    int num2 = 15;
    int num3 = 25;
    for (int index1 = 0; index1 < stringListList.Count; ++index1)
    {
      if (stringListList[index1].Count != 0)
      {
        for (int index2 = 0; index2 < num1; ++index2)
          intList.Add(index1);
      }
    }
    int num4 = krandom.Next(num2, num3);
    for (int index3 = 0; index3 < num4; ++index3)
    {
      int index4 = krandom.Next(0, intList.Count - 1);
      int num5 = intList[index4];
      intList.RemoveAt(index4);
      List<string> stringList = stringListList[num5];
      this.destinations.Add(new SpaceDestination(this.destinations.Count, stringList[krandom.Next(0, stringList.Count)], num5));
    }
    this.destinations.Add(new SpaceDestination(this.destinations.Count, Db.Get().SpaceDestinationTypes.Earth.Id, 4));
    this.destinations.Add(new SpaceDestination(this.destinations.Count, Db.Get().SpaceDestinationTypes.Wormhole.Id, stringListList.Count));
  }

  private void RestoreDestinations()
  {
    if (this.destinationsGenerated)
      return;
    this.GenerateFixedDestinations();
    this.GenerateRandomDestinations();
    this.destinations.Sort((Comparison<SpaceDestination>) ((a, b) => a.distance.CompareTo(b.distance)));
    List<float> floatList = new List<float>();
    for (int index = 0; index < 10; ++index)
      floatList.Add((float) index / 10f);
    for (int index1 = 0; index1 < 20; ++index1)
    {
      Util.Shuffle<float>((IList<float>) floatList);
      int index2 = 0;
      foreach (SpaceDestination destination in this.destinations)
      {
        if (destination.distance == index1)
        {
          ++index2;
          destination.startingOrbitPercentage = floatList[index2];
        }
      }
    }
    this.destinationsGenerated = true;
  }

  public SpaceDestination GetSpacecraftDestination(LaunchConditionManager lcm) => this.GetSpacecraftDestination(this.GetSpacecraftFromLaunchConditionManager(lcm).id);

  public SpaceDestination GetSpacecraftDestination(int spacecraftID)
  {
    this.CleanSavedSpacecraftDestinations();
    return this.savedSpacecraftDestinations.ContainsKey(spacecraftID) ? this.GetDestination(this.savedSpacecraftDestinations[spacecraftID]) : (SpaceDestination) null;
  }

  public List<int> GetSpacecraftsForDestination(SpaceDestination destination)
  {
    this.CleanSavedSpacecraftDestinations();
    List<int> spacecraftsForDestination = new List<int>();
    foreach (KeyValuePair<int, int> spacecraftDestination in this.savedSpacecraftDestinations)
    {
      if (spacecraftDestination.Value == destination.id)
        spacecraftsForDestination.Add(spacecraftDestination.Key);
    }
    return spacecraftsForDestination;
  }

  private void CleanSavedSpacecraftDestinations()
  {
    List<int> intList = new List<int>();
    foreach (KeyValuePair<int, int> spacecraftDestination in this.savedSpacecraftDestinations)
    {
      bool flag1 = false;
      foreach (Spacecraft spacecraft in this.spacecraft)
      {
        if (spacecraft.id == spacecraftDestination.Key)
        {
          flag1 = true;
          break;
        }
      }
      bool flag2 = false;
      foreach (SpaceDestination destination in this.destinations)
      {
        if (destination.id == spacecraftDestination.Value)
        {
          flag2 = true;
          break;
        }
      }
      if (!flag1 || !flag2)
        intList.Add(spacecraftDestination.Key);
    }
    foreach (int key in intList)
      this.savedSpacecraftDestinations.Remove(key);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    Game.Instance.spacecraftManager = this;
    if (DlcManager.FeatureClusterSpaceEnabled())
      Debug.Assert(this.spacecraft == null || this.spacecraft.Count == 0);
    else
      this.RestoreDestinations();
  }

  public void SetSpacecraftDestination(LaunchConditionManager lcm, SpaceDestination destination) => this.savedSpacecraftDestinations[this.GetSpacecraftFromLaunchConditionManager(lcm).id] = destination.id;

  public int GetSpacecraftID(ILaunchableRocket rocket)
  {
    foreach (Spacecraft spacecraft in this.spacecraft)
    {
      if (Object.op_Equality((Object) ((Component) spacecraft.launchConditions).gameObject, (Object) rocket.LaunchableGameObject))
        return spacecraft.id;
    }
    return -1;
  }

  public SpaceDestination GetDestination(int destinationID)
  {
    foreach (SpaceDestination destination in this.destinations)
    {
      if (destination.id == destinationID)
        return destination;
    }
    Debug.LogErrorFormat("No space destination with ID {0}", new object[1]
    {
      (object) destinationID
    });
    return (SpaceDestination) null;
  }

  public void RegisterSpacecraft(Spacecraft craft)
  {
    if (this.spacecraft.Contains(craft))
      return;
    if (craft.HasInvalidID())
    {
      craft.SetID(this.nextSpacecraftID);
      ++this.nextSpacecraftID;
    }
    this.spacecraft.Add(craft);
  }

  public void UnregisterSpacecraft(LaunchConditionManager conditionManager)
  {
    Spacecraft conditionManager1 = this.GetSpacecraftFromLaunchConditionManager(conditionManager);
    conditionManager1.SetState(Spacecraft.MissionState.Destroyed);
    this.spacecraft.Remove(conditionManager1);
  }

  public List<Spacecraft> GetSpacecraft() => this.spacecraft;

  public Spacecraft GetSpacecraftFromLaunchConditionManager(LaunchConditionManager lcm)
  {
    foreach (Spacecraft conditionManager in this.spacecraft)
    {
      if (Object.op_Equality((Object) conditionManager.launchConditions, (Object) lcm))
        return conditionManager;
    }
    return (Spacecraft) null;
  }

  public void Sim1000ms(float dt)
  {
    if (DlcManager.FeatureClusterSpaceEnabled())
      return;
    foreach (Spacecraft spacecraft in this.spacecraft)
      spacecraft.ProgressMission(dt);
    foreach (SpaceDestination destination in this.destinations)
      destination.Replenish(dt);
  }

  public void PushReadyToLandNotification(Spacecraft spacecraft)
  {
    Notification notification1 = new Notification((string) BUILDING.STATUSITEMS.SPACECRAFTREADYTOLAND.NOTIFICATION, NotificationType.Good, (Func<List<Notification>, object, string>) ((notificationList, data) =>
    {
      string landNotification = (string) BUILDING.STATUSITEMS.SPACECRAFTREADYTOLAND.NOTIFICATION_TOOLTIP;
      foreach (Notification notification2 in notificationList)
        landNotification = landNotification + "\n" + (string) notification2.tooltipData;
      return landNotification;
    }), (object) ("• " + spacecraft.rocketName));
    ((Component) spacecraft.launchConditions).gameObject.AddOrGet<Notifier>().Add(notification1);
  }

  private void SpawnMissionResults(Dictionary<SimHashes, float> results)
  {
    foreach (KeyValuePair<SimHashes, float> result in results)
      ElementLoader.FindElementByHash(result.Key).substance.SpawnResource(PlayerController.GetCursorPos(KInputManager.GetMousePos()), result.Value, 300f, (byte) 0, 0);
  }

  public float GetDestinationAnalysisScore(SpaceDestination destination) => this.GetDestinationAnalysisScore(destination.id);

  public float GetDestinationAnalysisScore(int destinationID) => this.destinationAnalysisScores.ContainsKey(destinationID) ? this.destinationAnalysisScores[destinationID] : 0.0f;

  public void EarnDestinationAnalysisPoints(int destinationID, float points)
  {
    if (!this.destinationAnalysisScores.ContainsKey(destinationID))
      this.destinationAnalysisScores.Add(destinationID, 0.0f);
    SpaceDestination destination = this.GetDestination(destinationID);
    int destinationAnalysisState1 = (int) this.GetDestinationAnalysisState(destination);
    this.destinationAnalysisScores[destinationID] += points;
    SpacecraftManager.DestinationAnalysisState destinationAnalysisState2 = this.GetDestinationAnalysisState(destination);
    int num = (int) destinationAnalysisState2;
    if (destinationAnalysisState1 == num)
      return;
    int analysisDestinationId = SpacecraftManager.instance.GetStarmapAnalysisDestinationID();
    if (analysisDestinationId != destinationID)
      return;
    if (destinationAnalysisState2 == SpacecraftManager.DestinationAnalysisState.Complete)
    {
      if (SpacecraftManager.instance.GetDestination(analysisDestinationId).type == Db.Get().SpaceDestinationTypes.Earth.Id)
        Game.Instance.unlocks.Unlock("earth");
      if (SpacecraftManager.instance.GetDestination(analysisDestinationId).type == Db.Get().SpaceDestinationTypes.Wormhole.Id)
        Game.Instance.unlocks.Unlock("wormhole");
      SpacecraftManager.instance.SetStarmapAnalysisDestinationID(-1);
    }
    this.Trigger(532901469, (object) null);
  }

  public SpacecraftManager.DestinationAnalysisState GetDestinationAnalysisState(
    SpaceDestination destination)
  {
    if (destination.startAnalyzed)
      return SpacecraftManager.DestinationAnalysisState.Complete;
    float destinationAnalysisScore = this.GetDestinationAnalysisScore(destination);
    if ((double) destinationAnalysisScore >= (double) TUNING.ROCKETRY.DESTINATION_ANALYSIS.COMPLETE)
      return SpacecraftManager.DestinationAnalysisState.Complete;
    return (double) destinationAnalysisScore >= (double) TUNING.ROCKETRY.DESTINATION_ANALYSIS.DISCOVERED ? SpacecraftManager.DestinationAnalysisState.Discovered : SpacecraftManager.DestinationAnalysisState.Hidden;
  }

  public bool AreAllDestinationsAnalyzed()
  {
    foreach (SpaceDestination destination in this.destinations)
    {
      if (this.GetDestinationAnalysisState(destination) != SpacecraftManager.DestinationAnalysisState.Complete)
        return false;
    }
    return true;
  }

  public void SetStarmapAnalysisDestinationID(int id)
  {
    this.analyzeDestinationID = id;
    this.Trigger(532901469, (object) id);
  }

  public int GetStarmapAnalysisDestinationID() => this.analyzeDestinationID;

  public bool HasAnalysisTarget() => this.analyzeDestinationID != -1;

  public enum DestinationAnalysisState
  {
    Hidden,
    Discovered,
    Complete,
  }
}
