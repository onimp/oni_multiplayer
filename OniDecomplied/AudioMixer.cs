// Decompiled with JetBrains decompiler
// Type: AudioMixer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using FMODUnity;
using System.Collections.Generic;
using UnityEngine;

public class AudioMixer
{
  private static AudioMixer _instance = (AudioMixer) null;
  private const string DUPLICANT_COUNT_ID = "duplicantCount";
  private const string PULSE_ID = "Pulse";
  private const string SNAPSHOT_ACTIVE_ID = "snapshotActive";
  private const string SPACE_VISIBLE_ID = "spaceVisible";
  private const string FACILITY_VISIBLE_ID = "facilityVisible";
  private const string FOCUS_BUS_PATH = "bus:/SFX/Focus";
  public Dictionary<HashedString, EventInstance> activeSnapshots = new Dictionary<HashedString, EventInstance>();
  public List<HashedString> SnapshotDebugLog = new List<HashedString>();
  public bool activeNIS;
  public static float LOW_PRIORITY_CUTOFF_DISTANCE = 10f;
  public static float PULSE_SNAPSHOT_BPM = 120f;
  public static int VISIBLE_DUPLICANTS_BEFORE_ATTENUATION = 2;
  private EventInstance duplicantCountInst;
  private EventInstance pulseInst;
  private EventInstance duplicantCountMovingInst;
  private EventInstance duplicantCountSleepingInst;
  private EventInstance spaceVisibleInst;
  private EventInstance facilityVisibleInst;
  private static readonly HashedString UserVolumeSettingsHash = new HashedString("event:/Snapshots/Mixing/Snapshot_UserVolumeSettings");
  public bool persistentSnapshotsActive;
  private Dictionary<string, int> visibleDupes = new Dictionary<string, int>();
  public Dictionary<string, AudioMixer.UserVolumeBus> userVolumeSettings = new Dictionary<string, AudioMixer.UserVolumeBus>();

  public static AudioMixer instance => AudioMixer._instance;

  public static AudioMixer Create()
  {
    AudioMixer._instance = new AudioMixer();
    AudioMixerSnapshots audioMixerSnapshots = AudioMixerSnapshots.Get();
    if (Object.op_Inequality((Object) audioMixerSnapshots, (Object) null))
      audioMixerSnapshots.ReloadSnapshots();
    return AudioMixer._instance;
  }

  public static void Destroy()
  {
    AudioMixer._instance.StopAll();
    AudioMixer._instance = (AudioMixer) null;
  }

  public EventInstance Start(EventReference event_ref)
  {
    EventDescription eventDescription = RuntimeManager.GetEventDescription(event_ref.Guid);
    string snapshot;
    ((EventDescription) ref eventDescription).getPath(ref snapshot);
    return this.Start(snapshot);
  }

  public EventInstance Start(string snapshot)
  {
    EventInstance eventInstance;
    if (!this.activeSnapshots.TryGetValue(HashedString.op_Implicit(snapshot), out eventInstance))
    {
      if (RuntimeManager.IsInitialized)
      {
        eventInstance = KFMOD.CreateInstance(snapshot);
        this.activeSnapshots[HashedString.op_Implicit(snapshot)] = eventInstance;
        ((EventInstance) ref eventInstance).start();
        ((EventInstance) ref eventInstance).setParameterByName("snapshotActive", 1f, false);
      }
      else
        eventInstance = new EventInstance();
    }
    AudioMixer.instance.Log("Start Snapshot: " + snapshot);
    return eventInstance;
  }

  public bool Stop(EventReference event_ref, STOP_MODE stop_mode = 0)
  {
    EventDescription eventDescription = RuntimeManager.GetEventDescription(event_ref.Guid);
    string str;
    ((EventDescription) ref eventDescription).getPath(ref str);
    return this.Stop(HashedString.op_Implicit(str), stop_mode);
  }

  public bool Stop(HashedString snapshot, STOP_MODE stop_mode = 0)
  {
    bool flag = false;
    EventInstance eventInstance;
    if (this.activeSnapshots.TryGetValue(snapshot, out eventInstance))
    {
      ((EventInstance) ref eventInstance).setParameterByName("snapshotActive", 0.0f, false);
      ((EventInstance) ref eventInstance).stop(stop_mode);
      ((EventInstance) ref eventInstance).release();
      this.activeSnapshots.Remove(snapshot);
      flag = true;
      AudioMixer.instance.Log("Stop Snapshot: [" + snapshot.ToString() + "] with fadeout mode: [" + stop_mode.ToString() + "]");
    }
    else
      AudioMixer.instance.Log("Tried to stop snapshot: [" + snapshot.ToString() + "] but it wasn't active.");
    return flag;
  }

  public void Reset() => this.StopAll();

  public void StopAll(STOP_MODE stop_mode = 1)
  {
    List<HashedString> hashedStringList = new List<HashedString>();
    foreach (KeyValuePair<HashedString, EventInstance> activeSnapshot in this.activeSnapshots)
    {
      if (HashedString.op_Inequality(activeSnapshot.Key, AudioMixer.UserVolumeSettingsHash))
        hashedStringList.Add(activeSnapshot.Key);
    }
    for (int index = 0; index < hashedStringList.Count; ++index)
      this.Stop(hashedStringList[index], stop_mode);
  }

