// Decompiled with JetBrains decompiler
// Type: SaveLoader
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Ionic.Zlib;
using Klei;
using Klei.AI;
using Klei.CustomSettings;
using KMod;
using KSerialization;
using Newtonsoft.Json;
using ProcGen;
using ProcGenGame;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SaveLoader")]
public class SaveLoader : KMonoBehaviour
{
  [MyCmpGet]
  private GridSettings gridSettings;
  private bool saveFileCorrupt;
  private bool compressSaveData = true;
  private int lastUncompressedSize;
  public bool saveAsText;
  public const string MAINMENU_LEVELNAME = "launchscene";
  public const string FRONTEND_LEVELNAME = "frontend";
  public const string BACKEND_LEVELNAME = "backend";
  public const string SAVE_EXTENSION = ".sav";
  public const string AUTOSAVE_FOLDER = "auto_save";
  public const string CLOUDSAVE_FOLDER = "cloud_save_files";
  public const string SAVE_FOLDER = "save_files";
  public const int MAX_AUTOSAVE_FILES = 10;
  [NonSerialized]
  public SaveManager saveManager;
  private Cluster m_clusterLayout;
  private const string CorruptFileSuffix = "_";
  private const float SAVE_BUFFER_HEAD_ROOM = 0.1f;
  private bool mustRestartOnFail;
  public const string METRIC_SAVED_PREFAB_KEY = "SavedPrefabs";
  public const string METRIC_IS_AUTO_SAVE_KEY = "IsAutoSave";
  public const string METRIC_WAS_DEBUG_EVER_USED = "WasDebugEverUsed";
  public const string METRIC_IS_SANDBOX_ENABLED = "IsSandboxEnabled";
  public const string METRIC_RESOURCES_ACCESSIBLE_KEY = "ResourcesAccessible";
  public const string METRIC_DAILY_REPORT_KEY = "DailyReport";
  public const string METRIC_MINION_METRICS_KEY = "MinionMetrics";
  public const string METRIC_CUSTOM_GAME_SETTINGS = "CustomGameSettings";
  public const string METRIC_PERFORMANCE_MEASUREMENTS = "PerformanceMeasurements";
  public const string METRIC_FRAME_TIME = "AverageFrameTime";
  private static bool force_infinity;

  public bool loadedFromSave { get; private set; }

  public static void DestroyInstance() => SaveLoader.Instance = (SaveLoader) null;

  public static SaveLoader Instance { get; private set; }

  public Action<Cluster> OnWorldGenComplete { get; set; }

  public Cluster ClusterLayout => this.m_clusterLayout;

  public SaveGame.GameInfo GameInfo { get; private set; }

  protected virtual void OnPrefabInit()
  {
    SaveLoader.Instance = this;
    this.saveManager = ((Component) this).GetComponent<SaveManager>();
  }

  private void MoveCorruptFile(string filename)
  {
  }

  protected virtual void OnSpawn()
  {
    string activeSaveFilePath = SaveLoader.GetActiveSaveFilePath();
    if (WorldGen.CanLoad(activeSaveFilePath))
    {
      Sim.SIM_Initialize(new Sim.GAME_MessageHandler(Sim.DLL_MessageHandler));
      SimMessages.CreateSimElementsTable(ElementLoader.elements);
      SimMessages.CreateDiseaseTable(Db.Get().Diseases);
      this.loadedFromSave = true;
      this.loadedFromSave = this.Load(activeSaveFilePath);
      this.saveFileCorrupt = !this.loadedFromSave;
      if (!this.loadedFromSave)
      {
        SaveLoader.SetActiveSaveFilePath((string) null);
        if (this.mustRestartOnFail)
        {
          this.MoveCorruptFile(activeSaveFilePath);
          Sim.Shutdown();
          App.LoadScene("frontend");
          return;
        }
      }
    }
    if (this.loadedFromSave)
      return;
    Sim.Shutdown();
    if (!string.IsNullOrEmpty(activeSaveFilePath))
      DebugUtil.LogArgs(new object[1]
      {
        (object) ("Couldn't load [" + activeSaveFilePath + "]")
      });
    if (this.saveFileCorrupt)
      this.MoveCorruptFile(activeSaveFilePath);
    int baseID = 0;
    bool flag = WorldGen.CanLoad(WorldGen.GetSIMSaveFilename(baseID));
    if (flag && this.LoadFromWorldGen())
      return;
    DebugUtil.LogWarningArgs(new object[1]
    {
      (object) "Couldn't start new game with current world gen, moving file"
    });
    if (flag)
    {
      KMonoBehaviour.isLoadingScene = true;
      for (; FileSystem.FileExists(WorldGen.GetSIMSaveFilename(baseID)); ++baseID)
        this.MoveCorruptFile(WorldGen.GetSIMSaveFilename(baseID));
    }
    App.LoadScene("frontend");
  }

  private static void CompressContents(BinaryWriter fileWriter, byte[] uncompressed, int length)
  {
    using (ZlibStream zlibStream = new ZlibStream(fileWriter.BaseStream, (CompressionMode) 0, (CompressionLevel) 1))
    {
      ((Stream) zlibStream).Write(uncompressed, 0, length);
      ((Stream) zlibStream).Flush();
    }
  }

  private byte[] FloatToBytes(float[] floats)
  {
    byte[] dst = new byte[floats.Length * 4];
    Buffer.BlockCopy((Array) floats, 0, (Array) dst, 0, dst.Length);
    return dst;
  }

  private static byte[] DecompressContents(byte[] compressed) => ZlibStream.UncompressBuffer(compressed);

  private float[] BytesToFloat(byte[] bytes)
  {
    float[] dst = new float[bytes.Length / 4];
    Buffer.BlockCopy((Array) bytes, 0, (Array) dst, 0, bytes.Length);
    return dst;
  }

