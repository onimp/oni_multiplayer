// Decompiled with JetBrains decompiler
// Type: ReportErrorDialog
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KMod;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReportErrorDialog : MonoBehaviour
{
  private System.Action submitAction;
  private System.Action quitAction;
  private System.Action continueAction;
  public KInputTextField messageInputField;
  public GameObject referenceMessage;
  private string m_stackTrace;
  [SerializeField]
  private KButton submitButton;
  [SerializeField]
  private KButton moreInfoButton;
  [SerializeField]
  private KButton quitButton;
  [SerializeField]
  private KButton continueGameButton;
  [SerializeField]
  private LocText CrashLabel;
  [SerializeField]
  private GameObject CrashDescription;
  [SerializeField]
  private GameObject ModsInfo;
  [SerializeField]
  private GameObject StackTrace;
  [SerializeField]
  private GameObject modEntryPrefab;
  [SerializeField]
  private Transform modEntryParent;
  private ReportErrorDialog.Mode mode;

  private void Start()
  {
    ThreadedHttps<KleiMetrics>.Instance.EndSession(true);
    if (Object.op_Implicit((Object) KScreenManager.Instance))
      KScreenManager.Instance.DisableInput(true);
    this.StackTrace.SetActive(false);
    ((TMP_Text) this.CrashLabel).text = (string) (this.mode == ReportErrorDialog.Mode.SubmitError ? STRINGS.UI.CRASHSCREEN.TITLE : STRINGS.UI.CRASHSCREEN.TITLE_MODS);
    this.CrashDescription.SetActive(this.mode == ReportErrorDialog.Mode.SubmitError);
    this.ModsInfo.SetActive(this.mode == ReportErrorDialog.Mode.DisableMods);
    if (this.mode == ReportErrorDialog.Mode.DisableMods)
      this.BuildModsList();
    ((Component) this.submitButton).gameObject.SetActive(this.submitAction != null);
    this.submitButton.onClick += new System.Action(this.OnSelect_SUBMIT);
    this.moreInfoButton.onClick += new System.Action(this.OnSelect_MOREINFO);
    ((Component) this.continueGameButton).gameObject.SetActive(this.continueAction != null);
    this.continueGameButton.onClick += new System.Action(this.OnSelect_CONTINUE);
    this.quitButton.onClick += new System.Action(this.OnSelect_QUIT);
    ((TMP_InputField) this.messageInputField).text = (string) STRINGS.UI.CRASHSCREEN.BODY;
  }

  private void BuildModsList()
  {
    DebugUtil.Assert(Object.op_Inequality((Object) Global.Instance, (Object) null) && Global.Instance.modManager != null);
    Manager mod_mgr = Global.Instance.modManager;
    List<KMod.Mod> allCrashableMods = mod_mgr.GetAllCrashableMods();
    allCrashableMods.Sort((Comparison<KMod.Mod>) ((x, y) => y.foundInStackTrace.CompareTo(x.foundInStackTrace)));
    foreach (KMod.Mod mod in allCrashableMods)
    {
      if (mod.foundInStackTrace && mod.label.distribution_platform != KMod.Label.DistributionPlatform.Dev)
        mod_mgr.EnableMod(mod.label, false, (object) this);
      HierarchyReferences hierarchyReferences = Util.KInstantiateUI<HierarchyReferences>(this.modEntryPrefab, ((Component) this.modEntryParent).gameObject, false);
      LocText reference = hierarchyReferences.GetReference<LocText>("Title");
      ((TMP_Text) reference).text = mod.title;
      ((Graphic) reference).color = mod.foundInStackTrace ? Color.red : Color.white;
      MultiToggle toggle = hierarchyReferences.GetReference<MultiToggle>("EnabledToggle");
      toggle.ChangeState(mod.IsEnabledForActiveDlc() ? 1 : 0);
      KMod.Label mod_label = mod.label;
      toggle.onClick += (System.Action) (() =>
      {
        bool enabled = !mod_mgr.IsModEnabled(mod_label);
        toggle.ChangeState(enabled ? 1 : 0);
        mod_mgr.EnableMod(mod_label, enabled, (object) this);
      });
      ((Component) toggle).GetComponent<ToolTip>().OnToolTip = (Func<string>) (() => (string) (mod_mgr.IsModEnabled(mod_label) ? STRINGS.UI.FRONTEND.MODS.TOOLTIPS.ENABLED : STRINGS.UI.FRONTEND.MODS.TOOLTIPS.DISABLED));
      ((Component) hierarchyReferences).gameObject.SetActive(true);
    }
  }

  private void Update() => Debug.developerConsoleVisible = false;

  private void OnDestroy()
  {
    if (KCrashReporter.terminateOnError)
      App.Quit();
    if (!Object.op_Implicit((Object) KScreenManager.Instance))
      return;
    KScreenManager.Instance.DisableInput(false);
  }

  public void OnKeyDown(KButtonEvent e)
  {
    if (!e.TryConsume((Action) 1))
      return;
    this.OnSelect_QUIT();
  }

  public void PopupSubmitErrorDialog(
    string stackTrace,
    System.Action onSubmit,
    System.Action onQuit,
    System.Action onContinue)
  {
    this.mode = ReportErrorDialog.Mode.SubmitError;
    this.m_stackTrace = stackTrace;
    this.submitAction = onSubmit;
    this.quitAction = onQuit;
    this.continueAction = onContinue;
  }

  public void PopupDisableModsDialog(string stackTrace, System.Action onQuit, System.Action onContinue)
  {
    this.mode = ReportErrorDialog.Mode.DisableMods;
    this.m_stackTrace = stackTrace;
    this.quitAction = onQuit;
    this.continueAction = onContinue;
  }

  public void OnSelect_MOREINFO()
  {
    ((TMP_Text) this.StackTrace.GetComponentInChildren<LocText>()).text = this.m_stackTrace;
    this.StackTrace.SetActive(true);
    ((TMP_Text) ((Component) this.moreInfoButton).GetComponentInChildren<LocText>()).text = (string) STRINGS.UI.CRASHSCREEN.COPYTOCLIPBOARDBUTTON;
    this.moreInfoButton.ClearOnClick();
    this.moreInfoButton.onClick += new System.Action(this.OnSelect_COPYTOCLIPBOARD);
  }

  public void OnSelect_COPYTOCLIPBOARD()
  {
    TextEditor textEditor = new TextEditor();
    textEditor.text = this.m_stackTrace + "\nBuild: " + BuildWatermark.GetBuildText();
    textEditor.SelectAll();
    textEditor.Copy();
  }

  public void OnSelect_SUBMIT()
  {
    ((TMP_Text) ((Component) this.submitButton).GetComponentInChildren<LocText>()).text = (string) STRINGS.UI.CRASHSCREEN.REPORTING;
    ((Component) this.submitButton).GetComponent<KButton>().isInteractable = false;
    this.Submit();
  }

  public void OnSelect_QUIT()
  {
    if (this.quitAction == null)
      return;
    this.quitAction();
  }

  public void OnSelect_CONTINUE()
  {
    if (this.continueAction == null)
      return;
    this.continueAction();
  }

  public void OpenRefMessage()
  {
    ((Component) this.submitButton).gameObject.SetActive(false);
    this.referenceMessage.SetActive(true);
  }

  public string UserMessage() => ((TMP_InputField) this.messageInputField).text;

  private void Submit()
  {
    this.submitAction();
    this.OpenRefMessage();
  }

  private enum Mode
  {
    SubmitError,
    DisableMods,
  }
}
