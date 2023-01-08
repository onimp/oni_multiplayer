// Decompiled with JetBrains decompiler
// Type: HiveGrowingStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;

public class HiveGrowingStates : 
  GameStateMachine<HiveGrowingStates, HiveGrowingStates.Instance, IStateMachineTarget, HiveGrowingStates.Def>
{
  public HiveGrowingStates.GrowUpStates growing;
  public GameStateMachine<HiveGrowingStates, HiveGrowingStates.Instance, IStateMachineTarget, HiveGrowingStates.Def>.State behaviourcomplete;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.growing;
    this.root.ToggleStatusItem((string) CREATURES.STATUSITEMS.GROWINGUP.NAME, (string) CREATURES.STATUSITEMS.GROWINGUP.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main);
    this.growing.DefaultState(this.growing.loop);
    this.growing.loop.PlayAnim((Func<HiveGrowingStates.Instance, string>) (smi => "grow"), (KAnim.PlayMode) 2).Enter((StateMachine<HiveGrowingStates, HiveGrowingStates.Instance, IStateMachineTarget, HiveGrowingStates.Def>.State.Callback) (smi => smi.RefreshPositionPercent())).Update((System.Action<HiveGrowingStates.Instance, float>) ((smi, dt) =>
    {
      smi.RefreshPositionPercent();
      if (!smi.hive.IsFullyGrown())
        return;
      smi.GoTo((StateMachine.BaseState) this.growing.pst);
    }), (UpdateRate) 7);
    this.growing.pst.PlayAnim("grow_pst", (KAnim.PlayMode) 1).OnAnimQueueComplete(this.behaviourcomplete);
    this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.Behaviours.GrowUpBehaviour);
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<HiveGrowingStates, HiveGrowingStates.Instance, IStateMachineTarget, HiveGrowingStates.Def>.GameInstance
  {
    [MySmiReq]
    public BeeHive.StatesInstance hive;
    [MyCmpReq]
    private KAnimControllerBase animController;

    public Instance(Chore<HiveGrowingStates.Instance> chore, HiveGrowingStates.Def def)
      : base((IStateMachineTarget) chore, def)
    {
      chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, (object) GameTags.Creatures.Behaviours.GrowUpBehaviour);
    }

    public void RefreshPositionPercent() => this.animController.SetPositionPercent(this.hive.sm.hiveGrowth.Get(this.hive));
  }

  public class GrowUpStates : 
    GameStateMachine<HiveGrowingStates, HiveGrowingStates.Instance, IStateMachineTarget, HiveGrowingStates.Def>.State
  {
    public GameStateMachine<HiveGrowingStates, HiveGrowingStates.Instance, IStateMachineTarget, HiveGrowingStates.Def>.State loop;
    public GameStateMachine<HiveGrowingStates, HiveGrowingStates.Instance, IStateMachineTarget, HiveGrowingStates.Def>.State pst;
  }
}
