// Decompiled with JetBrains decompiler
// Type: Database.ArtableStage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;

namespace Database
{
  public class ArtableStage : PermitResource
  {
    public string id;
    public string description;
    public string anim;
    public string animFile;
    public string prefabId;
    public string symbolName;
    public int decor;
    public bool cheerOnComplete;
    public ArtableStatusItem statusItem;

    public ArtableStage(
      string id,
      string name,
      string desc,
      PermitRarity rarity,
      string animFile,
      string anim,
      int decor_value,
      bool cheer_on_complete,
      ArtableStatusItem status_item,
      string prefabId,
      string symbolName = "")
      : base(id, name, PermitCategory.Artwork, rarity)
    {
      this.id = id;
      this.description = desc;
      this.animFile = animFile;
      this.anim = anim;
      this.symbolName = symbolName;
      this.decor = decor_value;
      this.cheerOnComplete = cheer_on_complete;
      this.statusItem = status_item;
      this.prefabId = prefabId;
    }

    public override PermitPresentationInfo GetPermitPresentationInfo()
    {
      PermitPresentationInfo presentationInfo = new PermitPresentationInfo();
      presentationInfo.name = this.Name;
      presentationInfo.description = this.description;
      presentationInfo.category = this.PermitCategory;
      presentationInfo.sprite = Def.GetUISpriteFromMultiObjectAnim(Assets.GetAnim(HashedString.op_Implicit(this.animFile)));
      presentationInfo.SetFacadeForText(UI.KLEI_INVENTORY_SCREEN.ARTABLE_ITEM_FACADE_FOR.Replace("{ConfigProperName}", Assets.GetPrefab(Tag.op_Implicit(this.prefabId)).GetProperName()).Replace("{ArtableQuality}", this.statusItem.GetName((object) null)));
      presentationInfo.SetRarityDetailsFor(this.Rarity);
      presentationInfo.ownedCount = PermitItems.GetOwnedCount((PermitResource) this);
      return presentationInfo;
    }
  }
}
