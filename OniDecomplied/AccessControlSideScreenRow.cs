// Decompiled with JetBrains decompiler
// Type: AccessControlSideScreenRow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

public class AccessControlSideScreenRow : AccessControlSideScreenDoor
{
  [SerializeField]
  private CrewPortrait crewPortraitPrefab;
  private CrewPortrait portraitInstance;
  public KToggle defaultButton;
  public GameObject defaultControls;
  public GameObject customControls;
  private Action<MinionAssignablesProxy, bool> defaultClickedCallback;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.defaultButton.onValueChanged += new Action<bool>(this.OnDefaultButtonChanged);
  }

  private void OnDefaultButtonChanged(bool state)
  {
    this.UpdateButtonStates(!state);
    if (this.defaultClickedCallback == null)
      return;
    this.defaultClickedCallback(this.targetIdentity, !state);
  }

  protected override void UpdateButtonStates(bool isDefault)
  {
    base.UpdateButtonStates(isDefault);
    ((Component) this.defaultButton).GetComponent<ToolTip>().SetSimpleTooltip((string) (isDefault ? UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.SET_TO_CUSTOM : UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.SET_TO_DEFAULT));
    this.defaultControls.SetActive(isDefault);
    this.customControls.SetActive(!isDefault);
  }

  public void SetMinionContent(
    MinionAssignablesProxy identity,
    AccessControl.Permission permission,
    bool isDefault,
    Action<MinionAssignablesProxy, AccessControl.Permission> onPermissionChange,
    Action<MinionAssignablesProxy, bool> onDefaultClick)
  {
    this.SetContent(permission, onPermissionChange);
    if (Object.op_Equality((Object) identity, (Object) null))
    {
      Debug.LogError((object) "Invalid data received.");
    }
    else
    {
      if (Object.op_Equality((Object) this.portraitInstance, (Object) null))
      {
        this.portraitInstance = Util.KInstantiateUI<CrewPortrait>(((Component) this.crewPortraitPrefab).gameObject, ((Component) this.defaultButton).gameObject, false);
        this.portraitInstance.SetAlpha(1f);
      }
      this.targetIdentity = identity;
      this.portraitInstance.SetIdentityObject((IAssignableIdentity) identity, false);
      this.portraitInstance.SetSubTitle((string) (isDefault ? UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.USING_DEFAULT : UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.USING_CUSTOM));
      this.defaultClickedCallback = (Action<MinionAssignablesProxy, bool>) null;
      this.defaultButton.isOn = !isDefault;
      this.defaultClickedCallback = onDefaultClick;
    }
  }
}
