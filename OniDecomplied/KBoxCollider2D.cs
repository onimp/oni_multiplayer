// Decompiled with JetBrains decompiler
// Type: KBoxCollider2D
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class KBoxCollider2D : KCollider2D
{
  [SerializeField]
  private Vector2 _size;

  public Vector2 size
  {
    get => this._size;
    set
    {
      this._size = value;
      this.MarkDirty();
    }
  }

  public override Extents GetExtents()
  {
    Vector3 vector3 = Vector3.op_Addition(TransformExtensions.GetPosition(this.transform), new Vector3(this.offset.x, this.offset.y, 0.0f));
    Vector2 vector2_1 = Vector2.op_Multiply(this.size, 0.9999f);
    Vector2 vector2_2;
    // ISSUE: explicit constructor call
    ((Vector2) ref vector2_2).\u002Ector(vector3.x - vector2_1.x * 0.5f, vector3.y - vector2_1.y * 0.5f);
    Vector2 vector2_3;
    // ISSUE: explicit constructor call
    ((Vector2) ref vector2_3).\u002Ector(vector3.x + vector2_1.x * 0.5f, vector3.y + vector2_1.y * 0.5f);
    Vector2I vector2I1;
    // ISSUE: explicit constructor call
    ((Vector2I) ref vector2I1).\u002Ector((int) vector2_2.x, (int) vector2_2.y);
    Vector2I vector2I2 = new Vector2I((int) vector2_3.x, (int) vector2_3.y);
    int width = vector2I2.x - vector2I1.x + 1;
    int height = vector2I2.y - vector2I1.y + 1;
    return new Extents(vector2I1.x, vector2I1.y, width, height);
  }

  public override bool Intersects(Vector2 intersect_pos)
  {
    Vector3 vector3 = Vector3.op_Addition(TransformExtensions.GetPosition(this.transform), new Vector3(this.offset.x, this.offset.y, 0.0f));
    Vector2 vector2_1;
    // ISSUE: explicit constructor call
    ((Vector2) ref vector2_1).\u002Ector(vector3.x - this.size.x * 0.5f, vector3.y - this.size.y * 0.5f);
    Vector2 vector2_2;
    // ISSUE: explicit constructor call
    ((Vector2) ref vector2_2).\u002Ector(vector3.x + this.size.x * 0.5f, vector3.y + this.size.y * 0.5f);
    return (double) intersect_pos.x >= (double) vector2_1.x && (double) intersect_pos.x <= (double) vector2_2.x && (double) intersect_pos.y >= (double) vector2_1.y && (double) intersect_pos.y <= (double) vector2_2.y;
  }

  public override Bounds bounds => new Bounds(Vector3.op_Addition(TransformExtensions.GetPosition(this.transform), new Vector3(this.offset.x, this.offset.y, 0.0f)), new Vector3(this._size.x, this._size.y, 0.0f));

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.green;
    Bounds bounds = this.bounds;
    Gizmos.DrawWireCube(((Bounds) ref bounds).center, new Vector3(this._size.x, this._size.y, 0.0f));
  }
}
