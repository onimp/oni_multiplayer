// Decompiled with JetBrains decompiler
// Type: TreeFilterable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/TreeFilterable")]
public class TreeFilterable : KMonoBehaviour, ISaveLoadable
{
  [MyCmpReq]
  private Storage storage;
  [MyCmpAdd]
  private CopyBuildingSettings copyBuildingSettings;
  public static readonly Color32 FILTER_TINT = Color32.op_Implicit(Color.white);
  public static readonly Color32 NO_FILTER_TINT = Color32.op_Implicit(new Color(0.5019608f, 0.5019608f, 0.5019608f, 1f));
  public Color32 filterTint = TreeFilterable.FILTER_TINT;
  public Color32 noFilterTint = TreeFilterable.NO_FILTER_TINT;
  [SerializeField]
  public bool dropIncorrectOnFilterChange = true;
  [SerializeField]
  public bool autoSelectStoredOnLoad = true;
  public bool showUserMenu = true;
  public TreeFilterable.UISideScreenHeight uiHeight = TreeFilterable.UISideScreenHeight.Tall;
  public bool filterByStorageCategoriesOnSpawn = true;
  [SerializeField]
  [Serialize]
  [Obsolete("Deprecated, use acceptedTagSet")]
  private List<Tag> acceptedTags = new List<Tag>();
  [SerializeField]
  [Serialize]
  private HashSet<Tag> acceptedTagSet = new HashSet<Tag>();
  public Action<HashSet<Tag>> OnFilterChanged;
  private static readonly EventSystem.IntraObjectHandler<TreeFilterable> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<TreeFilterable>((Action<TreeFilterable, object>) ((component, data) => component.OnCopySettings(data)));

  public HashSet<Tag> AcceptedTags => this.acceptedTagSet;

