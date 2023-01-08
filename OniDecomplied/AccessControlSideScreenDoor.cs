// Decompiled with JetBrains decompiler
// Type: AccessControlSideScreenDoor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/AccessControlSideScreenDoor")]
public class AccessControlSideScreenDoor : KMonoBehaviour
{
  public KToggle leftButton;
  public KToggle rightButton;
  private Action<MinionAssignablesProxy, AccessControl.Permission> permissionChangedCallback;
  private bool isUpDown;
  protected MinionAssignablesProxy targetIdentity;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.leftButton.onClick += new System.Action(this.OnPermissionButtonClicked);
    this.rightButton.onClick += new System.Action(this.OnPermissionButtonClicked);
  }

  private void OnPermissionButtonClicked()
  {
    AccessControl.Permission permission = !this.leftButton.isOn ? (!this.rightButton.isOn ? AccessControl.Permission.Neither : AccessControl.Permission.GoRight) : (!this.rightButton.isOn ? AccessControl.Permission.GoLeft : AccessControl.Permission.Both);
    this.UpdateButtonStates(false);
    this.permissionChangedCallback(this.targetIdentity, permission);
  }

  protected virtual void UpdateButtonStates(bool isDefault)
  {
    ToolTip component1 = ((Component) this.leftButton).GetComponent<ToolTip>();
    ToolTip component2 = ((Component) this.rightButton).GetComponent<ToolTip>();
    if (this.isUpDown)
    {
      component1.SetSimpleTooltip((string) (this.leftButton.isOn ? UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_UP_ENABLED : UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_UP_DISABLED));
      component2.SetSimpleTooltip((string) (this.rightButton.isOn ? UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_DOWN_ENABLED : UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_DOWN_DISABLED));
    }
    else
    {
      component1.SetSimpleTooltip((string) (this.leftButton.isOn ? UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_LEFT_ENABLED : UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_LEFT_DISABLED));
      component2.SetSimpleTooltip((string) (this.rightButton.isOn ? UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_RIGHT_ENABLED : UI.UISIDESCREENS.ACCESS_CONTROL_SIDE_SCREEN.GO_RIGHT_DISABLED));
    }
  }

  public void SetRotated(bool rotated) => this.isUpDown = rotated;

  public void SetContent(
    AccessControl.Permission permission,
    Action<MinionAssignablesProxy, AccessControl.Permission> onPermissionChange)
  {
    this.permissionChangedCallback = onPermissionChange;
    this.leftButton.isOn = permission == AccessControl.Permission.Both || permission == AccessControl.Permission.GoLeft;
    this.rightButton.isOn = permission == AccessControl.Permission.Both || permission == AccessControl.Permission.GoRight;
    this.UpdateButtonStates(false);
  }
}
