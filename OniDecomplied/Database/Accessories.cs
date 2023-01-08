// Decompiled with JetBrains decompiler
// Type: Database.Accessories
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

namespace Database
{
  public class Accessories : ResourceSet<Accessory>
  {
    public Accessories(ResourceSet parent)
      : base(nameof (Accessories), parent)
    {
    }

    public void AddAccessories(string id, KAnimFile anim_file)
    {
      if (!Object.op_Inequality((Object) anim_file, (Object) null))
        return;
      KAnim.Build build = anim_file.GetData().build;
      for (int index = 0; index < build.symbols.Length; ++index)
      {
        string str = HashCache.Get().Get(build.symbols[index].hash);
        AccessorySlot slot = Db.Get().AccessorySlots.Find(KAnimHashedString.op_Implicit(str));
        if (slot != null)
        {
          Accessory accessory = new Accessory(id + str, (ResourceSet) this, slot, anim_file.batchTag, build.symbols[index], anim_file);
          slot.accessories.Add(accessory);
          HashCache.Get().Add(((HashedString) ref accessory.IdHash).HashValue, accessory.Id);
        }
      }
    }
  }
}
