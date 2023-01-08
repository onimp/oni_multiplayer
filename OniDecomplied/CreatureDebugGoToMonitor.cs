// Decompiled with JetBrains decompiler
// Type: CreatureDebugGoToMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class CreatureDebugGoToMonitor : 
  GameStateMachine<CreatureDebugGoToMonitor, CreatureDebugGoToMonitor.Instance, IStateMachineTarget, CreatureDebugGoToMonitor.Def>
{
  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.root;
    this.root.ToggleBehaviour(GameTags.HasDebugDestination, new StateMachine<CreatureDebugGoToMonitor, CreatureDebugGoToMonitor.Instance, IStateMachineTarget, CreatureDebugGoToMonitor.Def>.Transition.ConditionCallback(CreatureDebugGoToMonitor.HasTargetCell), new System.Action<CreatureDebugGoToMonitor.Instance>(CreatureDebugGoToMonitor.ClearTargetCell));
  }

  private static bool HasTargetCell(CreatureDebugGoToMonitor.Instance smi) => smi.targetCell != Grid.InvalidCell;

  private static void ClearTargetCell(CreatureDebugGoToMonitor.Instance smi) => smi.targetCell = Grid.InvalidCell;

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<CreatureDebugGoToMonitor, CreatureDebugGoToMonitor.Instance, IStateMachineTarget, CreatureDebugGoToMonitor.Def>.GameInstance
  {
    public int targetCell = Grid.InvalidCell;

    public Instance(IStateMachineTarget target, CreatureDebugGoToMonitor.Def def)
      : base(target, def)
    {
    }

    public void GoToCursor() => this.targetCell = DebugHandler.GetMouseCell();
  }
}
