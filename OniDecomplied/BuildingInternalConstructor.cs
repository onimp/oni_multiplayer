// Decompiled with JetBrains decompiler
// Type: BuildingInternalConstructor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInternalConstructor : 
  GameStateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>
{
  public GameStateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.State inoperational;
  public BuildingInternalConstructor.OperationalStates operational;
  public StateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.BoolParameter constructionRequested = new StateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.BoolParameter(true);

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.inoperational;
    this.serializable = StateMachine.SerializeType.ParamsOnly;
    this.inoperational.EventTransition(GameHashes.OperationalChanged, (GameStateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.State) this.operational, (StateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.Transition.ConditionCallback) (smi => smi.GetComponent<Operational>().IsOperational)).Enter((StateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.State.Callback) (smi => smi.ShowConstructionSymbol(false)));
    this.operational.DefaultState(this.operational.constructionRequired).EventTransition(GameHashes.OperationalChanged, this.inoperational, (StateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.Transition.ConditionCallback) (smi => !smi.GetComponent<Operational>().IsOperational));
    this.operational.constructionRequired.EventTransition(GameHashes.OnStorageChange, this.operational.constructionHappening, (StateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.Transition.ConditionCallback) (smi => Object.op_Inequality((Object) smi.GetMassForConstruction(), (Object) null))).EventTransition(GameHashes.OnStorageChange, this.operational.constructionSatisfied, (StateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.Transition.ConditionCallback) (smi => smi.HasOutputInStorage())).ToggleFetch((Func<BuildingInternalConstructor.Instance, FetchList2>) (smi => smi.CreateFetchList()), this.operational.constructionHappening).ParamTransition<bool>((StateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.Parameter<bool>) this.constructionRequested, this.operational.constructionSatisfied, GameStateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.IsFalse).Enter((StateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.State.Callback) (smi => smi.ShowConstructionSymbol(true))).Exit((StateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.State.Callback) (smi => smi.ShowConstructionSymbol(false)));
    this.operational.constructionHappening.EventTransition(GameHashes.OnStorageChange, this.operational.constructionSatisfied, (StateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.Transition.ConditionCallback) (smi => smi.HasOutputInStorage())).EventTransition(GameHashes.OnStorageChange, this.operational.constructionRequired, (StateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.Transition.ConditionCallback) (smi => Object.op_Equality((Object) smi.GetMassForConstruction(), (Object) null))).ToggleChore((Func<BuildingInternalConstructor.Instance, Chore>) (smi => (Chore) smi.CreateWorkChore()), this.operational.constructionHappening, this.operational.constructionHappening).ParamTransition<bool>((StateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.Parameter<bool>) this.constructionRequested, this.operational.constructionSatisfied, GameStateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.IsFalse).Enter((StateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.State.Callback) (smi => smi.ShowConstructionSymbol(true))).Exit((StateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.State.Callback) (smi => smi.ShowConstructionSymbol(false)));
    this.operational.constructionSatisfied.EventTransition(GameHashes.OnStorageChange, this.operational.constructionRequired, (StateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.Transition.ConditionCallback) (smi => !smi.HasOutputInStorage() && this.constructionRequested.Get(smi))).ParamTransition<bool>((StateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.Parameter<bool>) this.constructionRequested, this.operational.constructionRequired, (StateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.Parameter<bool>.Callback) ((smi, p) => p && !smi.HasOutputInStorage()));
  }

  public class Def : StateMachine.BaseDef
  {
    public DefComponent<Storage> storage;
    public float constructionMass;
    public List<string> outputIDs;
    public bool spawnIntoStorage;
    public string constructionSymbol;
  }

  public class OperationalStates : 
    GameStateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.State
  {
    public GameStateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.State constructionRequired;
    public GameStateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.State constructionHappening;
    public GameStateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.State constructionSatisfied;
  }

  public new class Instance : 
    GameStateMachine<BuildingInternalConstructor, BuildingInternalConstructor.Instance, IStateMachineTarget, BuildingInternalConstructor.Def>.GameInstance,
    ISidescreenButtonControl
  {
    private Storage storage;
    [Serialize]
    private float constructionElapsed;
    private ProgressBar progressBar;

    public Instance(IStateMachineTarget master, BuildingInternalConstructor.Def def)
      : base(master, def)
    {
      this.storage = def.storage.Get((StateMachine.Instance) this);
      this.GetComponent<RocketModule>().AddModuleCondition(ProcessCondition.ProcessConditionType.RocketPrep, (ProcessCondition) new InternalConstructionCompleteCondition(this));
    }

    protected override void OnCleanUp()
    {
      Element element = (Element) null;
      float num1 = 0.0f;
      float num2 = 0.0f;
      byte maxValue = byte.MaxValue;
      int disease_count = 0;
      foreach (string outputId in this.def.outputIDs)
      {
        GameObject first = this.storage.FindFirst(Tag.op_Implicit(outputId));
        if (Object.op_Inequality((Object) first, (Object) null))
        {
          PrimaryElement component = first.GetComponent<PrimaryElement>();
          Debug.Assert(element == null || element == component.Element);
          element = component.Element;
          num2 = GameUtil.GetFinalTemperature(num1, num2, component.Mass, component.Temperature);
          num1 += component.Mass;
          TracesExtesions.DeleteObject(first);
        }
      }
      element?.substance.SpawnResource(TransformExtensions.GetPosition(this.transform), num1, num2, maxValue, disease_count);
      base.OnCleanUp();
    }

    public FetchList2 CreateFetchList()
    {
      FetchList2 fetchList = new FetchList2(this.storage, Db.Get().ChoreTypes.Fetch);
      fetchList.Add(this.GetComponent<PrimaryElement>().Element.tag, amount: this.def.constructionMass);
      return fetchList;
    }

    public PrimaryElement GetMassForConstruction() => this.storage.FindFirstWithMass(this.GetComponent<PrimaryElement>().Element.tag, this.def.constructionMass);

    public bool HasOutputInStorage() => Object.op_Implicit((Object) this.storage.FindFirst(TagExtensions.ToTag(this.def.outputIDs[0])));

    public bool IsRequestingConstruction()
    {
      this.sm.constructionRequested.Get(this);
      return this.smi.sm.constructionRequested.Get(this.smi);
    }

    public void ConstructionComplete(bool force = false)
    {
      SimHashes element_id;
      if (!force)
      {
        PrimaryElement massForConstruction = this.GetMassForConstruction();
        element_id = massForConstruction.ElementID;
        float mass = massForConstruction.Mass;
        double num1 = (double) massForConstruction.Temperature * (double) massForConstruction.Mass;
        massForConstruction.Mass -= this.def.constructionMass;
        double num2 = (double) mass;
        double num3 = (double) Mathf.Clamp((float) (num1 / num2), 288.15f, 318.15f);
      }
      else
      {
        element_id = SimHashes.Cuprite;
        double temperature = (double) this.GetComponent<PrimaryElement>().Temperature;
      }
      foreach (string outputId in this.def.outputIDs)
      {
        GameObject go = GameUtil.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(outputId)), TransformExtensions.GetPosition(this.transform), Grid.SceneLayer.Ore);
        go.GetComponent<PrimaryElement>().SetElement(element_id, false);
        go.SetActive(true);
        if (this.def.spawnIntoStorage)
          this.storage.Store(go);
      }
    }

    public WorkChore<BuildingInternalConstructorWorkable> CreateWorkChore() => new WorkChore<BuildingInternalConstructorWorkable>(Db.Get().ChoreTypes.Build, this.master);

    public void ShowConstructionSymbol(bool show)
    {
      KBatchedAnimController component = this.master.GetComponent<KBatchedAnimController>();
      if (!Object.op_Inequality((Object) component, (Object) null))
        return;
      component.SetSymbolVisiblity(KAnimHashedString.op_Implicit(this.def.constructionSymbol), show);
    }

    public string SidescreenButtonText => !this.smi.sm.constructionRequested.Get(this.smi) ? string.Format(UI.UISIDESCREENS.BUTTONMENUSIDESCREEN.ALLOW_INTERNAL_CONSTRUCTOR.text, (object) Assets.GetPrefab(Tag.op_Implicit(this.def.outputIDs[0])).GetProperName()) : string.Format(UI.UISIDESCREENS.BUTTONMENUSIDESCREEN.DISALLOW_INTERNAL_CONSTRUCTOR.text, (object) Assets.GetPrefab(Tag.op_Implicit(this.def.outputIDs[0])).GetProperName());

    public string SidescreenButtonTooltip => !this.smi.sm.constructionRequested.Get(this.smi) ? string.Format(UI.UISIDESCREENS.BUTTONMENUSIDESCREEN.ALLOW_INTERNAL_CONSTRUCTOR_TOOLTIP.text, (object) Assets.GetPrefab(Tag.op_Implicit(this.def.outputIDs[0])).GetProperName()) : string.Format(UI.UISIDESCREENS.BUTTONMENUSIDESCREEN.DISALLOW_INTERNAL_CONSTRUCTOR_TOOLTIP.text, (object) Assets.GetPrefab(Tag.op_Implicit(this.def.outputIDs[0])).GetProperName());

    public void OnSidescreenButtonPressed()
    {
      this.smi.sm.constructionRequested.Set(!this.smi.sm.constructionRequested.Get(this.smi), this.smi);
      if (!DebugHandler.InstantBuildMode || !this.smi.sm.constructionRequested.Get(this.smi) || this.HasOutputInStorage())
        return;
      this.ConstructionComplete(true);
    }

    public void SetButtonTextOverride(ButtonMenuTextOverride text) => throw new NotImplementedException();

    public bool SidescreenEnabled() => true;

    public bool SidescreenButtonInteractable() => true;

    public int ButtonSideScreenSortOrder() => 20;
  }
}
