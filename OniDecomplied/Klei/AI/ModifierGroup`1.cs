// Decompiled with JetBrains decompiler
// Type: Klei.AI.ModifierGroup`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

namespace Klei.AI
{
  public class ModifierGroup<T> : Resource
  {
    public List<T> modifiers = new List<T>();

    public IEnumerator<T> GetEnumerator() => (IEnumerator<T>) this.modifiers.GetEnumerator();

    public T this[int idx] => this.modifiers[idx];

    public int Count => this.modifiers.Count;

    public ModifierGroup(string id, string name)
      : base(id, name)
    {
    }

    public void Add(T modifier) => this.modifiers.Add(modifier);
  }
}
