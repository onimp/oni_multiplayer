// Decompiled with JetBrains decompiler
// Type: KButtonMenu
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class KButtonMenu : KScreen
{
  [SerializeField]
  protected bool followGameObject;
  [SerializeField]
  protected bool keepMenuOpen;
  [SerializeField]
  protected Transform buttonParent;
  public GameObject buttonPrefab;
  public bool ShouldConsumeMouseScroll;
  [NonSerialized]
  public GameObject[] buttonObjects;
  protected GameObject go;
  protected IList<KButtonMenu.ButtonInfo> buttons;
  private static readonly EventSystem.IntraObjectHandler<KButtonMenu> OnSetActivatorDelegate = new EventSystem.IntraObjectHandler<KButtonMenu>((Action<KButtonMenu, object>) ((component, data) => component.OnSetActivator(data)));

  protected virtual void OnActivate()
  {
    this.ConsumeMouseScroll = this.ShouldConsumeMouseScroll;
    this.RefreshButtons();
  }

  public void SetButtons(IList<KButtonMenu.ButtonInfo> buttons)
  {
    this.buttons = buttons;
    if (!this.activateOnSpawn)
      return;
    this.RefreshButtons();
  }

  public virtual void RefreshButtons()
  {
    if (this.buttonObjects != null)
    {
      for (int index = 0; index < this.buttonObjects.Length; ++index)
        Object.Destroy((Object) this.buttonObjects[index]);
      this.buttonObjects = (GameObject[]) null;
    }
    if (this.buttons == null)
      return;
    this.buttonObjects = new GameObject[this.buttons.Count];
    for (int index = 0; index < this.buttons.Count; ++index)
    {
      KButtonMenu.ButtonInfo binfo = this.buttons[index];
      GameObject gameObject = Object.Instantiate<GameObject>(this.buttonPrefab, Vector3.zero, Quaternion.identity);
      this.buttonObjects[index] = gameObject;
      Transform transform = Object.op_Inequality((Object) this.buttonParent, (Object) null) ? this.buttonParent : ((KMonoBehaviour) this).transform;
      gameObject.transform.SetParent(transform, false);
      gameObject.SetActive(true);
      ((Object) gameObject).name = binfo.text + "Button";
      LocText[] componentsInChildren = gameObject.GetComponentsInChildren<LocText>(true);
      if (componentsInChildren != null)
      {
        foreach (LocText locText in componentsInChildren)
        {
          ((TMP_Text) locText).text = ((Object) locText).name == "Hotkey" ? GameUtil.GetActionString(binfo.shortcutKey) : binfo.text;
          ((Graphic) locText).color = binfo.isEnabled ? new Color(1f, 1f, 1f) : new Color(0.5f, 0.5f, 0.5f);
        }
      }
      ToolTip componentInChildren = gameObject.GetComponentInChildren<ToolTip>();
      if (binfo.toolTip != null && binfo.toolTip != "" && Object.op_Inequality((Object) componentInChildren, (Object) null))
        componentInChildren.toolTip = binfo.toolTip;
      KButtonMenu screen = this;
      KButton button = gameObject.GetComponent<KButton>();
      button.isInteractable = binfo.isEnabled;
      if ((binfo.popupOptions != null ? 1 : (binfo.onPopulatePopup != null ? 1 : 0)) == 0)
      {
        UnityAction onClick = binfo.onClick;
        button.onClick += (System.Action) (() =>
        {
          onClick.Invoke();
          if (this.keepMenuOpen || !Object.op_Inequality((Object) screen, (Object) null))
            return;
          screen.Deactivate();
        });
      }
      else
        button.onClick += (System.Action) (() => this.SetupPopupMenu(binfo, button));
      binfo.uibutton = button;
      KButtonMenu.ButtonInfo.HoverCallback onHover = binfo.onHover;
    }
    this.Update();
  }

  protected Button.ButtonClickedEvent SetupPopupMenu(KButtonMenu.ButtonInfo binfo, KButton button)
  {
    // ISSUE: object of a compiler-generated type is created
    // ISSUE: variable of a compiler-generated type
    KButtonMenu.\u003C\u003Ec__DisplayClass12_0 cDisplayClass120 = new KButtonMenu.\u003C\u003Ec__DisplayClass12_0();
    // ISSUE: reference to a compiler-generated field
    cDisplayClass120.binfo = binfo;
    // ISSUE: reference to a compiler-generated field
    cDisplayClass120.\u003C\u003E4__this = this;
    // ISSUE: reference to a compiler-generated field
    cDisplayClass120.button = button;
    Button.ButtonClickedEvent buttonClickedEvent = new Button.ButtonClickedEvent();
    // ISSUE: method pointer
    UnityAction unityAction = new UnityAction((object) cDisplayClass120, __methodptr(\u003CSetupPopupMenu\u003Eb__0));
    // ISSUE: reference to a compiler-generated field
    cDisplayClass120.binfo.onClick = unityAction;
    ((UnityEvent) buttonClickedEvent).AddListener(unityAction);
    return buttonClickedEvent;
  }

  public virtual void OnKeyDown(KButtonEvent e)
  {
    if (this.buttons == null)
      return;
    for (int index = 0; index < this.buttons.Count; ++index)
    {
      KButtonMenu.ButtonInfo button = this.buttons[index];
      if (e.TryConsume(button.shortcutKey))
      {
        this.buttonObjects[index].GetComponent<KButton>().PlayPointerDownSound();
        this.buttonObjects[index].GetComponent<KButton>().SignalClick((KKeyCode) 323);
        break;
      }
    }
    base.OnKeyDown(e);
  }

  protected virtual void OnPrefabInit() => ((KMonoBehaviour) this).Subscribe<KButtonMenu>(315865555, KButtonMenu.OnSetActivatorDelegate);

  private void OnSetActivator(object data)
  {
    this.go = (GameObject) data;
    this.Update();
  }

  protected virtual void OnDeactivate()
  {
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

  public class ButtonInfo
  {
    public string text;
    public Action shortcutKey;
    public GameObject visualizer;
    public UnityAction onClick;
    public KButtonMenu.ButtonInfo.HoverCallback onHover;
    public FMODAsset clickSound;
    public KButton uibutton;
    public string toolTip;
    public bool isEnabled = true;
    public string[] popupOptions;
    public Action<string> onPopupClick;
    public Func<string[]> onPopulatePopup;
    public object userData;

    public ButtonInfo(
      string text = null,
      UnityAction on_click = null,
      Action shortcut_key = 275,
      KButtonMenu.ButtonInfo.HoverCallback on_hover = null,
      string tool_tip = null,
      GameObject visualizer = null,
      bool is_enabled = true,
      string[] popup_options = null,
      Action<string> on_popup_click = null,
      Func<string[]> on_populate_popup = null)
    {
      this.text = text;
      this.shortcutKey = shortcut_key;
      this.onClick = on_click;
      this.onHover = on_hover;
      this.visualizer = visualizer;
      this.toolTip = tool_tip;
      this.isEnabled = is_enabled;
      this.uibutton = (KButton) null;
      this.popupOptions = popup_options;
      this.onPopupClick = on_popup_click;
      this.onPopulatePopup = on_populate_popup;
    }

    public ButtonInfo(
      string text,
      Action shortcutKey,
      UnityAction onClick,
      KButtonMenu.ButtonInfo.HoverCallback onHover = null,
      object userData = null)
    {
      this.text = text;
      this.shortcutKey = shortcutKey;
      this.onClick = onClick;
      this.onHover = onHover;
      this.userData = userData;
      this.visualizer = (GameObject) null;
      this.uibutton = (KButton) null;
    }

    public ButtonInfo(
      string text,
      GameObject visualizer,
      Action shortcutKey,
      UnityAction onClick,
      KButtonMenu.ButtonInfo.HoverCallback onHover = null,
      object userData = null)
    {
      this.text = text;
      this.shortcutKey = shortcutKey;
      this.onClick = onClick;
      this.onHover = onHover;
      this.visualizer = visualizer;
      this.userData = userData;
      this.uibutton = (KButton) null;
    }

    public delegate void HoverCallback(GameObject hoverTarget);

    public delegate void Callback();
  }
}
