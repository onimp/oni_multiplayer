// Decompiled with JetBrains decompiler
// Type: CleaningMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

public class CleaningMonitor : 
  GameStateMachine<CleaningMonitor, CleaningMonitor.Instance, IStateMachineTarget, CleaningMonitor.Def>
{
  public GameStateMachine<CleaningMonitor, CleaningMonitor.Instance, IStateMachineTarget, CleaningMonitor.Def>.State cooldown;
  public GameStateMachine<CleaningMonitor, CleaningMonitor.Instance, IStateMachineTarget, CleaningMonitor.Def>.State clean;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.clean;
    this.clean.ToggleBehaviour(GameTags.Creatures.Cleaning, (StateMachine<CleaningMonitor, CleaningMonitor.Instance, IStateMachineTarget, CleaningMonitor.Def>.Transition.ConditionCallback) (smi => smi.CanCleanElementState()), (System.Action<CleaningMonitor.Instance>) (smi => smi.GoTo((StateMachine.BaseState) this.cooldown)));
    this.cooldown.ScheduleGoTo((Func<CleaningMonitor.Instance, float>) (smi => smi.def.coolDown), (StateMachine.BaseState) this.clean);
  }

  public class Def : StateMachine.BaseDef
  {
    public Element.State elementState = Element.State.Liquid;
    public CellOffset[] cellOffsets;
    public float coolDown = 30f;
  }

  public new class Instance : 
    GameStateMachine<CleaningMonitor, CleaningMonitor.Instance, IStateMachineTarget, CleaningMonitor.Def>.GameInstance
  {
    public Instance(IStateMachineTarget master, CleaningMonitor.Def def)
      : base(master, def)
    {
    }

    public bool CanCleanElementState()
    {
      int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.smi.transform));
      if (!Grid.IsValidCell(cell) || !Grid.IsLiquid(cell) && this.smi.def.elementState == Element.State.Liquid)
        return false;
      if (Grid.DiseaseCount[cell] > 0)
        return true;
      if (this.smi.def.cellOffsets != null)
      {
        foreach (CellOffset cellOffset in this.smi.def.cellOffsets)
        {
          int num = Grid.OffsetCell(cell, cellOffset);
          if (Grid.IsValidCell(num) && Grid.DiseaseCount[num] > 0)
            return true;
        }
      }
      return false;
    }
  }
}
