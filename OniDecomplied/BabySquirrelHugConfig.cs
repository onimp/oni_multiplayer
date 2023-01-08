// Decompiled with JetBrains decompiler
// Type: BabySquirrelHugConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class BabySquirrelHugConfig : IEntityConfig
{
  public const string ID = "SquirrelHugBaby";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject squirrelHug = SquirrelHugConfig.CreateSquirrelHug("SquirrelHugBaby", (string) CREATURES.SPECIES.SQUIRREL.VARIANT_HUG.BABY.NAME, (string) CREATURES.SPECIES.SQUIRREL.VARIANT_HUG.BABY.DESC, "baby_squirrel_kanim", true);
    EntityTemplates.ExtendEntityToBeingABaby(squirrelHug, Tag.op_Implicit("SquirrelHug"));
    return squirrelHug;
  }

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
