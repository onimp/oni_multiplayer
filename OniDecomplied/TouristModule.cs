// Decompiled with JetBrains decompiler
// Type: TouristModule
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class TouristModule : StateMachineComponent<TouristModule.StatesInstance>
{
  public Storage storage;
  [Serialize]
  private bool isSuspended;
  private bool releasingAstronaut;
  private const Sim.Cell.Properties floorCellProperties = Sim.Cell.Properties.GasImpermeable | Sim.Cell.Properties.LiquidImpermeable | Sim.Cell.Properties.SolidImpermeable | Sim.Cell.Properties.Opaque;
  public Assignable assignable;
  private HandleVector<int>.Handle partitionerEntry;
  private static readonly EventSystem.IntraObjectHandler<TouristModule> OnSuspendDelegate = new EventSystem.IntraObjectHandler<TouristModule>((Action<TouristModule, object>) ((component, data) => component.OnSuspend(data)));
  private static readonly EventSystem.IntraObjectHandler<TouristModule> OnAssigneeChangedDelegate = new EventSystem.IntraObjectHandler<TouristModule>((Action<TouristModule, object>) ((component, data) => component.OnAssigneeChanged(data)));

  public bool IsSuspended => this.isSuspended;

  protected virtual void OnPrefabInit() => base.OnPrefabInit();

  public void SetSuspended(bool state) => this.isSuspended = state;

  public void ReleaseAstronaut(object data, bool applyBuff = false)
  {
    if (this.releasingAstronaut)
      return;
    this.releasingAstronaut = true;
    MinionStorage component = ((Component) this).GetComponent<MinionStorage>();
    List<MinionStorage.Info> storedMinionInfo = component.GetStoredMinionInfo();
    for (int index = storedMinionInfo.Count - 1; index >= 0; --index)
    {
      MinionStorage.Info info = storedMinionInfo[index];
      GameObject go = component.DeserializeMinion(info.id, Grid.CellToPos(Grid.PosToCell(TransformExtensions.GetPosition(this.smi.master.transform))));
      if (Grid.FakeFloor[Grid.OffsetCell(Grid.PosToCell(((Component) this.smi.master).gameObject), 0, -1)])
      {
        go.GetComponent<Navigator>().SetCurrentNavType(NavType.Floor);
        if (applyBuff)
        {
          go.GetComponent<Effects>().Add(Db.Get().effects.Get("SpaceTourist"), true);
          go.GetSMI<JoyBehaviourMonitor.Instance>()?.GoToOverjoyed();
        }
      }
    }
    this.releasingAstronaut = false;
  }

  public void OnSuspend(object data)
  {
    Storage component = ((Component) this).GetComponent<Storage>();
    if (Object.op_Inequality((Object) component, (Object) null))
    {
      component.capacityKg = component.MassStored();
      component.allowItemRemoval = false;
    }
    if (Object.op_Inequality((Object) ((Component) this).GetComponent<ManualDeliveryKG>(), (Object) null))
      Object.Destroy((Object) ((Component) this).GetComponent<ManualDeliveryKG>());
    this.SetSuspended(true);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.storage = ((Component) this).GetComponent<Storage>();
    this.assignable = ((Component) this).GetComponent<Assignable>();
    this.smi.StartSM();
    this.partitionerEntry = GameScenePartitioner.Instance.Add("TouristModule.gantryChanged", (object) ((Component) this).gameObject, Grid.PosToCell(((Component) this).gameObject), GameScenePartitioner.Instance.validNavCellChangedLayer, new Action<object>(this.OnGantryChanged));
    this.OnGantryChanged((object) null);
    this.Subscribe<TouristModule>(-1277991738, TouristModule.OnSuspendDelegate);
    this.Subscribe<TouristModule>(684616645, TouristModule.OnAssigneeChangedDelegate);
  }

  private void OnGantryChanged(object data)
  {
    if (!Object.op_Inequality((Object) ((Component) this).gameObject, (Object) null))
      return;
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    component.RemoveStatusItem(Db.Get().BuildingStatusItems.HasGantry);
    component.RemoveStatusItem(Db.Get().BuildingStatusItems.MissingGantry);
    int i = Grid.OffsetCell(Grid.PosToCell(((Component) this.smi.master).gameObject), 0, -1);
    if (Grid.FakeFloor[i])
      component.AddStatusItem(Db.Get().BuildingStatusItems.HasGantry);
    else
      component.AddStatusItem(Db.Get().BuildingStatusItems.MissingGantry);
  }

  private Chore CreateWorkChore()
  {
    WorkChore<CommandModuleWorkable> workChore = new WorkChore<CommandModuleWorkable>(Db.Get().ChoreTypes.Astronaut, (IStateMachineTarget) this, allow_in_red_alert: false, override_anims: Assets.GetAnim(HashedString.op_Implicit("anim_hat_kanim")), allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.personalNeeds);
    workChore.AddPrecondition(ChorePreconditions.instance.IsAssignedtoMe, (object) this.assignable);
    return (Chore) workChore;
  }

  private void OnAssigneeChanged(object data)
  {
    if (data != null || !((Component) this).gameObject.HasTag(GameTags.RocketOnGround) || ((Component) this).GetComponent<MinionStorage>().GetStoredMinionInfo().Count <= 0)
      return;
    this.ReleaseAstronaut((object) null);
    Game.Instance.userMenu.Refresh(((Component) this).gameObject);
  }

  protected override void OnCleanUp()
  {
    GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
    this.partitionerEntry.Clear();
    this.ReleaseAstronaut((object) null);
    GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
    this.smi.StopSM("cleanup");
  }

  public class StatesInstance : 
    GameStateMachine<TouristModule.States, TouristModule.StatesInstance, TouristModule, object>.GameInstance
  {
    public StatesInstance(TouristModule smi)
      : base(smi)
    {
      KMonoBehaviourExtensions.Subscribe(((Component) smi).gameObject, -887025858, (Action<object>) (data =>
      {
        smi.SetSuspended(false);
        smi.ReleaseAstronaut((object) null, true);
        smi.assignable.Unassign();
      }));
    }
  }

  public class States : 
    GameStateMachine<TouristModule.States, TouristModule.StatesInstance, TouristModule>
  {
    public GameStateMachine<TouristModule.States, TouristModule.StatesInstance, TouristModule, object>.State idle;
    public GameStateMachine<TouristModule.States, TouristModule.StatesInstance, TouristModule, object>.State awaitingTourist;
    public GameStateMachine<TouristModule.States, TouristModule.StatesInstance, TouristModule, object>.State hasTourist;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.idle;
      this.idle.PlayAnim("grounded", (KAnim.PlayMode) 0).GoTo(this.awaitingTourist);
      this.awaitingTourist.PlayAnim("grounded", (KAnim.PlayMode) 0).ToggleChore((Func<TouristModule.StatesInstance, Chore>) (smi => smi.master.CreateWorkChore()), this.hasTourist);
      this.hasTourist.PlayAnim("grounded", (KAnim.PlayMode) 0).EventTransition(GameHashes.RocketLanded, this.idle).EventTransition(GameHashes.AssigneeChanged, this.idle);
    }
  }
}
