// Decompiled with JetBrains decompiler
// Type: CrewJobsEntry
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CrewJobsEntry : CrewListEntry
{
  public GameObject Prefab_JobPriorityButton;
  public GameObject Prefab_JobPriorityButtonAllTasks;
  private List<CrewJobsEntry.PriorityButton> PriorityButtons = new List<CrewJobsEntry.PriorityButton>();
  private CrewJobsEntry.PriorityButton AllTasksButton;
  public TextStyleSetting TooltipTextStyle_Title;
  public TextStyleSetting TooltipTextStyle_Ability;
  public TextStyleSetting TooltipTextStyle_AbilityPositiveModifier;
  public TextStyleSetting TooltipTextStyle_AbilityNegativeModifier;
  private bool dirty;
  private CrewJobsScreen.everyoneToggleState rowToggleState;

  public ChoreConsumer consumer { get; private set; }

  public override void Populate(MinionIdentity _identity)
  {
    base.Populate(_identity);
    this.consumer = ((Component) _identity).GetComponent<ChoreConsumer>();
    this.consumer.choreRulesChanged += new System.Action(this.Dirty);
    foreach (ChoreGroup resource in Db.Get().ChoreGroups.resources)
      this.CreateChoreButton(resource);
    this.CreateAllTaskButton();
    this.dirty = true;
  }

  private void CreateChoreButton(ChoreGroup chore_group)
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    CrewJobsEntry.\u003C\u003Ec__DisplayClass16_0 cDisplayClass160 = new CrewJobsEntry.\u003C\u003Ec__DisplayClass16_0();
    // ISSUE: reference to a compiler-generated field
    cDisplayClass160.\u003C\u003E4__this = this;
    // ISSUE: reference to a compiler-generated field
    cDisplayClass160.chore_group = chore_group;
    GameObject gameObject = Util.KInstantiateUI(this.Prefab_JobPriorityButton, ((Component) this.transform).gameObject, false);
    // ISSUE: reference to a compiler-generated field
    gameObject.GetComponent<OverviewColumnIdentity>().columnID = cDisplayClass160.chore_group.Id;
    // ISSUE: reference to a compiler-generated field
    gameObject.GetComponent<OverviewColumnIdentity>().Column_DisplayName = cDisplayClass160.chore_group.Name;
    // ISSUE: reference to a compiler-generated field
    cDisplayClass160.priorityButton = new CrewJobsEntry.PriorityButton();
    // ISSUE: reference to a compiler-generated field
    cDisplayClass160.priorityButton.button = gameObject.GetComponent<Button>();
    // ISSUE: reference to a compiler-generated field
    cDisplayClass160.priorityButton.border = ((Component) gameObject.transform.GetChild(1)).GetComponent<Image>();
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    cDisplayClass160.priorityButton.baseBorderColor = ((Graphic) cDisplayClass160.priorityButton.border).color;
    // ISSUE: reference to a compiler-generated field
    cDisplayClass160.priorityButton.background = ((Component) gameObject.transform.GetChild(0)).GetComponent<Image>();
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    cDisplayClass160.priorityButton.baseBackgroundColor = ((Graphic) cDisplayClass160.priorityButton.background).color;
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    cDisplayClass160.priorityButton.choreGroup = cDisplayClass160.chore_group;
    // ISSUE: reference to a compiler-generated field
    cDisplayClass160.priorityButton.ToggleIcon = ((Component) gameObject.transform.GetChild(2)).gameObject;
    // ISSUE: reference to a compiler-generated field
    cDisplayClass160.priorityButton.tooltip = gameObject.GetComponent<ToolTip>();
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated method
    cDisplayClass160.priorityButton.tooltip.OnToolTip = new Func<string>(cDisplayClass160.\u003CCreateChoreButton\u003Eb__0);
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    ((UnityEvent) cDisplayClass160.priorityButton.button.onClick).AddListener(new UnityAction((object) cDisplayClass160, __methodptr(\u003CCreateChoreButton\u003Eb__1)));
    // ISSUE: reference to a compiler-generated field
    this.PriorityButtons.Add(cDisplayClass160.priorityButton);
  }

  private void CreateAllTaskButton()
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    CrewJobsEntry.\u003C\u003Ec__DisplayClass17_0 cDisplayClass170 = new CrewJobsEntry.\u003C\u003Ec__DisplayClass17_0();
    // ISSUE: reference to a compiler-generated field
    cDisplayClass170.\u003C\u003E4__this = this;
    GameObject gameObject = Util.KInstantiateUI(this.Prefab_JobPriorityButtonAllTasks, ((Component) this.transform).gameObject, false);
    gameObject.GetComponent<OverviewColumnIdentity>().columnID = "AllTasks";
    gameObject.GetComponent<OverviewColumnIdentity>().Column_DisplayName = "";
    // ISSUE: reference to a compiler-generated field
    cDisplayClass170.b = gameObject.GetComponent<Button>();
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    ((UnityEvent) cDisplayClass170.b.onClick).AddListener(new UnityAction((object) cDisplayClass170, __methodptr(\u003CCreateAllTaskButton\u003Eb__0)));
    CrewJobsEntry.PriorityButton priorityButton = new CrewJobsEntry.PriorityButton()
    {
      button = gameObject.GetComponent<Button>(),
      border = ((Component) gameObject.transform.GetChild(1)).GetComponent<Image>()
    };
    priorityButton.baseBorderColor = ((Graphic) priorityButton.border).color;
    priorityButton.background = ((Component) gameObject.transform.GetChild(0)).GetComponent<Image>();
    priorityButton.baseBackgroundColor = ((Graphic) priorityButton.background).color;
    priorityButton.ToggleIcon = ((Component) gameObject.transform.GetChild(2)).gameObject;
    priorityButton.tooltip = gameObject.GetComponent<ToolTip>();
    this.AllTasksButton = priorityButton;
  }

  private void ToggleTasksAll(Button button)
  {
    bool is_allowed = this.rowToggleState != CrewJobsScreen.everyoneToggleState.on;
    string name = "HUD_Click_Deselect";
    if (is_allowed)
      name = "HUD_Click";
    KMonoBehaviour.PlaySound(GlobalAssets.GetSound(name));
    foreach (ChoreGroup resource in Db.Get().ChoreGroups.resources)
      this.consumer.SetPermittedByUser(resource, is_allowed);
  }

  private void OnPriorityPress(ChoreGroup chore_group)
  {
    int num = this.consumer.IsPermittedByUser(chore_group) ? 1 : 0;
    string name = "HUD_Click";
    if (num != 0)
      name = "HUD_Click_Deselect";
    KMonoBehaviour.PlaySound(GlobalAssets.GetSound(name));
    this.consumer.SetPermittedByUser(chore_group, !this.consumer.IsPermittedByUser(chore_group));
  }

  private void Refresh(object data = null)
  {
    if (Object.op_Equality((Object) this.identity, (Object) null))
    {
      this.dirty = false;
    }
    else
    {
      if (!this.dirty)
        return;
      Attributes attributes = this.identity.GetAttributes();
      foreach (CrewJobsEntry.PriorityButton priorityButton in this.PriorityButtons)
      {
        bool flag1 = this.consumer.IsPermittedByUser(priorityButton.choreGroup);
        if (priorityButton.ToggleIcon.activeSelf != flag1)
          priorityButton.ToggleIcon.SetActive(flag1);
        float num = Mathf.Min(attributes.Get(priorityButton.choreGroup.attribute).GetTotalValue() / 10f, 1f);
        Color baseBorderColor = priorityButton.baseBorderColor;
        baseBorderColor.r = Mathf.Lerp(priorityButton.baseBorderColor.r, 0.721568644f, num);
        baseBorderColor.g = Mathf.Lerp(priorityButton.baseBorderColor.g, 0.443137258f, num);
        baseBorderColor.b = Mathf.Lerp(priorityButton.baseBorderColor.b, 0.5803922f, num);
        if (Color.op_Inequality(((Graphic) priorityButton.border).color, baseBorderColor))
          ((Graphic) priorityButton.border).color = baseBorderColor;
        Color color = priorityButton.baseBackgroundColor;
        color.a = Mathf.Lerp(0.0f, 1f, num);
        bool flag2 = this.consumer.IsPermittedByTraits(priorityButton.choreGroup);
        if (!flag2)
        {
          color = Color.clear;
          ((Graphic) priorityButton.border).color = Color.clear;
          priorityButton.ToggleIcon.SetActive(false);
        }
        ((Selectable) priorityButton.button).interactable = flag2;
        if (Color.op_Inequality(((Graphic) priorityButton.background).color, color))
          ((Graphic) priorityButton.background).color = color;
      }
      int num1 = 0;
      int num2 = 0;
      foreach (ChoreGroup resource in Db.Get().ChoreGroups.resources)
      {
        if (this.consumer.IsPermittedByTraits(resource))
        {
          ++num2;
          if (this.consumer.IsPermittedByUser(resource))
            ++num1;
        }
      }
      this.rowToggleState = num1 != 0 ? (num1 >= num2 ? CrewJobsScreen.everyoneToggleState.on : CrewJobsScreen.everyoneToggleState.mixed) : CrewJobsScreen.everyoneToggleState.off;
      ImageToggleState component = this.AllTasksButton.ToggleIcon.GetComponent<ImageToggleState>();
      switch (this.rowToggleState)
      {
        case CrewJobsScreen.everyoneToggleState.off:
          component.SetDisabled();
          break;
        case CrewJobsScreen.everyoneToggleState.mixed:
          component.SetInactive();
          break;
        case CrewJobsScreen.everyoneToggleState.on:
          component.SetActive();
          break;
      }
      this.dirty = false;
    }
  }

  private string OnPriorityButtonTooltip(CrewJobsEntry.PriorityButton b)
  {
    b.tooltip.ClearMultiStringTooltip();
    if (Object.op_Inequality((Object) this.identity, (Object) null))
    {
      Attributes attributes = this.identity.GetAttributes();
      if (attributes != null)
      {
        if (!this.consumer.IsPermittedByTraits(b.choreGroup))
        {
          string str = string.Format((string) STRINGS.UI.TOOLTIPS.JOBSSCREEN_CANNOTPERFORMTASK, (object) ((Component) this.consumer).GetComponent<MinionIdentity>().GetProperName());
          b.tooltip.AddMultiStringTooltip(str, this.TooltipTextStyle_AbilityNegativeModifier);
          return "";
        }
        b.tooltip.AddMultiStringTooltip((string) STRINGS.UI.TOOLTIPS.JOBSSCREEN_RELEVANT_ATTRIBUTES, this.TooltipTextStyle_Ability);
        Klei.AI.Attribute attribute = b.choreGroup.attribute;
        AttributeInstance attributeInstance = attributes.Get(attribute);
        float totalValue = attributeInstance.GetTotalValue();
        TextStyleSetting textStyleSetting = this.TooltipTextStyle_Ability;
        if ((double) totalValue > 0.0)
          textStyleSetting = this.TooltipTextStyle_AbilityPositiveModifier;
        else if ((double) totalValue < 0.0)
          textStyleSetting = this.TooltipTextStyle_AbilityNegativeModifier;
        b.tooltip.AddMultiStringTooltip(attribute.Name + " " + attributeInstance.GetTotalValue().ToString(), textStyleSetting);
      }
    }
    return "";
  }

  private void LateUpdate() => this.Refresh((object) null);

  private void OnLevelUp(object data) => this.Dirty();

  private void Dirty()
  {
    this.dirty = true;
    CrewJobsScreen.Instance.Dirty();
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    if (!Object.op_Inequality((Object) this.consumer, (Object) null))
      return;
    this.consumer.choreRulesChanged -= new System.Action(this.Dirty);
  }

  [Serializable]
  public struct PriorityButton
  {
    public Button button;
    public GameObject ToggleIcon;
    public ChoreGroup choreGroup;
    public ToolTip tooltip;
    public Image border;
    public Image background;
    public Color baseBorderColor;
    public Color baseBackgroundColor;
  }
}
