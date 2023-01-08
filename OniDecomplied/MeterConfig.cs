// Decompiled with JetBrains decompiler
// Type: MeterConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class MeterConfig : IEntityConfig
{
  public static readonly string ID = "Meter";

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject entity = EntityTemplates.CreateEntity(MeterConfig.ID, MeterConfig.ID, false);
    entity.AddOrGet<KBatchedAnimController>();
    entity.AddOrGet<KBatchedAnimTracker>();
    return entity;
  }

  public void OnPrefabInit(GameObject go)
  {
  }

  public void OnSpawn(GameObject go)
  {
  }
}
