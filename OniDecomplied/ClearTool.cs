// Decompiled with JetBrains decompiler
// Type: ClearTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class ClearTool : DragTool
{
  public static ClearTool Instance;

  public static void DestroyInstance() => ClearTool.Instance = (ClearTool) null;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    ClearTool.Instance = this;
    this.interceptNumberKeysForPriority = true;
  }

  public void Activate() => PlayerController.Instance.ActivateTool((InterfaceTool) this);

  protected override void OnDragTool(int cell, int distFromOrigin)
  {
    GameObject gameObject1 = Grid.Objects[cell, 3];
    if (Object.op_Equality((Object) gameObject1, (Object) null))
      return;
    ObjectLayerListItem objectLayerListItem = gameObject1.GetComponent<Pickupable>().objectLayerListItem;
    while (objectLayerListItem != null)
    {
      GameObject gameObject2 = objectLayerListItem.gameObject;
      objectLayerListItem = objectLayerListItem.nextItem;
      if (!Object.op_Equality((Object) gameObject2, (Object) null) && !Object.op_Inequality((Object) gameObject2.GetComponent<MinionIdentity>(), (Object) null) && gameObject2.GetComponent<Clearable>().isClearable)
      {
        gameObject2.GetComponent<Clearable>().MarkForClear();
        Prioritizable component = gameObject2.GetComponent<Prioritizable>();
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