  [System.Runtime.Serialization.OnDeserialized]
  [Obsolete]
  private void OnDeserialized()
  {
    if (SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 20))
      this.filterByStorageCategoriesOnSpawn = false;
    if (!SaveLoader.Instance.GameInfo.IsVersionOlderThan(7, 29))
      return;
    this.acceptedTagSet.UnionWith((IEnumerable<Tag>) this.acceptedTags);
    this.acceptedTags = (List<Tag>) null;
  }

  private void OnDiscover(Tag category_tag, Tag tag)
  {
    if (!this.storage.storageFilters.Contains(category_tag))
      return;
    bool flag = false;
    if (DiscoveredResources.Instance.GetDiscoveredResourcesFromTag(category_tag).Count <= 1)
    {
      foreach (Tag storageFilter in this.storage.storageFilters)
      {
        if (!Tag.op_Equality(storageFilter, category_tag) && DiscoveredResources.Instance.IsDiscovered(storageFilter))
        {
          flag = true;
          foreach (Tag tag1 in DiscoveredResources.Instance.GetDiscoveredResourcesFromTag(storageFilter))
          {
            if (!this.acceptedTagSet.Contains(tag1))
              return;
          }
        }
      }
      if (!flag)
        return;
    }
    foreach (Tag tag2 in DiscoveredResources.Instance.GetDiscoveredResourcesFromTag(category_tag))
    {
      if (!Tag.op_Equality(tag2, tag) && !this.acceptedTagSet.Contains(tag2))
        return;
    }
    this.AddTagToFilter(tag);
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<TreeFilterable>(-905833192, TreeFilterable.OnCopySettingsDelegate);
  }

  protected virtual void OnSpawn()
  {
    DiscoveredResources.Instance.OnDiscover += new Action<Tag, Tag>(this.OnDiscover);
    if (this.autoSelectStoredOnLoad && Object.op_Inequality((Object) this.storage, (Object) null))
    {
      HashSet<Tag> filters = new HashSet<Tag>((IEnumerable<Tag>) this.acceptedTagSet);
      filters.UnionWith((IEnumerable<Tag>) this.storage.GetAllIDsInStorage());
      this.UpdateFilters(filters);
    }
    if (this.OnFilterChanged != null)
      this.OnFilterChanged(this.acceptedTagSet);
    this.RefreshTint();
    if (!this.filterByStorageCategoriesOnSpawn)
      return;
    this.RemoveIncorrectAcceptedTags();
  }

  private void RemoveIncorrectAcceptedTags()
  {
    List<Tag> tagList = new List<Tag>();
    foreach (Tag acceptedTag in this.acceptedTagSet)
    {
      bool flag = false;
      foreach (Tag storageFilter in this.storage.storageFilters)
      {
        if (DiscoveredResources.Instance.GetDiscoveredResourcesFromTag(storageFilter).Contains(acceptedTag))
        {
          flag = true;
          break;
        }
      }
      if (!flag)
        tagList.Add(acceptedTag);
    }
    foreach (Tag t in tagList)
      this.RemoveTagFromFilter(t);
  }

  protected virtual void OnCleanUp()
  {
    DiscoveredResources.Instance.OnDiscover -= new Action<Tag, Tag>(this.OnDiscover);
    base.OnCleanUp();
  }

  private void OnCopySettings(object data)
  {
    TreeFilterable component = ((GameObject) data).GetComponent<TreeFilterable>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    this.UpdateFilters(component.GetTags());
  }

  public HashSet<Tag> GetTags() => this.acceptedTagSet;

  public bool ContainsTag(Tag t) => this.acceptedTagSet.Contains(t);

  public void AddTagToFilter(Tag t)
  {
    if (this.ContainsTag(t))
      return;
    HashSet<Tag> filters = new HashSet<Tag>((IEnumerable<Tag>) this.acceptedTagSet);
    filters.Add(t);
    this.UpdateFilters(filters);
  }

  public void RemoveTagFromFilter(Tag t)
  {
    if (!this.ContainsTag(t))
      return;
    HashSet<Tag> filters = new HashSet<Tag>((IEnumerable<Tag>) this.acceptedTagSet);
    filters.Remove(t);
    this.UpdateFilters(filters);
  }

  public void UpdateFilters(HashSet<Tag> filters)
  {
    this.acceptedTagSet.Clear();
    this.acceptedTagSet.UnionWith((IEnumerable<Tag>) filters);
    if (this.OnFilterChanged != null)
      this.OnFilterChanged(this.acceptedTagSet);
    this.RefreshTint();
    if (!this.dropIncorrectOnFilterChange || Object.op_Equality((Object) this.storage, (Object) null) || this.storage.items == null)
      return;
    for (int index = this.storage.items.Count - 1; index >= 0; --index)
    {
      GameObject go = this.storage.items[index];
      if (!Object.op_Equality((Object) go, (Object) null) && !this.acceptedTagSet.Contains(go.GetComponent<KPrefabID>().PrefabTag))
        this.storage.Drop(go, true);
    }
  }

  public string GetTagsAsStatus(int maxDisplays = 6)
  {
    string tagsAsStatus = "Tags:\n";
    List<Tag> first = new List<Tag>((IEnumerable<Tag>) this.storage.storageFilters);
    ((IEnumerable<Tag>) first).Intersect<Tag>((IEnumerable<Tag>) this.acceptedTagSet);
    for (int index = 0; index < Mathf.Min(first.Count, maxDisplays); ++index)
    {
      tagsAsStatus += first[index].ProperName();
      if (index < Mathf.Min(first.Count, maxDisplays) - 1)
        tagsAsStatus += "\n";
      if (index == maxDisplays - 1 && first.Count > maxDisplays)
      {
        tagsAsStatus += "\n...";
        break;
      }
    }
    if (((Component) this).tag.Length == 0)
      tagsAsStatus = "No tags selected";
    return tagsAsStatus;
  }

  private void RefreshTint()
  {
    bool flag = this.acceptedTagSet != null && this.acceptedTagSet.Count != 0;
    ((Component) this).GetComponent<KBatchedAnimController>().TintColour = flag ? this.filterTint : this.noFilterTint;
    ((Component) this).GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.NoStorageFilterSet, !flag, (object) this);
  }

  public enum UISideScreenHeight
  {
    Short,
    Tall,
  }
}
