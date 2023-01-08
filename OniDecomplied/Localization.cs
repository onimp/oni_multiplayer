// Decompiled with JetBrains decompiler
// Type: Localization
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ArabicSupport;
using Klei;
using Steamworks;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public static class Localization
{
  private static TMP_FontAsset sFontAsset = (TMP_FontAsset) null;
  private static readonly List<Localization.Locale> Locales = new List<Localization.Locale>()
  {
    new Localization.Locale(Localization.Language.Chinese, Localization.Direction.LeftToRight, "zh", "NotoSansCJKsc-Regular"),
    new Localization.Locale(Localization.Language.Japanese, Localization.Direction.LeftToRight, "ja", "NotoSansCJKjp-Regular"),
    new Localization.Locale(Localization.Language.Korean, Localization.Direction.LeftToRight, "ko", "NotoSansCJKkr-Regular"),
    new Localization.Locale(Localization.Language.Russian, Localization.Direction.LeftToRight, "ru", "RobotoCondensed-Regular"),
    new Localization.Locale(Localization.Language.Thai, Localization.Direction.LeftToRight, "th", "NotoSansThai-Regular"),
    new Localization.Locale(Localization.Language.Arabic, Localization.Direction.RightToLeft, "ar", "NotoNaskhArabic-Regular"),
    new Localization.Locale(Localization.Language.Hebrew, Localization.Direction.RightToLeft, "he", "NotoSansHebrew-Regular"),
    new Localization.Locale(Localization.Language.Unspecified, Localization.Direction.LeftToRight, "", "RobotoCondensed-Regular")
  };
  private static Localization.Locale sLocale = (Localization.Locale) null;
  private static string currentFontName = (string) null;
  public static string DEFAULT_LANGUAGE_CODE = "en";
  public static readonly List<string> PreinstalledLanguages = new List<string>()
  {
    Localization.DEFAULT_LANGUAGE_CODE,
    "zh_klei",
    "ko_klei",
    "ru_klei"
  };
  public static string SELECTED_LANGUAGE_TYPE_KEY = "SelectedLanguageType";
  public static string SELECTED_LANGUAGE_CODE_KEY = "SelectedLanguageCode";
  private static Dictionary<string, List<Assembly>> translatable_assemblies = new Dictionary<string, List<Assembly>>();
  public const BindingFlags non_static_data_member_fields = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
  private const string start_link_token = "<link";
  private const string end_link_token = "</link";

  public static TMP_FontAsset FontAsset => Localization.sFontAsset;

  public static bool IsRightToLeft => Localization.sLocale != null && Localization.sLocale.IsRightToLeft;

  private static IEnumerable<System.Type> CollectLocStringTreeRoots(
    string locstrings_namespace,
    Assembly assembly)
  {
    return ((IEnumerable<System.Type>) assembly.GetTypes()).Where<System.Type>((Func<System.Type, bool>) (type => type.IsClass && type.Namespace == locstrings_namespace && !type.IsNested));
  }

  private static Dictionary<string, object> MakeRuntimeLocStringTree(System.Type locstring_tree_root)
  {
    Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
    foreach (FieldInfo field in locstring_tree_root.GetFields())
    {
      if (!(field.FieldType != typeof (LocString)))
      {
        LocString locString = (LocString) field.GetValue((object) null);
        if (locString == null)
          Debug.LogError((object) ("Tried to generate LocString for " + field.Name + " but it is null so skipping"));
        else
          dictionary1[field.Name] = (object) locString.text;
      }
    }
    foreach (System.Type nestedType in locstring_tree_root.GetNestedTypes())
    {
      Dictionary<string, object> dictionary2 = Localization.MakeRuntimeLocStringTree(nestedType);
      if (dictionary2.Count > 0)
        dictionary1[nestedType.Name] = (object) dictionary2;
    }
    return dictionary1;
  }

  private static void WriteStringsTemplate(
    string path,
    StreamWriter writer,
    Dictionary<string, object> runtime_locstring_tree)
  {
    List<string> stringList = new List<string>((IEnumerable<string>) runtime_locstring_tree.Keys);
    stringList.Sort();
    foreach (string key in stringList)
    {
      string path1 = path + "." + key;
      object runtime_locstring_tree1 = runtime_locstring_tree[key];
      if (runtime_locstring_tree1.GetType() != typeof (string))
      {
        Localization.WriteStringsTemplate(path1, writer, runtime_locstring_tree1 as Dictionary<string, object>);
      }
      else
      {
        string str = (runtime_locstring_tree1 as string).Replace("\\", "\\\\").Replace("\"", "\\\"").Replace("\n", "\\n");
        writer.WriteLine("#. " + path1);
        writer.WriteLine("msgctxt \"{0}\"", (object) path1);
        writer.WriteLine("msgid \"" + str + "\"");
        writer.WriteLine("msgstr \"\"");
        writer.WriteLine("");
      }
    }
  }

  public static void GenerateStringsTemplate(
    string locstrings_namespace,
    Assembly assembly,
    string output_filename,
    Dictionary<string, object> current_runtime_locstring_forest)
  {
    Dictionary<string, object> dictionary1 = new Dictionary<string, object>();
    foreach (System.Type locStringTreeRoot in Localization.CollectLocStringTreeRoots(locstrings_namespace, assembly))
    {
      Dictionary<string, object> dictionary2 = Localization.MakeRuntimeLocStringTree(locStringTreeRoot);
      if (dictionary2.Count > 0)
        dictionary1[locStringTreeRoot.Name] = (object) dictionary2;
    }
    if (current_runtime_locstring_forest != null)
      dictionary1.Concat<KeyValuePair<string, object>>((IEnumerable<KeyValuePair<string, object>>) current_runtime_locstring_forest);
    using (StreamWriter writer = new StreamWriter(output_filename, false, (Encoding) new UTF8Encoding(false)))
    {
      writer.WriteLine("msgid \"\"");
      writer.WriteLine("msgstr \"\"");
      writer.WriteLine("\"Application: Oxygen Not Included\"");
      writer.WriteLine("\"POT Version: 2.0\"");
      writer.WriteLine("");
      Localization.WriteStringsTemplate(locstrings_namespace, writer, dictionary1);
    }
    DebugUtil.LogArgs(new object[1]
    {
      (object) ("Generated " + output_filename)
    });
  }

  public static void GenerateStringsTemplate(System.Type locstring_tree_root, string output_folder)
  {
    output_folder = FileSystem.Normalize(output_folder);
    if (!FileUtil.CreateDirectory(output_folder, 5))
      return;
    Localization.GenerateStringsTemplate(locstring_tree_root.Namespace, Assembly.GetAssembly(locstring_tree_root), FileSystem.Normalize(System.IO.Path.Combine(output_folder, string.Format("{0}_template.pot", (object) locstring_tree_root.Namespace.ToLower()))), (Dictionary<string, object>) null);
  }

  public static void Initialize()
  {
    DebugUtil.LogArgs(new object[1]
    {
      (object) "Localization.Initialize!"
    });
    bool flag = false;
    switch (Localization.GetSelectedLanguageType())
    {
      case Localization.SelectedLanguageType.None:
        Localization.sFontAsset = Localization.GetFont(Localization.GetDefaultLocale().FontName);
        break;
      case Localization.SelectedLanguageType.Preinstalled:
        string currentLanguageCode = Localization.GetCurrentLanguageCode();
        if (!string.IsNullOrEmpty(currentLanguageCode))
        {
          DebugUtil.LogArgs(new object[1]
          {
            (object) "Localization Initialize... Preinstalled localization"
          });
          DebugUtil.LogArgs(new object[2]
          {
            (object) " -> ",
            (object) currentLanguageCode
          });
          Localization.LoadPreinstalledTranslation(currentLanguageCode);
          break;
        }
        flag = true;
        break;
      case Localization.SelectedLanguageType.UGC:
        if (LanguageOptionsScreen.HasInstalledLanguage())
        {
          DebugUtil.LogArgs(new object[1]
          {
            (object) "Localization Initialize... Mod-based localization"
          });
          string savedLanguageMod = LanguageOptionsScreen.GetSavedLanguageMod();
          if (LanguageOptionsScreen.SetCurrentLanguage(savedLanguageMod))
          {
            DebugUtil.LogArgs(new object[1]
            {
              (object) (" -> Loaded language from mod: " + savedLanguageMod)
            });
            break;
          }
          DebugUtil.LogArgs(new object[1]
          {
            (object) (" -> Failed to load language from mod: " + savedLanguageMod)
          });
          break;
        }
        flag = true;
        break;
    }
    if (!flag)
      return;
    Localization.ClearLanguage();
  }

  public static void VerifyTranslationModSubscription(GameObject context)
  {
    if (Localization.GetSelectedLanguageType() != Localization.SelectedLanguageType.UGC || !SteamManager.Initialized || LanguageOptionsScreen.HasInstalledLanguage())
      return;
    PublishedFileId_t publishedFileIdT;
    // ISSUE: explicit constructor call
    ((PublishedFileId_t) ref publishedFileIdT).\u002Ector((ulong) (uint) KPlayerPrefs.GetInt("InstalledLanguage", (int) PublishedFileId_t.Invalid.m_PublishedFileId));
    KMod.Label rhs = new KMod.Label()
    {
      distribution_platform = KMod.Label.DistributionPlatform.Steam,
      id = publishedFileIdT.ToString()
    };
    string str = (string) UI.FRONTEND.TRANSLATIONS_SCREEN.UNKNOWN;
    foreach (KMod.Mod mod in Global.Instance.modManager.mods)
    {
      if (mod.label.Match(rhs))
      {
        str = mod.title;
        break;
      }
    }
    Localization.ClearLanguage();
    KScreen component1 = KScreenManager.AddChild(context, ((Component) ScreenPrefabs.Instance.ConfirmDialogScreen).gameObject).GetComponent<KScreen>();
    component1.Activate();
    ConfirmDialogScreen component2 = ((Component) component1).GetComponent<ConfirmDialogScreen>();
    string dialogHeader = (string) UI.CONFIRMDIALOG.DIALOG_HEADER;
    string text = string.Format((string) UI.FRONTEND.TRANSLATIONS_SCREEN.MISSING_LANGUAGE_PACK, (object) str);
    string restart = (string) UI.FRONTEND.TRANSLATIONS_SCREEN.RESTART;
    System.Action on_confirm = new System.Action(App.instance.Restart);
    string title_text = dialogHeader;
    string confirm_text = restart;
    component2.PopupConfirmDialog(text, on_confirm, (System.Action) null, title_text: title_text, confirm_text: confirm_text);
  }

  public static void LoadPreinstalledTranslation(string code)
  {
    if (!string.IsNullOrEmpty(code) && code != Localization.DEFAULT_LANGUAGE_CODE)
    {
      if (!Localization.LoadLocalTranslationFile(Localization.SelectedLanguageType.Preinstalled, Localization.GetPreinstalledLocalizationFilePath(code)))
        return;
      KPlayerPrefs.SetString(Localization.SELECTED_LANGUAGE_CODE_KEY, code);
    }
    else
      Localization.ClearLanguage();
  }

  public static bool LoadLocalTranslationFile(Localization.SelectedLanguageType source, string path)
  {
    if (!File.Exists(path))
      return false;
    int num = Localization.LoadTranslationFromLines(File.ReadAllLines(path, Encoding.UTF8)) ? 1 : 0;
    if (num != 0)
    {
      KPlayerPrefs.SetString(Localization.SELECTED_LANGUAGE_TYPE_KEY, source.ToString());
      return num != 0;
    }
    Localization.ClearLanguage();
    return num != 0;
  }

  private static bool LoadTranslationFromLines(string[] lines)
  {
    if (lines == null || lines.Length == 0)
      return false;
    Localization.sLocale = Localization.GetLocale(lines);
    DebugUtil.LogArgs(new object[2]
    {
      (object) " -> Locale is now ",
      (object) Localization.sLocale.ToString()
    });
    int num = Localization.LoadTranslation(lines) ? 1 : 0;
    if (num == 0)
      return num != 0;
    Localization.currentFontName = Localization.GetFontName(lines);
    Localization.SwapToLocalizedFont(Localization.currentFontName);
    return num != 0;
  }

  public static bool LoadTranslation(string[] lines, bool isTemplate = false)
  {
    try
    {
      Localization.OverloadStrings(Localization.ExtractTranslatedStrings(lines, isTemplate));
      return true;
    }
    catch (Exception ex)
    {
      DebugUtil.LogWarningArgs(new object[1]{ (object) ex });
      return false;
    }
  }

  public static Dictionary<string, string> LoadStringsFile(string path, bool isTemplate) => Localization.ExtractTranslatedStrings(File.ReadAllLines(path, Encoding.UTF8), isTemplate);

  public static Dictionary<string, string> ExtractTranslatedStrings(string[] lines, bool isTemplate = false)
  {
    Dictionary<string, string> translatedStrings = new Dictionary<string, string>();
    Localization.Entry entry = new Localization.Entry();
    string key = isTemplate ? "msgid" : "msgstr";
    for (int idx = 0; idx < lines.Length; ++idx)
    {
      string line = lines[idx];
      if (line == null || line.Length == 0)
      {
        entry = new Localization.Entry();
      }
      else
      {
        string parameter1 = Localization.GetParameter("msgctxt", idx, lines);
        if (parameter1 != null)
          entry.msgctxt = parameter1;
        string parameter2 = Localization.GetParameter(key, idx, lines);
        if (parameter2 != null)
          entry.msgstr = parameter2;
      }
      if (entry.IsPopulated)
      {
        translatedStrings[entry.msgctxt] = entry.msgstr;
        entry = new Localization.Entry();
      }
    }
    return translatedStrings;
  }

  private static string FixupString(string result)
  {
    result = result.Replace("\\n", "\n");
    result = result.Replace("\\\"", "\"");
    result = result.Replace("<style=“", "<style=\"");
    result = result.Replace("”>", "\">");
    result = result.Replace("<color=^p", "<color=#");
    return result;
  }

  private static string GetParameter(string key, int idx, string[] all_lines)
  {
    if (!all_lines[idx].StartsWith(key))
      return (string) null;
    List<string> stringList = new List<string>();
    string allLine1 = all_lines[idx];
    string str1 = allLine1.Substring(key.Length + 1, allLine1.Length - key.Length - 1);
    stringList.Add(str1);
    for (int index = idx + 1; index < all_lines.Length; ++index)
    {
      string allLine2 = all_lines[index];
      if (allLine2.StartsWith("\""))
        stringList.Add(allLine2);
      else
        break;
    }
    string parameter = "";
    foreach (string str2 in stringList)
    {
      string str3 = str2;
      if (str3.EndsWith("\r"))
        str3 = str3.Substring(0, str3.Length - 1);
      string str4 = Localization.FixupString(str3.Substring(1, str3.Length - 2));
      parameter += str4;
    }
    return parameter;
  }

  private static void AddAssembly(string locstrings_namespace, Assembly assembly)
  {
    List<Assembly> assemblyList;
    if (!Localization.translatable_assemblies.TryGetValue(locstrings_namespace, out assemblyList))
    {
      assemblyList = new List<Assembly>();
      Localization.translatable_assemblies.Add(locstrings_namespace, assemblyList);
    }
    assemblyList.Add(assembly);
  }

  public static void AddAssembly(Assembly assembly) => Localization.AddAssembly("STRINGS", assembly);

  public static void RegisterForTranslation(System.Type locstring_tree_root)
  {
    Assembly assembly = Assembly.GetAssembly(locstring_tree_root);
    Localization.AddAssembly(locstring_tree_root.Namespace, assembly);
    string parent_path = locstring_tree_root.Namespace + ".";
    foreach (System.Type locStringTreeRoot in Localization.CollectLocStringTreeRoots(locstring_tree_root.Namespace, assembly))
      LocString.CreateLocStringKeys(locStringTreeRoot, parent_path);
  }

  public static void OverloadStrings(Dictionary<string, string> translated_strings)
  {
    string parameter_errors = "";
    string link_errors = "";
    string link_count_errors = "";
    foreach (KeyValuePair<string, List<Assembly>> translatableAssembly in Localization.translatable_assemblies)
    {
      foreach (Assembly assembly in translatableAssembly.Value)
      {
        foreach (System.Type locStringTreeRoot in Localization.CollectLocStringTreeRoots(translatableAssembly.Key, assembly))
        {
          string path = translatableAssembly.Key + "." + locStringTreeRoot.Name;
          Localization.OverloadStrings(translated_strings, path, locStringTreeRoot, ref parameter_errors, ref link_errors, ref link_count_errors);
        }
      }
    }
    if (!string.IsNullOrEmpty(parameter_errors))
      DebugUtil.LogArgs(new object[1]
      {
        (object) ("TRANSLATION ERROR! The following have missing or mismatched parameters:\n" + parameter_errors)
      });
    if (!string.IsNullOrEmpty(link_errors))
      DebugUtil.LogArgs(new object[1]
      {
        (object) ("TRANSLATION ERROR! The following have mismatched <link> tags:\n" + link_errors)
      });
    if (string.IsNullOrEmpty(link_count_errors))
      return;
    DebugUtil.LogArgs(new object[1]
    {
      (object) ("TRANSLATION ERROR! The following do not have the same amount of <link> tags as the english string which can cause nested link errors:\n" + link_count_errors)
    });
  }

  public static void OverloadStrings(
    Dictionary<string, string> translated_strings,
    string path,
    System.Type locstring_hierarchy,
    ref string parameter_errors,
    ref string link_errors,
    ref string link_count_errors)
  {
    foreach (FieldInfo field in locstring_hierarchy.GetFields())
    {
      if (!(field.FieldType != typeof (LocString)))
      {
        string str1 = path + "." + field.Name;
        string str2 = (string) null;
        if (translated_strings.TryGetValue(str1, out str2))
        {
          LocString locString1 = (LocString) field.GetValue((object) null);
          LocString locString2 = new LocString(str2, str1);
          if (!Localization.AreParametersPreserved(locString1.text, str2))
            parameter_errors = parameter_errors + "\t" + str1 + "\n";
          else if (!Localization.HasSameOrLessLinkCountAsEnglish(locString1.text, str2))
            link_count_errors = link_count_errors + "\t" + str1 + "\n";
          else if (!Localization.HasMatchingLinkTags(str2))
            link_errors = link_errors + "\t" + str1 + "\n";
          else
            field.SetValue((object) null, (object) locString2);
        }
      }
    }
    foreach (System.Type nestedType in locstring_hierarchy.GetNestedTypes())
    {
      string path1 = path + "." + nestedType.Name;
      Localization.OverloadStrings(translated_strings, path1, nestedType, ref parameter_errors, ref link_errors, ref link_count_errors);
    }
  }

  public static string GetDefaultLocalizationFilePath() => System.IO.Path.Combine(Application.streamingAssetsPath, "strings/strings_template.pot");

  public static string GetModLocalizationFilePath() => System.IO.Path.Combine(Application.streamingAssetsPath, "strings/strings.po");

  public static string GetPreinstalledLocalizationFilePath(string code) => System.IO.Path.Combine(Application.streamingAssetsPath, "strings/strings_preinstalled_" + code + ".po");

  public static string GetPreinstalledLocalizationTitle(string code) => StringEntry.op_Implicit(Strings.Get("STRINGS.UI.FRONTEND.TRANSLATIONS_SCREEN.PREINSTALLED_LANGUAGES." + code.ToUpper()));

  public static Texture2D GetPreinstalledLocalizationImage(string code)
  {
    string path = System.IO.Path.Combine(Application.streamingAssetsPath, "strings/preinstalled_icon_" + code + ".png");
    if (!File.Exists(path))
      return (Texture2D) null;
    byte[] numArray = File.ReadAllBytes(path);
    Texture2D localizationImage = new Texture2D(2, 2);
    ImageConversion.LoadImage(localizationImage, numArray);
    return localizationImage;
  }

  public static void SetLocale(Localization.Locale locale)
  {
    Localization.sLocale = locale;
    DebugUtil.LogArgs(new object[2]
    {
      (object) " -> Locale is now ",
      (object) Localization.sLocale.ToString()
    });
  }

  public static Localization.Locale GetLocale() => Localization.sLocale;

  private static string GetFontParam(string line)
  {
    string fontParam = (string) null;
    if (line.StartsWith("\"Font:"))
      fontParam = line.Substring("\"Font:".Length).Trim().Replace("\\n", "").Replace("\"", "");
    return fontParam;
  }

  public static string GetCurrentLanguageCode()
  {
    switch (Localization.GetSelectedLanguageType())
    {
      case Localization.SelectedLanguageType.None:
        return Localization.DEFAULT_LANGUAGE_CODE;
      case Localization.SelectedLanguageType.Preinstalled:
        return KPlayerPrefs.GetString(Localization.SELECTED_LANGUAGE_CODE_KEY);
      case Localization.SelectedLanguageType.UGC:
        return LanguageOptionsScreen.GetInstalledLanguageCode();
      default:
        return "";
    }
  }

  public static Localization.SelectedLanguageType GetSelectedLanguageType() => (Localization.SelectedLanguageType) Enum.Parse(typeof (Localization.SelectedLanguageType), KPlayerPrefs.GetString(Localization.SELECTED_LANGUAGE_TYPE_KEY, Localization.SelectedLanguageType.None.ToString()), true);

  private static string GetLanguageCode(string line)
  {
    string languageCode = (string) null;
    if (line.StartsWith("\"Language:"))
      languageCode = line.Substring("\"Language:".Length).Trim().Replace("\\n", "").Replace("\"", "");
    return languageCode;
  }

  private static Localization.Locale GetLocaleForCode(string code)
  {
    Localization.Locale localeForCode = (Localization.Locale) null;
    foreach (Localization.Locale locale in Localization.Locales)
    {
      if (locale.MatchesCode(code))
      {
        localeForCode = locale;
        break;
      }
    }
    return localeForCode;
  }

  public static Localization.Locale GetLocale(string[] lines)
  {
    Localization.Locale locale = (Localization.Locale) null;
    string code = (string) null;
    if (lines != null && lines.Length != 0)
    {
      foreach (string line in lines)
      {
        if (line != null && line.Length != 0)
        {
          code = Localization.GetLanguageCode(line);
          if (code != null)
            locale = Localization.GetLocaleForCode(code);
          if (code != null)
            break;
        }
      }
    }
    if (locale == null)
      locale = Localization.GetDefaultLocale();
    if (code != null && locale.Code == "")
      locale.SetCode(code);
    return locale;
  }

  private static string GetFontName(string filename) => Localization.GetFontName(File.ReadAllLines(filename, Encoding.UTF8));

  public static Localization.Locale GetDefaultLocale()
  {
    Localization.Locale defaultLocale = (Localization.Locale) null;
    foreach (Localization.Locale locale in Localization.Locales)
    {
      if (locale.Lang == Localization.Language.Unspecified)
      {
        defaultLocale = new Localization.Locale(locale);
        break;
      }
    }
    return defaultLocale;
  }

  public static string GetDefaultFontName()
  {
    string defaultFontName = (string) null;
    foreach (Localization.Locale locale in Localization.Locales)
    {
      if (locale.Lang == Localization.Language.Unspecified)
      {
        defaultFontName = locale.FontName;
        break;
      }
    }
    return defaultFontName;
  }

  public static string ValidateFontName(string fontName)
  {
    foreach (Localization.Locale locale in Localization.Locales)
    {
      if (locale.MatchesFont(fontName))
        return locale.FontName;
    }
    return (string) null;
  }

  public static string GetFontName(string[] lines)
  {
    string fontName = (string) null;
    foreach (string line in lines)
    {
      if (line != null && line.Length != 0)
      {
        string fontParam = Localization.GetFontParam(line);
        if (fontParam != null)
          fontName = Localization.ValidateFontName(fontParam);
      }
      if (fontName != null)
        break;
    }
    if (fontName == null)
      fontName = Localization.sLocale == null ? Localization.GetDefaultFontName() : Localization.sLocale.FontName;
    return fontName;
  }

  public static void SwapToLocalizedFont() => Localization.SwapToLocalizedFont(Localization.currentFontName);

  public static bool SwapToLocalizedFont(string fontname)
  {
    if (string.IsNullOrEmpty(fontname))
      return false;
    Localization.sFontAsset = Localization.GetFont(fontname);
    foreach (TextStyleSetting textStyleSetting in Resources.FindObjectsOfTypeAll<TextStyleSetting>())
    {
      if (Object.op_Inequality((Object) textStyleSetting, (Object) null))
        textStyleSetting.sdfFont = Localization.sFontAsset;
    }
    bool isRightToLeft = Localization.IsRightToLeft;
    foreach (LocText locText in Resources.FindObjectsOfTypeAll<LocText>())
    {
      if (Object.op_Inequality((Object) locText, (Object) null))
        locText.SwapFont(Localization.sFontAsset, isRightToLeft);
    }
    return true;
  }

  private static bool SetFont(
    System.Type target_type,
    object target,
    TMP_FontAsset font,
    bool is_right_to_left,
    HashSet<MemberInfo> excluded_members)
  {
    if (target_type == (System.Type) null || target == null || Object.op_Equality((Object) font, (Object) null))
      return false;
    foreach (FieldInfo field in target_type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy))
    {
      if (!excluded_members.Contains((MemberInfo) field))
      {
        if (field.FieldType == typeof (TextStyleSetting))
          ((TextStyleSetting) field.GetValue(target)).sdfFont = font;
        else if (field.FieldType == typeof (LocText))
          ((LocText) field.GetValue(target)).SwapFont(font, is_right_to_left);
        else if (field.FieldType == typeof (GameObject))
        {
          foreach (Component component in ((GameObject) field.GetValue(target)).GetComponents<Component>())
            Localization.SetFont(((object) component).GetType(), (object) component, font, is_right_to_left, excluded_members);
        }
        else if (field.MemberType == MemberTypes.Field && field.FieldType != field.DeclaringType)
          Localization.SetFont(field.FieldType, field.GetValue(target), font, is_right_to_left, excluded_members);
      }
    }
    return true;
  }

  public static bool SetFont<T>(
    T target,
    TMP_FontAsset font,
    bool is_right_to_left,
    HashSet<MemberInfo> excluded_members)
  {
    return Localization.SetFont(typeof (T), (object) target, font, is_right_to_left, excluded_members);
  }

  public static TMP_FontAsset GetFont(string fontname)
  {
    foreach (TMP_FontAsset font in Resources.FindObjectsOfTypeAll<TMP_FontAsset>())
    {
      if (((Object) font).name == fontname)
        return font;
    }
    return (TMP_FontAsset) null;
  }

  private static bool HasSameOrLessTokenCount(
    string english_string,
    string translated_string,
    string token)
  {
    return english_string.Split(new string[1]{ token }, StringSplitOptions.None).Length >= translated_string.Split(new string[1]
    {
      token
    }, StringSplitOptions.None).Length;
  }

  private static bool HasSameOrLessLinkCountAsEnglish(
    string english_string,
    string translated_string)
  {
    return Localization.HasSameOrLessTokenCount(english_string, translated_string, "<link") && Localization.HasSameOrLessTokenCount(english_string, translated_string, "</link");
  }

  private static bool HasMatchingLinkTags(string str, int idx = 0)
  {
    int num1 = str.IndexOf("<link", idx);
    int num2 = str.IndexOf("</link", idx);
    if (num1 == -1 && num2 == -1)
      return true;
    if (num1 == -1 && num2 != -1 || num1 != -1 && num2 == -1 || num2 < num1)
      return false;
    int num3 = str.IndexOf("<link", num1 + 1);
    return (num1 < 0 || num3 == -1 || num3 >= num2) && Localization.HasMatchingLinkTags(str, num2 + 1);
  }

  private static bool AreParametersPreserved(string old_string, string new_string)
  {
    MatchCollection matchCollection1 = Regex.Matches(old_string, "({.[^}]*?})(?!.*\\1)");
    MatchCollection matchCollection2 = Regex.Matches(new_string, "({.[^}]*?})(?!.*\\1)");
    bool flag1 = false;
    if (matchCollection1 == null && matchCollection2 == null)
      flag1 = true;
    else if (matchCollection1 != null && matchCollection2 != null && matchCollection1.Count == matchCollection2.Count)
    {
      flag1 = true;
      foreach (object obj1 in matchCollection1)
      {
        string str1 = obj1.ToString();
        bool flag2 = false;
        foreach (object obj2 in matchCollection2)
        {
          string str2 = obj2.ToString();
          if (str1 == str2)
          {
            flag2 = true;
            break;
          }
        }
        if (!flag2)
        {
          flag1 = false;
          break;
        }
      }
    }
    return flag1;
  }

  public static bool HasDirtyWords(string str) => Localization.FilterDirtyWords(str) != str;

  public static string FilterDirtyWords(string str) => DistributionPlatform.Inst.ApplyWordFilter(str);

  public static string GetFileDateFormat(int format_idx) => "{" + format_idx.ToString() + ":dd / MMM / yyyy}";

  public static void ClearLanguage()
  {
    DebugUtil.LogArgs(new object[1]
    {
      (object) " -> Clearing selected language! Either it didn't load correct or returning to english by menu."
    });
    Localization.sFontAsset = (TMP_FontAsset) null;
    Localization.sLocale = (Localization.Locale) null;
    KPlayerPrefs.SetString(Localization.SELECTED_LANGUAGE_TYPE_KEY, Localization.SelectedLanguageType.None.ToString());
    KPlayerPrefs.SetString(Localization.SELECTED_LANGUAGE_CODE_KEY, "");
    Localization.SwapToLocalizedFont(Localization.GetDefaultLocale().FontName);
    string localizationFilePath = Localization.GetDefaultLocalizationFilePath();
    if (File.Exists(localizationFilePath))
      Localization.LoadTranslation(File.ReadAllLines(localizationFilePath, Encoding.UTF8), true);
    LanguageOptionsScreen.CleanUpSavedLanguageMod();
  }

  private static string ReverseText(string source)
  {
    char[] chArray1 = new char[1]{ '\n' };
    string[] strArray = source.Split(chArray1);
    string str1 = "";
    int num = 0;
    foreach (string str2 in strArray)
    {
      ++num;
      char[] chArray2 = new char[str2.Length];
      for (int index = 0; index < str2.Length; ++index)
        chArray2[chArray2.Length - 1 - index] = str2[index];
      str1 += new string(chArray2);
      if (num < strArray.Length)
        str1 += "\n";
    }
    return str1;
  }

  public static string Fixup(string text) => Localization.sLocale != null && text != null && text != "" && Localization.sLocale.Lang == Localization.Language.Arabic ? Localization.ReverseText(ArabicFixer.Fix(text)) : text;

  public enum Language
  {
    Chinese,
    Japanese,
    Korean,
    Russian,
    Thai,
    Arabic,
    Hebrew,
    Unspecified,
  }

  public enum Direction
  {
    LeftToRight,
    RightToLeft,
  }

  public class Locale
  {
    private Localization.Language mLanguage;
    private string mCode;
    private string mFontName;
    private Localization.Direction mDirection;

    public Locale(Localization.Locale other)
    {
      this.mLanguage = other.mLanguage;
      this.mDirection = other.mDirection;
      this.mCode = other.mCode;
      this.mFontName = other.mFontName;
    }

    public Locale(
      Localization.Language language,
      Localization.Direction direction,
      string code,
      string fontName)
    {
      this.mLanguage = language;
      this.mDirection = direction;
      this.mCode = code.ToLower();
      this.mFontName = fontName;
    }

    public Localization.Language Lang => this.mLanguage;

    public void SetCode(string code) => this.mCode = code;

    public string Code => this.mCode;

    public string FontName => this.mFontName;

    public bool IsRightToLeft => this.mDirection == Localization.Direction.RightToLeft;

    public bool MatchesCode(string language_code) => language_code.ToLower().Contains(this.mCode);

    public bool MatchesFont(string fontname) => fontname.ToLower() == this.mFontName.ToLower();

    public override string ToString() => this.mCode + ":" + this.mLanguage.ToString() + ":" + this.mDirection.ToString() + ":" + this.mFontName;
  }

  private struct Entry
  {
    public string msgctxt;
    public string msgstr;

    public bool IsPopulated => this.msgctxt != null && this.msgstr != null && this.msgstr.Length > 0;
  }

  public enum SelectedLanguageType
  {
    None,
    Preinstalled,
    UGC,
  }
}
