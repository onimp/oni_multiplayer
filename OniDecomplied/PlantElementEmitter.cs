// Decompiled with JetBrains decompiler
// Type: PlantElementEmitter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class PlantElementEmitter : 
  StateMachineComponent<PlantElementEmitter.StatesInstance>,
  IGameObjectEffectDescriptor
{
  [MyCmpGet]
  private WiltCondition wiltCondition;
  [MyCmpReq]
  private KSelectable selectable;
  public SimHashes emittedElement;
  public float emitRate;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
  }

  public List<Descriptor> GetDescriptors(GameObject go) => new List<Descriptor>();

  public class StatesInstance : 
    GameStateMachine<PlantElementEmitter.States, PlantElementEmitter.StatesInstance, PlantElementEmitter, object>.GameInstance
  {
    public StatesInstance(PlantElementEmitter master)
      : base(master)
    {
    }

    public bool IsWilting() => !Object.op_Equality((Object) this.master.wiltCondition, (Object) null) && Object.op_Inequality((Object) this.master.wiltCondition, (Object) null) && this.master.wiltCondition.IsWilting();
  }

  public class States : 
    GameStateMachine<PlantElementEmitter.States, PlantElementEmitter.StatesInstance, PlantElementEmitter>
  {
    public GameStateMachine<PlantElementEmitter.States, PlantElementEmitter.StatesInstance, PlantElementEmitter, object>.State wilted;
    public GameStateMachine<PlantElementEmitter.States, PlantElementEmitter.StatesInstance, PlantElementEmitter, object>.State healthy;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.healthy;
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      this.healthy.EventTransition(GameHashes.Wilt, this.wilted, (StateMachine<PlantElementEmitter.States, PlantElementEmitter.StatesInstance, PlantElementEmitter, object>.Transition.ConditionCallback) (smi => smi.IsWilting())).Update("PlantEmit", (Action<PlantElementEmitter.StatesInstance, float>) ((smi, dt) => SimMessages.EmitMass(Grid.PosToCell(((Component) smi.master).gameObject), ElementLoader.FindElementByHash(smi.master.emittedElement).idx, smi.master.emitRate * dt, ElementLoader.FindElementByHash(smi.master.emittedElement).defaultValues.temperature, byte.MaxValue, 0)), (UpdateRate) 7);
      this.wilted.EventTransition(GameHashes.WiltRecover, this.healthy);
    }
  }
}
