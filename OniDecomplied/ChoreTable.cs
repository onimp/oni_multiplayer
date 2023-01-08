// Decompiled with JetBrains decompiler
// Type: ChoreTable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class ChoreTable
{
  private ChoreTable.Entry[] entries;
  public static ChoreTable.Entry InvalidEntry;

  public ChoreTable(ChoreTable.Entry[] entries) => this.entries = entries;

  public ref ChoreTable.Entry GetEntry<T>()
  {
    ref ChoreTable.Entry local = ref ChoreTable.InvalidEntry;
    for (int index = 0; index < this.entries.Length; ++index)
    {
      if (this.entries[index].stateMachineDef is T)
      {
        local = ref this.entries[index];
        break;
      }
    }
    return ref local;
  }

  public int GetChorePriority<StateMachineType>(ChoreConsumer chore_consumer)
  {
    for (int index = 0; index < this.entries.Length; ++index)
    {
      ChoreTable.Entry entry = this.entries[index];
      if (entry.stateMachineDef.GetStateMachineType() == typeof (StateMachineType))
        return entry.choreType.priority;
    }
    Debug.LogError((object) (((Object) chore_consumer).name + "'s chore table does not have an entry for: " + typeof (StateMachineType).Name));
    return -1;
  }

  public class Builder
  {
    private int interruptGroupId;
    private List<ChoreTable.Builder.Info> infos = new List<ChoreTable.Builder.Info>();
    private const int INVALID_PRIORITY = -1;

    public ChoreTable.Builder PushInterruptGroup()
    {
      ++this.interruptGroupId;
      return this;
    }

    public ChoreTable.Builder PopInterruptGroup()
    {
      DebugUtil.Assert(this.interruptGroupId > 0);
      --this.interruptGroupId;
      return this;
    }

    public ChoreTable.Builder Add(StateMachine.BaseDef def, bool condition = true, int forcePriority = -1)
    {
      if (condition)
        this.infos.Add(new ChoreTable.Builder.Info()
        {
          interruptGroupId = this.interruptGroupId,
          forcePriority = forcePriority,
          def = def
        });
      return this;
    }

    public ChoreTable CreateTable()
    {
      DebugUtil.Assert(this.interruptGroupId == 0);
      ChoreTable.Entry[] entries = new ChoreTable.Entry[this.infos.Count];
      Stack<int> intStack = new Stack<int>();
      int num = 10000;
      for (int index = 0; index < this.infos.Count; ++index)
      {
        int priority = this.infos[index].forcePriority != -1 ? this.infos[index].forcePriority : num - 100;
        num = priority;
        int interrupt_priority = 10000 - index * 100;
        int interruptGroupId = this.infos[index].interruptGroupId;
        if (interruptGroupId != 0)
        {
          if (intStack.Count != interruptGroupId)
            intStack.Push(interrupt_priority);
          else
            interrupt_priority = intStack.Peek();
        }
        else if (intStack.Count > 0)
          intStack.Pop();
        entries[index] = new ChoreTable.Entry(this.infos[index].def, priority, interrupt_priority);
      }
      return new ChoreTable(entries);
    }

    private struct Info
    {
      public int interruptGroupId;
      public int forcePriority;
      public StateMachine.BaseDef def;
    }
  }

  public class ChoreTableChore<StateMachineType, StateMachineInstanceType> : 
    Chore<StateMachineInstanceType>
    where StateMachineInstanceType : StateMachine.Instance
  {
    public ChoreTableChore(
      StateMachine.BaseDef state_machine_def,
      ChoreType chore_type,
      KPrefabID prefab_id)
      : base(chore_type, (IStateMachineTarget) prefab_id, ((Component) prefab_id).GetComponent<ChoreProvider>())
    {
      this.showAvailabilityInHoverText = false;
      this.smi = state_machine_def.CreateSMI((IStateMachineTarget) this) as StateMachineInstanceType;
    }
  }

  public struct Entry
  {
    public System.Type choreClassType;
    public ChoreType choreType;
    public StateMachine.BaseDef stateMachineDef;

    public Entry(StateMachine.BaseDef state_machine_def, int priority, int interrupt_priority)
    {
      System.Type machineInstanceType = Singleton<StateMachineManager>.Instance.CreateStateMachine(state_machine_def.GetStateMachineType()).GetStateMachineInstanceType();
      this.choreClassType = typeof (ChoreTable.ChoreTableChore<,>).MakeGenericType(state_machine_def.GetStateMachineType(), machineInstanceType);
      this.choreType = new ChoreType(state_machine_def.ToString(), (ResourceSet) null, new string[0], "", "", "", "", (IEnumerable<Tag>) new Tag[0], priority, priority);
      this.choreType.interruptPriority = interrupt_priority;
      this.stateMachineDef = state_machine_def;
    }
  }

  public class Instance
  {
    private static object[] parameters = new object[3];
    private KPrefabID prefabId;
    private ListPool<ChoreTable.Instance.Entry, ChoreTable.Instance>.PooledList entries;

    public static void ResetParameters()
    {
      for (int index = 0; index < ChoreTable.Instance.parameters.Length; ++index)
        ChoreTable.Instance.parameters[index] = (object) null;
    }

    public Instance(ChoreTable chore_table, KPrefabID prefab_id)
    {
      this.prefabId = prefab_id;
      this.entries = ListPool<ChoreTable.Instance.Entry, ChoreTable.Instance>.Allocate();
      for (int index = 0; index < chore_table.entries.Length; ++index)
        ((List<ChoreTable.Instance.Entry>) this.entries).Add(new ChoreTable.Instance.Entry(chore_table.entries[index], prefab_id));
    }

    ~Instance() => this.OnCleanUp(this.prefabId);

    public void OnCleanUp(KPrefabID prefab_id)
    {
      if (this.entries == null)
        return;
      for (int index = 0; index < ((List<ChoreTable.Instance.Entry>) this.entries).Count; ++index)
        ((List<ChoreTable.Instance.Entry>) this.entries)[index].OnCleanUp(prefab_id);
      this.entries.Recycle();
      this.entries = (ListPool<ChoreTable.Instance.Entry, ChoreTable.Instance>.PooledList) null;
    }

    private struct Entry
    {
      public Chore chore;

      public Entry(ChoreTable.Entry chore_table_entry, KPrefabID prefab_id)
      {
        ChoreTable.Instance.parameters[0] = (object) chore_table_entry.stateMachineDef;
        ChoreTable.Instance.parameters[1] = (object) chore_table_entry.choreType;
        ChoreTable.Instance.parameters[2] = (object) prefab_id;
        this.chore = (Chore) Activator.CreateInstance(chore_table_entry.choreClassType, ChoreTable.Instance.parameters);
        ChoreTable.Instance.parameters[0] = (object) null;
        ChoreTable.Instance.parameters[1] = (object) null;
        ChoreTable.Instance.parameters[2] = (object) null;
      }

      public void OnCleanUp(KPrefabID prefab_id)
      {
        if (this.chore == null)
          return;
        this.chore.Cancel("ChoreTable.Instance.OnCleanUp");
        this.chore = (Chore) null;
      }
    }
  }
}
