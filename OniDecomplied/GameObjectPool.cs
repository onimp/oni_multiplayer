// Decompiled with JetBrains decompiler
// Type: GameObjectPool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class GameObjectPool : ObjectPool<GameObject>
{
  public GameObjectPool(Func<GameObject> instantiator, int initial_count = 0)
    : base(instantiator, initial_count)
  {
  }

  public override GameObject GetInstance() => base.GetInstance();

  public void Destroy()
  {
    for (int index = this.unused.Count - 1; index >= 0; --index)
      Object.Destroy((Object) this.unused.Pop());
  }
}
