// Decompiled with JetBrains decompiler
// Type: KIconButtonMenu
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class KIconButtonMenu : KScreen
{
  [SerializeField]
  protected bool followGameObject;
  [SerializeField]
  protected bool keepMenuOpen;
  [SerializeField]
  protected bool automaticNavigation = true;
  [SerializeField]
  protected Transform buttonParent;
  [SerializeField]
  private GameObject buttonPrefab;
  [SerializeField]
  protected Sprite[] icons;
  [SerializeField]
  private ToggleGroup externalToggleGroup;
  protected KToggle currentlySelectedToggle;
  [NonSerialized]
  public GameObject[] buttonObjects;
  [SerializeField]
  public TextStyleSetting ToggleToolTipTextStyleSetting;
  private UnityAction inputChangeReceiver;
  protected GameObject go;
  protected IList<KIconButtonMenu.ButtonInfo> buttons;
  private static readonly EventSystem.IntraObjectHandler<KIconButtonMenu> OnSetActivatorDelegate = new EventSystem.IntraObjectHandler<KIconButtonMenu>((Action<KIconButtonMenu, object>) ((component, data) => component.OnSetActivator(data)));

  protected virtual void OnActivate()
  {
    base.OnActivate();
    this.RefreshButtons();
  }

  public void SetButtons(IList<KIconButtonMenu.ButtonInfo> buttons)
  {
    this.buttons = buttons;
    if (!this.activateOnSpawn)
      return;
    this.RefreshButtons();
  }

  public void RefreshButtonTooltip()
  {
    for (int index = 0; index < this.buttons.Count; ++index)
    {
      KIconButtonMenu.ButtonInfo button = this.buttons[index];
      if (Object.op_Equality((Object) button.buttonGo, (Object) null) || button == null)
        break;
      ToolTip componentInChildren1 = button.buttonGo.GetComponentInChildren<ToolTip>();
      if (button.text != null && button.text != "" && Object.op_Inequality((Object) componentInChildren1, (Object) null))
      {
        componentInChildren1.toolTip = button.GetTooltipText();
        LocText componentInChildren2 = button.buttonGo.GetComponentInChildren<LocText>();
        if (Object.op_Inequality((Object) componentInChildren2, (Object) null))
          ((TMP_Text) componentInChildren2).text = button.text;
      }
    }
  }

  public virtual void RefreshButtons()
  {
    if (this.buttonObjects != null)
    {
      for (int index = 0; index < this.buttonObjects.Length; ++index)
        Object.Destroy((Object) this.buttonObjects[index]);
      this.buttonObjects = (GameObject[]) null;
    }
    if (this.buttons == null || this.buttons.Count == 0)
      return;
    this.buttonObjects = new GameObject[this.buttons.Count];
    for (int index = 0; index < this.buttons.Count; ++index)
    {
      KIconButtonMenu.ButtonInfo button = this.buttons[index];
      if (button != null)
      {
        GameObject binstance = Object.Instantiate<GameObject>(this.buttonPrefab, Vector3.zero, Quaternion.identity);
        button.buttonGo = binstance;
        this.buttonObjects[index] = binstance;
        Transform transform = Object.op_Inequality((Object) this.buttonParent, (Object) null) ? this.buttonParent : ((KMonoBehaviour) this).transform;
        binstance.transform.SetParent(transform, false);
        binstance.SetActive(true);
        ((Object) binstance).name = button.text + "Button";
        KButton component1 = binstance.GetComponent<KButton>();
        if (Object.op_Inequality((Object) component1, (Object) null) && button.onClick != null)
          component1.onClick += button.onClick;
        Image image = (Image) null;
        if (Object.op_Implicit((Object) component1))
          image = component1.fgImage;
        if (Object.op_Inequality((Object) image, (Object) null))
        {
          ((Component) image).gameObject.SetActive(false);
          foreach (Sprite icon in this.icons)
          {
            if (Object.op_Inequality((Object) icon, (Object) null) && ((Object) icon).name == button.iconName)
            {
              image.sprite = icon;
              ((Component) image).gameObject.SetActive(true);
              break;
            }
          }
        }
        if (Object.op_Inequality((Object) button.texture, (Object) null))
        {
          RawImage componentInChildren = binstance.GetComponentInChildren<RawImage>();
          if (Object.op_Inequality((Object) componentInChildren, (Object) null))
          {
            ((Component) componentInChildren).gameObject.SetActive(true);
            componentInChildren.texture = button.texture;
          }
        }
        ToolTip componentInChildren1 = binstance.GetComponentInChildren<ToolTip>();
        if (button.text != null && button.text != "" && Object.op_Inequality((Object) componentInChildren1, (Object) null))
        {
          componentInChildren1.toolTip = button.GetTooltipText();
          LocText componentInChildren2 = binstance.GetComponentInChildren<LocText>();
          if (Object.op_Inequality((Object) componentInChildren2, (Object) null))
            ((TMP_Text) componentInChildren2).text = button.text;
        }
        if (button.onToolTip != null)
          componentInChildren1.OnToolTip = button.onToolTip;
        KIconButtonMenu screen = this;
        System.Action onClick = button.onClick;
        System.Action action = (System.Action) (() =>
        {
          Util.Signal(onClick);
          if (!this.keepMenuOpen && Object.op_Inequality((Object) screen, (Object) null))
            screen.Deactivate();
          if (!Object.op_Inequality((Object) binstance, (Object) null))
            return;
          KToggle component2 = binstance.GetComponent<KToggle>();
          if (!Object.op_Inequality((Object) component2, (Object) null))
            return;
          this.SelectToggle(component2);
        });
        KToggle componentInChildren3 = binstance.GetComponentInChildren<KToggle>();
        if (Object.op_Inequality((Object) componentInChildren3, (Object) null))
        {
          ToggleGroup toggleGroup = ((Component) this).GetComponent<ToggleGroup>();
          if (Object.op_Equality((Object) toggleGroup, (Object) null))
            toggleGroup = this.externalToggleGroup;
          ((Toggle) componentInChildren3).group = toggleGroup;
          componentInChildren3.onClick += action;
          Navigation navigation = ((Selectable) componentInChildren3).navigation;
          ((Navigation) ref navigation).mode = this.automaticNavigation ? (Navigation.Mode) 3 : (Navigation.Mode) 0;
          ((Selectable) componentInChildren3).navigation = navigation;
        }
        else
        {
          KBasicToggle componentInChildren4 = binstance.GetComponentInChildren<KBasicToggle>();
          if (Object.op_Inequality((Object) componentInChildren4, (Object) null))
            componentInChildren4.onClick += action;
        }
        if (Object.op_Inequality((Object) component1, (Object) null))
          component1.isInteractable = button.isInteractable;
        Util.Signal<KIconButtonMenu.ButtonInfo>(button.onCreate, button);
      }
    }
    this.Update();
  }

  public virtual void OnKeyDown(KButtonEvent e)
  {
    if (this.buttons == null || !((Component) this).gameObject.activeSelf || !((Behaviour) this).enabled)
      return;
    for (int index = 0; index < this.buttons.Count; ++index)
    {
      KIconButtonMenu.ButtonInfo button = this.buttons[index];
      if (e.TryConsume(button.shortcutKey))
      {
        this.buttonObjects[index].GetComponent<KButton>().PlayPointerDownSound();
        this.buttonObjects[index].GetComponent<KButton>().SignalClick((KKeyCode) 323);
        break;
      }
    }
    base.OnKeyDown(e);
  }

  protected virtual void OnPrefabInit() => ((KMonoBehaviour) this).Subscribe<KIconButtonMenu>(315865555, KIconButtonMenu.OnSetActivatorDelegate);

  private void OnSetActivator(object data)
  {
    this.go = (GameObject) data;
    this.Update();
  }

  private void Update()
  {
    if (!this.followGameObject || Object.op_Equality((Object) this.go, (Object) null) || Object.op_Equality((Object) this.canvas, (Object) null))
      return;
    Vector3 viewportPoint = Camera.main.WorldToViewportPoint(TransformExtensions.GetPosition(this.go.transform));
    RectTransform component1 = ((Component) this).GetComponent<RectTransform>();
    RectTransform component2 = ((Component) this.canvas).GetComponent<RectTransform>();
    if (!Object.op_Inequality((Object) component1, (Object) null))
      return;
    component1.anchoredPosition = new Vector2((float) ((double) viewportPoint.x * (double) component2.sizeDelta.x - (double) component2.sizeDelta.x * 0.5), (float) ((double) viewportPoint.y * (double) component2.sizeDelta.y - (double) component2.sizeDelta.y * 0.5));
  }

  protected void SelectToggle(KToggle selectedToggle)
  {
    if (Object.op_Equality((Object) EventSystem.current, (Object) null) || !((Behaviour) EventSystem.current).enabled)
      return;
    this.currentlySelectedToggle = !Object.op_Equality((Object) this.currentlySelectedToggle, (Object) selectedToggle) ? selectedToggle : (KToggle) null;
    foreach (GameObject buttonObject in this.buttonObjects)
    {
      KToggle component = buttonObject.GetComponent<KToggle>();
      if (Object.op_Inequality((Object) component, (Object) null))
      {
        if (Object.op_Equality((Object) component, (Object) this.currentlySelectedToggle))
        {
          ((Selectable) component).Select();
          component.isOn = true;
        }
        else
        {
          component.Deselect();
          component.isOn = false;
        }
      }
    }
  }

  public void ClearSelection()
  {
    foreach (GameObject buttonObject in this.buttonObjects)
    {
      KToggle component1 = buttonObject.GetComponent<KToggle>();
      if (Object.op_Inequality((Object) component1, (Object) null))
      {
        component1.Deselect();
        component1.isOn = false;
      }
      else
      {
        KBasicToggle component2 = buttonObject.GetComponent<KBasicToggle>();
        if (Object.op_Inequality((Object) component2, (Object) null))
          component2.isOn = false;
      }
      ImageToggleState component3 = buttonObject.GetComponent<ImageToggleState>();
      if (component3.GetIsActive())
        component3.SetInactive();
    }
    ToggleGroup component = ((Component) this).GetComponent<ToggleGroup>();
    if (Object.op_Inequality((Object) component, (Object) null))
      component.SetAllTogglesOff(true);
    this.SelectToggle((KToggle) null);
  }

  public class ButtonInfo
  {
    public string iconName;
    public string text;
    public string tooltipText;
    public string[] multiText;
    public Action shortcutKey;
    public bool isInteractable;
    public Action<KIconButtonMenu.ButtonInfo> onCreate;
    public System.Action onClick;
    public Func<string> onToolTip;
    public GameObject buttonGo;
    public object userData;
    public Texture texture;

    public ButtonInfo(
      string iconName = "",
      string text = "",
      System.Action on_click = null,
      Action shortcutKey = 275,
      Action<GameObject> on_refresh = null,
      Action<KIconButtonMenu.ButtonInfo> on_create = null,
      Texture texture = null,
      string tooltipText = "",
      bool is_interactable = true)
    {
      this.iconName = iconName;
      this.text = text;
      this.shortcutKey = shortcutKey;
      this.onClick = on_click;
      this.onCreate = on_create;
      this.texture = texture;
      this.tooltipText = tooltipText;
      this.isInteractable = is_interactable;
    }

    public string GetTooltipText()
    {
      string template = this.tooltipText == "" ? this.text : this.tooltipText;
      if (this.shortcutKey != 275)
        template = GameUtil.ReplaceHotkeyString(template, this.shortcutKey);
      return template;
    }

    public delegate void Callback();
  }
}
