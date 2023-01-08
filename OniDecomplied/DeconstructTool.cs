// Decompiled with JetBrains decompiler
// Type: DeconstructTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class DeconstructTool : FilteredDragTool
{
  public static DeconstructTool Instance;

  public static void DestroyInstance() => DeconstructTool.Instance = (DeconstructTool) null;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    DeconstructTool.Instance = this;
  }

  public void Activate() => PlayerController.Instance.ActivateTool((InterfaceTool) this);

  protected override string GetConfirmSound() => "Tile_Confirm_NegativeTool";

  protected override string GetDragSound() => "Tile_Drag_NegativeTool";

  protected override void OnDragTool(int cell, int distFromOrigin) => this.DeconstructCell(cell);

  public void DeconstructCell(int cell)
  {
    for (int layer = 0; layer < 44; ++layer)
    {
      GameObject input = Grid.Objects[cell, layer];
      if (Object.op_Inequality((Object) input, (Object) null) && this.IsActiveLayer(this.GetFilterLayerFromGameObject(input)))
      {
        EventExtensions.Trigger(input, -790448070, (object) null);
        Prioritizable component = input.GetComponent<Prioritizable>();
        if (Object.op_Inequality((Object) component, (Object) null))
          component.SetMasterPriority(ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority());
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
