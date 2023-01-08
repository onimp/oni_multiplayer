// Decompiled with JetBrains decompiler
// Type: InitializeCheck
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ProcGenGame;
using STRINGS;
using System.IO;
using UnityEngine;

public class InitializeCheck : MonoBehaviour
{
  private static readonly string testFile = "testfile";
  private static readonly string testSave = "testsavefile";
  public Canvas rootCanvasPrefab;
  public ConfirmDialogScreen confirmDialogScreen;
  public Sprite sadDupe;
  private InitializeCheck.SavePathIssue test_issue;

  public static InitializeCheck.SavePathIssue savePathState { get; private set; }

  private void Awake()
  {
    this.CheckForSavePathIssue();
    if (InitializeCheck.savePathState == InitializeCheck.SavePathIssue.Ok && !KCrashReporter.hasCrash)
    {
      AudioMixer.Create();
      App.LoadScene("frontend");
    }
    else
    {
      Canvas canvas = ((Component) this).gameObject.AddComponent<Canvas>();
      Util.rectTransform((Component) canvas).SetSizeWithCurrentAnchors((RectTransform.Axis) 0, 500f);
      Util.rectTransform((Component) canvas).SetSizeWithCurrentAnchors((RectTransform.Axis) 1, 500f);
      Camera camera = ((Component) this).gameObject.AddComponent<Camera>();
      camera.orthographic = true;
      camera.orthographicSize = 200f;
      camera.backgroundColor = Color.black;
      camera.clearFlags = (CameraClearFlags) 2;
      camera.nearClipPlane = 0.0f;
      Debug.Log((object) ("Cannot initialize filesystem. [" + InitializeCheck.savePathState.ToString() + "]"));
      Localization.Initialize();
      GameObject.Find("BootCanvas").SetActive(false);
      this.ShowFileErrorDialogs();
    }
  }

  private GameObject CreateUIRoot() => Util.KInstantiate((Component) this.rootCanvasPrefab, (GameObject) null, "CanvasRoot");

  private void ShowErrorDialog(string msg) => Util.KInstantiateUI<ConfirmDialogScreen>(((Component) this.confirmDialogScreen).gameObject, this.CreateUIRoot(), true).PopupConfirmDialog(msg, new System.Action(this.Quit), (System.Action) null, image_sprite: this.sadDupe);

  private void ShowFileErrorDialogs()
  {
    string msg = (string) null;
    switch (InitializeCheck.savePathState)
    {
      case InitializeCheck.SavePathIssue.WriteTestFail:
        msg = string.Format((string) UI.FRONTEND.SUPPORTWARNINGS.SAVE_DIRECTORY_READ_ONLY, (object) SaveLoader.GetSavePrefix());
        break;
      case InitializeCheck.SavePathIssue.SpaceTestFail:
        msg = string.Format((string) UI.FRONTEND.SUPPORTWARNINGS.SAVE_DIRECTORY_INSUFFICIENT_SPACE, (object) SaveLoader.GetSavePrefix());
        break;
      case InitializeCheck.SavePathIssue.WorldGenFilesFail:
        msg = string.Format((string) UI.FRONTEND.SUPPORTWARNINGS.WORLD_GEN_FILES, (object) (WorldGen.WORLDGEN_SAVE_FILENAME + "\n" + WorldGen.GetSIMSaveFilename()));
        break;
    }
    if (msg == null)
      return;
    this.ShowErrorDialog(msg);
  }

  private void CheckForSavePathIssue()
  {
    if (this.test_issue != InitializeCheck.SavePathIssue.Ok)
    {
      InitializeCheck.savePathState = this.test_issue;
    }
    else
    {
      string savePrefix = SaveLoader.GetSavePrefix();
      InitializeCheck.savePathState = InitializeCheck.SavePathIssue.Ok;
      try
      {
        SaveLoader.GetSavePrefixAndCreateFolder();
        using (FileStream output = File.Open(savePrefix + InitializeCheck.testFile, FileMode.Create, FileAccess.Write))
        {
          BinaryWriter binaryWriter = new BinaryWriter((Stream) output);
          output.Close();
        }
      }
      catch
      {
        InitializeCheck.savePathState = InitializeCheck.SavePathIssue.WriteTestFail;
        goto label_27;
      }
      using (FileStream output = File.Open(savePrefix + InitializeCheck.testSave, FileMode.Create, FileAccess.Write))
      {
        try
        {
          output.SetLength(15000000L);
          BinaryWriter binaryWriter = new BinaryWriter((Stream) output);
          output.Close();
        }
        catch
        {
          output.Close();
          InitializeCheck.savePathState = InitializeCheck.SavePathIssue.SpaceTestFail;
          goto label_27;
        }
      }
      try
      {
        using (File.Open(WorldGen.WORLDGEN_SAVE_FILENAME, FileMode.Append))
          ;
        using (File.Open(WorldGen.GetSIMSaveFilename(), FileMode.Append))
          ;
      }
      catch
      {
        InitializeCheck.savePathState = InitializeCheck.SavePathIssue.WorldGenFilesFail;
      }
label_27:
      try
      {
        if (File.Exists(savePrefix + InitializeCheck.testFile))
          File.Delete(savePrefix + InitializeCheck.testFile);
        if (!File.Exists(savePrefix + InitializeCheck.testSave))
          return;
        File.Delete(savePrefix + InitializeCheck.testSave);
      }
      catch
      {
      }
    }
  }

  private void Quit()
  {
    Debug.Log((object) "Quitting...");
    App.Quit();
  }

  public enum SavePathIssue
  {
    Ok,
    WriteTestFail,
    SpaceTestFail,
    WorldGenFilesFail,
  }
}
