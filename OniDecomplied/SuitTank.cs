// Decompiled with JetBrains decompiler
// Type: SuitTank
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/SuitTank")]
public class SuitTank : KMonoBehaviour, IGameObjectEffectDescriptor, OxygenBreather.IGasProvider
{
  [Serialize]
  public string element;
  [Serialize]
  public float amount;
  public Tag elementTag;
  [MyCmpReq]
  public Storage storage;
  public float capacity;
  public const float REFILL_PERCENT = 0.25f;
  public bool underwaterSupport;
  private SuitSuffocationMonitor.Instance suitSuffocationMonitor;
  private static readonly EventSystem.IntraObjectHandler<SuitTank> OnEquippedDelegate = new EventSystem.IntraObjectHandler<SuitTank>((Action<SuitTank, object>) ((component, data) => component.OnEquipped(data)));
  private static readonly EventSystem.IntraObjectHandler<SuitTank> OnUnequippedDelegate = new EventSystem.IntraObjectHandler<SuitTank>((Action<SuitTank, object>) ((component, data) => component.OnUnequipped(data)));

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<SuitTank>(-1617557748, SuitTank.OnEquippedDelegate);
    this.Subscribe<SuitTank>(-170173755, SuitTank.OnUnequippedDelegate);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if ((double) this.amount == 0.0)
      return;
    this.storage.AddGasChunk(SimHashes.Oxygen, this.amount, ((Component) this).GetComponent<PrimaryElement>().Temperature, byte.MaxValue, 0, false);
    this.amount = 0.0f;
  }

  public float GetTankAmount()
  {
    if (Object.op_Equality((Object) this.storage, (Object) null))
      this.storage = ((Component) this).GetComponent<Storage>();
    return this.storage.GetMassAvailable(this.elementTag);
  }

  public float PercentFull() => this.GetTankAmount() / this.capacity;

  public bool IsEmpty() => (double) this.GetTankAmount() <= 0.0;

  public bool IsFull() => (double) this.PercentFull() >= 1.0;

  public bool NeedsRecharging() => (double) this.PercentFull() < 0.25;

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    if (Tag.op_Equality(this.elementTag, GameTags.Breathable))
    {
      string str = this.underwaterSupport ? string.Format((string) UI.UISIDESCREENS.FABRICATORSIDESCREEN.EFFECTS.OXYGEN_TANK_UNDERWATER, (object) GameUtil.GetFormattedMass(this.GetTankAmount())) : string.Format((string) UI.UISIDESCREENS.FABRICATORSIDESCREEN.EFFECTS.OXYGEN_TANK, (object) GameUtil.GetFormattedMass(this.GetTankAmount()));
      descriptors.Add(new Descriptor(str, str, (Descriptor.DescriptorType) 1, false));
    }
    return descriptors;
  }

  private void OnEquipped(object data)
  {
    Equipment equipment = (Equipment) data;
    NameDisplayScreen.Instance.SetSuitTankDisplay(((Component) equipment).GetComponent<MinionAssignablesProxy>().GetTargetGameObject(), new Func<float>(this.PercentFull), true);
    OxygenBreather component = ((Component) equipment).GetComponent<MinionAssignablesProxy>().GetTargetGameObject().GetComponent<OxygenBreather>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.SetGasProvider((OxygenBreather.IGasProvider) this);
    ((Component) component).AddTag(GameTags.HasSuitTank);
  }

  private void OnUnequipped(object data)
  {
    Equipment equipment = (Equipment) data;
    if (equipment.destroyed)
      return;
    NameDisplayScreen.Instance.SetSuitTankDisplay(((Component) equipment).GetComponent<MinionAssignablesProxy>().GetTargetGameObject(), new Func<float>(this.PercentFull), false);
    OxygenBreather component = ((Component) equipment).GetComponent<MinionAssignablesProxy>().GetTargetGameObject().GetComponent<OxygenBreather>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.SetGasProvider((OxygenBreather.IGasProvider) new GasBreatherFromWorldProvider());
    ((Component) component).RemoveTag(GameTags.HasSuitTank);
  }

  public void OnSetOxygenBreather(OxygenBreather oxygen_breather)
  {
    this.suitSuffocationMonitor = new SuitSuffocationMonitor.Instance((IStateMachineTarget) oxygen_breather, this);
    this.suitSuffocationMonitor.StartSM();
  }

  public void OnClearOxygenBreather(OxygenBreather oxygen_breather)
  {
    this.suitSuffocationMonitor.StopSM("Removed suit tank");
    this.suitSuffocationMonitor = (SuitSuffocationMonitor.Instance) null;
  }

  public bool ConsumeGas(OxygenBreather oxygen_breather, float gas_consumed)
  {
    if (this.IsEmpty())
      return false;
    float amount_consumed;
    this.storage.ConsumeAndGetDisease(this.elementTag, gas_consumed, out amount_consumed, out SimUtil.DiseaseInfo _, out float _);
    Game.Instance.accumulators.Accumulate(oxygen_breather.O2Accumulator, amount_consumed);
    ReportManager.Instance.ReportValue(ReportManager.ReportType.OxygenCreated, -amount_consumed, ((Component) oxygen_breather).GetProperName());
    this.Trigger(608245985, (object) ((Component) this).gameObject);
    return true;
  }

  public bool ShouldEmitCO2() => !((Component) this).GetComponent<KPrefabID>().HasTag(GameTags.AirtightSuit);

  public bool ShouldStoreCO2() => ((Component) this).GetComponent<KPrefabID>().HasTag(GameTags.AirtightSuit);

  [ContextMenu("SetToRefillAmount")]
  public void SetToRefillAmount()
  {
    float tankAmount = this.GetTankAmount();
    float num = 0.25f * this.capacity;
    if ((double) tankAmount <= (double) num)
      return;
    this.storage.ConsumeIgnoringDisease(this.elementTag, tankAmount - num);
  }

  [ContextMenu("Empty")]
  public void Empty() => this.storage.ConsumeIgnoringDisease(this.elementTag, this.GetTankAmount());

  [ContextMenu("Fill Tank")]
  public void FillTank()
  {
    this.Empty();
    this.storage.AddGasChunk(SimHashes.Oxygen, this.capacity, 15f, (byte) 0, 0, false, false);
  }
}
