// Decompiled with JetBrains decompiler
// Type: FlowerVase
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class FlowerVase : StateMachineComponent<FlowerVase.SMInstance>
{
  [MyCmpReq]
  private PlantablePlot plantablePlot;
  [MyCmpReq]
  private KBoxCollider2D boxCollider;

  protected virtual void OnPrefabInit() => base.OnPrefabInit();

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
  }

  public class SMInstance : 
    GameStateMachine<FlowerVase.States, FlowerVase.SMInstance, FlowerVase, object>.GameInstance
  {
    public SMInstance(FlowerVase master)
      : base(master)
    {
    }
  }

  public class States : GameStateMachine<FlowerVase.States, FlowerVase.SMInstance, FlowerVase>
  {
    public GameStateMachine<FlowerVase.States, FlowerVase.SMInstance, FlowerVase, object>.State empty;
    public GameStateMachine<FlowerVase.States, FlowerVase.SMInstance, FlowerVase, object>.State full;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.empty;
      this.empty.EventTransition(GameHashes.OccupantChanged, this.full, (StateMachine<FlowerVase.States, FlowerVase.SMInstance, FlowerVase, object>.Transition.ConditionCallback) (smi => Object.op_Inequality((Object) smi.master.plantablePlot.Occupant, (Object) null))).PlayAnim("off");
      this.full.EventTransition(GameHashes.OccupantChanged, this.empty, (StateMachine<FlowerVase.States, FlowerVase.SMInstance, FlowerVase, object>.Transition.ConditionCallback) (smi => Object.op_Equality((Object) smi.master.plantablePlot.Occupant, (Object) null))).PlayAnim("on");
    }
  }
}
