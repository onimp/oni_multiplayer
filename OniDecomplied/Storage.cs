// Decompiled with JetBrains decompiler
// Type: Storage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/Workable/Storage")]
public class Storage : Workable, ISaveLoadableDetails, IGameObjectEffectDescriptor, IStorage
{
  public bool allowItemRemoval;
  public bool ignoreSourcePriority;
  public bool onlyTransferFromLowerPriority;
  public float capacityKg = 20000f;
  public bool showDescriptor;
  public bool doDiseaseTransfer = true;
  public List<Tag> storageFilters;
  public bool useGunForDelivery = true;
  public bool sendOnStoreOnSpawn;
  public bool showInUI = true;
  public bool allowClearable;
  public bool showCapacityStatusItem;
  public bool showCapacityAsMainStatus;
  public bool showUnreachableStatus;
  public bool showSideScreenTitleBar;
  public bool useWideOffsets;
  [MyCmpGet]
  private Rotatable rotatable;
  public Vector2 gunTargetOffset;
  public Storage.FetchCategory fetchCategory;
  public int storageNetworkID = -1;
  public float storageFullMargin;
  public Vector3 storageFXOffset = Vector3.zero;
  private static readonly EventSystem.IntraObjectHandler<Storage> OnReachableChangedDelegate = new EventSystem.IntraObjectHandler<Storage>((Action<Storage, object>) ((component, data) => component.OnReachableChanged(data)));
  public Storage.FXPrefix fxPrefix;
  public List<GameObject> items = new List<GameObject>();
  [MyCmpGet]
  public Prioritizable prioritizable;
  [MyCmpGet]
  public Automatable automatable;
  [MyCmpGet]
  protected PrimaryElement primaryElement;
  public bool dropOnLoad;
  protected float maxKGPerItem = float.MaxValue;
  private bool endOfLife;
  public bool allowSettingOnlyFetchMarkedItems = true;
  [KSerialization.Serialize]
  private bool onlyFetchMarkedItems;
  public float storageWorkTime = 1.5f;
  private static readonly List<Storage.StoredItemModifierInfo> StoredItemModifierHandlers = new List<Storage.StoredItemModifierInfo>()
  {
    new Storage.StoredItemModifierInfo(Storage.StoredItemModifier.Hide, new Action<GameObject, bool, bool>(Storage.MakeItemInvisible)),
    new Storage.StoredItemModifierInfo(Storage.StoredItemModifier.Insulate, new Action<GameObject, bool, bool>(Storage.MakeItemTemperatureInsulated)),
    new Storage.StoredItemModifierInfo(Storage.StoredItemModifier.Seal, new Action<GameObject, bool, bool>(Storage.MakeItemSealed)),
    new Storage.StoredItemModifierInfo(Storage.StoredItemModifier.Preserve, new Action<GameObject, bool, bool>(Storage.MakeItemPreserved))
  };
  [SerializeField]
  private List<Storage.StoredItemModifier> defaultStoredItemModifers = new List<Storage.StoredItemModifier>()
  {
    Storage.StoredItemModifier.Hide
  };
  public static readonly List<Storage.StoredItemModifier> StandardSealedStorage = new List<Storage.StoredItemModifier>()
  {
    Storage.StoredItemModifier.Hide,
    Storage.StoredItemModifier.Seal
  };
  public static readonly List<Storage.StoredItemModifier> StandardFabricatorStorage = new List<Storage.StoredItemModifier>()
  {
    Storage.StoredItemModifier.Hide,
    Storage.StoredItemModifier.Preserve
  };
  public static readonly List<Storage.StoredItemModifier> StandardInsulatedStorage = new List<Storage.StoredItemModifier>()
  {
    Storage.StoredItemModifier.Hide,
    Storage.StoredItemModifier.Seal,
    Storage.StoredItemModifier.Insulate
  };
  private static StatusItem capacityStatusItem;
  private static readonly EventSystem.IntraObjectHandler<Storage> OnDeadTagAddedDelegate = GameUtil.CreateHasTagHandler<Storage>(GameTags.Dead, (Action<Storage, object>) ((component, data) => component.OnDeath(data)));
  private static readonly EventSystem.IntraObjectHandler<Storage> OnQueueDestroyObjectDelegate = new EventSystem.IntraObjectHandler<Storage>((Action<Storage, object>) ((component, data) => component.OnQueueDestroyObject(data)));
  private static readonly EventSystem.IntraObjectHandler<Storage> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<Storage>((Action<Storage, object>) ((component, data) => component.OnCopySettings(data)));
  private List<GameObject> deleted_objects;

  public bool ShouldOnlyTransferFromLowerPriority => this.onlyTransferFromLowerPriority || this.allowItemRemoval;

  public bool allowUIItemRemoval { get; set; }

  public GameObject this[int idx] => this.items[idx];

  public int Count => this.items.Count;

  public bool ShouldShowInUI() => this.showInUI;

  public List<GameObject> GetItems() => this.items;

  public void SetDefaultStoredItemModifiers(List<Storage.StoredItemModifier> modifiers) => this.defaultStoredItemModifers = modifiers;

  public PrioritySetting masterPriority => Object.op_Implicit((Object) this.prioritizable) ? this.prioritizable.GetMasterPriority() : Chore.DefaultPrioritySetting;

  public override Workable.AnimInfo GetAnim(Worker worker)
  {
    if (!this.useGunForDelivery || !worker.usesMultiTool)
      return base.GetAnim(worker);
    return base.GetAnim(worker) with
    {
      smi = (StateMachine.Instance) new MultitoolController.Instance((Workable) this, worker, HashedString.op_Implicit("store"), Assets.GetPrefab(Tag.op_Implicit(EffectConfigs.OreAbsorbId)))
    };
  }

  public override Vector3 GetTargetPoint()
  {
    Vector3 targetPoint = base.GetTargetPoint();
    if (this.useGunForDelivery && Vector2.op_Inequality(this.gunTargetOffset, Vector2.zero))
      targetPoint = !Object.op_Inequality((Object) this.rotatable, (Object) null) ? Vector3.op_Addition(targetPoint, new Vector3(this.gunTargetOffset.x, this.gunTargetOffset.y, 0.0f)) : Vector3.op_Addition(targetPoint, this.rotatable.GetRotatedOffset(Vector2.op_Implicit(this.gunTargetOffset)));
    return targetPoint;
  }

  public event System.Action OnStorageIncreased;