  private SaveFileRoot PrepSaveFile()
  {
    SaveFileRoot saveFileRoot = new SaveFileRoot();
    saveFileRoot.WidthInCells = Grid.WidthInCells;
    saveFileRoot.HeightInCells = Grid.HeightInCells;
    saveFileRoot.streamed["GridVisible"] = Grid.Visible;
    saveFileRoot.streamed["GridSpawnable"] = Grid.Spawnable;
    saveFileRoot.streamed["GridDamage"] = this.FloatToBytes(Grid.Damage);
    Global.Instance.modManager.SendMetricsEvent();
    saveFileRoot.active_mods = new List<KMod.Label>();
    foreach (KMod.Mod mod in Global.Instance.modManager.mods)
    {
      if (mod.IsEnabledForActiveDlc())
        saveFileRoot.active_mods.Add(mod.label);
    }
    using (MemoryStream output = new MemoryStream())
    {
      using (BinaryWriter writer = new BinaryWriter((Stream) output))
        ((Component) ((Component) Camera.main).transform.parent).GetComponent<CameraController>().Save(writer);
      saveFileRoot.streamed["Camera"] = output.ToArray();
    }
    return saveFileRoot;
  }

  private void Save(BinaryWriter writer)
  {
    IOHelper.WriteKleiString(writer, "world");
    Serializer.Serialize((object) this.PrepSaveFile(), writer);
    Game.SaveSettings(writer);
    Sim.Save(writer, 0, 0);
    this.saveManager.Save(writer);
    Game.Instance.Save(writer);
  }

  private bool Load(IReader reader)
  {
    Debug.Assert(reader.ReadKleiString() == "world");
    Deserializer deserializer = new Deserializer(reader);
    SaveFileRoot saveFileRoot = new SaveFileRoot();
    deserializer.Deserialize((object) saveFileRoot);
    if ((this.GameInfo.saveMajorVersion == 7 || this.GameInfo.saveMinorVersion < 8) && saveFileRoot.requiredMods != null)
    {
      saveFileRoot.active_mods = new List<KMod.Label>();
      foreach (ModInfo requiredMod in saveFileRoot.requiredMods)
        saveFileRoot.active_mods.Add(new KMod.Label()
        {
          id = requiredMod.assetID,
          version = (long) requiredMod.lastModifiedTime,
          distribution_platform = KMod.Label.DistributionPlatform.Steam,
          title = requiredMod.description
        });
      saveFileRoot.requiredMods.Clear();
    }
    Manager modManager = Global.Instance.modManager;
    modManager.Load(Content.LayerableFiles);
    if (!modManager.MatchFootprint(saveFileRoot.active_mods, Content.LayerableFiles | Content.Strings | Content.DLL | Content.Translation | Content.Animation))
      DebugUtil.LogWarningArgs(new object[1]
      {
        (object) "Mod footprint of save file doesn't match current mod configuration"
      });
    Global.Instance.modManager.SendMetricsEvent();
    WorldGen.LoadSettings();
    CustomGameSettings.Instance.LoadClusters();
    if (this.GameInfo.clusterId == null)
    {
      SaveGame.GameInfo gameInfo = this.GameInfo;
      if (!string.IsNullOrEmpty(saveFileRoot.clusterID))
      {
        gameInfo.clusterId = saveFileRoot.clusterID;
      }
      else
      {
        try
        {
          gameInfo.clusterId = CustomGameSettings.Instance.GetCurrentQualitySetting((SettingConfig) CustomGameSettingConfigs.ClusterLayout).id;
        }
        catch
        {
          gameInfo.clusterId = WorldGenSettings.ClusterDefaultName;
          CustomGameSettings.Instance.SetQualitySetting((SettingConfig) CustomGameSettingConfigs.ClusterLayout, gameInfo.clusterId);
        }
      }
      this.GameInfo = gameInfo;
    }
    Game.clusterId = this.GameInfo.clusterId;
    Game.LoadSettings(deserializer);
    GridSettings.Reset(saveFileRoot.WidthInCells, saveFileRoot.HeightInCells);
    if (Application.isPlaying)
      Singleton<KBatchedAnimUpdater>.Instance.InitializeGrid();
    Sim.SIM_Initialize(new Sim.GAME_MessageHandler(Sim.DLL_MessageHandler));
    SimMessages.CreateSimElementsTable(ElementLoader.elements);
    Sim.AllocateCells(saveFileRoot.WidthInCells, saveFileRoot.HeightInCells);
    SimMessages.CreateDiseaseTable(Db.Get().Diseases);
    Sim.HandleMessage(SimMessageHashes.ClearUnoccupiedCells, 0, (byte[]) null);
    if (Sim.LoadWorld(!saveFileRoot.streamed.ContainsKey("Sim") ? reader : (IReader) new FastReader(saveFileRoot.streamed["Sim"])) != 0)
    {
      DebugUtil.LogWarningArgs(new object[1]
      {
        (object) "\n--- Error loading save ---\nSimDLL found bad data\n"
      });
      Sim.Shutdown();
      return false;
    }
    Sim.Start();
    SceneInitializer.Instance.PostLoadPrefabs();
    this.mustRestartOnFail = true;
    if (!this.saveManager.Load(reader))
    {
      Sim.Shutdown();
      DebugUtil.LogWarningArgs(new object[1]
      {
        (object) "\n--- Error loading save ---\n"
      });
      SaveLoader.SetActiveSaveFilePath((string) null);
      return false;
    }
    Grid.Visible = saveFileRoot.streamed["GridVisible"];
    if (saveFileRoot.streamed.ContainsKey("GridSpawnable"))
      Grid.Spawnable = saveFileRoot.streamed["GridSpawnable"];
    Grid.Damage = this.BytesToFloat(saveFileRoot.streamed["GridDamage"]);
    Game.Instance.Load(deserializer);
    CameraSaveData.Load(new FastReader(saveFileRoot.streamed["Camera"]));
    ClusterManager.Instance.InitializeWorldGrid();
    SimMessages.DefineWorldOffsets(ClusterManager.Instance.WorldContainers.Select<WorldContainer, SimMessages.WorldOffsetData>((Func<WorldContainer, SimMessages.WorldOffsetData>) (container => new SimMessages.WorldOffsetData()
    {
      worldOffsetX = container.WorldOffset.x,
      worldOffsetY = container.WorldOffset.y,
      worldSizeX = container.WorldSize.x,
      worldSizeY = container.WorldSize.y
    })).ToList<SimMessages.WorldOffsetData>());
    return true;
  }

