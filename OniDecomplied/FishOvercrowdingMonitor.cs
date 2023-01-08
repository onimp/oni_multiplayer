// Decompiled with JetBrains decompiler
// Type: FishOvercrowdingMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class FishOvercrowdingMonitor : 
  GameStateMachine<FishOvercrowdingMonitor, FishOvercrowdingMonitor.Instance, IStateMachineTarget, FishOvercrowdingMonitor.Def>
{
  public GameStateMachine<FishOvercrowdingMonitor, FishOvercrowdingMonitor.Instance, IStateMachineTarget, FishOvercrowdingMonitor.Def>.State satisfied;
  public GameStateMachine<FishOvercrowdingMonitor, FishOvercrowdingMonitor.Instance, IStateMachineTarget, FishOvercrowdingMonitor.Def>.State overcrowded;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.satisfied;
    this.root.Enter(new StateMachine<FishOvercrowdingMonitor, FishOvercrowdingMonitor.Instance, IStateMachineTarget, FishOvercrowdingMonitor.Def>.State.Callback(FishOvercrowdingMonitor.Register)).Exit(new StateMachine<FishOvercrowdingMonitor, FishOvercrowdingMonitor.Instance, IStateMachineTarget, FishOvercrowdingMonitor.Def>.State.Callback(FishOvercrowdingMonitor.Unregister));
    this.satisfied.DoNothing();
    this.overcrowded.DoNothing();
  }

  private static void Register(FishOvercrowdingMonitor.Instance smi) => FishOvercrowingManager.Instance.Add(smi);

  private static void Unregister(FishOvercrowdingMonitor.Instance smi)
  {
    FishOvercrowingManager instance = FishOvercrowingManager.Instance;
    if (Object.op_Equality((Object) instance, (Object) null))
      return;
    instance.Remove(smi);
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<FishOvercrowdingMonitor, FishOvercrowdingMonitor.Instance, IStateMachineTarget, FishOvercrowdingMonitor.Def>.GameInstance
  {
    public int cellCount;
    public int fishCount;

    public Instance(IStateMachineTarget master, FishOvercrowdingMonitor.Def def)
      : base(master, def)
    {
    }

    public void SetOvercrowdingInfo(int cell_count, int fish_count)
    {
      this.cellCount = cell_count;
      this.fishCount = fish_count;
    }
  }
}
