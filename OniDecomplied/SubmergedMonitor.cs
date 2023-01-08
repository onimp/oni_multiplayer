// Decompiled with JetBrains decompiler
// Type: SubmergedMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class SubmergedMonitor : 
  GameStateMachine<SubmergedMonitor, SubmergedMonitor.Instance, IStateMachineTarget, SubmergedMonitor.Def>
{
  public GameStateMachine<SubmergedMonitor, SubmergedMonitor.Instance, IStateMachineTarget, SubmergedMonitor.Def>.State satisfied;
  public GameStateMachine<SubmergedMonitor, SubmergedMonitor.Instance, IStateMachineTarget, SubmergedMonitor.Def>.State submerged;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.satisfied;
    this.satisfied.Enter("SetNavType", (StateMachine<SubmergedMonitor, SubmergedMonitor.Instance, IStateMachineTarget, SubmergedMonitor.Def>.State.Callback) (smi => smi.GetComponent<Navigator>().SetCurrentNavType(NavType.Hover))).Update("SetNavType", (System.Action<SubmergedMonitor.Instance, float>) ((smi, dt) => smi.GetComponent<Navigator>().SetCurrentNavType(NavType.Hover)), (UpdateRate) 6).Transition(this.submerged, (StateMachine<SubmergedMonitor, SubmergedMonitor.Instance, IStateMachineTarget, SubmergedMonitor.Def>.Transition.ConditionCallback) (smi => smi.IsSubmerged()), (UpdateRate) 6);
    this.submerged.Enter("SetNavType", (StateMachine<SubmergedMonitor, SubmergedMonitor.Instance, IStateMachineTarget, SubmergedMonitor.Def>.State.Callback) (smi => smi.GetComponent<Navigator>().SetCurrentNavType(NavType.Swim))).Update("SetNavType", (System.Action<SubmergedMonitor.Instance, float>) ((smi, dt) => smi.GetComponent<Navigator>().SetCurrentNavType(NavType.Swim)), (UpdateRate) 6).Transition(this.satisfied, (StateMachine<SubmergedMonitor, SubmergedMonitor.Instance, IStateMachineTarget, SubmergedMonitor.Def>.Transition.ConditionCallback) (smi => !smi.IsSubmerged()), (UpdateRate) 6).ToggleTag(GameTags.Creatures.Submerged);
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<SubmergedMonitor, SubmergedMonitor.Instance, IStateMachineTarget, SubmergedMonitor.Def>.GameInstance
  {
    public Instance(IStateMachineTarget master, SubmergedMonitor.Def def)
      : base(master, def)
    {
    }

    public bool IsSubmerged() => Grid.IsSubstantialLiquid(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)));
  }
}
