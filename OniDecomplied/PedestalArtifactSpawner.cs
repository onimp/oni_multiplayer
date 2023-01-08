// Decompiled with JetBrains decompiler
// Type: PedestalArtifactSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using UnityEngine;

public class PedestalArtifactSpawner : KMonoBehaviour
{
  [MyCmpReq]
  private Storage storage;
  [MyCmpReq]
  private SingleEntityReceptacle receptacle;
  [Serialize]
  private bool artifactSpawned;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    foreach (GameObject gameObject in this.storage.items)
    {
      if (ArtifactSelector.Instance.GetArtifactType(((Object) gameObject).name) == ArtifactType.Terrestrial)
        gameObject.GetComponent<KPrefabID>().AddTag(GameTags.TerrestrialArtifact, true);
    }
    if (this.artifactSpawned)
      return;
    GameObject gameObject1 = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(ArtifactSelector.Instance.GetUniqueArtifactID(ArtifactType.Terrestrial))), this.transform.position);
    gameObject1.SetActive(true);
    gameObject1.GetComponent<KPrefabID>().AddTag(GameTags.TerrestrialArtifact, true);
    this.storage.Store(gameObject1);
    this.receptacle.ForceDeposit(gameObject1);
    this.artifactSpawned = true;
  }
}
