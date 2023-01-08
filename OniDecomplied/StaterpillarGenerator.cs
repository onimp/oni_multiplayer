// Decompiled with JetBrains decompiler
// Type: StaterpillarGenerator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using KSerialization;
using UnityEngine;

public class StaterpillarGenerator : Generator
{
  private StaterpillarGenerator.StatesInstance smi;
  [Serialize]
  public Ref<Staterpillar> parent = new Ref<Staterpillar>();

  protected override void OnSpawn()
  {
    Staterpillar staterpillar = this.parent.Get();
    if (Object.op_Equality((Object) staterpillar, (Object) null) || Object.op_Inequality((Object) staterpillar.GetGenerator(), (Object) this))
    {
      Util.KDestroyGameObject(((Component) this).gameObject);
    }
    else
    {
      this.smi = new StaterpillarGenerator.StatesInstance(this);
      this.smi.StartSM();
      base.OnSpawn();
    }
  }

  public override void EnergySim200ms(float dt)
  {
    base.EnergySim200ms(dt);
    ushort circuitId = this.CircuitID;
    this.operational.SetFlag(Generator.wireConnectedFlag, circuitId != ushort.MaxValue);
    if (!this.operational.IsOperational)
      return;
    float wattageRating = ((Component) this).GetComponent<Generator>().WattageRating;
    if ((double) wattageRating <= 0.0)
      return;
    this.GenerateJoules(Mathf.Max(wattageRating * dt, 1f * dt));
  }

  public class StatesInstance : 
    GameStateMachine<StaterpillarGenerator.States, StaterpillarGenerator.StatesInstance, StaterpillarGenerator, object>.GameInstance
  {
    private Attributes attributes;

    public StatesInstance(StaterpillarGenerator master)
      : base(master)
    {
    }
  }

  public class States : 
    GameStateMachine<StaterpillarGenerator.States, StaterpillarGenerator.StatesInstance, StaterpillarGenerator>
  {
    public GameStateMachine<StaterpillarGenerator.States, StaterpillarGenerator.StatesInstance, StaterpillarGenerator, object>.State idle;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.root;
      this.root.EventTransition(GameHashes.OperationalChanged, this.idle, (StateMachine<StaterpillarGenerator.States, StaterpillarGenerator.StatesInstance, StaterpillarGenerator, object>.Transition.ConditionCallback) (smi => smi.GetComponent<Operational>().IsOperational));
      this.idle.EventTransition(GameHashes.OperationalChanged, this.root, (StateMachine<StaterpillarGenerator.States, StaterpillarGenerator.StatesInstance, StaterpillarGenerator, object>.Transition.ConditionCallback) (smi => !smi.GetComponent<Operational>().IsOperational)).Enter((StateMachine<StaterpillarGenerator.States, StaterpillarGenerator.StatesInstance, StaterpillarGenerator, object>.State.Callback) (smi => smi.GetComponent<Operational>().SetActive(true)));
    }
  }
}
