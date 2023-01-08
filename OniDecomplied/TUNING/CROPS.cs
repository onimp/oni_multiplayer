// Decompiled with JetBrains decompiler
// Type: TUNING.CROPS
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

namespace TUNING
{
  public class CROPS
  {
    public const float WILD_GROWTH_RATE_MODIFIER = 0.25f;
    public const float GROWTH_RATE = 0.00166666671f;
    public const float WILD_GROWTH_RATE = 0.000416666677f;
    public const float PLANTERPLOT_GROWTH_PENTALY = -0.5f;
    public const float BASE_BONUS_SEED_PROBABILITY = 0.1f;
    public const float SELF_HARVEST_TIME = 2400f;
    public const float SELF_PLANT_TIME = 2400f;
    public const float TREE_BRANCH_SELF_HARVEST_TIME = 12000f;
    public const float FERTILIZATION_GAIN_RATE = 1.66666663f;
    public const float FERTILIZATION_LOSS_RATE = -0.166666672f;
    public static List<Crop.CropVal> CROP_TYPES = new List<Crop.CropVal>()
    {
      new Crop.CropVal("BasicPlantFood", 1800f),
      new Crop.CropVal(PrickleFruitConfig.ID, 3600f),
      new Crop.CropVal(SwampFruitConfig.ID, 3960f),
      new Crop.CropVal(MushroomConfig.ID, 4500f),
      new Crop.CropVal("ColdWheatSeed", 10800f, 18),
      new Crop.CropVal(SpiceNutConfig.ID, 4800f, 4),
      new Crop.CropVal(BasicFabricConfig.ID, 1200f),
      new Crop.CropVal(SwampLilyFlowerConfig.ID, 7200f, 2),
      new Crop.CropVal("GasGrassHarvested", 2400f),
      new Crop.CropVal("WoodLog", 2700f, 300),
      new Crop.CropVal("Lettuce", 7200f, 12),
      new Crop.CropVal("BeanPlantSeed", 12600f, 12),
      new Crop.CropVal("OxyfernSeed", 7200f),
      new Crop.CropVal("PlantMeat", 18000f, 10),
      new Crop.CropVal("WormBasicFruit", 2400f),
      new Crop.CropVal("WormSuperFruit", 4800f, 8),
      new Crop.CropVal(SimHashes.Salt.ToString(), 3600f, 65),
      new Crop.CropVal(SimHashes.Water.ToString(), 6000f, 350)
    };
  }
}
