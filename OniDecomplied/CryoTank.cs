// Decompiled with JetBrains decompiler
// Type: CryoTank
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using UnityEngine;

public class CryoTank : StateMachineComponent<CryoTank.StatesInstance>, ISidescreenButtonControl
{
  public string[][] possible_contents_ids;
  public string machineSound;
  public string overrideAnim;
  public CellOffset dropOffset = CellOffset.none;
  private GameObject opener;
  private Chore chore;

  public string SidescreenButtonText => (string) BUILDINGS.PREFABS.CRYOTANK.DEFROSTBUTTON;

  public string SidescreenButtonTooltip => (string) BUILDINGS.PREFABS.CRYOTANK.DEFROSTBUTTONTOOLTIP;

  public bool SidescreenEnabled() => true;

  public void OnSidescreenButtonPressed() => this.OnClickOpen();

  public bool SidescreenButtonInteractable() => this.HasDefrostedFriend();

  public int ButtonSideScreenSortOrder() => 20;

  public void SetButtonTextOverride(ButtonMenuTextOverride text) => throw new NotImplementedException();

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
    Demolishable component = ((Component) this).GetComponent<Demolishable>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.allowDemolition = !this.HasDefrostedFriend();
  }

  public bool HasDefrostedFriend() => this.smi.IsInsideState((StateMachine.BaseState) this.smi.sm.closed) && this.chore == null;

  public void DropContents()
  {
    GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(MinionConfig.ID)), (GameObject) null, (string) null);
    ((Object) gameObject).name = ((Object) Assets.GetPrefab(Tag.op_Implicit(MinionConfig.ID))).name;
    Immigration.Instance.ApplyDefaultPersonalPriorities(gameObject);
    Vector3 posCbc = Grid.CellToPosCBC(Grid.OffsetCell(Grid.PosToCell(this.transform.position), this.dropOffset), Grid.SceneLayer.Move);
    TransformExtensions.SetLocalPosition(gameObject.transform, posCbc);
    gameObject.SetActive(true);
    new MinionStartingStats(false, guaranteedTraitID: "AncientKnowledge").Apply(gameObject);
    gameObject.GetComponent<MinionIdentity>().arrivalTime = (float) Random.Range(-2000, -1000);
    MinionResume component1 = gameObject.GetComponent<MinionResume>();
    int num = 3;
    for (int index = 0; index < num; ++index)
      component1.ForceAddSkillPoint();
    this.smi.sm.defrostedDuplicant.Set(gameObject, this.smi, false);
    gameObject.GetComponent<Navigator>().SetCurrentNavType(NavType.Floor);
    ChoreProvider component2 = gameObject.GetComponent<ChoreProvider>();
    if (Object.op_Inequality((Object) component2, (Object) null))
    {
      this.smi.defrostAnimChore = (Chore) new EmoteChore((IStateMachineTarget) component2, Db.Get().ChoreTypes.EmoteHighPriority, HashedString.op_Implicit("anim_interacts_cryo_chamber_kanim"), new HashedString[2]
      {
        HashedString.op_Implicit("defrost"),
        HashedString.op_Implicit("defrost_exit")
      }, (KAnim.PlayMode) 1);
      Vector3 position = TransformExtensions.GetPosition(gameObject.transform);
      position.z = Grid.GetLayerZ(Grid.SceneLayer.Gas);
      TransformExtensions.SetPosition(gameObject.transform, position);
      gameObject.GetMyWorld().SetDupeVisited();
    }
    ((Component) SaveGame.Instance).GetComponent<ColonyAchievementTracker>().defrostedDuplicant = true;
  }

  public void ShowEventPopup()
  {
    GameObject go = this.smi.sm.defrostedDuplicant.Get(this.smi);
    if (!Object.op_Inequality((Object) this.opener, (Object) null) || !Object.op_Inequality((Object) go, (Object) null))
      return;
    SimpleEvent.StatesInstance smi = GameplayEventManager.Instance.StartNewEvent(Db.Get().GameplayEvents.CryoFriend).smi as SimpleEvent.StatesInstance;
    smi.minions = new GameObject[2]{ go, this.opener };
    smi.SetTextParameter("dupe", this.opener.GetProperName());
    smi.SetTextParameter("friend", go.GetProperName());
    smi.ShowEventPopup();
  }

  public void Cheer()
  {
    GameObject gameObject = this.smi.sm.defrostedDuplicant.Get(this.smi);
    if (!Object.op_Inequality((Object) this.opener, (Object) null) || !Object.op_Inequality((Object) gameObject, (Object) null))
      return;
    Db db = Db.Get();
    this.opener.GetComponent<Effects>().Add(Db.Get().effects.Get("CryoFriend"), true);
    EmoteChore emoteChore1 = new EmoteChore((IStateMachineTarget) this.opener.GetComponent<Effects>(), db.ChoreTypes.EmoteHighPriority, db.Emotes.Minion.Cheer);
    gameObject.GetComponent<Effects>().Add(Db.Get().effects.Get("CryoFriend"), true);
    EmoteChore emoteChore2 = new EmoteChore((IStateMachineTarget) gameObject.GetComponent<Effects>(), db.ChoreTypes.EmoteHighPriority, db.Emotes.Minion.Cheer);
  }

  private void OnClickOpen() => this.ActivateChore();

  private void OnClickCancel() => this.CancelActivateChore();

  public void ActivateChore(object param = null)
  {
    if (this.chore != null)
      return;
    ((Component) this).GetComponent<Workable>().SetWorkTime(1.5f);
    this.chore = (Chore) new WorkChore<Workable>(Db.Get().ChoreTypes.EmptyStorage, (IStateMachineTarget) this, on_complete: ((Action<Chore>) (o => this.CompleteActivateChore())), override_anims: Assets.GetAnim(HashedString.op_Implicit(this.overrideAnim)), priority_class: PriorityScreen.PriorityClass.high);
  }

  public void CancelActivateChore(object param = null)
  {
    if (this.chore == null)
      return;
    this.chore.Cancel("User cancelled");
    this.chore = (Chore) null;
  }

  private void CompleteActivateChore()
  {
    this.opener = ((Component) this.chore.driver).gameObject;
    this.smi.GoTo((StateMachine.BaseState) this.smi.sm.open);
    this.chore = (Chore) null;
    Demolishable component = this.smi.GetComponent<Demolishable>();
    if (Object.op_Inequality((Object) component, (Object) null))
      component.allowDemolition = true;
    Game.Instance.userMenu.Refresh(((Component) this).gameObject);
  }

  public class StatesInstance : 
    GameStateMachine<CryoTank.States, CryoTank.StatesInstance, CryoTank, object>.GameInstance
  {
    public Chore defrostAnimChore;

    public StatesInstance(CryoTank master)
      : base(master)
    {
    }
  }

  public class States : GameStateMachine<CryoTank.States, CryoTank.StatesInstance, CryoTank>
  {
    public StateMachine<CryoTank.States, CryoTank.StatesInstance, CryoTank, object>.TargetParameter defrostedDuplicant;
    public GameStateMachine<CryoTank.States, CryoTank.StatesInstance, CryoTank, object>.State closed;
    public GameStateMachine<CryoTank.States, CryoTank.StatesInstance, CryoTank, object>.State open;
    public GameStateMachine<CryoTank.States, CryoTank.StatesInstance, CryoTank, object>.State defrost;
    public GameStateMachine<CryoTank.States, CryoTank.StatesInstance, CryoTank, object>.State defrostExit;
    public GameStateMachine<CryoTank.States, CryoTank.StatesInstance, CryoTank, object>.State off;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.closed;
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      this.closed.PlayAnim("on").Enter((StateMachine<CryoTank.States, CryoTank.StatesInstance, CryoTank, object>.State.Callback) (smi =>
      {
        if (smi.master.machineSound == null)
          return;
        LoopingSounds component = ((Component) smi.master).GetComponent<LoopingSounds>();
        if (!Object.op_Inequality((Object) component, (Object) null))
          return;
        component.StartSound(GlobalAssets.GetSound(smi.master.machineSound));
      }));
      this.open.GoTo(this.defrost).Exit((StateMachine<CryoTank.States, CryoTank.StatesInstance, CryoTank, object>.State.Callback) (smi => smi.master.DropContents()));
      this.defrost.PlayAnim("defrost").OnAnimQueueComplete(this.defrostExit).Update((Action<CryoTank.StatesInstance, float>) ((smi, dt) => smi.sm.defrostedDuplicant.Get(smi).GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.BuildingUse))).Exit((StateMachine<CryoTank.States, CryoTank.StatesInstance, CryoTank, object>.State.Callback) (smi => smi.master.ShowEventPopup()));
      this.defrostExit.PlayAnim("defrost_exit").Update((Action<CryoTank.StatesInstance, float>) ((smi, dt) =>
      {
        if (smi.defrostAnimChore != null && !smi.defrostAnimChore.isComplete)
          return;
        smi.GoTo((StateMachine.BaseState) this.off);
      })).Exit((StateMachine<CryoTank.States, CryoTank.StatesInstance, CryoTank, object>.State.Callback) (smi =>
      {
        smi.sm.defrostedDuplicant.Get(smi).GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.Move);
        smi.master.Cheer();
      }));
      this.off.PlayAnim("off").Enter((StateMachine<CryoTank.States, CryoTank.StatesInstance, CryoTank, object>.State.Callback) (smi =>
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
