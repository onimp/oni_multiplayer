// Decompiled with JetBrains decompiler
// Type: ImmobileDrowningStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

public class ImmobileDrowningStates : 
  GameStateMachine<ImmobileDrowningStates, ImmobileDrowningStates.Instance, IStateMachineTarget, ImmobileDrowningStates.Def>
{
  public GameStateMachine<ImmobileDrowningStates, ImmobileDrowningStates.Instance, IStateMachineTarget, ImmobileDrowningStates.Def>.State drown;
  public GameStateMachine<ImmobileDrowningStates, ImmobileDrowningStates.Instance, IStateMachineTarget, ImmobileDrowningStates.Def>.State drown_pst;
  public GameStateMachine<ImmobileDrowningStates, ImmobileDrowningStates.Instance, IStateMachineTarget, ImmobileDrowningStates.Def>.State behaviourcomplete;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.drown;
    this.root.ToggleStatusItem((string) CREATURES.STATUSITEMS.DROWNING.NAME, (string) CREATURES.STATUSITEMS.DROWNING.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main).TagTransition(GameTags.Creatures.Drowning, this.drown_pst, true);
    this.drown.PlayAnim("drown_pre").QueueAnim("drown_loop", true);
    this.drown_pst.PlayAnim("drown_pst").OnAnimQueueComplete(this.behaviourcomplete);
    this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.Drowning);
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<ImmobileDrowningStates, ImmobileDrowningStates.Instance, IStateMachineTarget, ImmobileDrowningStates.Def>.GameInstance
  {
    public Instance(Chore<ImmobileDrowningStates.Instance> chore, ImmobileDrowningStates.Def def)
      : base((IStateMachineTarget) chore, def)
    {
      chore.AddPrecondition(ChorePreconditions.instance.HasTag, (object) GameTags.Creatures.Drowning);
    }
  }
}
