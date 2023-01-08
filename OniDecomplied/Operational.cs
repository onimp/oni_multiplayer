// Decompiled with JetBrains decompiler
// Type: Operational
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Operational")]
public class Operational : KMonoBehaviour
{
  [Serialize]
  public float inactiveStartTime;
  [Serialize]
  public float activeStartTime;
  [Serialize]
  private List<float> uptimeData = new List<float>();
  [Serialize]
  private float activeTime;
  [Serialize]
  private float inactiveTime;
  private int MAX_DATA_POINTS = 5;
  public Dictionary<Operational.Flag, bool> Flags = new Dictionary<Operational.Flag, bool>();
  private static readonly EventSystem.IntraObjectHandler<Operational> OnNewBuildingDelegate = new EventSystem.IntraObjectHandler<Operational>((Action<Operational, object>) ((component, data) => component.OnNewBuilding(data)));

  public bool IsFunctional { get; private set; }

  public bool IsOperational { get; private set; }

  public bool IsActive { get; private set; }

  [System.Runtime.Serialization.OnSerializing]
  private void OnSerializing()
  {
    this.AddTimeData(this.IsActive);
    this.activeStartTime = GameClock.Instance.GetTime();
    this.inactiveStartTime = GameClock.Instance.GetTime();
  }

  protected virtual void OnPrefabInit()
  {
    this.UpdateFunctional();
    this.UpdateOperational();
    this.Subscribe<Operational>(-1661515756, Operational.OnNewBuildingDelegate);
    GameClock.Instance.Subscribe(631075836, new Action<object>(this.OnNewDay));
  }

  public void OnNewBuilding(object data)
  {
    BuildingComplete component = ((Component) this).GetComponent<BuildingComplete>();
    if ((double) component.creationTime <= 0.0)
      return;
    this.inactiveStartTime = component.creationTime;
    this.activeStartTime = component.creationTime;
  }

  public bool IsOperationalType(Operational.Flag.Type type) => type == Operational.Flag.Type.Functional ? this.IsFunctional : this.IsOperational;

  public void SetFlag(Operational.Flag flag, bool value)
  {
    bool flag1 = false;
    if (this.Flags.TryGetValue(flag, out flag1))
    {
      if (flag1 != value)
      {
        this.Flags[flag] = value;
        this.Trigger(187661686, (object) flag);
      }
    }
    else
    {
      this.Flags[flag] = value;
      this.Trigger(187661686, (object) flag);
    }
    if (flag.FlagType == Operational.Flag.Type.Functional && value != this.IsFunctional)
      this.UpdateFunctional();
    if (value == this.IsOperational)
      return;
    this.UpdateOperational();
  }

  public bool GetFlag(Operational.Flag flag)
  {
    bool flag1 = false;
    this.Flags.TryGetValue(flag, out flag1);
    return flag1;
  }

  private void UpdateFunctional()
  {
    bool flag1 = true;
    foreach (KeyValuePair<Operational.Flag, bool> flag2 in this.Flags)
    {
      if (flag2.Key.FlagType == Operational.Flag.Type.Functional && !flag2.Value)
      {
        flag1 = false;
        break;
      }
    }
    this.IsFunctional = flag1;
    this.Trigger(-1852328367, (object) this.IsFunctional);
  }

  private void UpdateOperational()
  {
    Dictionary<Operational.Flag, bool>.Enumerator enumerator = this.Flags.GetEnumerator();
    bool flag = true;
    while (enumerator.MoveNext())
    {
      if (!enumerator.Current.Value)
      {
        flag = false;
        break;
      }
    }
    if (flag == this.IsOperational)
      return;
    this.IsOperational = flag;
    if (!this.IsOperational)
      this.SetActive(false);
    if (this.IsOperational)
      ((Component) this).GetComponent<KPrefabID>().AddTag(GameTags.Operational, false);
    else
      ((Component) this).GetComponent<KPrefabID>().RemoveTag(GameTags.Operational);
    this.Trigger(-592767678, (object) this.IsOperational);
    Game.Instance.Trigger(-809948329, (object) ((Component) this).gameObject);
  }

  public void SetActive(bool value, bool force_ignore = false)
  {
    if (this.IsActive == value)
      return;
    this.AddTimeData(value);
    this.Trigger(824508782, (object) this);
    Game.Instance.Trigger(-809948329, (object) ((Component) this).gameObject);
  }

  private void AddTimeData(bool value)
  {
    float num1 = this.IsActive ? this.activeStartTime : this.inactiveStartTime;
    float time = GameClock.Instance.GetTime();
    float num2 = time - num1;
    if (this.IsActive)
      this.activeTime += num2;
    else
      this.inactiveTime += num2;
    this.IsActive = value;
    if (this.IsActive)
      this.activeStartTime = time;
    else
      this.inactiveStartTime = time;
  }

  public void OnNewDay(object data)
  {
    this.AddTimeData(this.IsActive);
    this.uptimeData.Add(this.activeTime / 600f);
    while (this.uptimeData.Count > this.MAX_DATA_POINTS)
      this.uptimeData.RemoveAt(0);
    this.activeTime = 0.0f;
    this.inactiveTime = 0.0f;
  }

  public float GetCurrentCycleUptime()
  {
    if (!this.IsActive)
      return this.activeTime / GameClock.Instance.GetTimeSinceStartOfCycle();
    float num = this.IsActive ? this.activeStartTime : this.inactiveStartTime;
    return (this.activeTime + (GameClock.Instance.GetTime() - num)) / GameClock.Instance.GetTimeSinceStartOfCycle();
  }

  public float GetLastCycleUptime() => this.uptimeData.Count > 0 ? this.uptimeData[this.uptimeData.Count - 1] : 0.0f;

  public float GetUptimeOverCycles(int num_cycles)
  {
    if (this.uptimeData.Count <= 0)
      return 0.0f;
    int num1 = Mathf.Min(this.uptimeData.Count, num_cycles);
    float num2 = 0.0f;
    for (int index = num1 - 1; index >= 0; --index)
      num2 += this.uptimeData[index];
    return num2 / (float) num1;
  }

  public bool MeetsRequirements(Operational.State stateRequirement)
  {
    switch (stateRequirement)
    {
      case Operational.State.Operational:
        return this.IsOperational;
      case Operational.State.Functional:
        return this.IsFunctional;
      case Operational.State.Active:
        return this.IsActive;
      default:
        return true;
    }
  }

  public static GameHashes GetEventForState(Operational.State state)
  {
    if (state == Operational.State.Operational)
      return GameHashes.OperationalChanged;
    return state == Operational.State.Functional ? GameHashes.FunctionalChanged : GameHashes.ActiveChanged;
  }

  public enum State
  {
    Operational,
    Functional,
    Active,
    None,
  }

  public class Flag
  {
    public string Name;
    public Operational.Flag.Type FlagType;

    public Flag(string name, Operational.Flag.Type type)
    {
      this.Name = name;
      this.FlagType = type;
    }

    public static Operational.Flag.Type GetFlagType(Operational.State operationalState)
    {
      switch (operationalState)
      {
        case Operational.State.Operational:
        case Operational.State.Active:
          return Operational.Flag.Type.Requirement;
        case Operational.State.Functional:
          return Operational.Flag.Type.Functional;
        default:
          throw new InvalidOperationException("Can not convert NONE state to an Operational Flag Type");
      }
    }

    public enum Type
    {
      Requirement,
      Functional,
    }
  }
}
