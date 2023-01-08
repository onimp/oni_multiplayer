// Decompiled with JetBrains decompiler
// Type: FallStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;

public class FallStates : 
  GameStateMachine<FallStates, FallStates.Instance, IStateMachineTarget, FallStates.Def>
{
  private GameStateMachine<FallStates, FallStates.Instance, IStateMachineTarget, FallStates.Def>.State loop;
  private GameStateMachine<FallStates, FallStates.Instance, IStateMachineTarget, FallStates.Def>.State snaptoground;
  private GameStateMachine<FallStates, FallStates.Instance, IStateMachineTarget, FallStates.Def>.State pst;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.loop;
    this.root.ToggleStatusItem((string) CREATURES.STATUSITEMS.FALLING.NAME, (string) CREATURES.STATUSITEMS.FALLING.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main);
    this.loop.PlayAnim((Func<FallStates.Instance, string>) (smi => smi.GetSMI<CreatureFallMonitor.Instance>().anim), (KAnim.PlayMode) 0).ToggleGravity().EventTransition(GameHashes.Landed, this.snaptoground).Transition(this.pst, (StateMachine<FallStates, FallStates.Instance, IStateMachineTarget, FallStates.Def>.Transition.ConditionCallback) (smi => smi.GetSMI<CreatureFallMonitor.Instance>().CanSwimAtCurrentLocation()), (UpdateRate) 4);
    this.snaptoground.Enter((StateMachine<FallStates, FallStates.Instance, IStateMachineTarget, FallStates.Def>.State.Callback) (smi => smi.GetSMI<CreatureFallMonitor.Instance>().SnapToGround())).GoTo(this.pst);
    this.pst.Enter(new StateMachine<FallStates, FallStates.Instance, IStateMachineTarget, FallStates.Def>.State.Callback(FallStates.PlayLandAnim)).BehaviourComplete(GameTags.Creatures.Falling);
  }

  private static void PlayLandAnim(FallStates.Instance smi) => smi.GetComponent<KBatchedAnimController>().Queue(HashedString.op_Implicit(smi.def.getLandAnim(smi)), (KAnim.PlayMode) 0);

  public class Def : StateMachine.BaseDef
  {
    public Func<FallStates.Instance, string> getLandAnim = (Func<FallStates.Instance, string>) (smi => "idle_loop");
  }

  public new class Instance : 
    GameStateMachine<FallStates, FallStates.Instance, IStateMachineTarget, FallStates.Def>.GameInstance
  {
    public Instance(Chore<FallStates.Instance> chore, FallStates.Def def)
      : base((IStateMachineTarget) chore, def)
    {
      chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, (object) GameTags.Creatures.Falling);
    }
  }
}
