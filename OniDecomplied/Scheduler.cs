// Decompiled with JetBrains decompiler
// Type: Scheduler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class Scheduler : IScheduler
{
  public FloatHOTQueue<SchedulerEntry> entries = new FloatHOTQueue<SchedulerEntry>();
  private SchedulerClock clock;
  private float previousTime = float.NegativeInfinity;

  public int Count => this.entries.Count;

  public Scheduler(SchedulerClock clock) => this.clock = clock;

  public float GetTime() => this.clock.GetTime();

  private SchedulerHandle Schedule(SchedulerEntry entry)
  {
    this.entries.Enqueue(entry.time, entry);
    return new SchedulerHandle(this, entry);
  }

  private SchedulerHandle Schedule(
    string name,
    float time,
    float time_interval,
    Action<object> callback,
    object callback_data,
    GameObject profiler_obj)
  {
    return this.Schedule(new SchedulerEntry(name, time + this.clock.GetTime(), time_interval, callback, callback_data, profiler_obj));
  }

  public void FreeResources()
  {
    this.clock = (SchedulerClock) null;
    if (this.entries != null)
    {
      while (this.entries.Count > 0)
        this.entries.Dequeue().Value.FreeResources();
    }
    this.entries = (FloatHOTQueue<SchedulerEntry>) null;
  }

  public SchedulerHandle Schedule(
    string name,
    float time,
    Action<object> callback,
    object callback_data = null,
    SchedulerGroup group = null)
  {
    if (group != null && group.scheduler != this)
      Debug.LogError((object) "Scheduler group mismatch!");
    SchedulerHandle handle = this.Schedule(name, time, -1f, callback, callback_data, (GameObject) null);
    group?.Add(handle);
    return handle;
  }

  public void Clear(SchedulerHandle handle) => handle.entry.Clear();

  public void Update()
  {
    if (this.Count == 0)
      return;
    int count = this.Count;
    int num = 0;
    KProfiler.Region region;
    // ISSUE: explicit constructor call
    ((KProfiler.Region) ref region).\u002Ector("Scheduler.Update", (Object) null);
    try
    {
      float time = this.clock.GetTime();
      if ((double) this.previousTime == (double) time)
        return;
      this.previousTime = time;
      for (; num < count; ++num)
      {
        KeyValuePair<float, SchedulerEntry> keyValuePair = this.entries.Peek();
        if ((double) time < (double) keyValuePair.Key)
          break;
        SchedulerEntry schedulerEntry = this.entries.Dequeue().Value;
        if (schedulerEntry.callback != null)
          schedulerEntry.callback(schedulerEntry.callbackData);
      }
    }
    finally
    {
      region.Dispose();
    }
  }
}
