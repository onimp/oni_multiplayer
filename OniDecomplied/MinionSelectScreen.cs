// Decompiled with JetBrains decompiler
// Type: MinionSelectScreen
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using TMPro;
using UnityEngine;

public class MinionSelectScreen : CharacterSelectionController
{
  [SerializeField]
  private NewBaseScreen newBasePrefab;
  [SerializeField]
  private WattsonMessage wattsonMessagePrefab;
  public const string WattsonGameObjName = "WattsonMessage";
  public KButton backButton;

  protected override void OnPrefabInit()
  {
    this.IsStarterMinion = true;
    base.OnPrefabInit();
    if (MusicManager.instance.SongIsPlaying("Music_FrontEnd"))
      MusicManager.instance.SetSongParameter("Music_FrontEnd", "songSection", 2f);
    GameObject gameObject = Util.KInstantiateUI(((Component) this.wattsonMessagePrefab).gameObject, GameObject.Find("ScreenSpaceOverlayCanvas"), false);
    ((Object) gameObject).name = "WattsonMessage";
    gameObject.SetActive(false);
    Game.Instance.Subscribe(-1992507039, new Action<object>(this.OnBaseAlreadyCreated));
    this.backButton.onClick += (System.Action) (() =>
    {
      LoadScreen.ForceStopGame();
      App.LoadScene("frontend");
    });
    this.InitializeContainers();
  }

  public void SetProceedButtonActive(bool state, string tooltip = null)
  {
    if (state)
      this.EnableProceedButton();
    else
      this.DisableProceedButton();
    ToolTip component = ((Component) this.proceedButton).GetComponent<ToolTip>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    if (tooltip != null)
      component.toolTip = tooltip;
    else
      component.ClearMultiStringTooltip();
  }

  protected virtual void OnSpawn()
  {
    this.OnDeliverableAdded();
    this.EnableProceedButton();
    ((TMP_Text) ((Component) this.proceedButton).GetComponentInChildren<LocText>()).text = (string) UI.IMMIGRANTSCREEN.EMBARK;
    this.containers.ForEach((Action<ITelepadDeliverableContainer>) (container =>
    {
      CharacterContainer characterContainer = container as CharacterContainer;
      if (!Object.op_Inequality((Object) characterContainer, (Object) null))
        return;
      characterContainer.DisableSelectButton();
    }));
  }

  protected override void OnProceed()
  {
    Util.KInstantiateUI(((Component) this.newBasePrefab).gameObject, GameScreenManager.Instance.ssOverlayCanvas, false);
    MusicManager.instance.StopSong("Music_FrontEnd");
    AudioMixer.instance.Start(AudioMixerSnapshots.Get().NewBaseSetupSnapshot);
    AudioMixer.instance.Stop(AudioMixerSnapshots.Get().FrontEndWorldGenerationSnapshot);
    this.selectedDeliverables.Clear();
    foreach (CharacterContainer container in this.containers)
      this.selectedDeliverables.Add((ITelepadDeliverable) container.Stats);
    NewBaseScreen.Instance.Init(SaveLoader.Instance.ClusterLayout, this.selectedDeliverables.ToArray());
    if (this.OnProceedEvent != null)
      this.OnProceedEvent();
    Game.Instance.Trigger(-838649377, (object) null);
    ((Component) BuildWatermark.Instance).gameObject.SetActive(false);
    this.Deactivate();
  }

  private void OnBaseAlreadyCreated(object data)
  {
    Game.Instance.StopFE();
    Game.Instance.StartBE();
    Game.Instance.SetGameStarted();
    this.Deactivate();
  }

  private void ReshuffleAll()
  {
    if (this.OnReshuffleEvent == null)
      return;
    this.OnReshuffleEvent(this.IsStarterMinion);
  }

  public override void OnPressBack()
  {
    foreach (ITelepadDeliverableContainer container in this.containers)
    {
      CharacterContainer characterContainer = container as CharacterContainer;
      if (Object.op_Inequality((Object) characterContainer, (Object) null))
        characterContainer.ForceStopEditingTitle();
    }
  }
}
