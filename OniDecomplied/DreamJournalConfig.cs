// Decompiled with JetBrains decompiler
// Type: DreamJournalConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class DreamJournalConfig : IEntityConfig
{
  public static Tag ID = new Tag("DreamJournal");
  public const float MASS = 1f;
  public const int FABRICATION_TIME_SECONDS = 300;
  private const string ANIM_FILE = "dream_journal_kanim";
  private const string INITIAL_ANIM = "object";
  public const int MAX_STACK_SIZE = 25;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }

  public GameObject CreatePrefab()
  {
    KAnimFile anim1 = Assets.GetAnim(HashedString.op_Implicit("dream_journal_kanim"));
    string name1 = ((Tag) ref DreamJournalConfig.ID).Name;
    string name2 = (string) ITEMS.DREAMJOURNAL.NAME;
    string desc = (string) ITEMS.DREAMJOURNAL.DESC;
    KAnimFile anim2 = anim1;
    List<Tag> additionalTags = new List<Tag>();
    additionalTags.Add(GameTags.StoryTraitResource);
    GameObject looseEntity = EntityTemplates.CreateLooseEntity(name1, name2, desc, 1f, true, anim2, "object", Grid.SceneLayer.BuildingFront, EntityTemplates.CollisionShape.RECTANGLE, isPickupable: true, additionalTags: additionalTags);
    looseEntity.AddOrGet<EntitySplitter>().maxStackSize = 25f;
    return looseEntity;
  }
}
