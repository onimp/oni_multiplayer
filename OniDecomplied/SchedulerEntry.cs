// Decompiled with JetBrains decompiler
// Type: SchedulerEntry
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public struct SchedulerEntry
{
  public float time;

  public SchedulerEntry.Details details { get; private set; }

  public SchedulerEntry(
    string name,
    float time,
    float time_interval,
    Action<object> callback,
    object callback_data,
    GameObject profiler_obj)
  {
    this.time = time;
    this.details = new SchedulerEntry.Details(name, callback, callback_data, time_interval, profiler_obj);
  }

  public void FreeResources() => this.details = (SchedulerEntry.Details) null;

  public Action<object> callback => this.details.callback;

  public object callbackData => this.details.callbackData;

  public float timeInterval => this.details.timeInterval;

  public override string ToString() => this.time.ToString();

  public void Clear() => this.details.callback = (Action<object>) null;

  public class Details
  {
    public Action<object> callback;
    public object callbackData;
    public float timeInterval;

    public Details(
      string name,
      Action<object> callback,
      object callback_data,
      float time_interval,
      GameObject profiler_obj)
    {
      this.timeInterval = time_interval;
      this.callback = callback;
      this.callbackData = callback_data;
    }
  }
}
