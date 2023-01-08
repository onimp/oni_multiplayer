// Decompiled with JetBrains decompiler
// Type: KMod.DLLLoader
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace KMod
{
  internal static class DLLLoader
  {
    private const string managed_path = "Managed";

    public static bool LoadUserModLoaderDLL()
    {
      try
      {
        string path = System.IO.Path.Combine(System.IO.Path.Combine(Application.dataPath, "Managed"), "ModLoader.dll");
        if (!File.Exists(path))
          return false;
        Assembly assembly = Assembly.LoadFile(path);
        if (assembly == (Assembly) null)
          return false;
        System.Type type = assembly.GetType("ModLoader.ModLoader");
        if (type == (System.Type) null)
          return false;
        MethodInfo method = type.GetMethod("Start");
        if (method == (MethodInfo) null)
          return false;
        method.Invoke((object) null, (object[]) null);
        Debug.Log((object) "Successfully started ModLoader.dll");
        return true;
      }
      catch (Exception ex)
      {
        Debug.Log((object) ex.ToString());
      }
      return false;
    }

    public static LoadedModData LoadDLLs(Mod ownerMod, string harmonyId, string path, bool isDev)
    {
      LoadedModData loadedModData = new LoadedModData();
      try
      {
        if (Testing.dll_loading == Testing.DLLLoading.Fail || Testing.dll_loading == Testing.DLLLoading.UseModLoaderDLLExclusively)
          return (LoadedModData) null;
        DirectoryInfo directoryInfo = new DirectoryInfo(path);
        if (!directoryInfo.Exists)
          return (LoadedModData) null;
        List<Assembly> assemblyList = new List<Assembly>();
        foreach (FileInfo file in directoryInfo.GetFiles())
        {
          if (file.Name.ToLower().EndsWith(".dll"))
          {
            Debug.Log((object) string.Format("Loading MOD dll: {0}", (object) file.Name));
            Assembly assembly = Assembly.LoadFrom(file.FullName);
            if (assembly != (Assembly) null)
              assemblyList.Add(assembly);
          }
        }
        if (assemblyList.Count == 0)
          return (LoadedModData) null;
        loadedModData.dlls = (ICollection<Assembly>) new HashSet<Assembly>();
        loadedModData.userMod2Instances = new Dictionary<Assembly, UserMod2>();
        foreach (Assembly key in assemblyList)
        {
          loadedModData.dlls.Add(key);
          UserMod2 userMod2 = (UserMod2) null;
          foreach (System.Type type in key.GetTypes())
          {
            if (!(type == (System.Type) null) && typeof (UserMod2).IsAssignableFrom(type))
            {
              if (userMod2 != null)
              {
                Debug.LogError((object) ("Found more than one class inheriting `UserMod2` in " + key.FullName + ", only one per assembly is allowed. Aborting load."));
                return (LoadedModData) null;
              }
              userMod2 = Activator.CreateInstance(type) as UserMod2;
            }
          }
          if (userMod2 == null)
          {
            if (isDev)
              Debug.LogWarning((object) string.Format("{0} at {1} has no classes inheriting from UserMod, creating one...", (object) key.GetName(), (object) path));
            userMod2 = new UserMod2();
          }
          userMod2.assembly = key;
          userMod2.path = path;
          userMod2.mod = ownerMod;
          loadedModData.userMod2Instances[key] = userMod2;
        }
        loadedModData.harmony = new Harmony(harmonyId);
        if (loadedModData.harmony != null)
        {
          foreach (KeyValuePair<Assembly, UserMod2> userMod2Instance in loadedModData.userMod2Instances)
            userMod2Instance.Value.OnLoad(loadedModData.harmony);
        }
        loadedModData.patched_methods = (ICollection<MethodBase>) loadedModData.harmony.GetPatchedMethods().Where<MethodBase>((Func<MethodBase, bool>) (method => Harmony.GetPatchInfo(method).Owners.Contains(harmonyId))).ToList<MethodBase>();
        return loadedModData;
      }
      catch (Exception ex)
      {
        DebugUtil.LogException((Object) null, "Exception while loading mod " + harmonyId + " at " + path + ".", ex);
        return (LoadedModData) null;
      }
    }

    public static void PostLoadDLLs(
      string harmonyId,
      LoadedModData modData,
      IReadOnlyList<Mod> mods)
    {
      try
      {
        foreach (KeyValuePair<Assembly, UserMod2> userMod2Instance in modData.userMod2Instances)
          userMod2Instance.Value.OnAllModsLoaded(modData.harmony, mods);
      }
      catch (Exception ex)
      {
        DebugUtil.LogException((Object) null, "Exception while postLoading mod " + harmonyId + ".", ex);
      }
    }
  }
}
