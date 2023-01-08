// Decompiled with JetBrains decompiler
// Type: ScoutLanderConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class ScoutLanderConfig : IEntityConfig
{
  public const string ID = "ScoutLander";
  public const string PREVIEW_ID = "ScoutLander_Preview";
  public const float MASS = 400f;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public GameObject CreatePrefab()
  {
    string name = (string) STRINGS.BUILDINGS.PREFABS.SCOUTLANDER.NAME;
    string desc = (string) STRINGS.BUILDINGS.PREFABS.SCOUTLANDER.DESC;
    EffectorValues tieR0_1 = TUNING.BUILDINGS.DECOR.BONUS.TIER0;
    EffectorValues tieR0_2 = NOISE_POLLUTION.NOISY.TIER0;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit("rocket_scout_cargo_lander_kanim"));
    EffectorValues decor = tieR0_1;
    EffectorValues noise = tieR0_2;
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity("ScoutLander", name, desc, 400f, anim, "grounded", Grid.SceneLayer.Building, 3, 3, decor, noise);
    placedEntity.AddOrGetDef<CargoLander.Def>().previewTag = TagExtensions.ToTag("ScoutLander_Preview");
    placedEntity.AddOrGetDef<CargoDropperStorage.Def>();
    placedEntity.AddOrGet<Prioritizable>();
    Prioritizable.AddRef(placedEntity);
    placedEntity.AddOrGet<Operational>();
    Storage storage = placedEntity.AddComponent<Storage>();
    storage.showInUI = true;
    storage.allowItemRemoval = false;
    storage.capacityKg = 2000f;
    storage.SetDefaultStoredItemModifiers(Storage.StandardInsulatedStorage);
    placedEntity.AddOrGet<Deconstructable>().audioSize = "large";
    placedEntity.AddOrGet<Storable>();
    Placeable placeable = placedEntity.AddOrGet<Placeable>();
    placeable.kAnimName = "rocket_scout_cargo_lander_kanim";
    placeable.animName = "place";
    placeable.placementRules = new List<Placeable.PlacementRules>()
    {
      Placeable.PlacementRules.OnFoundation,
      Placeable.PlacementRules.VisibleToSpace,
      Placeable.PlacementRules.RestrictToWorld
    };
    EntityTemplates.CreateAndRegisterPreview("ScoutLander_Preview", Assets.GetAnim(HashedString.op_Implicit("rocket_scout_cargo_lander_kanim")), "place", ObjectLayer.Building, 3, 3);
    return placedEntity;
  }

  public void OnPrefabInit(GameObject inst)
  {
    OccupyArea component = inst.GetComponent<OccupyArea>();
    component.ApplyToCells = false;
    component.objectLayers = new ObjectLayer[1]
    {
      ObjectLayer.Building
    };
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
