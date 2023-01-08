// Decompiled with JetBrains decompiler
// Type: KCircleCollider2D
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class KCircleCollider2D : KCollider2D
{
  [SerializeField]
  private float _radius;

  public float radius
  {
    get => this._radius;
    set
    {
      this._radius = value;
      this.MarkDirty();
    }
  }

  public override Extents GetExtents()
  {
    Vector3 vector3 = Vector3.op_Addition(TransformExtensions.GetPosition(this.transform), new Vector3(this.offset.x, this.offset.y, 0.0f));
    Vector2 vector2_1;
    // ISSUE: explicit constructor call
    ((Vector2) ref vector2_1).\u002Ector(vector3.x - this.radius, vector3.y - this.radius);
    Vector2 vector2_2 = new Vector2(vector3.x + this.radius, vector3.y + this.radius);
    int width = (int) vector2_2.x - (int) vector2_1.x + 1;
    int height = (int) vector2_2.y - (int) vector2_1.y + 1;
    return new Extents((int) ((double) vector3.x - (double) this._radius), (int) ((double) vector3.y - (double) this._radius), width, height);
  }

  public override Bounds bounds => new Bounds(Vector3.op_Addition(TransformExtensions.GetPosition(this.transform), new Vector3(this.offset.x, this.offset.y, 0.0f)), new Vector3(this._radius * 2f, this._radius * 2f, 0.0f));

  public override bool Intersects(Vector2 pos)
  {
    Vector3 position = TransformExtensions.GetPosition(this.transform);
    Vector2 vector2_1 = Vector2.op_Addition(new Vector2(position.x, position.y), this.offset);
    Vector2 vector2_2 = Vector2.op_Subtraction(pos, vector2_1);
    return (double) ((Vector2) ref vector2_2).sqrMagnitude <= (double) this._radius * (double) this._radius;
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.green;
    Bounds bounds = this.bounds;
    Gizmos.DrawWireSphere(((Bounds) ref bounds).center, this.radius);
  }
}
