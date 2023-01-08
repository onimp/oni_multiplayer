// Decompiled with JetBrains decompiler
// Type: HealthyGameMessageScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/HealthyGameMessageScreen")]
public class HealthyGameMessageScreen : KMonoBehaviour
{
  public KButton confirmButton;
  public CanvasGroup canvasGroup;
  private float spawnTime;
  private float totalTime = 10f;
  private float fadeTime = 1.5f;
  private bool isFirstUpdate = true;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.confirmButton.onClick += (System.Action) (() => this.PlayIntroShort());
    ((Component) this.confirmButton).gameObject.SetActive(false);
  }

  private void PlayIntroShort()
  {
    string str = KPlayerPrefs.GetString("PlayShortOnLaunch", "");
    if (!string.IsNullOrEmpty(MainMenu.Instance.IntroShortName) && str != MainMenu.Instance.IntroShortName)
    {
      VideoScreen component = KScreenManager.AddChild(((Component) FrontEndManager.Instance).gameObject, ((Component) ScreenPrefabs.Instance.VideoScreen).gameObject).GetComponent<VideoScreen>();
      component.PlayVideo(Assets.GetVideo(MainMenu.Instance.IntroShortName), overrideAudioSnapshot: AudioMixerSnapshots.Get().MainMenuVideoPlayingSnapshot);
      component.OnStop += (System.Action) (() =>
      {
        KPlayerPrefs.SetString("PlayShortOnLaunch", MainMenu.Instance.IntroShortName);
        if (!Object.op_Inequality((Object) ((Component) this).gameObject, (Object) null))
          return;
        Object.Destroy((Object) ((Component) this).gameObject);
      });
    }
    else
      Object.Destroy((Object) ((Component) this).gameObject);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    Object.Destroy((Object) ((Component) this).gameObject);
  }

  private void Update()
  {
    if (!DistributionPlatform.Inst.IsDLCStatusReady())
      return;
    if (this.isFirstUpdate)
    {
      this.isFirstUpdate = false;
      this.spawnTime = Time.unscaledTime;
    }
    else
    {
      float num1 = Mathf.Min(Time.unscaledDeltaTime, 0.0333333351f);
      float num2 = Time.unscaledTime - this.spawnTime;
      if ((double) num2 < (double) this.totalTime - (double) this.fadeTime)
        this.canvasGroup.alpha += num1 * (1f / this.fadeTime);
      else if ((double) num2 >= (double) this.totalTime + 0.75)
      {
        this.canvasGroup.alpha = 1f;
        ((Component) this.confirmButton).gameObject.SetActive(true);
      }
      else
      {
        if ((double) num2 < (double) this.totalTime - (double) this.fadeTime)
          return;
        this.canvasGroup.alpha -= num1 * (1f / this.fadeTime);
      }
    }
  }
}
