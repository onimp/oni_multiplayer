// Decompiled with JetBrains decompiler
// Type: PropSurfaceSatellite2Config
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class PropSurfaceSatellite2Config : IEntityConfig
{
  public static string ID = "PropSurfaceSatellite2";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    string id = PropSurfaceSatellite2Config.ID;
    string name = (string) STRINGS.BUILDINGS.PREFABS.PROPSURFACESATELLITE2.NAME;
    string desc = (string) STRINGS.BUILDINGS.PREFABS.PROPSURFACESATELLITE2.DESC;
    EffectorValues tieR0_1 = TUNING.BUILDINGS.DECOR.BONUS.TIER0;
    EffectorValues tieR0_2 = NOISE_POLLUTION.NOISY.TIER0;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit("satellite2_kanim"));
    EffectorValues decor = tieR0_1;
    EffectorValues noise = tieR0_2;
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.Gravitas);
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity(id, name, desc, 50f, anim, "off", Grid.SceneLayer.Building, 4, 4, decor, noise, additionalTags: additionalTags);
    PrimaryElement component = placedEntity.GetComponent<PrimaryElement>();
    component.SetElement(SimHashes.Unobtanium);
    component.Temperature = 294.15f;
    Workable workable = placedEntity.AddOrGet<Workable>();
    workable.synchronizeAnims = false;
    workable.resetProgressOnStop = true;
    SetLocker setLocker = placedEntity.AddOrGet<SetLocker>();
    setLocker.overrideAnim = "anim_interacts_clothingfactory_kanim";
    setLocker.dropOffset = new Vector2I(0, 1);
    setLocker.numDataBanks = new int[2]{ 4, 9 };
    LoreBearerUtil.AddLoreTo(placedEntity);
    placedEntity.AddOrGet<Demolishable>();
    return placedEntity;
  }

  public void OnPrefabInit(GameObject inst)
  {
    SetLocker component1 = inst.GetComponent<SetLocker>();
    component1.possible_contents_ids = PropSurfaceSatellite1Config.GetLockerBaseContents();
    component1.ChooseContents();
    OccupyArea component2 = inst.GetComponent<OccupyArea>();
    component2.objectLayers = new ObjectLayer[1]
    {
      ObjectLayer.Building
    };
    int cell = Grid.PosToCell(inst);
    foreach (CellOffset occupiedCellsOffset in component2.OccupiedCellsOffsets)
      Grid.GravitasFacility[Grid.OffsetCell(cell, occupiedCellsOffset)] = true;
    RadiationEmitter radiationEmitter = inst.AddOrGet<RadiationEmitter>();
    radiationEmitter.emitType = RadiationEmitter.RadiationEmitterType.Constant;
    radiationEmitter.radiusProportionalToRads = false;
    radiationEmitter.emitRadiusX = (short) 12;
    radiationEmitter.emitRadiusY = (short) 12;
    radiationEmitter.emitRads = (float) (2400.0 / ((double) radiationEmitter.emitRadiusX / 6.0));
  }

  public void OnSpawn(GameObject inst)
  {
    RadiationEmitter component = inst.GetComponent<RadiationEmitter>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.SetEmitting(true);
  }
}
