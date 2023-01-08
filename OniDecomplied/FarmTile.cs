// Decompiled with JetBrains decompiler
// Type: FarmTile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class FarmTile : StateMachineComponent<FarmTile.SMInstance>
{
  [MyCmpReq]
  private PlantablePlot plantablePlot;
  [MyCmpReq]
  private Storage storage;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
  }

  public class SMInstance : 
    GameStateMachine<FarmTile.States, FarmTile.SMInstance, FarmTile, object>.GameInstance
  {
    public SMInstance(FarmTile master)
      : base(master)
    {
    }

    public bool HasWater()
    {
      PrimaryElement primaryElement = this.master.storage.FindPrimaryElement(SimHashes.Water);
      return Object.op_Inequality((Object) primaryElement, (Object) null) && (double) primaryElement.Mass > 0.0;
    }
  }

  public class States : GameStateMachine<FarmTile.States, FarmTile.SMInstance, FarmTile>
  {
    public FarmTile.States.FarmStates empty;
    public FarmTile.States.FarmStates full;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.empty;
      this.empty.EventTransition(GameHashes.OccupantChanged, (GameStateMachine<FarmTile.States, FarmTile.SMInstance, FarmTile, object>.State) this.full, (StateMachine<FarmTile.States, FarmTile.SMInstance, FarmTile, object>.Transition.ConditionCallback) (smi => Object.op_Inequality((Object) smi.master.plantablePlot.Occupant, (Object) null)));
      this.empty.wet.EventTransition(GameHashes.OnStorageChange, this.empty.dry, (StateMachine<FarmTile.States, FarmTile.SMInstance, FarmTile, object>.Transition.ConditionCallback) (smi => !smi.HasWater()));
      this.empty.dry.EventTransition(GameHashes.OnStorageChange, this.empty.wet, (StateMachine<FarmTile.States, FarmTile.SMInstance, FarmTile, object>.Transition.ConditionCallback) (smi => !smi.HasWater()));
      this.full.EventTransition(GameHashes.OccupantChanged, (GameStateMachine<FarmTile.States, FarmTile.SMInstance, FarmTile, object>.State) this.empty, (StateMachine<FarmTile.States, FarmTile.SMInstance, FarmTile, object>.Transition.ConditionCallback) (smi => Object.op_Equality((Object) smi.master.plantablePlot.Occupant, (Object) null)));
      this.full.wet.EventTransition(GameHashes.OnStorageChange, this.full.dry, (StateMachine<FarmTile.States, FarmTile.SMInstance, FarmTile, object>.Transition.ConditionCallback) (smi => !smi.HasWater()));
      this.full.dry.EventTransition(GameHashes.OnStorageChange, this.full.wet, (StateMachine<FarmTile.States, FarmTile.SMInstance, FarmTile, object>.Transition.ConditionCallback) (smi => !smi.HasWater()));
    }

    public class FarmStates : 
      GameStateMachine<FarmTile.States, FarmTile.SMInstance, FarmTile, object>.State
    {
      public GameStateMachine<FarmTile.States, FarmTile.SMInstance, FarmTile, object>.State wet;
      public GameStateMachine<FarmTile.States, FarmTile.SMInstance, FarmTile, object>.State dry;
    }
  }
}
