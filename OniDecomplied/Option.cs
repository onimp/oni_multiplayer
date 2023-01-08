// Decompiled with JetBrains decompiler
// Type: Option
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Runtime.InteropServices;

public static class Option
{
  public static Option<T> Some<T>(T value) => new Option<T>(value);

  public static Option.Value_None None => new Option.Value_None();

  public static bool AllHaveValues(params Option.Value_HasValue[] options)
  {
    if (options == null || options.Length == 0)
      return false;
    for (int index = 0; index < options.Length; ++index)
    {
      if (!options[index].HasValue)
        return false;
    }
    return true;
  }

  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public readonly struct Value_None
  {
  }

  public readonly struct Value_HasValue
  {
    public readonly bool HasValue;

    public Value_HasValue(bool hasValue) => this.HasValue = hasValue;
  }
}
