// Decompiled with JetBrains decompiler
// Type: OxyRockConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class OxyRockConfig : IOreConfig
{
  public SimHashes ElementID => SimHashes.OxyRock;

  public SimHashes SublimeElementID => SimHashes.Oxygen;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject solidOreEntity = EntityTemplates.CreateSolidOreEntity(this.ElementID);
    Sublimates sublimates = solidOreEntity.AddOrGet<Sublimates>();
    sublimates.spawnFXHash = SpawnFXHashes.OxygenEmissionBubbles;
    sublimates.info = new Sublimates.Info(0.0100000007f, 0.00500000035f, 1.8f, 0.7f, this.SublimeElementID);
    return solidOreEntity;
  }
}
