// Decompiled with JetBrains decompiler
// Type: Option`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using System.Diagnostics;

[DebuggerDisplay("has_value={hasValue} {value}")]
[Serializable]
public readonly struct Option<T> : IEquatable<Option<T>>, IEquatable<T>
{
  [Serialize]
  private readonly bool hasValue;
  [Serialize]
  private readonly T value;

  public bool HasValue => this.hasValue;

  public bool IsNone => !this.hasValue;

  public T Value
  {
    get
    {
      if (this.IsNone)
        throw new Exception("Tried to get a value for a Option<" + typeof (T).Name + ">, but IsNone is true");
      return this.value;
    }
  }

  public Option(T value)
  {
    this.value = value;
    this.hasValue = true;
  }

  public static implicit operator Option<T>(T value) => new Option<T>(value);

  public static implicit operator T(Option<T> option) => option.Value;

  public static implicit operator Option<T>(Option.Value_None value) => new Option<T>();

  public static implicit operator Option.Value_HasValue(Option<T> value) => new Option.Value_HasValue(value.hasValue);

  public void Deconstruct(out bool hasValue, out T value)
  {
    hasValue = this.hasValue;
    value = this.value;
  }

  public bool Equals(Option<T> other) => EqualityComparer<bool>.Default.Equals(this.hasValue, other.hasValue) && EqualityComparer<T>.Default.Equals(this.value, other.value);

  public override bool Equals(object obj) => obj is Option<T> other && this.Equals(other);

  public static bool operator ==(Option<T> lhs, Option<T> rhs) => lhs.Equals(rhs);

  public static bool operator !=(Option<T> lhs, Option<T> rhs) => !(lhs == rhs);

  public override int GetHashCode() => (-363764631 * -1521134295 + this.hasValue.GetHashCode()) * -1521134295 + EqualityComparer<T>.Default.GetHashCode(this.value);

  public override string ToString() => !this.hasValue ? "None" : string.Format("{0}", (object) this.value);

  public Option<U> IfHasValue<U>(Func<T, Option<U>> fn) => this.IsNone ? (Option<U>) Option.None : fn(this.Value);

  public static bool operator ==(Option<T> lhs, T rhs) => lhs.Equals(rhs);

  public static bool operator !=(Option<T> lhs, T rhs) => !(lhs == rhs);

  public static bool operator ==(T lhs, Option<T> rhs) => lhs.Equals((object) rhs);

  public static bool operator !=(T lhs, Option<T> rhs) => !(lhs == rhs);

  public bool Equals(T other) => this.HasValue && EqualityComparer<T>.Default.Equals(this.value, other);
}
