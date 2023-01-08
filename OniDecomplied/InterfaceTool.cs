// Decompiled with JetBrains decompiler
// Type: InterfaceTool
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

[AddComponentMenu("KMonoBehaviour/scripts/InterfaceTool")]
public class InterfaceTool : KMonoBehaviour
{
  private static Dictionary<Action, InterfaceToolConfig> interfaceConfigMap = (Dictionary<Action, InterfaceToolConfig>) null;
  private static List<InterfaceToolConfig> activeConfigs = new List<InterfaceToolConfig>();
  public const float MaxClickDistance = 0.02f;
  public const float DepthBias = -0.15f;
  public GameObject visualizer;
  public Grid.SceneLayer visualizerLayer = Grid.SceneLayer.Move;
  public string placeSound;
  protected bool populateHitsList;
  [NonSerialized]
  public bool hasFocus;
  [SerializeField]
  protected Texture2D cursor;
  public Vector2 cursorOffset = new Vector2(2f, 2f);
  public System.Action OnDeactivate;
  private static Texture2D activeCursor = (Texture2D) null;
  private static HashedString toolActivatedViewMode = OverlayModes.None.ID;
  protected HashedString viewMode = OverlayModes.None.ID;
  private HoverTextConfiguration hoverTextConfiguration;
  private KSelectable hoverOverride;
  public KSelectable hover;
  protected int layerMask;
  protected SelectMarker selectMarker;
  private List<RaycastResult> castResults = new List<RaycastResult>();
  private bool isAppFocused = true;
  private List<KSelectable> hits = new List<KSelectable>();
  protected bool playedSoundThisFrame;
  private List<InterfaceTool.Intersection> intersections = new List<InterfaceTool.Intersection>();
  private HashSet<Component> prevIntersectionGroup = new HashSet<Component>();
  private HashSet<Component> curIntersectionGroup = new HashSet<Component>();
  private int hitCycleCount;

  public static InterfaceToolConfig ActiveConfig
  {
    get
    {
      if (InterfaceTool.interfaceConfigMap == null)
        InterfaceTool.InitializeConfigs((Action) 0, (List<InterfaceToolConfig>) null);
      return InterfaceTool.activeConfigs[InterfaceTool.activeConfigs.Count - 1];
    }
  }

  public static void ToggleConfig(Action configKey)
  {
    if (InterfaceTool.interfaceConfigMap == null)
      InterfaceTool.InitializeConfigs((Action) 0, (List<InterfaceToolConfig>) null);
    InterfaceToolConfig interfaceToolConfig;
    if (!InterfaceTool.interfaceConfigMap.TryGetValue(configKey, out interfaceToolConfig))
      Debug.LogWarning((object) (string.Format("[InterfaceTool] No config is associated with Key: {0}!", (object) configKey) + " Are you sure the configs were initialized properly!"));
    else if (InterfaceTool.activeConfigs.BinarySearch(interfaceToolConfig, (IComparer<InterfaceToolConfig>) InterfaceToolConfig.ConfigComparer) <= 0)
    {
      Debug.Log((object) string.Format("[InterfaceTool] Pushing config with key: {0}", (object) configKey));
      InterfaceTool.activeConfigs.Add(interfaceToolConfig);
      InterfaceTool.activeConfigs.Sort((IComparer<InterfaceToolConfig>) InterfaceToolConfig.ConfigComparer);
    }
    else
    {
      Debug.Log((object) string.Format("[InterfaceTool] Popping config with key: {0}", (object) configKey));
      InterfaceTool.activeConfigs.Remove(interfaceToolConfig);
    }
  }

  public static void InitializeConfigs(Action defaultKey, List<InterfaceToolConfig> configs)
  {
    string str = configs == null ? "null" : configs.Count.ToString();
    Debug.Log((object) string.Format("[InterfaceTool] Initializing configs with values of DefaultKey: {0} Configs: {1}", (object) defaultKey, (object) str));
    if (configs == null || configs.Count == 0)
    {
      InterfaceToolConfig instance = ScriptableObject.CreateInstance<InterfaceToolConfig>();
      InterfaceTool.interfaceConfigMap = new Dictionary<Action, InterfaceToolConfig>();
      InterfaceTool.interfaceConfigMap[instance.InputAction] = instance;
    }
    else
    {
      InterfaceTool.interfaceConfigMap = configs.ToDictionary<InterfaceToolConfig, Action>((Func<InterfaceToolConfig, Action>) (x => x.InputAction));
      InterfaceTool.ToggleConfig(defaultKey);
    }
  }

