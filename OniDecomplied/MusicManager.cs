// Decompiled with JetBrains decompiler
// Type: MusicManager
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/MusicManager")]
public class MusicManager : KMonoBehaviour, ISerializationCallbackReceiver
{
  private const string VARIATION_ID = "variation";
  private const string INTERRUPTED_DIMMED_ID = "interrupted_dimmed";
  private const string MUSIC_KEY = "MusicInKey";
  private const float DYNAMIC_MUSIC_SCHEDULE_DELAY = 16000f;
  private const float DYNAMIC_MUSIC_SCHEDULE_LOOKAHEAD = 48000f;
  [Header("Song Lists")]
  [Tooltip("Play during the daytime. The mix of the song is affected by the player's input, like pausing the sim, activating an overlay, or zooming in and out.")]
  [SerializeField]
  private MusicManager.DynamicSong[] fullSongs;
  [Tooltip("Simple dynamic songs which are more ambient in nature, which play quietly during \"non-music\" days. These are affected by Pause and OverlayActive.")]
  [SerializeField]
  private MusicManager.Minisong[] miniSongs;
  [Tooltip("Triggered by in-game events, such as completing research or night-time falling. They will temporarily interrupt a dynamicSong, fading the dynamicSong back in after the stinger is complete.")]
  [SerializeField]
  private MusicManager.Stinger[] stingers;
  [Tooltip("Generally songs that don't play during gameplay, while a menu is open. For example, the ESC menu or the Starmap.")]
  [SerializeField]
  private MusicManager.MenuSong[] menuSongs;
  private Dictionary<string, MusicManager.SongInfo> songMap = new Dictionary<string, MusicManager.SongInfo>();
  public Dictionary<string, MusicManager.SongInfo> activeSongs = new Dictionary<string, MusicManager.SongInfo>();
  [Space]
  [Header("Tuning Values")]
  [Tooltip("Just before night-time (88%), dynamic music fades out. At which point of the day should the music fade?")]
  [SerializeField]
  private float duskTimePercentage = 85f;
  [Tooltip("If we load into a save and the day is almost over, we shouldn't play music because it will stop soon anyway. At what point of the day should we not play music?")]
  [SerializeField]
  private float loadGameCutoffPercentage = 50f;
  [Tooltip("When dynamic music is active, we play a snapshot which attenuates the ambience and SFX. What intensity should that snapshot be applied?")]
  [SerializeField]
  private float dynamicMusicSFXAttenuationPercentage = 65f;
  [Tooltip("When mini songs are active, we play a snapshot which attenuates the ambience and SFX. What intensity should that snapshot be applied?")]
  [SerializeField]
  private float miniSongSFXAttenuationPercentage;
  [SerializeField]
  private MusicManager.TypeOfMusic[] musicStyleOrder;
  [NonSerialized]
  public bool alwaysPlayMusic;
  private MusicManager.DynamicSongPlaylist fullSongPlaylist = new MusicManager.DynamicSongPlaylist();
  private MusicManager.DynamicSongPlaylist miniSongPlaylist = new MusicManager.DynamicSongPlaylist();
  [NonSerialized]
  public MusicManager.SongInfo activeDynamicSong;
  [NonSerialized]
  public MusicManager.DynamicSongPlaylist activePlaylist;
  private MusicManager.TypeOfMusic nextMusicType;
  private int musicTypeIterator;
  private float time;
  private float timeOfDayUpdateRate = 2f;
  private static MusicManager _instance;
  [NonSerialized]
  public List<string> MusicDebugLog = new List<string>();

  public Dictionary<string, MusicManager.SongInfo> SongMap => this.songMap;

