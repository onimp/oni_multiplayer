// Decompiled with JetBrains decompiler
// Type: AttributeModifierExpectation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using UnityEngine;

public class AttributeModifierExpectation : Expectation
{
  public AttributeModifier modifier;
  public Sprite icon;

  public AttributeModifierExpectation(
    string id,
    string name,
    string description,
    AttributeModifier modifier,
    Sprite icon)
    : base(id, name, description, (Action<MinionResume>) (resume => resume.GetAttributes().Get(modifier.AttributeId).Add(modifier)), (Action<MinionResume>) (resume => resume.GetAttributes().Get(modifier.AttributeId).Remove(modifier)))
  {
    this.modifier = modifier;
    this.icon = icon;
  }
}
