// Decompiled with JetBrains decompiler
// Type: FeedbackScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FeedbackScreen : KModalScreen
{
  public LocText title;
  public KButton dismissButton;
  public KButton closeButton;
  public KButton bugForumsButton;
  public KButton suggestionForumsButton;
  public KButton logsDirectoryButton;
  public KButton saveFilesDirectoryButton;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    ((TMP_Text) this.title).SetText((string) STRINGS.UI.FRONTEND.FEEDBACK_SCREEN.TITLE);
    this.dismissButton.onClick += (System.Action) (() => this.Deactivate());
    this.closeButton.onClick += (System.Action) (() => this.Deactivate());
    this.bugForumsButton.onClick += (System.Action) (() => App.OpenWebURL("https://forums.kleientertainment.com/klei-bug-tracker/oni/"));
    this.suggestionForumsButton.onClick += (System.Action) (() => App.OpenWebURL("https://forums.kleientertainment.com/forums/forum/133-oxygen-not-included-suggestions-and-feedback/"));
    this.logsDirectoryButton.onClick += (System.Action) (() => App.OpenWebURL(Util.LogsFolder()));
    this.saveFilesDirectoryButton.onClick += (System.Action) (() => App.OpenWebURL(SaveLoader.GetSavePrefix()));
    if (!SteamUtils.IsSteamRunningOnSteamDeck())
      return;
    ((LayoutGroup) ((Component) this.logsDirectoryButton).GetComponentInParent<VerticalLayoutGroup>()).padding = new RectOffset(0, 0, 0, 0);
    ((Component) this.saveFilesDirectoryButton).gameObject.SetActive(false);
    ((Component) this.logsDirectoryButton).gameObject.SetActive(false);
  }
}
