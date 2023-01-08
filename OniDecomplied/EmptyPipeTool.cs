// Decompiled with JetBrains decompiler
// Type: EmptyPipeTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class EmptyPipeTool : FilteredDragTool
{
  public static EmptyPipeTool Instance;

  public static void DestroyInstance() => EmptyPipeTool.Instance = (EmptyPipeTool) null;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    EmptyPipeTool.Instance = this;
  }

  protected override void OnDragTool(int cell, int distFromOrigin)
  {
    for (int layer = 0; layer < 44; ++layer)
    {
      if (this.IsActiveLayer((ObjectLayer) layer))
      {
        GameObject gameObject = Grid.Objects[cell, layer];
        if (!Object.op_Equality((Object) gameObject, (Object) null))
        {
          EmptyConduitWorkable component1 = gameObject.GetComponent<EmptyConduitWorkable>();
          if (!Object.op_Equality((Object) component1, (Object) null))
          {
            if (DebugHandler.InstantBuildMode)
            {
              component1.EmptyPipeContents();
            }
            else
            {
              component1.MarkForEmptying();
              Prioritizable component2 = gameObject.GetComponent<Prioritizable>();
              if (Object.op_Inequality((Object) component2, (Object) null))
                component2.SetMasterPriority(ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority());
            }
          }
        }
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

  protected override void GetDefaultFilters(
    Dictionary<string, ToolParameterMenu.ToggleState> filters)
  {
    filters.Add(ToolParameterMenu.FILTERLAYERS.LIQUIDCONDUIT, ToolParameterMenu.ToggleState.On);
    filters.Add(ToolParameterMenu.FILTERLAYERS.GASCONDUIT, ToolParameterMenu.ToggleState.On);
    filters.Add(ToolParameterMenu.FILTERLAYERS.SOLIDCONDUIT, ToolParameterMenu.ToggleState.On);
  }
}
