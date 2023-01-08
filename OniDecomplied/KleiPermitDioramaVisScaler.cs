// Decompiled with JetBrains decompiler
// Type: KleiPermitDioramaVisScaler
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteAlways]
public class KleiPermitDioramaVisScaler : UIBehaviour
{
  public const float REFERENCE_WIDTH = 1700f;
  public const float REFERENCE_HEIGHT = 800f;
  [SerializeField]
  private RectTransform root;
  [SerializeField]
  private RectTransform scaleTarget;
  [SerializeField]
  private RectTransform slot;

  protected virtual void OnRectTransformDimensionsChange() => this.Layout();

  public void Layout() => KleiPermitDioramaVisScaler.Layout(this.root, this.scaleTarget, this.slot);

  public static void Layout(RectTransform root, RectTransform scaleTarget, RectTransform slot)
  {
    float num1 = 2.125f;
    AspectRatioFitter orAddComponent = Util.FindOrAddComponent<AspectRatioFitter>((Component) slot);
    orAddComponent.aspectRatio = num1;
    orAddComponent.aspectMode = (AspectRatioFitter.AspectMode) 1;
    float num2 = 1700f;
    Rect rect1 = root.rect;
    double num3 = (double) Mathf.Max(0.1f, ((Rect) ref rect1).width) / (double) num2;
    float num4 = 800f;
    Rect rect2 = root.rect;
    double num5 = (double) (Mathf.Max(0.1f, ((Rect) ref rect2).height) / num4);
    float num6 = Mathf.Max((float) num3, (float) num5);
    ((Transform) scaleTarget).localScale = Vector3.op_Multiply(Vector3.one, num6);
    scaleTarget.sizeDelta = new Vector2(1700f, 800f);
    scaleTarget.anchorMin = Vector2.op_Multiply(Vector2.one, 0.5f);
    scaleTarget.anchorMax = Vector2.op_Multiply(Vector2.one, 0.5f);
    scaleTarget.pivot = Vector2.op_Multiply(Vector2.one, 0.5f);
    scaleTarget.anchoredPosition = Vector2.zero;
  }
}
