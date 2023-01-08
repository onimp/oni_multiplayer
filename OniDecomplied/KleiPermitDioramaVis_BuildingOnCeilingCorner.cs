// Decompiled with JetBrains decompiler
// Type: KleiPermitDioramaVis_BuildingOnCeilingCorner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using UnityEngine;

public class KleiPermitDioramaVis_BuildingOnCeilingCorner : 
  KMonoBehaviour,
  IKleiPermitDioramaVisTarget
{
  [SerializeField]
  private KBatchedAnimController buildingKAnim;
  private PrefabDefinedUIPosition buildingKAnimPosition = new PrefabDefinedUIPosition();

  public GameObject GetGameObject() => ((Component) this).gameObject;

  public void ConfigureSetup()
  {
  }

  public void ConfigureWith(PermitResource permit, PermitPresentationInfo permitPresInfo)
  {
    KleiPermitVisUtil.ConfigureToRenderBuilding(this.buildingKAnim, (BuildingFacadeResource) permit);
    BuildingDef buildingDef = KleiPermitVisUtil.GetBuildingDef(permit).Value;
    this.buildingKAnimPosition.SetOn((Component) this.buildingKAnim);
    RectTransform rectTransform = Util.rectTransform((Component) this.buildingKAnim);
    rectTransform.anchoredPosition = Vector2.op_Addition(rectTransform.anchoredPosition, new Vector2(0.0f, (float) (-176.0 * (double) buildingDef.HeightInCells + 176.0)));
  }
}