  public void PlaySong(string song_name, bool canWait = false)
  {
    this.Log("Play: " + song_name);
    if (!AudioDebug.Get().musicEnabled)
      return;
    MusicManager.SongInfo songInfo = (MusicManager.SongInfo) null;
    if (!this.songMap.TryGetValue(song_name, out songInfo))
      DebugUtil.LogErrorArgs(new object[2]
      {
        (object) "Unknown song:",
        (object) song_name
      });
    else if (this.activeSongs.ContainsKey(song_name))
      DebugUtil.LogWarningArgs(new object[2]
      {
        (object) "Trying to play duplicate song:",
        (object) song_name
      });
    else if (this.activeSongs.Count == 0)
    {
      songInfo.ev = KFMOD.CreateInstance(songInfo.fmodEvent);
      if (!((EventInstance) ref songInfo.ev).isValid())
        DebugUtil.LogWarningArgs(new object[1]
        {
          (object) ("Failed to find FMOD event [" + songInfo.fmodEvent.ToString() + "]")
        });
      int num = songInfo.numberOfVariations > 0 ? Random.Range(1, songInfo.numberOfVariations + 1) : -1;
      if (num != -1)
        ((EventInstance) ref songInfo.ev).setParameterByName("variation", (float) num, false);
      if (songInfo.dynamic)
      {
        ((EventInstance) ref songInfo.ev).setProperty((EVENT_PROPERTY) 1, 16000f);
        ((EventInstance) ref songInfo.ev).setProperty((EVENT_PROPERTY) 2, 48000f);
        this.activeDynamicSong = songInfo;
      }
      ((EventInstance) ref songInfo.ev).start();
      this.activeSongs[song_name] = songInfo;
    }
    else
    {
      List<string> stringList = new List<string>((IEnumerable<string>) this.activeSongs.Keys);
      if (songInfo.interruptsActiveMusic)
      {
        for (int index = 0; index < stringList.Count; ++index)
        {
          if (!this.activeSongs[stringList[index]].interruptsActiveMusic)
          {
            MusicManager.SongInfo activeSong = this.activeSongs[stringList[index]];
            ((EventInstance) ref activeSong.ev).setParameterByName("interrupted_dimmed", 1f, false);
            this.Log("Dimming: " + Assets.GetSimpleSoundEventName(activeSong.fmodEvent));
            songInfo.songsOnHold.Add(stringList[index]);
          }
        }
        songInfo.ev = KFMOD.CreateInstance(songInfo.fmodEvent);
        if (!((EventInstance) ref songInfo.ev).isValid())
          DebugUtil.LogWarningArgs(new object[1]
          {
            (object) ("Failed to find FMOD event [" + songInfo.fmodEvent.ToString() + "]")
          });
        ((EventInstance) ref songInfo.ev).start();
        ((EventInstance) ref songInfo.ev).release();
        this.activeSongs[song_name] = songInfo;
      }
      else
      {
        int num1 = 0;
        foreach (string key in this.activeSongs.Keys)
        {
          MusicManager.SongInfo activeSong = this.activeSongs[key];
          if (!activeSong.interruptsActiveMusic && activeSong.priority > num1)
            num1 = activeSong.priority;
        }
        if (songInfo.priority < num1)
          return;
        for (int index = 0; index < stringList.Count; ++index)
        {
          MusicManager.SongInfo activeSong = this.activeSongs[stringList[index]];
          EventInstance ev = activeSong.ev;
          if (!activeSong.interruptsActiveMusic)
          {
            ((EventInstance) ref ev).setParameterByName("interrupted_dimmed", 1f, false);
            ((EventInstance) ref ev).stop((STOP_MODE) 0);
            this.activeSongs.Remove(stringList[index]);
            stringList.Remove(stringList[index]);
          }
        }
        songInfo.ev = KFMOD.CreateInstance(songInfo.fmodEvent);
        if (!((EventInstance) ref songInfo.ev).isValid())
          DebugUtil.LogWarningArgs(new object[1]
          {
            (object) ("Failed to find FMOD event [" + songInfo.fmodEvent.ToString() + "]")
          });
        int num2 = songInfo.numberOfVariations > 0 ? Random.Range(1, songInfo.numberOfVariations + 1) : -1;
        if (num2 != -1)
          ((EventInstance) ref songInfo.ev).setParameterByName("variation", (float) num2, false);
        ((EventInstance) ref songInfo.ev).start();
        this.activeSongs[song_name] = songInfo;
      }
    }
  }

