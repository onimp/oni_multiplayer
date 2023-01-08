// Decompiled with JetBrains decompiler
// Type: KMod.UserMod2
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;

namespace KMod
{
  public class UserMod2
  {
    public Assembly assembly { get; set; }

    public string path { get; set; }

    public Mod mod { get; set; }

    public virtual void OnLoad(Harmony harmony) => harmony.PatchAll(this.assembly);

    public virtual void OnAllModsLoaded(Harmony harmony, IReadOnlyList<Mod> mods)
    {
    }
  }
}
