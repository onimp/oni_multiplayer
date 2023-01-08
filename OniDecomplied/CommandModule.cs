// Decompiled with JetBrains decompiler
// Type: CommandModule
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class CommandModule : StateMachineComponent<CommandModule.StatesInstance>
{
  public Storage storage;
  public RocketStats rocketStats;
  public RocketCommandConditions conditions;
  private bool releasingAstronaut;
  private const Sim.Cell.Properties floorCellProperties = Sim.Cell.Properties.GasImpermeable | Sim.Cell.Properties.LiquidImpermeable | Sim.Cell.Properties.SolidImpermeable | Sim.Cell.Properties.Opaque;
  public Assignable assignable;
  private HandleVector<int>.Handle partitionerEntry;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.rocketStats = new RocketStats(this);
    this.conditions = ((Component) this).GetComponent<RocketCommandConditions>();
  }

  public void ReleaseAstronaut(bool fill_bladder)
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
      if (!Object.op_Equality((Object) go, (Object) null))
      {
        if (Grid.FakeFloor[Grid.OffsetCell(Grid.PosToCell(((Component) this.smi.master).gameObject), 0, -1)])
          go.GetComponent<Navigator>().SetCurrentNavType(NavType.Floor);
        if (fill_bladder)
        {
          AmountInstance amountInstance = Db.Get().Amounts.Bladder.Lookup(go);
          if (amountInstance != null)
            amountInstance.value = amountInstance.GetMax();
        }
      }
    }
    this.releasingAstronaut = false;
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.storage = ((Component) this).GetComponent<Storage>();
    this.assignable = ((Component) this).GetComponent<Assignable>();
    this.assignable.AddAssignPrecondition(new Func<MinionAssignablesProxy, bool>(this.CanAssignTo));
    this.smi.StartSM();
    this.partitionerEntry = GameScenePartitioner.Instance.Add("CommandModule.gantryChanged", (object) ((Component) this).gameObject, Grid.PosToCell(((Component) this).gameObject), GameScenePartitioner.Instance.validNavCellChangedLayer, new Action<object>(this.OnGantryChanged));
    this.OnGantryChanged((object) null);
  }

  private bool CanAssignTo(MinionAssignablesProxy worker)
  {
    if (worker.target is MinionIdentity)
      return ((Component) (worker.target as KMonoBehaviour)).GetComponent<MinionResume>().HasPerk(Db.Get().SkillPerks.CanUseRockets);
    return worker.target is StoredMinionIdentity && (worker.target as StoredMinionIdentity).HasPerk(Db.Get().SkillPerks.CanUseRockets);
  }

  private static bool HasValidGantry(GameObject go)
  {
    int num = Grid.OffsetCell(Grid.PosToCell(go), 0, -1);
    return Grid.IsValidCell(num) && Grid.FakeFloor[num];
  }

  private void OnGantryChanged(object data)
  {
    if (!Object.op_Inequality((Object) ((Component) this).gameObject, (Object) null))
      return;
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    component.RemoveStatusItem(Db.Get().BuildingStatusItems.HasGantry);
    component.RemoveStatusItem(Db.Get().BuildingStatusItems.MissingGantry);
    if (CommandModule.HasValidGantry(((Component) this.smi.master).gameObject))
      component.AddStatusItem(Db.Get().BuildingStatusItems.HasGantry);
    else
      component.AddStatusItem(Db.Get().BuildingStatusItems.MissingGantry);
    this.smi.sm.gantryChanged.Trigger(this.smi);
  }

  private Chore CreateWorkChore()
  {
    WorkChore<CommandModuleWorkable> workChore = new WorkChore<CommandModuleWorkable>(Db.Get().ChoreTypes.Astronaut, (IStateMachineTarget) this, allow_in_red_alert: false, override_anims: Assets.GetAnim(HashedString.op_Implicit("anim_hat_kanim")), allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.personalNeeds);
    workChore.AddPrecondition(ChorePreconditions.instance.HasSkillPerk, (object) Db.Get().SkillPerks.CanUseRockets);
    workChore.AddPrecondition(ChorePreconditions.instance.IsAssignedtoMe, (object) this.assignable);
    return (Chore) workChore;
  }

  protected override void OnCleanUp()
  {
    GameScenePartitioner.Instance.Free(ref this.partitionerEntry);
    this.partitionerEntry.Clear();
    this.ReleaseAstronaut(false);
    this.smi.StopSM("cleanup");
  }

  public class StatesInstance : 
    GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.GameInstance
  {
    public StatesInstance(CommandModule master)
      : base(master)
    {
    }

    public void SetSuspended(bool suspended)
    {
      Storage component1 = this.GetComponent<Storage>();
      if (Object.op_Inequality((Object) component1, (Object) null))
        component1.allowItemRemoval = !suspended;
      ManualDeliveryKG component2 = this.GetComponent<ManualDeliveryKG>();
      if (!Object.op_Inequality((Object) component2, (Object) null))
        return;
      component2.Pause(suspended, "Rocket is suspended");
    }

    public bool CheckStoredMinionIsAssignee()
    {
      foreach (MinionStorage.Info info in this.GetComponent<MinionStorage>().GetStoredMinionInfo())
      {
        if (info.serializedMinion != null)
        {
          KPrefabID kprefabId = info.serializedMinion.Get();
          if (!Object.op_Equality((Object) kprefabId, (Object) null))
          {
            StoredMinionIdentity component = ((Component) kprefabId).GetComponent<StoredMinionIdentity>();
            if (this.GetComponent<Assignable>().assignee == component.assignableProxy.Get())
              return true;
          }
        }
      }
      return false;
    }
  }

  public class States : 
    GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule>
  {
    public StateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.Signal gantryChanged;
    public StateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.BoolParameter accumulatedPee;
    public CommandModule.States.GroundedStates grounded;
    public CommandModule.States.SpaceborneStates spaceborne;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.grounded;
      this.grounded.PlayAnim("grounded", (KAnim.PlayMode) 0).DefaultState(this.grounded.awaitingAstronaut).TagTransition(GameTags.RocketNotOnGround, (GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State) this.spaceborne);
      this.grounded.awaitingAstronaut.Enter((StateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State.Callback) (smi =>
      {
        if (smi.CheckStoredMinionIsAssignee())
          smi.GoTo((StateMachine.BaseState) this.grounded.hasAstronaut);
        Game.Instance.userMenu.Refresh(smi.gameObject);
      })).EventHandler(GameHashes.AssigneeChanged, (StateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State.Callback) (smi =>
      {
        if (smi.CheckStoredMinionIsAssignee())
          smi.GoTo((StateMachine.BaseState) this.grounded.hasAstronaut);
        Game.Instance.userMenu.Refresh(smi.gameObject);
      })).ToggleChore((Func<CommandModule.StatesInstance, Chore>) (smi => smi.master.CreateWorkChore()), this.grounded.hasAstronaut);
      this.grounded.hasAstronaut.EventHandler(GameHashes.AssigneeChanged, (StateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State.Callback) (smi =>
      {
        if (smi.CheckStoredMinionIsAssignee())
          return;
        smi.GoTo((StateMachine.BaseState) this.grounded.waitingToRelease);
      }));
      this.grounded.waitingToRelease.ToggleStatusItem(Db.Get().BuildingStatusItems.DisembarkingDuplicant).OnSignal(this.gantryChanged, this.grounded.awaitingAstronaut, (Func<CommandModule.StatesInstance, bool>) (smi =>
      {
        if (!CommandModule.HasValidGantry(smi.gameObject))
          return false;
        smi.master.ReleaseAstronaut(this.accumulatedPee.Get(smi));
        this.accumulatedPee.Set(false, smi);
        Game.Instance.userMenu.Refresh(smi.gameObject);
        return true;
      }));
      this.spaceborne.DefaultState(this.spaceborne.launch);
      this.spaceborne.launch.Enter((StateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State.Callback) (smi => smi.SetSuspended(true))).GoTo(this.spaceborne.idle);
      this.spaceborne.idle.TagTransition(GameTags.RocketNotOnGround, this.spaceborne.land, true);
      this.spaceborne.land.Enter((StateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State.Callback) (smi =>
      {
        smi.SetSuspended(false);
        Game.Instance.userMenu.Refresh(smi.gameObject);
        this.accumulatedPee.Set(true, smi);
      })).GoTo(this.grounded.waitingToRelease);
    }

    public class GroundedStates : 
      GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State
    {
      public GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State awaitingAstronaut;
      public GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State hasAstronaut;
      public GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State waitingToRelease;
    }

    public class SpaceborneStates : 
      GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State
    {
      public GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State launch;
      public GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State idle;
      public GameStateMachine<CommandModule.States, CommandModule.StatesInstance, CommandModule, object>.State land;
    }
  }
}
