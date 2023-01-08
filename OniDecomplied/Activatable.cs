// Decompiled with JetBrains decompiler
// Type: Activatable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Activatable")]
public class Activatable : Workable, ISidescreenButtonControl
{
  public Operational.Flag.Type ActivationFlagType;
  private Operational.Flag activatedFlag;
  [Serialize]
  private bool activated;
  [Serialize]
  private bool awaitingActivation;
  private Guid statusItem;
  private Chore activateChore;
  public System.Action onActivate;
  [SerializeField]
  private ButtonMenuTextOverride textOverride;

  public bool IsActivated => this.activated;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.activatedFlag = new Operational.Flag("activated", this.ActivationFlagType);
  }

  protected override void OnSpawn()
  {
    this.UpdateFlag();
    if (!this.awaitingActivation || this.activateChore != null)
      return;
    this.CreateChore();
  }

  protected override void OnCompleteWork(Worker worker)
  {
    this.activated = true;
    if (this.onActivate != null)
      this.onActivate();
    this.awaitingActivation = false;
    this.UpdateFlag();
    Prioritizable.RemoveRef(((Component) this).gameObject);
    base.OnCompleteWork(worker);
  }

  private void UpdateFlag()
  {
    ((Component) this).GetComponent<Operational>().SetFlag(this.activatedFlag, this.activated);
    ((Component) this).GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.DuplicantActivationRequired, !this.activated);
    this.Trigger(-1909216579, (object) this.IsActivated);
  }

  private void CreateChore()
  {
    if (this.activateChore != null)
      return;
    Prioritizable.AddRef(((Component) this).gameObject);
    this.activateChore = (Chore) new WorkChore<Activatable>(Db.Get().ChoreTypes.Toggle, (IStateMachineTarget) this, only_when_operational: false);
    if (string.IsNullOrEmpty(this.requiredSkillPerk))
      return;
    this.shouldShowSkillPerkStatusItem = true;
    this.requireMinionToWork = true;
    this.UpdateStatusItem();
  }

  private void CancelChore()
  {
    if (this.activateChore == null)
      return;
    this.activateChore.Cancel("User cancelled");
    this.activateChore = (Chore) null;
  }

  public string SidescreenButtonText => this.activateChore != null ? (string) (this.textOverride.IsValid ? this.textOverride.CancelText : UI.USERMENUACTIONS.ACTIVATEBUILDING.ACTIVATE_CANCEL) : (string) (this.textOverride.IsValid ? this.textOverride.Text : UI.USERMENUACTIONS.ACTIVATEBUILDING.ACTIVATE);

  public string SidescreenButtonTooltip => this.activateChore != null ? (string) (this.textOverride.IsValid ? this.textOverride.CancelToolTip : UI.USERMENUACTIONS.ACTIVATEBUILDING.TOOLTIP_CANCEL) : (string) (this.textOverride.IsValid ? this.textOverride.ToolTip : UI.USERMENUACTIONS.ACTIVATEBUILDING.TOOLTIP_ACTIVATE);

  public bool SidescreenEnabled() => !this.activated;

  public void SetButtonTextOverride(ButtonMenuTextOverride text) => this.textOverride = text;

  public void OnSidescreenButtonPressed()
  {
    if (this.activateChore == null)
      this.CreateChore();
    else
      this.CancelChore();
    this.awaitingActivation = this.activateChore != null;
  }

  public bool SidescreenButtonInteractable() => !this.activated;

  public int ButtonSideScreenSortOrder() => 20;
}
