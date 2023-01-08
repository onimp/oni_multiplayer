// Decompiled with JetBrains decompiler
// Type: PrefersColder
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;

[SkipSaveFileSerialization]
public class PrefersColder : StateMachineComponent<PrefersColder.StatesInstance>
{
  protected virtual void OnSpawn() => this.smi.StartSM();

  public class StatesInstance : 
    GameStateMachine<PrefersColder.States, PrefersColder.StatesInstance, PrefersColder, object>.GameInstance
  {
    public StatesInstance(PrefersColder master)
      : base(master)
    {
    }
  }

  public class States : 
    GameStateMachine<PrefersColder.States, PrefersColder.StatesInstance, PrefersColder>
  {
    private AttributeModifier modifier = new AttributeModifier("ThermalConductivityBarrier", 0.005f, (string) DUPLICANTS.TRAITS.NEEDS.PREFERSCOOLER.NAME);

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.root;
      this.root.ToggleAttributeModifier((string) DUPLICANTS.TRAITS.NEEDS.PREFERSCOOLER.NAME, (Func<PrefersColder.StatesInstance, AttributeModifier>) (smi => this.modifier));
    }
  }
}
