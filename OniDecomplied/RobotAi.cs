// Decompiled with JetBrains decompiler
// Type: RobotAi
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class RobotAi : GameStateMachine<RobotAi, RobotAi.Instance>
{
  public RobotAi.AliveStates alive;
  public GameStateMachine<RobotAi, RobotAi.Instance, IStateMachineTarget, object>.State dead;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.root;
    this.root.ToggleStateMachine((Func<RobotAi.Instance, StateMachine.Instance>) (smi => (StateMachine.Instance) new DeathMonitor.Instance(smi.master, new DeathMonitor.Def()))).Enter((StateMachine<RobotAi, RobotAi.Instance, IStateMachineTarget, object>.State.Callback) (smi =>
    {
      if (smi.HasTag(GameTags.Dead))
        smi.GoTo((StateMachine.BaseState) this.dead);
      else
        smi.GoTo((StateMachine.BaseState) this.alive);
    }));
    this.alive.DefaultState(this.alive.normal).TagTransition(GameTags.Dead, this.dead);
    this.alive.normal.TagTransition(GameTags.Stored, this.alive.stored).ToggleStateMachine((Func<RobotAi.Instance, StateMachine.Instance>) (smi => (StateMachine.Instance) new FallMonitor.Instance(smi.master, false)));
    this.alive.stored.PlayAnim("in_storage").TagTransition(GameTags.Stored, this.alive.normal, true).ToggleBrain("stored").Enter((StateMachine<RobotAi, RobotAi.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.GetComponent<Navigator>().Pause("stored"))).Exit((StateMachine<RobotAi, RobotAi.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.GetComponent<Navigator>().Unpause("unstored")));
    this.dead.ToggleBrain("dead").ToggleComponent<Deconstructable>().ToggleStateMachine((Func<RobotAi.Instance, StateMachine.Instance>) (smi => (StateMachine.Instance) new FallWhenDeadMonitor.Instance(smi.master))).Enter("RefreshUserMenu", (StateMachine<RobotAi, RobotAi.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.RefreshUserMenu())).Enter("DropStorage", (StateMachine<RobotAi, RobotAi.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.GetComponent<Storage>().DropAll(false, false, new Vector3(), true, (List<GameObject>) null)));
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public class AliveStates : 
    GameStateMachine<RobotAi, RobotAi.Instance, IStateMachineTarget, object>.State
  {
    public GameStateMachine<RobotAi, RobotAi.Instance, IStateMachineTarget, object>.State normal;
    public GameStateMachine<RobotAi, RobotAi.Instance, IStateMachineTarget, object>.State stored;
  }

  public new class Instance : 
    GameStateMachine<RobotAi, RobotAi.Instance, IStateMachineTarget, object>.GameInstance
  {
    public Instance(IStateMachineTarget master, RobotAi.Def def)
      : base(master, (object) def)
    {
      ChoreConsumer component = this.GetComponent<ChoreConsumer>();
      component.AddUrge(Db.Get().Urges.EmoteHighPriority);
      component.AddUrge(Db.Get().Urges.EmoteIdle);
    }

    public void RefreshUserMenu() => Game.Instance.userMenu.Refresh(this.master.gameObject);
  }
}
