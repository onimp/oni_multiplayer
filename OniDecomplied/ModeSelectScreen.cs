// Decompiled with JetBrains decompiler
// Type: ModeSelectScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ModeSelectScreen : NewGameFlowScreen
{
  [SerializeField]
  private MultiToggle nosweatButton;
  private Image nosweatButtonHeader;
  private Image nosweatButtonSelectionFrame;
  [SerializeField]
  private MultiToggle survivalButton;
  private Image survivalButtonHeader;
  private Image survivalButtonSelectionFrame;
  [SerializeField]
  private LocText descriptionArea;
  [SerializeField]
  private KButton closeButton;
  [SerializeField]
  private KBatchedAnimController nosweatAnim;
  [SerializeField]
  private KBatchedAnimController survivalAnim;
  private static bool dataLoaded;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.LoadWorldAndClusterData();
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    HierarchyReferences component1 = ((Component) this.survivalButton).GetComponent<HierarchyReferences>();
    this.survivalButtonHeader = ((Component) component1.GetReference<RectTransform>("HeaderBackground")).GetComponent<Image>();
    this.survivalButtonSelectionFrame = ((Component) component1.GetReference<RectTransform>("SelectionFrame")).GetComponent<Image>();
    this.survivalButton.onEnter += new System.Action(this.OnHoverEnterSurvival);
    this.survivalButton.onExit += new System.Action(this.OnHoverExitSurvival);
    this.survivalButton.onClick += new System.Action(this.OnClickSurvival);
    HierarchyReferences component2 = ((Component) this.nosweatButton).GetComponent<HierarchyReferences>();
    this.nosweatButtonHeader = ((Component) component2.GetReference<RectTransform>("HeaderBackground")).GetComponent<Image>();
    this.nosweatButtonSelectionFrame = ((Component) component2.GetReference<RectTransform>("SelectionFrame")).GetComponent<Image>();
    this.nosweatButton.onEnter += new System.Action(this.OnHoverEnterNosweat);
    this.nosweatButton.onExit += new System.Action(this.OnHoverExitNosweat);
    this.nosweatButton.onClick += new System.Action(this.OnClickNosweat);
    this.closeButton.onClick += new System.Action(((NewGameFlowScreen) this).NavigateBackward);
  }

  private void OnHoverEnterSurvival()
  {
    KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover"));
    KMonoBehaviourExtensions.SetAlpha(this.survivalButtonSelectionFrame, 1f);
    ((Graphic) this.survivalButtonHeader).color = new Color(0.7019608f, 0.3647059f, 0.533333361f, 1f);
    ((TMP_Text) this.descriptionArea).text = (string) STRINGS.UI.FRONTEND.MODESELECTSCREEN.SURVIVAL_DESC;
  }

  private void OnHoverExitSurvival()
  {
    KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover"));
    KMonoBehaviourExtensions.SetAlpha(this.survivalButtonSelectionFrame, 0.0f);
    ((Graphic) this.survivalButtonHeader).color = new Color(0.309803933f, 0.34117648f, 0.384313732f, 1f);
    ((TMP_Text) this.descriptionArea).text = (string) STRINGS.UI.FRONTEND.MODESELECTSCREEN.BLANK_DESC;
  }

  private void OnClickSurvival()
  {
    this.Deactivate();
    CustomGameSettings.Instance.SetSurvivalDefaults();
    this.NavigateForward();
  }

  private void LoadWorldAndClusterData()
  {
    if (ModeSelectScreen.dataLoaded)
      return;
    CustomGameSettings.Instance.LoadClusters();
    Global.Instance.modManager.Report(((Component) this).gameObject);
    ModeSelectScreen.dataLoaded = true;
  }

  private void OnHoverEnterNosweat()
  {
    KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover"));
    KMonoBehaviourExtensions.SetAlpha(this.nosweatButtonSelectionFrame, 1f);
    ((Graphic) this.nosweatButtonHeader).color = new Color(0.7019608f, 0.3647059f, 0.533333361f, 1f);
    ((TMP_Text) this.descriptionArea).text = (string) STRINGS.UI.FRONTEND.MODESELECTSCREEN.NOSWEAT_DESC;
  }

  private void OnHoverExitNosweat()
  {
    KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover"));
    KMonoBehaviourExtensions.SetAlpha(this.nosweatButtonSelectionFrame, 0.0f);
    ((Graphic) this.nosweatButtonHeader).color = new Color(0.309803933f, 0.34117648f, 0.384313732f, 1f);
    ((TMP_Text) this.descriptionArea).text = (string) STRINGS.UI.FRONTEND.MODESELECTSCREEN.BLANK_DESC;
  }

  private void OnClickNosweat()
  {
    this.Deactivate();
    CustomGameSettings.Instance.SetNosweatDefaults();
    this.NavigateForward();
  }
}
