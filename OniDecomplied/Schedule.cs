// Decompiled with JetBrains decompiler
// Type: Schedule
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class Schedule : ISaveLoadable, IListableOption
{
  [Serialize]
  private List<ScheduleBlock> blocks;
  [Serialize]
  private List<Ref<Schedulable>> assigned;
  [Serialize]
  public string name;
  [Serialize]
  public bool alarmActivated = true;
  [Serialize]
  private int[] tones;
  public Action<Schedule> onChanged;

  public static int GetBlockIdx() => Math.Min((int) ((double) GameClock.Instance.GetCurrentCycleAsPercentage() * 24.0), 23);

  public static int GetLastBlockIdx() => (Schedule.GetBlockIdx() + 24 - 1) % 24;

  public void ClearNullReferences() => this.assigned.RemoveAll((Predicate<Ref<Schedulable>>) (x => Object.op_Equality((Object) x.Get(), (Object) null)));

  public Schedule(string name, List<ScheduleGroup> defaultGroups, bool alarmActivated)
  {
    this.name = name;
    this.alarmActivated = alarmActivated;
    this.blocks = new List<ScheduleBlock>(24);
    this.assigned = new List<Ref<Schedulable>>();
    this.tones = this.GenerateTones();
    this.SetBlocksToGroupDefaults(defaultGroups);
  }

  public void SetBlocksToGroupDefaults(List<ScheduleGroup> defaultGroups)
  {
    this.blocks.Clear();
    int num = 0;
    for (int index1 = 0; index1 < defaultGroups.Count; ++index1)
    {
      ScheduleGroup defaultGroup = defaultGroups[index1];
      for (int index2 = 0; index2 < defaultGroup.defaultSegments; ++index2)
      {
        this.blocks.Add(new ScheduleBlock(defaultGroup.Name, defaultGroup.allowedTypes, defaultGroup.Id));
        ++num;
      }
    }
    Debug.Assert(num == 24);
    this.Changed();
  }

  public void Tick()
  {
    ScheduleBlock block1 = this.GetBlock(Schedule.GetBlockIdx());
    ScheduleBlock block2 = this.GetBlock(Schedule.GetLastBlockIdx());
    if (!Schedule.AreScheduleTypesIdentical(block1.allowed_types, block2.allowed_types))
    {
      ScheduleGroup forScheduleTypes1 = Db.Get().ScheduleGroups.FindGroupForScheduleTypes(block1.allowed_types);
      ScheduleGroup forScheduleTypes2 = Db.Get().ScheduleGroups.FindGroupForScheduleTypes(block2.allowed_types);
      if (this.alarmActivated && forScheduleTypes2.alarm != forScheduleTypes1.alarm)
        ScheduleManager.Instance.PlayScheduleAlarm(this, block1, forScheduleTypes1.alarm);
      foreach (Ref<Schedulable> @ref in this.GetAssigned())
        @ref.Get().OnScheduleBlocksChanged(this);
    }
    foreach (Ref<Schedulable> @ref in this.GetAssigned())
      @ref.Get().OnScheduleBlocksTick(this);
  }

  string IListableOption.GetProperName() => this.name;

  public int[] GenerateTones()
  {
    int minToneIndex = TuningData<ScheduleManager.Tuning>.Get().minToneIndex;
    int maxToneIndex = TuningData<ScheduleManager.Tuning>.Get().maxToneIndex;
    int firstLastToneSpacing = TuningData<ScheduleManager.Tuning>.Get().firstLastToneSpacing;
    int[] tones = new int[4]
    {
      Random.Range(minToneIndex, maxToneIndex - firstLastToneSpacing + 1),
      Random.Range(minToneIndex, maxToneIndex + 1),
      Random.Range(minToneIndex, maxToneIndex + 1),
      0
    };
    tones[3] = Random.Range(tones[0] + firstLastToneSpacing, maxToneIndex + 1);
    return tones;
  }

  public List<Ref<Schedulable>> GetAssigned()
  {
    if (this.assigned == null)
      this.assigned = new List<Ref<Schedulable>>();
    return this.assigned;
  }

  public int[] GetTones()
  {
    if (this.tones == null)
      this.tones = this.GenerateTones();
    return this.tones;
  }

  public void SetGroup(int idx, ScheduleGroup group)
  {
    if (0 > idx || idx >= this.blocks.Count)
      return;
    this.blocks[idx] = new ScheduleBlock(group.Name, group.allowedTypes, group.Id);
    this.Changed();
  }

  private void Changed()
  {
    foreach (Ref<Schedulable> @ref in this.GetAssigned())
      @ref.Get().OnScheduleChanged(this);
    if (this.onChanged == null)
      return;
    this.onChanged(this);
  }

  public List<ScheduleBlock> GetBlocks() => this.blocks;

  public ScheduleBlock GetBlock(int idx) => this.blocks[idx];

  public void Assign(Schedulable schedulable)
  {
    if (!this.IsAssigned(schedulable))
      this.GetAssigned().Add(new Ref<Schedulable>(schedulable));
    this.Changed();
  }

  public void Unassign(Schedulable schedulable)
  {
    for (int index = 0; index < this.GetAssigned().Count; ++index)
    {
      if (Object.op_Equality((Object) this.GetAssigned()[index].Get(), (Object) schedulable))
      {
        this.GetAssigned().RemoveAt(index);
        break;
      }
    }
    this.Changed();
  }

  public bool IsAssigned(Schedulable schedulable)
  {
    foreach (Ref<Schedulable> @ref in this.GetAssigned())
    {
      if (Object.op_Equality((Object) @ref.Get(), (Object) schedulable))
        return true;
    }
    return false;
  }

  public static bool AreScheduleTypesIdentical(List<ScheduleBlockType> a, List<ScheduleBlockType> b)
  {
    if (a.Count != b.Count)
      return false;
    foreach (ScheduleBlockType scheduleBlockType1 in a)
    {
      bool flag = false;
      foreach (ScheduleBlockType scheduleBlockType2 in b)
      {
        if (HashedString.op_Equality(scheduleBlockType1.IdHash, scheduleBlockType2.IdHash))
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        return false;
    }
    return true;
  }
}