  public void StopSong(string song_name, bool shouldLog = true, STOP_MODE stopMode = 0)
  {
    if (shouldLog)
      this.Log("Stop: " + song_name);
    MusicManager.SongInfo songInfo1 = (MusicManager.SongInfo) null;
    if (!this.songMap.TryGetValue(song_name, out songInfo1))
      DebugUtil.LogErrorArgs(new object[2]
      {
        (object) "Unknown song:",
        (object) song_name
      });
    else if (!this.activeSongs.ContainsKey(song_name))
    {
      DebugUtil.LogWarningArgs(new object[2]
      {
        (object) "Trying to stop a song that isn't playing:",
        (object) song_name
      });
    }
    else
    {
      EventInstance ev1 = songInfo1.ev;
      ((EventInstance) ref ev1).stop(stopMode);
      ((EventInstance) ref ev1).release();
      if (songInfo1.dynamic)
        this.activeDynamicSong = (MusicManager.SongInfo) null;
      if (songInfo1.songsOnHold.Count > 0)
      {
        for (int index = 0; index < songInfo1.songsOnHold.Count; ++index)
        {
          MusicManager.SongInfo songInfo2;
          if (this.activeSongs.TryGetValue(songInfo1.songsOnHold[index], out songInfo2) && ((EventInstance) ref songInfo2.ev).isValid())
          {
            EventInstance ev2 = songInfo2.ev;
            this.Log("Undimming: " + Assets.GetSimpleSoundEventName(songInfo2.fmodEvent));
            ((EventInstance) ref ev2).setParameterByName("interrupted_dimmed", 0.0f, false);
            songInfo1.songsOnHold.Remove(songInfo1.songsOnHold[index]);
          }
          else
            songInfo1.songsOnHold.Remove(songInfo1.songsOnHold[index]);
        }
      }
      this.activeSongs.Remove(song_name);
    }
  }

  public void KillAllSongs(STOP_MODE stop_mode = 1)
  {
    this.Log("Kill All Songs");
    if (this.DynamicMusicIsActive())
      this.StopDynamicMusic(true);
    List<string> stringList = new List<string>((IEnumerable<string>) this.activeSongs.Keys);
    for (int index = 0; index < stringList.Count; ++index)
      this.StopSong(stringList[index]);
  }

  public void SetSongParameter(
    string song_name,
    string parameter_name,
    float parameter_value,
    bool shouldLog = true)
  {
    if (shouldLog)
      this.Log(string.Format("Set Param {0}: {1}, {2}", (object) song_name, (object) parameter_name, (object) parameter_value));
    MusicManager.SongInfo songInfo = (MusicManager.SongInfo) null;
    if (!this.activeSongs.TryGetValue(song_name, out songInfo))
      return;
    EventInstance ev = songInfo.ev;
    if (!((EventInstance) ref ev).isValid())
      return;
    ((EventInstance) ref ev).setParameterByName(parameter_name, parameter_value, false);
  }

  public bool SongIsPlaying(string song_name)
  {
    MusicManager.SongInfo songInfo = (MusicManager.SongInfo) null;
    return this.activeSongs.TryGetValue(song_name, out songInfo) && songInfo.musicPlaybackState != 2;
  }

  private void Update()
  {
    this.ClearFinishedSongs();
    if (!this.DynamicMusicIsActive())
      return;
    this.SetDynamicMusicZoomLevel();
    this.SetDynamicMusicTimeSinceLastJob();
    if (this.activeDynamicSong.useTimeOfDay)
      this.SetDynamicMusicTimeOfDay();
    if (!Object.op_Inequality((Object) GameClock.Instance, (Object) null) || (double) GameClock.Instance.GetCurrentCycleAsPercentage() < (double) this.duskTimePercentage / 100.0)
      return;
    this.StopDynamicMusic();
  }

