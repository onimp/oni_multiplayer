// Decompiled with JetBrains decompiler
// Type: AttackTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class AttackTool : DragTool
{
  protected override void OnDragComplete(Vector3 downPos, Vector3 upPos) => AttackTool.MarkForAttack(this.GetRegularizedPos(Vector2.Min(Vector2.op_Implicit(downPos), Vector2.op_Implicit(upPos)), true), this.GetRegularizedPos(Vector2.Max(Vector2.op_Implicit(downPos), Vector2.op_Implicit(upPos)), false), true);

  public static void MarkForAttack(Vector2 min, Vector2 max, bool mark)
  {
    foreach (FactionAlignment factionAlignment in Components.FactionAlignments.Items)
    {
      Vector2 vector2 = Vector2I.op_Implicit(Grid.PosToXY(TransformExtensions.GetPosition(factionAlignment.transform)));
      if ((double) vector2.x >= (double) min.x && (double) vector2.x < (double) max.x && (double) vector2.y >= (double) min.y && (double) vector2.y < (double) max.y)
      {
        if (mark)
        {
          if (FactionManager.Instance.GetDisposition(FactionManager.FactionID.Duplicant, factionAlignment.Alignment) != FactionManager.Disposition.Assist)
          {
            factionAlignment.SetPlayerTargeted(true);
            Prioritizable component = ((Component) factionAlignment).GetComponent<Prioritizable>();
            if (Object.op_Inequality((Object) component, (Object) null))
              component.SetMasterPriority(ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority());
          }
        }
        else
          EventExtensions.Trigger(((Component) factionAlignment).gameObject, 2127324410, (object) null);
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
