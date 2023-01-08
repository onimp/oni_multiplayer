// Decompiled with JetBrains decompiler
// Type: SandboxClearFloorTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SandboxClearFloorTool : BrushTool
{
  public static SandboxClearFloorTool instance;

  public static void DestroyInstance() => SandboxClearFloorTool.instance = (SandboxClearFloorTool) null;

  private SandboxSettings settings => SandboxToolParameterMenu.instance.settings;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    SandboxClearFloorTool.instance = this;
  }

  public void Activate() => PlayerController.Instance.ActivateTool((InterfaceTool) this);

  protected override void OnActivateTool()
  {
    base.OnActivateTool();
    ((Component) SandboxToolParameterMenu.instance).gameObject.SetActive(true);
    SandboxToolParameterMenu.instance.DisableParameters();
    SandboxToolParameterMenu.instance.brushRadiusSlider.row.SetActive(true);
    SandboxToolParameterMenu.instance.brushRadiusSlider.SetValue((float) this.settings.GetIntSetting("SandboxTools.BrushSize"));
  }

  protected override void OnDeactivateTool(InterfaceTool new_tool)
  {
    base.OnDeactivateTool(new_tool);
    ((Component) SandboxToolParameterMenu.instance).gameObject.SetActive(false);
  }

  public override void GetOverlayColorData(out HashSet<ToolMenu.CellColorData> colors)
  {
    colors = new HashSet<ToolMenu.CellColorData>();
    foreach (int cellsInRadiu in this.cellsInRadius)
      colors.Add(new ToolMenu.CellColorData(cellsInRadiu, this.radiusIndicatorColor));
  }

  public override void OnMouseMove(Vector3 cursorPos) => base.OnMouseMove(cursorPos);

  protected override void OnPaintCell(int cell, int distFromOrigin)
  {
    base.OnPaintCell(cell, distFromOrigin);
    bool flag = false;
    foreach (Pickupable pickupable in Components.Pickupables.Items)
    {
      Pickupable pickup = pickupable;
      if (!Object.op_Inequality((Object) pickup.storage, (Object) null) && Grid.PosToCell((KMonoBehaviour) pickup) == cell && Object.op_Equality((Object) Components.LiveMinionIdentities.Items.Find((Predicate<MinionIdentity>) (match => Object.op_Equality((Object) ((Component) match).gameObject, (Object) ((Component) pickup).gameObject))), (Object) null))
      {
        if (!flag)
        {
          UISounds.PlaySound(UISounds.Sound.Negative);
          PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, (string) UI.SANDBOXTOOLS.CLEARFLOOR.DELETED, pickup.transform);
          flag = true;
        }
        Util.KDestroyGameObject(((Component) pickup).gameObject);
      }
    }
  }
}
