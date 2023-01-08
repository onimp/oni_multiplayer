// Decompiled with JetBrains decompiler
// Type: BabyCrabWoodConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class BabyCrabWoodConfig : IEntityConfig
{
  public const string ID = "CrabWoodBaby";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject crabWood = CrabWoodConfig.CreateCrabWood("CrabWoodBaby", (string) CREATURES.SPECIES.CRAB.VARIANT_WOOD.BABY.NAME, (string) CREATURES.SPECIES.CRAB.VARIANT_WOOD.BABY.DESC, "baby_pincher_kanim", true, "BabyCrabWoodShell");
    EntityTemplates.ExtendEntityToBeingABaby(crabWood, Tag.op_Implicit("CrabWood"), "BabyCrabWoodShell");
    return crabWood;
  }

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
