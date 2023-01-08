// Decompiled with JetBrains decompiler
// Type: EffectConfigs
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class EffectConfigs : IMultiEntityConfig
{
  public static string EffectTemplateId = "EffectTemplateFx";
  public static string AttackSplashId = "AttackSplashFx";
  public static string OreAbsorbId = "OreAbsorbFx";
  public static string PlantDeathId = "PlantDeathFx";
  public static string BuildSplashId = "BuildSplashFx";
  public static string DemolishSplashId = "DemolishSplashFx";

  public List<GameObject> CreatePrefabs()
  {
    List<GameObject> prefabs = new List<GameObject>();
    \u003C\u003Ef__AnonymousType0<string, string[], string, KAnim.PlayMode, bool>[] dataArray = new \u003C\u003Ef__AnonymousType0<string, string[], string, KAnim.PlayMode, bool>[6]
    {
      new
      {
        id = EffectConfigs.EffectTemplateId,
        animFiles = new string[0],
        initialAnim = "",
        initialMode = (KAnim.PlayMode) 1,
        destroyOnAnimComplete = false
      },
      new
      {
        id = EffectConfigs.AttackSplashId,
        animFiles = new string[1]
        {
          "attack_beam_contact_fx_kanim"
        },
        initialAnim = "loop",
        initialMode = (KAnim.PlayMode) 0,
        destroyOnAnimComplete = false
      },
      new
      {
        id = EffectConfigs.OreAbsorbId,
        animFiles = new string[1]{ "ore_collision_kanim" },
        initialAnim = "idle",
        initialMode = (KAnim.PlayMode) 1,
        destroyOnAnimComplete = true
      },
      new
      {
        id = EffectConfigs.PlantDeathId,
        animFiles = new string[1]{ "plant_death_fx_kanim" },
        initialAnim = "plant_death",
        initialMode = (KAnim.PlayMode) 1,
        destroyOnAnimComplete = true
      },
      new
      {
        id = EffectConfigs.BuildSplashId,
        animFiles = new string[1]
        {
          "sparks_radial_build_kanim"
        },
        initialAnim = "loop",
        initialMode = (KAnim.PlayMode) 0,
        destroyOnAnimComplete = false
      },
      new
      {
        id = EffectConfigs.DemolishSplashId,
        animFiles = new string[1]
        {
          "poi_demolish_impact_kanim"
        },
        initialAnim = "POI_demolish_impact",
        initialMode = (KAnim.PlayMode) 0,
        destroyOnAnimComplete = false
      }
    };
    foreach (var data in dataArray)
    {
      GameObject entity = EntityTemplates.CreateEntity(data.id, data.id, false);
      KBatchedAnimController kbatchedAnimController = entity.AddOrGet<KBatchedAnimController>();
      kbatchedAnimController.materialType = (KAnimBatchGroup.MaterialType) 1;
      kbatchedAnimController.initialAnim = data.initialAnim;
      kbatchedAnimController.initialMode = data.initialMode;
      kbatchedAnimController.isMovable = true;
      kbatchedAnimController.destroyOnAnimComplete = data.destroyOnAnimComplete;
      if (data.animFiles.Length != 0)
      {
        KAnimFile[] kanimFileArray = new KAnimFile[data.animFiles.Length];
        for (int index = 0; index < kanimFileArray.Length; ++index)
          kanimFileArray[index] = Assets.GetAnim(HashedString.op_Implicit(data.animFiles[index]));
        kbatchedAnimController.AnimFiles = kanimFileArray;
      }
      entity.AddOrGet<LoopingSounds>();
      prefabs.Add(entity);
    }
    return prefabs;
  }

  public void OnPrefabInit(GameObject go)
  {
  }

  public void OnSpawn(GameObject go)
  {
  }
}
