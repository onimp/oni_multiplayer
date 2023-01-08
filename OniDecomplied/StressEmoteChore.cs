// Decompiled with JetBrains decompiler
// Type: StressEmoteChore
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class StressEmoteChore : Chore<StressEmoteChore.StatesInstance>
{
  private Func<StatusItem> getStatusItem;

  public StressEmoteChore(
    IStateMachineTarget target,
    ChoreType chore_type,
    HashedString emote_kanim,
    HashedString[] emote_anims,
    KAnim.PlayMode play_mode,
    Func<StatusItem> get_status_item)
    : base(chore_type, target, target.GetComponent<ChoreProvider>(), false, master_priority_class: PriorityScreen.PriorityClass.compulsory)
  {
    this.AddPrecondition(ChorePreconditions.instance.IsMoving);
    this.AddPrecondition(ChorePreconditions.instance.IsOffLadder);
    this.AddPrecondition(ChorePreconditions.instance.NotInTube);
    this.AddPrecondition(ChorePreconditions.instance.IsAwake);
    this.getStatusItem = get_status_item;
    this.smi = new StressEmoteChore.StatesInstance(this, target.gameObject, emote_kanim, emote_anims, play_mode);
  }

  protected override StatusItem GetStatusItem() => this.getStatusItem == null ? base.GetStatusItem() : this.getStatusItem();

  public override string ToString() => ((HashedString) ref this.smi.emoteKAnim).IsValid ? "StressEmoteChore<" + this.smi.emoteKAnim.ToString() + ">" : "StressEmoteChore<" + this.smi.emoteAnims[0].ToString() + ">";

  public class StatesInstance : 
    GameStateMachine<StressEmoteChore.States, StressEmoteChore.StatesInstance, StressEmoteChore, object>.GameInstance
  {
    public HashedString[] emoteAnims;
    public HashedString emoteKAnim;
    public KAnim.PlayMode mode = (KAnim.PlayMode) 1;

    public StatesInstance(
      StressEmoteChore master,
      GameObject emoter,
      HashedString emote_kanim,
      HashedString[] emote_anims,
      KAnim.PlayMode mode)
      : base(master)
    {
      this.emoteKAnim = emote_kanim;
      this.emoteAnims = emote_anims;
      this.mode = mode;
      this.sm.emoter.Set(emoter, this.smi, false);
    }
  }

  public class States : 
    GameStateMachine<StressEmoteChore.States, StressEmoteChore.StatesInstance, StressEmoteChore>
  {
    public StateMachine<StressEmoteChore.States, StressEmoteChore.StatesInstance, StressEmoteChore, object>.TargetParameter emoter;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.root;
      this.Target(this.emoter);
      this.root.ToggleAnims((Func<StressEmoteChore.StatesInstance, HashedString>) (smi => smi.emoteKAnim)).ToggleThought(Db.Get().Thoughts.Unhappy).PlayAnims((Func<StressEmoteChore.StatesInstance, HashedString[]>) (smi => smi.emoteAnims), (Func<StressEmoteChore.StatesInstance, KAnim.PlayMode>) (smi => smi.mode)).OnAnimQueueComplete((GameStateMachine<StressEmoteChore.States, StressEmoteChore.StatesInstance, StressEmoteChore, object>.State) null);
    }
  }
}
