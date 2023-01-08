// Decompiled with JetBrains decompiler
// Type: SandboxDestroyerTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class SandboxDestroyerTool : BrushTool
{
  public static SandboxDestroyerTool instance;
  protected HashSet<int> recentlyAffectedCells = new HashSet<int>();
  protected Color recentlyAffectedCellColor = new Color(1f, 1f, 1f, 0.1f);

  public static void DestroyInstance() => SandboxDestroyerTool.instance = (SandboxDestroyerTool) null;

  private SandboxSettings settings => SandboxToolParameterMenu.instance.settings;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    SandboxDestroyerTool.instance = this;
    this.affectFoundation = true;
  }

  public void Activate() => PlayerController.Instance.ActivateTool((InterfaceTool) this);

  protected override void OnActivateTool()
  {
    base.OnActivateTool();
    ((Component) SandboxToolParameterMenu.instance).gameObject.SetActive(true);
    SandboxToolParameterMenu.instance.DisableParameters();
    SandboxToolParameterMenu.instance.brushRadiusSlider.row.SetActive(true);
  }

  protected override void OnDeactivateTool(InterfaceTool new_tool)
  {
    base.OnDeactivateTool(new_tool);
    ((Component) SandboxToolParameterMenu.instance).gameObject.SetActive(false);
  }

  public override void GetOverlayColorData(out HashSet<ToolMenu.CellColorData> colors)
  {
    colors = new HashSet<ToolMenu.CellColorData>();
    foreach (int recentlyAffectedCell in this.recentlyAffectedCells)
      colors.Add(new ToolMenu.CellColorData(recentlyAffectedCell, this.recentlyAffectedCellColor));
    foreach (int cellsInRadiu in this.cellsInRadius)
      colors.Add(new ToolMenu.CellColorData(cellsInRadiu, this.radiusIndicatorColor));
  }

  public override void OnMouseMove(Vector3 cursorPos) => base.OnMouseMove(cursorPos);

  protected override void OnPaintCell(int cell, int distFromOrigin)
  {
    base.OnPaintCell(cell, distFromOrigin);
    this.recentlyAffectedCells.Add(cell);
    int index = Game.Instance.callbackManager.Add(new Game.CallbackInfo((System.Action) (() => this.recentlyAffectedCells.Remove(cell)))).index;
    SimMessages.ReplaceElement(cell, SimHashes.Vacuum, CellEventLogger.Instance.SandBoxTool, 0.0f, 0.0f, callbackIdx: index);
    HashSetPool<GameObject, SandboxDestroyerTool>.PooledHashSet pooledHashSet = HashSetPool<GameObject, SandboxDestroyerTool>.Allocate();
    foreach (Pickupable cmp in Components.Pickupables.Items)
    {
      if (Grid.PosToCell((KMonoBehaviour) cmp) == cell)
        ((HashSet<GameObject>) pooledHashSet).Add(((Component) cmp).gameObject);
    }
    foreach (BuildingComplete cmp in Components.BuildingCompletes.Items)
    {
      if (Grid.PosToCell((KMonoBehaviour) cmp) == cell)
        ((HashSet<GameObject>) pooledHashSet).Add(((Component) cmp).gameObject);
    }
    if (Object.op_Inequality((Object) Grid.Objects[cell, 1], (Object) null))
      ((HashSet<GameObject>) pooledHashSet).Add(Grid.Objects[cell, 1]);
    foreach (Crop cmp in Components.Crops.Items)
    {
      if (Grid.PosToCell((KMonoBehaviour) cmp) == cell)
        ((HashSet<GameObject>) pooledHashSet).Add(((Component) cmp).gameObject);
    }
    foreach (Health cmp in Components.Health.Items)
    {
      if (Grid.PosToCell((KMonoBehaviour) cmp) == cell)
        ((HashSet<GameObject>) pooledHashSet).Add(((Component) cmp).gameObject);
    }
    foreach (GameObject gameObject in (HashSet<GameObject>) pooledHashSet)
      Util.KDestroyGameObject(gameObject);
    pooledHashSet.Recycle();
  }
}
