// Decompiled with JetBrains decompiler
// Type: Database.MonumentPartResource
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

namespace Database
{
  public class MonumentPartResource : PermitResource
  {
    public MonumentPartResource.Part part;

    public KAnimFile AnimFile { get; private set; }

    public string SymbolName { get; private set; }

    public string State { get; private set; }

    public MonumentPartResource(
      string id,
      string animFilename,
      string state,
      string symbolName,
      MonumentPartResource.Part part)
      : base(id, PermitCategory.Artwork, PermitRarity.Unknown)
    {
      this.AnimFile = Assets.GetAnim(HashedString.op_Implicit(animFilename));
      this.SymbolName = symbolName;
      this.State = state;
      this.part = part;
    }

    public Tuple<Sprite, Color> GetUISprite()
    {
      Sprite sprite = Assets.GetSprite(HashedString.op_Implicit("unknown"));
      return new Tuple<Sprite, Color>(sprite, Object.op_Inequality((Object) sprite, (Object) null) ? Color.white : Color.clear);
    }

    public override PermitPresentationInfo GetPermitPresentationInfo()
    {
      PermitPresentationInfo presentationInfo = new PermitPresentationInfo();
      presentationInfo.name = string.Format("_{0}", (object) this.Id);
      presentationInfo.sprite = this.GetUISprite().first;
      presentationInfo.category = this.PermitCategory;
      presentationInfo.SetRarityDetailsFor(this.Rarity);
      presentationInfo.ownedCount = PermitItems.GetOwnedCount((PermitResource) this);
      presentationInfo.SetFacadeForText("_monument part");
      return presentationInfo;
    }

    public enum Part
    {
      Bottom,
      Middle,
      Top,
    }
  }
}
