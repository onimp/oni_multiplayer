// Decompiled with JetBrains decompiler
// Type: EntityElementExchanger
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class EntityElementExchanger : StateMachineComponent<EntityElementExchanger.StatesInstance>
{
  public Vector3 outputOffset = Vector3.zero;
  public bool reportExchange;
  [MyCmpReq]
  private KSelectable selectable;
  public SimHashes consumedElement;
  public SimHashes emittedElement;
  public float consumeRate;
  public float exchangeRatio;

  protected virtual void OnPrefabInit() => base.OnPrefabInit();

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
  }

  public void SetConsumptionRate(float consumptionRate) => this.consumeRate = consumptionRate;

  private static void OnSimConsumeCallback(Sim.MassConsumedCallback mass_cb_info, object data)
  {
    EntityElementExchanger elementExchanger = (EntityElementExchanger) data;
    if (!Object.op_Inequality((Object) elementExchanger, (Object) null))
      return;
    elementExchanger.OnSimConsume(mass_cb_info);
  }

  private void OnSimConsume(Sim.MassConsumedCallback mass_cb_info)
  {
    float mass = mass_cb_info.mass * this.smi.master.exchangeRatio;
    if (this.reportExchange && this.smi.master.emittedElement == SimHashes.Oxygen)
    {
      string note = ((Component) this).gameObject.GetProperName();
      ReceptacleMonitor component = ((Component) this).GetComponent<ReceptacleMonitor>();
      if (Object.op_Inequality((Object) component, (Object) null) && Object.op_Inequality((Object) component.GetReceptacle(), (Object) null))
        note = note + " (" + ((Component) component.GetReceptacle()).gameObject.GetProperName() + ")";
      ReportManager.Instance.ReportValue(ReportManager.ReportType.OxygenCreated, mass, note);
    }
    SimMessages.EmitMass(Grid.PosToCell(Vector3.op_Addition(TransformExtensions.GetPosition(this.smi.master.transform), this.outputOffset)), ElementLoader.FindElementByHash(this.smi.master.emittedElement).idx, mass, ElementLoader.FindElementByHash(this.smi.master.emittedElement).defaultValues.temperature, byte.MaxValue, 0);
  }

  public class StatesInstance : 
    GameStateMachine<EntityElementExchanger.States, EntityElementExchanger.StatesInstance, EntityElementExchanger, object>.GameInstance
  {
    public StatesInstance(EntityElementExchanger master)
      : base(master)
    {
    }
  }

  public class States : 
    GameStateMachine<EntityElementExchanger.States, EntityElementExchanger.StatesInstance, EntityElementExchanger>
  {
    public GameStateMachine<EntityElementExchanger.States, EntityElementExchanger.StatesInstance, EntityElementExchanger, object>.State exchanging;
    public GameStateMachine<EntityElementExchanger.States, EntityElementExchanger.StatesInstance, EntityElementExchanger, object>.State paused;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.exchanging;
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      this.exchanging.Enter((StateMachine<EntityElementExchanger.States, EntityElementExchanger.StatesInstance, EntityElementExchanger, object>.State.Callback) (smi =>
      {
        WiltCondition component = ((Component) smi.master).gameObject.GetComponent<WiltCondition>();
        if (!Object.op_Inequality((Object) component, (Object) null) || !component.IsWilting())
          return;
        smi.GoTo((StateMachine.BaseState) smi.sm.paused);
      })).EventTransition(GameHashes.Wilt, this.paused).ToggleStatusItem(Db.Get().CreatureStatusItems.ExchangingElementConsume).ToggleStatusItem(Db.Get().CreatureStatusItems.ExchangingElementOutput).Update(nameof (EntityElementExchanger), (Action<EntityElementExchanger.StatesInstance, float>) ((smi, dt) =>
      {
        HandleVector<Game.ComplexCallbackInfo<Sim.MassConsumedCallback>>.Handle handle = Game.Instance.massConsumedCallbackManager.Add(new Action<Sim.MassConsumedCallback, object>(EntityElementExchanger.OnSimConsumeCallback), (object) smi.master, nameof (EntityElementExchanger));
        SimMessages.ConsumeMass(Grid.PosToCell(((Component) smi.master).gameObject), smi.master.consumedElement, smi.master.consumeRate * dt, (byte) 3, handle.index);
      }), (UpdateRate) 6);
      this.paused.EventTransition(GameHashes.WiltRecover, this.exchanging);
    }
  }
}
