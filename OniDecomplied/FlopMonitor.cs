// Decompiled with JetBrains decompiler
// Type: FlopMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class FlopMonitor : 
  GameStateMachine<FlopMonitor, FlopMonitor.Instance, IStateMachineTarget, FlopMonitor.Def>
{
  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.root;
    this.root.ToggleBehaviour(GameTags.Creatures.Flopping, (StateMachine<FlopMonitor, FlopMonitor.Instance, IStateMachineTarget, FlopMonitor.Def>.Transition.ConditionCallback) (smi => smi.ShouldBeginFlopping()));
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<FlopMonitor, FlopMonitor.Instance, IStateMachineTarget, FlopMonitor.Def>.GameInstance
  {
    public Instance(IStateMachineTarget master, FlopMonitor.Def def)
      : base(master, def)
    {
    }

    public bool ShouldBeginFlopping()
    {
      Vector3 position = TransformExtensions.GetPosition(this.transform);
      position.y += CreatureFallMonitor.FLOOR_DISTANCE;
      int cell1 = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
      int cell2 = Grid.PosToCell(position);
      return Grid.IsValidCell(cell2) && Grid.Solid[cell2] && !Grid.IsSubstantialLiquid(cell1) && !Grid.IsLiquid(Grid.CellAbove(cell1));
    }
  }
}
