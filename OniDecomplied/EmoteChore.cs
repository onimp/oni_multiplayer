// Decompiled with JetBrains decompiler
// Type: EmoteChore
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using UnityEngine;

public class EmoteChore : Chore<EmoteChore.StatesInstance>
{
  private Func<StatusItem> getStatusItem;
  private SelfEmoteReactable reactable;

  public EmoteChore(
    IStateMachineTarget target,
    ChoreType chore_type,
    Emote emote,
    int emoteIterations = 1,
    Func<StatusItem> get_status_item = null)
    : base(chore_type, target, target.GetComponent<ChoreProvider>(), false, master_priority_class: PriorityScreen.PriorityClass.compulsory)
  {
    this.smi = new EmoteChore.StatesInstance(this, target.gameObject, emote, (KAnim.PlayMode) 1, emoteIterations, false);
    this.getStatusItem = get_status_item;
  }

  public EmoteChore(
    IStateMachineTarget target,
    ChoreType chore_type,
    Emote emote,
    KAnim.PlayMode play_mode,
    int emoteIterations = 1,
    bool flip_x = false)
    : base(chore_type, target, target.GetComponent<ChoreProvider>(), false, master_priority_class: PriorityScreen.PriorityClass.compulsory)
  {
    this.smi = new EmoteChore.StatesInstance(this, target.gameObject, emote, play_mode, emoteIterations, flip_x);
  }

  public EmoteChore(
    IStateMachineTarget target,
    ChoreType chore_type,
    HashedString animFile,
    HashedString[] anims,
    Func<StatusItem> get_status_item = null)
    : base(chore_type, target, target.GetComponent<ChoreProvider>(), false, master_priority_class: PriorityScreen.PriorityClass.compulsory)
  {
    this.smi = new EmoteChore.StatesInstance(this, target.gameObject, animFile, anims, (KAnim.PlayMode) 1, false);
    this.getStatusItem = get_status_item;
  }

  public EmoteChore(
    IStateMachineTarget target,
    ChoreType chore_type,
    HashedString animFile,
    HashedString[] anims,
    KAnim.PlayMode play_mode,
    bool flip_x = false)
    : base(chore_type, target, target.GetComponent<ChoreProvider>(), false, master_priority_class: PriorityScreen.PriorityClass.compulsory)
  {
    this.smi = new EmoteChore.StatesInstance(this, target.gameObject, animFile, anims, play_mode, flip_x);
  }

  protected override StatusItem GetStatusItem() => this.getStatusItem == null ? base.GetStatusItem() : this.getStatusItem();

  public override string ToString() => Object.op_Inequality((Object) this.smi.animFile, (Object) null) ? "EmoteChore<" + this.smi.animFile.GetData().name + ">" : "EmoteChore<" + this.smi.emoteAnims[0].ToString() + ">";

  public void PairReactable(SelfEmoteReactable reactable) => this.reactable = reactable;

  protected new virtual void End(string reason)
  {
    if (this.reactable != null)
    {
      this.reactable.PairEmote((EmoteChore) null);
      this.reactable.Cleanup();
      this.reactable = (SelfEmoteReactable) null;
    }
    base.End(reason);
  }

  public class StatesInstance : 
    GameStateMachine<EmoteChore.States, EmoteChore.StatesInstance, EmoteChore, object>.GameInstance
  {
    public KAnimFile animFile;
    public HashedString[] emoteAnims;
    public KAnim.PlayMode mode = (KAnim.PlayMode) 1;

    public StatesInstance(
      EmoteChore master,
      GameObject emoter,
      Emote emote,
      KAnim.PlayMode mode,
      int emoteIterations,
      bool flip_x)
      : base(master)
    {
      this.mode = mode;
      this.animFile = emote.AnimSet;
      emote.CollectStepAnims(out this.emoteAnims, emoteIterations);
      this.sm.emoter.Set(emoter, this.smi, false);
    }

    public StatesInstance(
      EmoteChore master,
      GameObject emoter,
      HashedString animFile,
      HashedString[] anims,
      KAnim.PlayMode mode,
      bool flip_x)
      : base(master)
    {
      this.mode = mode;
      this.animFile = Assets.GetAnim(animFile);
      this.emoteAnims = anims;
      this.sm.emoter.Set(emoter, this.smi, false);
    }
  }

  public class States : GameStateMachine<EmoteChore.States, EmoteChore.StatesInstance, EmoteChore>
  {
    public StateMachine<EmoteChore.States, EmoteChore.StatesInstance, EmoteChore, object>.TargetParameter emoter;
    public GameStateMachine<EmoteChore.States, EmoteChore.StatesInstance, EmoteChore, object>.State finish;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.root;
      this.Target(this.emoter);
      this.root.ToggleAnims((Func<EmoteChore.StatesInstance, KAnimFile>) (smi => smi.animFile)).PlayAnims((Func<EmoteChore.StatesInstance, HashedString[]>) (smi => smi.emoteAnims), (Func<EmoteChore.StatesInstance, KAnim.PlayMode>) (smi => smi.mode)).ScheduleGoTo(10f, (StateMachine.BaseState) this.finish).OnAnimQueueComplete(this.finish);
      this.finish.ReturnSuccess();
    }
  }
}
