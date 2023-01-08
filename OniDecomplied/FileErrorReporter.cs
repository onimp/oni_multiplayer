// Decompiled with JetBrains decompiler
// Type: FileErrorReporter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using UnityEngine;
using UnityEngine.UI;

[AddComponentMenu("KMonoBehaviour/scripts/FileErrorReporter")]
public class FileErrorReporter : KMonoBehaviour
{
  protected virtual void OnSpawn()
  {
    this.OnFileError();
    FileUtil.onErrorMessage += new System.Action(this.OnFileError);
  }

  private void OnFileError()
  {
    if (FileUtil.errorType == null)
      return;
    string text;
    switch ((int) FileUtil.errorType)
    {
      case 1:
        text = string.Format((string) STRINGS.UI.FRONTEND.SUPPORTWARNINGS.IO_UNAUTHORIZED, (object) FileUtil.errorSubject);
        break;
      case 2:
        text = string.Format((string) STRINGS.UI.FRONTEND.SUPPORTWARNINGS.IO_SUFFICIENT_SPACE, (object) FileUtil.errorSubject);
        break;
      default:
        text = string.Format((string) STRINGS.UI.FRONTEND.SUPPORTWARNINGS.IO_UNKNOWN, (object) FileUtil.errorSubject);
        break;
    }
    GameObject gameObject;
    if (Object.op_Inequality((Object) FrontEndManager.Instance, (Object) null))
      gameObject = ((Component) FrontEndManager.Instance).gameObject;
    else if (Object.op_Inequality((Object) GameScreenManager.Instance, (Object) null) && Object.op_Inequality((Object) GameScreenManager.Instance.ssOverlayCanvas, (Object) null))
    {
      gameObject = GameScreenManager.Instance.ssOverlayCanvas;
    }
    else
    {
      gameObject = new GameObject();
      ((Object) gameObject).name = "FileErrorCanvas";
      Object.DontDestroyOnLoad((Object) gameObject);
      Canvas canvas = gameObject.AddComponent<Canvas>();
      canvas.renderMode = (RenderMode) 0;
      canvas.additionalShaderChannels = (AdditionalCanvasShaderChannels) 1;
      canvas.sortingOrder = 10;
      gameObject.AddComponent<GraphicRaycaster>();
    }
    if ((FileUtil.exceptionMessage != null || FileUtil.exceptionStackTrace != null) && !KCrashReporter.hasReportedError)
      KCrashReporter.ReportError(FileUtil.exceptionMessage, FileUtil.exceptionStackTrace, (string) null, (ConfirmDialogScreen) null, (GameObject) null);
    ConfirmDialogScreen component = Util.KInstantiateUI(((Component) ScreenPrefabs.Instance.ConfirmDialogScreen).gameObject, gameObject, true).GetComponent<ConfirmDialogScreen>();
    component.PopupConfirmDialog(text, (System.Action) null, (System.Action) null);
    Object.DontDestroyOnLoad((Object) ((Component) component).gameObject);
  }

  private void OpenMoreInfo()
  {
  }
}
