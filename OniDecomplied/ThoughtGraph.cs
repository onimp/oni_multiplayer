// Decompiled with JetBrains decompiler
// Type: ThoughtGraph
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class ThoughtGraph : GameStateMachine<ThoughtGraph, ThoughtGraph.Instance>
{
  public StateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget, object>.Signal thoughtsChanged;
  public StateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget, object>.Signal thoughtsChangedImmediate;
  public StateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget, object>.FloatParameter thoughtDisplayTime;
  public GameStateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget, object>.State initialdelay;
  public GameStateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget, object>.State nothoughts;
  public ThoughtGraph.DisplayingThoughtState displayingthought;
  public GameStateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget, object>.State cooldown;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.initialdelay;
    this.initialdelay.ScheduleGoTo(1f, (StateMachine.BaseState) this.nothoughts);
    this.nothoughts.OnSignal(this.thoughtsChanged, (GameStateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget, object>.State) this.displayingthought, (Func<ThoughtGraph.Instance, bool>) (smi => smi.HasThoughts())).OnSignal(this.thoughtsChangedImmediate, (GameStateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget, object>.State) this.displayingthought, (Func<ThoughtGraph.Instance, bool>) (smi => smi.HasThoughts()));
    this.displayingthought.DefaultState(this.displayingthought.pre).Enter("CreateBubble", (StateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.CreateBubble())).Exit("DestroyBubble", (StateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.DestroyBubble())).ScheduleGoTo((Func<ThoughtGraph.Instance, float>) (smi => this.thoughtDisplayTime.Get(smi)), (StateMachine.BaseState) this.cooldown);
    this.displayingthought.pre.ScheduleGoTo((Func<ThoughtGraph.Instance, float>) (smi => TuningData<ThoughtGraph.Tuning>.Get().preLengthInSeconds), (StateMachine.BaseState) this.displayingthought.talking);
    this.displayingthought.talking.Enter(new StateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget, object>.State.Callback(ThoughtGraph.BeginTalking));
    this.cooldown.OnSignal(this.thoughtsChangedImmediate, (GameStateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget, object>.State) this.displayingthought, (Func<ThoughtGraph.Instance, bool>) (smi => smi.HasImmediateThought())).ScheduleGoTo(20f, (StateMachine.BaseState) this.nothoughts);
  }

  private static void BeginTalking(ThoughtGraph.Instance smi)
  {
    if (smi.currentThought == null || !SpeechMonitor.IsAllowedToPlaySpeech(smi.gameObject))
      return;
    smi.GetSMI<SpeechMonitor.Instance>().PlaySpeech(smi.currentThought.speechPrefix, smi.currentThought.sound);
  }

  public class Tuning : TuningData<ThoughtGraph.Tuning>
  {
    public float preLengthInSeconds;
  }

  public class DisplayingThoughtState : 
    GameStateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget, object>.State
  {
    public GameStateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget, object>.State pre;
    public GameStateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget, object>.State talking;
  }

  public new class Instance : 
    GameStateMachine<ThoughtGraph, ThoughtGraph.Instance, IStateMachineTarget, object>.GameInstance
  {
    private List<Thought> thoughts = new List<Thought>();
    public Thought currentThought;

    public Instance(IStateMachineTarget master)
      : base(master)
    {
      NameDisplayScreen.Instance.RegisterComponent(this.gameObject, (object) this);
    }

    public bool HasThoughts() => this.thoughts.Count > 0;

    public bool HasImmediateThought()
    {
      bool flag = false;
      for (int index = 0; index < this.thoughts.Count; ++index)
      {
        if (this.thoughts[index].showImmediately)
        {
          flag = true;
          break;
        }
      }
      return flag;
    }

    public void AddThought(Thought thought)
    {
      if (this.thoughts.Contains(thought))
        return;
      this.thoughts.Add(thought);
      if (thought.showImmediately)
        this.sm.thoughtsChangedImmediate.Trigger(this.smi);
      else
        this.sm.thoughtsChanged.Trigger(this.smi);
    }

    public void RemoveThought(Thought thought)
    {
      if (!this.thoughts.Contains(thought))
        return;
      this.thoughts.Remove(thought);
      this.sm.thoughtsChanged.Trigger(this.smi);
    }

    private int SortThoughts(Thought a, Thought b)
    {
      if (a.showImmediately == b.showImmediately)
        return b.priority.CompareTo(a.priority);
      return !a.showImmediately ? 1 : -1;
    }

    public void CreateBubble()
    {
      if (this.thoughts.Count == 0)
        return;
      this.thoughts.Sort(new Comparison<Thought>(this.SortThoughts));
      Thought thought = this.thoughts[0];
      if (Object.op_Inequality((Object) thought.modeSprite, (Object) null))
        NameDisplayScreen.Instance.SetThoughtBubbleConvoDisplay(this.gameObject, true, (string) thought.hoverText, thought.bubbleSprite, thought.sprite, thought.modeSprite);
      else
        NameDisplayScreen.Instance.SetThoughtBubbleDisplay(this.gameObject, true, (string) thought.hoverText, thought.bubbleSprite, thought.sprite);
      double num = (double) this.sm.thoughtDisplayTime.Set(thought.showTime, this);
      this.currentThought = thought;
      if (!thought.showImmediately)
        return;
      this.thoughts.RemoveAt(0);
    }

    public void DestroyBubble()
    {
      NameDisplayScreen.Instance.SetThoughtBubbleDisplay(this.gameObject, false, (string) null, (Sprite) null, (Sprite) null);
      NameDisplayScreen.Instance.SetThoughtBubbleConvoDisplay(this.gameObject, false, (string) null, (Sprite) null, (Sprite) null, (Sprite) null);
    }
  }
}
