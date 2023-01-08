// Decompiled with JetBrains decompiler
// Type: BleachStoneConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class BleachStoneConfig : IOreConfig
{
  public SimHashes ElementID => SimHashes.BleachStone;

  public SimHashes SublimeElementID => SimHashes.ChlorineGas;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject solidOreEntity = EntityTemplates.CreateSolidOreEntity(this.ElementID);
    Sublimates sublimates = solidOreEntity.AddOrGet<Sublimates>();
    sublimates.spawnFXHash = SpawnFXHashes.BleachStoneEmissionBubbles;
    sublimates.info = new Sublimates.Info(0.000200000009f, 0.00250000018f, 1.8f, 0.5f, this.SublimeElementID);
    return solidOreEntity;
  }
}
