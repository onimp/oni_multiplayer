// Decompiled with JetBrains decompiler
// Type: Light2D
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Light2D")]
public class Light2D : KMonoBehaviour, IGameObjectEffectDescriptor
{
  private bool dirty_shape;
  private bool dirty_position;
  [SerializeField]
  private LightGridManager.LightGridEmitter.State pending_emitter_state = LightGridManager.LightGridEmitter.State.DEFAULT;
  public float Angle;
  public Vector2 Direction;
  [SerializeField]
  private Vector2 _offset;
  public bool drawOverlay;
  public Color overlayColour;
  public MaterialPropertyBlock materialPropertyBlock;
  private HandleVector<int>.Handle solidPartitionerEntry = HandleVector<int>.InvalidHandle;
  private HandleVector<int>.Handle liquidPartitionerEntry = HandleVector<int>.InvalidHandle;
  private static readonly EventSystem.IntraObjectHandler<Light2D> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<Light2D>((Action<Light2D, object>) ((light, data) => ((Behaviour) light).enabled = (bool) data));

  private T MaybeDirty<T>(T old_value, T new_value, ref bool dirty)
  {
    if (EqualityComparer<T>.Default.Equals(old_value, new_value))
      return old_value;
    dirty = true;
    return new_value;
  }

  public LightShape shape
  {
    get => this.pending_emitter_state.shape;
    set => this.pending_emitter_state.shape = this.MaybeDirty<LightShape>(this.pending_emitter_state.shape, value, ref this.dirty_shape);
  }

  public LightGridManager.LightGridEmitter emitter { get; private set; }

  public Color Color
  {
    get => this.pending_emitter_state.colour;
    set => this.pending_emitter_state.colour = value;
  }

  public int Lux
  {
    get => this.pending_emitter_state.intensity;
    set => this.pending_emitter_state.intensity = value;
  }

  public float Range
  {
    get => this.pending_emitter_state.radius;
    set => this.pending_emitter_state.radius = this.MaybeDirty<float>(this.pending_emitter_state.radius, value, ref this.dirty_shape);
  }

  private int origin
  {
    get => this.pending_emitter_state.origin;
    set => this.pending_emitter_state.origin = this.MaybeDirty<int>(this.pending_emitter_state.origin, value, ref this.dirty_position);
  }

  public float IntensityAnimation { get; set; }

  public Vector2 Offset
  {
    get => this._offset;
    set
    {
      if (!Vector2.op_Inequality(this._offset, value))
        return;
      this._offset = value;
      this.origin = Grid.PosToCell(Vector3.op_Addition(TransformExtensions.GetPosition(this.transform), Vector2.op_Implicit(this._offset)));
    }
  }

  private bool isRegistered => HandleVector<int>.Handle.op_Inequality(this.solidPartitionerEntry, HandleVector<int>.InvalidHandle);

  public Light2D()
  {
    this.emitter = new LightGridManager.LightGridEmitter();
    this.Range = 5f;
    this.Lux = 1000;
  }

  protected virtual void OnPrefabInit()
  {
    this.Subscribe<Light2D>(-592767678, Light2D.OnOperationalChangedDelegate);
    this.IntensityAnimation = 1f;
  }

  protected virtual void OnCmpEnable()
  {
    this.materialPropertyBlock = new MaterialPropertyBlock();
    base.OnCmpEnable();
    Components.Light2Ds.Add(this);
    if (this.isSpawned)
    {
      this.AddToScenePartitioner();
      this.emitter.Refresh(this.pending_emitter_state, true);
    }
    Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(this.transform, new System.Action(this.OnMoved), "Light2D.OnMoved");
  }

  protected virtual void OnCmpDisable()
  {
    Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(this.transform, new System.Action(this.OnMoved));
    Components.Light2Ds.Remove(this);
    base.OnCmpDisable();
    this.FullRemove();
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.origin = Grid.PosToCell(Vector3.op_Addition(TransformExtensions.GetPosition(this.transform), Vector2.op_Implicit(this.Offset)));
    if (!((Behaviour) this).isActiveAndEnabled)
      return;
    this.AddToScenePartitioner();
    this.emitter.Refresh(this.pending_emitter_state, true);
  }

