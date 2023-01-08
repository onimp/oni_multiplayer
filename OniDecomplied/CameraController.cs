// Decompiled with JetBrains decompiler
// Type: CameraController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

[AddComponentMenu("KMonoBehaviour/scripts/CameraController")]
public class CameraController : KMonoBehaviour, IInputHandler
{
  public const float DEFAULT_MAX_ORTHO_SIZE = 20f;
  public float MAX_Y_SCALE = 1.1f;
  public LocText infoText;
  private const float FIXED_Z = -100f;
  public bool FreeCameraEnabled;
  public float zoomSpeed;
  public float minOrthographicSize;
  public float zoomFactor;
  public float keyPanningSpeed;
  public float keyPanningEasing;
  public Texture2D dayColourCube;
  public Texture2D nightColourCube;
  public Material LightBufferMaterial;
  public Material LightCircleOverlay;
  public Material LightConeOverlay;
  public Transform followTarget;
  public Vector3 followTargetPos;
  public GridVisibleArea VisibleArea = new GridVisibleArea();
  private float maxOrthographicSize = 20f;
  private float overrideZoomSpeed;
  private bool panning;
  private Vector3 keyPanDelta;
  [SerializeField]
  private LayerMask timelapseCameraCullingMask;
  [SerializeField]
  private LayerMask timelapseOverlayCameraCullingMask;
  private bool userCameraControlDisabled;
  private bool panLeft;
  private bool panRight;
  private bool panUp;
  private bool panDown;
  [NonSerialized]
  public Camera baseCamera;
  [NonSerialized]
  public Camera overlayCamera;
  [NonSerialized]
  public Camera overlayNoDepthCamera;
  [NonSerialized]
  public Camera uiCamera;
  [NonSerialized]
  public Camera lightBufferCamera;
  [NonSerialized]
  public Camera simOverlayCamera;
  [NonSerialized]
  public Camera infraredCamera;
  [NonSerialized]
  public Camera timelapseFreezeCamera;
  [SerializeField]
  private List<GameScreenManager.UIRenderTarget> uiCameraTargets;
  public List<Camera> cameras = new List<Camera>();
  private MultipleRenderTarget mrt;
  public SoundCuller soundCuller;
  private bool cinemaCamEnabled;
  private bool cinemaToggleLock;
  private bool cinemaToggleEasing;
  private bool cinemaUnpauseNextMove;
  private bool cinemaPanLeft;
  private bool cinemaPanRight;
  private bool cinemaPanUp;
  private bool cinemaPanDown;
  private bool cinemaZoomIn;
  private bool cinemaZoomOut;
  private int cinemaZoomSpeed = 10;
  private float cinemaEasing = 0.05f;
  private float cinemaZoomVelocity;
  private float smoothDt;

  public string handlerName => ((Object) ((Component) this).gameObject).name;

  public float OrthographicSize
  {
    get => !Object.op_Equality((Object) this.baseCamera, (Object) null) ? this.baseCamera.orthographicSize : 0.0f;
    set
    {
      for (int index = 0; index < this.cameras.Count; ++index)
        this.cameras[index].orthographicSize = value;
    }
  }

  public KInputHandler inputHandler { get; set; }

  public float targetOrthographicSize { get; private set; }

  public bool isTargetPosSet { get; set; }

  public Vector3 targetPos { get; private set; }

  public bool ignoreClusterFX { get; private set; }

  public void ToggleClusterFX() => this.ignoreClusterFX = !this.ignoreClusterFX;

  protected virtual void OnForcedCleanUp() => Global.GetInputManager()?.usedMenus.Remove((IInputHandler) this);

  public int cameraActiveCluster => Object.op_Equality((Object) ClusterManager.Instance, (Object) null) ? (int) ClusterManager.INVALID_WORLD_IDX : ClusterManager.Instance.activeWorldId;

  public void GetWorldCamera(out Vector2I worldOffset, out Vector2I worldSize)
  {
    WorldContainer worldContainer = (WorldContainer) null;
    if (Object.op_Inequality((Object) ClusterManager.Instance, (Object) null))
      worldContainer = ClusterManager.Instance.activeWorld;
    if (!this.ignoreClusterFX && Object.op_Inequality((Object) worldContainer, (Object) null))
    {
      worldOffset = worldContainer.WorldOffset;
      worldSize = worldContainer.WorldSize;
    }
    else
    {
      worldOffset = new Vector2I(0, 0);
      worldSize = new Vector2I(Grid.WidthInCells, Grid.HeightInCells);
    }
  }

  public bool DisableUserCameraControl
  {
    get => this.userCameraControlDisabled;
    set
    {
      this.userCameraControlDisabled = value;
      if (!this.userCameraControlDisabled)
        return;
      this.panning = false;
      this.panLeft = false;
      this.panRight = false;
      this.panUp = false;
      this.panDown = false;
    }
  }

  public static CameraController Instance { get; private set; }

  public static void DestroyInstance() => CameraController.Instance = (CameraController) null;

  public void ToggleColouredOverlayView(bool enabled) => this.mrt.ToggleColouredOverlayView(enabled);

