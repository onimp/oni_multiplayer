// Decompiled with JetBrains decompiler
// Type: CrewListScreen`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CrewListScreen<EntryType> : KScreen where EntryType : CrewListEntry
{
  public GameObject Prefab_CrewEntry;
  public List<EntryType> EntryObjects = new List<EntryType>();
  public Transform ScrollRectTransform;
  public Transform EntriesPanelTransform;
  protected Vector2 EntryRectSize = new Vector2(750f, 64f);
  public int maxEntriesBeforeScroll = 5;
  public Scrollbar PanelScrollbar;
  protected ToggleGroup sortToggleGroup;
  protected Toggle lastSortToggle;
  protected bool lastSortReversed;
  public GameObject Prefab_ColumnTitle;
  public Transform ColumnTitlesContainer;
  public bool autoColumn;
  public float columnTitleHorizontalOffset;

  protected virtual void OnActivate()
  {
    base.OnActivate();
    this.ClearEntries();
    this.SpawnEntries();
    this.PositionColumnTitles();
    if (this.autoColumn)
      this.UpdateColumnTitles();
    this.ConsumeMouseScroll = true;
  }

  protected virtual void OnCmpEnable()
  {
    if (this.autoColumn)
      this.UpdateColumnTitles();
    this.Reconstruct();
  }

  private void ClearEntries()
  {
    for (int index = this.EntryObjects.Count - 1; index > -1; --index)
      Util.KDestroyGameObject((Component) (object) this.EntryObjects[index]);
    this.EntryObjects.Clear();
  }

  protected void RefreshCrewPortraitContent() => this.EntryObjects.ForEach((Action<EntryType>) (eo => eo.RefreshCrewPortraitContent()));

  protected virtual void SpawnEntries()
  {
    if (this.EntryObjects.Count == 0)
      return;
    this.ClearEntries();
  }

  public virtual void ScreenUpdate(bool topLevel)
  {
    base.ScreenUpdate(topLevel);
    if (this.autoColumn)
      this.UpdateColumnTitles();
    bool flag = false;
    List<MinionIdentity> liveIdentities = new List<MinionIdentity>((IEnumerable<MinionIdentity>) Components.LiveMinionIdentities.Items);
    if (this.EntryObjects.Count != liveIdentities.Count || this.EntryObjects.FindAll((Predicate<EntryType>) (o => liveIdentities.Contains(o.Identity))).Count != this.EntryObjects.Count)
      flag = true;
    if (flag)
      this.Reconstruct();
    this.UpdateScroll();
  }

  public void Reconstruct()
  {
    this.ClearEntries();
    this.SpawnEntries();
  }

  private void UpdateScroll()
  {
    if (!Object.op_Implicit((Object) this.PanelScrollbar))
      return;
    if (this.EntryObjects.Count <= this.maxEntriesBeforeScroll)
    {
      this.PanelScrollbar.value = Mathf.Lerp(this.PanelScrollbar.value, 1f, 10f);
      ((Component) this.PanelScrollbar).gameObject.SetActive(false);
    }
    else
      ((Component) this.PanelScrollbar).gameObject.SetActive(true);
  }

  private void SetHeadersActive(bool state)
  {
    for (int index = 0; index < this.ColumnTitlesContainer.childCount; ++index)
      ((Component) this.ColumnTitlesContainer.GetChild(index)).gameObject.SetActive(state);
  }

  protected virtual void PositionColumnTitles()
  {
    if (Object.op_Equality((Object) this.ColumnTitlesContainer, (Object) null))
      return;
    if (this.EntryObjects.Count <= 0)
    {
      this.SetHeadersActive(false);
    }
    else
    {
      this.SetHeadersActive(true);
      int childCount = ((KMonoBehaviour) (object) this.EntryObjects[0]).transform.childCount;
      for (int index = 0; index < childCount; ++index)
      {
        OverviewColumnIdentity component = ((Component) ((KMonoBehaviour) (object) this.EntryObjects[0]).transform.GetChild(index)).GetComponent<OverviewColumnIdentity>();
        if (Object.op_Inequality((Object) component, (Object) null))
        {
          GameObject gameObject = Util.KInstantiate(this.Prefab_ColumnTitle, (GameObject) null, (string) null);
          ((Object) gameObject).name = component.Column_DisplayName;
          LocText componentInChildren = gameObject.GetComponentInChildren<LocText>();
          gameObject.transform.SetParent(this.ColumnTitlesContainer);
          ((TMP_Text) componentInChildren).text = component.StringLookup ? StringEntry.op_Implicit(Strings.Get(component.Column_DisplayName)) : component.Column_DisplayName;
          gameObject.GetComponent<ToolTip>().toolTip = string.Format((string) STRINGS.UI.TOOLTIPS.SORTCOLUMN, (object) ((TMP_Text) componentInChildren).text);
          Util.rectTransform(gameObject).anchoredPosition = new Vector2(Util.rectTransform((Component) component).anchoredPosition.x, 0.0f);
          OverviewColumnIdentity overviewColumnIdentity = gameObject.GetComponent<OverviewColumnIdentity>();
          if (Object.op_Equality((Object) overviewColumnIdentity, (Object) null))
            overviewColumnIdentity = gameObject.AddComponent<OverviewColumnIdentity>();
          overviewColumnIdentity.Column_DisplayName = component.Column_DisplayName;
          overviewColumnIdentity.columnID = component.columnID;
          overviewColumnIdentity.xPivot = component.xPivot;
          overviewColumnIdentity.Sortable = component.Sortable;
          if (overviewColumnIdentity.Sortable)
            ((Component) ((Component) overviewColumnIdentity).GetComponentInChildren<ImageToggleState>(true)).gameObject.SetActive(true);
        }
      }
      this.UpdateColumnTitles();
      this.sortToggleGroup = ((Component) this).gameObject.AddComponent<ToggleGroup>();
      this.sortToggleGroup.allowSwitchOff = true;
    }
  }

  protected void SortByName(bool reverse)
  {
    List<EntryType> sortedEntries = new List<EntryType>((IEnumerable<EntryType>) this.EntryObjects);
    sortedEntries.Sort((Comparison<EntryType>) ((a, b) => (a.Identity.GetProperName() + ((Object) ((Component) (object) a).gameObject).GetInstanceID().ToString()).CompareTo(b.Identity.GetProperName() + ((Object) ((Component) (object) b).gameObject).GetInstanceID().ToString())));
    this.ReorderEntries(sortedEntries, reverse);
  }

  protected void UpdateColumnTitles()
  {
    if (this.EntryObjects.Count <= 0 || !((Component) (object) this.EntryObjects[0]).gameObject.activeSelf)
    {
      this.SetHeadersActive(false);
    }
    else
    {
      this.SetHeadersActive(true);
      for (int index1 = 0; index1 < this.ColumnTitlesContainer.childCount; ++index1)
      {
        RectTransform rectTransform = Util.rectTransform((Component) this.ColumnTitlesContainer.GetChild(index1));
        for (int index2 = 0; index2 < ((KMonoBehaviour) (object) this.EntryObjects[0]).transform.childCount; ++index2)
        {
          OverviewColumnIdentity component = ((Component) ((KMonoBehaviour) (object) this.EntryObjects[0]).transform.GetChild(index2)).GetComponent<OverviewColumnIdentity>();
          if (Object.op_Inequality((Object) component, (Object) null) && component.Column_DisplayName == ((Object) rectTransform).name)
          {
            rectTransform.pivot = new Vector2(component.xPivot, rectTransform.pivot.y);
            rectTransform.anchoredPosition = new Vector2(Util.rectTransform((Component) component).anchoredPosition.x + this.columnTitleHorizontalOffset, 0.0f);
            rectTransform.sizeDelta = new Vector2(Util.rectTransform((Component) component).sizeDelta.x, rectTransform.sizeDelta.y);
            if ((double) rectTransform.anchoredPosition.x == 0.0)
              ((Component) rectTransform).gameObject.SetActive(false);
            else
              ((Component) rectTransform).gameObject.SetActive(true);
          }
        }
      }
    }
  }

  protected void ReorderEntries(List<EntryType> sortedEntries, bool reverse)
  {
    for (int index = 0; index < sortedEntries.Count; ++index)
    {
      if (reverse)
        ((KMonoBehaviour) (object) sortedEntries[index]).transform.SetSiblingIndex(sortedEntries.Count - 1 - index);
      else
        ((KMonoBehaviour) (object) sortedEntries[index]).transform.SetSiblingIndex(index);
    }
  }
}
