// Decompiled with JetBrains decompiler
// Type: SeedPlantingMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class SeedPlantingMonitor : 
  GameStateMachine<SeedPlantingMonitor, SeedPlantingMonitor.Instance, IStateMachineTarget, SeedPlantingMonitor.Def>
{
  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.root;
    this.root.ToggleBehaviour(GameTags.Creatures.WantsToPlantSeed, new StateMachine<SeedPlantingMonitor, SeedPlantingMonitor.Instance, IStateMachineTarget, SeedPlantingMonitor.Def>.Transition.ConditionCallback(SeedPlantingMonitor.ShouldSearchForSeeds), (System.Action<SeedPlantingMonitor.Instance>) (smi => smi.RefreshSearchTime()));
  }

  public static bool ShouldSearchForSeeds(SeedPlantingMonitor.Instance smi) => (double) Time.time >= (double) smi.nextSearchTime;

  public class Def : StateMachine.BaseDef
  {
    public float searchMinInterval = 60f;
    public float searchMaxInterval = 300f;
  }

  public new class Instance : 
    GameStateMachine<SeedPlantingMonitor, SeedPlantingMonitor.Instance, IStateMachineTarget, SeedPlantingMonitor.Def>.GameInstance
  {
    public float nextSearchTime;

    public Instance(IStateMachineTarget master, SeedPlantingMonitor.Def def)
      : base(master, def)
    {
      this.RefreshSearchTime();
    }

    public void RefreshSearchTime() => this.nextSearchTime = Time.time + Mathf.Lerp(this.def.searchMinInterval, this.def.searchMaxInterval, Random.value);
  }
}
