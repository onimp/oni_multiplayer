// Decompiled with JetBrains decompiler
// Type: Hit
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using UnityEngine;

public class Hit
{
  private AttackProperties properties;
  private GameObject target;

  public Hit(AttackProperties properties, GameObject target)
  {
    this.properties = properties;
    this.target = target;
    this.DeliverHit();
  }

  private float rollDamage() => (float) Mathf.RoundToInt(Random.Range(this.properties.base_damage_min, this.properties.base_damage_max));

  private void DeliverHit()
  {
    Health component1 = this.target.GetComponent<Health>();
    if (!Object.op_Implicit((Object) component1))
      return;
    EventExtensions.Trigger(this.target, -787691065, (object) ((Component) this.properties.attacker).GetComponent<FactionAlignment>());
    float amount = this.rollDamage() * (1f + this.target.GetComponent<AttackableBase>().GetDamageMultiplier());
    component1.Damage(amount);
    if (this.properties.effects == null)
      return;
    Effects component2 = this.target.GetComponent<Effects>();
    if (!Object.op_Implicit((Object) component2))
      return;
    foreach (AttackEffect effect in this.properties.effects)
    {
      if ((double) Random.Range(0.0f, 100f) < (double) effect.effectProbability * 100.0)
        component2.Add(effect.effectID, true);
    }
  }
}
