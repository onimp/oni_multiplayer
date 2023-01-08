// Decompiled with JetBrains decompiler
// Type: Pickupable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Pickupable")]
public class Pickupable : Workable, IHasSortOrder
{
  [MyCmpReq]
  private PrimaryElement primaryElement;
  public const float WorkTime = 1.5f;
  [SerializeField]
  private int _sortOrder;
  [MyCmpReq]
  [NonSerialized]
  public KPrefabID KPrefabID;
  [MyCmpAdd]
  [NonSerialized]
  public Clearable Clearable;
  [MyCmpAdd]
  [NonSerialized]
  public Prioritizable prioritizable;
  public bool absorbable;
  public Func<Pickupable, bool> CanAbsorb = (Func<Pickupable, bool>) (other => false);
  public Func<float, Pickupable> OnTake;
  public System.Action OnReservationsChanged;
  public ObjectLayerListItem objectLayerListItem;
  public Workable targetWorkable;
  public KAnimFile carryAnimOverride;
  private KBatchedAnimController lastCarrier;
  public bool useGunforPickup = true;
  private static CellOffset[] displacementOffsets = new CellOffset[8]
  {
    new CellOffset(0, 1),
    new CellOffset(0, -1),
    new CellOffset(1, 0),
    new CellOffset(-1, 0),
    new CellOffset(1, 1),
    new CellOffset(1, -1),
    new CellOffset(-1, 1),
    new CellOffset(-1, -1)
  };
  private bool isReachable;
  private bool isEntombed;
  private bool cleaningUp;
  public bool trackOnPickup = true;
  private int nextTicketNumber;
  [Serialize]
  public bool deleteOffGrid = true;
  private List<Pickupable.Reservation> reservations = new List<Pickupable.Reservation>();
  private HandleVector<int>.Handle solidPartitionerEntry;
  private HandleVector<int>.Handle partitionerEntry;
  private LoggerFSSF log;
  private static readonly EventSystem.IntraObjectHandler<Pickupable> OnStoreDelegate = new EventSystem.IntraObjectHandler<Pickupable>((Action<Pickupable, object>) ((component, data) => component.OnStore(data)));
  private static readonly EventSystem.IntraObjectHandler<Pickupable> OnLandedDelegate = new EventSystem.IntraObjectHandler<Pickupable>((Action<Pickupable, object>) ((component, data) => component.OnLanded(data)));
  private static readonly EventSystem.IntraObjectHandler<Pickupable> OnOreSizeChangedDelegate = new EventSystem.IntraObjectHandler<Pickupable>((Action<Pickupable, object>) ((component, data) => component.OnOreSizeChanged(data)));
  private static readonly EventSystem.IntraObjectHandler<Pickupable> OnReachableChangedDelegate = new EventSystem.IntraObjectHandler<Pickupable>((Action<Pickupable, object>) ((component, data) => component.OnReachableChanged(data)));
  private static readonly EventSystem.IntraObjectHandler<Pickupable> RefreshStorageTagsDelegate = new EventSystem.IntraObjectHandler<Pickupable>((Action<Pickupable, object>) ((component, data) => component.RefreshStorageTags(data)));
  private static readonly EventSystem.IntraObjectHandler<Pickupable> OnTagsChangedDelegate = new EventSystem.IntraObjectHandler<Pickupable>((Action<Pickupable, object>) ((component, data) => component.OnTagsChanged(data)));
  private int entombedCell = -1;

  public PrimaryElement PrimaryElement => this.primaryElement;

  public int sortOrder
  {
    get => this._sortOrder;
    set => this._sortOrder = value;
  }

  public Storage storage { get; set; }

  public float MinTakeAmount => 0.0f;

  public bool prevent_absorb_until_stored { get; set; }

  public bool isKinematic { get; set; }

  public bool wasAbsorbed { get; private set; }

  public int cachedCell { get; private set; }

