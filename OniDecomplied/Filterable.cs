// Decompiled with JetBrains decompiler
// Type: Filterable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Filterable")]
public class Filterable : KMonoBehaviour
{
  [MyCmpAdd]
  private CopyBuildingSettings copyBuildingSettings;
  [Serialize]
  public Filterable.ElementState filterElementState;
  [Serialize]
  private Tag selectedTag = GameTags.Void;
  private static TagSet filterableCategories = new TagSet(new TagSet[4]
  {
    GameTags.CalorieCategories,
    GameTags.UnitCategories,
    GameTags.MaterialCategories,
    GameTags.MaterialBuildingElements
  });
  private static readonly Operational.Flag filterSelected = new Operational.Flag(nameof (filterSelected), Operational.Flag.Type.Requirement);
  private static readonly EventSystem.IntraObjectHandler<Filterable> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<Filterable>((Action<Filterable, object>) ((component, data) => component.OnCopySettings(data)));

  public event Action<Tag> onFilterChanged;

  public Tag SelectedTag
  {
    get => this.selectedTag;
    set
    {
      this.selectedTag = value;
      this.OnFilterChanged();
    }
  }

  public Dictionary<Tag, HashSet<Tag>> GetTagOptions()
  {
    Dictionary<Tag, HashSet<Tag>> tagOptions = new Dictionary<Tag, HashSet<Tag>>();
    if (this.filterElementState == Filterable.ElementState.Solid)
    {
      tagOptions = DiscoveredResources.Instance.GetDiscoveredResourcesFromTagSet(Filterable.filterableCategories);
    }
    else
    {
      foreach (Element element in ElementLoader.elements)
      {
        if (!element.disabled && (element.IsGas && this.filterElementState == Filterable.ElementState.Gas || element.IsLiquid && this.filterElementState == Filterable.ElementState.Liquid))
        {
          Tag materialCategoryTag = element.GetMaterialCategoryTag();
          if (!tagOptions.ContainsKey(materialCategoryTag))
            tagOptions[materialCategoryTag] = new HashSet<Tag>();
          Tag tag = GameTagExtensions.Create(element.id);
          tagOptions[materialCategoryTag].Add(tag);
        }
      }
    }
    Dictionary<Tag, HashSet<Tag>> dictionary = tagOptions;
    Tag key = GameTags.Void;
    HashSet<Tag> tagSet = new HashSet<Tag>();
    tagSet.Add(GameTags.Void);
    dictionary.Add(key, tagSet);
    return tagOptions;
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<Filterable>(-905833192, Filterable.OnCopySettingsDelegate);
  }

  private void OnCopySettings(object data)
  {
    Filterable component = ((GameObject) data).GetComponent<Filterable>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    this.SelectedTag = component.SelectedTag;
  }

  protected virtual void OnSpawn() => this.OnFilterChanged();

  private void OnFilterChanged()
  {
    if (this.onFilterChanged != null)
      this.onFilterChanged(this.selectedTag);
    Operational component = ((Component) this).GetComponent<Operational>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.SetFlag(Filterable.filterSelected, ((Tag) ref this.selectedTag).IsValid);
  }

  public enum ElementState
  {
    None,
    Solid,
    Liquid,
    Gas,
  }
}
