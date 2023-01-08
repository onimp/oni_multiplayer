// Decompiled with JetBrains decompiler
// Type: SuperProductive
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class SuperProductive : GameStateMachine<SuperProductive, SuperProductive.Instance>
{
  public GameStateMachine<SuperProductive, SuperProductive.Instance, IStateMachineTarget, object>.State neutral;
  public SuperProductive.OverjoyedStates overjoyed;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.neutral;
    this.root.TagTransition(GameTags.Dead, (GameStateMachine<SuperProductive, SuperProductive.Instance, IStateMachineTarget, object>.State) null);
    this.neutral.TagTransition(GameTags.Overjoyed, (GameStateMachine<SuperProductive, SuperProductive.Instance, IStateMachineTarget, object>.State) this.overjoyed);
    this.overjoyed.TagTransition(GameTags.Overjoyed, this.neutral, true).ToggleStatusItem(Db.Get().DuplicantStatusItems.BeingProductive).Enter((StateMachine<SuperProductive, SuperProductive.Instance, IStateMachineTarget, object>.State.Callback) (smi =>
    {
      if (Object.op_Inequality((Object) PopFXManager.Instance, (Object) null))
        PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, (string) DUPLICANTS.TRAITS.SUPERPRODUCTIVE.NAME, smi.master.transform, new Vector3(0.0f, 0.5f, 0.0f));
      smi.fx = new SuperProductiveFX.Instance((IStateMachineTarget) smi.GetComponent<KMonoBehaviour>(), new Vector3(0.0f, 0.0f, -0.1f));
      smi.fx.StartSM();
    })).Exit((StateMachine<SuperProductive, SuperProductive.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.fx.sm.destroyFX.Trigger(smi.fx))).DefaultState(this.overjoyed.idle);
    this.overjoyed.idle.EventTransition(GameHashes.StartWork, this.overjoyed.working);
    this.overjoyed.working.ScheduleGoTo(0.33f, (StateMachine.BaseState) this.overjoyed.superProductive);
    this.overjoyed.superProductive.Enter((StateMachine<SuperProductive, SuperProductive.Instance, IStateMachineTarget, object>.State.Callback) (smi =>
    {
      Worker component = smi.GetComponent<Worker>();
      if (Object.op_Inequality((Object) component, (Object) null) && component.state == Worker.State.Working)
      {
        Workable workable = component.workable;
        if (Object.op_Inequality((Object) workable, (Object) null))
        {
          float num = workable.WorkTimeRemaining;
          if (Object.op_Inequality((Object) ((Component) workable).GetComponent<Diggable>(), (Object) null))
            num = Diggable.GetApproximateDigTime(Grid.PosToCell((KMonoBehaviour) workable));
          if ((double) num > 1.0 && smi.ShouldSkipWork() && component.InstantlyFinish())
          {
            smi.ReactSuperProductive();
            smi.fx.sm.wasProductive.Trigger(smi.fx);
          }
        }
      }
      smi.GoTo((StateMachine.BaseState) this.overjoyed.idle);
    }));
  }

  public class OverjoyedStates : 
    GameStateMachine<SuperProductive, SuperProductive.Instance, IStateMachineTarget, object>.State
  {
    public GameStateMachine<SuperProductive, SuperProductive.Instance, IStateMachineTarget, object>.State idle;
    public GameStateMachine<SuperProductive, SuperProductive.Instance, IStateMachineTarget, object>.State working;
    public GameStateMachine<SuperProductive, SuperProductive.Instance, IStateMachineTarget, object>.State superProductive;
  }

  public new class Instance : 
    GameStateMachine<SuperProductive, SuperProductive.Instance, IStateMachineTarget, object>.GameInstance
  {
    public SuperProductiveFX.Instance fx;

    public Instance(IStateMachineTarget master)
      : base(master)
    {
    }

    public bool ShouldSkipWork() => (double) Random.Range(0.0f, 100f) <= (double) TUNING.TRAITS.JOY_REACTIONS.SUPER_PRODUCTIVE.INSTANT_SUCCESS_CHANCE;

    public void ReactSuperProductive() => this.gameObject.GetSMI<ReactionMonitor.Instance>()?.AddSelfEmoteReactable(this.gameObject, HashedString.op_Implicit(nameof (SuperProductive)), Db.Get().Emotes.Minion.ProductiveCheer, true, Db.Get().ChoreTypes.EmoteHighPriority, localCooldown: 1f, lifeSpan: 1f);
  }
}
