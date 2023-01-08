// Decompiled with JetBrains decompiler
// Type: ShadowRect
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.UI;

public class ShadowRect : MonoBehaviour
{
  public RectTransform RectMain;
  public RectTransform RectShadow;
  [SerializeField]
  protected Color shadowColor = new Color(0.0f, 0.0f, 0.0f, 0.6f);
  [SerializeField]
  protected Vector2 ShadowOffset = new Vector2(1.5f, -1.5f);
  private LayoutElement shadowLayoutElement;

  private void OnEnable()
  {
    if (Object.op_Inequality((Object) this.RectShadow, (Object) null))
    {
      ((Object) this.RectShadow).name = "Shadow_" + ((Object) this.RectMain).name;
      this.MatchRect();
    }
    else
      Debug.LogWarning((object) ("Shadowrect is missing rectshadow: " + ((Object) ((Component) this).gameObject).name));
  }

  private void Update() => this.MatchRect();

  protected virtual void MatchRect()
  {
    if (Object.op_Equality((Object) this.RectShadow, (Object) null) || Object.op_Equality((Object) this.RectMain, (Object) null))
      return;
    if (Object.op_Equality((Object) this.shadowLayoutElement, (Object) null))
      this.shadowLayoutElement = ((Component) this.RectShadow).GetComponent<LayoutElement>();
    if (Object.op_Inequality((Object) this.shadowLayoutElement, (Object) null) && !this.shadowLayoutElement.ignoreLayout)
      this.shadowLayoutElement.ignoreLayout = true;
    if (Object.op_Inequality((Object) ((Component) this.RectShadow).transform.parent, (Object) ((Component) this.RectMain).transform.parent))
      ((Component) this.RectShadow).transform.SetParent(((Component) this.RectMain).transform.parent);
    if (((Transform) this.RectShadow).GetSiblingIndex() >= ((Transform) this.RectMain).GetSiblingIndex())
      ((Transform) this.RectShadow).SetAsFirstSibling();
    ((Component) this.RectShadow).transform.localScale = Vector3.one;
    if (Vector2.op_Inequality(this.RectShadow.pivot, this.RectMain.pivot))
      this.RectShadow.pivot = this.RectMain.pivot;
    if (Vector2.op_Inequality(this.RectShadow.anchorMax, this.RectMain.anchorMax))
      this.RectShadow.anchorMax = this.RectMain.anchorMax;
    if (Vector2.op_Inequality(this.RectShadow.anchorMin, this.RectMain.anchorMin))
      this.RectShadow.anchorMin = this.RectMain.anchorMin;
    if (Vector2.op_Inequality(this.RectShadow.sizeDelta, this.RectMain.sizeDelta))
      this.RectShadow.sizeDelta = this.RectMain.sizeDelta;
    if (Vector2.op_Inequality(this.RectShadow.anchoredPosition, Vector2.op_Addition(this.RectMain.anchoredPosition, this.ShadowOffset)))
      this.RectShadow.anchoredPosition = Vector2.op_Addition(this.RectMain.anchoredPosition, this.ShadowOffset);
    if (((Component) this.RectMain).gameObject.activeInHierarchy == ((Component) this.RectShadow).gameObject.activeInHierarchy)
      return;
    ((Component) this.RectShadow).gameObject.SetActive(((Component) this.RectMain).gameObject.activeInHierarchy);
  }
}
