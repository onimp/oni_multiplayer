// Decompiled with JetBrains decompiler
// Type: ChlorineGeyserConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;
using UnityEngine;

public class ChlorineGeyserConfig : IEntityConfig
{
  public const string ID = "ChlorineGeyser";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    string name = (string) STRINGS.CREATURES.SPECIES.CHLORINEGEYSER.NAME;
    string desc = (string) STRINGS.CREATURES.SPECIES.CHLORINEGEYSER.DESC;
    EffectorValues tieR1 = TUNING.BUILDINGS.DECOR.BONUS.TIER1;
    EffectorValues tieR5 = NOISE_POLLUTION.NOISY.TIER5;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit("geyser_side_chlorine_kanim"));
    EffectorValues decor = tieR1;
    EffectorValues noise = tieR5;
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity("ChlorineGeyser", name, desc, 2000f, anim, "inactive", Grid.SceneLayer.BuildingBack, 4, 2, decor, noise);
    placedEntity.GetComponent<KPrefabID>().AddTag(GameTags.DeprecatedContent, false);
    PrimaryElement component = placedEntity.GetComponent<PrimaryElement>();
    component.SetElement(SimHashes.IgneousRock);
    component.Temperature = 372.15f;
    placedEntity.AddOrGet<Geyser>().outputOffset = new Vector2I(0, 1);
    GeyserConfigurator geyserConfigurator = placedEntity.AddOrGet<GeyserConfigurator>();
    geyserConfigurator.presetType = HashedString.op_Implicit("chlorine_gas");
    geyserConfigurator.presetMin = 0.35f;
    geyserConfigurator.presetMax = 0.65f;
    Studyable studyable = placedEntity.AddOrGet<Studyable>();
    studyable.meterTrackerSymbol = "geotracker_target";
    studyable.meterAnim = "tracker";
    placedEntity.AddOrGet<LoopingSounds>();
    SoundEventVolumeCache.instance.AddVolume("geyser_methane_kanim", "GeyserMethane_shake_LP", NOISE_POLLUTION.NOISY.TIER5);
    SoundEventVolumeCache.instance.AddVolume("geyser_methane_kanim", "GeyserMethane_shake_LP", NOISE_POLLUTION.NOISY.TIER6);
    return placedEntity;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
