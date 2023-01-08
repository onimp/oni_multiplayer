// Decompiled with JetBrains decompiler
// Type: ReachedDistantPlanetSequence
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections;
using UnityEngine;

public static class ReachedDistantPlanetSequence
{
  public static void Start(KMonoBehaviour controller) => ((MonoBehaviour) controller).StartCoroutine(ReachedDistantPlanetSequence.Sequence());

  private static IEnumerator Sequence()
  {
    Vector3 cameraTagetMid = Vector3.zero;
    Vector3 cameraTargetTop = Vector3.zero;
    Spacecraft spacecraft = (Spacecraft) null;
    foreach (Spacecraft spacecraft1 in SpacecraftManager.instance.GetSpacecraft())
    {
      if (spacecraft1.state != Spacecraft.MissionState.Grounded && SpacecraftManager.instance.GetSpacecraftDestination(spacecraft1.id).GetDestinationType().Id == Db.Get().SpaceDestinationTypes.Wormhole.Id)
      {
        spacecraft = spacecraft1;
        foreach (RocketModule rocketModule in spacecraft1.launchConditions.rocketModules)
        {
          if (Object.op_Inequality((Object) ((Component) rocketModule).GetComponent<RocketEngine>(), (Object) null))
          {
            cameraTagetMid = Vector3.op_Addition(((Component) rocketModule).gameObject.transform.position, Vector3.op_Multiply(Vector3.up, 7f));
            break;
          }
        }
        cameraTargetTop = Vector3.op_Addition(cameraTagetMid, Vector3.op_Multiply(Vector3.up, 20f));
      }
    }
    if (!SpeedControlScreen.Instance.IsPaused)
      SpeedControlScreen.Instance.Pause(false);
    CameraController.Instance.SetWorldInteractive(false);
    AudioMixer.instance.Stop(AudioMixerSnapshots.Get().VictoryMessageSnapshot);
    CameraController.Instance.FadeOut();
    yield return (object) SequenceUtil.WaitForSecondsRealtime(3f);
    CameraController.Instance.SetTargetPos(cameraTagetMid, 15f, false);
    CameraController.Instance.SetOverrideZoomSpeed(5f);
    yield return (object) SequenceUtil.WaitForSecondsRealtime(1f);
    AudioMixer.instance.Start(Db.Get().ColonyAchievements.ReachedDistantPlanet.victoryNISSnapshot);
    CameraController.Instance.FadeIn();
    MusicManager.instance.PlaySong("Music_Victory_02_NIS");
    foreach (MinionIdentity liveMinionIdentity in Components.LiveMinionIdentities)
    {
      if (Object.op_Inequality((Object) liveMinionIdentity, (Object) null))
      {
        ((Component) liveMinionIdentity).GetComponent<Facing>().Face(cameraTagetMid.x);
        Db db = Db.Get();
        ChoreProvider component = ((Component) liveMinionIdentity).GetComponent<ChoreProvider>();
        EmoteChore emoteChore1 = new EmoteChore((IStateMachineTarget) component, db.ChoreTypes.EmoteHighPriority, db.Emotes.Minion.Cheer, 2);
        EmoteChore emoteChore2 = new EmoteChore((IStateMachineTarget) component, db.ChoreTypes.EmoteHighPriority, db.Emotes.Minion.Cheer, 2);
        EmoteChore emoteChore3 = new EmoteChore((IStateMachineTarget) component, db.ChoreTypes.EmoteHighPriority, db.Emotes.Minion.Cheer, 2);
      }
    }
    yield return (object) SequenceUtil.WaitForSecondsRealtime(0.5f);
    if (SpeedControlScreen.Instance.IsPaused)
      SpeedControlScreen.Instance.Unpause(false);
    SpeedControlScreen.Instance.SetSpeed(1);
    CameraController.Instance.SetOverrideZoomSpeed(0.01f);
    CameraController.Instance.SetTargetPos(cameraTargetTop, 35f, false);
    float baseZoomSpeed = 0.03f;
    for (int i = 0; i < 10; ++i)
    {
      yield return (object) SequenceUtil.WaitForSecondsRealtime(0.5f);
      CameraController.Instance.SetOverrideZoomSpeed(baseZoomSpeed + (float) i * (3f / 500f));
    }
    yield return (object) SequenceUtil.WaitForSecondsRealtime(6f);
    CameraController.Instance.FadeOut();
    MusicManager.instance.StopSong("Music_Victory_02_NIS");
    AudioMixer.instance.Stop(Db.Get().ColonyAchievements.ReachedDistantPlanet.victoryNISSnapshot);
    yield return (object) SequenceUtil.WaitForSecondsRealtime(2f);
    spacecraft.TemporallyTear();
    AudioMixer.instance.Start(AudioMixerSnapshots.Get().VictoryCinematicSnapshot);
    if (!SpeedControlScreen.Instance.IsPaused)
      SpeedControlScreen.Instance.Pause(false);
    VideoScreen component1 = ((Component) GameScreenManager.Instance.StartScreen(((Component) ScreenPrefabs.Instance.VideoScreen).gameObject, (GameObject) null, (GameScreenManager.UIRenderTarget) 2)).GetComponent<VideoScreen>();
    component1.PlayVideo(Assets.GetVideo(Db.Get().ColonyAchievements.ReachedDistantPlanet.shortVideoName), true, AudioMixerSnapshots.Get().VictoryCinematicSnapshot);
    component1.QueueVictoryVideoLoop(true, Db.Get().ColonyAchievements.ReachedDistantPlanet.messageBody, Db.Get().ColonyAchievements.ReachedDistantPlanet.Id, Db.Get().ColonyAchievements.ReachedDistantPlanet.loopVideoName);
    component1.OnStop += (System.Action) (() =>
    {
      StoryMessageScreen.HideInterface(false);
      CameraController.Instance.FadeIn();
      CameraController.Instance.SetWorldInteractive(true);
      HoverTextScreen.Instance.Show(true);
      CameraController.Instance.SetOverrideZoomSpeed(1f);
      AudioMixer.instance.Stop(AudioMixerSnapshots.Get().VictoryCinematicSnapshot);
      AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MuteDynamicMusicSnapshot);
      RootMenu.Instance.canTogglePauseScreen = true;
    });
  }
}
