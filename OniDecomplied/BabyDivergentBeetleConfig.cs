// Decompiled with JetBrains decompiler
// Type: BabyDivergentBeetleConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class BabyDivergentBeetleConfig : IEntityConfig
{
  public const string ID = "DivergentBeetleBaby";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public GameObject CreatePrefab()
  {
    GameObject divergentBeetle = DivergentBeetleConfig.CreateDivergentBeetle("DivergentBeetleBaby", (string) CREATURES.SPECIES.DIVERGENT.VARIANT_BEETLE.BABY.NAME, (string) CREATURES.SPECIES.DIVERGENT.VARIANT_BEETLE.BABY.DESC, "baby_critter_kanim", true);
    EntityTemplates.ExtendEntityToBeingABaby(divergentBeetle, Tag.op_Implicit("DivergentBeetle"));
    return divergentBeetle;
  }

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
