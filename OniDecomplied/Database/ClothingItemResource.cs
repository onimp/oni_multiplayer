// Decompiled with JetBrains decompiler
// Type: Database.ClothingItemResource
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

namespace Database
{
  public class ClothingItemResource : PermitResource
  {
    private static Dictionary<KAnimFile, Sprite> knownUISprites = new Dictionary<KAnimFile, Sprite>();
    private static HashSet<KAnimFile> knownNoSpriteAvailble = new HashSet<KAnimFile>();

    public string Description { get; private set; }

    public string animFilename { get; private set; }

    public KAnimFile AnimFile { get; private set; }

    public PermitCategory Category { get; private set; }

    public ClothingItemResource(
      string id,
      string name,
      string desc,
      PermitCategory category,
      PermitRarity rarity,
      string animFile)
      : base(id, name, category, rarity)
    {
      this.Description = desc;
      this.AnimFile = Assets.GetAnim(HashedString.op_Implicit(animFile));
      this.animFilename = animFile;
      this.Category = category;
      Db.Get().Accessories.AddAccessories(id, this.AnimFile);
    }

    public Tuple<Sprite, Color> GetUISprite()
    {
      if (Object.op_Equality((Object) this.AnimFile, (Object) null))
        Debug.LogError((object) ("Clothing AnimFile is null: " + this.animFilename));
      Sprite uiSprite = ClothingItemResource.GetUISprite(this.AnimFile);
      return new Tuple<Sprite, Color>(uiSprite, Object.op_Inequality((Object) uiSprite, (Object) null) ? Color.white : Color.clear);
    }

    public override PermitPresentationInfo GetPermitPresentationInfo()
    {
      PermitPresentationInfo presentationInfo = new PermitPresentationInfo();
      presentationInfo.name = this.Name != null ? this.Name : "NAME NOT POPULATED (DbClothingItems)";
      presentationInfo.description = this.Description;
      presentationInfo.sprite = this.GetUISprite().first;
      presentationInfo.category = this.PermitCategory;
      presentationInfo.SetFacadeForText((string) UI.KLEI_INVENTORY_SCREEN.CLOTHING_ITEM_FACADE_FOR);
      presentationInfo.SetRarityDetailsFor(this.Rarity);
      presentationInfo.ownedCount = PermitItems.GetOwnedCount((PermitResource) this);
      return presentationInfo;
    }

    public static Sprite GetUISprite(KAnimFile animFile)
    {
      if (ClothingItemResource.knownNoSpriteAvailble.Contains(animFile))
        return Assets.GetSprite(HashedString.op_Implicit("unknown"));
      Sprite uiSprite1;
      if (ClothingItemResource.knownUISprites.TryGetValue(animFile, out uiSprite1))
        return uiSprite1;
      Option<Sprite> uiSprite2 = ClothingItemResource.MaybeGenerateUISprite(animFile);
      if (uiSprite2.HasValue)
      {
        ClothingItemResource.knownUISprites.Add(animFile, uiSprite2.Value);
        return uiSprite2.Value;
      }
      ClothingItemResource.knownNoSpriteAvailble.Add(animFile);
      return Assets.GetSprite(HashedString.op_Implicit("unknown"));
    }

    public static Option<Sprite> MaybeGenerateUISprite(KAnimFile animFile)
    {
      if (Object.op_Equality((Object) animFile, (Object) null))
        return new Option<Sprite>();
      if (animFile.GetData() == null)
        return new Option<Sprite>();
      KAnim.Build build = animFile.GetData().build;
      if (build.textureCount == 0)
        return new Option<Sprite>();
      Texture2D texture = build.GetTexture(0);
      if (Object.op_Equality((Object) texture, (Object) null) || !Object.op_Implicit((Object) texture))
        return new Option<Sprite>();
      KAnim.Build.Symbol symbol = build.GetSymbol(KAnimHashedString.op_Implicit("ui"));
      if (symbol == null)
        return new Option<Sprite>();
      int firstFrameIdx = symbol.firstFrameIdx;
      if (firstFrameIdx < 0 || firstFrameIdx >= build.frames.Length)
        return new Option<Sprite>();
      KAnim.Build.SymbolFrame frame = build.frames[firstFrameIdx];
      float x1 = frame.uvMin.x;
      float x2 = frame.uvMax.x;
      float y1 = frame.uvMax.y;
      float y2 = frame.uvMin.y;
      Rect rect1 = new Rect();
      ((Rect) ref rect1).x = (float) (int) ((double) ((Texture) texture).width * (double) x1);
      ((Rect) ref rect1).y = (float) (int) ((double) ((Texture) texture).height * (double) y1);
      ((Rect) ref rect1).width = (float) (int) ((double) ((Texture) texture).width * (double) Mathf.Abs(x2 - x1));
      ((Rect) ref rect1).height = (float) (int) ((double) ((Texture) texture).height * (double) Mathf.Abs(y2 - y1));
      Rect rect2 = rect1;
      float num = 100f;
      if ((double) ((Rect) ref rect2).width != 0.0)
        num = (float) (100.0 / ((double) Mathf.Abs(frame.bboxMax.x - frame.bboxMin.x) / (double) ((Rect) ref rect2).width));
      Sprite uiSprite = Sprite.Create(texture, rect2, Vector2.zero, num, 0U, (SpriteMeshType) 0);
      ((Object) uiSprite).name = string.Format("{0}:{1}:{2}:{3}", (object) ((Object) texture).name, (object) ((Object) animFile).name, (object) frame.sourceFrameNum, (object) false);
      return (Option<Sprite>) uiSprite;
    }
  }
}
