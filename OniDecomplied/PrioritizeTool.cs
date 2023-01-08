// Decompiled with JetBrains decompiler
// Type: PrioritizeTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class PrioritizeTool : FilteredDragTool
{
  public GameObject Placer;
  public static PrioritizeTool Instance;
  public Texture2D[] cursors;

  public static void DestroyInstance() => PrioritizeTool.Instance = (PrioritizeTool) null;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.interceptNumberKeysForPriority = true;
    PrioritizeTool.Instance = this;
    this.visualizer = Util.KInstantiate(this.visualizer, (GameObject) null, (string) null);
    this.viewMode = OverlayModes.Priorities.ID;
    Game.Instance.prioritizableRenderer.currentTool = this;
  }

  public override string GetFilterLayerFromGameObject(GameObject input)
  {
    bool flag1 = false;
    bool flag2 = false;
    bool flag3 = false;
    if (Object.op_Implicit((Object) input.GetComponent<Diggable>()))
      flag1 = true;
    if (Object.op_Implicit((Object) input.GetComponent<Constructable>()) || Object.op_Implicit((Object) input.GetComponent<Deconstructable>()) && input.GetComponent<Deconstructable>().IsMarkedForDeconstruction())
      flag2 = true;
    if (Object.op_Implicit((Object) input.GetComponent<Clearable>()) || Object.op_Implicit((Object) input.GetComponent<Moppable>()) || Object.op_Implicit((Object) input.GetComponent<StorageLocker>()))
      flag3 = true;
    if (flag2)
      return ToolParameterMenu.FILTERLAYERS.CONSTRUCTION;
    if (flag1)
      return ToolParameterMenu.FILTERLAYERS.DIG;
    return flag3 ? ToolParameterMenu.FILTERLAYERS.CLEAN : ToolParameterMenu.FILTERLAYERS.OPERATE;
  }

  protected override void GetDefaultFilters(
    Dictionary<string, ToolParameterMenu.ToggleState> filters)
  {
    filters.Add(ToolParameterMenu.FILTERLAYERS.ALL, ToolParameterMenu.ToggleState.On);
    filters.Add(ToolParameterMenu.FILTERLAYERS.CONSTRUCTION, ToolParameterMenu.ToggleState.Off);
    filters.Add(ToolParameterMenu.FILTERLAYERS.DIG, ToolParameterMenu.ToggleState.Off);
    filters.Add(ToolParameterMenu.FILTERLAYERS.CLEAN, ToolParameterMenu.ToggleState.Off);
    filters.Add(ToolParameterMenu.FILTERLAYERS.OPERATE, ToolParameterMenu.ToggleState.Off);
  }

  private bool TryPrioritizeGameObject(GameObject target, PrioritySetting priority)
  {
    if (this.IsActiveLayer(this.GetFilterLayerFromGameObject(target)))
    {
      Prioritizable component = target.GetComponent<Prioritizable>();
      if (Object.op_Inequality((Object) component, (Object) null) && component.showIcon && component.IsPrioritizable())
      {
        component.SetMasterPriority(priority);
        return true;
      }
    }
    return false;
  }

  protected override void OnDragTool(int cell, int distFromOrigin)
  {
    PrioritySetting selectedPriority = ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority();
    int num = 0;
    for (int layer = 0; layer < 44; ++layer)
    {
      GameObject target = Grid.Objects[cell, layer];
      if (Object.op_Inequality((Object) target, (Object) null))
      {
        if (Object.op_Implicit((Object) target.GetComponent<Pickupable>()))
        {
          ObjectLayerListItem objectLayerListItem = target.GetComponent<Pickupable>().objectLayerListItem;
          while (objectLayerListItem != null)
          {
            GameObject gameObject = objectLayerListItem.gameObject;
            objectLayerListItem = objectLayerListItem.nextItem;
            if (!Object.op_Equality((Object) gameObject, (Object) null) && !Object.op_Inequality((Object) gameObject.GetComponent<MinionIdentity>(), (Object) null) && this.TryPrioritizeGameObject(gameObject, selectedPriority))
              ++num;
          }
        }
        else if (this.TryPrioritizeGameObject(target, selectedPriority))
          ++num;
      }
    }
    if (num <= 0)
      return;
    PriorityScreen.PlayPriorityConfirmSound(selectedPriority);
  }

  protected override void OnActivateTool()
  {
    base.OnActivateTool();
    ToolMenu.Instance.PriorityScreen.ShowDiagram(true);
    ToolMenu.Instance.PriorityScreen.Show(true);
    ((KMonoBehaviour) ToolMenu.Instance.PriorityScreen).transform.localScale = new Vector3(1.35f, 1.35f, 1.35f);
  }

  protected override void OnDeactivateTool(InterfaceTool new_tool)
  {
    base.OnDeactivateTool(new_tool);
    ToolMenu.Instance.PriorityScreen.Show(false);
    ToolMenu.Instance.PriorityScreen.ShowDiagram(false);
    ((KMonoBehaviour) ToolMenu.Instance.PriorityScreen).transform.localScale = new Vector3(1f, 1f, 1f);
  }

  public void Update()
  {
    PrioritySetting selectedPriority = ToolMenu.Instance.PriorityScreen.GetLastSelectedPriority();
    int num = 0;
    if (selectedPriority.priority_class >= PriorityScreen.PriorityClass.high)
      num += 9;
    if (selectedPriority.priority_class >= PriorityScreen.PriorityClass.topPriority)
      num = num;
    Texture2D cursor = this.cursors[num + selectedPriority.priority_value - 1];
    MeshRenderer componentInChildren = this.visualizer.GetComponentInChildren<MeshRenderer>();
    if (!Object.op_Inequality((Object) componentInChildren, (Object) null))
      return;
    ((Renderer) componentInChildren).material.mainTexture = (Texture) cursor;
  }
}
