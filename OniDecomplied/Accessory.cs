// Decompiled with JetBrains decompiler
// Type: Accessory
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class Accessory : Resource
{
  public KAnim.Build.Symbol symbol { get; private set; }

  public HashedString batchSource { get; private set; }

  public AccessorySlot slot { get; private set; }

  public KAnimFile animFile { get; private set; }

  public Accessory(
    string id,
    ResourceSet parent,
    AccessorySlot slot,
    HashedString batchSource,
    KAnim.Build.Symbol symbol,
    KAnimFile animFile = null,
    KAnimFile defaultAnimFile = null)
    : base(id, parent, (string) null)
  {
    this.slot = slot;
    this.symbol = symbol;
    this.batchSource = batchSource;
    this.animFile = animFile;
  }

  public bool IsDefault() => Object.op_Equality((Object) this.animFile, (Object) this.slot.defaultAnimFile);
}
