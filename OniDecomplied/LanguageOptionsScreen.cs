// Decompiled with JetBrains decompiler
// Type: LanguageOptionsScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KMod;
using Steamworks;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LanguageOptionsScreen : KModalScreen, SteamUGCService.IClient
{
  private static readonly string[] poFile = new string[1]
  {
    "strings.po"
  };
  public const string KPLAYER_PREFS_LANGUAGE_KEY = "InstalledLanguage";
  public const string TAG_LANGUAGE = "language";
  public KButton textButton;
  public KButton dismissButton;
  public KButton closeButton;
  public KButton workshopButton;
  public KButton uninstallButton;
  [Space]
  public GameObject languageButtonPrefab;
  public GameObject preinstalledLanguagesTitle;
  public GameObject preinstalledLanguagesContainer;
  public GameObject ugcLanguagesTitle;
  public GameObject ugcLanguagesContainer;
  private List<GameObject> buttons = new List<GameObject>();
  private string _currentLanguageModId;
  private System.DateTime currentLastModified;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.dismissButton.onClick += new System.Action(((KScreen) this).Deactivate);
    ((TMP_Text) ((Component) this.dismissButton).GetComponent<HierarchyReferences>().GetReference<LocText>("Title")).SetText((string) STRINGS.UI.FRONTEND.OPTIONS_SCREEN.BACK);
    this.closeButton.onClick += new System.Action(((KScreen) this).Deactivate);
    this.workshopButton.onClick += (System.Action) (() => this.OnClickOpenWorkshop());
    this.uninstallButton.onClick += (System.Action) (() => this.OnClickUninstall());
    ((Component) this.uninstallButton).gameObject.SetActive(false);
    this.RebuildScreen();
  }

  private void RebuildScreen()
  {
    foreach (Object button in this.buttons)
      Object.Destroy(button);
    this.buttons.Clear();
    this.uninstallButton.isInteractable = KPlayerPrefs.GetString(Localization.SELECTED_LANGUAGE_TYPE_KEY, Localization.SelectedLanguageType.None.ToString()) != Localization.SelectedLanguageType.None.ToString();
    this.RebuildPreinstalledButtons();
    this.RebuildUGCButtons();
  }

  private void RebuildPreinstalledButtons()
  {
    foreach (string preinstalledLanguage in Localization.PreinstalledLanguages)
    {
      string code = preinstalledLanguage;
      if (!(code != Localization.DEFAULT_LANGUAGE_CODE) || File.Exists(Localization.GetPreinstalledLocalizationFilePath(code)))
      {
        GameObject gameObject = Util.KInstantiateUI(this.languageButtonPrefab, this.preinstalledLanguagesContainer, false);
        ((Object) gameObject).name = code + "_button";
        HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
        LocText reference = component.GetReference<LocText>("Title");
        ((TMP_Text) reference).text = Localization.GetPreinstalledLocalizationTitle(code);
        ((Behaviour) reference).enabled = false;
        ((Behaviour) reference).enabled = true;
        Texture2D localizationImage = Localization.GetPreinstalledLocalizationImage(code);
        if (Object.op_Inequality((Object) localizationImage, (Object) null))
          component.GetReference<Image>("Image").sprite = Sprite.Create(localizationImage, new Rect(Vector2.zero, new Vector2((float) ((Texture) localizationImage).width, (float) ((Texture) localizationImage).height)), Vector2.op_Multiply(Vector2.one, 0.5f));
        gameObject.GetComponent<KButton>().onClick += (System.Action) (() => this.ConfirmLanguagePreinstalledOrMod(code != Localization.DEFAULT_LANGUAGE_CODE ? code : string.Empty, (string) null));
        this.buttons.Add(gameObject);
      }
    }
  }

  protected override void OnActivate()
  {
    base.OnActivate();
    Global.Instance.modManager.Sanitize(((Component) this).gameObject);
    if (!Object.op_Inequality((Object) SteamUGCService.Instance, (Object) null))
      return;
    SteamUGCService.Instance.AddClient((SteamUGCService.IClient) this);
  }

  protected override void OnDeactivate()
  {
    base.OnDeactivate();
    if (!Object.op_Inequality((Object) SteamUGCService.Instance, (Object) null))
      return;
    SteamUGCService.Instance.RemoveClient((SteamUGCService.IClient) this);
  }

  private void ConfirmLanguageChoiceDialog(
    string[] lines,
    bool is_template,
    System.Action install_language)
  {
    Localization.Locale locale = Localization.GetLocale(lines);
    Dictionary<string, string> translated_strings = Localization.ExtractTranslatedStrings(lines, is_template);
    TMP_FontAsset font = Localization.GetFont(locale.FontName);
    ConfirmDialogScreen screen = this.GetConfirmDialog();
    HashSet<MemberInfo> excluded_members = new HashSet<MemberInfo>((IEnumerable<MemberInfo>) typeof (ConfirmDialogScreen).GetMember("cancelButton", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy));
    Localization.SetFont<ConfirmDialogScreen>(screen, font, locale.IsRightToLeft, excluded_members);
    string str1;
    Func<LocString, string> func = (Func<LocString, string>) (loc_string => !translated_strings.TryGetValue(loc_string.key.String, out str1) ? (string) loc_string : str1);
    ConfirmDialogScreen confirmDialogScreen = screen;
    string str2 = func(STRINGS.UI.CONFIRMDIALOG.DIALOG_HEADER);
    string text = func(STRINGS.UI.FRONTEND.TRANSLATIONS_SCREEN.PLEASE_REBOOT);
    System.Action on_confirm = (System.Action) (() =>
    {
      LanguageOptionsScreen.CleanUpSavedLanguageMod();
      install_language();
      App.instance.Restart();
    });
    System.Action on_cancel = (System.Action) (() => Localization.SetFont<ConfirmDialogScreen>(screen, Localization.FontAsset, Localization.IsRightToLeft, excluded_members));
    string title_text = str2;
    string confirm_text = func(STRINGS.UI.FRONTEND.TRANSLATIONS_SCREEN.RESTART);
    string cancel = (string) STRINGS.UI.FRONTEND.TRANSLATIONS_SCREEN.CANCEL;
    confirmDialogScreen.PopupConfirmDialog(text, on_confirm, on_cancel, title_text: title_text, confirm_text: confirm_text, cancel_text: cancel);
  }

  private void ConfirmPreinstalledLanguage(string selected_preinstalled_translation)
  {
    int selectedLanguageType = (int) Localization.GetSelectedLanguageType();
  }

  private void ConfirmLanguagePreinstalledOrMod(
    string selected_preinstalled_translation,
    string mod_id)
  {
    Localization.SelectedLanguageType selectedLanguageType = Localization.GetSelectedLanguageType();
    if (mod_id != null)
    {
      if (selectedLanguageType == Localization.SelectedLanguageType.UGC && mod_id == this.currentLanguageModId)
        this.Deactivate();
      else
        this.ConfirmLanguageChoiceDialog(LanguageOptionsScreen.GetLanguageLinesForMod(mod_id), false, (System.Action) (() => LanguageOptionsScreen.SetCurrentLanguage(mod_id)));
    }
    else if (!string.IsNullOrEmpty(selected_preinstalled_translation))
    {
      string currentLanguageCode = Localization.GetCurrentLanguageCode();
      if (selectedLanguageType == Localization.SelectedLanguageType.Preinstalled && currentLanguageCode == selected_preinstalled_translation)
        this.Deactivate();
      else
        this.ConfirmLanguageChoiceDialog(File.ReadAllLines(Localization.GetPreinstalledLocalizationFilePath(selected_preinstalled_translation), Encoding.UTF8), false, (System.Action) (() => Localization.LoadPreinstalledTranslation(selected_preinstalled_translation)));
    }
    else if (selectedLanguageType == Localization.SelectedLanguageType.None)
      this.Deactivate();
    else
      this.ConfirmLanguageChoiceDialog(File.ReadAllLines(Localization.GetDefaultLocalizationFilePath(), Encoding.UTF8), true, (System.Action) (() => Localization.ClearLanguage()));
  }

  private ConfirmDialogScreen GetConfirmDialog()
  {
    KScreen component = KScreenManager.AddChild(((Component) ((KMonoBehaviour) this).transform.parent).gameObject, ((Component) ScreenPrefabs.Instance.ConfirmDialogScreen).gameObject).GetComponent<KScreen>();
    component.Activate();
    return ((Component) component).GetComponent<ConfirmDialogScreen>();
  }

  public override void OnKeyDown(KButtonEvent e)
  {
    if (((KInputEvent) e).Consumed)
      return;
    if (e.TryConsume((Action) 5))
      this.Deactivate();
    base.OnKeyDown(e);
  }

  private void RebuildUGCButtons()
  {
    foreach (KMod.Mod mod in Global.Instance.modManager.mods)
    {
      if ((mod.available_content & Content.Translation) != (Content) 0 && mod.status == KMod.Mod.Status.Installed)
      {
        GameObject gameObject = Util.KInstantiateUI(this.languageButtonPrefab, this.ugcLanguagesContainer, false);
        ((Object) gameObject).name = mod.title + "_button";
        HierarchyReferences component = gameObject.GetComponent<HierarchyReferences>();
        TMP_FontAsset font = Localization.GetFont(Localization.GetFontName(LanguageOptionsScreen.GetLanguageLinesForMod(mod)));
        LocText reference = component.GetReference<LocText>("Title");
        ((TMP_Text) reference).SetText(string.Format((string) STRINGS.UI.FRONTEND.TRANSLATIONS_SCREEN.UGC_MOD_TITLE_FORMAT, (object) mod.title));
        ((TMP_Text) reference).font = font;
        Texture2D previewImage = mod.GetPreviewImage();
        if (Object.op_Inequality((Object) previewImage, (Object) null))
          component.GetReference<Image>("Image").sprite = Sprite.Create(previewImage, new Rect(Vector2.zero, new Vector2((float) ((Texture) previewImage).width, (float) ((Texture) previewImage).height)), Vector2.op_Multiply(Vector2.one, 0.5f));
        string mod_id = mod.label.id;
        gameObject.GetComponent<KButton>().onClick += (System.Action) (() => this.ConfirmLanguagePreinstalledOrMod(string.Empty, mod_id));
        this.buttons.Add(gameObject);
      }
    }
  }

  private void Uninstall() => this.GetConfirmDialog().PopupConfirmDialog((string) STRINGS.UI.FRONTEND.TRANSLATIONS_SCREEN.ARE_YOU_SURE, (System.Action) (() =>
  {
    Localization.ClearLanguage();
    this.GetConfirmDialog().PopupConfirmDialog((string) STRINGS.UI.FRONTEND.TRANSLATIONS_SCREEN.PLEASE_REBOOT, new System.Action(App.instance.Restart), new System.Action(((KScreen) this).Deactivate));
  }), (System.Action) (() => { }));

  private void OnClickUninstall() => this.Uninstall();

  private void OnClickOpenWorkshop() => App.OpenWebURL("http://steamcommunity.com/workshop/browse/?appid=457140&requiredtags[]=language");

  public void UpdateMods(
    IEnumerable<PublishedFileId_t> added,
    IEnumerable<PublishedFileId_t> updated,
    IEnumerable<PublishedFileId_t> removed,
    IEnumerable<SteamUGCService.Mod> loaded_previews)
  {
    string savedLanguageMod = LanguageOptionsScreen.GetSavedLanguageMod();
    ulong result;
    if (ulong.TryParse(savedLanguageMod, out result))
    {
      PublishedFileId_t publishedFileIdT = PublishedFileId_t.op_Explicit(result);
      if (removed.Contains<PublishedFileId_t>(publishedFileIdT))
      {
        Debug.Log((object) ("Unsubscribe detected for currently installed language mod [" + savedLanguageMod + "]"));
        this.GetConfirmDialog().PopupConfirmDialog((string) STRINGS.UI.FRONTEND.TRANSLATIONS_SCREEN.PLEASE_REBOOT, (System.Action) (() =>
        {
          Localization.ClearLanguage();
          App.instance.Restart();
        }), (System.Action) null, confirm_text: ((string) STRINGS.UI.FRONTEND.TRANSLATIONS_SCREEN.RESTART));
      }
      if (updated.Contains<PublishedFileId_t>(publishedFileIdT))
        Debug.Log((object) ("Download complete for currently installed language [" + savedLanguageMod + "] updating in background. Changes will happen next restart."));
    }
    this.RebuildScreen();
  }

  public static string GetSavedLanguageMod() => KPlayerPrefs.GetString("InstalledLanguage");

  public static void SetSavedLanguageMod(string mod_id) => KPlayerPrefs.SetString("InstalledLanguage", mod_id);

  public static void CleanUpSavedLanguageMod() => KPlayerPrefs.SetString("InstalledLanguage", (string) null);

  public string currentLanguageModId
  {
    get => this._currentLanguageModId;
    private set
    {
      this._currentLanguageModId = value;
      LanguageOptionsScreen.SetSavedLanguageMod(this._currentLanguageModId);
    }
  }

  public static bool SetCurrentLanguage(string mod_id)
  {
    LanguageOptionsScreen.CleanUpSavedLanguageMod();
    if (!LanguageOptionsScreen.LoadTranslation(mod_id))
      return false;
    LanguageOptionsScreen.SetSavedLanguageMod(mod_id);
    return true;
  }

  public static bool HasInstalledLanguage()
  {
    string currentModId = LanguageOptionsScreen.GetSavedLanguageMod();
    if (currentModId == null)
      return false;
    if (Global.Instance.modManager.mods.Find((Predicate<KMod.Mod>) (m => m.label.id == currentModId)) != null)
      return true;
    LanguageOptionsScreen.CleanUpSavedLanguageMod();
    return false;
  }

  public static string GetInstalledLanguageCode()
  {
    string installedLanguageCode = "";
    string[] languageLinesForMod = LanguageOptionsScreen.GetLanguageLinesForMod(LanguageOptionsScreen.GetSavedLanguageMod());
    if (languageLinesForMod != null)
    {
      Localization.Locale locale = Localization.GetLocale(languageLinesForMod);
      if (locale != null)
        installedLanguageCode = locale.Code;
    }
    return installedLanguageCode;
  }

  private static bool LoadTranslation(string mod_id)
  {
    KMod.Mod mod = Global.Instance.modManager.mods.Find((Predicate<KMod.Mod>) (m => m.label.id == mod_id));
    if (mod == null)
    {
      Debug.LogWarning((object) ("Tried loading a translation from a non-existent mod id: " + mod_id));
      return false;
    }
    string languageFilename = LanguageOptionsScreen.GetLanguageFilename(mod);
    return languageFilename != null && Localization.LoadLocalTranslationFile(Localization.SelectedLanguageType.UGC, languageFilename);
  }

  private static string GetLanguageFilename(KMod.Mod mod)
  {
    Debug.Assert(mod.content_source.GetType() == typeof (KMod.Directory), (object) "Can only load translations from extracted mods.");
    string path = System.IO.Path.Combine(mod.ContentPath, "strings.po");
    if (File.Exists(path))
      return path;
    Debug.LogWarning((object) ("GetLanguagFile: " + path + " missing for mod " + mod.label.title));
    return (string) null;
  }

  private static string[] GetLanguageLinesForMod(string mod_id) => LanguageOptionsScreen.GetLanguageLinesForMod(Global.Instance.modManager.mods.Find((Predicate<KMod.Mod>) (m => m.label.id == mod_id)));

  private static string[] GetLanguageLinesForMod(KMod.Mod mod)
  {
    string languageFilename = LanguageOptionsScreen.GetLanguageFilename(mod);
    if (languageFilename == null)
      return (string[]) null;
    string[] languageLinesForMod = File.ReadAllLines(languageFilename, Encoding.UTF8);
    if (languageLinesForMod != null && languageLinesForMod.Length != 0)
      return languageLinesForMod;
    Debug.LogWarning((object) ("Couldn't find any strings in the translation mod " + mod.label.title));
    return (string[]) null;
  }
}
