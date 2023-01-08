// Decompiled with JetBrains decompiler
// Type: BarLayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class BarLayer : GraphLayer
{
  public GameObject bar_container;
  public GameObject prefab_bar;
  public GraphedBarFormatting[] bar_formats;
  private List<GraphedBar> bars = new List<GraphedBar>();

  public int bar_count => this.bars.Count;

  public void NewBar(int[] values, float x_position, string ID = "")
  {
    GameObject gameObject = Util.KInstantiateUI(this.prefab_bar, this.bar_container, true);
    if (ID == "")
      ID = this.bars.Count.ToString();
    ((Object) gameObject).name = string.Format("bar_{0}", (object) ID);
    GraphedBar component = gameObject.GetComponent<GraphedBar>();
    component.SetFormat(this.bar_formats[this.bars.Count % this.bar_formats.Length]);
    int[] values1 = new int[values.Length];
    for (int index1 = 0; index1 < values.Length; ++index1)
    {
      int[] numArray = values1;
      int index2 = index1;
      Rect rect = Util.rectTransform((Component) this.graph).rect;
      int num = (int) ((double) ((Rect) ref rect).height * (double) this.graph.GetRelativeSize(new Vector2(0.0f, (float) values[index1])).y);
      numArray[index2] = num;
    }
    component.SetValues(values1, this.graph.GetRelativePosition(new Vector2(x_position, 0.0f)).x);
    this.bars.Add(component);
  }

  public void ClearBars()
  {
    foreach (GraphedBar bar in this.bars)
    {
      if (Object.op_Inequality((Object) bar, (Object) null) && Object.op_Inequality((Object) ((Component) bar).gameObject, (Object) null))
        Object.DestroyImmediate((Object) ((Component) bar).gameObject);
    }
    this.bars.Clear();
  }
}
