// Decompiled with JetBrains decompiler
// Type: LeadSuitTank
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class LeadSuitTank : KMonoBehaviour, IGameObjectEffectDescriptor
{
  [Serialize]
  public float batteryCharge = 1f;
  public const float REFILL_PERCENT = 0.25f;
  public float batteryDuration = 200f;
  public float coolingOperationalTemperature = 333.15f;
  public Tag coolantTag;
  private LeadSuitMonitor.Instance leadSuitMonitor;
  private static readonly EventSystem.IntraObjectHandler<LeadSuitTank> OnEquippedDelegate = new EventSystem.IntraObjectHandler<LeadSuitTank>((Action<LeadSuitTank, object>) ((component, data) => component.OnEquipped(data)));
  private static readonly EventSystem.IntraObjectHandler<LeadSuitTank> OnUnequippedDelegate = new EventSystem.IntraObjectHandler<LeadSuitTank>((Action<LeadSuitTank, object>) ((component, data) => component.OnUnequipped(data)));

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<LeadSuitTank>(-1617557748, LeadSuitTank.OnEquippedDelegate);
    this.Subscribe<LeadSuitTank>(-170173755, LeadSuitTank.OnUnequippedDelegate);
  }

  public float PercentFull() => this.batteryCharge;

  public bool IsEmpty() => (double) this.batteryCharge <= 0.0;

  public bool IsFull() => (double) this.PercentFull() >= 1.0;

  public bool NeedsRecharging() => (double) this.PercentFull() <= 0.25;

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    string str = string.Format((string) UI.UISIDESCREENS.FABRICATORSIDESCREEN.EFFECTS.LEADSUIT_BATTERY, (object) GameUtil.GetFormattedPercent(this.PercentFull() * 100f));
    descriptors.Add(new Descriptor(str, str, (Descriptor.DescriptorType) 1, false));
    return descriptors;
  }

  private void OnEquipped(object data)
  {
    Equipment equipment = (Equipment) data;
    NameDisplayScreen.Instance.SetSuitBatteryDisplay(((Component) equipment).GetComponent<MinionAssignablesProxy>().GetTargetGameObject(), new Func<float>(this.PercentFull), true);
    this.leadSuitMonitor = new LeadSuitMonitor.Instance((IStateMachineTarget) this, ((Component) equipment).GetComponent<MinionAssignablesProxy>().GetTargetGameObject());
    this.leadSuitMonitor.StartSM();
    if (!this.NeedsRecharging())
      return;
    ((Component) equipment).GetComponent<MinionAssignablesProxy>().GetTargetGameObject().AddTag(GameTags.SuitBatteryLow);
  }

  private void OnUnequipped(object data)
  {
    Equipment equipment = (Equipment) data;
    if (!equipment.destroyed)
    {
      ((Component) equipment).GetComponent<MinionAssignablesProxy>().GetTargetGameObject().RemoveTag(GameTags.SuitBatteryLow);
      ((Component) equipment).GetComponent<MinionAssignablesProxy>().GetTargetGameObject().RemoveTag(GameTags.SuitBatteryOut);
      NameDisplayScreen.Instance.SetSuitBatteryDisplay(((Component) equipment).GetComponent<MinionAssignablesProxy>().GetTargetGameObject(), (Func<float>) null, false);
    }
    if (this.leadSuitMonitor == null)
      return;
    this.leadSuitMonitor.StopSM("Removed leadsuit tank");
    this.leadSuitMonitor = (LeadSuitMonitor.Instance) null;
  }
}
