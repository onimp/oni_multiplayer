// Decompiled with JetBrains decompiler
// Type: FilterSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FilterSideScreen : SideScreenContent
{
  public HierarchyReferences categoryFoldoutPrefab;
  public FilterSideScreenRow elementEntryPrefab;
  public RectTransform elementEntryContainer;
  public Image outputIcon;
  public Image everythingElseIcon;
  public LocText outputElementHeaderLabel;
  public LocText everythingElseHeaderLabel;
  public LocText selectElementHeaderLabel;
  public LocText currentSelectionLabel;
  private static TagNameComparer comparer = new TagNameComparer(GameTags.Void);
  public Dictionary<Tag, HierarchyReferences> categoryToggles = new Dictionary<Tag, HierarchyReferences>();
  public SortedDictionary<Tag, SortedDictionary<Tag, FilterSideScreenRow>> filterRowMap = new SortedDictionary<Tag, SortedDictionary<Tag, FilterSideScreenRow>>((IComparer<Tag>) FilterSideScreen.comparer);
  public bool isLogicFilter;
  private Filterable targetFilterable;

  protected virtual void OnPrefabInit() => base.OnPrefabInit();

  public override bool IsValidForTarget(GameObject target) => (!this.isLogicFilter ? Object.op_Inequality((Object) target.GetComponent<ElementFilter>(), (Object) null) || Object.op_Inequality((Object) target.GetComponent<RocketConduitStorageAccess>(), (Object) null) || Object.op_Inequality((Object) target.GetComponent<DevPump>(), (Object) null) : Object.op_Inequality((Object) target.GetComponent<ConduitElementSensor>(), (Object) null) || Object.op_Inequality((Object) target.GetComponent<LogicElementSensor>(), (Object) null)) && Object.op_Inequality((Object) target.GetComponent<Filterable>(), (Object) null);

  public override void SetTarget(GameObject target)
  {
    base.SetTarget(target);
    this.targetFilterable = target.GetComponent<Filterable>();
    if (Object.op_Equality((Object) this.targetFilterable, (Object) null))
      return;
    switch (this.targetFilterable.filterElementState)
    {
      case Filterable.ElementState.Solid:
        ((TMP_Text) this.everythingElseHeaderLabel).text = (string) STRINGS.UI.UISIDESCREENS.FILTERSIDESCREEN.UNFILTEREDELEMENTS.SOLID;
        break;
      case Filterable.ElementState.Gas:
        ((TMP_Text) this.everythingElseHeaderLabel).text = (string) STRINGS.UI.UISIDESCREENS.FILTERSIDESCREEN.UNFILTEREDELEMENTS.GAS;
        break;
      default:
        ((TMP_Text) this.everythingElseHeaderLabel).text = (string) STRINGS.UI.UISIDESCREENS.FILTERSIDESCREEN.UNFILTEREDELEMENTS.LIQUID;
        break;
    }
    this.Configure(this.targetFilterable);
    this.SetFilterTag(this.targetFilterable.SelectedTag);
  }

  private void ToggleCategory(Tag tag, bool forceOn = false)
  {
    HierarchyReferences categoryToggle = this.categoryToggles[tag];
    if (!Object.op_Inequality((Object) categoryToggle, (Object) null))
      return;
    MultiToggle reference = categoryToggle.GetReference<MultiToggle>("Toggle");
    if (!forceOn)
      reference.NextState();
    else
      reference.ChangeState(1);
    ((Component) categoryToggle.GetReference<RectTransform>("Entries")).gameObject.SetActive(reference.CurrentState != 0);
  }

  private void Configure(Filterable filterable)
  {
    Dictionary<Tag, HashSet<Tag>> tagOptions = filterable.GetTagOptions();
    foreach (KeyValuePair<Tag, HashSet<Tag>> keyValuePair in tagOptions)
    {
      KeyValuePair<Tag, HashSet<Tag>> category_tags = keyValuePair;
      if (!this.filterRowMap.ContainsKey(category_tags.Key))
      {
        if (Tag.op_Inequality(category_tags.Key, GameTags.Void))
        {
          HierarchyReferences hierarchyReferences = Util.KInstantiateUI<HierarchyReferences>(((Component) this.categoryFoldoutPrefab).gameObject, ((Component) this.elementEntryContainer).gameObject, false);
          ((TMP_Text) hierarchyReferences.GetReference<LocText>("Label")).text = category_tags.Key.ProperName();
          hierarchyReferences.GetReference<MultiToggle>("Toggle").onClick = (System.Action) (() => this.ToggleCategory(category_tags.Key));
          this.categoryToggles.Add(category_tags.Key, hierarchyReferences);
        }
        this.filterRowMap[category_tags.Key] = new SortedDictionary<Tag, FilterSideScreenRow>((IComparer<Tag>) FilterSideScreen.comparer);
      }
      else if (Tag.op_Equality(category_tags.Key, GameTags.Void) && !this.filterRowMap.ContainsKey(category_tags.Key))
        this.filterRowMap[category_tags.Key] = new SortedDictionary<Tag, FilterSideScreenRow>((IComparer<Tag>) FilterSideScreen.comparer);
      foreach (Tag tag in category_tags.Value)
      {
        if (!this.filterRowMap[category_tags.Key].ContainsKey(tag))
        {
          FilterSideScreenRow row = Util.KInstantiateUI<FilterSideScreenRow>(((Component) this.elementEntryPrefab).gameObject, ((Component) (Tag.op_Inequality(category_tags.Key, GameTags.Void) ? this.categoryToggles[category_tags.Key].GetReference<RectTransform>("Entries") : this.elementEntryContainer)).gameObject, false);
          row.SetTag(tag);
          row.button.onClick += (System.Action) (() => this.SetFilterTag(row.tag));
          this.filterRowMap[category_tags.Key].Add(row.tag, row);
        }
      }
    }
    int num1 = 0;
    Transform transform = this.filterRowMap[GameTags.Void][GameTags.Void].transform;
    int num2 = num1;
    int num3 = num2 + 1;
    transform.SetSiblingIndex(num2);
    foreach (KeyValuePair<Tag, SortedDictionary<Tag, FilterSideScreenRow>> filterRow in this.filterRowMap)
    {
      if (tagOptions.ContainsKey(filterRow.Key) && tagOptions[filterRow.Key].Count > 0)
      {
        if (Tag.op_Inequality(filterRow.Key, GameTags.Void))
        {
          ((Object) this.categoryToggles[filterRow.Key]).name = "CATE " + num3.ToString();
          this.categoryToggles[filterRow.Key].transform.SetSiblingIndex(num3++);
          ((Component) this.categoryToggles[filterRow.Key]).gameObject.SetActive(true);
        }
        int num4 = 0;
        foreach (KeyValuePair<Tag, FilterSideScreenRow> keyValuePair in filterRow.Value)
        {
          ((Object) keyValuePair.Value).name = "ELE " + num4.ToString();
          keyValuePair.Value.transform.SetSiblingIndex(num4++);
          ((Component) keyValuePair.Value).gameObject.SetActive(tagOptions[filterRow.Key].Contains(keyValuePair.Value.tag));
          if (Tag.op_Inequality(keyValuePair.Key, GameTags.Void) && Tag.op_Equality(keyValuePair.Key, this.targetFilterable.SelectedTag))
            this.ToggleCategory(filterRow.Key, true);
        }
      }
      else if (Tag.op_Inequality(filterRow.Key, GameTags.Void))
        ((Component) this.categoryToggles[filterRow.Key]).gameObject.SetActive(false);
    }
    this.RefreshUI();
  }

  private void SetFilterTag(Tag tag)
  {
    if (Object.op_Equality((Object) this.targetFilterable, (Object) null))
      return;
    if (((Tag) ref tag).IsValid)
      this.targetFilterable.SelectedTag = tag;
    this.RefreshUI();
  }

  private void RefreshUI()
  {
    LocString format;
    switch (this.targetFilterable.filterElementState)
    {
      case Filterable.ElementState.Solid:
        format = STRINGS.UI.UISIDESCREENS.FILTERSIDESCREEN.FILTEREDELEMENT.SOLID;
        break;
      case Filterable.ElementState.Gas:
        format = STRINGS.UI.UISIDESCREENS.FILTERSIDESCREEN.FILTEREDELEMENT.GAS;
        break;
      default:
        format = STRINGS.UI.UISIDESCREENS.FILTERSIDESCREEN.FILTEREDELEMENT.LIQUID;
        break;
    }
    ((TMP_Text) this.currentSelectionLabel).text = string.Format((string) format, (object) STRINGS.UI.UISIDESCREENS.FILTERSIDESCREEN.NOELEMENTSELECTED);
    foreach (KeyValuePair<Tag, SortedDictionary<Tag, FilterSideScreenRow>> filterRow in this.filterRowMap)
    {
      foreach (KeyValuePair<Tag, FilterSideScreenRow> keyValuePair in filterRow.Value)
      {
        bool selected = Tag.op_Equality(keyValuePair.Key, this.targetFilterable.SelectedTag);
        keyValuePair.Value.SetSelected(selected);
        if (selected)
        {
          if (Tag.op_Inequality(keyValuePair.Value.tag, GameTags.Void))
            ((TMP_Text) this.currentSelectionLabel).text = string.Format((string) format, (object) this.targetFilterable.SelectedTag.ProperName());
          else
            ((TMP_Text) this.currentSelectionLabel).text = (string) STRINGS.UI.UISIDESCREENS.FILTERSIDESCREEN.NO_SELECTION;
        }
      }
    }
  }
}
