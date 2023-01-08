// Decompiled with JetBrains decompiler
// Type: OutfitDesignerScreenConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public readonly struct OutfitDesignerScreenConfig
{
  public readonly ClothingOutfitTarget sourceTarget;
  public readonly Option<ClothingOutfitTarget> outfitTemplate;
  public readonly Option<Personality> minionPersonality;
  public readonly Option<GameObject> targetMinionInstance;
  public readonly Action<ClothingOutfitTarget> onWriteToOutfitTargetFn;
  public readonly bool isValid;

  public OutfitDesignerScreenConfig(
    Option<ClothingOutfitTarget> sourceTargetOpt,
    Option<Personality> minionPersonality,
    Option<GameObject> targetMinionInstance,
    Action<ClothingOutfitTarget> onWriteToOutfitTargetFn = null)
  {
    this.sourceTarget = sourceTargetOpt.HasValue ? sourceTargetOpt.Value : ClothingOutfitTarget.ForNewOutfit();
    this.outfitTemplate = this.sourceTarget.IsTemplateOutfit() ? Option.Some<ClothingOutfitTarget>(this.sourceTarget) : (Option<ClothingOutfitTarget>) Option.None;
    this.minionPersonality = minionPersonality;
    this.targetMinionInstance = targetMinionInstance;
    this.onWriteToOutfitTargetFn = onWriteToOutfitTargetFn;
    this.isValid = true;
    ClothingOutfitTarget.MinionInstance minionInstance;
    if (!this.sourceTarget.Is<ClothingOutfitTarget.MinionInstance>(out minionInstance))
      return;
    Debug.Assert(targetMinionInstance.HasValue && targetMinionInstance == minionInstance.minionInstance);
  }

  public OutfitDesignerScreenConfig WithOutfit(Option<ClothingOutfitTarget> sourceTarget) => new OutfitDesignerScreenConfig(sourceTarget, this.minionPersonality, this.targetMinionInstance, this.onWriteToOutfitTargetFn);

  public OutfitDesignerScreenConfig OnWriteToOutfitTarget(
    Action<ClothingOutfitTarget> onWriteToOutfitTargetFn)
  {
    return new OutfitDesignerScreenConfig((Option<ClothingOutfitTarget>) this.sourceTarget, this.minionPersonality, this.targetMinionInstance, onWriteToOutfitTargetFn);
  }

  public static OutfitDesignerScreenConfig Mannequin(Option<ClothingOutfitTarget> outfit) => new OutfitDesignerScreenConfig(outfit, (Option<Personality>) Option.None, (Option<GameObject>) Option.None);

  public static OutfitDesignerScreenConfig Minion(
    Option<ClothingOutfitTarget> outfit,
    Personality personality)
  {
    return new OutfitDesignerScreenConfig(outfit, (Option<Personality>) personality, (Option<GameObject>) Option.None);
  }

  public static OutfitDesignerScreenConfig Minion(
    Option<ClothingOutfitTarget> outfit,
    GameObject targetMinionInstance)
  {
    Personality minionPersonality = Db.Get().Personalities.Get(targetMinionInstance.GetComponent<MinionIdentity>().personalityResourceId);
    return new OutfitDesignerScreenConfig((Option<ClothingOutfitTarget>) (outfit.HasValue ? outfit.Value : ClothingOutfitTarget.FromMinion(targetMinionInstance)), (Option<Personality>) minionPersonality, (Option<GameObject>) targetMinionInstance);
  }

  public void ApplyAndOpenScreen()
  {
    LockerNavigator.Instance.outfitDesignerScreen.GetComponent<OutfitDesignerScreen>().Configure(this);
    LockerNavigator.Instance.PushScreen(LockerNavigator.Instance.outfitDesignerScreen);
  }
}
