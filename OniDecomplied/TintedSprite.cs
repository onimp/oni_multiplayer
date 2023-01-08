// Decompiled with JetBrains decompiler
// Type: TintedSprite
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Diagnostics;
using UnityEngine;

[DebuggerDisplay("{name}")]
[Serializable]
public class TintedSprite : ISerializationCallbackReceiver
{
  [ReadOnly]
  public string name;
  public Sprite sprite;
  public Color color;

  public void OnAfterDeserialize()
  {
  }

  public void OnBeforeSerialize()
  {
    if (!Object.op_Inequality((Object) this.sprite, (Object) null))
      return;
    this.name = ((Object) this.sprite).name;
  }
}
