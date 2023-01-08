// Decompiled with JetBrains decompiler
// Type: ClusterCategorySelectionScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ClusterCategorySelectionScreen : NewGameFlowScreen
{
  [SerializeField]
  private MultiToggle spacedOutButton;
  private Image spacedOutButtonHeader;
  private Image spacedOutButtonSelectionFrame;
  [SerializeField]
  private MultiToggle vanillaButton;
  private Image vanillaButtonHeader;
  private Image vanillalButtonSelectionFrame;
  [SerializeField]
  private LocText descriptionArea;
  [SerializeField]
  private KButton closeButton;
  [SerializeField]
  private KBatchedAnimController nosweatAnim;
  [SerializeField]
  private KBatchedAnimController survivalAnim;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    HierarchyReferences component1 = ((Component) this.vanillaButton).GetComponent<HierarchyReferences>();
    this.vanillaButtonHeader = ((Component) component1.GetReference<RectTransform>("HeaderBackground")).GetComponent<Image>();
    this.vanillalButtonSelectionFrame = ((Component) component1.GetReference<RectTransform>("SelectionFrame")).GetComponent<Image>();
    this.vanillaButton.onEnter += new System.Action(this.OnHoverEnterVanilla);
    this.vanillaButton.onExit += new System.Action(this.OnHoverExitVanilla);
    this.vanillaButton.onClick += new System.Action(this.OnClickVanilla);
    HierarchyReferences component2 = ((Component) this.spacedOutButton).GetComponent<HierarchyReferences>();
    this.spacedOutButtonHeader = ((Component) component2.GetReference<RectTransform>("HeaderBackground")).GetComponent<Image>();
    this.spacedOutButtonSelectionFrame = ((Component) component2.GetReference<RectTransform>("SelectionFrame")).GetComponent<Image>();
    this.spacedOutButton.onEnter += new System.Action(this.OnHoverEnterSpacedOut);
    this.spacedOutButton.onExit += new System.Action(this.OnHoverExitSpacedOut);
    this.spacedOutButton.onClick += new System.Action(this.OnClickSpacedOut);
    this.closeButton.onClick += new System.Action(((NewGameFlowScreen) this).NavigateBackward);
  }

  private void OnHoverEnterVanilla()
  {
    KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover"));
    KMonoBehaviourExtensions.SetAlpha(this.vanillalButtonSelectionFrame, 1f);
    ((Graphic) this.vanillaButtonHeader).color = new Color(0.7019608f, 0.3647059f, 0.533333361f, 1f);
    ((TMP_Text) this.descriptionArea).text = (string) STRINGS.UI.FRONTEND.CLUSTERCATEGORYSELECTSCREEN.VANILLA_DESC;
  }

  private void OnHoverExitVanilla()
  {
    KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover"));
    KMonoBehaviourExtensions.SetAlpha(this.vanillalButtonSelectionFrame, 0.0f);
    ((Graphic) this.vanillaButtonHeader).color = new Color(0.309803933f, 0.34117648f, 0.384313732f, 1f);
    ((TMP_Text) this.descriptionArea).text = (string) STRINGS.UI.FRONTEND.CLUSTERCATEGORYSELECTSCREEN.BLANK_DESC;
  }

  private void OnClickVanilla()
  {
    this.Deactivate();
    DestinationSelectPanel.ChosenClusterCategorySetting = 1;
    this.NavigateForward();
  }

  private void OnHoverEnterSpacedOut()
  {
    KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover"));
    KMonoBehaviourExtensions.SetAlpha(this.spacedOutButtonSelectionFrame, 1f);
    ((Graphic) this.spacedOutButtonHeader).color = new Color(0.7019608f, 0.3647059f, 0.533333361f, 1f);
    ((TMP_Text) this.descriptionArea).text = (string) STRINGS.UI.FRONTEND.CLUSTERCATEGORYSELECTSCREEN.SPACEDOUT_DESC;
  }

  private void OnHoverExitSpacedOut()
  {
    KMonoBehaviour.PlaySound(GlobalAssets.GetSound("HUD_Mouseover"));
    KMonoBehaviourExtensions.SetAlpha(this.spacedOutButtonSelectionFrame, 0.0f);
    ((Graphic) this.spacedOutButtonHeader).color = new Color(0.309803933f, 0.34117648f, 0.384313732f, 1f);
    ((TMP_Text) this.descriptionArea).text = (string) STRINGS.UI.FRONTEND.CLUSTERCATEGORYSELECTSCREEN.BLANK_DESC;
  }

  private void OnClickSpacedOut()
  {
    this.Deactivate();
    DestinationSelectPanel.ChosenClusterCategorySetting = 2;
    this.NavigateForward();
  }
}
