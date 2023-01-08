// Decompiled with JetBrains decompiler
// Type: Database.DbStickerBomb
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace Database
{
  public class DbStickerBomb : PermitResource
  {
    public string id;
    public string stickerName;
    public string sticker;
    public KAnimFile animFile;
    private const string stickerAnimPrefix = "idle_sticker";
    private const string stickerSymbolPrefix = "sticker";

    public DbStickerBomb(string id, string stickerName, string animfilename, string sticker)
      : base(id, PermitCategory.Artwork, PermitRarity.Unknown)
    {
      this.id = id;
      this.stickerName = stickerName;
      this.sticker = sticker;
      this.animFile = Assets.GetAnim(HashedString.op_Implicit(animfilename));
    }

    public override PermitPresentationInfo GetPermitPresentationInfo()
    {
      PermitPresentationInfo presentationInfo = new PermitPresentationInfo();
      presentationInfo.name = this.stickerName;
      presentationInfo.sprite = Def.GetUISpriteFromMultiObjectAnim(this.animFile, string.Format("{0}_{1}", (object) "idle_sticker", (object) this.sticker), symbolName: string.Format("{0}_{1}", (object) "sticker", (object) this.sticker));
      presentationInfo.category = this.PermitCategory;
      presentationInfo.SetRarityDetailsFor(this.Rarity);
      presentationInfo.ownedCount = PermitItems.GetOwnedCount((PermitResource) this);
      return presentationInfo;
    }
  }
}
