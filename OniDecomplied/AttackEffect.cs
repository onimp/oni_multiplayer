// Decompiled with JetBrains decompiler
// Type: AttackEffect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

[Serializable]
public class AttackEffect
{
  public string effectID;
  public float effectProbability;

  public AttackEffect(string ID, float probability)
  {
    this.effectID = ID;
    this.effectProbability = probability;
  }
}
