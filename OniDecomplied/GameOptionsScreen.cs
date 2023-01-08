// Decompiled with JetBrains decompiler
// Type: GameOptionsScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class GameOptionsScreen : KModalButtonMenu
{
  [SerializeField]
  private SaveConfigurationScreen saveConfiguration;
  [SerializeField]
  private UnitConfigurationScreen unitConfiguration;
  [SerializeField]
  private KButton resetTutorialButton;
  [SerializeField]
  private KButton controlsButton;
  [SerializeField]
  private KButton sandboxButton;
  [SerializeField]
  private ConfirmDialogScreen confirmPrefab;
  [SerializeField]
  private KButton doneButton;
  [SerializeField]
  private KButton closeButton;
  [SerializeField]
  private GameObject cloudSavesPanel;
  [SerializeField]
  private GameObject defaultToCloudSaveToggle;
  [SerializeField]
  private GameObject savePanel;
  [SerializeField]
  private InputBindingsScreen inputBindingsScreenPrefab;
  [SerializeField]
  private KSlider cameraSpeedSlider;
  [SerializeField]
  private LocText cameraSpeedSliderLabel;
  private const int cameraSliderNotchScale = 10;
  public const string PREFS_KEY_CAMERA_SPEED = "CameraSpeed";

  protected override void OnPrefabInit() => base.OnPrefabInit();

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.unitConfiguration.Init();
    if (Object.op_Inequality((Object) SaveGame.Instance, (Object) null))
    {
      this.saveConfiguration.ToggleDisabledContent(true);
      this.saveConfiguration.Init();
      this.SetSandboxModeActive(SaveGame.Instance.sandboxEnabled);
    }
    else
      this.saveConfiguration.ToggleDisabledContent(false);
    this.resetTutorialButton.onClick += new System.Action(this.OnTutorialReset);
    if (DistributionPlatform.Initialized && SteamUtils.IsSteamRunningOnSteamDeck())
      ((Component) this.controlsButton).gameObject.SetActive(false);
    else
      this.controlsButton.onClick += new System.Action(this.OnKeyBindings);
    this.sandboxButton.onClick += new System.Action(this.OnUnlockSandboxMode);
    this.doneButton.onClick += new System.Action(((KScreen) this).Deactivate);
    this.closeButton.onClick += new System.Action(((KScreen) this).Deactivate);
    if (Object.op_Inequality((Object) this.defaultToCloudSaveToggle, (Object) null))
    {
      this.RefreshCloudSaveToggle();
      this.defaultToCloudSaveToggle.GetComponentInChildren<KButton>().onClick += new System.Action(this.OnDefaultToCloudSaveToggle);
    }
    if (Object.op_Inequality((Object) this.cloudSavesPanel, (Object) null))
      this.cloudSavesPanel.SetActive(SaveLoader.GetCloudSavesAvailable());
    ((Slider) this.cameraSpeedSlider).minValue = 1f;
    ((Slider) this.cameraSpeedSlider).maxValue = 20f;
    // ISSUE: method pointer
    ((UnityEvent<float>) ((Slider) this.cameraSpeedSlider).onValueChanged).AddListener(new UnityAction<float>((object) this, __methodptr(\u003COnSpawn\u003Eb__17_0)));
    ((Slider) this.cameraSpeedSlider).value = this.CameraSpeedToSlider(KPlayerPrefs.GetFloat("CameraSpeed"));
    this.RefreshCameraSliderLabel();
  }

  protected override void OnShow(bool show)
  {
    base.OnShow(show);
    if (Object.op_Inequality((Object) SaveGame.Instance, (Object) null))
    {
      this.savePanel.SetActive(true);
      this.saveConfiguration.Show(show);
      this.SetSandboxModeActive(SaveGame.Instance.sandboxEnabled);
    }
    else
      this.savePanel.SetActive(false);
    if (KPlayerPrefs.HasKey("CameraSpeed"))
      return;
    CameraController.SetDefaultCameraSpeed();
  }

  private float CameraSpeedToSlider(float prefsValue) => prefsValue * 10f;

  private void OnCameraSpeedValueChanged(int sliderValue)
  {
    KPlayerPrefs.SetFloat("CameraSpeed", (float) sliderValue / 10f);
    this.RefreshCameraSliderLabel();
    if (!Object.op_Inequality((Object) Game.Instance, (Object) null))
      return;
    Game.Instance.Trigger(75424175, (object) null);
  }

  private void RefreshCameraSliderLabel() => ((TMP_Text) this.cameraSpeedSliderLabel).text = string.Format((string) STRINGS.UI.FRONTEND.GAME_OPTIONS_SCREEN.CAMERA_SPEED_LABEL, (object) ((float) ((double) KPlayerPrefs.GetFloat("CameraSpeed") * 10.0 * 10.0)).ToString());

  private void OnDefaultToCloudSaveToggle()
  {
    SaveLoader.SetCloudSavesDefault(!SaveLoader.GetCloudSavesDefault());
    this.RefreshCloudSaveToggle();
  }

  private void RefreshCloudSaveToggle()
  {
    bool cloudSavesDefault = SaveLoader.GetCloudSavesDefault();
    this.defaultToCloudSaveToggle.GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(cloudSavesDefault);
  }

  public override void OnKeyDown(KButtonEvent e)
  {
    if (e.TryConsume((Action) 1) || e.TryConsume((Action) 5))
      this.Deactivate();
    else
      base.OnKeyDown(e);
  }

  private void OnTutorialReset()
  {
    ConfirmDialogScreen component = this.ActivateChildScreen(((Component) ScreenPrefabs.Instance.ConfirmDialogScreen).gameObject).GetComponent<ConfirmDialogScreen>();
    component.PopupConfirmDialog((string) STRINGS.UI.FRONTEND.OPTIONS_SCREEN.RESET_TUTORIAL_WARNING, (System.Action) (() => Tutorial.ResetHiddenTutorialMessages()), (System.Action) (() => { }));
    component.Activate();
  }

  private void OnUnlockSandboxMode()
  {
    ConfirmDialogScreen component = this.ActivateChildScreen(((Component) ScreenPrefabs.Instance.ConfirmDialogScreen).gameObject).GetComponent<ConfirmDialogScreen>();
    string unlockSandboxWarning = (string) STRINGS.UI.FRONTEND.OPTIONS_SCREEN.TOGGLE_SANDBOX_SCREEN.UNLOCK_SANDBOX_WARNING;
    System.Action on_confirm = (System.Action) (() =>
    {
      SaveGame.Instance.sandboxEnabled = true;
      this.SetSandboxModeActive(SaveGame.Instance.sandboxEnabled);
      TopLeftControlScreen.Instance.UpdateSandboxToggleState();
      this.Deactivate();
    });
    System.Action on_cancel = (System.Action) (() =>
    {
      SaveLoader.Instance.Save(System.IO.Path.Combine(SaveLoader.GetSavePrefixAndCreateFolder(), SaveGame.Instance.BaseName + (string) STRINGS.UI.FRONTEND.OPTIONS_SCREEN.TOGGLE_SANDBOX_SCREEN.BACKUP_SAVE_GAME_APPEND + ".sav"), updateSavePointer: false);
      this.SetSandboxModeActive(SaveGame.Instance.sandboxEnabled);
      TopLeftControlScreen.Instance.UpdateSandboxToggleState();
      this.Deactivate();
    });
    string confirm = (string) STRINGS.UI.FRONTEND.OPTIONS_SCREEN.TOGGLE_SANDBOX_SCREEN.CONFIRM;
    string confirmSaveBackup = (string) STRINGS.UI.FRONTEND.OPTIONS_SCREEN.TOGGLE_SANDBOX_SCREEN.CONFIRM_SAVE_BACKUP;
    string cancel = (string) STRINGS.UI.FRONTEND.OPTIONS_SCREEN.TOGGLE_SANDBOX_SCREEN.CANCEL;
    string confirm_text = confirm;
    string cancel_text = confirmSaveBackup;
    component.PopupConfirmDialog(unlockSandboxWarning, on_confirm, on_cancel, cancel, (System.Action) (() => { }), confirm_text: confirm_text, cancel_text: cancel_text);
    component.Activate();
  }

  private void OnKeyBindings() => this.ActivateChildScreen(((Component) this.inputBindingsScreenPrefab).gameObject);

  private void SetSandboxModeActive(bool active)
  {
    ((Component) this.sandboxButton).GetComponent<HierarchyReferences>().GetReference("Checkmark").gameObject.SetActive(active);
    this.sandboxButton.isInteractable = !active;
    ((Component) this.sandboxButton).gameObject.GetComponentInParent<CanvasGroup>().alpha = active ? 0.5f : 1f;
  }
}
