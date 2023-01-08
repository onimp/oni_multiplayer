// Decompiled with JetBrains decompiler
// Type: LaunchConditionManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/LaunchConditionManager")]
public class LaunchConditionManager : KMonoBehaviour, ISim4000ms, ISim1000ms
{
  public HashedString triggerPort;
  public HashedString statusPort;
  private ILaunchableRocket launchable;
  [Serialize]
  private List<Tuple<string, string, string>> DEBUG_ModuleDestructions;
  private Dictionary<ProcessCondition, Guid> conditionStatuses = new Dictionary<ProcessCondition, Guid>();

  public List<RocketModule> rocketModules { get; private set; }

  public void DEBUG_TraceModuleDestruction(string moduleName, string state, string stackTrace)
  {
    if (this.DEBUG_ModuleDestructions == null)
      this.DEBUG_ModuleDestructions = new List<Tuple<string, string, string>>();
    this.DEBUG_ModuleDestructions.Add(new Tuple<string, string, string>(moduleName, state, stackTrace));
  }

  [ContextMenu("Dump Module Destructions")]
  private void DEBUG_DumpModuleDestructions()
  {
    if (this.DEBUG_ModuleDestructions == null || this.DEBUG_ModuleDestructions.Count == 0)
    {
      DebugUtil.LogArgs(new object[1]
      {
        (object) "Sorry, no logged module destructions. :("
      });
    }
    else
    {
      foreach (Tuple<string, string, string> moduleDestruction in this.DEBUG_ModuleDestructions)
        DebugUtil.LogArgs(new object[6]
        {
          (object) moduleDestruction.first,
          (object) ">",
          (object) moduleDestruction.second,
          (object) "\n",
          (object) moduleDestruction.third,
          (object) "\nEND MODULE DUMP\n\n"
        });
    }
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.rocketModules = new List<RocketModule>();
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.launchable = ((Component) this).GetComponent<ILaunchableRocket>();
    this.FindModules();
    ((Component) this).GetComponent<AttachableBuilding>().onAttachmentNetworkChanged = (Action<object>) (data => this.FindModules());
  }

  protected virtual void OnCleanUp() => base.OnCleanUp();

  public void Sim1000ms(float dt)
  {
    Spacecraft conditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this);
    if (conditionManager == null)
      return;
    Debug.Assert(!DlcManager.FeatureClusterSpaceEnabled());
    SpaceDestination spacecraftDestination = SpacecraftManager.instance.GetSpacecraftDestination(conditionManager.id);
    if (((Component) this).gameObject.GetComponent<LogicPorts>().GetInputValue(this.triggerPort) != 1 || spacecraftDestination == null || spacecraftDestination.id == -1)
      return;
    this.Launch(spacecraftDestination);
  }

  public void FindModules()
  {
    foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(((Component) this).GetComponent<AttachableBuilding>()))
    {
      RocketModule component = gameObject.GetComponent<RocketModule>();
      if (Object.op_Inequality((Object) component, (Object) null) && Object.op_Equality((Object) component.conditionManager, (Object) null))
      {
        component.conditionManager = this;
        component.RegisterWithConditionManager();
      }
    }
  }

  public void RegisterRocketModule(RocketModule module)
  {
    if (this.rocketModules.Contains(module))
      return;
    this.rocketModules.Add(module);
  }

  public void UnregisterRocketModule(RocketModule module) => this.rocketModules.Remove(module);

  public List<ProcessCondition> GetLaunchConditionList()
  {
    List<ProcessCondition> launchConditionList = new List<ProcessCondition>();
    foreach (RocketModule rocketModule in this.rocketModules)
    {
      foreach (ProcessCondition condition in rocketModule.GetConditionSet(ProcessCondition.ProcessConditionType.RocketPrep))
        launchConditionList.Add(condition);
      foreach (ProcessCondition condition in rocketModule.GetConditionSet(ProcessCondition.ProcessConditionType.RocketStorage))
        launchConditionList.Add(condition);
    }
    return launchConditionList;
  }

  public void Launch(SpaceDestination destination)
  {
    if (destination == null)
      Debug.LogError((object) "Null destination passed to launch");
    if (SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this).state != Spacecraft.MissionState.Grounded || !DebugHandler.InstantBuildMode && (!this.CheckReadyToLaunch() || !this.CheckAbleToFly()))
      return;
    EventExtensions.Trigger(this.launchable.LaunchableGameObject, 705820818, (object) null);
    SpacecraftManager.instance.SetSpacecraftDestination(this, destination);
    SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this).BeginMission(destination);
  }

  public bool CheckReadyToLaunch()
  {
    foreach (RocketModule rocketModule in this.rocketModules)
    {
      foreach (ProcessCondition condition in rocketModule.GetConditionSet(ProcessCondition.ProcessConditionType.RocketPrep))
      {
        if (condition.EvaluateCondition() == ProcessCondition.Status.Failure)
          return false;
      }
      foreach (ProcessCondition condition in rocketModule.GetConditionSet(ProcessCondition.ProcessConditionType.RocketStorage))
      {
        if (condition.EvaluateCondition() == ProcessCondition.Status.Failure)
          return false;
      }
    }
    return true;
  }

  public bool CheckAbleToFly()
  {
    foreach (RocketModule rocketModule in this.rocketModules)
    {
      foreach (ProcessCondition condition in rocketModule.GetConditionSet(ProcessCondition.ProcessConditionType.RocketFlight))
      {
        if (condition.EvaluateCondition() == ProcessCondition.Status.Failure)
          return false;
      }
    }
    return true;
  }

  private void ClearFlightStatuses()
  {
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    foreach (KeyValuePair<ProcessCondition, Guid> conditionStatuse in this.conditionStatuses)
      component.RemoveStatusItem(conditionStatuse.Value);
    this.conditionStatuses.Clear();
  }

  public void Sim4000ms(float dt)
  {
    int num = this.CheckReadyToLaunch() ? 1 : 0;
    LogicPorts component1 = ((Component) this).gameObject.GetComponent<LogicPorts>();
    if (num != 0)
    {
      Spacecraft conditionManager = SpacecraftManager.instance.GetSpacecraftFromLaunchConditionManager(this);
      if (conditionManager.state == Spacecraft.MissionState.Grounded || conditionManager.state == Spacecraft.MissionState.Launching)
        component1.SendSignal(this.statusPort, 1);
      else
        component1.SendSignal(this.statusPort, 0);
      KSelectable component2 = ((Component) this).GetComponent<KSelectable>();
      foreach (RocketModule rocketModule in this.rocketModules)
      {
        foreach (ProcessCondition condition in rocketModule.GetConditionSet(ProcessCondition.ProcessConditionType.RocketFlight))
        {
          if (condition.EvaluateCondition() == ProcessCondition.Status.Failure)
          {
            if (!this.conditionStatuses.ContainsKey(condition))
            {
              StatusItem statusItem = condition.GetStatusItem(ProcessCondition.Status.Failure);
              this.conditionStatuses[condition] = component2.AddStatusItem(statusItem, (object) condition);
            }
          }
          else if (this.conditionStatuses.ContainsKey(condition))
          {
            component2.RemoveStatusItem(this.conditionStatuses[condition]);
            this.conditionStatuses.Remove(condition);
          }
        }
      }
    }
    else
    {
      this.ClearFlightStatuses();
      component1.SendSignal(this.statusPort, 0);
    }
  }

  public enum ConditionType
  {
    Launch,
    Flight,
  }
}
