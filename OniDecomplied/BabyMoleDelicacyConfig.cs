// Decompiled with JetBrains decompiler
// Type: BabyMoleDelicacyConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class BabyMoleDelicacyConfig : IEntityConfig
{
  public const string ID = "MoleDelicacyBaby";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject mole = MoleDelicacyConfig.CreateMole("MoleDelicacyBaby", (string) CREATURES.SPECIES.MOLE.VARIANT_DELICACY.BABY.NAME, (string) CREATURES.SPECIES.MOLE.VARIANT_DELICACY.BABY.DESC, "baby_driller_kanim", true);
    EntityTemplates.ExtendEntityToBeingABaby(mole, Tag.op_Implicit("MoleDelicacy"));
    return mole;
  }

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst) => MoleConfig.SetSpawnNavType(inst);
}
