// Decompiled with JetBrains decompiler
// Type: CrewRationsScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CrewRationsScreen : CrewListScreen<CrewRationsEntry>
{
  [SerializeField]
  private KButton closebutton;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.closebutton.onClick += (System.Action) (() => ManagementMenu.Instance.CloseAll());
  }

  protected override void OnCmpEnable()
  {
    base.OnCmpEnable();
    this.RefreshCrewPortraitContent();
    this.SortByPreviousSelected();
  }

  private void SortByPreviousSelected()
  {
    if (Object.op_Equality((Object) this.sortToggleGroup, (Object) null) || Object.op_Equality((Object) this.lastSortToggle, (Object) null))
      return;
    for (int index = 0; index < this.ColumnTitlesContainer.childCount; ++index)
    {
      OverviewColumnIdentity component = ((Component) this.ColumnTitlesContainer.GetChild(index)).GetComponent<OverviewColumnIdentity>();
      if (Object.op_Equality((Object) ((Component) this.ColumnTitlesContainer.GetChild(index)).GetComponent<Toggle>(), (Object) this.lastSortToggle))
      {
        if (component.columnID == "name")
          this.SortByName(this.lastSortReversed);
        if (component.columnID == "health")
          this.SortByAmount("HitPoints", this.lastSortReversed);
        if (component.columnID == "stress")
          this.SortByAmount("Stress", this.lastSortReversed);
        if (component.columnID == "calories")
          this.SortByAmount("Calories", this.lastSortReversed);
      }
    }
  }

  protected override void PositionColumnTitles()
  {
    base.PositionColumnTitles();
    for (int index = 0; index < this.ColumnTitlesContainer.childCount; ++index)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      CrewRationsScreen.\u003C\u003Ec__DisplayClass4_0 cDisplayClass40 = new CrewRationsScreen.\u003C\u003Ec__DisplayClass4_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass40.\u003C\u003E4__this = this;
      OverviewColumnIdentity component = ((Component) this.ColumnTitlesContainer.GetChild(index)).GetComponent<OverviewColumnIdentity>();
      if (component.Sortable)
      {
        // ISSUE: reference to a compiler-generated field
        cDisplayClass40.toggle = ((Component) this.ColumnTitlesContainer.GetChild(index)).GetComponent<Toggle>();
        // ISSUE: reference to a compiler-generated field
        cDisplayClass40.toggle.group = this.sortToggleGroup;
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        cDisplayClass40.toggleImage = ((Component) cDisplayClass40.toggle).GetComponentInChildren<ImageToggleState>(true);
        if (component.columnID == "name")
        {
          // ISSUE: reference to a compiler-generated field
          // ISSUE: method pointer
          ((UnityEvent<bool>) cDisplayClass40.toggle.onValueChanged).AddListener(new UnityAction<bool>((object) cDisplayClass40, __methodptr(\u003CPositionColumnTitles\u003Eb__0)));
        }
        if (component.columnID == "health")
        {
          // ISSUE: reference to a compiler-generated field
          // ISSUE: method pointer
          ((UnityEvent<bool>) cDisplayClass40.toggle.onValueChanged).AddListener(new UnityAction<bool>((object) cDisplayClass40, __methodptr(\u003CPositionColumnTitles\u003Eb__1)));
        }
        if (component.columnID == "stress")
        {
          // ISSUE: reference to a compiler-generated field
          // ISSUE: method pointer
          ((UnityEvent<bool>) cDisplayClass40.toggle.onValueChanged).AddListener(new UnityAction<bool>((object) cDisplayClass40, __methodptr(\u003CPositionColumnTitles\u003Eb__2)));
        }
        if (component.columnID == "calories")
        {
          // ISSUE: reference to a compiler-generated field
          // ISSUE: method pointer
          ((UnityEvent<bool>) cDisplayClass40.toggle.onValueChanged).AddListener(new UnityAction<bool>((object) cDisplayClass40, __methodptr(\u003CPositionColumnTitles\u003Eb__3)));
        }
      }
    }
  }

  protected override void SpawnEntries()
  {
    base.SpawnEntries();
    foreach (MinionIdentity _identity in Components.LiveMinionIdentities.Items)
    {
      CrewRationsEntry component = Util.KInstantiateUI(this.Prefab_CrewEntry, ((Component) this.EntriesPanelTransform).gameObject, false).GetComponent<CrewRationsEntry>();
      component.Populate(_identity);
      this.EntryObjects.Add(component);
    }
    this.SortByPreviousSelected();
  }

  public override void ScreenUpdate(bool topLevel)
  {
    base.ScreenUpdate(topLevel);
    foreach (CrewListEntry entryObject in this.EntryObjects)
      entryObject.Refresh();
  }

  private void SortByAmount(string amount_id, bool reverse)
  {
    List<CrewRationsEntry> sortedEntries = new List<CrewRationsEntry>((IEnumerable<CrewRationsEntry>) this.EntryObjects);
    sortedEntries.Sort((Comparison<CrewRationsEntry>) ((a, b) => a.Identity.GetAmounts().GetValue(amount_id).CompareTo(b.Identity.GetAmounts().GetValue(amount_id))));
    this.ReorderEntries(sortedEntries, reverse);
  }

  private void ResetSortToggles(Toggle exceptToggle)
  {
    for (int index = 0; index < this.ColumnTitlesContainer.childCount; ++index)
    {
      Toggle component = ((Component) this.ColumnTitlesContainer.GetChild(index)).GetComponent<Toggle>();
      ImageToggleState componentInChildren = ((Component) component).GetComponentInChildren<ImageToggleState>(true);
      if (Object.op_Inequality((Object) component, (Object) exceptToggle))
        componentInChildren.SetDisabled();
    }
  }
}
