// Decompiled with JetBrains decompiler
// Type: PriorityScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PriorityScreen : KScreen
{
  [SerializeField]
  protected PriorityButton buttonPrefab_basic;
  [SerializeField]
  protected GameObject EmergencyContainer;
  [SerializeField]
  protected PriorityButton button_emergency;
  [SerializeField]
  protected GameObject PriorityMenuContainer;
  [SerializeField]
  protected KButton button_priorityMenu;
  [SerializeField]
  protected KToggle button_toggleHigh;
  [SerializeField]
  protected GameObject diagram;
  protected List<PriorityButton> buttons_basic = new List<PriorityButton>();
  protected List<PriorityButton> buttons_emergency = new List<PriorityButton>();
  private PrioritySetting priority;
  private PrioritySetting lastSelectedPriority = new PrioritySetting(PriorityScreen.PriorityClass.basic, -1);
  private Action<PrioritySetting> onClick;

  public void InstantiateButtons(Action<PrioritySetting> on_click, bool playSelectionSound = true)
  {
    this.onClick = on_click;
    for (int index = 1; index <= 9; ++index)
    {
      int priority_value = index;
      PriorityButton priorityButton = Util.KInstantiateUI<PriorityButton>(((Component) this.buttonPrefab_basic).gameObject, ((Component) this.buttonPrefab_basic.transform.parent).gameObject, false);
      this.buttons_basic.Add(priorityButton);
      priorityButton.playSelectionSound = playSelectionSound;
      priorityButton.onClick = this.onClick;
      ((TMP_Text) priorityButton.text).text = priority_value.ToString();
      priorityButton.priority = new PrioritySetting(PriorityScreen.PriorityClass.basic, priority_value);
      priorityButton.tooltip.SetSimpleTooltip(string.Format((string) STRINGS.UI.PRIORITYSCREEN.BASIC, (object) priority_value));
    }
    ((Component) this.buttonPrefab_basic).gameObject.SetActive(false);
    this.button_emergency.playSelectionSound = playSelectionSound;
    this.button_emergency.onClick = this.onClick;
    this.button_emergency.priority = new PrioritySetting(PriorityScreen.PriorityClass.topPriority, 1);
    this.button_emergency.tooltip.SetSimpleTooltip((string) STRINGS.UI.PRIORITYSCREEN.TOP_PRIORITY);
    ((Component) this.button_toggleHigh).gameObject.SetActive(false);
    this.PriorityMenuContainer.SetActive(true);
    ((Component) this.button_priorityMenu).gameObject.SetActive(true);
    this.button_priorityMenu.onClick += new System.Action(this.PriorityButtonClicked);
    ((Component) this.button_priorityMenu).GetComponent<ToolTip>().SetSimpleTooltip((string) STRINGS.UI.PRIORITYSCREEN.OPEN_JOBS_SCREEN);
    this.diagram.SetActive(false);
    this.SetScreenPriority(new PrioritySetting(PriorityScreen.PriorityClass.basic, 5));
  }

  private void OnClick(PrioritySetting priority)
  {
    if (this.onClick == null)
      return;
    this.onClick(priority);
  }

  public void ShowDiagram(bool show) => this.diagram.SetActive(show);

  public void ResetPriority() => this.SetScreenPriority(new PrioritySetting(PriorityScreen.PriorityClass.basic, 5));

  public void PriorityButtonClicked() => ManagementMenu.Instance.TogglePriorities();

  private void RefreshButton(PriorityButton b, PrioritySetting priority, bool play_sound)
  {
    if (b.priority == priority)
    {
      ((Selectable) b.toggle).Select();
      b.toggle.isOn = true;
      if (!play_sound)
        return;
      ((WidgetSoundPlayer) b.toggle.soundPlayer).Play(0);
    }
    else
      b.toggle.isOn = false;
  }

  public void SetScreenPriority(PrioritySetting priority, bool play_sound = false)
  {
    if (this.lastSelectedPriority == priority)
      return;
    this.lastSelectedPriority = priority;
    if (priority.priority_class == PriorityScreen.PriorityClass.high)
      this.button_toggleHigh.isOn = true;
    else if (priority.priority_class == PriorityScreen.PriorityClass.basic)
      this.button_toggleHigh.isOn = false;
    for (int index = 0; index < this.buttons_basic.Count; ++index)
    {
      this.buttons_basic[index].priority = new PrioritySetting(this.button_toggleHigh.isOn ? PriorityScreen.PriorityClass.high : PriorityScreen.PriorityClass.basic, index + 1);
      this.buttons_basic[index].tooltip.SetSimpleTooltip(string.Format((string) (this.button_toggleHigh.isOn ? STRINGS.UI.PRIORITYSCREEN.HIGH : STRINGS.UI.PRIORITYSCREEN.BASIC), (object) (index + 1)));
      this.RefreshButton(this.buttons_basic[index], this.lastSelectedPriority, play_sound);
    }
    this.RefreshButton(this.button_emergency, this.lastSelectedPriority, play_sound);
  }

  public PrioritySetting GetLastSelectedPriority() => this.lastSelectedPriority;

  public static void PlayPriorityConfirmSound(PrioritySetting priority)
  {
    EventInstance eventInstance = KFMOD.BeginOneShot(GlobalAssets.GetSound("Priority_Tool_Confirm"), Vector3.zero, 1f);
    if (!((EventInstance) ref eventInstance).isValid())
      return;
    float num1 = 0.0f;
    if (priority.priority_class >= PriorityScreen.PriorityClass.high)
      num1 += 10f;
    if (priority.priority_class >= PriorityScreen.PriorityClass.topPriority)
      num1 += 0.0f;
    float num2 = num1 + (float) priority.priority_value;
    ((EventInstance) ref eventInstance).setParameterByName(nameof (priority), num2, false);
    KFMOD.EndOneShot(eventInstance);
  }

  public enum PriorityClass
  {
    idle = -1, // 0xFFFFFFFF
    basic = 0,
    high = 1,
    personalNeeds = 2,
    topPriority = 3,
    compulsory = 4,
  }
}
