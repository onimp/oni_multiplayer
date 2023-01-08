// Decompiled with JetBrains decompiler
// Type: IdleChore
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class IdleChore : Chore<IdleChore.StatesInstance>
{
  public IdleChore(IStateMachineTarget target)
    : base(Db.Get().ChoreTypes.Idle, target, target.GetComponent<ChoreProvider>(), false, master_priority_class: PriorityScreen.PriorityClass.idle, report_type: ReportManager.ReportType.IdleTime)
  {
    this.showAvailabilityInHoverText = false;
    this.smi = new IdleChore.StatesInstance(this, target.gameObject);
  }

  public class StatesInstance : 
    GameStateMachine<IdleChore.States, IdleChore.StatesInstance, IdleChore, object>.GameInstance
  {
    private IdleCellSensor idleCellSensor;

    public StatesInstance(IdleChore master, GameObject idler)
      : base(master)
    {
      this.sm.idler.Set(idler, this.smi, false);
      this.idleCellSensor = this.GetComponent<Sensors>().GetSensor<IdleCellSensor>();
    }

    public void UpdateNavType()
    {
      NavType currentNavType = this.GetComponent<Navigator>().CurrentNavType;
      this.sm.isOnLadder.Set(currentNavType == NavType.Ladder || currentNavType == NavType.Pole, this);
      this.sm.isOnTube.Set(currentNavType == NavType.Tube, this);
    }

    public int GetIdleCell() => this.idleCellSensor.GetCell();

    public bool HasIdleCell() => this.idleCellSensor.GetCell() != Grid.InvalidCell;
  }

  public class States : GameStateMachine<IdleChore.States, IdleChore.StatesInstance, IdleChore>
  {
    public StateMachine<IdleChore.States, IdleChore.StatesInstance, IdleChore, object>.BoolParameter isOnLadder;
    public StateMachine<IdleChore.States, IdleChore.StatesInstance, IdleChore, object>.BoolParameter isOnTube;
    public StateMachine<IdleChore.States, IdleChore.StatesInstance, IdleChore, object>.TargetParameter idler;
    public IdleChore.States.IdleState idle;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.idle;
      this.Target(this.idler);
      this.idle.DefaultState(this.idle.onfloor).Enter("UpdateNavType", (StateMachine<IdleChore.States, IdleChore.StatesInstance, IdleChore, object>.State.Callback) (smi => smi.UpdateNavType())).Update("UpdateNavType", (Action<IdleChore.StatesInstance, float>) ((smi, dt) => smi.UpdateNavType())).ToggleStateMachine((Func<IdleChore.StatesInstance, StateMachine.Instance>) (smi => (StateMachine.Instance) new TaskAvailabilityMonitor.Instance((IStateMachineTarget) smi.master))).ToggleTag(GameTags.Idle);
      this.idle.onfloor.PlayAnim("idle_default", (KAnim.PlayMode) 0).ParamTransition<bool>((StateMachine<IdleChore.States, IdleChore.StatesInstance, IdleChore, object>.Parameter<bool>) this.isOnLadder, this.idle.onladder, GameStateMachine<IdleChore.States, IdleChore.StatesInstance, IdleChore, object>.IsTrue).ParamTransition<bool>((StateMachine<IdleChore.States, IdleChore.StatesInstance, IdleChore, object>.Parameter<bool>) this.isOnTube, this.idle.ontube, GameStateMachine<IdleChore.States, IdleChore.StatesInstance, IdleChore, object>.IsTrue).ToggleScheduleCallback("IdleMove", (Func<IdleChore.StatesInstance, float>) (smi => (float) Random.Range(5, 15)), (Action<IdleChore.StatesInstance>) (smi => smi.GoTo((StateMachine.BaseState) this.idle.move)));
      this.idle.onladder.PlayAnim("ladder_idle", (KAnim.PlayMode) 0).ToggleScheduleCallback("IdleMove", (Func<IdleChore.StatesInstance, float>) (smi => (float) Random.Range(5, 15)), (Action<IdleChore.StatesInstance>) (smi => smi.GoTo((StateMachine.BaseState) this.idle.move)));
      this.idle.ontube.PlayAnim("tube_idle_loop", (KAnim.PlayMode) 0).Update("IdleMove", (Action<IdleChore.StatesInstance, float>) ((smi, dt) =>
      {
        if (!smi.HasIdleCell())
          return;
        smi.GoTo((StateMachine.BaseState) this.idle.move);
      }), (UpdateRate) 6);
      this.idle.move.Transition((GameStateMachine<IdleChore.States, IdleChore.StatesInstance, IdleChore, object>.State) this.idle, (StateMachine<IdleChore.States, IdleChore.StatesInstance, IdleChore, object>.Transition.ConditionCallback) (smi => !smi.HasIdleCell())).TriggerOnEnter(GameHashes.BeginWalk).TriggerOnExit(GameHashes.EndWalk).ToggleAnims("anim_loco_walk_kanim").MoveTo((Func<IdleChore.StatesInstance, int>) (smi => smi.GetIdleCell()), (GameStateMachine<IdleChore.States, IdleChore.StatesInstance, IdleChore, object>.State) this.idle, (GameStateMachine<IdleChore.States, IdleChore.StatesInstance, IdleChore, object>.State) this.idle).Exit("UpdateNavType", (StateMachine<IdleChore.States, IdleChore.StatesInstance, IdleChore, object>.State.Callback) (smi => smi.UpdateNavType())).Exit("ClearWalk", (StateMachine<IdleChore.States, IdleChore.StatesInstance, IdleChore, object>.State.Callback) (smi => smi.GetComponent<KBatchedAnimController>().Play(HashedString.op_Implicit("idle_default"))));
    }

    public class IdleState : 
      GameStateMachine<IdleChore.States, IdleChore.StatesInstance, IdleChore, object>.State
    {
      public GameStateMachine<IdleChore.States, IdleChore.StatesInstance, IdleChore, object>.State onfloor;
      public GameStateMachine<IdleChore.States, IdleChore.StatesInstance, IdleChore, object>.State onladder;
      public GameStateMachine<IdleChore.States, IdleChore.StatesInstance, IdleChore, object>.State ontube;
      public GameStateMachine<IdleChore.States, IdleChore.StatesInstance, IdleChore, object>.State move;
    }
  }
}
