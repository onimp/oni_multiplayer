// Decompiled with JetBrains decompiler
// Type: Durability
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.CustomSettings;
using KSerialization;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Durability")]
public class Durability : KMonoBehaviour
{
  private static readonly EventSystem.IntraObjectHandler<Durability> OnEquippedDelegate = new EventSystem.IntraObjectHandler<Durability>((Action<Durability, object>) ((component, data) => component.OnEquipped()));
  private static readonly EventSystem.IntraObjectHandler<Durability> OnUnequippedDelegate = new EventSystem.IntraObjectHandler<Durability>((Action<Durability, object>) ((component, data) => component.OnUnequipped()));
  [Serialize]
  private bool isEquipped;
  [Serialize]
  private float timeEquipped;
  [Serialize]
  private float durability = 1f;
  public float durabilityLossPerCycle = -0.1f;
  public string wornEquipmentPrefabID;
  private float difficultySettingMod = 1f;

  public float TimeEquipped
  {
    get => this.timeEquipped;
    set => this.timeEquipped = value;
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<Durability>(-1617557748, Durability.OnEquippedDelegate);
    this.Subscribe<Durability>(-170173755, Durability.OnUnequippedDelegate);
  }

  protected virtual void OnSpawn()
  {
    ((Component) this).GetComponent<KSelectable>().AddStatusItem(Db.Get().MiscStatusItems.Durability, (object) ((Component) this).gameObject);
    SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting(CustomGameSettingConfigs.Durability);
    if (currentQualitySetting == null)
      return;
    switch (currentQualitySetting.id)
    {
      case "Indestructible":
        this.difficultySettingMod = EQUIPMENT.SUITS.INDESTRUCTIBLE_DURABILITY_MOD;
        break;
      case "Reinforced":
        this.difficultySettingMod = EQUIPMENT.SUITS.REINFORCED_DURABILITY_MOD;
        break;
      case "Flimsy":
        this.difficultySettingMod = EQUIPMENT.SUITS.FLIMSY_DURABILITY_MOD;
        break;
      case "Threadbare":
        this.difficultySettingMod = EQUIPMENT.SUITS.THREADBARE_DURABILITY_MOD;
        break;
    }
  }

  private void OnEquipped()
  {
    if (this.isEquipped)
      return;
    this.isEquipped = true;
    this.timeEquipped = GameClock.Instance.GetTimeInCycles();
  }

  private void OnUnequipped()
  {
    if (!this.isEquipped)
      return;
    this.isEquipped = false;
    this.DeltaDurability((GameClock.Instance.GetTimeInCycles() - this.timeEquipped) * this.durabilityLossPerCycle);
  }

  private void DeltaDurability(float delta)
  {
    delta *= this.difficultySettingMod;
    this.durability = Mathf.Clamp01(this.durability + delta);
  }

  public void ConvertToWornObject()
  {
    GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(this.wornEquipmentPrefabID)), Grid.SceneLayer.Ore);
    TransformExtensions.SetPosition(gameObject.transform, TransformExtensions.GetPosition(this.transform));
    gameObject.GetComponent<PrimaryElement>().SetElement(((Component) this).GetComponent<PrimaryElement>().ElementID, false);
    gameObject.SetActive(true);
    EquippableFacade component1 = ((Component) this).GetComponent<EquippableFacade>();
    if (Object.op_Inequality((Object) component1, (Object) null))
      gameObject.GetComponent<RepairableEquipment>().facadeID = component1.FacadeID;
    Storage component2 = ((Component) this).gameObject.GetComponent<Storage>();
    if (Object.op_Implicit((Object) component2))
    {
      JetSuitTank component3 = ((Component) this).gameObject.GetComponent<JetSuitTank>();
      if (Object.op_Implicit((Object) component3))
        component2.AddLiquid(SimHashes.Petroleum, component3.amount, ((Component) this).GetComponent<PrimaryElement>().Temperature, byte.MaxValue, 0);
      component2.DropAll(false, false, new Vector3(), true, (List<GameObject>) null);
    }
    Util.KDestroyGameObject(((Component) this).gameObject);
  }

  public float GetDurability() => this.isEquipped ? this.durability - (GameClock.Instance.GetTimeInCycles() - this.timeEquipped) * this.durabilityLossPerCycle : this.durability;

  public bool IsWornOut() => (double) this.GetDurability() <= 0.0;
}