  public bool IsEntombed
  {
    get => this.isEntombed;
    set
    {
      if (value == this.isEntombed)
        return;
      this.isEntombed = value;
      if (this.isEntombed)
        ((Component) this).GetComponent<KPrefabID>().AddTag(GameTags.Entombed, false);
      else
        ((Component) this).GetComponent<KPrefabID>().RemoveTag(GameTags.Entombed);
      this.Trigger(-1089732772, (object) null);
      this.UpdateEntombedVisualizer();
    }
  }

  private bool CouldBePickedUpCommon(GameObject carrier)
  {
    if ((double) this.UnreservedAmount < (double) this.MinTakeAmount)
      return false;
    return (double) this.UnreservedAmount > 0.0 || (double) this.FindReservedAmount(carrier) > 0.0;
  }

  public bool CouldBePickedUpByMinion(GameObject carrier)
  {
    if (!this.CouldBePickedUpCommon(carrier))
      return false;
    return Object.op_Equality((Object) this.storage, (Object) null) || !Object.op_Implicit((Object) this.storage.automatable) || !this.storage.automatable.GetAutomationOnly();
  }

  public bool CouldBePickedUpByTransferArm(GameObject carrier) => this.CouldBePickedUpCommon(carrier);

  public float FindReservedAmount(GameObject reserver)
  {
    for (int index = 0; index < this.reservations.Count; ++index)
    {
      if (Object.op_Equality((Object) this.reservations[index].reserver, (Object) reserver))
        return this.reservations[index].amount;
    }
    return 0.0f;
  }

  public float UnreservedAmount => this.TotalAmount - this.ReservedAmount;

  public float ReservedAmount { get; private set; }

  public float TotalAmount
  {
    get => this.primaryElement.Units;
    set
    {
      DebugUtil.Assert(Object.op_Inequality((Object) this.primaryElement, (Object) null));
      this.primaryElement.Units = value;
      if ((double) value < (double) PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT && !this.primaryElement.KeepZeroMassObject)
        TracesExtesions.DeleteObject(((Component) this).gameObject);
      this.NotifyChanged(Grid.PosToCell((KMonoBehaviour) this));
    }
  }

  private void RefreshReservedAmount()
  {
    this.ReservedAmount = 0.0f;
    for (int index = 0; index < this.reservations.Count; ++index)
      this.ReservedAmount += this.reservations[index].amount;
  }

  [Conditional("UNITY_EDITOR")]
  private void Log(string evt, string param, float value)
  {
  }

  public void ClearReservations()
  {
    this.reservations.Clear();
    this.RefreshReservedAmount();
  }

  [ContextMenu("Print Reservations")]
  public void PrintReservations()
  {
    foreach (Pickupable.Reservation reservation in this.reservations)
      Debug.Log((object) reservation.ToString());
  }

  public int Reserve(string context, GameObject reserver, float amount)
  {
    int ticket = this.nextTicketNumber++;
    this.reservations.Add(new Pickupable.Reservation(reserver, amount, ticket));
    this.RefreshReservedAmount();
    if (this.OnReservationsChanged != null)
      this.OnReservationsChanged();
    return ticket;
  }

  public void Unreserve(string context, int ticket)
  {
    for (int index = 0; index < this.reservations.Count; ++index)
    {
      if (this.reservations[index].ticket == ticket)
      {
        this.reservations.RemoveAt(index);
        this.RefreshReservedAmount();
        if (this.OnReservationsChanged == null)
          break;
        this.OnReservationsChanged();
        break;
      }
    }
  }

