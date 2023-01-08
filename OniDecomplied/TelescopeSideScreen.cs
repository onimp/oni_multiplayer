// Decompiled with JetBrains decompiler
// Type: TelescopeSideScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TelescopeSideScreen : SideScreenContent
{
  public KButton selectStarmapScreen;
  public Image researchButtonIcon;
  public GameObject content;
  private GameObject target;
  private Action<object> refreshDisplayStateDelegate;
  public LocText DescriptionText;

  public TelescopeSideScreen() => this.refreshDisplayStateDelegate = new Action<object>(this.RefreshDisplayState);

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.selectStarmapScreen.onClick += (System.Action) (() => ManagementMenu.Instance.ToggleStarmap());
    SpacecraftManager.instance.Subscribe(532901469, this.refreshDisplayStateDelegate);
    this.RefreshDisplayState();
  }

  protected virtual void OnCmpEnable()
  {
    ((KMonoBehaviour) this).OnCmpEnable();
    this.RefreshDisplayState();
    this.target = ((Component) ((Component) SelectTool.Instance.selected).GetComponent<KMonoBehaviour>()).gameObject;
  }

  protected virtual void OnCmpDisable()
  {
    ((KMonoBehaviour) this).OnCmpDisable();
    if (!Object.op_Implicit((Object) this.target))
      return;
    this.target = (GameObject) null;
  }

  protected virtual void OnCleanUp()
  {
    base.OnCleanUp();
    if (!Object.op_Implicit((Object) this.target))
      return;
    this.target = (GameObject) null;
  }

  public override bool IsValidForTarget(GameObject target) => Object.op_Inequality((Object) target.GetComponent<Telescope>(), (Object) null);

  private void RefreshDisplayState(object data = null)
  {
    if (Object.op_Equality((Object) SelectTool.Instance.selected, (Object) null) || Object.op_Equality((Object) ((Component) SelectTool.Instance.selected).GetComponent<Telescope>(), (Object) null))
      return;
    if (!SpacecraftManager.instance.HasAnalysisTarget())
      ((TMP_Text) this.DescriptionText).text = "<b><color=#FF0000>" + (string) STRINGS.UI.UISIDESCREENS.TELESCOPESIDESCREEN.NO_SELECTED_ANALYSIS_TARGET + "</color></b>";
    else
      ((TMP_Text) this.DescriptionText).text = (string) STRINGS.UI.UISIDESCREENS.TELESCOPESIDESCREEN.ANALYSIS_TARGET_SELECTED;
  }
}
