// Decompiled with JetBrains decompiler
// Type: HarvestTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class HarvestTool : DragTool
{
  public GameObject Placer;
  public static HarvestTool Instance;
  public Texture2D[] visualizerTextures;
  private Dictionary<string, ToolParameterMenu.ToggleState> options = new Dictionary<string, ToolParameterMenu.ToggleState>();

  public static void DestroyInstance() => HarvestTool.Instance = (HarvestTool) null;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    HarvestTool.Instance = this;
    this.options.Add("HARVEST_WHEN_READY", ToolParameterMenu.ToggleState.On);
    this.options.Add("DO_NOT_HARVEST", ToolParameterMenu.ToggleState.Off);
    this.viewMode = OverlayModes.Harvest.ID;
  }

  protected override void OnDragTool(int cell, int distFromOrigin)
  {
    if (!Grid.IsValidCell(cell))
      return;
    foreach (HarvestDesignatable cmp in Components.HarvestDesignatables.Items)
    {
      OccupyArea area = cmp.area;
      if (Grid.PosToCell((KMonoBehaviour) cmp) == cell || Object.op_Inequality((Object) area, (Object) null) && area.CheckIsOccupying(cell))
      {
        if (this.options["HARVEST_WHEN_READY"] == ToolParameterMenu.ToggleState.On)
          cmp.SetHarvestWhenReady(true);
        else if (this.options["DO_NOT_HARVEST"] == ToolParameterMenu.ToggleState.On)
          cmp.SetHarvestWhenReady(false);
        Prioritizable component = ((Component) cmp).GetComponent<Prioritizable>();
        if (Object.op_Inequality((Object) component, (Object) null))
          component.SetMasterPriority(ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority());
      }
    }
  }

  public void Update()
  {
    MeshRenderer componentInChildren = this.visualizer.GetComponentInChildren<MeshRenderer>();
    if (!Object.op_Inequality((Object) componentInChildren, (Object) null))
      return;
    if (this.options["HARVEST_WHEN_READY"] == ToolParameterMenu.ToggleState.On)
    {
      ((Renderer) componentInChildren).material.mainTexture = (Texture) this.visualizerTextures[0];
    }
    else
    {
      if (this.options["DO_NOT_HARVEST"] != ToolParameterMenu.ToggleState.On)
        return;
      ((Renderer) componentInChildren).material.mainTexture = (Texture) this.visualizerTextures[1];
    }
  }

  public override void OnLeftClickUp(Vector3 cursor_pos) => base.OnLeftClickUp(cursor_pos);

  protected override void OnActivateTool()
  {
    base.OnActivateTool();
    ToolMenu.Instance.PriorityScreen.Show(true);
    ToolMenu.Instance.toolParameterMenu.PopulateMenu(this.options);
  }

  protected override void OnDeactivateTool(InterfaceTool new_tool)
  {
    base.OnDeactivateTool(new_tool);
    ToolMenu.Instance.PriorityScreen.Show(false);
    ToolMenu.Instance.toolParameterMenu.ClearMenu();
  }
}
