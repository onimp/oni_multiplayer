// Decompiled with JetBrains decompiler
// Type: BabyStaterpillarConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class BabyStaterpillarConfig : IEntityConfig
{
  public const string ID = "StaterpillarBaby";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public GameObject CreatePrefab()
  {
    GameObject staterpillar = StaterpillarConfig.CreateStaterpillar("StaterpillarBaby", (string) CREATURES.SPECIES.STATERPILLAR.BABY.NAME, (string) CREATURES.SPECIES.STATERPILLAR.BABY.DESC, "baby_caterpillar_kanim", true);
    EntityTemplates.ExtendEntityToBeingABaby(staterpillar, Tag.op_Implicit("Staterpillar"));
    return staterpillar;
  }

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
