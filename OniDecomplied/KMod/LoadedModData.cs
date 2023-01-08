// Decompiled with JetBrains decompiler
// Type: KMod.LoadedModData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;

namespace KMod
{
  public class LoadedModData
  {
    public Harmony harmony;
    public Dictionary<Assembly, UserMod2> userMod2Instances;
    public ICollection<Assembly> dlls;
    public ICollection<MethodBase> patched_methods;
  }
}
