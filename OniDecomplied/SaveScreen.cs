// Decompiled with JetBrains decompiler
// Type: SaveScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.IO;
using TMPro;
using UnityEngine;

public class SaveScreen : KModalScreen
{
  [SerializeField]
  private KButton closeButton;
  [SerializeField]
  private KButton newSaveButton;
  [SerializeField]
  private KButton oldSaveButtonPrefab;
  [SerializeField]
  private Transform oldSavesRoot;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    ((Component) this.oldSaveButtonPrefab).gameObject.SetActive(false);
    this.newSaveButton.onClick += new System.Action(this.OnClickNewSave);
    this.closeButton.onClick += new System.Action(((KScreen) this).Deactivate);
  }

  protected override void OnCmpEnable()
  {
    foreach (SaveLoader.SaveFileEntry allColonyFile in SaveLoader.GetAllColonyFiles(true))
      this.AddExistingSaveFile(allColonyFile.path);
    SpeedControlScreen.Instance.Pause();
  }

  protected override void OnDeactivate()
  {
    SpeedControlScreen.Instance.Unpause();
    base.OnDeactivate();
  }

  private void AddExistingSaveFile(string filename)
  {
    KButton kbutton = Util.KInstantiateUI<KButton>(((Component) this.oldSaveButtonPrefab).gameObject, ((Component) this.oldSavesRoot).gameObject, true);
    HierarchyReferences component1 = ((Component) kbutton).GetComponent<HierarchyReferences>();
    LocText component2 = ((Component) component1.GetReference<RectTransform>("Title")).GetComponent<LocText>();
    LocText component3 = ((Component) component1.GetReference<RectTransform>("Date")).GetComponent<LocText>();
    System.DateTime lastWriteTime = File.GetLastWriteTime(filename);
    ((TMP_Text) component2).text = string.Format("{0}", (object) System.IO.Path.GetFileNameWithoutExtension(filename));
    string str = string.Format("{0:H:mm:ss}" + Localization.GetFileDateFormat(0), (object) lastWriteTime);
    ((TMP_Text) component3).text = str;
    kbutton.onClick += (System.Action) (() => this.Save(filename));
  }

  public static string GetValidSaveFilename(string filename)
  {
    string str = ".sav";
    if (System.IO.Path.GetExtension(filename).ToLower() != str)
      filename += str;
    return filename;
  }

  public void Save(string filename)
  {
    filename = SaveScreen.GetValidSaveFilename(filename);
    if (File.Exists(filename))
      ScreenPrefabs.Instance.ConfirmDoAction(string.Format((string) UI.FRONTEND.SAVESCREEN.OVERWRITEMESSAGE, (object) System.IO.Path.GetFileNameWithoutExtension(filename)), (System.Action) (() => this.DoSave(filename)), ((KMonoBehaviour) this).transform.parent);
    else
      this.DoSave(filename);
  }

  private void DoSave(string filename)
  {
    try
    {
      SaveLoader.Instance.Save(filename);
      this.Deactivate();
    }
    catch (IOException ex)
    {
      Util.KInstantiateUI(((Component) ScreenPrefabs.Instance.ConfirmDialogScreen).gameObject, ((Component) ((KMonoBehaviour) this).transform.parent).gameObject, true).GetComponent<ConfirmDialogScreen>().PopupConfirmDialog(string.Format((string) UI.FRONTEND.SAVESCREEN.IO_ERROR, (object) ex.ToString()), (System.Action) (() => this.Deactivate()), (System.Action) null, (string) UI.FRONTEND.SAVESCREEN.REPORT_BUG, (System.Action) (() => KCrashReporter.ReportError(ex.Message, ex.StackTrace.ToString(), (string) null, (ConfirmDialogScreen) null, (GameObject) null)));
    }
  }

  public void OnClickNewSave()
  {
    FileNameDialog fileNameDialog = (FileNameDialog) KScreenManager.Instance.StartScreen(((Component) ScreenPrefabs.Instance.FileNameDialog).gameObject, ((Component) ((KMonoBehaviour) this).transform.parent).gameObject);
    string activeSaveFilePath = SaveLoader.GetActiveSaveFilePath();
    if (activeSaveFilePath != null)
    {
      string withoutExtension = System.IO.Path.GetFileNameWithoutExtension(SaveLoader.GetOriginalSaveFileName(activeSaveFilePath));
      fileNameDialog.SetTextAndSelect(withoutExtension);
    }
    fileNameDialog.onConfirm = (Action<string>) (filename =>
    {
      filename = System.IO.Path.Combine(SaveLoader.GetActiveSaveColonyFolder(), filename);
      this.Save(filename);
    });
  }

  public override void OnKeyUp(KButtonEvent e)
  {
    if (e.TryConsume((Action) 1))
      this.Deactivate();
    ((KInputEvent) e).Consumed = true;
  }

  public override void OnKeyDown(KButtonEvent e) => ((KInputEvent) e).Consumed = true;
}