  private void ClearFinishedSongs()
  {
    if (this.activeSongs.Count <= 0)
      return;
    ListPool<string, MusicManager>.PooledList pooledList = ListPool<string, MusicManager>.Allocate();
    foreach (KeyValuePair<string, MusicManager.SongInfo> activeSong in this.activeSongs)
    {
      MusicManager.SongInfo songInfo = activeSong.Value;
      EventInstance ev = songInfo.ev;
      ((EventInstance) ref ev).getPlaybackState(ref songInfo.musicPlaybackState);
      if (songInfo.musicPlaybackState == 2 || songInfo.musicPlaybackState == 4)
      {
        ((List<string>) pooledList).Add(activeSong.Key);
        foreach (string song_name in songInfo.songsOnHold)
          this.SetSongParameter(song_name, "interrupted_dimmed", 0.0f);
        songInfo.songsOnHold.Clear();
      }
    }
    foreach (string key in (List<string>) pooledList)
      this.activeSongs.Remove(key);
    pooledList.Recycle();
  }

  public void OnEscapeMenu(bool paused)
  {
    foreach (KeyValuePair<string, MusicManager.SongInfo> activeSong in this.activeSongs)
    {
      if (activeSong.Value != null)
        this.StartFadeToPause(activeSong.Value.ev, paused);
    }
  }

  public void StartFadeToPause(EventInstance inst, bool paused, float fadeTime = 0.25f)
  {
    if (paused)
      ((MonoBehaviour) this).StartCoroutine(this.FadeToPause(inst, fadeTime));
    else
      ((MonoBehaviour) this).StartCoroutine(this.FadeToUnpause(inst, fadeTime));
  }

  private IEnumerator FadeToPause(EventInstance inst, float fadeTime)
  {
    float startVolume;
    float targetVolume;
    ((EventInstance) ref inst).getVolume(ref startVolume, ref targetVolume);
    targetVolume = 0.0f;
    float lerpTime = 0.0f;
    while ((double) lerpTime < 1.0)
    {
      lerpTime += Time.unscaledDeltaTime / fadeTime;
      ((EventInstance) ref inst).setVolume(Mathf.Lerp(startVolume, targetVolume, lerpTime));
      yield return (object) null;
    }
    ((EventInstance) ref inst).setPaused(true);
  }

  private IEnumerator FadeToUnpause(EventInstance inst, float fadeTime)
  {
    float startVolume;
    float targetVolume;
    ((EventInstance) ref inst).getVolume(ref startVolume, ref targetVolume);
    targetVolume = 1f;
    float lerpTime = 0.0f;
    ((EventInstance) ref inst).setPaused(false);
    while ((double) lerpTime < 1.0)
    {
      lerpTime += Time.unscaledDeltaTime / fadeTime;
      ((EventInstance) ref inst).setVolume(Mathf.Lerp(startVolume, targetVolume, lerpTime));
      yield return (object) null;
    }
  }

