// Decompiled with JetBrains decompiler
// Type: FetchList2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class FetchList2 : IFetchList
{
  private System.Action OnComplete;
  private ChoreType choreType;
  public Guid waitingForMaterialsHandle = Guid.Empty;
  public Guid materialsUnavailableForRefillHandle = Guid.Empty;
  public Guid materialsUnavailableHandle = Guid.Empty;
  public Dictionary<Tag, float> MinimumAmount = new Dictionary<Tag, float>();
  public List<FetchOrder2> FetchOrders = new List<FetchOrder2>();
  private Dictionary<Tag, float> Remaining = new Dictionary<Tag, float>();
  private bool bShowStatusItem = true;

  public bool ShowStatusItem
  {
    get => this.bShowStatusItem;
    set => this.bShowStatusItem = value;
  }

  public bool IsComplete => this.FetchOrders.Count == 0;

  public bool InProgress
  {
    get
    {
      if (this.FetchOrders.Count < 0)
        return false;
      bool inProgress = false;
      foreach (FetchOrder2 fetchOrder in this.FetchOrders)
      {
        if (fetchOrder.InProgress)
        {
          inProgress = true;
          break;
        }
      }
      return inProgress;
    }
  }

  public Storage Destination { get; private set; }

  public int PriorityMod { get; private set; }

  public FetchList2(Storage destination, ChoreType chore_type)
  {
    this.Destination = destination;
    this.choreType = chore_type;
  }

  public void SetPriorityMod(int priorityMod)
  {
    this.PriorityMod = priorityMod;
    for (int index = 0; index < this.FetchOrders.Count; ++index)
      this.FetchOrders[index].SetPriorityMod(this.PriorityMod);
  }

  public void Add(
    HashSet<Tag> tags,
    Tag[] forbidden_tags = null,
    float amount = 1f,
    Operational.State operationalRequirementDEPRECATED = Operational.State.None)
  {
    foreach (Tag tag in tags)
    {
      if (!this.MinimumAmount.ContainsKey(tag))
        this.MinimumAmount[tag] = amount;
    }
    this.FetchOrders.Add(new FetchOrder2(this.choreType, tags, FetchChore.MatchCriteria.MatchID, Tag.Invalid, forbidden_tags, this.Destination, amount, operationalRequirementDEPRECATED, this.PriorityMod));
  }

  public void Add(
    Tag tag,
    Tag[] forbidden_tags = null,
    float amount = 1f,
    Operational.State operationalRequirementDEPRECATED = Operational.State.None)
  {
    if (!this.MinimumAmount.ContainsKey(tag))
      this.MinimumAmount[tag] = amount;
    ChoreType choreType = this.choreType;
    HashSet<Tag> tags = new HashSet<Tag>();
    tags.Add(tag);
    Tag invalid = Tag.Invalid;
    Tag[] forbidden_tags1 = forbidden_tags;
    Storage destination = this.Destination;
    double amount1 = (double) amount;
    int operationalRequirementDEPRECATED1 = (int) operationalRequirementDEPRECATED;
    int priorityMod = this.PriorityMod;
    this.FetchOrders.Add(new FetchOrder2(choreType, tags, FetchChore.MatchCriteria.MatchTags, invalid, forbidden_tags1, destination, (float) amount1, (Operational.State) operationalRequirementDEPRECATED1, priorityMod));
  }

  public float GetMinimumAmount(Tag tag)
  {
    float minimumAmount = 0.0f;
    this.MinimumAmount.TryGetValue(tag, out minimumAmount);
    return minimumAmount;
  }

  private void OnFetchOrderComplete(FetchOrder2 fetch_order, Pickupable fetched_item)
  {
    this.FetchOrders.Remove(fetch_order);
    if (this.FetchOrders.Count != 0)
      return;
    if (this.OnComplete != null)
      this.OnComplete();
    FetchListStatusItemUpdater.instance.RemoveFetchList(this);
    this.ClearStatus();
  }

  public void Cancel(string reason)
  {
    FetchListStatusItemUpdater.instance.RemoveFetchList(this);
    this.ClearStatus();
    foreach (FetchOrder2 fetchOrder in this.FetchOrders)
      fetchOrder.Cancel(reason);
  }

  public void UpdateRemaining()
  {
    this.Remaining.Clear();
    for (int index = 0; index < this.FetchOrders.Count; ++index)
    {
      FetchOrder2 fetchOrder = this.FetchOrders[index];
      foreach (Tag tag in fetchOrder.Tags)
      {
        float num = 0.0f;
        this.Remaining.TryGetValue(tag, out num);
        this.Remaining[tag] = num + fetchOrder.AmountWaitingToFetch();
      }
    }
  }

  public Dictionary<Tag, float> GetRemaining() => this.Remaining;

  public Dictionary<Tag, float> GetRemainingMinimum()
  {
    Dictionary<Tag, float> remainingMinimum = new Dictionary<Tag, float>();
    foreach (FetchOrder2 fetchOrder in this.FetchOrders)
    {
      foreach (Tag tag in fetchOrder.Tags)
        remainingMinimum[tag] = this.MinimumAmount[tag];
    }
    foreach (GameObject gameObject in this.Destination.items)
    {
      if (Object.op_Inequality((Object) gameObject, (Object) null))
      {
        Pickupable component = gameObject.GetComponent<Pickupable>();
        if (Object.op_Inequality((Object) component, (Object) null))
        {
          KPrefabID kprefabId = component.KPrefabID;
          if (remainingMinimum.ContainsKey(kprefabId.PrefabTag))
            remainingMinimum[kprefabId.PrefabTag] = Math.Max(remainingMinimum[kprefabId.PrefabTag] - component.TotalAmount, 0.0f);
          foreach (Tag tag in kprefabId.Tags)
          {
            if (remainingMinimum.ContainsKey(tag))
              remainingMinimum[tag] = Math.Max(remainingMinimum[tag] - component.TotalAmount, 0.0f);
          }
        }
      }
    }
    return remainingMinimum;
  }

  public void Suspend(string reason)
  {
    foreach (FetchOrder2 fetchOrder in this.FetchOrders)
      fetchOrder.Suspend(reason);
  }

  public void Resume(string reason)
  {
    foreach (FetchOrder2 fetchOrder in this.FetchOrders)
      fetchOrder.Resume(reason);
  }

  public void Submit(System.Action on_complete, bool check_storage_contents)
  {
    this.OnComplete = on_complete;
    foreach (FetchOrder2 fetchOrder2 in this.FetchOrders.GetRange(0, this.FetchOrders.Count))
      fetchOrder2.Submit(new Action<FetchOrder2, Pickupable>(this.OnFetchOrderComplete), check_storage_contents);
    if (this.IsComplete || !this.ShowStatusItem)
      return;
    FetchListStatusItemUpdater.instance.AddFetchList(this);
  }

  private void ClearStatus()
  {
    if (!Object.op_Inequality((Object) this.Destination, (Object) null))
      return;
    KSelectable component = ((Component) this.Destination).GetComponent<KSelectable>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    this.waitingForMaterialsHandle = component.RemoveStatusItem(this.waitingForMaterialsHandle);
    this.materialsUnavailableHandle = component.RemoveStatusItem(this.materialsUnavailableHandle);
    this.materialsUnavailableForRefillHandle = component.RemoveStatusItem(this.materialsUnavailableForRefillHandle);
  }

  public void UpdateStatusItem(MaterialsStatusItem status_item, ref Guid handle, bool should_add)
  {
    bool flag = handle != Guid.Empty;
    if (should_add == flag)
      return;
    if (should_add)
    {
      KSelectable component = ((Component) this.Destination).GetComponent<KSelectable>();
      if (!Object.op_Inequality((Object) component, (Object) null))
        return;
      handle = component.AddStatusItem((StatusItem) status_item, (object) this);
      GameScheduler.Instance.Schedule("Digging Tutorial", 2f, (Action<object>) (obj => Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Digging)), (object) null, (SchedulerGroup) null);
    }
    else
    {
      KSelectable component = ((Component) this.Destination).GetComponent<KSelectable>();
      if (!Object.op_Inequality((Object) component, (Object) null))
        return;
      handle = component.RemoveStatusItem(handle);
    }
  }
}
