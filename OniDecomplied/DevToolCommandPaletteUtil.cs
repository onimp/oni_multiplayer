// Decompiled with JetBrains decompiler
// Type: DevToolCommandPaletteUtil
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Reflection;

public static class DevToolCommandPaletteUtil
{
  public static List<DevToolCommandPalette.Command> GenerateDefaultCommandPalette()
  {
    List<DevToolCommandPalette.Command> defaultCommandPalette = new List<DevToolCommandPalette.Command>();
    foreach (System.Type type in ReflectionUtil.CollectTypesThatInheritOrImplement<DevTool>(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
    {
      System.Type devToolType = type;
      if (!devToolType.IsAbstract && ReflectionUtil.HasDefaultConstructor(devToolType))
        defaultCommandPalette.Add(new DevToolCommandPalette.Command("[DevTool] Open \"" + DevToolUtil.GenerateDevToolName(devToolType) + "\"", (System.Action) (() => DevToolUtil.Open((DevTool) Activator.CreateInstance(devToolType)))));
    }
    return defaultCommandPalette;
  }
}
