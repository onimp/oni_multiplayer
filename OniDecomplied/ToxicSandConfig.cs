// Decompiled with JetBrains decompiler
// Type: ToxicSandConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class ToxicSandConfig : IOreConfig
{
  public SimHashes ElementID => SimHashes.ToxicSand;

  public SimHashes SublimeElementID => SimHashes.ContaminatedOxygen;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject solidOreEntity = EntityTemplates.CreateSolidOreEntity(this.ElementID);
    Sublimates sublimates = solidOreEntity.AddOrGet<Sublimates>();
    sublimates.spawnFXHash = SpawnFXHashes.ContaminatedOxygenBubble;
    sublimates.info = new Sublimates.Info(2.00000013E-05f, 0.05f, 1.8f, 0.5f, this.SublimeElementID);
    return solidOreEntity;
  }
}
