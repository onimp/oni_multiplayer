// Decompiled with JetBrains decompiler
// Type: IConsumableUIItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public interface IConsumableUIItem
{
  string ConsumableId { get; }

  string ConsumableName { get; }

  int MajorOrder { get; }

  int MinorOrder { get; }

  bool Display { get; }
}
