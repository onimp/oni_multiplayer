// Decompiled with JetBrains decompiler
// Type: GeneShufflerConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class GeneShufflerConfig : IEntityConfig
{
  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    string name = (string) STRINGS.BUILDINGS.PREFABS.GENESHUFFLER.NAME;
    string desc = (string) STRINGS.BUILDINGS.PREFABS.GENESHUFFLER.DESC;
    EffectorValues tieR0_1 = TUNING.BUILDINGS.DECOR.BONUS.TIER0;
    EffectorValues tieR0_2 = NOISE_POLLUTION.NOISY.TIER0;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit("geneshuffler_kanim"));
    EffectorValues decor = tieR0_1;
    EffectorValues noise = tieR0_2;
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.Gravitas);
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity("GeneShuffler", name, desc, 2000f, anim, "on", Grid.SceneLayer.Building, 4, 3, decor, noise, additionalTags: additionalTags);
    placedEntity.AddTag(GameTags.NotRoomAssignable);
    PrimaryElement component = placedEntity.GetComponent<PrimaryElement>();
    component.SetElement(SimHashes.Unobtanium);
    component.Temperature = 294.15f;
    placedEntity.AddOrGet<Operational>();
    placedEntity.AddOrGet<Notifier>();
    placedEntity.AddOrGet<GeneShuffler>();
    LoreBearerUtil.AddLoreTo(placedEntity, new LoreBearerAction(LoreBearerUtil.NerualVacillator));
    placedEntity.AddOrGet<LoopingSounds>();
    placedEntity.AddOrGet<Ownable>();
    placedEntity.AddOrGet<Prioritizable>();
    placedEntity.AddOrGet<Demolishable>();
    Storage storage = placedEntity.AddOrGet<Storage>();
    storage.dropOnLoad = true;
    ManualDeliveryKG manualDeliveryKg = placedEntity.AddOrGet<ManualDeliveryKG>();
    manualDeliveryKg.SetStorage(storage);
    manualDeliveryKg.choreTypeIDHash = Db.Get().ChoreTypes.MachineFetch.IdHash;
    manualDeliveryKg.RequestedItemTag = new Tag("GeneShufflerRecharge");
    manualDeliveryKg.refillMass = 1f;
    manualDeliveryKg.MinimumMass = 1f;
    manualDeliveryKg.capacity = 1f;
    KBatchedAnimController kbatchedAnimController = placedEntity.AddOrGet<KBatchedAnimController>();
    kbatchedAnimController.sceneLayer = Grid.SceneLayer.BuildingBack;
    kbatchedAnimController.fgLayer = Grid.SceneLayer.BuildingFront;
    return placedEntity;
  }

  public void OnPrefabInit(GameObject inst)
  {
    inst.GetComponent<GeneShuffler>().workLayer = Grid.SceneLayer.Building;
    inst.GetComponent<Ownable>().slotID = Db.Get().AssignableSlots.GeneShuffler.Id;
    inst.GetComponent<OccupyArea>().objectLayers = new ObjectLayer[1]
    {
      ObjectLayer.Building
    };
    inst.GetComponent<Deconstructable>();
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
