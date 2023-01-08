// Decompiled with JetBrains decompiler
// Type: VideoScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class VideoScreen : KModalScreen
{
  public static VideoScreen Instance;
  [SerializeField]
  private VideoPlayer videoPlayer;
  [SerializeField]
  private Slideshow slideshow;
  [SerializeField]
  private KButton closeButton;
  [SerializeField]
  private KButton proceedButton;
  [SerializeField]
  private RectTransform overlayContainer;
  [SerializeField]
  private List<VideoOverlay> overlayPrefabs;
  private RawImage screen;
  private RenderTexture renderTexture;
  private EventReference activeAudioSnapshot;
  [SerializeField]
  private Image fadeOverlay;
  private EventInstance audioHandle;
  private bool victoryLoopQueued;
  private string victoryLoopMessage = "";
  private string victoryLoopClip = "";
  private bool videoSkippable = true;
  public System.Action OnStop;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.ConsumeMouseScroll = true;
    this.closeButton.onClick += (System.Action) (() => this.Stop());
    this.proceedButton.onClick += (System.Action) (() => this.Stop());
    this.videoPlayer.isLooping = false;
    // ISSUE: method pointer
    this.videoPlayer.loopPointReached += new VideoPlayer.EventHandler((object) this, __methodptr(\u003COnPrefabInit\u003Eb__17_2));
    VideoScreen.Instance = this;
    this.Show(false);
  }

  protected virtual void OnForcedCleanUp()
  {
    VideoScreen.Instance = (VideoScreen) null;
    ((KMonoBehaviour) this).OnForcedCleanUp();
  }

  protected override void OnShow(bool show)
  {
    ((KMonoBehaviour) this).transform.SetAsLastSibling();
    base.OnShow(show);
    this.screen = ((Component) this.videoPlayer).gameObject.GetComponent<RawImage>();
  }

  public void DisableAllMedia()
  {
    ((Component) this.overlayContainer).gameObject.SetActive(false);
    ((Component) this.videoPlayer).gameObject.SetActive(false);
    ((Component) this.slideshow).gameObject.SetActive(false);
  }

  public void PlaySlideShow(Sprite[] sprites)
  {
    this.Show(true);
    this.DisableAllMedia();
    this.slideshow.updateType = SlideshowUpdateType.preloadedSprites;
    ((Component) this.slideshow).gameObject.SetActive(true);
    this.slideshow.SetSprites(sprites);
    this.slideshow.SetPaused(false);
  }

  public void PlaySlideShow(string[] files)
  {
    this.Show(true);
    this.DisableAllMedia();
    this.slideshow.updateType = SlideshowUpdateType.loadOnDemand;
    ((Component) this.slideshow).gameObject.SetActive(true);
    this.slideshow.SetFiles(files, 0);
    this.slideshow.SetPaused(false);
  }

  public override void OnKeyDown(KButtonEvent e)
  {
    if (e.IsAction((Action) 1))
    {
      if (((Component) this.slideshow).gameObject.activeSelf && e.TryConsume((Action) 1))
      {
        this.Stop();
        return;
      }
      if (e.TryConsume((Action) 1))
      {
        if (!this.videoSkippable)
          return;
        this.Stop();
        return;
      }
    }
    base.OnKeyDown(e);
  }

  public void PlayVideo(
    VideoClip clip,
    bool unskippable = false,
    EventReference overrideAudioSnapshot = default (EventReference),
    bool showProceedButton = false)
  {
    Debug.Assert(Object.op_Inequality((Object) clip, (Object) null));
    for (int index = 0; index < ((Transform) this.overlayContainer).childCount; ++index)
      Object.Destroy((Object) ((Component) ((Transform) this.overlayContainer).GetChild(index)).gameObject);
    this.Show(true);
    this.videoPlayer.isLooping = false;
    this.activeAudioSnapshot = ((EventReference) ref overrideAudioSnapshot).IsNull ? AudioMixerSnapshots.Get().TutorialVideoPlayingSnapshot : overrideAudioSnapshot;
    AudioMixer.instance.Start(this.activeAudioSnapshot);
    this.DisableAllMedia();
    ((Component) this.videoPlayer).gameObject.SetActive(true);
    this.renderTexture = new RenderTexture(Convert.ToInt32(clip.width), Convert.ToInt32(clip.height), 16);
    this.screen.texture = (Texture) this.renderTexture;
    this.videoPlayer.targetTexture = this.renderTexture;
    this.videoPlayer.audioOutputMode = (VideoAudioOutputMode) 0;
    this.videoPlayer.clip = clip;
    this.videoPlayer.Play();
    if (((EventInstance) ref this.audioHandle).isValid())
    {
      KFMOD.EndOneShot(this.audioHandle);
      ((EventInstance) ref this.audioHandle).clearHandle();
    }
    this.audioHandle = KFMOD.BeginOneShot(GlobalAssets.GetSound("vid_" + ((Object) clip).name), Vector3.zero, 1f);
    KFMOD.EndOneShot(this.audioHandle);
    this.videoSkippable = !unskippable;
    ((Component) this.closeButton).gameObject.SetActive(this.videoSkippable);
    ((Component) this.proceedButton).gameObject.SetActive(showProceedButton && this.videoSkippable);
  }

  public void QueueVictoryVideoLoop(
    bool queue,
    string message = "",
    string victoryAchievement = "",
    string loopVideo = "")
  {
    this.victoryLoopQueued = queue;
    this.victoryLoopMessage = message;
    this.victoryLoopClip = loopVideo;
    this.OnStop += (System.Action) (() =>
    {
      RetireColonyUtility.SaveColonySummaryData();
      MainMenu.ActivateRetiredColoniesScreenFromData(((Component) ((KMonoBehaviour) this).transform.parent).gameObject, RetireColonyUtility.GetCurrentColonyRetiredColonyData());
    });
  }

  public void SetOverlayText(string overlayTemplate, List<string> strings)
  {
    VideoOverlay videoOverlay = (VideoOverlay) null;
    foreach (VideoOverlay overlayPrefab in this.overlayPrefabs)
    {
      if (((Object) overlayPrefab).name == overlayTemplate)
      {
        videoOverlay = overlayPrefab;
        break;
      }
    }
    DebugUtil.Assert(Object.op_Inequality((Object) videoOverlay, (Object) null), "Could not find a template named ", overlayTemplate);
    Util.KInstantiateUI<VideoOverlay>(((Component) videoOverlay).gameObject, ((Component) this.overlayContainer).gameObject, true).SetText(strings);
    ((Component) this.overlayContainer).gameObject.SetActive(true);
  }

  private IEnumerator SwitchToVictoryLoop()
  {
    VideoScreen videoScreen = this;
    videoScreen.victoryLoopQueued = false;
    Color color = ((Graphic) videoScreen.fadeOverlay).color;
    float i;
    for (i = 0.0f; (double) i < 1.0; i += Time.unscaledDeltaTime)
    {
      ((Graphic) videoScreen.fadeOverlay).color = new Color(color.r, color.g, color.b, i);
      yield return (object) SequenceUtil.WaitForNextFrame;
    }
    ((Graphic) videoScreen.fadeOverlay).color = new Color(color.r, color.g, color.b, 1f);
    MusicManager.instance.PlaySong("Music_Victory_03_StoryAndSummary");
    MusicManager.instance.SetSongParameter("Music_Victory_03_StoryAndSummary", "songSection", 1f);
    ((Component) videoScreen.closeButton).gameObject.SetActive(true);
    ((Component) videoScreen.proceedButton).gameObject.SetActive(true);
    videoScreen.SetOverlayText("VictoryEnd", new List<string>()
    {
      videoScreen.victoryLoopMessage
    });
    videoScreen.videoPlayer.clip = Assets.GetVideo(videoScreen.victoryLoopClip);
    videoScreen.videoPlayer.isLooping = true;
    videoScreen.videoPlayer.Play();
    ((Component) videoScreen.proceedButton).gameObject.SetActive(true);
    yield return (object) SequenceUtil.WaitForSecondsRealtime(1f);
    for (i = 1f; (double) i >= 0.0; i -= Time.unscaledDeltaTime)
    {
      ((Graphic) videoScreen.fadeOverlay).color = new Color(color.r, color.g, color.b, i);
      yield return (object) SequenceUtil.WaitForNextFrame;
    }
    ((Graphic) videoScreen.fadeOverlay).color = new Color(color.r, color.g, color.b, 0.0f);
  }

  public void Stop()
  {
    this.videoPlayer.Stop();
    this.screen.texture = (Texture) null;
    this.videoPlayer.targetTexture = (RenderTexture) null;
    if (!((EventReference) ref this.activeAudioSnapshot).IsNull)
    {
      AudioMixer.instance.Stop(this.activeAudioSnapshot);
      ((EventInstance) ref this.audioHandle).stop((STOP_MODE) 0);
    }
    if (this.OnStop != null)
      this.OnStop();
    this.Show(false);
  }

  public virtual void ScreenUpdate(bool topLevel)
  {
    base.ScreenUpdate(topLevel);
    if (!((EventInstance) ref this.audioHandle).isValid())
      return;
    int num1;
    ((EventInstance) ref this.audioHandle).getTimelinePosition(ref num1);
    double num2 = this.videoPlayer.time * 1000.0;
    if ((double) num1 - num2 > 33.0)
    {
      ++this.videoPlayer.frame;
    }
    else
    {
      if (num2 - (double) num1 <= 33.0)
        return;
      --this.videoPlayer.frame;
    }
  }
}
