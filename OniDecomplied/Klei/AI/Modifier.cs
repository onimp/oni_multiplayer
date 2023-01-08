// Decompiled with JetBrains decompiler
// Type: Klei.AI.Modifier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

namespace Klei.AI
{
  public class Modifier : Resource
  {
    public string description;
    public List<AttributeModifier> SelfModifiers = new List<AttributeModifier>();

    public Modifier(string id, string name, string description)
      : base(id, name)
    {
      this.description = description;
    }

    public void Add(AttributeModifier modifier)
    {
      if (!(modifier.AttributeId != ""))
        return;
      this.SelfModifiers.Add(modifier);
    }

    public virtual void AddTo(Attributes attributes)
    {
      foreach (AttributeModifier selfModifier in this.SelfModifiers)
        attributes.Add(selfModifier);
    }

    public virtual void RemoveFrom(Attributes attributes)
    {
      foreach (AttributeModifier selfModifier in this.SelfModifiers)
        attributes.Remove(selfModifier);
    }
  }
}
