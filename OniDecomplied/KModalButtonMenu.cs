// Decompiled with JetBrains decompiler
// Type: KModalButtonMenu
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class KModalButtonMenu : KButtonMenu
{
  private bool shown;
  [SerializeField]
  private GameObject panelRoot;
  private GameObject childDialog;
  private RectTransform modalBackground;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.modalBackground = KModalScreen.MakeScreenModal((KScreen) this);
  }

  protected virtual void OnCmpEnable()
  {
    KModalScreen.ResizeBackground(this.modalBackground);
    ScreenResize.Instance.OnResize += new System.Action(this.OnResize);
  }

  protected virtual void OnCmpDisable()
  {
    ((KMonoBehaviour) this).OnCmpDisable();
    if (Object.op_Equality((Object) this.childDialog, (Object) null))
      ((KMonoBehaviour) this).Trigger(476357528, (object) null);
    ScreenResize.Instance.OnResize -= new System.Action(this.OnResize);
  }

  private void OnResize() => KModalScreen.ResizeBackground(this.modalBackground);

  public virtual bool IsModal() => true;

  public virtual float GetSortKey() => 100f;

  protected virtual void OnShow(bool show)
  {
    base.OnShow(show);
    if (Object.op_Inequality((Object) SpeedControlScreen.Instance, (Object) null))
    {
      if (show && !this.shown)
        SpeedControlScreen.Instance.Pause(false);
      else if (!show && this.shown)
        SpeedControlScreen.Instance.Unpause(false);
      this.shown = show;
    }
    if (!Object.op_Inequality((Object) CameraController.Instance, (Object) null))
      return;
    CameraController.Instance.DisableUserCameraControl = show;
  }

  public override void OnKeyDown(KButtonEvent e)
  {
    base.OnKeyDown(e);
    ((KInputEvent) e).Consumed = true;
  }

  public virtual void OnKeyUp(KButtonEvent e)
  {
    base.OnKeyUp(e);
    ((KInputEvent) e).Consumed = true;
  }

  public void SetBackgroundActive(bool active)
  {
  }

  protected GameObject ActivateChildScreen(GameObject screenPrefab)
  {
    GameObject gameObject = Util.KInstantiateUI(screenPrefab, ((Component) ((KMonoBehaviour) this).transform.parent).gameObject, false);
    this.childDialog = gameObject;
    KMonoBehaviourExtensions.Subscribe(gameObject, 476357528, new Action<object>(this.Unhide));
    this.Hide();
    return gameObject;
  }

  private void Hide() => ((Transform) Util.rectTransform(this.panelRoot)).localScale = Vector3.zero;

  private void Unhide(object data = null)
  {
    ((Transform) Util.rectTransform(this.panelRoot)).localScale = Vector3.one;
    KMonoBehaviourExtensions.Unsubscribe(this.childDialog, 476357528, new Action<object>(this.Unhide));
    this.childDialog = (GameObject) null;
  }
}
