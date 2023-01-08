// Decompiled with JetBrains decompiler
// Type: StoryInstance
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using KSerialization;
using System;
using System.Collections.Generic;

[SerializationConfig]
public class StoryInstance : ISaveLoadable
{
  public Action<StoryInstance.State> StoryStateChanged;
  [Serialize]
  public readonly string storyId;
  [Serialize]
  public int worldId;
  [Serialize]
  private StoryInstance.State state;
  [Serialize]
  private StoryManager.StoryTelemetry telemetry;
  [Serialize]
  private HashSet<EventInfoDataHelper.PopupType> popupDisplayedStates = new HashSet<EventInfoDataHelper.PopupType>();
  private Story _story;

  public StoryInstance.State CurrentState
  {
    get => this.state;
    set
    {
      if (this.state == value)
        return;
      this.state = value;
      this.Telemetry.LogStateChange(this.state, GameClock.Instance.GetTimeInCycles());
      Action<StoryInstance.State> storyStateChanged = this.StoryStateChanged;
      if (storyStateChanged == null)
        return;
      storyStateChanged(this.state);
    }
  }

  public StoryManager.StoryTelemetry Telemetry
  {
    get
    {
      if (this.telemetry == null)
        this.telemetry = new StoryManager.StoryTelemetry();
      return this.telemetry;
    }
  }

  public EventInfoData EventInfo { get; private set; }

  public Notification Notification { get; private set; }

  public EventInfoDataHelper.PopupType PendingType { get; private set; } = EventInfoDataHelper.PopupType.NONE;

  public Story GetStory()
  {
    if (this._story == null)
      this._story = Db.Get().Stories.Get(this.storyId);
    return this._story;
  }

  public StoryInstance()
  {
  }

  public StoryInstance(Story story, int worldId)
  {
    this._story = story;
    this.storyId = story.Id;
    this.worldId = worldId;
  }

  public bool HasDisplayedPopup(EventInfoDataHelper.PopupType type) => this.popupDisplayedStates != null && this.popupDisplayedStates.Contains(type);

  public void SetPopupData(
    StoryManager.PopupInfo info,
    EventInfoData eventInfo,
    Notification notification = null)
  {
    this.EventInfo = eventInfo;
    this.Notification = notification;
    this.PendingType = info.PopupType;
    eventInfo.showCallback += new System.Action(this.OnPopupDisplayed);
    if (!info.DisplayImmediate)
      return;
    EventInfoScreen.ShowPopup(eventInfo);
  }

  private void OnPopupDisplayed()
  {
    if (this.popupDisplayedStates == null)
      this.popupDisplayedStates = new HashSet<EventInfoDataHelper.PopupType>();
    this.popupDisplayedStates.Add(this.PendingType);
    this.EventInfo = (EventInfoData) null;
    this.Notification = (Notification) null;
    this.PendingType = EventInfoDataHelper.PopupType.NONE;
  }

  public enum State
  {
    RETROFITTED = -1, // 0xFFFFFFFF
    NOT_STARTED = 0,
    DISCOVERED = 1,
    IN_PROGRESS = 2,
    COMPLETE = 3,
  }
}
