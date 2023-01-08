// Decompiled with JetBrains decompiler
// Type: PlayerController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.Input;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("KMonoBehaviour/scripts/PlayerController")]
public class PlayerController : KMonoBehaviour, IInputHandler
{
  [SerializeField]
  private Action defaultConfigKey;
  [SerializeField]
  private List<InterfaceToolConfig> interfaceConfigs;
  public InterfaceTool[] tools;
  private InterfaceTool activeTool;
  public VirtualInputModule vim;
  private bool DebugHidingCursor;
  private Vector3 prevMousePos = new Vector3(float.PositiveInfinity, 0.0f, 0.0f);
  private const float MIN_DRAG_DIST_SQR = 36f;
  private const float MIN_DRAG_TIME = 0.3f;
  private Action dragAction;
  private bool draggingAllowed = true;
  private bool dragging;
  private bool queueStopDrag;
  private Vector3 startDragPos;
  private float startDragTime;
  private Vector3 dragDelta;
  private Vector3 worldDragDelta;

  public string handlerName => nameof (PlayerController);

  public KInputHandler inputHandler { get; set; }

  public InterfaceTool ActiveTool => this.activeTool;

  public static PlayerController Instance { get; private set; }

  public static void DestroyInstance() => PlayerController.Instance = (PlayerController) null;

  protected virtual void OnPrefabInit()
  {
    PlayerController.Instance = this;
    InterfaceTool.InitializeConfigs(this.defaultConfigKey, this.interfaceConfigs);
    this.vim = Object.FindObjectOfType<VirtualInputModule>(true);
    for (int index = 0; index < this.tools.Length; ++index)
    {
      if (DlcManager.IsDlcListValidForCurrentContent(this.tools[index].DlcIDs))
      {
        GameObject gameObject = Util.KInstantiate(((Component) this.tools[index]).gameObject, ((Component) this).gameObject, (string) null);
        this.tools[index] = gameObject.GetComponent<InterfaceTool>();
        ((Component) this.tools[index]).gameObject.SetActive(true);
        ((Component) this.tools[index]).gameObject.SetActive(false);
      }
    }
  }

  protected virtual void OnSpawn()
  {
    if (this.tools.Length == 0)
      return;
    this.ActivateTool(this.tools[0]);
  }

  private void InitializeConfigs()
  {
  }

  private Vector3 GetCursorPos() => PlayerController.GetCursorPos(KInputManager.GetMousePos());

  public static Vector3 GetCursorPos(Vector3 mouse_pos)
  {
    RaycastHit raycastHit;
    Vector3 cursorPos;
    if (Physics.Raycast(Camera.main.ScreenPointToRay(mouse_pos), ref raycastHit, float.PositiveInfinity, Game.BlockSelectionLayerMask))
    {
      cursorPos = ((RaycastHit) ref raycastHit).point;
    }
    else
    {
      mouse_pos.z = -TransformExtensions.GetPosition(((Component) Camera.main).transform).z - Grid.CellSizeInMeters;
      cursorPos = Camera.main.ScreenToWorldPoint(mouse_pos);
    }
    float x = cursorPos.x;
    float y = cursorPos.y;
    float num1 = Mathf.Min(Mathf.Max(x, 0.0f), Grid.WidthInMeters);
    float num2 = Mathf.Min(Mathf.Max(y, 0.0f), Grid.HeightInMeters);
    cursorPos.x = num1;
    cursorPos.y = num2;
    return cursorPos;
  }

  private void UpdateHover()
  {
    EventSystem current = EventSystem.current;
    if (!Object.op_Inequality((Object) current, (Object) null))
      return;
    this.activeTool.OnFocus(!current.IsPointerOverGameObject());
  }

