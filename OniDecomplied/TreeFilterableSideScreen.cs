// Decompiled with JetBrains decompiler
// Type: TreeFilterableSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TreeFilterableSideScreen : SideScreenContent
{
  [SerializeField]
  private MultiToggle allCheckBox;
  [SerializeField]
  private MultiToggle onlyAllowTransportItemsCheckBox;
  [SerializeField]
  private GameObject onlyallowTransportItemsRow;
  [SerializeField]
  private MultiToggle onlyAllowSpicedItemsCheckBox;
  [SerializeField]
  private GameObject onlyallowSpicedItemsRow;
  [SerializeField]
  private TreeFilterableSideScreenRow rowPrefab;
  [SerializeField]
  private GameObject rowGroup;
  [SerializeField]
  private TreeFilterableSideScreenElement elementPrefab;
  [SerializeField]
  private GameObject titlebar;
  [SerializeField]
  private GameObject contentMask;
  private GameObject target;
  private bool visualDirty;
  private bool initialized;
  private KImage onlyAllowTransportItemsImg;
  public UIPool<TreeFilterableSideScreenElement> elementPool;
  private UIPool<TreeFilterableSideScreenRow> rowPool;
  private TreeFilterable targetFilterable;
  private Dictionary<Tag, TreeFilterableSideScreenRow> tagRowMap = new Dictionary<Tag, TreeFilterableSideScreenRow>();
  private Storage storage;

  public bool IsStorage => Object.op_Inequality((Object) this.storage, (Object) null);

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Initialize();
  }

  private void Initialize()
  {
    if (this.initialized)
      return;
    this.rowPool = new UIPool<TreeFilterableSideScreenRow>(this.rowPrefab);
    this.elementPool = new UIPool<TreeFilterableSideScreenElement>(this.elementPrefab);
    this.allCheckBox.onClick += (System.Action) (() =>
    {
      switch (this.GetAllCheckboxState())
      {
        case TreeFilterableSideScreenRow.State.Off:
        case TreeFilterableSideScreenRow.State.Mixed:
          this.SetAllCheckboxState(TreeFilterableSideScreenRow.State.On);
          break;
        case TreeFilterableSideScreenRow.State.On:
          this.SetAllCheckboxState(TreeFilterableSideScreenRow.State.Off);
          break;
      }
    });
    this.onlyAllowTransportItemsCheckBox.onClick = new System.Action(this.OnlyAllowTransportItemsClicked);
    this.onlyAllowSpicedItemsCheckBox.onClick = new System.Action(this.OnlyAllowSpicedItemsClicked);
    this.initialized = true;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    ((Component) this.allCheckBox.transform.parent.parent).GetComponent<ToolTip>().SetSimpleTooltip((string) STRINGS.UI.UISIDESCREENS.TREEFILTERABLESIDESCREEN.ALLBUTTONTOOLTIP);
    ((Component) this.onlyAllowTransportItemsCheckBox.transform.parent).GetComponent<ToolTip>().SetSimpleTooltip((string) STRINGS.UI.UISIDESCREENS.TREEFILTERABLESIDESCREEN.ONLYALLOWTRANSPORTITEMSBUTTONTOOLTIP);
    ((Component) this.onlyAllowSpicedItemsCheckBox.transform.parent).GetComponent<ToolTip>().SetSimpleTooltip((string) STRINGS.UI.UISIDESCREENS.TREEFILTERABLESIDESCREEN.ONLYALLOWSPICEDITEMSBUTTONTOOLTIP);
  }

  private void UpdateAllCheckBoxVisualState()
  {
    switch (this.GetAllCheckboxState())
    {
      case TreeFilterableSideScreenRow.State.Off:
        this.allCheckBox.ChangeState(0);
        break;
      case TreeFilterableSideScreenRow.State.Mixed:
        this.allCheckBox.ChangeState(1);
        break;
      case TreeFilterableSideScreenRow.State.On:
        this.allCheckBox.ChangeState(2);
        break;
    }
    this.visualDirty = false;
  }

  public void Update()
  {
    foreach (KeyValuePair<Tag, TreeFilterableSideScreenRow> tagRow in this.tagRowMap)
    {
      if (tagRow.Value.visualDirty)
      {
        tagRow.Value.UpdateCheckBoxVisualState();
        this.visualDirty = true;
      }
    }
    if (!this.visualDirty)
      return;
    this.UpdateAllCheckBoxVisualState();
  }

  private void OnlyAllowTransportItemsClicked() => this.storage.SetOnlyFetchMarkedItems(!this.storage.GetOnlyFetchMarkedItems());

  private void OnlyAllowSpicedItemsClicked()
  {
    FoodStorage component = ((Component) this.storage).GetComponent<FoodStorage>();
    component.SpicedFoodOnly = !component.SpicedFoodOnly;
  }

  private TreeFilterableSideScreenRow.State GetAllCheckboxState()
  {
    bool flag1 = false;
    bool flag2 = false;
    bool flag3 = false;
    foreach (KeyValuePair<Tag, TreeFilterableSideScreenRow> tagRow in this.tagRowMap)
    {
      switch (tagRow.Value.GetState())
      {
        case TreeFilterableSideScreenRow.State.Off:
          flag2 = true;
          continue;
        case TreeFilterableSideScreenRow.State.Mixed:
          flag3 = true;
          continue;
        case TreeFilterableSideScreenRow.State.On:
          flag1 = true;
          continue;
        default:
          continue;
      }
    }
    if (flag3)
      return TreeFilterableSideScreenRow.State.Mixed;
    if (flag1 && !flag2)
      return TreeFilterableSideScreenRow.State.On;
    return !flag1 & flag2 || !(flag1 & flag2) ? TreeFilterableSideScreenRow.State.Off : TreeFilterableSideScreenRow.State.Mixed;
  }

  private void SetAllCheckboxState(TreeFilterableSideScreenRow.State newState)
  {
    switch (newState)
    {
      case TreeFilterableSideScreenRow.State.Off:
        using (Dictionary<Tag, TreeFilterableSideScreenRow>.Enumerator enumerator = this.tagRowMap.GetEnumerator())
        {
          while (enumerator.MoveNext())
            enumerator.Current.Value.ChangeCheckBoxState(TreeFilterableSideScreenRow.State.Off);
          break;
        }
      case TreeFilterableSideScreenRow.State.On:
        using (Dictionary<Tag, TreeFilterableSideScreenRow>.Enumerator enumerator = this.tagRowMap.GetEnumerator())
        {
          while (enumerator.MoveNext())
            enumerator.Current.Value.ChangeCheckBoxState(TreeFilterableSideScreenRow.State.On);
          break;
        }
    }
    this.visualDirty = true;
  }

  public bool GetElementTagAcceptedState(Tag t) => this.targetFilterable.ContainsTag(t);

  public override bool IsValidForTarget(GameObject target)
  {
    TreeFilterable component1 = target.GetComponent<TreeFilterable>();
    Storage component2 = target.GetComponent<Storage>();
    if (!Object.op_Inequality((Object) component1, (Object) null) || !Object.op_Equality((Object) target.GetComponent<FlatTagFilterable>(), (Object) null) || !component1.showUserMenu)
      return false;
    return Object.op_Equality((Object) component2, (Object) null) || component2.showInUI;
  }

  public override void SetTarget(GameObject target)
  {
    this.Initialize();
    this.target = target;
    if (Object.op_Equality((Object) target, (Object) null))
    {
      Debug.LogError((object) "The target object provided was null");
    }
    else
    {
      this.targetFilterable = target.GetComponent<TreeFilterable>();
      if (Object.op_Equality((Object) this.targetFilterable, (Object) null))
      {
        Debug.LogError((object) "The target provided does not have a Tree Filterable component");
      }
      else
      {
        this.contentMask.GetComponent<LayoutElement>().minHeight = this.targetFilterable.uiHeight == TreeFilterable.UISideScreenHeight.Tall ? 380f : 256f;
        this.storage = ((Component) this.targetFilterable).GetComponent<Storage>();
        this.storage.Subscribe(644822890, new Action<object>(this.OnOnlyFetchMarkedItemsSettingChanged));
        this.storage.Subscribe(1163645216, new Action<object>(this.OnOnlySpicedItemsSettingChanged));
        this.OnOnlyFetchMarkedItemsSettingChanged((object) null);
        this.OnOnlySpicedItemsSettingChanged((object) null);
        this.CreateCategories();
        this.titlebar.SetActive(false);
        if (!this.storage.showSideScreenTitleBar)
          return;
        this.titlebar.SetActive(true);
        ((TMP_Text) this.titlebar.GetComponentInChildren<LocText>()).SetText(((Component) this.storage).GetProperName());
      }
    }
  }

  private void OnOnlyFetchMarkedItemsSettingChanged(object data)
  {
    this.onlyAllowTransportItemsCheckBox.ChangeState(this.storage.GetOnlyFetchMarkedItems() ? 1 : 0);
    if (this.storage.allowSettingOnlyFetchMarkedItems)
      this.onlyallowTransportItemsRow.SetActive(true);
    else
      this.onlyallowTransportItemsRow.SetActive(false);
  }

  private void OnOnlySpicedItemsSettingChanged(object data)
  {
    FoodStorage component = ((Component) this.storage).GetComponent<FoodStorage>();
    if (Object.op_Inequality((Object) component, (Object) null))
    {
      this.onlyallowSpicedItemsRow.SetActive(true);
      this.onlyAllowSpicedItemsCheckBox.ChangeState(component.SpicedFoodOnly ? 1 : 0);
    }
    else
      this.onlyallowSpicedItemsRow.SetActive(false);
  }

  public bool IsTagAllowed(Tag tag) => this.targetFilterable.AcceptedTags.Contains(tag);

  public void AddTag(Tag tag)
  {
    if (Object.op_Equality((Object) this.targetFilterable, (Object) null))
      return;
    this.targetFilterable.AddTagToFilter(tag);
  }

  public void RemoveTag(Tag tag)
  {
    if (Object.op_Equality((Object) this.targetFilterable, (Object) null))
      return;
    this.targetFilterable.RemoveTagFromFilter(tag);
  }

  private List<TreeFilterableSideScreen.TagOrderInfo> GetTagsSortedAlphabetically(
    ICollection<Tag> tags)
  {
    List<TreeFilterableSideScreen.TagOrderInfo> sortedAlphabetically = new List<TreeFilterableSideScreen.TagOrderInfo>();
    foreach (Tag tag in (IEnumerable<Tag>) tags)
      sortedAlphabetically.Add(new TreeFilterableSideScreen.TagOrderInfo()
      {
        tag = tag,
        strippedName = tag.ProperNameStripLink()
      });
    sortedAlphabetically.Sort((Comparison<TreeFilterableSideScreen.TagOrderInfo>) ((a, b) => a.strippedName.CompareTo(b.strippedName)));
    return sortedAlphabetically;
  }

  private TreeFilterableSideScreenRow AddRow(Tag rowTag)
  {
    if (this.tagRowMap.ContainsKey(rowTag))
      return this.tagRowMap[rowTag];
    TreeFilterableSideScreenRow freeElement = this.rowPool.GetFreeElement(this.rowGroup, true);
    freeElement.Parent = this;
    this.tagRowMap.Add(rowTag, freeElement);
    Dictionary<Tag, bool> filterMap = new Dictionary<Tag, bool>();
    foreach (TreeFilterableSideScreen.TagOrderInfo tagOrderInfo in this.GetTagsSortedAlphabetically((ICollection<Tag>) DiscoveredResources.Instance.GetDiscoveredResourcesFromTag(rowTag)))
      filterMap.Add(tagOrderInfo.tag, this.targetFilterable.ContainsTag(tagOrderInfo.tag) || this.targetFilterable.ContainsTag(rowTag));
    freeElement.SetElement(rowTag, this.targetFilterable.ContainsTag(rowTag), filterMap);
    freeElement.transform.SetAsLastSibling();
    return freeElement;
  }

  public float GetAmountInStorage(Tag tag) => !this.IsStorage ? 0.0f : this.storage.GetMassAvailable(tag);

  private void CreateCategories()
  {
    if (this.storage.storageFilters != null && this.storage.storageFilters.Count >= 1)
    {
      bool flag = Object.op_Inequality((Object) this.target.GetComponent<CreatureDeliveryPoint>(), (Object) null);
      foreach (TreeFilterableSideScreen.TagOrderInfo tagOrderInfo in this.GetTagsSortedAlphabetically((ICollection<Tag>) this.storage.storageFilters))
      {
        Tag tag = tagOrderInfo.tag;
        if ((flag ? 1 : (DiscoveredResources.Instance.IsDiscovered(tag) ? 1 : 0)) != 0)
          this.AddRow(tag);
      }
      this.visualDirty = true;
    }
    else
      Debug.LogError((object) "If you're filtering, your storage filter should have the filters set on it");
  }

  protected virtual void OnCmpDisable()
  {
    ((KMonoBehaviour) this).OnCmpDisable();
    if (Object.op_Inequality((Object) this.storage, (Object) null))
    {
      this.storage.Unsubscribe(644822890, new Action<object>(this.OnOnlyFetchMarkedItemsSettingChanged));
      this.storage.Unsubscribe(1163645216, new Action<object>(this.OnOnlySpicedItemsSettingChanged));
    }
    this.rowPool.ClearAll();
    this.elementPool.ClearAll();
    this.tagRowMap.Clear();
  }

  private struct TagOrderInfo
  {
    public Tag tag;
    public string strippedName;
  }
}
