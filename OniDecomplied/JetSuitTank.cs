// Decompiled with JetBrains decompiler
// Type: JetSuitTank
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/JetSuitTank")]
public class JetSuitTank : KMonoBehaviour, IGameObjectEffectDescriptor
{
  [MyCmpGet]
  private ElementEmitter elementConverter;
  [Serialize]
  public float amount;
  public const float FUEL_CAPACITY = 25f;
  public const float FUEL_BURN_RATE = 0.1f;
  public const float CO2_EMITTED_PER_FUEL_BURNED = 3f;
  public const float EMIT_TEMPERATURE = 473.15f;
  public const float REFILL_PERCENT = 0.25f;
  private JetSuitMonitor.Instance jetSuitMonitor;
  private static readonly EventSystem.IntraObjectHandler<JetSuitTank> OnEquippedDelegate = new EventSystem.IntraObjectHandler<JetSuitTank>((Action<JetSuitTank, object>) ((component, data) => component.OnEquipped(data)));
  private static readonly EventSystem.IntraObjectHandler<JetSuitTank> OnUnequippedDelegate = new EventSystem.IntraObjectHandler<JetSuitTank>((Action<JetSuitTank, object>) ((component, data) => component.OnUnequipped(data)));

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.amount = 25f;
    this.Subscribe<JetSuitTank>(-1617557748, JetSuitTank.OnEquippedDelegate);
    this.Subscribe<JetSuitTank>(-170173755, JetSuitTank.OnUnequippedDelegate);
  }

  public float PercentFull() => this.amount / 25f;

  public bool IsEmpty() => (double) this.amount <= 0.0;

  public bool IsFull() => (double) this.PercentFull() >= 1.0;

  public bool NeedsRecharging() => (double) this.PercentFull() < 0.25;

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    string str = string.Format((string) UI.UISIDESCREENS.FABRICATORSIDESCREEN.EFFECTS.JETSUIT_TANK, (object) GameUtil.GetFormattedMass(this.amount));
    descriptors.Add(new Descriptor(str, str, (Descriptor.DescriptorType) 1, false));
    return descriptors;
  }

  private void OnEquipped(object data)
  {
    Equipment equipment = (Equipment) data;
    NameDisplayScreen.Instance.SetSuitFuelDisplay(((Component) equipment).GetComponent<MinionAssignablesProxy>().GetTargetGameObject(), new Func<float>(this.PercentFull), true);
    this.jetSuitMonitor = new JetSuitMonitor.Instance((IStateMachineTarget) this, ((Component) equipment).GetComponent<MinionAssignablesProxy>().GetTargetGameObject());
    this.jetSuitMonitor.StartSM();
    if (!this.IsEmpty())
      return;
    ((Component) equipment).GetComponent<MinionAssignablesProxy>().GetTargetGameObject().AddTag(GameTags.JetSuitOutOfFuel);
  }

  private void OnUnequipped(object data)
  {
    Equipment equipment = (Equipment) data;
    if (!equipment.destroyed)
    {
      ((Component) equipment).GetComponent<MinionAssignablesProxy>().GetTargetGameObject().RemoveTag(GameTags.JetSuitOutOfFuel);
      NameDisplayScreen.Instance.SetSuitFuelDisplay(((Component) equipment).GetComponent<MinionAssignablesProxy>().GetTargetGameObject(), (Func<float>) null, false);
      Navigator component = ((Component) equipment).GetComponent<MinionAssignablesProxy>().GetTargetGameObject().GetComponent<Navigator>();
      if (Object.op_Implicit((Object) component) && component.CurrentNavType == NavType.Hover)
        component.SetCurrentNavType(NavType.Floor);
    }
    if (this.jetSuitMonitor == null)
      return;
    this.jetSuitMonitor.StopSM("Removed jetsuit tank");
    this.jetSuitMonitor = (JetSuitMonitor.Instance) null;
  }
}
