// Decompiled with JetBrains decompiler
// Type: FocusTargetSequence
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections;
using UnityEngine;

public static class FocusTargetSequence
{
  private static Coroutine sequenceCoroutine;

  public static void Start(MonoBehaviour coroutineRunner, FocusTargetSequence.Data sequenceData) => FocusTargetSequence.sequenceCoroutine = coroutineRunner.StartCoroutine(FocusTargetSequence.RunSequence(sequenceData));

  public static void Cancel(MonoBehaviour coroutineRunner)
  {
    if (FocusTargetSequence.sequenceCoroutine == null)
      return;
    coroutineRunner.StopCoroutine(FocusTargetSequence.sequenceCoroutine);
    FocusTargetSequence.sequenceCoroutine = (Coroutine) null;
  }

  public static IEnumerator RunSequence(FocusTargetSequence.Data sequenceData)
  {
    ((Component) SaveGame.Instance).GetComponent<UserNavigation>();
    CameraController.Instance.FadeOut();
    int speed = SpeedControlScreen.Instance.GetSpeed();
    SpeedControlScreen.Instance.SetSpeed(0);
    bool wasPaused = SpeedControlScreen.Instance.IsPaused;
    if (!wasPaused)
      SpeedControlScreen.Instance.Pause(false);
    PlayerController.Instance.CancelDragging();
    CameraController.Instance.SetWorldInteractive(false);
    yield return (object) CameraController.Instance.activeFadeRoutine;
    KSelectable selectedObject = SelectTool.Instance.selected;
    SelectTool.Instance.Select((KSelectable) null, true);
    RootMenu.Instance.Show(false);
    ClusterManager.Instance.SetActiveWorld(sequenceData.WorldId);
    ManagementMenu.Instance.CloseAll();
    CameraController.Instance.SnapTo(sequenceData.Target, sequenceData.OrthographicSize);
    if (sequenceData.PopupData != null)
      EventInfoScreen.ShowPopup(sequenceData.PopupData);
    CameraController.Instance.FadeIn(speed: 2f);
    if ((double) sequenceData.TargetSize - (double) sequenceData.OrthographicSize > (double) Mathf.Epsilon)
      ((MonoBehaviour) CameraController.Instance).StartCoroutine(CameraController.Instance.DoCinematicZoom(sequenceData.TargetSize));
    if (sequenceData.CanCompleteCB != null)
    {
      SpeedControlScreen.Instance.Unpause(false);
      while (!sequenceData.CanCompleteCB())
        yield return (object) SequenceUtil.WaitForNextFrame;
      SpeedControlScreen.Instance.Pause(false);
    }
    CameraController.Instance.SetWorldInteractive(true);
    SpeedControlScreen.Instance.SetSpeed(speed);
    if (SpeedControlScreen.Instance.IsPaused && !wasPaused)
      SpeedControlScreen.Instance.Unpause(false);
    if (sequenceData.CompleteCB != null)
      sequenceData.CompleteCB();
    RootMenu.Instance.Show(true);
    SelectTool.Instance.Select(selectedObject, true);
    sequenceData.Clear();
    FocusTargetSequence.sequenceCoroutine = (Coroutine) null;
  }

  public struct Data
  {
    public int WorldId;
    public float OrthographicSize;
    public float TargetSize;
    public Vector3 Target;
    public EventInfoData PopupData;
    public System.Action CompleteCB;
    public Func<bool> CanCompleteCB;

    public void Clear()
    {
      this.PopupData = (EventInfoData) null;
      this.CompleteCB = (System.Action) null;
      this.CanCompleteCB = (Func<bool>) null;
    }
  }
}
