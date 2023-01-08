// Decompiled with JetBrains decompiler
// Type: BeeHiveMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class BeeHiveMonitor : 
  GameStateMachine<BeeHiveMonitor, BeeHiveMonitor.Instance, IStateMachineTarget, BeeHiveMonitor.Def>
{
  public GameStateMachine<BeeHiveMonitor, BeeHiveMonitor.Instance, IStateMachineTarget, BeeHiveMonitor.Def>.State idle;
  public GameStateMachine<BeeHiveMonitor, BeeHiveMonitor.Instance, IStateMachineTarget, BeeHiveMonitor.Def>.State night;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.idle;
    this.idle.EventTransition(GameHashes.Nighttime, (Func<BeeHiveMonitor.Instance, KMonoBehaviour>) (smi => (KMonoBehaviour) GameClock.Instance), this.night, (StateMachine<BeeHiveMonitor, BeeHiveMonitor.Instance, IStateMachineTarget, BeeHiveMonitor.Def>.Transition.ConditionCallback) (smi => GameClock.Instance.IsNighttime()));
    this.night.EventTransition(GameHashes.NewDay, (Func<BeeHiveMonitor.Instance, KMonoBehaviour>) (smi => (KMonoBehaviour) GameClock.Instance), this.idle, (StateMachine<BeeHiveMonitor, BeeHiveMonitor.Instance, IStateMachineTarget, BeeHiveMonitor.Def>.Transition.ConditionCallback) (smi => !GameClock.Instance.IsNighttime())).ToggleBehaviour(GameTags.Creatures.WantsToMakeHome, new StateMachine<BeeHiveMonitor, BeeHiveMonitor.Instance, IStateMachineTarget, BeeHiveMonitor.Def>.Transition.ConditionCallback(this.ShouldMakeHome));
  }

  public bool ShouldMakeHome(BeeHiveMonitor.Instance smi) => !this.CanGoHome(smi);

  public bool CanGoHome(BeeHiveMonitor.Instance smi) => Object.op_Inequality((Object) smi.gameObject.GetComponent<Bee>().FindHiveInRoom(), (Object) null);

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<BeeHiveMonitor, BeeHiveMonitor.Instance, IStateMachineTarget, BeeHiveMonitor.Def>.GameInstance
  {
    public Instance(IStateMachineTarget master, BeeHiveMonitor.Def def)
      : base(master, def)
    {
    }
  }
}