  public static string GetSavePrefix() => System.IO.Path.Combine(Util.RootFolder(), string.Format("{0}{1}", (object) "save_files", (object) System.IO.Path.DirectorySeparatorChar));

  public static string GetCloudSavePrefix()
  {
    string path1 = System.IO.Path.Combine(Util.RootFolder(), string.Format("{0}{1}", (object) "cloud_save_files", (object) System.IO.Path.DirectorySeparatorChar));
    string userId = SaveLoader.GetUserID();
    if (string.IsNullOrEmpty(userId))
      return (string) null;
    string path = System.IO.Path.Combine(path1, userId);
    if (!System.IO.Directory.Exists(path))
      System.IO.Directory.CreateDirectory(path);
    return path;
  }

  public static string GetSavePrefixAndCreateFolder()
  {
    string savePrefix = SaveLoader.GetSavePrefix();
    if (!System.IO.Directory.Exists(savePrefix))
      System.IO.Directory.CreateDirectory(savePrefix);
    return savePrefix;
  }

  public static string GetUserID() => ((object) DistributionPlatform.Inst.LocalUser?.Id).ToString();

  public static string GetNextUsableSavePath(string filename)
  {
    int num = 0;
    string str = System.IO.Path.ChangeExtension(filename, (string) null);
    while (File.Exists(filename))
    {
      filename = SaveScreen.GetValidSaveFilename(string.Format("{0} ({1})", (object) str, (object) num));
      ++num;
    }
    return filename;
  }

  public static string GetOriginalSaveFileName(string filename)
  {
    if (!filename.Contains("/") && !filename.Contains("\\"))
      return filename;
    filename.Replace('\\', '/');
    return System.IO.Path.GetFileName(filename);
  }

  public static bool IsSaveAuto(string filename)
  {
    filename = filename.Replace('\\', '/');
    return filename.Contains("/auto_save/");
  }

  public static bool IsSaveLocal(string filename)
  {
    filename = filename.Replace('\\', '/');
    return filename.Contains("/save_files/");
  }

  public static bool IsSaveCloud(string filename)
  {
    filename = filename.Replace('\\', '/');
    return filename.Contains("/cloud_save_files/");
  }

  public static string GetAutoSavePrefix()
  {
    string path = System.IO.Path.Combine(SaveLoader.GetSavePrefixAndCreateFolder(), string.Format("{0}{1}", (object) "auto_save", (object) System.IO.Path.DirectorySeparatorChar));
    if (!System.IO.Directory.Exists(path))
      System.IO.Directory.CreateDirectory(path);
    return path;
  }

  public static void SetActiveSaveFilePath(string path) => KPlayerPrefs.SetString("SaveFilenameKey/", path);

  public static string GetActiveSaveFilePath() => KPlayerPrefs.GetString("SaveFilenameKey/");

  public static string GetActiveAutoSavePath()
  {
    string activeSaveFilePath = SaveLoader.GetActiveSaveFilePath();
    return activeSaveFilePath == null ? SaveLoader.GetAutoSavePrefix() : System.IO.Path.Combine(System.IO.Path.GetDirectoryName(activeSaveFilePath), "auto_save");
  }

  public static string GetAutosaveFilePath() => SaveLoader.GetAutoSavePrefix() + "AutoSave Cycle 1.sav";

  public static string GetActiveSaveColonyFolder() => SaveLoader.GetActiveSaveFolder() ?? System.IO.Path.Combine(SaveLoader.GetSavePrefix(), SaveLoader.Instance.GameInfo.baseName);

  public static string GetActiveSaveFolder()
  {
    string activeSaveFilePath = SaveLoader.GetActiveSaveFilePath();
    return !string.IsNullOrEmpty(activeSaveFilePath) ? System.IO.Path.GetDirectoryName(activeSaveFilePath) : (string) null;
  }

  public static List<SaveLoader.SaveFileEntry> GetSaveFiles(
    string save_dir,
    bool sort,
    SearchOption search = SearchOption.AllDirectories)
  {
    List<SaveLoader.SaveFileEntry> saveFiles = new List<SaveLoader.SaveFileEntry>();
    if (string.IsNullOrEmpty(save_dir))
      return saveFiles;
    try
    {
      if (!System.IO.Directory.Exists(save_dir))
        System.IO.Directory.CreateDirectory(save_dir);
      foreach (string file in System.IO.Directory.GetFiles(save_dir, "*.sav", search))
      {
        try
        {
          System.DateTime lastWriteTimeUtc = File.GetLastWriteTimeUtc(file);
          SaveLoader.SaveFileEntry saveFileEntry = new SaveLoader.SaveFileEntry()
          {
            path = file,
            timeStamp = lastWriteTimeUtc
          };
          saveFiles.Add(saveFileEntry);
        }
        catch (Exception ex)
        {
          Debug.LogWarning((object) ("Problem reading file: " + file + "\n" + ex.ToString()));
        }
      }
      if (sort)
        saveFiles.Sort((Comparison<SaveLoader.SaveFileEntry>) ((x, y) => y.timeStamp.CompareTo(x.timeStamp)));
    }
    catch (Exception ex)
    {
      string text = (string) null;
      if (ex is UnauthorizedAccessException)
        text = string.Format((string) UI.FRONTEND.SUPPORTWARNINGS.SAVE_DIRECTORY_READ_ONLY, (object) save_dir);
      else if (ex is IOException)
        text = string.Format((string) UI.FRONTEND.SUPPORTWARNINGS.SAVE_DIRECTORY_INSUFFICIENT_SPACE, (object) save_dir);
      if (text == null)
        throw ex;
      Util.KInstantiateUI(((Component) ScreenPrefabs.Instance.ConfirmDialogScreen).gameObject, Object.op_Equality((Object) FrontEndManager.Instance, (Object) null) ? GameScreenManager.Instance.ssOverlayCanvas : ((Component) FrontEndManager.Instance).gameObject, true).GetComponent<ConfirmDialogScreen>().PopupConfirmDialog(text, (System.Action) null, (System.Action) null);
    }
    return saveFiles;
  }

