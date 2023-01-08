// Decompiled with JetBrains decompiler
// Type: FallerComponents
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class FallerComponents : KGameObjectComponentManager<FallerComponent>
{
  private const float EPSILON = 0.07f;

  public HandleVector<int>.Handle Add(GameObject go, Vector2 initial_velocity) => this.Add(go, new FallerComponent(go.transform, initial_velocity));

  public virtual void Remove(GameObject go)
  {
    HandleVector<int>.Handle handle = this.GetHandle(go);
    this.OnCleanUpImmediate(handle);
    KComponentManager<FallerComponent>.CleanupInfo cleanupInfo = new KComponentManager<FallerComponent>.CleanupInfo((object) go, handle);
    if (!KComponentCleanUp.InCleanUpPhase)
      ((KComponentManager<FallerComponent>) this).AddToCleanupList(cleanupInfo);
    else
      ((KComponentManager<FallerComponent>) this).InternalRemoveComponent(cleanupInfo);
  }

  protected virtual void OnPrefabInit(HandleVector<int>.Handle h)
  {
    FallerComponent data = ((KCompactedVector<FallerComponent>) this).GetData(h);
    Vector3 position = TransformExtensions.GetPosition(data.transform);
    int cell1 = Grid.PosToCell(position);
    data.cellChangedCB = (System.Action) (() => FallerComponents.OnSolidChanged(h));
    float groundOffset = GravityComponent.GetGroundOffset(((Component) data.transform).GetComponent<KCollider2D>());
    int cell2 = Grid.PosToCell(new Vector3(position.x, (float) ((double) position.y - (double) groundOffset - 0.070000000298023224), position.z));
    bool flag = (!Grid.IsValidCell(cell2) ? 0 : (Grid.Solid[cell2] ? 1 : 0)) != 0 && (double) ((Vector2) ref data.initialVelocity).sqrMagnitude == 0.0;
    if (((!Grid.IsValidCell(cell1) ? 0 : (Grid.Solid[cell1] ? 1 : 0)) | (flag ? 1 : 0)) != 0)
    {
      data.solidChangedCB = (Action<object>) (ev_data => FallerComponents.OnSolidChanged(h));
      int height = 2;
      Vector2I xy = Grid.CellToXY(cell1);
      --xy.y;
      if (xy.y < 0)
      {
        xy.y = 0;
        height = 1;
      }
      else if (xy.y == Grid.HeightInCells - 1)
        height = 1;
      data.partitionerEntry = GameScenePartitioner.Instance.Add("Faller", (object) ((Component) data.transform).gameObject, xy.x, xy.y, 1, height, GameScenePartitioner.Instance.solidChangedLayer, data.solidChangedCB);
      ((KCompactedVector<FallerComponent>) GameComps.Fallers).SetData(h, data);
    }
    else
    {
      ((KCompactedVector<FallerComponent>) GameComps.Fallers).SetData(h, data);
      FallerComponents.AddGravity(data.transform, data.initialVelocity);
    }
  }

  protected virtual void OnSpawn(HandleVector<int>.Handle h)
  {
    ((KComponentManager<FallerComponent>) this).OnSpawn(h);
    FallerComponent data = ((KCompactedVector<FallerComponent>) this).GetData(h);
    Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(data.transform, data.cellChangedCB, "FallerComponent.OnSpawn");
  }

  private void OnCleanUpImmediate(HandleVector<int>.Handle h)
  {
    FallerComponent data = ((KCompactedVector<FallerComponent>) this).GetData(h);
    GameScenePartitioner.Instance.Free(ref data.partitionerEntry);
    if (data.cellChangedCB != null)
    {
      Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(data.transformInstanceId, data.cellChangedCB);
      data.cellChangedCB = (System.Action) null;
    }
    if (((KComponentManager<GravityComponent>) GameComps.Gravities).Has((object) ((Component) data.transform).gameObject))
      GameComps.Gravities.Remove(((Component) data.transform).gameObject);
    ((KCompactedVector<FallerComponent>) this).SetData(h, data);
  }

  private static void AddGravity(Transform transform, Vector2 initial_velocity)
  {
    if (((KComponentManager<GravityComponent>) GameComps.Gravities).Has((object) ((Component) transform).gameObject))
      return;
    GameComps.Gravities.Add(((Component) transform).gameObject, initial_velocity, (System.Action) (() => FallerComponents.OnLanded(transform)));
    HandleVector<int>.Handle handle = GameComps.Fallers.GetHandle(((Component) transform).gameObject);
    FallerComponent data = ((KCompactedVector<FallerComponent>) GameComps.Fallers).GetData(handle);
    if (!data.partitionerEntry.IsValid())
      return;
    GameScenePartitioner.Instance.Free(ref data.partitionerEntry);
    ((KCompactedVector<FallerComponent>) GameComps.Fallers).SetData(handle, data);
  }

  private static void RemoveGravity(Transform transform)
  {
    if (!((KComponentManager<GravityComponent>) GameComps.Gravities).Has((object) ((Component) transform).gameObject))
      return;
    GameComps.Gravities.Remove(((Component) transform).gameObject);
    HandleVector<int>.Handle h = GameComps.Fallers.GetHandle(((Component) transform).gameObject);
    FallerComponent data = ((KCompactedVector<FallerComponent>) GameComps.Fallers).GetData(h);
    int cell = Grid.CellBelow(Grid.PosToCell(TransformExtensions.GetPosition(transform)));
    GameScenePartitioner.Instance.Free(ref data.partitionerEntry);
    if (Grid.IsValidCell(cell))
    {
      data.solidChangedCB = (Action<object>) (ev_data => FallerComponents.OnSolidChanged(h));
      data.partitionerEntry = GameScenePartitioner.Instance.Add("Faller", (object) ((Component) transform).gameObject, cell, GameScenePartitioner.Instance.solidChangedLayer, data.solidChangedCB);
    }
    ((KCompactedVector<FallerComponent>) GameComps.Fallers).SetData(h, data);
  }

  private static void OnLanded(Transform transform) => FallerComponents.RemoveGravity(transform);

  private static void OnSolidChanged(HandleVector<int>.Handle handle)
  {
    FallerComponent data = ((KCompactedVector<FallerComponent>) GameComps.Fallers).GetData(handle);
    if (Object.op_Equality((Object) data.transform, (Object) null))
      return;
    Vector3 position = TransformExtensions.GetPosition(data.transform);
    position.y = (float) ((double) position.y - (double) data.offset - 0.10000000149011612);
    int cell = Grid.PosToCell(position);
    if (!Grid.IsValidCell(cell))
      return;
    bool flag = !Grid.Solid[cell];
    if (flag == data.isFalling)
      return;
    data.isFalling = flag;
    if (flag)
      FallerComponents.AddGravity(data.transform, Vector2.zero);
    else
      FallerComponents.RemoveGravity(data.transform);
  }
}
