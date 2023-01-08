// Decompiled with JetBrains decompiler
// Type: Database.ClothingOutfitResource
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

namespace Database
{
  public class ClothingOutfitResource : Resource
  {
    public string[] itemsInOutfit { get; private set; }

    public ClothingOutfitResource(string id, string[] items_in_outfit)
      : base(id, (ResourceSet) null, (string) null)
    {
      this.itemsInOutfit = items_in_outfit;
    }

    public ClothingOutfitResource(string id, string[] items_in_outfit, LocString name)
      : base((string) name, (ResourceSet) null, (string) null)
    {
      this.itemsInOutfit = items_in_outfit;
    }

    public Tuple<Sprite, Color> GetUISprite()
    {
      Sprite sprite = Assets.GetSprite(HashedString.op_Implicit("unknown"));
      return new Tuple<Sprite, Color>(sprite, Object.op_Inequality((Object) sprite, (Object) null) ? Color.white : Color.clear);
    }
  }
}
