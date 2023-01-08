// Decompiled with JetBrains decompiler
// Type: OutfitBrowserScreenConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public readonly struct OutfitBrowserScreenConfig
{
  public readonly Option<ClothingOutfitTarget> selectedTarget;
  public readonly Option<Personality> minionPersonality;
  public readonly Option<GameObject> targetMinionInstance;
  public readonly bool isValid;
  public readonly bool isPickingOutfitForDupe;

  public OutfitBrowserScreenConfig(
    Option<ClothingOutfitTarget> selectedTarget,
    Option<Personality> minionPersonality,
    Option<GameObject> minionInstance)
  {
    this.selectedTarget = selectedTarget;
    this.minionPersonality = minionPersonality;
    this.isPickingOutfitForDupe = minionPersonality.HasValue || minionInstance.HasValue;
    this.targetMinionInstance = minionInstance;
    this.isValid = true;
  }

  public OutfitBrowserScreenConfig WithOutfit(Option<ClothingOutfitTarget> sourceTarget) => new OutfitBrowserScreenConfig(sourceTarget, this.minionPersonality, this.targetMinionInstance);

  public string GetMinionName()
  {
    if (this.targetMinionInstance.HasValue)
      return this.targetMinionInstance.Value.GetProperName();
    return this.minionPersonality.HasValue ? this.minionPersonality.Value.Name : "-";
  }

  public static OutfitBrowserScreenConfig Mannequin() => new OutfitBrowserScreenConfig((Option<ClothingOutfitTarget>) Option.None, (Option<Personality>) Option.None, (Option<GameObject>) Option.None);

  public static OutfitBrowserScreenConfig Minion(Personality personality) => new OutfitBrowserScreenConfig((Option<ClothingOutfitTarget>) Option.None, (Option<Personality>) personality, (Option<GameObject>) Option.None);

  public static OutfitBrowserScreenConfig Minion(GameObject minionInstance)
  {
    Personality minionPersonality = Db.Get().Personalities.Get(minionInstance.GetComponent<MinionIdentity>().personalityResourceId);
    return new OutfitBrowserScreenConfig((Option<ClothingOutfitTarget>) ClothingOutfitTarget.FromMinion(minionInstance), (Option<Personality>) minionPersonality, (Option<GameObject>) minionInstance);
  }

  public void ApplyAndOpenScreen()
  {
    LockerNavigator.Instance.outfitBrowserScreen.GetComponent<OutfitBrowserScreen>().Configure(this);
    LockerNavigator.Instance.PushScreen(LockerNavigator.Instance.outfitBrowserScreen);
  }
}