  protected override void OnPrefabInit()
  {
    if (this.useWideOffsets)
      this.SetOffsetTable(OffsetGroups.InvertedWideTable);
    else
      this.SetOffsetTable(OffsetGroups.InvertedStandardTable);
    this.showProgressBar = false;
    this.faceTargetWhenWorking = true;
    base.OnPrefabInit();
    GameUtil.SubscribeToTags<Storage>(this, Storage.OnDeadTagAddedDelegate, true);
    this.Subscribe<Storage>(1502190696, Storage.OnQueueDestroyObjectDelegate);
    this.Subscribe<Storage>(-905833192, Storage.OnCopySettingsDelegate);
    this.workerStatusItem = Db.Get().DuplicantStatusItems.Storing;
    this.resetProgressOnStop = true;
    this.synchronizeAnims = false;
    this.workingPstComplete = (HashedString[]) null;
    this.workingPstFailed = (HashedString[]) null;
    this.SetupStorageStatusItems();
  }

  private void SetupStorageStatusItems()
  {
    if (Storage.capacityStatusItem == null)
    {
      Storage.capacityStatusItem = new StatusItem("StorageLocker", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      Storage.capacityStatusItem.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        Storage storage = (Storage) data;
        float num1 = storage.MassStored();
        float num2 = storage.capacityKg;
        string newValue1 = Util.FormatWholeNumber((double) num1 <= (double) num2 - (double) storage.storageFullMargin || (double) num1 >= (double) num2 ? Mathf.Floor(num1) : num2);
        IUserControlledCapacity component = ((Component) storage).GetComponent<IUserControlledCapacity>();
        if (component != null)
          num2 = Mathf.Min(component.UserMaxCapacity, num2);
        string newValue2 = Util.FormatWholeNumber(num2);
        str = str.Replace("{Stored}", newValue1);
        str = str.Replace("{Capacity}", newValue2);
        str = component == null ? str.Replace("{Units}", (string) GameUtil.GetCurrentMassUnit()) : str.Replace("{Units}", (string) component.CapacityUnits);
        return str;
      });
    }
    if (!this.showCapacityStatusItem)
      return;
    if (this.showCapacityAsMainStatus)
      ((Component) this).GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Storage.capacityStatusItem, (object) this);
    else
      ((Component) this).GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Stored, Storage.capacityStatusItem, (object) this);
  }

  [System.Runtime.Serialization.OnDeserialized]
  private void OnDeserialized()
  {
    if (!this.allowSettingOnlyFetchMarkedItems)
      this.onlyFetchMarkedItems = false;
    this.UpdateFetchCategory();
  }

  protected override void OnSpawn()
  {
    this.SetWorkTime(this.storageWorkTime);
    foreach (GameObject go in this.items)
    {
      this.ApplyStoredItemModifiers(go, true, true);
      if (this.sendOnStoreOnSpawn)
        EventExtensions.Trigger(go, 856640610, (object) this);
    }
    KBatchedAnimController component1 = ((Component) this).GetComponent<KBatchedAnimController>();
    if (Object.op_Inequality((Object) component1, (Object) null))
      component1.SetSymbolVisiblity(KAnimHashedString.op_Implicit("sweep"), this.onlyFetchMarkedItems);
    Prioritizable component2 = ((Component) this).GetComponent<Prioritizable>();
    if (Object.op_Inequality((Object) component2, (Object) null))
      component2.onPriorityChanged += new Action<PrioritySetting>(this.OnPriorityChanged);
    this.UpdateFetchCategory();
    if (!this.showUnreachableStatus)
      return;
    this.Subscribe<Storage>(-1432940121, Storage.OnReachableChangedDelegate);
    new ReachabilityMonitor.Instance((Workable) this).StartSM();
  }

  public GameObject Store(
    GameObject go,
    bool hide_popups = false,
    bool block_events = false,
    bool do_disease_transfer = true,
    bool is_deserializing = false)
  {
    if (Object.op_Equality((Object) go, (Object) null))
      return (GameObject) null;
    PrimaryElement component1 = go.GetComponent<PrimaryElement>();
    GameObject gameObject1 = go;
    if (!hide_popups && Object.op_Inequality((Object) PopFXManager.Instance, (Object) null))
    {
      LocString format;
      Transform transform;
      if (this.fxPrefix == Storage.FXPrefix.Delivered)
      {
        format = UI.DELIVERED;
        transform = this.transform;
      }
      else
      {
        format = UI.PICKEDUP;
        transform = go.transform;
      }
      string text = Assets.IsTagCountable(go.PrefabID()) ? string.Format((string) format, (object) (int) component1.Units, (object) go.GetProperName()) : string.Format((string) format, (object) GameUtil.GetFormattedMass(component1.Units), (object) go.GetProperName());
      PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, text, transform, this.storageFXOffset);
    }
    go.transform.parent = this.transform;
    Vector3 posCcc = Grid.CellToPosCCC(Grid.PosToCell((KMonoBehaviour) this), Grid.SceneLayer.Move);
    posCcc.z = TransformExtensions.GetPosition(go.transform).z;
    TransformExtensions.SetPosition(go.transform, posCcc);
    if (!block_events & do_disease_transfer)
      this.TransferDiseaseWithObject(go);
    if (!is_deserializing)
    {
      Pickupable component2 = go.GetComponent<Pickupable>();
      if (Object.op_Inequality((Object) component2, (Object) null))
      {
        if (Object.op_Inequality((Object) component2, (Object) null) && component2.prevent_absorb_until_stored)
          component2.prevent_absorb_until_stored = false;
        foreach (GameObject gameObject2 in this.items)
        {
          if (Object.op_Inequality((Object) gameObject2, (Object) null))
          {
            Pickupable component3 = gameObject2.GetComponent<Pickupable>();
            if (Object.op_Inequality((Object) component3, (Object) null) && component3.TryAbsorb(component2, hide_popups, true))
            {
              if (!block_events)
              {
                this.Trigger(-1697596308, (object) go);
                this.Trigger(-778359855, (object) this);
                if (this.OnStorageIncreased != null)
                  this.OnStorageIncreased();
              }
              this.ApplyStoredItemModifiers(go, true, false);
              gameObject1 = gameObject2;
              go = (GameObject) null;
              break;
            }
          }
        }
      }
    }
    if (Object.op_Inequality((Object) go, (Object) null))
    {
      this.items.Add(go);
      if (!is_deserializing)
        this.ApplyStoredItemModifiers(go, true, false);
      if (!block_events)
      {
        EventExtensions.Trigger(go, 856640610, (object) this);
        this.Trigger(-1697596308, (object) go);
        this.Trigger(-778359855, (object) this);
        if (this.OnStorageIncreased != null)
          this.OnStorageIncreased();
      }
    }
    return gameObject1;
  }

  public PrimaryElement AddElement(
    SimHashes element,
    float mass,
    float temperature,
    byte disease_idx,
    int disease_count,
    bool keep_zero_mass = false,
    bool do_disease_transfer = true)
  {
    Element elementByHash = ElementLoader.FindElementByHash(element);
    if (elementByHash.IsGas)
      return this.AddGasChunk(element, mass, temperature, disease_idx, disease_count, keep_zero_mass, do_disease_transfer);
    if (elementByHash.IsLiquid)
      return this.AddLiquid(element, mass, temperature, disease_idx, disease_count, keep_zero_mass, do_disease_transfer);
    return elementByHash.IsSolid ? this.AddOre(element, mass, temperature, disease_idx, disease_count, keep_zero_mass, do_disease_transfer) : (PrimaryElement) null;
  }

  public PrimaryElement AddOre(
    SimHashes element,
    float mass,
    float temperature,
    byte disease_idx,
    int disease_count,
    bool keep_zero_mass = false,
    bool do_disease_transfer = true)
  {
    if ((double) mass <= 0.0)
      return (PrimaryElement) null;
    PrimaryElement primaryElement = this.FindPrimaryElement(element);
    if (Object.op_Inequality((Object) primaryElement, (Object) null))
    {
      float finalTemperature = GameUtil.GetFinalTemperature(primaryElement.Temperature, primaryElement.Mass, temperature, mass);
      primaryElement.KeepZeroMassObject = keep_zero_mass;
      primaryElement.Mass += mass;
      primaryElement.Temperature = finalTemperature;
      primaryElement.AddDisease(disease_idx, disease_count, "Storage.AddOre");
      this.Trigger(-1697596308, (object) ((Component) primaryElement).gameObject);
    }
    else
    {
      Element elementByHash = ElementLoader.FindElementByHash(element);
      GameObject go = elementByHash.substance.SpawnResource(TransformExtensions.GetPosition(this.transform), mass, temperature, disease_idx, disease_count, true, manual_activation: true);
      go.GetComponent<Pickupable>().prevent_absorb_until_stored = true;
      elementByHash.substance.ActivateSubstanceGameObject(go, disease_idx, disease_count);
      this.Store(go, true, do_disease_transfer: do_disease_transfer);
    }
    return primaryElement;
  }

  public PrimaryElement AddLiquid(
    SimHashes element,
    float mass,
    float temperature,
    byte disease_idx,
    int disease_count,
    bool keep_zero_mass = false,
    bool do_disease_transfer = true)
  {
    if ((double) mass <= 0.0)
      return (PrimaryElement) null;
    PrimaryElement primaryElement = this.FindPrimaryElement(element);
    if (Object.op_Inequality((Object) primaryElement, (Object) null))
    {
      float finalTemperature = GameUtil.GetFinalTemperature(primaryElement.Temperature, primaryElement.Mass, temperature, mass);
      primaryElement.KeepZeroMassObject = keep_zero_mass;
      primaryElement.Mass += mass;
      primaryElement.Temperature = finalTemperature;
      primaryElement.AddDisease(disease_idx, disease_count, "Storage.AddLiquid");
      this.Trigger(-1697596308, (object) ((Component) primaryElement).gameObject);
    }
    else
    {
      SubstanceChunk chunk = LiquidSourceManager.Instance.CreateChunk(element, mass, temperature, disease_idx, disease_count, TransformExtensions.GetPosition(this.transform));
      primaryElement = ((Component) chunk).GetComponent<PrimaryElement>();
      primaryElement.KeepZeroMassObject = keep_zero_mass;
      this.Store(((Component) chunk).gameObject, true, do_disease_transfer: do_disease_transfer);
    }
    return primaryElement;
  }

  public PrimaryElement AddGasChunk(
    SimHashes element,
    float mass,
    float temperature,
    byte disease_idx,
    int disease_count,
    bool keep_zero_mass,
    bool do_disease_transfer = true)
  {
    if ((double) mass <= 0.0)
      return (PrimaryElement) null;
    PrimaryElement primaryElement = this.FindPrimaryElement(element);
    if (Object.op_Inequality((Object) primaryElement, (Object) null))
    {
      float mass1 = primaryElement.Mass;
      float finalTemperature = GameUtil.GetFinalTemperature(primaryElement.Temperature, mass1, temperature, mass);
      primaryElement.KeepZeroMassObject = keep_zero_mass;
      primaryElement.SetMassTemperature(mass1 + mass, finalTemperature);
      primaryElement.AddDisease(disease_idx, disease_count, "Storage.AddGasChunk");
      this.Trigger(-1697596308, (object) ((Component) primaryElement).gameObject);
    }
    else
    {
      SubstanceChunk chunk = GasSourceManager.Instance.CreateChunk(element, mass, temperature, disease_idx, disease_count, TransformExtensions.GetPosition(this.transform));
      primaryElement = ((Component) chunk).GetComponent<PrimaryElement>();
      primaryElement.KeepZeroMassObject = keep_zero_mass;
      this.Store(((Component) chunk).gameObject, true, do_disease_transfer: do_disease_transfer);
    }
    return primaryElement;
  }

  public void Transfer(Storage target, bool block_events = false, bool hide_popups = false)
  {
    while (this.items.Count > 0)
      this.Transfer(this.items[0], target, block_events, hide_popups);
  }

  public float Transfer(
    Storage dest_storage,
    Tag tag,
    float amount,
    bool block_events = false,
    bool hide_popups = false)
  {
    GameObject first = this.FindFirst(tag);
    if (!Object.op_Inequality((Object) first, (Object) null))
      return 0.0f;
    PrimaryElement component1 = first.GetComponent<PrimaryElement>();
    if ((double) amount < (double) component1.Units)
    {
      Pickupable component2 = first.GetComponent<Pickupable>();
      Pickupable pickupable = component2.Take(amount);
      dest_storage.Store(((Component) pickupable).gameObject, hide_popups, block_events);
      if (!block_events)
        this.Trigger(-1697596308, (object) ((Component) component2).gameObject);
    }
    else
    {
      this.Transfer(first, dest_storage, block_events, hide_popups);
      amount = component1.Units;
    }
    return amount;
  }

  public bool Transfer(GameObject go, Storage target, bool block_events = false, bool hide_popups = false)
  {
    this.items.RemoveAll((Predicate<GameObject>) (it => Object.op_Equality((Object) it, (Object) null)));
    int count = this.items.Count;
    for (int index = 0; index < count; ++index)
    {
      if (Object.op_Equality((Object) this.items[index], (Object) go))
      {
        this.items.RemoveAt(index);
        this.ApplyStoredItemModifiers(go, false, false);
        target.Store(go, hide_popups, block_events);
        if (!block_events)
          this.Trigger(-1697596308, (object) go);
        return true;
      }
    }
    return false;
  }

  public bool DropSome(
    Tag tag,
    float amount,
    bool ventGas = false,
    bool dumpLiquid = false,
    Vector3 offset = default (Vector3),
    bool doDiseaseTransfer = true,
    bool showInWorldNotification = false)
  {
    bool flag1 = false;
    float amount1 = amount;
    ListPool<GameObject, Storage>.PooledList result = ListPool<GameObject, Storage>.Allocate();
    this.Find(tag, (List<GameObject>) result);
    foreach (GameObject gameObject in (List<GameObject>) result)
    {
      Pickupable component1 = gameObject.GetComponent<Pickupable>();
      if (Object.op_Implicit((Object) component1))
      {
        Pickupable pickupable = component1.Take(amount1);
        if (Object.op_Inequality((Object) pickupable, (Object) null))
        {
          bool flag2 = false;
          if (ventGas | dumpLiquid)
          {
            Dumpable component2 = ((Component) pickupable).GetComponent<Dumpable>();
            if (Object.op_Inequality((Object) component2, (Object) null))
            {
              if (ventGas && ((Component) pickupable).GetComponent<PrimaryElement>().Element.IsGas)
              {
                component2.Dump(Vector3.op_Addition(TransformExtensions.GetPosition(this.transform), offset));
                flag2 = true;
                amount1 -= ((Component) pickupable).GetComponent<PrimaryElement>().Mass;
                this.Trigger(-1697596308, (object) ((Component) pickupable).gameObject);
                flag1 = true;
                if (showInWorldNotification)
                  PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, ((Component) pickupable).GetComponent<PrimaryElement>().Element.name + " " + GameUtil.GetFormattedMass(pickupable.TotalAmount), pickupable.transform, this.storageFXOffset);
              }
              if (dumpLiquid && ((Component) pickupable).GetComponent<PrimaryElement>().Element.IsLiquid)
              {
                component2.Dump(Vector3.op_Addition(TransformExtensions.GetPosition(this.transform), offset));
                flag2 = true;
                amount1 -= ((Component) pickupable).GetComponent<PrimaryElement>().Mass;
                this.Trigger(-1697596308, (object) ((Component) pickupable).gameObject);
                flag1 = true;
                if (showInWorldNotification)
                  PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, ((Component) pickupable).GetComponent<PrimaryElement>().Element.name + " " + GameUtil.GetFormattedMass(pickupable.TotalAmount), pickupable.transform, this.storageFXOffset);
              }
            }
          }
          if (!flag2)
          {
            Vector3 vector3 = Vector3.op_Addition(Grid.CellToPosCCC(Grid.PosToCell((KMonoBehaviour) this), Grid.SceneLayer.Ore), offset);
            TransformExtensions.SetPosition(pickupable.transform, vector3);
            KBatchedAnimController component3 = ((Component) pickupable).GetComponent<KBatchedAnimController>();
            if (Object.op_Implicit((Object) component3))
              component3.SetSceneLayer(Grid.SceneLayer.Ore);
            amount1 -= ((Component) pickupable).GetComponent<PrimaryElement>().Mass;
            this.MakeWorldActive(((Component) pickupable).gameObject);
            this.Trigger(-1697596308, (object) ((Component) pickupable).gameObject);
            flag1 = true;
            if (showInWorldNotification)
              PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, ((Component) pickupable).GetComponent<PrimaryElement>().Element.name + " " + GameUtil.GetFormattedMass(pickupable.TotalAmount), pickupable.transform, this.storageFXOffset);
          }
        }
      }
      if ((double) amount1 <= 0.0)
        break;
    }
    result.Recycle();
    return flag1;
  }

  public void DropAll(
    Vector3 position,
    bool vent_gas = false,
    bool dump_liquid = false,
    Vector3 offset = default (Vector3),
    bool do_disease_transfer = true,
    List<GameObject> collect_dropped_items = null)
  {
    while (this.items.Count > 0)
    {
      GameObject go = this.items[0];
      if (do_disease_transfer)
        this.TransferDiseaseWithObject(go);
      this.items.RemoveAt(0);
      if (Object.op_Inequality((Object) go, (Object) null))
      {
        bool flag = false;
        if (vent_gas | dump_liquid)
        {
          Dumpable component = go.GetComponent<Dumpable>();
          if (Object.op_Inequality((Object) component, (Object) null))
          {
            if (vent_gas && go.GetComponent<PrimaryElement>().Element.IsGas)
            {
              component.Dump(Vector3.op_Addition(position, offset));
              flag = true;
            }
            if (dump_liquid && go.GetComponent<PrimaryElement>().Element.IsLiquid)
            {
              component.Dump(Vector3.op_Addition(position, offset));
              flag = true;
            }
          }
        }
        if (!flag)
        {
          TransformExtensions.SetPosition(go.transform, Vector3.op_Addition(position, offset));
          KBatchedAnimController component = go.GetComponent<KBatchedAnimController>();
          if (Object.op_Implicit((Object) component))
            component.SetSceneLayer(Grid.SceneLayer.Ore);
          this.MakeWorldActive(go);
          collect_dropped_items?.Add(go);
        }
      }
    }
  }

  public void DropAll(
    bool vent_gas = false,
    bool dump_liquid = false,
    Vector3 offset = default (Vector3),
    bool do_disease_transfer = true,
    List<GameObject> collect_dropped_items = null)
  {
    this.DropAll(Grid.CellToPosCCC(Grid.PosToCell((KMonoBehaviour) this), Grid.SceneLayer.Ore), vent_gas, dump_liquid, offset, do_disease_transfer, collect_dropped_items);
  }

  public void Drop(Tag t, List<GameObject> obj_list)
  {
    this.Find(t, obj_list);
    foreach (GameObject go in obj_list)
      this.Drop(go, true);
  }

  public void Drop(Tag t)
  {
    ListPool<GameObject, Storage>.PooledList result = ListPool<GameObject, Storage>.Allocate();
    this.Find(t, (List<GameObject>) result);
    foreach (GameObject go in (List<GameObject>) result)
      this.Drop(go, true);
    result.Recycle();
  }

  public void DropUnlessMatching(FetchChore chore)
  {
    for (int index = 0; index < this.items.Count; ++index)
    {
      if (!Object.op_Equality((Object) this.items[index], (Object) null))
      {
        KPrefabID component = this.items[index].GetComponent<KPrefabID>();
        if (((chore.criteria != FetchChore.MatchCriteria.MatchID || !chore.tags.Contains(component.PrefabTag) ? (chore.criteria != FetchChore.MatchCriteria.MatchTags ? 0 : (component.HasTag(chore.tagsFirst) ? 1 : 0)) : 1) & (!((Tag) ref chore.requiredTag).IsValid ? 1 : (component.HasTag(chore.requiredTag) ? 1 : 0)) & (!component.HasAnyTags(chore.forbiddenTags) ? 1 : 0)) == 0)
        {
          GameObject go = this.items[index];
          this.items.RemoveAt(index);
          --index;
          this.TransferDiseaseWithObject(go);
          this.MakeWorldActive(go);
        }
      }
    }
  }

  public void DropUnlessHasTag(Tag tag)
  {
    for (int index = 0; index < this.items.Count; ++index)
    {
      if (!Object.op_Equality((Object) this.items[index], (Object) null) && !this.items[index].GetComponent<KPrefabID>().HasTag(tag))
      {
        GameObject go = this.items[index];
        this.items.RemoveAt(index);
        --index;
        this.TransferDiseaseWithObject(go);
        this.MakeWorldActive(go);
        Dumpable component = go.GetComponent<Dumpable>();
        if (Object.op_Inequality((Object) component, (Object) null))
          component.Dump(TransformExtensions.GetPosition(this.transform));
      }
    }
  }

  public GameObject Drop(GameObject go, bool do_disease_transfer = true)
  {
    if (Object.op_Equality((Object) go, (Object) null))
      return (GameObject) null;
    int count = this.items.Count;
    for (int index = 0; index < count; ++index)
    {
      if (!Object.op_Inequality((Object) go, (Object) this.items[index]))
      {
        this.items[index] = this.items[count - 1];
        this.items.RemoveAt(count - 1);
        if (do_disease_transfer)
          this.TransferDiseaseWithObject(go);
        this.MakeWorldActive(go);
        break;
      }
    }
    return go;
  }

  public void RenotifyAll()
  {
    this.items.RemoveAll((Predicate<GameObject>) (it => Object.op_Equality((Object) it, (Object) null)));
    foreach (GameObject gameObject in this.items)
      EventExtensions.Trigger(gameObject, 856640610, (object) this);
  }

  private void TransferDiseaseWithObject(GameObject obj)
  {
    if (Object.op_Equality((Object) obj, (Object) null) || !this.doDiseaseTransfer || Object.op_Equality((Object) this.primaryElement, (Object) null))
      return;
    PrimaryElement component = obj.GetComponent<PrimaryElement>();
    if (Object.op_Equality((Object) component, (Object) null))
      return;
    SimUtil.DiseaseInfo invalid1 = SimUtil.DiseaseInfo.Invalid with
    {
      idx = component.DiseaseIdx,
      count = (int) ((double) component.DiseaseCount * 0.05000000074505806)
    };
    SimUtil.DiseaseInfo invalid2 = SimUtil.DiseaseInfo.Invalid with
    {
      idx = this.primaryElement.DiseaseIdx,
      count = (int) ((double) this.primaryElement.DiseaseCount * 0.05000000074505806)
    };
    component.ModifyDiseaseCount(-invalid1.count, "Storage.TransferDiseaseWithObject");
    this.primaryElement.ModifyDiseaseCount(-invalid2.count, "Storage.TransferDiseaseWithObject");
    if (invalid1.count > 0)
      this.primaryElement.AddDisease(invalid1.idx, invalid1.count, "Storage.TransferDiseaseWithObject");
    if (invalid2.count <= 0)
      return;
    component.AddDisease(invalid2.idx, invalid2.count, "Storage.TransferDiseaseWithObject");
  }

  private void MakeWorldActive(GameObject go)
  {
    go.transform.parent = (Transform) null;
    EventExtensions.Trigger(go, 856640610, (object) null);
    this.Trigger(-1697596308, (object) go);
    this.ApplyStoredItemModifiers(go, false, false);
    if (!Object.op_Inequality((Object) go, (Object) null))
      return;
    PrimaryElement component = go.GetComponent<PrimaryElement>();
    if (!Object.op_Inequality((Object) component, (Object) null) || !component.KeepZeroMassObject)
      return;
    component.KeepZeroMassObject = false;
    if ((double) component.Mass > 0.0)
      return;
    Util.KDestroyGameObject(go);
  }

  public List<GameObject> Find(Tag tag, List<GameObject> result)
  {
    for (int index = 0; index < this.items.Count; ++index)
    {
      GameObject go = this.items[index];
      if (!Object.op_Equality((Object) go, (Object) null) && go.HasTag(tag))
        result.Add(go);
    }
    return result;
  }

  public GameObject FindFirst(Tag tag)
  {
    GameObject first = (GameObject) null;
    for (int index = 0; index < this.items.Count; ++index)
    {
      GameObject go = this.items[index];
      if (!Object.op_Equality((Object) go, (Object) null) && go.HasTag(tag))
      {
        first = go;
        break;
      }
    }
    return first;
  }

  public PrimaryElement FindFirstWithMass(Tag tag, float mass = 0.0f)
  {
    PrimaryElement firstWithMass = (PrimaryElement) null;
    for (int index = 0; index < this.items.Count; ++index)
    {
      GameObject go = this.items[index];
      if (!Object.op_Equality((Object) go, (Object) null) && go.HasTag(tag))
      {
        PrimaryElement component = go.GetComponent<PrimaryElement>();
        if ((double) component.Mass > 0.0 && (double) component.Mass >= (double) mass)
        {
          firstWithMass = component;
          break;
        }
      }
    }
    return firstWithMass;
  }

  public HashSet<Tag> GetAllIDsInStorage()
  {
    HashSet<Tag> allIdsInStorage = new HashSet<Tag>();
    for (int index = 0; index < this.items.Count; ++index)
    {
      GameObject go = this.items[index];
      allIdsInStorage.Add(go.PrefabID());
    }
    return allIdsInStorage;
  }

  public GameObject Find(int ID)
  {
    for (int index = 0; index < this.items.Count; ++index)
    {
      GameObject go = this.items[index];
      if (ID == go.PrefabID().GetHashCode())
        return go;
    }
    return (GameObject) null;
  }

  public void ConsumeAllIgnoringDisease() => this.ConsumeAllIgnoringDisease(Tag.Invalid);

  public void ConsumeAllIgnoringDisease(Tag tag)
  {
    for (int index = this.items.Count - 1; index >= 0; --index)
    {
      if (!Tag.op_Inequality(tag, Tag.Invalid) || this.items[index].HasTag(tag))
        this.ConsumeIgnoringDisease(this.items[index]);
    }
  }

  public void ConsumeAndGetDisease(
    Tag tag,
    float amount,
    out float amount_consumed,
    out SimUtil.DiseaseInfo disease_info,
    out float aggregate_temperature)
  {
    DebugUtil.Assert(((Tag) ref tag).IsValid);
    amount_consumed = 0.0f;
    disease_info = SimUtil.DiseaseInfo.Invalid;
    aggregate_temperature = 0.0f;
    bool flag = false;
    for (int index = 0; index < this.items.Count && (double) amount > 0.0; ++index)
    {
      GameObject go = this.items[index];
      if (!Object.op_Equality((Object) go, (Object) null) && go.HasTag(tag))
      {
        PrimaryElement component = go.GetComponent<PrimaryElement>();
        if ((double) component.Units > 0.0)
        {
          flag = true;
          float mass2 = Math.Min(component.Units, amount);
          Debug.Assert((double) mass2 > 0.0, (object) "Delta amount was zero, which should be impossible.");
          aggregate_temperature = SimUtil.CalculateFinalTemperature(amount_consumed, aggregate_temperature, mass2, component.Temperature);
          SimUtil.DiseaseInfo percentOfDisease = SimUtil.GetPercentOfDisease(component, mass2 / component.Units);
          disease_info = SimUtil.CalculateFinalDiseaseInfo(disease_info, percentOfDisease);
          component.Units -= mass2;
          component.ModifyDiseaseCount(-percentOfDisease.count, "Storage.ConsumeAndGetDisease");
          amount -= mass2;
          amount_consumed += mass2;
        }
        if ((double) component.Units <= 0.0 && !component.KeepZeroMassObject)
        {
          if (this.deleted_objects == null)
            this.deleted_objects = new List<GameObject>();
          this.deleted_objects.Add(go);
        }
        this.Trigger(-1697596308, (object) go);
      }
    }
    if (!flag)
      aggregate_temperature = ((Component) this).GetComponent<PrimaryElement>().Temperature;
    if (this.deleted_objects == null)
      return;
    for (int index = 0; index < this.deleted_objects.Count; ++index)
    {
      this.items.Remove(this.deleted_objects[index]);
      Util.KDestroyGameObject(this.deleted_objects[index]);
    }
    this.deleted_objects.Clear();
  }

  public void ConsumeAndGetDisease(
    Recipe.Ingredient ingredient,
    out SimUtil.DiseaseInfo disease_info,
    out float temperature)
  {
    this.ConsumeAndGetDisease(ingredient.tag, ingredient.amount, out float _, out disease_info, out temperature);
  }

  public void ConsumeIgnoringDisease(Tag tag, float amount) => this.ConsumeAndGetDisease(tag, amount, out float _, out SimUtil.DiseaseInfo _, out float _);

  public void ConsumeIgnoringDisease(GameObject item_go)
  {
    if (!this.items.Contains(item_go))
      return;
    PrimaryElement component = item_go.GetComponent<PrimaryElement>();
    if (Object.op_Inequality((Object) component, (Object) null) && component.KeepZeroMassObject)
    {
      component.Units = 0.0f;
      component.ModifyDiseaseCount(-component.DiseaseCount, "consume item");
      this.Trigger(-1697596308, (object) item_go);
    }
    else
    {
      this.items.Remove(item_go);
      this.Trigger(-1697596308, (object) item_go);
      TracesExtesions.DeleteObject(item_go);
    }
  }

  public GameObject Drop(int ID) => this.Drop(this.Find(ID), true);

  private void OnDeath(object data) => this.DropAll(true, true, new Vector3());

  public bool IsFull() => (double) this.RemainingCapacity() <= 0.0;

  public bool IsEmpty() => this.items.Count == 0;

  public float Capacity() => this.capacityKg;

  public bool IsEndOfLife() => this.endOfLife;

  public float MassStored()
  {
    float num = 0.0f;
    for (int index = 0; index < this.items.Count; ++index)
    {
      if (!Object.op_Equality((Object) this.items[index], (Object) null))
      {
        PrimaryElement component = this.items[index].GetComponent<PrimaryElement>();
        if (Object.op_Inequality((Object) component, (Object) null))
          num += component.Units * component.MassPerUnit;
      }
    }
    return (float) Mathf.RoundToInt(num * 1000f) / 1000f;
  }

  public float UnitsStored()
  {
    float num = 0.0f;
    for (int index = 0; index < this.items.Count; ++index)
    {
      if (!Object.op_Equality((Object) this.items[index], (Object) null))
      {
        PrimaryElement component = this.items[index].GetComponent<PrimaryElement>();
        if (Object.op_Inequality((Object) component, (Object) null))
          num += component.Units;
      }
    }
    return (float) Mathf.RoundToInt(num * 1000f) / 1000f;
  }

  public bool Has(Tag tag)
  {
    bool flag = false;
    foreach (GameObject gameObject in this.items)
    {
      if (!Object.op_Equality((Object) gameObject, (Object) null))
      {
        PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
        if (((Component) component).HasTag(tag) && (double) component.Mass > 0.0)
        {
          flag = true;
          break;
        }
      }
    }
    return flag;
  }

  public PrimaryElement AddToPrimaryElement(
    SimHashes element,
    float additional_mass,
    float temperature)
  {
    PrimaryElement primaryElement = this.FindPrimaryElement(element);
    if (Object.op_Inequality((Object) primaryElement, (Object) null))
    {
      float finalTemperature = GameUtil.GetFinalTemperature(primaryElement.Temperature, primaryElement.Mass, temperature, additional_mass);
      primaryElement.Mass += additional_mass;
      primaryElement.Temperature = finalTemperature;
    }
    return primaryElement;
  }

  public PrimaryElement FindPrimaryElement(SimHashes element)
  {
    PrimaryElement primaryElement = (PrimaryElement) null;
    foreach (GameObject gameObject in this.items)
    {
      if (!Object.op_Equality((Object) gameObject, (Object) null))
      {
        PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
        if (component.ElementID == element)
        {
          primaryElement = component;
          break;
        }
      }
    }
    return primaryElement;
  }

  public float RemainingCapacity() => this.capacityKg - this.MassStored();

  public bool GetOnlyFetchMarkedItems() => this.onlyFetchMarkedItems;

  public void SetOnlyFetchMarkedItems(bool is_set)
  {
    if (is_set == this.onlyFetchMarkedItems)
      return;
    this.onlyFetchMarkedItems = is_set;
    this.UpdateFetchCategory();
    this.Trigger(644822890, (object) null);
    ((Component) this).GetComponent<KBatchedAnimController>().SetSymbolVisiblity(KAnimHashedString.op_Implicit("sweep"), is_set);
  }

  private void UpdateFetchCategory()
  {
    if (this.fetchCategory == Storage.FetchCategory.Building)
      return;
    this.fetchCategory = this.onlyFetchMarkedItems ? Storage.FetchCategory.StorageSweepOnly : Storage.FetchCategory.GeneralStorage;
  }

  protected override void OnCleanUp()
  {
    if (this.items.Count != 0)
      Debug.LogWarning((object) ("Storage for [" + ((Object) ((Component) this).gameObject).name + "] is being destroyed but it still contains items!"), (Object) ((Component) this).gameObject);
    base.OnCleanUp();
  }

  private void OnQueueDestroyObject(object data)
  {
    this.endOfLife = true;
    this.DropAll(true, offset: new Vector3());
    base.OnCleanUp();
  }

  public void Remove(GameObject go, bool do_disease_transfer = true)
  {
    this.items.Remove(go);
    if (do_disease_transfer)
      this.TransferDiseaseWithObject(go);
    this.Trigger(-1697596308, (object) go);
    this.ApplyStoredItemModifiers(go, false, false);
  }

  public bool ForceStore(Tag tag, float amount)
  {
    Debug.Assert((double) amount < (double) PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT);
    for (int index = 0; index < this.items.Count; ++index)
    {
      GameObject go = this.items[index];
      if (Object.op_Inequality((Object) go, (Object) null) && go.HasTag(tag))
      {
        go.GetComponent<PrimaryElement>().Mass += amount;
        return true;
      }
    }
    return false;
  }

  public float GetAmountAvailable(Tag tag)
  {
    float amountAvailable = 0.0f;
    for (int index = 0; index < this.items.Count; ++index)
    {
      GameObject go = this.items[index];
      if (Object.op_Inequality((Object) go, (Object) null) && go.HasTag(tag))
        amountAvailable += go.GetComponent<PrimaryElement>().Units;
    }
    return amountAvailable;
  }

  public float GetAmountAvailable(Tag tag, Tag[] forbiddenTags = null)
  {
    if (forbiddenTags == null)
      return this.GetAmountAvailable(tag);
    float amountAvailable = 0.0f;
    for (int index = 0; index < this.items.Count; ++index)
    {
      GameObject go = this.items[index];
      if (Object.op_Inequality((Object) go, (Object) null) && go.HasTag(tag) && !go.HasAnyTags(forbiddenTags))
        amountAvailable += go.GetComponent<PrimaryElement>().Units;
    }
    return amountAvailable;
  }

  public float GetUnitsAvailable(Tag tag)
  {
    float unitsAvailable = 0.0f;
    for (int index = 0; index < this.items.Count; ++index)
    {
      GameObject go = this.items[index];
      if (Object.op_Inequality((Object) go, (Object) null) && go.HasTag(tag))
        unitsAvailable += go.GetComponent<PrimaryElement>().Units;
    }
    return unitsAvailable;
  }

  public float GetMassAvailable(Tag tag)
  {
    float massAvailable = 0.0f;
    for (int index = 0; index < this.items.Count; ++index)
    {
      GameObject go = this.items[index];
      if (Object.op_Inequality((Object) go, (Object) null) && go.HasTag(tag))
        massAvailable += go.GetComponent<PrimaryElement>().Mass;
    }
    return massAvailable;
  }

  public float GetMassAvailable(SimHashes element)
  {
    float massAvailable = 0.0f;
    for (int index = 0; index < this.items.Count; ++index)
    {
      GameObject gameObject = this.items[index];
      if (Object.op_Inequality((Object) gameObject, (Object) null))
      {
        PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
        if (component.ElementID == element)
          massAvailable += component.Mass;
      }
    }
    return massAvailable;
  }

  public override List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = base.GetDescriptors(go);
    if (this.showDescriptor)
      descriptors.Add(new Descriptor(string.Format((string) UI.BUILDINGEFFECTS.STORAGECAPACITY, (object) GameUtil.GetFormattedMass(this.Capacity())), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.STORAGECAPACITY, (object) GameUtil.GetFormattedMass(this.Capacity())), (Descriptor.DescriptorType) 1, false));
    return descriptors;
  }

  public static void MakeItemTemperatureInsulated(
    GameObject go,
    bool is_stored,
    bool is_initializing)
  {
    SimTemperatureTransfer component = go.GetComponent<SimTemperatureTransfer>();
    if (Object.op_Equality((Object) component, (Object) null))
      return;
    ((Behaviour) component).enabled = !is_stored;
  }

  public static void MakeItemInvisible(GameObject go, bool is_stored, bool is_initializing)
  {
    if (is_initializing)
      return;
    bool flag = !is_stored;
    KAnimControllerBase component1 = go.GetComponent<KAnimControllerBase>();
    if (Object.op_Inequality((Object) component1, (Object) null) && component1.enabled != flag)
      component1.enabled = flag;
    KSelectable component2 = go.GetComponent<KSelectable>();
    if (!Object.op_Inequality((Object) component2, (Object) null) || ((Behaviour) component2).enabled == flag)
      return;
    ((Behaviour) component2).enabled = flag;
  }

  public static void MakeItemSealed(GameObject go, bool is_stored, bool is_initializing)
  {
    if (!Object.op_Inequality((Object) go, (Object) null))
      return;
    if (is_stored)
      go.GetComponent<KPrefabID>().AddTag(GameTags.Sealed, false);
    else
      go.GetComponent<KPrefabID>().RemoveTag(GameTags.Sealed);
  }

  public static void MakeItemPreserved(GameObject go, bool is_stored, bool is_initializing)
  {
    if (!Object.op_Inequality((Object) go, (Object) null))
      return;
    if (is_stored)
      go.GetComponent<KPrefabID>().AddTag(GameTags.Preserved, false);
    else
      go.GetComponent<KPrefabID>().RemoveTag(GameTags.Preserved);
  }

  private void ApplyStoredItemModifiers(GameObject go, bool is_stored, bool is_initializing)
  {
    List<Storage.StoredItemModifier> storedItemModifers = this.defaultStoredItemModifers;
    for (int index1 = 0; index1 < storedItemModifers.Count; ++index1)
    {
      Storage.StoredItemModifier storedItemModifier = storedItemModifers[index1];
      for (int index2 = 0; index2 < Storage.StoredItemModifierHandlers.Count; ++index2)
      {
        Storage.StoredItemModifierInfo itemModifierHandler = Storage.StoredItemModifierHandlers[index2];
        if (itemModifierHandler.modifier == storedItemModifier)
        {
          itemModifierHandler.toggleState(go, is_stored, is_initializing);
          break;
        }
      }
    }
  }

  protected virtual void OnCopySettings(object data)
  {
    Storage component = ((GameObject) data).GetComponent<Storage>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    this.SetOnlyFetchMarkedItems(component.onlyFetchMarkedItems);
  }

  private void OnPriorityChanged(PrioritySetting priority)
  {
    foreach (GameObject gameObject in this.items)
      EventExtensions.Trigger(gameObject, -1626373771, (object) this);
  }

  private void OnReachableChanged(object data)
  {
    int num = (bool) data ? 1 : 0;
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    if (num != 0)
      component.RemoveStatusItem(Db.Get().BuildingStatusItems.StorageUnreachable);
    else
      component.AddStatusItem(Db.Get().BuildingStatusItems.StorageUnreachable, (object) this);
  }

  private bool ShouldSaveItem(GameObject go)
  {
    bool flag = false;
    if (Object.op_Inequality((Object) go, (Object) null) && Object.op_Inequality((Object) go.GetComponent<SaveLoadRoot>(), (Object) null) && (double) go.GetComponent<PrimaryElement>().Mass > 0.0)
      flag = true;
    return flag;
  }

  public void Serialize(BinaryWriter writer)
  {
    int num = 0;
    int count = this.items.Count;
    for (int index = 0; index < count; ++index)
    {
      if (this.ShouldSaveItem(this.items[index]))
        ++num;
    }
    writer.Write(num);
    if (num == 0 || this.items == null || this.items.Count <= 0)
      return;
    for (int index = 0; index < this.items.Count; ++index)
    {
      GameObject go = this.items[index];
      if (this.ShouldSaveItem(go))
      {
        SaveLoadRoot component = go.GetComponent<SaveLoadRoot>();
        if (Object.op_Inequality((Object) component, (Object) null))
        {
          Tag saveLoadTag = go.GetComponent<KPrefabID>().GetSaveLoadTag();
          string name = ((Tag) ref saveLoadTag).Name;
          IOHelper.WriteKleiString(writer, name);
          component.Save(writer);
        }
        else
          Debug.Log((object) "Tried to save obj in storage but obj has no SaveLoadRoot", (Object) go);
      }
    }
  }

  public void Deserialize(IReader reader)
  {
    double realtimeSinceStartup1 = (double) Time.realtimeSinceStartup;
    float num1 = 0.0f;
    float num2 = 0.0f;
    float num3 = 0.0f;
    this.ClearItems();
    int capacity = reader.ReadInt32();
    this.items = new List<GameObject>(capacity);
    for (int index = 0; index < capacity; ++index)
    {
      float realtimeSinceStartup2 = Time.realtimeSinceStartup;
      Tag tag = TagManager.Create(reader.ReadKleiString());
      SaveLoadRoot saveLoadRoot = SaveLoadRoot.Load(tag, reader);
      num1 += Time.realtimeSinceStartup - realtimeSinceStartup2;
      if (Object.op_Inequality((Object) saveLoadRoot, (Object) null))
      {
        KBatchedAnimController component1 = ((Component) saveLoadRoot).GetComponent<KBatchedAnimController>();
        if (Object.op_Inequality((Object) component1, (Object) null))
          component1.enabled = false;
        saveLoadRoot.SetRegistered(false);
        float realtimeSinceStartup3 = Time.realtimeSinceStartup;
        GameObject gameObject = this.Store(((Component) saveLoadRoot).gameObject, true, true, false, true);
        num2 += Time.realtimeSinceStartup - realtimeSinceStartup3;
        if (Object.op_Inequality((Object) gameObject, (Object) null))
        {
          Pickupable component2 = gameObject.GetComponent<Pickupable>();
          if (Object.op_Inequality((Object) component2, (Object) null))
          {
            float realtimeSinceStartup4 = Time.realtimeSinceStartup;
            component2.OnStore((object) this);
            num3 += Time.realtimeSinceStartup - realtimeSinceStartup4;
          }
          Storable component3 = gameObject.GetComponent<Storable>();
          if (Object.op_Inequality((Object) component3, (Object) null))
          {
            float realtimeSinceStartup5 = Time.realtimeSinceStartup;
            component3.OnStore((object) this);
            num3 += Time.realtimeSinceStartup - realtimeSinceStartup5;
          }
          if (this.dropOnLoad)
            this.Drop(((Component) saveLoadRoot).gameObject, true);
        }
      }
      else
        Debug.LogWarning((object) ("Tried to deserialize " + tag.ToString() + " into storage but failed"), (Object) ((Component) this).gameObject);
    }
  }

  private void ClearItems()
  {
    foreach (GameObject gameObject in this.items)
      TracesExtesions.DeleteObject(gameObject);
    this.items.Clear();
  }

  public void UpdateStoredItemCachedCells()
  {
    foreach (GameObject gameObject in this.items)
    {
      Pickupable component = gameObject.GetComponent<Pickupable>();
      if (Object.op_Inequality((Object) component, (Object) null))
        component.UpdateCachedCellFromStoragePosition();
    }
  }

  public enum StoredItemModifier
  {
    Insulate,
    Hide,
    Seal,
    Preserve,
  }

  public enum FetchCategory
  {
    Building,
    GeneralStorage,
    StorageSweepOnly,
  }

  public enum FXPrefix
  {
    Delivered,
    PickedUp,
  }

  private struct StoredItemModifierInfo
  {
    public Storage.StoredItemModifier modifier;
    public Action<GameObject, bool, bool> toggleState;

    public StoredItemModifierInfo(
      Storage.StoredItemModifier modifier,
      Action<GameObject, bool, bool> toggle_state)
    {
      this.modifier = modifier;
      this.toggleState = toggle_state;
    }
  }
}
