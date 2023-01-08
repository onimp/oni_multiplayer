// Decompiled with JetBrains decompiler
// Type: DirtyWaterConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class DirtyWaterConfig : IOreConfig
{
  public SimHashes ElementID => SimHashes.DirtyWater;

  public SimHashes SublimeElementID => SimHashes.ContaminatedOxygen;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject liquidOreEntity = EntityTemplates.CreateLiquidOreEntity(this.ElementID);
    Sublimates sublimates = liquidOreEntity.AddOrGet<Sublimates>();
    sublimates.spawnFXHash = SpawnFXHashes.ContaminatedOxygenBubbleWater;
    sublimates.info = new Sublimates.Info(4.00000063E-05f, 0.025f, 1.8f, 1f, this.SublimeElementID);
    return liquidOreEntity;
  }
}
