// Decompiled with JetBrains decompiler
// Type: IdleStandStillStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

public class IdleStandStillStates : 
  GameStateMachine<IdleStandStillStates, IdleStandStillStates.Instance, IStateMachineTarget, IdleStandStillStates.Def>
{
  private GameStateMachine<IdleStandStillStates, IdleStandStillStates.Instance, IStateMachineTarget, IdleStandStillStates.Def>.State loop;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.loop;
    this.root.ToggleStatusItem((string) CREATURES.STATUSITEMS.IDLE.NAME, (string) CREATURES.STATUSITEMS.IDLE.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main).ToggleTag(GameTags.Idle);
    this.loop.Enter(new StateMachine<IdleStandStillStates, IdleStandStillStates.Instance, IStateMachineTarget, IdleStandStillStates.Def>.State.Callback(this.PlayIdle));
  }

  public void PlayIdle(IdleStandStillStates.Instance smi)
  {
    KAnimControllerBase component = smi.GetComponent<KAnimControllerBase>();
    if (smi.def.customIdleAnim != null)
    {
      HashedString invalid = HashedString.Invalid;
      HashedString anim_name = smi.def.customIdleAnim(smi, ref invalid);
      if (HashedString.op_Inequality(anim_name, HashedString.Invalid))
      {
        if (HashedString.op_Inequality(invalid, HashedString.Invalid))
          component.Play(invalid);
        component.Queue(anim_name, (KAnim.PlayMode) 0);
        return;
      }
    }
    component.Play(HashedString.op_Implicit("idle"), (KAnim.PlayMode) 0);
  }

  public class Def : StateMachine.BaseDef
  {
    public IdleStandStillStates.Def.IdleAnimCallback customIdleAnim;

    public delegate HashedString IdleAnimCallback(
      IdleStandStillStates.Instance smi,
      ref HashedString pre_anim);
  }

  public new class Instance : 
    GameStateMachine<IdleStandStillStates, IdleStandStillStates.Instance, IStateMachineTarget, IdleStandStillStates.Def>.GameInstance
  {
    public Instance(Chore<IdleStandStillStates.Instance> chore, IdleStandStillStates.Def def)
      : base((IStateMachineTarget) chore, def)
    {
    }
  }
}