  private void Update()
  {
    this.UpdateDrag();
    if (Object.op_Implicit((Object) this.activeTool) && ((Behaviour) this.activeTool).enabled)
    {
      this.UpdateHover();
      Vector3 cursorPos = this.GetCursorPos();
      if (Vector3.op_Inequality(cursorPos, this.prevMousePos))
      {
        this.prevMousePos = cursorPos;
        this.activeTool.OnMouseMove(cursorPos);
      }
    }
    if (!UnityEngine.Input.GetKeyDown((KeyCode) 293) || !UnityEngine.Input.GetKey((KeyCode) 308) && !UnityEngine.Input.GetKey((KeyCode) 307))
      return;
    this.DebugHidingCursor = !this.DebugHidingCursor;
    Cursor.visible = !this.DebugHidingCursor;
    HoverTextScreen.Instance.Show(!this.DebugHidingCursor);
  }

  private void OnCleanup() => Global.GetInputManager().usedMenus.Remove((IInputHandler) this);

  private void LateUpdate()
  {
    if (!this.queueStopDrag)
      return;
    this.queueStopDrag = false;
    this.dragging = false;
    this.dragAction = (Action) 0;
    this.dragDelta = Vector3.zero;
    this.worldDragDelta = Vector3.zero;
  }

  public void ActivateTool(InterfaceTool tool)
  {
    if (Object.op_Equality((Object) this.activeTool, (Object) tool))
      return;
    this.DeactivateTool(tool);
    this.activeTool = tool;
    ((Behaviour) this.activeTool).enabled = true;
    ((Component) this.activeTool).gameObject.SetActive(true);
    this.activeTool.ActivateTool();
    this.UpdateHover();
  }

  public void ToolDeactivated(InterfaceTool tool)
  {
    if (Object.op_Equality((Object) this.activeTool, (Object) tool) && Object.op_Inequality((Object) this.activeTool, (Object) null))
      this.DeactivateTool();
    if (!Object.op_Equality((Object) this.activeTool, (Object) null))
      return;
    this.ActivateTool((InterfaceTool) SelectTool.Instance);
  }

  private void DeactivateTool(InterfaceTool new_tool = null)
  {
    if (!Object.op_Inequality((Object) this.activeTool, (Object) null))
      return;
    ((Behaviour) this.activeTool).enabled = false;
    ((Component) this.activeTool).gameObject.SetActive(false);
    InterfaceTool activeTool = this.activeTool;
    this.activeTool = (InterfaceTool) null;
    InterfaceTool new_tool1 = new_tool;
    activeTool.DeactivateTool(new_tool1);
  }

  public bool IsUsingDefaultTool() => this.tools.Length != 0 && Object.op_Equality((Object) this.activeTool, (Object) this.tools[0]);

  private void StartDrag(Action action)
  {
    if (this.dragAction != null)
      return;
    this.dragAction = action;
    this.startDragPos = KInputManager.GetMousePos();
    this.startDragTime = Time.unscaledTime;
  }

  private void UpdateDrag()
  {
    this.dragDelta = Vector2.op_Implicit(Vector2.zero);
    Vector3 mousePos = KInputManager.GetMousePos();
    if (!this.dragging && this.CanDrag())
    {
      Vector3 vector3 = Vector3.op_Subtraction(mousePos, this.startDragPos);
      if ((double) ((Vector3) ref vector3).sqrMagnitude > 36.0 || (double) Time.unscaledTime - (double) this.startDragTime > 0.30000001192092896)
        this.dragging = true;
    }
    if (DistributionPlatform.Initialized && KInputManager.currentControllerIsGamepad && this.dragging || !this.dragging)
      return;
    this.dragDelta = Vector3.op_Subtraction(mousePos, this.startDragPos);
    this.worldDragDelta = Vector3.op_Subtraction(Camera.main.ScreenToWorldPoint(mousePos), Camera.main.ScreenToWorldPoint(this.startDragPos));
    this.startDragPos = mousePos;
  }

  private void StopDrag(Action action)
  {
    if (this.dragAction != action)
      return;
    this.queueStopDrag = true;
    if (!KInputManager.currentControllerIsGamepad)
      return;
    this.dragging = false;
  }

