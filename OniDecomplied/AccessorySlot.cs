// Decompiled with JetBrains decompiler
// Type: AccessorySlot
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class AccessorySlot : Resource
{
  private KAnimFile file;

  public KAnimHashedString targetSymbolId { get; private set; }

  public List<Accessory> accessories { get; private set; }

  public KAnimFile AnimFile => this.file;

  public KAnimFile defaultAnimFile { get; private set; }

  public bool Required { get; private set; }

  public AccessorySlot(string id, ResourceSet parent, KAnimFile swap_build, bool required = false)
    : base(id, parent, (string) null)
  {
    if (Object.op_Equality((Object) swap_build, (Object) null))
      Debug.LogErrorFormat("AccessorySlot {0} missing swap_build", new object[1]
      {
        (object) id
      });
    this.targetSymbolId = new KAnimHashedString("snapTo_" + id.ToLower());
    this.accessories = new List<Accessory>();
    this.file = swap_build;
    this.Required = required;
    this.defaultAnimFile = swap_build;
  }

  public AccessorySlot(
    string id,
    ResourceSet parent,
    KAnimHashedString target_symbol_id,
    KAnimFile swap_build,
    bool required = false,
    KAnimFile defaultAnimFile = null)
    : base(id, parent, (string) null)
  {
    if (Object.op_Equality((Object) swap_build, (Object) null))
      Debug.LogErrorFormat("AccessorySlot {0} missing swap_build", new object[1]
      {
        (object) id
      });
    this.targetSymbolId = target_symbol_id;
    this.accessories = new List<Accessory>();
    this.file = swap_build;
    this.Required = required;
    this.defaultAnimFile = Object.op_Inequality((Object) defaultAnimFile, (Object) null) ? defaultAnimFile : swap_build;
  }

  public void AddAccessories(KAnimFile default_build, ResourceSet parent)
  {
    KAnim.Build build = default_build.GetData().build;
    default_build.GetData().build.GetSymbol(this.targetSymbolId);
    string lower = this.Id.ToLower();
    for (int index = 0; index < build.symbols.Length; ++index)
    {
      string id = HashCache.Get().Get(build.symbols[index].hash);
      if (id.StartsWith(lower))
      {
        Accessory accessory = new Accessory(id, parent, this, this.file.batchTag, build.symbols[index], default_build);
        this.accessories.Add(accessory);
        HashCache.Get().Add(((HashedString) ref accessory.IdHash).HashValue, accessory.Id);
      }
    }
  }

  public Accessory Lookup(string id) => this.Lookup(new HashedString(id));

  public Accessory Lookup(HashedString full_id) => !((HashedString) ref full_id).IsValid ? (Accessory) null : this.accessories.Find((Predicate<Accessory>) (a => HashedString.op_Equality(a.IdHash, full_id)));
}
