// Decompiled with JetBrains decompiler
// Type: SetLocker
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using UnityEngine;

public class SetLocker : StateMachineComponent<SetLocker.StatesInstance>, ISidescreenButtonControl
{
  public string[][] possible_contents_ids;
  public string machineSound;
  public string overrideAnim;
  public Vector2I dropOffset = Vector2I.zero;
  public int[] numDataBanks;
  [Serialize]
  private string[] contents;
  [Serialize]
  private bool used;
  private Chore chore;

  protected virtual void OnPrefabInit() => base.OnPrefabInit();

  public void ChooseContents() => this.contents = this.possible_contents_ids[Random.Range(0, this.possible_contents_ids.GetLength(0))];

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
  }

  public void DropContents()
  {
    if (this.contents == null)
      return;
    for (int index = 0; index < this.contents.Length; ++index)
    {
      Scenario.SpawnPrefab(Grid.PosToCell(((Component) this).gameObject), this.dropOffset.x, this.dropOffset.y, this.contents[index], Grid.SceneLayer.Front).SetActive(true);
      PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, Assets.GetPrefab(TagExtensions.ToTag(this.contents[index])).GetProperName(), this.smi.master.transform);
    }
    if (DlcManager.IsExpansion1Active() && this.numDataBanks.Length >= 2)
    {
      int num = Random.Range(this.numDataBanks[0], this.numDataBanks[1]);
      for (int index = 0; index <= num; ++index)
      {
        Scenario.SpawnPrefab(Grid.PosToCell(((Component) this).gameObject), this.dropOffset.x, this.dropOffset.y, "OrbitalResearchDatabank", Grid.SceneLayer.Front).SetActive(true);
        PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Plus, Assets.GetPrefab(TagExtensions.ToTag("OrbitalResearchDatabank")).GetProperName(), this.smi.master.transform);
      }
    }
    EventExtensions.Trigger(((Component) this).gameObject, -372600542, (object) this);
  }

  private void OnClickOpen() => this.ActivateChore();

  private void OnClickCancel() => this.CancelChore();

  public void ActivateChore(object param = null)
  {
    if (this.chore != null)
      return;
    ((Component) this).GetComponent<Workable>().SetWorkTime(1.5f);
    this.chore = (Chore) new WorkChore<Workable>(Db.Get().ChoreTypes.EmptyStorage, (IStateMachineTarget) this, on_complete: ((Action<Chore>) (o => this.CompleteChore())), override_anims: Assets.GetAnim(HashedString.op_Implicit(this.overrideAnim)), priority_class: PriorityScreen.PriorityClass.high);
  }

  public void CancelChore(object param = null)
  {
    if (this.chore == null)
      return;
    this.chore.Cancel("User cancelled");
    this.chore = (Chore) null;
  }

  private void CompleteChore()
  {
    this.used = true;
    this.smi.GoTo((StateMachine.BaseState) this.smi.sm.open);
    this.chore = (Chore) null;
    Game.Instance.userMenu.Refresh(((Component) this).gameObject);
  }

  public string SidescreenButtonText => (string) (this.chore == null ? UI.USERMENUACTIONS.OPENPOI.NAME : UI.USERMENUACTIONS.OPENPOI.NAME_OFF);

  public string SidescreenButtonTooltip => (string) (this.chore == null ? UI.USERMENUACTIONS.OPENPOI.TOOLTIP : UI.USERMENUACTIONS.OPENPOI.TOOLTIP_OFF);

  public bool SidescreenEnabled() => true;

  public void OnSidescreenButtonPressed()
  {
    if (this.chore == null)
      this.OnClickOpen();
    else
      this.OnClickCancel();
  }

  public bool SidescreenButtonInteractable() => !this.used;

  public int ButtonSideScreenSortOrder() => 20;

  public void SetButtonTextOverride(ButtonMenuTextOverride text) => throw new NotImplementedException();

  public class StatesInstance : 
    GameStateMachine<SetLocker.States, SetLocker.StatesInstance, SetLocker, object>.GameInstance
  {
    public StatesInstance(SetLocker master)
      : base(master)
    {
    }
  }

  public class States : GameStateMachine<SetLocker.States, SetLocker.StatesInstance, SetLocker>
  {
    public GameStateMachine<SetLocker.States, SetLocker.StatesInstance, SetLocker, object>.State closed;
    public GameStateMachine<SetLocker.States, SetLocker.StatesInstance, SetLocker, object>.State open;
    public GameStateMachine<SetLocker.States, SetLocker.StatesInstance, SetLocker, object>.State off;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.closed;
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      this.closed.PlayAnim("on").Enter((StateMachine<SetLocker.States, SetLocker.StatesInstance, SetLocker, object>.State.Callback) (smi =>
      {
        if (smi.master.machineSound == null)
          return;
        LoopingSounds component = ((Component) smi.master).GetComponent<LoopingSounds>();
        if (!Object.op_Inequality((Object) component, (Object) null))
          return;
        component.StartSound(GlobalAssets.GetSound(smi.master.machineSound));
      }));
      this.open.PlayAnim("working").OnAnimQueueComplete(this.off).Exit((StateMachine<SetLocker.States, SetLocker.StatesInstance, SetLocker, object>.State.Callback) (smi => smi.master.DropContents()));
      this.off.PlayAnim("off").Enter((StateMachine<SetLocker.States, SetLocker.StatesInstance, SetLocker, object>.State.Callback) (smi =>
      {
        if (smi.master.machineSound == null)
          return;
        LoopingSounds component = ((Component) smi.master).GetComponent<LoopingSounds>();
        if (!Object.op_Inequality((Object) component, (Object) null))
          return;
        component.StopSound(GlobalAssets.GetSound(smi.master.machineSound));
      }));
    }
  }
}
