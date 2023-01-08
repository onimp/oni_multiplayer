// Decompiled with JetBrains decompiler
// Type: Padding
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public readonly struct Padding
{
  public readonly float top;
  public readonly float bottom;
  public readonly float left;
  public readonly float right;

  public float Width => this.left + this.right;

  public float Height => this.top + this.bottom;

  public Padding(float left, float right, float top, float bottom)
  {
    this.top = top;
    this.bottom = bottom;
    this.left = left;
    this.right = right;
  }

  public static Padding All(float padding) => new Padding(padding, padding, padding, padding);

  public static Padding Symmetric(float horizontal, float vertical) => new Padding(horizontal, horizontal, vertical, vertical);

  public static Padding Only(float left = 0.0f, float right = 0.0f, float top = 0.0f, float bottom = 0.0f) => new Padding(left, right, top, bottom);

  public static Padding Vertical(float vertical) => new Padding(0.0f, 0.0f, vertical, vertical);

  public static Padding Horizontal(float horizontal) => new Padding(horizontal, horizontal, 0.0f, 0.0f);

  public static Padding Top(float amount) => new Padding(0.0f, 0.0f, amount, 0.0f);

  public static Padding Left(float amount) => new Padding(amount, 0.0f, 0.0f, 0.0f);

  public static Padding Bottom(float amount) => new Padding(0.0f, 0.0f, 0.0f, amount);

  public static Padding Right(float amount) => new Padding(0.0f, amount, 0.0f, 0.0f);

  public static Padding operator +(Padding a, Padding b) => new Padding(a.left + b.left, a.right + b.right, a.top + b.top, a.bottom + b.bottom);

  public static Padding operator -(Padding a, Padding b) => new Padding(a.left - b.left, a.right - b.right, a.top - b.top, a.bottom - b.bottom);

  public static Padding operator *(float f, Padding p) => p * f;

  public static Padding operator *(Padding p, float f) => new Padding(p.left * f, p.right * f, p.top * f, p.bottom * f);

  public static Padding operator /(Padding p, float f) => new Padding(p.left / f, p.right / f, p.top / f, p.bottom / f);
}