  public static List<SaveLoader.SaveFileEntry> GetAllFiles(bool sort, SaveLoader.SaveType type = SaveLoader.SaveType.both)
  {
    switch (type)
    {
      case SaveLoader.SaveType.local:
        return SaveLoader.GetSaveFiles(SaveLoader.GetSavePrefixAndCreateFolder(), sort);
      case SaveLoader.SaveType.cloud:
        return SaveLoader.GetSaveFiles(SaveLoader.GetCloudSavePrefix(), sort);
      case SaveLoader.SaveType.both:
        List<SaveLoader.SaveFileEntry> saveFiles1 = SaveLoader.GetSaveFiles(SaveLoader.GetSavePrefixAndCreateFolder(), false);
        List<SaveLoader.SaveFileEntry> saveFiles2 = SaveLoader.GetSaveFiles(SaveLoader.GetCloudSavePrefix(), false);
        saveFiles1.AddRange((IEnumerable<SaveLoader.SaveFileEntry>) saveFiles2);
        if (sort)
          saveFiles1.Sort((Comparison<SaveLoader.SaveFileEntry>) ((x, y) => y.timeStamp.CompareTo(x.timeStamp)));
        return saveFiles1;
      default:
        return new List<SaveLoader.SaveFileEntry>();
    }
  }

  public static List<SaveLoader.SaveFileEntry> GetAllColonyFiles(bool sort, SearchOption search = SearchOption.TopDirectoryOnly) => SaveLoader.GetSaveFiles(SaveLoader.GetActiveSaveColonyFolder(), sort, search);

  public static bool GetCloudSavesDefault() => !(SaveLoader.GetCloudSavesDefaultPref() == "Disabled");

  public static string GetCloudSavesDefaultPref()
  {
    string savesDefaultPref = KPlayerPrefs.GetString("SavesDefaultToCloud", "Enabled");
    if (savesDefaultPref != "Enabled" && savesDefaultPref != "Disabled")
      savesDefaultPref = "Enabled";
    return savesDefaultPref;
  }

  public static void SetCloudSavesDefault(bool value) => SaveLoader.SetCloudSavesDefaultPref(value ? "Enabled" : "Disabled");

  public static void SetCloudSavesDefaultPref(string pref)
  {
    if (pref != "Enabled" && pref != "Disabled")
      Debug.LogWarning((object) ("Ignoring cloud saves default pref `" + pref + "` as it's not valid, expected `Enabled` or `Disabled`"));
    else
      KPlayerPrefs.SetString("SavesDefaultToCloud", pref);
  }

  public static bool GetCloudSavesAvailable() => !string.IsNullOrEmpty(SaveLoader.GetUserID()) && SaveLoader.GetCloudSavePrefix() != null;

  public static string GetLatestSaveFile()
  {
    List<SaveLoader.SaveFileEntry> allFiles = SaveLoader.GetAllFiles(true);
    return allFiles.Count == 0 ? (string) null : allFiles[0].path;
  }

  public static string GetLatestSaveForCurrentDLC()
  {
    List<SaveLoader.SaveFileEntry> allFiles = SaveLoader.GetAllFiles(true);
    for (int index = 0; index < allFiles.Count; ++index)
    {
      Tuple<SaveGame.Header, SaveGame.GameInfo> fileInfo = SaveGame.GetFileInfo(allFiles[index].path);
      if (fileInfo != null)
      {
        SaveGame.Header first = fileInfo.first;
        SaveGame.GameInfo second = fileInfo.second;
        if (second.saveMajorVersion >= 7 && DlcManager.GetHighestActiveDlcId() == second.dlcId)
          return allFiles[index].path;
      }
    }
    return (string) null;
  }

  public void InitialSave()
  {
    string filename = SaveLoader.GetActiveSaveFilePath();
    if (string.IsNullOrEmpty(filename))
      filename = SaveLoader.GetAutosaveFilePath();
    else if (!filename.Contains(".sav"))
      filename += ".sav";
    this.Save(filename);
  }

