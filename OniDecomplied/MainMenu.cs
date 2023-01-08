// Decompiled with JetBrains decompiler
// Type: MainMenu
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using FMOD.Studio;
using Klei;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : KScreen
{
  private static MainMenu _instance;
  public KButton Button_ResumeGame;
  private KButton Button_NewGame;
  public GameObject topLeftAlphaMessage;
  private MotdServerClient m_motdServerClient;
  private GameObject GameSettingsScreen;
  private bool m_screenshotMode;
  [SerializeField]
  private CanvasGroup uiCanvas;
  [SerializeField]
  private KButton buttonPrefab;
  [SerializeField]
  private GameObject buttonParent;
  [SerializeField]
  private ColorStyleSetting topButtonStyle;
  [SerializeField]
  private ColorStyleSetting normalButtonStyle;
  [SerializeField]
  private string menuMusicEventName;
  [SerializeField]
  private string ambientLoopEventName;
  private EventInstance ambientLoop;
  [SerializeField]
  private GameObject MOTDContainer;
  [SerializeField]
  private GameObject buttonContainer;
  [SerializeField]
  private LocText motdImageHeader;
  [SerializeField]
  private KButton motdImageButton;
  [SerializeField]
  private Image motdImage;
  [SerializeField]
  private LocText motdNewsHeader;
  [SerializeField]
  private LocText motdNewsBody;
  [SerializeField]
  private PatchNotesScreen patchNotesScreenPrefab;
  [SerializeField]
  private NextUpdateTimer nextUpdateTimer;
  [SerializeField]
  private DLCToggle expansion1Toggle;
  [SerializeField]
  private GameObject expansion1Ad;
  [SerializeField]
  private BuildWatermark buildWatermark;
  [SerializeField]
  public string IntroShortName;
  private static bool HasAutoresumedOnce = false;
  private bool refreshResumeButton = true;
  private int m_cheatInputCounter;
  public const string AutoResumeSaveFileKey = "AutoResumeSaveFile";
  public const string PLAY_SHORT_ON_LAUNCH = "PlayShortOnLaunch";
  private static int LANGUAGE_CONFIRMATION_VERSION = 2;
  private Dictionary<string, MainMenu.SaveFileEntry> saveFileEntries = new Dictionary<string, MainMenu.SaveFileEntry>();

  public static MainMenu Instance => MainMenu._instance;

  private KButton MakeButton(MainMenu.ButtonInfo info)
  {
    KButton kbutton = Util.KInstantiateUI<KButton>(((Component) this.buttonPrefab).gameObject, this.buttonParent, true);
    kbutton.onClick += info.action;
    KImage component = ((Component) kbutton).GetComponent<KImage>();
    component.colorStyleSetting = info.style;
    component.ApplyColorStyleSetting();
    LocText componentInChildren = ((Component) kbutton).GetComponentInChildren<LocText>();
    ((TMP_Text) componentInChildren).text = (string) info.text;
    ((TMP_Text) componentInChildren).fontSize = (float) info.fontSize;
    return kbutton;
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    MainMenu._instance = this;
    this.Button_NewGame = this.MakeButton(new MainMenu.ButtonInfo(STRINGS.UI.FRONTEND.MAINMENU.NEWGAME, new System.Action(this.NewGame), 22, this.topButtonStyle));
    this.MakeButton(new MainMenu.ButtonInfo(STRINGS.UI.FRONTEND.MAINMENU.LOADGAME, new System.Action(this.LoadGame), 22, this.normalButtonStyle));
    this.MakeButton(new MainMenu.ButtonInfo(STRINGS.UI.FRONTEND.MAINMENU.RETIREDCOLONIES, (System.Action) (() => MainMenu.ActivateRetiredColoniesScreen(((Component) ((KMonoBehaviour) this).transform).gameObject)), 14, this.normalButtonStyle));
    this.MakeButton(new MainMenu.ButtonInfo(STRINGS.UI.FRONTEND.MAINMENU.LOCKERMENU, (System.Action) (() => MainMenu.ActivateLockerMenu()), 14, this.normalButtonStyle));
    if (DistributionPlatform.Initialized)
    {
      this.MakeButton(new MainMenu.ButtonInfo(STRINGS.UI.FRONTEND.MAINMENU.TRANSLATIONS, new System.Action(this.Translations), 14, this.normalButtonStyle));
      this.MakeButton(new MainMenu.ButtonInfo(STRINGS.UI.FRONTEND.MODS.TITLE, new System.Action(this.Mods), 14, this.normalButtonStyle));
    }
    this.MakeButton(new MainMenu.ButtonInfo(STRINGS.UI.FRONTEND.MAINMENU.OPTIONS, new System.Action(this.Options), 14, this.normalButtonStyle));
    this.MakeButton(new MainMenu.ButtonInfo(STRINGS.UI.FRONTEND.MAINMENU.QUITTODESKTOP, new System.Action(this.QuitGame), 14, this.normalButtonStyle));
    this.RefreshResumeButton();
    this.Button_ResumeGame.onClick += new System.Action(this.ResumeGame);
    this.SpawnVideoScreen();
    this.StartFEAudio();
    this.CheckPlayerPrefsCorruption();
    if (PatchNotesScreen.ShouldShowScreen())
      Util.KInstantiateUI(((Component) this.patchNotesScreenPrefab).gameObject, ((Component) FrontEndManager.Instance).gameObject, true);
    this.CheckDoubleBoundKeys();
    this.topLeftAlphaMessage.gameObject.SetActive(false);
    this.MOTDContainer.SetActive(false);
    this.buttonContainer.SetActive(false);
    ((Component) this.nextUpdateTimer).gameObject.SetActive(true);
    bool flag = DistributionPlatform.Inst.IsDLCPurchased("EXPANSION1_ID");
    ((Component) this.expansion1Toggle).gameObject.SetActive(flag);
    if (Object.op_Inequality((Object) this.expansion1Ad, (Object) null))
      this.expansion1Ad.gameObject.SetActive(!flag);
    this.m_motdServerClient = new MotdServerClient();
    this.m_motdServerClient.GetMotd((Action<MotdServerClient.MotdResponse, string>) ((response, error) =>
    {
      if (error == null)
      {
        if (DlcManager.IsExpansion1Active())
          this.nextUpdateTimer.UpdateReleaseTimes(response.expansion1_update_data.last_update_time, response.expansion1_update_data.next_update_time, response.expansion1_update_data.update_text_override);
        else
          this.nextUpdateTimer.UpdateReleaseTimes(response.vanilla_update_data.last_update_time, response.vanilla_update_data.next_update_time, response.vanilla_update_data.update_text_override);
        this.topLeftAlphaMessage.gameObject.SetActive(true);
        this.MOTDContainer.SetActive(true);
        this.buttonContainer.SetActive(true);
        ((TMP_Text) this.motdImageHeader).text = response.image_header_text;
        ((TMP_Text) this.motdNewsHeader).text = response.news_header_text;
        ((TMP_Text) this.motdNewsBody).text = response.news_body_text;
        PatchNotesScreen.UpdatePatchNotes(response.patch_notes_summary, response.patch_notes_link_url);
        if (Object.op_Inequality((Object) response.image_texture, (Object) null))
          this.motdImage.sprite = Sprite.Create(response.image_texture, new Rect(0.0f, 0.0f, (float) ((Texture) response.image_texture).width, (float) ((Texture) response.image_texture).height), Vector2.zero);
        else
          Debug.LogWarning((object) "GetMotd failed to return an image texture");
        if (Object.op_Inequality((Object) this.motdImage.sprite, (Object) null))
        {
          Rect rect = this.motdImage.sprite.rect;
          if ((double) ((Rect) ref rect).height != 0.0)
          {
            AspectRatioFitter component = ((Component) this.motdImage).gameObject.GetComponent<AspectRatioFitter>();
            if (Object.op_Inequality((Object) component, (Object) null))
            {
              rect = this.motdImage.sprite.rect;
              double width = (double) ((Rect) ref rect).width;
              rect = this.motdImage.sprite.rect;
              double height = (double) ((Rect) ref rect).height;
              float num = (float) (width / height);
              component.aspectRatio = num;
              goto label_13;
            }
            else
            {
              Debug.LogWarning((object) "Missing AspectRatioFitter on MainMenu motd image.");
              goto label_13;
            }
          }
        }
        Debug.LogWarning((object) "Cannot resize motd image, missing sprite");
label_13:
        this.motdImageButton.ClearOnClick();
        this.motdImageButton.onClick += (System.Action) (() => App.OpenWebURL(response.image_link_url));
      }
      else
        Debug.LogWarning((object) ("Motd Request error: " + error));
    }));
    if (DistributionPlatform.Initialized && DistributionPlatform.Inst.IsPreviousVersionBranch)
      Object.Instantiate<GameObject>(ScreenPrefabs.Instance.OldVersionWarningScreen, ((Component) this.uiCanvas).transform);
    string targetExpansion1AdURL = "";
    Sprite sprite = Assets.GetSprite(HashedString.op_Implicit("expansionPromo_en"));
    if (DistributionPlatform.Initialized && Object.op_Inequality((Object) this.expansion1Ad, (Object) null))
    {
      switch (DistributionPlatform.Inst.Name)
      {
        case "Steam":
          targetExpansion1AdURL = "https://store.steampowered.com/app/1452490/Oxygen_Not_Included__Spaced_Out/";
          break;
        case "Epic":
          targetExpansion1AdURL = "https://store.epicgames.com/en-US/p/oxygen-not-included--spaced-out";
          break;
        case "Rail":
          targetExpansion1AdURL = "https://www.wegame.com.cn/store/2001539/";
          sprite = Assets.GetSprite(HashedString.op_Implicit("expansionPromo_cn"));
          break;
      }
      this.expansion1Ad.GetComponentInChildren<KButton>().onClick += (System.Action) (() => App.OpenWebURL(targetExpansion1AdURL));
      this.expansion1Ad.GetComponent<HierarchyReferences>().GetReference<Image>("Image").sprite = sprite;
    }
    this.activateOnSpawn = true;
  }

  private void OnApplicationFocus(bool focus)
  {
    if (!focus)
      return;
    this.RefreshResumeButton();
  }

  public virtual void OnKeyDown(KButtonEvent e)
  {
    base.OnKeyDown(e);
    if (((KInputEvent) e).Consumed)
      return;
    if (e.TryConsume((Action) 188))
    {
      this.m_screenshotMode = !this.m_screenshotMode;
      this.uiCanvas.alpha = this.m_screenshotMode ? 0.0f : 1f;
    }
    KKeyCode kkeyCode;
    switch (this.m_cheatInputCounter)
    {
      case 0:
        kkeyCode = (KKeyCode) 107;
        break;
      case 1:
        kkeyCode = (KKeyCode) 108;
        break;
      case 2:
        kkeyCode = (KKeyCode) 101;
        break;
      case 3:
        kkeyCode = (KKeyCode) 105;
        break;
      case 4:
        kkeyCode = (KKeyCode) 112;
        break;
      case 5:
        kkeyCode = (KKeyCode) 108;
        break;
      case 6:
        kkeyCode = (KKeyCode) 97;
        break;
      default:
        kkeyCode = (KKeyCode) 121;
        break;
    }
    if (((KInputEvent) e).Controller.GetKeyDown(kkeyCode))
    {
      ((KInputEvent) e).Consumed = true;
      ++this.m_cheatInputCounter;
      if (this.m_cheatInputCounter < 8)
        return;
      Debug.Log((object) "Cheat Detected - enabling Debug Mode");
      DebugHandler.SetDebugEnabled(true);
      this.buildWatermark.RefreshText();
      this.m_cheatInputCounter = 0;
    }
    else
      this.m_cheatInputCounter = 0;
  }

  private void PlayMouseOverSound() => KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover"));

  private void PlayMouseClickSound() => KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click_Open"));

  protected virtual void OnSpawn()
  {
    Debug.Log((object) "-- MAIN MENU -- ");
    base.OnSpawn();
    this.m_cheatInputCounter = 0;
    Canvas.ForceUpdateCanvases();
    this.ShowLanguageConfirmation();
    this.InitLoadScreen();
    LoadScreen.Instance.ShowMigrationIfNecessary(true);
    string savePrefix = SaveLoader.GetSavePrefix();
    try
    {
      string path = System.IO.Path.Combine(savePrefix, "__SPCCHK");
      using (FileStream fileStream = File.OpenWrite(path))
      {
        byte[] buffer = new byte[1024];
        for (int index = 0; index < 15360; ++index)
          fileStream.Write(buffer, 0, buffer.Length);
      }
      File.Delete(path);
    }
    catch (Exception ex)
    {
      string text = string.Format(!(ex is IOException) ? string.Format((string) STRINGS.UI.FRONTEND.SUPPORTWARNINGS.SAVE_DIRECTORY_READ_ONLY, (object) savePrefix) : string.Format((string) STRINGS.UI.FRONTEND.SUPPORTWARNINGS.SAVE_DIRECTORY_INSUFFICIENT_SPACE, (object) savePrefix), (object) savePrefix);
      Util.KInstantiateUI<ConfirmDialogScreen>(((Component) ScreenPrefabs.Instance.ConfirmDialogScreen).gameObject, ((Component) this).gameObject, true).PopupConfirmDialog(text, (System.Action) null, (System.Action) null);
    }
    Global.Instance.modManager.Report(((Component) this).gameObject);
    if (GenericGameSettings.instance.autoResumeGame && !MainMenu.HasAutoresumedOnce && !KCrashReporter.hasCrash || !string.IsNullOrEmpty(GenericGameSettings.instance.performanceCapture.saveGame) || KPlayerPrefs.HasKey("AutoResumeSaveFile"))
    {
      MainMenu.HasAutoresumedOnce = true;
      this.ResumeGame();
    }
    if (!GenericGameSettings.instance.devAutoWorldGen || KCrashReporter.hasCrash)
      return;
    GenericGameSettings.instance.devAutoWorldGen = false;
    GenericGameSettings.instance.devAutoWorldGenActive = true;
    GenericGameSettings.instance.SaveSettings();
    Util.KInstantiateUI(((Component) ScreenPrefabs.Instance.WorldGenScreen).gameObject, ((Component) this).gameObject, true);
  }

  private void UnregisterMotdRequest()
  {
    if (this.m_motdServerClient == null)
      return;
    this.m_motdServerClient.UnregisterCallback();
    this.m_motdServerClient = (MotdServerClient) null;
  }

  protected virtual void OnActivate()
  {
    if (Util.IsNullOrWhiteSpace(this.ambientLoopEventName))
      return;
    this.ambientLoop = KFMOD.CreateInstance(GlobalAssets.GetSound(this.ambientLoopEventName));
    if (!((EventInstance) ref this.ambientLoop).isValid())
      return;
    ((EventInstance) ref this.ambientLoop).start();
  }

  protected virtual void OnDeactivate()
  {
    base.OnDeactivate();
    this.UnregisterMotdRequest();
  }

  public virtual void ScreenUpdate(bool topLevel) => this.refreshResumeButton = topLevel;

  protected virtual void OnLoadLevel()
  {
    ((KMonoBehaviour) this).OnLoadLevel();
    this.StopAmbience();
    this.UnregisterMotdRequest();
  }

  private void ShowLanguageConfirmation()
  {
    if (!SteamManager.Initialized || SteamUtils.GetSteamUILanguage() != "schinese" || KPlayerPrefs.GetInt("LanguageConfirmationVersion") >= MainMenu.LANGUAGE_CONFIRMATION_VERSION)
      return;
    KPlayerPrefs.SetInt("LanguageConfirmationVersion", MainMenu.LANGUAGE_CONFIRMATION_VERSION);
    this.Translations();
  }

  private void ResumeGame()
  {
    string path;
    if (KPlayerPrefs.HasKey("AutoResumeSaveFile"))
    {
      path = KPlayerPrefs.GetString("AutoResumeSaveFile");
      KPlayerPrefs.DeleteKey("AutoResumeSaveFile");
    }
    else
      path = string.IsNullOrEmpty(GenericGameSettings.instance.performanceCapture.saveGame) ? SaveLoader.GetLatestSaveForCurrentDLC() : GenericGameSettings.instance.performanceCapture.saveGame;
    if (string.IsNullOrEmpty(path))
      return;
    KCrashReporter.MOST_RECENT_SAVEFILE = path;
    SaveLoader.SetActiveSaveFilePath(path);
    LoadingOverlay.Load((System.Action) (() => App.LoadScene("backend")));
  }

  private void NewGame() => ((Component) this).GetComponent<NewGameFlow>().BeginFlow();

  private void InitLoadScreen()
  {
    if (!Object.op_Equality((Object) LoadScreen.Instance, (Object) null))
      return;
    Util.KInstantiateUI(((Component) ScreenPrefabs.Instance.LoadScreen).gameObject, ((Component) this).gameObject, true).GetComponent<LoadScreen>();
  }

  private void LoadGame()
  {
    this.InitLoadScreen();
    LoadScreen.Instance.Activate();
  }

  public static void ActivateRetiredColoniesScreen(GameObject parent, string colonyID = "")
  {
    if (Object.op_Equality((Object) RetiredColonyInfoScreen.Instance, (Object) null))
      Util.KInstantiateUI(((Component) ScreenPrefabs.Instance.RetiredColonyInfoScreen).gameObject, parent, true);
    RetiredColonyInfoScreen.Instance.Show(true);
    if (string.IsNullOrEmpty(colonyID))
      return;
    if (Object.op_Inequality((Object) SaveGame.Instance, (Object) null))
      RetireColonyUtility.SaveColonySummaryData();
    RetiredColonyInfoScreen.Instance.LoadColony(RetiredColonyInfoScreen.Instance.GetColonyDataByBaseName(colonyID));
  }

  public static void ActivateRetiredColoniesScreenFromData(
    GameObject parent,
    RetiredColonyData data)
  {
    if (Object.op_Equality((Object) RetiredColonyInfoScreen.Instance, (Object) null))
      Util.KInstantiateUI(((Component) ScreenPrefabs.Instance.RetiredColonyInfoScreen).gameObject, parent, true);
    RetiredColonyInfoScreen.Instance.Show(true);
    RetiredColonyInfoScreen.Instance.LoadColony(data);
  }

  public static void ActivateInventoyScreen() => LockerNavigator.Instance.PushScreen(LockerNavigator.Instance.kleiInventoryScreen);

  public static void ActivateLockerMenu() => ((KScreen) LockerMenuScreen.Instance).Show(true);

  private void SpawnVideoScreen() => VideoScreen.Instance = Util.KInstantiateUI(((Component) ScreenPrefabs.Instance.VideoScreen).gameObject, ((Component) this).gameObject, false).GetComponent<VideoScreen>();

  private void Update()
  {
  }

  public void RefreshResumeButton(bool simpleCheck = false)
  {
    string saveForCurrentDlc = SaveLoader.GetLatestSaveForCurrentDLC();
    bool flag = !string.IsNullOrEmpty(saveForCurrentDlc) && File.Exists(saveForCurrentDlc);
    if (flag)
    {
      try
      {
        if (GenericGameSettings.instance.demoMode)
          flag = false;
        System.DateTime lastWriteTime = File.GetLastWriteTime(saveForCurrentDlc);
        MainMenu.SaveFileEntry saveFileEntry1 = new MainMenu.SaveFileEntry();
        SaveGame.Header header = new SaveGame.Header();
        SaveGame.GameInfo gameInfo1 = new SaveGame.GameInfo();
        SaveGame.GameInfo gameInfo2;
        if (!this.saveFileEntries.TryGetValue(saveForCurrentDlc, out saveFileEntry1) || saveFileEntry1.timeStamp != lastWriteTime)
        {
          gameInfo2 = SaveLoader.LoadHeader(saveForCurrentDlc, out header);
          MainMenu.SaveFileEntry saveFileEntry2 = new MainMenu.SaveFileEntry()
          {
            timeStamp = lastWriteTime,
            header = header,
            headerData = gameInfo2
          };
          this.saveFileEntries[saveForCurrentDlc] = saveFileEntry2;
        }
        else
        {
          header = saveFileEntry1.header;
          gameInfo2 = saveFileEntry1.headerData;
        }
        if (header.buildVersion > 535842U || gameInfo2.saveMajorVersion != 7 || gameInfo2.saveMinorVersion > 31)
          flag = false;
        if (!DlcManager.IsContentActive(gameInfo2.dlcId))
          flag = false;
        string withoutExtension = System.IO.Path.GetFileNameWithoutExtension(saveForCurrentDlc);
        if (!string.IsNullOrEmpty(gameInfo2.baseName))
          ((TMP_Text) ((Component) this.Button_ResumeGame).GetComponentsInChildren<LocText>()[1]).text = string.Format((string) STRINGS.UI.FRONTEND.MAINMENU.RESUMEBUTTON_BASENAME, (object) gameInfo2.baseName, (object) (gameInfo2.numberOfCycles + 1));
        else
          ((TMP_Text) ((Component) this.Button_ResumeGame).GetComponentsInChildren<LocText>()[1]).text = withoutExtension;
      }
      catch (Exception ex)
      {
        Debug.LogWarning((object) ex);
        flag = false;
      }
    }
    if (Object.op_Inequality((Object) this.Button_ResumeGame, (Object) null) && Object.op_Inequality((Object) ((Component) this.Button_ResumeGame).gameObject, (Object) null))
    {
      ((Component) this.Button_ResumeGame).gameObject.SetActive(flag);
      KImage component = ((Component) this.Button_NewGame).GetComponent<KImage>();
      component.colorStyleSetting = flag ? this.normalButtonStyle : this.topButtonStyle;
      component.ApplyColorStyleSetting();
    }
    else
      Debug.LogWarning((object) "Why is the resume game button null?");
  }

  private void Translations() => Util.KInstantiateUI<LanguageOptionsScreen>(((Component) ScreenPrefabs.Instance.languageOptionsScreen).gameObject, ((Component) ((KMonoBehaviour) this).transform.parent).gameObject, false);

  private void Mods() => Util.KInstantiateUI<ModsScreen>(((Component) ScreenPrefabs.Instance.modsMenu).gameObject, ((Component) ((KMonoBehaviour) this).transform.parent).gameObject, false);

  private void Options() => Util.KInstantiateUI<OptionsMenuScreen>(((Component) ScreenPrefabs.Instance.OptionsScreen).gameObject, ((Component) this).gameObject, true);

  private void QuitGame() => App.Quit();

  public void StartFEAudio()
  {
    AudioMixer.instance.Reset();
    MusicManager.instance.KillAllSongs((STOP_MODE) 0);
    AudioMixer.instance.Start(AudioMixerSnapshots.Get().FrontEndSnapshot);
    if (!AudioMixer.instance.SnapshotIsActive(AudioMixerSnapshots.Get().UserVolumeSettingsSnapshot))
      AudioMixer.instance.StartUserVolumesSnapshot();
    if (AudioDebug.Get().musicEnabled && !MusicManager.instance.SongIsPlaying(this.menuMusicEventName))
      MusicManager.instance.PlaySong(this.menuMusicEventName);
    this.CheckForAudioDriverIssue();
  }

  public void StopAmbience()
  {
    if (!((EventInstance) ref this.ambientLoop).isValid())
      return;
    ((EventInstance) ref this.ambientLoop).stop((STOP_MODE) 0);
    ((EventInstance) ref this.ambientLoop).release();
    ((EventInstance) ref this.ambientLoop).clearHandle();
  }

  public void StopMainMenuMusic()
  {
    if (!MusicManager.instance.SongIsPlaying(this.menuMusicEventName))
      return;
    MusicManager.instance.StopSong(this.menuMusicEventName);
    AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndSnapshot);
  }

  private void CheckForAudioDriverIssue()
  {
    if (KFMOD.didFmodInitializeSuccessfully)
      return;
    Util.KInstantiateUI<ConfirmDialogScreen>(((Component) ScreenPrefabs.Instance.ConfirmDialogScreen).gameObject, ((Component) this).gameObject, true).PopupConfirmDialog((string) STRINGS.UI.FRONTEND.SUPPORTWARNINGS.AUDIO_DRIVERS, (System.Action) null, (System.Action) null, (string) STRINGS.UI.FRONTEND.SUPPORTWARNINGS.AUDIO_DRIVERS_MORE_INFO, (System.Action) (() => App.OpenWebURL("http://support.kleientertainment.com/customer/en/portal/articles/2947881-no-audio-when-playing-oxygen-not-included")), image_sprite: GlobalResources.Instance().sadDupeAudio);
  }

  private void CheckPlayerPrefsCorruption()
  {
    if (!KPlayerPrefs.HasCorruptedFlag())
      return;
    KPlayerPrefs.ResetCorruptedFlag();
    Util.KInstantiateUI<ConfirmDialogScreen>(((Component) ScreenPrefabs.Instance.ConfirmDialogScreen).gameObject, ((Component) this).gameObject, true).PopupConfirmDialog((string) STRINGS.UI.FRONTEND.SUPPORTWARNINGS.PLAYER_PREFS_CORRUPTED, (System.Action) null, (System.Action) null, image_sprite: GlobalResources.Instance().sadDupe);
  }

  private void CheckDoubleBoundKeys()
  {
    string str = "";
    HashSet<BindingEntry> bindingEntrySet = new HashSet<BindingEntry>();
    for (int index1 = 0; index1 < GameInputMapping.KeyBindings.Length; ++index1)
    {
      if (GameInputMapping.KeyBindings[index1].mKeyCode != 324)
      {
        for (int index2 = 0; index2 < GameInputMapping.KeyBindings.Length; ++index2)
        {
          if (index1 != index2)
          {
            BindingEntry keyBinding1 = GameInputMapping.KeyBindings[index2];
            if (!bindingEntrySet.Contains(keyBinding1))
            {
              BindingEntry keyBinding2 = GameInputMapping.KeyBindings[index1];
              if (keyBinding2.mKeyCode != null && keyBinding2.mKeyCode == keyBinding1.mKeyCode && keyBinding2.mModifier == keyBinding1.mModifier && keyBinding2.mRebindable && keyBinding1.mRebindable)
              {
                string mGroup1 = GameInputMapping.KeyBindings[index1].mGroup;
                string mGroup2 = GameInputMapping.KeyBindings[index2].mGroup;
                if ((mGroup1 == "Root" || mGroup2 == "Root" || mGroup1 == mGroup2) && (!(mGroup1 == "Root") || !keyBinding1.mIgnoreRootConflics) && (!(mGroup2 == "Root") || !keyBinding2.mIgnoreRootConflics))
                {
                  str = str + "\n\n" + keyBinding2.mAction.ToString() + ": <b>" + keyBinding2.mKeyCode.ToString() + "</b>\n" + keyBinding1.mAction.ToString() + ": <b>" + keyBinding1.mKeyCode.ToString() + "</b>";
                  BindingEntry bindingEntry1 = keyBinding2;
                  bindingEntry1.mKeyCode = (KKeyCode) 0;
                  bindingEntry1.mModifier = (Modifier) 0;
                  GameInputMapping.KeyBindings[index1] = bindingEntry1;
                  BindingEntry bindingEntry2 = keyBinding1;
                  bindingEntry2.mKeyCode = (KKeyCode) 0;
                  bindingEntry2.mModifier = (Modifier) 0;
                  GameInputMapping.KeyBindings[index2] = bindingEntry2;
                }
              }
            }
          }
        }
        bindingEntrySet.Add(GameInputMapping.KeyBindings[index1]);
      }
    }
    if (!(str != ""))
      return;
    Util.KInstantiateUI<ConfirmDialogScreen>(((Component) ScreenPrefabs.Instance.ConfirmDialogScreen).gameObject, ((Component) this).gameObject, true).PopupConfirmDialog(string.Format((string) STRINGS.UI.FRONTEND.SUPPORTWARNINGS.DUPLICATE_KEY_BINDINGS, (object) str), (System.Action) null, (System.Action) null, image_sprite: GlobalResources.Instance().sadDupe);
  }

  private void RestartGame() => App.instance.Restart();

  private struct ButtonInfo
  {
    public LocString text;
    public System.Action action;
    public int fontSize;
    public ColorStyleSetting style;

    public ButtonInfo(LocString text, System.Action action, int font_size, ColorStyleSetting style)
    {
      this.text = text;
      this.action = action;
      this.fontSize = font_size;
      this.style = style;
    }
  }

  private struct SaveFileEntry
  {
    public System.DateTime timeStamp;
    public SaveGame.Header header;
    public SaveGame.GameInfo headerData;
  }
}
