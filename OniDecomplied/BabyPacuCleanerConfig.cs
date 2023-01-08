// Decompiled with JetBrains decompiler
// Type: BabyPacuCleanerConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

[EntityConfigOrder(1)]
public class BabyPacuCleanerConfig : IEntityConfig
{
  public const string ID = "PacuCleanerBaby";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject pacu = PacuCleanerConfig.CreatePacu("PacuCleanerBaby", (string) CREATURES.SPECIES.PACU.VARIANT_CLEANER.BABY.NAME, (string) CREATURES.SPECIES.PACU.VARIANT_CLEANER.BABY.DESC, "baby_pacu_kanim", true);
    EntityTemplates.ExtendEntityToBeingABaby(pacu, Tag.op_Implicit("PacuCleaner"));
    return pacu;
  }

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
