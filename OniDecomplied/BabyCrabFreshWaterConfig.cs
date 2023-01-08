// Decompiled with JetBrains decompiler
// Type: BabyCrabFreshWaterConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class BabyCrabFreshWaterConfig : IEntityConfig
{
  public const string ID = "CrabFreshWaterBaby";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject crabFreshWater = CrabFreshWaterConfig.CreateCrabFreshWater("CrabFreshWaterBaby", (string) CREATURES.SPECIES.CRAB.VARIANT_FRESH_WATER.BABY.NAME, (string) CREATURES.SPECIES.CRAB.VARIANT_FRESH_WATER.BABY.DESC, "baby_pincher_kanim", true);
    EntityTemplates.ExtendEntityToBeingABaby(crabFreshWater, Tag.op_Implicit("CrabFreshWater"));
    return crabFreshWater;
  }

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
