// Decompiled with JetBrains decompiler
// Type: SandboxCritterTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class SandboxCritterTool : BrushTool
{
  public static SandboxCritterTool instance;

  public static void DestroyInstance() => SandboxCritterTool.instance = (SandboxCritterTool) null;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    SandboxCritterTool.instance = this;
  }

  public void Activate() => PlayerController.Instance.ActivateTool((InterfaceTool) this);

  protected override void OnActivateTool()
  {
    base.OnActivateTool();
    ((Component) SandboxToolParameterMenu.instance).gameObject.SetActive(true);
    SandboxToolParameterMenu.instance.DisableParameters();
    SandboxToolParameterMenu.instance.brushRadiusSlider.SetValue(6f);
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
    HashSetPool<GameObject, SandboxCritterTool>.PooledHashSet pooledHashSet = HashSetPool<GameObject, SandboxCritterTool>.Allocate();
    foreach (Health cmp in Components.Health.Items)
    {
      if (Grid.PosToCell((KMonoBehaviour) cmp) == cell && ((Component) cmp).GetComponent<KPrefabID>().HasTag(GameTags.Creature))
        ((HashSet<GameObject>) pooledHashSet).Add(((Component) cmp).gameObject);
    }
    foreach (GameObject gameObject in (HashSet<GameObject>) pooledHashSet)
      Util.KDestroyGameObject(gameObject);
    pooledHashSet.Recycle();
  }
}
