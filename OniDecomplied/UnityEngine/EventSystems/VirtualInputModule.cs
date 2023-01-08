// Decompiled with JetBrains decompiler
// Type: UnityEngine.EventSystems.VirtualInputModule
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace UnityEngine.EventSystems
{
  [AddComponentMenu("Event/Virtual Input Module")]
  public class VirtualInputModule : PointerInputModule, IInputHandler
  {
    private float m_PrevActionTime;
    private Vector2 m_LastMoveVector;
    private int m_ConsecutiveMoveCount;
    private string debugName;
    private Vector2 m_LastMousePosition;
    private Vector2 m_MousePosition;
    public bool mouseMovementOnly;
    [SerializeField]
    private RectTransform m_VirtualCursor;
    [SerializeField]
    private float m_VirtualCursorSpeed = 1f;
    [SerializeField]
    private Vector2 m_VirtualCursorOffset = Vector2.zero;
    [SerializeField]
    private Camera m_canvasCamera;
    private Camera VCcam;
    public bool CursorCanvasShouldBeOverlay;
    private Canvas m_virtualCursorCanvas;
    private CanvasScaler m_virtualCursorScaler;
    private PointerEventData leftClickData;
    private PointerEventData rightClickData;
    private VirtualInputModule.ControllerButtonStates conButtonStates;
    private GameObject m_CurrentFocusedGameObject;
    private bool leftReleased;
    private bool rightReleased;
    private bool leftFirstClick;
    private bool rightFirstClick;
    [SerializeField]
    private string m_HorizontalAxis = "Horizontal";
    [SerializeField]
    private string m_VerticalAxis = "Vertical";
    [SerializeField]
    private string m_SubmitButton = "Submit";
    [SerializeField]
    private string m_CancelButton = "Cancel";
    [SerializeField]
    private float m_InputActionsPerSecond = 10f;
    [SerializeField]
    private float m_RepeatDelay = 0.5f;
    [SerializeField]
    [FormerlySerializedAs("m_AllowActivationOnMobileDevice")]
    private bool m_ForceModuleActive;
    private readonly PointerInputModule.MouseState m_MouseState = new PointerInputModule.MouseState();

    public string handlerName => "VirtualCursorInput";

    public KInputHandler inputHandler { get; set; }

    protected VirtualInputModule()
    {
    }

    [Obsolete("Mode is no longer needed on input module as it handles both mouse and keyboard simultaneously.", false)]
    public VirtualInputModule.InputMode inputMode => VirtualInputModule.InputMode.Mouse;

    [Obsolete("allowActivationOnMobileDevice has been deprecated. Use forceModuleActive instead (UnityUpgradable) -> forceModuleActive")]
    public bool allowActivationOnMobileDevice
    {
      get => this.m_ForceModuleActive;
      set => this.m_ForceModuleActive = value;
    }

    public bool forceModuleActive
    {
      get => this.m_ForceModuleActive;
      set => this.m_ForceModuleActive = value;
    }

    public float inputActionsPerSecond
    {
      get => this.m_InputActionsPerSecond;
      set => this.m_InputActionsPerSecond = value;
    }

    public float repeatDelay
    {
      get => this.m_RepeatDelay;
      set => this.m_RepeatDelay = value;
    }

    public string horizontalAxis
    {
      get => this.m_HorizontalAxis;
      set => this.m_HorizontalAxis = value;
    }

    public string verticalAxis
    {
      get => this.m_VerticalAxis;
      set => this.m_VerticalAxis = value;
    }

    public string submitButton
    {
      get => this.m_SubmitButton;
      set => this.m_SubmitButton = value;
    }

    public string cancelButton
    {
      get => this.m_CancelButton;
      set => this.m_CancelButton = value;
    }

    public void SetCursor(Texture2D tex)
    {
      ((BaseInputModule) this).UpdateModule();
      if (!Object.op_Implicit((Object) this.m_VirtualCursor))
        return;
      ((Component) this.m_VirtualCursor).GetComponent<RawImage>().texture = (Texture) tex;
    }

    public virtual void UpdateModule()
    {
      GameInputManager inputManager = Global.GetInputManager();
      if (((KInputManager) inputManager).GetControllerCount() <= 1)
        return;
      if (this.inputHandler == null || !this.inputHandler.UsesController((IInputHandler) this, ((KInputManager) inputManager).GetController(1)))
      {
        KInputHandler.Add((IInputHandler) ((KInputManager) inputManager).GetController(1), (IInputHandler) this, int.MaxValue);
        if (!inputManager.usedMenus.Contains((IInputHandler) this))
          inputManager.usedMenus.Add((IInputHandler) this);
        Scene activeScene = SceneManager.GetActiveScene();
        this.debugName = ((Scene) ref activeScene).name + "-VirtualInputModule";
      }
      if (Object.op_Equality((Object) this.m_VirtualCursor, (Object) null))
        this.m_VirtualCursor = GameObject.Find("VirtualCursor").GetComponent<RectTransform>();
      if (Object.op_Equality((Object) this.m_canvasCamera, (Object) null))
      {
        this.m_canvasCamera = ((Component) this).gameObject.AddComponent<Camera>();
        ((Behaviour) this.m_canvasCamera).enabled = false;
      }
      if (Object.op_Inequality((Object) CameraController.Instance, (Object) null))
        this.m_canvasCamera.CopyFrom(CameraController.Instance.overlayCamera);
      else if (this.CursorCanvasShouldBeOverlay)
        this.m_canvasCamera.CopyFrom(GameObject.Find("FrontEndCamera").GetComponent<Camera>());
      if (Object.op_Inequality((Object) this.m_canvasCamera, (Object) null) && Object.op_Equality((Object) this.VCcam, (Object) null))
      {
        this.VCcam = GameObject.Find("VirtualCursorCamera").GetComponent<Camera>();
        if (Object.op_Inequality((Object) this.VCcam, (Object) null))
        {
          if (Object.op_Equality((Object) this.m_virtualCursorCanvas, (Object) null))
          {
            this.m_virtualCursorCanvas = GameObject.Find("VirtualCursorCanvas").GetComponent<Canvas>();
            this.m_virtualCursorScaler = ((Component) this.m_virtualCursorCanvas).GetComponent<CanvasScaler>();
          }
          if (this.CursorCanvasShouldBeOverlay)
          {
            this.m_virtualCursorCanvas.renderMode = (RenderMode) 0;
            this.VCcam.orthographic = false;
          }
          else
          {
            this.VCcam.orthographic = this.m_canvasCamera.orthographic;
            this.VCcam.orthographicSize = this.m_canvasCamera.orthographicSize;
            ((Component) this.VCcam).transform.position = ((Component) this.m_canvasCamera).transform.position;
            ((Behaviour) this.VCcam).enabled = true;
            this.m_virtualCursorCanvas.renderMode = (RenderMode) 1;
            this.m_virtualCursorCanvas.worldCamera = this.VCcam;
          }
        }
      }
      if (Object.op_Inequality((Object) this.m_canvasCamera, (Object) null) && Object.op_Inequality((Object) this.VCcam, (Object) null))
      {
        this.VCcam.orthographic = this.m_canvasCamera.orthographic;
        this.VCcam.orthographicSize = this.m_canvasCamera.orthographicSize;
        ((Component) this.VCcam).transform.position = ((Component) this.m_canvasCamera).transform.position;
        this.VCcam.aspect = this.m_canvasCamera.aspect;
        ((Behaviour) this.VCcam).enabled = true;
      }
      Vector2 vector2;
      // ISSUE: explicit constructor call
      ((Vector2) ref vector2).\u002Ector((float) Screen.width, (float) Screen.height);
      if (Object.op_Inequality((Object) this.m_virtualCursorScaler, (Object) null) && Vector2.op_Inequality(this.m_virtualCursorScaler.referenceResolution, vector2))
        this.m_virtualCursorScaler.referenceResolution = vector2;
      this.m_LastMousePosition = this.m_MousePosition;
      ((Transform) this.m_VirtualCursor).localScale = Vector2.op_Implicit(Vector2.one);
      Vector2 steamCursorMovement = KInputManager.steamInputInterpreter.GetSteamCursorMovement();
      float num = (float) (1.0 / (4500.0 / (double) vector2.x));
      steamCursorMovement.x *= num;
      steamCursorMovement.y *= num;
      RectTransform virtualCursor = this.m_VirtualCursor;
      virtualCursor.anchoredPosition = Vector2.op_Addition(virtualCursor.anchoredPosition, Vector2.op_Multiply(steamCursorMovement, this.m_VirtualCursorSpeed));
      this.m_VirtualCursor.anchoredPosition = new Vector2(Mathf.Clamp(this.m_VirtualCursor.anchoredPosition.x, 0.0f, vector2.x), Mathf.Clamp(this.m_VirtualCursor.anchoredPosition.y, 0.0f, vector2.y));
      KInputManager.virtualCursorPos = new Vector3F(this.m_VirtualCursor.anchoredPosition.x, this.m_VirtualCursor.anchoredPosition.y, 0.0f);
      this.m_MousePosition = this.m_VirtualCursor.anchoredPosition;
    }

    public virtual bool IsModuleSupported() => this.m_ForceModuleActive || Input.mousePresent;

    public virtual bool ShouldActivateModule()
    {
      if (!((BaseInputModule) this).ShouldActivateModule())
        return false;
      if (KInputManager.currentControllerIsGamepad)
        return true;
      int num1 = this.m_ForceModuleActive ? 1 : 0;
      Input.GetButtonDown(this.m_SubmitButton);
      int num2 = Input.GetButtonDown(this.m_CancelButton) ? 1 : 0;
      int num3 = num1 | num2 | (!Mathf.Approximately(Input.GetAxisRaw(this.m_HorizontalAxis), 0.0f) ? 1 : 0) | (!Mathf.Approximately(Input.GetAxisRaw(this.m_VerticalAxis), 0.0f) ? 1 : 0);
      Vector2 vector2 = Vector2.op_Subtraction(this.m_MousePosition, this.m_LastMousePosition);
      int num4 = (double) ((Vector2) ref vector2).sqrMagnitude > 0.0 ? 1 : 0;
      return (num3 | num4 | (Input.GetMouseButtonDown(0) ? 1 : 0)) != 0;
    }

    public virtual void ActivateModule()
    {
      ((BaseInputModule) this).ActivateModule();
      if (Object.op_Equality((Object) this.m_canvasCamera, (Object) null))
      {
        this.m_canvasCamera = ((Component) this).gameObject.AddComponent<Camera>();
        ((Behaviour) this.m_canvasCamera).enabled = false;
      }
      this.m_VirtualCursor.anchoredPosition = (double) Input.mousePosition.x <= 0.0 || (double) Input.mousePosition.x >= (double) Screen.width || (double) Input.mousePosition.y <= 0.0 || (double) Input.mousePosition.y >= (double) Screen.height ? new Vector2((float) (Screen.width / 2), (float) (Screen.height / 2)) : Vector2.op_Implicit(Input.mousePosition);
      this.m_VirtualCursor.anchoredPosition = new Vector2(Mathf.Clamp(this.m_VirtualCursor.anchoredPosition.x, 0.0f, (float) Screen.width), Mathf.Clamp(this.m_VirtualCursor.anchoredPosition.y, 0.0f, (float) Screen.height));
      ((Transform) this.m_VirtualCursor).localScale = Vector2.op_Implicit(Vector2.zero);
      this.m_MousePosition = this.m_VirtualCursor.anchoredPosition;
      this.m_LastMousePosition = this.m_VirtualCursor.anchoredPosition;
      GameObject selectedGameObject = ((BaseInputModule) this).eventSystem.currentSelectedGameObject;
      if (Object.op_Equality((Object) selectedGameObject, (Object) null))
        selectedGameObject = ((BaseInputModule) this).eventSystem.firstSelectedGameObject;
      if (Object.op_Equality((Object) this.m_VirtualCursor, (Object) null))
        this.m_VirtualCursor = GameObject.Find("VirtualCursor").GetComponent<RectTransform>();
      if (Object.op_Equality((Object) this.m_canvasCamera, (Object) null))
        this.m_canvasCamera = GameObject.Find("FrontEndCamera").GetComponent<Camera>();
      ((BaseInputModule) this).eventSystem.SetSelectedGameObject(selectedGameObject, ((BaseInputModule) this).GetBaseEventData());
    }

    public virtual void DeactivateModule()
    {
      ((BaseInputModule) this).DeactivateModule();
      this.ClearSelection();
      this.conButtonStates.affirmativeDown = false;
      this.conButtonStates.affirmativeHoldTime = 0.0f;
      this.conButtonStates.negativeDown = false;
      this.conButtonStates.negativeHoldTime = 0.0f;
    }

    public virtual void Process()
    {
      bool selectedObject = this.SendUpdateEventToSelectedObject();
      if (((BaseInputModule) this).eventSystem.sendNavigationEvents)
      {
        if (!selectedObject)
          selectedObject |= this.SendMoveEventToSelectedObject();
        if (!selectedObject)
          this.SendSubmitEventToSelectedObject();
      }
      this.ProcessMouseEvent();
    }

    protected bool SendSubmitEventToSelectedObject()
    {
      if (Object.op_Equality((Object) ((BaseInputModule) this).eventSystem.currentSelectedGameObject, (Object) null))
        return false;
      BaseEventData baseEventData = ((BaseInputModule) this).GetBaseEventData();
      if (Input.GetButtonDown(this.m_SubmitButton))
        ExecuteEvents.Execute<ISubmitHandler>(((BaseInputModule) this).eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.submitHandler);
      if (Input.GetButtonDown(this.m_CancelButton))
        ExecuteEvents.Execute<ICancelHandler>(((BaseInputModule) this).eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.cancelHandler);
      return ((AbstractEventData) baseEventData).used;
    }

    private Vector2 GetRawMoveVector()
    {
      Vector2 zero = Vector2.zero;
      zero.x = Input.GetAxisRaw(this.m_HorizontalAxis);
      zero.y = Input.GetAxisRaw(this.m_VerticalAxis);
      if (Input.GetButtonDown(this.m_HorizontalAxis))
      {
        if ((double) zero.x < 0.0)
          zero.x = -1f;
        if ((double) zero.x > 0.0)
          zero.x = 1f;
      }
      if (Input.GetButtonDown(this.m_VerticalAxis))
      {
        if ((double) zero.y < 0.0)
          zero.y = -1f;
        if ((double) zero.y > 0.0)
          zero.y = 1f;
      }
      return zero;
    }

    protected bool SendMoveEventToSelectedObject()
    {
      float unscaledTime = Time.unscaledTime;
      Vector2 rawMoveVector = this.GetRawMoveVector();
      if (Mathf.Approximately(rawMoveVector.x, 0.0f) && Mathf.Approximately(rawMoveVector.y, 0.0f))
      {
        this.m_ConsecutiveMoveCount = 0;
        return false;
      }
      bool flag1 = Input.GetButtonDown(this.m_HorizontalAxis) || Input.GetButtonDown(this.m_VerticalAxis);
      bool flag2 = (double) Vector2.Dot(rawMoveVector, this.m_LastMoveVector) > 0.0;
      if (!flag1)
        flag1 = !flag2 || this.m_ConsecutiveMoveCount != 1 ? (double) unscaledTime > (double) this.m_PrevActionTime + 1.0 / (double) this.m_InputActionsPerSecond : (double) unscaledTime > (double) this.m_PrevActionTime + (double) this.m_RepeatDelay;
      if (!flag1)
        return false;
      AxisEventData axisEventData = ((BaseInputModule) this).GetAxisEventData(rawMoveVector.x, rawMoveVector.y, 0.6f);
      ExecuteEvents.Execute<IMoveHandler>(((BaseInputModule) this).eventSystem.currentSelectedGameObject, (BaseEventData) axisEventData, ExecuteEvents.moveHandler);
      if (!flag2)
        this.m_ConsecutiveMoveCount = 0;
      ++this.m_ConsecutiveMoveCount;
      this.m_PrevActionTime = unscaledTime;
      this.m_LastMoveVector = rawMoveVector;
      return ((AbstractEventData) axisEventData).used;
    }

    protected void ProcessMouseEvent() => this.ProcessMouseEvent(0);

    protected void ProcessMouseEvent(int id)
    {
      if (this.mouseMovementOnly)
        return;
      PointerInputModule.MouseState pointerEventData = base.GetMousePointerEventData(id);
      PointerInputModule.MouseButtonEventData eventData = pointerEventData.GetButtonState((PointerEventData.InputButton) 0).eventData;
      RaycastResult pointerCurrentRaycast1 = eventData.buttonData.pointerCurrentRaycast;
      this.m_CurrentFocusedGameObject = ((RaycastResult) ref pointerCurrentRaycast1).gameObject;
      this.ProcessControllerPress(eventData, true);
      this.ProcessControllerPress(pointerEventData.GetButtonState((PointerEventData.InputButton) 1).eventData, false);
      this.ProcessMove(eventData.buttonData);
      this.ProcessDrag(eventData.buttonData);
      this.ProcessDrag(pointerEventData.GetButtonState((PointerEventData.InputButton) 1).eventData.buttonData);
      this.ProcessDrag(pointerEventData.GetButtonState((PointerEventData.InputButton) 2).eventData.buttonData);
      Vector2 scrollDelta = eventData.buttonData.scrollDelta;
      if (Mathf.Approximately(((Vector2) ref scrollDelta).sqrMagnitude, 0.0f))
        return;
      RaycastResult pointerCurrentRaycast2 = eventData.buttonData.pointerCurrentRaycast;
      ExecuteEvents.ExecuteHierarchy<IScrollHandler>(ExecuteEvents.GetEventHandler<IScrollHandler>(((RaycastResult) ref pointerCurrentRaycast2).gameObject), (BaseEventData) eventData.buttonData, ExecuteEvents.scrollHandler);
    }

    protected bool SendUpdateEventToSelectedObject()
    {
      if (Object.op_Equality((Object) ((BaseInputModule) this).eventSystem.currentSelectedGameObject, (Object) null))
        return false;
      BaseEventData baseEventData = ((BaseInputModule) this).GetBaseEventData();
      ExecuteEvents.Execute<IUpdateSelectedHandler>(((BaseInputModule) this).eventSystem.currentSelectedGameObject, baseEventData, ExecuteEvents.updateSelectedHandler);
      return ((AbstractEventData) baseEventData).used;
    }

    protected void ProcessMousePress(PointerInputModule.MouseButtonEventData data)
    {
      PointerEventData buttonData = data.buttonData;
      RaycastResult pointerCurrentRaycast = buttonData.pointerCurrentRaycast;
      GameObject gameObject1 = ((RaycastResult) ref pointerCurrentRaycast).gameObject;
      if (data.PressedThisFrame())
      {
        buttonData.eligibleForClick = true;
        buttonData.delta = Vector2.zero;
        buttonData.dragging = false;
        buttonData.useDragThreshold = true;
        buttonData.pressPosition = buttonData.position;
        buttonData.pointerPressRaycast = buttonData.pointerCurrentRaycast;
        buttonData.position = this.m_VirtualCursor.anchoredPosition;
        this.DeselectIfSelectionChanged(gameObject1, (BaseEventData) buttonData);
        GameObject gameObject2 = ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>(gameObject1, (BaseEventData) buttonData, ExecuteEvents.pointerDownHandler);
        if (Object.op_Equality((Object) gameObject2, (Object) null))
          gameObject2 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject1);
        float unscaledTime = Time.unscaledTime;
        if (Object.op_Equality((Object) gameObject2, (Object) buttonData.lastPress))
        {
          if ((double) unscaledTime - (double) buttonData.clickTime < 0.30000001192092896)
            ++buttonData.clickCount;
          else
            buttonData.clickCount = 1;
          buttonData.clickTime = unscaledTime;
        }
        else
          buttonData.clickCount = 1;
        buttonData.pointerPress = gameObject2;
        buttonData.rawPointerPress = gameObject1;
        buttonData.clickTime = unscaledTime;
        buttonData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject1);
        if (Object.op_Inequality((Object) buttonData.pointerDrag, (Object) null))
          ExecuteEvents.Execute<IInitializePotentialDragHandler>(buttonData.pointerDrag, (BaseEventData) buttonData, ExecuteEvents.initializePotentialDrag);
      }
      if (!data.ReleasedThisFrame())
        return;
      ExecuteEvents.Execute<IPointerUpHandler>(buttonData.pointerPress, (BaseEventData) buttonData, ExecuteEvents.pointerUpHandler);
      GameObject eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject1);
      if (Object.op_Equality((Object) buttonData.pointerPress, (Object) eventHandler) && buttonData.eligibleForClick)
        ExecuteEvents.Execute<IPointerClickHandler>(buttonData.pointerPress, (BaseEventData) buttonData, ExecuteEvents.pointerClickHandler);
      else if (Object.op_Inequality((Object) buttonData.pointerDrag, (Object) null) && buttonData.dragging)
        ExecuteEvents.ExecuteHierarchy<IDropHandler>(gameObject1, (BaseEventData) buttonData, ExecuteEvents.dropHandler);
      buttonData.eligibleForClick = false;
      buttonData.pointerPress = (GameObject) null;
      buttonData.rawPointerPress = (GameObject) null;
      if (Object.op_Inequality((Object) buttonData.pointerDrag, (Object) null) && buttonData.dragging)
        ExecuteEvents.Execute<IEndDragHandler>(buttonData.pointerDrag, (BaseEventData) buttonData, ExecuteEvents.endDragHandler);
      buttonData.dragging = false;
      buttonData.pointerDrag = (GameObject) null;
      if (!Object.op_Inequality((Object) gameObject1, (Object) buttonData.pointerEnter))
        return;
      ((BaseInputModule) this).HandlePointerExitAndEnter(buttonData, (GameObject) null);
      ((BaseInputModule) this).HandlePointerExitAndEnter(buttonData, gameObject1);
    }

    public void OnKeyDown(KButtonEvent e)
    {
      if (!KInputManager.currentControllerIsGamepad)
        return;
      if (e.IsAction((Action) 3) || e.IsAction((Action) 4))
      {
        if (this.conButtonStates.affirmativeDown)
          this.conButtonStates.affirmativeHoldTime += Time.unscaledDeltaTime;
        if (!this.conButtonStates.affirmativeDown)
        {
          this.leftFirstClick = true;
          this.leftReleased = false;
        }
        this.conButtonStates.affirmativeDown = true;
      }
      else
      {
        if (!e.IsAction((Action) 5))
          return;
        if (this.conButtonStates.negativeDown)
          this.conButtonStates.negativeHoldTime += Time.unscaledDeltaTime;
        if (!this.conButtonStates.negativeDown)
        {
          this.rightFirstClick = true;
          this.rightReleased = false;
        }
        this.conButtonStates.negativeDown = true;
      }
    }

    public void OnKeyUp(KButtonEvent e)
    {
      if (!KInputManager.currentControllerIsGamepad)
        return;
      if (e.IsAction((Action) 3) || e.IsAction((Action) 4))
      {
        this.conButtonStates.affirmativeHoldTime = 0.0f;
        this.leftReleased = true;
        this.leftFirstClick = false;
        this.conButtonStates.affirmativeDown = false;
      }
      else
      {
        if (!e.IsAction((Action) 5))
          return;
        this.conButtonStates.negativeHoldTime = 0.0f;
        this.rightReleased = true;
        this.rightFirstClick = false;
        this.conButtonStates.negativeDown = false;
      }
    }

    protected void ProcessControllerPress(
      PointerInputModule.MouseButtonEventData data,
      bool leftClick)
    {
      if (this.leftClickData == null)
        this.leftClickData = data.buttonData;
      if (this.rightClickData == null)
        this.rightClickData = data.buttonData;
      if (leftClick)
      {
        PointerEventData buttonData = data.buttonData;
        RaycastResult pointerCurrentRaycast = buttonData.pointerCurrentRaycast;
        GameObject gameObject1 = ((RaycastResult) ref pointerCurrentRaycast).gameObject;
        buttonData.position = this.m_VirtualCursor.anchoredPosition;
        if (this.leftFirstClick)
        {
          buttonData.button = (PointerEventData.InputButton) 0;
          buttonData.eligibleForClick = true;
          buttonData.delta = Vector2.zero;
          buttonData.dragging = false;
          buttonData.useDragThreshold = true;
          buttonData.pressPosition = buttonData.position;
          buttonData.pointerPressRaycast = buttonData.pointerCurrentRaycast;
          buttonData.position = new Vector2(KInputManager.virtualCursorPos.x, KInputManager.virtualCursorPos.y);
          this.DeselectIfSelectionChanged(gameObject1, (BaseEventData) buttonData);
          GameObject gameObject2 = ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>(gameObject1, (BaseEventData) buttonData, ExecuteEvents.pointerDownHandler);
          if (Object.op_Equality((Object) gameObject2, (Object) null))
            gameObject2 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject1);
          float unscaledTime = Time.unscaledTime;
          if (Object.op_Equality((Object) gameObject2, (Object) buttonData.lastPress))
          {
            if ((double) unscaledTime - (double) buttonData.clickTime < 0.30000001192092896)
              ++buttonData.clickCount;
            else
              buttonData.clickCount = 1;
            buttonData.clickTime = unscaledTime;
          }
          else
            buttonData.clickCount = 1;
          buttonData.pointerPress = gameObject2;
          buttonData.rawPointerPress = gameObject1;
          buttonData.clickTime = unscaledTime;
          buttonData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject1);
          if (Object.op_Inequality((Object) buttonData.pointerDrag, (Object) null))
            ExecuteEvents.Execute<IInitializePotentialDragHandler>(buttonData.pointerDrag, (BaseEventData) buttonData, ExecuteEvents.initializePotentialDrag);
          this.leftFirstClick = false;
        }
        else
        {
          if (!this.leftReleased)
            return;
          buttonData.button = (PointerEventData.InputButton) 0;
          ExecuteEvents.Execute<IPointerUpHandler>(buttonData.pointerPress, (BaseEventData) buttonData, ExecuteEvents.pointerUpHandler);
          GameObject eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject1);
          if (Object.op_Equality((Object) buttonData.pointerPress, (Object) eventHandler) && buttonData.eligibleForClick)
            ExecuteEvents.Execute<IPointerClickHandler>(buttonData.pointerPress, (BaseEventData) buttonData, ExecuteEvents.pointerClickHandler);
          else if (Object.op_Inequality((Object) buttonData.pointerDrag, (Object) null) && buttonData.dragging)
            ExecuteEvents.ExecuteHierarchy<IDropHandler>(gameObject1, (BaseEventData) buttonData, ExecuteEvents.dropHandler);
          buttonData.eligibleForClick = false;
          buttonData.pointerPress = (GameObject) null;
          buttonData.rawPointerPress = (GameObject) null;
          if (Object.op_Inequality((Object) buttonData.pointerDrag, (Object) null) && buttonData.dragging)
            ExecuteEvents.Execute<IEndDragHandler>(buttonData.pointerDrag, (BaseEventData) buttonData, ExecuteEvents.endDragHandler);
          buttonData.dragging = false;
          buttonData.pointerDrag = (GameObject) null;
          if (Object.op_Inequality((Object) gameObject1, (Object) buttonData.pointerEnter))
          {
            ((BaseInputModule) this).HandlePointerExitAndEnter(buttonData, (GameObject) null);
            ((BaseInputModule) this).HandlePointerExitAndEnter(buttonData, gameObject1);
          }
          this.leftReleased = false;
        }
      }
      else
      {
        PointerEventData buttonData = data.buttonData;
        RaycastResult pointerCurrentRaycast = buttonData.pointerCurrentRaycast;
        GameObject gameObject3 = ((RaycastResult) ref pointerCurrentRaycast).gameObject;
        buttonData.position = this.m_VirtualCursor.anchoredPosition;
        if (this.rightFirstClick)
        {
          buttonData.button = (PointerEventData.InputButton) 1;
          buttonData.eligibleForClick = true;
          buttonData.delta = Vector2.zero;
          buttonData.dragging = false;
          buttonData.useDragThreshold = true;
          buttonData.pressPosition = buttonData.position;
          buttonData.pointerPressRaycast = buttonData.pointerCurrentRaycast;
          buttonData.position = this.m_VirtualCursor.anchoredPosition;
          this.DeselectIfSelectionChanged(gameObject3, (BaseEventData) buttonData);
          GameObject gameObject4 = ExecuteEvents.ExecuteHierarchy<IPointerDownHandler>(gameObject3, (BaseEventData) buttonData, ExecuteEvents.pointerDownHandler);
          if (Object.op_Equality((Object) gameObject4, (Object) null))
            gameObject4 = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject3);
          float unscaledTime = Time.unscaledTime;
          if (Object.op_Equality((Object) gameObject4, (Object) buttonData.lastPress))
          {
            if ((double) unscaledTime - (double) buttonData.clickTime < 0.30000001192092896)
              ++buttonData.clickCount;
            else
              buttonData.clickCount = 1;
            buttonData.clickTime = unscaledTime;
          }
          else
            buttonData.clickCount = 1;
          buttonData.pointerPress = gameObject4;
          buttonData.rawPointerPress = gameObject3;
          buttonData.pointerDrag = ExecuteEvents.GetEventHandler<IDragHandler>(gameObject3);
          if (Object.op_Inequality((Object) buttonData.pointerDrag, (Object) null))
            ExecuteEvents.Execute<IInitializePotentialDragHandler>(buttonData.pointerDrag, (BaseEventData) buttonData, ExecuteEvents.initializePotentialDrag);
          this.rightFirstClick = false;
        }
        else
        {
          if (!this.rightReleased)
            return;
          buttonData.button = (PointerEventData.InputButton) 1;
          ExecuteEvents.Execute<IPointerUpHandler>(buttonData.pointerPress, (BaseEventData) buttonData, ExecuteEvents.pointerUpHandler);
          GameObject eventHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(gameObject3);
          if (Object.op_Equality((Object) buttonData.pointerPress, (Object) eventHandler) && buttonData.eligibleForClick)
            ExecuteEvents.Execute<IPointerClickHandler>(buttonData.pointerPress, (BaseEventData) buttonData, ExecuteEvents.pointerClickHandler);
          else if (Object.op_Inequality((Object) buttonData.pointerDrag, (Object) null) && buttonData.dragging)
            ExecuteEvents.ExecuteHierarchy<IDropHandler>(gameObject3, (BaseEventData) buttonData, ExecuteEvents.dropHandler);
          buttonData.eligibleForClick = false;
          buttonData.pointerPress = (GameObject) null;
          buttonData.rawPointerPress = (GameObject) null;
          if (Object.op_Inequality((Object) buttonData.pointerDrag, (Object) null) && buttonData.dragging)
            ExecuteEvents.Execute<IEndDragHandler>(buttonData.pointerDrag, (BaseEventData) buttonData, ExecuteEvents.endDragHandler);
          buttonData.dragging = false;
          buttonData.pointerDrag = (GameObject) null;
          if (Object.op_Inequality((Object) gameObject3, (Object) buttonData.pointerEnter))
          {
            ((BaseInputModule) this).HandlePointerExitAndEnter(buttonData, (GameObject) null);
            ((BaseInputModule) this).HandlePointerExitAndEnter(buttonData, gameObject3);
          }
          this.rightReleased = false;
        }
      }
    }

    protected virtual PointerInputModule.MouseState GetMousePointerEventData(int id)
    {
      PointerEventData pointerEventData1;
      int num = this.GetPointerData(-1, ref pointerEventData1, true) ? 1 : 0;
      ((AbstractEventData) pointerEventData1).Reset();
      Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(this.m_canvasCamera, ((Transform) this.m_VirtualCursor).position);
      if (num != 0)
        pointerEventData1.position = screenPoint;
      Vector2 anchoredPosition = this.m_VirtualCursor.anchoredPosition;
      pointerEventData1.delta = Vector2.op_Subtraction(anchoredPosition, pointerEventData1.position);
      pointerEventData1.position = anchoredPosition;
      pointerEventData1.scrollDelta = Input.mouseScrollDelta;
      pointerEventData1.button = (PointerEventData.InputButton) 0;
      ((BaseInputModule) this).eventSystem.RaycastAll(pointerEventData1, ((BaseInputModule) this).m_RaycastResultCache);
      RaycastResult firstRaycast = BaseInputModule.FindFirstRaycast(((BaseInputModule) this).m_RaycastResultCache);
      pointerEventData1.pointerCurrentRaycast = firstRaycast;
      ((BaseInputModule) this).m_RaycastResultCache.Clear();
      PointerEventData pointerEventData2;
      this.GetPointerData(-2, ref pointerEventData2, true);
      this.CopyFromTo(pointerEventData1, pointerEventData2);
      pointerEventData2.button = (PointerEventData.InputButton) 1;
      PointerEventData pointerEventData3;
      this.GetPointerData(-3, ref pointerEventData3, true);
      this.CopyFromTo(pointerEventData1, pointerEventData3);
      pointerEventData3.button = (PointerEventData.InputButton) 2;
      this.m_MouseState.SetButtonState((PointerEventData.InputButton) 0, this.StateForMouseButton(0), pointerEventData1);
      this.m_MouseState.SetButtonState((PointerEventData.InputButton) 1, this.StateForMouseButton(1), pointerEventData2);
      this.m_MouseState.SetButtonState((PointerEventData.InputButton) 2, this.StateForMouseButton(2), pointerEventData3);
      return this.m_MouseState;
    }

    [Obsolete("Mode is no longer needed on input module as it handles both mouse and keyboard simultaneously.", false)]
    public enum InputMode
    {
      Mouse,
      Buttons,
    }

    private struct ControllerButtonStates
    {
      public bool affirmativeDown;
      public float affirmativeHoldTime;
      public bool negativeDown;
      public float negativeHoldTime;
    }
  }
}
