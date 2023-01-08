// Decompiled with JetBrains decompiler
// Type: ConduitSleepMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class ConduitSleepMonitor : 
  GameStateMachine<ConduitSleepMonitor, ConduitSleepMonitor.Instance, IStateMachineTarget, ConduitSleepMonitor.Def>
{
  private GameStateMachine<ConduitSleepMonitor, ConduitSleepMonitor.Instance, IStateMachineTarget, ConduitSleepMonitor.Def>.State idle;
  private ConduitSleepMonitor.SleepSearchStates searching;
  public StateMachine<ConduitSleepMonitor, ConduitSleepMonitor.Instance, IStateMachineTarget, ConduitSleepMonitor.Def>.IntParameter targetSleepCell = new StateMachine<ConduitSleepMonitor, ConduitSleepMonitor.Instance, IStateMachineTarget, ConduitSleepMonitor.Def>.IntParameter(Grid.InvalidCell);

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.idle;
    this.serializable = StateMachine.SerializeType.ParamsOnly;
    this.idle.Enter((StateMachine<ConduitSleepMonitor, ConduitSleepMonitor.Instance, IStateMachineTarget, ConduitSleepMonitor.Def>.State.Callback) (smi =>
    {
      this.targetSleepCell.Set(Grid.InvalidCell, smi);
      smi.GetComponent<Staterpillar>().DestroyOrphanedConnectorBuilding();
    })).EventTransition(GameHashes.NewBlock, (Func<ConduitSleepMonitor.Instance, KMonoBehaviour>) (smi => (KMonoBehaviour) GameClock.Instance), this.searching.looking, new StateMachine<ConduitSleepMonitor, ConduitSleepMonitor.Instance, IStateMachineTarget, ConduitSleepMonitor.Def>.Transition.ConditionCallback(ConduitSleepMonitor.IsSleepyTime));
    this.searching.Enter(new StateMachine<ConduitSleepMonitor, ConduitSleepMonitor.Instance, IStateMachineTarget, ConduitSleepMonitor.Def>.State.Callback(this.TryRecoverSave)).EventTransition(GameHashes.NewBlock, (Func<ConduitSleepMonitor.Instance, KMonoBehaviour>) (smi => (KMonoBehaviour) GameClock.Instance), this.idle, GameStateMachine<ConduitSleepMonitor, ConduitSleepMonitor.Instance, IStateMachineTarget, ConduitSleepMonitor.Def>.Not(new StateMachine<ConduitSleepMonitor, ConduitSleepMonitor.Instance, IStateMachineTarget, ConduitSleepMonitor.Def>.Transition.ConditionCallback(ConduitSleepMonitor.IsSleepyTime))).Exit((StateMachine<ConduitSleepMonitor, ConduitSleepMonitor.Instance, IStateMachineTarget, ConduitSleepMonitor.Def>.State.Callback) (smi =>
    {
      this.targetSleepCell.Set(Grid.InvalidCell, smi);
      smi.GetComponent<Staterpillar>().DestroyOrphanedConnectorBuilding();
    }));
    this.searching.looking.Update((System.Action<ConduitSleepMonitor.Instance, float>) ((smi, dt) => this.FindSleepLocation(smi)), (UpdateRate) 6).ToggleStatusItem(Db.Get().CreatureStatusItems.NoSleepSpot).ParamTransition<int>((StateMachine<ConduitSleepMonitor, ConduitSleepMonitor.Instance, IStateMachineTarget, ConduitSleepMonitor.Def>.Parameter<int>) this.targetSleepCell, this.searching.found, (StateMachine<ConduitSleepMonitor, ConduitSleepMonitor.Instance, IStateMachineTarget, ConduitSleepMonitor.Def>.Parameter<int>.Callback) ((smi, sleepCell) => sleepCell != Grid.InvalidCell));
    this.searching.found.Enter((StateMachine<ConduitSleepMonitor, ConduitSleepMonitor.Instance, IStateMachineTarget, ConduitSleepMonitor.Def>.State.Callback) (smi => smi.GetComponent<Staterpillar>().SpawnConnectorBuilding(this.targetSleepCell.Get(smi)))).ParamTransition<int>((StateMachine<ConduitSleepMonitor, ConduitSleepMonitor.Instance, IStateMachineTarget, ConduitSleepMonitor.Def>.Parameter<int>) this.targetSleepCell, this.searching.looking, (StateMachine<ConduitSleepMonitor, ConduitSleepMonitor.Instance, IStateMachineTarget, ConduitSleepMonitor.Def>.Parameter<int>.Callback) ((smi, sleepCell) => sleepCell == Grid.InvalidCell)).ToggleBehaviour(GameTags.Creatures.WantsConduitConnection, (StateMachine<ConduitSleepMonitor, ConduitSleepMonitor.Instance, IStateMachineTarget, ConduitSleepMonitor.Def>.Transition.ConditionCallback) (smi => this.targetSleepCell.Get(smi) != Grid.InvalidCell && ConduitSleepMonitor.IsSleepyTime(smi)));
  }

  public static bool IsSleepyTime(ConduitSleepMonitor.Instance smi) => (double) GameClock.Instance.GetTimeSinceStartOfCycle() >= 500.0;

  private void TryRecoverSave(ConduitSleepMonitor.Instance smi)
  {
    Staterpillar component = smi.GetComponent<Staterpillar>();
    if (this.targetSleepCell.Get(smi) != Grid.InvalidCell || !component.IsConnectorBuildingSpawned())
      return;
    this.targetSleepCell.Set(Grid.PosToCell((KMonoBehaviour) component.GetConnectorBuilding()), smi);
  }

  private void FindSleepLocation(ConduitSleepMonitor.Instance smi)
  {
    StaterpillarCellQuery query = PathFinderQueries.staterpillarCellQuery.Reset(10, smi.gameObject, smi.def.conduitLayer);
    smi.GetComponent<Navigator>().RunQuery((PathFinderQuery) query);
    if (query.result_cells.Count <= 0)
      return;
    foreach (int resultCell in query.result_cells)
    {
      int cellInDirection = Grid.GetCellInDirection(resultCell, Direction.Down);
      if (Object.op_Inequality((Object) Grid.Objects[cellInDirection, (int) smi.def.conduitLayer], (Object) null))
      {
        this.targetSleepCell.Set(resultCell, smi);
        break;
      }
    }
    if (this.targetSleepCell.Get(smi) != Grid.InvalidCell)
      return;
    this.targetSleepCell.Set(query.result_cells[Random.Range(0, query.result_cells.Count)], smi);
  }

  public class Def : StateMachine.BaseDef
  {
    public ObjectLayer conduitLayer;
  }

  private class SleepSearchStates : 
    GameStateMachine<ConduitSleepMonitor, ConduitSleepMonitor.Instance, IStateMachineTarget, ConduitSleepMonitor.Def>.State
  {
    public GameStateMachine<ConduitSleepMonitor, ConduitSleepMonitor.Instance, IStateMachineTarget, ConduitSleepMonitor.Def>.State looking;
    public GameStateMachine<ConduitSleepMonitor, ConduitSleepMonitor.Instance, IStateMachineTarget, ConduitSleepMonitor.Def>.State found;
  }

  public new class Instance : 
    GameStateMachine<ConduitSleepMonitor, ConduitSleepMonitor.Instance, IStateMachineTarget, ConduitSleepMonitor.Def>.GameInstance
  {
    public Instance(IStateMachineTarget master, ConduitSleepMonitor.Def def)
      : base(master, def)
    {
    }
  }
}
