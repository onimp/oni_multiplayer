// Decompiled with JetBrains decompiler
// Type: BabyPuftAlphaConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class BabyPuftAlphaConfig : IEntityConfig
{
  public const string ID = "PuftAlphaBaby";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject puftAlpha = PuftAlphaConfig.CreatePuftAlpha("PuftAlphaBaby", (string) CREATURES.SPECIES.PUFT.VARIANT_ALPHA.BABY.NAME, (string) CREATURES.SPECIES.PUFT.VARIANT_ALPHA.BABY.DESC, "baby_puft_kanim", true);
    EntityTemplates.ExtendEntityToBeingABaby(puftAlpha, Tag.op_Implicit("PuftAlpha"));
    return puftAlpha;
  }

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst) => BasePuftConfig.OnSpawn(inst);
}
