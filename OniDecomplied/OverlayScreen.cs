// Decompiled with JetBrains decompiler
// Type: OverlayScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/OverlayScreen")]
public class OverlayScreen : KMonoBehaviour
{
  public static HashSet<Tag> WireIDs = new HashSet<Tag>();
  public static HashSet<Tag> GasVentIDs = new HashSet<Tag>();
  public static HashSet<Tag> LiquidVentIDs = new HashSet<Tag>();
  public static HashSet<Tag> HarvestableIDs = new HashSet<Tag>();
  public static HashSet<Tag> DiseaseIDs = new HashSet<Tag>();
  public static HashSet<Tag> SuitIDs = new HashSet<Tag>();
  public static HashSet<Tag> SolidConveyorIDs = new HashSet<Tag>();
  public static HashSet<Tag> RadiationIDs = new HashSet<Tag>();
  [SerializeField]
  public EventReference techViewSoundPath;
  private EventInstance techViewSound;
  private bool techViewSoundPlaying;
  public static OverlayScreen Instance;
  [Header("Power")]
  [SerializeField]
  private Canvas powerLabelParent;
  [SerializeField]
  private LocText powerLabelPrefab;
  [SerializeField]
  private BatteryUI batUIPrefab;
  [SerializeField]
  private Vector3 powerLabelOffset;
  [SerializeField]
  private Vector3 batteryUIOffset;
  [SerializeField]
  private Vector3 batteryUITransformerOffset;
  [SerializeField]
  private Vector3 batteryUISmallTransformerOffset;
  [SerializeField]
  private Color consumerColour;
  [SerializeField]
  private Color generatorColour;
  [SerializeField]
  private Color buildingDisabledColour = Color.gray;
  [Header("Circuits")]
  [SerializeField]
  private Color32 circuitUnpoweredColour;
  [SerializeField]
  private Color32 circuitSafeColour;
  [SerializeField]
  private Color32 circuitStrainingColour;
  [SerializeField]
  private Color32 circuitOverloadingColour;
  [Header("Crops")]
  [SerializeField]
  private GameObject harvestableNotificationPrefab;
  [Header("Disease")]
  [SerializeField]
  private GameObject diseaseOverlayPrefab;
  [Header("Suit")]
  [SerializeField]
  private GameObject suitOverlayPrefab;
  [Header("ToolTip")]
  [SerializeField]
  private TextStyleSetting TooltipHeader;
  [SerializeField]
  private TextStyleSetting TooltipDescription;
  [Header("Logic")]
  [SerializeField]
  private LogicModeUI logicModeUIPrefab;
  public Action<HashedString> OnOverlayChanged;
  private OverlayScreen.ModeInfo currentModeInfo;
  private Dictionary<HashedString, OverlayScreen.ModeInfo> modeInfos = new Dictionary<HashedString, OverlayScreen.ModeInfo>();

  public HashedString mode => this.currentModeInfo.mode.ViewMode();

  protected virtual void OnPrefabInit()
  {
    Debug.Assert(Object.op_Equality((Object) OverlayScreen.Instance, (Object) null));
    OverlayScreen.Instance = this;
    this.powerLabelParent = GameObject.Find("WorldSpaceCanvas").GetComponent<Canvas>();
  }

  protected virtual void OnLoadLevel()
  {
    this.harvestableNotificationPrefab = (GameObject) null;
    this.powerLabelParent = (Canvas) null;
    OverlayScreen.Instance = (OverlayScreen) null;
    OverlayModes.Mode.Clear();
    this.modeInfos = (Dictionary<HashedString, OverlayScreen.ModeInfo>) null;
    this.currentModeInfo = new OverlayScreen.ModeInfo();
    base.OnLoadLevel();
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.techViewSound = KFMOD.CreateInstance(this.techViewSoundPath);
    this.techViewSoundPlaying = false;
    Shader.SetGlobalVector("_OverlayParams", Vector4.zero);
    this.RegisterModes();
    this.currentModeInfo = this.modeInfos[OverlayModes.None.ID];
  }

