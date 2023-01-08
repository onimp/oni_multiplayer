// Decompiled with JetBrains decompiler
// Type: BabyPuftBleachstoneConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class BabyPuftBleachstoneConfig : IEntityConfig
{
  public const string ID = "PuftBleachstoneBaby";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject puftBleachstone = PuftBleachstoneConfig.CreatePuftBleachstone("PuftBleachstoneBaby", (string) CREATURES.SPECIES.PUFT.VARIANT_BLEACHSTONE.BABY.NAME, (string) CREATURES.SPECIES.PUFT.VARIANT_BLEACHSTONE.BABY.DESC, "baby_puft_kanim", true);
    EntityTemplates.ExtendEntityToBeingABaby(puftBleachstone, Tag.op_Implicit("PuftBleachstone"));
    return puftBleachstone;
  }

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst) => BasePuftConfig.OnSpawn(inst);
}
