// Decompiled with JetBrains decompiler
// Type: SpiceInstance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;

[Serializable]
public struct SpiceInstance
{
  public Tag Id;
  public float TotalKG;

  public AttributeModifier CalorieModifier => SpiceGrinder.SettingOptions[this.Id].Spice.CalorieModifier;

  public AttributeModifier FoodModifier => SpiceGrinder.SettingOptions[this.Id].Spice.FoodModifier;

  public Effect StatBonus => SpiceGrinder.SettingOptions[this.Id].StatBonus;
}
