// Decompiled with JetBrains decompiler
// Type: ClustercraftInteriorDoorConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class ClustercraftInteriorDoorConfig : IEntityConfig
{
  public static string ID = "ClustercraftInteriorDoor";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_EXPANSION1_ONLY;

  public GameObject CreatePrefab()
  {
    string id = ClustercraftInteriorDoorConfig.ID;
    string name = (string) STRINGS.BUILDINGS.PREFABS.CLUSTERCRAFTINTERIORDOOR.NAME;
    string desc = (string) STRINGS.BUILDINGS.PREFABS.CLUSTERCRAFTINTERIORDOOR.DESC;
    EffectorValues tieR0_1 = TUNING.BUILDINGS.DECOR.BONUS.TIER0;
    EffectorValues tieR0_2 = NOISE_POLLUTION.NOISY.TIER0;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit("rocket_hatch_door_kanim"));
    EffectorValues decor = tieR0_1;
    EffectorValues noise = tieR0_2;
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.Gravitas);
    GameObject placedEntity = EntityTemplates.CreatePlacedEntity(id, name, desc, 400f, anim, "closed", Grid.SceneLayer.TileMain, 1, 2, decor, noise, additionalTags: additionalTags);
    placedEntity.AddTag(GameTags.NotRoomAssignable);
    PrimaryElement component = placedEntity.GetComponent<PrimaryElement>();
    component.SetElement(SimHashes.Unobtanium);
    component.Temperature = 294.15f;
    placedEntity.AddOrGet<Operational>();
    placedEntity.AddOrGet<LoopingSounds>();
    placedEntity.AddOrGet<Prioritizable>();
    placedEntity.AddOrGet<KBatchedAnimController>().fgLayer = Grid.SceneLayer.InteriorWall;
    placedEntity.AddOrGet<ClustercraftInteriorDoor>();
    placedEntity.AddOrGet<AssignmentGroupController>().generateGroupOnStart = false;
    placedEntity.AddOrGet<NavTeleporter>().offset = new CellOffset(1, 0);
    placedEntity.AddOrGet<AccessControl>();
    return placedEntity;
  }

  public void OnPrefabInit(GameObject inst) => inst.GetComponent<OccupyArea>().objectLayers = new ObjectLayer[1]
  {
    ObjectLayer.Building
  };

  public void OnSpawn(GameObject inst)
  {
    PrimaryElement component1 = inst.GetComponent<PrimaryElement>();
    OccupyArea component2 = inst.GetComponent<OccupyArea>();
    int cell = Grid.PosToCell(inst);
    CellOffset[] occupiedCellsOffsets = component2.OccupiedCellsOffsets;
    int[] numArray = new int[occupiedCellsOffsets.Length];
    for (int index = 0; index < occupiedCellsOffsets.Length; ++index)
    {
      CellOffset offset = occupiedCellsOffsets[index];
      int num = Grid.OffsetCell(cell, offset);
      numArray[index] = num;
    }
    for (int index = 0; index < numArray.Length; ++index)
    {
      int num = numArray[index];
      Grid.HasDoor[num] = true;
      SimMessages.SetCellProperties(num, (byte) 8);
      Grid.RenderedByWorld[num] = false;
      World.Instance.groundRenderer.MarkDirty(num);
      SimMessages.ReplaceAndDisplaceElement(num, component1.ElementID, CellEventLogger.Instance.DoorClose, component1.Mass / 2f, component1.Temperature);
      SimMessages.SetCellProperties(num, (byte) 4);
    }
  }
}
