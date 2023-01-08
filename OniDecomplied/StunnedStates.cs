// Decompiled with JetBrains decompiler
// Type: StunnedStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

public class StunnedStates : 
  GameStateMachine<StunnedStates, StunnedStates.Instance, IStateMachineTarget, StunnedStates.Def>
{
  public GameStateMachine<StunnedStates, StunnedStates.Instance, IStateMachineTarget, StunnedStates.Def>.State stunned;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.stunned;
    this.root.ToggleStatusItem((string) CREATURES.STATUSITEMS.GETTING_WRANGLED.NAME, (string) CREATURES.STATUSITEMS.GETTING_WRANGLED.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main);
    this.stunned.PlayAnim("idle_loop", (KAnim.PlayMode) 0).TagTransition(GameTags.Creatures.Stunned, (GameStateMachine<StunnedStates, StunnedStates.Instance, IStateMachineTarget, StunnedStates.Def>.State) null, true);
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<StunnedStates, StunnedStates.Instance, IStateMachineTarget, StunnedStates.Def>.GameInstance
  {
    public static readonly Chore.Precondition IsStunned = new Chore.Precondition()
    {
      id = nameof (IsStunned),
      fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) => context.consumerState.prefabid.HasTag(GameTags.Creatures.Stunned))
    };

    public Instance(Chore<StunnedStates.Instance> chore, StunnedStates.Def def)
      : base((IStateMachineTarget) chore, def)
    {
      chore.AddPrecondition(StunnedStates.Instance.IsStunned);
    }
  }
}
