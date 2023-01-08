// Decompiled with JetBrains decompiler
// Type: MaskStation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class MaskStation : StateMachineComponent<MaskStation.SMInstance>, IBasicBuilding
{
  private static readonly EventSystem.IntraObjectHandler<MaskStation> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<MaskStation>((Action<MaskStation, object>) ((component, data) => component.OnStorageChange(data)));
  private static readonly EventSystem.IntraObjectHandler<MaskStation> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<MaskStation>((Action<MaskStation, object>) ((component, data) => component.OnOperationalChanged((bool) data)));
  private static readonly EventSystem.IntraObjectHandler<MaskStation> OnRotatedDelegate = new EventSystem.IntraObjectHandler<MaskStation>((Action<MaskStation, object>) ((component, data) => component.isRotated = ((Rotatable) data).IsRotated));
  public float materialConsumedPerMask = 1f;
  public float oxygenConsumedPerMask = 1f;
  public Tag materialTag = GameTags.Metal;
  public Tag oxygenTag = GameTags.Breathable;
  public int maxUses = 10;
  [MyCmpReq]
  private Operational operational;
  [MyCmpGet]
  private KSelectable selectable;
  public Storage materialStorage;
  public Storage oxygenStorage;
  private bool shouldPump;
  private MaskStation.OxygenMaskReactable reactable;
  private MeterController materialsMeter;
  private MeterController oxygenMeter;
  public Meter.Offset materialsMeterOffset = Meter.Offset.Behind;
  public Meter.Offset oxygenMeterOffset;
  public string choreTypeID;
  protected FilteredStorage filteredStorage;
  public KAnimFile interactAnim = Assets.GetAnim(HashedString.op_Implicit("anim_equip_clothing_kanim"));
  private int cell;
  public PathFinder.PotentialPath.Flags PathFlag;
  private Guid noElementStatusGuid;
  private Grid.SuitMarker.Flags gridFlags;

  private bool isRotated
  {
    get => (this.gridFlags & Grid.SuitMarker.Flags.Rotated) != 0;
    set => this.UpdateGridFlag(Grid.SuitMarker.Flags.Rotated, value);
  }

  private bool isOperational
  {
    get => (this.gridFlags & Grid.SuitMarker.Flags.Operational) != 0;
    set => this.UpdateGridFlag(Grid.SuitMarker.Flags.Operational, value);
  }

  public void UpdateOperational()
  {
    bool flag = (double) this.GetTotalOxygenAmount() >= (double) this.oxygenConsumedPerMask * (double) this.maxUses;
    this.shouldPump = this.IsPumpable();
    if (this.operational.IsOperational && this.shouldPump && !flag)
      this.operational.SetActive(true);
    else
      this.operational.SetActive(false);
    this.noElementStatusGuid = this.selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.InvalidMaskStationConsumptionState, this.noElementStatusGuid, !this.shouldPump);
  }

  private bool IsPumpable()
  {
    ElementConsumer[] components = ((Component) this).GetComponents<ElementConsumer>();
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
    bool flag = false;
    foreach (ElementConsumer elementConsumer in components)
    {
      for (int index1 = 0; index1 < (int) elementConsumer.consumptionRadius; ++index1)
      {
        for (int index2 = 0; index2 < (int) elementConsumer.consumptionRadius; ++index2)
        {
          int index3 = cell + index2 + Grid.WidthInCells * index1;
          if (Grid.Element[index3].IsState(Element.State.Gas) & Grid.Element[index3].id == elementConsumer.elementToConsume)
            flag = true;
        }
      }
    }
    return flag;
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.filteredStorage = new FilteredStorage((KMonoBehaviour) this, (Tag[]) null, (IUserControlledCapacity) null, false, Db.Get().ChoreTypes.Get(this.choreTypeID));
  }

  private List<GameObject> GetPossibleMaterials()
  {
    List<GameObject> result = new List<GameObject>();
    this.materialStorage.Find(this.materialTag, result);
    return result;
  }

  private float GetTotalMaterialAmount() => this.materialStorage.GetMassAvailable(this.materialTag);

  private float GetTotalOxygenAmount() => this.oxygenStorage.GetMassAvailable(this.oxygenTag);

  private void RefreshMeters()
  {
    float percent_full1 = Mathf.Clamp01(this.GetTotalMaterialAmount() / ((float) this.maxUses * this.materialConsumedPerMask));
    float percent_full2 = Mathf.Clamp01(this.GetTotalOxygenAmount() / ((float) this.maxUses * this.oxygenConsumedPerMask));
    this.materialsMeter.SetPositionPercent(percent_full1);
    this.oxygenMeter.SetPositionPercent(percent_full2);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
    this.CreateNewReactable();
    this.cell = Grid.PosToCell((KMonoBehaviour) this);
    Grid.RegisterSuitMarker(this.cell);
    this.isOperational = ((Component) this).GetComponent<Operational>().IsOperational;
    this.Subscribe<MaskStation>(-592767678, MaskStation.OnOperationalChangedDelegate);
    this.isRotated = ((Component) this).GetComponent<Rotatable>().IsRotated;
    this.Subscribe<MaskStation>(-1643076535, MaskStation.OnRotatedDelegate);
    this.materialsMeter = new MeterController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "meter_resources_target", "meter_resources", this.materialsMeterOffset, Grid.SceneLayer.BuildingBack, new string[1]
    {
      "meter_resources_target"
    });
    this.oxygenMeter = new MeterController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "meter_oxygen_target", "meter_oxygen", this.oxygenMeterOffset, Grid.SceneLayer.BuildingFront, new string[1]
    {
      "meter_oxygen_target"
    });
    if (this.filteredStorage != null)
      this.filteredStorage.FilterChanged();
    this.Subscribe<MaskStation>(-1697596308, MaskStation.OnStorageChangeDelegate);
    this.RefreshMeters();
  }

  private void Update() => Grid.UpdateSuitMarker(this.cell, (int) Mathf.Min(this.GetTotalMaterialAmount() / this.materialConsumedPerMask, this.GetTotalOxygenAmount() / this.oxygenConsumedPerMask), 0, this.gridFlags, this.PathFlag);

  protected override void OnCleanUp()
  {
    if (this.filteredStorage != null)
      this.filteredStorage.CleanUp();
    if (this.isSpawned)
      Grid.UnregisterSuitMarker(this.cell);
    if (this.reactable != null)
      this.reactable.Cleanup();
    base.OnCleanUp();
  }

  private void OnOperationalChanged(bool isOperational) => this.isOperational = isOperational;

  private void OnStorageChange(object data) => this.RefreshMeters();

  private void UpdateGridFlag(Grid.SuitMarker.Flags flag, bool state)
  {
    if (state)
      this.gridFlags |= flag;
    else
      this.gridFlags &= ~flag;
  }

  private void CreateNewReactable() => this.reactable = new MaskStation.OxygenMaskReactable(this);

  private class OxygenMaskReactable : Reactable
  {
    private MaskStation maskStation;
    private float startTime;

    public OxygenMaskReactable(MaskStation mask_station)
      : base(((Component) mask_station).gameObject, HashedString.op_Implicit("OxygenMask"), Db.Get().ChoreTypes.SuitMarker, 1, 1)
    {
      this.maskStation = mask_station;
    }

    public override bool InternalCanBegin(
      GameObject new_reactor,
      Navigator.ActiveTransition transition)
    {
      if (Object.op_Inequality((Object) this.reactor, (Object) null))
        return false;
      if (Object.op_Equality((Object) this.maskStation, (Object) null))
      {
        this.Cleanup();
        return false;
      }
      bool flag = !new_reactor.GetComponent<MinionIdentity>().GetEquipment().IsSlotOccupied(Db.Get().AssignableSlots.Suit);
      int x = transition.navGridTransition.x;
      if (x == 0)
        return false;
      return !flag ? (x >= 0 || !this.maskStation.isRotated) && (x <= 0 || this.maskStation.isRotated) : this.maskStation.smi.IsReady() && (x <= 0 || !this.maskStation.isRotated) && (x >= 0 || this.maskStation.isRotated);
    }

    protected override void InternalBegin()
    {
      this.startTime = Time.time;
      KBatchedAnimController component = this.reactor.GetComponent<KBatchedAnimController>();
      component.AddAnimOverrides(this.maskStation.interactAnim, 1f);
      component.Play(HashedString.op_Implicit("working_pre"));
      component.Queue(HashedString.op_Implicit("working_loop"));
      component.Queue(HashedString.op_Implicit("working_pst"));
      this.maskStation.CreateNewReactable();
    }

    public override void Update(float dt)
    {
      Facing facing = Object.op_Implicit((Object) this.reactor) ? this.reactor.GetComponent<Facing>() : (Facing) null;
      if (Object.op_Implicit((Object) facing) && Object.op_Implicit((Object) this.maskStation))
        facing.SetFacing(((Component) this.maskStation).GetComponent<Rotatable>().GetOrientation() == Orientation.FlipH);
      if ((double) Time.time - (double) this.startTime <= 2.7999999523162842)
        return;
      this.Run();
      this.Cleanup();
    }

    private void Run()
    {
      GameObject reactor = this.reactor;
      Equipment equipment = reactor.GetComponent<MinionIdentity>().GetEquipment();
      bool flag1 = !equipment.IsSlotOccupied(Db.Get().AssignableSlots.Suit);
      Navigator component1 = reactor.GetComponent<Navigator>();
      bool flag2 = Object.op_Inequality((Object) component1, (Object) null) && (component1.flags & this.maskStation.PathFlag) != 0;
      if (flag1)
      {
        if (!this.maskStation.smi.IsReady())
          return;
        GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(TagExtensions.ToTag("Oxygen_Mask")), (GameObject) null, (string) null);
        gameObject.SetActive(true);
        SimHashes elementId = this.maskStation.GetPossibleMaterials()[0].GetComponent<PrimaryElement>().ElementID;
        gameObject.GetComponent<PrimaryElement>().SetElement(elementId, false);
        SuitTank component2 = gameObject.GetComponent<SuitTank>();
        this.maskStation.materialStorage.ConsumeIgnoringDisease(this.maskStation.materialTag, this.maskStation.materialConsumedPerMask);
        double num = (double) this.maskStation.oxygenStorage.Transfer(component2.storage, component2.elementTag, this.maskStation.oxygenConsumedPerMask, hide_popups: true);
        Equippable component3 = gameObject.GetComponent<Equippable>();
        component3.Assign(((Component) equipment).GetComponent<IAssignableIdentity>());
        component3.isEquipped = true;
      }
      if (flag1)
        return;
      Assignable assignable = equipment.GetAssignable(Db.Get().AssignableSlots.Suit);
      assignable.Unassign();
      if (flag2)
        return;
      Notification notification = new Notification((string) MISC.NOTIFICATIONS.SUIT_DROPPED.NAME, NotificationType.BadMinor, (Func<List<Notification>, object, string>) ((notificationList, data) => (string) MISC.NOTIFICATIONS.SUIT_DROPPED.TOOLTIP));
      ((Component) assignable).GetComponent<Notifier>().Add(notification);
    }

    protected override void InternalEnd()
    {
      if (!Object.op_Inequality((Object) this.reactor, (Object) null))
        return;
      this.reactor.GetComponent<KBatchedAnimController>().RemoveAnimOverrides(this.maskStation.interactAnim);
    }

    protected override void InternalCleanup()
    {
    }
  }

  public class SMInstance : 
    GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.GameInstance
  {
    public SMInstance(MaskStation master)
      : base(master)
    {
    }

    private bool HasSufficientMaterials() => (double) this.master.GetTotalMaterialAmount() >= (double) this.master.materialConsumedPerMask;

    private bool HasSufficientOxygen() => (double) this.master.GetTotalOxygenAmount() >= (double) this.master.oxygenConsumedPerMask;

    public bool OxygenIsFull() => (double) this.master.GetTotalOxygenAmount() >= (double) this.master.oxygenConsumedPerMask * (double) this.master.maxUses;

    public bool IsReady() => this.HasSufficientMaterials() && this.HasSufficientOxygen();
  }

  public class States : GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation>
  {
    public GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State notOperational;
    public MaskStation.States.ChargingStates charging;
    public MaskStation.States.NotChargingStates notCharging;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.notOperational;
      this.notOperational.PlayAnim("off").TagTransition(GameTags.Operational, (GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State) this.charging);
      this.charging.TagTransition(GameTags.Operational, this.notOperational, true).EventTransition(GameHashes.OnStorageChange, (GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State) this.notCharging, (StateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.Transition.ConditionCallback) (smi => smi.OxygenIsFull() || !smi.master.shouldPump)).Update((Action<MaskStation.SMInstance, float>) ((smi, dt) => smi.master.UpdateOperational()), (UpdateRate) 6).Enter((StateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State.Callback) (smi =>
      {
        if (smi.OxygenIsFull() || !smi.master.shouldPump)
          smi.GoTo((StateMachine.BaseState) this.notCharging);
        else if (smi.IsReady())
          smi.GoTo((StateMachine.BaseState) this.charging.openChargingPre);
        else
          smi.GoTo((StateMachine.BaseState) this.charging.closedChargingPre);
      }));
      this.charging.opening.QueueAnim("opening_charging").OnAnimQueueComplete(this.charging.open);
      this.charging.open.PlayAnim("open_charging_loop", (KAnim.PlayMode) 0).EventTransition(GameHashes.OnStorageChange, this.charging.closing, (StateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.Transition.ConditionCallback) (smi => !smi.IsReady()));
      this.charging.closing.QueueAnim("closing_charging").OnAnimQueueComplete(this.charging.closed);
      this.charging.closed.PlayAnim("closed_charging_loop", (KAnim.PlayMode) 0).EventTransition(GameHashes.OnStorageChange, this.charging.opening, (StateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.Transition.ConditionCallback) (smi => smi.IsReady()));
      this.charging.openChargingPre.PlayAnim("open_charging_pre").OnAnimQueueComplete(this.charging.open);
      this.charging.closedChargingPre.PlayAnim("closed_charging_pre").OnAnimQueueComplete(this.charging.closed);
      this.notCharging.TagTransition(GameTags.Operational, this.notOperational, true).EventTransition(GameHashes.OnStorageChange, (GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State) this.charging, (StateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.Transition.ConditionCallback) (smi => !smi.OxygenIsFull() && smi.master.shouldPump)).Update((Action<MaskStation.SMInstance, float>) ((smi, dt) => smi.master.UpdateOperational()), (UpdateRate) 6).Enter((StateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State.Callback) (smi =>
      {
        if (!smi.OxygenIsFull() && smi.master.shouldPump)
          smi.GoTo((StateMachine.BaseState) this.charging);
        else if (smi.IsReady())
          smi.GoTo((StateMachine.BaseState) this.notCharging.openChargingPst);
        else
          smi.GoTo((StateMachine.BaseState) this.notCharging.closedChargingPst);
      }));
      this.notCharging.opening.PlayAnim("opening_not_charging").OnAnimQueueComplete(this.notCharging.open);
      this.notCharging.open.PlayAnim("open_not_charging_loop").EventTransition(GameHashes.OnStorageChange, this.notCharging.closing, (StateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.Transition.ConditionCallback) (smi => !smi.IsReady()));
      this.notCharging.closing.PlayAnim("closing_not_charging").OnAnimQueueComplete(this.notCharging.closed);
      this.notCharging.closed.PlayAnim("closed_not_charging_loop").EventTransition(GameHashes.OnStorageChange, this.notCharging.opening, (StateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.Transition.ConditionCallback) (smi => smi.IsReady()));
      this.notCharging.openChargingPst.PlayAnim("open_charging_pst").OnAnimQueueComplete(this.notCharging.open);
      this.notCharging.closedChargingPst.PlayAnim("closed_charging_pst").OnAnimQueueComplete(this.notCharging.closed);
    }

    public class ChargingStates : 
      GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State
    {
      public GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State opening;
      public GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State open;
      public GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State closing;
      public GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State closed;
      public GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State openChargingPre;
      public GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State closedChargingPre;
    }

    public class NotChargingStates : 
      GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State
    {
      public GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State opening;
      public GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State open;
      public GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State closing;
      public GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State closed;
      public GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State openChargingPst;
      public GameStateMachine<MaskStation.States, MaskStation.SMInstance, MaskStation, object>.State closedChargingPst;
    }
  }
}