  public HashedString ViewMode => this.viewMode;

  public virtual string[] DlcIDs => DlcManager.AVAILABLE_ALL_VERSIONS;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.hoverTextConfiguration = ((Component) this).GetComponent<HoverTextConfiguration>();
  }

  public void ActivateTool()
  {
    this.OnActivateTool();
    this.OnMouseMove(PlayerController.GetCursorPos(KInputManager.GetMousePos()));
    Game.Instance.Trigger(1174281782, (object) this);
  }

  public virtual bool ShowHoverUI()
  {
    if (Object.op_Equality((Object) ManagementMenu.Instance, (Object) null) || ManagementMenu.Instance.IsFullscreenUIActive())
      return false;
    Vector3 worldPoint = Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos());
    if (Object.op_Equality((Object) OverlayScreen.Instance, (Object) null) || !ClusterManager.Instance.IsPositionInActiveWorld(worldPoint) || (double) worldPoint.x < 0.0 || (double) worldPoint.x > (double) Grid.WidthInMeters || (double) worldPoint.y < 0.0 || (double) worldPoint.y > (double) Grid.HeightInMeters)
      return false;
    EventSystem current = EventSystem.current;
    return Object.op_Inequality((Object) current, (Object) null) && !current.IsPointerOverGameObject();
  }

  protected virtual void OnActivateTool()
  {
    if (Object.op_Inequality((Object) OverlayScreen.Instance, (Object) null) && HashedString.op_Inequality(this.viewMode, OverlayModes.None.ID) && HashedString.op_Inequality(OverlayScreen.Instance.mode, this.viewMode))
    {
      OverlayScreen.Instance.ToggleOverlay(this.viewMode);
      InterfaceTool.toolActivatedViewMode = this.viewMode;
    }
    this.SetCursor(this.cursor, this.cursorOffset, (CursorMode) 0);
  }

  public void SetCurrentVirtualInputModuleMousMovementMode(
    bool mouseMovementOnly,
    Action<VirtualInputModule> extraActions = null)
  {
    EventSystem current = EventSystem.current;
    if (!Object.op_Inequality((Object) current, (Object) null) || !Object.op_Inequality((Object) current.currentInputModule, (Object) null))
      return;
    VirtualInputModule currentInputModule = current.currentInputModule as VirtualInputModule;
    if (!Object.op_Inequality((Object) currentInputModule, (Object) null))
      return;
    currentInputModule.mouseMovementOnly = mouseMovementOnly;
    if (extraActions == null)
      return;
    extraActions(currentInputModule);
  }

  public void DeactivateTool(InterfaceTool new_tool = null)
  {
    this.OnDeactivateTool(new_tool);
    if (!Object.op_Equality((Object) new_tool, (Object) null) && !Object.op_Equality((Object) new_tool, (Object) SelectTool.Instance) || !HashedString.op_Inequality(InterfaceTool.toolActivatedViewMode, OverlayModes.None.ID) || !HashedString.op_Equality(InterfaceTool.toolActivatedViewMode, SimDebugView.Instance.GetMode()))
      return;
    OverlayScreen.Instance.ToggleOverlay(OverlayModes.None.ID);
    InterfaceTool.toolActivatedViewMode = OverlayModes.None.ID;
  }

  public virtual void GetOverlayColorData(out HashSet<ToolMenu.CellColorData> colors) => colors = (HashSet<ToolMenu.CellColorData>) null;

  protected virtual void OnDeactivateTool(InterfaceTool new_tool)
  {
  }

  private void OnApplicationFocus(bool focusStatus) => this.isAppFocused = focusStatus;

  public virtual string GetDeactivateSound() => "Tile_Cancel";

  public virtual void OnMouseMove(Vector3 cursor_pos)
  {
    if (Object.op_Equality((Object) this.visualizer, (Object) null) || !this.isAppFocused)
      return;
    cursor_pos = Grid.CellToPosCBC(Grid.PosToCell(cursor_pos), this.visualizerLayer);
    cursor_pos.z += -0.15f;
    TransformExtensions.SetLocalPosition(this.visualizer.transform, cursor_pos);
  }

  public virtual void OnKeyDown(KButtonEvent e)
  {
  }

  public virtual void OnKeyUp(KButtonEvent e)
  {
  }

  public virtual void OnLeftClickDown(Vector3 cursor_pos)
  {
  }

  public virtual void OnLeftClickUp(Vector3 cursor_pos)
  {
  }

  public virtual void OnRightClickDown(Vector3 cursor_pos, KButtonEvent e)
  {
  }

  public virtual void OnRightClickUp(Vector3 cursor_pos)
  {
  }

  public virtual void OnFocus(bool focus)
  {
    if (Object.op_Inequality((Object) this.visualizer, (Object) null))
      this.visualizer.SetActive(focus);
    this.hasFocus = focus;
  }

  protected Vector2 GetRegularizedPos(Vector2 input, bool minimize)
  {
    Vector3 vector3;
    // ISSUE: explicit constructor call
    ((Vector3) ref vector3).\u002Ector(Grid.HalfCellSizeInMeters, Grid.HalfCellSizeInMeters, 0.0f);
    return Vector2.op_Implicit(Vector3.op_Addition(Grid.CellToPosCCC(Grid.PosToCell(input), Grid.SceneLayer.Background), minimize ? Vector3.op_UnaryNegation(vector3) : vector3));
  }

  protected Vector2 GetWorldRestrictedPosition(Vector2 input)
  {
    input.x = Mathf.Clamp(input.x, ClusterManager.Instance.activeWorld.minimumBounds.x, ClusterManager.Instance.activeWorld.maximumBounds.x);
    input.y = Mathf.Clamp(input.y, ClusterManager.Instance.activeWorld.minimumBounds.y, ClusterManager.Instance.activeWorld.maximumBounds.y);
    return input;
  }

  protected void SetCursor(Texture2D new_cursor, Vector2 offset, CursorMode mode)
  {
    if (!Object.op_Inequality((Object) new_cursor, (Object) InterfaceTool.activeCursor) || !Object.op_Inequality((Object) new_cursor, (Object) null))
      return;
    InterfaceTool.activeCursor = new_cursor;
    try
    {
      Cursor.SetCursor(new_cursor, offset, mode);
      if (!Object.op_Inequality((Object) PlayerController.Instance.vim, (Object) null))
        return;
      PlayerController.Instance.vim.SetCursor(new_cursor);
    }
    catch (Exception ex)
    {
      KCrashReporter.ReportErrorDevNotification("SetCursor Failed", ex.StackTrace, string.Format("SetCursor Failed new_cursor={0} offset={1} mode={2}", (object) new_cursor, (object) offset, (object) mode));
    }
  }

  protected void UpdateHoverElements(List<KSelectable> hits)
  {
    if (!Object.op_Inequality((Object) this.hoverTextConfiguration, (Object) null))
      return;
    this.hoverTextConfiguration.UpdateHoverElements(hits);
  }

  public virtual void LateUpdate()
  {
    if (this.populateHitsList)
    {
      if (!this.isAppFocused || !Grid.IsValidCell(Grid.PosToCell(Camera.main.ScreenToWorldPoint(KInputManager.GetMousePos()))))
        return;
      this.hits.Clear();
      this.GetSelectablesUnderCursor(this.hits);
      KSelectable objectUnderCursor = this.GetObjectUnderCursor<KSelectable>(false, (Func<KSelectable, bool>) (s => ((Component) s).GetComponent<KSelectable>().IsSelectable));
      this.UpdateHoverElements(this.hits);
      if (!this.hasFocus && Object.op_Equality((Object) this.hoverOverride, (Object) null))
        this.ClearHover();
      else if (Object.op_Inequality((Object) objectUnderCursor, (Object) this.hover))
      {
        this.ClearHover();
        this.hover = objectUnderCursor;
        if (Object.op_Inequality((Object) objectUnderCursor, (Object) null))
        {
          Game.Instance.Trigger(2095258329, (object) ((Component) objectUnderCursor).gameObject);
          objectUnderCursor.Hover(!this.playedSoundThisFrame);
          this.playedSoundThisFrame = true;
        }
      }
      this.playedSoundThisFrame = false;
    }
    else
      this.UpdateHoverElements((List<KSelectable>) null);
  }

  public void GetSelectablesUnderCursor(List<KSelectable> hits)
  {
    if (Object.op_Inequality((Object) this.hoverOverride, (Object) null))
      hits.Add(this.hoverOverride);
    Camera main = Camera.main;
    Vector3 vector3;
    // ISSUE: explicit constructor call
    ((Vector3) ref vector3).\u002Ector(KInputManager.GetMousePos().x, KInputManager.GetMousePos().y, -TransformExtensions.GetPosition(((Component) main).transform).z);
    Vector3 worldPoint = main.ScreenToWorldPoint(vector3);
    Vector2 pos;
    // ISSUE: explicit constructor call
    ((Vector2) ref pos).\u002Ector(worldPoint.x, worldPoint.y);
    int cell = Grid.PosToCell(worldPoint);
    if (!Grid.IsValidCell(cell) || !Grid.IsVisible(cell))
      return;
    Game.Instance.statusItemRenderer.GetIntersections(pos, hits);
    ListPool<ScenePartitionerEntry, SelectTool>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, SelectTool>.Allocate();
    GameScenePartitioner.Instance.GatherEntries((int) pos.x, (int) pos.y, 1, 1, GameScenePartitioner.Instance.collisionLayer, (List<ScenePartitionerEntry>) gathered_entries);
    ((List<ScenePartitionerEntry>) gathered_entries).Sort((Comparison<ScenePartitionerEntry>) ((x, y) => this.SortHoverCards(x, y)));
    foreach (ScenePartitionerEntry partitionerEntry in (List<ScenePartitionerEntry>) gathered_entries)
    {
      KCollider2D kcollider2D = partitionerEntry.obj as KCollider2D;
      if (!Object.op_Equality((Object) kcollider2D, (Object) null) && kcollider2D.Intersects(new Vector2(pos.x, pos.y)))
      {
        KSelectable kselectable = ((Component) kcollider2D).GetComponent<KSelectable>();
        if (Object.op_Equality((Object) kselectable, (Object) null))
          kselectable = ((Component) kcollider2D).GetComponentInParent<KSelectable>();
        if (!Object.op_Equality((Object) kselectable, (Object) null) && ((Behaviour) kselectable).isActiveAndEnabled && !hits.Contains(kselectable) && kselectable.IsSelectable)
          hits.Add(kselectable);
      }
    }
    gathered_entries.Recycle();
  }

  public void SetLinkCursor(bool set) => this.SetCursor(set ? Assets.GetTexture("cursor_hand") : this.cursor, set ? Vector2.zero : this.cursorOffset, (CursorMode) 0);

  protected T GetObjectUnderCursor<T>(
    bool cycleSelection,
    Func<T, bool> condition = null,
    Component previous_selection = null)
    where T : MonoBehaviour
  {
    this.intersections.Clear();
    this.GetObjectUnderCursor2D<T>(this.intersections, condition, this.layerMask);
    this.intersections.RemoveAll(new Predicate<InterfaceTool.Intersection>(InterfaceTool.is_component_null));
    if (this.intersections.Count <= 0)
    {
      this.prevIntersectionGroup.Clear();
      return default (T);
    }
    this.curIntersectionGroup.Clear();
    foreach (InterfaceTool.Intersection intersection in this.intersections)
      this.curIntersectionGroup.Add((Component) intersection.component);
    if (!this.prevIntersectionGroup.Equals((object) this.curIntersectionGroup))
    {
      this.hitCycleCount = 0;
      this.prevIntersectionGroup = this.curIntersectionGroup;
    }
    this.intersections.Sort((Comparison<InterfaceTool.Intersection>) ((a, b) => this.SortSelectables(a.component as KMonoBehaviour, b.component as KMonoBehaviour)));
    int index = 0;
    if (cycleSelection)
    {
      if (Object.op_Inequality((Object) this.intersections[this.hitCycleCount % this.intersections.Count].component, (Object) previous_selection) || Object.op_Equality((Object) previous_selection, (Object) null))
      {
        index = 0;
        this.hitCycleCount = 0;
      }
      else
        index = ++this.hitCycleCount % this.intersections.Count;
    }
    return this.intersections[index].component as T;
  }

  private void GetObjectUnderCursor2D<T>(
    List<InterfaceTool.Intersection> intersections,
    Func<T, bool> condition,
    int layer_mask)
    where T : MonoBehaviour
  {
    Camera main = Camera.main;
    Vector3 vector3;
    // ISSUE: explicit constructor call
    ((Vector3) ref vector3).\u002Ector(KInputManager.GetMousePos().x, KInputManager.GetMousePos().y, -TransformExtensions.GetPosition(((Component) main).transform).z);
    Vector3 worldPoint = main.ScreenToWorldPoint(vector3);
    Vector2 pos;
    // ISSUE: explicit constructor call
    ((Vector2) ref pos).\u002Ector(worldPoint.x, worldPoint.y);
    InterfaceTool.Intersection intersection1;
    if (Object.op_Inequality((Object) this.hoverOverride, (Object) null))
    {
      List<InterfaceTool.Intersection> intersectionList = intersections;
      intersection1 = new InterfaceTool.Intersection();
      intersection1.component = (MonoBehaviour) this.hoverOverride;
      intersection1.distance = -100f;
      InterfaceTool.Intersection intersection2 = intersection1;
      intersectionList.Add(intersection2);
    }
    int cell = Grid.PosToCell(worldPoint);
    if (!Grid.IsValidCell(cell) || !Grid.IsVisible(cell))
      return;
    Game.Instance.statusItemRenderer.GetIntersections(pos, intersections);
    ListPool<ScenePartitionerEntry, SelectTool>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, SelectTool>.Allocate();
    int x = 0;
    int y = 0;
    Grid.CellToXY(cell, out x, out y);
    GameScenePartitioner.Instance.GatherEntries(x, y, 1, 1, GameScenePartitioner.Instance.collisionLayer, (List<ScenePartitionerEntry>) gathered_entries);
    foreach (ScenePartitionerEntry partitionerEntry in (List<ScenePartitionerEntry>) gathered_entries)
    {
      KCollider2D kcollider2D = partitionerEntry.obj as KCollider2D;
      if (!Object.op_Equality((Object) kcollider2D, (Object) null) && kcollider2D.Intersects(new Vector2(worldPoint.x, worldPoint.y)))
      {
        T obj = ((Component) kcollider2D).GetComponent<T>();
        if (Object.op_Equality((Object) (object) obj, (Object) null))
          obj = ((Component) kcollider2D).GetComponentInParent<T>();
        if (!Object.op_Equality((Object) (object) obj, (Object) null) && (1 << ((Component) (object) obj).gameObject.layer & layer_mask) != 0 && !Object.op_Equality((Object) (object) obj, (Object) null) && (condition == null || condition(obj)))
        {
          float num = TransformExtensions.GetPosition(((Component) (object) obj).transform).z - worldPoint.z;
          bool flag = false;
          for (int index = 0; index < intersections.Count; ++index)
          {
            InterfaceTool.Intersection intersection3 = intersections[index];
            if (Object.op_Equality((Object) ((Component) intersection3.component).gameObject, (Object) ((Component) (object) obj).gameObject))
            {
              intersection3.distance = Mathf.Min(intersection3.distance, num);
              intersections[index] = intersection3;
              flag = true;
              break;
            }
          }
          if (!flag)
          {
            List<InterfaceTool.Intersection> intersectionList = intersections;
            intersection1 = new InterfaceTool.Intersection();
            intersection1.component = (MonoBehaviour) (object) obj;
            intersection1.distance = num;
            InterfaceTool.Intersection intersection4 = intersection1;
            intersectionList.Add(intersection4);
          }
        }
      }
    }
    gathered_entries.Recycle();
  }

  private int SortSelectables(KMonoBehaviour x, KMonoBehaviour y)
  {
    if (Object.op_Equality((Object) x, (Object) null) && Object.op_Equality((Object) y, (Object) null))
      return 0;
    if (Object.op_Equality((Object) x, (Object) null))
      return -1;
    if (Object.op_Equality((Object) y, (Object) null))
      return 1;
    int num = TransformExtensions.GetPosition(x.transform).z.CompareTo(TransformExtensions.GetPosition(y.transform).z);
    return num != 0 ? num : ((Object) x).GetInstanceID().CompareTo(((Object) y).GetInstanceID());
  }

  public void SetHoverOverride(KSelectable hover_override) => this.hoverOverride = hover_override;

  private int SortHoverCards(ScenePartitionerEntry x, ScenePartitionerEntry y) => this.SortSelectables(x.obj as KMonoBehaviour, y.obj as KMonoBehaviour);

  private static bool is_component_null(InterfaceTool.Intersection intersection) => !Object.op_Implicit((Object) intersection.component);

  protected void ClearHover()
  {
    if (!Object.op_Inequality((Object) this.hover, (Object) null))
      return;
    KSelectable hover = this.hover;
    this.hover = (KSelectable) null;
    hover.Unhover();
    Game.Instance.Trigger(-1201923725, (object) null);
  }

  public struct Intersection
  {
    public MonoBehaviour component;
    public float distance;
  }
}
