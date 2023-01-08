// Decompiled with JetBrains decompiler
// Type: OxyliteRefinery
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using UnityEngine;

[SerializationConfig]
public class OxyliteRefinery : StateMachineComponent<OxyliteRefinery.StatesInstance>
{
  [MyCmpAdd]
  private Storage storage;
  [MyCmpReq]
  private Operational operational;
  public Tag emitTag;
  public float emitMass;
  public Vector3 dropOffset;

  protected virtual void OnSpawn() => this.smi.StartSM();

  public class StatesInstance : 
    GameStateMachine<OxyliteRefinery.States, OxyliteRefinery.StatesInstance, OxyliteRefinery, object>.GameInstance
  {
    public StatesInstance(OxyliteRefinery smi)
      : base(smi)
    {
    }

    public void TryEmit()
    {
      Storage storage = this.smi.master.storage;
      GameObject first = storage.FindFirst(this.smi.master.emitTag);
      if (!Object.op_Inequality((Object) first, (Object) null) || (double) first.GetComponent<PrimaryElement>().Mass < (double) this.master.emitMass)
        return;
      Vector3 vector3 = Vector3.op_Addition(TransformExtensions.GetPosition(this.transform), this.master.dropOffset);
      vector3.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
      TransformExtensions.SetPosition(first.transform, vector3);
      storage.Drop(first, true);
    }
  }

  public class States : 
    GameStateMachine<OxyliteRefinery.States, OxyliteRefinery.StatesInstance, OxyliteRefinery>
  {
    public GameStateMachine<OxyliteRefinery.States, OxyliteRefinery.StatesInstance, OxyliteRefinery, object>.State disabled;
    public GameStateMachine<OxyliteRefinery.States, OxyliteRefinery.StatesInstance, OxyliteRefinery, object>.State waiting;
    public GameStateMachine<OxyliteRefinery.States, OxyliteRefinery.StatesInstance, OxyliteRefinery, object>.State converting;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.disabled;
      this.root.EventTransition(GameHashes.OperationalChanged, this.disabled, (StateMachine<OxyliteRefinery.States, OxyliteRefinery.StatesInstance, OxyliteRefinery, object>.Transition.ConditionCallback) (smi => !smi.master.operational.IsOperational));
      this.disabled.EventTransition(GameHashes.OperationalChanged, this.waiting, (StateMachine<OxyliteRefinery.States, OxyliteRefinery.StatesInstance, OxyliteRefinery, object>.Transition.ConditionCallback) (smi => smi.master.operational.IsOperational));
      this.waiting.EventTransition(GameHashes.OnStorageChange, this.converting, (StateMachine<OxyliteRefinery.States, OxyliteRefinery.StatesInstance, OxyliteRefinery, object>.Transition.ConditionCallback) (smi => ((Component) smi.master).GetComponent<ElementConverter>().HasEnoughMassToStartConverting()));
      this.converting.Enter((StateMachine<OxyliteRefinery.States, OxyliteRefinery.StatesInstance, OxyliteRefinery, object>.State.Callback) (smi => smi.master.operational.SetActive(true))).Exit((StateMachine<OxyliteRefinery.States, OxyliteRefinery.StatesInstance, OxyliteRefinery, object>.State.Callback) (smi => smi.master.operational.SetActive(false))).Transition(this.waiting, (StateMachine<OxyliteRefinery.States, OxyliteRefinery.StatesInstance, OxyliteRefinery, object>.Transition.ConditionCallback) (smi => !((Component) smi.master).GetComponent<ElementConverter>().CanConvertAtAll())).EventHandler(GameHashes.OnStorageChange, (StateMachine<OxyliteRefinery.States, OxyliteRefinery.StatesInstance, OxyliteRefinery, object>.State.Callback) (smi => smi.TryEmit()));
    }
  }
}
