// Decompiled with JetBrains decompiler
// Type: EditableTitleBar
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EditableTitleBar : TitleBar
{
  public KButton editNameButton;
  public KButton randomNameButton;
  public KInputTextField inputField;
  private Coroutine postEndEdit;
  private Coroutine preToggleNameEditing;

  public event Action<string> OnNameChanged;

  public event System.Action OnStartedEditing;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (Object.op_Inequality((Object) this.randomNameButton, (Object) null))
      this.randomNameButton.onClick += new System.Action(this.GenerateRandomName);
    if (Object.op_Inequality((Object) this.editNameButton, (Object) null))
      this.EnableEditButtonClick();
    if (!Object.op_Inequality((Object) this.inputField, (Object) null))
      return;
    // ISSUE: method pointer
    ((UnityEvent<string>) ((TMP_InputField) this.inputField).onEndEdit).AddListener(new UnityAction<string>((object) this, __methodptr(OnEndEdit)));
  }

  public void UpdateRenameTooltip(GameObject target)
  {
    if (!Object.op_Inequality((Object) this.editNameButton, (Object) null) || !Object.op_Inequality((Object) target, (Object) null))
      return;
    if (Object.op_Inequality((Object) target.GetComponent<MinionBrain>(), (Object) null))
      ((Component) this.editNameButton).GetComponent<ToolTip>().toolTip = (string) STRINGS.UI.TOOLTIPS.EDITNAME;
    if (Object.op_Inequality((Object) target.GetComponent<ClustercraftExteriorDoor>(), (Object) null) || Object.op_Inequality((Object) target.GetComponent<CommandModule>(), (Object) null))
      ((Component) this.editNameButton).GetComponent<ToolTip>().toolTip = (string) STRINGS.UI.TOOLTIPS.EDITNAMEROCKET;
    else
      ((Component) this.editNameButton).GetComponent<ToolTip>().toolTip = string.Format((string) STRINGS.UI.TOOLTIPS.EDITNAMEGENERIC, (object) target.GetProperName());
  }

  private void OnEndEdit(string finalStr)
  {
    finalStr = Localization.FilterDirtyWords(finalStr);
    this.SetEditingState(false);
    if (string.IsNullOrEmpty(finalStr))
      return;
    if (this.OnNameChanged != null)
      this.OnNameChanged(finalStr);
    ((TMP_Text) this.titleText).text = finalStr;
    if (this.postEndEdit != null)
      ((MonoBehaviour) this).StopCoroutine(this.postEndEdit);
    if (!((Component) this).gameObject.activeInHierarchy || !((Behaviour) this).enabled)
      return;
    this.postEndEdit = ((MonoBehaviour) this).StartCoroutine(this.PostOnEndEditRoutine());
  }

  private IEnumerator PostOnEndEditRoutine()
  {
    int i = 0;
    while (i < 10)
    {
      ++i;
      yield return (object) SequenceUtil.WaitForEndOfFrame;
    }
    this.EnableEditButtonClick();
    if (Object.op_Inequality((Object) this.randomNameButton, (Object) null))
      ((Component) this.randomNameButton).gameObject.SetActive(false);
  }

  private IEnumerator PreToggleNameEditingRoutine()
  {
    yield return (object) SequenceUtil.WaitForEndOfFrame;
    this.ToggleNameEditing();
    this.preToggleNameEditing = (Coroutine) null;
  }

  private void EnableEditButtonClick() => this.editNameButton.onClick += (System.Action) (() =>
  {
    if (this.preToggleNameEditing != null)
      return;
    this.preToggleNameEditing = ((MonoBehaviour) this).StartCoroutine(this.PreToggleNameEditingRoutine());
  });

  private void GenerateRandomName()
  {
    if (this.postEndEdit != null)
      ((MonoBehaviour) this).StopCoroutine(this.postEndEdit);
    string randomDuplicantName = GameUtil.GenerateRandomDuplicantName();
    if (this.OnNameChanged != null)
      this.OnNameChanged(randomDuplicantName);
    ((TMP_Text) this.titleText).text = randomDuplicantName;
    this.SetEditingState(true);
  }

  private void ToggleNameEditing()
  {
    this.editNameButton.ClearOnClick();
    bool state = !((Component) this.inputField).gameObject.activeInHierarchy;
    if (Object.op_Inequality((Object) this.randomNameButton, (Object) null))
      ((Component) this.randomNameButton).gameObject.SetActive(state);
    this.SetEditingState(state);
  }

  private void SetEditingState(bool state)
  {
    ((Component) this.titleText).gameObject.SetActive(!state);
    if (this.setCameraControllerState)
      CameraController.Instance.DisableUserCameraControl = state;
    if (Object.op_Equality((Object) this.inputField, (Object) null))
      return;
    ((Component) this.inputField).gameObject.SetActive(state);
    if (state)
    {
      ((TMP_InputField) this.inputField).text = ((TMP_Text) this.titleText).text;
      ((Selectable) this.inputField).Select();
      ((TMP_InputField) this.inputField).ActivateInputField();
      if (this.OnStartedEditing == null)
        return;
      this.OnStartedEditing();
    }
    else
      ((TMP_InputField) this.inputField).DeactivateInputField();
  }

  public void ForceStopEditing()
  {
    if (this.postEndEdit != null)
      ((MonoBehaviour) this).StopCoroutine(this.postEndEdit);
    this.editNameButton.ClearOnClick();
    this.SetEditingState(false);
    this.EnableEditButtonClick();
  }

  public void SetUserEditable(bool editable)
  {
    this.userEditable = editable;
    ((Component) this.editNameButton).gameObject.SetActive(editable);
    this.editNameButton.ClearOnClick();
    this.EnableEditButtonClick();
  }
}
