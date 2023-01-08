// Decompiled with JetBrains decompiler
// Type: PodLander
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class PodLander : StateMachineComponent<PodLander.StatesInstance>, IGameObjectEffectDescriptor
{
  [Serialize]
  private int landOffLocation;
  [Serialize]
  private float flightAnimOffset;
  private float rocketSpeed;
  public float exhaustEmitRate = 2f;
  public float exhaustTemperature = 1000f;
  public SimHashes exhaustElement = SimHashes.CarbonDioxide;
  private GameObject soundSpeakerObject;
  private bool releasingAstronaut;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
  }

  public void ReleaseAstronaut()
  {
    if (this.releasingAstronaut)
      return;
    this.releasingAstronaut = true;
    MinionStorage component = ((Component) this).GetComponent<MinionStorage>();
    List<MinionStorage.Info> storedMinionInfo = component.GetStoredMinionInfo();
    for (int index = storedMinionInfo.Count - 1; index >= 0; --index)
    {
      MinionStorage.Info info = storedMinionInfo[index];
      component.DeserializeMinion(info.id, Grid.CellToPos(Grid.PosToCell(TransformExtensions.GetPosition(this.smi.master.transform))));
    }
    this.releasingAstronaut = false;
  }

  public List<Descriptor> GetDescriptors(GameObject go) => (List<Descriptor>) null;

  public class StatesInstance : 
    GameStateMachine<PodLander.States, PodLander.StatesInstance, PodLander, object>.GameInstance
  {
    public StatesInstance(PodLander master)
      : base(master)
    {
    }
  }

  public class States : GameStateMachine<PodLander.States, PodLander.StatesInstance, PodLander>
  {
    public GameStateMachine<PodLander.States, PodLander.StatesInstance, PodLander, object>.State landing;
    public GameStateMachine<PodLander.States, PodLander.StatesInstance, PodLander, object>.State crashed;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.landing;
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      this.landing.PlayAnim("launch_loop", (KAnim.PlayMode) 0).Enter((StateMachine<PodLander.States, PodLander.StatesInstance, PodLander, object>.State.Callback) (smi => smi.master.flightAnimOffset = 50f)).Update((Action<PodLander.StatesInstance, float>) ((smi, dt) =>
      {
        float num = 10f;
        smi.master.rocketSpeed = num - Mathf.Clamp(Mathf.Pow(smi.timeinstate / 3.5f, 4f), 0.0f, num - 2f);
        smi.master.flightAnimOffset -= dt * smi.master.rocketSpeed;
        KBatchedAnimController component = ((Component) smi.master).GetComponent<KBatchedAnimController>();
        component.Offset = Vector3.op_Multiply(Vector3.up, smi.master.flightAnimOffset);
        Vector3 positionIncludingOffset = component.PositionIncludingOffset;
        int cell = Grid.PosToCell(Vector3.op_Addition(TransformExtensions.GetPosition(((Component) smi.master).gameObject.transform), ((Component) smi.master).GetComponent<KBatchedAnimController>().Offset));
        if (Grid.IsValidCell(cell))
          SimMessages.EmitMass(cell, ElementLoader.GetElementIndex(smi.master.exhaustElement), dt * smi.master.exhaustEmitRate, smi.master.exhaustTemperature, (byte) 0, 0);
        if ((double) component.Offset.y > 0.0)
          return;
        smi.GoTo((StateMachine.BaseState) this.crashed);
      }), (UpdateRate) 4);
      this.crashed.PlayAnim("grounded").Enter((StateMachine<PodLander.States, PodLander.StatesInstance, PodLander, object>.State.Callback) (smi =>
      {
        ((Component) smi.master).GetComponent<KBatchedAnimController>().Offset = Vector3.zero;
        smi.master.rocketSpeed = 0.0f;
        smi.master.ReleaseAstronaut();
      }));
    }
  }
}
