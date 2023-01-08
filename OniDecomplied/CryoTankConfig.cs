// Decompiled with JetBrains decompiler
// Type: CryoTankConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using TUNING;
using UnityEngine;

public class CryoTankConfig : IEntityConfig
{
  public const string ID = "CryoTank";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public GameObject CreatePrefab()
  {
    string name = (string) STRINGS.BUILDINGS.PREFABS.CRYOTANK.NAME;
    string desc = (string) STRINGS.BUILDINGS.PREFABS.CRYOTANK.DESC;
    EffectorValues tieR0_1 = TUNING.BUILDINGS.DECOR.BONUS.TIER0;
    EffectorValues tieR0_2 = NOISE_POLLUTION.NOISY.TIER0;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit("cryo_chamber_kanim"));
    EffectorValues decor = tieR0_1;
    EffectorValues noise = tieR0_2;
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity("CryoTank", name, desc, 100f, anim, "off", Grid.SceneLayer.Building, 2, 3, decor, noise);
    placedEntity.GetComponent<KAnimControllerBase>().SetFGLayer(Grid.SceneLayer.BuildingFront);
    PrimaryElement component = placedEntity.GetComponent<PrimaryElement>();
    component.SetElement(SimHashes.Unobtanium);
    component.Temperature = 294.15f;
    Workable workable = placedEntity.AddOrGet<Workable>();
    workable.synchronizeAnims = false;
    workable.resetProgressOnStop = true;
    CryoTank cryoTank = placedEntity.AddOrGet<CryoTank>();
    cryoTank.overrideAnim = "anim_interacts_cryo_activation_kanim";
    cryoTank.dropOffset = new CellOffset(1, 0);
    LoreBearerUtil.AddLoreTo(placedEntity, LoreBearerUtil.UnlockSpecificEntry("cryotank_warning", (string) UI.USERMENUACTIONS.READLORE.SEARCH_CRYO_TANK));
    placedEntity.AddOrGet<Demolishable>().allowDemolition = false;
    placedEntity.AddOrGet<OccupyArea>().objectLayers = new ObjectLayer[1]
    {
      ObjectLayer.Building
    };
    return placedEntity;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