  public void PlayDynamicMusic()
  {
    if (this.DynamicMusicIsActive())
    {
      this.Log("Trying to play DynamicMusic when it is already playing.");
    }
    else
    {
      string nextDynamicSong = this.GetNextDynamicSong();
      if (nextDynamicSong == "NONE")
        return;
      this.PlaySong(nextDynamicSong);
      MusicManager.SongInfo songInfo;
      if (this.activeSongs.TryGetValue(nextDynamicSong, out songInfo))
      {
        this.activeDynamicSong = songInfo;
        AudioMixer.instance.Start(AudioMixerSnapshots.Get().DynamicMusicPlayingSnapshot);
        if (Object.op_Inequality((Object) SpeedControlScreen.Instance, (Object) null) && SpeedControlScreen.Instance.IsPaused)
          this.SetDynamicMusicPaused();
        if (Object.op_Inequality((Object) OverlayScreen.Instance, (Object) null) && HashedString.op_Inequality(OverlayScreen.Instance.mode, OverlayModes.None.ID))
          this.SetDynamicMusicOverlayActive();
        this.SetDynamicMusicPlayHook();
        this.SetDynamicMusicKeySigniture();
        string str = "Volume_Music";
        if (KPlayerPrefs.HasKey(str))
        {
          float parameter_value = KPlayerPrefs.GetFloat(str);
          AudioMixer.instance.SetSnapshotParameter(AudioMixerSnapshots.Get().DynamicMusicPlayingSnapshot, "userVolume_Music", parameter_value);
        }
        AudioMixer.instance.SetSnapshotParameter(AudioMixerSnapshots.Get().DynamicMusicPlayingSnapshot, "intensity", songInfo.sfxAttenuationPercentage / 100f);
      }
      else
      {
        this.Log("DynamicMusic song " + nextDynamicSong + " did not start.");
        string str = "";
        foreach (KeyValuePair<string, MusicManager.SongInfo> activeSong in this.activeSongs)
        {
          str = str + activeSong.Key + ", ";
          Debug.Log((object) str);
        }
        DebugUtil.DevAssert(false, "Song failed to play: " + nextDynamicSong, (Object) null);
      }
    }
  }

  public void StopDynamicMusic(bool stopImmediate = false)
  {
    if (this.activeDynamicSong == null)
      return;
    STOP_MODE stopMode = stopImmediate ? (STOP_MODE) 1 : (STOP_MODE) 0;
    this.Log("Stop DynamicMusic: " + Assets.GetSimpleSoundEventName(this.activeDynamicSong.fmodEvent));
    this.StopSong(Assets.GetSimpleSoundEventName(this.activeDynamicSong.fmodEvent), stopMode: stopMode);
    this.activeDynamicSong = (MusicManager.SongInfo) null;
    AudioMixer.instance.Stop(AudioMixerSnapshots.Get().DynamicMusicPlayingSnapshot);
  }

  public string GetNextDynamicSong()
  {
    string nextDynamicSong = "";
    if (this.alwaysPlayMusic && this.nextMusicType == MusicManager.TypeOfMusic.None)
    {
      while (this.nextMusicType == MusicManager.TypeOfMusic.None)
        this.CycleToNextMusicType();
    }
    switch (this.nextMusicType)
    {
      case MusicManager.TypeOfMusic.DynamicSong:
        nextDynamicSong = this.fullSongPlaylist.GetNextSong();
        this.activePlaylist = this.fullSongPlaylist;
        break;
      case MusicManager.TypeOfMusic.MiniSong:
        nextDynamicSong = this.miniSongPlaylist.GetNextSong();
        this.activePlaylist = this.miniSongPlaylist;
        break;
      case MusicManager.TypeOfMusic.None:
        nextDynamicSong = "NONE";
        this.activePlaylist = (MusicManager.DynamicSongPlaylist) null;
        break;
    }
    this.CycleToNextMusicType();
    return nextDynamicSong;
  }

  private void CycleToNextMusicType()
  {
    this.musicTypeIterator = ++this.musicTypeIterator % this.musicStyleOrder.Length;
    this.nextMusicType = this.musicStyleOrder[this.musicTypeIterator];
  }

  public bool DynamicMusicIsActive() => this.activeDynamicSong != null;

  public void SetDynamicMusicPaused()
  {
    if (!this.DynamicMusicIsActive())
      return;
    this.SetSongParameter(Assets.GetSimpleSoundEventName(this.activeDynamicSong.fmodEvent), "Paused", 1f);
  }

  public void SetDynamicMusicUnpaused()
  {
    if (!this.DynamicMusicIsActive())
      return;
    this.SetSongParameter(Assets.GetSimpleSoundEventName(this.activeDynamicSong.fmodEvent), "Paused", 0.0f);
  }

