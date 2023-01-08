// Decompiled with JetBrains decompiler
// Type: MopPlacerConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

public class MopPlacerConfig : CommonPlacerConfig, IEntityConfig
{
  public static string ID = "MopPlacer";

  public GameObject CreatePrefab()
  {
    GameObject prefab = this.CreatePrefab(MopPlacerConfig.ID, (string) MISC.PLACERS.MOPPLACER.NAME, Assets.instance.mopPlacerAssets.material);
    prefab.AddTag(GameTags.NotConversationTopic);
    Moppable moppable = prefab.AddOrGet<Moppable>();
    moppable.synchronizeAnims = false;
    moppable.amountMoppedPerTick = 20f;
    prefab.AddOrGet<Cancellable>();
    return prefab;
  }

  public void OnPrefabInit(GameObject go)
  {
  }

  public void OnSpawn(GameObject go)
  {
  }

  [Serializable]
  public class MopPlacerAssets
  {
    public Material material;
  }
}
