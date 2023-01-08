// Decompiled with JetBrains decompiler
// Type: SaveGame
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.CustomSettings;
using KSerialization;
using Newtonsoft.Json;
using ProcGen;
using STRINGS;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/SaveGame")]
public class SaveGame : KMonoBehaviour, ISaveLoadable
{
  [Serialize]
  private int speed;
  [Serialize]
  public List<Tag> expandedResourceTags = new List<Tag>();
  [Serialize]
  public int minGermCountForDisinfect = 10000;
  [Serialize]
  public bool enableAutoDisinfect = true;
  [Serialize]
  public bool sandboxEnabled;
  [Serialize]
  private int autoSaveCycleInterval = 1;
  [Serialize]
  private Vector2I timelapseResolution = new Vector2I(512, 768);
  private string baseName;
  public static SaveGame Instance;
  public EntombedItemManager entombedItemManager;
  public WorldGenSpawner worldGenSpawner;
  [MyCmpReq]
  public MaterialSelectorSerializer materialSelectorSerializer;
  private static bool debug_SaveFileHeaderBlank_sent;

  public int AutoSaveCycleInterval
  {
    get => this.autoSaveCycleInterval;
    set => this.autoSaveCycleInterval = value;
  }

  public Vector2I TimelapseResolution
  {
    get => this.timelapseResolution;
    set => this.timelapseResolution = value;
  }

  public string BaseName => this.baseName;

  public static void DestroyInstance() => SaveGame.Instance = (SaveGame) null;

  protected virtual void OnPrefabInit()
  {
    SaveGame.Instance = this;
    new ColonyRationMonitor.Instance((IStateMachineTarget) this).StartSM();
    this.entombedItemManager = ((Component) this).gameObject.AddComponent<EntombedItemManager>();
    this.worldGenSpawner = ((Component) this).gameObject.AddComponent<WorldGenSpawner>();
    ((Component) this).gameObject.AddOrGetDef<GameplaySeasonManager.Def>();
    ((Component) this).gameObject.AddOrGetDef<ClusterFogOfWarManager.Def>();
  }

  [OnSerializing]
  private void OnSerialize() => this.speed = SpeedControlScreen.Instance.GetSpeed();

  [OnDeserializing]
  private void OnDeserialize() => this.baseName = SaveLoader.Instance.GameInfo.baseName;

  public int GetSpeed() => this.speed;

