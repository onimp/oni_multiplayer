// Decompiled with JetBrains decompiler
// Type: FoodSplatConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using UnityEngine;

public class FoodSplatConfig : IEntityConfig
{
  public const string ID = "FoodSplat";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab() => EntityTemplates.CreateBasicEntity("FoodSplat", (string) ITEMS.FOOD.FOODSPLAT.NAME, (string) ITEMS.FOOD.FOODSPLAT.DESC, 1f, true, Assets.GetAnim(HashedString.op_Implicit("sticker_a_kanim")), "idle_sticker_a", Grid.SceneLayer.Backwall);

  public void OnPrefabInit(GameObject inst)
  {
    inst.AddOrGet<OccupyArea>().OccupiedCellsOffsets = new CellOffset[1];
    inst.AddComponent<Modifiers>();
    inst.AddOrGet<KSelectable>();
    inst.AddOrGet<DecorProvider>().SetValues(TUNING.DECOR.PENALTY.TIER2);
    inst.AddOrGetDef<Splat.Def>();
    inst.AddOrGet<SplatWorkable>();
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
