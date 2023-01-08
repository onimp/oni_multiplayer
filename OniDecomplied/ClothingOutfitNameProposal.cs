// Decompiled with JetBrains decompiler
// Type: ClothingOutfitNameProposal
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public readonly struct ClothingOutfitNameProposal
{
  public readonly string candidateName;
  public readonly ClothingOutfitNameProposal.Result result;

  private ClothingOutfitNameProposal(string candidateName, ClothingOutfitNameProposal.Result result)
  {
    this.candidateName = candidateName;
    this.result = result;
  }

  public static ClothingOutfitNameProposal ForNewOutfit(string candidateName)
  {
    if (string.IsNullOrEmpty(candidateName))
      return Make(ClothingOutfitNameProposal.Result.Error_NoInputName);
    return ClothingOutfitTarget.DoesExist(candidateName) ? Make(ClothingOutfitNameProposal.Result.Error_NameAlreadyExists) : Make(ClothingOutfitNameProposal.Result.NewOutfit);

    ClothingOutfitNameProposal Make(ClothingOutfitNameProposal.Result result) => new ClothingOutfitNameProposal(candidateName, result);
  }

  public static ClothingOutfitNameProposal FromExistingOutfit(
    string candidateName,
    ClothingOutfitTarget existingOutfit,
    bool isSameNameAllowed)
  {
    if (string.IsNullOrEmpty(candidateName))
      return Make(ClothingOutfitNameProposal.Result.Error_NoInputName);
    if (!ClothingOutfitTarget.DoesExist(candidateName))
      return Make(ClothingOutfitNameProposal.Result.NewOutfit);
    if (!isSameNameAllowed || !(candidateName == existingOutfit.ReadName()))
      return Make(ClothingOutfitNameProposal.Result.Error_NameAlreadyExists);
    return existingOutfit.CanWriteName ? Make(ClothingOutfitNameProposal.Result.SameOutfit) : Make(ClothingOutfitNameProposal.Result.Error_SameOutfitReadonly);

    ClothingOutfitNameProposal Make(ClothingOutfitNameProposal.Result result) => new ClothingOutfitNameProposal(candidateName, result);
  }

  public enum Result
  {
    None,
    NewOutfit,
    SameOutfit,
    Error_NoInputName,
    Error_NameAlreadyExists,
    Error_SameOutfitReadonly,
  }
}
