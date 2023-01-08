// Decompiled with JetBrains decompiler
// Type: SchedulerGroup
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

public class SchedulerGroup
{
  private List<SchedulerHandle> handles = new List<SchedulerHandle>();

  public Scheduler scheduler { get; private set; }

  public SchedulerGroup(Scheduler scheduler)
  {
    this.scheduler = scheduler;
    this.Reset();
  }

  public void FreeResources()
  {
    if (this.scheduler != null)
      this.scheduler.FreeResources();
    this.scheduler = (Scheduler) null;
    if (this.handles != null)
      this.handles.Clear();
    this.handles = (List<SchedulerHandle>) null;
  }

  public void Reset()
  {
    foreach (SchedulerHandle handle in this.handles)
      handle.ClearScheduler();
    this.handles.Clear();
  }

  public void Add(SchedulerHandle handle) => this.handles.Add(handle);
}
