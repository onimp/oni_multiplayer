// Decompiled with JetBrains decompiler
// Type: BabySquirrelConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class BabySquirrelConfig : IEntityConfig
{
  public const string ID = "SquirrelBaby";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject squirrel = SquirrelConfig.CreateSquirrel("SquirrelBaby", (string) CREATURES.SPECIES.SQUIRREL.BABY.NAME, (string) CREATURES.SPECIES.SQUIRREL.BABY.DESC, "baby_squirrel_kanim", true);
    EntityTemplates.ExtendEntityToBeingABaby(squirrel, Tag.op_Implicit("Squirrel"));
    return squirrel;
  }

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