  protected virtual void OnPrefabInit()
  {
    Util.Reset(this.transform);
    TransformExtensions.SetLocalPosition(this.transform, new Vector3(Grid.WidthInMeters / 2f, Grid.HeightInMeters / 2f, -100f));
    this.targetOrthographicSize = this.maxOrthographicSize;
    CameraController.Instance = this;
    this.DisableUserCameraControl = false;
    this.baseCamera = this.CopyCamera(Camera.main, "baseCamera");
    this.mrt = ((Component) this.baseCamera).gameObject.AddComponent<MultipleRenderTarget>();
    this.mrt.onSetupComplete += new Action<Camera>(this.OnMRTSetupComplete);
    ((Component) this.baseCamera).gameObject.AddComponent<LightBufferCompositor>();
    this.baseCamera.transparencySortMode = (TransparencySortMode) 2;
    ((Component) this.baseCamera).transform.parent = this.transform;
    Util.Reset(((Component) this.baseCamera).transform);
    int mask1 = LayerMask.GetMask(new string[2]
    {
      "PlaceWithDepth",
      "Overlay"
    });
    int mask2 = LayerMask.GetMask(new string[1]
    {
      "Construction"
    });
    this.baseCamera.cullingMask &= ~mask1;
    this.baseCamera.cullingMask |= mask2;
    ((Component) this.baseCamera).tag = "Untagged";
    ((Component) this.baseCamera).gameObject.AddComponent<CameraRenderTexture>().TextureName = "_LitTex";
    this.infraredCamera = this.CopyCamera(this.baseCamera, "Infrared");
    this.infraredCamera.cullingMask = 0;
    this.infraredCamera.clearFlags = (CameraClearFlags) 2;
    this.infraredCamera.depth = this.baseCamera.depth - 1f;
    ((Component) this.infraredCamera).transform.parent = this.transform;
    ((Component) this.infraredCamera).gameObject.AddComponent<Infrared>();
    if (Object.op_Inequality((Object) SimDebugView.Instance, (Object) null))
    {
      this.simOverlayCamera = this.CopyCamera(this.baseCamera, "SimOverlayCamera");
      this.simOverlayCamera.cullingMask = LayerMask.GetMask(new string[1]
      {
        "SimDebugView"
      });
      this.simOverlayCamera.clearFlags = (CameraClearFlags) 2;
      this.simOverlayCamera.depth = this.baseCamera.depth + 1f;
      ((Component) this.simOverlayCamera).transform.parent = this.transform;
      ((Component) this.simOverlayCamera).gameObject.AddComponent<CameraRenderTexture>().TextureName = "_SimDebugViewTex";
    }
    this.overlayCamera = Camera.main;
    ((Object) this.overlayCamera).name = "Overlay";
    this.overlayCamera.cullingMask = mask1 | mask2;
    this.overlayCamera.clearFlags = (CameraClearFlags) 4;
    ((Component) this.overlayCamera).transform.parent = this.transform;
    this.overlayCamera.depth = this.baseCamera.depth + 3f;
    TransformExtensions.SetLocalPosition(((Component) this.overlayCamera).transform, Vector3.zero);
    ((Component) this.overlayCamera).transform.localRotation = Quaternion.identity;
    this.overlayCamera.renderingPath = (RenderingPath) 1;
    this.overlayCamera.allowHDR = false;
    ((Component) this.overlayCamera).tag = "Untagged";
    ((Component) this.overlayCamera).gameObject.AddComponent<CameraReferenceTexture>().referenceCamera = this.baseCamera;
    ColorCorrectionLookup component = ((Component) this.overlayCamera).GetComponent<ColorCorrectionLookup>();
    component.Convert(this.dayColourCube, "");
    component.Convert2(this.nightColourCube, "");
    this.cameras.Add(this.overlayCamera);
    this.lightBufferCamera = this.CopyCamera(this.overlayCamera, "Light Buffer");
    this.lightBufferCamera.clearFlags = (CameraClearFlags) 2;
    this.lightBufferCamera.cullingMask = LayerMask.GetMask(new string[1]
    {
      "Lights"
    });
    this.lightBufferCamera.depth = this.baseCamera.depth - 1f;
    ((Component) this.lightBufferCamera).transform.parent = this.transform;
    TransformExtensions.SetLocalPosition(((Component) this.lightBufferCamera).transform, Vector3.zero);
    this.lightBufferCamera.rect = new Rect(0.0f, 0.0f, 1f, 1f);
    LightBuffer lightBuffer = ((Component) this.lightBufferCamera).gameObject.AddComponent<LightBuffer>();
    lightBuffer.Material = this.LightBufferMaterial;
    lightBuffer.CircleMaterial = this.LightCircleOverlay;
    lightBuffer.ConeMaterial = this.LightConeOverlay;
    this.overlayNoDepthCamera = this.CopyCamera(this.overlayCamera, "overlayNoDepth");
    int mask3 = LayerMask.GetMask(new string[2]
    {
      "Overlay",
      "Place"
    });
    this.baseCamera.cullingMask &= ~mask3;
    this.overlayNoDepthCamera.clearFlags = (CameraClearFlags) 3;
    this.overlayNoDepthCamera.cullingMask = mask3;
    ((Component) this.overlayNoDepthCamera).transform.parent = this.transform;
    TransformExtensions.SetLocalPosition(((Component) this.overlayNoDepthCamera).transform, Vector3.zero);
    this.overlayNoDepthCamera.depth = this.baseCamera.depth + 4f;
    ((Component) this.overlayNoDepthCamera).tag = "MainCamera";
    ((Component) this.overlayNoDepthCamera).gameObject.AddComponent<NavPathDrawer>();
    this.uiCamera = this.CopyCamera(this.overlayCamera, "uiCamera");
    this.uiCamera.clearFlags = (CameraClearFlags) 3;
    this.uiCamera.cullingMask = LayerMask.GetMask(new string[1]
    {
      "UI"
    });
    ((Component) this.uiCamera).transform.parent = this.transform;
    TransformExtensions.SetLocalPosition(((Component) this.uiCamera).transform, Vector3.zero);
    this.uiCamera.depth = this.baseCamera.depth + 5f;
    if (Object.op_Inequality((Object) Game.Instance, (Object) null))
    {
      this.timelapseFreezeCamera = this.CopyCamera(this.uiCamera, "timelapseFreezeCamera");
      this.timelapseFreezeCamera.depth = this.uiCamera.depth + 3f;
      ((Component) this.timelapseFreezeCamera).gameObject.AddComponent<FillRenderTargetEffect>();
      ((Behaviour) this.timelapseFreezeCamera).enabled = false;
      Camera camera = CameraController.CloneCamera(this.overlayCamera, "timelapseCamera");
      Timelapser timelapser = ((Component) camera).gameObject.AddComponent<Timelapser>();
      camera.transparencySortMode = (TransparencySortMode) 2;
      camera.depth = this.baseCamera.depth + 2f;
      Game.Instance.timelapser = timelapser;
    }
    if (Object.op_Inequality((Object) GameScreenManager.Instance, (Object) null))
    {
      for (int index = 0; index < this.uiCameraTargets.Count; ++index)
        GameScreenManager.Instance.SetCamera(this.uiCameraTargets[index], this.uiCamera);
      this.infoText = GameScreenManager.Instance.screenshotModeCanvas.GetComponentInChildren<LocText>();
    }
    if (!KPlayerPrefs.HasKey("CameraSpeed"))
      CameraController.SetDefaultCameraSpeed();
    this.SetSpeedFromPrefs();
    Game.Instance.Subscribe(75424175, new Action<object>(this.SetSpeedFromPrefs));
  }

  private void SetSpeedFromPrefs(object data = null) => this.keyPanningSpeed = Mathf.Clamp(0.1f, KPlayerPrefs.GetFloat("CameraSpeed"), 2f);

