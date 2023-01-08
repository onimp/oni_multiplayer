// Decompiled with JetBrains decompiler
// Type: Refinery
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/Refinery")]
public class Refinery : KMonoBehaviour
{
  protected virtual void OnSpawn() => base.OnSpawn();

  [Serializable]
  public struct OrderSaveData
  {
    public string id;
    public bool infinite;

    public OrderSaveData(string id, bool infinite)
    {
      this.id = id;
      this.infinite = infinite;
    }
  }
}
