// Decompiled with JetBrains decompiler
// Type: FallerComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public struct FallerComponent
{
  public Transform transform;
  public int transformInstanceId;
  public bool isFalling;
  public float offset;
  public Vector2 initialVelocity;
  public HandleVector<int>.Handle partitionerEntry;
  public Action<object> solidChangedCB;
  public System.Action cellChangedCB;

  public FallerComponent(Transform transform, Vector2 initial_velocity)
  {
    this.transform = transform;
    this.transformInstanceId = ((Object) transform).GetInstanceID();
    this.isFalling = false;
    this.initialVelocity = initial_velocity;
    this.partitionerEntry = new HandleVector<int>.Handle();
    this.solidChangedCB = (Action<object>) null;
    this.cellChangedCB = (System.Action) null;
    KCircleCollider2D component1 = ((Component) transform).GetComponent<KCircleCollider2D>();
    if (Object.op_Inequality((Object) component1, (Object) null))
    {
      this.offset = component1.radius;
    }
    else
    {
      KCollider2D component2 = ((Component) transform).GetComponent<KCollider2D>();
      if (Object.op_Inequality((Object) component2, (Object) null))
      {
        double y1 = (double) TransformExtensions.GetPosition(transform).y;
        Bounds bounds = component2.bounds;
        double y2 = (double) ((Bounds) ref bounds).min.y;
        this.offset = (float) (y1 - y2);
      }
      else
        this.offset = 0.0f;
    }
  }
}
