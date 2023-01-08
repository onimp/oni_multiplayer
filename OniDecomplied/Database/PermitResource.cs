// Decompiled with JetBrains decompiler
// Type: Database.PermitResource
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace Database
{
  public abstract class PermitResource : Resource
  {
    public string PermitId;
    public PermitCategory PermitCategory;
    public PermitRarity Rarity;

    public PermitResource(string id, PermitCategory permitCategory, PermitRarity rarity)
      : this(id, id, permitCategory, rarity)
    {
    }

    public PermitResource(
      string id,
      string Name,
      PermitCategory permitCategory,
      PermitRarity rarity)
      : base(id, Name)
    {
      this.PermitId = id;
      this.PermitCategory = permitCategory;
      this.Rarity = rarity;
    }

    public abstract PermitPresentationInfo GetPermitPresentationInfo();

    public bool IsOwnable() => PermitItems.IsPermitOwnable(this.PermitId);

    public bool IsUnlocked() => PermitItems.IsPermitUnlocked(this.PermitId);
  }
}
