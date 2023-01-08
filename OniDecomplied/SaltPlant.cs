// Decompiled with JetBrains decompiler
// Type: SaltPlant
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class SaltPlant : StateMachineComponent<SaltPlant.StatesInstance>
{
  private static readonly EventSystem.IntraObjectHandler<SaltPlant> OnWiltDelegate = new EventSystem.IntraObjectHandler<SaltPlant>((Action<SaltPlant, object>) ((component, data) => component.OnWilt(data)));
  private static readonly EventSystem.IntraObjectHandler<SaltPlant> OnWiltRecoverDelegate = new EventSystem.IntraObjectHandler<SaltPlant>((Action<SaltPlant, object>) ((component, data) => component.OnWiltRecover(data)));

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.Subscribe<SaltPlant>(-724860998, SaltPlant.OnWiltDelegate);
    this.Subscribe<SaltPlant>(712767498, SaltPlant.OnWiltRecoverDelegate);
  }

  private void OnWilt(object data = null) => ((Component) this).gameObject.GetComponent<ElementConsumer>().EnableConsumption(false);

  private void OnWiltRecover(object data = null) => ((Component) this).gameObject.GetComponent<ElementConsumer>().EnableConsumption(true);

  public class StatesInstance : 
    GameStateMachine<SaltPlant.States, SaltPlant.StatesInstance, SaltPlant, object>.GameInstance
  {
    public StatesInstance(SaltPlant master)
      : base(master)
    {
    }
  }

  public class States : GameStateMachine<SaltPlant.States, SaltPlant.StatesInstance, SaltPlant>
  {
    public GameStateMachine<SaltPlant.States, SaltPlant.StatesInstance, SaltPlant, object>.State alive;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      default_state = (StateMachine.BaseState) this.alive;
      this.alive.DoNothing();
    }
  }
}
