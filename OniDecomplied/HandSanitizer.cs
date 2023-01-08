// Decompiled with JetBrains decompiler
// Type: HandSanitizer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HandSanitizer : 
  StateMachineComponent<HandSanitizer.SMInstance>,
  IGameObjectEffectDescriptor,
  IBasicBuilding
{
  public float massConsumedPerUse = 1f;
  public SimHashes consumedElement = SimHashes.BleachStone;
  public int diseaseRemovalCount = 10000;
  public int maxUses = 10;
  public SimHashes outputElement = SimHashes.Vacuum;
  public bool dumpWhenFull;
  public bool alwaysUse;
  public bool canSanitizeSuit;
  public bool canSanitizeStorage;
  private WorkableReactable reactable;
  private MeterController cleanMeter;
  private MeterController dirtyMeter;
  public Meter.Offset cleanMeterOffset;
  public Meter.Offset dirtyMeterOffset;
  [Serialize]
  public int maxPossiblyRemoved;
  private static readonly EventSystem.IntraObjectHandler<HandSanitizer> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<HandSanitizer>((Action<HandSanitizer, object>) ((component, data) => component.OnStorageChange(data)));

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    Util.FindOrAddComponent<Workable>(((Component) this).gameObject);
  }

  private void RefreshMeters()
  {
    float percent_full1 = 0.0f;
    PrimaryElement primaryElement1 = ((Component) this).GetComponent<Storage>().FindPrimaryElement(this.consumedElement);
    float num = (float) this.maxUses * this.massConsumedPerUse;
    ConduitConsumer component = ((Component) this).GetComponent<ConduitConsumer>();
    if (Object.op_Inequality((Object) component, (Object) null))
      num = component.capacityKG;
    if (Object.op_Inequality((Object) primaryElement1, (Object) null))
      percent_full1 = Mathf.Clamp01(primaryElement1.Mass / num);
    float percent_full2 = 0.0f;
    PrimaryElement primaryElement2 = ((Component) this).GetComponent<Storage>().FindPrimaryElement(this.outputElement);
    if (Object.op_Inequality((Object) primaryElement2, (Object) null))
      percent_full2 = Mathf.Clamp01(primaryElement2.Mass / ((float) this.maxUses * this.massConsumedPerUse));
    this.cleanMeter.SetPositionPercent(percent_full1);
    this.dirtyMeter.SetPositionPercent(percent_full2);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
    this.cleanMeter = new MeterController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "meter_clean_target", "meter_clean", this.cleanMeterOffset, Grid.SceneLayer.NoLayer, new string[1]
    {
      "meter_clean_target"
    });
    this.dirtyMeter = new MeterController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "meter_dirty_target", "meter_dirty", this.dirtyMeterOffset, Grid.SceneLayer.NoLayer, new string[1]
    {
      "meter_dirty_target"
    });
    this.RefreshMeters();
    Components.HandSanitizers.Add(this);
    Components.BasicBuildings.Add((IBasicBuilding) this);
    this.Subscribe<HandSanitizer>(-1697596308, HandSanitizer.OnStorageChangeDelegate);
    ((Component) this).GetComponent<DirectionControl>().onDirectionChanged += new Action<WorkableReactable.AllowedDirection>(this.OnDirectionChanged);
    this.OnDirectionChanged(((Component) this).GetComponent<DirectionControl>().allowedDirection);
  }

  protected override void OnCleanUp()
  {
    Components.BasicBuildings.Remove((IBasicBuilding) this);
    Components.HandSanitizers.Remove(this);
    base.OnCleanUp();
  }

  private void OnDirectionChanged(
    WorkableReactable.AllowedDirection allowed_direction)
  {
    if (this.reactable == null)
      return;
    this.reactable.allowedDirection = allowed_direction;
  }

  public List<Descriptor> RequirementDescriptors()
  {
    List<Descriptor> descriptorList = new List<Descriptor>();
    descriptorList.Add(new Descriptor(string.Format((string) UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, (object) ElementLoader.FindElementByHash(this.consumedElement).name, (object) GameUtil.GetFormattedMass(this.massConsumedPerUse)), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, (object) ElementLoader.FindElementByHash(this.consumedElement).name, (object) GameUtil.GetFormattedMass(this.massConsumedPerUse)), (Descriptor.DescriptorType) 0, false));
    return descriptorList;
  }

  public List<Descriptor> EffectDescriptors()
  {
    List<Descriptor> descriptorList = new List<Descriptor>();
    if (this.outputElement != SimHashes.Vacuum)
      descriptorList.Add(new Descriptor(string.Format((string) UI.BUILDINGEFFECTS.ELEMENTEMITTEDPERUSE, (object) ElementLoader.FindElementByHash(this.outputElement).name, (object) GameUtil.GetFormattedMass(this.massConsumedPerUse)), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTEDPERUSE, (object) ElementLoader.FindElementByHash(this.outputElement).name, (object) GameUtil.GetFormattedMass(this.massConsumedPerUse)), (Descriptor.DescriptorType) 1, false));
    descriptorList.Add(new Descriptor(string.Format((string) UI.BUILDINGEFFECTS.DISEASECONSUMEDPERUSE, (object) GameUtil.GetFormattedDiseaseAmount(this.diseaseRemovalCount)), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.DISEASECONSUMEDPERUSE, (object) GameUtil.GetFormattedDiseaseAmount(this.diseaseRemovalCount)), (Descriptor.DescriptorType) 1, false));
    return descriptorList;
  }

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    descriptors.AddRange((IEnumerable<Descriptor>) this.RequirementDescriptors());
    descriptors.AddRange((IEnumerable<Descriptor>) this.EffectDescriptors());
    return descriptors;
  }

  private void OnStorageChange(object data)
  {
    if (this.dumpWhenFull && this.smi.OutputFull())
      this.smi.DumpOutput();
    this.RefreshMeters();
  }

  private class WashHandsReactable : WorkableReactable
  {
    public WashHandsReactable(
      Workable workable,
      ChoreType chore_type,
      WorkableReactable.AllowedDirection allowed_direction = WorkableReactable.AllowedDirection.Any)
      : base(workable, HashedString.op_Implicit("WashHands"), chore_type, allowed_direction)
    {
    }

    public override bool InternalCanBegin(
      GameObject new_reactor,
      Navigator.ActiveTransition transition)
    {
      if (base.InternalCanBegin(new_reactor, transition))
      {
        HandSanitizer component1 = ((Component) this.workable).GetComponent<HandSanitizer>();
        if (!component1.smi.IsReady())
          return false;
        if (component1.alwaysUse)
          return true;
        PrimaryElement component2 = new_reactor.GetComponent<PrimaryElement>();
        if (Object.op_Inequality((Object) component2, (Object) null))
          return component2.DiseaseIdx != byte.MaxValue;
      }
      return false;
    }
  }

  public class SMInstance : 
    GameStateMachine<HandSanitizer.States, HandSanitizer.SMInstance, HandSanitizer, object>.GameInstance
  {
    public SMInstance(HandSanitizer master)
      : base(master)
    {
    }

    private bool HasSufficientMass()
    {
      bool flag = false;
      PrimaryElement primaryElement = this.GetComponent<Storage>().FindPrimaryElement(this.master.consumedElement);
      if (Object.op_Inequality((Object) primaryElement, (Object) null))
        flag = (double) primaryElement.Mass >= (double) this.master.massConsumedPerUse;
      return flag;
    }

    public bool OutputFull()
    {
      PrimaryElement primaryElement = this.GetComponent<Storage>().FindPrimaryElement(this.master.outputElement);
      return Object.op_Inequality((Object) primaryElement, (Object) null) && (double) primaryElement.Mass >= (double) this.master.maxUses * (double) this.master.massConsumedPerUse;
    }

    public bool IsReady() => this.HasSufficientMass() && !this.OutputFull();

    public void DumpOutput()
    {
      Storage component = ((Component) this.master).GetComponent<Storage>();
      if (this.master.outputElement == SimHashes.Vacuum)
        return;
      component.Drop(ElementLoader.FindElementByHash(this.master.outputElement).tag);
    }
  }

  public class States : 
    GameStateMachine<HandSanitizer.States, HandSanitizer.SMInstance, HandSanitizer>
  {
    public GameStateMachine<HandSanitizer.States, HandSanitizer.SMInstance, HandSanitizer, object>.State notready;
    public HandSanitizer.States.ReadyStates ready;
    public GameStateMachine<HandSanitizer.States, HandSanitizer.SMInstance, HandSanitizer, object>.State notoperational;
    public GameStateMachine<HandSanitizer.States, HandSanitizer.SMInstance, HandSanitizer, object>.State full;
    public GameStateMachine<HandSanitizer.States, HandSanitizer.SMInstance, HandSanitizer, object>.State empty;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.notready;
      this.root.Update(new Action<HandSanitizer.SMInstance, float>(this.UpdateStatusItems));
      this.notoperational.PlayAnim("off").TagTransition(GameTags.Operational, this.notready);
      this.notready.PlayAnim("off").EventTransition(GameHashes.OnStorageChange, (GameStateMachine<HandSanitizer.States, HandSanitizer.SMInstance, HandSanitizer, object>.State) this.ready, (StateMachine<HandSanitizer.States, HandSanitizer.SMInstance, HandSanitizer, object>.Transition.ConditionCallback) (smi => smi.IsReady())).TagTransition(GameTags.Operational, this.notoperational, true);
      this.ready.DefaultState(this.ready.free).ToggleReactable((Func<HandSanitizer.SMInstance, Reactable>) (smi => (Reactable) (smi.master.reactable = (WorkableReactable) new HandSanitizer.WashHandsReactable((Workable) ((Component) smi.master).GetComponent<HandSanitizer.Work>(), Db.Get().ChoreTypes.WashHands, ((Component) smi.master).GetComponent<DirectionControl>().allowedDirection)))).TagTransition(GameTags.Operational, this.notoperational, true);
      this.ready.free.PlayAnim("on").WorkableStartTransition((Func<HandSanitizer.SMInstance, Workable>) (smi => (Workable) smi.GetComponent<HandSanitizer.Work>()), this.ready.occupied);
      this.ready.occupied.PlayAnim("working_pre").QueueAnim("working_loop", true).Enter((StateMachine<HandSanitizer.States, HandSanitizer.SMInstance, HandSanitizer, object>.State.Callback) (smi =>
      {
        ConduitConsumer component = smi.GetComponent<ConduitConsumer>();
        if (!Object.op_Inequality((Object) component, (Object) null))
          return;
        ((Behaviour) component).enabled = false;
      })).Exit((StateMachine<HandSanitizer.States, HandSanitizer.SMInstance, HandSanitizer, object>.State.Callback) (smi =>
      {
        ConduitConsumer component = smi.GetComponent<ConduitConsumer>();
        if (!Object.op_Inequality((Object) component, (Object) null))
          return;
        ((Behaviour) component).enabled = true;
      })).WorkableStopTransition((Func<HandSanitizer.SMInstance, Workable>) (smi => (Workable) smi.GetComponent<HandSanitizer.Work>()), this.notready);
    }

    private void UpdateStatusItems(HandSanitizer.SMInstance smi, float dt)
    {
      if (smi.OutputFull())
        ((Component) smi.master).GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.OutputPipeFull, (object) this);
      else
        ((Component) smi.master).GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.OutputPipeFull);
    }

    public class ReadyStates : 
      GameStateMachine<HandSanitizer.States, HandSanitizer.SMInstance, HandSanitizer, object>.State
    {
      public GameStateMachine<HandSanitizer.States, HandSanitizer.SMInstance, HandSanitizer, object>.State free;
      public GameStateMachine<HandSanitizer.States, HandSanitizer.SMInstance, HandSanitizer, object>.State occupied;
    }
  }

  [AddComponentMenu("KMonoBehaviour/Workable/Work")]
  public class Work : Workable, IGameObjectEffectDescriptor
  {
    public bool removeIrritation;
    private int diseaseRemoved;

    protected override void OnPrefabInit()
    {
      base.OnPrefabInit();
      this.resetProgressOnStop = true;
      this.shouldTransferDiseaseWithWorker = false;
      GameScheduler.Instance.Schedule("WaterFetchingTutorial", 2f, (Action<object>) (obj => Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_FetchingWater)), (object) null, (SchedulerGroup) null);
    }

    protected override void OnStartWork(Worker worker)
    {
      base.OnStartWork(worker);
      this.diseaseRemoved = 0;
    }

    protected override bool OnWorkTick(Worker worker, float dt)
    {
      base.OnWorkTick(worker, dt);
      HandSanitizer component1 = ((Component) this).GetComponent<HandSanitizer>();
      Storage component2 = ((Component) this).GetComponent<Storage>();
      float massAvailable = component2.GetMassAvailable(component1.consumedElement);
      if ((double) massAvailable == 0.0)
        return true;
      PrimaryElement component3 = ((Component) worker).GetComponent<PrimaryElement>();
      float amount = Mathf.Min(component1.massConsumedPerUse * dt / this.workTime, massAvailable);
      int num1 = Math.Min((int) ((double) dt / (double) this.workTime * (double) component1.diseaseRemovalCount), component3.DiseaseCount);
      this.diseaseRemoved += num1;
      SimUtil.DiseaseInfo invalid = SimUtil.DiseaseInfo.Invalid with
      {
        idx = component3.DiseaseIdx,
        count = num1
      };
      component3.ModifyDiseaseCount(-num1, "HandSanitizer.OnWorkTick");
      component1.maxPossiblyRemoved += num1;
      if (component1.canSanitizeStorage && Object.op_Implicit((Object) ((Component) worker).GetComponent<Storage>()))
      {
        foreach (GameObject gameObject in ((Component) worker).GetComponent<Storage>().GetItems())
        {
          PrimaryElement component4 = gameObject.GetComponent<PrimaryElement>();
          if (Object.op_Implicit((Object) component4))
          {
            int num2 = Math.Min((int) ((double) dt / (double) this.workTime * (double) component1.diseaseRemovalCount), component4.DiseaseCount);
            component4.ModifyDiseaseCount(-num2, "HandSanitizer.OnWorkTick");
            component1.maxPossiblyRemoved += num2;
          }
        }
      }
      SimUtil.DiseaseInfo disease_info = SimUtil.DiseaseInfo.Invalid;
      float amount_consumed;
      float aggregate_temperature;
      component2.ConsumeAndGetDisease(ElementLoader.FindElementByHash(component1.consumedElement).tag, amount, out amount_consumed, out disease_info, out aggregate_temperature);
      if (component1.outputElement != SimHashes.Vacuum)
      {
        SimUtil.DiseaseInfo finalDiseaseInfo = SimUtil.CalculateFinalDiseaseInfo(invalid, disease_info);
        component2.AddLiquid(component1.outputElement, amount_consumed, aggregate_temperature, finalDiseaseInfo.idx, finalDiseaseInfo.count);
      }
      return false;
    }

    protected override void OnCompleteWork(Worker worker)
    {
      base.OnCompleteWork(worker);
      if (!this.removeIrritation || ((Component) worker).HasTag(GameTags.HasSuitTank))
        return;
      ((Component) worker).GetSMI<GasLiquidExposureMonitor.Instance>()?.ResetExposure();
    }
  }
}
