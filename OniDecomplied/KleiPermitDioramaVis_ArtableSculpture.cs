// Decompiled with JetBrains decompiler
// Type: KleiPermitDioramaVis_ArtableSculpture
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using UnityEngine;

public class KleiPermitDioramaVis_ArtableSculpture : KMonoBehaviour, IKleiPermitDioramaVisTarget
{
  [SerializeField]
  private KBatchedAnimController buildingKAnim;

  public GameObject GetGameObject() => ((Component) this).gameObject;

  public void ConfigureSetup() => SymbolOverrideControllerUtil.AddToPrefab(((Component) this.buildingKAnim).gameObject);

  public void ConfigureWith(PermitResource permit, PermitPresentationInfo permitPresInfo) => KleiPermitVisUtil.ConfigureToRenderBuilding(this.buildingKAnim, (ArtableStage) permit);
}
