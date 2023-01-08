// Decompiled with JetBrains decompiler
// Type: ConfirmDialogScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmDialogScreen : KModalScreen
{
  private System.Action confirmAction;
  private System.Action cancelAction;
  private System.Action configurableAction;
  public bool deactivateOnConfigurableAction = true;
  public bool deactivateOnConfirmAction = true;
  public bool deactivateOnCancelAction = true;
  public System.Action onDeactivateCB;
  [SerializeField]
  private GameObject confirmButton;
  [SerializeField]
  private GameObject cancelButton;
  [SerializeField]
  private GameObject configurableButton;
  [SerializeField]
  private LocText titleText;
  [SerializeField]
  private LocText popupMessage;
  [SerializeField]
  private Image image;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    ((Component) this).gameObject.SetActive(false);
  }

  public override bool IsModal() => true;

  public override void OnKeyDown(KButtonEvent e)
  {
    if (e.TryConsume((Action) 1))
      this.OnSelect_CANCEL();
    else
      base.OnKeyDown(e);
  }

  public void PopupConfirmDialog(
    string text,
    System.Action on_confirm,
    System.Action on_cancel,
    string configurable_text = null,
    System.Action on_configurable_clicked = null,
    string title_text = null,
    string confirm_text = null,
    string cancel_text = null,
    Sprite image_sprite = null)
  {
    while (Object.op_Equality((Object) ((Component) ((KMonoBehaviour) this).transform.parent).GetComponent<Canvas>(), (Object) null) && Object.op_Inequality((Object) ((KMonoBehaviour) this).transform.parent.parent, (Object) null))
      ((KMonoBehaviour) this).transform.SetParent(((KMonoBehaviour) this).transform.parent.parent);
    ((KMonoBehaviour) this).transform.SetAsLastSibling();
    this.confirmAction = on_confirm;
    this.cancelAction = on_cancel;
    this.configurableAction = on_configurable_clicked;
    int num1 = 0;
    if (this.confirmAction != null)
      ++num1;
    if (this.cancelAction != null)
      ++num1;
    if (this.configurableAction != null)
    {
      int num2 = num1 + 1;
    }
    ((TMP_Text) this.confirmButton.GetComponentInChildren<LocText>()).text = confirm_text == null ? STRINGS.UI.CONFIRMDIALOG.OK.text : confirm_text;
    ((TMP_Text) this.cancelButton.GetComponentInChildren<LocText>()).text = cancel_text == null ? STRINGS.UI.CONFIRMDIALOG.CANCEL.text : cancel_text;
    this.confirmButton.GetComponent<KButton>().onClick += new System.Action(this.OnSelect_OK);
    this.cancelButton.GetComponent<KButton>().onClick += new System.Action(this.OnSelect_CANCEL);
    this.configurableButton.GetComponent<KButton>().onClick += new System.Action(this.OnSelect_third);
    this.cancelButton.SetActive(on_cancel != null);
    if (Object.op_Inequality((Object) this.configurableButton, (Object) null))
    {
      this.configurableButton.SetActive(this.configurableAction != null);
      if (configurable_text != null)
        ((TMP_Text) this.configurableButton.GetComponentInChildren<LocText>()).text = configurable_text;
    }
    if (Object.op_Inequality((Object) image_sprite, (Object) null))
    {
      this.image.sprite = image_sprite;
      ((Component) this.image).gameObject.SetActive(true);
    }
    if (title_text != null)
    {
      this.titleText.key = "";
      ((TMP_Text) this.titleText).text = title_text;
    }
    ((TMP_Text) this.popupMessage).text = text;
  }

  public void OnSelect_OK()
  {
    if (this.deactivateOnConfirmAction)
      this.Deactivate();
    if (this.confirmAction == null)
      return;
    this.confirmAction();
  }

  public void OnSelect_CANCEL()
  {
    if (this.deactivateOnCancelAction)
      this.Deactivate();
    if (this.cancelAction == null)
      return;
    this.cancelAction();
  }

  public void OnSelect_third()
  {
    if (this.deactivateOnConfigurableAction)
      this.Deactivate();
    if (this.configurableAction == null)
      return;
    this.configurableAction();
  }

  protected override void OnDeactivate()
  {
    if (this.onDeactivateCB != null)
      this.onDeactivateCB();
    base.OnDeactivate();
  }
}
