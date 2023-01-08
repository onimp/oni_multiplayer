// Decompiled with JetBrains decompiler
// Type: Repairable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;
using TUNING;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/Workable/Repairable")]
public class Repairable : Workable
{
  public float expectedRepairTime = -1f;
  [MyCmpGet]
  private BuildingHP hp;
  private Repairable.SMInstance smi;
  private Storage storageProxy;
  [Serialize]
  private byte[] storedData;
  private float timeSpentRepairing;
  private static readonly Operational.Flag repairedFlag = new Operational.Flag("repaired", Operational.Flag.Type.Functional);
  private static readonly EventSystem.IntraObjectHandler<Repairable> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<Repairable>((Action<Repairable, object>) ((component, data) => component.OnRefreshUserMenu(data)));

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.SetOffsetTable(OffsetGroups.InvertedStandardTableWithCorners);
    this.Subscribe<Repairable>(493375141, Repairable.OnRefreshUserMenuDelegate);
    this.attributeConverter = Db.Get().AttributeConverters.ConstructionSpeed;
    this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
    this.skillExperienceSkillGroup = Db.Get().SkillGroups.Building.Id;
    this.skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
    this.showProgressBar = false;
    this.faceTargetWhenWorking = true;
    this.multitoolContext = HashedString.op_Implicit("build");
    this.multitoolHitEffectTag = Tag.op_Implicit(EffectConfigs.BuildSplashId);
    this.workingPstComplete = (HashedString[]) null;
    this.workingPstFailed = (HashedString[]) null;
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.smi = new Repairable.SMInstance(this);
    this.smi.StartSM();
    this.workTime = float.PositiveInfinity;
    this.workTimeRemaining = float.PositiveInfinity;
  }

  private void OnProxyStorageChanged(object data) => this.Trigger(-1697596308, data);

  protected override void OnLoadLevel()
  {
    this.smi = (Repairable.SMInstance) null;
    base.OnLoadLevel();
  }

  protected override void OnCleanUp()
  {
    if (this.smi != null)
      this.smi.StopSM("Destroy Repairable");
    base.OnCleanUp();
  }

  private void OnRefreshUserMenu(object data)
  {
    if (!Object.op_Inequality((Object) ((Component) this).gameObject, (Object) null) || this.smi == null)
      return;
    if (this.smi.GetCurrentState() == this.smi.sm.forbidden)
      Game.Instance.userMenu.AddButton(((Component) this).gameObject, new KIconButtonMenu.ButtonInfo("action_repair", (string) STRINGS.BUILDINGS.REPAIRABLE.ENABLE_AUTOREPAIR.NAME, new System.Action(this.AllowRepair), tooltipText: ((string) STRINGS.BUILDINGS.REPAIRABLE.ENABLE_AUTOREPAIR.TOOLTIP)), 0.5f);
    else
      Game.Instance.userMenu.AddButton(((Component) this).gameObject, new KIconButtonMenu.ButtonInfo("action_repair", (string) STRINGS.BUILDINGS.REPAIRABLE.DISABLE_AUTOREPAIR.NAME, new System.Action(this.CancelRepair), tooltipText: ((string) STRINGS.BUILDINGS.REPAIRABLE.DISABLE_AUTOREPAIR.TOOLTIP)), 0.5f);
  }

  private void AllowRepair()
  {
    if (DebugHandler.InstantBuildMode)
    {
      this.hp.Repair(this.hp.MaxHitPoints);
      this.OnCompleteWork((Worker) null);
    }
    this.smi.sm.allow.Trigger(this.smi);
    this.OnRefreshUserMenu((object) null);
  }

  public void CancelRepair()
  {
    if (this.smi != null)
      this.smi.sm.forbid.Trigger(this.smi);
    this.OnRefreshUserMenu((object) null);
  }

  protected override void OnStartWork(Worker worker)
  {
    base.OnStartWork(worker);
    Operational component = ((Component) this).GetComponent<Operational>();
    if (Object.op_Inequality((Object) component, (Object) null))
      component.SetFlag(Repairable.repairedFlag, false);
    this.timeSpentRepairing = 0.0f;
  }

  protected override bool OnWorkTick(Worker worker, float dt)
  {
    float num1 = Mathf.Sqrt(((Component) this).GetComponent<PrimaryElement>().Mass);
    float num2 = (float) (((double) this.expectedRepairTime < 0.0 ? (double) num1 : (double) this.expectedRepairTime) * 0.10000000149011612);
    if ((double) this.timeSpentRepairing >= (double) num2)
    {
      this.timeSpentRepairing -= num2;
      int num3 = 0;
      if (Object.op_Inequality((Object) worker, (Object) null))
        num3 = (int) Db.Get().Attributes.Machinery.Lookup((Component) worker).GetTotalValue();
      this.hp.Repair(Mathf.CeilToInt((float) (10 + Math.Max(0, num3 * 10)) * 0.1f));
      if (this.hp.HitPoints >= this.hp.MaxHitPoints)
        return true;
    }
    this.timeSpentRepairing += dt;
    return false;
  }

  protected override void OnStopWork(Worker worker)
  {
    base.OnStopWork(worker);
    Operational component = ((Component) this).GetComponent<Operational>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.SetFlag(Repairable.repairedFlag, true);
  }

  protected override void OnCompleteWork(Worker worker)
  {
    Operational component = ((Component) this).GetComponent<Operational>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.SetFlag(Repairable.repairedFlag, true);
  }

  public void CreateStorageProxy()
  {
    if (!Object.op_Equality((Object) this.storageProxy, (Object) null))
      return;
    GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(RepairableStorageProxy.ID)), ((Component) this.transform).gameObject, (string) null);
    TransformExtensions.SetLocalPosition(gameObject.transform, Vector3.zero);
    this.storageProxy = gameObject.GetComponent<Storage>();
    this.storageProxy.prioritizable = ((Component) this.transform).GetComponent<Prioritizable>();
    this.storageProxy.prioritizable.AddRef();
    gameObject.SetActive(true);
  }

  [System.Runtime.Serialization.OnSerializing]
  private void OnSerializing()
  {
    this.storedData = (byte[]) null;
    if (!Object.op_Inequality((Object) this.storageProxy, (Object) null) || this.storageProxy.IsEmpty())
      return;
    using (MemoryStream output = new MemoryStream())
    {
      using (BinaryWriter writer = new BinaryWriter((Stream) output))
        this.storageProxy.Serialize(writer);
      this.storedData = output.ToArray();
    }
  }

  [System.Runtime.Serialization.OnSerialized]
  private void OnSerialized() => this.storedData = (byte[]) null;

  [System.Runtime.Serialization.OnDeserialized]
  private void OnDeserialized()
  {
    if (this.storedData == null)
      return;
    FastReader reader = new FastReader(this.storedData);
    this.CreateStorageProxy();
    this.storageProxy.Deserialize((IReader) reader);
    this.storedData = (byte[]) null;
  }

  public class SMInstance : 
    GameStateMachine<Repairable.States, Repairable.SMInstance, Repairable, object>.GameInstance
  {
    private const float REQUIRED_MASS_SCALE = 0.1f;

    public SMInstance(Repairable smi)
      : base(smi)
    {
    }

    public bool HasRequiredMass()
    {
      PrimaryElement component = this.GetComponent<PrimaryElement>();
      float num = component.Mass * 0.1f;
      PrimaryElement primaryElement = this.smi.master.storageProxy.FindPrimaryElement(component.ElementID);
      return Object.op_Inequality((Object) primaryElement, (Object) null) && (double) primaryElement.Mass >= (double) num;
    }

    public KeyValuePair<Tag, float> GetRequiredMass()
    {
      PrimaryElement component = this.GetComponent<PrimaryElement>();
      float num1 = component.Mass * 0.1f;
      PrimaryElement primaryElement = this.smi.master.storageProxy.FindPrimaryElement(component.ElementID);
      float num2 = Object.op_Inequality((Object) primaryElement, (Object) null) ? Math.Max(0.0f, num1 - primaryElement.Mass) : num1;
      return new KeyValuePair<Tag, float>(component.Element.tag, num2);
    }

    public void ConsumeRepairMaterials() => this.smi.master.storageProxy.ConsumeAllIgnoringDisease();

    public void DestroyStorageProxy()
    {
      if (!Object.op_Inequality((Object) this.smi.master.storageProxy, (Object) null))
        return;
      ((Component) this.smi.master.transform).GetComponent<Prioritizable>().RemoveRef();
      this.smi.master.storageProxy.DropAll(false, false, new Vector3(), true, (List<GameObject>) null);
      Util.KDestroyGameObject(((Component) this.smi.master.storageProxy).gameObject);
    }

    public bool NeedsRepairs() => ((Component) this.smi.master).GetComponent<BuildingHP>().NeedsRepairs;
  }

  public class States : GameStateMachine<Repairable.States, Repairable.SMInstance, Repairable>
  {
    public StateMachine<Repairable.States, Repairable.SMInstance, Repairable, object>.Signal allow;
    public StateMachine<Repairable.States, Repairable.SMInstance, Repairable, object>.Signal forbid;
    public GameStateMachine<Repairable.States, Repairable.SMInstance, Repairable, object>.State forbidden;
    public Repairable.States.AllowedState allowed;
    public GameStateMachine<Repairable.States, Repairable.SMInstance, Repairable, object>.State repaired;
    public static readonly Chore.Precondition IsNotBeingAttacked = new Chore.Precondition()
    {
      id = nameof (IsNotBeingAttacked),
      description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_NOT_BEING_ATTACKED,
      fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
      {
        bool flag = true;
        if (data != null)
          flag = Object.op_Equality((Object) ((Workable) data).worker, (Object) null);
        return flag;
      })
    };
    public static readonly Chore.Precondition IsNotAngry = new Chore.Precondition()
    {
      id = nameof (IsNotAngry),
      description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_NOT_ANGRY,
      fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
      {
        Traits traits = context.consumerState.traits;
        AmountInstance amountInstance = Db.Get().Amounts.Stress.Lookup(context.consumerState.gameObject);
        return !Object.op_Inequality((Object) traits, (Object) null) || amountInstance == null || (double) amountInstance.value < (double) TUNING.STRESS.ACTING_OUT_RESET || !traits.HasTrait("Aggressive");
      })
    };

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.repaired;
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      this.forbidden.OnSignal(this.allow, this.repaired);
      this.allowed.Enter((StateMachine<Repairable.States, Repairable.SMInstance, Repairable, object>.State.Callback) (smi => smi.master.CreateStorageProxy())).DefaultState(this.allowed.needMass).EventHandler(GameHashes.BuildingFullyRepaired, (StateMachine<Repairable.States, Repairable.SMInstance, Repairable, object>.State.Callback) (smi => smi.ConsumeRepairMaterials())).EventTransition(GameHashes.BuildingFullyRepaired, this.repaired).OnSignal(this.forbid, this.forbidden).Exit((StateMachine<Repairable.States, Repairable.SMInstance, Repairable, object>.State.Callback) (smi => smi.DestroyStorageProxy()));
      this.allowed.needMass.Enter((StateMachine<Repairable.States, Repairable.SMInstance, Repairable, object>.State.Callback) (smi => Prioritizable.AddRef(((Component) smi.master.storageProxy.transform.parent).gameObject))).Exit((StateMachine<Repairable.States, Repairable.SMInstance, Repairable, object>.State.Callback) (smi =>
      {
        if (smi.isMasterNull || !Object.op_Inequality((Object) smi.master.storageProxy, (Object) null))
          return;
        Prioritizable.RemoveRef(((Component) smi.master.storageProxy.transform.parent).gameObject);
      })).EventTransition(GameHashes.OnStorageChange, this.allowed.repairable, (StateMachine<Repairable.States, Repairable.SMInstance, Repairable, object>.Transition.ConditionCallback) (smi => smi.HasRequiredMass())).ToggleChore(new Func<Repairable.SMInstance, Chore>(this.CreateFetchChore), this.allowed.repairable, this.allowed.needMass).ToggleStatusItem(Db.Get().BuildingStatusItems.WaitingForRepairMaterials, (Func<Repairable.SMInstance, object>) (smi => (object) smi.GetRequiredMass()));
      this.allowed.repairable.ToggleRecurringChore(new Func<Repairable.SMInstance, Chore>(this.CreateRepairChore)).ToggleStatusItem(Db.Get().BuildingStatusItems.PendingRepair);
      this.repaired.EventTransition(GameHashes.BuildingReceivedDamage, (GameStateMachine<Repairable.States, Repairable.SMInstance, Repairable, object>.State) this.allowed, (StateMachine<Repairable.States, Repairable.SMInstance, Repairable, object>.Transition.ConditionCallback) (smi => smi.NeedsRepairs())).OnSignal(this.allow, (GameStateMachine<Repairable.States, Repairable.SMInstance, Repairable, object>.State) this.allowed).OnSignal(this.forbid, this.forbidden);
    }

    private Chore CreateFetchChore(Repairable.SMInstance smi)
    {
      PrimaryElement component = ((Component) smi.master).GetComponent<PrimaryElement>();
      PrimaryElement primaryElement = smi.master.storageProxy.FindPrimaryElement(component.ElementID);
      float amount = (float) ((double) component.Mass * 0.10000000149011612 - (Object.op_Inequality((Object) primaryElement, (Object) null) ? (double) primaryElement.Mass : 0.0));
      HashSet<Tag> tagSet = new HashSet<Tag>();
      tagSet.Add(GameTagExtensions.Create(component.ElementID));
      HashSet<Tag> tags = tagSet;
      return (Chore) new FetchChore(Db.Get().ChoreTypes.RepairFetch, smi.master.storageProxy, amount, tags, FetchChore.MatchCriteria.MatchID, Tag.Invalid, operational_requirement: Operational.State.None);
    }

    private Chore CreateRepairChore(Repairable.SMInstance smi)
    {
      WorkChore<Repairable> repairChore = new WorkChore<Repairable>(Db.Get().ChoreTypes.Repair, (IStateMachineTarget) smi.master, only_when_operational: false, ignore_building_assignment: true);
      Deconstructable component1 = ((Component) smi.master).GetComponent<Deconstructable>();
      if (Object.op_Inequality((Object) component1, (Object) null))
        repairChore.AddPrecondition(ChorePreconditions.instance.IsNotMarkedForDeconstruction, (object) component1);
      Breakable component2 = ((Component) smi.master).GetComponent<Breakable>();
      if (Object.op_Inequality((Object) component2, (Object) null))
        repairChore.AddPrecondition(Repairable.States.IsNotBeingAttacked, (object) component2);
      repairChore.AddPrecondition(Repairable.States.IsNotAngry);
      return (Chore) repairChore;
    }

    public class AllowedState : 
      GameStateMachine<Repairable.States, Repairable.SMInstance, Repairable, object>.State
    {
      public GameStateMachine<Repairable.States, Repairable.SMInstance, Repairable, object>.State needMass;
      public GameStateMachine<Repairable.States, Repairable.SMInstance, Repairable, object>.State repairable;
    }
  }
}
