// Decompiled with JetBrains decompiler
// Type: StateMachineManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

public class StateMachineManager : Singleton<StateMachineManager>, IScheduler
{
  private Scheduler scheduler;
  private Dictionary<System.Type, StateMachine> stateMachines = new Dictionary<System.Type, StateMachine>();
  private Dictionary<System.Type, List<Action<StateMachine>>> stateMachineCreatedCBs = new Dictionary<System.Type, List<Action<StateMachine>>>();
  private static object[] parameters = new object[2];

  public void RegisterScheduler(Scheduler scheduler) => this.scheduler = scheduler;

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

  public SchedulerGroup CreateSchedulerGroup() => new SchedulerGroup(this.scheduler);

  public StateMachine CreateStateMachine(System.Type type)
  {
    StateMachine state_machine = (StateMachine) null;
    if (!this.stateMachines.TryGetValue(type, out state_machine))
    {
      state_machine = (StateMachine) Activator.CreateInstance(type);
      state_machine.CreateStates((object) state_machine);
      state_machine.BindStates();
      state_machine.InitializeStateMachine();
      this.stateMachines[type] = state_machine;
      List<Action<StateMachine>> actionList;
      if (this.stateMachineCreatedCBs.TryGetValue(type, out actionList))
      {
        foreach (Action<StateMachine> action in actionList)
          action(state_machine);
      }
    }
    return state_machine;
  }

  public T CreateStateMachine<T>() => (T) this.CreateStateMachine(typeof (T));

  public static void ResetParameters()
  {
    for (int index = 0; index < StateMachineManager.parameters.Length; ++index)
      StateMachineManager.parameters[index] = (object) null;
  }

  public StateMachine.Instance CreateSMIFromDef(
    IStateMachineTarget master,
    StateMachine.BaseDef def)
  {
    StateMachineManager.parameters[0] = (object) master;
    StateMachineManager.parameters[1] = (object) def;
    return (StateMachine.Instance) Activator.CreateInstance(Singleton<StateMachineManager>.Instance.CreateStateMachine(def.GetStateMachineType()).GetStateMachineInstanceType(), StateMachineManager.parameters);
  }

  public void Clear()
  {
    if (this.scheduler != null)
      this.scheduler.FreeResources();
    if (this.stateMachines == null)
      return;
    this.stateMachines.Clear();
  }

  public void AddStateMachineCreatedCallback(System.Type sm_type, Action<StateMachine> cb)
  {
    List<Action<StateMachine>> actionList;
    if (!this.stateMachineCreatedCBs.TryGetValue(sm_type, out actionList))
    {
      actionList = new List<Action<StateMachine>>();
      this.stateMachineCreatedCBs[sm_type] = actionList;
    }
    actionList.Add(cb);
  }

  public void RemoveStateMachineCreatedCallback(System.Type sm_type, Action<StateMachine> cb)
  {
    List<Action<StateMachine>> actionList;
    if (!this.stateMachineCreatedCBs.TryGetValue(sm_type, out actionList))
      return;
    actionList.Remove(cb);
  }
}
