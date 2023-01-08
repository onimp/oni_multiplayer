// Decompiled with JetBrains decompiler
// Type: OreScrubber
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class OreScrubber : StateMachineComponent<OreScrubber.SMInstance>, IGameObjectEffectDescriptor
{
  public float massConsumedPerUse = 1f;
  public SimHashes consumedElement = SimHashes.BleachStone;
  public int diseaseRemovalCount = 10000;
  public SimHashes outputElement = SimHashes.Vacuum;
  private WorkableReactable reactable;
  private MeterController cleanMeter;
  [Serialize]
  public int maxPossiblyRemoved;
  private static readonly EventSystem.IntraObjectHandler<OreScrubber> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<OreScrubber>((Action<OreScrubber, object>) ((component, data) => component.OnStorageChange(data)));

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    Util.FindOrAddComponent<Workable>(((Component) this).gameObject);
  }

  private void RefreshMeters()
  {
    float percent_full = 0.0f;
    PrimaryElement primaryElement = ((Component) this).GetComponent<Storage>().FindPrimaryElement(this.consumedElement);
    if (Object.op_Inequality((Object) primaryElement, (Object) null))
      percent_full = Mathf.Clamp01(primaryElement.Mass / ((Component) this).GetComponent<ConduitConsumer>().capacityKG);
    this.cleanMeter.SetPositionPercent(percent_full);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
    this.cleanMeter = new MeterController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "meter_clean_target", "meter_clean", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[1]
    {
      "meter_clean_target"
    });
    this.RefreshMeters();
    this.Subscribe<OreScrubber>(-1697596308, OreScrubber.OnStorageChangeDelegate);
    ((Component) this).GetComponent<DirectionControl>().onDirectionChanged += new Action<WorkableReactable.AllowedDirection>(this.OnDirectionChanged);
    this.OnDirectionChanged(((Component) this).GetComponent<DirectionControl>().allowedDirection);
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
    string name = ElementLoader.FindElementByHash(this.consumedElement).name;
    descriptorList.Add(new Descriptor(string.Format((string) UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, (object) name, (object) GameUtil.GetFormattedMass(this.massConsumedPerUse)), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, (object) name, (object) GameUtil.GetFormattedMass(this.massConsumedPerUse)), (Descriptor.DescriptorType) 0, false));
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

  private void OnStorageChange(object data) => this.RefreshMeters();

  private static PrimaryElement GetFirstInfected(Storage storage)
  {
    foreach (GameObject go in storage.items)
    {
      if (!Object.op_Equality((Object) go, (Object) null))
      {
        PrimaryElement component = go.GetComponent<PrimaryElement>();
        if (component.DiseaseIdx != byte.MaxValue && !go.HasTag(GameTags.Edible))
          return component;
      }
    }
    return (PrimaryElement) null;
  }

  private class ScrubOreReactable : WorkableReactable
  {
    public ScrubOreReactable(
      Workable workable,
      ChoreType chore_type,
      WorkableReactable.AllowedDirection allowed_direction = WorkableReactable.AllowedDirection.Any)
      : base(workable, HashedString.op_Implicit("ScrubOre"), chore_type, allowed_direction)
    {
    }

    public override bool InternalCanBegin(
      GameObject new_reactor,
      Navigator.ActiveTransition transition)
    {
      if (base.InternalCanBegin(new_reactor, transition))
      {
        Storage component = new_reactor.GetComponent<Storage>();
        if (Object.op_Inequality((Object) component, (Object) null) && Object.op_Inequality((Object) OreScrubber.GetFirstInfected(component), (Object) null))
          return true;
      }
      return false;
    }
  }

  public class SMInstance : 
    GameStateMachine<OreScrubber.States, OreScrubber.SMInstance, OreScrubber, object>.GameInstance
  {
    public SMInstance(OreScrubber master)
      : base(master)
    {
    }

    public bool HasSufficientMass()
    {
      bool flag = false;
      PrimaryElement primaryElement = this.GetComponent<Storage>().FindPrimaryElement(this.master.consumedElement);
      if (Object.op_Inequality((Object) primaryElement, (Object) null))
        flag = (double) primaryElement.Mass > 0.0;
      return flag;
    }

    public Dictionary<Tag, float> GetNeededMass()
    {
      Dictionary<Tag, float> neededMass = new Dictionary<Tag, float>();
      neededMass.Add(this.master.consumedElement.CreateTag(), this.master.massConsumedPerUse);
      return neededMass;
    }

    public void OnCompleteWork(Worker worker)
    {
    }

    public void DumpOutput()
    {
      Storage component = ((Component) this.master).GetComponent<Storage>();
      if (this.master.outputElement == SimHashes.Vacuum)
        return;
      component.Drop(ElementLoader.FindElementByHash(this.master.outputElement).tag);
    }
  }

  public class States : GameStateMachine<OreScrubber.States, OreScrubber.SMInstance, OreScrubber>
  {
    public GameStateMachine<OreScrubber.States, OreScrubber.SMInstance, OreScrubber, object>.State notready;
    public OreScrubber.States.ReadyStates ready;
    public GameStateMachine<OreScrubber.States, OreScrubber.SMInstance, OreScrubber, object>.State notoperational;
    public GameStateMachine<OreScrubber.States, OreScrubber.SMInstance, OreScrubber, object>.State full;
    public GameStateMachine<OreScrubber.States, OreScrubber.SMInstance, OreScrubber, object>.State empty;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.notready;
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      this.notoperational.PlayAnim("off").TagTransition(GameTags.Operational, this.notready);
      this.notready.PlayAnim("off").EventTransition(GameHashes.OnStorageChange, (GameStateMachine<OreScrubber.States, OreScrubber.SMInstance, OreScrubber, object>.State) this.ready, (StateMachine<OreScrubber.States, OreScrubber.SMInstance, OreScrubber, object>.Transition.ConditionCallback) (smi => smi.HasSufficientMass())).ToggleStatusItem((StatusItem) Db.Get().BuildingStatusItems.MaterialsUnavailable, (Func<OreScrubber.SMInstance, object>) (smi => (object) smi.GetNeededMass())).TagTransition(GameTags.Operational, this.notoperational, true);
      this.ready.DefaultState(this.ready.free).ToggleReactable((Func<OreScrubber.SMInstance, Reactable>) (smi => (Reactable) (smi.master.reactable = (WorkableReactable) new OreScrubber.ScrubOreReactable((Workable) ((Component) smi.master).GetComponent<OreScrubber.Work>(), Db.Get().ChoreTypes.ScrubOre, ((Component) smi.master).GetComponent<DirectionControl>().allowedDirection)))).EventTransition(GameHashes.OnStorageChange, this.notready, (StateMachine<OreScrubber.States, OreScrubber.SMInstance, OreScrubber, object>.Transition.ConditionCallback) (smi => !smi.HasSufficientMass())).TagTransition(GameTags.Operational, this.notoperational, true);
      this.ready.free.PlayAnim("on").WorkableStartTransition((Func<OreScrubber.SMInstance, Workable>) (smi => (Workable) smi.GetComponent<OreScrubber.Work>()), this.ready.occupied);
      this.ready.occupied.PlayAnim("working_pre").QueueAnim("working_loop", true).WorkableStopTransition((Func<OreScrubber.SMInstance, Workable>) (smi => (Workable) smi.GetComponent<OreScrubber.Work>()), (GameStateMachine<OreScrubber.States, OreScrubber.SMInstance, OreScrubber, object>.State) this.ready);
    }

    public class ReadyStates : 
      GameStateMachine<OreScrubber.States, OreScrubber.SMInstance, OreScrubber, object>.State
    {
      public GameStateMachine<OreScrubber.States, OreScrubber.SMInstance, OreScrubber, object>.State free;
      public GameStateMachine<OreScrubber.States, OreScrubber.SMInstance, OreScrubber, object>.State occupied;
    }
  }

  [AddComponentMenu("KMonoBehaviour/Workable/Work")]
  public class Work : Workable, IGameObjectEffectDescriptor
  {
    private int diseaseRemoved;

    protected override void OnPrefabInit()
    {
      base.OnPrefabInit();
      this.resetProgressOnStop = true;
      this.shouldTransferDiseaseWithWorker = false;
    }

    protected override void OnStartWork(Worker worker)
    {
      base.OnStartWork(worker);
      this.diseaseRemoved = 0;
    }

    protected override bool OnWorkTick(Worker worker, float dt)
    {
      base.OnWorkTick(worker, dt);
      OreScrubber component1 = ((Component) this).GetComponent<OreScrubber>();
      Storage component2 = ((Component) this).GetComponent<Storage>();
      PrimaryElement firstInfected = OreScrubber.GetFirstInfected(((Component) worker).GetComponent<Storage>());
      int num = 0;
      SimUtil.DiseaseInfo invalid = SimUtil.DiseaseInfo.Invalid;
      if (Object.op_Inequality((Object) firstInfected, (Object) null))
      {
        num = Math.Min((int) ((double) dt / (double) this.workTime * (double) component1.diseaseRemovalCount), firstInfected.DiseaseCount);
        this.diseaseRemoved += num;
        invalid.idx = firstInfected.DiseaseIdx;
        invalid.count = num;
        firstInfected.ModifyDiseaseCount(-num, "OreScrubber.OnWorkTick");
      }
      component1.maxPossiblyRemoved += num;
      float amount = component1.massConsumedPerUse * dt / this.workTime;
      SimUtil.DiseaseInfo disease_info = SimUtil.DiseaseInfo.Invalid;
      float amount_consumed;
      float aggregate_temperature;
      component2.ConsumeAndGetDisease(ElementLoader.FindElementByHash(component1.consumedElement).tag, amount, out amount_consumed, out disease_info, out aggregate_temperature);
      if (component1.outputElement != SimHashes.Vacuum)
      {
        disease_info = SimUtil.CalculateFinalDiseaseInfo(invalid, disease_info);
        component2.AddLiquid(component1.outputElement, amount_consumed, aggregate_temperature, disease_info.idx, disease_info.count);
      }
      return this.diseaseRemoved > component1.diseaseRemovalCount;
    }

    protected override void OnCompleteWork(Worker worker) => base.OnCompleteWork(worker);
  }
}