  public void CancelDragging()
  {
    this.queueStopDrag = true;
    if (!Object.op_Inequality((Object) this.activeTool, (Object) null))
      return;
    DragTool activeTool = this.activeTool as DragTool;
    if (!Object.op_Inequality((Object) activeTool, (Object) null))
      return;
    activeTool.CancelDragging();
  }

  public void OnCancelInput() => this.CancelDragging();

  public void OnKeyDown(KButtonEvent e)
  {
    if (e.TryConsume((Action) 168))
      DebugHandler.ToggleScreenshotMode();
    else if (DebugHandler.HideUI && e.TryConsume((Action) 1))
    {
      DebugHandler.ToggleScreenshotMode();
    }
    else
    {
      bool flag = true;
      if (e.IsAction((Action) 3) || e.IsAction((Action) 4))
        this.StartDrag((Action) 3);
      else if (e.IsAction((Action) 5))
        this.StartDrag((Action) 5);
      else if (e.IsAction((Action) 6))
        this.StartDrag((Action) 6);
      else
        flag = false;
      if (Object.op_Equality((Object) this.activeTool, (Object) null) || !((Behaviour) this.activeTool).enabled)
        return;
      List<RaycastResult> raycastResultList = new List<RaycastResult>();
      PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
      pointerEventData.position = Vector2.op_Implicit(KInputManager.GetMousePos());
      EventSystem current = EventSystem.current;
      if (Object.op_Inequality((Object) current, (Object) null))
      {
        current.RaycastAll(pointerEventData, raycastResultList);
        if (raycastResultList.Count > 0)
          return;
      }
      if (flag && !this.draggingAllowed)
        e.TryConsume(e.GetAction());
      else if (e.TryConsume((Action) 3) || e.TryConsume((Action) 4))
        this.activeTool.OnLeftClickDown(this.GetCursorPos());
      else if (e.IsAction((Action) 5))
        this.activeTool.OnRightClickDown(this.GetCursorPos(), e);
      else
        this.activeTool.OnKeyDown(e);
    }
  }

  public void OnKeyUp(KButtonEvent e)
  {
    bool flag = true;
    if (e.IsAction((Action) 3) || e.IsAction((Action) 4))
      this.StopDrag((Action) 3);
    else if (e.IsAction((Action) 5))
      this.StopDrag((Action) 5);
    else if (e.IsAction((Action) 6))
      this.StopDrag((Action) 6);
    else
      flag = false;
    if (Object.op_Equality((Object) this.activeTool, (Object) null) || !((Behaviour) this.activeTool).enabled || !this.activeTool.hasFocus)
      return;
    if (flag && !this.draggingAllowed)
      e.TryConsume(e.GetAction());
    else if (!KInputManager.currentControllerIsGamepad)
    {
      if (e.TryConsume((Action) 3) || e.TryConsume((Action) 4))
        this.activeTool.OnLeftClickUp(this.GetCursorPos());
      else if (e.IsAction((Action) 5))
        this.activeTool.OnRightClickUp(this.GetCursorPos());
      else
        this.activeTool.OnKeyUp(e);
    }
    else if (e.IsAction((Action) 3) || e.IsAction((Action) 4))
      this.activeTool.OnLeftClickUp(this.GetCursorPos());
    else if (e.IsAction((Action) 5))
      this.activeTool.OnRightClickUp(this.GetCursorPos());
    else
      this.activeTool.OnKeyUp(e);
  }

  public bool ConsumeIfNotDragging(KButtonEvent e, Action action) => (this.dragAction != action || !this.dragging) && e.TryConsume(action);

  public bool IsDragging() => this.dragging && this.CanDrag();

  public bool CanDrag() => this.draggingAllowed && this.dragAction > 0;

  public void AllowDragging(bool allow) => this.draggingAllowed = allow;

  public Vector3 GetDragDelta() => this.dragDelta;

  public Vector3 GetWorldDragDelta() => !this.draggingAllowed ? Vector3.zero : this.worldDragDelta;
}
