// Decompiled with JetBrains decompiler
// Type: TrappedStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

public class TrappedStates : 
  GameStateMachine<TrappedStates, TrappedStates.Instance, IStateMachineTarget, TrappedStates.Def>
{
  private GameStateMachine<TrappedStates, TrappedStates.Instance, IStateMachineTarget, TrappedStates.Def>.State trapped;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.trapped;
    this.root.ToggleStatusItem((string) CREATURES.STATUSITEMS.TRAPPED.NAME, (string) CREATURES.STATUSITEMS.TRAPPED.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main);
    this.trapped.Enter((StateMachine<TrappedStates, TrappedStates.Instance, IStateMachineTarget, TrappedStates.Def>.State.Callback) (smi =>
    {
      Navigator component = smi.GetComponent<Navigator>();
      if (!component.IsValidNavType(NavType.Floor))
        return;
      component.SetCurrentNavType(NavType.Floor);
    })).ToggleTag(GameTags.Creatures.Deliverable).PlayAnim("trapped", (KAnim.PlayMode) 0).TagTransition(GameTags.Trapped, (GameStateMachine<TrappedStates, TrappedStates.Instance, IStateMachineTarget, TrappedStates.Def>.State) null, true);
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<TrappedStates, TrappedStates.Instance, IStateMachineTarget, TrappedStates.Def>.GameInstance
  {
    public static readonly Chore.Precondition IsTrapped = new Chore.Precondition()
    {
      id = nameof (IsTrapped),
      fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) => context.consumerState.prefabid.HasTag(GameTags.Trapped))
    };

    public Instance(Chore<TrappedStates.Instance> chore, TrappedStates.Def def)
      : base((IStateMachineTarget) chore, def)
    {
      chore.AddPrecondition(TrappedStates.Instance.IsTrapped);
    }
  }
}