  public string Save(string filename, bool isAutoSave = false, bool updateSavePointer = true)
  {
    Manager.Clear();
    string directoryName = System.IO.Path.GetDirectoryName(filename);
    try
    {
      if (directoryName != null)
      {
        if (!System.IO.Directory.Exists(directoryName))
          System.IO.Directory.CreateDirectory(directoryName);
      }
    }
    catch (Exception ex)
    {
      Debug.LogWarning((object) ("Problem creating save folder for " + filename + "!\n" + ex.ToString()));
    }
    this.ReportSaveMetrics(isAutoSave);
    RetireColonyUtility.SaveColonySummaryData();
    if (isAutoSave && !GenericGameSettings.instance.keepAllAutosaves)
    {
      List<SaveLoader.SaveFileEntry> saveFiles = SaveLoader.GetSaveFiles(SaveLoader.GetActiveAutoSavePath(), true);
      List<string> stringList = new List<string>();
      foreach (SaveLoader.SaveFileEntry saveFileEntry in saveFiles)
      {
        Tuple<SaveGame.Header, SaveGame.GameInfo> fileInfo = SaveGame.GetFileInfo(saveFileEntry.path);
        if (fileInfo != null && SaveGame.GetSaveUniqueID(fileInfo.second) == SaveLoader.Instance.GameInfo.colonyGuid.ToString())
          stringList.Add(saveFileEntry.path);
      }
      for (int index = stringList.Count - 1; index >= 9; --index)
      {
        string path1 = stringList[index];
        try
        {
          Debug.Log((object) ("Deleting old autosave: " + path1));
          File.Delete(path1);
        }
        catch (Exception ex)
        {
          Debug.LogWarning((object) ("Problem deleting autosave: " + path1 + "\n" + ex.ToString()));
        }
        string path2 = System.IO.Path.ChangeExtension(path1, ".png");
        try
        {
          if (File.Exists(path2))
            File.Delete(path2);
        }
        catch (Exception ex)
        {
          Debug.LogWarning((object) ("Problem deleting autosave screenshot: " + path2 + "\n" + ex.ToString()));
        }
      }
    }
    using (MemoryStream output = new MemoryStream((int) ((double) this.lastUncompressedSize * 1.1000000238418579)))
    {
      using (BinaryWriter writer = new BinaryWriter((Stream) output))
      {
        this.Save(writer);
        this.lastUncompressedSize = (int) output.Length;
        try
        {
          using (BinaryWriter fileWriter = new BinaryWriter((Stream) File.Open(filename, FileMode.Create)))
          {
            SaveGame.Header header;
            byte[] saveHeader = SaveGame.Instance.GetSaveHeader(isAutoSave, this.compressSaveData, out header);
            fileWriter.Write(header.buildVersion);
            fileWriter.Write(header.headerSize);
            fileWriter.Write(header.headerVersion);
            fileWriter.Write(header.compression);
            fileWriter.Write(saveHeader);
            Manager.SerializeDirectory(fileWriter);
            if (this.compressSaveData)
              SaveLoader.CompressContents(fileWriter, output.GetBuffer(), (int) output.Length);
            else
              fileWriter.Write(output.ToArray());
            KCrashReporter.MOST_RECENT_SAVEFILE = filename;
            Stats.Print();
          }
        }
        catch (Exception ex)
        {
          switch (ex)
          {
            case UnauthorizedAccessException _:
              DebugUtil.LogArgs(new object[1]
              {
                (object) ("UnauthorizedAccessException for " + filename)
              });
              ((ConfirmDialogScreen) GameScreenManager.Instance.StartScreen(((Component) ScreenPrefabs.Instance.ConfirmDialogScreen).gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, (GameScreenManager.UIRenderTarget) 2)).PopupConfirmDialog(string.Format((string) UI.CRASHSCREEN.SAVEFAILED, (object) "Unauthorized Access Exception"), (System.Action) null, (System.Action) null);
              return SaveLoader.GetActiveSaveFilePath();
            case IOException _:
              DebugUtil.LogArgs(new object[1]
              {
                (object) ("IOException (probably out of disk space) for " + filename)
              });
              ((ConfirmDialogScreen) GameScreenManager.Instance.StartScreen(((Component) ScreenPrefabs.Instance.ConfirmDialogScreen).gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, (GameScreenManager.UIRenderTarget) 2)).PopupConfirmDialog(string.Format((string) UI.CRASHSCREEN.SAVEFAILED, (object) "IOException. You may not have enough free space!"), (System.Action) null, (System.Action) null);
              return SaveLoader.GetActiveSaveFilePath();
            default:
              throw ex;
          }
        }
      }
    }
    if (updateSavePointer)
      SaveLoader.SetActiveSaveFilePath(filename);
    Game.Instance.timelapser.SaveColonyPreview(filename);
    DebugUtil.LogArgs(new object[2]
    {
      (object) "Saved to",
      (object) ("[" + filename + "]")
    });
    GC.Collect();
    return filename;
  }

  public static SaveGame.GameInfo LoadHeader(string filename, out SaveGame.Header header)
  {
    byte[] buffer = new byte[512];
    using (FileStream fileStream = File.OpenRead(filename))
    {
      fileStream.Read(buffer, 0, 512);
      return SaveGame.GetHeader((IReader) new FastReader(buffer), out header, filename);
    }
  }

