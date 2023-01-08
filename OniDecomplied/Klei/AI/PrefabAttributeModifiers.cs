// Decompiled with JetBrains decompiler
// Type: Klei.AI.PrefabAttributeModifiers
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

namespace Klei.AI
{
  [AddComponentMenu("KMonoBehaviour/scripts/PrefabAttributeModifiers")]
  public class PrefabAttributeModifiers : KMonoBehaviour
  {
    public List<AttributeModifier> descriptors = new List<AttributeModifier>();

    protected virtual void OnPrefabInit() => base.OnPrefabInit();

    public void AddAttributeDescriptor(AttributeModifier modifier) => this.descriptors.Add(modifier);

    public void RemovePrefabAttribute(AttributeModifier modifier) => this.descriptors.Remove(modifier);
  }
}
