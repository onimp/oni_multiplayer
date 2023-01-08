// Decompiled with JetBrains decompiler
// Type: HiveEatingMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class HiveEatingMonitor : 
  GameStateMachine<HiveEatingMonitor, HiveEatingMonitor.Instance, IStateMachineTarget, HiveEatingMonitor.Def>
{
  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.root;
    this.root.ToggleBehaviour(GameTags.Creatures.WantsToEat, new StateMachine<HiveEatingMonitor, HiveEatingMonitor.Instance, IStateMachineTarget, HiveEatingMonitor.Def>.Transition.ConditionCallback(HiveEatingMonitor.ShouldEat));
  }

  public static bool ShouldEat(HiveEatingMonitor.Instance smi) => Object.op_Inequality((Object) smi.storage.FindFirst(smi.def.consumedOre), (Object) null);

  public class Def : StateMachine.BaseDef
  {
    public Tag consumedOre;
  }

  public new class Instance : 
    GameStateMachine<HiveEatingMonitor, HiveEatingMonitor.Instance, IStateMachineTarget, HiveEatingMonitor.Def>.GameInstance
  {
    [MyCmpReq]
    public Storage storage;

    public Instance(IStateMachineTarget master, HiveEatingMonitor.Def def)
      : base(master, def)
    {
    }
  }
}
