// Decompiled with JetBrains decompiler
// Type: KCollider2D
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public abstract class KCollider2D : KMonoBehaviour, IRenderEveryTick
{
  [SerializeField]
  public Vector2 _offset;
  private Extents cachedExtents;
  private HandleVector<int>.Handle partitionerEntry;

  public Vector2 offset
  {
    get => this._offset;
    set
    {
      this._offset = value;
      this.MarkDirty();
    }
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.autoRegisterSimRender = false;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    Singleton<CellChangeMonitor>.Instance.RegisterMovementStateChanged(this.transform, new Action<Transform, bool>(KCollider2D.OnMovementStateChanged));
    this.MarkDirty(true);
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    Singleton<CellChangeMonitor>.Instance.UnregisterMovementStateChanged(this.transform, new Action<Transform, bool>(KCollider2D.OnMovementStateChanged));
    GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
  }

  public void MarkDirty(bool force = false)
  {
    bool flag = force || this.partitionerEntry.IsValid();
    if (!flag)
      return;
    Extents extents = this.GetExtents();
    if (!force && this.cachedExtents.x == extents.x && this.cachedExtents.y == extents.y && this.cachedExtents.width == extents.width && this.cachedExtents.height == extents.height)
      return;
    this.cachedExtents = extents;
    GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
    if (!flag)
      return;
    this.partitionerEntry = GameScenePartitioner.Instance.Add(((Object) this).name, (object) this, this.cachedExtents, GameScenePartitioner.Instance.collisionLayer, (Action<object>) null);
  }

  private void OnMovementStateChanged(bool is_moving)
  {
    if (is_moving)
    {
      this.MarkDirty();
      SimAndRenderScheduler.instance.Add((object) this, false);
    }
    else
      SimAndRenderScheduler.instance.Remove((object) this);
  }

  private static void OnMovementStateChanged(Transform transform, bool is_moving) => ((Component) transform).GetComponent<KCollider2D>().OnMovementStateChanged(is_moving);

  public void RenderEveryTick(float dt) => this.MarkDirty();

  public abstract bool Intersects(Vector2 pos);

  public abstract Extents GetExtents();

  public abstract Bounds bounds { get; }
}
