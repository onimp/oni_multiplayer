// Decompiled with JetBrains decompiler
// Type: EnterTemporalTearSequence
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections;
using UnityEngine;

public static class EnterTemporalTearSequence
{
  public static GameObject tearOpenerGameObject;

  public static void Start(KMonoBehaviour controller) => ((MonoBehaviour) controller).StartCoroutine(EnterTemporalTearSequence.Sequence());

  private static IEnumerator Sequence()
  {
    if (!SpeedControlScreen.Instance.IsPaused)
      SpeedControlScreen.Instance.Pause(false);
    CameraController.Instance.SetWorldInteractive(false);
    AudioMixer.instance.Stop(AudioMixerSnapshots.Get().VictoryMessageSnapshot);
    CameraController.Instance.FadeOut();
    yield return (object) SequenceUtil.WaitForSecondsRealtime(3f);
    ManagementMenu.Instance.CloseAll();
    AudioMixer.instance.Start(Db.Get().ColonyAchievements.ReachedDistantPlanet.victoryNISSnapshot);
    MusicManager.instance.PlaySong("Music_Victory_02_NIS");
    Vector3 cameraBiasUp = Vector3.op_Multiply(Vector3.up, 5f);
    GameObject cameraTaget = EnterTemporalTearSequence.tearOpenerGameObject;
    if (Object.op_Inequality((Object) cameraTaget, (Object) null))
    {
      CameraController.Instance.SetTargetPos(Vector3.op_Addition(cameraTaget.transform.position, cameraBiasUp), 10f, false);
      CameraController.Instance.SetOverrideZoomSpeed(10f);
      yield return (object) SequenceUtil.WaitForSecondsRealtime(0.4f);
      if (SpeedControlScreen.Instance.IsPaused)
        SpeedControlScreen.Instance.Unpause(false);
      SpeedControlScreen.Instance.SetSpeed(1);
      CameraController.Instance.SetOverrideZoomSpeed(0.1f);
      CameraController.Instance.SetTargetPos(Vector3.op_Addition(cameraTaget.transform.position, cameraBiasUp), 20f, false);
      CameraController.Instance.FadeIn(speed: 2f);
      foreach (MinionIdentity liveMinionIdentity in Components.LiveMinionIdentities)
      {
        if (Object.op_Inequality((Object) liveMinionIdentity, (Object) null))
        {
          ((Component) liveMinionIdentity).GetComponent<Facing>().Face(cameraTaget.transform.position.x);
          Db db = Db.Get();
          EmoteChore emoteChore = new EmoteChore((IStateMachineTarget) ((Component) liveMinionIdentity).GetComponent<ChoreProvider>(), db.ChoreTypes.EmoteHighPriority, db.Emotes.Minion.Cheer, 2);
        }
      }
      yield return (object) SequenceUtil.WaitForSecondsRealtime(0.5f);
      yield return (object) SequenceUtil.WaitForSecondsRealtime(1.5f);
      CameraController.Instance.FadeOut();
      yield return (object) SequenceUtil.WaitForSecondsRealtime(1.5f);
    }
    cameraTaget = (GameObject) null;
    cameraTaget = (GameObject) null;
    foreach (Telepad telepad in Components.Telepads)
    {
      if (Object.op_Inequality((Object) telepad, (Object) null))
      {
        cameraTaget = ((Component) telepad).gameObject;
        CameraController.Instance.SetTargetPos(cameraTaget.transform.position, 10f, false);
        CameraController.Instance.SetOverrideZoomSpeed(10f);
        yield return (object) SequenceUtil.WaitForSecondsRealtime(0.4f);
        if (SpeedControlScreen.Instance.IsPaused)
          SpeedControlScreen.Instance.Unpause(false);
        SpeedControlScreen.Instance.SetSpeed(1);
        CameraController.Instance.SetOverrideZoomSpeed(0.05f);
        CameraController.Instance.SetTargetPos(cameraTaget.transform.position, 20f, false);
        CameraController.Instance.FadeIn(speed: 2f);
        foreach (MinionIdentity liveMinionIdentity in Components.LiveMinionIdentities)
        {
          if (Object.op_Inequality((Object) liveMinionIdentity, (Object) null))
          {
            ((Component) liveMinionIdentity).GetComponent<Facing>().Face(cameraTaget.transform.position.x);
            Db db = Db.Get();
            EmoteChore emoteChore = new EmoteChore((IStateMachineTarget) ((Component) liveMinionIdentity).GetComponent<ChoreProvider>(), db.ChoreTypes.EmoteHighPriority, db.Emotes.Minion.Cheer, 2);
          }
        }
        yield return (object) SequenceUtil.WaitForSecondsRealtime(0.5f);
        yield return (object) SequenceUtil.WaitForSecondsRealtime(1.5f);
        CameraController.Instance.FadeOut();
        yield return (object) SequenceUtil.WaitForSecondsRealtime(1.5f);
      }
    }
    cameraTaget = (GameObject) null;
    MusicManager.instance.StopSong("Music_Victory_02_NIS");
    AudioMixer.instance.Stop(Db.Get().ColonyAchievements.ReachedDistantPlanet.victoryNISSnapshot);
    yield return (object) SequenceUtil.WaitForSecondsRealtime(2f);
    AudioMixer.instance.Start(AudioMixerSnapshots.Get().VictoryCinematicSnapshot);
    if (!SpeedControlScreen.Instance.IsPaused)
      SpeedControlScreen.Instance.Pause(false);
    VideoScreen component = ((Component) GameScreenManager.Instance.StartScreen(((Component) ScreenPrefabs.Instance.VideoScreen).gameObject, (GameObject) null, (GameScreenManager.UIRenderTarget) 2)).GetComponent<VideoScreen>();
    component.PlayVideo(Assets.GetVideo(Db.Get().ColonyAchievements.ReachedDistantPlanet.shortVideoName), true, AudioMixerSnapshots.Get().VictoryCinematicSnapshot);
    component.QueueVictoryVideoLoop(true, Db.Get().ColonyAchievements.ReachedDistantPlanet.messageBody, Db.Get().ColonyAchievements.ReachedDistantPlanet.Id, Db.Get().ColonyAchievements.ReachedDistantPlanet.loopVideoName);
    component.OnStop += (System.Action) (() =>
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
