// Decompiled with JetBrains decompiler
// Type: SteppedInMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using UnityEngine;

public class SteppedInMonitor : GameStateMachine<SteppedInMonitor, SteppedInMonitor.Instance>
{
  public GameStateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.State satisfied;
  public GameStateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.State carpetedFloor;
  public GameStateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.State wetFloor;
  public GameStateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.State wetBody;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.satisfied;
    this.satisfied.Transition(this.carpetedFloor, new StateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(SteppedInMonitor.IsOnCarpet)).Transition(this.wetFloor, new StateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(SteppedInMonitor.IsFloorWet)).Transition(this.wetBody, new StateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(SteppedInMonitor.IsSubmerged));
    this.carpetedFloor.Enter(new StateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.State.Callback(SteppedInMonitor.GetCarpetFeet)).ToggleExpression(Db.Get().Expressions.Tickled).Update(new System.Action<SteppedInMonitor.Instance, float>(SteppedInMonitor.GetCarpetFeet), (UpdateRate) 6).Transition(this.satisfied, GameStateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.Not(new StateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(SteppedInMonitor.IsOnCarpet))).Transition(this.wetFloor, new StateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(SteppedInMonitor.IsFloorWet)).Transition(this.wetBody, new StateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(SteppedInMonitor.IsSubmerged));
    this.wetFloor.Enter(new StateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.State.Callback(SteppedInMonitor.GetWetFeet)).Update(new System.Action<SteppedInMonitor.Instance, float>(SteppedInMonitor.GetWetFeet), (UpdateRate) 6).Transition(this.satisfied, GameStateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.Not(new StateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(SteppedInMonitor.IsFloorWet))).Transition(this.wetBody, new StateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(SteppedInMonitor.IsSubmerged));
    this.wetBody.Enter(new StateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.State.Callback(SteppedInMonitor.GetSoaked)).Update(new System.Action<SteppedInMonitor.Instance, float>(SteppedInMonitor.GetSoaked), (UpdateRate) 6).Transition(this.wetFloor, GameStateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.Not(new StateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback(SteppedInMonitor.IsSubmerged)));
  }

  private static void GetCarpetFeet(SteppedInMonitor.Instance smi, float dt) => SteppedInMonitor.GetCarpetFeet(smi);

  private static void GetCarpetFeet(SteppedInMonitor.Instance smi)
  {
    if (smi.effects.HasEffect("SoakingWet") || smi.effects.HasEffect("WetFeet"))
      return;
    smi.effects.Add("CarpetFeet", true);
  }

  private static void GetWetFeet(SteppedInMonitor.Instance smi, float dt) => SteppedInMonitor.GetWetFeet(smi);

  private static void GetWetFeet(SteppedInMonitor.Instance smi)
  {
    if (!smi.effects.HasEffect("CarpetFeet"))
      smi.effects.Remove("CarpetFeet");
    if (smi.effects.HasEffect("SoakingWet"))
      return;
    smi.effects.Add("WetFeet", true);
  }

  private static void GetSoaked(SteppedInMonitor.Instance smi, float dt) => SteppedInMonitor.GetSoaked(smi);

  private static void GetSoaked(SteppedInMonitor.Instance smi)
  {
    if (!smi.effects.HasEffect("CarpetFeet"))
      smi.effects.Remove("CarpetFeet");
    if (smi.effects.HasEffect("WetFeet"))
      smi.effects.Remove("WetFeet");
    smi.effects.Add("SoakingWet", true);
  }

  private static bool IsOnCarpet(SteppedInMonitor.Instance smi)
  {
    int cell = Grid.CellBelow(Grid.PosToCell((StateMachine.Instance) smi));
    if (!Grid.IsValidCell(cell))
      return false;
    GameObject go = Grid.Objects[cell, 9];
    return Grid.IsValidCell(cell) && Object.op_Inequality((Object) go, (Object) null) && go.HasTag(GameTags.Carpeted);
  }

  private static bool IsFloorWet(SteppedInMonitor.Instance smi)
  {
    int cell = Grid.PosToCell((StateMachine.Instance) smi);
    return Grid.IsValidCell(cell) && Grid.Element[cell].IsLiquid;
  }

  private static bool IsSubmerged(SteppedInMonitor.Instance smi)
  {
    int cell = Grid.CellAbove(Grid.PosToCell((StateMachine.Instance) smi));
    return Grid.IsValidCell(cell) && Grid.Element[cell].IsLiquid;
  }

  public new class Instance : 
    GameStateMachine<SteppedInMonitor, SteppedInMonitor.Instance, IStateMachineTarget, object>.GameInstance
  {
    public Effects effects;

    public Instance(IStateMachineTarget master)
      : base(master)
    {
      this.effects = this.GetComponent<Effects>();
    }
  }
}
