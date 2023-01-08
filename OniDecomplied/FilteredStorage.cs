// Decompiled with JetBrains decompiler
// Type: FilteredStorage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FilteredStorage
{
  public static readonly HashedString FULL_PORT_ID = HashedString.op_Implicit("FULL");
  private KMonoBehaviour root;
  private FetchList2 fetchList;
  private IUserControlledCapacity capacityControl;
  private TreeFilterable filterable;
  private Storage storage;
  private MeterController meter;
  private MeterController logicMeter;
  private Tag[] forbiddenTags;
  private bool hasMeter = true;
  private bool useLogicMeter;
  private ChoreType choreType;

  public void SetHasMeter(bool has_meter) => this.hasMeter = has_meter;

  public FilteredStorage(
    KMonoBehaviour root,
    Tag[] forbidden_tags,
    IUserControlledCapacity capacity_control,
    bool use_logic_meter,
    ChoreType fetch_chore_type)
  {
    this.root = root;
    this.forbiddenTags = forbidden_tags;
    this.capacityControl = capacity_control;
    this.useLogicMeter = use_logic_meter;
    this.choreType = fetch_chore_type;
    root.Subscribe(-1697596308, new Action<object>(this.OnStorageChanged));
    root.Subscribe(-543130682, new Action<object>(this.OnUserSettingsChanged));
    this.filterable = root.FindOrAdd<TreeFilterable>();
    this.filterable.OnFilterChanged += new Action<HashSet<Tag>>(this.OnFilterChanged);
    this.storage = ((Component) root).GetComponent<Storage>();
    this.storage.Subscribe(644822890, new Action<object>(this.OnOnlyFetchMarkedItemsSettingChanged));
    this.storage.Subscribe(-1852328367, new Action<object>(this.OnFunctionalChanged));
  }

  private void OnOnlyFetchMarkedItemsSettingChanged(object data) => this.OnFilterChanged(this.filterable.GetTags());

  private void CreateMeter()
  {
    if (!this.hasMeter)
      return;
    this.meter = new MeterController((KAnimControllerBase) ((Component) this.root).GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[2]
    {
      "meter_frame",
      "meter_level"
    });
  }

  private void CreateLogicMeter()
  {
    if (!this.hasMeter)
      return;
    this.logicMeter = new MeterController((KAnimControllerBase) ((Component) this.root).GetComponent<KBatchedAnimController>(), "logicmeter_target", "logicmeter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
  }

  public void SetMeter(MeterController meter)
  {
    this.hasMeter = true;
    this.meter = meter;
    this.UpdateMeter();
  }

  public void CleanUp()
  {
    if (Object.op_Inequality((Object) this.filterable, (Object) null))
      this.filterable.OnFilterChanged -= new Action<HashSet<Tag>>(this.OnFilterChanged);
    if (this.fetchList == null)
      return;
    this.fetchList.Cancel("Parent destroyed");
  }

  public void FilterChanged()
  {
    if (this.hasMeter)
    {
      if (this.meter == null)
        this.CreateMeter();
      if (this.logicMeter == null && this.useLogicMeter)
        this.CreateLogicMeter();
    }
    this.OnFilterChanged(this.filterable.GetTags());
    this.UpdateMeter();
  }

  private void OnUserSettingsChanged(object data)
  {
    this.OnFilterChanged(this.filterable.GetTags());
    this.UpdateMeter();
  }

  private void OnStorageChanged(object data)
  {
    if (this.fetchList == null)
      this.OnFilterChanged(this.filterable.GetTags());
    this.UpdateMeter();
  }

  private void OnFunctionalChanged(object data) => this.OnFilterChanged(this.filterable.GetTags());

  private void UpdateMeter()
  {
    float minusStorageMargin = this.GetMaxCapacityMinusStorageMargin();
    float percent_full = Mathf.Clamp01(this.GetAmountStored() / minusStorageMargin);
    if (this.meter == null)
      return;
    this.meter.SetPositionPercent(percent_full);
  }

  public bool IsFull()
  {
    float minusStorageMargin = this.GetMaxCapacityMinusStorageMargin();
    float percent_full = Mathf.Clamp01(this.GetAmountStored() / minusStorageMargin);
    if (this.meter != null)
      this.meter.SetPositionPercent(percent_full);
    return (double) percent_full >= 1.0;
  }

  private void OnFetchComplete() => this.OnFilterChanged(this.filterable.GetTags());

  private float GetMaxCapacity()
  {
    float maxCapacity = this.storage.capacityKg;
    if (this.capacityControl != null)
      maxCapacity = Mathf.Min(maxCapacity, this.capacityControl.UserMaxCapacity);
    return maxCapacity;
  }

  private float GetMaxCapacityMinusStorageMargin() => this.GetMaxCapacity() - this.storage.storageFullMargin;

  private float GetAmountStored()
  {
    float amountStored = this.storage.MassStored();
    if (this.capacityControl != null)
      amountStored = this.capacityControl.AmountStored;
    return amountStored;
  }

  private bool IsFunctional()
  {
    Operational component = ((Component) this.storage).GetComponent<Operational>();
    return Object.op_Equality((Object) component, (Object) null) || component.IsFunctional;
  }

  private void OnFilterChanged(HashSet<Tag> tags)
  {
    bool flag = tags != null && tags.Count != 0;
    if (this.fetchList != null)
    {
      this.fetchList.Cancel("");
      this.fetchList = (FetchList2) null;
    }
    float minusStorageMargin = this.GetMaxCapacityMinusStorageMargin();
    float amountStored = this.GetAmountStored();
    if (!((double) Mathf.Max(0.0f, minusStorageMargin - amountStored) > 0.0 & flag) || !this.IsFunctional())
      return;
    float amount = Mathf.Max(0.0f, this.GetMaxCapacity() - amountStored);
    this.fetchList = new FetchList2(this.storage, this.choreType);
    this.fetchList.ShowStatusItem = false;
    this.fetchList.Add(tags, this.forbiddenTags, amount, Operational.State.Functional);
    this.fetchList.Submit(new System.Action(this.OnFetchComplete), false);
  }

  public void SetLogicMeter(bool on)
  {
    if (this.logicMeter == null)
      return;
    this.logicMeter.SetPositionPercent(on ? 1f : 0.0f);
  }

  public void AddForbiddenTag(Tag forbidden_tag)
  {
    if (this.forbiddenTags == null)
      this.forbiddenTags = new Tag[0];
    if (((IEnumerable<Tag>) this.forbiddenTags).Contains<Tag>(forbidden_tag))
      return;
    this.forbiddenTags = Util.Append<Tag>(this.forbiddenTags, forbidden_tag);
    this.OnFilterChanged(this.filterable.GetTags());
  }

  public void RemoveForbiddenTag(Tag forbidden_tag)
  {
    if (this.forbiddenTags == null)
      return;
    List<Tag> tagList = new List<Tag>((IEnumerable<Tag>) this.forbiddenTags);
    tagList.Remove(forbidden_tag);
    this.forbiddenTags = tagList.ToArray();
    this.OnFilterChanged(this.filterable.GetTags());
  }
}
