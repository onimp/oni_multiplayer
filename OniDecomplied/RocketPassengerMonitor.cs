// Decompiled with JetBrains decompiler
// Type: RocketPassengerMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

public class RocketPassengerMonitor : 
  GameStateMachine<RocketPassengerMonitor, RocketPassengerMonitor.Instance>
{
  public StateMachine<RocketPassengerMonitor, RocketPassengerMonitor.Instance, IStateMachineTarget, object>.IntParameter targetCell = new StateMachine<RocketPassengerMonitor, RocketPassengerMonitor.Instance, IStateMachineTarget, object>.IntParameter(Grid.InvalidCell);
  public GameStateMachine<RocketPassengerMonitor, RocketPassengerMonitor.Instance, IStateMachineTarget, object>.State satisfied;
  public GameStateMachine<RocketPassengerMonitor, RocketPassengerMonitor.Instance, IStateMachineTarget, object>.State moving;
  public GameStateMachine<RocketPassengerMonitor, RocketPassengerMonitor.Instance, IStateMachineTarget, object>.State movingToModuleDeployPre;
  public GameStateMachine<RocketPassengerMonitor, RocketPassengerMonitor.Instance, IStateMachineTarget, object>.State movingToModuleDeploy;
  public GameStateMachine<RocketPassengerMonitor, RocketPassengerMonitor.Instance, IStateMachineTarget, object>.State moduleDeploy;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.satisfied;
    this.serializable = StateMachine.SerializeType.ParamsOnly;
    this.satisfied.ParamTransition<int>((StateMachine<RocketPassengerMonitor, RocketPassengerMonitor.Instance, IStateMachineTarget, object>.Parameter<int>) this.targetCell, this.moving, (StateMachine<RocketPassengerMonitor, RocketPassengerMonitor.Instance, IStateMachineTarget, object>.Parameter<int>.Callback) ((smi, p) => p != Grid.InvalidCell));
    this.moving.ParamTransition<int>((StateMachine<RocketPassengerMonitor, RocketPassengerMonitor.Instance, IStateMachineTarget, object>.Parameter<int>) this.targetCell, this.satisfied, (StateMachine<RocketPassengerMonitor, RocketPassengerMonitor.Instance, IStateMachineTarget, object>.Parameter<int>.Callback) ((smi, p) => p == Grid.InvalidCell)).ToggleChore((Func<RocketPassengerMonitor.Instance, Chore>) (smi => this.CreateChore(smi)), this.satisfied).Exit((StateMachine<RocketPassengerMonitor, RocketPassengerMonitor.Instance, IStateMachineTarget, object>.State.Callback) (smi => this.targetCell.Set(Grid.InvalidCell, smi)));
    this.movingToModuleDeployPre.Enter((StateMachine<RocketPassengerMonitor, RocketPassengerMonitor.Instance, IStateMachineTarget, object>.State.Callback) (smi =>
    {
      this.targetCell.Set(smi.moduleDeployTaskTargetMoveCell, smi);
      smi.GoTo((StateMachine.BaseState) this.movingToModuleDeploy);
    }));
    this.movingToModuleDeploy.ParamTransition<int>((StateMachine<RocketPassengerMonitor, RocketPassengerMonitor.Instance, IStateMachineTarget, object>.Parameter<int>) this.targetCell, this.satisfied, (StateMachine<RocketPassengerMonitor, RocketPassengerMonitor.Instance, IStateMachineTarget, object>.Parameter<int>.Callback) ((smi, p) => p == Grid.InvalidCell)).ToggleChore((Func<RocketPassengerMonitor.Instance, Chore>) (smi => this.CreateChore(smi)), this.moduleDeploy);
    this.moduleDeploy.Enter((StateMachine<RocketPassengerMonitor, RocketPassengerMonitor.Instance, IStateMachineTarget, object>.State.Callback) (smi =>
    {
      smi.moduleDeployCompleteCallback((Chore) null);
      this.targetCell.Set(Grid.InvalidCell, smi);
      smi.moduleDeployCompleteCallback = (System.Action<Chore>) null;
      smi.GoTo((StateMachine.BaseState) smi.sm.satisfied);
    }));
  }

  public Chore CreateChore(RocketPassengerMonitor.Instance smi)
  {
    MoveChore chore = new MoveChore(smi.master, Db.Get().ChoreTypes.RocketEnterExit, (Func<MoveChore.StatesInstance, int>) (mover_smi => this.targetCell.Get(smi)));
    chore.AddPrecondition(ChorePreconditions.instance.CanMoveToCell, (object) this.targetCell.Get(smi));
    return (Chore) chore;
  }

  public new class Instance : 
    GameStateMachine<RocketPassengerMonitor, RocketPassengerMonitor.Instance, IStateMachineTarget, object>.GameInstance
  {
    public int lastWorldID;
    public System.Action<Chore> moduleDeployCompleteCallback;
    public int moduleDeployTaskTargetMoveCell;

    public Instance(IStateMachineTarget master)
      : base(master)
    {
    }

    public bool ShouldMoveThroughRocketDoor()
    {
      int cell = this.sm.targetCell.Get(this);
      if (!Grid.IsValidCell(cell))
        return false;
      if ((int) Grid.WorldIdx[cell] != this.GetMyWorldId())
        return true;
      this.sm.targetCell.Set(Grid.InvalidCell, this);
      return false;
    }

    public void SetMoveTarget(int cell)
    {
      if ((int) Grid.WorldIdx[cell] == this.GetMyWorldId())
        return;
      this.sm.targetCell.Set(cell, this);
    }

    public void SetModuleDeployChore(int cell, System.Action<Chore> OnChoreCompleteCallback)
    {
      this.moduleDeployCompleteCallback = OnChoreCompleteCallback;
      this.moduleDeployTaskTargetMoveCell = cell;
      this.GoTo((StateMachine.BaseState) this.sm.movingToModuleDeployPre);
      this.sm.targetCell.Set(cell, this);
    }

    public void CancelModuleDeployChore()
    {
      this.moduleDeployCompleteCallback = (System.Action<Chore>) null;
      this.moduleDeployTaskTargetMoveCell = Grid.InvalidCell;
      this.sm.targetCell.Set(Grid.InvalidCell, this.smi);
    }

    public void ClearMoveTarget(int testCell)
    {
      int cell = this.sm.targetCell.Get(this);
      if (!Grid.IsValidCell(cell) || (int) Grid.WorldIdx[cell] != (int) Grid.WorldIdx[testCell])
        return;
      this.sm.targetCell.Set(Grid.InvalidCell, this);
      if (!this.IsInsideState((StateMachine.BaseState) this.sm.moving))
        return;
      this.GoTo((StateMachine.BaseState) this.sm.satisfied);
    }
  }
}
