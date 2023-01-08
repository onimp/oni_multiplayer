// Decompiled with JetBrains decompiler
// Type: SleepClinicPajamas
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SleepClinicPajamas : IEquipmentConfig
{
  public const string ID = "SleepClinicPajamas";
  public const string EFFECT_ID = "SleepClinic";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public EquipmentDef CreateEquipmentDef()
  {
    ClothingWearer.ClothingInfo clothingInfo = ClothingWearer.ClothingInfo.FANCY_CLOTHING;
    List<AttributeModifier> AttributeModifiers = new List<AttributeModifier>();
    EquipmentDef equipmentDef = EquipmentTemplates.CreateEquipmentDef(nameof (SleepClinicPajamas), TUNING.EQUIPMENT.CLOTHING.SLOT, SimHashes.Carbon, (float) TUNING.EQUIPMENT.VESTS.FUNKY_VEST_MASS, "pajamas_kanim", TUNING.EQUIPMENT.VESTS.SNAPON0, "body_pajamas_kanim", 4, AttributeModifiers, TUNING.EQUIPMENT.VESTS.SNAPON1, true, EntityTemplates.CollisionShape.RECTANGLE, 0.75f, 0.4f);
    equipmentDef.RecipeDescription = (string) STRINGS.EQUIPMENT.PREFABS.SLEEPCLINICPAJAMAS.DESC + "\n\n" + (string) STRINGS.EQUIPMENT.PREFABS.SLEEPCLINICPAJAMAS.EFFECT;
    Descriptor descriptor1;
    // ISSUE: explicit constructor call
    ((Descriptor) ref descriptor1).\u002Ector(string.Format("{0}: {1}", (object) DUPLICANTS.ATTRIBUTES.THERMALCONDUCTIVITYBARRIER.NAME, (object) GameUtil.GetFormattedDistance(ClothingWearer.ClothingInfo.FANCY_CLOTHING.conductivityMod)), string.Format("{0}: {1}", (object) DUPLICANTS.ATTRIBUTES.THERMALCONDUCTIVITYBARRIER.NAME, (object) GameUtil.GetFormattedDistance(ClothingWearer.ClothingInfo.FANCY_CLOTHING.conductivityMod)), (Descriptor.DescriptorType) 1, false);
    Descriptor descriptor2;
    // ISSUE: explicit constructor call
    ((Descriptor) ref descriptor2).\u002Ector(string.Format("{0}: {1}", (object) DUPLICANTS.ATTRIBUTES.DECOR.NAME, (object) ClothingWearer.ClothingInfo.FANCY_CLOTHING.decorMod), string.Format("{0}: {1}", (object) DUPLICANTS.ATTRIBUTES.DECOR.NAME, (object) ClothingWearer.ClothingInfo.FANCY_CLOTHING.decorMod), (Descriptor.DescriptorType) 1, false);
    equipmentDef.additionalDescriptors.Add(descriptor1);
    equipmentDef.additionalDescriptors.Add(descriptor2);
    Effect.AddModifierDescriptions((GameObject) null, equipmentDef.additionalDescriptors, "SleepClinic");
    equipmentDef.OnEquipCallBack = (Action<Equippable>) (eq => CoolVestConfig.OnEquipVest(eq, clothingInfo));
    equipmentDef.OnUnequipCallBack = (Action<Equippable>) (eq =>
    {
      CoolVestConfig.OnUnequipVest(eq);
      PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, (string) STRINGS.EQUIPMENT.PREFABS.SLEEPCLINICPAJAMAS.DESTROY_TOAST, eq.transform);
      TracesExtesions.DeleteObject(((Component) eq).gameObject);
    });
    return equipmentDef;
  }

  public void DoPostConfigure(GameObject go)
  {
    KPrefabID component = go.GetComponent<KPrefabID>();
    component.AddTag(GameTags.Clothes, false);
    component.AddTag(GameTags.PedestalDisplayable, false);
    go.AddOrGet<ClinicDreamable>().workTime = 300f;
    go.AddOrGet<Equippable>().SetQuality(QualityLevel.Poor);
    go.GetComponent<KBatchedAnimController>().sceneLayer = Grid.SceneLayer.BuildingFront;
  }
}
