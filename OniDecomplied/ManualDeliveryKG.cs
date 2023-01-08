// Decompiled with JetBrains decompiler
// Type: ManualDeliveryKG
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/ManualDeliveryKG")]
public class ManualDeliveryKG : KMonoBehaviour, ISim1000ms
{
  private static ObjectPool<ManualDeliveryKG.Request> requestPool = new ObjectPool<ManualDeliveryKG.Request>((Func<ManualDeliveryKG.Request>) (() => new ManualDeliveryKG.Request()), 64);
  public float capacity = 100f;
  public float refillMass = 10f;
  public bool allowPause;
  public bool RoundFetchAmountToInt;
  public HashedString choreTypeIDHash;
  public Operational.State operationalRequirement;
  [SerializeField]
  private float minimumMass = 10f;
  [SerializeField]
  private Storage storage;
  [SerializeField]
  private bool paused;
  [Serialize]
  private bool userPaused;
  [MyCmpGet]
  private Operational operational;
  public bool ShowStatusItem = true;
  [SerializeField]
  private List<ManualDeliveryKG.Request> deliveryRequests = new List<ManualDeliveryKG.Request>();
  private FetchList2 fetchList;
  private Tag[] forbiddenTags;
  private int onStorageChangeSubscription = -1;
  private static readonly EventSystem.IntraObjectHandler<ManualDeliveryKG> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<ManualDeliveryKG>((Action<ManualDeliveryKG, object>) ((component, data) => component.OnRefreshUserMenu(data)));
  private static readonly EventSystem.IntraObjectHandler<ManualDeliveryKG> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<ManualDeliveryKG>((Action<ManualDeliveryKG, object>) ((component, data) => component.OnOperationalChanged(data)));
  private static readonly EventSystem.IntraObjectHandler<ManualDeliveryKG> OnStorageChangedDelegate = new EventSystem.IntraObjectHandler<ManualDeliveryKG>((Action<ManualDeliveryKG, object>) ((component, data) => component.OnStorageChanged(data)));

  public Tag RequestedItemTag
  {
    get => this.deliveryRequests.Count == 0 ? Tag.Invalid : this.deliveryRequests[0].Id;
    set
    {
      this.AbortDelivery("Requested Item Tag Changed");
      this.ClearRequests();
      this.RequestItemInternal(value, this.minimumMass);
    }
  }

  public Tag[] ForbiddenTags
  {
    get => this.forbiddenTags;
    set
    {
      this.forbiddenTags = value;
      this.AbortDelivery("Forbidden Tags Changed");
    }
  }

  public bool IsPaused => this.paused;

  public float Capacity => this.capacity;

  public float MinimumMass
  {
    get => this.minimumMass;
    set
    {
      this.minimumMass = value;
      if (this.deliveryRequests == null || this.deliveryRequests.Count != 1)
        return;
      this.deliveryRequests[0].MinimumAmountKG = this.minimumMass;
    }
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    DebugUtil.Assert(((HashedString) ref this.choreTypeIDHash).IsValid, "ManualDeliveryKG Must have a valid chore type specified!", ((Object) this).name);
    if (this.allowPause)
    {
      this.Subscribe<ManualDeliveryKG>(493375141, ManualDeliveryKG.OnRefreshUserMenuDelegate);
      this.Subscribe<ManualDeliveryKG>(-111137758, ManualDeliveryKG.OnRefreshUserMenuDelegate);
    }
    this.Subscribe<ManualDeliveryKG>(-592767678, ManualDeliveryKG.OnOperationalChangedDelegate);
    if (Object.op_Inequality((Object) this.storage, (Object) null))
      this.SetStorage(this.storage);
    Prioritizable.AddRef(((Component) this).gameObject);
    if (!this.userPaused || !this.allowPause)
      return;
    this.OnPause();
  }

  protected virtual void OnCleanUp()
  {
    this.AbortDelivery("ManualDeliverKG destroyed");
    Prioritizable.RemoveRef(((Component) this).gameObject);
    base.OnCleanUp();
  }

