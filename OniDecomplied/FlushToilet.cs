// Decompiled with JetBrains decompiler
// Type: FlushToilet
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FlushToilet : 
  StateMachineComponent<FlushToilet.SMInstance>,
  IUsable,
  IGameObjectEffectDescriptor,
  IBasicBuilding
{
  private MeterController fillMeter;
  private MeterController contaminationMeter;
  public Meter.Offset meterOffset = Meter.Offset.Behind;
  [SerializeField]
  public float massConsumedPerUse = 5f;
  [SerializeField]
  public float massEmittedPerUse = 5f;
  [SerializeField]
  public float newPeeTemperature;
  [SerializeField]
  public string diseaseId;
  [SerializeField]
  public int diseasePerFlush;
  [SerializeField]
  public int diseaseOnDupePerFlush;
  [SerializeField]
  public bool requireOutput = true;
  [MyCmpGet]
  private ConduitConsumer conduitConsumer;
  [MyCmpGet]
  private Storage storage;
  public static readonly Tag WaterTag = GameTagExtensions.Create(SimHashes.Water);
  private int inputCell;
  private int outputCell;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    Building component1 = ((Component) this).GetComponent<Building>();
    this.inputCell = component1.GetUtilityInputCell();
    this.outputCell = component1.GetUtilityOutputCell();
    ConduitFlow liquidConduitFlow = Game.Instance.liquidConduitFlow;
    liquidConduitFlow.onConduitsRebuilt += new System.Action(this.OnConduitsRebuilt);
    liquidConduitFlow.AddConduitUpdater(new Action<float>(this.OnConduitUpdate), ConduitFlowPriority.Default);
    KBatchedAnimController component2 = ((Component) this).GetComponent<KBatchedAnimController>();
    this.fillMeter = new MeterController((KAnimControllerBase) component2, "meter_target", "meter", this.meterOffset, Grid.SceneLayer.NoLayer, new Vector3(0.4f, 3.2f, 0.1f), Array.Empty<string>());
    this.contaminationMeter = new MeterController((KAnimControllerBase) component2, "meter_target", "meter_dirty", this.meterOffset, Grid.SceneLayer.NoLayer, new Vector3(0.4f, 3.2f, 0.1f), Array.Empty<string>());
    Components.Toilets.Add((IUsable) this);
    Components.BasicBuildings.Add((IBasicBuilding) this);
    this.smi.StartSM();
    this.smi.ShowFillMeter();
  }

  protected override void OnCleanUp()
  {
    Game.Instance.liquidConduitFlow.onConduitsRebuilt -= new System.Action(this.OnConduitsRebuilt);
    Components.BasicBuildings.Remove((IBasicBuilding) this);
    Components.Toilets.Remove((IUsable) this);
    base.OnCleanUp();
  }

  private void OnConduitsRebuilt() => this.Trigger(-2094018600, (object) null);

  public bool IsUsable() => this.smi.HasTag(GameTags.Usable);

  private void Flush(Worker worker)
  {
    ListPool<GameObject, Storage>.PooledList result = ListPool<GameObject, Storage>.Allocate();
    this.storage.Find(FlushToilet.WaterTag, (List<GameObject>) result);
    float num1 = 0.0f;
    float massConsumedPerUse = this.massConsumedPerUse;
    foreach (GameObject gameObject in (List<GameObject>) result)
    {
      PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
      float num2 = Mathf.Min(component.Mass, massConsumedPerUse);
      component.Mass -= num2;
      massConsumedPerUse -= num2;
      num1 += num2 * component.Temperature;
    }
    result.Recycle();
    float num3 = this.massEmittedPerUse - this.massConsumedPerUse;
    float temperature = (num1 + num3 * this.newPeeTemperature) / this.massEmittedPerUse;
    byte index = Db.Get().Diseases.GetIndex(HashedString.op_Implicit(this.diseaseId));
    this.storage.AddLiquid(SimHashes.DirtyWater, this.massEmittedPerUse, temperature, index, this.diseasePerFlush);
    if (Object.op_Inequality((Object) worker, (Object) null))
    {
      ((Component) worker).GetComponent<PrimaryElement>().AddDisease(index, this.diseaseOnDupePerFlush, "FlushToilet.Flush");
      PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, string.Format((string) DUPLICANTS.DISEASES.ADDED_POPFX, (object) Db.Get().Diseases[(int) index].Name, (object) (this.diseasePerFlush + this.diseaseOnDupePerFlush)), this.transform, Vector3.up);
      Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_LotsOfGerms);
    }
    else
      DebugUtil.LogWarningArgs(new object[1]
      {
        (object) "Tried to add disease on toilet use but worker was null"
      });
  }

  public List<Descriptor> RequirementDescriptors()
  {
    List<Descriptor> descriptorList = new List<Descriptor>();
    string str = ElementLoader.FindElementByHash(SimHashes.Water).tag.ProperName();
    descriptorList.Add(new Descriptor(string.Format((string) UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, (object) str, (object) GameUtil.GetFormattedMass(this.massConsumedPerUse, floatFormat: "{0:0.##}")), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, (object) str, (object) GameUtil.GetFormattedMass(this.massConsumedPerUse, floatFormat: "{0:0.##}")), (Descriptor.DescriptorType) 0, false));
    return descriptorList;
  }

  public List<Descriptor> EffectDescriptors()
  {
    List<Descriptor> descriptorList = new List<Descriptor>();
    string str = ElementLoader.FindElementByHash(SimHashes.DirtyWater).tag.ProperName();
    descriptorList.Add(new Descriptor(string.Format((string) UI.BUILDINGEFFECTS.ELEMENTEMITTED_TOILET, (object) str, (object) GameUtil.GetFormattedMass(this.massEmittedPerUse, floatFormat: "{0:0.##}"), (object) GameUtil.GetFormattedTemperature(this.newPeeTemperature)), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_TOILET, (object) str, (object) GameUtil.GetFormattedMass(this.massEmittedPerUse, floatFormat: "{0:0.##}"), (object) GameUtil.GetFormattedTemperature(this.newPeeTemperature)), (Descriptor.DescriptorType) 1, false));
    Klei.AI.Disease disease = Db.Get().Diseases.Get(this.diseaseId);
    int units = this.diseasePerFlush + this.diseaseOnDupePerFlush;
    descriptorList.Add(new Descriptor(string.Format((string) UI.BUILDINGEFFECTS.DISEASEEMITTEDPERUSE, (object) disease.Name, (object) GameUtil.GetFormattedDiseaseAmount(units)), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.DISEASEEMITTEDPERUSE, (object) disease.Name, (object) GameUtil.GetFormattedDiseaseAmount(units)), (Descriptor.DescriptorType) 4, false));
    return descriptorList;
  }

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    descriptors.AddRange((IEnumerable<Descriptor>) this.RequirementDescriptors());
    descriptors.AddRange((IEnumerable<Descriptor>) this.EffectDescriptors());
    return descriptors;
  }

  private void OnConduitUpdate(float dt)
  {
    if (this.GetSMI() == null)
      return;
    ConduitFlow liquidConduitFlow = Game.Instance.liquidConduitFlow;
    this.smi.sm.outputBlocked.Set(this.smi.master.requireOutput && (double) liquidConduitFlow.GetContents(this.outputCell).mass > 0.0 && this.smi.HasContaminatedMass(), this.smi);
  }

  public class SMInstance : 
    GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.GameInstance
  {
    public List<Chore> activeUseChores;

    public SMInstance(FlushToilet master)
      : base(master)
    {
      this.activeUseChores = new List<Chore>();
      this.UpdateFullnessState();
      this.UpdateDirtyState();
    }

    public bool HasValidConnections()
    {
      if (!Game.Instance.liquidConduitFlow.HasConduit(this.master.inputCell))
        return false;
      return !this.master.requireOutput || Game.Instance.liquidConduitFlow.HasConduit(this.master.outputCell);
    }

    public bool UpdateFullnessState()
    {
      float num = 0.0f;
      ListPool<GameObject, FlushToilet>.PooledList result = ListPool<GameObject, FlushToilet>.Allocate();
      this.master.storage.Find(FlushToilet.WaterTag, (List<GameObject>) result);
      foreach (GameObject gameObject in (List<GameObject>) result)
      {
        PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
        num += component.Mass;
      }
      result.Recycle();
      bool flag = (double) num >= (double) this.master.massConsumedPerUse;
      ((Behaviour) this.master.conduitConsumer).enabled = !flag;
      this.master.fillMeter.SetPositionPercent(Mathf.Clamp01(num / this.master.massConsumedPerUse));
      return flag;
    }

    public void UpdateDirtyState() => this.master.contaminationMeter.SetPositionPercent(this.GetComponent<ToiletWorkableUse>().GetPercentComplete());

    public void Flush()
    {
      this.master.fillMeter.SetPositionPercent(0.0f);
      this.master.contaminationMeter.SetPositionPercent(1f);
      this.smi.ShowFillMeter();
      this.master.Flush(((Component) this.master).GetComponent<ToiletWorkableUse>().worker);
    }

    public void ShowFillMeter()
    {
      this.master.fillMeter.gameObject.SetActive(true);
      this.master.contaminationMeter.gameObject.SetActive(false);
    }

    public bool HasContaminatedMass()
    {
      foreach (GameObject gameObject in this.GetComponent<Storage>().items)
      {
        PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
        if (!Object.op_Equality((Object) component, (Object) null) && component.ElementID == SimHashes.DirtyWater && (double) component.Mass > 0.0)
          return true;
      }
      return false;
    }

    public void ShowContaminatedMeter()
    {
      this.master.fillMeter.gameObject.SetActive(false);
      this.master.contaminationMeter.gameObject.SetActive(true);
    }
  }

  public class States : GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet>
  {
    public GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State disconnected;
    public GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State backedup;
    public FlushToilet.States.ReadyStates ready;
    public GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State fillingInactive;
    public GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State filling;
    public GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State flushing;
    public GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State flushed;
    public StateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.BoolParameter outputBlocked;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.disconnected;
      this.disconnected.PlayAnim("off").EventTransition(GameHashes.ConduitConnectionChanged, this.backedup, (StateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.Transition.ConditionCallback) (smi => smi.HasValidConnections())).Enter((StateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State.Callback) (smi => smi.GetComponent<Operational>().SetActive(false)));
      this.backedup.PlayAnim("off").ToggleStatusItem(Db.Get().BuildingStatusItems.OutputPipeFull).EventTransition(GameHashes.ConduitConnectionChanged, this.disconnected, (StateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.Transition.ConditionCallback) (smi => !smi.HasValidConnections())).ParamTransition<bool>((StateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.Parameter<bool>) this.outputBlocked, this.fillingInactive, GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.IsFalse).Enter((StateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State.Callback) (smi => smi.GetComponent<Operational>().SetActive(false)));
      this.filling.PlayAnim("off").Enter((StateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State.Callback) (smi => smi.GetComponent<Operational>().SetActive(true))).EventTransition(GameHashes.ConduitConnectionChanged, this.disconnected, (StateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.Transition.ConditionCallback) (smi => !smi.HasValidConnections())).ParamTransition<bool>((StateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.Parameter<bool>) this.outputBlocked, this.backedup, GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.IsTrue).EventTransition(GameHashes.OnStorageChange, (GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State) this.ready, (StateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.Transition.ConditionCallback) (smi => smi.UpdateFullnessState())).EventTransition(GameHashes.OperationalChanged, this.fillingInactive, (StateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.Transition.ConditionCallback) (smi => !smi.GetComponent<Operational>().IsOperational));
      this.fillingInactive.PlayAnim("off").Enter((StateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State.Callback) (smi => smi.GetComponent<Operational>().SetActive(false))).EventTransition(GameHashes.OperationalChanged, this.filling, (StateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.Transition.ConditionCallback) (smi => smi.GetComponent<Operational>().IsOperational)).ParamTransition<bool>((StateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.Parameter<bool>) this.outputBlocked, this.backedup, GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.IsTrue);
      this.ready.DefaultState(this.ready.idle).ToggleTag(GameTags.Usable).Enter((StateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State.Callback) (smi =>
      {
        smi.master.fillMeter.SetPositionPercent(1f);
        smi.master.contaminationMeter.SetPositionPercent(0.0f);
      })).PlayAnim("off").EventTransition(GameHashes.ConduitConnectionChanged, this.disconnected, (StateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.Transition.ConditionCallback) (smi => !smi.HasValidConnections())).ParamTransition<bool>((StateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.Parameter<bool>) this.outputBlocked, this.backedup, GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.IsTrue).ToggleChore(new Func<FlushToilet.SMInstance, Chore>(this.CreateUrgentUseChore), this.flushing).ToggleChore(new Func<FlushToilet.SMInstance, Chore>(this.CreateBreakUseChore), this.flushing);
      this.ready.idle.Enter((StateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State.Callback) (smi => smi.GetComponent<Operational>().SetActive(false))).ToggleMainStatusItem(Db.Get().BuildingStatusItems.FlushToilet).WorkableStartTransition((Func<FlushToilet.SMInstance, Workable>) (smi => (Workable) ((Component) smi.master).GetComponent<ToiletWorkableUse>()), this.ready.inuse);
      this.ready.inuse.Enter((StateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State.Callback) (smi => smi.ShowContaminatedMeter())).ToggleMainStatusItem(Db.Get().BuildingStatusItems.FlushToiletInUse).Update((Action<FlushToilet.SMInstance, float>) ((smi, dt) => smi.UpdateDirtyState())).WorkableCompleteTransition((Func<FlushToilet.SMInstance, Workable>) (smi => (Workable) ((Component) smi.master).GetComponent<ToiletWorkableUse>()), this.flushing).WorkableStopTransition((Func<FlushToilet.SMInstance, Workable>) (smi => (Workable) ((Component) smi.master).GetComponent<ToiletWorkableUse>()), this.flushed);
      this.flushing.Enter((StateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State.Callback) (smi => smi.Flush())).PlayAnim("flush").OnAnimQueueComplete(this.flushed);
      this.flushed.EventTransition(GameHashes.OnStorageChange, this.fillingInactive, (StateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.Transition.ConditionCallback) (smi => !smi.HasContaminatedMass())).ParamTransition<bool>((StateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.Parameter<bool>) this.outputBlocked, this.backedup, GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.IsTrue);
    }

    private Chore CreateUrgentUseChore(FlushToilet.SMInstance smi)
    {
      Chore useChore = this.CreateUseChore(smi, Db.Get().ChoreTypes.Pee);
      useChore.AddPrecondition(ChorePreconditions.instance.IsBladderFull);
      useChore.AddPrecondition(ChorePreconditions.instance.NotCurrentlyPeeing);
      return useChore;
    }

    private Chore CreateBreakUseChore(FlushToilet.SMInstance smi)
    {
      Chore useChore = this.CreateUseChore(smi, Db.Get().ChoreTypes.BreakPee);
      useChore.AddPrecondition(ChorePreconditions.instance.IsBladderNotFull);
      return useChore;
    }

    private Chore CreateUseChore(FlushToilet.SMInstance smi, ChoreType choreType)
    {
      WorkChore<ToiletWorkableUse> useChore = new WorkChore<ToiletWorkableUse>(choreType, (IStateMachineTarget) smi.master, allow_in_red_alert: false, ignore_schedule_block: true, allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.personalNeeds, add_to_daily_report: false);
      smi.activeUseChores.Add((Chore) useChore);
      WorkChore<ToiletWorkableUse> workChore = useChore;
      workChore.onExit = workChore.onExit + (Action<Chore>) (exiting_chore => smi.activeUseChores.Remove(exiting_chore));
      useChore.AddPrecondition(ChorePreconditions.instance.IsPreferredAssignableOrUrgentBladder, (object) ((Component) smi.master).GetComponent<Assignable>());
      useChore.AddPrecondition(ChorePreconditions.instance.IsExclusivelyAvailableWithOtherChores, (object) smi.activeUseChores);
      return (Chore) useChore;
    }

    public class ReadyStates : 
      GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State
    {
      public GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State idle;
      public GameStateMachine<FlushToilet.States, FlushToilet.SMInstance, FlushToilet, object>.State inuse;
    }
  }
}
