// Decompiled with JetBrains decompiler
// Type: MissionControlCluster
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class MissionControlCluster : 
  GameStateMachine<MissionControlCluster, MissionControlCluster.Instance, IStateMachineTarget, MissionControlCluster.Def>
{
  public GameStateMachine<MissionControlCluster, MissionControlCluster.Instance, IStateMachineTarget, MissionControlCluster.Def>.State Inoperational;
  public MissionControlCluster.OperationalState Operational;
  public StateMachine<MissionControlCluster, MissionControlCluster.Instance, IStateMachineTarget, MissionControlCluster.Def>.BoolParameter WorkableRocketsAreInRange;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.Inoperational;
    this.Inoperational.EventTransition(GameHashes.OperationalChanged, (GameStateMachine<MissionControlCluster, MissionControlCluster.Instance, IStateMachineTarget, MissionControlCluster.Def>.State) this.Operational, new StateMachine<MissionControlCluster, MissionControlCluster.Instance, IStateMachineTarget, MissionControlCluster.Def>.Transition.ConditionCallback(this.ValidateOperationalTransition)).EventTransition(GameHashes.UpdateRoom, (GameStateMachine<MissionControlCluster, MissionControlCluster.Instance, IStateMachineTarget, MissionControlCluster.Def>.State) this.Operational, new StateMachine<MissionControlCluster, MissionControlCluster.Instance, IStateMachineTarget, MissionControlCluster.Def>.Transition.ConditionCallback(this.ValidateOperationalTransition));
    this.Operational.EventTransition(GameHashes.OperationalChanged, this.Inoperational, new StateMachine<MissionControlCluster, MissionControlCluster.Instance, IStateMachineTarget, MissionControlCluster.Def>.Transition.ConditionCallback(this.ValidateOperationalTransition)).EventTransition(GameHashes.UpdateRoom, this.Operational.WrongRoom, GameStateMachine<MissionControlCluster, MissionControlCluster.Instance, IStateMachineTarget, MissionControlCluster.Def>.Not(new StateMachine<MissionControlCluster, MissionControlCluster.Instance, IStateMachineTarget, MissionControlCluster.Def>.Transition.ConditionCallback(this.IsInLabRoom))).Enter(new StateMachine<MissionControlCluster, MissionControlCluster.Instance, IStateMachineTarget, MissionControlCluster.Def>.State.Callback(this.OnEnterOperational)).DefaultState(this.Operational.NoRockets).Update((System.Action<MissionControlCluster.Instance, float>) ((smi, dt) => smi.UpdateWorkableRocketsInRange((object) null)), (UpdateRate) 6);
    this.Operational.WrongRoom.EventTransition(GameHashes.UpdateRoom, this.Operational.NoRockets, new StateMachine<MissionControlCluster, MissionControlCluster.Instance, IStateMachineTarget, MissionControlCluster.Def>.Transition.ConditionCallback(this.IsInLabRoom));
    this.Operational.NoRockets.ToggleStatusItem(Db.Get().BuildingStatusItems.NoRocketsToMissionControlClusterBoost).ParamTransition<bool>((StateMachine<MissionControlCluster, MissionControlCluster.Instance, IStateMachineTarget, MissionControlCluster.Def>.Parameter<bool>) this.WorkableRocketsAreInRange, this.Operational.HasRockets, (StateMachine<MissionControlCluster, MissionControlCluster.Instance, IStateMachineTarget, MissionControlCluster.Def>.Parameter<bool>.Callback) ((smi, inRange) => this.WorkableRocketsAreInRange.Get(smi)));
    this.Operational.HasRockets.ParamTransition<bool>((StateMachine<MissionControlCluster, MissionControlCluster.Instance, IStateMachineTarget, MissionControlCluster.Def>.Parameter<bool>) this.WorkableRocketsAreInRange, this.Operational.NoRockets, (StateMachine<MissionControlCluster, MissionControlCluster.Instance, IStateMachineTarget, MissionControlCluster.Def>.Parameter<bool>.Callback) ((smi, inRange) => !this.WorkableRocketsAreInRange.Get(smi))).ToggleChore(new Func<MissionControlCluster.Instance, Chore>(this.CreateChore), (GameStateMachine<MissionControlCluster, MissionControlCluster.Instance, IStateMachineTarget, MissionControlCluster.Def>.State) this.Operational);
  }

  private Chore CreateChore(MissionControlCluster.Instance smi)
  {
    MissionControlClusterWorkable component = smi.master.gameObject.GetComponent<MissionControlClusterWorkable>();
    WorkChore<MissionControlClusterWorkable> chore = new WorkChore<MissionControlClusterWorkable>(Db.Get().ChoreTypes.Research, (IStateMachineTarget) component);
    Clustercraft boostableClustercraft = smi.GetRandomBoostableClustercraft();
    component.TargetClustercraft = boostableClustercraft;
    return (Chore) chore;
  }

  private void OnEnterOperational(MissionControlCluster.Instance smi)
  {
    smi.UpdateWorkableRocketsInRange((object) null);
    if (this.WorkableRocketsAreInRange.Get(smi))
      smi.GoTo((StateMachine.BaseState) this.Operational.HasRockets);
    else
      smi.GoTo((StateMachine.BaseState) this.Operational.NoRockets);
  }

  private bool ValidateOperationalTransition(MissionControlCluster.Instance smi)
  {
    global::Operational component = smi.GetComponent<global::Operational>();
    bool flag = smi.IsInsideState((StateMachine.BaseState) smi.sm.Operational);
    return Object.op_Inequality((Object) component, (Object) null) && flag != component.IsOperational;
  }

  private bool IsInLabRoom(MissionControlCluster.Instance smi) => smi.roomTracker.IsInCorrectRoom();

  public class Def : StateMachine.BaseDef
  {
  }

  public new class Instance : 
    GameStateMachine<MissionControlCluster, MissionControlCluster.Instance, IStateMachineTarget, MissionControlCluster.Def>.GameInstance
  {
    private int clusterUpdatedHandle = -1;
    private List<Clustercraft> boostableClustercraft = new List<Clustercraft>();
    [MyCmpReq]
    public RoomTracker roomTracker;

    public Instance(IStateMachineTarget master, MissionControlCluster.Def def)
      : base(master, def)
    {
    }

    public override void StartSM()
    {
      base.StartSM();
      this.clusterUpdatedHandle = Game.Instance.Subscribe(-1298331547, new System.Action<object>(this.UpdateWorkableRocketsInRange));
    }

    public override void StopSM(string reason)
    {
      base.StopSM(reason);
      Game.Instance.Unsubscribe(this.clusterUpdatedHandle);
    }

    public void UpdateWorkableRocketsInRange(object data)
    {
      this.boostableClustercraft.Clear();
      AxialI myWorldLocation = this.gameObject.GetMyWorldLocation();
      for (int idx = 0; idx < Components.Clustercrafts.Count; ++idx)
      {
        if (ClusterGrid.Instance.IsInRange(Components.Clustercrafts[idx].Location, myWorldLocation, 2) && !this.IsOwnWorld(Components.Clustercrafts[idx]) && this.CanBeBoosted(Components.Clustercrafts[idx]))
        {
          bool flag = false;
          foreach (MissionControlClusterWorkable controlClusterWorkable in Components.MissionControlClusterWorkables)
          {
            if (!Object.op_Equality((Object) ((Component) controlClusterWorkable).gameObject, (Object) this.gameObject) && Object.op_Equality((Object) controlClusterWorkable.TargetClustercraft, (Object) Components.Clustercrafts[idx]))
            {
              flag = true;
              break;
            }
          }
          if (!flag)
            this.boostableClustercraft.Add(Components.Clustercrafts[idx]);
        }
      }
      this.sm.WorkableRocketsAreInRange.Set(this.boostableClustercraft.Count > 0, this.smi);
    }

    public Clustercraft GetRandomBoostableClustercraft() => Util.GetRandom<Clustercraft>(this.boostableClustercraft);

    private bool CanBeBoosted(Clustercraft clustercraft) => (double) clustercraft.controlStationBuffTimeRemaining == 0.0 && clustercraft.HasResourcesToMove() && clustercraft.IsFlightInProgress();

    private bool IsOwnWorld(Clustercraft candidateClustercraft)
    {
      int myWorldId = this.gameObject.GetMyWorldId();
      WorldContainer interiorWorld = candidateClustercraft.ModuleInterface.GetInteriorWorld();
      return !Object.op_Equality((Object) interiorWorld, (Object) null) && myWorldId == interiorWorld.id;
    }

    public void ApplyEffect(Clustercraft clustercraft) => clustercraft.controlStationBuffTimeRemaining = 600f;
  }

  public class OperationalState : 
    GameStateMachine<MissionControlCluster, MissionControlCluster.Instance, IStateMachineTarget, MissionControlCluster.Def>.State
  {
    public GameStateMachine<MissionControlCluster, MissionControlCluster.Instance, IStateMachineTarget, MissionControlCluster.Def>.State WrongRoom;
    public GameStateMachine<MissionControlCluster, MissionControlCluster.Instance, IStateMachineTarget, MissionControlCluster.Def>.State NoRockets;
    public GameStateMachine<MissionControlCluster, MissionControlCluster.Instance, IStateMachineTarget, MissionControlCluster.Def>.State HasRockets;
  }
}
