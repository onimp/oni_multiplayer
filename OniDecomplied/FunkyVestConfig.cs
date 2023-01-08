// Decompiled with JetBrains decompiler
// Type: FunkyVestConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FunkyVestConfig : IEquipmentConfig
{
  public const string ID = "Funky_Vest";
  public static ComplexRecipe recipe;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public EquipmentDef CreateEquipmentDef()
  {
    new Dictionary<string, float>()
    {
      {
        "BasicFabric",
        (float) TUNING.EQUIPMENT.VESTS.FUNKY_VEST_MASS
      }
    };
    ClothingWearer.ClothingInfo clothingInfo = ClothingWearer.ClothingInfo.FANCY_CLOTHING;
    List<AttributeModifier> AttributeModifiers = new List<AttributeModifier>();
    EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef("Funky_Vest", TUNING.EQUIPMENT.CLOTHING.SLOT, SimHashes.Carbon, (float) TUNING.EQUIPMENT.VESTS.FUNKY_VEST_MASS, "shirt_decor01_kanim", TUNING.EQUIPMENT.VESTS.SNAPON0, "body_shirt_decor01_kanim", 4, AttributeModifiers, TUNING.EQUIPMENT.VESTS.SNAPON1, true, EntityTemplates.CollisionShape.RECTANGLE, 0.75f, 0.4f);
    Descriptor descriptor1;
    // ISSUE: explicit constructor call
    ((Descriptor) ref descriptor1).\u002Ector(string.Format("{0}: {1}", (object) DUPLICANTS.ATTRIBUTES.THERMALCONDUCTIVITYBARRIER.NAME, (object) GameUtil.GetFormattedDistance(ClothingWearer.ClothingInfo.FANCY_CLOTHING.conductivityMod)), string.Format("{0}: {1}", (object) DUPLICANTS.ATTRIBUTES.THERMALCONDUCTIVITYBARRIER.NAME, (object) GameUtil.GetFormattedDistance(ClothingWearer.ClothingInfo.FANCY_CLOTHING.conductivityMod)), (Descriptor.DescriptorType) 1, false);
    Descriptor descriptor2;
    // ISSUE: explicit constructor call
    ((Descriptor) ref descriptor2).\u002Ector(string.Format("{0}: {1}", (object) DUPLICANTS.ATTRIBUTES.DECOR.NAME, (object) ClothingWearer.ClothingInfo.FANCY_CLOTHING.decorMod), string.Format("{0}: {1}", (object) DUPLICANTS.ATTRIBUTES.DECOR.NAME, (object) ClothingWearer.ClothingInfo.FANCY_CLOTHING.decorMod), (Descriptor.DescriptorType) 1, false);
    equipmentDef.additionalDescriptors.Add(descriptor1);
    equipmentDef.additionalDescriptors.Add(descriptor2);
    equipmentDef.OnEquipCallBack = (Action<Equippable>) (eq => CoolVestConfig.OnEquipVest(eq, clothingInfo));
    equipmentDef.OnUnequipCallBack = new Action<Equippable>(CoolVestConfig.OnUnequipVest);
    equipmentDef.RecipeDescription = (string) STRINGS.EQUIPMENT.PREFABS.FUNKY_VEST.RECIPE_DESC;
    return equipmentDef;
  }

  public static void SetupVest(GameObject go)
  {
    go.GetComponent<KPrefabID>().AddTag(GameTags.Clothes, false);
    Equippable equippable = go.GetComponent<Equippable>();
    if (Object.op_Equality((Object) equippable, (Object) null))
      equippable = go.AddComponent<Equippable>();
    equippable.SetQuality(QualityLevel.Poor);
    go.GetComponent<KBatchedAnimController>().sceneLayer = Grid.SceneLayer.BuildingBack;
  }

  public void DoPostConfigure(GameObject go)
  {
    FunkyVestConfig.SetupVest(go);
    go.GetComponent<KPrefabID>().AddTag(GameTags.PedestalDisplayable, false);
  }
}
