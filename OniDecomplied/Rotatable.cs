// Decompiled with JetBrains decompiler
// Type: Rotatable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/Rotatable")]
public class Rotatable : KMonoBehaviour, ISaveLoadable
{
  [MyCmpReq]
  private KBatchedAnimController batchedAnimController;
  [MyCmpGet]
  private Building building;
  [Serialize]
  [SerializeField]
  private Orientation orientation;
  [SerializeField]
  private Vector3 pivot = Vector3.zero;
  [SerializeField]
  private Vector3 visualizerOffset = Vector3.zero;
  public PermittedRotations permittedRotations;
  [SerializeField]
  private int width;
  [SerializeField]
  private int height;

  public Orientation Orientation => this.orientation;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (Object.op_Inequality((Object) this.building, (Object) null))
    {
      BuildingDef def = ((Component) this).GetComponent<Building>().Def;
      this.SetSize(def.WidthInCells, def.HeightInCells);
    }
    this.OrientVisualizer(this.orientation);
    this.OrientCollider(this.orientation);
  }

  public void SetSize(int width, int height)
  {
    this.width = width;
    this.height = height;
    if (width % 2 == 0)
    {
      this.pivot = new Vector3(-0.5f, 0.5f, 0.0f);
      this.visualizerOffset = new Vector3(0.5f, 0.0f, 0.0f);
    }
    else
    {
      this.pivot = new Vector3(0.0f, 0.5f, 0.0f);
      this.visualizerOffset = Vector3.zero;
    }
  }

  public Orientation Rotate()
  {
    switch (this.permittedRotations)
    {
      case PermittedRotations.R90:
        this.orientation = this.orientation == Orientation.Neutral ? Orientation.R90 : Orientation.Neutral;
        break;
      case PermittedRotations.R360:
        this.orientation = (Orientation) ((int) (this.orientation + 1) % 4);
        break;
      case PermittedRotations.FlipH:
        this.orientation = this.orientation == Orientation.Neutral ? Orientation.FlipH : Orientation.Neutral;
        break;
      case PermittedRotations.FlipV:
        this.orientation = this.orientation == Orientation.Neutral ? Orientation.FlipV : Orientation.Neutral;
        break;
    }
    this.OrientVisualizer(this.orientation);
    return this.orientation;
  }

  public void SetOrientation(Orientation new_orientation)
  {
    this.orientation = new_orientation;
    this.OrientVisualizer(new_orientation);
    this.OrientCollider(new_orientation);
  }

  public void Match(Rotatable other)
  {
    this.pivot = other.pivot;
    this.visualizerOffset = other.visualizerOffset;
    this.permittedRotations = other.permittedRotations;
    this.orientation = other.orientation;
    this.OrientVisualizer(this.orientation);
    this.OrientCollider(this.orientation);
  }

  public float GetVisualizerRotation()
  {
    switch (this.permittedRotations)
    {
      case PermittedRotations.R90:
      case PermittedRotations.R360:
        return -90f * (float) this.orientation;
      default:
        return 0.0f;
    }
  }

  public bool GetVisualizerFlipX() => this.orientation == Orientation.FlipH;

  public bool GetVisualizerFlipY() => this.orientation == Orientation.FlipV;

  public Vector3 GetVisualizerPivot()
  {
    Vector3 pivot = this.pivot;
    switch (this.orientation)
    {
      case Orientation.FlipH:
        pivot.x = -this.pivot.x;
        break;
    }
    return pivot;
  }

  private Vector3 GetVisualizerOffset()
  {
    Vector3 visualizerOffset;
    switch (this.orientation)
    {
      case Orientation.FlipH:
        // ISSUE: explicit constructor call
        ((Vector3) ref visualizerOffset).\u002Ector(-this.visualizerOffset.x, this.visualizerOffset.y, this.visualizerOffset.z);
        break;
      case Orientation.FlipV:
        // ISSUE: explicit constructor call
        ((Vector3) ref visualizerOffset).\u002Ector(this.visualizerOffset.x, 1f, this.visualizerOffset.z);
        break;
      default:
        visualizerOffset = this.visualizerOffset;
        break;
    }
    return visualizerOffset;
  }

  private void OrientVisualizer(Orientation orientation)
  {
    float visualizerRotation = this.GetVisualizerRotation();
    KBatchedAnimController component = ((Component) this).GetComponent<KBatchedAnimController>();
    component.Pivot = this.GetVisualizerPivot();
    component.Rotation = visualizerRotation;
    component.Offset = this.GetVisualizerOffset();
    component.FlipX = this.GetVisualizerFlipX();
    component.FlipY = this.GetVisualizerFlipY();
    this.Trigger(-1643076535, (object) this);
  }

  private void OrientCollider(Orientation orientation)
  {
    KBoxCollider2D component = ((Component) this).GetComponent<KBoxCollider2D>();
    if (Object.op_Equality((Object) component, (Object) null))
      return;
    float num1 = 0.5f * (float) ((this.width + 1) % 2);
    float num2 = 0.0f;
    switch (orientation)
    {
      case Orientation.R90:
        num2 = -90f;
        break;
      case Orientation.R180:
        num2 = -180f;
        break;
      case Orientation.R270:
        num2 = -270f;
        break;
      case Orientation.FlipH:
        component.offset = new Vector2((float) ((double) num1 + (double) (this.width % 2) - 1.0), 0.5f * (float) this.height);
        component.size = new Vector2((float) this.width, (float) this.height);
        break;
      case Orientation.FlipV:
        component.offset = new Vector2(num1, -0.5f * (float) (this.height - 2));
        component.size = new Vector2((float) this.width, (float) this.height);
        break;
      default:
        component.offset = new Vector2(num1, 0.5f * (float) this.height);
        component.size = new Vector2((float) this.width, (float) this.height);
        break;
    }
    if ((double) num2 == 0.0)
      return;
    Matrix2x3 matrix2x3_1 = Matrix2x3.Translate(Vector2.op_Implicit(Vector3.op_UnaryNegation(this.pivot)));
    Matrix2x3 matrix2x3_2 = Matrix2x3.Rotate(num2 * ((float) Math.PI / 180f));
    Matrix2x3 matrix2x3_3 = Matrix2x3.op_Multiply(Matrix2x3.op_Multiply(Matrix2x3.Translate(Vector2.op_Implicit(Vector3.op_Addition(this.pivot, new Vector3(num1, 0.0f, 0.0f)))), matrix2x3_2), matrix2x3_1);
    Vector2 vector2_1;
    // ISSUE: explicit constructor call
    ((Vector2) ref vector2_1).\u002Ector(-0.5f * (float) this.width, 0.0f);
    Vector2 vector2_2;
    // ISSUE: explicit constructor call
    ((Vector2) ref vector2_2).\u002Ector(0.5f * (float) this.width, (float) this.height);
    Vector2 vector2_3;
    // ISSUE: explicit constructor call
    ((Vector2) ref vector2_3).\u002Ector(0.0f, 0.5f * (float) this.height);
    Vector2 vector2_4 = Vector2.op_Implicit(((Matrix2x3) ref matrix2x3_3).MultiplyPoint(Vector2.op_Implicit(vector2_1)));
    vector2_2 = Vector2.op_Implicit(((Matrix2x3) ref matrix2x3_3).MultiplyPoint(Vector2.op_Implicit(vector2_2)));
    Vector2 vector2_5 = Vector2.op_Implicit(((Matrix2x3) ref matrix2x3_3).MultiplyPoint(Vector2.op_Implicit(vector2_3)));
    float num3 = Mathf.Min(vector2_4.x, vector2_2.x);
    float num4 = Mathf.Max(vector2_4.x, vector2_2.x);
    float num5 = Mathf.Min(vector2_4.y, vector2_2.y);
    float num6 = Mathf.Max(vector2_4.y, vector2_2.y);
    component.offset = vector2_5;
    component.size = new Vector2(num4 - num3, num6 - num5);
  }

  public CellOffset GetRotatedCellOffset(CellOffset offset) => Rotatable.GetRotatedCellOffset(offset, this.orientation);

  public static CellOffset GetRotatedCellOffset(CellOffset offset, Orientation orientation)
  {
    switch (orientation)
    {
      case Orientation.R90:
        return new CellOffset(offset.y, -offset.x);
      case Orientation.R180:
        return new CellOffset(-offset.x, -offset.y);
      case Orientation.R270:
        return new CellOffset(-offset.y, offset.x);
      case Orientation.FlipH:
        return new CellOffset(-offset.x, offset.y);
      case Orientation.FlipV:
        return new CellOffset(offset.x, -offset.y);
      default:
        return offset;
    }
  }

  public static CellOffset GetRotatedCellOffset(int x, int y, Orientation orientation) => Rotatable.GetRotatedCellOffset(new CellOffset(x, y), orientation);

  public Vector3 GetRotatedOffset(Vector3 offset) => Rotatable.GetRotatedOffset(offset, this.orientation);

  public static Vector3 GetRotatedOffset(Vector3 offset, Orientation orientation)
  {
    switch (orientation)
    {
      case Orientation.R90:
        return new Vector3(offset.y, -offset.x);
      case Orientation.R180:
        return new Vector3(-offset.x, -offset.y);
      case Orientation.R270:
        return new Vector3(-offset.y, offset.x);
      case Orientation.FlipH:
        return new Vector3(-offset.x, offset.y);
      case Orientation.FlipV:
        return new Vector3(offset.x, -offset.y);
      default:
        return offset;
    }
  }

  public Orientation GetOrientation() => this.orientation;

  public bool IsRotated => this.orientation != 0;
}
