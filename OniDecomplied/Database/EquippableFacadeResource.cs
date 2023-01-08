// Decompiled with JetBrains decompiler
// Type: Database.EquippableFacadeResource
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

namespace Database
{
  public class EquippableFacadeResource : PermitResource
  {
    public string BuildOverride { get; private set; }

    public string DefID { get; private set; }

    public KAnimFile AnimFile { get; private set; }

    public EquippableFacadeResource(
      string id,
      string name,
      string buildOverride,
      string defID,
      string animFile)
      : base(id, name, PermitCategory.Equipment, PermitRarity.Unknown)
    {
      this.DefID = defID;
      this.BuildOverride = buildOverride;
      this.AnimFile = Assets.GetAnim(HashedString.op_Implicit(animFile));
    }

    public Tuple<Sprite, Color> GetUISprite()
    {
      if (Object.op_Equality((Object) this.AnimFile, (Object) null))
        Debug.LogError((object) ("Facade AnimFile is null: " + this.DefID));
      Sprite fromMultiObjectAnim = Def.GetUISpriteFromMultiObjectAnim(this.AnimFile);
      return new Tuple<Sprite, Color>(fromMultiObjectAnim, Object.op_Inequality((Object) fromMultiObjectAnim, (Object) null) ? Color.white : Color.clear);
    }

    public override PermitPresentationInfo GetPermitPresentationInfo()
    {
      PermitPresentationInfo presentationInfo = new PermitPresentationInfo();
      presentationInfo.name = StringEntry.op_Implicit(Strings.Get("STRINGS.EQUIPMENT.PREFABS." + this.DefID.ToUpper() + ".FACADES." + this.Name.ToUpper()));
      presentationInfo.sprite = this.GetUISprite().first;
      presentationInfo.category = this.PermitCategory;
      presentationInfo.buildOverride = this.BuildOverride;
      presentationInfo.SetRarityDetailsFor(this.Rarity);
      presentationInfo.ownedCount = PermitItems.GetOwnedCount((PermitResource) this);
      GameObject prefab = Assets.TryGetPrefab(Tag.op_Implicit(this.DefID));
      if (Object.op_Equality((Object) prefab, (Object) null) || !Object.op_Implicit((Object) prefab))
        presentationInfo.SetFacadeForPrefabID(this.DefID);
      else
        presentationInfo.SetFacadeForPrefabName(prefab.GetProperName());
      return presentationInfo;
    }
  }
}
