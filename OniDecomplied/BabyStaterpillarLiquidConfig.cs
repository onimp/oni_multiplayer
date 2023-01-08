// Decompiled with JetBrains decompiler
// Type: BabyStaterpillarLiquidConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class BabyStaterpillarLiquidConfig : IEntityConfig
{
  public const string ID = "StaterpillarLiquidBaby";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public GameObject CreatePrefab()
  {
    GameObject staterpillarLiquid = StaterpillarLiquidConfig.CreateStaterpillarLiquid("StaterpillarLiquidBaby", (string) CREATURES.SPECIES.STATERPILLAR.VARIANT_LIQUID.BABY.NAME, (string) CREATURES.SPECIES.STATERPILLAR.VARIANT_LIQUID.BABY.DESC, "baby_caterpillar_kanim", true);
    EntityTemplates.ExtendEntityToBeingABaby(staterpillarLiquid, Tag.op_Implicit("StaterpillarLiquid"));
    return staterpillarLiquid;
  }

  public void OnPrefabInit(GameObject prefab) => prefab.GetComponent<KBatchedAnimController>().SetSymbolVisiblity(KAnimHashedString.op_Implicit("electric_bolt_c_bloom"), false);

  public void OnSpawn(GameObject inst)
  {
  }
}
