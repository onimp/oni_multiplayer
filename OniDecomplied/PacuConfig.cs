// Decompiled with JetBrains decompiler
// Type: PacuConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

[EntityConfigOrder(1)]
public class PacuConfig : IEntityConfig
{
  public const string ID = "Pacu";
  public const string BASE_TRAIT_ID = "PacuBaseTrait";
  public const string EGG_ID = "PacuEgg";
  public const int EGG_SORT_ORDER = 500;

  public static GameObject CreatePacu(
    string id,
    string name,
    string desc,
    string anim_file,
    bool is_baby)
  {
    return EntityTemplates.ExtendEntityToWildCreature(BasePacuConfig.CreatePrefab(id, "PacuBaseTrait", name, desc, anim_file, is_baby, (string) null, 273.15f, 333.15f), PacuTuning.PEN_SIZE_PER_CREATURE);
  }

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject fertileCreature = EntityTemplates.ExtendEntityToFertileCreature(PacuConfig.CreatePacu("Pacu", (string) CREATURES.SPECIES.PACU.NAME, (string) CREATURES.SPECIES.PACU.DESC, "pacu_kanim", false), "PacuEgg", (string) CREATURES.SPECIES.PACU.EGG_NAME, (string) CREATURES.SPECIES.PACU.DESC, "egg_pacu_kanim", PacuTuning.EGG_MASS, "PacuBaby", 15.000001f, 5f, PacuTuning.EGG_CHANCES_BASE, 500, false, true, false, 0.75f);
    fertileCreature.AddTag(GameTags.OriginalCreature);
    return fertileCreature;
  }

  public void OnPrefabInit(GameObject prefab) => prefab.AddOrGet<LoopingSounds>();

  public void OnSpawn(GameObject inst)
  {
  }
}
