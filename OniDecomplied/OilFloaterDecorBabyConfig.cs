// Decompiled with JetBrains decompiler
// Type: OilFloaterDecorBabyConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class OilFloaterDecorBabyConfig : IEntityConfig
{
  public const string ID = "OilfloaterDecorBaby";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject oilFloater = OilFloaterDecorConfig.CreateOilFloater("OilfloaterDecorBaby", (string) CREATURES.SPECIES.OILFLOATER.VARIANT_DECOR.BABY.NAME, (string) CREATURES.SPECIES.OILFLOATER.VARIANT_DECOR.BABY.DESC, "baby_oilfloater_kanim", true);
    EntityTemplates.ExtendEntityToBeingABaby(oilFloater, Tag.op_Implicit("OilfloaterDecor"));
    return oilFloater;
  }

  public void OnPrefabInit(GameObject prefab)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
