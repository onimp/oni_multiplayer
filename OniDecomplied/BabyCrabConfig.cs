// Decompiled with JetBrains decompiler
// Type: BabyCrabConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class BabyCrabConfig : IEntityConfig
{
  public const string ID = "CrabBaby";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject crab = CrabConfig.CreateCrab("CrabBaby", (string) CREATURES.SPECIES.CRAB.BABY.NAME, (string) CREATURES.SPECIES.CRAB.BABY.DESC, "baby_pincher_kanim", true, "BabyCrabShell");
    EntityTemplates.ExtendEntityToBeingABaby(crab, Tag.op_Implicit("Crab"), "BabyCrabShell");
    return crab;
  }

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
