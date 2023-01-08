// Decompiled with JetBrains decompiler
// Type: CrewJobsScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CrewJobsScreen : CrewListScreen<CrewJobsEntry>
{
  public static CrewJobsScreen Instance;
  private Dictionary<Button, CrewJobsScreen.everyoneToggleState> EveryoneToggles = new Dictionary<Button, CrewJobsScreen.everyoneToggleState>();
  private KeyValuePair<Button, CrewJobsScreen.everyoneToggleState> EveryoneAllTaskToggle;
  public TextStyleSetting TextStyle_JobTooltip_Title;
  public TextStyleSetting TextStyle_JobTooltip_Description;
  public TextStyleSetting TextStyle_JobTooltip_RelevantAttributes;
  public Toggle SortEveryoneToggle;
  private List<ChoreGroup> choreGroups = new List<ChoreGroup>();
  private bool dirty;
  private float screenWidth;

  protected override void OnActivate()
  {
    CrewJobsScreen.Instance = this;
    foreach (ChoreGroup resource in Db.Get().ChoreGroups.resources)
      this.choreGroups.Add(resource);
    base.OnActivate();
  }

  protected override void OnCmpEnable()
  {
    base.OnCmpEnable();
    this.RefreshCrewPortraitContent();
    this.SortByPreviousSelected();
  }

  protected virtual void OnForcedCleanUp()
  {
    CrewJobsScreen.Instance = (CrewJobsScreen) null;
    ((KMonoBehaviour) this).OnForcedCleanUp();
  }

  protected override void SpawnEntries()
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    CrewJobsScreen.\u003C\u003Ec__DisplayClass14_0 cDisplayClass140 = new CrewJobsScreen.\u003C\u003Ec__DisplayClass14_0();
    // ISSUE: reference to a compiler-generated field
    cDisplayClass140.\u003C\u003E4__this = this;
    base.SpawnEntries();
    foreach (MinionIdentity _identity in Components.LiveMinionIdentities.Items)
    {
      CrewJobsEntry component = Util.KInstantiateUI(this.Prefab_CrewEntry, ((Component) this.EntriesPanelTransform).gameObject, false).GetComponent<CrewJobsEntry>();
      component.Populate(_identity);
      this.EntryObjects.Add(component);
    }
    this.SortEveryoneToggle.group = this.sortToggleGroup;
    // ISSUE: reference to a compiler-generated field
    cDisplayClass140.toggleImage = ((Component) this.SortEveryoneToggle).GetComponentInChildren<ImageToggleState>(true);
    // ISSUE: method pointer
    ((UnityEvent<bool>) this.SortEveryoneToggle.onValueChanged).AddListener(new UnityAction<bool>((object) cDisplayClass140, __methodptr(\u003CSpawnEntries\u003Eb__0)));
    this.SortByPreviousSelected();
    this.dirty = true;
  }

  private void SortByPreviousSelected()
  {
    if (Object.op_Equality((Object) this.sortToggleGroup, (Object) null) || Object.op_Equality((Object) this.lastSortToggle, (Object) null))
      return;
    int childCount = this.ColumnTitlesContainer.childCount;
    for (int index = 0; index < childCount; ++index)
    {
      if (index < this.choreGroups.Count && Object.op_Equality((Object) ((Component) this.ColumnTitlesContainer.GetChild(index).Find("Title")).GetComponentInChildren<Toggle>(), (Object) this.lastSortToggle))
      {
        this.SortByEffectiveness(this.choreGroups[index], this.lastSortReversed, false);
        return;
      }
    }
    if (!Object.op_Equality((Object) this.SortEveryoneToggle, (Object) this.lastSortToggle))
      return;
    this.SortByName(this.lastSortReversed);
  }

  protected override void PositionColumnTitles()
  {
    // ISSUE: unable to decompile the method.
  }

  private string GetJobTooltip(GameObject go)
  {
    ToolTip component1 = go.GetComponent<ToolTip>();
    component1.ClearMultiStringTooltip();
    OverviewColumnIdentity component2 = go.GetComponent<OverviewColumnIdentity>();
    if (component2.columnID != "AllTasks")
    {
      ChoreGroup choreGroup = Db.Get().ChoreGroups.Get(component2.columnID);
      component1.AddMultiStringTooltip(component2.Column_DisplayName, this.TextStyle_JobTooltip_Title);
      component1.AddMultiStringTooltip(choreGroup.description, this.TextStyle_JobTooltip_Description);
      component1.AddMultiStringTooltip("\n", this.TextStyle_JobTooltip_Description);
      component1.AddMultiStringTooltip((string) STRINGS.UI.TOOLTIPS.JOBSSCREEN_ATTRIBUTES, this.TextStyle_JobTooltip_Description);
      component1.AddMultiStringTooltip("•  " + choreGroup.attribute.Name, this.TextStyle_JobTooltip_RelevantAttributes);
    }
    return "";
  }

  private void ToggleAllTasksEveryone()
  {
    string name = "HUD_Click_Deselect";
    if (this.EveryoneAllTaskToggle.Value != CrewJobsScreen.everyoneToggleState.on)
      name = "HUD_Click";
    KMonoBehaviour.PlaySound(GlobalAssets.GetSound(name));
    for (int index = 0; index < this.choreGroups.Count; ++index)
      this.SetJobEveryone(this.EveryoneAllTaskToggle.Value != CrewJobsScreen.everyoneToggleState.on, this.choreGroups[index]);
  }

  private void SetJobEveryone(Button button, ChoreGroup chore_group) => this.SetJobEveryone(this.EveryoneToggles[button] != CrewJobsScreen.everyoneToggleState.on, chore_group);

  private void SetJobEveryone(bool state, ChoreGroup chore_group)
  {
    foreach (CrewJobsEntry entryObject in this.EntryObjects)
      entryObject.consumer.SetPermittedByUser(chore_group, state);
  }

  private void ToggleJobEveryone(Button button, ChoreGroup chore_group)
  {
    string name = "HUD_Click_Deselect";
    if (this.EveryoneToggles[button] != CrewJobsScreen.everyoneToggleState.on)
      name = "HUD_Click";
    KMonoBehaviour.PlaySound(GlobalAssets.GetSound(name));
    foreach (CrewJobsEntry entryObject in this.EntryObjects)
      entryObject.consumer.SetPermittedByUser(chore_group, this.EveryoneToggles[button] != CrewJobsScreen.everyoneToggleState.on);
  }

  private void SortByEffectiveness(ChoreGroup chore_group, bool reverse, bool playSound)
  {
    if (playSound)
      KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click"));
    List<CrewJobsEntry> sortedEntries = new List<CrewJobsEntry>((IEnumerable<CrewJobsEntry>) this.EntryObjects);
    sortedEntries.Sort((Comparison<CrewJobsEntry>) ((a, b) => a.Identity.GetAttributes().GetValue(chore_group.attribute.Id).CompareTo(b.Identity.GetAttributes().GetValue(chore_group.attribute.Id))));
    this.ReorderEntries(sortedEntries, reverse);
  }

  private void ResetSortToggles(Toggle exceptToggle)
  {
    for (int index = 0; index < this.ColumnTitlesContainer.childCount; ++index)
    {
      Toggle componentInChildren1 = ((Component) this.ColumnTitlesContainer.GetChild(index).Find("Title")).GetComponentInChildren<Toggle>();
      if (!Object.op_Equality((Object) componentInChildren1, (Object) null))
      {
        ImageToggleState componentInChildren2 = ((Component) componentInChildren1).GetComponentInChildren<ImageToggleState>(true);
        if (Object.op_Inequality((Object) componentInChildren1, (Object) exceptToggle))
          componentInChildren2.SetDisabled();
      }
    }
    ImageToggleState componentInChildren = ((Component) this.SortEveryoneToggle).GetComponentInChildren<ImageToggleState>(true);
    if (!Object.op_Inequality((Object) this.SortEveryoneToggle, (Object) exceptToggle))
      return;
    componentInChildren.SetDisabled();
  }

  private void Refresh()
  {
    if (!this.dirty)
      return;
    int childCount = this.ColumnTitlesContainer.childCount;
    bool flag1 = false;
    bool flag2 = false;
    for (int index1 = 0; index1 < childCount; ++index1)
    {
      bool flag3 = false;
      bool flag4 = false;
      if (this.choreGroups.Count - 1 >= index1)
      {
        ChoreGroup choreGroup = this.choreGroups[index1];
        for (int index2 = 0; index2 < this.EntryObjects.Count; ++index2)
        {
          ChoreConsumer consumer = ((Component) this.EntryObjects[index2]).GetComponent<CrewJobsEntry>().consumer;
          if (consumer.IsPermittedByTraits(choreGroup))
          {
            if (consumer.IsPermittedByUser(choreGroup))
            {
              flag3 = true;
              flag1 = true;
            }
            else
            {
              flag4 = true;
              flag2 = true;
            }
          }
        }
        if (flag3 & flag4)
          this.EveryoneToggles[((IEnumerable<KeyValuePair<Button, CrewJobsScreen.everyoneToggleState>>) this.EveryoneToggles).ElementAt<KeyValuePair<Button, CrewJobsScreen.everyoneToggleState>>(index1).Key] = CrewJobsScreen.everyoneToggleState.mixed;
        else if (flag3)
          this.EveryoneToggles[((IEnumerable<KeyValuePair<Button, CrewJobsScreen.everyoneToggleState>>) this.EveryoneToggles).ElementAt<KeyValuePair<Button, CrewJobsScreen.everyoneToggleState>>(index1).Key] = CrewJobsScreen.everyoneToggleState.on;
        else
          this.EveryoneToggles[((IEnumerable<KeyValuePair<Button, CrewJobsScreen.everyoneToggleState>>) this.EveryoneToggles).ElementAt<KeyValuePair<Button, CrewJobsScreen.everyoneToggleState>>(index1).Key] = CrewJobsScreen.everyoneToggleState.off;
        Button componentInChildren = ((Component) this.ColumnTitlesContainer.GetChild(index1)).GetComponentInChildren<Button>();
        ImageToggleState component = ((Component) ((Component) componentInChildren).GetComponentsInChildren<Image>(true)[1]).GetComponent<ImageToggleState>();
        switch (this.EveryoneToggles[componentInChildren])
        {
          case CrewJobsScreen.everyoneToggleState.off:
            component.SetDisabled();
            continue;
          case CrewJobsScreen.everyoneToggleState.mixed:
            component.SetInactive();
            continue;
          case CrewJobsScreen.everyoneToggleState.on:
            component.SetActive();
            continue;
          default:
            continue;
        }
      }
    }
    if (flag1 & flag2)
      this.EveryoneAllTaskToggle = new KeyValuePair<Button, CrewJobsScreen.everyoneToggleState>(this.EveryoneAllTaskToggle.Key, CrewJobsScreen.everyoneToggleState.mixed);
    else if (flag1)
      this.EveryoneAllTaskToggle = new KeyValuePair<Button, CrewJobsScreen.everyoneToggleState>(this.EveryoneAllTaskToggle.Key, CrewJobsScreen.everyoneToggleState.on);
    else if (flag2)
      this.EveryoneAllTaskToggle = new KeyValuePair<Button, CrewJobsScreen.everyoneToggleState>(this.EveryoneAllTaskToggle.Key, CrewJobsScreen.everyoneToggleState.off);
    ImageToggleState component1 = ((Component) ((Component) this.EveryoneAllTaskToggle.Key).GetComponentsInChildren<Image>(true)[1]).GetComponent<ImageToggleState>();
    switch (this.EveryoneAllTaskToggle.Value)
    {
      case CrewJobsScreen.everyoneToggleState.off:
        component1.SetDisabled();
        break;
      case CrewJobsScreen.everyoneToggleState.mixed:
        component1.SetInactive();
        break;
      case CrewJobsScreen.everyoneToggleState.on:
        component1.SetActive();
        break;
    }
    this.screenWidth = Util.rectTransform((Component) this.EntriesPanelTransform).sizeDelta.x;
    ((Component) this.ScrollRectTransform).GetComponent<LayoutElement>().minWidth = this.screenWidth;
    float num = 31f;
    ((Component) this).GetComponent<LayoutElement>().minWidth = this.screenWidth + num;
    this.dirty = false;
  }

  private void Update() => this.Refresh();

  public void Dirty(object data = null) => this.dirty = true;

  public enum everyoneToggleState
  {
    off,
    mixed,
    on,
  }
}