  public bool SnapshotIsActive(EventReference event_ref)
  {
    EventDescription eventDescription = RuntimeManager.GetEventDescription(event_ref.Guid);
    string str;
    ((EventDescription) ref eventDescription).getPath(ref str);
    return this.SnapshotIsActive(HashedString.op_Implicit(str));
  }

  public bool SnapshotIsActive(HashedString snapshot_name) => this.activeSnapshots.ContainsKey(snapshot_name);

  public void SetSnapshotParameter(
    EventReference event_ref,
    string parameter_name,
    float parameter_value,
    bool shouldLog = true)
  {
    EventDescription eventDescription = RuntimeManager.GetEventDescription(event_ref.Guid);
    string snapshot_name;
    ((EventDescription) ref eventDescription).getPath(ref snapshot_name);
    this.SetSnapshotParameter(snapshot_name, parameter_name, parameter_value, shouldLog);
  }

  public void SetSnapshotParameter(
    string snapshot_name,
    string parameter_name,
    float parameter_value,
    bool shouldLog = true)
  {
    if (shouldLog)
      this.Log(string.Format("Set Param {0}: {1}, {2}", (object) snapshot_name, (object) parameter_name, (object) parameter_value));
    EventInstance eventInstance;
    if (this.activeSnapshots.TryGetValue(HashedString.op_Implicit(snapshot_name), out eventInstance))
      ((EventInstance) ref eventInstance).setParameterByName(parameter_name, parameter_value, false);
    else
      this.Log("Tried to set [" + parameter_name + "] to [" + parameter_value.ToString() + "] but [" + snapshot_name + "] is not active.");
  }

  public void StartPersistentSnapshots()
  {
    this.persistentSnapshotsActive = true;
    this.Start(AudioMixerSnapshots.Get().DuplicantCountAttenuatorMigrated);
    this.Start(AudioMixerSnapshots.Get().DuplicantCountMovingSnapshot);
    this.Start(AudioMixerSnapshots.Get().DuplicantCountSleepingSnapshot);
    this.spaceVisibleInst = this.Start(AudioMixerSnapshots.Get().SpaceVisibleSnapshot);
    this.facilityVisibleInst = this.Start(AudioMixerSnapshots.Get().FacilityVisibleSnapshot);
    this.Start(AudioMixerSnapshots.Get().PulseSnapshot);
  }

  public void StopPersistentSnapshots()
  {
    this.persistentSnapshotsActive = false;
    this.Stop(AudioMixerSnapshots.Get().DuplicantCountAttenuatorMigrated);
    this.Stop(AudioMixerSnapshots.Get().DuplicantCountMovingSnapshot);
    this.Stop(AudioMixerSnapshots.Get().DuplicantCountSleepingSnapshot);
    this.Stop(AudioMixerSnapshots.Get().SpaceVisibleSnapshot);
    this.Stop(AudioMixerSnapshots.Get().FacilityVisibleSnapshot);
    this.Stop(AudioMixerSnapshots.Get().PulseSnapshot);
  }

  private string GetSnapshotName(EventReference event_ref)
  {
    EventDescription eventDescription = RuntimeManager.GetEventDescription(event_ref.Guid);
    string snapshotName;
    ((EventDescription) ref eventDescription).getPath(ref snapshotName);
    return snapshotName;
  }

  public void UpdatePersistentSnapshotParameters()
  {
    this.SetVisibleDuplicants();
    if (this.activeSnapshots.TryGetValue(HashedString.op_Implicit(this.GetSnapshotName(AudioMixerSnapshots.Get().DuplicantCountMovingSnapshot)), out this.duplicantCountMovingInst))
      ((EventInstance) ref this.duplicantCountMovingInst).setParameterByName("duplicantCount", (float) Mathf.Max(0, this.visibleDupes["moving"] - AudioMixer.VISIBLE_DUPLICANTS_BEFORE_ATTENUATION), false);
    if (this.activeSnapshots.TryGetValue(HashedString.op_Implicit(this.GetSnapshotName(AudioMixerSnapshots.Get().DuplicantCountSleepingSnapshot)), out this.duplicantCountSleepingInst))
      ((EventInstance) ref this.duplicantCountSleepingInst).setParameterByName("duplicantCount", (float) Mathf.Max(0, this.visibleDupes["sleeping"] - AudioMixer.VISIBLE_DUPLICANTS_BEFORE_ATTENUATION), false);
    if (this.activeSnapshots.TryGetValue(HashedString.op_Implicit(this.GetSnapshotName(AudioMixerSnapshots.Get().DuplicantCountAttenuatorMigrated)), out this.duplicantCountInst))
      ((EventInstance) ref this.duplicantCountInst).setParameterByName("duplicantCount", (float) Mathf.Max(0, this.visibleDupes["visible"] - AudioMixer.VISIBLE_DUPLICANTS_BEFORE_ATTENUATION), false);
    if (!this.activeSnapshots.TryGetValue(HashedString.op_Implicit(this.GetSnapshotName(AudioMixerSnapshots.Get().PulseSnapshot)), out this.pulseInst))
      return;
    float num = AudioMixer.PULSE_SNAPSHOT_BPM / 60f;
    switch (SpeedControlScreen.Instance.GetSpeed())
    {
      case 1:
        num /= 2f;
        break;
      case 2:
        num /= 3f;
        break;
    }
    ((EventInstance) ref this.pulseInst).setParameterByName("Pulse", Mathf.Abs(Mathf.Sin(Time.time * 3.14159274f * num)), false);
  }

