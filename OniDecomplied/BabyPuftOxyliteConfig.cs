// Decompiled with JetBrains decompiler
// Type: BabyPuftOxyliteConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class BabyPuftOxyliteConfig : IEntityConfig
{
  public const string ID = "PuftOxyliteBaby";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject puftOxylite = PuftOxyliteConfig.CreatePuftOxylite("PuftOxyliteBaby", (string) CREATURES.SPECIES.PUFT.VARIANT_OXYLITE.BABY.NAME, (string) CREATURES.SPECIES.PUFT.VARIANT_OXYLITE.BABY.DESC, "baby_puft_kanim", true);
    EntityTemplates.ExtendEntityToBeingABaby(puftOxylite, Tag.op_Implicit("PuftOxylite"));
    return puftOxylite;
  }

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
