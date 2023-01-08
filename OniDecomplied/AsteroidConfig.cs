// Decompiled with JetBrains decompiler
// Type: AsteroidConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class AsteroidConfig : IEntityConfig
{
  public const string ID = "Asteroid";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject entity = EntityTemplates.CreateEntity("Asteroid", "Asteroid");
    entity.AddOrGet<SaveLoadRoot>();
    entity.AddOrGet<WorldInventory>();
    entity.AddOrGet<WorldContainer>();
    entity.AddOrGet<AsteroidGridEntity>();
    entity.AddOrGet<OrbitalMechanics>();
    entity.AddOrGetDef<GameplaySeasonManager.Def>();
    entity.AddOrGetDef<AlertStateManager.Def>();
    return entity;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }
}
