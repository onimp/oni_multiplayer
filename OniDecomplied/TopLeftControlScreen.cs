// Decompiled with JetBrains decompiler
// Type: TopLeftControlScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TopLeftControlScreen : KScreen
{
  public static TopLeftControlScreen Instance;
  [SerializeField]
  private MultiToggle sandboxToggle;
  [SerializeField]
  private LocText locText;
  [SerializeField]
  private RectTransform secondaryRow;

  public static void DestroyInstance() => TopLeftControlScreen.Instance = (TopLeftControlScreen) null;

  protected virtual void OnActivate()
  {
    base.OnActivate();
    TopLeftControlScreen.Instance = this;
    this.RefreshName();
    // ISSUE: method pointer
    KInputManager.InputChange.AddListener(new UnityAction((object) this, __methodptr(ResetToolTip)));
    this.UpdateSandboxToggleState();
    this.sandboxToggle.onClick += new System.Action(this.OnClickSandboxToggle);
    Game.Instance.Subscribe(-1948169901, (Action<object>) (data => this.UpdateSandboxToggleState()));
    LayoutRebuilder.ForceRebuildLayoutImmediate(this.secondaryRow);
  }

  protected virtual void OnForcedCleanUp()
  {
    // ISSUE: method pointer
    KInputManager.InputChange.RemoveListener(new UnityAction((object) this, __methodptr(ResetToolTip)));
    ((KMonoBehaviour) this).OnForcedCleanUp();
  }

  public void RefreshName()
  {
    if (!Object.op_Inequality((Object) SaveGame.Instance, (Object) null))
      return;
    ((TMP_Text) this.locText).text = SaveGame.Instance.BaseName;
  }

  public void ResetToolTip()
  {
    if (this.CheckSandboxModeLocked())
      ((Component) this.sandboxToggle).GetComponent<ToolTip>().SetSimpleTooltip(GameUtil.ReplaceHotkeyString((string) STRINGS.UI.SANDBOX_TOGGLE.TOOLTIP_LOCKED, (Action) 238));
    else
      ((Component) this.sandboxToggle).GetComponent<ToolTip>().SetSimpleTooltip(GameUtil.ReplaceHotkeyString((string) STRINGS.UI.SANDBOX_TOGGLE.TOOLTIP_UNLOCKED, (Action) 238));
  }

  public void UpdateSandboxToggleState()
  {
    if (this.CheckSandboxModeLocked())
    {
      ((Component) this.sandboxToggle).GetComponent<ToolTip>().SetSimpleTooltip(GameUtil.ReplaceHotkeyString((string) STRINGS.UI.SANDBOX_TOGGLE.TOOLTIP_LOCKED, (Action) 238));
      this.sandboxToggle.ChangeState(0);
    }
    else
    {
      ((Component) this.sandboxToggle).GetComponent<ToolTip>().SetSimpleTooltip(GameUtil.ReplaceHotkeyString((string) STRINGS.UI.SANDBOX_TOGGLE.TOOLTIP_UNLOCKED, (Action) 238));
      this.sandboxToggle.ChangeState(Game.Instance.SandboxModeActive ? 2 : 1);
    }
    ((Component) this.sandboxToggle).gameObject.SetActive(SaveGame.Instance.sandboxEnabled);
  }

  private void OnClickSandboxToggle()
  {
    if (this.CheckSandboxModeLocked())
    {
      KMonoBehaviour.PlaySound(GlobalAssets.GetSound("Negative"));
    }
    else
    {
      KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Click"));
      Game.Instance.SandboxModeActive = !Game.Instance.SandboxModeActive;
    }
    this.UpdateSandboxToggleState();
  }

  private bool CheckSandboxModeLocked() => !SaveGame.Instance.sandboxEnabled;
}
