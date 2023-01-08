// Decompiled with JetBrains decompiler
// Type: StoredMinionConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using UnityEngine;

public class StoredMinionConfig : IEntityConfig
{
  public static string ID = "StoredMinion";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject entity = EntityTemplates.CreateEntity(StoredMinionConfig.ID, StoredMinionConfig.ID);
    entity.AddOrGet<SaveLoadRoot>();
    entity.AddOrGet<KPrefabID>();
    entity.AddOrGet<Traits>();
    entity.AddOrGet<Schedulable>();
    entity.AddOrGet<StoredMinionIdentity>();
    entity.AddOrGet<KSelectable>().IsSelectable = false;
    entity.AddOrGet<MinionModifiers>().addBaseTraits = false;
    return entity;
  }

  public void OnPrefabInit(GameObject go)
  {
  }

  public void OnSpawn(GameObject go)
  {
  }
}
