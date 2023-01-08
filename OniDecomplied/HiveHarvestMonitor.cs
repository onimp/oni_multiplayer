// Decompiled with JetBrains decompiler
// Type: HiveHarvestMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;

public class HiveHarvestMonitor : 
  GameStateMachine<HiveHarvestMonitor, HiveHarvestMonitor.Instance, IStateMachineTarget, HiveHarvestMonitor.Def>
{
  public StateMachine<HiveHarvestMonitor, HiveHarvestMonitor.Instance, IStateMachineTarget, HiveHarvestMonitor.Def>.BoolParameter shouldHarvest;
  public GameStateMachine<HiveHarvestMonitor, HiveHarvestMonitor.Instance, IStateMachineTarget, HiveHarvestMonitor.Def>.State do_not_harvest;
  public HiveHarvestMonitor.HarvestStates harvest;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.do_not_harvest;
    this.serializable = StateMachine.SerializeType.ParamsOnly;
    this.root.EventHandler(GameHashes.RefreshUserMenu, (StateMachine<HiveHarvestMonitor, HiveHarvestMonitor.Instance, IStateMachineTarget, HiveHarvestMonitor.Def>.State.Callback) (smi => smi.OnRefreshUserMenu()));
    this.do_not_harvest.ParamTransition<bool>((StateMachine<HiveHarvestMonitor, HiveHarvestMonitor.Instance, IStateMachineTarget, HiveHarvestMonitor.Def>.Parameter<bool>) this.shouldHarvest, (GameStateMachine<HiveHarvestMonitor, HiveHarvestMonitor.Instance, IStateMachineTarget, HiveHarvestMonitor.Def>.State) this.harvest, (StateMachine<HiveHarvestMonitor, HiveHarvestMonitor.Instance, IStateMachineTarget, HiveHarvestMonitor.Def>.Parameter<bool>.Callback) ((smi, bShouldHarvest) => bShouldHarvest));
    this.harvest.ParamTransition<bool>((StateMachine<HiveHarvestMonitor, HiveHarvestMonitor.Instance, IStateMachineTarget, HiveHarvestMonitor.Def>.Parameter<bool>) this.shouldHarvest, this.do_not_harvest, (StateMachine<HiveHarvestMonitor, HiveHarvestMonitor.Instance, IStateMachineTarget, HiveHarvestMonitor.Def>.Parameter<bool>.Callback) ((smi, bShouldHarvest) => !bShouldHarvest)).DefaultState(this.harvest.not_ready);
    this.harvest.not_ready.EventTransition(GameHashes.OnStorageChange, this.harvest.ready, (StateMachine<HiveHarvestMonitor, HiveHarvestMonitor.Instance, IStateMachineTarget, HiveHarvestMonitor.Def>.Transition.ConditionCallback) (smi => (double) smi.storage.GetMassAvailable(smi.def.producedOre) >= (double) smi.def.harvestThreshold));
    this.harvest.ready.ToggleChore((Func<HiveHarvestMonitor.Instance, Chore>) (smi => smi.CreateHarvestChore()), this.harvest.not_ready).EventTransition(GameHashes.OnStorageChange, this.harvest.not_ready, (StateMachine<HiveHarvestMonitor, HiveHarvestMonitor.Instance, IStateMachineTarget, HiveHarvestMonitor.Def>.Transition.ConditionCallback) (smi => (double) smi.storage.GetMassAvailable(smi.def.producedOre) < (double) smi.def.harvestThreshold));
  }

  public class Def : StateMachine.BaseDef
  {
    public Tag producedOre;
    public float harvestThreshold;
  }

  public class HarvestStates : 
    GameStateMachine<HiveHarvestMonitor, HiveHarvestMonitor.Instance, IStateMachineTarget, HiveHarvestMonitor.Def>.State
  {
    public GameStateMachine<HiveHarvestMonitor, HiveHarvestMonitor.Instance, IStateMachineTarget, HiveHarvestMonitor.Def>.State not_ready;
    public GameStateMachine<HiveHarvestMonitor, HiveHarvestMonitor.Instance, IStateMachineTarget, HiveHarvestMonitor.Def>.State ready;
  }

  public new class Instance : 
    GameStateMachine<HiveHarvestMonitor, HiveHarvestMonitor.Instance, IStateMachineTarget, HiveHarvestMonitor.Def>.GameInstance
  {
    [MyCmpReq]
    public Storage storage;

    public Instance(IStateMachineTarget master, HiveHarvestMonitor.Def def)
      : base(master, def)
    {
    }

    public void OnRefreshUserMenu()
    {
      if (this.sm.shouldHarvest.Get(this))
        Game.Instance.userMenu.AddButton(this.gameObject, new KIconButtonMenu.ButtonInfo("action_building_disabled", (string) UI.USERMENUACTIONS.CANCELEMPTYBEEHIVE.NAME, (System.Action) (() => this.sm.shouldHarvest.Set(false, this)), tooltipText: ((string) UI.USERMENUACTIONS.CANCELEMPTYBEEHIVE.TOOLTIP)));
      else
        Game.Instance.userMenu.AddButton(this.gameObject, new KIconButtonMenu.ButtonInfo("action_empty_contents", (string) UI.USERMENUACTIONS.EMPTYBEEHIVE.NAME, (System.Action) (() => this.sm.shouldHarvest.Set(true, this)), tooltipText: ((string) UI.USERMENUACTIONS.EMPTYBEEHIVE.TOOLTIP)));
    }

    public Chore CreateHarvestChore() => (Chore) new WorkChore<HiveWorkableEmpty>(Db.Get().ChoreTypes.Ranch, (IStateMachineTarget) this.master.GetComponent<HiveWorkableEmpty>(), on_complete: new System.Action<Chore>(this.smi.OnEmptyComplete));

    public void OnEmptyComplete(Chore chore) => this.smi.storage.Drop(this.smi.def.producedOre);
  }
}
