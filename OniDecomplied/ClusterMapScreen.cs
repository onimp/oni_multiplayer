// Decompiled with JetBrains decompiler
// Type: ClusterMapScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ClusterMapScreen : KScreen
{
  public static ClusterMapScreen Instance;
  public GameObject cellVisContainer;
  public GameObject terrainVisContainer;
  public GameObject mobileVisContainer;
  public GameObject telescopeVisContainer;
  public GameObject POIVisContainer;
  public GameObject FXVisContainer;
  public ClusterMapVisualizer cellVisPrefab;
  public ClusterMapVisualizer terrainVisPrefab;
  public ClusterMapVisualizer mobileVisPrefab;
  public ClusterMapVisualizer staticVisPrefab;
  public Color rocketPathColor;
  public Color rocketSelectedPathColor;
  public Color rocketPreviewPathColor;
  private ClusterMapHex m_selectedHex;
  private ClusterMapHex m_hoveredHex;
  private ClusterGridEntity m_selectedEntity;
  public KButton closeButton;
  private const float ZOOM_SCALE_MIN = 50f;
  private const float ZOOM_SCALE_MAX = 150f;
  private const float ZOOM_SCALE_INCREMENT = 25f;
  private const float ZOOM_SCALE_SPEED = 4f;
  private const float ZOOM_NAME_THRESHOLD = 115f;
  private float m_currentZoomScale = 75f;
  private float m_targetZoomScale = 75f;
  private ClusterMapPath m_previewMapPath;
  private Dictionary<ClusterGridEntity, ClusterMapVisualizer> m_gridEntityVis = new Dictionary<ClusterGridEntity, ClusterMapVisualizer>();
  private Dictionary<ClusterGridEntity, ClusterMapVisualizer> m_gridEntityAnims = new Dictionary<ClusterGridEntity, ClusterMapVisualizer>();
  private Dictionary<AxialI, ClusterMapVisualizer> m_cellVisByLocation = new Dictionary<AxialI, ClusterMapVisualizer>();
  private Action<object> m_onDestinationChangedDelegate;
  private Action<object> m_onSelectObjectDelegate;
  [SerializeField]
  private KScrollRect mapScrollRect;
  [SerializeField]
  private float scrollSpeed = 15f;
  public GameObject selectMarkerPrefab;
  public ClusterMapPathDrawer pathDrawer;
  private SelectMarker m_selectMarker;
  private bool movingToTargetNISPosition;
  private Vector3 targetNISPosition;
  private float targetNISZoom;
  private AxialI selectOnMoveNISComplete;
  private ClusterMapScreen.Mode m_mode;
  private ClusterDestinationSelector m_destinationSelector;
  private bool m_closeOnSelect;
  private Coroutine activeMoveToTargetRoutine;
  public float floatCycleScale = 4f;
  public float floatCycleOffset = 0.75f;
  public float floatCycleSpeed = 0.75f;

  public static void DestroyInstance() => ClusterMapScreen.Instance = (ClusterMapScreen) null;

  public ClusterMapVisualizer GetEntityVisAnim(ClusterGridEntity entity) => this.m_gridEntityAnims.ContainsKey(entity) ? this.m_gridEntityAnims[entity] : (ClusterMapVisualizer) null;

  public virtual float GetSortKey() => this.isEditing ? 50f : 20f;

  public float CurrentZoomPercentage() => (float) (((double) this.m_currentZoomScale - 50.0) / 100.0);

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.m_selectMarker = Util.KInstantiateUI<SelectMarker>(this.selectMarkerPrefab, ((Component) this).gameObject, false);
    ((Component) this.m_selectMarker).gameObject.SetActive(false);
    ClusterMapScreen.Instance = this;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    Debug.Assert(Vector2.op_Equality(Util.rectTransform((Component) this.cellVisPrefab).sizeDelta, new Vector2(2f, 2f)), (object) "The radius of the cellVisPrefab hex must be 1");
    Debug.Assert(Vector2.op_Equality(Util.rectTransform((Component) this.terrainVisPrefab).sizeDelta, new Vector2(2f, 2f)), (object) "The radius of the terrainVisPrefab hex must be 1");
    Debug.Assert(Vector2.op_Equality(Util.rectTransform((Component) this.mobileVisPrefab).sizeDelta, new Vector2(2f, 2f)), (object) "The radius of the mobileVisPrefab hex must be 1");
    Debug.Assert(Vector2.op_Equality(Util.rectTransform((Component) this.staticVisPrefab).sizeDelta, new Vector2(2f, 2f)), (object) "The radius of the staticVisPrefab hex must be 1");
    int maxR;
    int maxQ;
    this.GenerateGridVis(out int _, out maxR, out int _, out maxQ);
    this.Show(false);
    ((ScrollRect) this.mapScrollRect).content.sizeDelta = new Vector2((float) (maxR * 4), (float) (maxQ * 4));
    ((Transform) ((ScrollRect) this.mapScrollRect).content).localScale = new Vector3(this.m_currentZoomScale, this.m_currentZoomScale, 1f);
    this.m_onDestinationChangedDelegate = new Action<object>(this.OnDestinationChanged);
    this.m_onSelectObjectDelegate = new Action<object>(this.OnSelectObject);
    ((KMonoBehaviour) this).Subscribe(1980521255, new Action<object>(this.UpdateVis));
  }

  protected void MoveToNISPosition()
  {
    if (!this.movingToTargetNISPosition)
      return;
    Vector3 vector3;
    // ISSUE: explicit constructor call
    ((Vector3) ref vector3).\u002Ector(-this.targetNISPosition.x * ((Transform) ((ScrollRect) this.mapScrollRect).content).localScale.x, -this.targetNISPosition.y * ((Transform) ((ScrollRect) this.mapScrollRect).content).localScale.y, this.targetNISPosition.z);
    this.m_targetZoomScale = Mathf.Lerp(this.m_targetZoomScale, this.targetNISZoom, Time.unscaledDeltaTime * 2f);
    TransformExtensions.SetLocalPosition((Transform) ((ScrollRect) this.mapScrollRect).content, Vector3.Lerp(TransformExtensions.GetLocalPosition((Transform) ((ScrollRect) this.mapScrollRect).content), vector3, Time.unscaledDeltaTime * 2.5f));
    float num = Vector3.Distance(TransformExtensions.GetLocalPosition((Transform) ((ScrollRect) this.mapScrollRect).content), vector3);
    if ((double) num >= 100.0)
      return;
    ClusterMapHex component = ((Component) this.m_cellVisByLocation[this.selectOnMoveNISComplete]).GetComponent<ClusterMapHex>();
    if (Object.op_Inequality((Object) this.m_selectedHex, (Object) component))
      this.SelectHex(component);
    if ((double) num >= 10.0)
      return;
    this.movingToTargetNISPosition = false;
  }

  public void SetTargetFocusPosition(AxialI targetPosition, float delayBeforeMove = 0.5f)
  {
    if (this.activeMoveToTargetRoutine != null)
      ((MonoBehaviour) this).StopCoroutine(this.activeMoveToTargetRoutine);
    this.activeMoveToTargetRoutine = ((MonoBehaviour) this).StartCoroutine(this.MoveToTargetRoutine(targetPosition, delayBeforeMove));
  }

  private IEnumerator MoveToTargetRoutine(AxialI targetPosition, float delayBeforeMove)
  {
    delayBeforeMove = Mathf.Max(delayBeforeMove, 0.0f);
    yield return (object) SequenceUtil.WaitForSecondsRealtime(delayBeforeMove);
    this.targetNISPosition = AxialUtil.AxialToWorld((float) targetPosition.r, (float) targetPosition.q);
    this.targetNISZoom = 150f;
    this.movingToTargetNISPosition = true;
    this.selectOnMoveNISComplete = targetPosition;
  }

  public virtual void OnKeyDown(KButtonEvent e)
  {
    if (!((KInputEvent) e).Consumed && (e.IsAction((Action) 7) || e.IsAction((Action) 8)))
    {
      List<RaycastResult> raycastResultList = new List<RaycastResult>();
      PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
      pointerEventData.position = Vector2.op_Implicit(KInputManager.GetMousePos());
      EventSystem current = EventSystem.current;
      if (Object.op_Inequality((Object) current, (Object) null))
      {
        current.RaycastAll(pointerEventData, raycastResultList);
        bool flag = false;
        foreach (RaycastResult raycastResult in raycastResultList)
        {
          if (!((RaycastResult) ref raycastResult).gameObject.transform.IsChildOf(((KMonoBehaviour) this).transform))
          {
            flag = true;
            break;
          }
        }
        if (!flag)
        {
          this.m_targetZoomScale = Mathf.Clamp(this.m_targetZoomScale + (!KInputManager.currentControllerIsGamepad ? Input.mouseScrollDelta.y * 25f : (float) (25.0 * (e.IsAction((Action) 7) ? 1.0 : -1.0))), 50f, 150f);
          e.TryConsume((Action) 7);
          if (!((KInputEvent) e).Consumed)
            e.TryConsume((Action) 8);
        }
      }
    }
    CameraController.Instance.ChangeWorldInput(e);
    base.OnKeyDown(e);
  }

  public bool TryHandleCancel()
  {
    if (this.m_mode != ClusterMapScreen.Mode.SelectDestination || this.m_closeOnSelect)
      return false;
    this.SetMode(ClusterMapScreen.Mode.Default);
    return true;
  }

  public void ShowInSelectDestinationMode(ClusterDestinationSelector destination_selector)
  {
    this.m_destinationSelector = destination_selector;
    if (!((Component) this).gameObject.activeSelf)
    {
      ManagementMenu.Instance.ToggleClusterMap();
      this.m_closeOnSelect = true;
    }
    this.SetSelectedEntity(((Component) destination_selector).GetComponent<ClusterGridEntity>());
    this.m_selectedHex = !Object.op_Inequality((Object) this.m_selectedEntity, (Object) null) ? ((Component) this.m_cellVisByLocation[destination_selector.GetMyWorldLocation()]).GetComponent<ClusterMapHex>() : ((Component) this.m_cellVisByLocation[this.m_selectedEntity.Location]).GetComponent<ClusterMapHex>();
    this.SetMode(ClusterMapScreen.Mode.SelectDestination);
  }

  private void SetMode(ClusterMapScreen.Mode mode)
  {
    this.m_mode = mode;
    if (this.m_mode == ClusterMapScreen.Mode.Default)
      this.m_destinationSelector = (ClusterDestinationSelector) null;
    this.UpdateVis();
  }

  public ClusterMapScreen.Mode GetMode() => this.m_mode;

  protected virtual void OnShow(bool show)
  {
    base.OnShow(show);
    if (show)
    {
      this.MoveToNISPosition();
      this.UpdateVis();
      if (this.m_mode == ClusterMapScreen.Mode.Default)
        this.TrySelectDefault();
      Game.Instance.Subscribe(-1991583975, new Action<object>(this.OnFogOfWarRevealed));
      Game.Instance.Subscribe(-1554423969, new Action<object>(this.OnNewTelescopeTarget));
      Game.Instance.Subscribe(-1298331547, new Action<object>(this.OnClusterLocationChanged));
      ClusterMapSelectTool.Instance.Activate();
      this.SetShowingNonClusterMapHud(false);
      CameraController.Instance.DisableUserCameraControl = true;
      AudioMixer.instance.Start(AudioMixerSnapshots.Get().MENUStarmapNotPausedSnapshot);
      MusicManager.instance.PlaySong("Music_Starmap");
      this.UpdateTearStatus();
    }
    else
    {
      Game.Instance.Unsubscribe(-1554423969, new Action<object>(this.OnNewTelescopeTarget));
      Game.Instance.Unsubscribe(-1991583975, new Action<object>(this.OnFogOfWarRevealed));
      Game.Instance.Unsubscribe(-1298331547, new Action<object>(this.OnClusterLocationChanged));
      this.m_mode = ClusterMapScreen.Mode.Default;
      this.m_closeOnSelect = false;
      this.m_destinationSelector = (ClusterDestinationSelector) null;
      SelectTool.Instance.Activate();
      this.SetShowingNonClusterMapHud(true);
      CameraController.Instance.DisableUserCameraControl = false;
      AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MENUStarmapNotPausedSnapshot);
      if (!MusicManager.instance.SongIsPlaying("Music_Starmap"))
        return;
      MusicManager.instance.StopSong("Music_Starmap");
    }
  }

  private void SetShowingNonClusterMapHud(bool show)
  {
    ((Component) PlanScreen.Instance).gameObject.SetActive(show);
    ((Component) ToolMenu.Instance).gameObject.SetActive(show);
    ((Component) OverlayScreen.Instance).gameObject.SetActive(show);
  }

  private void SetSelectedEntity(ClusterGridEntity entity, bool frameDelay = false)
  {
    if (Object.op_Inequality((Object) this.m_selectedEntity, (Object) null))
    {
      this.m_selectedEntity.Unsubscribe(543433792, this.m_onDestinationChangedDelegate);
      this.m_selectedEntity.Unsubscribe(-1503271301, this.m_onSelectObjectDelegate);
    }
    this.m_selectedEntity = entity;
    if (Object.op_Inequality((Object) this.m_selectedEntity, (Object) null))
    {
      this.m_selectedEntity.Subscribe(543433792, this.m_onDestinationChangedDelegate);
      this.m_selectedEntity.Subscribe(-1503271301, this.m_onSelectObjectDelegate);
    }
    KSelectable new_selected = Object.op_Inequality((Object) this.m_selectedEntity, (Object) null) ? ((Component) this.m_selectedEntity).GetComponent<KSelectable>() : (KSelectable) null;
    if (frameDelay)
      ClusterMapSelectTool.Instance.SelectNextFrame(new_selected);
    else
      ClusterMapSelectTool.Instance.Select(new_selected);
  }

  private void OnDestinationChanged(object data) => this.UpdateVis();

  private void OnSelectObject(object data)
  {
    if (Object.op_Equality((Object) this.m_selectedEntity, (Object) null))
      return;
    KSelectable component = ((Component) this.m_selectedEntity).GetComponent<KSelectable>();
    if (Object.op_Equality((Object) component, (Object) null) || component.IsSelected)
      return;
    this.SetSelectedEntity((ClusterGridEntity) null);
    if (this.m_mode == ClusterMapScreen.Mode.SelectDestination)
    {
      if (this.m_closeOnSelect)
        ManagementMenu.Instance.CloseAll();
      else
        this.SetMode(ClusterMapScreen.Mode.Default);
    }
    this.UpdateVis();
  }

  private void OnFogOfWarRevealed(object data = null) => this.UpdateVis();

  private void OnNewTelescopeTarget(object data = null) => this.UpdateVis();

  private void Update()
  {
    if (!KInputManager.currentControllerIsGamepad)
      return;
    this.mapScrollRect.AnalogUpdate(Vector2.op_Multiply(KInputManager.steamInputInterpreter.GetSteamCameraMovement(), this.scrollSpeed));
  }

  private void TrySelectDefault()
  {
    if (Object.op_Inequality((Object) this.m_selectedHex, (Object) null) && Object.op_Inequality((Object) this.m_selectedEntity, (Object) null))
    {
      this.UpdateVis();
    }
    else
    {
      WorldContainer activeWorld = ClusterManager.Instance.activeWorld;
      if (Object.op_Equality((Object) activeWorld, (Object) null))
        return;
      ClusterGridEntity component = ((Component) activeWorld).GetComponent<ClusterGridEntity>();
      if (Object.op_Equality((Object) component, (Object) null))
        return;
      this.SelectEntity(component);
    }
  }

  private void GenerateGridVis(out int minR, out int maxR, out int minQ, out int maxQ)
  {
    minR = int.MaxValue;
    maxR = int.MinValue;
    minQ = int.MaxValue;
    maxQ = int.MinValue;
    foreach (KeyValuePair<AxialI, List<ClusterGridEntity>> cellContent in ClusterGrid.Instance.cellContents)
    {
      ClusterMapVisualizer clusterMapVisualizer = Object.Instantiate<ClusterMapVisualizer>(this.cellVisPrefab, Vector3.zero, Quaternion.identity, this.cellVisContainer.transform);
      RectTransform rectTransform = Util.rectTransform((Component) clusterMapVisualizer);
      AxialI axialI = cellContent.Key;
      Vector3 world = ((AxialI) ref axialI).ToWorld();
      TransformExtensions.SetLocalPosition((Transform) rectTransform, world);
      ((Component) clusterMapVisualizer).gameObject.SetActive(true);
      ClusterMapHex component = ((Component) clusterMapVisualizer).GetComponent<ClusterMapHex>();
      component.SetLocation(cellContent.Key);
      this.m_cellVisByLocation.Add(cellContent.Key, clusterMapVisualizer);
      ref int local1 = ref minR;
      int num1 = minR;
      axialI = component.location;
      int r1 = ((AxialI) ref axialI).R;
      int num2 = Mathf.Min(num1, r1);
      local1 = num2;
      ref int local2 = ref maxR;
      int num3 = maxR;
      axialI = component.location;
      int r2 = ((AxialI) ref axialI).R;
      int num4 = Mathf.Max(num3, r2);
      local2 = num4;
      ref int local3 = ref minQ;
      int num5 = minQ;
      axialI = component.location;
      int q1 = ((AxialI) ref axialI).Q;
      int num6 = Mathf.Min(num5, q1);
      local3 = num6;
      ref int local4 = ref maxQ;
      int num7 = maxQ;
      axialI = component.location;
      int q2 = ((AxialI) ref axialI).Q;
      int num8 = Mathf.Max(num7, q2);
      local4 = num8;
    }
    this.SetupVisGameObjects();
    this.UpdateVis();
  }

  public Transform GetGridEntityNameTarget(ClusterGridEntity entity)
  {
    ClusterMapVisualizer clusterMapVisualizer;
    return (double) this.m_currentZoomScale >= 115.0 && this.m_gridEntityVis.TryGetValue(entity, out clusterMapVisualizer) ? clusterMapVisualizer.nameTarget : (Transform) null;
  }

  public virtual void ScreenUpdate(bool topLevel)
  {
    this.m_currentZoomScale = Mathf.Lerp(this.m_currentZoomScale, this.m_targetZoomScale, Mathf.Min(4f * Time.unscaledDeltaTime, 0.9f));
    Vector2 vector2 = Vector2.op_Implicit(KInputManager.GetMousePos());
    Vector3 vector3_1 = ((Transform) ((ScrollRect) this.mapScrollRect).content).InverseTransformPoint(Vector2.op_Implicit(vector2));
    ((Transform) ((ScrollRect) this.mapScrollRect).content).localScale = new Vector3(this.m_currentZoomScale, this.m_currentZoomScale, 1f);
    Vector3 vector3_2 = ((Transform) ((ScrollRect) this.mapScrollRect).content).InverseTransformPoint(Vector2.op_Implicit(vector2));
    RectTransform content = ((ScrollRect) this.mapScrollRect).content;
    ((Transform) content).localPosition = Vector3.op_Addition(((Transform) content).localPosition, Vector3.op_Multiply(Vector3.op_Subtraction(vector3_2, vector3_1), this.m_currentZoomScale));
    this.MoveToNISPosition();
    this.FloatyAsteroidAnimation();
  }

  private void FloatyAsteroidAnimation()
  {
    float num = 0.0f;
    foreach (Component worldContainer in (IEnumerable<WorldContainer>) ClusterManager.Instance.WorldContainers)
    {
      AsteroidGridEntity component = worldContainer.GetComponent<AsteroidGridEntity>();
      if (Object.op_Inequality((Object) component, (Object) null) && this.m_gridEntityVis.ContainsKey((ClusterGridEntity) component) && ClusterMapScreen.GetRevealLevel((ClusterGridEntity) component) == ClusterRevealLevel.Visible)
        this.m_gridEntityVis[(ClusterGridEntity) component].GetFirstAnimController().Offset = Vector2.op_Implicit(new Vector2(0.0f, this.floatCycleOffset + this.floatCycleScale * Mathf.Sin(this.floatCycleSpeed * (num + GameClock.Instance.GetTime()))));
      ++num;
    }
  }

  private void SetupVisGameObjects()
  {
    foreach (KeyValuePair<AxialI, List<ClusterGridEntity>> cellContent in ClusterGrid.Instance.cellContents)
    {
      foreach (ClusterGridEntity clusterGridEntity in cellContent.Value)
      {
        int cellRevealLevel = (int) ClusterGrid.Instance.GetCellRevealLevel(cellContent.Key);
        int isVisibleInFow = (int) clusterGridEntity.IsVisibleInFOW;
        ClusterRevealLevel revealLevel = ClusterMapScreen.GetRevealLevel(clusterGridEntity);
        if (clusterGridEntity.IsVisible && revealLevel != ClusterRevealLevel.Hidden && !this.m_gridEntityVis.ContainsKey(clusterGridEntity))
        {
          ClusterMapVisualizer clusterMapVisualizer1 = (ClusterMapVisualizer) null;
          GameObject gameObject = (GameObject) null;
          switch (clusterGridEntity.Layer)
          {
            case EntityLayer.Asteroid:
              clusterMapVisualizer1 = this.terrainVisPrefab;
              gameObject = this.terrainVisContainer;
              break;
            case EntityLayer.Craft:
              clusterMapVisualizer1 = this.mobileVisPrefab;
              gameObject = this.mobileVisContainer;
              break;
            case EntityLayer.POI:
              clusterMapVisualizer1 = this.staticVisPrefab;
              gameObject = this.POIVisContainer;
              break;
            case EntityLayer.Telescope:
              clusterMapVisualizer1 = this.staticVisPrefab;
              gameObject = this.telescopeVisContainer;
              break;
            case EntityLayer.Payload:
              clusterMapVisualizer1 = this.mobileVisPrefab;
              gameObject = this.mobileVisContainer;
              break;
            case EntityLayer.FX:
              clusterMapVisualizer1 = this.staticVisPrefab;
              gameObject = this.FXVisContainer;
              break;
          }
          ClusterNameDisplayScreen.Instance.AddNewEntry(clusterGridEntity);
          ClusterMapVisualizer clusterMapVisualizer2 = Object.Instantiate<ClusterMapVisualizer>(clusterMapVisualizer1, gameObject.transform);
          clusterMapVisualizer2.Init(clusterGridEntity, this.pathDrawer);
          ((Component) clusterMapVisualizer2).gameObject.SetActive(true);
          this.m_gridEntityAnims.Add(clusterGridEntity, clusterMapVisualizer2);
          this.m_gridEntityVis.Add(clusterGridEntity, clusterMapVisualizer2);
          clusterGridEntity.positionDirty = false;
          clusterGridEntity.Subscribe(1502190696, new Action<object>(this.RemoveDeletedEntities));
        }
      }
    }
    this.RemoveDeletedEntities();
    foreach (KeyValuePair<ClusterGridEntity, ClusterMapVisualizer> gridEntityVi in this.m_gridEntityVis)
    {
      ClusterGridEntity key = gridEntityVi.Key;
      if (key.Layer == EntityLayer.Asteroid)
      {
        int id = ((Component) key).GetComponent<WorldContainer>().id;
        gridEntityVi.Value.alertVignette.worldID = id;
      }
    }
  }

  private void RemoveDeletedEntities(object obj = null)
  {
    foreach (ClusterGridEntity key in this.m_gridEntityVis.Keys.Where<ClusterGridEntity>((Func<ClusterGridEntity, bool>) (x => Object.op_Equality((Object) x, (Object) null) || Object.op_Equality((Object) ((Component) x).gameObject, (Object) obj))).ToList<ClusterGridEntity>())
    {
      Util.KDestroyGameObject((Component) this.m_gridEntityVis[key]);
      this.m_gridEntityVis.Remove(key);
      this.m_gridEntityAnims.Remove(key);
    }
  }

  private void OnClusterLocationChanged(object data) => this.UpdateVis();

  public static ClusterRevealLevel GetRevealLevel(ClusterGridEntity entity)
  {
    ClusterRevealLevel cellRevealLevel = ClusterGrid.Instance.GetCellRevealLevel(entity.Location);
    ClusterRevealLevel isVisibleInFow = entity.IsVisibleInFOW;
    if (cellRevealLevel == ClusterRevealLevel.Visible || isVisibleInFow == ClusterRevealLevel.Visible)
      return ClusterRevealLevel.Visible;
    return cellRevealLevel == ClusterRevealLevel.Peeked && isVisibleInFow == ClusterRevealLevel.Peeked ? ClusterRevealLevel.Peeked : ClusterRevealLevel.Hidden;
  }

  private void UpdateVis(object data = null)
  {
    this.SetupVisGameObjects();
    this.UpdatePaths();
    foreach (KeyValuePair<ClusterGridEntity, ClusterMapVisualizer> gridEntityAnim in this.m_gridEntityAnims)
    {
      ClusterRevealLevel revealLevel = ClusterMapScreen.GetRevealLevel(gridEntityAnim.Key);
      gridEntityAnim.Value.Show(revealLevel);
      bool selected = Object.op_Equality((Object) this.m_selectedEntity, (Object) gridEntityAnim.Key);
      gridEntityAnim.Value.Select(selected);
      if (gridEntityAnim.Key.positionDirty)
      {
        Vector3 position = ClusterGrid.Instance.GetPosition(gridEntityAnim.Key);
        TransformExtensions.SetLocalPosition((Transform) Util.rectTransform((Component) gridEntityAnim.Value), position);
        gridEntityAnim.Key.positionDirty = false;
      }
    }
    if (Object.op_Inequality((Object) this.m_selectedEntity, (Object) null) && this.m_gridEntityVis.ContainsKey(this.m_selectedEntity))
    {
      ClusterMapVisualizer gridEntityVi = this.m_gridEntityVis[this.m_selectedEntity];
      this.m_selectMarker.SetTargetTransform(gridEntityVi.transform);
      ((Component) this.m_selectMarker).gameObject.SetActive(true);
      gridEntityVi.transform.SetAsLastSibling();
    }
    else
      ((Component) this.m_selectMarker).gameObject.SetActive(false);
    foreach (KeyValuePair<AxialI, ClusterMapVisualizer> keyValuePair in this.m_cellVisByLocation)
    {
      ClusterMapHex component = ((Component) keyValuePair.Value).GetComponent<ClusterMapHex>();
      AxialI key = keyValuePair.Key;
      int cellRevealLevel = (int) ClusterGrid.Instance.GetCellRevealLevel(key);
      component.SetRevealed((ClusterRevealLevel) cellRevealLevel);
    }
    this.UpdateHexToggleStates();
    this.FloatyAsteroidAnimation();
  }

  private void OnEntityDestroyed(object obj) => this.RemoveDeletedEntities();

  private void UpdateHexToggleStates()
  {
    bool flag = Object.op_Inequality((Object) this.m_hoveredHex, (Object) null) && Object.op_Implicit((Object) ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(this.m_hoveredHex.location, EntityLayer.Asteroid));
    foreach (KeyValuePair<AxialI, ClusterMapVisualizer> keyValuePair in this.m_cellVisByLocation)
    {
      ClusterMapHex component = ((Component) keyValuePair.Value).GetComponent<ClusterMapHex>();
      AxialI key = keyValuePair.Key;
      int state = !Object.op_Inequality((Object) this.m_selectedHex, (Object) null) || !AxialI.op_Equality(this.m_selectedHex.location, key) ? (!flag || !AxialUtil.IsAdjacent(this.m_hoveredHex.location, key) ? 0 : 2) : 1;
      component.UpdateToggleState((ClusterMapHex.ToggleState) state);
    }
  }

  public void SelectEntity(ClusterGridEntity entity, bool frameDelay = false)
  {
    if (Object.op_Inequality((Object) entity, (Object) null))
    {
      this.SetSelectedEntity(entity, frameDelay);
      this.m_selectedHex = ((Component) this.m_cellVisByLocation[entity.Location]).GetComponent<ClusterMapHex>();
    }
    this.UpdateVis();
  }

  public void SelectHex(ClusterMapHex newSelectionHex)
  {
    if (this.m_mode == ClusterMapScreen.Mode.Default)
    {
      List<ClusterGridEntity> visibleEntitiesAtCell = ClusterGrid.Instance.GetVisibleEntitiesAtCell(newSelectionHex.location);
      for (int index = visibleEntitiesAtCell.Count - 1; index >= 0; --index)
      {
        KSelectable component = ((Component) visibleEntitiesAtCell[index]).GetComponent<KSelectable>();
        if (Object.op_Equality((Object) component, (Object) null) || !component.IsSelectable)
          visibleEntitiesAtCell.RemoveAt(index);
      }
      if (visibleEntitiesAtCell.Count == 0)
      {
        this.SetSelectedEntity((ClusterGridEntity) null);
      }
      else
      {
        int num = visibleEntitiesAtCell.IndexOf(this.m_selectedEntity);
        int index = 0;
        if (num >= 0)
          index = (num + 1) % visibleEntitiesAtCell.Count;
        this.SetSelectedEntity(visibleEntitiesAtCell[index]);
      }
      this.m_selectedHex = newSelectionHex;
    }
    else if (this.m_mode == ClusterMapScreen.Mode.SelectDestination)
    {
      Debug.Assert(Object.op_Inequality((Object) this.m_destinationSelector, (Object) null), (object) "Selected a hex in SelectDestination mode with no ClusterDestinationSelector");
      if (ClusterGrid.Instance.GetPath(this.m_selectedHex.location, newSelectionHex.location, this.m_destinationSelector) != null)
      {
        this.m_destinationSelector.SetDestination(newSelectionHex.location);
        if (this.m_closeOnSelect)
          ManagementMenu.Instance.CloseAll();
        else
          this.SetMode(ClusterMapScreen.Mode.Default);
      }
    }
    this.UpdateVis();
  }

  public bool HasCurrentHover() => Object.op_Inequality((Object) this.m_hoveredHex, (Object) null);

  public AxialI GetCurrentHoverLocation() => this.m_hoveredHex.location;

  public void OnHoverHex(ClusterMapHex newHoverHex)
  {
    this.m_hoveredHex = newHoverHex;
    if (this.m_mode == ClusterMapScreen.Mode.SelectDestination)
      this.UpdateVis();
    this.UpdateHexToggleStates();
  }

  public void OnUnhoverHex(ClusterMapHex unhoveredHex)
  {
    if (!Object.op_Equality((Object) this.m_hoveredHex, (Object) unhoveredHex))
      return;
    this.m_hoveredHex = (ClusterMapHex) null;
    this.UpdateHexToggleStates();
  }

  public void SetLocationHighlight(AxialI location, bool highlight) => ((Component) this.m_cellVisByLocation[location]).GetComponent<ClusterMapHex>().ChangeState(highlight ? 1 : 0);

  private void UpdatePaths()
  {
    ClusterDestinationSelector destinationSelector = Object.op_Inequality((Object) this.m_selectedEntity, (Object) null) ? ((Component) this.m_selectedEntity).GetComponent<ClusterDestinationSelector>() : (ClusterDestinationSelector) null;
    if (this.m_mode == ClusterMapScreen.Mode.SelectDestination && Object.op_Inequality((Object) this.m_hoveredHex, (Object) null))
    {
      Debug.Assert(Object.op_Inequality((Object) this.m_destinationSelector, (Object) null), (object) "In SelectDestination mode without a destination selector");
      AxialI myWorldLocation = this.m_destinationSelector.GetMyWorldLocation();
      string fail_reason;
      List<AxialI> path = ClusterGrid.Instance.GetPath(myWorldLocation, this.m_hoveredHex.location, this.m_destinationSelector, out fail_reason);
      if (path != null)
      {
        if (Object.op_Equality((Object) this.m_previewMapPath, (Object) null))
          this.m_previewMapPath = this.pathDrawer.AddPath();
        this.m_previewMapPath.SetPoints(ClusterMapPathDrawer.GetDrawPathList(Vector2.op_Implicit(this.m_gridEntityVis[this.GetSelectorGridEntity(this.m_destinationSelector)].transform.localPosition), path));
        this.m_previewMapPath.SetColor(this.rocketPreviewPathColor);
      }
      else if (Object.op_Inequality((Object) this.m_previewMapPath, (Object) null))
      {
        Util.KDestroyGameObject((Component) this.m_previewMapPath);
        this.m_previewMapPath = (ClusterMapPath) null;
      }
      // ISSUE: explicit non-virtual call
      int pathLength = path != null ? __nonvirtual (path.Count) : -1;
      if (Object.op_Inequality((Object) this.m_selectedEntity, (Object) null))
      {
        float range = ((Component) this.m_selectedEntity).GetComponent<IClusterRange>().GetRange();
        if ((double) pathLength > (double) range / 600.0 && string.IsNullOrEmpty(fail_reason))
          fail_reason = string.Format((string) STRINGS.UI.CLUSTERMAP.TOOLTIP_INVALID_DESTINATION_OUT_OF_RANGE, (object) (float) ((double) range / 600.0));
        bool repeat = ((Component) destinationSelector).GetComponent<RocketClusterDestinationSelector>().Repeat;
        this.m_hoveredHex.SetDestinationStatus(fail_reason, pathLength, (int) range, repeat);
      }
      else
        this.m_hoveredHex.SetDestinationStatus(fail_reason);
    }
    else
    {
      if (!Object.op_Inequality((Object) this.m_previewMapPath, (Object) null))
        return;
      Util.KDestroyGameObject((Component) this.m_previewMapPath);
      this.m_previewMapPath = (ClusterMapPath) null;
    }
  }

  private ClusterGridEntity GetSelectorGridEntity(ClusterDestinationSelector selector)
  {
    ClusterGridEntity component = ((Component) selector).GetComponent<ClusterGridEntity>();
    if (Object.op_Inequality((Object) component, (Object) null) && ClusterGrid.Instance.IsVisible(component))
      return component;
    ClusterGridEntity entityOfLayerAtCell = ClusterGrid.Instance.GetVisibleEntityOfLayerAtCell(selector.GetMyWorldLocation(), EntityLayer.Asteroid);
    Debug.Assert(Object.op_Inequality((Object) component, (Object) null) || Object.op_Inequality((Object) entityOfLayerAtCell, (Object) null), (object) string.Format("{0} has no grid entity and isn't located at a visible asteroid at {1}", (object) selector, (object) selector.GetMyWorldLocation()));
    return Object.op_Implicit((Object) entityOfLayerAtCell) ? entityOfLayerAtCell : component;
  }

  private void UpdateTearStatus()
  {
    ClusterPOIManager clusterPoiManager = (ClusterPOIManager) null;
    if (Object.op_Inequality((Object) ClusterManager.Instance, (Object) null))
      clusterPoiManager = ((Component) ClusterManager.Instance).GetComponent<ClusterPOIManager>();
    if (!Object.op_Inequality((Object) clusterPoiManager, (Object) null))
      return;
    TemporalTear temporalTear = clusterPoiManager.GetTemporalTear();
    if (!Object.op_Inequality((Object) temporalTear, (Object) null))
      return;
    temporalTear.UpdateStatus();
  }

  public enum Mode
  {
    Default,
    SelectDestination,
  }
}
