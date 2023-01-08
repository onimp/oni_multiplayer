// Decompiled with JetBrains decompiler
// Type: SimpleDoorController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class SimpleDoorController : 
  GameStateMachine<SimpleDoorController, SimpleDoorController.StatesInstance, IStateMachineTarget, SimpleDoorController.Def>
{
  public GameStateMachine<SimpleDoorController, SimpleDoorController.StatesInstance, IStateMachineTarget, SimpleDoorController.Def>.State inactive;
  public SimpleDoorController.ActiveStates active;
  public StateMachine<SimpleDoorController, SimpleDoorController.StatesInstance, IStateMachineTarget, SimpleDoorController.Def>.IntParameter numOpens;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.inactive;
    this.inactive.TagTransition(GameTags.RocketOnGround, (GameStateMachine<SimpleDoorController, SimpleDoorController.StatesInstance, IStateMachineTarget, SimpleDoorController.Def>.State) this.active);
    this.active.DefaultState(this.active.closed).TagTransition(GameTags.RocketOnGround, this.inactive, true).Enter((StateMachine<SimpleDoorController, SimpleDoorController.StatesInstance, IStateMachineTarget, SimpleDoorController.Def>.State.Callback) (smi => smi.Register())).Exit((StateMachine<SimpleDoorController, SimpleDoorController.StatesInstance, IStateMachineTarget, SimpleDoorController.Def>.State.Callback) (smi => smi.Unregister()));
    this.active.closed.PlayAnim((Func<SimpleDoorController.StatesInstance, string>) (smi => smi.GetDefaultAnim()), (KAnim.PlayMode) 0).ParamTransition<int>((StateMachine<SimpleDoorController, SimpleDoorController.StatesInstance, IStateMachineTarget, SimpleDoorController.Def>.Parameter<int>) this.numOpens, this.active.opening, (StateMachine<SimpleDoorController, SimpleDoorController.StatesInstance, IStateMachineTarget, SimpleDoorController.Def>.Parameter<int>.Callback) ((smi, p) => p > 0));
    this.active.opening.PlayAnim("enter_pre", (KAnim.PlayMode) 1).OnAnimQueueComplete(this.active.open);
    this.active.open.PlayAnim("enter_loop", (KAnim.PlayMode) 0).ParamTransition<int>((StateMachine<SimpleDoorController, SimpleDoorController.StatesInstance, IStateMachineTarget, SimpleDoorController.Def>.Parameter<int>) this.numOpens, this.active.closedelay, (StateMachine<SimpleDoorController, SimpleDoorController.StatesInstance, IStateMachineTarget, SimpleDoorController.Def>.Parameter<int>.Callback) ((smi, p) => p == 0));
    this.active.closedelay.ParamTransition<int>((StateMachine<SimpleDoorController, SimpleDoorController.StatesInstance, IStateMachineTarget, SimpleDoorController.Def>.Parameter<int>) this.numOpens, this.active.open, (StateMachine<SimpleDoorController, SimpleDoorController.StatesInstance, IStateMachineTarget, SimpleDoorController.Def>.Parameter<int>.Callback) ((smi, p) => p > 0)).ScheduleGoTo(0.5f, (StateMachine.BaseState) this.active.closing);
    this.active.closing.PlayAnim("enter_pst", (KAnim.PlayMode) 1).OnAnimQueueComplete(this.active.closed);
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public class ActiveStates : 
    GameStateMachine<SimpleDoorController, SimpleDoorController.StatesInstance, IStateMachineTarget, SimpleDoorController.Def>.State
  {
    public GameStateMachine<SimpleDoorController, SimpleDoorController.StatesInstance, IStateMachineTarget, SimpleDoorController.Def>.State closed;
    public GameStateMachine<SimpleDoorController, SimpleDoorController.StatesInstance, IStateMachineTarget, SimpleDoorController.Def>.State opening;
    public GameStateMachine<SimpleDoorController, SimpleDoorController.StatesInstance, IStateMachineTarget, SimpleDoorController.Def>.State open;
    public GameStateMachine<SimpleDoorController, SimpleDoorController.StatesInstance, IStateMachineTarget, SimpleDoorController.Def>.State closedelay;
    public GameStateMachine<SimpleDoorController, SimpleDoorController.StatesInstance, IStateMachineTarget, SimpleDoorController.Def>.State closing;
  }

  public class StatesInstance : 
    GameStateMachine<SimpleDoorController, SimpleDoorController.StatesInstance, IStateMachineTarget, SimpleDoorController.Def>.GameInstance,
    INavDoor
  {
    public StatesInstance(IStateMachineTarget master, SimpleDoorController.Def def)
      : base(master, def)
    {
    }

    public string GetDefaultAnim()
    {
      KBatchedAnimController component = this.master.GetComponent<KBatchedAnimController>();
      return Object.op_Inequality((Object) component, (Object) null) ? component.initialAnim : "idle_loop";
    }

    public void Register()
    {
      int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.gameObject.transform));
      Grid.HasDoor[cell] = true;
    }

    public void Unregister()
    {
      int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.gameObject.transform));
      Grid.HasDoor[cell] = false;
    }

    public bool isSpawned => this.master.gameObject.GetComponent<KMonoBehaviour>().isSpawned;

    public void Close() => this.sm.numOpens.Delta(-1, this.smi);

    public bool IsOpen() => this.IsInsideState((StateMachine.BaseState) this.sm.active.open) || this.IsInsideState((StateMachine.BaseState) this.sm.active.closedelay);

    public void Open() => this.sm.numOpens.Delta(1, this.smi);
  }
}