  public void SetDynamicMusicZoomLevel()
  {
    if (!Object.op_Inequality((Object) CameraController.Instance, (Object) null))
      return;
    float parameter_value = (float) (100.0 - (double) Camera.main.orthographicSize / 20.0 * 100.0);
    this.SetSongParameter(Assets.GetSimpleSoundEventName(this.activeDynamicSong.fmodEvent), "zoomPercentage", parameter_value, false);
  }

  public void SetDynamicMusicTimeSinceLastJob() => this.SetSongParameter(Assets.GetSimpleSoundEventName(this.activeDynamicSong.fmodEvent), "secsSinceNewJob", Time.time - Game.Instance.LastTimeWorkStarted, false);

  public void SetDynamicMusicTimeOfDay()
  {
    if ((double) this.time >= (double) this.timeOfDayUpdateRate)
    {
      this.SetSongParameter(Assets.GetSimpleSoundEventName(this.activeDynamicSong.fmodEvent), "timeOfDay", GameClock.Instance.GetCurrentCycleAsPercentage(), false);
      this.time = 0.0f;
    }
    this.time += Time.deltaTime;
  }

  public void SetDynamicMusicOverlayActive()
  {
    if (!this.DynamicMusicIsActive())
      return;
    this.SetSongParameter(Assets.GetSimpleSoundEventName(this.activeDynamicSong.fmodEvent), "overlayActive", 1f);
  }

  public void SetDynamicMusicOverlayInactive()
  {
    if (!this.DynamicMusicIsActive())
      return;
    this.SetSongParameter(Assets.GetSimpleSoundEventName(this.activeDynamicSong.fmodEvent), "overlayActive", 0.0f);
  }

  public void SetDynamicMusicPlayHook()
  {
    if (!this.DynamicMusicIsActive())
      return;
    string simpleSoundEventName = Assets.GetSimpleSoundEventName(this.activeDynamicSong.fmodEvent);
    this.SetSongParameter(simpleSoundEventName, "playHook", this.activeDynamicSong.playHook ? 1f : 0.0f);
    this.activePlaylist.songMap[simpleSoundEventName].playHook = !this.activePlaylist.songMap[simpleSoundEventName].playHook;
  }

  public bool ShouldPlayDynamicMusicLoadedGame() => (double) GameClock.Instance.GetCurrentCycleAsPercentage() <= (double) this.loadGameCutoffPercentage / 100.0;

  public void SetDynamicMusicKeySigniture()
  {
    if (!this.DynamicMusicIsActive())
      return;
    float num;
    switch (this.activePlaylist.songMap[Assets.GetSimpleSoundEventName(this.activeDynamicSong.fmodEvent)].musicKeySigniture)
    {
      case "Ab":
        num = 0.0f;
        break;
      case "Bb":
        num = 1f;
        break;
      case "C":
        num = 2f;
        break;
      case "D":
        num = 3f;
        break;
      default:
        num = 2f;
        break;
    }
    FMOD.Studio.System studioSystem = RuntimeManager.StudioSystem;
    ((FMOD.Studio.System) ref studioSystem).setParameterByName("MusicInKey", num, false);
  }

