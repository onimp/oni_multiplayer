// Decompiled with JetBrains decompiler
// Type: DragTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragTool : InterfaceTool
{
  [SerializeField]
  private Texture2D boxCursor;
  [SerializeField]
  private GameObject areaVisualizer;
  [SerializeField]
  private GameObject areaVisualizerTextPrefab;
  [SerializeField]
  private Color32 areaColour = Color32.op_Implicit(new Color(1f, 1f, 1f, 0.5f));
  protected SpriteRenderer areaVisualizerSpriteRenderer;
  protected Guid areaVisualizerText;
  protected Vector3 placementPivot;
  protected bool interceptNumberKeysForPriority;
  private bool dragging;
  private Vector3 previousCursorPos;
  private DragTool.Mode mode = DragTool.Mode.Box;
  private DragTool.DragAxis dragAxis = DragTool.DragAxis.Invalid;
  protected bool canChangeDragAxis = true;
  protected Vector3 downPos;
  private VirtualInputModule currentVirtualInputInUse;

  public bool Dragging => this.dragging;

  protected virtual DragTool.Mode GetMode() => this.mode;

  protected override void OnActivateTool()
  {
    base.OnActivateTool();
    this.dragging = false;
    this.SetMode(this.mode);
  }

  protected override void OnDeactivateTool(InterfaceTool new_tool)
  {
    KScreenManager.Instance.SetEventSystemEnabled(true);
    if (KInputManager.currentControllerIsGamepad)
      this.SetCurrentVirtualInputModuleMousMovementMode(false);
    this.RemoveCurrentAreaText();
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
    this.areaVisualizerSpriteRenderer = this.areaVisualizer.GetComponent<SpriteRenderer>();
    this.areaVisualizer.transform.SetParent(this.transform);
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
    cursor_pos = this.ClampPositionToWorld(cursor_pos, ClusterManager.Instance.activeWorld);
    this.dragging = true;
    this.downPos = cursor_pos;
    this.previousCursorPos = cursor_pos;
    if (Object.op_Inequality((Object) this.currentVirtualInputInUse, (Object) null))
    {
      this.currentVirtualInputInUse.mouseMovementOnly = false;
      this.currentVirtualInputInUse = (VirtualInputModule) null;
    }
    if (!KInputManager.currentControllerIsGamepad)
    {
      KScreenManager.Instance.SetEventSystemEnabled(false);
    }
    else
    {
      EventSystem current = EventSystem.current;
      this.SetCurrentVirtualInputModuleMousMovementMode(true, (Action<VirtualInputModule>) (module => this.currentVirtualInputInUse = module));
    }
    this.hasFocus = true;
    this.RemoveCurrentAreaText();
    if (Object.op_Inequality((Object) this.areaVisualizerTextPrefab, (Object) null))
    {
      this.areaVisualizerText = NameDisplayScreen.Instance.AddWorldText("", this.areaVisualizerTextPrefab);
      ((Graphic) NameDisplayScreen.Instance.GetWorldText(this.areaVisualizerText).GetComponent<LocText>()).color = Color32.op_Implicit(this.areaColour);
    }
    switch (this.GetMode())
    {
      case DragTool.Mode.Brush:
        if (!Object.op_Inequality((Object) this.visualizer, (Object) null))
          break;
        this.AddDragPoint(cursor_pos);
        break;
      case DragTool.Mode.Box:
        if (Object.op_Inequality((Object) this.visualizer, (Object) null))
          this.visualizer.SetActive(false);
        if (!Object.op_Inequality((Object) this.areaVisualizer, (Object) null))
          break;
        this.areaVisualizer.SetActive(true);
        TransformExtensions.SetPosition(this.areaVisualizer.transform, cursor_pos);
        this.areaVisualizerSpriteRenderer.size = new Vector2(0.01f, 0.01f);
        break;
    }
  }

  public void RemoveCurrentAreaText()
  {
    if (!(this.areaVisualizerText != Guid.Empty))
      return;
    NameDisplayScreen.Instance.RemoveWorldText(this.areaVisualizerText);
    this.areaVisualizerText = Guid.Empty;
  }

  public void CancelDragging()
  {
    KScreenManager.Instance.SetEventSystemEnabled(true);
    if (Object.op_Inequality((Object) this.currentVirtualInputInUse, (Object) null))
    {
      this.currentVirtualInputInUse.mouseMovementOnly = false;
      this.currentVirtualInputInUse = (VirtualInputModule) null;
    }
    if (KInputManager.currentControllerIsGamepad)
      this.SetCurrentVirtualInputModuleMousMovementMode(false);
    this.dragAxis = DragTool.DragAxis.Invalid;
    if (!this.dragging)
      return;
    this.dragging = false;
    this.RemoveCurrentAreaText();
    if (this.GetMode() != DragTool.Mode.Box || !Object.op_Inequality((Object) this.areaVisualizer, (Object) null))
      return;
    this.areaVisualizer.SetActive(false);
  }

  public override void OnLeftClickUp(Vector3 cursor_pos)
  {
    KScreenManager.Instance.SetEventSystemEnabled(true);
    if (Object.op_Inequality((Object) this.currentVirtualInputInUse, (Object) null))
    {
      this.currentVirtualInputInUse.mouseMovementOnly = false;
      this.currentVirtualInputInUse = (VirtualInputModule) null;
    }
    if (KInputManager.currentControllerIsGamepad)
      this.SetCurrentVirtualInputModuleMousMovementMode(false);
    this.dragAxis = DragTool.DragAxis.Invalid;
    if (!this.dragging)
      return;
    this.dragging = false;
    cursor_pos = this.ClampPositionToWorld(cursor_pos, ClusterManager.Instance.activeWorld);
    this.RemoveCurrentAreaText();
    if (this.GetMode() != DragTool.Mode.Box || !Object.op_Inequality((Object) this.areaVisualizer, (Object) null))
      return;
    this.areaVisualizer.SetActive(false);
    int x1;
    int y1;
    Grid.PosToXY(this.downPos, out x1, out y1);
    int num1 = x1;
    int num2 = y1;
    int x2;
    int y2;
    Grid.PosToXY(cursor_pos, out x2, out y2);
    if (x2 < x1)
      Util.Swap<int>(ref x1, ref x2);
    if (y2 < y1)
      Util.Swap<int>(ref y1, ref y2);
    for (int y3 = y1; y3 <= y2; ++y3)
    {
      for (int x3 = x1; x3 <= x2; ++x3)
      {
        int cell = Grid.XYToCell(x3, y3);
        if (Grid.IsValidCell(cell) && Grid.IsVisible(cell))
        {
          int num3 = y3 - num2;
          int num4 = x3 - num1;
          int num5 = Mathf.Abs(num3);
          int num6 = Mathf.Abs(num4);
          this.OnDragTool(cell, num5 + num6);
        }
      }
    }
    KMonoBehaviour.PlaySound(GlobalAssets.GetSound(this.GetConfirmSound()));
    this.OnDragComplete(this.downPos, cursor_pos);
  }

  protected virtual string GetConfirmSound() => "Tile_Confirm";

  protected virtual string GetDragSound() => "Tile_Drag";

  public override string GetDeactivateSound() => "Tile_Cancel";

  protected Vector3 ClampPositionToWorld(Vector3 position, WorldContainer world)
  {
    position.x = Mathf.Clamp(position.x, world.minimumBounds.x, world.maximumBounds.x);
    position.y = Mathf.Clamp(position.y, world.minimumBounds.y, world.maximumBounds.y);
    return position;
  }

  public override void OnMouseMove(Vector3 cursorPos)
  {
    cursorPos = this.ClampPositionToWorld(cursorPos, ClusterManager.Instance.activeWorld);
    if (this.dragging)
    {
      if (Input.GetKey((KeyCode) ((KInputManager) Global.GetInputManager()).GetDefaultController().GetInputForAction((Action) 14)))
      {
        Vector3 vector3 = Vector3.op_Subtraction(cursorPos, this.downPos);
        if ((this.canChangeDragAxis || this.dragAxis == DragTool.DragAxis.Invalid) && (double) ((Vector3) ref vector3).sqrMagnitude > 0.7070000171661377)
          this.dragAxis = (double) Mathf.Abs(vector3.x) >= (double) Mathf.Abs(vector3.y) ? DragTool.DragAxis.Horizontal : DragTool.DragAxis.Vertical;
      }
      else
        this.dragAxis = DragTool.DragAxis.Invalid;
      switch (this.dragAxis)
      {
        case DragTool.DragAxis.Horizontal:
          cursorPos.y = this.downPos.y;
          break;
        case DragTool.DragAxis.Vertical:
          cursorPos.x = this.downPos.x;
          break;
      }
    }
    base.OnMouseMove(cursorPos);
    if (!this.dragging)
      return;
    switch (this.GetMode())
    {
      case DragTool.Mode.Brush:
        this.AddDragPoints(cursorPos, this.previousCursorPos);
        if (this.areaVisualizerText != Guid.Empty)
        {
          int dragLength = this.GetDragLength();
          LocText component = NameDisplayScreen.Instance.GetWorldText(this.areaVisualizerText).GetComponent<LocText>();
          ((TMP_Text) component).text = string.Format((string) STRINGS.UI.TOOLS.TOOL_LENGTH_FMT, (object) dragLength);
          TransformExtensions.SetPosition(((TMP_Text) component).transform, Vector3.op_Addition(Grid.CellToPos(Grid.PosToCell(cursorPos)), new Vector3(0.0f, 1f, 0.0f)));
          break;
        }
        break;
      case DragTool.Mode.Box:
        Vector2 input1 = Vector2.op_Implicit(Vector3.Max(this.downPos, cursorPos));
        Vector2 input2 = Vector2.op_Implicit(Vector3.Min(this.downPos, cursorPos));
        Vector2 restrictedPosition1 = this.GetWorldRestrictedPosition(input1);
        Vector2 restrictedPosition2 = this.GetWorldRestrictedPosition(input2);
        Vector2 regularizedPos1 = this.GetRegularizedPos(restrictedPosition1, false);
        Vector2 regularizedPos2 = this.GetRegularizedPos(restrictedPosition2, true);
        Vector2 vector2_1 = Vector2.op_Subtraction(regularizedPos1, regularizedPos2);
        Vector2 vector2_2 = Vector2.op_Multiply(Vector2.op_Addition(regularizedPos1, regularizedPos2), 0.5f);
        TransformExtensions.SetPosition(this.areaVisualizer.transform, Vector2.op_Implicit(new Vector2(vector2_2.x, vector2_2.y)));
        int num = (int) ((double) regularizedPos1.x - (double) regularizedPos2.x + ((double) regularizedPos1.y - (double) regularizedPos2.y) - 1.0);
        if (Vector2.op_Inequality(this.areaVisualizerSpriteRenderer.size, vector2_1))
        {
          string sound = GlobalAssets.GetSound(this.GetDragSound());
          if (sound != null)
          {
            Vector3 position = TransformExtensions.GetPosition(this.areaVisualizer.transform);
            position.z = 0.0f;
            EventInstance instance = SoundEvent.BeginOneShot(sound, position);
            ((EventInstance) ref instance).setParameterByName("tileCount", (float) num, false);
            SoundEvent.EndOneShot(instance);
          }
        }
        this.areaVisualizerSpriteRenderer.size = vector2_1;
        if (this.areaVisualizerText != Guid.Empty)
        {
          Vector2I vector2I;
          // ISSUE: explicit constructor call
          ((Vector2I) ref vector2I).\u002Ector(Mathf.RoundToInt(vector2_1.x), Mathf.RoundToInt(vector2_1.y));
          LocText component = NameDisplayScreen.Instance.GetWorldText(this.areaVisualizerText).GetComponent<LocText>();
          ((TMP_Text) component).text = string.Format((string) STRINGS.UI.TOOLS.TOOL_AREA_FMT, (object) vector2I.x, (object) vector2I.y, (object) (vector2I.x * vector2I.y));
          TransformExtensions.SetPosition(((TMP_Text) component).transform, Vector2.op_Implicit(vector2_2));
          break;
        }
        break;
    }
    this.previousCursorPos = cursorPos;
  }

  protected virtual void OnDragTool(int cell, int distFromOrigin)
  {
  }

  protected virtual void OnDragComplete(Vector3 cursorDown, Vector3 cursorUp)
  {
  }

  protected virtual int GetDragLength() => 0;

  private void AddDragPoint(Vector3 cursorPos)
  {
    cursorPos = this.ClampPositionToWorld(cursorPos, ClusterManager.Instance.activeWorld);
    int cell = Grid.PosToCell(cursorPos);
    if (!Grid.IsValidCell(cell) || !Grid.IsVisible(cell))
      return;
    this.OnDragTool(cell, 0);
  }

  private void AddDragPoints(Vector3 cursorPos, Vector3 previousCursorPos)
  {
    cursorPos = this.ClampPositionToWorld(cursorPos, ClusterManager.Instance.activeWorld);
    Vector3 vector3 = Vector3.op_Subtraction(cursorPos, previousCursorPos);
    float magnitude = ((Vector3) ref vector3).magnitude;
    float num1 = Grid.CellSizeInMeters * 0.25f;
    int num2 = 1 + (int) ((double) magnitude / (double) num1);
    ((Vector3) ref vector3).Normalize();
    for (int index = 0; index < num2; ++index)
      this.AddDragPoint(Vector3.op_Addition(previousCursorPos, Vector3.op_Multiply(vector3, (float) index * num1)));
  }

  public override void OnKeyDown(KButtonEvent e)
  {
    if (this.interceptNumberKeysForPriority)
      this.HandlePriortyKeysDown(e);
    if (((KInputEvent) e).Consumed)
      return;
    base.OnKeyDown(e);
  }

  public override void OnKeyUp(KButtonEvent e)
  {
    if (this.interceptNumberKeysForPriority)
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

  protected void SetMode(DragTool.Mode newMode)
  {
    this.mode = newMode;
    switch (this.mode)
    {
      case DragTool.Mode.Brush:
        if (Object.op_Inequality((Object) this.areaVisualizer, (Object) null))
          this.areaVisualizer.SetActive(false);
        if (Object.op_Inequality((Object) this.visualizer, (Object) null))
          this.visualizer.SetActive(true);
        this.SetCursor(this.cursor, this.cursorOffset, (CursorMode) 0);
        break;
      case DragTool.Mode.Box:
        if (Object.op_Inequality((Object) this.visualizer, (Object) null))
          this.visualizer.SetActive(true);
        this.mode = DragTool.Mode.Box;
        this.SetCursor(this.boxCursor, this.cursorOffset, (CursorMode) 0);
        break;
    }
  }

  public override void OnFocus(bool focus)
  {
    switch (this.GetMode())
    {
      case DragTool.Mode.Brush:
        if (Object.op_Inequality((Object) this.visualizer, (Object) null))
          this.visualizer.SetActive(focus);
        this.hasFocus = focus;
        break;
      case DragTool.Mode.Box:
        if (Object.op_Inequality((Object) this.visualizer, (Object) null) && !this.dragging)
          this.visualizer.SetActive(focus);
        this.hasFocus = focus || this.dragging;
        break;
    }
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

  public enum Mode
  {
    Brush,
    Box,
  }
}
