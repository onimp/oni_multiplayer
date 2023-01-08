// Decompiled with JetBrains decompiler
// Type: SplashMessageScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TMPro;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/SplashMessageScreen")]
public class SplashMessageScreen : KMonoBehaviour
{
  public KButton forumButton;
  public KButton confirmButton;
  public LocText bodyText;
  public bool previewInEditor;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.forumButton.onClick += (System.Action) (() => App.OpenWebURL("https://forums.kleientertainment.com/forums/forum/118-oxygen-not-included/"));
    this.confirmButton.onClick += (System.Action) (() =>
    {
      ((Component) this).gameObject.SetActive(false);
      AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndWelcomeScreenSnapshot);
    });
    ((TMP_Text) this.bodyText).text = (string) STRINGS.UI.DEVELOPMENTBUILDS.ALPHA.LOADING.BODY;
  }

  private void OnEnable()
  {
    ((Component) this.confirmButton).GetComponent<LayoutElement>();
    ((Component) this.confirmButton).GetComponentInChildren<LocText>();
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (!DlcManager.IsExpansion1Active())
      Object.Destroy((Object) ((Component) this).gameObject);
    else
      AudioMixer.instance.Start(AudioMixerSnapshots.Get().FrontEndWelcomeScreenSnapshot);
  }
}