  public static MusicManager instance => MusicManager._instance;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (!RuntimeManager.IsInitialized)
    {
      ((Behaviour) this).enabled = false;
    }
    else
    {
      if (!KPlayerPrefs.HasKey(AudioOptionsScreen.AlwaysPlayMusicKey))
        return;
      this.alwaysPlayMusic = KPlayerPrefs.GetInt(AudioOptionsScreen.AlwaysPlayMusicKey) == 1;
    }
  }

  protected virtual void OnPrefabInit()
  {
    MusicManager._instance = this;
    this.ConfigureSongs();
    this.fullSongPlaylist.ResetUnplayedSongs();
    this.miniSongPlaylist.ResetUnplayedSongs();
    this.nextMusicType = this.musicStyleOrder[this.musicTypeIterator];
  }

  protected virtual void OnCleanUp() => MusicManager._instance = (MusicManager) null;

  [ContextMenu("Reload")]
  private void ConfigureSongs()
  {
    this.songMap.Clear();
    foreach (MusicManager.DynamicSong fullSong in this.fullSongs)
    {
      if (DlcManager.IsContentActive(fullSong.requiredDlcId))
      {
        string simpleSoundEventName = Assets.GetSimpleSoundEventName(fullSong.fmodEvent);
        MusicManager.SongInfo songInfo = new MusicManager.SongInfo();
        songInfo.fmodEvent = fullSong.fmodEvent;
        songInfo.requiredDlcId = fullSong.requiredDlcId;
        songInfo.priority = 100;
        songInfo.interruptsActiveMusic = false;
        songInfo.dynamic = true;
        songInfo.useTimeOfDay = fullSong.useTimeOfDay;
        songInfo.numberOfVariations = fullSong.numberOfVariations;
        songInfo.musicKeySigniture = fullSong.musicKeySigniture;
        songInfo.sfxAttenuationPercentage = this.dynamicMusicSFXAttenuationPercentage;
        this.songMap[simpleSoundEventName] = songInfo;
        this.fullSongPlaylist.songMap[simpleSoundEventName] = songInfo;
      }
    }
    foreach (MusicManager.Minisong miniSong in this.miniSongs)
    {
      if (DlcManager.IsContentActive(miniSong.requiredDlcId))
      {
        string simpleSoundEventName = Assets.GetSimpleSoundEventName(miniSong.fmodEvent);
        MusicManager.SongInfo songInfo = new MusicManager.SongInfo();
        songInfo.fmodEvent = miniSong.fmodEvent;
        songInfo.requiredDlcId = miniSong.requiredDlcId;
        songInfo.priority = 100;
        songInfo.interruptsActiveMusic = false;
        songInfo.dynamic = true;
        songInfo.useTimeOfDay = false;
        songInfo.numberOfVariations = 5;
        songInfo.musicKeySigniture = miniSong.musicKeySigniture;
        songInfo.sfxAttenuationPercentage = this.miniSongSFXAttenuationPercentage;
        this.songMap[simpleSoundEventName] = songInfo;
        this.miniSongPlaylist.songMap[simpleSoundEventName] = songInfo;
      }
    }
    foreach (MusicManager.Stinger stinger in this.stingers)
    {
      if (DlcManager.IsContentActive(stinger.requiredDlcId))
        this.SongMap[Assets.GetSimpleSoundEventName(stinger.fmodEvent)] = new MusicManager.SongInfo()
        {
          fmodEvent = stinger.fmodEvent,
          priority = 100,
          interruptsActiveMusic = true,
          dynamic = false,
          useTimeOfDay = false,
          numberOfVariations = 0,
          requiredDlcId = stinger.requiredDlcId
        };
    }
    foreach (MusicManager.MenuSong menuSong in this.menuSongs)
    {
      if (DlcManager.IsContentActive(menuSong.requiredDlcId))
        this.SongMap[Assets.GetSimpleSoundEventName(menuSong.fmodEvent)] = new MusicManager.SongInfo()
        {
          fmodEvent = menuSong.fmodEvent,
          priority = 100,
          interruptsActiveMusic = true,
          dynamic = false,
          useTimeOfDay = false,
          numberOfVariations = 0,
          requiredDlcId = menuSong.requiredDlcId
        };
    }
  }

  public void OnBeforeSerialize()
  {
  }

  public void OnAfterDeserialize()
  {
  }

  private void Log(string s)
  {
  }

  [DebuggerDisplay("{fmodEvent}")]
  [Serializable]
  public class SongInfo
  {
    public EventReference fmodEvent;
    [NonSerialized]
    public int priority;
    [NonSerialized]
    public bool interruptsActiveMusic;
    [NonSerialized]
    public bool dynamic;
    [NonSerialized]
    public string requiredDlcId = "";
    [NonSerialized]
    public bool useTimeOfDay;
    [NonSerialized]
    public int numberOfVariations;
    [NonSerialized]
    public string musicKeySigniture = "C";
    [NonSerialized]
    public EventInstance ev;
    [NonSerialized]
    public List<string> songsOnHold = new List<string>();
    [NonSerialized]
    public PLAYBACK_STATE musicPlaybackState;
    [NonSerialized]
    public bool playHook = true;
    [NonSerialized]
    public float sfxAttenuationPercentage = 65f;
  }

  [DebuggerDisplay("{fmodEvent}")]
  [Serializable]
  public class DynamicSong
  {
    public EventReference fmodEvent;
    [Tooltip("Some songs are set up to have Morning, Daytime, Hook, and Intro sections. Toggle this ON if this song has those sections.")]
    [SerializeField]
    public bool useTimeOfDay;
    [Tooltip("Some songs have different possible start locations. Enter how many start locations this song is set up to support.")]
    [SerializeField]
    public int numberOfVariations;
    [Tooltip("Some songs have different key signitures. Enter the key this music is in.")]
    [SerializeField]
    public string musicKeySigniture = "";
    [Tooltip("Should playback of this song be limited to an active DLC?")]
    [SerializeField]
    public string requiredDlcId = "";
  }

  [DebuggerDisplay("{fmodEvent}")]
  [Serializable]
  public class Stinger
  {
    public EventReference fmodEvent;
    [Tooltip("Should playback of this song be limited to an active DLC?")]
    [SerializeField]
    public string requiredDlcId = "";
  }

  [DebuggerDisplay("{fmodEvent}")]
  [Serializable]
  public class MenuSong
  {
    public EventReference fmodEvent;
    [Tooltip("Should playback of this song be limited to an active DLC?")]
    [SerializeField]
    public string requiredDlcId = "";
  }

  [DebuggerDisplay("{fmodEvent}")]
  [Serializable]
  public class Minisong
  {
    public EventReference fmodEvent;
    [Tooltip("Some songs have different key signitures. Enter the key this music is in.")]
    [SerializeField]
    public string musicKeySigniture = "";
    [Tooltip("Should playback of this song be limited to an active DLC?")]
    [SerializeField]
    public string requiredDlcId = "";
  }

  public enum TypeOfMusic
  {
    DynamicSong,
    MiniSong,
    None,
  }

  public class DynamicSongPlaylist
  {
    public Dictionary<string, MusicManager.SongInfo> songMap = new Dictionary<string, MusicManager.SongInfo>();
    public List<string> unplayedSongs = new List<string>();
    private string lastSongPlayed = "";

    public string GetNextSong()
    {
      string unplayedSong;
      if (this.unplayedSongs.Count > 0)
      {
        int index = Random.Range(0, this.unplayedSongs.Count);
        unplayedSong = this.unplayedSongs[index];
        this.unplayedSongs.RemoveAt(index);
      }
      else
      {
        this.ResetUnplayedSongs();
        bool flag = this.unplayedSongs.Count > 1;
        if (flag)
        {
          for (int index = 0; index < this.unplayedSongs.Count; ++index)
          {
            if (this.unplayedSongs[index] == this.lastSongPlayed)
            {
              this.unplayedSongs.Remove(this.unplayedSongs[index]);
              break;
            }
          }
        }
        int index1 = Random.Range(0, this.unplayedSongs.Count);
        unplayedSong = this.unplayedSongs[index1];
        this.unplayedSongs.RemoveAt(index1);
        if (flag)
          this.unplayedSongs.Add(this.lastSongPlayed);
      }
      this.lastSongPlayed = unplayedSong;
      Debug.Assert(this.songMap.ContainsKey(unplayedSong), (object) ("Missing song " + unplayedSong));
      return Assets.GetSimpleSoundEventName(this.songMap[unplayedSong].fmodEvent);
    }

    public void ResetUnplayedSongs()
    {
      this.unplayedSongs.Clear();
      foreach (KeyValuePair<string, MusicManager.SongInfo> song in this.songMap)
      {
        if (DlcManager.IsContentActive(song.Value.requiredDlcId))
          this.unplayedSongs.Add(song.Key);
      }
    }
  }
}
