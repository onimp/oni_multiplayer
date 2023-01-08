// Decompiled with JetBrains decompiler
// Type: SuperWormPlantConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class SuperWormPlantConfig : IEntityConfig
{
  public const string ID = "SuperWormPlant";
  public static readonly EffectorValues SUPER_DECOR = TUNING.DECOR.BONUS.TIER1;
  public const string SUPER_CROP_ID = "WormSuperFruit";
  public const int CROP_YIELD = 8;
  private static StandardCropPlant.AnimSet animSet = new StandardCropPlant.AnimSet()
  {
    grow = "super_grow",
    grow_pst = "super_grow_pst",
    idle_full = "super_idle_full",
    wilt_base = "super_wilt",
    harvest = "super_harvest"
  };

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public GameObject CreatePrefab()
  {
    GameObject go = WormPlantConfig.BaseWormPlant("SuperWormPlant", (string) STRINGS.CREATURES.SPECIES.SUPERWORMPLANT.NAME, (string) STRINGS.CREATURES.SPECIES.SUPERWORMPLANT.DESC, "wormwood_kanim", SuperWormPlantConfig.SUPER_DECOR, "WormSuperFruit");
    go.AddOrGet<SeedProducer>().Configure("WormPlantSeed", SeedProducer.ProductionType.Harvest);
    return go;
  }

  public void OnPrefabInit(GameObject prefab)
  {
    TransformingPlant transformingPlant = prefab.AddOrGet<TransformingPlant>();
    transformingPlant.SubscribeToTransformEvent(GameHashes.HarvestComplete);
    transformingPlant.transformPlantId = "WormPlant";
    prefab.GetComponent<KAnimControllerBase>().SetSymbolVisiblity(KAnimHashedString.op_Implicit("flower"), false);
    prefab.AddOrGet<StandardCropPlant>().anims = SuperWormPlantConfig.animSet;
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
