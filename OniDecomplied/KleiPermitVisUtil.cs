// Decompiled with JetBrains decompiler
// Type: KleiPermitVisUtil
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using UnityEngine;

public static class KleiPermitVisUtil
{
  public const float TILE_SIZE_UI = 176f;

  public static void ConfigureToRenderBuilding(
    KBatchedAnimController buildingKAnim,
    BuildingFacadeResource buildingPermit)
  {
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit(buildingPermit.AnimFile));
    buildingKAnim.Stop();
    buildingKAnim.SwapAnims(new KAnimFile[1]{ anim });
    buildingKAnim.Play(KleiPermitVisUtil.GetFirstAnimHash(anim), (KAnim.PlayMode) 0);
    Util.rectTransform((Component) buildingKAnim).sizeDelta = Vector2.op_Multiply(176f, Vector2.one);
  }

  public static void ConfigureToRenderBuilding(
    KBatchedAnimController buildingKAnim,
    BuildingDef buildingDef)
  {
    buildingKAnim.Stop();
    buildingKAnim.SwapAnims(buildingDef.AnimFiles);
    buildingKAnim.Play(KleiPermitVisUtil.GetFirstAnimHash(buildingDef.AnimFiles[0]), (KAnim.PlayMode) 0);
    Util.rectTransform((Component) buildingKAnim).sizeDelta = Vector2.op_Multiply(176f, Vector2.one);
  }

  public static void ConfigureToRenderBuilding(
    KBatchedAnimController buildingKAnim,
    ArtableStage artablePermit)
  {
    buildingKAnim.Stop();
    buildingKAnim.SwapAnims(new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit(artablePermit.animFile))
    });
    buildingKAnim.Play(HashedString.op_Implicit(artablePermit.anim));
    Util.rectTransform((Component) buildingKAnim).sizeDelta = Vector2.op_Multiply(176f, Vector2.one);
  }

  public static void ConfigureToRenderBuilding(
    KBatchedAnimController buildingKAnim,
    DbStickerBomb artablePermit)
  {
    buildingKAnim.Stop();
    buildingKAnim.SwapAnims(new KAnimFile[1]
    {
      artablePermit.animFile
    });
    (bool hasValue, HashedString hashedString) = KleiPermitVisUtil.GetDefaultStickerAnimHash(artablePermit.animFile);
    int num = hasValue ? 1 : 0;
    HashedString anim_name = hashedString;
    if (num != 0)
    {
      buildingKAnim.Play(anim_name);
    }
    else
    {
      Debug.Assert(false, (object) ("Couldn't find default sticker for sticker " + artablePermit.Id));
      buildingKAnim.Play(KleiPermitVisUtil.GetFirstAnimHash(artablePermit.animFile));
    }
    Util.rectTransform((Component) buildingKAnim).sizeDelta = Vector2.op_Multiply(176f, Vector2.one);
  }

  public static HashedString GetFirstAnimHash(KAnimFile animFile) => animFile.GetData().GetAnim(0).hash;

  public static Option<HashedString> GetDefaultStickerAnimHash(KAnimFile stickerAnimFile)
  {
    KAnimFileData data = stickerAnimFile.GetData();
    for (int index = 0; index < data.animCount; ++index)
    {
      KAnim.Anim anim = data.GetAnim(index);
      if (anim.name.StartsWith("idle_sticker"))
        return (Option<HashedString>) anim.hash;
    }
    return (Option<HashedString>) Option.None;
  }

  public static Option<BuildLocationRule> GetBuildLocationRule(PermitResource permit)
  {
    Option<BuildingDef> buildingDef = KleiPermitVisUtil.GetBuildingDef(permit);
    return !buildingDef.HasValue ? (Option<BuildLocationRule>) Option.None : (Option<BuildLocationRule>) buildingDef.Value.BuildLocationRule;
  }

  public static Option<BuildingDef> GetBuildingDef(PermitResource permit)
  {
    switch (permit)
    {
      case BuildingFacadeResource buildingFacadeResource:
        BuildingComplete component1 = Assets.GetPrefab(Tag.op_Implicit(buildingFacadeResource.PrefabID)).GetComponent<BuildingComplete>();
        return Object.op_Equality((Object) component1, (Object) null) || !Object.op_Implicit((Object) component1) ? (Option<BuildingDef>) Option.None : (Option<BuildingDef>) component1.Def;
      case ArtableStage artableStage:
        BuildingComplete component2 = Assets.GetPrefab(Tag.op_Implicit(artableStage.prefabId)).GetComponent<BuildingComplete>();
        return Object.op_Equality((Object) component2, (Object) null) || !Object.op_Implicit((Object) component2) ? (Option<BuildingDef>) Option.None : (Option<BuildingDef>) component2.Def;
      default:
        return (Option<BuildingDef>) Option.None;
    }
  }
}
