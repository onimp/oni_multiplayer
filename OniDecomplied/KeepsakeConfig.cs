// Decompiled with JetBrains decompiler
// Type: KeepsakeConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class KeepsakeConfig : IMultiEntityConfig
{
  public List<GameObject> CreatePrefabs()
  {
    List<GameObject> prefabs = new List<GameObject>();
    prefabs.Add(KeepsakeConfig.CreateKeepsake("MegaBrain", (string) UI.KEEPSAKES.MEGA_BRAIN.NAME, (string) UI.KEEPSAKES.MEGA_BRAIN.DESCRIPTION, "keepsake_mega_brain_kanim", dlcIDs: DlcManager.AVAILABLE_ALL_VERSIONS));
    prefabs.Add(KeepsakeConfig.CreateKeepsake("CritterManipulator", (string) UI.KEEPSAKES.CRITTER_MANIPULATOR.NAME, (string) UI.KEEPSAKES.CRITTER_MANIPULATOR.DESCRIPTION, "keepsake_critter_manipulator_kanim", dlcIDs: DlcManager.AVAILABLE_ALL_VERSIONS));
    prefabs.Add(KeepsakeConfig.CreateKeepsake("LonelyMinion", (string) UI.KEEPSAKES.LONELY_MINION.NAME, (string) UI.KEEPSAKES.LONELY_MINION.DESCRIPTION, "keepsake_lonelyminion_kanim", dlcIDs: DlcManager.AVAILABLE_ALL_VERSIONS));
    prefabs.RemoveAll((Predicate<GameObject>) (x => Object.op_Equality((Object) x, (Object) null)));
    return prefabs;
  }

  public static GameObject CreateKeepsake(
    string id,
    string name,
    string desc,
    string animFile,
    string initial_anim = "idle",
    string ui_anim = "ui",
    string[] dlcIDs = null,
    KeepsakeConfig.PostInitFn postInitFn = null,
    SimHashes element = SimHashes.Creature)
  {
    if (dlcIDs == null)
      dlcIDs = DlcManager.AVAILABLE_ALL_VERSIONS;
    if (!DlcManager.IsDlcListValidForCurrentContent(dlcIDs))
      return (GameObject) null;
    string id1 = "keepsake_" + id.ToLower();
    string name1 = name;
    string desc1 = desc;
    KAnimFile anim = Assets.GetAnim(HashedString.op_Implicit(animFile));
    string initialAnim = initial_anim;
    int keepsakes = SORTORDER.KEEPSAKES;
    int element1 = (int) element;
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.MiscPickupable);
    GameObject looseEntity = EntityTemplates.CreateLooseEntity(id1, name1, desc1, 25f, true, anim, initialAnim, Grid.SceneLayer.Ore, EntityTemplates.CollisionShape.RECTANGLE, isPickupable: true, sortOrder: keepsakes, element: ((SimHashes) element1), additionalTags: additionalTags);
    looseEntity.AddOrGet<OccupyArea>().OccupiedCellsOffsets = EntityTemplates.GenerateOffsets(1, 1);
    DecorProvider decorProvider = looseEntity.AddOrGet<DecorProvider>();
    decorProvider.SetValues(TUNING.DECOR.BONUS.TIER1);
    decorProvider.overrideName = ((Object) looseEntity).name;
    looseEntity.AddOrGet<KSelectable>();
    looseEntity.GetComponent<KBatchedAnimController>().initialMode = (KAnim.PlayMode) 0;
    if (postInitFn != null)
      postInitFn(looseEntity);
    KPrefabID component = looseEntity.GetComponent<KPrefabID>();
    component.AddTag(GameTags.PedestalDisplayable, false);
    component.AddTag(GameTags.Keepsake, false);
    return looseEntity;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }

  public delegate void PostInitFn(GameObject gameObject);
}
