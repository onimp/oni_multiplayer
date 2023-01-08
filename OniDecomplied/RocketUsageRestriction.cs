// Decompiled with JetBrains decompiler
// Type: RocketUsageRestriction
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using UnityEngine;

public class RocketUsageRestriction : 
  GameStateMachine<RocketUsageRestriction, RocketUsageRestriction.StatesInstance, IStateMachineTarget, RocketUsageRestriction.Def>
{
  public static readonly Operational.Flag rocketUsageAllowed = new Operational.Flag(nameof (rocketUsageAllowed), Operational.Flag.Type.Requirement);
  private StateMachine<RocketUsageRestriction, RocketUsageRestriction.StatesInstance, IStateMachineTarget, RocketUsageRestriction.Def>.TargetParameter rocketControlStation;
  public RocketUsageRestriction.RestrictionStates restriction;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.root;
    this.serializable = StateMachine.SerializeType.ParamsOnly;
    this.root.Enter((StateMachine<RocketUsageRestriction, RocketUsageRestriction.StatesInstance, IStateMachineTarget, RocketUsageRestriction.Def>.State.Callback) (smi =>
    {
      if (DlcManager.FeatureClusterSpaceEnabled() && smi.master.gameObject.GetMyWorld().IsModuleInterior)
      {
        smi.Subscribe(493375141, new System.Action<object>(smi.OnRefreshUserMenu));
        smi.GoToRestrictionState();
      }
      else
        smi.StopSM("Not inside rocket or no cluster space");
    }));
    this.restriction.Enter(new StateMachine<RocketUsageRestriction, RocketUsageRestriction.StatesInstance, IStateMachineTarget, RocketUsageRestriction.Def>.State.Callback(this.AquireRocketControlStation)).Enter((StateMachine<RocketUsageRestriction, RocketUsageRestriction.StatesInstance, IStateMachineTarget, RocketUsageRestriction.Def>.State.Callback) (smi => Components.RocketControlStations.OnAdd += new System.Action<RocketControlStation>(smi.ControlStationBuilt))).Exit((StateMachine<RocketUsageRestriction, RocketUsageRestriction.StatesInstance, IStateMachineTarget, RocketUsageRestriction.Def>.State.Callback) (smi => Components.RocketControlStations.OnAdd -= new System.Action<RocketControlStation>(smi.ControlStationBuilt)));
    this.restriction.uncontrolled.ToggleStatusItem(Db.Get().BuildingStatusItems.NoRocketRestriction).Enter((StateMachine<RocketUsageRestriction, RocketUsageRestriction.StatesInstance, IStateMachineTarget, RocketUsageRestriction.Def>.State.Callback) (smi => this.RestrictUsage(smi, false)));
    this.restriction.controlled.DefaultState(this.restriction.controlled.nostation);
    this.restriction.controlled.nostation.Enter(new StateMachine<RocketUsageRestriction, RocketUsageRestriction.StatesInstance, IStateMachineTarget, RocketUsageRestriction.Def>.State.Callback(this.OnRocketRestrictionChanged)).ParamTransition<GameObject>((StateMachine<RocketUsageRestriction, RocketUsageRestriction.StatesInstance, IStateMachineTarget, RocketUsageRestriction.Def>.Parameter<GameObject>) this.rocketControlStation, this.restriction.controlled.controlled, GameStateMachine<RocketUsageRestriction, RocketUsageRestriction.StatesInstance, IStateMachineTarget, RocketUsageRestriction.Def>.IsNotNull);
    this.restriction.controlled.controlled.OnTargetLost(this.rocketControlStation, this.restriction.controlled.nostation).Enter(new StateMachine<RocketUsageRestriction, RocketUsageRestriction.StatesInstance, IStateMachineTarget, RocketUsageRestriction.Def>.State.Callback(this.OnRocketRestrictionChanged)).Target(this.rocketControlStation).EventHandler(GameHashes.RocketRestrictionChanged, new StateMachine<RocketUsageRestriction, RocketUsageRestriction.StatesInstance, IStateMachineTarget, RocketUsageRestriction.Def>.State.Callback(this.OnRocketRestrictionChanged)).Target(this.masterTarget);
  }

  private void OnRocketRestrictionChanged(RocketUsageRestriction.StatesInstance smi) => this.RestrictUsage(smi, smi.BuildingRestrictionsActive());

  private void RestrictUsage(RocketUsageRestriction.StatesInstance smi, bool restrict)
  {
    smi.master.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.RocketRestrictionInactive, !restrict && smi.isControlled);
    if (smi.isRestrictionApplied == restrict)
      return;
    smi.isRestrictionApplied = restrict;
    smi.operational.SetFlag(RocketUsageRestriction.rocketUsageAllowed, !smi.def.restrictOperational || !restrict);
    smi.master.GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.RocketRestrictionActive, restrict);
    Storage[] components = smi.master.gameObject.GetComponents<Storage>();
    if (components != null && components.Length != 0)
    {
      for (int index = 0; index < components.Length; ++index)
      {
        if (restrict)
        {
          smi.previousStorageAllowItemRemovalStates = new bool[components.Length];
          smi.previousStorageAllowItemRemovalStates[index] = components[index].allowItemRemoval;
          components[index].allowItemRemoval = false;
        }
        else if (smi.previousStorageAllowItemRemovalStates != null && index < smi.previousStorageAllowItemRemovalStates.Length)
          components[index].allowItemRemoval = smi.previousStorageAllowItemRemovalStates[index];
        foreach (GameObject gameObject in components[index].items)
          EventExtensions.Trigger(gameObject, -778359855, (object) components[index]);
      }
    }
    Ownable component = smi.master.GetComponent<Ownable>();
    if (!restrict || !Object.op_Inequality((Object) component, (Object) null) || !component.IsAssigned())
      return;
    component.Unassign();
  }

  private void AquireRocketControlStation(RocketUsageRestriction.StatesInstance smi)
  {
    if (!this.rocketControlStation.IsNull(smi))
      return;
    foreach (RocketControlStation rocketControlStation in Components.RocketControlStations)
    {
      if (rocketControlStation.GetMyWorldId() == smi.GetMyWorldId())
        this.rocketControlStation.Set((KMonoBehaviour) rocketControlStation, smi);
    }
  }

  public class Def : StateMachine.BaseDef
  {
    public bool initialControlledStateWhenBuilt = true;
    public bool restrictOperational = true;

    public override void Configure(GameObject prefab) => RocketControlStation.CONTROLLED_BUILDINGS.Add(prefab.PrefabID());
  }

  public class ControlledStates : 
    GameStateMachine<RocketUsageRestriction, RocketUsageRestriction.StatesInstance, IStateMachineTarget, RocketUsageRestriction.Def>.State
  {
    public GameStateMachine<RocketUsageRestriction, RocketUsageRestriction.StatesInstance, IStateMachineTarget, RocketUsageRestriction.Def>.State nostation;
    public GameStateMachine<RocketUsageRestriction, RocketUsageRestriction.StatesInstance, IStateMachineTarget, RocketUsageRestriction.Def>.State controlled;
  }

  public class RestrictionStates : 
    GameStateMachine<RocketUsageRestriction, RocketUsageRestriction.StatesInstance, IStateMachineTarget, RocketUsageRestriction.Def>.State
  {
    public GameStateMachine<RocketUsageRestriction, RocketUsageRestriction.StatesInstance, IStateMachineTarget, RocketUsageRestriction.Def>.State uncontrolled;
    public RocketUsageRestriction.ControlledStates controlled;
  }

  public class StatesInstance : 
    GameStateMachine<RocketUsageRestriction, RocketUsageRestriction.StatesInstance, IStateMachineTarget, RocketUsageRestriction.Def>.GameInstance
  {
    [MyCmpGet]
    public Operational operational;
    public bool[] previousStorageAllowItemRemovalStates;
    [Serialize]
    public bool isControlled = true;
    public bool isRestrictionApplied;

    public StatesInstance(IStateMachineTarget master, RocketUsageRestriction.Def def)
      : base(master, def)
    {
      this.isControlled = def.initialControlledStateWhenBuilt;
    }

    public void OnRefreshUserMenu(object data) => Game.Instance.userMenu.AddButton(this.gameObject, !this.isControlled ? new KIconButtonMenu.ButtonInfo("action_rocket_restriction_controlled", (string) UI.USERMENUACTIONS.ROCKETUSAGERESTRICTION.NAME_CONTROLLED, new System.Action(this.OnChange), tooltipText: ((string) UI.USERMENUACTIONS.ROCKETUSAGERESTRICTION.TOOLTIP_CONTROLLED)) : new KIconButtonMenu.ButtonInfo("action_rocket_restriction_uncontrolled", (string) UI.USERMENUACTIONS.ROCKETUSAGERESTRICTION.NAME_UNCONTROLLED, new System.Action(this.OnChange), tooltipText: ((string) UI.USERMENUACTIONS.ROCKETUSAGERESTRICTION.TOOLTIP_UNCONTROLLED)), 11f);

    public void ControlStationBuilt(object o) => this.sm.AquireRocketControlStation(this.smi);

    private void OnChange()
    {
      this.isControlled = !this.isControlled;
      this.GoToRestrictionState();
    }

    public void GoToRestrictionState()
    {
      if (this.smi.isControlled)
        this.smi.GoTo((StateMachine.BaseState) this.sm.restriction.controlled);
      else
        this.smi.GoTo((StateMachine.BaseState) this.sm.restriction.uncontrolled);
    }

    public bool BuildingRestrictionsActive() => this.isControlled && !this.sm.rocketControlStation.IsNull(this.smi) && this.sm.rocketControlStation.Get<RocketControlStation>(this.smi).BuildingRestrictionsActive;
  }
}
