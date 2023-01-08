// Decompiled with JetBrains decompiler
// Type: ScalerMask
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/ScalerMask")]
public class ScalerMask : 
  KMonoBehaviour,
  IPointerEnterHandler,
  IEventSystemHandler,
  IPointerExitHandler
{
  public RectTransform SourceTransform;
  private RectTransform _thisTransform;
  private LayoutElement _thisLayoutElement;
  public GameObject hoverIndicator;
  public bool hoverLock;
  private bool grandparentIsHovered;
  private bool isHovered;
  private bool queuedSizeUpdate = true;
  public float topPadding;
  public float bottomPadding;

  private RectTransform ThisTransform
  {
    get
    {
      if (Object.op_Equality((Object) this._thisTransform, (Object) null))
        this._thisTransform = ((Component) this).GetComponent<RectTransform>();
      return this._thisTransform;
    }
  }

  private LayoutElement ThisLayoutElement
  {
    get
    {
      if (Object.op_Equality((Object) this._thisLayoutElement, (Object) null))
        this._thisLayoutElement = ((Component) this).GetComponent<LayoutElement>();
      return this._thisLayoutElement;
    }
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    DetailsScreen componentInParent = ((Component) this).GetComponentInParent<DetailsScreen>();
    if (!Object.op_Implicit((Object) componentInParent))
      return;
    DetailsScreen detailsScreen1 = componentInParent;
    // ISSUE: method pointer
    ((KScreen) detailsScreen1).pointerEnterActions = (KScreen.PointerEnterActions) Delegate.Combine((Delegate) ((KScreen) detailsScreen1).pointerEnterActions, (Delegate) new KScreen.PointerEnterActions((object) this, __methodptr(OnPointerEnterGrandparent)));
    DetailsScreen detailsScreen2 = componentInParent;
    // ISSUE: method pointer
    ((KScreen) detailsScreen2).pointerExitActions = (KScreen.PointerExitActions) Delegate.Combine((Delegate) ((KScreen) detailsScreen2).pointerExitActions, (Delegate) new KScreen.PointerExitActions((object) this, __methodptr(OnPointerExitGrandparent)));
  }

  protected virtual void OnCleanUp()
  {
    DetailsScreen componentInParent = ((Component) this).GetComponentInParent<DetailsScreen>();
    if (Object.op_Implicit((Object) componentInParent))
    {
      DetailsScreen detailsScreen1 = componentInParent;
      // ISSUE: method pointer
      ((KScreen) detailsScreen1).pointerEnterActions = (KScreen.PointerEnterActions) Delegate.Remove((Delegate) ((KScreen) detailsScreen1).pointerEnterActions, (Delegate) new KScreen.PointerEnterActions((object) this, __methodptr(OnPointerEnterGrandparent)));
      DetailsScreen detailsScreen2 = componentInParent;
      // ISSUE: method pointer
      ((KScreen) detailsScreen2).pointerExitActions = (KScreen.PointerExitActions) Delegate.Remove((Delegate) ((KScreen) detailsScreen2).pointerExitActions, (Delegate) new KScreen.PointerExitActions((object) this, __methodptr(OnPointerExitGrandparent)));
    }
    base.OnCleanUp();
  }

  private void Update()
  {
    Rect rect;
    if (Object.op_Inequality((Object) this.SourceTransform, (Object) null))
    {
      RectTransform sourceTransform = this.SourceTransform;
      rect = this.ThisTransform.rect;
      double width = (double) ((Rect) ref rect).width;
      sourceTransform.SetSizeWithCurrentAnchors((RectTransform.Axis) 0, (float) width);
    }
    if (Object.op_Inequality((Object) this.SourceTransform, (Object) null) && (!this.hoverLock || !this.grandparentIsHovered || this.isHovered || this.queuedSizeUpdate))
    {
      LayoutElement thisLayoutElement = this.ThisLayoutElement;
      rect = this.SourceTransform.rect;
      double num = (double) ((Rect) ref rect).height + (double) this.topPadding + (double) this.bottomPadding;
      thisLayoutElement.minHeight = (float) num;
      this.SourceTransform.anchoredPosition = new Vector2(0.0f, -this.topPadding);
      this.queuedSizeUpdate = false;
    }
    if (!Object.op_Inequality((Object) this.hoverIndicator, (Object) null))
      return;
    if (Object.op_Inequality((Object) this.SourceTransform, (Object) null))
    {
      rect = this.SourceTransform.rect;
      double height1 = (double) ((Rect) ref rect).height;
      rect = this.ThisTransform.rect;
      double height2 = (double) ((Rect) ref rect).height;
      if (height1 > height2)
      {
        this.hoverIndicator.SetActive(true);
        return;
      }
    }
    this.hoverIndicator.SetActive(false);
  }

  public void UpdateSize() => this.queuedSizeUpdate = true;

  public void OnPointerEnterGrandparent(PointerEventData eventData) => this.grandparentIsHovered = true;

  public void OnPointerExitGrandparent(PointerEventData eventData) => this.grandparentIsHovered = false;

  public void OnPointerEnter(PointerEventData eventData) => this.isHovered = true;

  public void OnPointerExit(PointerEventData eventData) => this.isHovered = false;
}
