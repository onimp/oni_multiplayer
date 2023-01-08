// Decompiled with JetBrains decompiler
// Type: FileNameDialog
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FileNameDialog : KModalScreen
{
  public Action<string> onConfirm;
  public System.Action onCancel;
  [SerializeField]
  private KInputTextField inputField;
  [SerializeField]
  private KButton confirmButton;
  [SerializeField]
  private KButton cancelButton;
  [SerializeField]
  private KButton closeButton;

  public override float GetSortKey() => 150f;

  public void SetTextAndSelect(string text)
  {
    if (Object.op_Equality((Object) this.inputField, (Object) null))
      return;
    ((TMP_InputField) this.inputField).text = text;
    ((Selectable) this.inputField).Select();
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.confirmButton.onClick += new System.Action(this.OnConfirm);
    this.cancelButton.onClick += new System.Action(this.OnCancel);
    this.closeButton.onClick += new System.Action(this.OnCancel);
    // ISSUE: method pointer
    ((UnityEvent<string>) ((TMP_InputField) this.inputField).onValueChanged).AddListener(new UnityAction<string>((object) this, __methodptr(\u003COnSpawn\u003Eb__8_0)));
    // ISSUE: method pointer
    ((UnityEvent<string>) ((TMP_InputField) this.inputField).onEndEdit).AddListener(new UnityAction<string>((object) this, __methodptr(OnEndEdit)));
  }

  protected override void OnActivate()
  {
    base.OnActivate();
    ((Selectable) this.inputField).Select();
    ((TMP_InputField) this.inputField).ActivateInputField();
    CameraController.Instance.DisableUserCameraControl = true;
  }

  protected override void OnDeactivate()
  {
    CameraController.Instance.DisableUserCameraControl = false;
    base.OnDeactivate();
  }

  public void OnConfirm()
  {
    if (this.onConfirm == null || string.IsNullOrEmpty(((TMP_InputField) this.inputField).text))
      return;
    string text = ((TMP_InputField) this.inputField).text;
    if (!text.EndsWith(".sav"))
      text += ".sav";
    this.onConfirm(text);
    this.Deactivate();
  }

  private void OnEndEdit(string str)
  {
    if (!Localization.HasDirtyWords(str))
      return;
    ((TMP_InputField) this.inputField).text = "";
  }

  public void OnCancel()
  {
    if (this.onCancel != null)
      this.onCancel();
    this.Deactivate();
  }

  public override void OnKeyUp(KButtonEvent e)
  {
    if (e.TryConsume((Action) 1))
      this.Deactivate();
    else if (e.TryConsume((Action) 228))
      this.OnConfirm();
    ((KInputEvent) e).Consumed = true;
  }

  public override void OnKeyDown(KButtonEvent e) => ((KInputEvent) e).Consumed = true;
}
