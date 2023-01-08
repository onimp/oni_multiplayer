// Decompiled with JetBrains decompiler
// Type: DefComponent`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[Serializable]
public class DefComponent<T> where T : Component
{
  [SerializeField]
  private T cmp;

  public DefComponent(T cmp) => this.cmp = cmp;

  public T Get(StateMachine.Instance smi)
  {
    T[] components = ((Component) (object) this.cmp).GetComponents<T>();
    int index = 0;
    while (index < components.Length && !Object.op_Equality((Object) (object) components[index], (Object) (object) this.cmp))
      ++index;
    return smi.gameObject.GetComponents<T>()[index];
  }

  public static implicit operator DefComponent<T>(T cmp) => new DefComponent<T>(cmp);
}