  private Pickupable()
  {
    this.showProgressBar = false;
    this.SetOffsetTable(OffsetGroups.InvertedStandardTable);
    this.shouldTransferDiseaseWithWorker = false;
  }

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.workingPstComplete = (HashedString[]) null;
    this.workingPstFailed = (HashedString[]) null;
    this.log = new LoggerFSSF(nameof (Pickupable));
    this.workerStatusItem = Db.Get().DuplicantStatusItems.PickingUp;
    this.SetWorkTime(1.5f);
    this.targetWorkable = (Workable) this;
    this.resetProgressOnStop = true;
    ((Component) this).gameObject.layer = Game.PickupableLayer;
    this.UpdateCachedCell(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)));
    this.Subscribe<Pickupable>(856640610, Pickupable.OnStoreDelegate);
    this.Subscribe<Pickupable>(1188683690, Pickupable.OnLandedDelegate);
    this.Subscribe<Pickupable>(1807976145, Pickupable.OnOreSizeChangedDelegate);
    this.Subscribe<Pickupable>(-1432940121, Pickupable.OnReachableChangedDelegate);
    this.Subscribe<Pickupable>(-778359855, Pickupable.RefreshStorageTagsDelegate);
    this.KPrefabID.AddTag(GameTags.Pickupable, false);
    Components.Pickupables.Add(this);
  }

  protected override void OnLoadLevel() => base.OnLoadLevel();

  protected override void OnSpawn()
  {
    base.OnSpawn();
    int cell = Grid.PosToCell((KMonoBehaviour) this);
    if (!Grid.IsValidCell(cell) && this.deleteOffGrid)
    {
      TracesExtesions.DeleteObject(((Component) this).gameObject);
    }
    else
    {
      this.UpdateCachedCell(cell);
      new ReachabilityMonitor.Instance((Workable) this).StartSM();
      new FetchableMonitor.Instance(this).StartSM();
      this.SetWorkTime(1.5f);
      this.faceTargetWhenWorking = true;
      KSelectable component1 = ((Component) this).GetComponent<KSelectable>();
      if (Object.op_Inequality((Object) component1, (Object) null))
        component1.SetStatusIndicatorOffset(new Vector3(0.0f, -0.65f, 0.0f));
      this.OnTagsChanged((object) null);
      this.TryToOffsetIfBuried();
      DecorProvider component2 = ((Component) this).GetComponent<DecorProvider>();
      if (Object.op_Inequality((Object) component2, (Object) null) && string.IsNullOrEmpty(component2.overrideName))
        component2.overrideName = (string) UI.OVERLAYS.DECOR.CLUTTER;
      this.UpdateEntombedVisualizer();
      this.Subscribe<Pickupable>(-1582839653, Pickupable.OnTagsChangedDelegate);
      this.NotifyChanged(cell);
    }
  }

  [OnDeserialized]
  public void OnDeserialize()
  {
    if (!SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 28) || (double) this.transform.position.z != 0.0)
      return;
    KBatchedAnimController component = ((Component) this.transform).GetComponent<KBatchedAnimController>();
    component.SetSceneLayer(component.sceneLayer);
  }

  public void RegisterListeners()
  {
    if (this.cleaningUp || this.solidPartitionerEntry.IsValid())
      return;
    int cell = Grid.PosToCell((KMonoBehaviour) this);
    this.objectLayerListItem = new ObjectLayerListItem(((Component) this).gameObject, ObjectLayer.Pickupables, cell);
    this.solidPartitionerEntry = GameScenePartitioner.Instance.Add("Pickupable.RegisterSolidListener", (object) ((Component) this).gameObject, cell, GameScenePartitioner.Instance.solidChangedLayer, new Action<object>(this.OnSolidChanged));
    this.partitionerEntry = GameScenePartitioner.Instance.Add("Pickupable.RegisterPickupable", (object) this, cell, GameScenePartitioner.Instance.pickupablesLayer, (Action<object>) null);
    Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(this.transform, new System.Action(this.OnCellChange), "Pickupable.OnCellChange");
    Singleton<CellChangeMonitor>.Instance.MarkDirty(this.transform);
    Singleton<CellChangeMonitor>.Instance.ClearLastKnownCell(this.transform);
  }

  public void UnregisterListeners()
  {
    if (this.objectLayerListItem != null)
    {
      this.objectLayerListItem.Clear();
      this.objectLayerListItem = (ObjectLayerListItem) null;
    }
    GameScenePartitioner.Instance.Free(ref this.solidPartitionerEntry);
    GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
    Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(this.transform, new System.Action(this.OnCellChange));
  }

  private void OnSolidChanged(object data) => this.TryToOffsetIfBuried();

  public void TryToOffsetIfBuried()
  {
    if (this.KPrefabID.HasTag(GameTags.Stored) || this.KPrefabID.HasTag(GameTags.Equipped))
      return;
    int num1 = Grid.PosToCell((KMonoBehaviour) this);
    if (!Grid.IsValidCell(num1))
      return;
    DeathMonitor.Instance smi = ((Component) this).gameObject.GetSMI<DeathMonitor.Instance>();
    if ((smi == null ? 1 : (smi.IsDead() ? 1 : 0)) != 0 && (Grid.Solid[num1] && Grid.Foundation[num1] || Grid.Properties[num1] != (byte) 0))
    {
      for (int index = 0; index < Pickupable.displacementOffsets.Length; ++index)
      {
        int num2 = Grid.OffsetCell(num1, Pickupable.displacementOffsets[index]);
        if (Grid.IsValidCell(num2) && !Grid.Solid[num2])
        {
          Vector3 posCbc = Grid.CellToPosCBC(num2, Grid.SceneLayer.Move);
          KCollider2D component = ((Component) this).GetComponent<KCollider2D>();
          if (Object.op_Inequality((Object) component, (Object) null))
          {
            ref float local = ref posCbc.y;
            double num3 = (double) local;
            double y1 = (double) TransformExtensions.GetPosition(this.transform).y;
            Bounds bounds = component.bounds;
            double y2 = (double) ((Bounds) ref bounds).min.y;
            double num4 = y1 - y2;
            local = (float) (num3 + num4);
          }
          TransformExtensions.SetPosition(this.transform, posCbc);
          num1 = num2;
          this.RemoveFaller();
          this.AddFaller(Vector2.zero);
          break;
        }
      }
    }
    this.HandleSolidCell(num1);
  }

  private bool HandleSolidCell(int cell)
  {
    bool isEntombed = this.IsEntombed;
    bool flag = false;
    if (Grid.IsValidCell(cell) && Grid.Solid[cell])
    {
      DeathMonitor.Instance smi = ((Component) this).gameObject.GetSMI<DeathMonitor.Instance>();
      if ((smi == null ? 1 : (smi.IsDead() ? 1 : 0)) != 0)
      {
        this.Clearable.CancelClearing();
        flag = true;
      }
    }
    if (flag != isEntombed && !this.KPrefabID.HasTag(GameTags.Stored))
    {
      this.IsEntombed = flag;
      ((Component) this).GetComponent<KSelectable>().IsSelectable = !this.IsEntombed;
    }
    this.UpdateEntombedVisualizer();
    return this.IsEntombed;
  }

  private void OnCellChange()
  {
    Vector3 position = TransformExtensions.GetPosition(this.transform);
    int cell = Grid.PosToCell(position);
    if (!Grid.IsValidCell(cell))
    {
      Vector2 vector2_1;
      // ISSUE: explicit constructor call
      ((Vector2) ref vector2_1).\u002Ector(-0.1f * (float) Grid.WidthInCells, 1.1f * (float) Grid.WidthInCells);
      Vector2 vector2_2;
      // ISSUE: explicit constructor call
      ((Vector2) ref vector2_2).\u002Ector(-0.1f * (float) Grid.HeightInCells, 1.1f * (float) Grid.HeightInCells);
      if (!this.deleteOffGrid || (double) position.x >= (double) vector2_1.x && (double) vector2_1.y >= (double) position.x && (double) position.y >= (double) vector2_2.x && (double) vector2_2.y >= (double) position.y)
        return;
      TracesExtesions.DeleteObject((Component) this);
    }
    else
    {
      this.ReleaseEntombedVisualizerAndAddFaller(true);
      if (this.HandleSolidCell(cell))
        return;
      this.objectLayerListItem.Update(cell);
      bool flag = false;
      if (this.absorbable && !this.KPrefabID.HasTag(GameTags.Stored))
      {
        int num = Grid.CellBelow(cell);
        if (Grid.IsValidCell(num) && Grid.Solid[num])
        {
          ObjectLayerListItem nextItem = this.objectLayerListItem.nextItem;
          while (nextItem != null)
          {
            GameObject gameObject = nextItem.gameObject;
            nextItem = nextItem.nextItem;
            Pickupable component = gameObject.GetComponent<Pickupable>();
            if (Object.op_Inequality((Object) component, (Object) null))
            {
              flag = component.TryAbsorb(this, false);
              if (flag)
                break;
            }
          }
        }
      }
      GameScenePartitioner.Instance.UpdatePosition(this.solidPartitionerEntry, cell);
      GameScenePartitioner.Instance.UpdatePosition(this.partitionerEntry, cell);
      int cachedCell = this.cachedCell;
      this.UpdateCachedCell(cell);
      if (!flag)
        this.NotifyChanged(cell);
      if (!Grid.IsValidCell(cachedCell) || cell == cachedCell)
        return;
      this.NotifyChanged(cachedCell);
    }
  }

  private void OnTagsChanged(object data)
  {
    if (!this.KPrefabID.HasTag(GameTags.Stored) && !this.KPrefabID.HasTag(GameTags.Equipped))
    {
      this.RegisterListeners();
      this.AddFaller(Vector2.zero);
    }
    else
    {
      this.UnregisterListeners();
      this.RemoveFaller();
    }
  }

  private void NotifyChanged(int new_cell) => GameScenePartitioner.Instance.TriggerEvent(new_cell, GameScenePartitioner.Instance.pickupablesChangedLayer, (object) this);

  public bool TryAbsorb(Pickupable other, bool hide_effects, bool allow_cross_storage = false)
  {
    if (Object.op_Equality((Object) other, (Object) null) || other.wasAbsorbed || this.wasAbsorbed || !other.CanAbsorb(this) || this.prevent_absorb_until_stored || !allow_cross_storage && Object.op_Equality((Object) this.storage, (Object) null) != Object.op_Equality((Object) other.storage, (Object) null))
      return false;
    this.Absorb(other);
    if (!hide_effects && Object.op_Inequality((Object) EffectPrefabs.Instance, (Object) null) && !Object.op_Implicit((Object) this.storage))
    {
      Vector3 position = TransformExtensions.GetPosition(this.transform);
      position.z = Grid.GetLayerZ(Grid.SceneLayer.Front);
      Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(EffectConfigs.OreAbsorbId)), position, Quaternion.identity, (GameObject) null, (string) null, true, 0).SetActive(true);
    }
    return true;
  }

  protected override void OnCleanUp()
  {
    this.cleaningUp = true;
    this.ReleaseEntombedVisualizerAndAddFaller(false);
    this.RemoveFaller();
    if (Object.op_Implicit((Object) this.storage))
      this.storage.Remove(((Component) this).gameObject);
    this.UnregisterListeners();
    Components.Pickupables.Remove(this);
    if (this.reservations.Count > 0)
    {
      this.reservations.Clear();
      if (this.OnReservationsChanged != null)
        this.OnReservationsChanged();
    }
    if (Grid.IsValidCell(this.cachedCell))
      this.NotifyChanged(this.cachedCell);
    base.OnCleanUp();
  }

  public Pickupable Take(float amount)
  {
    if ((double) amount <= 0.0)
      return (Pickupable) null;
    if (this.OnTake != null)
    {
      if ((double) amount >= (double) this.TotalAmount && Object.op_Inequality((Object) this.storage, (Object) null) && !this.primaryElement.KeepZeroMassObject)
        this.storage.Remove(((Component) this).gameObject);
      float num = Math.Min(this.TotalAmount, amount);
      return (double) num <= 0.0 ? (Pickupable) null : this.OnTake(num);
    }
    if (Object.op_Inequality((Object) this.storage, (Object) null))
      this.storage.Remove(((Component) this).gameObject);
    return this;
  }

  private void Absorb(Pickupable pickupable)
  {
    Debug.Assert(!this.wasAbsorbed);
    Debug.Assert(!pickupable.wasAbsorbed);
    this.Trigger(-2064133523, (object) pickupable);
    pickupable.Trigger(-1940207677, (object) ((Component) this).gameObject);
    pickupable.wasAbsorbed = true;
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    if (Object.op_Inequality((Object) SelectTool.Instance, (Object) null) && Object.op_Inequality((Object) SelectTool.Instance.selected, (Object) null) && Object.op_Equality((Object) SelectTool.Instance.selected, (Object) ((Component) pickupable).GetComponent<KSelectable>()))
      SelectTool.Instance.Select(component);
    TracesExtesions.DeleteObject(((Component) pickupable).gameObject);
    this.NotifyChanged(Grid.PosToCell((KMonoBehaviour) this));
  }

  private void RefreshStorageTags(object data = null)
  {
    bool flag = data is Storage || data != null && (bool) data;
    if (flag && data is Storage && Object.op_Equality((Object) ((Component) data).gameObject, (Object) ((Component) this).gameObject))
      return;
    if (flag)
    {
      this.KPrefabID.AddTag(GameTags.Stored, false);
      if ((this.storage == null ? 1 : (!this.storage.allowItemRemoval ? 1 : 0)) != 0)
        this.KPrefabID.AddTag(GameTags.StoredPrivate, false);
      else
        this.KPrefabID.RemoveTag(GameTags.StoredPrivate);
    }
    else
    {
      this.KPrefabID.RemoveTag(GameTags.Stored);
      this.KPrefabID.RemoveTag(GameTags.StoredPrivate);
    }
  }

  public void OnStore(object data)
  {
    this.storage = data as Storage;
    bool flag = data is Storage || data != null && (bool) data;
    SaveLoadRoot component1 = ((Component) this).GetComponent<SaveLoadRoot>();
    if (Object.op_Inequality((Object) this.carryAnimOverride, (Object) null) && Object.op_Inequality((Object) this.lastCarrier, (Object) null))
    {
      this.lastCarrier.RemoveAnimOverrides(this.carryAnimOverride);
      this.lastCarrier = (KBatchedAnimController) null;
    }
    KSelectable component2 = ((Component) this).GetComponent<KSelectable>();
    if (Object.op_Implicit((Object) component2))
      component2.IsSelectable = !flag;
    if (flag)
    {
      int cachedCell = this.cachedCell;
      this.RefreshStorageTags(data);
      if (this.storage != null)
      {
        if (Object.op_Inequality((Object) this.carryAnimOverride, (Object) null) && Object.op_Inequality((Object) ((Component) this.storage).GetComponent<Navigator>(), (Object) null))
        {
          this.lastCarrier = ((Component) this.storage).GetComponent<KBatchedAnimController>();
          if (Object.op_Inequality((Object) this.lastCarrier, (Object) null))
            this.lastCarrier.AddAnimOverrides(this.carryAnimOverride);
        }
        this.UpdateCachedCell(Grid.PosToCell((KMonoBehaviour) this.storage));
      }
      this.NotifyChanged(cachedCell);
      if (!Object.op_Inequality((Object) component1, (Object) null))
        return;
      component1.SetRegistered(false);
    }
    else
    {
      if (Object.op_Inequality((Object) component1, (Object) null))
        component1.SetRegistered(true);
      this.RemovedFromStorage();
    }
  }

  private void RemovedFromStorage()
  {
    this.storage = (Storage) null;
    this.UpdateCachedCell(Grid.PosToCell((KMonoBehaviour) this));
    this.RefreshStorageTags();
    this.AddFaller(Vector2.zero);
    KBatchedAnimController component = ((Component) this).GetComponent<KBatchedAnimController>();
    component.enabled = true;
    ((Component) this).gameObject.transform.rotation = Quaternion.identity;
    this.RegisterListeners();
    component.GetBatchInstanceData().ClearOverrideTransformMatrix();
  }

  public void UpdateCachedCellFromStoragePosition()
  {
    Debug.Assert(Object.op_Inequality((Object) this.storage, (Object) null), (object) "Only call UpdateCachedCellFromStoragePosition on pickupables in storage!");
    this.UpdateCachedCell(Grid.PosToCell((KMonoBehaviour) this.storage));
  }

  private void UpdateCachedCell(int cell)
  {
    this.cachedCell = cell;
    this.GetOffsets(this.cachedCell);
  }

  public override Workable.AnimInfo GetAnim(Worker worker)
  {
    if (!this.useGunforPickup || !worker.usesMultiTool)
      return base.GetAnim(worker);
    return base.GetAnim(worker) with
    {
      smi = (StateMachine.Instance) new MultitoolController.Instance((Workable) this, worker, HashedString.op_Implicit("pickup"), Assets.GetPrefab(Tag.op_Implicit(EffectConfigs.OreAbsorbId)))
    };
  }

  protected override void OnCompleteWork(Worker worker)
  {
    Storage component = ((Component) worker).GetComponent<Storage>();
    Pickupable.PickupableStartWorkInfo startWorkInfo = (Pickupable.PickupableStartWorkInfo) worker.startWorkInfo;
    float amount = startWorkInfo.amount;
    Pickupable pickupable1 = this;
    if (Object.op_Inequality((Object) pickupable1, (Object) null))
    {
      Pickupable pickupable2 = pickupable1.Take(amount);
      if (Object.op_Inequality((Object) pickupable2, (Object) null))
      {
        component.Store(((Component) pickupable2).gameObject);
        worker.workCompleteData = (object) pickupable2;
        startWorkInfo.setResultCb(((Component) pickupable2).gameObject);
      }
      else
        startWorkInfo.setResultCb((GameObject) null);
    }
    else
      startWorkInfo.setResultCb((GameObject) null);
  }

  public override bool InstantlyFinish(Worker worker) => false;

  public override Vector3 GetTargetPoint() => TransformExtensions.GetPosition(this.transform);

  public bool IsReachable() => this.isReachable;

  private void OnReachableChanged(object data)
  {
    this.isReachable = (bool) data;
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    if (this.isReachable)
      component.RemoveStatusItem(Db.Get().MiscStatusItems.PickupableUnreachable);
    else
      component.AddStatusItem(Db.Get().MiscStatusItems.PickupableUnreachable, (object) this);
  }

  private void AddFaller(Vector2 initial_velocity)
  {
    if (Object.op_Inequality((Object) ((Component) this).GetComponent<Health>(), (Object) null) || ((KComponentManager<FallerComponent>) GameComps.Fallers).Has((object) ((Component) this).gameObject))
      return;
    GameComps.Fallers.Add(((Component) this).gameObject, initial_velocity);
  }

  private void RemoveFaller()
  {
    if (Object.op_Inequality((Object) ((Component) this).GetComponent<Health>(), (Object) null) || !((KComponentManager<FallerComponent>) GameComps.Fallers).Has((object) ((Component) this).gameObject))
      return;
    ((KGameObjectComponentManager<FallerComponent>) GameComps.Fallers).Remove(((Component) this).gameObject);
  }

  private void OnOreSizeChanged(object data)
  {
    Vector3 vector3 = Vector3.zero;
    HandleVector<int>.Handle handle = GameComps.Gravities.GetHandle(((Component) this).gameObject);
    if (handle.IsValid())
      vector3 = Vector2.op_Implicit(((KCompactedVector<GravityComponent>) GameComps.Gravities).GetData(handle).velocity);
    this.RemoveFaller();
    if (this.KPrefabID.HasTag(GameTags.Stored))
      return;
    this.AddFaller(Vector2.op_Implicit(vector3));
  }

  private void OnLanded(object data)
  {
    if (Object.op_Equality((Object) CameraController.Instance, (Object) null))
      return;
    Vector3 position = TransformExtensions.GetPosition(this.transform);
    Vector2I xy = Grid.PosToXY(position);
    if (xy.x < 0 || Grid.WidthInCells <= xy.x || xy.y < 0 || Grid.HeightInCells <= xy.y)
    {
      TracesExtesions.DeleteObject((Component) this);
    }
    else
    {
      Vector2 vector2 = (Vector2) data;
      if ((double) ((Vector2) ref vector2).sqrMagnitude <= 0.20000000298023224 || SpeedControlScreen.Instance.IsPaused)
        return;
      Element element = this.primaryElement.Element;
      if (element.substance == null)
        return;
      string str1 = element.substance.GetOreBumpSound() ?? (!element.HasTag(GameTags.RefinedMetal) ? (!element.HasTag(GameTags.Metal) ? "Rock" : "RawMetal") : "RefinedMetal");
      string str2 = GlobalAssets.GetSound(!(element.tag.ToString() == "Creature") || ((Component) this).gameObject.HasTag(GameTags.Seed) ? "Ore_bump_" + str1 : "Bodyfall_rock", true) ?? GlobalAssets.GetSound("Ore_bump_rock");
      if (!CameraController.Instance.IsAudibleSound(TransformExtensions.GetPosition(this.transform), HashedString.op_Implicit(str2)))
        return;
      int cell = Grid.PosToCell(position);
      int num1 = Grid.Element[cell].IsLiquid ? 1 : 0;
      float num2 = 0.0f;
      if (num1 != 0)
        num2 = SoundUtil.GetLiquidDepth(cell);
      EventInstance eventInstance = KFMOD.BeginOneShot(str2, CameraController.Instance.GetVerticallyScaledPosition(TransformExtensions.GetPosition(this.transform)), 1f);
      ((EventInstance) ref eventInstance).setParameterByName("velocity", ((Vector2) ref vector2).magnitude, false);
      ((EventInstance) ref eventInstance).setParameterByName("liquidDepth", num2, false);
      KFMOD.EndOneShot(eventInstance);
    }
  }

  private void UpdateEntombedVisualizer()
  {
    if (this.IsEntombed)
    {
      if (this.entombedCell != -1)
        return;
      int cell = Grid.PosToCell((KMonoBehaviour) this);
      if (EntombedItemManager.CanEntomb(this))
        SaveGame.Instance.entombedItemManager.Add(this);
      if (!Object.op_Equality((Object) Grid.Objects[cell, 1], (Object) null))
        return;
      KBatchedAnimController component = ((Component) this).GetComponent<KBatchedAnimController>();
      if (!Object.op_Inequality((Object) component, (Object) null) || !((Component) Game.Instance).GetComponent<EntombedItemVisualizer>().AddItem(cell))
        return;
      this.entombedCell = cell;
      component.enabled = false;
      this.RemoveFaller();
    }
    else
      this.ReleaseEntombedVisualizerAndAddFaller(true);
  }

  private void ReleaseEntombedVisualizerAndAddFaller(bool add_faller_if_necessary)
  {
    if (this.entombedCell == -1)
      return;
    ((Component) Game.Instance).GetComponent<EntombedItemVisualizer>().RemoveItem(this.entombedCell);
    this.entombedCell = -1;
    ((Component) this).GetComponent<KBatchedAnimController>().enabled = true;
    if (!add_faller_if_necessary)
      return;
    this.AddFaller(Vector2.zero);
  }

  private struct Reservation
  {
    public GameObject reserver;
    public float amount;
    public int ticket;

    public Reservation(GameObject reserver, float amount, int ticket)
    {
      this.reserver = reserver;
      this.amount = amount;
      this.ticket = ticket;
    }

    public override string ToString() => ((Object) this.reserver).name + ", " + this.amount.ToString() + ", " + this.ticket.ToString();
  }

  public class PickupableStartWorkInfo : Worker.StartWorkInfo
  {
    public float amount { get; private set; }

    public Pickupable originalPickupable { get; private set; }

    public Action<GameObject> setResultCb { get; private set; }

    public PickupableStartWorkInfo(
      Pickupable pickupable,
      float amount,
      Action<GameObject> set_result_cb)
      : base(pickupable.targetWorkable)
    {
      this.originalPickupable = pickupable;
      this.amount = amount;
      this.setResultCb = set_result_cb;
    }
  }
}
