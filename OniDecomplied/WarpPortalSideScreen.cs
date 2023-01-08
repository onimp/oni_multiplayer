// Decompiled with JetBrains decompiler
// Type: WarpPortalSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarpPortalSideScreen : SideScreenContent
{
  [SerializeField]
  private LocText label;
  [SerializeField]
  private KButton button;
  [SerializeField]
  private LocText buttonLabel;
  [SerializeField]
  private KButton cancelButton;
  [SerializeField]
  private LocText cancelButtonLabel;
  [SerializeField]
  private WarpPortal target;
  [SerializeField]
  private GameObject contents;
  [SerializeField]
  private GameObject progressBar;
  [SerializeField]
  private LocText progressLabel;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    ((TMP_Text) this.buttonLabel).SetText((string) STRINGS.UI.UISIDESCREENS.WARPPORTALSIDESCREEN.BUTTON);
    ((TMP_Text) this.cancelButtonLabel).SetText((string) STRINGS.UI.UISIDESCREENS.WARPPORTALSIDESCREEN.CANCELBUTTON);
    this.button.onClick += new System.Action(this.OnButtonClick);
    this.cancelButton.onClick += new System.Action(this.OnCancelClick);
    this.Refresh();
  }

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<WarpPortal>(), (Object) null);

  public override void SetTarget(GameObject target)
  {
    WarpPortal component = target.GetComponent<WarpPortal>();
    if (Object.op_Equality((Object) component, (Object) null))
    {
      Debug.LogError((object) "Target doesn't have a WarpPortal associated with it.");
    }
    else
    {
      this.target = component;
      target.GetComponent<Assignable>().OnAssign += new Action<IAssignableIdentity>(this.Refresh);
      this.Refresh();
    }
  }

  private void Update()
  {
    if (!this.progressBar.activeSelf)
      return;
    RectTransform rectTransform = ((Graphic) this.progressBar.GetComponentsInChildren<Image>()[1]).rectTransform;
    float num = this.target.rechargeProgress / 3000f;
    rectTransform.sizeDelta = new Vector2(((Component) ((Component) rectTransform).transform.parent).GetComponent<LayoutElement>().minWidth * num, 24f);
    ((TMP_Text) this.progressLabel).text = GameUtil.GetFormattedPercent(num * 100f);
  }

  private void OnButtonClick()
  {
    if (!this.target.ReadyToWarp)
      return;
    this.target.StartWarpSequence();
    this.Refresh();
  }

  private void OnCancelClick()
  {
    this.target.CancelAssignment();
    this.Refresh();
  }

  private void Refresh(object data = null)
  {
    this.progressBar.SetActive(false);
    ((Component) this.cancelButton).gameObject.SetActive(false);
    if (Object.op_Inequality((Object) this.target, (Object) null))
    {
      if (this.target.ReadyToWarp)
      {
        ((TMP_Text) this.label).text = (string) STRINGS.UI.UISIDESCREENS.WARPPORTALSIDESCREEN.WAITING;
        ((Component) this.button).gameObject.SetActive(true);
        ((Component) this.cancelButton).gameObject.SetActive(true);
      }
      else if (this.target.IsConsumed)
      {
        ((Component) this.button).gameObject.SetActive(false);
        this.progressBar.SetActive(true);
        ((TMP_Text) this.label).text = (string) STRINGS.UI.UISIDESCREENS.WARPPORTALSIDESCREEN.CONSUMED;
      }
      else if (this.target.IsWorking)
      {
        ((TMP_Text) this.label).text = (string) STRINGS.UI.UISIDESCREENS.WARPPORTALSIDESCREEN.UNDERWAY;
        ((Component) this.button).gameObject.SetActive(false);
        ((Component) this.cancelButton).gameObject.SetActive(true);
      }
      else
      {
        ((TMP_Text) this.label).text = (string) STRINGS.UI.UISIDESCREENS.WARPPORTALSIDESCREEN.IDLE;
        ((Component) this.button).gameObject.SetActive(false);
      }
    }
    else
    {
      ((TMP_Text) this.label).text = (string) STRINGS.UI.UISIDESCREENS.WARPPORTALSIDESCREEN.IDLE;
      ((Component) this.button).gameObject.SetActive(false);
    }
  }
}
