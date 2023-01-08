// Decompiled with JetBrains decompiler
// Type: OldVersionMessageScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SplashMessageScreen")]
public class OldVersionMessageScreen : KModalScreen
{
  public KButton forumButton;
  public KButton confirmButton;
  public KButton quitButton;
  public LocText bodyText;
  public bool previewInEditor;
  public RectTransform messageContainer;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.forumButton.onClick += (System.Action) (() => App.OpenWebURL("https://forums.kleientertainment.com/forums/topic/140474-previous-update-steam-branch-access/"));
    this.confirmButton.onClick += (System.Action) (() =>
    {
      ((Component) this).gameObject.SetActive(false);
      AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndWelcomeScreenSnapshot);
    });
    this.quitButton.onClick += (System.Action) (() => App.Quit());
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.messageContainer.sizeDelta = new Vector2(Mathf.Max(384f, (float) Screen.width * 0.25f), this.messageContainer.sizeDelta.y);
    AudioMixer.instance.Start(AudioMixerSnapshots.Get().FrontEndWelcomeScreenSnapshot);
  }
}
