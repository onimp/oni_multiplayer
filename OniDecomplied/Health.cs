// Decompiled with JetBrains decompiler
// Type: Health
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using KSerialization;
using System;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/Health")]
public class Health : KMonoBehaviour, ISaveLoadable
{
  [Serialize]
  public bool CanBeIncapacitated;
  [Serialize]
  public Health.HealthState State;
  [Serialize]
  private Death source_of_death;
  public HealthBar healthBar;
  private Effects effects;
  private AmountInstance amountInstance;

  public AmountInstance GetAmountInstance => this.amountInstance;

  public float hitPoints
  {
    get => this.amountInstance.value;
    set => this.amountInstance.value = value;
  }

  public float maxHitPoints => this.amountInstance.GetMax();

  public float percent() => this.hitPoints / this.maxHitPoints;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    Components.Health.Add(this);
    this.amountInstance = Db.Get().Amounts.HitPoints.Lookup(((Component) this).gameObject);
    this.amountInstance.value = this.amountInstance.GetMax();
    this.amountInstance.OnDelta += new Action<float>(this.OnHealthChanged);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (this.State == Health.HealthState.Incapacitated || (double) this.hitPoints == 0.0)
    {
      if (this.CanBeIncapacitated)
        this.Incapacitate(GameTags.HitPointsDepleted);
      else
        this.Kill();
    }
    if (this.State != Health.HealthState.Incapacitated && this.State != Health.HealthState.Dead)
      this.UpdateStatus();
    this.effects = ((Component) this).GetComponent<Effects>();
    this.UpdateHealthBar();
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    Components.Health.Remove(this);
  }

  public void UpdateHealthBar()
  {
    if (Object.op_Equality((Object) NameDisplayScreen.Instance, (Object) null))
      return;
    bool flag = this.State == Health.HealthState.Dead || this.State == Health.HealthState.Incapacitated || (double) this.hitPoints >= (double) this.maxHitPoints;
    NameDisplayScreen.Instance.SetHealthDisplay(((Component) this).gameObject, new Func<float>(this.percent), !flag);
  }

  private void Recover() => ((Component) this).GetComponent<KPrefabID>().RemoveTag(GameTags.HitPointsDepleted);

  public void OnHealthChanged(float delta)
  {
    this.Trigger(-1664904872, (object) delta);
    if (this.State != Health.HealthState.Invincible)
    {
      if ((double) this.hitPoints == 0.0 && !this.IsDefeated())
      {
        if (this.CanBeIncapacitated)
          this.Incapacitate(GameTags.HitPointsDepleted);
        else
          this.Kill();
      }
      else
        ((Component) this).GetComponent<KPrefabID>().RemoveTag(GameTags.HitPointsDepleted);
    }
    this.UpdateStatus();
    this.UpdateWoundEffects();
    this.UpdateHealthBar();
  }

  [ContextMenu("DoDamage")]
  public void DoDamage() => this.Damage(1f);

  public void Damage(float amount)
  {
    if (this.State != Health.HealthState.Invincible)
      this.hitPoints = Mathf.Max(0.0f, this.hitPoints - amount);
    this.OnHealthChanged(-amount);
  }

  private void UpdateWoundEffects()
  {
    if (!Object.op_Implicit((Object) this.effects))
      return;
    switch (this.State)
    {
      case Health.HealthState.Perfect:
        this.effects.Remove("LightWounds");
        this.effects.Remove("ModerateWounds");
        this.effects.Remove("SevereWounds");
        break;
      case Health.HealthState.Alright:
        this.effects.Remove("LightWounds");
        this.effects.Remove("ModerateWounds");
        this.effects.Remove("SevereWounds");
        break;
      case Health.HealthState.Scuffed:
        this.effects.Remove("ModerateWounds");
        this.effects.Remove("SevereWounds");
        if (this.effects.HasEffect("LightWounds"))
          break;
        this.effects.Add("LightWounds", true);
        break;
      case Health.HealthState.Injured:
        this.effects.Remove("LightWounds");
        this.effects.Remove("SevereWounds");
        if (this.effects.HasEffect("ModerateWounds"))
          break;
        this.effects.Add("ModerateWounds", true);
        break;
      case Health.HealthState.Critical:
        this.effects.Remove("LightWounds");
        this.effects.Remove("ModerateWounds");
        if (this.effects.HasEffect("SevereWounds"))
          break;
        this.effects.Add("SevereWounds", true);
        break;
      case Health.HealthState.Incapacitated:
        this.effects.Remove("LightWounds");
        this.effects.Remove("ModerateWounds");
        this.effects.Remove("SevereWounds");
        break;
      case Health.HealthState.Dead:
        this.effects.Remove("LightWounds");
        this.effects.Remove("ModerateWounds");
        this.effects.Remove("SevereWounds");
        break;
    }
  }

  private void UpdateStatus()
  {
    float num = this.hitPoints / this.maxHitPoints;
    Health.HealthState healthState = this.State != Health.HealthState.Invincible ? ((double) num < 1.0 ? ((double) num < 0.85000002384185791 ? ((double) num < 0.6600000262260437 ? ((double) num < 0.33 ? ((double) num <= 0.0 ? ((double) num != 0.0 ? Health.HealthState.Dead : Health.HealthState.Incapacitated) : Health.HealthState.Critical) : Health.HealthState.Injured) : Health.HealthState.Scuffed) : Health.HealthState.Alright) : Health.HealthState.Perfect) : Health.HealthState.Invincible;
    if (this.State == healthState)
      return;
    if (this.State == Health.HealthState.Incapacitated && healthState != Health.HealthState.Dead)
      this.Recover();
    if (healthState == Health.HealthState.Perfect)
      this.Trigger(-1491582671, (object) this);
    this.State = healthState;
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    if (this.State != Health.HealthState.Dead && this.State != Health.HealthState.Perfect && this.State != Health.HealthState.Alright)
      component.SetStatusItem(Db.Get().StatusItemCategories.Hitpoints, Db.Get().CreatureStatusItems.HealthStatus, (object) this.State);
    else
      component.SetStatusItem(Db.Get().StatusItemCategories.Hitpoints, (StatusItem) null);
  }

  public bool IsIncapacitated() => this.State == Health.HealthState.Incapacitated;

  public bool IsDefeated() => this.State == Health.HealthState.Incapacitated || this.State == Health.HealthState.Dead;

  public void Incapacitate(Tag cause)
  {
    this.State = Health.HealthState.Incapacitated;
    ((Component) this).GetComponent<KPrefabID>().AddTag(cause, false);
    this.Damage(this.hitPoints);
  }

  private void Kill()
  {
    if (((Component) this).gameObject.GetSMI<DeathMonitor.Instance>() == null)
      return;
    ((Component) this).gameObject.GetSMI<DeathMonitor.Instance>().Kill(Db.Get().Deaths.Slain);
  }

  public enum HealthState
  {
    Perfect,
    Alright,
    Scuffed,
    Injured,
    Critical,
    Incapacitated,
    Dead,
    Invincible,
  }
}
