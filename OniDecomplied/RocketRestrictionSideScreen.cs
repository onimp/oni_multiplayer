// Decompiled with JetBrains decompiler
// Type: RocketRestrictionSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class RocketRestrictionSideScreen : SideScreenContent
{
  private RocketControlStation controlStation;
  [Header("Buttons")]
  public KToggle unrestrictedButton;
  public KToggle spaceRestrictedButton;
  public GameObject automationControlled;
  private int controlStationLogicSubHandle = -1;

  protected virtual void OnSpawn()
  {
    this.unrestrictedButton.onClick += new System.Action(this.ClickNone);
    this.spaceRestrictedButton.onClick += new System.Action(this.ClickSpace);
  }

  public override int GetSideScreenSortOrder() => 0;

  public override bool IsValidForTarget(GameObject target) => target.GetSMI<RocketControlStation.StatesInstance>() != null;

  public override void SetTarget(GameObject new_target)
  {
    this.controlStation = new_target.GetComponent<RocketControlStation>();
    this.controlStationLogicSubHandle = this.controlStation.Subscribe(1861523068, new Action<object>(this.UpdateButtonStates));
    this.UpdateButtonStates();
  }

  public override void ClearTarget()
  {
    if (this.controlStationLogicSubHandle != -1 && Object.op_Inequality((Object) this.controlStation, (Object) null))
    {
      this.controlStation.Unsubscribe(this.controlStationLogicSubHandle);
      this.controlStationLogicSubHandle = -1;
    }
    this.controlStation = (RocketControlStation) null;
  }

  private void UpdateButtonStates(object data = null)
  {
    bool flag = this.controlStation.IsLogicInputConnected();
    if (!flag)
    {
      this.unrestrictedButton.isOn = !this.controlStation.RestrictWhenGrounded;
      this.spaceRestrictedButton.isOn = this.controlStation.RestrictWhenGrounded;
    }
    ((Component) this.unrestrictedButton).gameObject.SetActive(!flag);
    ((Component) this.spaceRestrictedButton).gameObject.SetActive(!flag);
    this.automationControlled.gameObject.SetActive(flag);
  }

  private void ClickNone()
  {
    this.controlStation.RestrictWhenGrounded = false;
    this.UpdateButtonStates();
  }

  private void ClickSpace()
  {
    this.controlStation.RestrictWhenGrounded = true;
    this.UpdateButtonStates();
  }
}
