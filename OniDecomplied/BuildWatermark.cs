// Decompiled with JetBrains decompiler
// Type: BuildWatermark
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BuildWatermark : KScreen
{
  public bool interactable = true;
  public LocText textDisplay;
  public ToolTip toolTip;
  public KButton button;
  public List<GameObject> archiveIcons;
  public static BuildWatermark Instance;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    BuildWatermark.Instance = this;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.RefreshText();
  }

  public static string GetBuildText()
  {
    string str1 = DistributionPlatform.Initialized ? LaunchInitializer.BuildPrefix() + "-" : "??-";
    string buildText;
    if (Application.isEditor)
    {
      buildText = str1 + "<EDITOR>";
    }
    else
    {
      string str2 = str1 + 535842U.ToString();
      buildText = !DistributionPlatform.Initialized ? str2 + "-?" : str2 + "-" + DlcManager.GetActiveContentLetters();
      if (DebugHandler.enabled)
        buildText += "D";
    }
    return buildText;
  }

  public void RefreshText()
  {
    bool flag1 = true;
    bool flag2 = DistributionPlatform.Initialized && DistributionPlatform.Inst.IsArchiveBranch;
    string buildText = BuildWatermark.GetBuildText();
    this.button.ClearOnClick();
    if (flag1)
    {
      ((TMP_Text) this.textDisplay).SetText(string.Format((string) UI.DEVELOPMENTBUILDS.WATERMARK, (object) buildText));
      this.toolTip.ClearMultiStringTooltip();
    }
    else
    {
      ((TMP_Text) this.textDisplay).SetText(string.Format((string) UI.DEVELOPMENTBUILDS.TESTING_WATERMARK, (object) buildText));
      this.toolTip.SetSimpleTooltip((string) UI.DEVELOPMENTBUILDS.TESTING_TOOLTIP);
      if (this.interactable)
        this.button.onClick += new System.Action(this.ShowTestingMessage);
    }
    foreach (GameObject archiveIcon in this.archiveIcons)
      archiveIcon.SetActive(flag1 & flag2);
  }

  private void ShowTestingMessage() => Util.KInstantiateUI<ConfirmDialogScreen>(((Component) ScreenPrefabs.Instance.ConfirmDialogScreen).gameObject, Global.Instance.globalCanvas, true).PopupConfirmDialog((string) UI.DEVELOPMENTBUILDS.TESTING_MESSAGE, (System.Action) (() => App.OpenWebURL("https://forums.kleientertainment.com/klei-bug-tracker/oni/")), (System.Action) (() => { }), title_text: ((string) UI.DEVELOPMENTBUILDS.TESTING_MESSAGE_TITLE), confirm_text: ((string) UI.DEVELOPMENTBUILDS.TESTING_MORE_INFO));
}