  private void RegisterModes()
  {
    this.modeInfos.Clear();
    this.RegisterMode((OverlayModes.Mode) new OverlayModes.None());
    this.RegisterMode((OverlayModes.Mode) new OverlayModes.Oxygen());
    this.RegisterMode((OverlayModes.Mode) new OverlayModes.Power(this.powerLabelParent, this.powerLabelPrefab, this.batUIPrefab, this.powerLabelOffset, this.batteryUIOffset, this.batteryUITransformerOffset, this.batteryUISmallTransformerOffset));
    this.RegisterMode((OverlayModes.Mode) new OverlayModes.Temperature());
    this.RegisterMode((OverlayModes.Mode) new OverlayModes.ThermalConductivity());
    this.RegisterMode((OverlayModes.Mode) new OverlayModes.Light());
    this.RegisterMode((OverlayModes.Mode) new OverlayModes.LiquidConduits());
    this.RegisterMode((OverlayModes.Mode) new OverlayModes.GasConduits());
    this.RegisterMode((OverlayModes.Mode) new OverlayModes.Decor());
    this.RegisterMode((OverlayModes.Mode) new OverlayModes.Disease(this.powerLabelParent, this.diseaseOverlayPrefab));
    this.RegisterMode((OverlayModes.Mode) new OverlayModes.Crop(this.powerLabelParent, this.harvestableNotificationPrefab));
    this.RegisterMode((OverlayModes.Mode) new OverlayModes.Harvest());
    this.RegisterMode((OverlayModes.Mode) new OverlayModes.Priorities());
    this.RegisterMode((OverlayModes.Mode) new OverlayModes.HeatFlow());
    this.RegisterMode((OverlayModes.Mode) new OverlayModes.Rooms());
    this.RegisterMode((OverlayModes.Mode) new OverlayModes.Suit(this.powerLabelParent, this.suitOverlayPrefab));
    this.RegisterMode((OverlayModes.Mode) new OverlayModes.Logic(this.logicModeUIPrefab));
    this.RegisterMode((OverlayModes.Mode) new OverlayModes.SolidConveyor());
    this.RegisterMode((OverlayModes.Mode) new OverlayModes.TileMode());
    this.RegisterMode((OverlayModes.Mode) new OverlayModes.Radiation());
  }

  private void RegisterMode(OverlayModes.Mode mode) => this.modeInfos[mode.ViewMode()] = new OverlayScreen.ModeInfo()
  {
    mode = mode
  };

  private void LateUpdate() => this.currentModeInfo.mode.Update();

  public void ToggleOverlay(HashedString newMode, bool allowSound = true)
  {
    int num = !allowSound ? 0 : (HashedString.op_Equality(this.currentModeInfo.mode.ViewMode(), newMode) ? 0 : 1);
    if (HashedString.op_Inequality(newMode, OverlayModes.None.ID))
      ManagementMenu.Instance.CloseAll();
    this.currentModeInfo.mode.Disable();
    if (HashedString.op_Inequality(newMode, this.currentModeInfo.mode.ViewMode()) && HashedString.op_Equality(newMode, OverlayModes.None.ID))
      ManagementMenu.Instance.CloseAll();
    SimDebugView.Instance.SetMode(newMode);
    if (!this.modeInfos.TryGetValue(newMode, out this.currentModeInfo))
      this.currentModeInfo = this.modeInfos[OverlayModes.None.ID];
    this.currentModeInfo.mode.Enable();
    if (num != 0)
      this.UpdateOverlaySounds();
    if (HashedString.op_Equality(OverlayModes.None.ID, this.currentModeInfo.mode.ViewMode()))
    {
      AudioMixer.instance.Stop(AudioMixerSnapshots.Get().TechFilterOnMigrated);
      MusicManager.instance.SetDynamicMusicOverlayInactive();
      ((EventInstance) ref this.techViewSound).stop((STOP_MODE) 0);
      this.techViewSoundPlaying = false;
    }
    else if (!this.techViewSoundPlaying)
    {
      AudioMixer.instance.Start(AudioMixerSnapshots.Get().TechFilterOnMigrated);
      MusicManager.instance.SetDynamicMusicOverlayActive();
      ((EventInstance) ref this.techViewSound).start();
      this.techViewSoundPlaying = true;
    }
    if (this.OnOverlayChanged != null)
      this.OnOverlayChanged(this.currentModeInfo.mode.ViewMode());
    this.ActivateLegend();
  }

  private void ActivateLegend()
  {
    if (Object.op_Equality((Object) OverlayLegend.Instance, (Object) null))
      return;
    OverlayLegend.Instance.SetLegend(this.currentModeInfo.mode);
  }

  public void Refresh() => this.LateUpdate();

  public HashedString GetMode() => this.currentModeInfo.mode == null ? OverlayModes.None.ID : this.currentModeInfo.mode.ViewMode();

  private void UpdateOverlaySounds()
  {
    string soundName = this.currentModeInfo.mode.GetSoundName();
    if (!(soundName != ""))
      return;
    KMonoBehaviour.PlaySound(GlobalAssets.GetSound(soundName));
  }

  private struct ModeInfo
  {
    public OverlayModes.Mode mode;
  }
}
