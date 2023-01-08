// Decompiled with JetBrains decompiler
// Type: AssignableSideScreenRow
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using TMPro;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/AssignableSideScreenRow")]
public class AssignableSideScreenRow : KMonoBehaviour
{
  [SerializeField]
  private CrewPortrait crewPortraitPrefab;
  [SerializeField]
  private LocText assignmentText;
  public AssignableSideScreen sideScreen;
  private CrewPortrait portraitInstance;
  [MyCmpReq]
  private MultiToggle toggle;
  public IAssignableIdentity targetIdentity;
  public AssignableSideScreenRow.AssignableState currentState;
  private int refreshHandle = -1;

  public void Refresh(object data = null)
  {
    if (!this.sideScreen.targetAssignable.CanAssignTo(this.targetIdentity))
    {
      this.currentState = AssignableSideScreenRow.AssignableState.Disabled;
      ((TMP_Text) this.assignmentText).text = (string) UI.UISIDESCREENS.ASSIGNABLESIDESCREEN.DISABLED;
    }
    else if (this.sideScreen.targetAssignable.assignee == this.targetIdentity)
    {
      this.currentState = AssignableSideScreenRow.AssignableState.Selected;
      ((TMP_Text) this.assignmentText).text = (string) UI.UISIDESCREENS.ASSIGNABLESIDESCREEN.ASSIGNED;
    }
    else
    {
      bool flag = false;
      KMonoBehaviour targetIdentity = this.targetIdentity as KMonoBehaviour;
      if (Object.op_Inequality((Object) targetIdentity, (Object) null))
      {
        Ownables component1 = ((Component) targetIdentity).GetComponent<Ownables>();
        if (Object.op_Inequality((Object) component1, (Object) null))
        {
          AssignableSlotInstance slot = component1.GetSlot(this.sideScreen.targetAssignable.slot);
          if (slot != null && slot.IsAssigned())
          {
            this.currentState = AssignableSideScreenRow.AssignableState.AssignedToOther;
            ((TMP_Text) this.assignmentText).text = ((Component) slot.assignable).GetProperName();
            flag = true;
          }
        }
        Equipment component2 = ((Component) targetIdentity).GetComponent<Equipment>();
        if (Object.op_Inequality((Object) component2, (Object) null))
        {
          AssignableSlotInstance slot = component2.GetSlot(this.sideScreen.targetAssignable.slot);
          if (slot != null && slot.IsAssigned())
          {
            this.currentState = AssignableSideScreenRow.AssignableState.AssignedToOther;
            ((TMP_Text) this.assignmentText).text = ((Component) slot.assignable).GetProperName();
            flag = true;
          }
        }
      }
      if (!flag)
      {
        this.currentState = AssignableSideScreenRow.AssignableState.Unassigned;
        ((TMP_Text) this.assignmentText).text = (string) UI.UISIDESCREENS.ASSIGNABLESIDESCREEN.UNASSIGNED;
      }
    }
    this.toggle.ChangeState((int) this.currentState);
  }

  protected virtual void OnCleanUp()
  {
    if (this.refreshHandle == -1)
      Game.Instance.Unsubscribe(this.refreshHandle);
    base.OnCleanUp();
  }

  public void SetContent(
    IAssignableIdentity identity_object,
    Action<IAssignableIdentity> selectionCallback,
    AssignableSideScreen assignableSideScreen)
  {
    if (this.refreshHandle == -1)
      Game.Instance.Unsubscribe(this.refreshHandle);
    this.refreshHandle = Game.Instance.Subscribe(-2146166042, (Action<object>) (o =>
    {
      if (!Object.op_Inequality((Object) this, (Object) null) || !Object.op_Inequality((Object) ((Component) this).gameObject, (Object) null) || !((Component) this).gameObject.activeInHierarchy)
        return;
      this.Refresh();
    }));
    this.toggle = ((Component) this).GetComponent<MultiToggle>();
    this.sideScreen = assignableSideScreen;
    this.targetIdentity = identity_object;
    if (Object.op_Equality((Object) this.portraitInstance, (Object) null))
    {
      this.portraitInstance = Util.KInstantiateUI<CrewPortrait>(((Component) this.crewPortraitPrefab).gameObject, ((Component) this).gameObject, false);
      this.portraitInstance.transform.SetSiblingIndex(1);
      this.portraitInstance.SetAlpha(1f);
    }
    this.toggle.onClick = (System.Action) (() => selectionCallback(this.targetIdentity));
    this.portraitInstance.SetIdentityObject(identity_object, false);
    ((Component) this).GetComponent<ToolTip>().OnToolTip = new Func<string>(this.GetTooltip);
    this.Refresh();
  }

  private string GetTooltip()
  {
    ToolTip component = ((Component) this).GetComponent<ToolTip>();
    component.ClearMultiStringTooltip();
    if (this.targetIdentity != null && !this.targetIdentity.IsNull())
    {
      switch (this.currentState)
      {
        case AssignableSideScreenRow.AssignableState.Selected:
          component.AddMultiStringTooltip(string.Format((string) UI.UISIDESCREENS.ASSIGNABLESIDESCREEN.UNASSIGN_TOOLTIP, (object) this.targetIdentity.GetProperName()), (TextStyleSetting) null);
          break;
        case AssignableSideScreenRow.AssignableState.Disabled:
          component.AddMultiStringTooltip(string.Format((string) UI.UISIDESCREENS.ASSIGNABLESIDESCREEN.DISABLED_TOOLTIP, (object) this.targetIdentity.GetProperName()), (TextStyleSetting) null);
          break;
        default:
          component.AddMultiStringTooltip(string.Format((string) UI.UISIDESCREENS.ASSIGNABLESIDESCREEN.ASSIGN_TO_TOOLTIP, (object) this.targetIdentity.GetProperName()), (TextStyleSetting) null);
          break;
      }
    }
    return "";
  }

  public enum AssignableState
  {
    Selected,
    AssignedToOther,
    Unassigned,
    Disabled,
  }
}
