// Decompiled with JetBrains decompiler
// Type: BabyHatchConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class BabyHatchConfig : IEntityConfig
{
  public const string ID = "HatchBaby";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject hatch = HatchConfig.CreateHatch("HatchBaby", (string) CREATURES.SPECIES.HATCH.BABY.NAME, (string) CREATURES.SPECIES.HATCH.BABY.DESC, "baby_hatch_kanim", true);
    EntityTemplates.ExtendEntityToBeingABaby(hatch, Tag.op_Implicit("Hatch"));
    return hatch;
  }

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