  public byte[] GetSaveHeader(bool isAutoSave, bool isCompressed, out SaveGame.Header header)
  {
    string originalSaveFileName = SaveLoader.GetOriginalSaveFileName(SaveLoader.GetActiveSaveFilePath());
    byte[] bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object) new SaveGame.GameInfo(GameClock.Instance.GetCycle(), Components.LiveMinionIdentities.Count, this.baseName, isAutoSave, originalSaveFileName, SaveLoader.Instance.GameInfo.clusterId, SaveLoader.Instance.GameInfo.worldTraits, SaveLoader.Instance.GameInfo.colonyGuid, DlcManager.GetHighestActiveDlcId(), this.sandboxEnabled)));
    header = new SaveGame.Header();
    header.buildVersion = 535842U;
    header.headerSize = bytes.Length;
    header.headerVersion = 1U;
    header.compression = isCompressed ? 1 : 0;
    return bytes;
  }

  public static string GetSaveUniqueID(SaveGame.GameInfo info) => !(info.colonyGuid != Guid.Empty) ? info.baseName + "/" + info.clusterId : info.colonyGuid.ToString();

  public static Tuple<SaveGame.Header, SaveGame.GameInfo> GetFileInfo(string filename)
  {
    try
    {
      SaveGame.Header header;
      SaveGame.GameInfo gameInfo = SaveLoader.LoadHeader(filename, out header);
      if (gameInfo.saveMajorVersion >= 7)
        return new Tuple<SaveGame.Header, SaveGame.GameInfo>(header, gameInfo);
    }
    catch (Exception ex)
    {
      Debug.LogWarning((object) ("Exception while loading " + filename));
      Debug.LogWarning((object) ex);
    }
    return (Tuple<SaveGame.Header, SaveGame.GameInfo>) null;
  }

  public static SaveGame.GameInfo GetHeader(
    IReader br,
    out SaveGame.Header header,
    string debugFileName)
  {
    header = new SaveGame.Header();
    header.buildVersion = br.ReadUInt32();
    header.headerSize = br.ReadInt32();
    header.headerVersion = br.ReadUInt32();
    if (1U <= header.headerVersion)
      header.compression = br.ReadInt32();
    byte[] data = br.ReadBytes(header.headerSize);
    if (header.headerSize == 0 && !SaveGame.debug_SaveFileHeaderBlank_sent)
    {
      SaveGame.debug_SaveFileHeaderBlank_sent = true;
      Debug.LogWarning((object) ("SaveFileHeaderBlank - " + debugFileName));
    }
    SaveGame.GameInfo gameInfo = SaveGame.GetGameInfo(data);
    if (gameInfo.IsVersionOlderThan(7, 14) && gameInfo.worldTraits != null)
    {
      string[] worldTraits = gameInfo.worldTraits;
      for (int index = 0; index < worldTraits.Length; ++index)
        worldTraits[index] = worldTraits[index].Replace('\\', '/');
    }
    if (gameInfo.IsVersionOlderThan(7, 20))
      gameInfo.dlcId = "";
    return gameInfo;
  }

  public static SaveGame.GameInfo GetGameInfo(byte[] data) => JsonConvert.DeserializeObject<SaveGame.GameInfo>(Encoding.UTF8.GetString(data));

  public void SetBaseName(string newBaseName)
  {
    if (string.IsNullOrEmpty(newBaseName))
      Debug.LogWarning((object) "Cannot give the base an empty name");
    else
      this.baseName = newBaseName;
  }

  protected virtual void OnSpawn()
  {
    ThreadedHttps<KleiMetrics>.Instance.SendProfileStats();
    Game.Instance.Trigger(-1917495436, (object) null);
  }

  public List<Tuple<string, TextStyleSetting>> GetColonyToolTip()
  {
    List<Tuple<string, TextStyleSetting>> colonyToolTip = new List<Tuple<string, TextStyleSetting>>();
    SettingLevel currentQualitySetting = CustomGameSettings.Instance.GetCurrentQualitySetting((SettingConfig) CustomGameSettingConfigs.ClusterLayout);
    ClusterLayout clusterLayout;
    SettingsCache.clusterLayouts.clusterCache.TryGetValue(currentQualitySetting.id, out clusterLayout);
    colonyToolTip.Add(new Tuple<string, TextStyleSetting>(this.baseName, ToolTipScreen.Instance.defaultTooltipHeaderStyle));
    if (DlcManager.IsExpansion1Active())
    {
      StringEntry stringEntry = Strings.Get(clusterLayout.name);
      colonyToolTip.Add(new Tuple<string, TextStyleSetting>(StringEntry.op_Implicit(stringEntry), ToolTipScreen.Instance.defaultTooltipBodyStyle));
    }
    if (Object.op_Inequality((Object) GameClock.Instance, (Object) null))
    {
      colonyToolTip.Add(new Tuple<string, TextStyleSetting>(" ", (TextStyleSetting) null));
      colonyToolTip.Add(new Tuple<string, TextStyleSetting>(string.Format((string) UI.ASTEROIDCLOCK.CYCLES_OLD, (object) GameUtil.GetCurrentCycle()), ToolTipScreen.Instance.defaultTooltipHeaderStyle));
      colonyToolTip.Add(new Tuple<string, TextStyleSetting>(string.Format((string) UI.ASTEROIDCLOCK.TIME_PLAYED, (object) (GameClock.Instance.GetTimePlayedInSeconds() / 3600f).ToString("0.00")), ToolTipScreen.Instance.defaultTooltipBodyStyle));
    }
    int cameraActiveCluster = CameraController.Instance.cameraActiveCluster;
    WorldContainer world = ClusterManager.Instance.GetWorld(cameraActiveCluster);
    colonyToolTip.Add(new Tuple<string, TextStyleSetting>(" ", (TextStyleSetting) null));
    if (DlcManager.IsExpansion1Active())
    {
      colonyToolTip.Add(new Tuple<string, TextStyleSetting>(((Component) world).GetComponent<ClusterGridEntity>().Name, ToolTipScreen.Instance.defaultTooltipHeaderStyle));
    }
    else
    {
      StringEntry stringEntry = Strings.Get(clusterLayout.name);
      colonyToolTip.Add(new Tuple<string, TextStyleSetting>(StringEntry.op_Implicit(stringEntry), ToolTipScreen.Instance.defaultTooltipHeaderStyle));
    }
    if (SaveLoader.Instance.GameInfo.worldTraits != null && SaveLoader.Instance.GameInfo.worldTraits.Length != 0)
    {
      foreach (string worldTrait in SaveLoader.Instance.GameInfo.worldTraits)
      {
        WorldTrait cachedWorldTrait = SettingsCache.GetCachedWorldTrait(worldTrait, false);
        if (cachedWorldTrait != null)
          colonyToolTip.Add(new Tuple<string, TextStyleSetting>(StringEntry.op_Implicit(Strings.Get(cachedWorldTrait.name)), ToolTipScreen.Instance.defaultTooltipBodyStyle));
        else
          colonyToolTip.Add(new Tuple<string, TextStyleSetting>((string) WORLD_TRAITS.MISSING_TRAIT, ToolTipScreen.Instance.defaultTooltipBodyStyle));
      }
    }
    else if (world.WorldTraitIds != null)
    {
      foreach (string worldTraitId in world.WorldTraitIds)
      {
        WorldTrait cachedWorldTrait = SettingsCache.GetCachedWorldTrait(worldTraitId, false);
        if (cachedWorldTrait != null)
          colonyToolTip.Add(new Tuple<string, TextStyleSetting>(StringEntry.op_Implicit(Strings.Get(cachedWorldTrait.name)), ToolTipScreen.Instance.defaultTooltipBodyStyle));
        else
          colonyToolTip.Add(new Tuple<string, TextStyleSetting>((string) WORLD_TRAITS.MISSING_TRAIT, ToolTipScreen.Instance.defaultTooltipBodyStyle));
      }
      if (world.WorldTraitIds.Count == 0)
        colonyToolTip.Add(new Tuple<string, TextStyleSetting>((string) WORLD_TRAITS.NO_TRAITS.NAME_SHORTHAND, ToolTipScreen.Instance.defaultTooltipBodyStyle));
    }
    return colonyToolTip;
  }

  public struct Header
  {
    public uint buildVersion;
    public int headerSize;
    public uint headerVersion;
    public int compression;

    public bool IsCompressed => this.compression != 0;
  }

  public struct GameInfo
  {
    public int numberOfCycles;
    public int numberOfDuplicants;
    public string baseName;
    public bool isAutoSave;
    public string originalSaveName;
    public int saveMajorVersion;
    public int saveMinorVersion;
    public string clusterId;
    public string[] worldTraits;
    public bool sandboxEnabled;
    public Guid colonyGuid;
    public string dlcId;

    public GameInfo(
      int numberOfCycles,
      int numberOfDuplicants,
      string baseName,
      bool isAutoSave,
      string originalSaveName,
      string clusterId,
      string[] worldTraits,
      Guid colonyGuid,
      string dlcId,
      bool sandboxEnabled = false)
    {
      this.numberOfCycles = numberOfCycles;
      this.numberOfDuplicants = numberOfDuplicants;
      this.baseName = baseName;
      this.isAutoSave = isAutoSave;
      this.originalSaveName = originalSaveName;
      this.clusterId = clusterId;
      this.worldTraits = worldTraits;
      this.colonyGuid = colonyGuid;
      this.sandboxEnabled = sandboxEnabled;
      this.dlcId = dlcId;
      this.saveMajorVersion = 7;
      this.saveMinorVersion = 31;
    }

    public bool IsVersionOlderThan(int major, int minor)
    {
      if (this.saveMajorVersion < major)
        return true;
      return this.saveMajorVersion == major && this.saveMinorVersion < minor;
    }

    public bool IsVersionExactly(int major, int minor) => this.saveMajorVersion == major && this.saveMinorVersion == minor;
  }
}
