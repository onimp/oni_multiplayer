// Decompiled with JetBrains decompiler
// Type: MetricsOptionsScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MetricsOptionsScreen : KModalScreen
{
  public LocText title;
  public KButton dismissButton;
  public KButton closeButton;
  public GameObject enableButton;
  public Button descriptionButton;
  public LocText restartWarningText;
  private bool disableDataCollection;

  private bool IsSettingsDirty() => this.disableDataCollection != KPrivacyPrefs.instance.disableDataCollection;

  public override void OnKeyDown(KButtonEvent e)
  {
    if ((e.TryConsume((Action) 1) || e.TryConsume((Action) 5)) && !this.IsSettingsDirty())
      this.Show(false);
    base.OnKeyDown(e);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.disableDataCollection = KPrivacyPrefs.instance.disableDataCollection;
    ((TMP_Text) this.title).SetText((string) STRINGS.UI.FRONTEND.METRICS_OPTIONS_SCREEN.TITLE);
    GameObject gameObject = this.enableButton.GetComponent<HierarchyReferences>().GetReference("Button").gameObject;
    gameObject.GetComponent<ToolTip>().SetSimpleTooltip((string) STRINGS.UI.FRONTEND.METRICS_OPTIONS_SCREEN.TOOLTIP);
    gameObject.GetComponent<KButton>().onClick += (System.Action) (() => this.OnClickToggle());
    ((TMP_Text) this.enableButton.GetComponent<HierarchyReferences>().GetReference<LocText>("Text")).SetText((string) STRINGS.UI.FRONTEND.METRICS_OPTIONS_SCREEN.ENABLE_BUTTON);
    this.dismissButton.onClick += (System.Action) (() =>
    {
      if (this.IsSettingsDirty())
        this.ApplySettingsAndDoRestart();
      else
        this.Deactivate();
    });
    this.closeButton.onClick += (System.Action) (() => this.Deactivate());
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: reference to a compiler-generated field
    // ISSUE: method pointer
    ((UnityEvent) this.descriptionButton.onClick).AddListener(MetricsOptionsScreen.\u003C\u003Ec.\u003C\u003E9__9_3 ?? (MetricsOptionsScreen.\u003C\u003Ec.\u003C\u003E9__9_3 = new UnityAction((object) MetricsOptionsScreen.\u003C\u003Ec.\u003C\u003E9, __methodptr(\u003COnSpawn\u003Eb__9_3))));
    this.Refresh();
  }

  private void OnClickToggle()
  {
    this.disableDataCollection = !this.disableDataCollection;
    this.enableButton.GetComponent<HierarchyReferences>().GetReference("CheckMark").gameObject.SetActive(this.disableDataCollection);
    this.Refresh();
  }

  private void ApplySettingsAndDoRestart()
  {
    KPrivacyPrefs.instance.disableDataCollection = this.disableDataCollection;
    KPrivacyPrefs.Save();
    KPlayerPrefs.SetString("DisableDataCollection", KPrivacyPrefs.instance.disableDataCollection ? "yes" : "no");
    KPlayerPrefs.Save();
    ThreadedHttps<KleiMetrics>.Instance.SetEnabled(!KPrivacyPrefs.instance.disableDataCollection);
    this.enableButton.GetComponent<HierarchyReferences>().GetReference("CheckMark").gameObject.SetActive(ThreadedHttps<KleiMetrics>.Instance.enabled);
    App.instance.Restart();
  }

  private void Refresh()
  {
    ((Component) this.enableButton.GetComponent<HierarchyReferences>().GetReference("Button").transform.GetChild(0)).gameObject.SetActive(!this.disableDataCollection);
    this.closeButton.isInteractable = !this.IsSettingsDirty();
    ((Component) this.restartWarningText).gameObject.SetActive(this.IsSettingsDirty());
    if (this.IsSettingsDirty())
      ((TMP_Text) ((Component) this.dismissButton).GetComponentInChildren<LocText>()).text = (string) STRINGS.UI.FRONTEND.METRICS_OPTIONS_SCREEN.RESTART_BUTTON;
    else
      ((TMP_Text) ((Component) this.dismissButton).GetComponentInChildren<LocText>()).text = (string) STRINGS.UI.FRONTEND.METRICS_OPTIONS_SCREEN.DONE_BUTTON;
  }
}
