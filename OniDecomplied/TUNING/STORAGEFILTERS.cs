// Decompiled with JetBrains decompiler
// Type: TUNING.STORAGEFILTERS
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

namespace TUNING
{
  public class STORAGEFILTERS
  {
    public static List<Tag> FOOD;
    public static List<Tag> BAGABLE_CREATURES;
    public static List<Tag> SWIMMING_CREATURES;
    public static List<Tag> NOT_EDIBLE_SOLIDS;
    public static List<Tag> LIQUIDS;
    public static List<Tag> GASES;
    public static List<Tag> PAYLOADS;

    static STORAGEFILTERS()
    {
      List<Tag> tagList1 = new List<Tag>();
      tagList1.Add(GameTags.Edible);
      tagList1.Add(GameTags.CookingIngredient);
      tagList1.Add(GameTags.Medicine);
      STORAGEFILTERS.FOOD = tagList1;
      List<Tag> tagList2 = new List<Tag>();
      tagList2.Add(GameTags.BagableCreature);
      STORAGEFILTERS.BAGABLE_CREATURES = tagList2;
      List<Tag> tagList3 = new List<Tag>();
      tagList3.Add(GameTags.SwimmingCreature);
      STORAGEFILTERS.SWIMMING_CREATURES = tagList3;
      List<Tag> tagList4 = new List<Tag>();
      tagList4.Add(GameTags.Alloy);
      tagList4.Add(GameTags.RefinedMetal);
      tagList4.Add(GameTags.Metal);
      tagList4.Add(GameTags.BuildableRaw);
      tagList4.Add(GameTags.BuildableProcessed);
      tagList4.Add(GameTags.Farmable);
      tagList4.Add(GameTags.Organics);
      tagList4.Add(GameTags.Compostable);
      tagList4.Add(GameTags.Seed);
      tagList4.Add(GameTags.Agriculture);
      tagList4.Add(GameTags.Filter);
      tagList4.Add(GameTags.ConsumableOre);
      tagList4.Add(GameTags.Liquifiable);
      tagList4.Add(GameTags.IndustrialProduct);
      tagList4.Add(GameTags.IndustrialIngredient);
      tagList4.Add(GameTags.MedicalSupplies);
      tagList4.Add(GameTags.Clothes);
      tagList4.Add(GameTags.ManufacturedMaterial);
      tagList4.Add(GameTags.Egg);
      tagList4.Add(GameTags.RareMaterials);
      tagList4.Add(GameTags.Other);
      STORAGEFILTERS.NOT_EDIBLE_SOLIDS = tagList4;
      List<Tag> tagList5 = new List<Tag>();
      tagList5.Add(GameTags.Liquid);
      STORAGEFILTERS.LIQUIDS = tagList5;
      List<Tag> tagList6 = new List<Tag>();
      tagList6.Add(GameTags.Breathable);
      tagList6.Add(GameTags.Unbreathable);
      STORAGEFILTERS.GASES = tagList6;
      List<Tag> tagList7 = new List<Tag>();
      tagList7.Add(GameTags.RailGunPayloadEmptyable);
      STORAGEFILTERS.PAYLOADS = tagList7;
    }
  }
}
