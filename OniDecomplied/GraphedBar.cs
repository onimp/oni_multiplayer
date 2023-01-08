// Decompiled with JetBrains decompiler
// Type: GraphedBar
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/GraphedBar")]
[Serializable]
public class GraphedBar : KMonoBehaviour
{
  public GameObject segments_container;
  public GameObject prefab_segment;
  private List<GameObject> segments = new List<GameObject>();
  private GraphedBarFormatting format;

  public void SetFormat(GraphedBarFormatting format) => this.format = format;

  public void SetValues(int[] values, float x_position)
  {
    this.ClearValues();
    Util.rectTransform(((Component) this).gameObject).anchorMin = new Vector2(x_position, 0.0f);
    Util.rectTransform(((Component) this).gameObject).anchorMax = new Vector2(x_position, 1f);
    Util.rectTransform(((Component) this).gameObject).SetSizeWithCurrentAnchors((RectTransform.Axis) 0, (float) this.format.width);
    for (int index = 0; index < values.Length; ++index)
    {
      GameObject gameObject = Util.KInstantiateUI(this.prefab_segment, this.segments_container, true);
      LayoutElement component = gameObject.GetComponent<LayoutElement>();
      component.preferredHeight = (float) values[index];
      component.minWidth = (float) this.format.width;
      ((Graphic) gameObject.GetComponent<Image>()).color = this.format.colors[index % this.format.colors.Length];
      this.segments.Add(gameObject);
    }
  }

  public void ClearValues()
  {
    foreach (Object segment in this.segments)
      Object.DestroyImmediate(segment);
    this.segments.Clear();
  }
}
