// Decompiled with JetBrains decompiler
// Type: GraphLayer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[RequireComponent(typeof (GraphBase))]
[AddComponentMenu("KMonoBehaviour/scripts/GraphLayer")]
public class GraphLayer : KMonoBehaviour
{
  [MyCmpReq]
  private GraphBase graph_base;

  public GraphBase graph
  {
    get
    {
      if (Object.op_Equality((Object) this.graph_base, (Object) null))
        this.graph_base = ((Component) this).GetComponent<GraphBase>();
      return this.graph_base;
    }
  }
}
