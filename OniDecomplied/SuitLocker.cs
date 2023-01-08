// Decompiled with JetBrains decompiler
// Type: SuitLocker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SuitLocker : StateMachineComponent<SuitLocker.StatesInstance>
{
  [MyCmpGet]
  private Building building;
  public Tag[] OutfitTags;
  private FetchChore fetchChore;
  [MyCmpAdd]
  public SuitLocker.ReturnSuitWorkable returnSuitWorkable;
  private MeterController meter;
  private SuitLocker.SuitMarkerState suitMarkerState;

  public float OxygenAvailable
  {
    get
    {
      KPrefabID storedOutfit = this.GetStoredOutfit();
      return Object.op_Equality((Object) storedOutfit, (Object) null) ? 0.0f : ((Component) storedOutfit).GetComponent<SuitTank>().PercentFull();
    }
  }

  public float BatteryAvailable
  {
    get
    {
      KPrefabID storedOutfit = this.GetStoredOutfit();
      return Object.op_Equality((Object) storedOutfit, (Object) null) ? 0.0f : ((Component) storedOutfit).GetComponent<LeadSuitTank>().batteryCharge;
    }
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.meter = new MeterController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[3]
    {
      "meter_target",
      "meter_arrow",
      "meter_scale"
    });
    SuitLocker.UpdateSuitMarkerStates(Grid.PosToCell(this.transform.position), ((Component) this).gameObject);
    this.smi.StartSM();
    Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Suits);
  }

  public KPrefabID GetStoredOutfit()
  {
    foreach (GameObject gameObject in ((Component) this).GetComponent<Storage>().items)
    {
      if (!Object.op_Equality((Object) gameObject, (Object) null))
      {
        KPrefabID component = gameObject.GetComponent<KPrefabID>();
        if (!Object.op_Equality((Object) component, (Object) null) && component.IsAnyPrefabID(this.OutfitTags))
          return component;
      }
    }
    return (KPrefabID) null;
  }

  public float GetSuitScore()
  {
    float suitScore = -1f;
    KPrefabID partiallyChargedOutfit = this.GetPartiallyChargedOutfit();
    if (Object.op_Implicit((Object) partiallyChargedOutfit))
    {
      suitScore = ((Component) partiallyChargedOutfit).GetComponent<SuitTank>().PercentFull();
      JetSuitTank component = ((Component) partiallyChargedOutfit).GetComponent<JetSuitTank>();
      if (Object.op_Implicit((Object) component) && (double) component.PercentFull() < (double) suitScore)
        suitScore = component.PercentFull();
    }
    return suitScore;
  }

  public KPrefabID GetPartiallyChargedOutfit()
  {
    KPrefabID storedOutfit = this.GetStoredOutfit();
    if (!Object.op_Implicit((Object) storedOutfit))
      return (KPrefabID) null;
    if ((double) ((Component) storedOutfit).GetComponent<SuitTank>().PercentFull() < (double) TUNING.EQUIPMENT.SUITS.MINIMUM_USABLE_SUIT_CHARGE)
      return (KPrefabID) null;
    JetSuitTank component = ((Component) storedOutfit).GetComponent<JetSuitTank>();
    return Object.op_Implicit((Object) component) && (double) component.PercentFull() < (double) TUNING.EQUIPMENT.SUITS.MINIMUM_USABLE_SUIT_CHARGE ? (KPrefabID) null : storedOutfit;
  }

  public KPrefabID GetFullyChargedOutfit()
  {
    KPrefabID storedOutfit = this.GetStoredOutfit();
    if (!Object.op_Implicit((Object) storedOutfit))
      return (KPrefabID) null;
    if (!((Component) storedOutfit).GetComponent<SuitTank>().IsFull())
      return (KPrefabID) null;
    JetSuitTank component = ((Component) storedOutfit).GetComponent<JetSuitTank>();
    return Object.op_Implicit((Object) component) && !component.IsFull() ? (KPrefabID) null : storedOutfit;
  }

  private void CreateFetchChore()
  {
    this.fetchChore = new FetchChore(Db.Get().ChoreTypes.EquipmentFetch, ((Component) this).GetComponent<Storage>(), 1f, new HashSet<Tag>((IEnumerable<Tag>) this.OutfitTags), FetchChore.MatchCriteria.MatchID, Tag.Invalid, new Tag[1]
    {
      GameTags.Assigned
    }, operational_requirement: Operational.State.None);
    this.fetchChore.allowMultifetch = false;
  }

  private void CancelFetchChore()
  {
    if (this.fetchChore == null)
      return;
    this.fetchChore.Cancel("SuitLocker.CancelFetchChore");
    this.fetchChore = (FetchChore) null;
  }

  public bool HasOxygen()
  {
    GameObject oxygen = this.GetOxygen();
    return Object.op_Inequality((Object) oxygen, (Object) null) && (double) oxygen.GetComponent<PrimaryElement>().Mass > 0.0;
  }

  private void RefreshMeter()
  {
    GameObject oxygen = this.GetOxygen();
    float percent_full = 0.0f;
    if (Object.op_Inequality((Object) oxygen, (Object) null))
      percent_full = Math.Min(oxygen.GetComponent<PrimaryElement>().Mass / ((Component) this).GetComponent<ConduitConsumer>().capacityKG, 1f);
    this.meter.SetPositionPercent(percent_full);
  }

  public bool IsSuitFullyCharged()
  {
    KPrefabID storedOutfit = this.GetStoredOutfit();
    if (!Object.op_Inequality((Object) storedOutfit, (Object) null))
      return false;
    SuitTank component1 = ((Component) storedOutfit).GetComponent<SuitTank>();
    if (Object.op_Inequality((Object) component1, (Object) null) && (double) component1.PercentFull() < 1.0)
      return false;
    JetSuitTank component2 = ((Component) storedOutfit).GetComponent<JetSuitTank>();
    if (Object.op_Inequality((Object) component2, (Object) null) && (double) component2.PercentFull() < 1.0)
      return false;
    LeadSuitTank leadSuitTank = Object.op_Inequality((Object) storedOutfit, (Object) null) ? ((Component) storedOutfit).GetComponent<LeadSuitTank>() : (LeadSuitTank) null;
    return !Object.op_Inequality((Object) leadSuitTank, (Object) null) || (double) leadSuitTank.PercentFull() >= 1.0;
  }

  public bool IsOxygenTankFull()
  {
    KPrefabID storedOutfit = this.GetStoredOutfit();
    if (!Object.op_Inequality((Object) storedOutfit, (Object) null))
      return false;
    SuitTank component = ((Component) storedOutfit).GetComponent<SuitTank>();
    return Object.op_Equality((Object) component, (Object) null) || (double) component.PercentFull() >= 1.0;
  }

  private void OnRequestOutfit() => this.smi.sm.isWaitingForSuit.Set(true, this.smi);

  private void OnCancelRequest() => this.smi.sm.isWaitingForSuit.Set(false, this.smi);

  public void DropSuit()
  {
    KPrefabID storedOutfit = this.GetStoredOutfit();
    if (Object.op_Equality((Object) storedOutfit, (Object) null))
      return;
    ((Component) this).GetComponent<Storage>().Drop(((Component) storedOutfit).gameObject, true);
  }

  public void EquipTo(Equipment equipment)
  {
    KPrefabID storedOutfit = this.GetStoredOutfit();
    if (Object.op_Equality((Object) storedOutfit, (Object) null))
      return;
    ((Component) this).GetComponent<Storage>().Drop(((Component) storedOutfit).gameObject, true);
    ((Component) storedOutfit).GetComponent<Equippable>().Assign(((Component) equipment).GetComponent<IAssignableIdentity>());
    ((Component) storedOutfit).GetComponent<EquippableWorkable>().CancelChore("Manual equip");
    equipment.Equip(((Component) storedOutfit).GetComponent<Equippable>());
    this.returnSuitWorkable.CreateChore();
  }

  public void UnequipFrom(Equipment equipment)
  {
    Assignable assignable = equipment.GetAssignable(Db.Get().AssignableSlots.Suit);
    assignable.Unassign();
    ((Component) this).GetComponent<Storage>().Store(((Component) assignable).gameObject);
    Durability component = ((Component) assignable).GetComponent<Durability>();
    if (!Object.op_Inequality((Object) component, (Object) null) || !component.IsWornOut())
      return;
    this.ConfigRequestSuit();
  }

  public void ConfigRequestSuit()
  {
    this.smi.sm.isConfigured.Set(true, this.smi);
    this.smi.sm.isWaitingForSuit.Set(true, this.smi);
  }

  public void ConfigNoSuit()
  {
    this.smi.sm.isConfigured.Set(true, this.smi);
    this.smi.sm.isWaitingForSuit.Set(false, this.smi);
  }

  public bool CanDropOffSuit() => this.smi.sm.isConfigured.Get(this.smi) && !this.smi.sm.isWaitingForSuit.Get(this.smi) && Object.op_Equality((Object) this.GetStoredOutfit(), (Object) null);

  private GameObject GetOxygen() => ((Component) this).GetComponent<Storage>().FindFirst(GameTags.Oxygen);

  private void ChargeSuit(float dt)
  {
    KPrefabID storedOutfit = this.GetStoredOutfit();
    if (Object.op_Equality((Object) storedOutfit, (Object) null))
      return;
    GameObject oxygen = this.GetOxygen();
    if (Object.op_Equality((Object) oxygen, (Object) null))
      return;
    SuitTank component = ((Component) storedOutfit).GetComponent<SuitTank>();
    float num1 = Mathf.Min((float) ((double) component.capacity * 15.0 * (double) dt / 600.0), component.capacity - component.GetTankAmount());
    float amount = Mathf.Min(oxygen.GetComponent<PrimaryElement>().Mass, num1);
    if ((double) amount <= 0.0)
      return;
    double num2 = (double) ((Component) this).GetComponent<Storage>().Transfer(component.storage, component.elementTag, amount, hide_popups: true);
  }

  public void SetSuitMarker(SuitMarker suit_marker)
  {
    SuitLocker.SuitMarkerState suitMarkerState = SuitLocker.SuitMarkerState.HasMarker;
    if (Object.op_Equality((Object) suit_marker, (Object) null))
      suitMarkerState = SuitLocker.SuitMarkerState.NoMarker;
    else if ((double) TransformExtensions.GetPosition(suit_marker.transform).x > (double) TransformExtensions.GetPosition(this.transform).x && ((Component) suit_marker).GetComponent<Rotatable>().IsRotated)
      suitMarkerState = SuitLocker.SuitMarkerState.WrongSide;
    else if ((double) TransformExtensions.GetPosition(suit_marker.transform).x < (double) TransformExtensions.GetPosition(this.transform).x && !((Component) suit_marker).GetComponent<Rotatable>().IsRotated)
      suitMarkerState = SuitLocker.SuitMarkerState.WrongSide;
    else if (!((Component) suit_marker).GetComponent<Operational>().IsOperational)
      suitMarkerState = SuitLocker.SuitMarkerState.NotOperational;
    if (suitMarkerState == this.suitMarkerState)
      return;
    ((Component) this).GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.NoSuitMarker);
    ((Component) this).GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.SuitMarkerWrongSide);
    switch (suitMarkerState)
    {
      case SuitLocker.SuitMarkerState.NoMarker:
        ((Component) this).GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.NoSuitMarker);
        break;
      case SuitLocker.SuitMarkerState.WrongSide:
        ((Component) this).GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.SuitMarkerWrongSide);
        break;
    }
    this.suitMarkerState = suitMarkerState;
  }

  protected override void OnCleanUp()
  {
    base.OnCleanUp();
    SuitLocker.UpdateSuitMarkerStates(Grid.PosToCell(this.transform.position), (GameObject) null);
  }

  private static void GatherSuitBuildings(
    int cell,
    int dir,
    List<SuitLocker.SuitLockerEntry> suit_lockers,
    List<SuitLocker.SuitMarkerEntry> suit_markers)
  {
    int x = dir;
    while (true)
    {
      int cell1 = Grid.OffsetCell(cell, x, 0);
      if (!Grid.IsValidCell(cell1) || SuitLocker.GatherSuitBuildingsOnCell(cell1, suit_lockers, suit_markers))
        x += dir;
      else
        break;
    }
  }

  private static bool GatherSuitBuildingsOnCell(
    int cell,
    List<SuitLocker.SuitLockerEntry> suit_lockers,
    List<SuitLocker.SuitMarkerEntry> suit_markers)
  {
    GameObject gameObject = Grid.Objects[cell, 1];
    if (Object.op_Equality((Object) gameObject, (Object) null))
      return false;
    SuitMarker component1 = gameObject.GetComponent<SuitMarker>();
    if (Object.op_Inequality((Object) component1, (Object) null))
    {
      suit_markers.Add(new SuitLocker.SuitMarkerEntry()
      {
        suitMarker = component1,
        cell = cell
      });
      return true;
    }
    SuitLocker component2 = gameObject.GetComponent<SuitLocker>();
    if (!Object.op_Inequality((Object) component2, (Object) null))
      return false;
    suit_lockers.Add(new SuitLocker.SuitLockerEntry()
    {
      suitLocker = component2,
      cell = cell
    });
    return true;
  }

  private static SuitMarker FindSuitMarker(int cell, List<SuitLocker.SuitMarkerEntry> suit_markers)
  {
    if (!Grid.IsValidCell(cell))
      return (SuitMarker) null;
    foreach (SuitLocker.SuitMarkerEntry suitMarker in suit_markers)
    {
      if (suitMarker.cell == cell)
        return suitMarker.suitMarker;
    }
    return (SuitMarker) null;
  }

  public static void UpdateSuitMarkerStates(int cell, GameObject self)
  {
    ListPool<SuitLocker.SuitLockerEntry, SuitLocker>.PooledList suit_lockers = ListPool<SuitLocker.SuitLockerEntry, SuitLocker>.Allocate();
    ListPool<SuitLocker.SuitMarkerEntry, SuitLocker>.PooledList suit_markers1 = ListPool<SuitLocker.SuitMarkerEntry, SuitLocker>.Allocate();
    if (Object.op_Inequality((Object) self, (Object) null))
    {
      SuitLocker component1 = self.GetComponent<SuitLocker>();
      if (Object.op_Inequality((Object) component1, (Object) null))
        ((List<SuitLocker.SuitLockerEntry>) suit_lockers).Add(new SuitLocker.SuitLockerEntry()
        {
          suitLocker = component1,
          cell = cell
        });
      SuitMarker component2 = self.GetComponent<SuitMarker>();
      if (Object.op_Inequality((Object) component2, (Object) null))
        ((List<SuitLocker.SuitMarkerEntry>) suit_markers1).Add(new SuitLocker.SuitMarkerEntry()
        {
          suitMarker = component2,
          cell = cell
        });
    }
    SuitLocker.GatherSuitBuildings(cell, 1, (List<SuitLocker.SuitLockerEntry>) suit_lockers, (List<SuitLocker.SuitMarkerEntry>) suit_markers1);
    SuitLocker.GatherSuitBuildings(cell, -1, (List<SuitLocker.SuitLockerEntry>) suit_lockers, (List<SuitLocker.SuitMarkerEntry>) suit_markers1);
    ((List<SuitLocker.SuitLockerEntry>) suit_lockers).Sort((IComparer<SuitLocker.SuitLockerEntry>) SuitLocker.SuitLockerEntry.comparer);
    for (int index1 = 0; index1 < ((List<SuitLocker.SuitLockerEntry>) suit_lockers).Count; ++index1)
    {
      SuitLocker.SuitLockerEntry suitLockerEntry1 = ((List<SuitLocker.SuitLockerEntry>) suit_lockers)[index1];
      SuitLocker.SuitLockerEntry suitLockerEntry2 = suitLockerEntry1;
      ListPool<SuitLocker.SuitLockerEntry, SuitLocker>.PooledList pooledList = ListPool<SuitLocker.SuitLockerEntry, SuitLocker>.Allocate();
      ((List<SuitLocker.SuitLockerEntry>) pooledList).Add(suitLockerEntry1);
      for (int index2 = index1 + 1; index2 < ((List<SuitLocker.SuitLockerEntry>) suit_lockers).Count; ++index2)
      {
        SuitLocker.SuitLockerEntry suitLockerEntry3 = ((List<SuitLocker.SuitLockerEntry>) suit_lockers)[index2];
        if (Grid.CellRight(suitLockerEntry2.cell) == suitLockerEntry3.cell)
        {
          ++index1;
          suitLockerEntry2 = suitLockerEntry3;
          ((List<SuitLocker.SuitLockerEntry>) pooledList).Add(suitLockerEntry3);
        }
        else
          break;
      }
      int cell1 = Grid.CellLeft(suitLockerEntry1.cell);
      int cell2 = Grid.CellRight(suitLockerEntry2.cell);
      ListPool<SuitLocker.SuitMarkerEntry, SuitLocker>.PooledList suit_markers2 = suit_markers1;
      SuitMarker suitMarker = SuitLocker.FindSuitMarker(cell1, (List<SuitLocker.SuitMarkerEntry>) suit_markers2);
      if (Object.op_Equality((Object) suitMarker, (Object) null))
        suitMarker = SuitLocker.FindSuitMarker(cell2, (List<SuitLocker.SuitMarkerEntry>) suit_markers1);
      foreach (SuitLocker.SuitLockerEntry suitLockerEntry4 in (List<SuitLocker.SuitLockerEntry>) pooledList)
        suitLockerEntry4.suitLocker.SetSuitMarker(suitMarker);
      pooledList.Recycle();
    }
    suit_lockers.Recycle();
    suit_markers1.Recycle();
  }

  [AddComponentMenu("KMonoBehaviour/Workable/ReturnSuitWorkable")]
  public class ReturnSuitWorkable : Workable
  {
    public static readonly Chore.Precondition DoesSuitNeedRechargingUrgent = new Chore.Precondition()
    {
      id = nameof (DoesSuitNeedRechargingUrgent),
      description = (string) DUPLICANTS.CHORES.PRECONDITIONS.DOES_SUIT_NEED_RECHARGING_URGENT,
      fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
      {
        Equipment equipment = context.consumerState.equipment;
        if (Object.op_Equality((Object) equipment, (Object) null))
          return false;
        AssignableSlotInstance slot = equipment.GetSlot(Db.Get().AssignableSlots.Suit);
        if (Object.op_Equality((Object) slot.assignable, (Object) null))
          return false;
        Equippable component1 = ((Component) slot.assignable).GetComponent<Equippable>();
        if (Object.op_Equality((Object) component1, (Object) null) || !component1.isEquipped)
          return false;
        SuitTank component2 = ((Component) slot.assignable).GetComponent<SuitTank>();
        if (Object.op_Inequality((Object) component2, (Object) null) && component2.NeedsRecharging())
          return true;
        JetSuitTank component3 = ((Component) slot.assignable).GetComponent<JetSuitTank>();
        if (Object.op_Inequality((Object) component3, (Object) null) && component3.NeedsRecharging())
          return true;
        LeadSuitTank component4 = ((Component) slot.assignable).GetComponent<LeadSuitTank>();
        return Object.op_Inequality((Object) component4, (Object) null) && component4.NeedsRecharging();
      })
    };
    public static readonly Chore.Precondition DoesSuitNeedRechargingIdle = new Chore.Precondition()
    {
      id = nameof (DoesSuitNeedRechargingIdle),
      description = (string) DUPLICANTS.CHORES.PRECONDITIONS.DOES_SUIT_NEED_RECHARGING_IDLE,
      fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
      {
        Equipment equipment = context.consumerState.equipment;
        if (Object.op_Equality((Object) equipment, (Object) null))
          return false;
        AssignableSlotInstance slot = equipment.GetSlot(Db.Get().AssignableSlots.Suit);
        if (Object.op_Equality((Object) slot.assignable, (Object) null))
          return false;
        Equippable component = ((Component) slot.assignable).GetComponent<Equippable>();
        return !Object.op_Equality((Object) component, (Object) null) && component.isEquipped && (Object.op_Inequality((Object) ((Component) slot.assignable).GetComponent<SuitTank>(), (Object) null) || Object.op_Inequality((Object) ((Component) slot.assignable).GetComponent<JetSuitTank>(), (Object) null) || Object.op_Inequality((Object) ((Component) slot.assignable).GetComponent<LeadSuitTank>(), (Object) null));
      })
    };
    public Chore.Precondition HasSuitMarker;
    public Chore.Precondition SuitTypeMatchesLocker;
    private WorkChore<SuitLocker.ReturnSuitWorkable> urgentChore;
    private WorkChore<SuitLocker.ReturnSuitWorkable> idleChore;

    protected override void OnPrefabInit()
    {
      base.OnPrefabInit();
      this.resetProgressOnStop = true;
      this.workTime = 0.25f;
      this.synchronizeAnims = false;
    }

    public void CreateChore()
    {
      if (this.urgentChore != null)
        return;
      SuitLocker component = ((Component) this).GetComponent<SuitLocker>();
      this.urgentChore = new WorkChore<SuitLocker.ReturnSuitWorkable>(Db.Get().ChoreTypes.ReturnSuitUrgent, (IStateMachineTarget) this, only_when_operational: false, allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.personalNeeds, add_to_daily_report: false);
      this.urgentChore.AddPrecondition(SuitLocker.ReturnSuitWorkable.DoesSuitNeedRechargingUrgent);
      this.urgentChore.AddPrecondition(this.HasSuitMarker, (object) component);
      this.urgentChore.AddPrecondition(this.SuitTypeMatchesLocker, (object) component);
      this.idleChore = new WorkChore<SuitLocker.ReturnSuitWorkable>(Db.Get().ChoreTypes.ReturnSuitIdle, (IStateMachineTarget) this, only_when_operational: false, allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.idle, add_to_daily_report: false);
      this.idleChore.AddPrecondition(SuitLocker.ReturnSuitWorkable.DoesSuitNeedRechargingIdle);
      this.idleChore.AddPrecondition(this.HasSuitMarker, (object) component);
      this.idleChore.AddPrecondition(this.SuitTypeMatchesLocker, (object) component);
    }

    public void CancelChore()
    {
      if (this.urgentChore != null)
      {
        this.urgentChore.Cancel("ReturnSuitWorkable.CancelChore");
        this.urgentChore = (WorkChore<SuitLocker.ReturnSuitWorkable>) null;
      }
      if (this.idleChore == null)
        return;
      this.idleChore.Cancel("ReturnSuitWorkable.CancelChore");
      this.idleChore = (WorkChore<SuitLocker.ReturnSuitWorkable>) null;
    }

    protected override void OnStartWork(Worker worker) => this.ShowProgressBar(false);

    protected override bool OnWorkTick(Worker worker, float dt) => true;

    protected override void OnCompleteWork(Worker worker)
    {
      Equipment equipment = ((Component) worker).GetComponent<MinionIdentity>().GetEquipment();
      if (equipment.IsSlotOccupied(Db.Get().AssignableSlots.Suit))
      {
        if (((Component) this).GetComponent<SuitLocker>().CanDropOffSuit())
          ((Component) this).GetComponent<SuitLocker>().UnequipFrom(equipment);
        else
          equipment.GetAssignable(Db.Get().AssignableSlots.Suit).Unassign();
      }
      if (this.urgentChore == null)
        return;
      this.CancelChore();
      this.CreateChore();
    }

    public override HashedString[] GetWorkAnims(Worker worker) => new HashedString[1]
    {
      new HashedString("none")
    };

    public ReturnSuitWorkable()
    {
      Chore.Precondition precondition = new Chore.Precondition();
      precondition.id = "IsValid";
      precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.HAS_SUIT_MARKER;
      precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) => ((SuitLocker) data).suitMarkerState == SuitLocker.SuitMarkerState.HasMarker);
      this.HasSuitMarker = precondition;
      precondition = new Chore.Precondition();
      precondition.id = "IsValid";
      precondition.description = (string) DUPLICANTS.CHORES.PRECONDITIONS.HAS_SUIT_MARKER;
      precondition.fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
      {
        SuitLocker suitLocker = (SuitLocker) data;
        Equipment equipment = context.consumerState.equipment;
        if (Object.op_Equality((Object) equipment, (Object) null))
          return false;
        AssignableSlotInstance slot = equipment.GetSlot(Db.Get().AssignableSlots.Suit);
        return !Object.op_Equality((Object) slot.assignable, (Object) null) && ((Component) slot.assignable).GetComponent<KPrefabID>().IsAnyPrefabID(suitLocker.OutfitTags);
      });
      this.SuitTypeMatchesLocker = precondition;
      // ISSUE: explicit constructor call
      base.\u002Ector();
    }
  }

  public class StatesInstance : 
    GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.GameInstance
  {
    public StatesInstance(SuitLocker suit_locker)
      : base(suit_locker)
    {
    }
  }

  public class States : GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker>
  {
    public SuitLocker.States.EmptyStates empty;
    public SuitLocker.States.ChargingStates charging;
    public GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State waitingforsuit;
    public GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State suitfullycharged;
    public StateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.BoolParameter isWaitingForSuit;
    public StateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.BoolParameter isConfigured;
    public StateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.BoolParameter hasSuitMarker;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.empty;
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      this.root.Update("RefreshMeter", (Action<SuitLocker.StatesInstance, float>) ((smi, dt) => smi.master.RefreshMeter()), (UpdateRate) 1);
      this.empty.DefaultState(this.empty.notconfigured).EventTransition(GameHashes.OnStorageChange, (GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State) this.charging, (StateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.Transition.ConditionCallback) (smi => Object.op_Inequality((Object) smi.master.GetStoredOutfit(), (Object) null))).ParamTransition<bool>((StateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.Parameter<bool>) this.isWaitingForSuit, this.waitingforsuit, GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.IsTrue).Enter("CreateReturnSuitChore", (StateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State.Callback) (smi => smi.master.returnSuitWorkable.CreateChore())).RefreshUserMenuOnEnter().Exit("CancelReturnSuitChore", (StateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State.Callback) (smi => smi.master.returnSuitWorkable.CancelChore())).PlayAnim("no_suit_pre").QueueAnim("no_suit");
      this.empty.notconfigured.ParamTransition<bool>((StateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.Parameter<bool>) this.isConfigured, this.empty.configured, GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.IsTrue).ToggleStatusItem((string) BUILDING.STATUSITEMS.SUIT_LOCKER_NEEDS_CONFIGURATION.NAME, (string) BUILDING.STATUSITEMS.SUIT_LOCKER_NEEDS_CONFIGURATION.TOOLTIP, "status_item_no_filter_set", StatusItem.IconType.Custom, NotificationType.BadMinor, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main);
      this.empty.configured.RefreshUserMenuOnEnter().ToggleStatusItem((string) BUILDING.STATUSITEMS.SUIT_LOCKER.READY.NAME, (string) BUILDING.STATUSITEMS.SUIT_LOCKER.READY.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main);
      this.waitingforsuit.EventTransition(GameHashes.OnStorageChange, (GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State) this.charging, (StateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.Transition.ConditionCallback) (smi => Object.op_Inequality((Object) smi.master.GetStoredOutfit(), (Object) null))).Enter("CreateFetchChore", (StateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State.Callback) (smi => smi.master.CreateFetchChore())).ParamTransition<bool>((StateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.Parameter<bool>) this.isWaitingForSuit, (GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State) this.empty, GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.IsFalse).RefreshUserMenuOnEnter().PlayAnim("no_suit_pst").QueueAnim("awaiting_suit").Exit("ClearIsWaitingForSuit", (StateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State.Callback) (smi => this.isWaitingForSuit.Set(false, smi))).Exit("CancelFetchChore", (StateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State.Callback) (smi => smi.master.CancelFetchChore())).ToggleStatusItem((string) BUILDING.STATUSITEMS.SUIT_LOCKER.SUIT_REQUESTED.NAME, (string) BUILDING.STATUSITEMS.SUIT_LOCKER.SUIT_REQUESTED.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main);
      this.charging.DefaultState(this.charging.pre).RefreshUserMenuOnEnter().EventTransition(GameHashes.OnStorageChange, (GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State) this.empty, (StateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.Transition.ConditionCallback) (smi => Object.op_Equality((Object) smi.master.GetStoredOutfit(), (Object) null))).ToggleStatusItem(Db.Get().MiscStatusItems.StoredItemDurability, (Func<SuitLocker.StatesInstance, object>) (smi => (object) ((Component) smi.master.GetStoredOutfit()).gameObject)).Enter((StateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State.Callback) (smi =>
      {
        KAnim.Build.Symbol symbol = ((Component) smi.master.GetStoredOutfit()).GetComponent<KBatchedAnimController>().AnimFiles[0].GetData().build.GetSymbol(KAnimHashedString.op_Implicit("suit"));
        SymbolOverrideController component = smi.GetComponent<SymbolOverrideController>();
        component.TryRemoveSymbolOverride(HashedString.op_Implicit("suit_swap"));
        if (symbol == null)
          return;
        component.AddSymbolOverride(HashedString.op_Implicit("suit_swap"), symbol);
      }));
      this.charging.pre.Enter((StateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State.Callback) (smi =>
      {
        if (smi.master.IsSuitFullyCharged())
        {
          smi.GoTo((StateMachine.BaseState) this.suitfullycharged);
        }
        else
        {
          smi.GetComponent<KBatchedAnimController>().Play(HashedString.op_Implicit("no_suit_pst"));
          smi.GetComponent<KBatchedAnimController>().Queue(HashedString.op_Implicit("charging_pre"));
        }
      })).OnAnimQueueComplete(this.charging.operational);
      this.charging.operational.TagTransition(GameTags.Operational, this.charging.notoperational, true).Transition(this.charging.nooxygen, (StateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.Transition.ConditionCallback) (smi => !smi.master.HasOxygen())).PlayAnim("charging_loop", (KAnim.PlayMode) 0).Enter("SetActive", (StateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State.Callback) (smi => ((Component) smi.master).GetComponent<Operational>().SetActive(true))).Transition(this.charging.pst, (StateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.Transition.ConditionCallback) (smi => smi.master.IsSuitFullyCharged())).Update("ChargeSuit", (Action<SuitLocker.StatesInstance, float>) ((smi, dt) => smi.master.ChargeSuit(dt))).Exit("ClearActive", (StateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State.Callback) (smi => ((Component) smi.master).GetComponent<Operational>().SetActive(false))).ToggleStatusItem((string) BUILDING.STATUSITEMS.SUIT_LOCKER.CHARGING.NAME, (string) BUILDING.STATUSITEMS.SUIT_LOCKER.CHARGING.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main);
      this.charging.nooxygen.TagTransition(GameTags.Operational, this.charging.notoperational, true).Transition(this.charging.operational, (StateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.Transition.ConditionCallback) (smi => smi.master.HasOxygen())).Transition(this.charging.pst, (StateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.Transition.ConditionCallback) (smi => smi.master.IsSuitFullyCharged())).PlayAnim("no_o2_loop", (KAnim.PlayMode) 0).ToggleStatusItem((string) BUILDING.STATUSITEMS.SUIT_LOCKER.NO_OXYGEN.NAME, (string) BUILDING.STATUSITEMS.SUIT_LOCKER.NO_OXYGEN.TOOLTIP, "status_item_suit_locker_no_oxygen", StatusItem.IconType.Custom, NotificationType.BadMinor, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main);
      this.charging.notoperational.TagTransition(GameTags.Operational, this.charging.operational).PlayAnim("not_charging_loop", (KAnim.PlayMode) 0).Transition(this.charging.pst, (StateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.Transition.ConditionCallback) (smi => smi.master.IsSuitFullyCharged())).ToggleStatusItem((string) BUILDING.STATUSITEMS.SUIT_LOCKER.NOT_OPERATIONAL.NAME, (string) BUILDING.STATUSITEMS.SUIT_LOCKER.NOT_OPERATIONAL.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main);
      this.charging.pst.PlayAnim("charging_pst").OnAnimQueueComplete(this.suitfullycharged);
      this.suitfullycharged.EventTransition(GameHashes.OnStorageChange, (GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State) this.empty, (StateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.Transition.ConditionCallback) (smi => Object.op_Equality((Object) smi.master.GetStoredOutfit(), (Object) null))).PlayAnim("has_suit").RefreshUserMenuOnEnter().ToggleStatusItem(Db.Get().MiscStatusItems.StoredItemDurability, (Func<SuitLocker.StatesInstance, object>) (smi => (object) ((Component) smi.master.GetStoredOutfit()).gameObject)).ToggleStatusItem((string) BUILDING.STATUSITEMS.SUIT_LOCKER.FULLY_CHARGED.NAME, (string) BUILDING.STATUSITEMS.SUIT_LOCKER.FULLY_CHARGED.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main);
    }

    public class ChargingStates : 
      GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State
    {
      public GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State pre;
      public GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State pst;
      public GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State operational;
      public GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State nooxygen;
      public GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State notoperational;
    }

    public class EmptyStates : 
      GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State
    {
      public GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State configured;
      public GameStateMachine<SuitLocker.States, SuitLocker.StatesInstance, SuitLocker, object>.State notconfigured;
    }
  }

  private enum SuitMarkerState
  {
    HasMarker,
    NoMarker,
    WrongSide,
    NotOperational,
  }

  private struct SuitLockerEntry
  {
    public SuitLocker suitLocker;
    public int cell;
    public static SuitLocker.SuitLockerEntry.Comparer comparer = new SuitLocker.SuitLockerEntry.Comparer();

    public class Comparer : IComparer<SuitLocker.SuitLockerEntry>
    {
      public int Compare(SuitLocker.SuitLockerEntry a, SuitLocker.SuitLockerEntry b) => a.cell - b.cell;
    }
  }

  private struct SuitMarkerEntry
  {
    public SuitMarker suitMarker;
    public int cell;
  }
}
