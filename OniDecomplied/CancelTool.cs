// Decompiled with JetBrains decompiler
// Type: CancelTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class CancelTool : FilteredDragTool
{
  public static CancelTool Instance;

  public static void DestroyInstance() => CancelTool.Instance = (CancelTool) null;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    CancelTool.Instance = this;
  }

  protected override void GetDefaultFilters(
    Dictionary<string, ToolParameterMenu.ToggleState> filters)
  {
    base.GetDefaultFilters(filters);
    filters.Add(ToolParameterMenu.FILTERLAYERS.CLEANANDCLEAR, ToolParameterMenu.ToggleState.Off);
    filters.Add(ToolParameterMenu.FILTERLAYERS.DIGPLACER, ToolParameterMenu.ToggleState.Off);
  }

  protected override string GetConfirmSound() => "Tile_Confirm_NegativeTool";

  protected override string GetDragSound() => "Tile_Drag_NegativeTool";

  protected override void OnDragTool(int cell, int distFromOrigin)
  {
    for (int layer = 0; layer < 44; ++layer)
    {
      GameObject input = Grid.Objects[cell, layer];
      if (Object.op_Inequality((Object) input, (Object) null) && this.IsActiveLayer(this.GetFilterLayerFromGameObject(input)))
        EventExtensions.Trigger(input, 2127324410, (object) null);
    }
  }

  protected override void OnDragComplete(Vector3 downPos, Vector3 upPos)
  {
    Vector2 regularizedPos1 = this.GetRegularizedPos(Vector2.Min(Vector2.op_Implicit(downPos), Vector2.op_Implicit(upPos)), true);
    Vector2 regularizedPos2 = this.GetRegularizedPos(Vector2.Max(Vector2.op_Implicit(downPos), Vector2.op_Implicit(upPos)), false);
    AttackTool.MarkForAttack(regularizedPos1, regularizedPos2, false);
    CaptureTool.MarkForCapture(regularizedPos1, regularizedPos2, false);
  }
}
