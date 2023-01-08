// Decompiled with JetBrains decompiler
// Type: ThrivingSequence
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections;
using UnityEngine;

public static class ThrivingSequence
{
  public static void Start(KMonoBehaviour controller) => ((MonoBehaviour) controller).StartCoroutine(ThrivingSequence.Sequence());

  private static IEnumerator Sequence()
  {
    if (!SpeedControlScreen.Instance.IsPaused)
      SpeedControlScreen.Instance.Pause(false);
    CameraController.Instance.SetWorldInteractive(false);
    AudioMixer.instance.Stop(AudioMixerSnapshots.Get().VictoryMessageSnapshot);
    AudioMixer.instance.Start(Db.Get().ColonyAchievements.Thriving.victoryNISSnapshot);
    MusicManager.instance.PlaySong("Music_Victory_02_NIS");
    Vector3 cameraBiasUp = Vector3.op_Multiply(Vector3.up, 5f);
    GameObject cameraTaget = (GameObject) null;
    foreach (Telepad telepad in Components.Telepads)
    {
      if (Object.op_Inequality((Object) telepad, (Object) null))
        cameraTaget = ((Component) telepad).gameObject;
    }
    if (Object.op_Inequality((Object) cameraTaget, (Object) null))
    {
      CameraController.Instance.FadeOut(speed: 2f);
      yield return (object) SequenceUtil.WaitForSecondsRealtime(1f);
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
      yield return (object) SequenceUtil.WaitForSecondsRealtime(3f);
    }
    cameraTaget = (GameObject) null;
    cameraTaget = (GameObject) null;
    foreach (ComplexFabricator complexFabricator in Components.ComplexFabricators)
    {
      if (Object.op_Inequality((Object) complexFabricator, (Object) null))
        cameraTaget = ((Component) complexFabricator).gameObject;
    }
    if (Object.op_Equality((Object) cameraTaget, (Object) null))
    {
      foreach (Generator generator in Components.Generators)
      {
        if (Object.op_Inequality((Object) generator, (Object) null))
          cameraTaget = ((Component) generator).gameObject;
      }
    }
    if (Object.op_Equality((Object) cameraTaget, (Object) null))
    {
      foreach (Fabricator fabricator in Components.Fabricators)
      {
        if (Object.op_Inequality((Object) fabricator, (Object) null))
          cameraTaget = ((Component) fabricator).gameObject;
      }
    }
    if (Object.op_Inequality((Object) cameraTaget, (Object) null))
    {
      CameraController.Instance.FadeOut(speed: 2f);
      yield return (object) SequenceUtil.WaitForSecondsRealtime(1f);
      CameraController.Instance.SetTargetPos(Vector3.op_Addition(cameraTaget.transform.position, cameraBiasUp), 10f, false);
      CameraController.Instance.SetOverrideZoomSpeed(10f);
      yield return (object) SequenceUtil.WaitForSecondsRealtime(0.4f);
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
      yield return (object) SequenceUtil.WaitForSecondsRealtime(3f);
    }
    cameraTaget = (GameObject) null;
    cameraTaget = (GameObject) null;
    foreach (MonumentPart monumentPart in Components.MonumentParts)
    {
      if (monumentPart.IsMonumentCompleted())
        cameraTaget = ((Component) monumentPart).gameObject;
    }
    if (Object.op_Inequality((Object) cameraTaget, (Object) null))
    {
      CameraController.Instance.FadeOut(speed: 2f);
      yield return (object) SequenceUtil.WaitForSecondsRealtime(1f);
      CameraController.Instance.SetTargetPos(cameraTaget.transform.position, 15f, false);
      CameraController.Instance.SetOverrideZoomSpeed(10f);
      yield return (object) SequenceUtil.WaitForSecondsRealtime(0.4f);
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
      CameraController.Instance.SetOverrideZoomSpeed(0.075f);
      CameraController.Instance.SetTargetPos(cameraTaget.transform.position, 25f, false);
      yield return (object) SequenceUtil.WaitForSecondsRealtime(5f);
    }
    cameraTaget = (GameObject) null;
    CameraController.Instance.FadeOut();
    MusicManager.instance.StopSong("Music_Victory_02_NIS");
    AudioMixer.instance.Stop(Db.Get().ColonyAchievements.Thriving.victoryNISSnapshot);
    yield return (object) SequenceUtil.WaitForSecondsRealtime(2f);
    AudioMixer.instance.Start(AudioMixerSnapshots.Get().VictoryCinematicSnapshot);
    if (!SpeedControlScreen.Instance.IsPaused)
      SpeedControlScreen.Instance.Pause(false);
    VideoScreen component = ((Component) GameScreenManager.Instance.StartScreen(((Component) ScreenPrefabs.Instance.VideoScreen).gameObject, (GameObject) null, (GameScreenManager.UIRenderTarget) 2)).GetComponent<VideoScreen>();
    component.PlayVideo(Assets.GetVideo(Db.Get().ColonyAchievements.Thriving.shortVideoName), true, AudioMixerSnapshots.Get().VictoryCinematicSnapshot);
    component.QueueVictoryVideoLoop(true, Db.Get().ColonyAchievements.Thriving.messageBody, Db.Get().ColonyAchievements.Thriving.Id, Db.Get().ColonyAchievements.Thriving.loopVideoName);
    component.OnStop += (System.Action) (() =>
    {
      StoryMessageScreen.HideInterface(false);
      CameraController.Instance.FadeIn();
      CameraController.Instance.SetWorldInteractive(true);
      CameraController.Instance.SetOverrideZoomSpeed(1f);
      HoverTextScreen.Instance.Show(true);
      AudioMixer.instance.Stop(AudioMixerSnapshots.Get().VictoryCinematicSnapshot);
      AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MuteDynamicMusicSnapshot);
      RootMenu.Instance.canTogglePauseScreen = true;
    });
  }
}
