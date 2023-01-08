// Decompiled with JetBrains decompiler
// Type: StoryManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class StoryManager : KMonoBehaviour
{
  public const int BEFORE_STORIES = -2;
  private static List<StoryManager.StoryTelemetry> storyTelemetry = new List<StoryManager.StoryTelemetry>();
  [Serialize]
  private Dictionary<int, StoryInstance> _stories = new Dictionary<int, StoryInstance>();
  [Serialize]
  private int highestStoryCoordinateWhenGenerated = -2;
  private const string STORY_TRAIT_KEY = "StoryTraits";
  private const string STORY_CREATION_KEY = "StoryTraitsCreation";
  private const string STORY_COORDINATE_KEY = "SavedHighestStoryCoordinate";

  public static StoryManager Instance { get; private set; }

  public static IReadOnlyList<StoryManager.StoryTelemetry> GetTelemetry() => (IReadOnlyList<StoryManager.StoryTelemetry>) StoryManager.storyTelemetry;

  protected virtual void OnPrefabInit()
  {
    StoryManager.Instance = this;
    GameClock.Instance.Subscribe(631075836, new Action<object>(this.OnNewDayStarted));
    Game.Instance.OnLoad += new Action<Game.GameSaveData>(this.OnGameLoaded);
  }

  protected virtual void OnCleanUp()
  {
    GameClock.Instance.Unsubscribe(631075836, new Action<object>(this.OnNewDayStarted));
    Game.Instance.OnLoad -= new Action<Game.GameSaveData>(this.OnGameLoaded);
  }

  public void InitialSaveSetup()
  {
    this.highestStoryCoordinateWhenGenerated = Db.Get().Stories.GetHighestCoordinateOffset();
    foreach (WorldContainer worldContainer in (IEnumerable<WorldContainer>) ClusterManager.Instance.WorldContainers)
    {
      foreach (string storyTraitId in worldContainer.StoryTraitIds)
        this.CreateStory(Db.Get().Stories.GetStoryFromStoryTrait(storyTraitId), worldContainer.id);
    }
    this.LogInitialSaveSetup();
  }

  public StoryInstance CreateStory(string id, int worldId) => this.CreateStory(Db.Get().Stories.Get(id), worldId);

  public StoryInstance CreateStory(Story story, int worldId)
  {
    StoryInstance story1 = new StoryInstance(story, worldId);
    this._stories.Add(story.HashId, story1);
    StoryManager.InitTelemetry(story1);
    if (story.autoStart)
      this.BeginStoryEvent(story);
    return story1;
  }

  public StoryInstance GetStoryInstance(int hash)
  {
    StoryInstance storyInstance;
    this._stories.TryGetValue(hash, out storyInstance);
    return storyInstance;
  }

  public Dictionary<int, StoryInstance> GetStoryInstances() => this._stories;

  public int GetHighestCoordinate() => this.highestStoryCoordinateWhenGenerated;

  private string GetCompleteUnlockId(string id) => id + "_STORY_COMPLETE";

  public void ForceCreateStory(Story story, int worldId)
  {
    if (this.GetStoryInstance(story.HashId) != null)
      return;
    this.CreateStory(story, worldId);
  }

  public void DiscoverStoryEvent(Story story)
  {
    StoryInstance storyInstance = this.GetStoryInstance(story.HashId);
    if (storyInstance == null || this.CheckState(StoryInstance.State.DISCOVERED, story))
      return;
    storyInstance.CurrentState = StoryInstance.State.DISCOVERED;
  }

  public void BeginStoryEvent(Story story)
  {
    StoryInstance storyInstance = this.GetStoryInstance(story.HashId);
    if (storyInstance == null || this.CheckState(StoryInstance.State.IN_PROGRESS, story))
      return;
    storyInstance.CurrentState = StoryInstance.State.IN_PROGRESS;
  }

  public void CompleteStoryEvent(
    Story story,
    MonoBehaviour keepsakeSpawnTarget,
    FocusTargetSequence.Data sequenceData)
  {
    if (this.GetStoryInstance(story.HashId) == null || this.CheckState(StoryInstance.State.COMPLETE, story))
      return;
    FocusTargetSequence.Start(keepsakeSpawnTarget, sequenceData);
  }

  public void CompleteStoryEvent(Story story, Vector3 keepsakeSpawnPosition)
  {
    StoryInstance storyInstance = this.GetStoryInstance(story.HashId);
    if (storyInstance == null)
      return;
    GameObject prefab = Assets.GetPrefab(Tag.op_Implicit(storyInstance.GetStory().keepsakePrefabId));
    if (Object.op_Inequality((Object) prefab, (Object) null))
    {
      keepsakeSpawnPosition.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
      GameObject gameObject = Util.KInstantiate(prefab, keepsakeSpawnPosition);
      gameObject.SetActive(true);
      new UpgradeFX.Instance((IStateMachineTarget) gameObject.GetComponent<KMonoBehaviour>(), new Vector3(0.0f, -0.5f, -0.1f)).StartSM();
    }
    storyInstance.CurrentState = StoryInstance.State.COMPLETE;
    Game.Instance.unlocks.Unlock(this.GetCompleteUnlockId(story.Id));
  }

  public bool CheckState(StoryInstance.State state, Story story)
  {
    StoryInstance storyInstance = this.GetStoryInstance(story.HashId);
    return storyInstance != null && storyInstance.CurrentState >= state;
  }

  public bool IsStoryComplete(Story story) => this.CheckState(StoryInstance.State.COMPLETE, story);

  public bool IsStoryCompleteGlobal(Story story) => Game.Instance.unlocks.IsUnlocked(this.GetCompleteUnlockId(story.Id));

  public StoryInstance DisplayPopup(
    Story story,
    StoryManager.PopupInfo info,
    System.Action popupCB = null,
    Notification.ClickCallback notificationCB = null)
  {
    StoryInstance storyInstance = this.GetStoryInstance(story.HashId);
    if (storyInstance == null || storyInstance.HasDisplayedPopup(info.PopupType))
      return (StoryInstance) null;
    EventInfoData storyTraitData = EventInfoDataHelper.GenerateStoryTraitData(info.Title, info.Description, info.CloseButtonText, info.TextureName, info.PopupType, info.CloseButtonToolTip, info.Minions, popupCB);
    Notification notification = (Notification) null;
    if (!info.DisplayImmediate)
      notification = EventInfoScreen.CreateNotification(storyTraitData, notificationCB);
    storyInstance.SetPopupData(info, storyTraitData, notification);
    return storyInstance;
  }

  public bool HasDisplayedPopup(Story story, EventInfoDataHelper.PopupType type)
  {
    StoryInstance storyInstance = this.GetStoryInstance(story.HashId);
    return storyInstance != null && storyInstance.HasDisplayedPopup(type);
  }

  private void LogInitialSaveSetup()
  {
    int index = 0;
    StoryManager.StoryCreationTelemetry[] data = new StoryManager.StoryCreationTelemetry[CustomGameSettings.Instance.CurrentStoryLevelsBySetting.Count];
    foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) CustomGameSettings.Instance.CurrentStoryLevelsBySetting)
    {
      data[index] = new StoryManager.StoryCreationTelemetry()
      {
        StoryId = keyValuePair.Key,
        Enabled = CustomGameSettings.Instance.IsStoryActive(keyValuePair.Key, keyValuePair.Value)
      };
      ++index;
    }
    OniMetrics.LogEvent(OniMetrics.Event.NewSave, "StoryTraitsCreation", (object) data);
  }

  private void OnNewDayStarted(object _)
  {
    OniMetrics.LogEvent(OniMetrics.Event.EndOfCycle, "SavedHighestStoryCoordinate", (object) this.highestStoryCoordinateWhenGenerated);
    OniMetrics.LogEvent(OniMetrics.Event.EndOfCycle, "StoryTraits", (object) StoryManager.storyTelemetry);
  }

  private static void InitTelemetry(StoryInstance story)
  {
    WorldContainer world = ClusterManager.Instance.GetWorld(story.worldId);
    if (Object.op_Equality((Object) world, (Object) null))
      return;
    story.Telemetry.StoryId = story.storyId;
    story.Telemetry.WorldId = world.worldName;
    StoryManager.storyTelemetry.Add(story.Telemetry);
  }

  private void OnGameLoaded(object _)
  {
    StoryManager.storyTelemetry.Clear();
    foreach (KeyValuePair<int, StoryInstance> storey in this._stories)
      StoryManager.InitTelemetry(storey.Value);
  }

  public static void DestroyInstance()
  {
    StoryManager.storyTelemetry.Clear();
    StoryManager.Instance = (StoryManager) null;
  }

  public struct PopupInfo
  {
    public string Title;
    public string Description;
    public string CloseButtonText;
    public string CloseButtonToolTip;
    public string TextureName;
    public GameObject[] Minions;
    public bool DisplayImmediate;
    public EventInfoDataHelper.PopupType PopupType;
  }

  [SerializationConfig]
  public class StoryTelemetry : ISaveLoadable
  {
    public string StoryId;
    public string WorldId;
    [Serialize]
    public float Retrofitted = -1f;
    [Serialize]
    public float Discovered = -1f;
    [Serialize]
    public float InProgress = -1f;
    [Serialize]
    public float Completed = -1f;

    public void LogStateChange(StoryInstance.State state, float time)
    {
      switch (state)
      {
        case StoryInstance.State.RETROFITTED:
          this.Retrofitted = (double) this.Retrofitted >= 0.0 ? this.Retrofitted : time;
          break;
        case StoryInstance.State.DISCOVERED:
          this.Discovered = (double) this.Discovered >= 0.0 ? this.Discovered : time;
          break;
        case StoryInstance.State.IN_PROGRESS:
          this.InProgress = (double) this.InProgress >= 0.0 ? this.InProgress : time;
          break;
        case StoryInstance.State.COMPLETE:
          this.Completed = (double) this.Completed >= 0.0 ? this.Completed : time;
          break;
      }
    }
  }

  public class StoryCreationTelemetry
  {
    public string StoryId;
    public bool Enabled;
  }
}
