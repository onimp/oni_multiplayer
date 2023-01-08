// Decompiled with JetBrains decompiler
// Type: LaunchInitializer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.IO;
using System.Threading;
using UnityEngine;

public class LaunchInitializer : MonoBehaviour
{
  private const string PREFIX = "U";
  private const int UPDATE_NUMBER = 44;
  private static readonly string BUILD_PREFIX = "U" + 44.ToString();
  public GameObject[] SpawnPrefabs;
  [SerializeField]
  private int numWaitFrames = 1;

  public static string BuildPrefix() => LaunchInitializer.BUILD_PREFIX;

  public static int UpdateNumber() => 44;

  private void Update()
  {
    if (this.numWaitFrames > Time.renderedFrameCount)
      return;
    if (!DistributionPlatform.Initialized)
    {
      if (!SystemInfo.SupportsTextureFormat((TextureFormat) 20))
        Debug.LogError((object) "Machine does not support RGBAFloat32");
      GraphicsOptionsScreen.SetSettingsFromPrefs();
      Util.ApplyInvariantCultureToThread(Thread.CurrentThread);
      Debug.Log((object) ("Current date: " + System.DateTime.Now.ToString()));
      Debug.Log((object) ("release Build: " + BuildWatermark.GetBuildText()));
      Object.DontDestroyOnLoad((Object) ((Component) this).gameObject);
      KPlayerPrefs.instance.Load();
      DistributionPlatform.Initialize();
    }
    if (!DistributionPlatform.Inst.IsDLCStatusReady())
      return;
    Debug.Log((object) "DistributionPlatform initialized.");
    Debug.Log((object) ("release Build: " + BuildWatermark.GetBuildText()));
    Debug.Log((object) string.Format("EXPANSION1 installed: {0}  active: {1}", (object) DlcManager.IsExpansion1Installed(), (object) DlcManager.IsExpansion1Active()));
    KFMOD.Initialize();
    for (int index = 0; index < this.SpawnPrefabs.Length; ++index)
    {
      GameObject spawnPrefab = this.SpawnPrefabs[index];
      if (Object.op_Inequality((Object) spawnPrefab, (Object) null))
        Util.KInstantiate(spawnPrefab, ((Component) this).gameObject, (string) null);
    }
    LaunchInitializer.DeleteLingeringFiles();
    ((Behaviour) this).enabled = false;
  }

  private static void DeleteLingeringFiles()
  {
    string[] strArray = new string[3]
    {
      "fmod.log",
      "load_stats_0.json",
      "OxygenNotIncluded_Data/output_log.txt"
    };
    string directoryName = System.IO.Path.GetDirectoryName(Application.dataPath);
    foreach (string path2 in strArray)
    {
      string path = System.IO.Path.Combine(directoryName, path2);
      try
      {
        if (File.Exists(path))
          File.Delete(path);
      }
      catch (Exception ex)
      {
        Debug.LogWarning((object) ex);
      }
    }
  }
}
