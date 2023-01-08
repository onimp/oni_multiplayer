// Decompiled with JetBrains decompiler
// Type: InfraredVisualizerComponents
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class InfraredVisualizerComponents : KGameObjectComponentManager<InfraredVisualizerData>
{
  public HandleVector<int>.Handle Add(GameObject go) => this.Add(go, new InfraredVisualizerData(go));

  public void UpdateTemperature()
  {
    GridArea visibleArea = GridVisibleArea.GetVisibleArea();
    for (int index = 0; index < ((KCompactedVector<InfraredVisualizerData>) this).data.Count; ++index)
    {
      KAnimControllerBase controller = ((KCompactedVector<InfraredVisualizerData>) this).data[index].controller;
      if (Object.op_Inequality((Object) controller, (Object) null))
      {
        Vector3 position = TransformExtensions.GetPosition(((Component) controller).transform);
        if (Vector2I.op_LessThanOrEqual(visibleArea.Min, Vector2.op_Implicit(position)) && Vector2I.op_LessThanOrEqual(Vector2.op_Implicit(position), visibleArea.Max))
          ((KCompactedVector<InfraredVisualizerData>) this).data[index].Update();
      }
    }
  }

  public void ClearOverlayColour()
  {
    Color32 color32 = Color32.op_Implicit(Color.black);
    for (int index = 0; index < ((KCompactedVector<InfraredVisualizerData>) this).data.Count; ++index)
    {
      KAnimControllerBase controller = ((KCompactedVector<InfraredVisualizerData>) this).data[index].controller;
      if (Object.op_Inequality((Object) controller, (Object) null))
        controller.OverlayColour = Color32.op_Implicit(color32);
    }
  }

  public static void ClearOverlayColour(KBatchedAnimController controller)
  {
    if (!Object.op_Inequality((Object) controller, (Object) null))
      return;
    controller.OverlayColour = Color.black;
  }
}
