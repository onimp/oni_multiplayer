// Decompiled with JetBrains decompiler
// Type: ObjectPool`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;

public class ObjectPool<T>
{
  protected Stack<T> unused;
  protected Func<T> instantiator;

  public ObjectPool(Func<T> instantiator, int initial_count = 0)
  {
    this.instantiator = instantiator;
    this.unused = new Stack<T>(initial_count);
    for (int index = 0; index < initial_count; ++index)
      this.unused.Push(instantiator());
  }

  public virtual T GetInstance()
  {
    T obj = default (T);
    return this.unused.Count <= 0 ? this.instantiator() : this.unused.Pop();
  }

  public void ReleaseInstance(T instance)
  {
    if (object.Equals((object) instance, (object) null))
      return;
    this.unused.Push(instance);
  }
}
