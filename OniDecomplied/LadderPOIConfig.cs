// Decompiled with JetBrains decompiler
// Type: LadderPOIConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class LadderPOIConfig : IEntityConfig
{
  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    int width1 = 1;
    int height1 = 1;
    string name = (string) STRINGS.BUILDINGS.PREFABS.PROPLADDER.NAME;
    string desc = (string) STRINGS.BUILDINGS.PREFABS.PROPLADDER.DESC;
    int num1 = width1;
    int num2 = height1;
    EffectorValues tieR0_1 = TUNING.BUILDINGS.DECOR.BONUS.TIER0;
    EffectorValues tieR0_2 = NOISE_POLLUTION.NOISY.TIER0;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit("ladder_poi_kanim"));
    int width2 = num1;
    int height2 = num2;
    EffectorValues decor = tieR0_1;
    EffectorValues noise = tieR0_2;
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.Gravitas);
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity("PropLadder", name, desc, 50f, anim, "off", Grid.SceneLayer.Building, width2, height2, decor, noise, additionalTags: additionalTags);
    PrimaryElement component = placedEntity.GetComponent<PrimaryElement>();
    component.SetElement(SimHashes.Polypropylene);
    component.Temperature = 294.15f;
    Ladder ladder = placedEntity.AddOrGet<Ladder>();
    ladder.upwardsMovementSpeedMultiplier = 1.5f;
    ladder.downwardsMovementSpeedMultiplier = 1.5f;
    placedEntity.AddOrGet<AnimTileable>();
    Object.DestroyImmediate((Object) placedEntity.AddOrGet<OccupyArea>());
    OccupyArea occupyArea = placedEntity.AddOrGet<OccupyArea>();
    occupyArea.OccupiedCellsOffsets = EntityTemplates.GenerateOffsets(width1, height1);
    occupyArea.objectLayers = new ObjectLayer[1]
    {
      ObjectLayer.Building
    };
    placedEntity.AddOrGet<Demolishable>();
    return placedEntity;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
