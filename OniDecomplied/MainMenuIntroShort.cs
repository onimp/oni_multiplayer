// Decompiled with JetBrains decompiler
// Type: MainMenuIntroShort
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/MainMenuIntroShort")]
public class MainMenuIntroShort : KMonoBehaviour
{
  [SerializeField]
  private bool alwaysPlay;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    string str = KPlayerPrefs.GetString("PlayShortOnLaunch", "");
    if ((string.IsNullOrEmpty(MainMenu.Instance.IntroShortName) ? 0 : (str != MainMenu.Instance.IntroShortName ? 1 : 0)) != 0)
    {
      VideoScreen component = KScreenManager.AddChild(((Component) FrontEndManager.Instance).gameObject, ((Component) ScreenPrefabs.Instance.VideoScreen).gameObject).GetComponent<VideoScreen>();
      component.PlayVideo(Assets.GetVideo(MainMenu.Instance.IntroShortName), overrideAudioSnapshot: AudioMixerSnapshots.Get().MainMenuVideoPlayingSnapshot);
      component.OnStop += (System.Action) (() =>
      {
        KPlayerPrefs.SetString("PlayShortOnLaunch", MainMenu.Instance.IntroShortName);
        ((Component) this).gameObject.SetActive(false);
      });
    }
    else
      ((Component) this).gameObject.SetActive(false);
  }
}
