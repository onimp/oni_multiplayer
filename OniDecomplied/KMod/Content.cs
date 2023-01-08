// Decompiled with JetBrains decompiler
// Type: KMod.Content
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;

namespace KMod
{
  [Flags]
  public enum Content : byte
  {
    LayerableFiles = 1,
    Strings = 2,
    DLL = 4,
    Translation = 8,
    Animation = 16, // 0x10
  }
}
