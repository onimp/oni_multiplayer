// Decompiled with JetBrains decompiler
// Type: Toilet
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Toilet : 
  StateMachineComponent<Toilet.StatesInstance>,
  ISaveLoadable,
  IUsable,
  IGameObjectEffectDescriptor,
  IBasicBuilding
{
  [SerializeField]
  public Toilet.SpawnInfo solidWastePerUse;
  [SerializeField]
  public float solidWasteTemperature;
  [SerializeField]
  public Toilet.SpawnInfo gasWasteWhenFull;
  [SerializeField]
  public int maxFlushes = 15;
  [SerializeField]
  public string diseaseId;
  [SerializeField]
  public int diseasePerFlush;
  [SerializeField]
  public int diseaseOnDupePerFlush;
  [SerializeField]
  public float dirtUsedPerFlush = 13f;
  [Serialize]
  public int _flushesUsed;
  private MeterController meter;
  [MyCmpReq]
  private Storage storage;
  [MyCmpReq]
  private ManualDeliveryKG manualdeliverykg;
  private static readonly EventSystem.IntraObjectHandler<Toilet> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Toilet>((Action<Toilet, object>) ((component, data) => component.OnRefreshUserMenu(data)));

  public int FlushesUsed
  {
    get => this._flushesUsed;
    set
    {
      this._flushesUsed = value;
      this.smi.sm.flushes.Set(value, this.smi);
    }
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    Components.Toilets.Add((IUsable) this);
    Components.BasicBuildings.Add((IBasicBuilding) this);
    this.smi.StartSM();
    ((Component) this).GetComponent<ToiletWorkableUse>().trackUses = true;
    this.meter = new MeterController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, new string[3]
    {
      "meter_target",
      "meter_arrow",
      "meter_scale"
    });
    this.meter.SetPositionPercent((float) this.FlushesUsed / (float) this.maxFlushes);
    this.FlushesUsed = this._flushesUsed;
    this.Subscribe<Toilet>(493375141, Toilet.OnRefreshUserMenuDelegate);
  }

  protected override void OnCleanUp()
  {
    base.OnCleanUp();
    Components.BasicBuildings.Remove((IBasicBuilding) this);
    Components.Toilets.Remove((IUsable) this);
  }

  public bool IsUsable() => this.smi.HasTag(GameTags.Usable);

  public void Flush(Worker worker)
  {
    ++this.FlushesUsed;
    this.meter.SetPositionPercent((float) this.FlushesUsed / (float) this.maxFlushes);
    float aggregate_temperature = 0.0f;
    float amount_consumed;
    SimUtil.DiseaseInfo disease_info;
    this.storage.ConsumeAndGetDisease(ElementLoader.FindElementByHash(SimHashes.Dirt).tag, this.smi.DirtUsedPerFlush(), out amount_consumed, out disease_info, out aggregate_temperature);
    byte index = Db.Get().Diseases.GetIndex(HashedString.op_Implicit(this.diseaseId));
    float mass = this.smi.MassPerFlush() + amount_consumed;
    GameObject go = ElementLoader.FindElementByHash(this.solidWastePerUse.elementID).substance.SpawnResource(TransformExtensions.GetPosition(this.transform), mass, this.solidWasteTemperature, index, this.diseasePerFlush, true);
    go.GetComponent<PrimaryElement>().AddDisease(disease_info.idx, disease_info.count, "Toilet.Flush");
    this.storage.Store(go);
    ((Component) worker).GetComponent<PrimaryElement>().AddDisease(index, this.diseaseOnDupePerFlush, "Toilet.Flush");
    PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, string.Format((string) DUPLICANTS.DISEASES.ADDED_POPFX, (object) Db.Get().Diseases[(int) index].Name, (object) (this.diseasePerFlush + this.diseaseOnDupePerFlush)), this.transform, Vector3.up);
    Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_LotsOfGerms);
  }

  private void OnRefreshUserMenu(object data)
  {
    if (this.smi.GetCurrentState() == this.smi.sm.full || !this.smi.IsSoiled || this.smi.cleanChore != null)
      return;
    Game.Instance.userMenu.AddButton(((Component) this).gameObject, new KIconButtonMenu.ButtonInfo("status_item_toilet_needs_emptying", (string) UI.USERMENUACTIONS.CLEANTOILET.NAME, (System.Action) (() => this.smi.GoTo((StateMachine.BaseState) this.smi.sm.earlyclean)), tooltipText: ((string) UI.USERMENUACTIONS.CLEANTOILET.TOOLTIP)));
  }

  private void SpawnMonster() => GameUtil.KInstantiate(Assets.GetPrefab(new Tag("Glom")), TransformExtensions.GetPosition(this.smi.transform), Grid.SceneLayer.Creatures).SetActive(true);

  public List<Descriptor> RequirementDescriptors()
  {
    List<Descriptor> descriptorList = new List<Descriptor>();
    string str = ((Component) this).GetComponent<ManualDeliveryKG>().RequestedItemTag.ProperName();
    float mass = this.smi.DirtUsedPerFlush();
    Descriptor descriptor = new Descriptor();
    ((Descriptor) ref descriptor).SetupDescriptor(string.Format((string) UI.BUILDINGEFFECTS.ELEMENTCONSUMEDPERUSE, (object) str, (object) GameUtil.GetFormattedMass(mass, floatFormat: "{0:0.##}")), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTCONSUMEDPERUSE, (object) str, (object) GameUtil.GetFormattedMass(mass, floatFormat: "{0:0.##}")), (Descriptor.DescriptorType) 0);
    descriptorList.Add(descriptor);
    return descriptorList;
  }

  public List<Descriptor> EffectDescriptors()
  {
    List<Descriptor> descriptorList = new List<Descriptor>();
    string str = ElementLoader.FindElementByHash(this.solidWastePerUse.elementID).tag.ProperName();
    float mass = this.smi.MassPerFlush() + this.smi.DirtUsedPerFlush();
    descriptorList.Add(new Descriptor(string.Format((string) UI.BUILDINGEFFECTS.ELEMENTEMITTED_TOILET, (object) str, (object) GameUtil.GetFormattedMass(mass, floatFormat: "{0:0.##}"), (object) GameUtil.GetFormattedTemperature(this.solidWasteTemperature)), string.Format((string) UI.BUILDINGEFFECTS.TOOLTIPS.ELEMENTEMITTED_TOILET, (object) str, (object) GameUtil.GetFormattedMass(mass, floatFormat: "{0:0.##}"), (object) GameUtil.GetFormattedTemperature(this.solidWasteTemperature)), (Descriptor.DescriptorType) 1, false));
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

  [Serializable]
  public struct SpawnInfo
  {
    [HashedEnum]
    public SimHashes elementID;
    public float mass;
    public float interval;

    public SpawnInfo(SimHashes element_id, float mass, float interval)
    {
      this.elementID = element_id;
      this.mass = mass;
      this.interval = interval;
    }
  }

  public class StatesInstance : 
    GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.GameInstance
  {
    public Chore cleanChore;
    public List<Chore> activeUseChores;
    public float monsterSpawnTime = 1200f;

    public StatesInstance(Toilet master)
      : base(master)
    {
      this.activeUseChores = new List<Chore>();
    }

    public bool IsSoiled => this.master.FlushesUsed > 0;

    public int GetFlushesRemaining() => this.master.maxFlushes - this.master.FlushesUsed;

    public bool RequiresDirtDelivery()
    {
      if (this.master.storage.IsEmpty())
        return true;
      Tag tag = ElementLoader.FindElementByHash(SimHashes.Dirt).tag;
      return !this.master.storage.Has(tag) || (double) this.master.storage.GetAmountAvailable(tag) < (double) this.master.manualdeliverykg.capacity && !this.IsSoiled;
    }

    public float MassPerFlush() => this.master.solidWastePerUse.mass;

    public float DirtUsedPerFlush() => this.master.dirtUsedPerFlush;

    public bool IsToxicSandRemoved() => Object.op_Equality((Object) this.master.storage.FindFirst(GameTagExtensions.Create(this.master.solidWastePerUse.elementID)), (Object) null);

    public void CreateCleanChore()
    {
      if (this.cleanChore != null)
        this.cleanChore.Cancel("dupe");
      ToiletWorkableClean component = ((Component) this.master).GetComponent<ToiletWorkableClean>();
      this.cleanChore = (Chore) new WorkChore<ToiletWorkableClean>(Db.Get().ChoreTypes.CleanToilet, (IStateMachineTarget) component, on_complete: new Action<Chore>(this.OnCleanComplete), ignore_building_assignment: true);
    }

    public void CancelCleanChore()
    {
      if (this.cleanChore == null)
        return;
      this.cleanChore.Cancel("Cancelled");
      this.cleanChore = (Chore) null;
    }

    private void DropFromStorage(Tag tag)
    {
      ListPool<GameObject, Toilet>.PooledList result = ListPool<GameObject, Toilet>.Allocate();
      this.master.storage.Find(tag, (List<GameObject>) result);
      foreach (GameObject go in (List<GameObject>) result)
        this.master.storage.Drop(go, true);
      result.Recycle();
    }

    private void OnCleanComplete(Chore chore)
    {
      this.cleanChore = (Chore) null;
      Tag tag1 = GameTagExtensions.Create(this.master.solidWastePerUse.elementID);
      Tag tag2 = ElementLoader.FindElementByHash(SimHashes.Dirt).tag;
      this.DropFromStorage(tag1);
      this.DropFromStorage(tag2);
      this.master.meter.SetPositionPercent((float) this.master.FlushesUsed / (float) this.master.maxFlushes);
    }

    public void Flush() => this.master.Flush(((Component) this.master).GetComponent<ToiletWorkableUse>().worker);
  }

  public class States : GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet>
  {
    public GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.State needsdirt;
    public GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.State empty;
    public GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.State notoperational;
    public GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.State ready;
    public GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.State earlyclean;
    public GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.State earlyWaitingForClean;
    public GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.State full;
    public GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.State fullWaitingForClean;
    private static readonly HashedString[] FULL_ANIMS = new HashedString[2]
    {
      HashedString.op_Implicit("full_pre"),
      HashedString.op_Implicit(nameof (full))
    };
    public StateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.IntParameter flushes = new StateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.IntParameter(0);

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.needsdirt;
      this.root.PlayAnim("off").EventTransition(GameHashes.OnStorageChange, this.needsdirt, (StateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.Transition.ConditionCallback) (smi => smi.RequiresDirtDelivery())).EventTransition(GameHashes.OperationalChanged, this.notoperational, (StateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.Transition.ConditionCallback) (smi => !smi.Get<Operational>().IsOperational));
      this.needsdirt.Enter((StateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.State.Callback) (smi =>
      {
        if (!smi.RequiresDirtDelivery())
          return;
        smi.master.manualdeliverykg.RequestDelivery();
      })).ToggleMainStatusItem(Db.Get().BuildingStatusItems.Unusable).EventTransition(GameHashes.OnStorageChange, this.ready, (StateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.Transition.ConditionCallback) (smi => !smi.RequiresDirtDelivery()));
      this.ready.ParamTransition<int>((StateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.Parameter<int>) this.flushes, this.full, (StateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.Parameter<int>.Callback) ((smi, p) => smi.GetFlushesRemaining() <= 0)).ToggleMainStatusItem(Db.Get().BuildingStatusItems.Toilet).ToggleRecurringChore(new Func<Toilet.StatesInstance, Chore>(this.CreateUrgentUseChore)).ToggleRecurringChore(new Func<Toilet.StatesInstance, Chore>(this.CreateBreakUseChore)).ToggleTag(GameTags.Usable).EventHandler(GameHashes.Flush, (GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.GameEvent.Callback) ((smi, data) => smi.Flush()));
      this.earlyclean.PlayAnims((Func<Toilet.StatesInstance, HashedString[]>) (smi => Toilet.States.FULL_ANIMS)).OnAnimQueueComplete(this.earlyWaitingForClean);
      this.earlyWaitingForClean.Enter((StateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.State.Callback) (smi => smi.CreateCleanChore())).Exit((StateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.State.Callback) (smi => smi.CancelCleanChore())).ToggleStatusItem(Db.Get().BuildingStatusItems.ToiletNeedsEmptying).ToggleMainStatusItem(Db.Get().BuildingStatusItems.Unusable).EventTransition(GameHashes.OnStorageChange, this.empty, (StateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.Transition.ConditionCallback) (smi => smi.IsToxicSandRemoved()));
      this.full.PlayAnims((Func<Toilet.StatesInstance, HashedString[]>) (smi => Toilet.States.FULL_ANIMS)).OnAnimQueueComplete(this.fullWaitingForClean);
      this.fullWaitingForClean.Enter((StateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.State.Callback) (smi => smi.CreateCleanChore())).Exit((StateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.State.Callback) (smi => smi.CancelCleanChore())).ToggleStatusItem(Db.Get().BuildingStatusItems.ToiletNeedsEmptying).ToggleMainStatusItem(Db.Get().BuildingStatusItems.Unusable).EventTransition(GameHashes.OnStorageChange, this.empty, (StateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.Transition.ConditionCallback) (smi => smi.IsToxicSandRemoved())).Enter((StateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.State.Callback) (smi => smi.Schedule(smi.monsterSpawnTime, (Action<object>) (_param1 => smi.master.SpawnMonster()), (object) null)));
      this.empty.PlayAnim("off").Enter("ClearFlushes", (StateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.State.Callback) (smi => smi.master.FlushesUsed = 0)).GoTo(this.needsdirt);
      this.notoperational.EventTransition(GameHashes.OperationalChanged, this.needsdirt, (StateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.Transition.ConditionCallback) (smi => smi.Get<Operational>().IsOperational)).ToggleMainStatusItem(Db.Get().BuildingStatusItems.Unusable);
    }

    private Chore CreateUrgentUseChore(Toilet.StatesInstance smi)
    {
      Chore useChore = this.CreateUseChore(smi, Db.Get().ChoreTypes.Pee);
      useChore.AddPrecondition(ChorePreconditions.instance.IsBladderFull);
      useChore.AddPrecondition(ChorePreconditions.instance.NotCurrentlyPeeing);
      return useChore;
    }

    private Chore CreateBreakUseChore(Toilet.StatesInstance smi)
    {
      Chore useChore = this.CreateUseChore(smi, Db.Get().ChoreTypes.BreakPee);
      useChore.AddPrecondition(ChorePreconditions.instance.IsBladderNotFull);
      useChore.AddPrecondition(ChorePreconditions.instance.IsScheduledTime, (object) Db.Get().ScheduleBlockTypes.Hygiene);
      return useChore;
    }

    private Chore CreateUseChore(Toilet.StatesInstance smi, ChoreType choreType)
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
      GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.State
    {
      public GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.State idle;
      public GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.State inuse;
      public GameStateMachine<Toilet.States, Toilet.StatesInstance, Toilet, object>.State flush;
    }
  }
}
