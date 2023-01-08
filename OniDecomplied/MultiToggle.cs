// Decompiled with JetBrains decompiler
// Type: MultiToggle
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/MultiToggle")]
public class MultiToggle : 
  KMonoBehaviour,
  IPointerClickHandler,
  IEventSystemHandler,
  IPointerEnterHandler,
  IPointerExitHandler,
  IPointerDownHandler,
  IPointerUpHandler
{
  [Header("Settings")]
  [SerializeField]
  public ToggleState[] states;
  public bool play_sound_on_click = true;
  public bool play_sound_on_release;
  public Image toggle_image;
  protected int state;
  public System.Action onClick;
  private bool stateDirty = true;
  public Func<bool> onDoubleClick;
  public System.Action onEnter;
  public System.Action onExit;
  public System.Action onHold;
  public System.Action onStopHold;
  public bool allowRightClick = true;
  protected bool clickHeldDown;
  protected float totalHeldTime;
  protected float heldTimeThreshold = 0.4f;
  private bool pointerOver;

  public int CurrentState => this.state;

  public void NextState() => this.ChangeState((this.state + 1) % this.states.Length);

  protected virtual void Update()
  {
    if (!this.clickHeldDown)
      return;
    this.totalHeldTime += Time.unscaledDeltaTime;
    if ((double) this.totalHeldTime <= (double) this.heldTimeThreshold || this.onHold == null)
      return;
    this.onHold();
  }

  private void OnDisable()
  {
    if (((Component) this).gameObject.activeInHierarchy)
      return;
    this.RefreshHoverColor();
    this.pointerOver = false;
  }

  public void ChangeState(int new_state_index, bool forceRefreshState)
  {
    if (forceRefreshState)
      this.stateDirty = true;
    this.ChangeState(new_state_index);
  }

  public void ChangeState(int new_state_index)
  {
    if (!this.stateDirty && new_state_index == this.state)
      return;
    this.stateDirty = false;
    this.state = new_state_index;
    try
    {
      this.toggle_image.sprite = this.states[new_state_index].sprite;
      ((Graphic) this.toggle_image).color = this.states[new_state_index].color;
      if (this.states[new_state_index].use_rect_margins)
        Util.rectTransform((Component) this.toggle_image).sizeDelta = this.states[new_state_index].rect_margins;
    }
    catch
    {
      string str = ((Object) ((Component) this).gameObject).name;
      for (Transform transform = this.transform; Object.op_Inequality((Object) transform.parent, (Object) null); transform = transform.parent)
        str = str.Insert(0, ((Object) transform).name + ">");
      Debug.LogError((object) ("Multi Toggle state index out of range: " + str + " idx:" + new_state_index.ToString()), (Object) ((Component) this).gameObject);
    }
    foreach (StatePresentationSetting additionalDisplaySetting in this.states[this.state].additional_display_settings)
    {
      if (!Object.op_Equality((Object) additionalDisplaySetting.image_target, (Object) null))
      {
        additionalDisplaySetting.image_target.sprite = additionalDisplaySetting.sprite;
        ((Graphic) additionalDisplaySetting.image_target).color = additionalDisplaySetting.color;
      }
    }
    this.RefreshHoverColor();
  }

  public virtual void OnPointerClick(PointerEventData eventData)
  {
    if (!this.allowRightClick && eventData.button == 1)
      return;
    if (this.states.Length - 1 < this.state)
      Debug.LogWarning((object) "Multi toggle has too few / no states");
    bool flag = false;
    if (this.onDoubleClick != null && eventData.clickCount == 2)
      flag = this.onDoubleClick();
    if (this.onClick != null && !flag)
      this.onClick();
    this.RefreshHoverColor();
  }

  public void OnPointerEnter(PointerEventData eventData)
  {
    this.pointerOver = true;
    if (!KInputManager.isFocused)
      return;
    KInputManager.SetUserActive();
    if (this.states.Length == 0)
      return;
    if (this.states[this.state].use_color_on_hover && Color.op_Inequality(this.states[this.state].color_on_hover, this.states[this.state].color))
      ((Graphic) this.toggle_image).color = this.states[this.state].color_on_hover;
    if (this.states[this.state].use_rect_margins)
      Util.rectTransform((Component) this.toggle_image).sizeDelta = this.states[this.state].rect_margins;
    foreach (StatePresentationSetting additionalDisplaySetting in this.states[this.state].additional_display_settings)
    {
      if (!Object.op_Equality((Object) additionalDisplaySetting.image_target, (Object) null) && additionalDisplaySetting.use_color_on_hover)
        ((Graphic) additionalDisplaySetting.image_target).color = additionalDisplaySetting.color_on_hover;
    }
    if (this.onEnter == null)
      return;
    this.onEnter();
  }

  protected void RefreshHoverColor()
  {
    if (!((Component) this).gameObject.activeInHierarchy)
    {
      if (this.states.Length == 0)
        return;
      if (this.states[this.state].use_color_on_hover && Color.op_Inequality(this.states[this.state].color_on_hover, this.states[this.state].color))
        ((Graphic) this.toggle_image).color = this.states[this.state].color;
      foreach (StatePresentationSetting additionalDisplaySetting in this.states[this.state].additional_display_settings)
      {
        if (!Object.op_Equality((Object) additionalDisplaySetting.image_target, (Object) null) && additionalDisplaySetting.use_color_on_hover)
          ((Graphic) additionalDisplaySetting.image_target).color = additionalDisplaySetting.color;
      }
    }
    else
    {
      if (!this.pointerOver)
        return;
      if (this.states[this.state].use_color_on_hover && Color.op_Inequality(this.states[this.state].color_on_hover, this.states[this.state].color))
        ((Graphic) this.toggle_image).color = this.states[this.state].color_on_hover;
      foreach (StatePresentationSetting additionalDisplaySetting in this.states[this.state].additional_display_settings)
      {
        if (!Object.op_Equality((Object) additionalDisplaySetting.image_target, (Object) null) && !Object.op_Equality((Object) additionalDisplaySetting.image_target, (Object) null) && additionalDisplaySetting.use_color_on_hover)
          ((Graphic) additionalDisplaySetting.image_target).color = additionalDisplaySetting.color_on_hover;
      }
    }
  }

  public void OnPointerExit(PointerEventData eventData)
  {
    this.pointerOver = false;
    if (!KInputManager.isFocused)
      return;
    KInputManager.SetUserActive();
    if (this.states.Length == 0)
      return;
    if (this.states[this.state].use_color_on_hover && Color.op_Inequality(this.states[this.state].color_on_hover, this.states[this.state].color))
      ((Graphic) this.toggle_image).color = this.states[this.state].color;
    if (this.states[this.state].use_rect_margins)
      Util.rectTransform((Component) this.toggle_image).sizeDelta = this.states[this.state].rect_margins;
    foreach (StatePresentationSetting additionalDisplaySetting in this.states[this.state].additional_display_settings)
    {
      if (!Object.op_Equality((Object) additionalDisplaySetting.image_target, (Object) null) && additionalDisplaySetting.use_color_on_hover)
        ((Graphic) additionalDisplaySetting.image_target).color = additionalDisplaySetting.color;
    }
    if (this.onExit == null)
      return;
    this.onExit();
  }

  public virtual void OnPointerDown(PointerEventData eventData)
  {
    if (!this.allowRightClick && eventData.button == 1)
      return;
    this.clickHeldDown = true;
    if (!this.play_sound_on_click)
      return;
    if (this.states[this.state].on_click_override_sound_path == "")
      KFMOD.PlayUISound(GlobalAssets.GetSound("HUD_Click"));
    else
      KFMOD.PlayUISound(GlobalAssets.GetSound(this.states[this.state].on_click_override_sound_path));
  }

  public virtual void OnPointerUp(PointerEventData eventData)
  {
    if (!this.allowRightClick && eventData.button == 1)
      return;
    if (this.clickHeldDown)
    {
      if (this.play_sound_on_release && this.states[this.state].on_release_override_sound_path != "")
        KFMOD.PlayUISound(GlobalAssets.GetSound(this.states[this.state].on_release_override_sound_path));
      this.clickHeldDown = false;
      if (this.onStopHold != null)
        this.onStopHold();
    }
    this.totalHeldTime = 0.0f;
  }
}
