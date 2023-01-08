// Decompiled with JetBrains decompiler
// Type: FactionAlignment
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/FactionAlignment")]
public class FactionAlignment : KMonoBehaviour
{
  [SerializeField]
  public bool canBePlayerTargeted = true;
  [SerializeField]
  public bool updatePrioritizable = true;
  [Serialize]
  private bool alignmentActive = true;
  public FactionManager.FactionID Alignment;
  [Serialize]
  private bool targeted;
  [Serialize]
  private bool targetable = true;
  private static readonly EventSystem.IntraObjectHandler<FactionAlignment> OnDeadTagAddedDelegate = GameUtil.CreateHasTagHandler<FactionAlignment>(GameTags.Dead, (Action<FactionAlignment, object>) ((component, data) => component.OnDeath(data)));
  private static readonly EventSystem.IntraObjectHandler<FactionAlignment> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<FactionAlignment>((Action<FactionAlignment, object>) ((component, data) => component.OnRefreshUserMenu(data)));
  private static readonly EventSystem.IntraObjectHandler<FactionAlignment> SetPlayerTargetedFalseDelegate = new EventSystem.IntraObjectHandler<FactionAlignment>((Action<FactionAlignment, object>) ((component, data) => component.SetPlayerTargeted(false)));

  [MyCmpAdd]
  public Health health { get; private set; }

  public AttackableBase attackable { get; private set; }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.health = ((Component) this).GetComponent<Health>();
    this.attackable = ((Component) this).GetComponent<AttackableBase>();
    Components.FactionAlignments.Add(this);
    this.Subscribe<FactionAlignment>(493375141, FactionAlignment.OnRefreshUserMenuDelegate);
    this.Subscribe<FactionAlignment>(2127324410, FactionAlignment.SetPlayerTargetedFalseDelegate);
    if (this.alignmentActive)
      FactionManager.Instance.GetFaction(this.Alignment).Members.Add(this);
    GameUtil.SubscribeToTags<FactionAlignment>(this, FactionAlignment.OnDeadTagAddedDelegate, true);
    this.UpdateStatusItem();
  }

  protected virtual void OnPrefabInit()
  {
  }

  private void OnDeath(object data) => this.SetAlignmentActive(false);

  public void SetAlignmentActive(bool active)
  {
    this.SetPlayerTargetable(active);
    this.alignmentActive = active;
    if (active)
      FactionManager.Instance.GetFaction(this.Alignment).Members.Add(this);
    else
      FactionManager.Instance.GetFaction(this.Alignment).Members.Remove(this);
  }

  public bool IsAlignmentActive() => FactionManager.Instance.GetFaction(this.Alignment).Members.Contains(this);

  public bool IsPlayerTargeted() => this.targeted;

  public void SetPlayerTargetable(bool state)
  {
    this.targetable = state && this.canBePlayerTargeted;
    if (state)
      return;
    this.SetPlayerTargeted(false);
  }

  public void SetPlayerTargeted(bool state)
  {
    this.targeted = this.canBePlayerTargeted & state && this.targetable;
    this.UpdateStatusItem();
  }

  private void UpdateStatusItem()
  {
    this.TogglePrioritizable(this.targeted);
    if (this.targeted)
      ((Component) this).GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.OrderAttack);
    else
      ((Component) this).GetComponent<KSelectable>().RemoveStatusItem(Db.Get().MiscStatusItems.OrderAttack);
  }

  private void TogglePrioritizable(bool enable)
  {
    Prioritizable component = ((Component) this).GetComponent<Prioritizable>();
    if (Object.op_Equality((Object) component, (Object) null) || !this.updatePrioritizable)
      return;
    if (enable)
    {
      Prioritizable.AddRef(((Component) this).gameObject);
    }
    else
    {
      if (!component.IsPrioritizable())
        return;
      Prioritizable.RemoveRef(((Component) this).gameObject);
    }
  }

  public void SwitchAlignment(FactionManager.FactionID newAlignment)
  {
    this.SetAlignmentActive(false);
    this.Alignment = newAlignment;
    this.SetAlignmentActive(true);
  }

  protected virtual void OnCleanUp()
  {
    Components.FactionAlignments.Remove(this);
    FactionManager.Instance.GetFaction(this.Alignment).Members.Remove(this);
    base.OnCleanUp();
  }

  private void OnRefreshUserMenu(object data)
  {
    if (this.Alignment == FactionManager.FactionID.Duplicant || !this.canBePlayerTargeted || !this.IsAlignmentActive())
      return;
    Game.Instance.userMenu.AddButton(((Component) this).gameObject, !this.targeted ? new KIconButtonMenu.ButtonInfo("action_attack", (string) UI.USERMENUACTIONS.ATTACK.NAME, (System.Action) (() => this.SetPlayerTargeted(true)), tooltipText: ((string) UI.USERMENUACTIONS.ATTACK.TOOLTIP)) : new KIconButtonMenu.ButtonInfo("action_attack", (string) UI.USERMENUACTIONS.CANCELATTACK.NAME, (System.Action) (() => this.SetPlayerTargeted(false)), tooltipText: ((string) UI.USERMENUACTIONS.CANCELATTACK.TOOLTIP)));
  }
}
