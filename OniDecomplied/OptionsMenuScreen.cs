// Decompiled with JetBrains decompiler
// Type: OptionsMenuScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class OptionsMenuScreen : KModalButtonMenu
{
  [SerializeField]
  private GameOptionsScreen gameOptionsScreenPrefab;
  [SerializeField]
  private AudioOptionsScreen audioOptionsScreenPrefab;
  [SerializeField]
  private GraphicsOptionsScreen graphicsOptionsScreenPrefab;
  [SerializeField]
  private CreditsScreen creditsScreenPrefab;
  [SerializeField]
  private KButton closeButton;
  [SerializeField]
  private MetricsOptionsScreen metricsScreenPrefab;
  [SerializeField]
  private FeedbackScreen feedbackScreenPrefab;
  [SerializeField]
  private LocText title;
  [SerializeField]
  private KButton backButton;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.keepMenuOpen = true;
    // ISSUE: method pointer
    // ISSUE: method pointer
    // ISSUE: method pointer
    // ISSUE: method pointer
    // ISSUE: method pointer
    // ISSUE: method pointer
    this.buttons = (IList<KButtonMenu.ButtonInfo>) new List<KButtonMenu.ButtonInfo>()
    {
      new KButtonMenu.ButtonInfo((string) UI.FRONTEND.OPTIONS_SCREEN.GRAPHICS, (Action) 275, new UnityAction((object) this, __methodptr(OnGraphicsOptions))),
      new KButtonMenu.ButtonInfo((string) UI.FRONTEND.OPTIONS_SCREEN.AUDIO, (Action) 275, new UnityAction((object) this, __methodptr(OnAudioOptions))),
      new KButtonMenu.ButtonInfo((string) UI.FRONTEND.OPTIONS_SCREEN.GAME, (Action) 275, new UnityAction((object) this, __methodptr(OnGameOptions))),
      new KButtonMenu.ButtonInfo((string) UI.FRONTEND.OPTIONS_SCREEN.METRICS, (Action) 275, new UnityAction((object) this, __methodptr(OnMetrics))),
      new KButtonMenu.ButtonInfo((string) UI.FRONTEND.OPTIONS_SCREEN.FEEDBACK, (Action) 275, new UnityAction((object) this, __methodptr(OnFeedback))),
      new KButtonMenu.ButtonInfo((string) UI.FRONTEND.OPTIONS_SCREEN.CREDITS, (Action) 275, new UnityAction((object) this, __methodptr(OnCredits)))
    };
    this.closeButton.onClick += new System.Action(((KScreen) this).Deactivate);
    this.backButton.onClick += new System.Action(((KScreen) this).Deactivate);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    ((TMP_Text) this.title).SetText((string) UI.FRONTEND.OPTIONS_SCREEN.TITLE);
    ((KMonoBehaviour) this.backButton).transform.SetAsLastSibling();
  }

  protected override void OnActivate()
  {
    base.OnActivate();
    foreach (GameObject buttonObject in this.buttonObjects)
      ;
  }

  public override void OnKeyDown(KButtonEvent e)
  {
    if (e.TryConsume((Action) 1) || e.TryConsume((Action) 5))
      this.Deactivate();
    else
      base.OnKeyDown(e);
  }

  private void OnGraphicsOptions() => this.ActivateChildScreen(((Component) this.graphicsOptionsScreenPrefab).gameObject);

  private void OnAudioOptions() => this.ActivateChildScreen(((Component) this.audioOptionsScreenPrefab).gameObject);

  private void OnGameOptions() => this.ActivateChildScreen(((Component) this.gameOptionsScreenPrefab).gameObject);

  private void OnMetrics() => this.ActivateChildScreen(((Component) this.metricsScreenPrefab).gameObject);

  public void ShowMetricsScreen() => this.ActivateChildScreen(((Component) this.metricsScreenPrefab).gameObject);

  private void OnFeedback() => this.ActivateChildScreen(((Component) this.feedbackScreenPrefab).gameObject);

  private void OnCredits() => this.ActivateChildScreen(((Component) this.creditsScreenPrefab).gameObject);

  private void Update() => Debug.developerConsoleVisible = false;
}
