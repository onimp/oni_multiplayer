// Decompiled with JetBrains decompiler
// Type: BabyWormConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class BabyWormConfig : IEntityConfig
{
  public const string ID = "DivergentWormBaby";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public GameObject CreatePrefab()
  {
    GameObject worm = DivergentWormConfig.CreateWorm("DivergentWormBaby", (string) CREATURES.SPECIES.DIVERGENT.VARIANT_WORM.BABY.NAME, (string) CREATURES.SPECIES.DIVERGENT.VARIANT_WORM.BABY.DESC, "baby_worm_kanim", true);
    EntityTemplates.ExtendEntityToBeingABaby(worm, Tag.op_Implicit("DivergentWorm"));
    return worm;
  }

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
