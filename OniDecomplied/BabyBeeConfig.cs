// Decompiled with JetBrains decompiler
// Type: BabyBeeConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class BabyBeeConfig : IEntityConfig
{
  public const string ID = "BeeBaby";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public GameObject CreatePrefab()
  {
    GameObject bee = BeeConfig.CreateBee("BeeBaby", (string) CREATURES.SPECIES.BEE.BABY.NAME, (string) CREATURES.SPECIES.BEE.BABY.DESC, "baby_blarva_kanim", true);
    EntityTemplates.ExtendEntityToBeingABaby(bee, Tag.op_Implicit("Bee"), force_adult_nav_type: true, adult_threshold: 2f);
    bee.GetComponent<KPrefabID>().AddTag(GameTags.Creatures.Walker, false);
    return bee;
  }

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst) => BaseBeeConfig.SetupLoopingSounds(inst);
}