  public void UpdateSpaceVisibleSnapshot(float percent) => ((EventInstance) ref this.spaceVisibleInst).setParameterByName("spaceVisible", percent, false);

  public void UpdateFacilityVisibleSnapshot(float percent) => ((EventInstance) ref this.facilityVisibleInst).setParameterByName("facilityVisible", percent, false);

  private void SetVisibleDuplicants()
  {
    int num1 = 0;
    int num2 = 0;
    int num3 = 0;
    for (int idx = 0; idx < Components.LiveMinionIdentities.Count; ++idx)
    {
      if (CameraController.Instance.IsVisiblePos(TransformExtensions.GetPosition(Components.LiveMinionIdentities[idx].transform)))
      {
        ++num1;
        Navigator component = ((Component) Components.LiveMinionIdentities[idx]).GetComponent<Navigator>();
        if (Object.op_Inequality((Object) component, (Object) null) && component.IsMoving())
        {
          ++num2;
        }
        else
        {
          StaminaMonitor.Instance smi = ((Component) ((Component) Components.LiveMinionIdentities[idx]).GetComponent<Worker>()).GetSMI<StaminaMonitor.Instance>();
          if (smi != null && smi.IsSleeping())
            ++num3;
        }
      }
    }
    this.visibleDupes["visible"] = num1;
    this.visibleDupes["moving"] = num2;
    this.visibleDupes["sleeping"] = num3;
  }

  public void StartUserVolumesSnapshot()
  {
    this.Start(AudioMixerSnapshots.Get().UserVolumeSettingsSnapshot);
    EventInstance eventInstance;
    if (!this.activeSnapshots.TryGetValue(HashedString.op_Implicit(this.GetSnapshotName(AudioMixerSnapshots.Get().UserVolumeSettingsSnapshot)), out eventInstance))
      return;
    EventDescription eventDescription;
    ((EventInstance) ref eventInstance).getDescription(ref eventDescription);
    USER_PROPERTY userProperty;
    ((EventDescription) ref eventDescription).getUserProperty("buses", ref userProperty);
    string[] strArray = ((USER_PROPERTY) ref userProperty).stringValue().Split('-');
    for (int index = 0; index < strArray.Length; ++index)
    {
      float num = 1f;
      string str = "Volume_" + strArray[index];
      if (KPlayerPrefs.HasKey(str))
        num = KPlayerPrefs.GetFloat(str);
      AudioMixer.UserVolumeBus userVolumeBus = new AudioMixer.UserVolumeBus();
      userVolumeBus.busLevel = num;
      userVolumeBus.labelString = StringEntry.op_Implicit(Strings.Get("STRINGS.UI.FRONTEND.AUDIO_OPTIONS_SCREEN.AUDIO_BUS_" + strArray[index].ToUpper()));
      this.userVolumeSettings.Add(strArray[index], userVolumeBus);
      this.SetUserVolume(strArray[index], userVolumeBus.busLevel);
    }
  }

  public void SetUserVolume(string bus, float value)
  {
    if (!this.userVolumeSettings.ContainsKey(bus))
    {
      Debug.LogError((object) "The provided bus doesn't exist. Check yo'self fool!");
    }
    else
    {
      if ((double) value > 1.0)
        value = 1f;
      else if ((double) value < 0.0)
        value = 0.0f;
      this.userVolumeSettings[bus].busLevel = value;
      KPlayerPrefs.SetFloat("Volume_" + bus, value);
      EventInstance eventInstance;
      if (this.activeSnapshots.TryGetValue(HashedString.op_Implicit(this.GetSnapshotName(AudioMixerSnapshots.Get().UserVolumeSettingsSnapshot)), out eventInstance))
        ((EventInstance) ref eventInstance).setParameterByName("userVolume_" + bus, this.userVolumeSettings[bus].busLevel, false);
      else
        this.Log("Tried to set [" + bus + "] to [" + value.ToString() + "] but UserVolumeSettingsSnapshot is not active.");
      if (!(bus == "Music"))
        return;
      this.SetSnapshotParameter(AudioMixerSnapshots.Get().DynamicMusicPlayingSnapshot, "userVolume_Music", value);
    }
  }

  private void Log(string s)
  {
  }

  public class UserVolumeBus
  {
    public string labelString;
    public float busLevel;
  }
}
