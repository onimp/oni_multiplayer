// Decompiled with JetBrains decompiler
// Type: PatchNotesScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TMPro;
using UnityEngine;

public class PatchNotesScreen : KModalScreen
{
  [SerializeField]
  private KButton closeButton;
  [SerializeField]
  private KButton okButton;
  [SerializeField]
  private KButton fullPatchNotes;
  [SerializeField]
  private KButton previousVersion;
  [SerializeField]
  private LocText changesLabel;
  private static string m_patchNotesUrl;
  private static string m_patchNotesText;
  private static int PatchNotesVersion = 9;
  private static PatchNotesScreen instance;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    ((TMP_Text) this.changesLabel).text = PatchNotesScreen.m_patchNotesText;
    this.closeButton.onClick += new System.Action(this.MarkAsReadAndClose);
    ((WidgetSoundPlayer) this.closeButton.soundPlayer).widget_sound_events()[0].OverrideAssetName = "HUD_Click_Close";
    this.okButton.onClick += new System.Action(this.MarkAsReadAndClose);
    this.previousVersion.onClick += (System.Action) (() => App.OpenWebURL("http://support.kleientertainment.com/customer/portal/articles/2776550"));
    this.fullPatchNotes.onClick += new System.Action(this.OnPatchNotesClick);
    PatchNotesScreen.instance = this;
  }

  protected virtual void OnCleanUp() => PatchNotesScreen.instance = (PatchNotesScreen) null;

  public static bool ShouldShowScreen() => KPlayerPrefs.GetInt("PatchNotesVersion") < PatchNotesScreen.PatchNotesVersion;

  private void MarkAsReadAndClose()
  {
    KPlayerPrefs.SetInt("PatchNotesVersion", PatchNotesScreen.PatchNotesVersion);
    this.Deactivate();
  }

  public static void UpdatePatchNotes(string patchNotesSummary, string url)
  {
    PatchNotesScreen.m_patchNotesUrl = url;
    PatchNotesScreen.m_patchNotesText = patchNotesSummary;
    if (!Object.op_Inequality((Object) PatchNotesScreen.instance, (Object) null))
      return;
    ((TMP_Text) PatchNotesScreen.instance.changesLabel).text = PatchNotesScreen.m_patchNotesText;
  }

  private void OnPatchNotesClick() => App.OpenWebURL(PatchNotesScreen.m_patchNotesUrl);

  public override void OnKeyDown(KButtonEvent e)
  {
    if (e.TryConsume((Action) 1) || e.TryConsume((Action) 5))
      this.MarkAsReadAndClose();
    else
      base.OnKeyDown(e);
  }
}
