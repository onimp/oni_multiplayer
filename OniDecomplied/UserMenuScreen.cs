// Decompiled with JetBrains decompiler
// Type: UserMenuScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UserMenuScreen : KIconButtonMenu
{
  private GameObject selected;
  public MinMaxSlider sliderPrefab;
  public GameObject sliderParent;
  public PriorityScreen priorityScreenPrefab;
  public GameObject priorityScreenParent;
  private List<MinMaxSlider> sliders = new List<MinMaxSlider>();
  private List<UserMenu.SliderInfo> slidersInfos = new List<UserMenu.SliderInfo>();
  private List<KIconButtonMenu.ButtonInfo> buttonInfos = new List<KIconButtonMenu.ButtonInfo>();
  private PriorityScreen priorityScreen;

  protected override void OnPrefabInit()
  {
    this.keepMenuOpen = true;
    base.OnPrefabInit();
    this.priorityScreen = Util.KInstantiateUI<PriorityScreen>(((Component) this.priorityScreenPrefab).gameObject, this.priorityScreenParent, false);
    this.priorityScreen.InstantiateButtons(new Action<PrioritySetting>(this.OnPriorityClicked));
    ((Component) this.buttonParent).transform.SetAsLastSibling();
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    Game.Instance.Subscribe(1980521255, new Action<object>(this.OnUIRefresh));
    // ISSUE: method pointer
    KInputManager.InputChange.AddListener(new UnityAction((object) this, __methodptr(RefreshButtonTooltip)));
  }

  protected virtual void OnForcedCleanUp()
  {
    // ISSUE: method pointer
    KInputManager.InputChange.RemoveListener(new UnityAction((object) this, __methodptr(RefreshButtonTooltip)));
    ((KMonoBehaviour) this).OnForcedCleanUp();
  }

  public void SetSelected(GameObject go)
  {
    this.ClearPrioritizable();
    this.selected = go;
    this.RefreshPrioritizable();
  }

  private void ClearPrioritizable()
  {
    if (!Object.op_Inequality((Object) this.selected, (Object) null))
      return;
    Prioritizable component = this.selected.GetComponent<Prioritizable>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.onPriorityChanged -= new Action<PrioritySetting>(this.OnPriorityChanged);
  }

  private void RefreshPrioritizable()
  {
    if (!Object.op_Inequality((Object) this.selected, (Object) null))
      return;
    Prioritizable component = this.selected.GetComponent<Prioritizable>();
    if (Object.op_Inequality((Object) component, (Object) null) && component.IsPrioritizable())
    {
      component.onPriorityChanged += new Action<PrioritySetting>(this.OnPriorityChanged);
      ((Component) this.priorityScreen).gameObject.SetActive(true);
      this.priorityScreen.SetScreenPriority(component.GetMasterPriority());
    }
    else
      ((Component) this.priorityScreen).gameObject.SetActive(false);
  }

  public void Refresh(GameObject go)
  {
    if (Object.op_Inequality((Object) go, (Object) this.selected))
      return;
    this.buttonInfos.Clear();
    this.slidersInfos.Clear();
    Game.Instance.userMenu.AppendToScreen(go, this);
    this.SetButtons((IList<KIconButtonMenu.ButtonInfo>) this.buttonInfos);
    this.RefreshButtons();
    this.RefreshSliders();
    this.ClearPrioritizable();
    this.RefreshPrioritizable();
    if ((this.sliders == null || this.sliders.Count == 0) && (this.buttonInfos == null || this.buttonInfos.Count == 0) && !((Component) this.priorityScreen).gameObject.activeSelf)
      ((Component) ((KMonoBehaviour) this).transform.parent).gameObject.SetActive(false);
    else
      ((Component) ((KMonoBehaviour) this).transform.parent).gameObject.SetActive(true);
  }

  public void AddSliders(IList<UserMenu.SliderInfo> sliders) => this.slidersInfos.AddRange((IEnumerable<UserMenu.SliderInfo>) sliders);

  public void AddButtons(IList<KIconButtonMenu.ButtonInfo> buttons) => this.buttonInfos.AddRange((IEnumerable<KIconButtonMenu.ButtonInfo>) buttons);

  private void OnUIRefresh(object data) => this.Refresh(data as GameObject);

  public void RefreshSliders()
  {
    if (this.sliders != null)
    {
      for (int index = 0; index < this.sliders.Count; ++index)
        Object.Destroy((Object) ((Component) this.sliders[index]).gameObject);
      this.sliders = (List<MinMaxSlider>) null;
    }
    if (this.slidersInfos == null || this.slidersInfos.Count == 0)
      return;
    this.sliders = new List<MinMaxSlider>();
    for (int index = 0; index < this.slidersInfos.Count; ++index)
    {
      GameObject gameObject = Object.Instantiate<GameObject>(((Component) this.sliderPrefab).gameObject, Vector3.zero, Quaternion.identity);
      this.slidersInfos[index].sliderGO = gameObject;
      MinMaxSlider component = gameObject.GetComponent<MinMaxSlider>();
      this.sliders.Add(component);
      Transform transform = Object.op_Inequality((Object) this.sliderParent, (Object) null) ? this.sliderParent.transform : ((KMonoBehaviour) this).transform;
      gameObject.transform.SetParent(transform, false);
      gameObject.SetActive(true);
      ((Object) gameObject).name = "Slider";
      if (Object.op_Implicit((Object) component.toolTip))
        component.toolTip.toolTip = this.slidersInfos[index].toolTip;
      component.lockType = this.slidersInfos[index].lockType;
      component.interactable = this.slidersInfos[index].interactable;
      component.minLimit = this.slidersInfos[index].minLimit;
      component.maxLimit = this.slidersInfos[index].maxLimit;
      component.currentMinValue = this.slidersInfos[index].currentMinValue;
      component.currentMaxValue = this.slidersInfos[index].currentMaxValue;
      component.onMinChange = this.slidersInfos[index].onMinChange;
      component.onMaxChange = this.slidersInfos[index].onMaxChange;
      component.direction = this.slidersInfos[index].direction;
      component.SetMode(this.slidersInfos[index].mode);
      component.SetMinMaxValue(this.slidersInfos[index].currentMinValue, this.slidersInfos[index].currentMaxValue, this.slidersInfos[index].minLimit, this.slidersInfos[index].maxLimit);
    }
  }

  private void OnPriorityClicked(PrioritySetting priority)
  {
    if (!Object.op_Inequality((Object) this.selected, (Object) null))
      return;
    Prioritizable component = this.selected.GetComponent<Prioritizable>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.SetMasterPriority(priority);
  }

  private void OnPriorityChanged(PrioritySetting priority) => this.priorityScreen.SetScreenPriority(priority);
}
