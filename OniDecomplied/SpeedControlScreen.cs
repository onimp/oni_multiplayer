// Decompiled with JetBrains decompiler
// Type: SpeedControlScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class SpeedControlScreen : KScreen
{
  public GameObject playButtonWidget;
  public GameObject pauseButtonWidget;
  public Image playIcon;
  public Image pauseIcon;
  [SerializeField]
  private TextStyleSetting TooltipTextStyle;
  public GameObject speedButtonWidget_slow;
  public GameObject speedButtonWidget_medium;
  public GameObject speedButtonWidget_fast;
  public GameObject mainMenuWidget;
  public float normalSpeed;
  public float fastSpeed;
  public float ultraSpeed;
  private KToggle pauseButton;
  private KToggle slowButton;
  private KToggle mediumButton;
  private KToggle fastButton;
  private int speed;
  private int pauseCount;
  private float stepTime;

  public static SpeedControlScreen Instance { get; private set; }

  public static void DestroyInstance() => SpeedControlScreen.Instance = (SpeedControlScreen) null;

  public bool IsPaused => this.pauseCount > 0;

  protected virtual void OnPrefabInit()
  {
    SpeedControlScreen.Instance = this;
    this.pauseButton = this.pauseButtonWidget.GetComponent<KToggle>();
    this.slowButton = this.speedButtonWidget_slow.GetComponent<KToggle>();
    this.mediumButton = this.speedButtonWidget_medium.GetComponent<KToggle>();
    this.fastButton = this.speedButtonWidget_fast.GetComponent<KToggle>();
    KToggle[] ktoggleArray = new KToggle[4]
    {
      this.pauseButton,
      this.slowButton,
      this.mediumButton,
      this.fastButton
    };
    foreach (KToggle ktoggle in ktoggleArray)
      ((WidgetSoundPlayer) ktoggle.soundPlayer).Enabled = false;
    this.slowButton.onClick += (System.Action) (() =>
    {
      this.PlaySpeedChangeSound(1f);
      this.SetSpeed(0);
    });
    this.mediumButton.onClick += (System.Action) (() =>
    {
      this.PlaySpeedChangeSound(2f);
      this.SetSpeed(1);
    });
    this.fastButton.onClick += (System.Action) (() =>
    {
      this.PlaySpeedChangeSound(3f);
      this.SetSpeed(2);
    });
    this.pauseButton.onClick += (System.Action) (() => this.TogglePause());
    this.speedButtonWidget_slow.GetComponent<ToolTip>().AddMultiStringTooltip(GameUtil.ReplaceHotkeyString((string) STRINGS.UI.TOOLTIPS.SPEEDBUTTON_SLOW, (Action) 12), this.TooltipTextStyle);
    this.speedButtonWidget_medium.GetComponent<ToolTip>().AddMultiStringTooltip(GameUtil.ReplaceHotkeyString((string) STRINGS.UI.TOOLTIPS.SPEEDBUTTON_MEDIUM, (Action) 12), this.TooltipTextStyle);
    this.speedButtonWidget_fast.GetComponent<ToolTip>().AddMultiStringTooltip(GameUtil.ReplaceHotkeyString((string) STRINGS.UI.TOOLTIPS.SPEEDBUTTON_FAST, (Action) 12), this.TooltipTextStyle);
    this.playButtonWidget.GetComponent<KButton>().onClick += (System.Action) (() => this.TogglePause());
    // ISSUE: method pointer
    KInputManager.InputChange.AddListener(new UnityAction((object) this, __methodptr(ResetToolTip)));
  }

  protected virtual void OnSpawn()
  {
    if (Object.op_Inequality((Object) SaveGame.Instance, (Object) null))
    {
      this.speed = SaveGame.Instance.GetSpeed();
      this.SetSpeed(this.speed);
    }
    base.OnSpawn();
    this.OnChanged();
  }

  protected virtual void OnForcedCleanUp()
  {
    // ISSUE: method pointer
    KInputManager.InputChange.RemoveListener(new UnityAction((object) this, __methodptr(ResetToolTip)));
    ((KMonoBehaviour) this).OnForcedCleanUp();
  }

  public int GetSpeed() => this.speed;

  public void SetSpeed(int Speed)
  {
    this.speed = Speed % 3;
    switch (this.speed)
    {
      case 0:
        ((Selectable) this.slowButton).Select();
        this.slowButton.isOn = true;
        this.mediumButton.isOn = false;
        this.fastButton.isOn = false;
        break;
      case 1:
        ((Selectable) this.mediumButton).Select();
        this.slowButton.isOn = false;
        this.mediumButton.isOn = true;
        this.fastButton.isOn = false;
        break;
      case 2:
        ((Selectable) this.fastButton).Select();
        this.slowButton.isOn = false;
        this.mediumButton.isOn = false;
        this.fastButton.isOn = true;
        break;
    }
    this.OnSpeedChange();
  }

  public void ToggleRidiculousSpeed()
  {
    this.ultraSpeed = (double) this.ultraSpeed != 3.0 ? 3f : 10f;
    this.speed = 2;
    this.OnChanged();
  }

  public void TogglePause(bool playsound = true)
  {
    if (this.IsPaused)
      this.Unpause(playsound);
    else
      this.Pause(playsound);
  }

  public void ResetToolTip()
  {
    this.speedButtonWidget_slow.GetComponent<ToolTip>().ClearMultiStringTooltip();
    this.speedButtonWidget_medium.GetComponent<ToolTip>().ClearMultiStringTooltip();
    this.speedButtonWidget_fast.GetComponent<ToolTip>().ClearMultiStringTooltip();
    this.speedButtonWidget_slow.GetComponent<ToolTip>().AddMultiStringTooltip(GameUtil.ReplaceHotkeyString((string) STRINGS.UI.TOOLTIPS.SPEEDBUTTON_SLOW, (Action) 12), this.TooltipTextStyle);
    this.speedButtonWidget_medium.GetComponent<ToolTip>().AddMultiStringTooltip(GameUtil.ReplaceHotkeyString((string) STRINGS.UI.TOOLTIPS.SPEEDBUTTON_MEDIUM, (Action) 12), this.TooltipTextStyle);
    this.speedButtonWidget_fast.GetComponent<ToolTip>().AddMultiStringTooltip(GameUtil.ReplaceHotkeyString((string) STRINGS.UI.TOOLTIPS.SPEEDBUTTON_FAST, (Action) 12), this.TooltipTextStyle);
    if (this.pauseButton.isOn)
    {
      this.pauseButtonWidget.GetComponent<ToolTip>().ClearMultiStringTooltip();
      this.pauseButtonWidget.GetComponent<ToolTip>().AddMultiStringTooltip(GameUtil.ReplaceHotkeyString((string) STRINGS.UI.TOOLTIPS.UNPAUSE, (Action) 11), this.TooltipTextStyle);
    }
    else
    {
      this.pauseButtonWidget.GetComponent<ToolTip>().ClearMultiStringTooltip();
      this.pauseButtonWidget.GetComponent<ToolTip>().AddMultiStringTooltip(GameUtil.ReplaceHotkeyString((string) STRINGS.UI.TOOLTIPS.PAUSE, (Action) 11), this.TooltipTextStyle);
    }
  }

  public void Pause(bool playSound = true, bool isCrashed = false)
  {
    ++this.pauseCount;
    if (this.pauseCount != 1)
      return;
    if (playSound)
    {
      if (isCrashed)
        KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Crash_Screen"));
      else
        KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Speed_Pause"));
      if (Object.op_Inequality((Object) SoundListenerController.Instance, (Object) null))
        SoundListenerController.Instance.SetLoopingVolume(0.0f);
    }
    AudioMixer.instance.Start(AudioMixerSnapshots.Get().SpeedPausedMigrated);
    MusicManager.instance.SetDynamicMusicPaused();
    this.pauseButtonWidget.GetComponent<ToolTip>().ClearMultiStringTooltip();
    this.pauseButtonWidget.GetComponent<ToolTip>().AddMultiStringTooltip(GameUtil.ReplaceHotkeyString((string) STRINGS.UI.TOOLTIPS.UNPAUSE, (Action) 11), this.TooltipTextStyle);
    this.pauseButton.isOn = true;
    this.OnPause();
  }

  public void Unpause(bool playSound = true)
  {
    this.pauseCount = Mathf.Max(0, this.pauseCount - 1);
    if (this.pauseCount != 0)
      return;
    if (playSound)
    {
      KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Speed_Unpause"));
      if (Object.op_Inequality((Object) SoundListenerController.Instance, (Object) null))
        SoundListenerController.Instance.SetLoopingVolume(1f);
    }
    AudioMixer.instance.Stop(AudioMixerSnapshots.Get().SpeedPausedMigrated);
    MusicManager.instance.SetDynamicMusicUnpaused();
    this.pauseButtonWidget.GetComponent<ToolTip>().ClearMultiStringTooltip();
    this.pauseButtonWidget.GetComponent<ToolTip>().AddMultiStringTooltip(GameUtil.ReplaceHotkeyString((string) STRINGS.UI.TOOLTIPS.PAUSE, (Action) 11), this.TooltipTextStyle);
    this.pauseButton.isOn = false;
    this.SetSpeed(this.speed);
    this.OnPlay();
  }

  private void OnPause() => this.OnChanged();

  private void OnPlay() => this.OnChanged();

  public void OnSpeedChange()
  {
    if (Game.IsQuitting())
      return;
    this.OnChanged();
  }

  private void OnChanged()
  {
    if (this.IsPaused)
      Time.timeScale = 0.0f;
    else if (this.speed == 0)
      Time.timeScale = this.normalSpeed;
    else if (this.speed == 1)
    {
      Time.timeScale = this.fastSpeed;
    }
    else
    {
      if (this.speed != 2)
        return;
      Time.timeScale = this.ultraSpeed;
    }
  }

  public virtual void OnKeyDown(KButtonEvent e)
  {
    if (e.TryConsume((Action) 11))
      this.TogglePause();
    else if (e.TryConsume((Action) 12))
    {
      this.PlaySpeedChangeSound((float) ((this.speed + 1) % 3 + 1));
      this.SetSpeed(this.speed + 1);
      this.OnSpeedChange();
    }
    else if (e.TryConsume((Action) 9))
    {
      ++this.speed;
      this.speed = Math.Min(this.speed, 2);
      this.SetSpeed(this.speed);
    }
    else
    {
      if (!e.TryConsume((Action) 10))
        return;
      --this.speed;
      this.speed = Math.Max(this.speed, 0);
      this.SetSpeed(this.speed);
    }
  }

  private void PlaySpeedChangeSound(float speed)
  {
    string sound = GlobalAssets.GetSound("Speed_Change");
    if (sound == null)
      return;
    EventInstance instance = SoundEvent.BeginOneShot(sound, Vector3.zero);
    ((EventInstance) ref instance).setParameterByName("Speed", speed, false);
    SoundEvent.EndOneShot(instance);
  }

  public void DebugStepFrame()
  {
    DebugUtil.LogArgs(new object[1]
    {
      (object) string.Format("Stepping one frame {0} ({1})", (object) GameClock.Instance.GetTime(), (object) (float) ((double) GameClock.Instance.GetTime() / 600.0))
    });
    this.stepTime = Time.time;
    this.Unpause(false);
    ((MonoBehaviour) this).StartCoroutine(this.DebugStepFrameDelay());
  }

  private IEnumerator DebugStepFrameDelay()
  {
    yield return (object) null;
    DebugUtil.LogArgs(new object[3]
    {
      (object) "Stepped one frame",
      (object) (float) ((double) Time.time - (double) this.stepTime),
      (object) "seconds"
    });
    this.Pause(false);
  }
}
