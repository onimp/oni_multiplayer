// Decompiled with JetBrains decompiler
// Type: DeathStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class DeathStates : 
  GameStateMachine<DeathStates, DeathStates.Instance, IStateMachineTarget, DeathStates.Def>
{
  private GameStateMachine<DeathStates, DeathStates.Instance, IStateMachineTarget, DeathStates.Def>.State loop;
  private GameStateMachine<DeathStates, DeathStates.Instance, IStateMachineTarget, DeathStates.Def>.State pst;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.loop;
    this.loop.ToggleStatusItem((string) CREATURES.STATUSITEMS.DEAD.NAME, (string) CREATURES.STATUSITEMS.DEAD.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main).Enter("EnableGravity", (StateMachine<DeathStates, DeathStates.Instance, IStateMachineTarget, DeathStates.Def>.State.Callback) (smi => smi.EnableGravityIfNecessary())).PlayAnim("Death").OnAnimQueueComplete(this.pst);
    this.pst.TriggerOnEnter(GameHashes.DeathAnimComplete).TriggerOnEnter(GameHashes.Died).Enter("Butcher", (StateMachine<DeathStates, DeathStates.Instance, IStateMachineTarget, DeathStates.Def>.State.Callback) (smi =>
    {
      if (!Object.op_Inequality((Object) smi.gameObject.GetComponent<Butcherable>(), (Object) null))
        return;
      smi.GetComponent<Butcherable>().OnButcherComplete();
    })).Enter("Destroy", (StateMachine<DeathStates, DeathStates.Instance, IStateMachineTarget, DeathStates.Def>.State.Callback) (smi => TracesExtesions.DeleteObject(smi.gameObject))).BehaviourComplete(GameTags.Creatures.Die);
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<DeathStates, DeathStates.Instance, IStateMachineTarget, DeathStates.Def>.GameInstance
  {
    public Instance(Chore<DeathStates.Instance> chore, DeathStates.Def def)
      : base((IStateMachineTarget) chore, def)
    {
      chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, (object) GameTags.Creatures.Die);
    }

    public void EnableGravityIfNecessary()
    {
      if (!this.HasTag(GameTags.Creatures.Flyer))
        return;
      GameComps.Gravities.Add(this.smi.gameObject, Vector2.zero, (System.Action) (() => this.smi.DisableGravity()));
    }

    public void DisableGravity()
    {
      if (!((KComponentManager<GravityComponent>) GameComps.Gravities).Has((object) this.smi.gameObject))
        return;
      GameComps.Gravities.Remove(this.smi.gameObject);
    }
  }
}
