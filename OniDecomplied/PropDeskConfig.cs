// Decompiled with JetBrains decompiler
// Type: PropDeskConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class PropDeskConfig : IEntityConfig
{
  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    string name = (string) STRINGS.BUILDINGS.PREFABS.PROPDESK.NAME;
    string desc = (string) STRINGS.BUILDINGS.PREFABS.PROPDESK.DESC;
    EffectorValues tieR0_1 = TUNING.BUILDINGS.DECOR.BONUS.TIER0;
    EffectorValues tieR0_2 = NOISE_POLLUTION.NOISY.TIER0;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit("setpiece_desk_kanim"));
    EffectorValues decor = tieR0_1;
    EffectorValues noise = tieR0_2;
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.Gravitas);
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity("PropDesk", name, desc, 50f, anim, "off", Grid.SceneLayer.Building, 3, 2, decor, noise, additionalTags: additionalTags);
    PrimaryElement component = placedEntity.GetComponent<PrimaryElement>();
    component.SetElement(SimHashes.Steel);
    component.Temperature = 294.15f;
    LoreBearerUtil.AddLoreTo(placedEntity, new LoreBearerAction(LoreBearerUtil.UnlockNextEmail));
    placedEntity.AddOrGet<Demolishable>();
    return placedEntity;
  }

  public void OnPrefabInit(GameObject inst) => inst.GetComponent<OccupyArea>().objectLayers = new ObjectLayer[1]
  {
    ObjectLayer.Building
  };

  public void OnSpawn(GameObject inst)
  {
  }
}
