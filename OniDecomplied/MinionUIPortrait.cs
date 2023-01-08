// Decompiled with JetBrains decompiler
// Type: MinionUIPortrait
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class MinionUIPortrait : IEntityConfig
{
  public static string ID = nameof (MinionUIPortrait);

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject entity = EntityTemplates.CreateEntity(MinionUIPortrait.ID, MinionUIPortrait.ID);
    RectTransform rectTransform = entity.AddOrGet<RectTransform>();
    rectTransform.anchorMin = new Vector2(0.0f, 0.0f);
    rectTransform.anchorMax = new Vector2(1f, 1f);
    rectTransform.pivot = new Vector2(0.5f, 0.0f);
    rectTransform.anchoredPosition = new Vector2(0.0f, 0.0f);
    rectTransform.sizeDelta = new Vector2(0.0f, 0.0f);
    LayoutElement layoutElement = entity.AddOrGet<LayoutElement>();
    layoutElement.preferredHeight = 100f;
    layoutElement.preferredWidth = 100f;
    entity.AddOrGet<BoxCollider2D>().size = new Vector2(1f, 1f);
    entity.AddOrGet<Accessorizer>();
    KBatchedAnimController kbatchedAnimController = entity.AddOrGet<KBatchedAnimController>();
    kbatchedAnimController.materialType = (KAnimBatchGroup.MaterialType) 3;
    kbatchedAnimController.animScale = 0.5f;
    kbatchedAnimController.setScaleFromAnim = false;
    kbatchedAnimController.animOverrideSize = new Vector2(100f, 120f);
    kbatchedAnimController.AnimFiles = new KAnimFile[6]
    {
      Assets.GetAnim(HashedString.op_Implicit("body_comp_default_kanim")),
      Assets.GetAnim(HashedString.op_Implicit("anim_idles_default_kanim")),
      Assets.GetAnim(HashedString.op_Implicit("anim_idle_healthy_kanim")),
      Assets.GetAnim(HashedString.op_Implicit("anim_cheer_kanim")),
      Assets.GetAnim(HashedString.op_Implicit("inventory_screen_dupe_kanim")),
      Assets.GetAnim(HashedString.op_Implicit("anim_react_wave_shy_kanim"))
    };
    SymbolOverrideControllerUtil.AddToPrefab(entity);
    MinionConfig.ConfigureSymbols(entity);
    return entity;
  }

  public void OnPrefabInit(GameObject go)
  {
  }

  public void OnSpawn(GameObject go)
  {
  }
}