  protected virtual void OnCleanUp() => this.FullRemove();

  private void OnMoved()
  {
    if (!this.isSpawned)
      return;
    this.FullRefresh();
  }

  private HandleVector<int>.Handle AddToLayer(Extents ext, ScenePartitionerLayer layer) => GameScenePartitioner.Instance.Add(nameof (Light2D), (object) ((Component) this).gameObject, ext, layer, new Action<object>(this.OnWorldChanged));

  private Extents ComputeExtents()
  {
    Vector2I xy = Grid.CellToXY(this.origin);
    int range = (int) this.Range;
    Vector2I vector2I;
    // ISSUE: explicit constructor call
    ((Vector2I) ref vector2I).\u002Ector(xy.x - range, xy.y - range);
    int width = 2 * range;
    int height = this.shape == LightShape.Circle ? 2 * range : range;
    return new Extents(vector2I.x, vector2I.y, width, height);
  }

  private void AddToScenePartitioner()
  {
    Extents extents = this.ComputeExtents();
    this.solidPartitionerEntry = this.AddToLayer(extents, GameScenePartitioner.Instance.solidChangedLayer);
    this.liquidPartitionerEntry = this.AddToLayer(extents, GameScenePartitioner.Instance.liquidChangedLayer);
  }

  private void RemoveFromScenePartitioner()
  {
    if (!this.isRegistered)
      return;
    GameScenePartitioner.Instance.Free(ref this.solidPartitionerEntry);
    GameScenePartitioner.Instance.Free(ref this.liquidPartitionerEntry);
  }

  private void MoveInScenePartitioner()
  {
    GameScenePartitioner.Instance.UpdatePosition(this.solidPartitionerEntry, this.ComputeExtents());
    GameScenePartitioner.Instance.UpdatePosition(this.liquidPartitionerEntry, this.ComputeExtents());
  }

  [ContextMenu("Refresh")]
  public void FullRefresh()
  {
    if (!this.isSpawned || !((Behaviour) this).isActiveAndEnabled)
      return;
    DebugUtil.DevAssert(this.isRegistered, "shouldn't be refreshing if we aren't spawned and enabled", (Object) null);
    int num = (int) this.RefreshShapeAndPosition();
    this.emitter.Refresh(this.pending_emitter_state, true);
  }

  public void FullRemove()
  {
    this.RemoveFromScenePartitioner();
    this.emitter.RemoveFromGrid();
  }

  public Light2D.RefreshResult RefreshShapeAndPosition()
  {
    if (!this.isSpawned)
      return Light2D.RefreshResult.None;
    if (!((Behaviour) this).isActiveAndEnabled)
    {
      this.FullRemove();
      return Light2D.RefreshResult.Removed;
    }
    int cell = Grid.PosToCell(Vector3.op_Addition(TransformExtensions.GetPosition(this.transform), Vector2.op_Implicit(this.Offset)));
    if (!Grid.IsValidCell(cell))
    {
      this.FullRemove();
      return Light2D.RefreshResult.Removed;
    }
    this.origin = cell;
    if (this.dirty_shape)
    {
      this.RemoveFromScenePartitioner();
      this.AddToScenePartitioner();
    }
    else if (this.dirty_position)
      this.MoveInScenePartitioner();
    this.dirty_shape = false;
    this.dirty_position = false;
    return Light2D.RefreshResult.Updated;
  }

  private void OnWorldChanged(object data) => this.FullRefresh();

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    descriptors.Add(new Descriptor(string.Format((string) UI.GAMEOBJECTEFFECTS.EMITS_LIGHT, (object) this.Range), (string) UI.GAMEOBJECTEFFECTS.TOOLTIPS.EMITS_LIGHT, (Descriptor.DescriptorType) 1, false));
    descriptors.Add(new Descriptor(string.Format((string) UI.GAMEOBJECTEFFECTS.EMITS_LIGHT_LUX, (object) this.Lux), (string) UI.GAMEOBJECTEFFECTS.TOOLTIPS.EMITS_LIGHT_LUX, (Descriptor.DescriptorType) 1, false));
    return descriptors;
  }

  public enum RefreshResult
  {
    None,
    Removed,
    Updated,
  }
}
