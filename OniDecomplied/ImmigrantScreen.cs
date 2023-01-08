// Decompiled with JetBrains decompiler
// Type: ImmigrantScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using TMPro;
using UnityEngine;

public class ImmigrantScreen : CharacterSelectionController
{
  [SerializeField]
  private KButton closeButton;
  [SerializeField]
  private KButton rejectButton;
  [SerializeField]
  private LocText title;
  [SerializeField]
  private GameObject rejectConfirmationScreen;
  [SerializeField]
  private KButton confirmRejectionBtn;
  [SerializeField]
  private KButton cancelRejectionBtn;
  public static ImmigrantScreen instance;
  private Telepad telepad;
  private bool hasShown;

  public static void DestroyInstance() => ImmigrantScreen.instance = (ImmigrantScreen) null;

  public Telepad Telepad => this.telepad;

  protected override void OnPrefabInit() => base.OnPrefabInit();

  protected virtual void OnSpawn()
  {
    this.activateOnSpawn = false;
    this.ConsumeMouseScroll = false;
    base.OnSpawn();
    this.IsStarterMinion = false;
    this.rejectButton.onClick += new System.Action(this.OnRejectAll);
    this.confirmRejectionBtn.onClick += new System.Action(this.OnRejectionConfirmed);
    this.cancelRejectionBtn.onClick += new System.Action(this.OnRejectionCancelled);
    ImmigrantScreen.instance = this;
    ((TMP_Text) this.title).text = (string) UI.IMMIGRANTSCREEN.IMMIGRANTSCREENTITLE;
    ((TMP_Text) ((Component) this.proceedButton).GetComponentInChildren<LocText>()).text = (string) UI.IMMIGRANTSCREEN.PROCEEDBUTTON;
    this.closeButton.onClick += (System.Action) (() => this.Show(false));
    this.Show(false);
  }

  protected override void OnShow(bool show)
  {
    if (show)
    {
      KFMOD.PlayUISound(GlobalAssets.GetSound("Dialog_Popup"));
      AudioMixer.instance.Start(AudioMixerSnapshots.Get().MENUNewDuplicantSnapshot);
      MusicManager.instance.PlaySong("Music_SelectDuplicant");
      this.hasShown = true;
    }
    else
    {
      AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MENUNewDuplicantSnapshot);
      if (MusicManager.instance.SongIsPlaying("Music_SelectDuplicant"))
        MusicManager.instance.StopSong("Music_SelectDuplicant");
      if (Immigration.Instance.ImmigrantsAvailable && this.hasShown)
        AudioMixer.instance.Start(AudioMixerSnapshots.Get().PortalLPDimmedSnapshot);
    }
    base.OnShow(show);
  }

  public void DebugShuffleOptions()
  {
    this.OnRejectionConfirmed();
    Immigration.Instance.timeBeforeSpawn = 0.0f;
  }

  public override void OnPressBack()
  {
    if (this.rejectConfirmationScreen.activeSelf)
      this.OnRejectionCancelled();
    else
      base.OnPressBack();
  }

  public virtual void Deactivate() => this.Show(false);

  public static void InitializeImmigrantScreen(Telepad telepad)
  {
    ImmigrantScreen.instance.Initialize(telepad);
    ImmigrantScreen.instance.Show(true);
  }

  private void Initialize(Telepad telepad)
  {
    this.InitializeContainers();
    foreach (ITelepadDeliverableContainer container in this.containers)
    {
      CharacterContainer characterContainer = container as CharacterContainer;
      if (Object.op_Inequality((Object) characterContainer, (Object) null))
        characterContainer.SetReshufflingState(false);
    }
    this.telepad = telepad;
  }

  protected override void OnProceed()
  {
    this.telepad.OnAcceptDelivery(this.selectedDeliverables[0]);
    this.Show(false);
    this.containers.ForEach((Action<ITelepadDeliverableContainer>) (cc => Object.Destroy((Object) cc.GetGameObject())));
    this.containers.Clear();
    AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MENUNewDuplicantSnapshot);
    AudioMixer.instance.Stop(AudioMixerSnapshots.Get().PortalLPDimmedSnapshot);
    MusicManager.instance.PlaySong("Stinger_NewDuplicant");
  }

  private void OnRejectAll()
  {
    this.rejectConfirmationScreen.transform.SetAsLastSibling();
    this.rejectConfirmationScreen.SetActive(true);
  }

  private void OnRejectionCancelled() => this.rejectConfirmationScreen.SetActive(false);

  private void OnRejectionConfirmed()
  {
    this.telepad.RejectAll();
    this.containers.ForEach((Action<ITelepadDeliverableContainer>) (cc => Object.Destroy((Object) cc.GetGameObject())));
    this.containers.Clear();
    this.rejectConfirmationScreen.SetActive(false);
    this.Show(false);
    AudioMixer.instance.Stop(AudioMixerSnapshots.Get().MENUNewDuplicantSnapshot);
    AudioMixer.instance.Stop(AudioMixerSnapshots.Get().PortalLPDimmedSnapshot);
  }
}
