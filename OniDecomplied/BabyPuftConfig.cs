// Decompiled with JetBrains decompiler
// Type: BabyPuftConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class BabyPuftConfig : IEntityConfig
{
  public const string ID = "PuftBaby";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject puft = PuftConfig.CreatePuft("PuftBaby", (string) CREATURES.SPECIES.PUFT.BABY.NAME, (string) CREATURES.SPECIES.PUFT.BABY.DESC, "baby_puft_kanim", true);
    EntityTemplates.ExtendEntityToBeingABaby(puft, Tag.op_Implicit("Puft"));
    return puft;
  }

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
