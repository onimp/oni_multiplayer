// Decompiled with JetBrains decompiler
// Type: GravityComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public struct GravityComponent
{
  public Transform transform;
  public Vector2 velocity;
  public float elapsedTime;
  public System.Action onLanded;
  public bool landOnFakeFloors;
  public bool mayLeaveWorld;
  public Vector2 extents;
  public float bottomYOffset;

  public GravityComponent(
    Transform transform,
    System.Action on_landed,
    Vector2 initial_velocity,
    bool land_on_fake_floors,
    bool mayLeaveWorld)
  {
    this.transform = transform;
    this.elapsedTime = 0.0f;
    this.velocity = initial_velocity;
    this.onLanded = on_landed;
    this.landOnFakeFloors = land_on_fake_floors;
    this.mayLeaveWorld = mayLeaveWorld;
    KCollider2D component = ((Component) transform).GetComponent<KCollider2D>();
    this.extents = GravityComponent.GetExtents(component);
    this.bottomYOffset = GravityComponent.GetGroundOffset(component);
  }

  public static float GetGroundOffset(KCollider2D collider)
  {
    if (!Object.op_Inequality((Object) collider, (Object) null))
      return 0.0f;
    Bounds bounds = collider.bounds;
    return ((Bounds) ref bounds).extents.y - collider.offset.y;
  }

  public static Vector2 GetExtents(KCollider2D collider)
  {
    if (!Object.op_Inequality((Object) collider, (Object) null))
      return Vector2.zero;
    Bounds bounds = collider.bounds;
    return Vector2.op_Implicit(((Bounds) ref bounds).extents);
  }

  public static Vector2 GetOffset(KCollider2D collider) => Object.op_Inequality((Object) collider, (Object) null) ? collider.offset : Vector2.zero;
}
