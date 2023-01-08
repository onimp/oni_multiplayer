// Decompiled with JetBrains decompiler
// Type: WeaponExtensions
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public static class WeaponExtensions
{
  public static Weapon AddWeapon(
    this GameObject prefab,
    float base_damage_min,
    float base_damage_max,
    AttackProperties.DamageType attackType = AttackProperties.DamageType.Standard,
    AttackProperties.TargetType targetType = AttackProperties.TargetType.Single,
    int maxHits = 1,
    float aoeRadius = 0.0f)
  {
    Weapon weapon = prefab.AddOrGet<Weapon>();
    weapon.Configure(base_damage_min, base_damage_max, attackType, targetType, maxHits, aoeRadius);
    return weapon;
  }
}
