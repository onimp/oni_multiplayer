// Decompiled with JetBrains decompiler
// Type: UIScheduler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/UIScheduler")]
public class UIScheduler : KMonoBehaviour, IScheduler
{
  private Scheduler scheduler = new Scheduler((SchedulerClock) new UIScheduler.UISchedulerClock());
  public static UIScheduler Instance;

  public static void DestroyInstance() => UIScheduler.Instance = (UIScheduler) null;

  protected virtual void OnPrefabInit() => UIScheduler.Instance = this;

  public SchedulerHandle Schedule(
    string name,
    float time,
    Action<object> callback,
    object callback_data = null,
    SchedulerGroup group = null)
  {
    return this.scheduler.Schedule(name, time, callback, callback_data, group);
  }

  public SchedulerHandle ScheduleNextFrame(
    string name,
    Action<object> callback,
    object callback_data = null,
    SchedulerGroup group = null)
  {
    return this.scheduler.Schedule(name, 0.0f, callback, callback_data, group);
  }

  private void Update() => this.scheduler.Update();

  protected virtual void OnLoadLevel()
  {
    this.scheduler.FreeResources();
    this.scheduler = (Scheduler) null;
  }

  public SchedulerGroup CreateGroup() => new SchedulerGroup(this.scheduler);

  public Scheduler GetScheduler() => this.scheduler;

  public class UISchedulerClock : SchedulerClock
  {
    public override float GetTime() => Time.unscaledTime;
  }
}
