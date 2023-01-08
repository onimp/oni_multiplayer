// Decompiled with JetBrains decompiler
// Type: WattsonMessage
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WattsonMessage : KScreen
{
  private const float STARTTIME = 0.1f;
  private const float ENDTIME = 6.6f;
  private const float ALPHA_SPEED = 0.01f;
  private const float expandedHeight = 300f;
  [SerializeField]
  private GameObject dialog;
  [SerializeField]
  private RectTransform content;
  [SerializeField]
  private LocText message;
  [SerializeField]
  private Image bg;
  [SerializeField]
  private KButton button;
  [SerializeField]
  private EventReference dialogSound;
  private List<KScreen> hideScreensWhileActive = new List<KScreen>();
  private bool startFade;
  private List<SchedulerHandle> scheduleHandles = new List<SchedulerHandle>();
  private static readonly HashedString[] WorkLoopAnims = new HashedString[2]
  {
    HashedString.op_Implicit("working_pre"),
    HashedString.op_Implicit("working_loop")
  };
  private int birthsComplete;

  public virtual float GetSortKey() => 8f;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    Game.Instance.Subscribe(-122303817, new Action<object>(this.OnNewBaseCreated));
    if (DlcManager.IsExpansion1Active())
      ((TMP_Text) this.message).SetText((string) STRINGS.UI.WELCOMEMESSAGEBODY_SPACEDOUT);
    else
      ((TMP_Text) this.message).SetText((string) STRINGS.UI.WELCOMEMESSAGEBODY);
  }

  private IEnumerator ExpandPanel()
  {
    yield return (object) SequenceUtil.WaitForSecondsRealtime(0.2f);
    float height = 0.0f;
    while ((double) height < 299.0)
    {
      height = Mathf.Lerp(Util.rectTransform(this.dialog).sizeDelta.y, 300f, Time.unscaledDeltaTime * 15f);
      Util.rectTransform(this.dialog).sizeDelta = new Vector2(Util.rectTransform(this.dialog).sizeDelta.x, height);
      yield return (object) 0;
    }
    yield return (object) null;
  }

  private IEnumerator CollapsePanel()
  {
    WattsonMessage wattsonMessage = this;
    float height = 300f;
    while ((double) height > 1.0)
    {
      height = Mathf.Lerp(Util.rectTransform(wattsonMessage.dialog).sizeDelta.y, 0.0f, Time.unscaledDeltaTime * 15f);
      Util.rectTransform(wattsonMessage.dialog).sizeDelta = new Vector2(Util.rectTransform(wattsonMessage.dialog).sizeDelta.x, height);
      yield return (object) 0;
    }
    wattsonMessage.Deactivate();
    yield return (object) null;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.hideScreensWhileActive.Add((KScreen) NotificationScreen.Instance);
    this.hideScreensWhileActive.Add((KScreen) OverlayMenu.Instance);
    if (Object.op_Inequality((Object) PlanScreen.Instance, (Object) null))
      this.hideScreensWhileActive.Add((KScreen) PlanScreen.Instance);
    if (Object.op_Inequality((Object) BuildMenu.Instance, (Object) null))
      this.hideScreensWhileActive.Add((KScreen) BuildMenu.Instance);
    this.hideScreensWhileActive.Add((KScreen) ManagementMenu.Instance);
    this.hideScreensWhileActive.Add((KScreen) ToolMenu.Instance);
    this.hideScreensWhileActive.Add((KScreen) ToolMenu.Instance.PriorityScreen);
    this.hideScreensWhileActive.Add((KScreen) PinnedResourcesPanel.Instance);
    this.hideScreensWhileActive.Add((KScreen) TopLeftControlScreen.Instance);
    this.hideScreensWhileActive.Add((KScreen) DateTime.Instance);
    this.hideScreensWhileActive.Add((KScreen) BuildWatermark.Instance);
    this.hideScreensWhileActive.Add((KScreen) BuildWatermark.Instance);
    this.hideScreensWhileActive.Add((KScreen) ColonyDiagnosticScreen.Instance);
    if (Object.op_Inequality((Object) WorldSelector.Instance, (Object) null))
      this.hideScreensWhileActive.Add((KScreen) WorldSelector.Instance);
    foreach (KScreen kscreen in this.hideScreensWhileActive)
      kscreen.Show(false);
  }

  public void Update()
  {
    if (!this.startFade)
      return;
    Color color = ((Graphic) this.bg).color;
    color.a -= 0.01f;
    if ((double) color.a <= 0.0)
      color.a = 0.0f;
    ((Graphic) this.bg).color = color;
  }

  protected virtual void OnActivate()
  {
    Debug.Log((object) "WattsonMessage OnActivate");
    base.OnActivate();
    AudioMixer.instance.Stop(AudioMixerSnapshots.Get().NewBaseSetupSnapshot);
    AudioMixer.instance.Start(AudioMixerSnapshots.Get().IntroNIS);
    AudioMixer.instance.activeNIS = true;
    this.button.onClick += (System.Action) (() => ((MonoBehaviour) this).StartCoroutine(this.CollapsePanel()));
    this.dialog.GetComponent<KScreen>().Show(false);
    this.startFade = false;
    GameObject telepad = GameUtil.GetTelepad(ClusterManager.Instance.GetStartWorld().id);
    if (Object.op_Inequality((Object) telepad, (Object) null))
    {
      KAnimControllerBase kac = telepad.GetComponent<KAnimControllerBase>();
      kac.Play(WattsonMessage.WorkLoopAnims, (KAnim.PlayMode) 0);
      for (int idx1 = 0; idx1 < Components.LiveMinionIdentities.Count; ++idx1)
      {
        int idx = idx1 + 1;
        MinionIdentity liveMinionIdentity = Components.LiveMinionIdentities[idx1];
        TransformExtensions.SetPosition(((Component) liveMinionIdentity).gameObject.transform, new Vector3((float) ((double) TransformExtensions.GetPosition(telepad.transform).x + (double) idx - 1.5), TransformExtensions.GetPosition(telepad.transform).y, TransformExtensions.GetPosition(((Component) liveMinionIdentity).gameObject.transform).z));
        ChoreProvider chore_provider = ((Component) liveMinionIdentity).gameObject.GetComponent<ChoreProvider>();
        EmoteChore chorePre = new EmoteChore((IStateMachineTarget) chore_provider, Db.Get().ChoreTypes.EmoteHighPriority, HashedString.op_Implicit("anim_interacts_portal_kanim"), new HashedString[1]
        {
          HashedString.op_Implicit("portalbirth_pre_" + idx.ToString())
        }, (KAnim.PlayMode) 0);
        UIScheduler.Instance.Schedule("DupeBirth", (float) idx * 0.5f, (Action<object>) (data =>
        {
          chorePre.Cancel("Done looping");
          EmoteChore emoteChore = new EmoteChore((IStateMachineTarget) chore_provider, Db.Get().ChoreTypes.EmoteHighPriority, HashedString.op_Implicit("anim_interacts_portal_kanim"), new HashedString[1]
          {
            HashedString.op_Implicit("portalbirth_" + idx.ToString())
          });
          emoteChore.onComplete = emoteChore.onComplete + (Action<Chore>) (param =>
          {
            ++this.birthsComplete;
            if (this.birthsComplete != Components.LiveMinionIdentities.Count - 1 || !this.IsActive())
              return;
            this.PauseAndShowMessage();
          });
        }), (object) null, (SchedulerGroup) null);
      }
      UIScheduler.Instance.Schedule("Welcome", 6.6f, (Action<object>) (data => kac.Play(new HashedString[2]
      {
        HashedString.op_Implicit("working_pst"),
        HashedString.op_Implicit("idle")
      })), (object) null, (SchedulerGroup) null);
      CameraController.Instance.DisableUserCameraControl = true;
    }
    else
    {
      Debug.LogWarning((object) "Failed to spawn telepad - does the starting base template lack a 'Headquarters' ?");
      this.PauseAndShowMessage();
    }
    this.scheduleHandles.Add(UIScheduler.Instance.Schedule("GoHome", 0.1f, (Action<object>) (data =>
    {
      CameraController.Instance.OrthographicSize = TuningData<WattsonMessage.Tuning>.Get().initialOrthographicSize;
      CameraController.Instance.CameraGoHome(0.5f);
      this.startFade = true;
      MusicManager.instance.PlaySong("Music_WattsonMessage");
    }), (object) null, (SchedulerGroup) null));
  }

  protected void PauseAndShowMessage()
  {
    SpeedControlScreen.Instance.Pause(false);
    ((MonoBehaviour) this).StartCoroutine(this.ExpandPanel());
    KFMOD.PlayUISound(this.dialogSound);
    this.dialog.GetComponent<KScreen>().Activate();
    this.dialog.GetComponent<KScreen>().SetShouldFadeIn(true);
    this.dialog.GetComponent<KScreen>().Show(true);
  }

  protected virtual void OnDeactivate()
  {
    base.OnDeactivate();
    AudioMixer.instance.Stop(AudioMixerSnapshots.Get().IntroNIS);
    AudioMixer.instance.StartPersistentSnapshots();
    MusicManager.instance.StopSong("Music_WattsonMessage");
    MusicManager.instance.PlayDynamicMusic();
    AudioMixer.instance.activeNIS = false;
    DemoTimer.Instance.CountdownActive = true;
    SpeedControlScreen.Instance.Unpause(false);
    CameraController.Instance.DisableUserCameraControl = false;
    foreach (SchedulerHandle scheduleHandle in this.scheduleHandles)
      scheduleHandle.ClearScheduler();
    UIScheduler.Instance.Schedule("fadeInUI", 0.5f, (Action<object>) (d =>
    {
      foreach (KScreen kscreen in this.hideScreensWhileActive)
      {
        if (!Object.op_Equality((Object) kscreen, (Object) null))
        {
          kscreen.SetShouldFadeIn(true);
          kscreen.Show(true);
        }
      }
      CameraController.Instance.SetMaxOrthographicSize(20f);
      Game.Instance.StartDelayedInitialSave();
      UIScheduler.Instance.Schedule("InitialScreenshot", 1f, (Action<object>) (data => Game.Instance.timelapser.InitialScreenshot()), (object) null, (SchedulerGroup) null);
      GameScheduler.Instance.Schedule("BasicTutorial", 1.5f, (Action<object>) (data => Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Basics)), (object) null, (SchedulerGroup) null);
      GameScheduler.Instance.Schedule("WelcomeTutorial", 2f, (Action<object>) (data => Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Welcome)), (object) null, (SchedulerGroup) null);
      GameScheduler.Instance.Schedule("DiggingTutorial", 420f, (Action<object>) (data => Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Digging)), (object) null, (SchedulerGroup) null);
    }), (object) null, (SchedulerGroup) null);
    Game.Instance.SetGameStarted();
    if (!Object.op_Inequality((Object) TopLeftControlScreen.Instance, (Object) null))
      return;
    TopLeftControlScreen.Instance.RefreshName();
  }

  public virtual void OnKeyDown(KButtonEvent e)
  {
    if (e.TryConsume((Action) 1))
    {
      CameraController.Instance.CameraGoHome();
      this.Deactivate();
    }
    ((KInputEvent) e).Consumed = true;
  }

  public virtual void OnKeyUp(KButtonEvent e) => ((KInputEvent) e).Consumed = true;

  private void OnNewBaseCreated(object data) => ((Component) this).gameObject.SetActive(true);

  public class Tuning : TuningData<WattsonMessage.Tuning>
  {
    public float initialOrthographicSize;
  }
}
