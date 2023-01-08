// Decompiled with JetBrains decompiler
// Type: BeeSleepMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class BeeSleepMonitor : 
  GameStateMachine<BeeSleepMonitor, BeeSleepMonitor.Instance, IStateMachineTarget, BeeSleepMonitor.Def>
{
  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.root;
    this.root.Update(new System.Action<BeeSleepMonitor.Instance, float>(this.UpdateCO2Exposure), (UpdateRate) 6).ToggleBehaviour(GameTags.Creatures.BeeWantsToSleep, new StateMachine<BeeSleepMonitor, BeeSleepMonitor.Instance, IStateMachineTarget, BeeSleepMonitor.Def>.Transition.ConditionCallback(this.ShouldSleep));
  }

  public bool ShouldSleep(BeeSleepMonitor.Instance smi) => (double) smi.CO2Exposure >= 5.0;

  public void UpdateCO2Exposure(BeeSleepMonitor.Instance smi, float dt)
  {
    if (this.IsInCO2(smi))
      ++smi.CO2Exposure;
    else
      smi.CO2Exposure -= 0.5f;
    smi.CO2Exposure = Mathf.Clamp(smi.CO2Exposure, 0.0f, 10f);
  }

  public bool IsInCO2(BeeSleepMonitor.Instance smi)
  {
    int cell = Grid.PosToCell(smi.gameObject);
    return Grid.IsValidCell(cell) && Grid.Element[cell].id == SimHashes.CarbonDioxide;
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<BeeSleepMonitor, BeeSleepMonitor.Instance, IStateMachineTarget, BeeSleepMonitor.Def>.GameInstance
  {
    public float CO2Exposure;

    public Instance(IStateMachineTarget master, BeeSleepMonitor.Def def)
      : base(master, def)
    {
    }
  }
}
