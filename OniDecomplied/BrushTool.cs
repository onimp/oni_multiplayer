// Decompiled with JetBrains decompiler
// Type: BrushTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class BrushTool : InterfaceTool
{
  [SerializeField]
  private Texture2D brushCursor;
  [SerializeField]
  private GameObject areaVisualizer;
  [SerializeField]
  private Color32 areaColour = Color32.op_Implicit(new Color(1f, 1f, 1f, 0.5f));
  protected Color radiusIndicatorColor = new Color(0.5f, 0.7f, 0.5f, 0.2f);
  protected Vector3 placementPivot;
  protected bool interceptNumberKeysForPriority;
  protected List<Vector2> brushOffsets = new List<Vector2>();
  protected bool affectFoundation;
  private bool dragging;
  protected int brushRadius = -1;
  private BrushTool.DragAxis dragAxis = BrushTool.DragAxis.Invalid;
  protected Vector3 downPos;
  protected int currentCell;
  protected HashSet<int> cellsInRadius = new HashSet<int>();

  public bool Dragging => this.dragging;

  protected override void OnActivateTool()
  {
    base.OnActivateTool();
    this.dragging = false;
  }

  public override void GetOverlayColorData(out HashSet<ToolMenu.CellColorData> colors)
  {
    colors = new HashSet<ToolMenu.CellColorData>();
    foreach (int cellsInRadiu in this.cellsInRadius)
      colors.Add(new ToolMenu.CellColorData(cellsInRadiu, this.radiusIndicatorColor));
  }

  public virtual void SetBrushSize(int radius)
  {
    if (radius == this.brushRadius)
      return;
    this.brushRadius = radius;
    this.brushOffsets.Clear();
    for (int index1 = 0; index1 < this.brushRadius * 2; ++index1)
    {
      for (int index2 = 0; index2 < this.brushRadius * 2; ++index2)
      {
        if ((double) Vector2.Distance(new Vector2((float) index1, (float) index2), new Vector2((float) this.brushRadius, (float) this.brushRadius)) < (double) this.brushRadius - 0.800000011920929)
          this.brushOffsets.Add(new Vector2((float) (index1 - this.brushRadius), (float) (index2 - this.brushRadius)));
      }
    }
  }

  protected override void OnDeactivateTool(InterfaceTool new_tool)
  {
    KScreenManager.Instance.SetEventSystemEnabled(true);
    if (KInputManager.currentControllerIsGamepad)
      this.SetCurrentVirtualInputModuleMousMovementMode(false);
    base.OnDeactivateTool(new_tool);
  }

  protected virtual void OnPrefabInit()
  {
    Game.Instance.Subscribe(1634669191, new Action<object>(this.OnTutorialOpened));
    base.OnPrefabInit();
    if (Object.op_Inequality((Object) this.visualizer, (Object) null))
      this.visualizer = Util.KInstantiate(this.visualizer, (GameObject) null, (string) null);
    if (!Object.op_Inequality((Object) this.areaVisualizer, (Object) null))
      return;
    this.areaVisualizer = Util.KInstantiate(this.areaVisualizer, (GameObject) null, (string) null);
    this.areaVisualizer.SetActive(false);
    ((Transform) this.areaVisualizer.GetComponent<RectTransform>()).SetParent(this.transform);
    this.areaVisualizer.GetComponent<Renderer>().material.color = Color32.op_Implicit(this.areaColour);
  }

  protected virtual void OnCmpEnable() => this.dragging = false;

  protected virtual void OnCmpDisable()
  {
    if (Object.op_Inequality((Object) this.visualizer, (Object) null))
      this.visualizer.SetActive(false);
    if (!Object.op_Inequality((Object) this.areaVisualizer, (Object) null))
      return;
    this.areaVisualizer.SetActive(false);
  }

  public override void OnLeftClickDown(Vector3 cursor_pos)
  {
    cursor_pos = Vector3.op_Subtraction(cursor_pos, this.placementPivot);
    this.dragging = true;
    this.downPos = cursor_pos;
    if (!KInputManager.currentControllerIsGamepad)
      KScreenManager.Instance.SetEventSystemEnabled(false);
    else
      this.SetCurrentVirtualInputModuleMousMovementMode(true);
    this.Paint();
  }

  public override void OnLeftClickUp(Vector3 cursor_pos)
  {
    cursor_pos = Vector3.op_Subtraction(cursor_pos, this.placementPivot);
    KScreenManager.Instance.SetEventSystemEnabled(true);
    if (KInputManager.currentControllerIsGamepad)
      this.SetCurrentVirtualInputModuleMousMovementMode(false);
    if (!this.dragging)
      return;
    this.dragging = false;
    switch (this.dragAxis)
    {
      case BrushTool.DragAxis.Horizontal:
        cursor_pos.y = this.downPos.y;
        this.dragAxis = BrushTool.DragAxis.None;
        break;
      case BrushTool.DragAxis.Vertical:
        cursor_pos.x = this.downPos.x;
        this.dragAxis = BrushTool.DragAxis.None;
        break;
    }
  }

  protected virtual string GetConfirmSound() => "Tile_Confirm";

  protected virtual string GetDragSound() => "Tile_Drag";

  public override string GetDeactivateSound() => "Tile_Cancel";

  private static int GetGridDistance(int cell, int center_cell)
  {
    Vector2I vector2I = Vector2I.op_Subtraction(Grid.CellToXY(cell), Grid.CellToXY(center_cell));
    return Math.Abs(vector2I.x) + Math.Abs(vector2I.y);
  }

  private void Paint()
  {
    foreach (int cellsInRadiu in this.cellsInRadius)
    {
      if (Grid.IsValidCell(cellsInRadiu) && (int) Grid.WorldIdx[cellsInRadiu] == ClusterManager.Instance.activeWorldId && (!Grid.Foundation[cellsInRadiu] || this.affectFoundation))
        this.OnPaintCell(cellsInRadiu, Grid.GetCellDistance(this.currentCell, cellsInRadiu));
    }
  }

  public override void OnMouseMove(Vector3 cursorPos)
  {
    this.currentCell = Grid.PosToCell(cursorPos);
    base.OnMouseMove(cursorPos);
    this.cellsInRadius.Clear();
    foreach (Vector2 brushOffset in this.brushOffsets)
    {
      int cell = Grid.OffsetCell(Grid.PosToCell(cursorPos), new CellOffset((int) brushOffset.x, (int) brushOffset.y));
      if (Grid.IsValidCell(cell) && (int) Grid.WorldIdx[cell] == ClusterManager.Instance.activeWorldId)
        this.cellsInRadius.Add(Grid.OffsetCell(Grid.PosToCell(cursorPos), new CellOffset((int) brushOffset.x, (int) brushOffset.y)));
    }
    if (!this.dragging)
      return;
    this.Paint();
  }

  protected virtual void OnPaintCell(int cell, int distFromOrigin)
  {
  }

  public override void OnKeyDown(KButtonEvent e)
  {
    if (e.TryConsume((Action) 14))
      this.dragAxis = BrushTool.DragAxis.None;
    else if (this.interceptNumberKeysForPriority)
      this.HandlePriortyKeysDown(e);
    if (((KInputEvent) e).Consumed)
      return;
    base.OnKeyDown(e);
  }

  public override void OnKeyUp(KButtonEvent e)
  {
    if (e.TryConsume((Action) 14))
      this.dragAxis = BrushTool.DragAxis.Invalid;
    else if (this.interceptNumberKeysForPriority)
      this.HandlePriorityKeysUp(e);
    if (((KInputEvent) e).Consumed)
      return;
    base.OnKeyUp(e);
  }

  private void HandlePriortyKeysDown(KButtonEvent e)
  {
    Action action = e.GetAction();
    if (36 > action || action > 45 || !e.TryConsume(action))
      return;
    int priority_value = action - 36 + 1;
    if (priority_value <= 9)
      ToolMenu.Instance.PriorityScreen.SetScreenPriority(new PrioritySetting(PriorityScreen.PriorityClass.basic, priority_value), true);
    else
      ToolMenu.Instance.PriorityScreen.SetScreenPriority(new PrioritySetting(PriorityScreen.PriorityClass.topPriority, 1), true);
  }

  private void HandlePriorityKeysUp(KButtonEvent e)
  {
    Action action = e.GetAction();
    if (36 > action || action > 45)
      return;
    e.TryConsume(action);
  }

  public override void OnFocus(bool focus)
  {
    if (Object.op_Inequality((Object) this.visualizer, (Object) null))
      this.visualizer.SetActive(focus);
    this.hasFocus = focus;
    base.OnFocus(focus);
  }

  private void OnTutorialOpened(object data) => this.dragging = false;

  public override bool ShowHoverUI() => this.dragging || base.ShowHoverUI();

  public override void LateUpdate() => base.LateUpdate();

  private enum DragAxis
  {
    Invalid = -1, // 0xFFFFFFFF
    None = 0,
    Horizontal = 1,
    Vertical = 2,
  }
}
