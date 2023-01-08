// Decompiled with JetBrains decompiler
// Type: AudioOptionsScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMODUnity;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class AudioOptionsScreen : KModalScreen
{
  [SerializeField]
  private KButton closeButton;
  [SerializeField]
  private KButton doneButton;
  [SerializeField]
  private SliderContainer sliderPrefab;
  [SerializeField]
  private GameObject sliderGroup;
  [SerializeField]
  private Image jambell;
  [SerializeField]
  private GameObject alwaysPlayMusicButton;
  [SerializeField]
  private GameObject alwaysPlayAutomationButton;
  [SerializeField]
  private GameObject muteOnFocusLostToggle;
  [SerializeField]
  private Dropdown deviceDropdown;
  private UIPool<SliderContainer> sliderPool;
  private Dictionary<KSlider, string> sliderBusMap = new Dictionary<KSlider, string>();
  public static readonly string AlwaysPlayMusicKey = "AlwaysPlayMusic";
  public static readonly string AlwaysPlayAutomation = nameof (AlwaysPlayAutomation);
  public static readonly string MuteOnFocusLost = nameof (MuteOnFocusLost);
  private Dictionary<string, object> alwaysPlayMusicMetric = new Dictionary<string, object>()
  {
    {
      AudioOptionsScreen.AlwaysPlayMusicKey,
      (object) null
    }
  };
  private List<KFMOD.AudioDevice> audioDevices = new List<KFMOD.AudioDevice>();
  private List<Dropdown.OptionData> audioDeviceOptions = new List<Dropdown.OptionData>();

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.closeButton.onClick += (System.Action) (() => this.OnClose(((Component) this).gameObject));
    this.doneButton.onClick += (System.Action) (() => this.OnClose(((Component) this).gameObject));
    this.sliderPool = new UIPool<SliderContainer>(this.sliderPrefab);
    foreach (KeyValuePair<string, AudioMixer.UserVolumeBus> userVolumeSetting in AudioMixer.instance.userVolumeSettings)
    {
      // ISSUE: object of a compiler-generated type is created
      // ISSUE: variable of a compiler-generated type
      AudioOptionsScreen.\u003C\u003Ec__DisplayClass17_0 cDisplayClass170 = new AudioOptionsScreen.\u003C\u003Ec__DisplayClass17_0();
      // ISSUE: reference to a compiler-generated field
      cDisplayClass170.\u003C\u003E4__this = this;
      // ISSUE: reference to a compiler-generated field
      cDisplayClass170.newSlider = this.sliderPool.GetFreeElement(this.sliderGroup, true);
      // ISSUE: reference to a compiler-generated field
      this.sliderBusMap.Add(cDisplayClass170.newSlider.slider, userVolumeSetting.Key);
      // ISSUE: reference to a compiler-generated field
      ((Slider) cDisplayClass170.newSlider.slider).value = userVolumeSetting.Value.busLevel;
      // ISSUE: reference to a compiler-generated field
      ((TMP_Text) cDisplayClass170.newSlider.nameLabel).text = userVolumeSetting.Value.labelString;
      // ISSUE: reference to a compiler-generated field
      cDisplayClass170.newSlider.UpdateSliderLabel(userVolumeSetting.Value.busLevel);
      // ISSUE: reference to a compiler-generated field
      cDisplayClass170.newSlider.slider.ClearReleaseHandleEvent();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: method pointer
      ((UnityEvent<float>) ((Slider) cDisplayClass170.newSlider.slider).onValueChanged).AddListener(new UnityAction<float>((object) cDisplayClass170, __methodptr(\u003COnSpawn\u003Eb__2)));
      if (userVolumeSetting.Key == "Master")
      {
        // ISSUE: reference to a compiler-generated field
        cDisplayClass170.newSlider.transform.SetSiblingIndex(2);
        // ISSUE: reference to a compiler-generated field
        // ISSUE: method pointer
        ((UnityEvent<float>) ((Slider) cDisplayClass170.newSlider.slider).onValueChanged).AddListener(new UnityAction<float>((object) this, __methodptr(CheckMasterValue)));
        this.CheckMasterValue(userVolumeSetting.Value.busLevel);
      }
    }
    HierarchyReferences component1 = this.alwaysPlayMusicButton.GetComponent<HierarchyReferences>();
    GameObject gameObject1 = component1.GetReference("Button").gameObject;
    gameObject1.GetComponent<ToolTip>().SetSimpleTooltip((string) STRINGS.UI.FRONTEND.AUDIO_OPTIONS_SCREEN.MUSIC_EVERY_CYCLE_TOOLTIP);
    component1.GetReference("CheckMark").gameObject.SetActive(MusicManager.instance.alwaysPlayMusic);
    gameObject1.GetComponent<KButton>().onClick += (System.Action) (() => this.ToggleAlwaysPlayMusic());
    ((TMP_Text) component1.GetReference<LocText>("Label")).SetText((string) STRINGS.UI.FRONTEND.AUDIO_OPTIONS_SCREEN.MUSIC_EVERY_CYCLE);
    if (!KPlayerPrefs.HasKey(AudioOptionsScreen.AlwaysPlayAutomation))
      KPlayerPrefs.SetInt(AudioOptionsScreen.AlwaysPlayAutomation, 1);
    HierarchyReferences component2 = this.alwaysPlayAutomationButton.GetComponent<HierarchyReferences>();
    GameObject gameObject2 = component2.GetReference("Button").gameObject;
    gameObject2.GetComponent<ToolTip>().SetSimpleTooltip((string) STRINGS.UI.FRONTEND.AUDIO_OPTIONS_SCREEN.AUTOMATION_SOUNDS_ALWAYS_TOOLTIP);
    gameObject2.GetComponent<KButton>().onClick += (System.Action) (() => this.ToggleAlwaysPlayAutomation());
    ((TMP_Text) component2.GetReference<LocText>("Label")).SetText((string) STRINGS.UI.FRONTEND.AUDIO_OPTIONS_SCREEN.AUTOMATION_SOUNDS_ALWAYS);
    component2.GetReference("CheckMark").gameObject.SetActive(KPlayerPrefs.GetInt(AudioOptionsScreen.AlwaysPlayAutomation) == 1);
    if (!KPlayerPrefs.HasKey(AudioOptionsScreen.MuteOnFocusLost))
      KPlayerPrefs.SetInt(AudioOptionsScreen.MuteOnFocusLost, 0);
    HierarchyReferences component3 = this.muteOnFocusLostToggle.GetComponent<HierarchyReferences>();
    GameObject gameObject3 = component3.GetReference("Button").gameObject;
    gameObject3.GetComponent<ToolTip>().SetSimpleTooltip((string) STRINGS.UI.FRONTEND.AUDIO_OPTIONS_SCREEN.MUTE_ON_FOCUS_LOST_TOOLTIP);
    gameObject3.GetComponent<KButton>().onClick += (System.Action) (() => this.ToggleMuteOnFocusLost());
    ((TMP_Text) component3.GetReference<LocText>("Label")).SetText((string) STRINGS.UI.FRONTEND.AUDIO_OPTIONS_SCREEN.MUTE_ON_FOCUS_LOST);
    component3.GetReference("CheckMark").gameObject.SetActive(KPlayerPrefs.GetInt(AudioOptionsScreen.MuteOnFocusLost) == 1);
  }

  public override void OnKeyDown(KButtonEvent e)
  {
    if (e.TryConsume((Action) 1) || e.TryConsume((Action) 5))
      this.Deactivate();
    else
      base.OnKeyDown(e);
  }

  private void CheckMasterValue(float value) => ((Behaviour) this.jambell).enabled = (double) value == 0.0;

  private void OnReleaseHandle(KSlider slider) => AudioMixer.instance.SetUserVolume(this.sliderBusMap[slider], ((Slider) slider).value);

  private void ToggleAlwaysPlayMusic()
  {
    MusicManager.instance.alwaysPlayMusic = !MusicManager.instance.alwaysPlayMusic;
    this.alwaysPlayMusicButton.GetComponent<HierarchyReferences>().GetReference("CheckMark").gameObject.SetActive(MusicManager.instance.alwaysPlayMusic);
    KPlayerPrefs.SetInt(AudioOptionsScreen.AlwaysPlayMusicKey, MusicManager.instance.alwaysPlayMusic ? 1 : 0);
  }

  private void ToggleAlwaysPlayAutomation()
  {
    KPlayerPrefs.SetInt(AudioOptionsScreen.AlwaysPlayAutomation, KPlayerPrefs.GetInt(AudioOptionsScreen.AlwaysPlayAutomation) == 1 ? 0 : 1);
    this.alwaysPlayAutomationButton.GetComponent<HierarchyReferences>().GetReference("CheckMark").gameObject.SetActive(KPlayerPrefs.GetInt(AudioOptionsScreen.AlwaysPlayAutomation) == 1);
  }

  private void ToggleMuteOnFocusLost()
  {
    KPlayerPrefs.SetInt(AudioOptionsScreen.MuteOnFocusLost, KPlayerPrefs.GetInt(AudioOptionsScreen.MuteOnFocusLost) == 1 ? 0 : 1);
    this.muteOnFocusLostToggle.GetComponent<HierarchyReferences>().GetReference("CheckMark").gameObject.SetActive(KPlayerPrefs.GetInt(AudioOptionsScreen.MuteOnFocusLost) == 1);
  }

  private void BuildAudioDeviceList()
  {
    this.audioDevices.Clear();
    this.audioDeviceOptions.Clear();
    FMOD.System coreSystem1 = RuntimeManager.CoreSystem;
    int num;
    ((FMOD.System) ref coreSystem1).getNumDrivers(ref num);
    for (int index = 0; index < num; ++index)
    {
      KFMOD.AudioDevice audioDevice = new KFMOD.AudioDevice();
      FMOD.System coreSystem2 = RuntimeManager.CoreSystem;
      string str;
      ((FMOD.System) ref coreSystem2).getDriverInfo(index, ref str, 64, ref audioDevice.guid, ref audioDevice.systemRate, ref audioDevice.speakerMode, ref audioDevice.speakerModeChannels);
      audioDevice.name = str;
      audioDevice.fmod_id = index;
      this.audioDevices.Add(audioDevice);
      this.audioDeviceOptions.Add(new Dropdown.OptionData(audioDevice.name));
    }
  }

  private void OnAudioDeviceChanged(int idx)
  {
    FMOD.System coreSystem = RuntimeManager.CoreSystem;
    ((FMOD.System) ref coreSystem).setDriver(idx);
    for (int index = 0; index < this.audioDevices.Count; ++index)
    {
      if (idx == this.audioDevices[index].fmod_id)
      {
        KFMOD.currentDevice = this.audioDevices[index];
        KPlayerPrefs.SetString("AudioDeviceGuid", KFMOD.currentDevice.guid.ToString());
        break;
      }
    }
  }

  private void OnClose(GameObject go)
  {
    this.alwaysPlayMusicMetric[AudioOptionsScreen.AlwaysPlayMusicKey] = (object) MusicManager.instance.alwaysPlayMusic;
    ThreadedHttps<KleiMetrics>.Instance.SendEvent(this.alwaysPlayMusicMetric, nameof (AudioOptionsScreen));
    Object.Destroy((Object) go);
  }
}
