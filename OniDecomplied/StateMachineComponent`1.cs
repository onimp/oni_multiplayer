// Decompiled with JetBrains decompiler
// Type: StateMachineComponent`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;

[SerializationConfig]
public class StateMachineComponent<StateMachineInstanceType> : StateMachineComponent, ISaveLoadable where StateMachineInstanceType : StateMachine.Instance
{
  private StateMachineInstanceType _smi;

  public StateMachineInstanceType smi
  {
    get
    {
      if ((object) this._smi == null)
        this._smi = (StateMachineInstanceType) Activator.CreateInstance(typeof (StateMachineInstanceType), (object) this);
      return this._smi;
    }
  }

  public override StateMachine.Instance GetSMI() => (StateMachine.Instance) this._smi;

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    if ((object) this._smi == null)
      return;
    this._smi.StopSM("StateMachineComponent.OnCleanUp");
    this._smi = default (StateMachineInstanceType);
  }

  protected virtual void OnCmpEnable()
  {
    base.OnCmpEnable();
    if (!this.isSpawned)
      return;
    this.smi.StartSM();
  }

  protected virtual void OnCmpDisable()
  {
    base.OnCmpDisable();
    if ((object) this._smi == null)
      return;
    this._smi.StopSM("StateMachineComponent.OnDisable");
  }
}
