// Decompiled with JetBrains decompiler
// Type: MoltDropperMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using UnityEngine;

public class MoltDropperMonitor : 
  GameStateMachine<MoltDropperMonitor, MoltDropperMonitor.Instance, IStateMachineTarget, MoltDropperMonitor.Def>
{
  public StateMachine<MoltDropperMonitor, MoltDropperMonitor.Instance, IStateMachineTarget, MoltDropperMonitor.Def>.BoolParameter droppedThisCycle = new StateMachine<MoltDropperMonitor, MoltDropperMonitor.Instance, IStateMachineTarget, MoltDropperMonitor.Def>.BoolParameter(false);
  public GameStateMachine<MoltDropperMonitor, MoltDropperMonitor.Instance, IStateMachineTarget, MoltDropperMonitor.Def>.State satisfied;
  public GameStateMachine<MoltDropperMonitor, MoltDropperMonitor.Instance, IStateMachineTarget, MoltDropperMonitor.Def>.State drop;
  public StateMachine<MoltDropperMonitor, MoltDropperMonitor.Instance, IStateMachineTarget, MoltDropperMonitor.Def>.Signal cellChangedSignal;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.satisfied;
    this.root.EventHandler(GameHashes.NewDay, (Func<MoltDropperMonitor.Instance, KMonoBehaviour>) (smi => (KMonoBehaviour) GameClock.Instance), (StateMachine<MoltDropperMonitor, MoltDropperMonitor.Instance, IStateMachineTarget, MoltDropperMonitor.Def>.State.Callback) (smi => smi.spawnedThisCycle = false));
    this.satisfied.OnSignal(this.cellChangedSignal, this.drop, (Func<MoltDropperMonitor.Instance, bool>) (smi => smi.ShouldDropElement()));
    this.drop.Enter((StateMachine<MoltDropperMonitor, MoltDropperMonitor.Instance, IStateMachineTarget, MoltDropperMonitor.Def>.State.Callback) (smi => smi.Drop())).EventTransition(GameHashes.NewDay, (Func<MoltDropperMonitor.Instance, KMonoBehaviour>) (smi => (KMonoBehaviour) GameClock.Instance), this.satisfied);
  }

  public class Def : StateMachine.BaseDef
  {
    public string onGrowDropID;
    public float massToDrop;
    public SimHashes blockedElement;
  }

  public new class Instance : 
    GameStateMachine<MoltDropperMonitor, MoltDropperMonitor.Instance, IStateMachineTarget, MoltDropperMonitor.Def>.GameInstance
  {
    [Serialize]
    public bool spawnedThisCycle;
    [Serialize]
    public float timeOfLastDrop;

    public Instance(IStateMachineTarget master, MoltDropperMonitor.Def def)
      : base(master, def)
    {
      Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(this.transform, new System.Action(this.OnCellChange), "ElementDropperMonitor.Instance");
    }

    public override void StopSM(string reason)
    {
      base.StopSM(reason);
      Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(this.transform, new System.Action(this.OnCellChange));
    }

    private void OnCellChange() => this.sm.cellChangedSignal.Trigger(this);

    public bool ShouldDropElement() => this.IsValidTimeToDrop() && !this.smi.HasTag(GameTags.Creatures.Hungry) && !this.smi.HasTag(GameTags.Creatures.Unhappy) && this.IsValidDropCell();

    public void Drop()
    {
      GameObject gameObject = Scenario.SpawnPrefab(this.GetDropSpawnLocation(), 0, 0, this.def.onGrowDropID);
      gameObject.SetActive(true);
      gameObject.GetComponent<PrimaryElement>().Mass = this.def.massToDrop;
      this.spawnedThisCycle = true;
      this.timeOfLastDrop = GameClock.Instance.GetTime();
    }

    private int GetDropSpawnLocation()
    {
      int cell = Grid.PosToCell(this.gameObject);
      int num = Grid.CellAbove(cell);
      return Grid.IsValidCell(num) && !Grid.Solid[num] ? num : cell;
    }

    public bool IsValidTimeToDrop()
    {
      if (this.spawnedThisCycle)
        return false;
      return (double) this.timeOfLastDrop <= 0.0 || (double) GameClock.Instance.GetTime() - (double) this.timeOfLastDrop > 600.0;
    }

    public bool IsValidDropCell()
    {
      int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
      return Grid.IsValidCell(cell) && Grid.Element[cell].id != this.def.blockedElement;
    }
  }
}
