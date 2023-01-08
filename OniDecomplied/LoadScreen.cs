// Decompiled with JetBrains decompiler
// Type: LoadScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ProcGen;
using ProcGenGame;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadScreen : KModalScreen
{
  private const int MAX_CLOUD_TUTORIALS = 5;
  private const string CLOUD_TUTORIAL_KEY = "LoadScreenCloudTutorialTimes";
  [SerializeField]
  private KButton closeButton;
  [SerializeField]
  private GameObject saveButtonRoot;
  [SerializeField]
  private GameObject colonyListRoot;
  [SerializeField]
  private GameObject colonyViewRoot;
  [SerializeField]
  private HierarchyReferences migrationPanelRefs;
  [SerializeField]
  private HierarchyReferences saveButtonPrefab;
  [Space]
  [SerializeField]
  private KButton colonyCloudButton;
  [SerializeField]
  private KButton colonyLocalButton;
  [SerializeField]
  private KButton colonyInfoButton;
  [SerializeField]
  private Sprite localToCloudSprite;
  [SerializeField]
  private Sprite cloudToLocalSprite;
  [SerializeField]
  private Sprite errorSprite;
  [SerializeField]
  private Sprite infoSprite;
  [SerializeField]
  private Bouncer cloudTutorialBouncer;
  public bool requireConfirmation = true;
  private LoadScreen.SelectedSave selectedSave;
  private List<LoadScreen.SaveGameFileDetails> currentColony;
  private UIPool<HierarchyReferences> colonyListPool;
  private ConfirmDialogScreen confirmScreen;
  private InfoDialogScreen infoScreen;
  private InfoDialogScreen errorInfoScreen;
  private ConfirmDialogScreen errorScreen;
  private InspectSaveScreen inspectScreenInstance;

  public static LoadScreen Instance { get; private set; }

  public static void DestroyInstance() => LoadScreen.Instance = (LoadScreen) null;

  protected override void OnPrefabInit()
  {
    Debug.Assert(Object.op_Equality((Object) LoadScreen.Instance, (Object) null));
    LoadScreen.Instance = this;
    base.OnPrefabInit();
    this.colonyListPool = new UIPool<HierarchyReferences>(this.saveButtonPrefab);
    if (Object.op_Inequality((Object) SpeedControlScreen.Instance, (Object) null))
      SpeedControlScreen.Instance.Pause(false);
    if (Object.op_Inequality((Object) this.closeButton, (Object) null))
      this.closeButton.onClick += (System.Action) (() => this.Deactivate());
    if (Object.op_Inequality((Object) this.colonyCloudButton, (Object) null))
      this.colonyCloudButton.onClick += (System.Action) (() => this.ConvertAllToCloud());
    if (Object.op_Inequality((Object) this.colonyLocalButton, (Object) null))
      this.colonyLocalButton.onClick += (System.Action) (() => this.ConvertAllToLocal());
    if (!Object.op_Inequality((Object) this.colonyInfoButton, (Object) null))
      return;
    this.colonyInfoButton.onClick += (System.Action) (() => this.ShowSaveInfo());
  }

  private bool IsInMenu() => App.GetCurrentSceneName() == "frontend";

  private bool CloudSavesVisible() => SaveLoader.GetCloudSavesAvailable() && this.IsInMenu();

  protected override void OnActivate()
  {
    base.OnActivate();
    WorldGen.LoadSettings();
    this.SetCloudSaveInfoActive(this.CloudSavesVisible());
    this.RefreshColonyList();
    this.ShowColonyList();
    bool cloudSavesAvailable = SaveLoader.GetCloudSavesAvailable();
    ((Component) this.cloudTutorialBouncer).gameObject.SetActive(cloudSavesAvailable);
    if (cloudSavesAvailable && !this.cloudTutorialBouncer.IsBouncing())
    {
      int num = KPlayerPrefs.GetInt("LoadScreenCloudTutorialTimes", 0);
      if (num < 5)
      {
        this.cloudTutorialBouncer.Bounce();
        KPlayerPrefs.SetInt("LoadScreenCloudTutorialTimes", num + 1);
        KPlayerPrefs.GetInt("LoadScreenCloudTutorialTimes", 0);
      }
      else
        ((Component) this.cloudTutorialBouncer).gameObject.SetActive(false);
    }
    if (!DistributionPlatform.Initialized || !SteamUtils.IsSteamRunningOnSteamDeck())
      return;
    ((Component) this.colonyInfoButton).gameObject.SetActive(false);
  }

  private Dictionary<string, List<LoadScreen.SaveGameFileDetails>> GetColoniesDetails(
    List<SaveLoader.SaveFileEntry> files)
  {
    Dictionary<string, List<LoadScreen.SaveGameFileDetails>> coloniesDetails = new Dictionary<string, List<LoadScreen.SaveGameFileDetails>>();
    if (files.Count <= 0)
      return coloniesDetails;
    for (int index = 0; index < files.Count; ++index)
    {
      if (this.IsFileValid(files[index].path))
      {
        Tuple<SaveGame.Header, SaveGame.GameInfo> fileInfo = SaveGame.GetFileInfo(files[index].path);
        SaveGame.Header first = fileInfo.first;
        SaveGame.GameInfo second = fileInfo.second;
        System.DateTime timeStamp = files[index].timeStamp;
        long num = 0;
        try
        {
          num = new FileInfo(files[index].path).Length;
        }
        catch (Exception ex)
        {
          Debug.LogWarning((object) ("Failed to get size for file: " + files[index].ToString() + "\n" + ex.ToString()));
        }
        LoadScreen.SaveGameFileDetails saveGameFileDetails = new LoadScreen.SaveGameFileDetails();
        saveGameFileDetails.BaseName = second.baseName;
        saveGameFileDetails.FileName = files[index].path;
        saveGameFileDetails.FileDate = timeStamp;
        saveGameFileDetails.FileHeader = first;
        saveGameFileDetails.FileInfo = second;
        saveGameFileDetails.Size = num;
        saveGameFileDetails.UniqueID = SaveGame.GetSaveUniqueID(second);
        if (!coloniesDetails.ContainsKey(saveGameFileDetails.UniqueID))
          coloniesDetails.Add(saveGameFileDetails.UniqueID, new List<LoadScreen.SaveGameFileDetails>());
        coloniesDetails[saveGameFileDetails.UniqueID].Add(saveGameFileDetails);
      }
    }
    return coloniesDetails;
  }

  private Dictionary<string, List<LoadScreen.SaveGameFileDetails>> GetColonies(bool sort) => this.GetColoniesDetails(SaveLoader.GetAllFiles(sort));

  private Dictionary<string, List<LoadScreen.SaveGameFileDetails>> GetLocalColonies(bool sort) => this.GetColoniesDetails(SaveLoader.GetAllFiles(sort, SaveLoader.SaveType.local));

  private Dictionary<string, List<LoadScreen.SaveGameFileDetails>> GetCloudColonies(bool sort) => this.GetColoniesDetails(SaveLoader.GetAllFiles(sort, SaveLoader.SaveType.cloud));

  private bool IsFileValid(string filename)
  {
    bool flag = false;
    try
    {
      flag = SaveLoader.LoadHeader(filename, out SaveGame.Header _).saveMajorVersion >= 7;
    }
    catch (Exception ex)
    {
      Debug.LogWarning((object) ("Corrupted save file: " + filename + "\n" + ex.ToString()));
    }
    return flag;
  }

  private void CheckCloudLocalOverlap()
  {
    if (!SaveLoader.GetCloudSavesAvailable())
      return;
    string cloudSavePrefix = SaveLoader.GetCloudSavePrefix();
    if (cloudSavePrefix == null)
      return;
    foreach (KeyValuePair<string, List<LoadScreen.SaveGameFileDetails>> colony in this.GetColonies(false))
    {
      bool flag = false;
      List<LoadScreen.SaveGameFileDetails> saveGameFileDetailsList = new List<LoadScreen.SaveGameFileDetails>();
      foreach (LoadScreen.SaveGameFileDetails saveGameFileDetails in colony.Value)
      {
        if (SaveLoader.IsSaveCloud(saveGameFileDetails.FileName))
          flag = true;
        else
          saveGameFileDetailsList.Add(saveGameFileDetails);
      }
      if (flag && saveGameFileDetailsList.Count != 0)
      {
        string baseName = saveGameFileDetailsList[0].BaseName;
        string path1 = System.IO.Path.Combine(SaveLoader.GetSavePrefix(), baseName);
        string path2 = System.IO.Path.Combine(cloudSavePrefix, baseName);
        if (!Directory.Exists(path2))
          Directory.CreateDirectory(path2);
        Debug.Log((object) ("Saves / Found overlapped cloud/local saves for colony '" + baseName + "', moving to cloud..."));
        foreach (LoadScreen.SaveGameFileDetails saveGameFileDetails in saveGameFileDetailsList)
        {
          string fileName = saveGameFileDetails.FileName;
          string source = System.IO.Path.ChangeExtension(fileName, "png");
          string path1_1 = path2;
          if (SaveLoader.IsSaveAuto(fileName))
          {
            string path3 = System.IO.Path.Combine(path1_1, "auto_save");
            if (!Directory.Exists(path3))
              Directory.CreateDirectory(path3);
            path1_1 = path3;
          }
          string str = System.IO.Path.Combine(path1_1, System.IO.Path.GetFileName(fileName));
          if (this.FileMatch(fileName, str, out Tuple<bool, bool> _))
          {
            Debug.Log((object) ("Saves / file match found for `" + fileName + "`..."));
            this.MigrateFile(fileName, str);
            string dest = System.IO.Path.ChangeExtension(str, "png");
            this.MigrateFile(source, dest, true);
          }
          else
          {
            Debug.Log((object) ("Saves / no file match found for `" + fileName + "`... move as copy"));
            string nextUsableSavePath = SaveLoader.GetNextUsableSavePath(str);
            this.MigrateFile(fileName, nextUsableSavePath);
            string dest = System.IO.Path.ChangeExtension(nextUsableSavePath, "png");
            this.MigrateFile(source, dest, true);
          }
        }
        this.RemoveEmptyFolder(path1);
      }
    }
  }

  private void DeleteFileAndEmptyFolder(string file)
  {
    if (File.Exists(file))
      File.Delete(file);
    this.RemoveEmptyFolder(System.IO.Path.GetDirectoryName(file));
  }

  private void RemoveEmptyFolder(string path)
  {
    if (!Directory.Exists(path) || !File.GetAttributes(path).HasFlag((Enum) FileAttributes.Directory))
      return;
    if (Directory.EnumerateFileSystemEntries(path).Any<string>())
      return;
    try
    {
      Directory.Delete(path);
    }
    catch (Exception ex)
    {
      Debug.LogWarning((object) ("Failed to remove empty directory `" + path + "`..."));
      Debug.LogWarning((object) ex);
    }
  }

  private void RefreshColonyList()
  {
    if (this.colonyListPool != null)
      this.colonyListPool.ClearAll();
    this.CheckCloudLocalOverlap();
    Dictionary<string, List<LoadScreen.SaveGameFileDetails>> colonies = this.GetColonies(true);
    if (colonies.Count <= 0)
      return;
    foreach (KeyValuePair<string, List<LoadScreen.SaveGameFileDetails>> keyValuePair in colonies)
      this.AddColonyToList(keyValuePair.Value);
  }

  private string GetFileHash(string path)
  {
    using (MD5 md5 = MD5.Create())
    {
      using (FileStream inputStream = File.OpenRead(path))
        return BitConverter.ToString(md5.ComputeHash((Stream) inputStream)).Replace("-", "").ToLowerInvariant();
    }
  }

  private bool FileMatch(string file, string other_file, out Tuple<bool, bool> matches)
  {
    matches = new Tuple<bool, bool>(false, false);
    if (!File.Exists(file) || !File.Exists(other_file))
      return false;
    bool flag1;
    bool flag2;
    try
    {
      string fileHash1 = this.GetFileHash(file);
      string fileHash2 = this.GetFileHash(other_file);
      flag1 = new FileInfo(file).Length == new FileInfo(other_file).Length;
      string str = fileHash2;
      flag2 = fileHash1 == str;
    }
    catch (Exception ex)
    {
      Debug.LogWarning((object) ("FileMatch / file match failed for `" + file + "` vs `" + other_file + "`!"));
      Debug.LogWarning((object) ex);
      return false;
    }
    matches.first = flag1;
    matches.second = flag2;
    return flag1 & flag2;
  }

  private bool MigrateFile(string source, string dest, bool ignoreMissing = false)
  {
    Debug.Log((object) ("Migration / moving `" + source + "` to `" + dest + "` ..."));
    if (dest == source)
    {
      Debug.Log((object) ("Migration / ignored `" + source + "` to `" + dest + "` ... same location"));
      return true;
    }
    if (this.FileMatch(source, dest, out Tuple<bool, bool> _))
    {
      Debug.Log((object) "Migration / dest and source are identical size + hash ... removing original");
      try
      {
        this.DeleteFileAndEmptyFolder(source);
      }
      catch (Exception ex)
      {
        Debug.LogWarning((object) ("Migration / removing original failed for `" + source + "`!"));
        Debug.LogWarning((object) ex);
        throw ex;
      }
      return true;
    }
    try
    {
      Debug.Log((object) "Migration / copying...");
      File.Copy(source, dest, false);
    }
    catch (FileNotFoundException ex) when (ignoreMissing)
    {
      Debug.Log((object) ("Migration / File `" + source + "` wasn't found but we're ignoring that."));
      return true;
    }
    catch (Exception ex)
    {
      Debug.LogWarning((object) ("Migration / copy failed for `" + source + "`! Leaving it alone"));
      Debug.LogWarning((object) ex);
      Debug.LogWarning((object) ("failed to convert colony: " + ex.ToString()));
      throw ex;
    }
    Debug.Log((object) "Migration / copy ok ...");
    Tuple<bool, bool> matches;
    if (!this.FileMatch(source, dest, out matches))
    {
      Debug.LogWarning((object) ("Migration / failed to match dest file for `" + source + "`!"));
      Debug.LogWarning((object) string.Format("Migration / did hash match? {0} did size match? {1}", (object) matches.second, (object) matches.first));
      throw new Exception("Hash/Size didn't match for source and destination");
    }
    Debug.Log((object) "Migration / hash validation ok ... removing original");
    try
    {
      this.DeleteFileAndEmptyFolder(source);
    }
    catch (Exception ex)
    {
      Debug.LogWarning((object) ("Migration / removing original failed for `" + source + "`!"));
      Debug.LogWarning((object) ex);
      throw ex;
    }
    Debug.Log((object) ("Migration / moved ok for `" + source + "`!"));
    return true;
  }

  private bool MigrateSave(string dest_root, string file, bool is_auto_save, out string saveError)
  {
    saveError = (string) null;
    Tuple<SaveGame.Header, SaveGame.GameInfo> fileInfo = SaveGame.GetFileInfo(file);
    SaveGame.Header first = fileInfo.first;
    string path2 = fileInfo.second.baseName.TrimEnd(' ');
    string fileName = System.IO.Path.GetFileName(file);
    string str1 = System.IO.Path.Combine(dest_root, path2);
    if (!Directory.Exists(str1))
      str1 = Directory.CreateDirectory(str1).FullName;
    string path1 = str1;
    if (is_auto_save)
    {
      string path = System.IO.Path.Combine(str1, "auto_save");
      if (!Directory.Exists(path))
        Directory.CreateDirectory(path);
      path1 = path;
    }
    string str2 = System.IO.Path.Combine(path1, fileName);
    string source = System.IO.Path.ChangeExtension(file, "png");
    string dest = System.IO.Path.ChangeExtension(str2, "png");
    try
    {
      this.MigrateFile(file, str2);
      this.MigrateFile(source, dest, true);
    }
    catch (Exception ex)
    {
      saveError = ex.Message;
      return false;
    }
    return true;
  }

  private (int, int, ulong) GetSavesSizeAndCounts(List<LoadScreen.SaveGameFileDetails> list)
  {
    ulong num1 = 0;
    int num2 = 0;
    int num3 = 0;
    for (int index = 0; index < list.Count; ++index)
    {
      LoadScreen.SaveGameFileDetails saveGameFileDetails = list[index];
      num1 += (ulong) saveGameFileDetails.Size;
      if (saveGameFileDetails.FileInfo.isAutoSave)
        ++num3;
      else
        ++num2;
    }
    return (num2, num3, num1);
  }

  private int CountValidSaves(string path, SearchOption searchType = SearchOption.AllDirectories)
  {
    int num = 0;
    List<SaveLoader.SaveFileEntry> saveFiles = SaveLoader.GetSaveFiles(path, false, searchType);
    for (int index = 0; index < saveFiles.Count; ++index)
    {
      if (this.IsFileValid(saveFiles[index].path))
        ++num;
    }
    return num;
  }

  private (int, int) GetMigrationSaveCounts() => (this.CountValidSaves(SaveLoader.GetSavePrefixAndCreateFolder(), SearchOption.TopDirectoryOnly), this.CountValidSaves(SaveLoader.GetAutoSavePrefix()));

  private (int, int) MigrateSaves(out string errorColony, out string errorMessage)
  {
    errorColony = (string) null;
    errorMessage = (string) null;
    int num1 = 0;
    string prefixAndCreateFolder = SaveLoader.GetSavePrefixAndCreateFolder();
    List<SaveLoader.SaveFileEntry> saveFiles1 = SaveLoader.GetSaveFiles(prefixAndCreateFolder, false, SearchOption.TopDirectoryOnly);
    for (int index = 0; index < saveFiles1.Count; ++index)
    {
      SaveLoader.SaveFileEntry saveFileEntry = saveFiles1[index];
      if (this.IsFileValid(saveFileEntry.path))
      {
        string saveError;
        if (this.MigrateSave(prefixAndCreateFolder, saveFileEntry.path, false, out saveError))
          ++num1;
        else if (errorColony == null)
        {
          errorColony = saveFileEntry.path;
          errorMessage = saveError;
        }
      }
    }
    int num2 = 0;
    List<SaveLoader.SaveFileEntry> saveFiles2 = SaveLoader.GetSaveFiles(SaveLoader.GetAutoSavePrefix(), false);
    for (int index = 0; index < saveFiles2.Count; ++index)
    {
      SaveLoader.SaveFileEntry saveFileEntry = saveFiles2[index];
      if (this.IsFileValid(saveFileEntry.path))
      {
        string saveError;
        if (this.MigrateSave(prefixAndCreateFolder, saveFileEntry.path, true, out saveError))
          ++num2;
        else if (errorColony == null)
        {
          errorColony = saveFileEntry.path;
          errorMessage = saveError;
        }
      }
    }
    return (num1, num2);
  }

  public void ShowMigrationIfNecessary(bool fromMainMenu)
  {
    (int num1, int num2) = this.GetMigrationSaveCounts();
    if (num1 == 0 && num2 == 0)
    {
      if (!fromMainMenu)
        return;
      this.Deactivate();
    }
    else
    {
      this.Activate();
      ((Component) this.migrationPanelRefs).gameObject.SetActive(true);
      KButton migrateButton = ((Component) this.migrationPanelRefs.GetReference<RectTransform>("MigrateSaves")).GetComponent<KButton>();
      KButton continueButton = ((Component) this.migrationPanelRefs.GetReference<RectTransform>("Continue")).GetComponent<KButton>();
      KButton moreInfoButton = ((Component) this.migrationPanelRefs.GetReference<RectTransform>("MoreInfo")).GetComponent<KButton>();
      KButton component = ((Component) this.migrationPanelRefs.GetReference<RectTransform>("OpenSaves")).GetComponent<KButton>();
      LocText statsText = ((Component) this.migrationPanelRefs.GetReference<RectTransform>("CountText")).GetComponent<LocText>();
      LocText infoText = ((Component) this.migrationPanelRefs.GetReference<RectTransform>("InfoText")).GetComponent<LocText>();
      ((Component) migrateButton).gameObject.SetActive(true);
      ((Component) continueButton).gameObject.SetActive(false);
      ((Component) moreInfoButton).gameObject.SetActive(false);
      ((TMP_Text) statsText).text = string.Format((string) STRINGS.UI.FRONTEND.LOADSCREEN.MIGRATE_COUNT, (object) num1, (object) num2);
      component.ClearOnClick();
      component.onClick += (System.Action) (() => App.OpenWebURL(SaveLoader.GetSavePrefixAndCreateFolder()));
      migrateButton.ClearOnClick();
      migrateButton.onClick += (System.Action) (() =>
      {
        ((Component) migrateButton).gameObject.SetActive(false);
        string errorColony;
        string errorMessage;
        (int num5, int num6) = this.MigrateSaves(out errorColony, out errorMessage);
        bool flag = errorColony == null;
        string format = flag ? STRINGS.UI.FRONTEND.LOADSCREEN.MIGRATE_RESULT.text : STRINGS.UI.FRONTEND.LOADSCREEN.MIGRATE_RESULT_FAILURES.Replace("{ErrorColony}", errorColony).Replace("{ErrorMessage}", errorMessage);
        ((TMP_Text) statsText).text = string.Format(format, (object) num5, (object) num1, (object) num6, (object) num2);
        ((Component) infoText).gameObject.SetActive(false);
        if (flag)
          ((Component) continueButton).gameObject.SetActive(true);
        else
          ((Component) moreInfoButton).gameObject.SetActive(true);
        MainMenu.Instance.RefreshResumeButton();
        this.RefreshColonyList();
      });
      continueButton.ClearOnClick();
      continueButton.onClick += (System.Action) (() =>
      {
        ((Component) this.migrationPanelRefs).gameObject.SetActive(false);
        this.cloudTutorialBouncer.Bounce();
      });
      moreInfoButton.ClearOnClick();
      moreInfoButton.onClick += (System.Action) (() =>
      {
        if (DistributionPlatform.Initialized && SteamUtils.IsSteamRunningOnSteamDeck())
          Util.KInstantiateUI<InfoDialogScreen>(((Component) ScreenPrefabs.Instance.InfoDialogScreen).gameObject, ((Component) this).gameObject, false).SetHeader((string) STRINGS.UI.FRONTEND.LOADSCREEN.MIGRATE_RESULT_FAILURES_MORE_INFO_TITLE).AddPlainText((string) STRINGS.UI.FRONTEND.LOADSCREEN.MIGRATE_RESULT_FAILURES_MORE_INFO_PRE).AddLineItem((string) STRINGS.UI.FRONTEND.LOADSCREEN.MIGRATE_RESULT_FAILURES_MORE_INFO_ITEM1, "").AddLineItem((string) STRINGS.UI.FRONTEND.LOADSCREEN.MIGRATE_RESULT_FAILURES_MORE_INFO_ITEM2, "").AddLineItem((string) STRINGS.UI.FRONTEND.LOADSCREEN.MIGRATE_RESULT_FAILURES_MORE_INFO_ITEM3, "").AddPlainText((string) STRINGS.UI.FRONTEND.LOADSCREEN.MIGRATE_RESULT_FAILURES_MORE_INFO_POST).AddOption((string) STRINGS.UI.CONFIRMDIALOG.OK, (Action<InfoDialogScreen>) (d =>
          {
            ((Component) this.migrationPanelRefs).gameObject.SetActive(false);
            this.cloudTutorialBouncer.Bounce();
            d.Deactivate();
          }), true).Activate();
        else
          Util.KInstantiateUI<InfoDialogScreen>(((Component) ScreenPrefabs.Instance.InfoDialogScreen).gameObject, ((Component) this).gameObject, false).SetHeader((string) STRINGS.UI.FRONTEND.LOADSCREEN.MIGRATE_RESULT_FAILURES_MORE_INFO_TITLE).AddPlainText((string) STRINGS.UI.FRONTEND.LOADSCREEN.MIGRATE_RESULT_FAILURES_MORE_INFO_PRE).AddLineItem((string) STRINGS.UI.FRONTEND.LOADSCREEN.MIGRATE_RESULT_FAILURES_MORE_INFO_ITEM1, "").AddLineItem((string) STRINGS.UI.FRONTEND.LOADSCREEN.MIGRATE_RESULT_FAILURES_MORE_INFO_ITEM2, "").AddLineItem((string) STRINGS.UI.FRONTEND.LOADSCREEN.MIGRATE_RESULT_FAILURES_MORE_INFO_ITEM3, "").AddPlainText((string) STRINGS.UI.FRONTEND.LOADSCREEN.MIGRATE_RESULT_FAILURES_MORE_INFO_POST).AddOption((string) STRINGS.UI.FRONTEND.LOADSCREEN.MIGRATE_FAILURES_FORUM_BUTTON, (Action<InfoDialogScreen>) (d => App.OpenWebURL("https://forums.kleientertainment.com/klei-bug-tracker/oni/"))).AddOption((string) STRINGS.UI.CONFIRMDIALOG.OK, (Action<InfoDialogScreen>) (d =>
          {
            ((Component) this.migrationPanelRefs).gameObject.SetActive(false);
            this.cloudTutorialBouncer.Bounce();
            d.Deactivate();
          }), true).Activate();
      });
    }
  }

  private void SetCloudSaveInfoActive(bool active)
  {
    ((Component) this.colonyCloudButton).gameObject.SetActive(active);
    ((Component) this.colonyLocalButton).gameObject.SetActive(active);
  }

  private bool ConvertToLocalOrCloud(string fromRoot, string destRoot, string colonyName)
  {
    string sourceDirName = System.IO.Path.Combine(fromRoot, colonyName);
    string destDirName = System.IO.Path.Combine(destRoot, colonyName);
    Debug.Log((object) ("Convert / Colony '" + colonyName + "' from `" + sourceDirName + "` => `" + destDirName + "`"));
    try
    {
      Directory.Move(sourceDirName, destDirName);
      return true;
    }
    catch (Exception ex)
    {
      Debug.LogWarning((object) ("failed to convert colony: " + ex.ToString()));
      this.ShowConvertError(STRINGS.UI.FRONTEND.LOADSCREEN.CONVERT_ERROR.Replace("{Colony}", colonyName).Replace("{Error}", ex.Message));
    }
    return false;
  }

  private bool ConvertColonyToCloud(string colonyName)
  {
    string savePrefix = SaveLoader.GetSavePrefix();
    string cloudSavePrefix = SaveLoader.GetCloudSavePrefix();
    if (cloudSavePrefix != null)
      return this.ConvertToLocalOrCloud(savePrefix, cloudSavePrefix, colonyName);
    Debug.LogWarning((object) "Failed to move colony to cloud, no cloud save prefix found (usually a userID is missing, not logged in?)");
    return false;
  }

  private bool ConvertColonyToLocal(string colonyName)
  {
    string savePrefix = SaveLoader.GetSavePrefix();
    string cloudSavePrefix = SaveLoader.GetCloudSavePrefix();
    if (cloudSavePrefix != null)
      return this.ConvertToLocalOrCloud(cloudSavePrefix, savePrefix, colonyName);
    Debug.LogWarning((object) "Failed to move colony from cloud, no cloud save prefix found (usually a userID is missing, not logged in?)");
    return false;
  }

  private void DoConvertAllToLocal()
  {
    Dictionary<string, List<LoadScreen.SaveGameFileDetails>> cloudColonies = this.GetCloudColonies(false);
    if (cloudColonies.Count == 0)
      return;
    bool flag = true;
    foreach (KeyValuePair<string, List<LoadScreen.SaveGameFileDetails>> keyValuePair in cloudColonies)
      flag &= this.ConvertColonyToLocal(keyValuePair.Value[0].BaseName);
    if (flag)
    {
      string steam = (string) STRINGS.UI.PLATFORMS.STEAM;
      this.ShowSimpleDialog((string) STRINGS.UI.FRONTEND.LOADSCREEN.CONVERT_TO_LOCAL, STRINGS.UI.FRONTEND.LOADSCREEN.CONVERT_ALL_TO_LOCAL_SUCCESS.Replace("{Client}", steam));
    }
    this.RefreshColonyList();
    MainMenu.Instance.RefreshResumeButton();
    SaveLoader.SetCloudSavesDefault(false);
  }

  private void DoConvertAllToCloud()
  {
    Dictionary<string, List<LoadScreen.SaveGameFileDetails>> localColonies = this.GetLocalColonies(false);
    if (localColonies.Count == 0)
      return;
    List<string> stringList = new List<string>();
    foreach (KeyValuePair<string, List<LoadScreen.SaveGameFileDetails>> keyValuePair in localColonies)
    {
      string baseName = keyValuePair.Value[0].BaseName;
      if (!stringList.Contains(baseName))
        stringList.Add(baseName);
    }
    bool flag = true;
    foreach (string colonyName in stringList)
      flag &= this.ConvertColonyToCloud(colonyName);
    if (flag)
    {
      string steam = (string) STRINGS.UI.PLATFORMS.STEAM;
      this.ShowSimpleDialog((string) STRINGS.UI.FRONTEND.LOADSCREEN.CONVERT_TO_CLOUD, STRINGS.UI.FRONTEND.LOADSCREEN.CONVERT_ALL_TO_CLOUD_SUCCESS.Replace("{Client}", steam));
    }
    this.RefreshColonyList();
    MainMenu.Instance.RefreshResumeButton();
    SaveLoader.SetCloudSavesDefault(true);
  }

  private void ConvertAllToCloud()
  {
    string message = string.Format("{0}\n{1}\n", (object) STRINGS.UI.FRONTEND.LOADSCREEN.CONVERT_TO_CLOUD_DETAILS, (object) STRINGS.UI.FRONTEND.LOADSCREEN.CONVERT_ALL_WARNING);
    KPlayerPrefs.SetInt("LoadScreenCloudTutorialTimes", 5);
    this.ConfirmCloudSaveMigrations(message, (string) STRINGS.UI.FRONTEND.LOADSCREEN.CONVERT_TO_CLOUD, (string) STRINGS.UI.FRONTEND.LOADSCREEN.CONVERT_ALL_COLONIES, (string) STRINGS.UI.FRONTEND.LOADSCREEN.OPEN_SAVE_FOLDER, (System.Action) (() => this.DoConvertAllToCloud()), (System.Action) (() => App.OpenWebURL(SaveLoader.GetSavePrefix())), this.localToCloudSprite);
  }

  private void ConvertAllToLocal()
  {
    string message = string.Format("{0}\n{1}\n", (object) STRINGS.UI.FRONTEND.LOADSCREEN.CONVERT_TO_LOCAL_DETAILS, (object) STRINGS.UI.FRONTEND.LOADSCREEN.CONVERT_ALL_WARNING);
    KPlayerPrefs.SetInt("LoadScreenCloudTutorialTimes", 5);
    this.ConfirmCloudSaveMigrations(message, (string) STRINGS.UI.FRONTEND.LOADSCREEN.CONVERT_TO_LOCAL, (string) STRINGS.UI.FRONTEND.LOADSCREEN.CONVERT_ALL_COLONIES, (string) STRINGS.UI.FRONTEND.LOADSCREEN.OPEN_SAVE_FOLDER, (System.Action) (() => this.DoConvertAllToLocal()), (System.Action) (() => App.OpenWebURL(SaveLoader.GetCloudSavePrefix())), this.cloudToLocalSprite);
  }

  private void ShowSaveInfo()
  {
    if (!Object.op_Equality((Object) this.infoScreen, (Object) null))
      return;
    this.infoScreen = Util.KInstantiateUI<InfoDialogScreen>(((Component) ScreenPrefabs.Instance.InfoDialogScreen).gameObject, ((Component) this).gameObject, false).SetHeader((string) STRINGS.UI.FRONTEND.LOADSCREEN.SAVE_INFO_DIALOG_TITLE).AddSprite(this.infoSprite).AddPlainText((string) STRINGS.UI.FRONTEND.LOADSCREEN.SAVE_INFO_DIALOG_TEXT).AddOption((string) STRINGS.UI.FRONTEND.LOADSCREEN.OPEN_SAVE_FOLDER, (Action<InfoDialogScreen>) (d => App.OpenWebURL(SaveLoader.GetSavePrefix())), true).AddDefaultCancel();
    string cloudRoot = SaveLoader.GetCloudSavePrefix();
    if (cloudRoot != null && this.CloudSavesVisible())
      this.infoScreen.AddOption((string) STRINGS.UI.FRONTEND.LOADSCREEN.OPEN_CLOUDSAVE_FOLDER, (Action<InfoDialogScreen>) (d => App.OpenWebURL(cloudRoot)), true);
    ((Component) this.infoScreen).gameObject.SetActive(true);
  }

  protected override void OnDeactivate()
  {
    if (Object.op_Inequality((Object) SpeedControlScreen.Instance, (Object) null))
      SpeedControlScreen.Instance.Unpause(false);
    this.selectedSave = (LoadScreen.SelectedSave) null;
    base.OnDeactivate();
  }

  private void ShowColonyList()
  {
    this.colonyListRoot.SetActive(true);
    this.colonyViewRoot.SetActive(false);
    this.currentColony = (List<LoadScreen.SaveGameFileDetails>) null;
    this.selectedSave = (LoadScreen.SelectedSave) null;
  }

  private bool CheckSave(LoadScreen.SaveGameFileDetails save, LocText display)
  {
    if (LoadScreen.IsSaveFileFromUninstalledDLC(save.FileInfo) && Object.op_Inequality((Object) display, (Object) null))
      ((TMP_Text) display).text = string.Format((string) STRINGS.UI.FRONTEND.LOADSCREEN.SAVE_MISSING_CONTENT, (object) save.FileName);
    if (LoadScreen.IsSaveFileFromUnsupportedFutureBuild(save.FileHeader, save.FileInfo))
    {
      if (Object.op_Inequality((Object) display, (Object) null))
        ((TMP_Text) display).text = string.Format((string) STRINGS.UI.FRONTEND.LOADSCREEN.SAVE_TOO_NEW, (object) save.FileName, (object) save.FileHeader.buildVersion, (object) save.FileInfo.saveMinorVersion, (object) 535842U, (object) 31);
      return false;
    }
    if (save.FileInfo.saveMajorVersion >= 7)
      return true;
    if (Object.op_Inequality((Object) display, (Object) null))
      ((TMP_Text) display).text = string.Format((string) STRINGS.UI.FRONTEND.LOADSCREEN.UNSUPPORTED_SAVE_VERSION, (object) save.FileName, (object) save.FileInfo.saveMajorVersion, (object) save.FileInfo.saveMinorVersion, (object) 7, (object) 31);
    return false;
  }

  private void ShowColonySave(LoadScreen.SaveGameFileDetails save)
  {
    HierarchyReferences component1 = this.colonyViewRoot.GetComponent<HierarchyReferences>();
    ((TMP_Text) ((Component) component1.GetReference<RectTransform>("Title")).GetComponent<LocText>()).text = save.BaseName;
    ((TMP_Text) ((Component) component1.GetReference<RectTransform>("Date")).GetComponent<LocText>()).text = string.Format("{0:H:mm:ss} - " + Localization.GetFileDateFormat(0), (object) save.FileDate.ToLocalTime());
    string key1 = save.FileInfo.clusterId;
    if (key1 != null && !SettingsCache.clusterLayouts.clusterCache.ContainsKey(key1))
    {
      string key2 = SettingsCache.GetScope("EXPANSION1_ID") + key1;
      if (SettingsCache.clusterLayouts.clusterCache.ContainsKey(key2))
      {
        key1 = key2;
      }
      else
      {
        DebugUtil.LogWarningArgs(new object[1]
        {
          (object) ("Failed to find cluster " + key1 + " including the scoped path, setting to default cluster name.")
        });
        Debug.Log((object) ("ClusterCache: " + string.Join(",", (IEnumerable<string>) SettingsCache.clusterLayouts.clusterCache.Keys)));
        key1 = WorldGenSettings.ClusterDefaultName;
      }
    }
    World world = key1 != null ? SettingsCache.clusterLayouts.GetWorldData(key1, 0) : (World) null;
    string str1 = world != null ? StringEntry.op_Implicit(Strings.Get(world.name)) : " - ";
    ((TMP_Text) component1.GetReference<LocText>("InfoWorld")).text = string.Format((string) STRINGS.UI.FRONTEND.LOADSCREEN.COLONY_INFO_FMT, (object) STRINGS.UI.FRONTEND.LOADSCREEN.WORLD_NAME, (object) str1);
    ((TMP_Text) component1.GetReference<LocText>("InfoCycles")).text = string.Format((string) STRINGS.UI.FRONTEND.LOADSCREEN.COLONY_INFO_FMT, (object) STRINGS.UI.FRONTEND.LOADSCREEN.CYCLES_SURVIVED, (object) save.FileInfo.numberOfCycles);
    ((TMP_Text) component1.GetReference<LocText>("InfoDupes")).text = string.Format((string) STRINGS.UI.FRONTEND.LOADSCREEN.COLONY_INFO_FMT, (object) STRINGS.UI.FRONTEND.LOADSCREEN.DUPLICANTS_ALIVE, (object) save.FileInfo.numberOfDuplicants);
    LocText component2 = ((Component) component1.GetReference<RectTransform>("FileSize")).GetComponent<LocText>();
    string formattedBytes = GameUtil.GetFormattedBytes((ulong) save.Size);
    string str2 = string.Format((string) STRINGS.UI.FRONTEND.LOADSCREEN.COLONY_FILE_SIZE, (object) formattedBytes);
    ((TMP_Text) component2).text = str2;
    ((TMP_Text) ((Component) component1.GetReference<RectTransform>("Filename")).GetComponent<LocText>()).text = string.Format((string) STRINGS.UI.FRONTEND.LOADSCREEN.COLONY_FILE_NAME, (object) System.IO.Path.GetFileName(save.FileName));
    LocText component3 = ((Component) component1.GetReference<RectTransform>("AutoInfo")).GetComponent<LocText>();
    ((Component) component3).gameObject.SetActive(!this.CheckSave(save, component3));
    Image component4 = ((Component) component1.GetReference<RectTransform>("Preview")).GetComponent<Image>();
    this.SetPreview(save.FileName, save.BaseName, component4);
    KButton component5 = ((Component) component1.GetReference<RectTransform>("DeleteButton")).GetComponent<KButton>();
    component5.ClearOnClick();
    component5.onClick += (System.Action) (() => this.Delete((System.Action) (() =>
    {
      int num = this.currentColony.IndexOf(save);
      this.currentColony.Remove(save);
      this.ShowColony(this.currentColony, num - 1);
    })));
  }

  private void ShowColony(List<LoadScreen.SaveGameFileDetails> saves, int selectIndex = -1)
  {
    if (saves.Count <= 0)
    {
      this.RefreshColonyList();
      this.ShowColonyList();
    }
    else
    {
      this.currentColony = saves;
      this.colonyListRoot.SetActive(false);
      this.colonyViewRoot.SetActive(true);
      string baseName = saves[0].BaseName;
      HierarchyReferences component1 = this.colonyViewRoot.GetComponent<HierarchyReferences>();
      KButton component2 = ((Component) component1.GetReference<RectTransform>("Back")).GetComponent<KButton>();
      component2.ClearOnClick();
      component2.onClick += (System.Action) (() => this.ShowColonyList());
      ((TMP_Text) ((Component) component1.GetReference<RectTransform>("ColonyTitle")).GetComponent<LocText>()).text = string.Format((string) STRINGS.UI.FRONTEND.LOADSCREEN.COLONY_TITLE, (object) baseName);
      GameObject gameObject1 = ((Component) component1.GetReference<RectTransform>("Content")).gameObject;
      RectTransform reference = component1.GetReference<RectTransform>("SaveTemplate");
      for (int index = 0; index < gameObject1.transform.childCount; ++index)
      {
        GameObject gameObject2 = ((Component) gameObject1.transform.GetChild(index)).gameObject;
        if (Object.op_Inequality((Object) gameObject2, (Object) null) && ((Object) gameObject2).name.Contains("Clone"))
          Object.Destroy((Object) gameObject2);
      }
      if (selectIndex < 0)
        selectIndex = 0;
      if (selectIndex > saves.Count - 1)
        selectIndex = saves.Count - 1;
      for (int index = 0; index < saves.Count; ++index)
      {
        LoadScreen.SaveGameFileDetails save = saves[index];
        RectTransform rectTransform = Object.Instantiate<RectTransform>(reference, gameObject1.transform);
        HierarchyReferences component3 = ((Component) rectTransform).GetComponent<HierarchyReferences>();
        ((Component) rectTransform).gameObject.SetActive(true);
        ((Component) component3.GetReference<RectTransform>("AutoLabel")).gameObject.SetActive(save.FileInfo.isAutoSave);
        ((TMP_Text) ((Component) component3.GetReference<RectTransform>("SaveText")).GetComponent<LocText>()).text = System.IO.Path.GetFileNameWithoutExtension(save.FileName);
        ((TMP_Text) ((Component) component3.GetReference<RectTransform>("DateText")).GetComponent<LocText>()).text = string.Format("{0:H:mm:ss} - " + Localization.GetFileDateFormat(0), (object) save.FileDate.ToLocalTime());
        ((Component) component3.GetReference<RectTransform>("NewestLabel")).gameObject.SetActive(index == 0);
        int num = this.CheckSave(save, (LocText) null) ? 1 : 0;
        KButton button = ((Component) rectTransform).GetComponent<KButton>();
        button.ClearOnClick();
        button.onClick += (System.Action) (() =>
        {
          this.UpdateSelected(button, save.FileName, save.FileInfo.dlcId);
          this.ShowColonySave(save);
        });
        if (num != 0)
          button.onDoubleClick += (System.Action) (() =>
          {
            this.UpdateSelected(button, save.FileName, save.FileInfo.dlcId);
            this.Load();
          });
        KButton component4 = ((Component) component3.GetReference<RectTransform>("LoadButton")).GetComponent<KButton>();
        component4.ClearOnClick();
        if (num == 0)
        {
          component4.isInteractable = false;
          ((Component) component4).GetComponent<ImageToggleState>().SetState((ImageToggleState.State) 0);
        }
        else
          component4.onClick += (System.Action) (() =>
          {
            this.UpdateSelected(button, save.FileName, save.FileInfo.dlcId);
            this.Load();
          });
        if (index == selectIndex)
        {
          this.UpdateSelected(button, save.FileName, save.FileInfo.dlcId);
          this.ShowColonySave(save);
        }
      }
    }
  }

  private void AddColonyToList(List<LoadScreen.SaveGameFileDetails> saves)
  {
    if (saves.Count == 0)
      return;
    HierarchyReferences freeElement = this.colonyListPool.GetFreeElement(this.saveButtonRoot, true);
    saves.Sort((Comparison<LoadScreen.SaveGameFileDetails>) ((x, y) => y.FileDate.CompareTo(x.FileDate)));
    LoadScreen.SaveGameFileDetails firstSave = saves[0];
    bool flag1 = LoadScreen.IsSaveFromCurrentDLC(firstSave.FileInfo, out string _);
    string colonyName = firstSave.BaseName;
    (int, int, ulong) savesSizeAndCounts = this.GetSavesSizeAndCounts(saves);
    int num1 = savesSizeAndCounts.Item1;
    int num2 = savesSizeAndCounts.Item2;
    string formattedBytes = GameUtil.GetFormattedBytes(savesSizeAndCounts.Item3);
    ((TMP_Text) ((Component) freeElement.GetReference<RectTransform>("HeaderTitle")).GetComponent<LocText>()).text = colonyName;
    ((TMP_Text) ((Component) freeElement.GetReference<RectTransform>("HeaderDate")).GetComponent<LocText>()).text = string.Format("{0:H:mm:ss} - " + Localization.GetFileDateFormat(0), (object) firstSave.FileDate.ToLocalTime());
    ((TMP_Text) ((Component) freeElement.GetReference<RectTransform>("SaveTitle")).GetComponent<LocText>()).text = string.Format((string) STRINGS.UI.FRONTEND.LOADSCREEN.SAVE_INFO, (object) num1, (object) num2, (object) formattedBytes);
    Image component1 = ((Component) freeElement.GetReference<RectTransform>("Preview")).GetComponent<Image>();
    this.SetPreview(firstSave.FileName, colonyName, component1, true);
    KImage reference1 = freeElement.GetReference<KImage>("DlcIcon");
    if (firstSave.FileInfo.dlcId == "EXPANSION1_ID")
    {
      ((Behaviour) reference1).enabled = true;
      ((Component) reference1).GetComponent<ToolTip>().SetSimpleTooltip((string) STRINGS.UI.FRONTEND.LOADSCREEN.SAVE_FROM_SPACED_OUT_TOOLTIP);
    }
    else
      ((Behaviour) reference1).enabled = false;
    RectTransform reference2 = freeElement.GetReference<RectTransform>("LocationIcons");
    bool flag2 = this.CloudSavesVisible();
    ((Component) reference2).gameObject.SetActive(flag2);
    if (flag2)
    {
      LocText locationText = ((Component) freeElement.GetReference<RectTransform>("LocationText")).GetComponent<LocText>();
      bool isLocal = SaveLoader.IsSaveLocal(firstSave.FileName);
      ((TMP_Text) locationText).text = (string) (isLocal ? STRINGS.UI.FRONTEND.LOADSCREEN.LOCAL_SAVE : STRINGS.UI.FRONTEND.LOADSCREEN.CLOUD_SAVE);
      KButton cloudButton = ((Component) freeElement.GetReference<RectTransform>("CloudButton")).GetComponent<KButton>();
      KButton localButton = ((Component) freeElement.GetReference<RectTransform>("LocalButton")).GetComponent<KButton>();
      ((Component) cloudButton).gameObject.SetActive(!isLocal);
      cloudButton.ClearOnClick();
      cloudButton.onClick += (System.Action) (() => this.ConfirmCloudSaveMigrations(string.Format("{0}\n", (object) STRINGS.UI.FRONTEND.LOADSCREEN.CONVERT_TO_LOCAL_DETAILS), (string) STRINGS.UI.FRONTEND.LOADSCREEN.CONVERT_TO_LOCAL, (string) STRINGS.UI.FRONTEND.LOADSCREEN.CONVERT_COLONY, (string) null, (System.Action) (() =>
      {
        ((Component) cloudButton).gameObject.SetActive(false);
        isLocal = true;
        ((TMP_Text) locationText).text = (string) (isLocal ? STRINGS.UI.FRONTEND.LOADSCREEN.LOCAL_SAVE : STRINGS.UI.FRONTEND.LOADSCREEN.CLOUD_SAVE);
        this.ConvertColonyToLocal(colonyName);
        this.RefreshColonyList();
        MainMenu.Instance.RefreshResumeButton();
      }), (System.Action) null, this.cloudToLocalSprite));
      ((Component) localButton).gameObject.SetActive(isLocal);
      localButton.ClearOnClick();
      localButton.onClick += (System.Action) (() => this.ConfirmCloudSaveMigrations(string.Format("{0}\n", (object) STRINGS.UI.FRONTEND.LOADSCREEN.CONVERT_TO_CLOUD_DETAILS), (string) STRINGS.UI.FRONTEND.LOADSCREEN.CONVERT_TO_CLOUD, (string) STRINGS.UI.FRONTEND.LOADSCREEN.CONVERT_COLONY, (string) null, (System.Action) (() =>
      {
        ((Component) localButton).gameObject.SetActive(false);
        isLocal = false;
        ((TMP_Text) locationText).text = (string) (isLocal ? STRINGS.UI.FRONTEND.LOADSCREEN.LOCAL_SAVE : STRINGS.UI.FRONTEND.LOADSCREEN.CLOUD_SAVE);
        this.ConvertColonyToCloud(colonyName);
        this.RefreshColonyList();
        MainMenu.Instance.RefreshResumeButton();
      }), (System.Action) null, this.localToCloudSprite));
    }
    KButton component2 = ((Component) freeElement.GetReference<RectTransform>("Button")).GetComponent<KButton>();
    component2.ClearOnClick();
    component2.isInteractable = flag1;
    component2.onClick += (System.Action) (() => this.ShowColony(saves));
    if (this.CheckSave(firstSave, (LocText) null))
      component2.onDoubleClick += (System.Action) (() =>
      {
        this.UpdateSelected((KButton) null, firstSave.FileName, firstSave.FileInfo.dlcId);
        this.Load();
      });
    freeElement.transform.SetAsLastSibling();
  }

  private void SetPreview(
    string filename,
    string basename,
    Image preview,
    bool fallbackToTimelapse = false)
  {
    ((Graphic) preview).color = Color.black;
    ((Component) preview).gameObject.SetActive(false);
    try
    {
      Sprite sprite = RetireColonyUtility.LoadColonyPreview(filename, basename, fallbackToTimelapse);
      if (Object.op_Equality((Object) sprite, (Object) null))
        return;
      Rect rect = Util.rectTransform((Component) ((Transform) ((Graphic) preview).rectTransform).parent).rect;
      preview.sprite = sprite;
      ((Graphic) preview).color = Object.op_Implicit((Object) sprite) ? Color.white : Color.black;
      Bounds bounds1 = sprite.bounds;
      double x = (double) ((Bounds) ref bounds1).size.x;
      Bounds bounds2 = sprite.bounds;
      double y = (double) ((Bounds) ref bounds2).size.y;
      float num = (float) (x / y);
      if ((double) num >= 16.0 / 9.0)
        ((Graphic) preview).rectTransform.sizeDelta = new Vector2(((Rect) ref rect).height * num, ((Rect) ref rect).height);
      else
        ((Graphic) preview).rectTransform.sizeDelta = new Vector2(((Rect) ref rect).width, ((Rect) ref rect).width / num);
      ((Component) preview).gameObject.SetActive(true);
    }
    catch (Exception ex)
    {
      Debug.Log((object) ex);
    }
  }

  public static void ForceStopGame()
  {
    ThreadedHttps<KleiMetrics>.Instance.SendProfileStats();
    Game.Instance.SetIsLoading();
    Grid.CellCount = 0;
    Sim.Shutdown();
  }

  private static bool IsSaveFileFromUnsupportedFutureBuild(
    SaveGame.Header header,
    SaveGame.GameInfo gameInfo)
  {
    return gameInfo.saveMajorVersion > 7 || gameInfo.saveMajorVersion == 7 && gameInfo.saveMinorVersion > 31 || header.buildVersion > 535842U;
  }

  private static bool IsSaveFromCurrentDLC(SaveGame.GameInfo gameInfo, out string saveDlcName)
  {
    switch (gameInfo.dlcId)
    {
      case "EXPANSION1_ID":
        saveDlcName = (string) STRINGS.UI.DLC1.NAME_ITAL;
        break;
      default:
        saveDlcName = (string) STRINGS.UI.VANILLA.NAME_ITAL;
        break;
    }
    return gameInfo.dlcId == DlcManager.GetHighestActiveDlcId();
  }

  private static bool IsSaveFileFromUninstalledDLC(SaveGame.GameInfo gameInfo) => DlcManager.IsContentActive(gameInfo.dlcId);

  private void UpdateSelected(KButton button, string filename, string dlcId)
  {
    if (this.selectedSave != null && Object.op_Inequality((Object) this.selectedSave.button, (Object) null))
      ((Component) this.selectedSave.button).GetComponent<ImageToggleState>().SetState((ImageToggleState.State) 1);
    if (this.selectedSave == null)
      this.selectedSave = new LoadScreen.SelectedSave();
    this.selectedSave.button = button;
    this.selectedSave.filename = filename;
    this.selectedSave.dlcId = dlcId;
    if (!Object.op_Inequality((Object) this.selectedSave.button, (Object) null))
      return;
    ((Component) this.selectedSave.button).GetComponent<ImageToggleState>().SetState((ImageToggleState.State) 2);
  }

  private void Load()
  {
    if (this.selectedSave.dlcId != DlcManager.GetHighestActiveDlcId())
      this.ConfirmDoAction((string) (DlcManager.IsVanillaId(this.selectedSave.dlcId) ? STRINGS.UI.FRONTEND.LOADSCREEN.VANILLA_RESTART : STRINGS.UI.FRONTEND.LOADSCREEN.EXPANSION1_RESTART), (System.Action) (() =>
      {
        KPlayerPrefs.SetString("AutoResumeSaveFile", this.selectedSave.filename);
        DlcManager.ToggleDLC("EXPANSION1_ID");
      }));
    else
      LoadingOverlay.Load(new System.Action(this.DoLoad));
  }

  private void DoLoad()
  {
    if (this.selectedSave == null)
      return;
    LoadScreen.DoLoad(this.selectedSave.filename);
    this.Deactivate();
  }

  private static void DoLoad(string filename)
  {
    KCrashReporter.MOST_RECENT_SAVEFILE = filename;
    SaveGame.Header header;
    SaveGame.GameInfo gameInfo = SaveLoader.LoadHeader(filename, out header);
    string str1 = (string) null;
    string str2 = (string) null;
    if (header.buildVersion > 535842U)
    {
      str1 = header.buildVersion.ToString();
      str2 = 535842U.ToString();
    }
    else if (gameInfo.saveMajorVersion < 7)
    {
      str1 = string.Format("v{0}.{1}", (object) gameInfo.saveMajorVersion, (object) gameInfo.saveMinorVersion);
      str2 = string.Format("v{0}.{1}", (object) 7, (object) 31);
    }
    if (false)
    {
      Util.KInstantiateUI(((Component) ScreenPrefabs.Instance.ConfirmDialogScreen).gameObject, Object.op_Equality((Object) FrontEndManager.Instance, (Object) null) ? GameScreenManager.Instance.ssOverlayCanvas : ((Component) FrontEndManager.Instance).gameObject, true).GetComponent<ConfirmDialogScreen>().PopupConfirmDialog(string.Format((string) STRINGS.UI.CRASHSCREEN.LOADFAILED, (object) "Version Mismatch", (object) str1, (object) str2), (System.Action) null, (System.Action) null);
    }
    else
    {
      if (Object.op_Inequality((Object) Game.Instance, (Object) null))
        LoadScreen.ForceStopGame();
      SaveLoader.SetActiveSaveFilePath(filename);
      Time.timeScale = 0.0f;
      App.LoadScene("backend");
    }
  }

  private void MoreInfo() => App.OpenWebURL("http://support.kleientertainment.com/customer/portal/articles/2776550");

  private void Delete(System.Action onDelete)
  {
    if (this.selectedSave == null || string.IsNullOrEmpty(this.selectedSave.filename))
      Debug.LogError((object) "The path provided is not valid and cannot be deleted.");
    else
      this.ConfirmDoAction(string.Format((string) STRINGS.UI.FRONTEND.LOADSCREEN.CONFIRMDELETE, (object) System.IO.Path.GetFileName(this.selectedSave.filename)), (System.Action) (() =>
      {
        try
        {
          this.DeleteFileAndEmptyFolder(this.selectedSave.filename);
          this.DeleteFileAndEmptyFolder(System.IO.Path.ChangeExtension(this.selectedSave.filename, "png"));
          if (onDelete != null)
            onDelete();
          MainMenu.Instance.RefreshResumeButton();
        }
        catch (SystemException ex)
        {
          Debug.LogError((object) ex.ToString());
        }
      }));
  }

  private void ShowSimpleDialog(string title, string message) => Util.KInstantiateUI<InfoDialogScreen>(((Component) ScreenPrefabs.Instance.InfoDialogScreen).gameObject, ((Component) this).gameObject, false).SetHeader(title).AddPlainText(message).AddDefaultOK().Activate();

  private void ConfirmCloudSaveMigrations(
    string message,
    string title,
    string confirmText,
    string backupText,
    System.Action commitAction,
    System.Action backupAction,
    Sprite sprite)
  {
    Util.KInstantiateUI<InfoDialogScreen>(((Component) ScreenPrefabs.Instance.InfoDialogScreen).gameObject, ((Component) this).gameObject, false).SetHeader(title).AddSprite(sprite).AddPlainText(message).AddDefaultCancel().AddOption(confirmText, (Action<InfoDialogScreen>) (d =>
    {
      d.Deactivate();
      commitAction();
    }), true).Activate();
  }

  private void ShowConvertError(string message)
  {
    if (!Object.op_Equality((Object) this.errorInfoScreen, (Object) null))
      return;
    if (DistributionPlatform.Initialized && SteamUtils.IsSteamRunningOnSteamDeck())
    {
      this.errorInfoScreen = Util.KInstantiateUI<InfoDialogScreen>(((Component) ScreenPrefabs.Instance.InfoDialogScreen).gameObject, ((Component) this).gameObject, false).SetHeader((string) STRINGS.UI.FRONTEND.LOADSCREEN.CONVERT_ERROR_TITLE).AddSprite(this.errorSprite).AddPlainText(message).AddDefaultOK();
      this.errorInfoScreen.Activate();
    }
    else
    {
      this.errorInfoScreen = Util.KInstantiateUI<InfoDialogScreen>(((Component) ScreenPrefabs.Instance.InfoDialogScreen).gameObject, ((Component) this).gameObject, false).SetHeader((string) STRINGS.UI.FRONTEND.LOADSCREEN.CONVERT_ERROR_TITLE).AddSprite(this.errorSprite).AddPlainText(message).AddOption((string) STRINGS.UI.FRONTEND.LOADSCREEN.MIGRATE_FAILURES_FORUM_BUTTON, (Action<InfoDialogScreen>) (d => App.OpenWebURL("https://forums.kleientertainment.com/klei-bug-tracker/oni/"))).AddDefaultOK();
      this.errorInfoScreen.Activate();
    }
  }

  private void ConfirmDoAction(string message, System.Action action)
  {
    if (!Object.op_Equality((Object) this.confirmScreen, (Object) null))
      return;
    this.confirmScreen = Util.KInstantiateUI<ConfirmDialogScreen>(((Component) ScreenPrefabs.Instance.ConfirmDialogScreen).gameObject, ((Component) this).gameObject, false);
    this.confirmScreen.PopupConfirmDialog(message, action, (System.Action) (() => { }));
    ((Component) this.confirmScreen).gameObject.SetActive(true);
  }

  public override void OnKeyDown(KButtonEvent e)
  {
    if (this.currentColony != null && e.TryConsume((Action) 1))
      this.ShowColonyList();
    base.OnKeyDown(e);
  }

  private struct SaveGameFileDetails
  {
    public string BaseName;
    public string FileName;
    public string UniqueID;
    public System.DateTime FileDate;
    public SaveGame.Header FileHeader;
    public SaveGame.GameInfo FileInfo;
    public long Size;
  }

  private class SelectedSave
  {
    public string filename;
    public string dlcId;
    public KButton button;
  }
}
