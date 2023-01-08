// Decompiled with JetBrains decompiler
// Type: LoreBearer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/LoreBearer")]
public class LoreBearer : KMonoBehaviour, ISidescreenButtonControl
{
  [Serialize]
  private bool BeenClicked;
  public string BeenSearched = (string) UI.USERMENUACTIONS.READLORE.ALREADY_SEARCHED;
  private LoreBearerAction displayContentAction;

  public string content => StringEntry.op_Implicit(Strings.Get("STRINGS.LORE.BUILDINGS." + ((Object) ((Component) this).gameObject).name + ".ENTRY"));

  protected virtual void OnSpawn() => base.OnSpawn();

  public LoreBearer Internal_SetContent(LoreBearerAction action)
  {
    this.displayContentAction = action;
    return this;
  }

  public static InfoDialogScreen ShowPopupDialog() => (InfoDialogScreen) GameScreenManager.Instance.StartScreen(((Component) ScreenPrefabs.Instance.InfoDialogScreen).gameObject, GameScreenManager.Instance.ssOverlayCanvas.gameObject, (GameScreenManager.UIRenderTarget) 2);

  private void OnClickRead()
  {
    InfoDialogScreen screen = LoreBearer.ShowPopupDialog().SetHeader(((Component) this).gameObject.GetComponent<KSelectable>().GetProperName()).AddDefaultOK(true);
    if (this.BeenClicked)
    {
      screen.AddPlainText(this.BeenSearched);
    }
    else
    {
      this.BeenClicked = true;
      if (DlcManager.IsExpansion1Active())
      {
        Scenario.SpawnPrefab(Grid.PosToCell(((Component) this).gameObject), 0, 1, "OrbitalResearchDatabank", Grid.SceneLayer.Front).SetActive(true);
        PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, Assets.GetPrefab(TagExtensions.ToTag("OrbitalResearchDatabank")).GetProperName(), ((Component) this).gameObject.transform);
      }
      if (this.displayContentAction != null)
        this.displayContentAction(screen);
      else
        LoreBearerUtil.UnlockNextJournalEntry(screen);
    }
  }

  public string SidescreenButtonText => (string) (this.BeenClicked ? UI.USERMENUACTIONS.READLORE.ALREADYINSPECTED : UI.USERMENUACTIONS.READLORE.NAME);

  public string SidescreenButtonTooltip => (string) (this.BeenClicked ? UI.USERMENUACTIONS.READLORE.TOOLTIP_ALREADYINSPECTED : UI.USERMENUACTIONS.READLORE.TOOLTIP);

  public bool SidescreenEnabled() => true;

  public void OnSidescreenButtonPressed() => this.OnClickRead();

  public bool SidescreenButtonInteractable() => !this.BeenClicked;

  public int ButtonSideScreenSortOrder() => 20;

  public void SetButtonTextOverride(ButtonMenuTextOverride text) => throw new NotImplementedException();
}
