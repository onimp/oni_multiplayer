// Decompiled with JetBrains decompiler
// Type: ArtifactSequence
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArtifactSequence
{
  public static void Start(KMonoBehaviour controller) => ((MonoBehaviour) controller).StartCoroutine(ArtifactSequence.Sequence());

  private static IEnumerator Sequence()
  {
    if (!SpeedControlScreen.Instance.IsPaused)
      SpeedControlScreen.Instance.Pause(false);
    CameraController.Instance.SetWorldInteractive(false);
    AudioMixer.instance.Stop(AudioMixerSnapshots.Get().VictoryMessageSnapshot);
    AudioMixer.instance.Start(Db.Get().ColonyAchievements.CollectedArtifacts.victoryNISSnapshot);
    MusicManager.instance.PlaySong("Music_Victory_02_NIS");
    GameObject cameraTaget = (GameObject) null;
    foreach (Telepad telepad in Components.Telepads)
    {
      if (Object.op_Inequality((Object) telepad, (Object) null))
        cameraTaget = ((Component) telepad).gameObject;
    }
    CameraController.Instance.FadeOut(speed: 2f);
    yield return (object) SequenceUtil.WaitForSecondsRealtime(1f);
    CameraController.Instance.SetTargetPos(cameraTaget.transform.position, 10f, false);
    CameraController.Instance.SetOverrideZoomSpeed(10f);
    yield return (object) SequenceUtil.WaitForSecondsRealtime(0.6f);
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
        EmoteChore emoteChore = new EmoteChore((IStateMachineTarget) ((Component) liveMinionIdentity).GetComponent<ChoreProvider>(), db.ChoreTypes.EmoteHighPriority, db.Emotes.Minion.Cheer, 4);
      }
    }
    yield return (object) SequenceUtil.WaitForSecondsRealtime(0.5f);
    yield return (object) SequenceUtil.WaitForSecondsRealtime(3f);
    cameraTaget = (GameObject) null;
    List<SpaceArtifact> spaceArtifactList = new List<SpaceArtifact>();
    foreach (SpaceArtifact spaceArtifact1 in Components.SpaceArtifacts)
    {
      if (Object.op_Inequality((Object) spaceArtifact1, (Object) null) && ((Component) spaceArtifact1).HasTag(GameTags.Stored) && !((Component) spaceArtifact1).HasTag(GameTags.CharmedArtifact))
      {
        bool flag = true;
        foreach (SpaceArtifact spaceArtifact2 in spaceArtifactList)
        {
          if (!Object.op_Equality((Object) spaceArtifact2, (Object) spaceArtifact1) && (Object.op_Equality((Object) spaceArtifact1.GetMyWorld(), (Object) spaceArtifact2.GetMyWorld()) || Grid.GetCellDistance(Grid.PosToCell((KMonoBehaviour) spaceArtifact1), Grid.PosToCell((KMonoBehaviour) spaceArtifact2)) < 10))
          {
            flag = false;
            break;
          }
        }
        if (flag)
          spaceArtifactList.Add(spaceArtifact1);
      }
      if (spaceArtifactList.Count >= 3)
        break;
    }
    if (spaceArtifactList.Count < 3)
    {
      foreach (SpaceArtifact spaceArtifact in Components.SpaceArtifacts)
      {
        if (!spaceArtifactList.Contains(spaceArtifact))
        {
          if (Object.op_Inequality((Object) spaceArtifact, (Object) null) && !((Component) spaceArtifact).HasTag(GameTags.CharmedArtifact))
          {
            if (spaceArtifactList.Count == 0)
            {
              spaceArtifactList.Add(spaceArtifact);
            }
            else
            {
              bool flag = true;
              foreach (SpaceArtifact cmp in spaceArtifactList)
              {
                if (!Object.op_Equality((Object) cmp, (Object) spaceArtifact) && Grid.GetCellDistance(Grid.PosToCell((KMonoBehaviour) spaceArtifact), Grid.PosToCell((KMonoBehaviour) cmp)) < 10)
                {
                  flag = false;
                  break;
                }
              }
              if (flag)
                spaceArtifactList.Add(spaceArtifact);
            }
          }
          if (spaceArtifactList.Count >= 3)
            break;
        }
      }
    }
    foreach (Component component in spaceArtifactList)
    {
      cameraTaget = component.gameObject;
      CameraController.Instance.FadeOut(speed: 2f);
      yield return (object) SequenceUtil.WaitForSecondsRealtime(1f);
      CameraController.Instance.SetTargetPos(cameraTaget.transform.position, 4f, false);
      CameraController.Instance.SetOverrideZoomSpeed(10f);
      yield return (object) SequenceUtil.WaitForSecondsRealtime(0.5f);
      CameraController.Instance.FadeIn(speed: 2f);
      foreach (MinionIdentity liveMinionIdentity in Components.LiveMinionIdentities)
      {
        if (Object.op_Inequality((Object) liveMinionIdentity, (Object) null))
        {
          ((Component) liveMinionIdentity).GetComponent<Facing>().Face(cameraTaget.transform.position.x);
          Db db = Db.Get();
          EmoteChore emoteChore = new EmoteChore((IStateMachineTarget) ((Component) liveMinionIdentity).GetComponent<ChoreProvider>(), db.ChoreTypes.EmoteHighPriority, db.Emotes.Minion.Cheer, 4);
        }
      }
      yield return (object) SequenceUtil.WaitForSecondsRealtime(0.5f);
      CameraController.Instance.SetOverrideZoomSpeed(0.04f);
      CameraController.Instance.SetTargetPos(cameraTaget.transform.position, 8f, false);
      yield return (object) SequenceUtil.WaitForSecondsRealtime(3f);
      cameraTaget = (GameObject) null;
    }
    CameraController.Instance.FadeOut();
    MusicManager.instance.StopSong("Music_Victory_02_NIS");
    AudioMixer.instance.Stop(Db.Get().ColonyAchievements.CollectedArtifacts.victoryNISSnapshot);
    yield return (object) SequenceUtil.WaitForSecondsRealtime(2f);
    AudioMixer.instance.Start(AudioMixerSnapshots.Get().VictoryCinematicSnapshot);
    if (!SpeedControlScreen.Instance.IsPaused)
      SpeedControlScreen.Instance.Pause(false);
    VideoScreen component1 = ((Component) GameScreenManager.Instance.StartScreen(((Component) ScreenPrefabs.Instance.VideoScreen).gameObject, (GameObject) null, (GameScreenManager.UIRenderTarget) 2)).GetComponent<VideoScreen>();
    component1.PlayVideo(Assets.GetVideo(Db.Get().ColonyAchievements.CollectedArtifacts.shortVideoName), true, AudioMixerSnapshots.Get().VictoryCinematicSnapshot);
    component1.QueueVictoryVideoLoop(true, Db.Get().ColonyAchievements.CollectedArtifacts.messageBody, Db.Get().ColonyAchievements.CollectedArtifacts.Id, Db.Get().ColonyAchievements.CollectedArtifacts.loopVideoName);
    component1.OnStop += (System.Action) (() =>
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
