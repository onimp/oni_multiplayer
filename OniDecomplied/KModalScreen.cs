// Decompiled with JetBrains decompiler
// Type: KModalScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class KModalScreen : KScreen
{
  private bool shown;
  public bool pause = true;
  [Tooltip("Only used for main menu")]
  public bool canBackoutWithRightClick;
  private RectTransform backgroundRectTransform;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.backgroundRectTransform = KModalScreen.MakeScreenModal((KScreen) this);
  }

  public static RectTransform MakeScreenModal(KScreen screen)
  {
    screen.ConsumeMouseScroll = true;
    screen.activateOnSpawn = true;
    GameObject gameObject = new GameObject("background");
    gameObject.AddComponent<LayoutElement>().ignoreLayout = true;
    gameObject.AddComponent<CanvasRenderer>();
    Image image = gameObject.AddComponent<Image>();
    ((Graphic) image).color = Color32.op_Implicit(new Color32((byte) 0, (byte) 0, (byte) 0, (byte) 160));
    ((Graphic) image).raycastTarget = true;
    RectTransform component = gameObject.GetComponent<RectTransform>();
    ((Transform) component).SetParent(((KMonoBehaviour) screen).transform);
    KModalScreen.ResizeBackground(component);
    return component;
  }

  public static void ResizeBackground(RectTransform rectTransform)
  {
    ((Transform) rectTransform).SetAsFirstSibling();
    TransformExtensions.SetLocalPosition((Transform) rectTransform, Vector3.zero);
    ((Transform) rectTransform).localScale = Vector3.one;
    rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
    rectTransform.anchorMax = new Vector2(1f, 1f);
    rectTransform.sizeDelta = new Vector2(0.0f, 0.0f);
  }

  protected virtual void OnCmpEnable()
  {
    ((KMonoBehaviour) this).OnCmpEnable();
    if (Object.op_Inequality((Object) CameraController.Instance, (Object) null))
      CameraController.Instance.DisableUserCameraControl = true;
    if (!Object.op_Inequality((Object) ScreenResize.Instance, (Object) null))
      return;
    ScreenResize.Instance.OnResize += new System.Action(this.OnResize);
  }

  protected virtual void OnCmpDisable()
  {
    ((KMonoBehaviour) this).OnCmpDisable();
    if (Object.op_Inequality((Object) CameraController.Instance, (Object) null))
      CameraController.Instance.DisableUserCameraControl = false;
    ((KMonoBehaviour) this).Trigger(476357528, (object) null);
    if (!Object.op_Inequality((Object) ScreenResize.Instance, (Object) null))
      return;
    ScreenResize.Instance.OnResize -= new System.Action(this.OnResize);
  }

  private void OnResize() => KModalScreen.ResizeBackground(this.backgroundRectTransform);

  public virtual bool IsModal() => true;

  public virtual float GetSortKey() => 100f;

  protected virtual void OnActivate() => base.OnShow(true);

  protected virtual void OnDeactivate() => base.OnShow(false);

  protected virtual void OnShow(bool show)
  {
    base.OnShow(show);
    if (!this.pause || !Object.op_Inequality((Object) SpeedControlScreen.Instance, (Object) null))
      return;
    if (show && !this.shown)
      SpeedControlScreen.Instance.Pause(false);
    else if (!show && this.shown)
      SpeedControlScreen.Instance.Unpause(false);
    this.shown = show;
  }

  public virtual void OnKeyDown(KButtonEvent e)
  {
    if (((KInputEvent) e).Consumed)
      return;
    if (Object.op_Inequality((Object) Game.Instance, (Object) null) && (e.TryConsume((Action) 11) || e.TryConsume((Action) 12)))
      KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative"));
    if (!((KInputEvent) e).Consumed && (e.TryConsume((Action) 1) || e.TryConsume((Action) 5) && this.canBackoutWithRightClick))
      this.Deactivate();
    base.OnKeyDown(e);
    ((KInputEvent) e).Consumed = true;
  }

  public virtual void OnKeyUp(KButtonEvent e)
  {
    base.OnKeyUp(e);
    ((KInputEvent) e).Consumed = true;
  }
}
