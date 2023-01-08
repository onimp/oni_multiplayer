// Decompiled with JetBrains decompiler
// Type: RanchStation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class RanchStation : 
  GameStateMachine<RanchStation, RanchStation.Instance, IStateMachineTarget, RanchStation.Def>
{
  public StateMachine<RanchStation, RanchStation.Instance, IStateMachineTarget, RanchStation.Def>.BoolParameter RancherIsReady;
  public GameStateMachine<RanchStation, RanchStation.Instance, IStateMachineTarget, RanchStation.Def>.State Unoperational;
  public RanchStation.OperationalState Operational;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.Operational;
    this.Unoperational.TagTransition(GameTags.Operational, (GameStateMachine<RanchStation, RanchStation.Instance, IStateMachineTarget, RanchStation.Def>.State) this.Operational);
    this.Operational.TagTransition(GameTags.Operational, this.Unoperational, true).ToggleChore((Func<RanchStation.Instance, Chore>) (smi => smi.CreateChore()), this.Unoperational, this.Unoperational).Update("FindRanachable", (System.Action<RanchStation.Instance, float>) ((smi, dt) => smi.FindRanchable()));
  }

  public class Def : StateMachine.BaseDef
  {
    public Func<GameObject, RanchStation.Instance, bool> IsCritterEligibleToBeRanchedCb;
    public System.Action<GameObject> OnRanchCompleteCb;
    public HashedString RanchedPreAnim = HashedString.op_Implicit("idle_loop");
    public HashedString RanchedLoopAnim = HashedString.op_Implicit("idle_loop");
    public HashedString RanchedPstAnim = HashedString.op_Implicit("idle_loop");
    public HashedString RanchedAbortAnim = HashedString.op_Implicit("idle_loop");
    public HashedString RancherInteractAnim = HashedString.op_Implicit("anim_interacts_rancherstation_kanim");
    public StatusItem RanchingStatusItem = Db.Get().DuplicantStatusItems.Ranching;
    public float WorkTime = 12f;
    public Func<RanchStation.Instance, int> GetTargetRanchCell = (Func<RanchStation.Instance, int>) (smi => Grid.PosToCell((StateMachine.Instance) smi));
  }

  public class OperationalState : 
    GameStateMachine<RanchStation, RanchStation.Instance, IStateMachineTarget, RanchStation.Def>.State
  {
  }

  public new class Instance : 
    GameStateMachine<RanchStation, RanchStation.Instance, IStateMachineTarget, RanchStation.Def>.GameInstance
  {
    private const int QUEUE_SIZE = 2;
    private List<RanchableMonitor.Instance> targetRanchables = new List<RanchableMonitor.Instance>();
    private RanchedStates.Instance activeRanchable;
    private Room ranch;
    private Worker rancher;
    private BuildingComplete station;
    private StateMachine<RanchStation, RanchStation.Instance, IStateMachineTarget, RanchStation.Def>.BoolParameter.Context rancherReadyContext;

    public RanchedStates.Instance ActiveRanchable => this.activeRanchable;

    private bool isCritterAvailableForRanching => this.targetRanchables.Count > 0;

    public bool IsCritterAvailableForRanching
    {
      get
      {
        this.ValidateTargetRanchables();
        return this.isCritterAvailableForRanching;
      }
    }

    public bool HasRancher => Object.op_Inequality((Object) this.rancher, (Object) null);

    public bool IsRancherReady => this.rancherReadyContext.value;

    public System.Action<RanchStation.Instance> RancherStateChanged
    {
      get => this.rancherReadyContext.onDirty;
      set => this.rancherReadyContext.onDirty = value;
    }

    public Extents StationExtents => this.station.GetExtents();

    public int GetRanchNavTarget() => this.def.GetTargetRanchCell(this);

    public Instance(IStateMachineTarget master, RanchStation.Def def)
      : base(master, def)
    {
      this.gameObject.AddOrGet<RancherChore.RancherWorkable>();
      this.station = this.GetComponent<BuildingComplete>();
      this.rancherReadyContext = this.GetParameterContext((StateMachine.Parameter) this.sm.RancherIsReady) as StateMachine<RanchStation, RanchStation.Instance, IStateMachineTarget, RanchStation.Def>.BoolParameter.Context;
    }

    public Chore CreateChore()
    {
      RancherChore chore = new RancherChore(this.GetComponent<KPrefabID>());
      StateMachine<RancherChore.RancherChoreStates, RancherChore.RancherChoreStates.Instance, IStateMachineTarget, object>.TargetParameter rancher = chore.smi.sm.rancher;
      rancher.GetContext(chore.smi).onDirty += new System.Action<RancherChore.RancherChoreStates.Instance>(this.OnRancherChanged);
      this.rancher = rancher.Get<Worker>(chore.smi);
      return (Chore) chore;
    }

    public int GetTargetRanchCell() => this.def.GetTargetRanchCell(this);

    public override void StartSM()
    {
      base.StartSM();
      this.Subscribe(144050788, new System.Action<object>(this.OnRoomUpdated));
      CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(this.GetTargetRanchCell());
      if (cavityForCell == null || cavityForCell.room == null)
        return;
      this.OnRoomUpdated((object) cavityForCell.room);
    }

    public override void StopSM(string reason)
    {
      base.StopSM(reason);
      this.Unsubscribe(144050788, new System.Action<object>(this.OnRoomUpdated));
    }

    private void OnRoomUpdated(object data)
    {
      if (data == null)
        return;
      this.ranch = data as Room;
      if (this.ranch.roomType == Db.Get().RoomTypes.CreaturePen)
        return;
      this.TriggerRanchStationNoLongerAvailable();
      this.ranch = (Room) null;
    }

    private void OnRancherChanged(
      RancherChore.RancherChoreStates.Instance choreInstance)
    {
      this.rancher = choreInstance.sm.rancher.Get<Worker>(choreInstance);
      this.TriggerRanchStationNoLongerAvailable();
    }

    public bool TryGetRanched(RanchedStates.Instance ranchable) => this.activeRanchable == null || this.activeRanchable == ranchable;

    public void MessageCreatureArrived(RanchedStates.Instance critter)
    {
      this.activeRanchable = critter;
      this.rancherReadyContext.Set(false, this);
      this.smi.ScheduleNextFrame(new System.Action<object>(this.DelayedNotification), (object) null);
    }

    public void DelayedNotification(object _) => this.Trigger(-1357116271);

    public void MessageRancherReady() => this.rancherReadyContext.Set(true, this);

    private bool CanRanchableBeRanchedAtRanchStation(RanchableMonitor.Instance ranchable)
    {
      bool flag1 = !ranchable.IsNullOrStopped();
      if (flag1 && ranchable.TargetRanchStation != null && ranchable.TargetRanchStation != this)
        flag1 = !ranchable.TargetRanchStation.IsRunning() || !ranchable.TargetRanchStation.HasRancher;
      bool flag2 = flag1 && this.def.IsCritterEligibleToBeRanchedCb(ranchable.gameObject, this) && ranchable.ChoreConsumer.IsChoreEqualOrAboveCurrentChorePriority<RanchedStates>();
      if (flag2)
      {
        CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell(TransformExtensions.GetPosition(ranchable.transform)));
        if (cavityForCell == null || cavityForCell != this.ranch.cavity)
        {
          flag2 = false;
        }
        else
        {
          int cell = this.GetRanchNavTarget();
          if (ranchable.HasTag(GameTags.Creatures.Flyer))
            cell = Grid.CellAbove(cell);
          flag2 = ranchable.NavComponent.GetNavigationCost(cell) != -1;
        }
      }
      return flag2;
    }

    public void ValidateTargetRanchables()
    {
      if (!this.HasRancher)
        return;
      for (int index = this.targetRanchables.Count - 1; index >= 0; --index)
      {
        RanchableMonitor.Instance targetRanchable = this.targetRanchables[index];
        if (targetRanchable.States == null)
          this.Abandon(targetRanchable);
        else if (!this.CanRanchableBeRanchedAtRanchStation(targetRanchable))
          targetRanchable.States.AbandonRanchStation();
      }
    }

    public void FindRanchable(object _ = null)
    {
      if (this.ranch == null)
        return;
      this.ValidateTargetRanchables();
      if (this.targetRanchables.Count == 2)
        return;
      List<KPrefabID> creatures = this.ranch.cavity.creatures;
      if (this.HasRancher && !this.isCritterAvailableForRanching && creatures.Count == 0)
        this.TryNotifyEmptyRanch();
      for (int index = 0; index < creatures.Count; ++index)
      {
        KPrefabID cmp = creatures[index];
        if (!Object.op_Equality((Object) cmp, (Object) null))
        {
          RanchableMonitor.Instance smi = ((Component) cmp).GetSMI<RanchableMonitor.Instance>();
          if (!this.targetRanchables.Contains(smi) && this.CanRanchableBeRanchedAtRanchStation(smi))
          {
            if (smi != null)
            {
              smi.States.AbandonRanchStation();
              smi.TargetRanchStation = this;
            }
            this.targetRanchables.Add(smi);
            break;
          }
        }
      }
    }

    public void RanchCreature()
    {
      if (this.activeRanchable.IsNullOrStopped())
        return;
      Debug.Assert(this.activeRanchable != null, (object) "targetRanchable was null");
      Debug.Assert(this.activeRanchable.GetMaster() != null, (object) "GetMaster was null");
      Debug.Assert(this.def != null, (object) "def was null");
      Debug.Assert(this.def.OnRanchCompleteCb != null, (object) "onRanchCompleteCb cb was null");
      this.def.OnRanchCompleteCb(this.activeRanchable.gameObject);
      this.targetRanchables.Remove(this.activeRanchable.Monitor);
      this.activeRanchable.Trigger(1827504087);
      this.activeRanchable = (RanchedStates.Instance) null;
      this.smi.ScheduleNextFrame(new System.Action<object>(this.FindRanchable), (object) null);
    }

    public void TriggerRanchStationNoLongerAvailable()
    {
      for (int index = 0; index < this.targetRanchables.Count; ++index)
      {
        RanchableMonitor.Instance targetRanchable = this.targetRanchables[index];
        if (!targetRanchable.IsNullOrStopped() && !targetRanchable.States.IsNullOrStopped())
          targetRanchable.Trigger(1689625967);
      }
      this.targetRanchables.Clear();
      this.RancherStateChanged = (System.Action<RanchStation.Instance>) null;
      this.rancherReadyContext.Set(false, this);
    }

    public void Abandon(RanchableMonitor.Instance critter)
    {
      this.targetRanchables.Remove(critter);
      if (critter.States == null)
        return;
      bool flag = !this.isCritterAvailableForRanching;
      if (critter.States == this.activeRanchable)
      {
        flag = true;
        this.activeRanchable = (RanchedStates.Instance) null;
      }
      critter.TargetRanchStation = (RanchStation.Instance) null;
      if (!flag)
        return;
      this.TryNotifyEmptyRanch();
    }

    private void TryNotifyEmptyRanch()
    {
      if (!this.HasRancher)
        return;
      this.rancher.Trigger(-364750427, (object) null);
    }
  }
}
