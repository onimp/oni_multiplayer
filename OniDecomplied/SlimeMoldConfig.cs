// Decompiled with JetBrains decompiler
// Type: SlimeMoldConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class SlimeMoldConfig : IOreConfig
{
  public SimHashes ElementID => SimHashes.SlimeMold;

  public SimHashes SublimeElementID => SimHashes.ContaminatedOxygen;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject solidOreEntity = EntityTemplates.CreateSolidOreEntity(this.ElementID);
    Sublimates sublimates = solidOreEntity.AddOrGet<Sublimates>();
    sublimates.spawnFXHash = SpawnFXHashes.ContaminatedOxygenBubble;
    sublimates.info = new Sublimates.Info(0.025f, 0.125f, 1.8f, 0.0f, this.SublimeElementID);
    return solidOreEntity;
  }
}