  public int GetCursorCell()
  {
    Vector3 worldPoint = Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos());
    Vector3 vector3 = Vector3.Max(Vector2.op_Implicit(ClusterManager.Instance.activeWorld.minimumBounds), worldPoint);
    return Grid.PosToCell(Vector3.Min(Vector2.op_Implicit(ClusterManager.Instance.activeWorld.maximumBounds), vector3));
  }

  public static Camera CloneCamera(Camera camera, string name)
  {
    GameObject gameObject = new GameObject();
    ((Object) gameObject).name = name;
    Camera camera1 = gameObject.AddComponent<Camera>();
    camera1.CopyFrom(camera);
    return camera1;
  }

  private Camera CopyCamera(Camera camera, string name)
  {
    Camera camera1 = CameraController.CloneCamera(camera, name);
    this.cameras.Add(camera1);
    return camera1;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.Restore();
  }

  public static void SetDefaultCameraSpeed() => KPlayerPrefs.SetFloat("CameraSpeed", 1f);

  public Coroutine activeFadeRoutine { get; private set; }

  public void FadeOut(float targetPercentage = 1f, float speed = 1f, System.Action callback = null)
  {
    if (this.activeFadeRoutine != null)
      ((MonoBehaviour) this).StopCoroutine(this.activeFadeRoutine);
    this.activeFadeRoutine = ((MonoBehaviour) this).StartCoroutine(this.FadeWithBlack(true, 0.0f, targetPercentage, speed));
  }

  public void FadeIn(float targetPercentage = 0.0f, float speed = 1f, System.Action callback = null)
  {
    if (this.activeFadeRoutine != null)
      ((MonoBehaviour) this).StopCoroutine(this.activeFadeRoutine);
    this.activeFadeRoutine = ((MonoBehaviour) this).StartCoroutine(this.FadeWithBlack(true, 1f, targetPercentage, speed, callback));
  }

  public void ActiveWorldStarWipe(int id, System.Action callback = null) => this.ActiveWorldStarWipe(id, false, new Vector3(), 10f, callback);

  public void ActiveWorldStarWipe(
    int id,
    Vector3 position,
    float forceOrthgraphicSize = 10f,
    System.Action callback = null)
  {
    this.ActiveWorldStarWipe(id, true, position, forceOrthgraphicSize, callback);
  }

  private void ActiveWorldStarWipe(
    int id,
    bool useForcePosition,
    Vector3 forcePosition,
    float forceOrthgraphicSize,
    System.Action callback)
  {
    if (this.activeFadeRoutine != null)
      ((MonoBehaviour) this).StopCoroutine(this.activeFadeRoutine);
    if (ClusterManager.Instance.activeWorldId != id)
    {
      DetailsScreen.Instance.DeselectAndClose();
      this.activeFadeRoutine = ((MonoBehaviour) this).StartCoroutine(this.SwapToWorldFade(id, useForcePosition, forcePosition, forceOrthgraphicSize, callback));
    }
    else
    {
      ManagementMenu.Instance.CloseAll();
      if (!useForcePosition)
        return;
      CameraController.Instance.SetTargetPos(forcePosition, 8f, true);
      if (callback == null)
        return;
      callback();
    }
  }

  private IEnumerator SwapToWorldFade(
    int worldId,
    bool useForcePosition,
    Vector3 forcePosition,
    float forceOrthgraphicSize,
    System.Action newWorldCallback)
  {
    CameraController cameraController = this;
    AudioMixer.instance.Start(AudioMixerSnapshots.Get().ActiveBaseChangeSnapshot);
    ClusterManager.Instance.UpdateWorldReverbSnapshot(worldId);
    yield return (object) ((MonoBehaviour) cameraController).StartCoroutine(cameraController.FadeWithBlack(false, 0.0f, 1f, 3f));
    ClusterManager.Instance.SetActiveWorld(worldId);
    if (useForcePosition)
    {
      CameraController.Instance.SetTargetPos(forcePosition, forceOrthgraphicSize, false);
      CameraController.Instance.SetPosition(forcePosition);
    }
    if (newWorldCallback != null)
      newWorldCallback();
    ManagementMenu.Instance.CloseAll();
    AudioMixer.instance.Stop(AudioMixerSnapshots.Get().ActiveBaseChangeSnapshot);
    yield return (object) ((MonoBehaviour) cameraController).StartCoroutine(cameraController.FadeWithBlack(false, 1f, 0.0f, 3f));
  }

  public void SetWorldInteractive(bool state) => ((Graphic) GameScreenManager.Instance.fadePlaneFront).raycastTarget = !state;

  private IEnumerator FadeWithBlack(
    bool fadeUI,
    float startBlackPercent,
    float targetBlackPercent,
    float speed = 1f,
    System.Action callback = null)
  {
    Image fadePlane = fadeUI ? GameScreenManager.Instance.fadePlaneFront : GameScreenManager.Instance.fadePlaneBack;
    float percent = 0.0f;
    while ((double) percent < 1.0)
    {
      percent += Time.unscaledDeltaTime * speed;
      ((Graphic) fadePlane).color = new Color(0.0f, 0.0f, 0.0f, MathUtil.ReRange(percent, 0.0f, 1f, startBlackPercent, targetBlackPercent));
      yield return (object) SequenceUtil.WaitForNextFrame;
    }
    ((Graphic) fadePlane).color = new Color(0.0f, 0.0f, 0.0f, targetBlackPercent);
    if (callback != null)
      callback();
    this.activeFadeRoutine = (Coroutine) null;
    yield return (object) SequenceUtil.WaitForNextFrame;
  }

  public void EnableFreeCamera(bool enable)
  {
    this.FreeCameraEnabled = enable;
    this.SetInfoText("Screenshot Mode (ESC to exit)");
  }

  private static bool WithinInputField()
  {
    EventSystem current = EventSystem.current;
    if (Object.op_Equality((Object) current, (Object) null))
      return false;
    bool flag = false;
    if (Object.op_Inequality((Object) current.currentSelectedGameObject, (Object) null) && (Object.op_Inequality((Object) current.currentSelectedGameObject.GetComponent<KInputTextField>(), (Object) null) || Object.op_Inequality((Object) current.currentSelectedGameObject.GetComponent<InputField>(), (Object) null)))
      flag = true;
    return flag;
  }

  private void SetInfoText(string text)
  {
    ((TMP_Text) this.infoText).text = text;
    Color color = ((Graphic) this.infoText).color;
    color.a = 0.5f;
    ((Graphic) this.infoText).color = color;
  }

  public void OnKeyDown(KButtonEvent e)
  {
    if (((KInputEvent) e).Consumed || this.DisableUserCameraControl || CameraController.WithinInputField() || Object.op_Inequality((Object) SaveGame.Instance, (Object) null) && ((Component) SaveGame.Instance).GetComponent<UserNavigation>().Handle(e))
      return;
    if (!this.ChangeWorldInput(e))
    {
      if (e.TryConsume((Action) 11))
        SpeedControlScreen.Instance.TogglePause(false);
      else if (e.TryConsume((Action) 7))
      {
        this.targetOrthographicSize = Mathf.Max(this.targetOrthographicSize * (1f / this.zoomFactor), this.minOrthographicSize);
        this.overrideZoomSpeed = 0.0f;
        this.isTargetPosSet = false;
      }
      else if (e.TryConsume((Action) 8))
      {
        this.targetOrthographicSize = Mathf.Min(this.targetOrthographicSize * this.zoomFactor, this.FreeCameraEnabled ? TuningData<CameraController.Tuning>.Get().maxOrthographicSizeDebug : this.maxOrthographicSize);
        this.overrideZoomSpeed = 0.0f;
        this.isTargetPosSet = false;
      }
      else if (e.TryConsume((Action) 6) || e.IsAction((Action) 5))
      {
        this.panning = true;
        this.overrideZoomSpeed = 0.0f;
        this.isTargetPosSet = false;
      }
      else if (this.FreeCameraEnabled && e.TryConsume((Action) 244))
      {
        this.cinemaCamEnabled = !this.cinemaCamEnabled;
        DebugUtil.LogArgs(new object[2]
        {
          (object) "Cinema Cam Enabled ",
          (object) this.cinemaCamEnabled
        });
        this.SetInfoText(this.cinemaCamEnabled ? "Cinema Cam Enabled" : "Cinema Cam Disabled");
      }
      else if (this.FreeCameraEnabled && this.cinemaCamEnabled)
      {
        if (e.TryConsume((Action) (int) byte.MaxValue))
        {
          this.cinemaToggleLock = !this.cinemaToggleLock;
          DebugUtil.LogArgs(new object[2]
          {
            (object) "Cinema Toggle Lock ",
            (object) this.cinemaToggleLock
          });
          this.SetInfoText(this.cinemaToggleLock ? "Cinema Input Lock ON" : "Cinema Input Lock OFF");
        }
        else if (e.TryConsume((Action) 256))
        {
          this.cinemaToggleEasing = !this.cinemaToggleEasing;
          DebugUtil.LogArgs(new object[2]
          {
            (object) "Cinema Toggle Easing ",
            (object) this.cinemaToggleEasing
          });
          this.SetInfoText(this.cinemaToggleEasing ? "Cinema Easing ON" : "Cinema Easing OFF");
        }
        else if (e.TryConsume((Action) 257))
        {
          this.cinemaUnpauseNextMove = !this.cinemaUnpauseNextMove;
          DebugUtil.LogArgs(new object[2]
          {
            (object) "Cinema Unpause Next Move ",
            (object) this.cinemaUnpauseNextMove
          });
          this.SetInfoText(this.cinemaUnpauseNextMove ? "Cinema Unpause Next Move ON" : "Cinema Unpause Next Move OFF");
        }
        else if (e.TryConsume((Action) 245))
        {
          this.cinemaPanLeft = !this.cinemaToggleLock || !this.cinemaPanLeft;
          this.cinemaPanRight = false;
          this.CheckMoveUnpause();
        }
        else if (e.TryConsume((Action) 246))
        {
          this.cinemaPanRight = !this.cinemaToggleLock || !this.cinemaPanRight;
          this.cinemaPanLeft = false;
          this.CheckMoveUnpause();
        }
        else if (e.TryConsume((Action) 247))
        {
          this.cinemaPanUp = !this.cinemaToggleLock || !this.cinemaPanUp;
          this.cinemaPanDown = false;
          this.CheckMoveUnpause();
        }
        else if (e.TryConsume((Action) 248))
        {
          this.cinemaPanDown = !this.cinemaToggleLock || !this.cinemaPanDown;
          this.cinemaPanUp = false;
          this.CheckMoveUnpause();
        }
        else if (e.TryConsume((Action) 249))
        {
          this.cinemaZoomIn = !this.cinemaToggleLock || !this.cinemaZoomIn;
          this.cinemaZoomOut = false;
          this.CheckMoveUnpause();
        }
        else if (e.TryConsume((Action) 250))
        {
          this.cinemaZoomOut = !this.cinemaToggleLock || !this.cinemaZoomOut;
          this.cinemaZoomIn = false;
          this.CheckMoveUnpause();
        }
        else if (e.TryConsume((Action) 254))
        {
          ++this.cinemaZoomSpeed;
          DebugUtil.LogArgs(new object[2]
          {
            (object) "Cinema Zoom Speed ",
            (object) this.cinemaZoomSpeed
          });
          this.SetInfoText("Cinema Zoom Speed: " + this.cinemaZoomSpeed.ToString());
        }
        else if (e.TryConsume((Action) 253))
        {
          --this.cinemaZoomSpeed;
          DebugUtil.LogArgs(new object[2]
          {
            (object) "Cinema Zoom Speed ",
            (object) this.cinemaZoomSpeed
          });
          this.SetInfoText("Cinema Zoom Speed: " + this.cinemaZoomSpeed.ToString());
        }
      }
      else if (e.TryConsume((Action) 136))
        this.panLeft = true;
      else if (e.TryConsume((Action) 137))
        this.panRight = true;
      else if (e.TryConsume((Action) 134))
        this.panUp = true;
      else if (e.TryConsume((Action) 135))
        this.panDown = true;
    }
    if (((KInputEvent) e).Consumed || !Object.op_Inequality((Object) OverlayMenu.Instance, (Object) null))
      return;
    ((KScreen) OverlayMenu.Instance).OnKeyDown(e);
  }

  public bool ChangeWorldInput(KButtonEvent e)
  {
    if (((KInputEvent) e).Consumed)
      return true;
    int index = -1;
    if (e.TryConsume((Action) 258))
      index = 0;
    else if (e.TryConsume((Action) 259))
      index = 1;
    else if (e.TryConsume((Action) 260))
      index = 2;
    else if (e.TryConsume((Action) 261))
      index = 3;
    else if (e.TryConsume((Action) 262))
      index = 4;
    else if (e.TryConsume((Action) 263))
      index = 5;
    else if (e.TryConsume((Action) 264))
      index = 6;
    else if (e.TryConsume((Action) 265))
      index = 7;
    else if (e.TryConsume((Action) 266))
      index = 8;
    else if (e.TryConsume((Action) 267))
      index = 9;
    if (index == -1)
      return false;
    List<int> asteroidIdsSorted = ClusterManager.Instance.GetDiscoveredAsteroidIDsSorted();
    if (index < asteroidIdsSorted.Count && index >= 0)
    {
      int id = asteroidIdsSorted[index];
      WorldContainer world = ClusterManager.Instance.GetWorld(id);
      if (Object.op_Inequality((Object) world, (Object) null) && world.IsDiscovered && ClusterManager.Instance.activeWorldId != world.id)
      {
        ManagementMenu.Instance.CloseClusterMap();
        this.ActiveWorldStarWipe(world.id);
      }
    }
    return true;
  }

  public void OnKeyUp(KButtonEvent e)
  {
    if (this.DisableUserCameraControl || CameraController.WithinInputField())
      return;
    if (e.TryConsume((Action) 6) || e.IsAction((Action) 5))
      this.panning = false;
    else if (this.FreeCameraEnabled && this.cinemaCamEnabled)
    {
      if (e.TryConsume((Action) 245))
        this.cinemaPanLeft = this.cinemaToggleLock && this.cinemaPanLeft;
      else if (e.TryConsume((Action) 246))
        this.cinemaPanRight = this.cinemaToggleLock && this.cinemaPanRight;
      else if (e.TryConsume((Action) 247))
        this.cinemaPanUp = this.cinemaToggleLock && this.cinemaPanUp;
      else if (e.TryConsume((Action) 248))
        this.cinemaPanDown = this.cinemaToggleLock && this.cinemaPanDown;
      else if (e.TryConsume((Action) 249))
      {
        this.cinemaZoomIn = this.cinemaToggleLock && this.cinemaZoomIn;
      }
      else
      {
        if (!e.TryConsume((Action) 250))
          return;
        this.cinemaZoomOut = this.cinemaToggleLock && this.cinemaZoomOut;
      }
    }
    else if (e.TryConsume((Action) 138))
      this.CameraGoHome();
    else if (e.TryConsume((Action) 136))
      this.panLeft = false;
    else if (e.TryConsume((Action) 137))
      this.panRight = false;
    else if (e.TryConsume((Action) 134))
    {
      this.panUp = false;
    }
    else
    {
      if (!e.TryConsume((Action) 135))
        return;
      this.panDown = false;
    }
  }

  public void ForcePanningState(bool state) => this.panning = false;

  public void CameraGoHome(float speed = 2f)
  {
    GameObject activeTelepad = GameUtil.GetActiveTelepad();
    if (!Object.op_Inequality((Object) activeTelepad, (Object) null) || !ClusterUtil.ActiveWorldHasPrinter())
      return;
    Vector3 pos;
    // ISSUE: explicit constructor call
    ((Vector3) ref pos).\u002Ector(TransformExtensions.GetPosition(activeTelepad.transform).x, TransformExtensions.GetPosition(activeTelepad.transform).y + 1f, TransformExtensions.GetPosition(this.transform).z);
    this.SetTargetPos(pos, 10f, true);
    this.SetOverrideZoomSpeed(speed);
  }

  public void CameraGoTo(Vector3 pos, float speed = 2f, bool playSound = true)
  {
    pos.z = TransformExtensions.GetPosition(this.transform).z;
    this.SetTargetPos(pos, 10f, playSound);
    this.SetOverrideZoomSpeed(speed);
  }

  public void SnapTo(Vector3 pos)
  {
    this.ClearFollowTarget();
    pos.z = -100f;
    this.targetPos = Vector3.zero;
    this.isTargetPosSet = false;
    TransformExtensions.SetPosition(this.transform, pos);
    this.keyPanDelta = Vector3.zero;
    this.OrthographicSize = this.targetOrthographicSize;
  }

  public void SnapTo(Vector3 pos, float orthographicSize)
  {
    this.targetOrthographicSize = orthographicSize;
    this.SnapTo(pos);
  }

  public void SetOverrideZoomSpeed(float tempZoomSpeed) => this.overrideZoomSpeed = tempZoomSpeed;

  public void SetTargetPos(Vector3 pos, float orthographic_size, bool playSound)
  {
    int cell = Grid.PosToCell(pos);
    if (!Grid.IsValidCell(cell) || (int) Grid.WorldIdx[cell] == (int) ClusterManager.INVALID_WORLD_IDX || Object.op_Equality((Object) ClusterManager.Instance.GetWorld((int) Grid.WorldIdx[cell]), (Object) null))
      return;
    this.ClearFollowTarget();
    if (playSound && !this.isTargetPosSet)
      KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Click_Notification"));
    pos.z = -100f;
    if ((int) Grid.WorldIdx[cell] != ClusterManager.Instance.activeWorldId)
    {
      this.targetOrthographicSize = 20f;
      this.ActiveWorldStarWipe((int) Grid.WorldIdx[cell], pos, callback: ((System.Action) (() =>
      {
        this.targetPos = pos;
        this.isTargetPosSet = true;
        this.OrthographicSize = orthographic_size + 5f;
        this.targetOrthographicSize = orthographic_size;
      })));
    }
    else
    {
      this.targetPos = pos;
      this.isTargetPosSet = true;
      this.targetOrthographicSize = orthographic_size;
    }
    PlayerController.Instance.CancelDragging();
    this.CheckMoveUnpause();
  }

  public void SetTargetPosForWorldChange(Vector3 pos, float orthographic_size, bool playSound)
  {
    int cell = Grid.PosToCell(pos);
    if (!Grid.IsValidCell(cell) || (int) Grid.WorldIdx[cell] == (int) ClusterManager.INVALID_WORLD_IDX || Object.op_Equality((Object) ClusterManager.Instance.GetWorld((int) Grid.WorldIdx[cell]), (Object) null))
      return;
    this.ClearFollowTarget();
    if (playSound && !this.isTargetPosSet)
      KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Click_Notification"));
    pos.z = -100f;
    this.targetPos = pos;
    this.isTargetPosSet = true;
    this.targetOrthographicSize = orthographic_size;
    PlayerController.Instance.CancelDragging();
    this.CheckMoveUnpause();
    this.SetPosition(pos);
    this.OrthographicSize = orthographic_size;
  }

  public void SetMaxOrthographicSize(float size) => this.maxOrthographicSize = size;

  public void SetPosition(Vector3 pos) => TransformExtensions.SetPosition(this.transform, pos);

  public IEnumerator DoCinematicZoom(float targetOrthographicSize)
  {
    this.cinemaCamEnabled = true;
    this.FreeCameraEnabled = true;
    this.targetOrthographicSize = targetOrthographicSize;
    while ((double) targetOrthographicSize - (double) this.OrthographicSize >= 1.0 / 1000.0)
      yield return (object) SequenceUtil.WaitForEndOfFrame;
    this.OrthographicSize = targetOrthographicSize;
    this.FreeCameraEnabled = false;
    this.cinemaCamEnabled = false;
  }

  private Vector3 PointUnderCursor(Vector3 mousePos, Camera cam)
  {
    Ray ray = cam.ScreenPointToRay(mousePos);
    Vector3 direction = ((Ray) ref ray).direction;
    Vector3 vector3 = Vector3.op_Multiply(direction, Mathf.Abs(TransformExtensions.GetPosition(((Component) cam).transform).z / direction.z));
    return Vector3.op_Addition(((Ray) ref ray).origin, vector3);
  }

  private void CinemaCamUpdate()
  {
    float unscaledDeltaTime = Time.unscaledDeltaTime;
    Camera main = Camera.main;
    Vector3 localPosition = TransformExtensions.GetLocalPosition(this.transform);
    float num1 = Mathf.Pow((float) this.cinemaZoomSpeed, 3f);
    if (this.cinemaZoomIn)
    {
      this.overrideZoomSpeed = -num1 / TuningData<CameraController.Tuning>.Get().cinemaZoomFactor;
      this.isTargetPosSet = false;
    }
    else if (this.cinemaZoomOut)
    {
      this.overrideZoomSpeed = num1 / TuningData<CameraController.Tuning>.Get().cinemaZoomFactor;
      this.isTargetPosSet = false;
    }
    else
      this.overrideZoomSpeed = 0.0f;
    if (this.cinemaToggleEasing)
      this.cinemaZoomVelocity += (this.overrideZoomSpeed - this.cinemaZoomVelocity) * this.cinemaEasing;
    else
      this.cinemaZoomVelocity = this.overrideZoomSpeed;
    if ((double) this.cinemaZoomVelocity != 0.0)
    {
      this.OrthographicSize = main.orthographicSize + (float) ((double) this.cinemaZoomVelocity * (double) unscaledDeltaTime * ((double) main.orthographicSize / 20.0));
      this.targetOrthographicSize = main.orthographicSize;
    }
    float num2 = num1 / TuningData<CameraController.Tuning>.Get().cinemaZoomToFactor;
    float num3 = this.keyPanningSpeed / 20f * main.orthographicSize;
    float num4 = num3 * (num1 / TuningData<CameraController.Tuning>.Get().cinemaPanToFactor);
    if (!this.isTargetPosSet && (double) this.targetOrthographicSize != (double) main.orthographicSize)
    {
      float num5 = Mathf.Min(num2 * unscaledDeltaTime, 0.1f);
      this.OrthographicSize = Mathf.Lerp(main.orthographicSize, this.targetOrthographicSize, num5);
    }
    Vector3 vector3_1 = Vector3.zero;
    if (this.isTargetPosSet)
    {
      float num6 = this.cinemaEasing * TuningData<CameraController.Tuning>.Get().targetZoomEasingFactor;
      float num7 = this.cinemaEasing * TuningData<CameraController.Tuning>.Get().targetPanEasingFactor;
      float num8 = this.targetOrthographicSize - main.orthographicSize;
      Vector3 vector3_2 = Vector3.op_Subtraction(this.targetPos, localPosition);
      float num9;
      float num10;
      if (!this.cinemaToggleEasing)
      {
        num9 = num2 * unscaledDeltaTime;
        num10 = num4 * unscaledDeltaTime;
      }
      else
      {
        DebugUtil.LogArgs(new object[3]
        {
          (object) "Min zoom of:",
          (object) (float) ((double) num2 * (double) unscaledDeltaTime),
          (object) (float) ((double) Mathf.Abs(num8) * (double) num6 * (double) unscaledDeltaTime)
        });
        num9 = Mathf.Min(num2 * unscaledDeltaTime, Mathf.Abs(num8) * num6 * unscaledDeltaTime);
        DebugUtil.LogArgs(new object[3]
        {
          (object) "Min pan of:",
          (object) (float) ((double) num4 * (double) unscaledDeltaTime),
          (object) (float) ((double) ((Vector3) ref vector3_2).magnitude * (double) num7 * (double) unscaledDeltaTime)
        });
        num10 = Mathf.Min(num4 * unscaledDeltaTime, ((Vector3) ref vector3_2).magnitude * num7 * unscaledDeltaTime);
      }
      float num11 = (double) Mathf.Abs(num8) >= (double) num9 ? Mathf.Sign(num8) * num9 : num8;
      vector3_1 = (double) ((Vector3) ref vector3_2).magnitude >= (double) num10 ? Vector3.op_Multiply(((Vector3) ref vector3_2).normalized, num10) : vector3_2;
      if ((double) Mathf.Abs(num11) < 1.0 / 1000.0 && (double) ((Vector3) ref vector3_1).magnitude < 1.0 / 1000.0)
      {
        this.isTargetPosSet = false;
        num11 = num8;
        vector3_1 = vector3_2;
      }
      this.OrthographicSize = main.orthographicSize + num11 * (main.orthographicSize / 20f);
    }
    if (!PlayerController.Instance.CanDrag())
      this.panning = false;
    Vector3 vector3_3 = Vector3.zero;
    if (this.panning)
    {
      vector3_3 = Vector3.op_UnaryNegation(PlayerController.Instance.GetWorldDragDelta());
      this.isTargetPosSet = false;
      if ((double) ((Vector3) ref vector3_3).magnitude > 0.0)
        this.ClearFollowTarget();
      this.keyPanDelta = Vector3.zero;
    }
    else
    {
      float num12 = num1 / TuningData<CameraController.Tuning>.Get().cinemaPanFactor;
      Vector3 zero = Vector3.zero;
      if (this.cinemaPanLeft)
      {
        this.ClearFollowTarget();
        zero.x = -num3 * num12;
        this.isTargetPosSet = false;
      }
      if (this.cinemaPanRight)
      {
        this.ClearFollowTarget();
        zero.x = num3 * num12;
        this.isTargetPosSet = false;
      }
      if (this.cinemaPanUp)
      {
        this.ClearFollowTarget();
        zero.y = num3 * num12;
        this.isTargetPosSet = false;
      }
      if (this.cinemaPanDown)
      {
        this.ClearFollowTarget();
        zero.y = -num3 * num12;
        this.isTargetPosSet = false;
      }
      this.keyPanDelta = !this.cinemaToggleEasing ? zero : Vector3.op_Addition(this.keyPanDelta, Vector3.op_Multiply(Vector3.op_Subtraction(zero, this.keyPanDelta), this.cinemaEasing));
    }
    Vector3 vector3_4 = Vector3.op_Addition(Vector3.op_Addition(Vector3.op_Addition(localPosition, vector3_1), vector3_3), Vector3.op_Multiply(this.keyPanDelta, unscaledDeltaTime));
    if (Object.op_Inequality((Object) this.followTarget, (Object) null))
    {
      vector3_4.x = this.followTargetPos.x;
      vector3_4.y = this.followTargetPos.y;
    }
    vector3_4.z = -100f;
    Vector3 vector3_5 = Vector3.op_Subtraction(vector3_4, TransformExtensions.GetLocalPosition(this.transform));
    if ((double) ((Vector3) ref vector3_5).magnitude <= 0.001)
      return;
    TransformExtensions.SetLocalPosition(this.transform, vector3_4);
  }

  private void NormalCamUpdate()
  {
    float unscaledDeltaTime = Time.unscaledDeltaTime;
    Camera main = Camera.main;
    this.smoothDt = (float) ((double) this.smoothDt * 2.0 / 3.0 + (double) unscaledDeltaTime / 3.0);
    float num1 = (double) this.overrideZoomSpeed != 0.0 ? this.overrideZoomSpeed : this.zoomSpeed;
    Vector3 localPosition = TransformExtensions.GetLocalPosition(this.transform);
    Vector3 mousePos = (double) this.overrideZoomSpeed != 0.0 ? new Vector3((float) Screen.width / 2f, (float) Screen.height / 2f, 0.0f) : KInputManager.GetMousePos();
    Vector3 vector3_1 = this.PointUnderCursor(mousePos, main);
    Vector3 viewportPoint1 = main.ScreenToViewportPoint(mousePos);
    float num2 = this.keyPanningSpeed / 20f * main.orthographicSize * Mathf.Min(unscaledDeltaTime / 0.0166666657f, 10f);
    float num3 = num1 * Mathf.Min(this.smoothDt, 0.3f);
    this.OrthographicSize = Mathf.Lerp(main.orthographicSize, this.targetOrthographicSize, num3);
    TransformExtensions.SetLocalPosition(this.transform, localPosition);
    Vector3 viewportPoint2 = main.WorldToViewportPoint(vector3_1);
    viewportPoint1.z = viewportPoint2.z;
    Vector3 vector3_2 = Vector3.op_Subtraction(main.ViewportToWorldPoint(viewportPoint2), main.ViewportToWorldPoint(viewportPoint1));
    if (this.isTargetPosSet)
    {
      vector3_2 = Vector3.op_Subtraction(Vector3.Lerp(localPosition, this.targetPos, num1 * this.smoothDt), localPosition);
      if ((double) ((Vector3) ref vector3_2).magnitude < 1.0 / 1000.0)
      {
        this.isTargetPosSet = false;
        vector3_2 = Vector3.op_Subtraction(this.targetPos, localPosition);
      }
    }
    if (!PlayerController.Instance.CanDrag())
      this.panning = false;
    Vector3 vector3_3 = Vector3.zero;
    if (this.panning)
    {
      vector3_3 = Vector3.op_UnaryNegation(PlayerController.Instance.GetWorldDragDelta());
      this.isTargetPosSet = false;
    }
    Vector3 vector3_4 = Vector3.op_Addition(Vector3.op_Addition(localPosition, vector3_2), vector3_3);
    if (this.panning)
    {
      if ((double) ((Vector3) ref vector3_3).magnitude > 0.0)
        this.ClearFollowTarget();
      this.keyPanDelta = Vector3.zero;
    }
    else if (!this.DisableUserCameraControl)
    {
      if (this.panLeft)
      {
        this.ClearFollowTarget();
        this.keyPanDelta.x -= num2;
        this.isTargetPosSet = false;
        this.overrideZoomSpeed = 0.0f;
      }
      if (this.panRight)
      {
        this.ClearFollowTarget();
        this.keyPanDelta.x += num2;
        this.isTargetPosSet = false;
        this.overrideZoomSpeed = 0.0f;
      }
      if (this.panUp)
      {
        this.ClearFollowTarget();
        this.keyPanDelta.y += num2;
        this.isTargetPosSet = false;
        this.overrideZoomSpeed = 0.0f;
      }
      if (this.panDown)
      {
        this.ClearFollowTarget();
        this.keyPanDelta.y -= num2;
        this.isTargetPosSet = false;
        this.overrideZoomSpeed = 0.0f;
      }
      if (KInputManager.currentControllerIsGamepad)
      {
        Vector2 vector2 = Vector2.op_Multiply(num2, KInputManager.steamInputInterpreter.GetSteamCameraMovement());
        if ((double) Mathf.Abs(vector2.x) > (double) Mathf.Epsilon || (double) Mathf.Abs(vector2.y) > (double) Mathf.Epsilon)
        {
          this.ClearFollowTarget();
          this.isTargetPosSet = false;
          this.overrideZoomSpeed = 0.0f;
        }
        this.keyPanDelta = Vector3.op_Addition(this.keyPanDelta, new Vector3(vector2.x, vector2.y, 0.0f));
      }
      Vector3 vector3_5;
      // ISSUE: explicit constructor call
      ((Vector3) ref vector3_5).\u002Ector(Mathf.Lerp(0.0f, this.keyPanDelta.x, this.smoothDt * this.keyPanningEasing), Mathf.Lerp(0.0f, this.keyPanDelta.y, this.smoothDt * this.keyPanningEasing), 0.0f);
      this.keyPanDelta = Vector3.op_Subtraction(this.keyPanDelta, vector3_5);
      vector3_4.x += vector3_5.x;
      vector3_4.y += vector3_5.y;
    }
    if (Object.op_Inequality((Object) this.followTarget, (Object) null))
    {
      vector3_4.x = this.followTargetPos.x;
      vector3_4.y = this.followTargetPos.y;
    }
    vector3_4.z = -100f;
    Vector3 vector3_6 = Vector3.op_Subtraction(vector3_4, TransformExtensions.GetLocalPosition(this.transform));
    if ((double) ((Vector3) ref vector3_6).magnitude <= 0.001)
      return;
    TransformExtensions.SetLocalPosition(this.transform, vector3_4);
  }

  private void Update()
  {
    if (Object.op_Equality((Object) Game.Instance, (Object) null) || !Game.Instance.timelapser.CapturingTimelapseScreenshot)
    {
      if (this.FreeCameraEnabled && this.cinemaCamEnabled)
        this.CinemaCamUpdate();
      else
        this.NormalCamUpdate();
    }
    if (Object.op_Inequality((Object) this.infoText, (Object) null) && (double) ((Graphic) this.infoText).color.a > 0.0)
    {
      Color color = ((Graphic) this.infoText).color;
      color.a = Mathf.Max(0.0f, ((Graphic) this.infoText).color.a - Time.unscaledDeltaTime * 0.5f);
      ((Graphic) this.infoText).color = color;
    }
    this.ConstrainToWorld();
    Vector3 vector3 = this.PointUnderCursor(KInputManager.GetMousePos(), Camera.main);
    Shader.SetGlobalVector("_WorldCameraPos", new Vector4(TransformExtensions.GetPosition(this.transform).x, TransformExtensions.GetPosition(this.transform).y, TransformExtensions.GetPosition(this.transform).z, Camera.main.orthographicSize));
    Shader.SetGlobalVector("_WorldCursorPos", new Vector4(vector3.x, vector3.y, 0.0f, 0.0f));
    this.VisibleArea.Update();
    this.soundCuller = SoundCuller.CreateCuller();
  }

  private Vector3 GetFollowPos()
  {
    if (!Object.op_Inequality((Object) this.followTarget, (Object) null))
      return Vector3.zero;
    Vector3 followPos = TransformExtensions.GetPosition(((Component) this.followTarget).transform);
    KAnimControllerBase component = ((Component) this.followTarget).GetComponent<KAnimControllerBase>();
    if (Object.op_Inequality((Object) component, (Object) null))
      followPos = component.GetWorldPivot();
    return followPos;
  }

  private void ConstrainToWorld()
  {
    if (Object.op_Inequality((Object) Game.Instance, (Object) null) && Game.Instance.IsLoading() || this.FreeCameraEnabled)
      return;
    Camera main = Camera.main;
    float num1 = 0.33f;
    Ray ray1 = main.ViewportPointToRay(Vector3.op_Addition(Vector3.zero, Vector3.op_Multiply(Vector3.one, num1)));
    Ray ray2 = main.ViewportPointToRay(Vector3.op_Subtraction(Vector3.one, Vector3.op_Multiply(Vector3.one, num1)));
    float num2 = Mathf.Abs(((Ray) ref ray1).origin.z / ((Ray) ref ray1).direction.z);
    float num3 = Mathf.Abs(((Ray) ref ray2).origin.z / ((Ray) ref ray2).direction.z);
    Vector3 point1 = ((Ray) ref ray1).GetPoint(num2);
    Vector3 point2 = ((Ray) ref ray2).GetPoint(num3);
    Vector2 vector2_1 = Vector2.zero;
    Vector2 vector2_2;
    // ISSUE: explicit constructor call
    ((Vector2) ref vector2_2).\u002Ector(Grid.WidthInMeters, Grid.HeightInMeters);
    Vector2 vector2_3 = vector2_2;
    if (Object.op_Inequality((Object) ClusterManager.Instance, (Object) null))
    {
      WorldContainer activeWorld = ClusterManager.Instance.activeWorld;
      vector2_1 = Vector2.op_Multiply(activeWorld.minimumBounds, Grid.CellSizeInMeters);
      vector2_2 = Vector2.op_Multiply(activeWorld.maximumBounds, Grid.CellSizeInMeters);
      vector2_3 = Vector2.op_Multiply(new Vector2((float) activeWorld.Width, (float) activeWorld.Height), Grid.CellSizeInMeters);
    }
    if ((double) point2.x - (double) point1.x > (double) vector2_3.x || (double) point2.y - (double) point1.y > (double) vector2_3.y)
      return;
    Vector3 vector3_1 = Vector3.op_Subtraction(TransformExtensions.GetPosition(this.transform), ((Ray) ref ray1).origin);
    Vector3 vector3_2 = point1;
    vector3_2.x = Mathf.Max(vector2_1.x, vector3_2.x);
    vector3_2.y = Mathf.Max(vector2_1.y * Grid.CellSizeInMeters, vector3_2.y);
    ((Ray) ref ray1).origin = vector3_2;
    ((Ray) ref ray1).direction = Vector3.op_UnaryNegation(((Ray) ref ray1).direction);
    TransformExtensions.SetPosition(this.transform, Vector3.op_Addition(((Ray) ref ray1).GetPoint(num2), vector3_1));
    Vector3 vector3_3 = Vector3.op_Subtraction(TransformExtensions.GetPosition(this.transform), ((Ray) ref ray2).origin);
    Vector3 vector3_4 = point2;
    vector3_4.x = Mathf.Min(vector2_2.x, vector3_4.x);
    vector3_4.y = Mathf.Min(vector2_2.y * this.MAX_Y_SCALE, vector3_4.y);
    ((Ray) ref ray2).origin = vector3_4;
    ((Ray) ref ray2).direction = Vector3.op_UnaryNegation(((Ray) ref ray2).direction);
    Vector3 vector3_5 = Vector3.op_Addition(((Ray) ref ray2).GetPoint(num3), vector3_3);
    vector3_5.z = -100f;
    TransformExtensions.SetPosition(this.transform, vector3_5);
  }

  public void Save(BinaryWriter writer)
  {
    Util.Write(writer, TransformExtensions.GetPosition(this.transform));
    Util.Write(writer, this.transform.localScale);
    Util.Write(writer, this.transform.rotation);
    writer.Write(this.targetOrthographicSize);
    CameraSaveData.position = TransformExtensions.GetPosition(this.transform);
    CameraSaveData.localScale = this.transform.localScale;
    CameraSaveData.rotation = this.transform.rotation;
  }

  private void Restore()
  {
    if (!CameraSaveData.valid)
      return;
    int cell = Grid.PosToCell(CameraSaveData.position);
    if (Grid.IsValidCell(cell) && !Grid.IsVisible(cell))
    {
      Debug.LogWarning((object) "Resetting Camera Position... camera was saved in an undiscovered area of the map.");
      this.CameraGoHome();
    }
    else
    {
      TransformExtensions.SetPosition(this.transform, CameraSaveData.position);
      this.transform.localScale = CameraSaveData.localScale;
      this.transform.rotation = CameraSaveData.rotation;
      this.targetOrthographicSize = Mathf.Clamp(CameraSaveData.orthographicsSize, this.minOrthographicSize, this.FreeCameraEnabled ? TuningData<CameraController.Tuning>.Get().maxOrthographicSizeDebug : this.maxOrthographicSize);
      this.SnapTo(TransformExtensions.GetPosition(this.transform));
    }
  }

  private void OnMRTSetupComplete(Camera cam) => this.cameras.Add(cam);

  public bool IsAudibleSound(Vector2 pos) => this.soundCuller.IsAudible(pos);

  public bool IsAudibleSound(Vector3 pos, EventReference event_ref)
  {
    string eventReferencePath = KFMOD.GetEventReferencePath(event_ref);
    return this.soundCuller.IsAudible(Vector2.op_Implicit(pos), HashedString.op_Implicit(eventReferencePath));
  }

  public bool IsAudibleSound(Vector3 pos, HashedString sound_path) => this.soundCuller.IsAudible(Vector2.op_Implicit(pos), sound_path);

  public Vector3 GetVerticallyScaledPosition(Vector3 pos, bool objectIsSelectedAndVisible = false) => this.soundCuller.GetVerticallyScaledPosition(pos, objectIsSelectedAndVisible);

  public bool IsVisiblePos(Vector3 pos)
  {
    GridArea visibleArea = GridVisibleArea.GetVisibleArea();
    return Vector2I.op_LessThanOrEqual(visibleArea.Min, Vector2.op_Implicit(pos)) && Vector2I.op_LessThanOrEqual(Vector2.op_Implicit(pos), visibleArea.Max);
  }

  protected virtual void OnCleanUp() => CameraController.Instance = (CameraController) null;

  public void SetFollowTarget(Transform follow_target)
  {
    this.ClearFollowTarget();
    if (Object.op_Equality((Object) follow_target, (Object) null))
      return;
    this.followTarget = follow_target;
    this.OrthographicSize = 6f;
    this.targetOrthographicSize = 6f;
    Vector3 followPos = this.GetFollowPos();
    this.followTargetPos = new Vector3(followPos.x, followPos.y, TransformExtensions.GetPosition(this.transform).z);
    TransformExtensions.SetPosition(this.transform, this.followTargetPos);
    ((Component) this.followTarget).GetComponent<KMonoBehaviour>().Trigger(-1506069671, (object) null);
  }

  public void ClearFollowTarget()
  {
    if (Object.op_Equality((Object) this.followTarget, (Object) null))
      return;
    ((Component) this.followTarget).GetComponent<KMonoBehaviour>().Trigger(-485480405, (object) null);
    this.followTarget = (Transform) null;
  }

  public void UpdateFollowTarget()
  {
    if (!Object.op_Inequality((Object) this.followTarget, (Object) null))
      return;
    Vector3 followPos = this.GetFollowPos();
    Vector2 vector2_1;
    // ISSUE: explicit constructor call
    ((Vector2) ref vector2_1).\u002Ector(TransformExtensions.GetLocalPosition(this.transform).x, TransformExtensions.GetLocalPosition(this.transform).y);
    byte worldIdx = Grid.WorldIdx[Grid.PosToCell(followPos)];
    if (ClusterManager.Instance.activeWorldId != (int) worldIdx)
    {
      Transform followTarget = this.followTarget;
      this.SetFollowTarget((Transform) null);
      ClusterManager.Instance.SetActiveWorld((int) worldIdx);
      this.SetFollowTarget(followTarget);
    }
    else
    {
      Vector2 vector2_2 = Vector2.Lerp(vector2_1, Vector2.op_Implicit(followPos), Time.unscaledDeltaTime * 25f);
      this.followTargetPos = new Vector3(vector2_2.x, vector2_2.y, TransformExtensions.GetLocalPosition(this.transform).z);
    }
  }

  public void RenderForTimelapser(ref RenderTexture tex)
  {
    this.RenderCameraForTimelapse(this.baseCamera, ref tex, this.timelapseCameraCullingMask);
    CameraClearFlags clearFlags = this.overlayCamera.clearFlags;
    this.overlayCamera.clearFlags = (CameraClearFlags) 4;
    this.RenderCameraForTimelapse(this.overlayCamera, ref tex, this.timelapseOverlayCameraCullingMask);
    this.overlayCamera.clearFlags = clearFlags;
  }

  private void RenderCameraForTimelapse(
    Camera cam,
    ref RenderTexture tex,
    LayerMask mask,
    float overrideAspect = -1f)
  {
    int cullingMask = cam.cullingMask;
    RenderTexture targetTexture = cam.targetTexture;
    cam.targetTexture = tex;
    cam.aspect = (float) ((Texture) tex).width / (float) ((Texture) tex).height;
    if ((double) overrideAspect != -1.0)
      cam.aspect = overrideAspect;
    if (LayerMask.op_Implicit(mask) != -1)
      cam.cullingMask = LayerMask.op_Implicit(mask);
    cam.Render();
    cam.ResetAspect();
    cam.cullingMask = cullingMask;
    cam.targetTexture = targetTexture;
  }

  private void CheckMoveUnpause()
  {
    if (!this.cinemaCamEnabled || !this.cinemaUnpauseNextMove)
      return;
    this.cinemaUnpauseNextMove = !this.cinemaUnpauseNextMove;
    if (!SpeedControlScreen.Instance.IsPaused)
      return;
    SpeedControlScreen.Instance.Unpause(false);
  }

  public class Tuning : TuningData<CameraController.Tuning>
  {
    public float maxOrthographicSizeDebug;
    public float cinemaZoomFactor = 100f;
    public float cinemaPanFactor = 50f;
    public float cinemaZoomToFactor = 100f;
    public float cinemaPanToFactor = 50f;
    public float targetZoomEasingFactor = 400f;
    public float targetPanEasingFactor = 100f;
  }
}
