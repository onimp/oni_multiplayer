// Decompiled with JetBrains decompiler
// Type: PauseScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using ProcGen;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PauseScreen : KModalButtonMenu
{
  [SerializeField]
  private OptionsMenuScreen optionsScreen;
  [SerializeField]
  private SaveScreen saveScreenPrefab;
  [SerializeField]
  private LoadScreen loadScreenPrefab;
  [SerializeField]
  private KButton closeButton;
  [SerializeField]
  private LocText title;
  [SerializeField]
  private LocText worldSeed;
  [SerializeField]
  private CopyTextFieldToClipboard clipboard;
  private float originalTimeScale;
  private static PauseScreen instance;

  public static PauseScreen Instance => PauseScreen.instance;

  public static void DestroyInstance() => PauseScreen.instance = (PauseScreen) null;

  protected override void OnPrefabInit()
  {
    this.keepMenuOpen = true;
    base.OnPrefabInit();
    if (!GenericGameSettings.instance.demoMode)
    {
      // ISSUE: method pointer
      // ISSUE: method pointer
      // ISSUE: method pointer
      // ISSUE: method pointer
      // ISSUE: method pointer
      // ISSUE: method pointer
      // ISSUE: method pointer
      // ISSUE: method pointer
      // ISSUE: method pointer
      this.buttons = (IList<KButtonMenu.ButtonInfo>) new KButtonMenu.ButtonInfo[9]
      {
        new KButtonMenu.ButtonInfo((string) UI.FRONTEND.PAUSE_SCREEN.RESUME, (Action) 275, new UnityAction((object) this, __methodptr(OnResume))),
        new KButtonMenu.ButtonInfo((string) UI.FRONTEND.PAUSE_SCREEN.SAVE, (Action) 275, new UnityAction((object) this, __methodptr(OnSave))),
        new KButtonMenu.ButtonInfo((string) UI.FRONTEND.PAUSE_SCREEN.SAVEAS, (Action) 275, new UnityAction((object) this, __methodptr(OnSaveAs))),
        new KButtonMenu.ButtonInfo((string) UI.FRONTEND.PAUSE_SCREEN.LOAD, (Action) 275, new UnityAction((object) this, __methodptr(OnLoad))),
        new KButtonMenu.ButtonInfo((string) UI.FRONTEND.PAUSE_SCREEN.OPTIONS, (Action) 275, new UnityAction((object) this, __methodptr(OnOptions))),
        new KButtonMenu.ButtonInfo((string) UI.FRONTEND.PAUSE_SCREEN.COLONY_SUMMARY, (Action) 275, new UnityAction((object) this, __methodptr(OnColonySummary))),
        new KButtonMenu.ButtonInfo((string) UI.FRONTEND.PAUSE_SCREEN.LOCKERMENU, (Action) 275, new UnityAction((object) this, __methodptr(OnLockerMenu))),
        new KButtonMenu.ButtonInfo((string) UI.FRONTEND.PAUSE_SCREEN.QUIT, (Action) 275, new UnityAction((object) this, __methodptr(OnQuit))),
        new KButtonMenu.ButtonInfo((string) UI.FRONTEND.PAUSE_SCREEN.DESKTOPQUIT, (Action) 275, new UnityAction((object) this, __methodptr(OnDesktopQuit)))
      };
    }
    else
    {
      // ISSUE: method pointer
      // ISSUE: method pointer
      // ISSUE: method pointer
      // ISSUE: method pointer
      this.buttons = (IList<KButtonMenu.ButtonInfo>) new KButtonMenu.ButtonInfo[4]
      {
        new KButtonMenu.ButtonInfo((string) UI.FRONTEND.PAUSE_SCREEN.RESUME, (Action) 275, new UnityAction((object) this, __methodptr(OnResume))),
        new KButtonMenu.ButtonInfo((string) UI.FRONTEND.PAUSE_SCREEN.OPTIONS, (Action) 275, new UnityAction((object) this, __methodptr(OnOptions))),
        new KButtonMenu.ButtonInfo((string) UI.FRONTEND.PAUSE_SCREEN.QUIT, (Action) 275, new UnityAction((object) this, __methodptr(OnQuit))),
        new KButtonMenu.ButtonInfo((string) UI.FRONTEND.PAUSE_SCREEN.DESKTOPQUIT, (Action) 275, new UnityAction((object) this, __methodptr(OnDesktopQuit)))
      };
    }
    this.closeButton.onClick += new System.Action(this.OnResume);
    PauseScreen.instance = this;
    this.Show(false);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.clipboard.GetText = new Func<string>(this.GetClipboardText);
    ((TMP_Text) this.title).SetText((string) UI.FRONTEND.PAUSE_SCREEN.TITLE);
    try
    {
      string settingsCoordinate = CustomGameSettings.Instance.GetSettingsCoordinate();
      string[] settingCoordinate = CustomGameSettings.ParseSettingCoordinate(settingsCoordinate);
      ((TMP_Text) this.worldSeed).SetText(string.Format((string) UI.FRONTEND.PAUSE_SCREEN.WORLD_SEED, (object) settingsCoordinate));
      ((Component) this.worldSeed).GetComponent<ToolTip>().toolTip = string.Format((string) UI.FRONTEND.PAUSE_SCREEN.WORLD_SEED_TOOLTIP, (object) settingCoordinate[1], (object) settingCoordinate[2], (object) settingCoordinate[3], (object) settingCoordinate[4]);
    }
    catch (Exception ex)
    {
      Debug.LogWarning((object) string.Format("Failed to load Coordinates on ClusterLayout {0}, please report this error on the forums", (object) ex));
      CustomGameSettings.Instance.Print();
      Debug.Log((object) ("ClusterCache: " + string.Join(",", (IEnumerable<string>) SettingsCache.clusterLayouts.clusterCache.Keys)));
      ((TMP_Text) this.worldSeed).SetText(string.Format((string) UI.FRONTEND.PAUSE_SCREEN.WORLD_SEED, (object) "0"));
    }
  }

  public override float GetSortKey() => 30f;

  private string GetClipboardText()
  {
    try
    {
      return CustomGameSettings.Instance.GetSettingsCoordinate();
    }
    catch
    {
      return "";
    }
  }

  private void OnResume() => this.Show(false);

  protected override void OnShow(bool show)
  {
    base.OnShow(show);
    if (show)
    {
      AudioMixer.instance.Start(AudioMixerSnapshots.Get().ESCPauseSnapshot);
      MusicManager.instance.OnEscapeMenu(true);
      MusicManager.instance.PlaySong("Music_ESC_Menu");
    }
    else
    {
      ToolTipScreen.Instance.ClearToolTip(((Component) this.closeButton).GetComponent<ToolTip>());
      AudioMixer.instance.Stop(AudioMixerSnapshots.Get().ESCPauseSnapshot);
      MusicManager.instance.OnEscapeMenu(false);
      if (!MusicManager.instance.SongIsPlaying("Music_ESC_Menu"))
        return;
      MusicManager.instance.StopSong("Music_ESC_Menu");
    }
  }

  private void OnOptions() => this.ActivateChildScreen(((Component) this.optionsScreen).gameObject);

  private void OnSaveAs() => this.ActivateChildScreen(((Component) this.saveScreenPrefab).gameObject);

  private void OnSave()
  {
    string filename = SaveLoader.GetActiveSaveFilePath();
    if (!string.IsNullOrEmpty(filename) && File.Exists(filename))
    {
      ((Component) this).gameObject.SetActive(false);
      ((ConfirmDialogScreen) GameScreenManager.Instance.StartScreen(((Component) ScreenPrefabs.Instance.ConfirmDialogScreen).gameObject, ((Component) ((KMonoBehaviour) this).transform.parent).gameObject, (GameScreenManager.UIRenderTarget) 2)).PopupConfirmDialog(string.Format((string) UI.FRONTEND.SAVESCREEN.OVERWRITEMESSAGE, (object) System.IO.Path.GetFileNameWithoutExtension(filename)), (System.Action) (() =>
      {
        this.DoSave(filename);
        ((Component) this).gameObject.SetActive(true);
      }), new System.Action(this.OnCancelPopup));
    }
    else
      this.OnSaveAs();
  }

  private void DoSave(string filename)
  {
    try
    {
      SaveLoader.Instance.Save(filename);
    }
    catch (IOException ex)
    {
      Util.KInstantiateUI(((Component) ScreenPrefabs.Instance.ConfirmDialogScreen).gameObject, ((Component) ((KMonoBehaviour) this).transform.parent).gameObject, true).GetComponent<ConfirmDialogScreen>().PopupConfirmDialog(string.Format((string) UI.FRONTEND.SAVESCREEN.IO_ERROR, (object) ex.ToString()), (System.Action) (() => this.Deactivate()), (System.Action) null, (string) UI.FRONTEND.SAVESCREEN.REPORT_BUG, (System.Action) (() => KCrashReporter.ReportError(ex.Message, ex.StackTrace.ToString(), (string) null, (ConfirmDialogScreen) null, (GameObject) null)));
    }
  }

  private void ConfirmDecision(string text, System.Action onConfirm)
  {
    ((Component) this).gameObject.SetActive(false);
    ((ConfirmDialogScreen) GameScreenManager.Instance.StartScreen(((Component) ScreenPrefabs.Instance.ConfirmDialogScreen).gameObject, ((Component) ((KMonoBehaviour) this).transform.parent).gameObject, (GameScreenManager.UIRenderTarget) 2)).PopupConfirmDialog(text, onConfirm, new System.Action(this.OnCancelPopup));
  }

  private void OnLoad() => this.ActivateChildScreen(((Component) this.loadScreenPrefab).gameObject);

  private void OnColonySummary() => MainMenu.ActivateRetiredColoniesScreenFromData(((Component) ((KMonoBehaviour) PauseScreen.Instance).transform.parent).gameObject, RetireColonyUtility.GetCurrentColonyRetiredColonyData());

  private void OnLockerMenu() => ((KScreen) LockerMenuScreen.Instance).Show(true);

  private void OnQuit() => this.ConfirmDecision((string) UI.FRONTEND.MAINMENU.QUITCONFIRM, new System.Action(this.OnQuitConfirm));

  private void OnDesktopQuit() => this.ConfirmDecision((string) UI.FRONTEND.MAINMENU.DESKTOPQUITCONFIRM, new System.Action(this.OnDesktopQuitConfirm));

  private void OnCancelPopup() => ((Component) this).gameObject.SetActive(true);

  private void OnLoadConfirm() => LoadingOverlay.Load((System.Action) (() =>
  {
    LoadScreen.ForceStopGame();
    this.Deactivate();
    App.LoadScene("frontend");
  }));

  private void OnRetireConfirm() => RetireColonyUtility.SaveColonySummaryData();

  private void OnQuitConfirm() => LoadingOverlay.Load((System.Action) (() =>
  {
    this.Deactivate();
    PauseScreen.TriggerQuitGame();
  }));

  private void OnDesktopQuitConfirm() => App.Quit();

  public override void OnKeyDown(KButtonEvent e)
  {
    if (e.TryConsume((Action) 1) || e.TryConsume((Action) 5))
      this.Show(false);
    else
      base.OnKeyDown(e);
  }

  public static void TriggerQuitGame()
  {
    ThreadedHttps<KleiMetrics>.Instance.EndGame();
    LoadScreen.ForceStopGame();
    App.LoadScene("frontend");
  }
}
