// Decompiled with JetBrains decompiler
// Type: NuclearWasteConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class NuclearWasteConfig : IOreConfig
{
  public SimHashes ElementID => SimHashes.NuclearWaste;

  public string[] GetDlcIds() => DlcManager.AVAILABLE_ALL_VERSIONS;

  public GameObject CreatePrefab()
  {
    GameObject liquidOreEntity = EntityTemplates.CreateLiquidOreEntity(this.ElementID);
    Sublimates sublimates = liquidOreEntity.AddOrGet<Sublimates>();
    sublimates.decayStorage = true;
    sublimates.spawnFXHash = SpawnFXHashes.NuclearWasteDrip;
    sublimates.info = new Sublimates.Info(0.066f, 6.6f, 1000f, 0.0f, this.ElementID);
    return liquidOreEntity;
  }
}
