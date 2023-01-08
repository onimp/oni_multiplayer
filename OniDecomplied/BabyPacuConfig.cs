// Decompiled with JetBrains decompiler
// Type: BabyPacuConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

[EntityConfigOrder(1)]
public class BabyPacuConfig : IEntityConfig
{
  public const string ID = "PacuBaby";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject pacu = PacuConfig.CreatePacu("PacuBaby", (string) CREATURES.SPECIES.PACU.BABY.NAME, (string) CREATURES.SPECIES.PACU.BABY.DESC, "baby_pacu_kanim", true);
    EntityTemplates.ExtendEntityToBeingABaby(pacu, Tag.op_Implicit("Pacu"));
    return pacu;
  }

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
