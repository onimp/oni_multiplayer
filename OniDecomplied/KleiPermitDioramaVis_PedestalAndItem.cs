// Decompiled with JetBrains decompiler
// Type: KleiPermitDioramaVis_PedestalAndItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using UnityEngine;
using UnityEngine.UI;

public class KleiPermitDioramaVis_PedestalAndItem : KMonoBehaviour, IKleiPermitDioramaVisTarget
{
  [SerializeField]
  private KBatchedAnimController pedestalKAnim;
  [SerializeField]
  private Image itemSprite;
  private const float TILE_COUNT_TO_PEDESTAL_SLOT = 0.79f;

  public GameObject GetGameObject() => ((Component) this).gameObject;

  public void ConfigureSetup()
  {
  }

  public void ConfigureWith(PermitResource permit, PermitPresentationInfo permitPresInfo)
  {
    RectTransform rectTransform1 = Util.rectTransform((Component) this.pedestalKAnim);
    RectTransform rectTransform2 = Util.rectTransform((Component) this.itemSprite);
    rectTransform1.pivot = new Vector2(0.5f, 0.0f);
    KleiPermitVisUtil.ConfigureToRenderBuilding(this.pedestalKAnim, Assets.GetBuildingDef("ItemPedestal"));
    rectTransform2.pivot = new Vector2(0.5f, 0.0f);
    rectTransform2.anchoredPosition = Vector2.op_Addition(rectTransform1.anchoredPosition, Vector2.op_Multiply(Vector2.op_Multiply(Vector2.up, 0.79f), 176f));
    rectTransform2.sizeDelta = Vector2.op_Multiply(Vector2.one, 176f);
    this.itemSprite.sprite = permitPresInfo.sprite;
  }
}
