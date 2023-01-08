// Decompiled with JetBrains decompiler
// Type: Spacecraft
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using KSerialization;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class Spacecraft
{
  [Serialize]
  public int id = -1;
  [Serialize]
  public string rocketName = (string) UI.STARMAP.DEFAULT_NAME;
  [Serialize]
  public float controlStationBuffTimeRemaining;
  [Serialize]
  public Ref<LaunchConditionManager> refLaunchConditions = new Ref<LaunchConditionManager>();
  [Serialize]
  public Spacecraft.MissionState state;
  [Serialize]
  private float missionElapsed;
  [Serialize]
  private float missionDuration;

  public Spacecraft(LaunchConditionManager launchConditions) => this.launchConditions = launchConditions;

  public Spacecraft()
  {
  }

  public LaunchConditionManager launchConditions
  {
    get => this.refLaunchConditions.Get();
    set => this.refLaunchConditions.Set(value);
  }

  public void SetRocketName(string newName)
  {
    this.rocketName = newName;
    this.UpdateNameOnRocketModules();
  }

  public string GetRocketName() => this.rocketName;

  public void UpdateNameOnRocketModules()
  {
    foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(((Component) this.launchConditions).GetComponent<AttachableBuilding>()))
    {
      RocketModule component = gameObject.GetComponent<RocketModule>();
      if (Object.op_Inequality((Object) component, (Object) null))
        component.SetParentRocketName(this.rocketName);
    }
  }

  public bool HasInvalidID() => this.id == -1;

  public void SetID(int id) => this.id = id;

  public void SetState(Spacecraft.MissionState state) => this.state = state;

  public void BeginMission(SpaceDestination destination)
  {
    this.missionElapsed = 0.0f;
    this.missionDuration = (float) destination.OneBasedDistance * TUNING.ROCKETRY.MISSION_DURATION_SCALE / this.GetPilotNavigationEfficiency();
    this.SetState(Spacecraft.MissionState.Launching);
  }

  private float GetPilotNavigationEfficiency()
  {
    List<MinionStorage.Info> storedMinionInfo = ((Component) this.launchConditions).GetComponent<MinionStorage>().GetStoredMinionInfo();
    if (storedMinionInfo.Count < 1)
      return 1f;
    StoredMinionIdentity component = ((Component) storedMinionInfo[0].serializedMinion.Get()).GetComponent<StoredMinionIdentity>();
    string id = Db.Get().Attributes.SpaceNavigation.Id;
    float navigationEfficiency = 1f;
    foreach (KeyValuePair<string, bool> keyValuePair in component.MasteryBySkillID)
    {
      foreach (SkillPerk perk in Db.Get().Skills.Get(keyValuePair.Key).perks)
      {
        if (perk is SkillAttributePerk skillAttributePerk && skillAttributePerk.modifier.AttributeId == id)
          navigationEfficiency += skillAttributePerk.modifier.Value;
      }
    }
    return navigationEfficiency;
  }

  public void ForceComplete() => this.missionElapsed = this.missionDuration;

  public void ProgressMission(float deltaTime)
  {
    if (this.state != Spacecraft.MissionState.Underway)
      return;
    this.missionElapsed += deltaTime;
    if ((double) this.controlStationBuffTimeRemaining > 0.0)
    {
      this.missionElapsed += deltaTime * 0.200000048f;
      this.controlStationBuffTimeRemaining -= deltaTime;
    }
    else
      this.controlStationBuffTimeRemaining = 0.0f;
    if ((double) this.missionElapsed <= (double) this.missionDuration)
      return;
    this.CompleteMission();
  }

  public float GetTimeLeft() => this.missionDuration - this.missionElapsed;

  public float GetDuration() => this.missionDuration;

  public void CompleteMission()
  {
    SpacecraftManager.instance.PushReadyToLandNotification(this);
    this.SetState(Spacecraft.MissionState.WaitingToLand);
    this.Land();
  }

  private void Land()
  {
    this.launchConditions.Trigger(-1165815793, (object) SpacecraftManager.instance.GetSpacecraftDestination(this.id));
    foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(((Component) this.launchConditions).GetComponent<AttachableBuilding>()))
    {
      if (Object.op_Inequality((Object) gameObject, (Object) ((Component) this.launchConditions).gameObject))
        EventExtensions.Trigger(gameObject, -1165815793, (object) SpacecraftManager.instance.GetSpacecraftDestination(this.id));
    }
  }

  public void TemporallyTear()
  {
    SpacecraftManager.instance.hasVisitedWormHole = true;
    LaunchConditionManager launchConditions = this.launchConditions;
    for (int index1 = launchConditions.rocketModules.Count - 1; index1 >= 0; --index1)
    {
      Storage component1 = ((Component) launchConditions.rocketModules[index1]).GetComponent<Storage>();
      if (Object.op_Inequality((Object) component1, (Object) null))
        component1.ConsumeAllIgnoringDisease();
      MinionStorage component2 = ((Component) launchConditions.rocketModules[index1]).GetComponent<MinionStorage>();
      if (Object.op_Inequality((Object) component2, (Object) null))
      {
        List<MinionStorage.Info> storedMinionInfo = component2.GetStoredMinionInfo();
        for (int index2 = storedMinionInfo.Count - 1; index2 >= 0; --index2)
          component2.DeleteStoredMinion(storedMinionInfo[index2].id);
      }
      Util.KDestroyGameObject(((Component) launchConditions.rocketModules[index1]).gameObject);
    }
  }

  public void GenerateName() => this.SetRocketName(GameUtil.GenerateRandomRocketName());

  public enum MissionState
  {
    Grounded,
    Launching,
    Underway,
    WaitingToLand,
    Landing,
    Destroyed,
  }
}
