// Decompiled with JetBrains decompiler
// Type: KleiPermitDioramaVis_BuildingOnBackground
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using UnityEngine;

public class KleiPermitDioramaVis_BuildingOnBackground : KMonoBehaviour, IKleiPermitDioramaVisTarget
{
  [SerializeField]
  private KBatchedAnimController buildingKAnimPrefab;
  private KBatchedAnimController[] buildingKAnimArray;

  public void ConfigureSetup()
  {
    ((Component) this.buildingKAnimPrefab).gameObject.SetActive(false);
    this.buildingKAnimArray = new KBatchedAnimController[9];
    for (int index = 0; index < this.buildingKAnimArray.Length; ++index)
      this.buildingKAnimArray[index] = (KBatchedAnimController) Object.Instantiate((Object) this.buildingKAnimPrefab, ((Component) this.buildingKAnimPrefab).transform.parent, false);
    Vector2 anchoredPosition = Util.rectTransform((Component) this.buildingKAnimPrefab).anchoredPosition;
    Vector2 vector2_1 = Vector2.op_Multiply(175f, Vector2.one);
    Vector2 vector2_2 = Vector2.op_Multiply(vector2_1, new Vector2(-1f, 0.0f));
    Vector2 vector2_3 = Vector2.op_Addition(anchoredPosition, vector2_2);
    int index1 = 0;
    for (int index2 = 0; index2 < 3; ++index2)
    {
      int num = 0;
      while (num < 3)
      {
        Util.rectTransform((Component) this.buildingKAnimArray[index1]).anchoredPosition = Vector2.op_Addition(vector2_3, Vector2.op_Multiply(vector2_1, new Vector2((float) index2, (float) num)));
        ((Component) this.buildingKAnimArray[index1]).gameObject.SetActive(true);
        ++num;
        ++index1;
      }
    }
  }

  public GameObject GetGameObject() => ((Component) this).gameObject;

  public void ConfigureWith(PermitResource permit, PermitPresentationInfo permitPresInfo)
  {
    BuildingFacadeResource buildingPermit = (BuildingFacadeResource) permit;
    BuildingDef buildingDef = KleiPermitVisUtil.GetBuildingDef(permit).Value;
    DebugUtil.DevAssert(buildingDef.WidthInCells == 1, "assert failed", (Object) null);
    DebugUtil.DevAssert(buildingDef.HeightInCells == 1, "assert failed", (Object) null);
    foreach (KBatchedAnimController buildingKanim in this.buildingKAnimArray)
      KleiPermitVisUtil.ConfigureToRenderBuilding(buildingKanim, buildingPermit);
  }
}
