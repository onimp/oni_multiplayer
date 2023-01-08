// Decompiled with JetBrains decompiler
// Type: KleiPermitDioramaVis_DupeEquipment
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using System.Collections.Generic;
using UnityEngine;

public class KleiPermitDioramaVis_DupeEquipment : KMonoBehaviour, IKleiPermitDioramaVisTarget
{
  [SerializeField]
  private UIMannequin uiMannequin;

  public GameObject GetGameObject() => ((Component) this).gameObject;

  public void ConfigureSetup()
  {
  }

  public void ConfigureWith(PermitResource permit, PermitPresentationInfo permitPresInfo)
  {
    if (!(permit is ClothingItemResource clothingItemResource))
      return;
    this.uiMannequin.SetOutfit((IEnumerable<ClothingItemResource>) new ClothingItemResource[1]
    {
      clothingItemResource
    });
    this.uiMannequin.ReactToClothingItemChange(clothingItemResource.Category);
  }
}
