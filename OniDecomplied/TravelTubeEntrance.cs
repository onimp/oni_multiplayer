// Decompiled with JetBrains decompiler
// Type: TravelTubeEntrance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using UnityEngine;

[SerializationConfig]
public class TravelTubeEntrance : 
  StateMachineComponent<TravelTubeEntrance.SMInstance>,
  ISaveLoadable,
  ISim200ms
{
  [MyCmpReq]
  private Operational operational;
  [MyCmpReq]
  private TravelTubeEntrance.Work launch_workable;
  [MyCmpReq]
  private EnergyConsumerSelfSustaining energyConsumer;
  [MyCmpGet]
  private BuildingEnabledButton button;
  [MyCmpReq]
  private KSelectable selectable;
  public float jouleCapacity = 1f;
  public float joulesPerLaunch = 1f;
  [Serialize]
  private float availableJoules;
  private TravelTube travelTube;
  private TravelTubeEntrance.WaitReactable wait_reactable;
  private MeterController meter;
  private const int MAX_CHARGES = 3;
  private const float RECHARGE_TIME = 10f;
  private static readonly Operational.Flag tubeConnected = new Operational.Flag(nameof (tubeConnected), Operational.Flag.Type.Functional);
  private HandleVector<int>.Handle tubeChangedEntry;
  private static readonly EventSystem.IntraObjectHandler<TravelTubeEntrance> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<TravelTubeEntrance>((Action<TravelTubeEntrance, object>) ((component, data) => component.OnOperationalChanged(data)));
  private Guid connectedStatus;

  public float AvailableJoules => this.availableJoules;

  public float TotalCapacity => this.jouleCapacity;

  public float UsageJoules => this.joulesPerLaunch;

  public bool HasLaunchPower => (double) this.availableJoules > (double) this.joulesPerLaunch;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.energyConsumer.OnConnectionChanged += new System.Action(this.OnConnectionChanged);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    int x = (int) TransformExtensions.GetPosition(this.transform).x;
    int y = (int) TransformExtensions.GetPosition(this.transform).y + 2;
    Extents extents = new Extents(x, y, 1, 1);
    this.TubeConnectionsChanged((object) Game.Instance.travelTubeSystem.GetConnections(Grid.XYToCell(x, y), true));
    this.tubeChangedEntry = GameScenePartitioner.Instance.Add("TravelTubeEntrance.TubeListener", (object) ((Component) this).gameObject, extents, GameScenePartitioner.Instance.objectLayers[35], new Action<object>(this.TubeChanged));
    this.Subscribe<TravelTubeEntrance>(-592767678, TravelTubeEntrance.OnOperationalChangedDelegate);
    this.meter = new MeterController((KMonoBehaviour) this, Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
    this.CreateNewWaitReactable();
    Grid.RegisterTubeEntrance(Grid.PosToCell((KMonoBehaviour) this), Mathf.FloorToInt(this.availableJoules / this.joulesPerLaunch));
    this.smi.StartSM();
    this.UpdateCharge();
  }

  protected override void OnCleanUp()
  {
    if (Object.op_Inequality((Object) this.travelTube, (Object) null))
    {
      this.travelTube.Unsubscribe(-1041684577, new Action<object>(this.TubeConnectionsChanged));
      this.travelTube = (TravelTube) null;
    }
    Grid.UnregisterTubeEntrance(Grid.PosToCell((KMonoBehaviour) this));
    this.ClearWaitReactable();
    GameScenePartitioner.Instance.Free(ref this.tubeChangedEntry);
    base.OnCleanUp();
  }

  private void TubeChanged(object data)
  {
    if (Object.op_Inequality((Object) this.travelTube, (Object) null))
    {
      this.travelTube.Unsubscribe(-1041684577, new Action<object>(this.TubeConnectionsChanged));
      this.travelTube = (TravelTube) null;
    }
    GameObject gameObject = data as GameObject;
    if (data != null)
    {
      TravelTube component = gameObject.GetComponent<TravelTube>();
      if (Object.op_Inequality((Object) component, (Object) null))
      {
        component.Subscribe(-1041684577, new Action<object>(this.TubeConnectionsChanged));
        this.travelTube = component;
      }
      else
        this.TubeConnectionsChanged((object) 0);
    }
    else
      this.TubeConnectionsChanged((object) 0);
  }

  private void TubeConnectionsChanged(object data)
  {
    bool flag = (UtilityConnections) data == UtilityConnections.Up;
    this.operational.SetFlag(TravelTubeEntrance.tubeConnected, flag);
  }

  private bool CanAcceptMorePower() => this.operational.IsOperational && (Object.op_Equality((Object) this.button, (Object) null) || this.button.IsEnabled) && this.energyConsumer.IsExternallyPowered && (double) this.availableJoules < (double) this.jouleCapacity;

  public void Sim200ms(float dt)
  {
    if (this.CanAcceptMorePower())
    {
      this.availableJoules = Mathf.Min(this.jouleCapacity, this.availableJoules + this.energyConsumer.WattsUsed * dt);
      this.UpdateCharge();
    }
    this.energyConsumer.SetSustained(this.HasLaunchPower);
    this.UpdateActive();
    this.UpdateConnectionStatus();
  }

  public void Reserve(TubeTraveller.Instance traveller, int prefabInstanceID) => Grid.ReserveTubeEntrance(Grid.PosToCell((KMonoBehaviour) this), prefabInstanceID, true);

  public void Unreserve(TubeTraveller.Instance traveller, int prefabInstanceID) => Grid.ReserveTubeEntrance(Grid.PosToCell((KMonoBehaviour) this), prefabInstanceID, false);

  public bool IsTraversable(Navigator agent) => Grid.HasUsableTubeEntrance(Grid.PosToCell((KMonoBehaviour) this), ((Component) agent).gameObject.GetComponent<KPrefabID>().InstanceID);

  public bool HasChargeSlotReserved(Navigator agent) => Grid.HasReservedTubeEntrance(Grid.PosToCell((KMonoBehaviour) this), ((Component) agent).gameObject.GetComponent<KPrefabID>().InstanceID);

  public bool HasChargeSlotReserved(TubeTraveller.Instance tube_traveller, int prefabInstanceID) => Grid.HasReservedTubeEntrance(Grid.PosToCell((KMonoBehaviour) this), prefabInstanceID);

  public bool IsChargedSlotAvailable(TubeTraveller.Instance tube_traveller, int prefabInstanceID) => Grid.HasUsableTubeEntrance(Grid.PosToCell((KMonoBehaviour) this), prefabInstanceID);

  public bool ShouldWait(GameObject reactor) => this.operational.IsOperational && this.HasLaunchPower && !Object.op_Equality((Object) this.launch_workable.worker, (Object) null) && this.HasChargeSlotReserved(reactor.GetSMI<TubeTraveller.Instance>(), reactor.GetComponent<KPrefabID>().InstanceID);

  public void ConsumeCharge(GameObject reactor)
  {
    if (!this.HasLaunchPower)
      return;
    this.availableJoules -= this.joulesPerLaunch;
    this.UpdateCharge();
  }

  private void CreateNewWaitReactable()
  {
    if (this.wait_reactable != null)
      return;
    this.wait_reactable = new TravelTubeEntrance.WaitReactable(this);
  }

  private void OrphanWaitReactable() => this.wait_reactable = (TravelTubeEntrance.WaitReactable) null;

  private void ClearWaitReactable()
  {
    if (this.wait_reactable == null)
      return;
    this.wait_reactable.Cleanup();
    this.wait_reactable = (TravelTubeEntrance.WaitReactable) null;
  }

  private void OnOperationalChanged(object data)
  {
    bool operational = (bool) data;
    Grid.SetTubeEntranceOperational(Grid.PosToCell((KMonoBehaviour) this), operational);
    this.UpdateActive();
  }

  private void OnConnectionChanged()
  {
    this.UpdateActive();
    this.UpdateConnectionStatus();
  }

  private void UpdateActive() => this.operational.SetActive(this.CanAcceptMorePower());

  private void UpdateCharge()
  {
    this.smi.sm.hasLaunchCharges.Set(this.HasLaunchPower, this.smi);
    this.meter.SetPositionPercent(Mathf.Clamp01(this.availableJoules / this.jouleCapacity));
    this.energyConsumer.UpdatePoweredStatus();
    Grid.SetTubeEntranceReservationCapacity(Grid.PosToCell((KMonoBehaviour) this), Mathf.FloorToInt(this.availableJoules / this.joulesPerLaunch));
  }

  private void UpdateConnectionStatus()
  {
    int num = !Object.op_Inequality((Object) this.button, (Object) null) ? 0 : (!this.button.IsEnabled ? 1 : 0);
    bool isConnected = this.energyConsumer.IsConnected;
    bool hasLaunchPower = this.HasLaunchPower;
    if (((num != 0 ? 1 : (!isConnected ? 1 : 0)) | (hasLaunchPower ? 1 : 0)) != 0)
    {
      this.connectedStatus = this.selectable.RemoveStatusItem(this.connectedStatus);
    }
    else
    {
      if (!(this.connectedStatus == Guid.Empty))
        return;
      this.connectedStatus = this.selectable.AddStatusItem(Db.Get().BuildingStatusItems.NotEnoughPower);
    }
  }

  private class LaunchReactable : WorkableReactable
  {
    private TravelTubeEntrance entrance;

    public LaunchReactable(Workable workable, TravelTubeEntrance entrance)
      : base(workable, HashedString.op_Implicit(nameof (LaunchReactable)), Db.Get().ChoreTypes.TravelTubeEntrance)
    {
      this.entrance = entrance;
    }

    public override bool InternalCanBegin(
      GameObject new_reactor,
      Navigator.ActiveTransition transition)
    {
      if (!base.InternalCanBegin(new_reactor, transition))
        return false;
      Navigator component = new_reactor.GetComponent<Navigator>();
      return Object.op_Implicit((Object) component) && this.entrance.HasChargeSlotReserved(component);
    }
  }

  private class WaitReactable : Reactable
  {
    private TravelTubeEntrance entrance;

    public WaitReactable(TravelTubeEntrance entrance)
      : base(((Component) entrance).gameObject, HashedString.op_Implicit(nameof (WaitReactable)), Db.Get().ChoreTypes.TravelTubeEntrance, 2, 1)
    {
      this.entrance = entrance;
      this.preventChoreInterruption = false;
    }

    public override bool InternalCanBegin(
      GameObject new_reactor,
      Navigator.ActiveTransition transition)
    {
      if (Object.op_Inequality((Object) this.reactor, (Object) null))
        return false;
      if (!Object.op_Equality((Object) this.entrance, (Object) null))
        return this.entrance.ShouldWait(new_reactor);
      this.Cleanup();
      return false;
    }

    protected override void InternalBegin()
    {
      KBatchedAnimController component = this.reactor.GetComponent<KBatchedAnimController>();
      component.AddAnimOverrides(Assets.GetAnim(HashedString.op_Implicit("anim_idle_distracted_kanim")), 1f);
      component.Play(HashedString.op_Implicit("idle_pre"));
      component.Queue(HashedString.op_Implicit("idle_default"), (KAnim.PlayMode) 0);
      this.entrance.OrphanWaitReactable();
      this.entrance.CreateNewWaitReactable();
    }

    public override void Update(float dt)
    {
      if (Object.op_Equality((Object) this.entrance, (Object) null))
      {
        this.Cleanup();
      }
      else
      {
        if (this.entrance.ShouldWait(this.reactor))
          return;
        this.Cleanup();
      }
    }

    protected override void InternalEnd()
    {
      if (!Object.op_Inequality((Object) this.reactor, (Object) null))
        return;
      this.reactor.GetComponent<KBatchedAnimController>().RemoveAnimOverrides(Assets.GetAnim(HashedString.op_Implicit("anim_idle_distracted_kanim")));
    }

    protected override void InternalCleanup()
    {
    }
  }

  public class SMInstance : 
    GameStateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance, object>.GameInstance
  {
    public SMInstance(TravelTubeEntrance master)
      : base(master)
    {
    }
  }

  public class States : 
    GameStateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance>
  {
    public StateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance, object>.BoolParameter hasLaunchCharges;
    public TravelTubeEntrance.States.NotOperationalStates notoperational;
    public GameStateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance, object>.State notready;
    public TravelTubeEntrance.States.ReadyStates ready;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.notoperational;
      this.root.ToggleStatusItem(Db.Get().BuildingStatusItems.StoredCharge);
      this.notoperational.DefaultState(this.notoperational.normal).PlayAnim("off").TagTransition(GameTags.Operational, (GameStateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance, object>.State) this.ready);
      this.notoperational.normal.EventTransition(GameHashes.OperationalFlagChanged, this.notoperational.notube, (StateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance, object>.Transition.ConditionCallback) (smi => !smi.master.operational.GetFlag(TravelTubeEntrance.tubeConnected)));
      this.notoperational.notube.EventTransition(GameHashes.OperationalFlagChanged, this.notoperational.normal, (StateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance, object>.Transition.ConditionCallback) (smi => smi.master.operational.GetFlag(TravelTubeEntrance.tubeConnected))).ToggleStatusItem(Db.Get().BuildingStatusItems.NoTubeConnected);
      this.notready.PlayAnim("off").ParamTransition<bool>((StateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance, object>.Parameter<bool>) this.hasLaunchCharges, (GameStateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance, object>.State) this.ready, (StateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance, object>.Parameter<bool>.Callback) ((smi, hasLaunchCharges) => hasLaunchCharges)).TagTransition(GameTags.Operational, (GameStateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance, object>.State) this.notoperational, true);
      this.ready.DefaultState(this.ready.free).ToggleReactable((Func<TravelTubeEntrance.SMInstance, Reactable>) (smi => (Reactable) new TravelTubeEntrance.LaunchReactable((Workable) ((Component) smi.master).GetComponent<TravelTubeEntrance.Work>(), ((Component) smi.master).GetComponent<TravelTubeEntrance>()))).ParamTransition<bool>((StateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance, object>.Parameter<bool>) this.hasLaunchCharges, this.notready, (StateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance, object>.Parameter<bool>.Callback) ((smi, hasLaunchCharges) => !hasLaunchCharges)).TagTransition(GameTags.Operational, (GameStateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance, object>.State) this.notoperational, true);
      this.ready.free.PlayAnim("on").WorkableStartTransition((Func<TravelTubeEntrance.SMInstance, Workable>) (smi => (Workable) smi.GetComponent<TravelTubeEntrance.Work>()), this.ready.occupied);
      this.ready.occupied.PlayAnim("working_pre").QueueAnim("working_loop", true).WorkableStopTransition((Func<TravelTubeEntrance.SMInstance, Workable>) (smi => (Workable) smi.GetComponent<TravelTubeEntrance.Work>()), this.ready.post);
      this.ready.post.PlayAnim("working_pst").OnAnimQueueComplete((GameStateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance, object>.State) this.ready);
    }

    public class NotOperationalStates : 
      GameStateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance, object>.State
    {
      public GameStateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance, object>.State normal;
      public GameStateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance, object>.State notube;
    }

    public class ReadyStates : 
      GameStateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance, object>.State
    {
      public GameStateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance, object>.State free;
      public GameStateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance, object>.State occupied;
      public GameStateMachine<TravelTubeEntrance.States, TravelTubeEntrance.SMInstance, TravelTubeEntrance, object>.State post;
    }
  }

  [AddComponentMenu("KMonoBehaviour/Workable/Work")]
  public class Work : Workable, IGameObjectEffectDescriptor
  {
    protected override void OnPrefabInit()
    {
      base.OnPrefabInit();
      this.resetProgressOnStop = true;
      this.showProgressBar = false;
      this.overrideAnims = new KAnimFile[1]
      {
        Assets.GetAnim(HashedString.op_Implicit("anim_interacts_tube_launcher_kanim"))
      };
      this.workLayer = Grid.SceneLayer.BuildingUse;
    }

    protected override void OnStartWork(Worker worker) => this.SetWorkTime(1f);
  }
}
