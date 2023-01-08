// Decompiled with JetBrains decompiler
// Type: EatChore
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class EatChore : Chore<EatChore.StatesInstance>
{
  public static readonly Chore.Precondition EdibleIsNotNull = new Chore.Precondition()
  {
    id = nameof (EdibleIsNotNull),
    description = (string) DUPLICANTS.CHORES.PRECONDITIONS.EDIBLE_IS_NOT_NULL,
    fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) => Object.op_Inequality((Object) null, (Object) ((Component) context.consumerState.consumer).GetSMI<RationMonitor.Instance>().GetEdible()))
  };

  public EatChore(IStateMachineTarget master)
    : base(Db.Get().ChoreTypes.Eat, master, master.GetComponent<ChoreProvider>(), false, master_priority_class: PriorityScreen.PriorityClass.personalNeeds, report_type: ReportManager.ReportType.PersonalTime)
  {
    this.smi = new EatChore.StatesInstance(this);
    this.showAvailabilityInHoverText = false;
    this.AddPrecondition(ChorePreconditions.instance.IsNotRedAlert);
    this.AddPrecondition(EatChore.EdibleIsNotNull);
  }

  public override void Begin(Chore.Precondition.Context context)
  {
    if (Object.op_Equality((Object) context.consumerState.consumer, (Object) null))
    {
      Debug.LogError((object) "EATCHORE null context.consumer");
    }
    else
    {
      RationMonitor.Instance smi = ((Component) context.consumerState.consumer).GetSMI<RationMonitor.Instance>();
      if (smi == null)
      {
        Debug.LogError((object) "EATCHORE null RationMonitor.Instance");
      }
      else
      {
        Edible edible = smi.GetEdible();
        if (Object.op_Equality((Object) ((Component) edible).gameObject, (Object) null))
          Debug.LogError((object) "EATCHORE null edible.gameObject");
        else if (this.smi == null)
          Debug.LogError((object) "EATCHORE null smi");
        else if (this.smi.sm == null)
          Debug.LogError((object) "EATCHORE null smi.sm");
        else if (this.smi.sm.ediblesource == null)
        {
          Debug.LogError((object) "EATCHORE null smi.sm.ediblesource");
        }
        else
        {
          this.smi.sm.ediblesource.Set(((Component) edible).gameObject, this.smi, false);
          KCrashReporter.Assert((double) edible.FoodInfo.CaloriesPerUnit > 0.0, ((Component) edible).GetProperName() + " has invalid calories per unit. Will result in NaNs");
          AmountInstance amountInstance = Db.Get().Amounts.Calories.Lookup(this.gameObject);
          float num1 = (amountInstance.GetMax() - amountInstance.value) / edible.FoodInfo.CaloriesPerUnit;
          KCrashReporter.Assert((double) num1 > 0.0, "EatChore is requesting an invalid amount of food");
          double num2 = (double) this.smi.sm.requestedfoodunits.Set(num1, this.smi);
          this.smi.sm.eater.Set(context.consumerState.gameObject, this.smi, false);
          base.Begin(context);
        }
      }
    }
  }

  public class StatesInstance : 
    GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.GameInstance
  {
    private int locatorCell;

    public StatesInstance(EatChore master)
      : base(master)
    {
    }

    public void UpdateMessStation()
    {
      Ownables soleOwner = this.sm.eater.Get(this.smi).GetComponent<MinionIdentity>().GetSoleOwner();
      List<Assignable> preferredAssignables = Game.Instance.assignmentManager.GetPreferredAssignables((Assignables) soleOwner, Db.Get().AssignableSlots.MessStation);
      if (preferredAssignables.Count == 0)
      {
        soleOwner.AutoAssignSlot(Db.Get().AssignableSlots.MessStation);
        preferredAssignables = Game.Instance.assignmentManager.GetPreferredAssignables((Assignables) soleOwner, Db.Get().AssignableSlots.MessStation);
      }
      this.smi.sm.messstation.Set(preferredAssignables.Count > 0 ? (KMonoBehaviour) preferredAssignables[0] : (KMonoBehaviour) null, this.smi);
    }

    public bool UseSalt()
    {
      if (this.smi.sm.messstation == null || !Object.op_Inequality((Object) this.smi.sm.messstation.Get(this.smi), (Object) null))
        return false;
      MessStation component = this.smi.sm.messstation.Get(this.smi).GetComponent<MessStation>();
      return Object.op_Inequality((Object) component, (Object) null) && component.HasSalt;
    }

    public void CreateLocator()
    {
      int num = this.sm.eater.Get<Sensors>(this.smi).GetSensor<SafeCellSensor>().GetCellQuery();
      if (num == Grid.InvalidCell)
        num = Grid.PosToCell(TransformExtensions.GetPosition(this.sm.eater.Get<Transform>(this.smi)));
      Vector3 posCbc = Grid.CellToPosCBC(num, Grid.SceneLayer.Move);
      Grid.Reserved[num] = true;
      this.sm.locator.Set(ChoreHelpers.CreateLocator("EatLocator", posCbc), this, false);
      this.locatorCell = num;
    }

    public void DestroyLocator()
    {
      Grid.Reserved[this.locatorCell] = false;
      ChoreHelpers.DestroyLocator(this.sm.locator.Get(this));
      this.sm.locator.Set((KMonoBehaviour) null, this);
    }

    public void SetZ(GameObject go, float z)
    {
      Vector3 position = TransformExtensions.GetPosition(go.transform);
      position.z = z;
      TransformExtensions.SetPosition(go.transform, position);
    }

    public void ApplyRoomEffects() => Game.Instance.roomProber.GetRoomOfGameObject(this.sm.messstation.Get(this.smi).gameObject)?.roomType.TriggerRoomEffects(this.sm.messstation.Get(this.smi).gameObject.GetComponent<KPrefabID>(), this.sm.eater.Get(this.smi).gameObject.GetComponent<Effects>());

    public void ApplySaltEffect()
    {
      Storage component = this.sm.messstation.Get(this.smi).gameObject.GetComponent<Storage>();
      if (!Object.op_Inequality((Object) component, (Object) null) || !component.Has(TagExtensions.ToTag(TableSaltConfig.ID)))
        return;
      component.ConsumeIgnoringDisease(TagExtensions.ToTag(TableSaltConfig.ID), TableSaltTuning.CONSUMABLE_RATE);
      ((Component) this.sm.eater.Get(this.smi).gameObject.GetComponent<Worker>()).GetComponent<Effects>().Add("MessTableSalt", true);
      EventExtensions.Trigger(this.sm.messstation.Get(this.smi).gameObject, 1356255274, (object) null);
    }
  }

  public class States : GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore>
  {
    public StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.TargetParameter eater;
    public StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.TargetParameter ediblesource;
    public StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.TargetParameter ediblechunk;
    public StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.TargetParameter messstation;
    public StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.FloatParameter requestedfoodunits;
    public StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.FloatParameter actualfoodunits;
    public StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.TargetParameter locator;
    public GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.FetchSubState fetch;
    public EatChore.States.EatOnFloorState eatonfloorstate;
    public EatChore.States.EatAtMessStationState eatatmessstation;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.fetch;
      this.Target(this.eater);
      this.root.Enter("SetMessStation", (StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.State.Callback) (smi => smi.UpdateMessStation())).EventHandler(GameHashes.AssignablesChanged, (StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.State.Callback) (smi => smi.UpdateMessStation()));
      this.fetch.InitializeStates(this.eater, this.ediblesource, this.ediblechunk, this.requestedfoodunits, this.actualfoodunits, (GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.State) this.eatatmessstation);
      this.eatatmessstation.DefaultState((GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.State) this.eatatmessstation.moveto).ParamTransition<GameObject>((StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.Parameter<GameObject>) this.messstation, (GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.State) this.eatonfloorstate, (StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.Parameter<GameObject>.Callback) ((smi, p) => Object.op_Equality((Object) p, (Object) null))).ParamTransition<GameObject>((StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.Parameter<GameObject>) this.messstation, (GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.State) this.eatonfloorstate, (StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.Parameter<GameObject>.Callback) ((smi, p) => Object.op_Inequality((Object) p, (Object) null) && !p.GetComponent<Operational>().IsOperational));
      this.eatatmessstation.moveto.InitializeStates(this.eater, this.messstation, this.eatatmessstation.eat, (GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.State) this.eatonfloorstate);
      this.eatatmessstation.eat.Enter("AnimOverride", (StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.State.Callback) (smi => smi.GetComponent<KAnimControllerBase>().AddAnimOverrides(Assets.GetAnim(HashedString.op_Implicit("anim_eat_table_kanim"))))).DoEat(this.ediblechunk, this.actualfoodunits, (GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.State) null, (GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.State) null).Enter((StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.State.Callback) (smi =>
      {
        smi.SetZ(this.eater.Get(smi), Grid.GetLayerZ(Grid.SceneLayer.BuildingFront));
        smi.ApplyRoomEffects();
        smi.ApplySaltEffect();
      })).Exit((StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.State.Callback) (smi =>
      {
        smi.SetZ(this.eater.Get(smi), Grid.GetLayerZ(Grid.SceneLayer.Move));
        smi.GetComponent<KAnimControllerBase>().RemoveAnimOverrides(Assets.GetAnim(HashedString.op_Implicit("anim_eat_table_kanim")));
      }));
      this.eatonfloorstate.DefaultState((GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.State) this.eatonfloorstate.moveto).Enter("CreateLocator", (StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.State.Callback) (smi => smi.CreateLocator())).Exit("DestroyLocator", (StateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.State.Callback) (smi => smi.DestroyLocator()));
      this.eatonfloorstate.moveto.InitializeStates(this.eater, this.locator, this.eatonfloorstate.eat, this.eatonfloorstate.eat);
      this.eatonfloorstate.eat.ToggleAnims("anim_eat_floor_kanim").DoEat(this.ediblechunk, this.actualfoodunits, (GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.State) null, (GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.State) null);
    }

    public class EatOnFloorState : 
      GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.State
    {
      public GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.ApproachSubState<IApproachable> moveto;
      public GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.State eat;
    }

    public class EatAtMessStationState : 
      GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.State
    {
      public GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.ApproachSubState<MessStation> moveto;
      public GameStateMachine<EatChore.States, EatChore.StatesInstance, EatChore, object>.State eat;
    }
  }
}
