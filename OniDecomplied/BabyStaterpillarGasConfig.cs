// Decompiled with JetBrains decompiler
// Type: BabyStaterpillarGasConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class BabyStaterpillarGasConfig : IEntityConfig
{
  public const string ID = "StaterpillarGasBaby";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public GameObject CreatePrefab()
  {
    GameObject staterpillarGas = StaterpillarGasConfig.CreateStaterpillarGas("StaterpillarGasBaby", (string) CREATURES.SPECIES.STATERPILLAR.VARIANT_GAS.BABY.NAME, (string) CREATURES.SPECIES.STATERPILLAR.VARIANT_GAS.BABY.DESC, "baby_caterpillar_kanim", true);
    EntityTemplates.ExtendEntityToBeingABaby(staterpillarGas, Tag.op_Implicit("StaterpillarGas"));
    return staterpillarGas;
  }

  public void OnPrefabInit(GameObject prefab) => prefab.GetComponent<KBatchedAnimController>().SetSymbolVisiblity(KAnimHashedString.op_Implicit("electric_bolt_c_bloom"), false);

  public void OnSpawn(GameObject inst)
  {
  }
}
