// Decompiled with JetBrains decompiler
// Type: AudioMixerSnapshots
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMODUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioMixerSnapshots : ScriptableObject
{
  public EventReference TechFilterOnMigrated;
  public EventReference TechFilterLogicOn;
  public EventReference NightStartedMigrated;
  public EventReference MenuOpenMigrated;
  public EventReference MenuOpenHalfEffect;
  public EventReference SpeedPausedMigrated;
  public EventReference DuplicantCountAttenuatorMigrated;
  public EventReference NewBaseSetupSnapshot;
  public EventReference FrontEndSnapshot;
  public EventReference FrontEndWelcomeScreenSnapshot;
  public EventReference FrontEndWorldGenerationSnapshot;
  public EventReference IntroNIS;
  public EventReference PulseSnapshot;
  public EventReference ESCPauseSnapshot;
  public EventReference MENUNewDuplicantSnapshot;
  public EventReference UserVolumeSettingsSnapshot;
  public EventReference DuplicantCountMovingSnapshot;
  public EventReference DuplicantCountSleepingSnapshot;
  public EventReference PortalLPDimmedSnapshot;
  public EventReference DynamicMusicPlayingSnapshot;
  public EventReference FabricatorSideScreenOpenSnapshot;
  public EventReference SpaceVisibleSnapshot;
  public EventReference MENUStarmapSnapshot;
  public EventReference MENUStarmapNotPausedSnapshot;
  public EventReference GameNotFocusedSnapshot;
  public EventReference FacilityVisibleSnapshot;
  public EventReference TutorialVideoPlayingSnapshot;
  public EventReference VictoryMessageSnapshot;
  public EventReference VictoryNISGenericSnapshot;
  public EventReference VictoryNISRocketSnapshot;
  public EventReference VictoryCinematicSnapshot;
  public EventReference VictoryFadeToBlackSnapshot;
  public EventReference MuteDynamicMusicSnapshot;
  public EventReference ActiveBaseChangeSnapshot;
  public EventReference EventPopupSnapshot;
  public EventReference SmallRocketInteriorReverbSnapshot;
  public EventReference MediumRocketInteriorReverbSnapshot;
  public EventReference MainMenuVideoPlayingSnapshot;
  public EventReference TechFilterRadiationOn;
  public EventReference FrontEndSupplyClosetSnapshot;
  [SerializeField]
  private EventReference[] snapshots;
  [NonSerialized]
  public List<string> snapshotMap = new List<string>();
  private static AudioMixerSnapshots instance;

  [ContextMenu("Reload")]
  public void ReloadSnapshots()
  {
    this.snapshotMap.Clear();
    foreach (EventReference snapshot in this.snapshots)
    {
      string eventReferencePath = KFMOD.GetEventReferencePath(snapshot);
      if (!Util.IsNullOrWhiteSpace(eventReferencePath))
        this.snapshotMap.Add(eventReferencePath);
    }
  }

  public static AudioMixerSnapshots Get()
  {
    if (Object.op_Equality((Object) AudioMixerSnapshots.instance, (Object) null))
      AudioMixerSnapshots.instance = Resources.Load<AudioMixerSnapshots>(nameof (AudioMixerSnapshots));
    return AudioMixerSnapshots.instance;
  }
}
