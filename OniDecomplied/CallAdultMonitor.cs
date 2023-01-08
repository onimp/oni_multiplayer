// Decompiled with JetBrains decompiler
// Type: CallAdultMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class CallAdultMonitor : 
  GameStateMachine<CallAdultMonitor, CallAdultMonitor.Instance, IStateMachineTarget, CallAdultMonitor.Def>
{
  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.root;
    this.root.ToggleBehaviour(GameTags.Creatures.Behaviours.CallAdultBehaviour, new StateMachine<CallAdultMonitor, CallAdultMonitor.Instance, IStateMachineTarget, CallAdultMonitor.Def>.Transition.ConditionCallback(CallAdultMonitor.ShouldCallAdult), (System.Action<CallAdultMonitor.Instance>) (smi => smi.RefreshCallTime()));
  }

  public static bool ShouldCallAdult(CallAdultMonitor.Instance smi) => (double) Time.time >= (double) smi.nextCallTime;

  public class Def : StateMachine.BaseDef
  {
    public float callMinInterval = 120f;
    public float callMaxInterval = 240f;
  }

  public new class Instance : 
    GameStateMachine<CallAdultMonitor, CallAdultMonitor.Instance, IStateMachineTarget, CallAdultMonitor.Def>.GameInstance
  {
    public float nextCallTime;

    public Instance(IStateMachineTarget master, CallAdultMonitor.Def def)
      : base(master, def)
    {
      this.RefreshCallTime();
    }

    public void RefreshCallTime() => this.nextCallTime = Time.time + Random.value * (this.def.callMaxInterval - this.def.callMinInterval) + this.def.callMinInterval;
  }
}