  public bool Load(string filename)
  {
    SaveLoader.SetActiveSaveFilePath(filename);
    try
    {
      Manager.Clear();
      byte[] sourceArray = File.ReadAllBytes(filename);
      IReader ireader = (IReader) new FastReader(sourceArray);
      SaveGame.Header header;
      this.GameInfo = SaveGame.GetHeader(ireader, out header, filename);
      DebugUtil.LogArgs(new object[1]
      {
        (object) string.Format("Loading save file: {4}\n headerVersion:{0}, buildVersion:{1}, headerSize:{2}, IsCompressed:{3}", (object) header.headerVersion, (object) header.buildVersion, (object) header.headerSize, (object) header.IsCompressed, (object) filename)
      });
      DebugUtil.LogArgs(new object[1]
      {
        (object) string.Format("GameInfo loaded from save header:\n  numberOfCycles:{0},\n  numberOfDuplicants:{1},\n  baseName:{2},\n  isAutoSave:{3},\n  originalSaveName:{4},\n  clusterId:{5},\n  worldTraits:{6},\n  colonyGuid:{7},\n  saveVersion:{8}.{9}", (object) this.GameInfo.numberOfCycles, (object) this.GameInfo.numberOfDuplicants, (object) this.GameInfo.baseName, (object) this.GameInfo.isAutoSave, (object) this.GameInfo.originalSaveName, (object) this.GameInfo.clusterId, this.GameInfo.worldTraits == null || this.GameInfo.worldTraits.Length == 0 ? (object) "<i>none</i>" : (object) string.Join(", ", this.GameInfo.worldTraits), (object) this.GameInfo.colonyGuid, (object) this.GameInfo.saveMajorVersion, (object) this.GameInfo.saveMinorVersion)
      });
      string originalSaveName = this.GameInfo.originalSaveName;
      if ((originalSaveName.Contains("/") ? 1 : (originalSaveName.Contains("\\") ? 1 : 0)) != 0)
      {
        string originalSaveFileName = SaveLoader.GetOriginalSaveFileName(originalSaveName);
        this.GameInfo = this.GameInfo with
        {
          originalSaveName = originalSaveFileName
        };
        Debug.Log((object) ("Migration / Save originalSaveName updated from: `" + originalSaveName + "` => `" + this.GameInfo.originalSaveName + "`"));
      }
      if (this.GameInfo.saveMajorVersion == 7 && this.GameInfo.saveMinorVersion < 4)
        Helper.SetTypeInfoMask((SerializationTypeInfo) 191);
      Manager.DeserializeDirectory(ireader);
      if (header.IsCompressed)
      {
        int length = sourceArray.Length - ireader.Position;
        byte[] numArray1 = new byte[length];
        Array.Copy((Array) sourceArray, ireader.Position, (Array) numArray1, 0, length);
        byte[] numArray2 = SaveLoader.DecompressContents(numArray1);
        this.lastUncompressedSize = numArray2.Length;
        this.Load((IReader) new FastReader(numArray2));
      }
      else
      {
        this.lastUncompressedSize = sourceArray.Length;
        this.Load(ireader);
      }
      KCrashReporter.MOST_RECENT_SAVEFILE = filename;
      if (this.GameInfo.isAutoSave)
      {
        if (!string.IsNullOrEmpty(this.GameInfo.originalSaveName))
        {
          string originalSaveFileName = SaveLoader.GetOriginalSaveFileName(this.GameInfo.originalSaveName);
          string path;
          if (SaveLoader.IsSaveCloud(filename))
          {
            string cloudSavePrefix = SaveLoader.GetCloudSavePrefix();
            path = cloudSavePrefix == null ? System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filename).Replace("auto_save", ""), this.GameInfo.baseName, originalSaveFileName) : System.IO.Path.Combine(cloudSavePrefix, this.GameInfo.baseName, originalSaveFileName);
          }
          else
            path = System.IO.Path.Combine(SaveLoader.GetSavePrefix(), this.GameInfo.baseName, originalSaveFileName);
          if (path != null)
            SaveLoader.SetActiveSaveFilePath(path);
        }
      }
    }
    catch (Exception ex)
    {
      DebugUtil.LogWarningArgs(new object[1]
      {
        (object) ("\n--- Error loading save ---\n" + ex.Message + "\n" + ex.StackTrace)
      });
      Sim.Shutdown();
      SaveLoader.SetActiveSaveFilePath((string) null);
      return false;
    }
    Stats.Print();
    DebugUtil.LogArgs(new object[2]
    {
      (object) "Loaded",
      (object) ("[" + filename + "]")
    });
    DebugUtil.LogArgs(new object[2]
    {
      (object) "World Seeds",
      (object) ("[" + this.clusterDetailSave.globalWorldSeed.ToString() + "/" + this.clusterDetailSave.globalWorldLayoutSeed.ToString() + "/" + this.clusterDetailSave.globalTerrainSeed.ToString() + "/" + this.clusterDetailSave.globalNoiseSeed.ToString() + "]")
    });
    GC.Collect();
    return true;
  }

  public bool LoadFromWorldGen()
  {
    DebugUtil.LogArgs(new object[1]
    {
      (object) "Attempting to start a new game with current world gen"
    });
    WorldGen.LoadSettings();
    this.m_clusterLayout = Cluster.Load();
    ListPool<SimSaveFileStructure, SaveLoader>.PooledList loadedWorlds = ListPool<SimSaveFileStructure, SaveLoader>.Allocate();
    this.m_clusterLayout.LoadClusterLayoutSim((List<SimSaveFileStructure>) loadedWorlds);
    this.GameInfo = this.GameInfo with
    {
      clusterId = this.m_clusterLayout.Id,
      colonyGuid = Guid.NewGuid()
    };
    if (((List<SimSaveFileStructure>) loadedWorlds).Count != this.m_clusterLayout.worlds.Count)
    {
      Debug.LogError((object) "Attempt failed. Failed to load all worlds.");
      loadedWorlds.Recycle();
      return false;
    }
    GridSettings.Reset(this.m_clusterLayout.size.x, this.m_clusterLayout.size.y);
    if (Application.isPlaying)
      Singleton<KBatchedAnimUpdater>.Instance.InitializeGrid();
    this.clusterDetailSave = new WorldDetailSave();
    foreach (SimSaveFileStructure saveFileStructure in (List<SimSaveFileStructure>) loadedWorlds)
    {
      this.clusterDetailSave.globalNoiseSeed = saveFileStructure.worldDetail.globalNoiseSeed;
      this.clusterDetailSave.globalTerrainSeed = saveFileStructure.worldDetail.globalTerrainSeed;
      this.clusterDetailSave.globalWorldLayoutSeed = saveFileStructure.worldDetail.globalWorldLayoutSeed;
      this.clusterDetailSave.globalWorldSeed = saveFileStructure.worldDetail.globalWorldSeed;
      Vector2 vector2 = Vector2.op_Implicit(Grid.CellToPos2D(Grid.PosToCell(Vector2I.op_Implicit(new Vector2I(saveFileStructure.x, saveFileStructure.y)))));
      foreach (WorldDetailSave.OverworldCell overworldCell in saveFileStructure.worldDetail.overworldCells)
      {
        for (int index1 = 0; index1 != overworldCell.poly.Vertices.Count; ++index1)
        {
          List<Vector2> vertices = overworldCell.poly.Vertices;
          int index2 = index1;
          vertices[index2] = Vector2.op_Addition(vertices[index2], vector2);
        }
        overworldCell.poly.RefreshBounds();
      }
      this.clusterDetailSave.overworldCells.AddRange((IEnumerable<WorldDetailSave.OverworldCell>) saveFileStructure.worldDetail.overworldCells);
    }
    Sim.SIM_Initialize(new Sim.GAME_MessageHandler(Sim.DLL_MessageHandler));
    SimMessages.CreateSimElementsTable(ElementLoader.elements);
    Sim.AllocateCells(this.m_clusterLayout.size.x, this.m_clusterLayout.size.y);
    SimMessages.DefineWorldOffsets(this.m_clusterLayout.worlds.Select<WorldGen, SimMessages.WorldOffsetData>((Func<WorldGen, SimMessages.WorldOffsetData>) (world => new SimMessages.WorldOffsetData()
    {
      worldOffsetX = world.WorldOffset.x,
      worldOffsetY = world.WorldOffset.y,
      worldSizeX = world.WorldSize.x,
      worldSizeY = world.WorldSize.y
    })).ToList<SimMessages.WorldOffsetData>());
    SimMessages.CreateDiseaseTable(Db.Get().Diseases);
    Sim.HandleMessage(SimMessageHashes.ClearUnoccupiedCells, 0, (byte[]) null);
    try
    {
      foreach (SimSaveFileStructure saveFileStructure in (List<SimSaveFileStructure>) loadedWorlds)
      {
        if (Sim.Load((IReader) new FastReader(saveFileStructure.Sim)) != 0)
        {
          DebugUtil.LogWarningArgs(new object[1]
          {
            (object) "\n--- Error loading save ---\nSimDLL found bad data\n"
          });
          Sim.Shutdown();
          loadedWorlds.Recycle();
          return false;
        }
      }
    }
    catch (Exception ex)
    {
      Debug.LogWarning((object) ("--- Error loading Sim FROM NEW WORLDGEN ---" + ex.Message + "\n" + ex.StackTrace));
      Sim.Shutdown();
      loadedWorlds.Recycle();
      return false;
    }
    Debug.Log((object) "Attempt success");
    Sim.Start();
    SceneInitializer.Instance.PostLoadPrefabs();
    SceneInitializer.Instance.NewSaveGamePrefab();
    this.cachedGSD = this.m_clusterLayout.currentWorld.SpawnData;
    Util.Signal<Cluster>(this.OnWorldGenComplete, this.m_clusterLayout);
    OniMetrics.LogEvent(OniMetrics.Event.NewSave, "NewGame", (object) true);
    StoryManager.Instance.InitialSaveSetup();
    ThreadedHttps<KleiMetrics>.Instance.IncrementGameCount();
    OniMetrics.SendEvent(OniMetrics.Event.NewSave, "New Save");
    loadedWorlds.Recycle();
    return true;
  }

  public GameSpawnData cachedGSD { get; private set; }

  public WorldDetailSave clusterDetailSave { get; private set; }

  public void SetWorldDetail(WorldDetailSave worldDetail) => this.clusterDetailSave = worldDetail;

  private void ReportSaveMetrics(bool is_auto_save)
  {
    if (ThreadedHttps<KleiMetrics>.Instance == null || !ThreadedHttps<KleiMetrics>.Instance.enabled || Object.op_Equality((Object) this.saveManager, (Object) null))
      return;
    Dictionary<string, object> dictionary = new Dictionary<string, object>();
    dictionary[GameClock.NewCycleKey] = (object) (GameClock.Instance.GetCycle() + 1);
    dictionary["IsAutoSave"] = (object) is_auto_save;
    dictionary["SavedPrefabs"] = (object) this.GetSavedPrefabMetrics();
    dictionary["ResourcesAccessible"] = (object) this.GetWorldInventoryMetrics();
    dictionary["MinionMetrics"] = (object) this.GetMinionMetrics();
    if (is_auto_save)
    {
      dictionary["DailyReport"] = (object) this.GetDailyReportMetrics();
      dictionary["PerformanceMeasurements"] = (object) this.GetPerformanceMeasurements();
      dictionary["AverageFrameTime"] = (object) this.GetFrameTime();
    }
    dictionary["CustomGameSettings"] = (object) CustomGameSettings.Instance.GetSettingsForMetrics();
    ThreadedHttps<KleiMetrics>.Instance.SendEvent(dictionary, nameof (ReportSaveMetrics));
  }

  private List<SaveLoader.MinionMetricsData> GetMinionMetrics()
  {
    List<SaveLoader.MinionMetricsData> minionMetrics = new List<SaveLoader.MinionMetricsData>();
    foreach (MinionIdentity minionIdentity in Components.LiveMinionIdentities.Items)
    {
      if (!Object.op_Equality((Object) minionIdentity, (Object) null))
      {
        Amounts amounts = ((Component) minionIdentity).gameObject.GetComponent<Modifiers>().amounts;
        List<SaveLoader.MinionAttrFloatData> minionAttrFloatDataList = new List<SaveLoader.MinionAttrFloatData>(amounts.Count);
        foreach (AmountInstance amountInstance in (Modifications<Amount, AmountInstance>) amounts)
        {
          float f = amountInstance.value;
          if (!float.IsNaN(f) && !float.IsInfinity(f))
            minionAttrFloatDataList.Add(new SaveLoader.MinionAttrFloatData()
            {
              Name = amountInstance.modifier.Id,
              Value = amountInstance.value
            });
        }
        MinionResume component = ((Component) minionIdentity).gameObject.GetComponent<MinionResume>();
        float experienceGained = component.TotalExperienceGained;
        List<string> stringList = new List<string>();
        foreach (KeyValuePair<string, bool> keyValuePair in component.MasteryBySkillID)
        {
          if (keyValuePair.Value)
            stringList.Add(keyValuePair.Key);
        }
        minionMetrics.Add(new SaveLoader.MinionMetricsData()
        {
          Name = ((Object) minionIdentity).name,
          Modifiers = minionAttrFloatDataList,
          TotalExperienceGained = experienceGained,
          Skills = stringList
        });
      }
    }
    return minionMetrics;
  }

  private List<SaveLoader.SavedPrefabMetricsData> GetSavedPrefabMetrics()
  {
    Dictionary<Tag, List<SaveLoadRoot>> lists = this.saveManager.GetLists();
    List<SaveLoader.SavedPrefabMetricsData> savedPrefabMetrics = new List<SaveLoader.SavedPrefabMetricsData>(lists.Count);
    foreach (KeyValuePair<Tag, List<SaveLoadRoot>> keyValuePair in lists)
    {
      Tag key = keyValuePair.Key;
      List<SaveLoadRoot> saveLoadRootList = keyValuePair.Value;
      if (saveLoadRootList.Count > 0)
        savedPrefabMetrics.Add(new SaveLoader.SavedPrefabMetricsData()
        {
          PrefabName = key.ToString(),
          Count = saveLoadRootList.Count
        });
    }
    return savedPrefabMetrics;
  }

  private List<SaveLoader.WorldInventoryMetricsData> GetWorldInventoryMetrics()
  {
    Dictionary<Tag, float> accessibleAmounts = ClusterManager.Instance.GetAllWorldsAccessibleAmounts();
    List<SaveLoader.WorldInventoryMetricsData> inventoryMetrics = new List<SaveLoader.WorldInventoryMetricsData>(accessibleAmounts.Count);
    foreach (KeyValuePair<Tag, float> keyValuePair in accessibleAmounts)
    {
      float f = keyValuePair.Value;
      if (!float.IsInfinity(f) && !float.IsNaN(f))
        inventoryMetrics.Add(new SaveLoader.WorldInventoryMetricsData()
        {
          Name = keyValuePair.Key.ToString(),
          Amount = f
        });
    }
    return inventoryMetrics;
  }

  private List<SaveLoader.DailyReportMetricsData> GetDailyReportMetrics()
  {
    List<SaveLoader.DailyReportMetricsData> dailyReportMetrics = new List<SaveLoader.DailyReportMetricsData>();
    ReportManager.DailyReport report = ReportManager.Instance.FindReport(GameClock.Instance.GetCycle());
    if (report != null)
    {
      foreach (ReportManager.ReportEntry reportEntry in report.reportEntries)
      {
        SaveLoader.DailyReportMetricsData reportMetricsData = new SaveLoader.DailyReportMetricsData();
        reportMetricsData.Name = reportEntry.reportType.ToString();
        if (!float.IsInfinity(reportEntry.Net) && !float.IsNaN(reportEntry.Net))
          reportMetricsData.Net = new float?(reportEntry.Net);
        if (SaveLoader.force_infinity)
          reportMetricsData.Net = new float?();
        if (!float.IsInfinity(reportEntry.Positive) && !float.IsNaN(reportEntry.Positive))
          reportMetricsData.Positive = new float?(reportEntry.Positive);
        if (!float.IsInfinity(reportEntry.Negative) && !float.IsNaN(reportEntry.Negative))
          reportMetricsData.Negative = new float?(reportEntry.Negative);
        dailyReportMetrics.Add(reportMetricsData);
      }
      dailyReportMetrics.Add(new SaveLoader.DailyReportMetricsData()
      {
        Name = "MinionCount",
        Net = new float?((float) Components.LiveMinionIdentities.Count),
        Positive = new float?(0.0f),
        Negative = new float?(0.0f)
      });
    }
    return dailyReportMetrics;
  }

  private List<SaveLoader.PerformanceMeasurement> GetPerformanceMeasurements()
  {
    List<SaveLoader.PerformanceMeasurement> performanceMeasurements = new List<SaveLoader.PerformanceMeasurement>();
    if (Object.op_Inequality((Object) Global.Instance, (Object) null))
    {
      PerformanceMonitor component = ((Component) Global.Instance).GetComponent<PerformanceMonitor>();
      List<SaveLoader.PerformanceMeasurement> performanceMeasurementList1 = performanceMeasurements;
      SaveLoader.PerformanceMeasurement performanceMeasurement1 = new SaveLoader.PerformanceMeasurement();
      performanceMeasurement1.name = "FramesAbove30";
      performanceMeasurement1.value = (float) component.NumFramesAbove30;
      SaveLoader.PerformanceMeasurement performanceMeasurement2 = performanceMeasurement1;
      performanceMeasurementList1.Add(performanceMeasurement2);
      List<SaveLoader.PerformanceMeasurement> performanceMeasurementList2 = performanceMeasurements;
      performanceMeasurement1 = new SaveLoader.PerformanceMeasurement();
      performanceMeasurement1.name = "FramesBelow30";
      performanceMeasurement1.value = (float) component.NumFramesBelow30;
      SaveLoader.PerformanceMeasurement performanceMeasurement3 = performanceMeasurement1;
      performanceMeasurementList2.Add(performanceMeasurement3);
      component.Reset();
    }
    return performanceMeasurements;
  }

  private float GetFrameTime()
  {
    PerformanceMonitor component = ((Component) Global.Instance).GetComponent<PerformanceMonitor>();
    DebugUtil.LogArgs(new object[2]
    {
      (object) "Average frame time:",
      (object) (float) (1.0 / (double) component.FPS)
    });
    return 1f / component.FPS;
  }

  public class FlowUtilityNetworkInstance
  {
    public int id = -1;
    public SimHashes containedElement = SimHashes.Vacuum;
    public float containedMass;
    public float containedTemperature;
  }

  [SerializationConfig]
  public class FlowUtilityNetworkSaver : ISaveLoadable
  {
    public List<SaveLoader.FlowUtilityNetworkInstance> gas;
    public List<SaveLoader.FlowUtilityNetworkInstance> liquid;

    public FlowUtilityNetworkSaver()
    {
      this.gas = new List<SaveLoader.FlowUtilityNetworkInstance>();
      this.liquid = new List<SaveLoader.FlowUtilityNetworkInstance>();
    }
  }

  public struct SaveFileEntry
  {
    public string path;
    public System.DateTime timeStamp;
  }

  public enum SaveType
  {
    local,
    cloud,
    both,
  }

  private struct MinionAttrFloatData
  {
    public string Name;
    public float Value;
  }

  private struct MinionMetricsData
  {
    public string Name;
    public List<SaveLoader.MinionAttrFloatData> Modifiers;
    public float TotalExperienceGained;
    public List<string> Skills;
  }

  private struct SavedPrefabMetricsData
  {
    public string PrefabName;
    public int Count;
  }

  private struct WorldInventoryMetricsData
  {
    public string Name;
    public float Amount;
  }

  private struct DailyReportMetricsData
  {
    public string Name;
    [JsonProperty]
    public float? Net;
    [JsonProperty]
    public float? Positive;
    [JsonProperty]
    public float? Negative;
  }

  private struct PerformanceMeasurement
  {
    public string name;
    public float value;
  }
}