  public void SetStorage(Storage storage)
  {
    if (Object.op_Inequality((Object) this.storage, (Object) null))
    {
      this.storage.Unsubscribe(this.onStorageChangeSubscription);
      this.onStorageChangeSubscription = -1;
    }
    this.AbortDelivery("storage pointer changed");
    this.storage = storage;
    if (!Object.op_Inequality((Object) this.storage, (Object) null) || !this.isSpawned)
      return;
    Debug.Assert(this.onStorageChangeSubscription == -1);
    this.onStorageChangeSubscription = this.storage.Subscribe<ManualDeliveryKG>(-1697596308, ManualDeliveryKG.OnStorageChangedDelegate);
  }

  public void Pause(bool pause, string reason)
  {
    if (this.paused == pause)
      return;
    this.paused = pause;
    if (!pause)
      return;
    this.AbortDelivery(reason);
  }

  public void ClearRequests()
  {
    for (int index = this.deliveryRequests.Count - 1; index >= 0; --index)
    {
      this.deliveryRequests[index].Reset();
      ManualDeliveryKG.requestPool.ReleaseInstance(this.deliveryRequests[index]);
      this.deliveryRequests.RemoveAt(index);
    }
  }

  public void RequestItem(Tag id, float minimumAmountKg) => this.RequestItemInternal(id, minimumAmountKg);

  public void RequestItem(Tag[] idSet, float minimumAmountKg) => this.RequestItemInternal(Tag.Invalid, minimumAmountKg, idSet);

  private void RequestItemInternal(Tag id, float minimumAmountKg, Tag[] idSet = null)
  {
    ManualDeliveryKG.Request instance = ManualDeliveryKG.requestPool.GetInstance();
    instance.Id = id;
    instance.MinimumAmountKG = minimumAmountKg;
    for (int index = 0; idSet != null && index < idSet.Length; ++index)
      instance.IdSet.Add(idSet[index]);
    this.deliveryRequests.Add(instance);
  }

  public void Sim1000ms(float dt) => this.UpdateDeliveryState();

  [ContextMenu("UpdateDeliveryState")]
  public void UpdateDeliveryState()
  {
    if (this.deliveryRequests == null || Object.op_Equality((Object) this.storage, (Object) null))
      return;
    this.UpdateFetchList();
  }

  private void CalculateDeliveryStats(
    out float storedMass,
    out float requestKG,
    out bool requiresRefill)
  {
    requestKG = 0.0f;
    storedMass = 0.0f;
    float num1 = 0.0f;
    float num2 = float.PositiveInfinity;
    for (int index = 0; index < this.deliveryRequests.Count; ++index)
    {
      ManualDeliveryKG.Request deliveryRequest = this.deliveryRequests[index];
      if (deliveryRequest.IdSet.Count == 0)
      {
        deliveryRequest.LastStoredAmount = this.storage.GetMassAvailable(deliveryRequest.Id);
      }
      else
      {
        deliveryRequest.LastStoredAmount = 0.0f;
        foreach (Tag id in deliveryRequest.IdSet)
          deliveryRequest.LastStoredAmount += this.storage.GetMassAvailable(id);
      }
      if ((double) deliveryRequest.LastStoredAmount < (double) num2)
      {
        num1 = deliveryRequest.MinimumAmountKG;
        num2 = deliveryRequest.LastStoredAmount;
      }
      storedMass += deliveryRequest.LastStoredAmount;
      requestKG += deliveryRequest.MinimumAmountKG;
    }
    requiresRefill = (double) storedMass <= (double) this.refillMass;
    if ((double) requestKG <= 0.0)
      return;
    requiresRefill |= (double) num2 <= (double) num1 / (double) requestKG * (double) this.refillMass;
  }

