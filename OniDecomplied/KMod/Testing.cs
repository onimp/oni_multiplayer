// Decompiled with JetBrains decompiler
// Type: KMod.Testing
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

namespace KMod
{
  public static class Testing
  {
    public static Testing.DLLLoading dll_loading;
    public const Testing.SaveLoad SAVE_LOAD = Testing.SaveLoad.NoTesting;
    public const Testing.Install INSTALL = Testing.Install.NoTesting;
    public const Testing.Boot BOOT = Testing.Boot.NoTesting;

    public enum DLLLoading
    {
      NoTesting,
      Fail,
      UseModLoaderDLLExclusively,
    }

    public enum SaveLoad
    {
      NoTesting,
      FailSave,
      FailLoad,
    }

    public enum Install
    {
      NoTesting,
      ForceUninstall,
      ForceReinstall,
      ForceUpdate,
    }

    public enum Boot
    {
      NoTesting,
      Crash,
    }
  }
}
