// Decompiled with JetBrains decompiler
// Type: LocText
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class LocText : TextMeshProUGUI
{
  public string key;
  public TextStyleSetting textStyleSetting;
  public bool allowOverride;
  public bool staticLayout;
  private TextLinkHandler textLinkHandler;
  private string originalString = string.Empty;
  [SerializeField]
  private bool allowLinksInternal;
  private static readonly Dictionary<string, Action> ActionLookup = ((IEnumerable<string>) Enum.GetNames(typeof (Action))).ToDictionary<string, string, Action>((Func<string, string>) (x => x), (Func<string, Action>) (x => (Action) Enum.Parse(typeof (Action), x)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
  private static readonly Dictionary<string, Pair<string, string>> ClickLookup;
  private const string linkPrefix_open = "<link=\"";
  private const string linkSuffix = "</link>";
  private const string linkColorPrefix = "<b><style=\"KLink\">";
  private const string linkColorSuffix = "</style></b>";
  private static readonly string combinedPrefix;
  private static readonly string combinedSuffix;

  protected virtual void OnEnable() => base.OnEnable();

  public bool AllowLinks
  {
    get => this.allowLinksInternal;
    set
    {
      this.allowLinksInternal = value;
      this.RefreshLinkHandler();
      ((Graphic) this).raycastTarget = ((Graphic) this).raycastTarget || this.allowLinksInternal;
    }
  }

  [ContextMenu("Apply Settings")]
  public void ApplySettings()
  {
    if (this.key != "" && Application.isPlaying)
    {
      StringKey stringKey;
      // ISSUE: explicit constructor call
      ((StringKey) ref stringKey).\u002Ector(this.key);
      ((TMP_Text) this).text = StringEntry.op_Implicit(Strings.Get(stringKey));
    }
    if (!Object.op_Inequality((Object) this.textStyleSetting, (Object) null))
      return;
    SetTextStyleSetting.ApplyStyle((TextMeshProUGUI) this, this.textStyleSetting);
  }

  private void Awake()
  {
    base.Awake();
    if (!Application.isPlaying)
      return;
    if (this.key != "")
      ((TMP_Text) this).text = Strings.Get(new StringKey(this.key)).String;
    ((TMP_Text) this).text = Localization.Fixup(((TMP_Text) this).text);
    ((TMP_Text) this).isRightToLeftText = Localization.IsRightToLeft;
    // ISSUE: method pointer
    KInputManager.InputChange.AddListener(new UnityAction((object) this, __methodptr(RefreshText)));
    SetTextStyleSetting textStyleSetting = ((Component) this).gameObject.GetComponent<SetTextStyleSetting>();
    if (Object.op_Equality((Object) textStyleSetting, (Object) null))
      textStyleSetting = ((Component) this).gameObject.AddComponent<SetTextStyleSetting>();
    if (!this.allowOverride)
      textStyleSetting.SetStyle(this.textStyleSetting);
    this.textLinkHandler = ((Component) this).GetComponent<TextLinkHandler>();
  }

  private void Start()
  {
    ((UIBehaviour) this).Start();
    this.RefreshLinkHandler();
  }

  private void OnDestroy()
  {
    // ISSUE: method pointer
    KInputManager.InputChange.RemoveListener(new UnityAction((object) this, __methodptr(RefreshText)));
    base.OnDestroy();
  }

  public virtual void SetLayoutDirty()
  {
    if (this.staticLayout)
      return;
    base.SetLayoutDirty();
  }

  public virtual string text
  {
    get => ((TMP_Text) this).text;
    set => ((TMP_Text) this).text = this.FilterInput(value);
  }

  public virtual void SetText(string text)
  {
    text = this.FilterInput(text);
    ((TMP_Text) this).SetText(text);
  }

  private string FilterInput(string input)
  {
    if (input != null)
    {
      string text = LocText.ParseText(input);
      this.originalString = !(text != input) ? string.Empty : input;
      input = text;
    }
    return this.AllowLinks ? LocText.ModifyLinkStrings(input) : input;
  }

  public static string ParseText(string input)
  {
    string pattern = "\\{Hotkey/(\\w+)\\}";
    return Regex.Replace(Regex.Replace(input, pattern, (MatchEvaluator) (m =>
    {
      string key = m.Groups[1].Value;
      Action action;
      return LocText.ActionLookup.TryGetValue(key, out action) ? GameUtil.GetHotkeyString(action) : m.Value;
    })), "\\(ClickType/(\\w+)\\)", (MatchEvaluator) (m =>
    {
      string key = m.Groups[1].Value;
      Pair<string, string> pair;
      if (!LocText.ClickLookup.TryGetValue(key, out pair))
        return m.Value;
      return KInputManager.currentControllerIsGamepad ? pair.first : pair.second;
    }));
  }

  private void RefreshText()
  {
    if (!(this.originalString != string.Empty))
      return;
    ((TMP_Text) this).SetText(this.originalString);
  }

  protected virtual void GenerateTextMesh() => base.GenerateTextMesh();

  internal void SwapFont(TMP_FontAsset font, bool isRightToLeft)
  {
    ((TMP_Text) this).font = font;
    if (this.key != "")
      ((TMP_Text) this).text = Strings.Get(new StringKey(this.key)).String;
    ((TMP_Text) this).text = Localization.Fixup(((TMP_Text) this).text);
    ((TMP_Text) this).isRightToLeftText = isRightToLeft;
  }

  private static string ModifyLinkStrings(string input)
  {
    if (input == null || input.IndexOf("<b><style=\"KLink\">") != -1)
      return input;
    StringBuilder stringBuilder = new StringBuilder(input);
    stringBuilder.Replace("<link=\"", LocText.combinedPrefix);
    stringBuilder.Replace("</link>", LocText.combinedSuffix);
    return stringBuilder.ToString();
  }

  private void RefreshLinkHandler()
  {
    if (Object.op_Equality((Object) this.textLinkHandler, (Object) null) && this.allowLinksInternal)
    {
      this.textLinkHandler = ((Component) this).GetComponent<TextLinkHandler>();
      if (Object.op_Equality((Object) this.textLinkHandler, (Object) null))
        this.textLinkHandler = ((Component) this).gameObject.AddComponent<TextLinkHandler>();
    }
    else if (!this.allowLinksInternal && Object.op_Inequality((Object) this.textLinkHandler, (Object) null))
    {
      Object.Destroy((Object) this.textLinkHandler);
      this.textLinkHandler = (TextLinkHandler) null;
    }
    if (!Object.op_Inequality((Object) this.textLinkHandler, (Object) null))
      return;
    this.textLinkHandler.CheckMouseOver();
  }

  static LocText()
  {
    Dictionary<string, Pair<string, string>> dictionary = new Dictionary<string, Pair<string, string>>();
    dictionary.Add(STRINGS.UI.ClickType.Click.ToString(), new Pair<string, string>((string) STRINGS.UI.CONTROLS.PRESS, (string) STRINGS.UI.CONTROLS.CLICK));
    dictionary.Add(STRINGS.UI.ClickType.Clickable.ToString(), new Pair<string, string>((string) STRINGS.UI.CONTROLS.PRESSABLE, (string) STRINGS.UI.CONTROLS.CLICKABLE));
    dictionary.Add(STRINGS.UI.ClickType.Clicked.ToString(), new Pair<string, string>((string) STRINGS.UI.CONTROLS.PRESSED, (string) STRINGS.UI.CONTROLS.CLICKED));
    dictionary.Add(STRINGS.UI.ClickType.Clicking.ToString(), new Pair<string, string>((string) STRINGS.UI.CONTROLS.PRESSING, (string) STRINGS.UI.CONTROLS.CLICKING));
    dictionary.Add(STRINGS.UI.ClickType.Clicks.ToString(), new Pair<string, string>((string) STRINGS.UI.CONTROLS.PRESSES, (string) STRINGS.UI.CONTROLS.CLICKS));
    dictionary.Add(STRINGS.UI.ClickType.click.ToString(), new Pair<string, string>((string) STRINGS.UI.CONTROLS.PRESSLOWER, (string) STRINGS.UI.CONTROLS.CLICKLOWER));
    dictionary.Add(STRINGS.UI.ClickType.clickable.ToString(), new Pair<string, string>((string) STRINGS.UI.CONTROLS.PRESSABLELOWER, (string) STRINGS.UI.CONTROLS.CLICKABLELOWER));
    dictionary.Add(STRINGS.UI.ClickType.clicked.ToString(), new Pair<string, string>((string) STRINGS.UI.CONTROLS.PRESSEDLOWER, (string) STRINGS.UI.CONTROLS.CLICKEDLOWER));
    dictionary.Add(STRINGS.UI.ClickType.clicking.ToString(), new Pair<string, string>((string) STRINGS.UI.CONTROLS.PRESSINGLOWER, (string) STRINGS.UI.CONTROLS.CLICKINGLOWER));
    dictionary.Add(STRINGS.UI.ClickType.clicks.ToString(), new Pair<string, string>((string) STRINGS.UI.CONTROLS.PRESSESLOWER, (string) STRINGS.UI.CONTROLS.CLICKSLOWER));
    dictionary.Add(STRINGS.UI.ClickType.CLICK.ToString(), new Pair<string, string>((string) STRINGS.UI.CONTROLS.PRESSUPPER, (string) STRINGS.UI.CONTROLS.CLICKUPPER));
    dictionary.Add(STRINGS.UI.ClickType.CLICKABLE.ToString(), new Pair<string, string>((string) STRINGS.UI.CONTROLS.PRESSABLEUPPER, (string) STRINGS.UI.CONTROLS.CLICKABLEUPPER));
    dictionary.Add(STRINGS.UI.ClickType.CLICKED.ToString(), new Pair<string, string>((string) STRINGS.UI.CONTROLS.PRESSEDUPPER, (string) STRINGS.UI.CONTROLS.CLICKEDUPPER));
    dictionary.Add(STRINGS.UI.ClickType.CLICKING.ToString(), new Pair<string, string>((string) STRINGS.UI.CONTROLS.PRESSINGUPPER, (string) STRINGS.UI.CONTROLS.CLICKINGUPPER));
    dictionary.Add(STRINGS.UI.ClickType.CLICKS.ToString(), new Pair<string, string>((string) STRINGS.UI.CONTROLS.PRESSESUPPER, (string) STRINGS.UI.CONTROLS.CLICKSUPPER));
    LocText.ClickLookup = dictionary;
    LocText.combinedPrefix = "<b><style=\"KLink\"><link=\"";
    LocText.combinedSuffix = "</style></b></link>";
  }
}
