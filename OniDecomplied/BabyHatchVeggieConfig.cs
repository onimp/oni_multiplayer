// Decompiled with JetBrains decompiler
// Type: BabyHatchVeggieConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class BabyHatchVeggieConfig : IEntityConfig
{
  public const string ID = "HatchVeggieBaby";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject hatch = HatchVeggieConfig.CreateHatch("HatchVeggieBaby", (string) CREATURES.SPECIES.HATCH.VARIANT_VEGGIE.BABY.NAME, (string) CREATURES.SPECIES.HATCH.VARIANT_VEGGIE.BABY.DESC, "baby_hatch_kanim", true);
    EntityTemplates.ExtendEntityToBeingABaby(hatch, Tag.op_Implicit("HatchVeggie"));
    return hatch;
  }

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
