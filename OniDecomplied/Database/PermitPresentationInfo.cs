// Decompiled with JetBrains decompiler
// Type: Database.PermitPresentationInfo
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

namespace Database
{
  public struct PermitPresentationInfo
  {
    public string name;
    public string description;
    public PermitCategory category;
    public Sprite sprite;
    public string buildOverride;
    public Option<int> ownedCount;
    public bool isNone;

    public string facadeFor { get; private set; }

    public string rarityDetails { get; private set; }

    public static Sprite GetUnknownSprite() => Assets.GetSprite(HashedString.op_Implicit("unknown"));

    public bool IsOwnable() => this.ownedCount.HasValue;

    public bool IsUnlocked() => !this.ownedCount.HasValue || 0 < this.ownedCount.Value;

    public void SetFacadeForPrefabName(string prefabName) => this.facadeFor = UI.KLEI_INVENTORY_SCREEN.ITEM_FACADE_FOR.Replace("{ConfigProperName}", prefabName);

    public void SetFacadeForPrefabID(string prefabId) => this.facadeFor = UI.KLEI_INVENTORY_SCREEN.ITEM_FACADE_FOR.Replace("{ConfigProperName}", Assets.GetPrefab(Tag.op_Implicit(prefabId)).GetProperName());

    public void SetFacadeForText(string text) => this.facadeFor = text;

    public void SetRarityDetailsFor(PermitRarity rarity) => this.rarityDetails = UI.KLEI_INVENTORY_SCREEN.ITEM_RARITY_DETAILS.Replace("{RarityName}", rarity.GetLocStringName());

    public void SetRarityDetailsText(string text) => this.rarityDetails = text;
  }
}
