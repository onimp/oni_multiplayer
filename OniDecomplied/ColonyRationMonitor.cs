// Decompiled with JetBrains decompiler
// Type: ColonyRationMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class ColonyRationMonitor : 
  GameStateMachine<ColonyRationMonitor, ColonyRationMonitor.Instance>
{
  public GameStateMachine<ColonyRationMonitor, ColonyRationMonitor.Instance, IStateMachineTarget, object>.State satisfied;
  public GameStateMachine<ColonyRationMonitor, ColonyRationMonitor.Instance, IStateMachineTarget, object>.State outofrations;
  private StateMachine<ColonyRationMonitor, ColonyRationMonitor.Instance, IStateMachineTarget, object>.BoolParameter isOutOfRations = new StateMachine<ColonyRationMonitor, ColonyRationMonitor.Instance, IStateMachineTarget, object>.BoolParameter();

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.satisfied;
    this.root.Update("UpdateOutOfRations", (System.Action<ColonyRationMonitor.Instance, float>) ((smi, dt) => smi.UpdateIsOutOfRations()));
    this.satisfied.ParamTransition<bool>((StateMachine<ColonyRationMonitor, ColonyRationMonitor.Instance, IStateMachineTarget, object>.Parameter<bool>) this.isOutOfRations, this.outofrations, GameStateMachine<ColonyRationMonitor, ColonyRationMonitor.Instance, IStateMachineTarget, object>.IsTrue).TriggerOnEnter(GameHashes.ColonyHasRationsChanged);
    this.outofrations.ParamTransition<bool>((StateMachine<ColonyRationMonitor, ColonyRationMonitor.Instance, IStateMachineTarget, object>.Parameter<bool>) this.isOutOfRations, this.satisfied, GameStateMachine<ColonyRationMonitor, ColonyRationMonitor.Instance, IStateMachineTarget, object>.IsFalse).TriggerOnEnter(GameHashes.ColonyHasRationsChanged);
  }

  public new class Instance : 
    GameStateMachine<ColonyRationMonitor, ColonyRationMonitor.Instance, IStateMachineTarget, object>.GameInstance
  {
    public Instance(IStateMachineTarget master)
      : base(master)
    {
      this.UpdateIsOutOfRations();
    }

    public void UpdateIsOutOfRations()
    {
      bool flag = true;
      foreach (Component component in Components.Edibles.Items)
      {
        if ((double) component.GetComponent<Pickupable>().UnreservedAmount > 0.0)
        {
          flag = false;
          break;
        }
      }
      this.smi.sm.isOutOfRations.Set(flag, this.smi);
    }

    public bool IsOutOfRations() => this.smi.sm.isOutOfRations.Get(this.smi);
  }
}
