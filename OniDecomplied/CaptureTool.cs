// Decompiled with JetBrains decompiler
// Type: CaptureTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

public class CaptureTool : DragTool
{
  protected override void OnDragComplete(Vector3 downPos, Vector3 upPos) => CaptureTool.MarkForCapture(this.GetRegularizedPos(Vector2.Min(Vector2.op_Implicit(downPos), Vector2.op_Implicit(upPos)), true), this.GetRegularizedPos(Vector2.Max(Vector2.op_Implicit(downPos), Vector2.op_Implicit(upPos)), false), true);

  public static void MarkForCapture(Vector2 min, Vector2 max, bool mark)
  {
    foreach (Capturable capturable in Components.Capturables.Items)
    {
      Vector2 vector2 = Vector2I.op_Implicit(Grid.PosToXY(TransformExtensions.GetPosition(capturable.transform)));
      if ((double) vector2.x >= (double) min.x && (double) vector2.x < (double) max.x && (double) vector2.y >= (double) min.y && (double) vector2.y < (double) max.y)
      {
        if (capturable.allowCapture)
        {
          PrioritySetting selectedPriority = ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority();
          capturable.MarkForCapture(mark, selectedPriority, true);
        }
        else if (mark)
          PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, (string) UI.TOOLS.CAPTURE.NOT_CAPTURABLE, (Transform) null, TransformExtensions.GetPosition(capturable.transform));
      }
    }
  }

  protected override void OnActivateTool()
  {
    base.OnActivateTool();
    ToolMenu.Instance.PriorityScreen.Show(true);
  }

  protected override void OnDeactivateTool(InterfaceTool new_tool)
  {
    base.OnDeactivateTool(new_tool);
    ToolMenu.Instance.PriorityScreen.Show(false);
  }
}
