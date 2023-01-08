// Decompiled with JetBrains decompiler
// Type: BabyDreckoConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class BabyDreckoConfig : IEntityConfig
{
  public const string ID = "DreckoBaby";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject drecko = DreckoConfig.CreateDrecko("DreckoBaby", (string) CREATURES.SPECIES.DRECKO.BABY.NAME, (string) CREATURES.SPECIES.DRECKO.BABY.DESC, "baby_drecko_kanim", true);
    EntityTemplates.ExtendEntityToBeingABaby(drecko, Tag.op_Implicit("Drecko"));
    return drecko;
  }

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