  private void RequestDeliveryInternal(float storedMass, float requestKG)
  {
    if ((double) storedMass >= (double) this.capacity)
      return;
    this.fetchList = new FetchList2(this.storage, Db.Get().ChoreTypes.GetByHash(this.choreTypeIDHash));
    for (int index = 0; index < this.deliveryRequests.Count; ++index)
    {
      ManualDeliveryKG.Request deliveryRequest = this.deliveryRequests[index];
      float num1 = this.capacity * (deliveryRequest.MinimumAmountKG / requestKG) - deliveryRequest.LastStoredAmount;
      if ((double) num1 > (double) Mathf.Epsilon)
      {
        if (this.RoundFetchAmountToInt)
          num1 = (float) (int) num1;
        float num2 = Mathf.Max(PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT, num1);
        this.fetchList.MinimumAmount[deliveryRequest.Id] = Mathf.Max(PICKUPABLETUNING.MINIMUM_PICKABLE_AMOUNT, deliveryRequest.MinimumAmountKG);
        if (deliveryRequest.IdSet.Count == 0)
        {
          FetchList2 fetchList = this.fetchList;
          Tag id = deliveryRequest.Id;
          float num3 = num2;
          Tag[] forbiddenTags = this.forbiddenTags;
          double amount = (double) num3;
          fetchList.Add(id, forbiddenTags, (float) amount);
        }
        else
        {
          FetchList2 fetchList = this.fetchList;
          HashSet<Tag> idSet = deliveryRequest.IdSet;
          float num4 = num2;
          Tag[] forbiddenTags = this.forbiddenTags;
          double amount = (double) num4;
          fetchList.Add(idSet, forbiddenTags, (float) amount);
        }
      }
    }
    this.fetchList.ShowStatusItem = this.ShowStatusItem;
    this.fetchList.Submit((System.Action) null, false);
  }

  public void RequestDelivery()
  {
    if (this.fetchList != null)
      return;
    float storedMass;
    float requestKG;
    this.CalculateDeliveryStats(out storedMass, out requestKG, out bool _);
    this.RequestDeliveryInternal(storedMass, requestKG);
  }

  private void UpdateFetchList()
  {
    if (this.paused)
      return;
    if (this.fetchList != null && this.fetchList.IsComplete)
      this.fetchList = (FetchList2) null;
    bool flag1 = this.fetchList != null;
    bool flag2 = Object.op_Inequality((Object) this.operational, (Object) null) && !this.operational.MeetsRequirements(this.operationalRequirement);
    if (flag2 & flag1)
    {
      this.fetchList.Cancel("Operational requirements");
      this.fetchList = (FetchList2) null;
    }
    if (flag1 | flag2)
      return;
    float storedMass;
    float requestKG;
    bool requiresRefill;
    this.CalculateDeliveryStats(out storedMass, out requestKG, out requiresRefill);
    if (!requiresRefill)
      return;
    this.RequestDeliveryInternal(storedMass, requestKG);
  }

  public void AbortDelivery(string reason)
  {
    if (this.fetchList == null)
      return;
    FetchList2 fetchList = this.fetchList;
    this.fetchList = (FetchList2) null;
    string reason1 = reason;
    fetchList.Cancel(reason1);
  }

  protected void OnStorageChanged(object data) => this.UpdateDeliveryState();

  private void OnPause()
  {
    this.userPaused = true;
    this.Pause(true, "Forbid manual delivery");
  }

  private void OnResume()
  {
    this.userPaused = false;
    this.Pause(false, "Allow manual delivery");
  }

  private void OnRefreshUserMenu(object data)
  {
    if (!this.allowPause)
      return;
    Game.Instance.userMenu.AddButton(((Component) this).gameObject, !this.paused ? new KIconButtonMenu.ButtonInfo("action_move_to_storage", (string) UI.USERMENUACTIONS.MANUAL_DELIVERY.NAME, new System.Action(this.OnPause), tooltipText: ((string) UI.USERMENUACTIONS.MANUAL_DELIVERY.TOOLTIP)) : new KIconButtonMenu.ButtonInfo("action_move_to_storage", (string) UI.USERMENUACTIONS.MANUAL_DELIVERY.NAME_OFF, new System.Action(this.OnResume), tooltipText: ((string) UI.USERMENUACTIONS.MANUAL_DELIVERY.TOOLTIP_OFF)));
  }

  private void OnOperationalChanged(object data) => this.UpdateDeliveryState();

  [Serializable]
  public class Request
  {
    public Tag Id;
    public HashSet<Tag> IdSet = new HashSet<Tag>();
    public float MinimumAmountKG;
    public float LastStoredAmount;

    public void Reset()
    {
      this.Id = Tag.Invalid;
      this.MinimumAmountKG = 0.0f;
      this.LastStoredAmount = 0.0f;
      this.IdSet.Clear();
    }
  }
}
