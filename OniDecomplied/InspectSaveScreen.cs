// Decompiled with JetBrains decompiler
// Type: InspectSaveScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InspectSaveScreen : KModalScreen
{
  [SerializeField]
  private KButton closeButton;
  [SerializeField]
  private KButton mainSaveBtn;
  [SerializeField]
  private KButton backupBtnPrefab;
  [SerializeField]
  private KButton deleteSaveBtn;
  [SerializeField]
  private GameObject buttonGroup;
  private UIPool<KButton> buttonPool;
  private Dictionary<KButton, string> buttonFileMap = new Dictionary<KButton, string>();
  private ConfirmDialogScreen confirmScreen;
  private string currentPath = "";

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.closeButton.onClick += new System.Action(this.CloseScreen);
    this.deleteSaveBtn.onClick += new System.Action(this.DeleteSave);
  }

  private void CloseScreen()
  {
    LoadScreen.Instance.Show(true);
    this.Show(false);
  }

  protected override void OnShow(bool show)
  {
    base.OnShow(show);
    if (show)
      return;
    this.buttonPool.ClearAll();
    this.buttonFileMap.Clear();
  }

  public void SetTarget(string path)
  {
    if (string.IsNullOrEmpty(path))
    {
      Debug.LogError((object) "The directory path provided is empty.");
      this.Show(false);
    }
    else if (!Directory.Exists(path))
    {
      Debug.LogError((object) "The directory provided does not exist.");
      this.Show(false);
    }
    else
    {
      if (this.buttonPool == null)
        this.buttonPool = new UIPool<KButton>(this.backupBtnPrefab);
      this.currentPath = path;
      List<string> list = ((IEnumerable<string>) Directory.GetFiles(path)).Where<string>((Func<string, bool>) (filename => System.IO.Path.GetExtension(filename).ToLower() == ".sav")).OrderByDescending<string, System.DateTime>((Func<string, System.DateTime>) (filename => File.GetLastWriteTime(filename))).ToList<string>();
      string str = list[0];
      if (File.Exists(str))
      {
        ((Component) this.mainSaveBtn).gameObject.SetActive(true);
        this.AddNewSave(this.mainSaveBtn, str);
      }
      else
        ((Component) this.mainSaveBtn).gameObject.SetActive(false);
      if (list.Count > 1)
      {
        for (int index = 1; index < list.Count; ++index)
          this.AddNewSave(this.buttonPool.GetFreeElement(this.buttonGroup, true), list[index]);
      }
      this.Show(true);
    }
  }

  private void ConfirmDoAction(string message, System.Action action)
  {
    if (!Object.op_Equality((Object) this.confirmScreen, (Object) null))
      return;
    this.confirmScreen = Util.KInstantiateUI<ConfirmDialogScreen>(((Component) ScreenPrefabs.Instance.ConfirmDialogScreen).gameObject, ((Component) this).gameObject, false);
    this.confirmScreen.PopupConfirmDialog(message, action, (System.Action) (() => { }));
    ((Component) this.confirmScreen).GetComponent<LayoutElement>().ignoreLayout = true;
    ((Component) this.confirmScreen).gameObject.SetActive(true);
  }

  private void DeleteSave()
  {
    if (string.IsNullOrEmpty(this.currentPath))
      Debug.LogError((object) "The path provided is not valid and cannot be deleted.");
    else
      this.ConfirmDoAction((string) STRINGS.UI.FRONTEND.LOADSCREEN.CONFIRMDELETE, (System.Action) (() =>
      {
        foreach (string file in Directory.GetFiles(this.currentPath))
          File.Delete(file);
        Directory.Delete(this.currentPath);
        this.CloseScreen();
      }));
  }

  private void AddNewSave(KButton btn, string file)
  {
  }

  private void ButtonClicked(KButton btn) => LoadingOverlay.Load((System.Action) (() => this.Load(this.buttonFileMap[btn])));

  private void Load(string filename)
  {
    if (Object.op_Inequality((Object) Game.Instance, (Object) null))
      LoadScreen.ForceStopGame();
    SaveLoader.SetActiveSaveFilePath(filename);
    App.LoadScene("backend");
    this.Deactivate();
  }

  public override void OnKeyDown(KButtonEvent e)
  {
    if (e.TryConsume((Action) 1) || e.TryConsume((Action) 5))
      this.CloseScreen();
    else
      base.OnKeyDown(e);
  }
}
