// Decompiled with JetBrains decompiler
// Type: AttackProperties
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

[Serializable]
public class AttackProperties
{
  public Weapon attacker;
  public AttackProperties.DamageType damageType;
  public AttackProperties.TargetType targetType;
  public float base_damage_min;
  public float base_damage_max;
  public int maxHits;
  public float aoe_radius = 2f;
  public List<AttackEffect> effects;

  public enum DamageType
  {
    Standard,
  }

  public enum TargetType
  {
    Single,
    AreaOfEffect,
  }
}
