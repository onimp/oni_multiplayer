// Decompiled with JetBrains decompiler
// Type: NuclearResearchCenter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class NuclearResearchCenter : 
  StateMachineComponent<NuclearResearchCenter.StatesInstance>,
  IResearchCenter,
  IGameObjectEffectDescriptor
{
  [MyCmpGet]
  private Operational operational;
  public string researchTypeID;
  public float materialPerPoint = 50f;
  public float timePerPoint;
  public Tag inputMaterial;
  [MyCmpReq]
  private HighEnergyParticleStorage particleStorage;
  public Meter.Offset particleMeterOffset;
  private MeterController particleMeter;
  private static readonly EventSystem.IntraObjectHandler<NuclearResearchCenter> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<NuclearResearchCenter>((Action<NuclearResearchCenter, object>) ((component, data) => component.OnStorageChange(data)));

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    Components.ResearchCenters.Add((IResearchCenter) this);
    this.particleMeter = new MeterController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "meter_target", "meter", this.particleMeterOffset, Grid.SceneLayer.NoLayer, new string[1]
    {
      "meter_target"
    });
    this.Subscribe<NuclearResearchCenter>(-1837862626, NuclearResearchCenter.OnStorageChangeDelegate);
    this.RefreshMeter();
    this.smi.StartSM();
    Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Radiation);
  }

  protected override void OnCleanUp()
  {
    base.OnCleanUp();
    Components.ResearchCenters.Remove((IResearchCenter) this);
  }

  public string GetResearchType() => this.researchTypeID;

  private void OnStorageChange(object data) => this.RefreshMeter();

  private void RefreshMeter() => this.particleMeter.SetPositionPercent(Mathf.Clamp01(this.particleStorage.Particles / this.particleStorage.Capacity()));

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    descriptors.Add(new Descriptor(string.Format((string) UI.BUILDINGEFFECTS.RESEARCH_MATERIALS, (object) this.inputMaterial.ProperName(), (object) GameUtil.GetFormattedByTag(this.inputMaterial, this.materialPerPoint)), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.RESEARCH_MATERIALS, (object) this.inputMaterial.ProperName(), (object) GameUtil.GetFormattedByTag(this.inputMaterial, this.materialPerPoint)), (Descriptor.DescriptorType) 0, false));
    descriptors.Add(new Descriptor(string.Format((string) UI.BUILDINGEFFECTS.PRODUCES_RESEARCH_POINTS, (object) Research.Instance.researchTypes.GetResearchType(this.researchTypeID).name), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.PRODUCES_RESEARCH_POINTS, (object) Research.Instance.researchTypes.GetResearchType(this.researchTypeID).name), (Descriptor.DescriptorType) 1, false));
    return descriptors;
  }

  public class States : 
    GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter>
  {
    public GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.State inoperational;
    public NuclearResearchCenter.States.RequirementsState requirements;
    public NuclearResearchCenter.States.ReadyState ready;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.inoperational;
      this.inoperational.PlayAnim("off").TagTransition(GameTags.Operational, (GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.State) this.requirements);
      this.requirements.PlayAnim("on").TagTransition(GameTags.Operational, this.inoperational, true).DefaultState(this.requirements.highEnergyParticlesNeeded);
      this.requirements.highEnergyParticlesNeeded.ToggleMainStatusItem(Db.Get().BuildingStatusItems.WaitingForHighEnergyParticles).EventTransition(GameHashes.OnParticleStorageChanged, this.requirements.noResearchSelected, new StateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.Transition.ConditionCallback(this.IsReady));
      this.requirements.noResearchSelected.Enter((StateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.State.Callback) (smi => this.UpdateNoResearchSelectedStatusItem(smi, true))).Exit((StateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.State.Callback) (smi => this.UpdateNoResearchSelectedStatusItem(smi, false))).EventTransition(GameHashes.ActiveResearchChanged, this.requirements.noApplicableResearch, new StateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.Transition.ConditionCallback(this.IsResearchSelected));
      this.requirements.noApplicableResearch.EventTransition(GameHashes.ActiveResearchChanged, (GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.State) this.ready, new StateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.Transition.ConditionCallback(this.IsResearchApplicable)).EventTransition(GameHashes.ActiveResearchChanged, (GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.State) this.requirements, GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.Not(new StateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.Transition.ConditionCallback(this.IsResearchSelected)));
      this.ready.Enter((StateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.State.Callback) (smi => smi.CreateChore())).TagTransition(GameTags.Operational, this.inoperational, true).DefaultState(this.ready.idle).Exit((StateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.State.Callback) (smi => smi.DestroyChore())).EventTransition(GameHashes.ActiveResearchChanged, this.requirements.noResearchSelected, GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.Not(new StateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.Transition.ConditionCallback(this.IsResearchSelected))).EventTransition(GameHashes.ActiveResearchChanged, this.requirements.noApplicableResearch, GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.Not(new StateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.Transition.ConditionCallback(this.IsResearchApplicable))).EventTransition(GameHashes.ResearchPointsChanged, this.requirements.noApplicableResearch, GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.Not(new StateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.Transition.ConditionCallback(this.IsResearchApplicable))).EventTransition(GameHashes.OnParticleStorageEmpty, this.requirements.highEnergyParticlesNeeded, GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.Not(new StateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.Transition.ConditionCallback(this.HasRadiation)));
      this.ready.idle.WorkableStartTransition((Func<NuclearResearchCenter.StatesInstance, Workable>) (smi => (Workable) ((Component) smi.master).GetComponent<NuclearResearchCenterWorkable>()), this.ready.working);
      this.ready.working.Enter("SetActive(true)", (StateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.State.Callback) (smi => smi.master.operational.SetActive(true))).Exit("SetActive(false)", (StateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.State.Callback) (smi => smi.master.operational.SetActive(false))).WorkableStopTransition((Func<NuclearResearchCenter.StatesInstance, Workable>) (smi => (Workable) ((Component) smi.master).GetComponent<NuclearResearchCenterWorkable>()), this.ready.idle).WorkableCompleteTransition((Func<NuclearResearchCenter.StatesInstance, Workable>) (smi => (Workable) ((Component) smi.master).GetComponent<NuclearResearchCenterWorkable>()), this.ready.idle);
    }

    protected bool IsAllResearchComplete()
    {
      foreach (Tech resource in Db.Get().Techs.resources)
      {
        if (!resource.IsComplete())
          return false;
      }
      return true;
    }

    private void UpdateNoResearchSelectedStatusItem(
      NuclearResearchCenter.StatesInstance smi,
      bool entering)
    {
      int num = !entering || this.IsResearchSelected(smi) ? 0 : (!this.IsAllResearchComplete() ? 1 : 0);
      KSelectable component = smi.GetComponent<KSelectable>();
      if (num != 0)
        component.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.NoResearchSelected);
      else
        component.RemoveStatusItem(Db.Get().BuildingStatusItems.NoResearchSelected);
    }

    private bool IsReady(NuclearResearchCenter.StatesInstance smi) => (double) smi.GetComponent<HighEnergyParticleStorage>().Particles > (double) smi.master.materialPerPoint;

    private bool IsResearchSelected(NuclearResearchCenter.StatesInstance smi) => Research.Instance.GetActiveResearch() != null;

    private bool IsResearchApplicable(NuclearResearchCenter.StatesInstance smi)
    {
      TechInstance activeResearch = Research.Instance.GetActiveResearch();
      return activeResearch != null && activeResearch.tech.costsByResearchTypeID.ContainsKey(smi.master.researchTypeID) && (double) activeResearch.progressInventory.PointsByTypeID[smi.master.researchTypeID] < (double) activeResearch.tech.costsByResearchTypeID[smi.master.researchTypeID];
    }

    private bool HasRadiation(NuclearResearchCenter.StatesInstance smi) => !smi.GetComponent<HighEnergyParticleStorage>().IsEmpty();

    public class RequirementsState : 
      GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.State
    {
      public GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.State highEnergyParticlesNeeded;
      public GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.State noResearchSelected;
      public GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.State noApplicableResearch;
    }

    public class ReadyState : 
      GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.State
    {
      public GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.State idle;
      public GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.State working;
    }
  }

  public class StatesInstance : 
    GameStateMachine<NuclearResearchCenter.States, NuclearResearchCenter.StatesInstance, NuclearResearchCenter, object>.GameInstance
  {
    private WorkChore<NuclearResearchCenterWorkable> chore;

    public StatesInstance(NuclearResearchCenter master)
      : base(master)
    {
    }

    public void CreateChore()
    {
      Workable component = (Workable) ((Component) this.smi.master).GetComponent<NuclearResearchCenterWorkable>();
      this.chore = new WorkChore<NuclearResearchCenterWorkable>(Db.Get().ChoreTypes.Research, (IStateMachineTarget) component, is_preemptable: true);
      this.chore.preemption_cb = new Func<Chore.Precondition.Context, bool>(NuclearResearchCenter.StatesInstance.CanPreemptCB);
    }

    public void DestroyChore()
    {
      this.chore.Cancel("destroy me!");
      this.chore = (WorkChore<NuclearResearchCenterWorkable>) null;
    }

    private static bool CanPreemptCB(Chore.Precondition.Context context)
    {
      Worker component = ((Component) context.chore.driver).GetComponent<Worker>();
      float num1 = Db.Get().AttributeConverters.ResearchSpeed.Lookup((Component) component).Evaluate();
      Worker worker = context.consumerState.worker;
      float num2 = Db.Get().AttributeConverters.ResearchSpeed.Lookup((Component) worker).Evaluate();
      TechInstance activeResearch = Research.Instance.GetActiveResearch();
      if (activeResearch != null)
      {
        NuclearResearchCenter.StatesInstance smi = context.chore.gameObject.GetSMI<NuclearResearchCenter.StatesInstance>();
        if (smi != null && (double) num2 > (double) num1)
          return (double) activeResearch.PercentageCompleteResearchType(smi.master.researchTypeID) < 1.0;
      }
      return false;
    }
  }
}
