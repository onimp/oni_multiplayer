// Decompiled with JetBrains decompiler
// Type: Workable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/scripts/Workable")]
public class Workable : KMonoBehaviour, ISaveLoadable, IApproachable
{
  public float workTime;
  public Vector3 AnimOffset = Vector3.zero;
  protected bool showProgressBar = true;
  public bool alwaysShowProgressBar;
  protected bool lightEfficiencyBonus = true;
  protected Guid lightEfficiencyBonusStatusItemHandle;
  public bool currentlyLit;
  public Tag laboratoryEfficiencyBonusTagRequired = RoomConstraints.ConstraintTags.ScienceBuilding;
  private bool useLaboratoryEfficiencyBonus;
  protected Guid laboratoryEfficiencyBonusStatusItemHandle;
  private bool currentlyInLaboratory;
  protected StatusItem workerStatusItem;
  protected StatusItem workingStatusItem;
  protected Guid workStatusItemHandle;
  protected OffsetTracker offsetTracker;
  [SerializeField]
  protected string attributeConverterId;
  protected AttributeConverter attributeConverter;
  protected float minimumAttributeMultiplier = 0.5f;
  public bool resetProgressOnStop;
  protected bool shouldTransferDiseaseWithWorker = true;
  [SerializeField]
  protected float attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
  [SerializeField]
  protected string skillExperienceSkillGroup;
  [SerializeField]
  protected float skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
  public bool triggerWorkReactions = true;
  public ReportManager.ReportType reportType = ReportManager.ReportType.WorkTime;
  [SerializeField]
  [Tooltip("What layer does the dupe switch to when interacting with the building")]
  public Grid.SceneLayer workLayer = Grid.SceneLayer.Move;
  [SerializeField]
  [Serialize]
  protected float workTimeRemaining = float.PositiveInfinity;
  [SerializeField]
  public KAnimFile[] overrideAnims;
  [SerializeField]
  protected HashedString multitoolContext;
  [SerializeField]
  protected Tag multitoolHitEffectTag;
  [SerializeField]
  [Tooltip("Whether to user the KAnimSynchronizer or not")]
  public bool synchronizeAnims = true;
  [SerializeField]
  [Tooltip("Whether to display number of uses in the details panel")]
  public bool trackUses;
  [Serialize]
  protected int numberOfUses;
  public Action<Workable, Workable.WorkableEvent> OnWorkableEventCB;
  private int skillsUpdateHandle = -1;
  private int minionUpdateHandle = -1;
  public string requiredSkillPerk;
  [SerializeField]
  protected bool shouldShowSkillPerkStatusItem = true;
  [SerializeField]
  public bool requireMinionToWork;
  protected StatusItem readyForSkillWorkStatusItem;
  public HashedString[] workAnims = new HashedString[2]
  {
    HashedString.op_Implicit("working_pre"),
    HashedString.op_Implicit("working_loop")
  };
  public HashedString[] workingPstComplete = new HashedString[1]
  {
    HashedString.op_Implicit("working_pst")
  };
  public HashedString[] workingPstFailed = new HashedString[1]
  {
    HashedString.op_Implicit("working_pst")
  };
  public KAnim.PlayMode workAnimPlayMode;
  public bool faceTargetWhenWorking;
  private static readonly EventSystem.IntraObjectHandler<Workable> OnUpdateRoomDelegate = new EventSystem.IntraObjectHandler<Workable>((Action<Workable, object>) ((component, data) => component.OnUpdateRoom(data)));
  protected ProgressBar progressBar;

  public Worker worker { get; protected set; }

  public float WorkTimeRemaining
  {
    get => this.workTimeRemaining;
    set => this.workTimeRemaining = value;
  }

  public bool preferUnreservedCell { get; set; }

  public virtual float GetWorkTime() => this.workTime;

  public Worker GetWorker() => this.worker;

  public virtual float GetPercentComplete() => (double) this.workTimeRemaining > (double) this.workTime ? -1f : (float) (1.0 - (double) this.workTimeRemaining / (double) this.workTime);

  public void ConfigureMultitoolContext(HashedString context, Tag hitEffectTag)
  {
    this.multitoolContext = context;
    this.multitoolHitEffectTag = hitEffectTag;
  }

  public virtual Workable.AnimInfo GetAnim(Worker worker)
  {
    Workable.AnimInfo anim = new Workable.AnimInfo();
    if (this.overrideAnims != null && this.overrideAnims.Length != 0)
    {
      BuildingFacade buildingFacade = this.GetBuildingFacade();
      bool flag = false;
      if (Object.op_Inequality((Object) buildingFacade, (Object) null) && !buildingFacade.IsOriginal)
        flag = buildingFacade.interactAnims.TryGetValue(((Object) this).name, out anim.overrideAnims);
      if (!flag)
        anim.overrideAnims = this.overrideAnims;
    }
    if (((HashedString) ref this.multitoolContext).IsValid && ((Tag) ref this.multitoolHitEffectTag).IsValid)
      anim.smi = (StateMachine.Instance) new MultitoolController.Instance(this, worker, this.multitoolContext, Assets.GetPrefab(this.multitoolHitEffectTag));
    return anim;
  }

  public virtual HashedString[] GetWorkAnims(Worker worker) => this.workAnims;

  public virtual KAnim.PlayMode GetWorkAnimPlayMode() => this.workAnimPlayMode;

  public virtual HashedString[] GetWorkPstAnims(Worker worker, bool successfully_completed) => successfully_completed ? this.workingPstComplete : this.workingPstFailed;

  public virtual Vector3 GetWorkOffset() => Vector3.zero;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.workerStatusItem = Db.Get().MiscStatusItems.Using;
    this.workingStatusItem = Db.Get().MiscStatusItems.Operating;
    this.readyForSkillWorkStatusItem = Db.Get().BuildingStatusItems.RequiresSkillPerk;
    this.workTime = this.GetWorkTime();
    this.workTimeRemaining = Mathf.Min(this.workTimeRemaining, this.workTime);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    if (this.shouldShowSkillPerkStatusItem && !string.IsNullOrEmpty(this.requiredSkillPerk))
    {
      if (this.skillsUpdateHandle != -1)
        Game.Instance.Unsubscribe(this.skillsUpdateHandle);
      this.skillsUpdateHandle = Game.Instance.Subscribe(-1523247426, new Action<object>(this.UpdateStatusItem));
    }
    if (this.requireMinionToWork && this.minionUpdateHandle != -1)
      Game.Instance.Unsubscribe(this.minionUpdateHandle);
    this.minionUpdateHandle = Game.Instance.Subscribe(586301400, new Action<object>(this.UpdateStatusItem));
    ((Component) this).GetComponent<KPrefabID>().AddTag(GameTags.HasChores, false);
    if (((Component) this).gameObject.HasTag(this.laboratoryEfficiencyBonusTagRequired))
    {
      this.useLaboratoryEfficiencyBonus = true;
      this.Subscribe<Workable>(144050788, Workable.OnUpdateRoomDelegate);
    }
    this.ShowProgressBar(this.alwaysShowProgressBar && (double) this.workTimeRemaining < (double) this.GetWorkTime());
    this.UpdateStatusItem();
  }

  private void RefreshRoom()
  {
    CavityInfo cavityForCell = Game.Instance.roomProber.GetCavityForCell(Grid.PosToCell(((Component) this).gameObject));
    if (cavityForCell != null && cavityForCell.room != null)
      this.OnUpdateRoom((object) cavityForCell.room);
    else
      this.OnUpdateRoom((object) null);
  }

  private void OnUpdateRoom(object data)
  {
    if (Object.op_Equality((Object) this.worker, (Object) null))
      return;
    Room room = (Room) data;
    if (room != null && room.roomType == Db.Get().RoomTypes.Laboratory)
    {
      this.currentlyInLaboratory = true;
      if (!(this.laboratoryEfficiencyBonusStatusItemHandle == Guid.Empty))
        return;
      this.laboratoryEfficiencyBonusStatusItemHandle = ((Component) this.worker).GetComponent<KSelectable>().AddStatusItem(Db.Get().DuplicantStatusItems.LaboratoryWorkEfficiencyBonus, (object) this);
    }
    else
    {
      this.currentlyInLaboratory = false;
      if (!(this.laboratoryEfficiencyBonusStatusItemHandle != Guid.Empty))
        return;
      this.laboratoryEfficiencyBonusStatusItemHandle = ((Component) this.worker).GetComponent<KSelectable>().RemoveStatusItem(this.laboratoryEfficiencyBonusStatusItemHandle);
    }
  }

  protected virtual void UpdateStatusItem(object data = null)
  {
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    if (Object.op_Equality((Object) component, (Object) null))
      return;
    component.RemoveStatusItem(this.workStatusItemHandle);
    if (Object.op_Equality((Object) this.worker, (Object) null))
    {
      if (this.requireMinionToWork && Components.LiveMinionIdentities.GetWorldItems(this.GetMyWorldId()).Count == 0)
      {
        this.workStatusItemHandle = component.AddStatusItem(Db.Get().BuildingStatusItems.WorkRequiresMinion);
      }
      else
      {
        if (!this.shouldShowSkillPerkStatusItem || string.IsNullOrEmpty(this.requiredSkillPerk))
          return;
        if (!MinionResume.AnyMinionHasPerk(this.requiredSkillPerk, this.GetMyWorldId()))
        {
          StatusItem status_item = DlcManager.FeatureClusterSpaceEnabled() ? Db.Get().BuildingStatusItems.ClusterColonyLacksRequiredSkillPerk : Db.Get().BuildingStatusItems.ColonyLacksRequiredSkillPerk;
          this.workStatusItemHandle = component.AddStatusItem(status_item, (object) this.requiredSkillPerk);
        }
        else
          this.workStatusItemHandle = component.AddStatusItem(this.readyForSkillWorkStatusItem, (object) this.requiredSkillPerk);
      }
    }
    else
    {
      if (this.workingStatusItem == null)
        return;
      this.workStatusItemHandle = component.AddStatusItem(this.workingStatusItem, (object) this);
    }
  }

  protected virtual void OnLoadLevel()
  {
    this.overrideAnims = (KAnimFile[]) null;
    base.OnLoadLevel();
  }

  public int GetCell() => Grid.PosToCell((KMonoBehaviour) this);

  public void StartWork(Worker worker_to_start)
  {
    Debug.Assert(Object.op_Inequality((Object) worker_to_start, (Object) null), (object) "How did we get a null worker?");
    this.worker = worker_to_start;
    this.UpdateStatusItem();
    if (this.showProgressBar)
      this.ShowProgressBar(true);
    if (this.useLaboratoryEfficiencyBonus)
      this.RefreshRoom();
    this.OnStartWork(this.worker);
    if (Object.op_Inequality((Object) this.worker, (Object) null))
    {
      string conversationTopic = this.GetConversationTopic();
      if (conversationTopic != null)
        this.worker.Trigger(937885943, (object) conversationTopic);
    }
    if (this.OnWorkableEventCB != null)
      this.OnWorkableEventCB(this, Workable.WorkableEvent.WorkStarted);
    ++this.numberOfUses;
    if (Object.op_Inequality((Object) this.worker, (Object) null))
    {
      if (Object.op_Inequality((Object) ((Component) this).gameObject.GetComponent<KSelectable>(), (Object) null) && ((Component) this).gameObject.GetComponent<KSelectable>().IsSelected && Object.op_Inequality((Object) ((Component) this.worker).gameObject.GetComponent<LoopingSounds>(), (Object) null))
        ((Component) this.worker).gameObject.GetComponent<LoopingSounds>().UpdateObjectSelection(true);
      else if (Object.op_Inequality((Object) ((Component) this.worker).gameObject.GetComponent<KSelectable>(), (Object) null) && ((Component) this.worker).gameObject.GetComponent<KSelectable>().IsSelected && Object.op_Inequality((Object) ((Component) this).gameObject.GetComponent<LoopingSounds>(), (Object) null))
        ((Component) this).gameObject.GetComponent<LoopingSounds>().UpdateObjectSelection(true);
    }
    EventExtensions.Trigger(((Component) this).gameObject, 853695848, (object) this);
  }

  public bool WorkTick(Worker worker, float dt)
  {
    bool flag = false;
    if ((double) dt > 0.0)
    {
      this.workTimeRemaining -= dt;
      flag = this.OnWorkTick(worker, dt);
    }
    return flag || (double) this.workTimeRemaining < 0.0;
  }

  public virtual float GetEfficiencyMultiplier(Worker worker)
  {
    float num = 1f;
    if (this.attributeConverter != null)
    {
      AttributeConverterInstance converter = ((Component) worker).GetComponent<AttributeConverters>().GetConverter(this.attributeConverter.Id);
      num += converter.Evaluate();
    }
    if (this.lightEfficiencyBonus)
    {
      int cell = Grid.PosToCell(((Component) worker).gameObject);
      if (Grid.IsValidCell(cell))
      {
        if (Grid.LightIntensity[cell] > 0)
        {
          this.currentlyLit = true;
          num += 0.15f;
          if (this.lightEfficiencyBonusStatusItemHandle == Guid.Empty)
            this.lightEfficiencyBonusStatusItemHandle = ((Component) worker).GetComponent<KSelectable>().AddStatusItem(Db.Get().DuplicantStatusItems.LightWorkEfficiencyBonus, (object) this);
        }
        else
        {
          this.currentlyLit = false;
          if (this.lightEfficiencyBonusStatusItemHandle != Guid.Empty)
            ((Component) worker).GetComponent<KSelectable>().RemoveStatusItem(this.lightEfficiencyBonusStatusItemHandle);
        }
      }
    }
    if (this.useLaboratoryEfficiencyBonus && this.currentlyInLaboratory)
      num += 0.1f;
    return Mathf.Max(num, this.minimumAttributeMultiplier);
  }

  public virtual Klei.AI.Attribute GetWorkAttribute() => this.attributeConverter != null ? this.attributeConverter.attribute : (Klei.AI.Attribute) null;

  public virtual string GetConversationTopic()
  {
    KPrefabID component = ((Component) this).GetComponent<KPrefabID>();
    return !component.HasTag(GameTags.NotConversationTopic) ? ((Tag) ref component.PrefabTag).Name : (string) null;
  }

  public float GetAttributeExperienceMultiplier() => this.attributeExperienceMultiplier;

  public string GetSkillExperienceSkillGroup() => this.skillExperienceSkillGroup;

  public float GetSkillExperienceMultiplier() => this.skillExperienceMultiplier;

  protected virtual bool OnWorkTick(Worker worker, float dt) => false;

  public void StopWork(Worker workerToStop, bool aborted)
  {
    if (Object.op_Equality((Object) this.worker, (Object) workerToStop) & aborted)
      this.OnAbortWork(workerToStop);
    if (this.shouldTransferDiseaseWithWorker)
      this.TransferDiseaseWithWorker(workerToStop);
    if (this.OnWorkableEventCB != null)
      this.OnWorkableEventCB(this, Workable.WorkableEvent.WorkStopped);
    this.OnStopWork(workerToStop);
    if (this.resetProgressOnStop)
      this.workTimeRemaining = this.GetWorkTime();
    this.ShowProgressBar(this.alwaysShowProgressBar && (double) this.workTimeRemaining < (double) this.GetWorkTime());
    if (this.lightEfficiencyBonusStatusItemHandle != Guid.Empty)
      this.lightEfficiencyBonusStatusItemHandle = ((Component) workerToStop).GetComponent<KSelectable>().RemoveStatusItem(this.lightEfficiencyBonusStatusItemHandle);
    if (this.laboratoryEfficiencyBonusStatusItemHandle != Guid.Empty)
      this.laboratoryEfficiencyBonusStatusItemHandle = ((Component) this.worker).GetComponent<KSelectable>().RemoveStatusItem(Db.Get().DuplicantStatusItems.LaboratoryWorkEfficiencyBonus);
    if (Object.op_Inequality((Object) ((Component) this).gameObject.GetComponent<KSelectable>(), (Object) null) && !((Component) this).gameObject.GetComponent<KSelectable>().IsSelected && Object.op_Inequality((Object) ((Component) this).gameObject.GetComponent<LoopingSounds>(), (Object) null))
      ((Component) this).gameObject.GetComponent<LoopingSounds>().UpdateObjectSelection(false);
    else if (Object.op_Inequality((Object) ((Component) workerToStop).gameObject.GetComponent<KSelectable>(), (Object) null) && !((Component) workerToStop).gameObject.GetComponent<KSelectable>().IsSelected && Object.op_Inequality((Object) ((Component) workerToStop).gameObject.GetComponent<LoopingSounds>(), (Object) null))
      ((Component) workerToStop).gameObject.GetComponent<LoopingSounds>().UpdateObjectSelection(false);
    this.worker = (Worker) null;
    EventExtensions.Trigger(((Component) this).gameObject, 679550494, (object) this);
    this.UpdateStatusItem();
  }

  public virtual StatusItem GetWorkerStatusItem() => this.workerStatusItem;

  public void SetWorkerStatusItem(StatusItem item) => this.workerStatusItem = item;

  public void CompleteWork(Worker worker)
  {
    if (this.shouldTransferDiseaseWithWorker)
      this.TransferDiseaseWithWorker(worker);
    this.OnCompleteWork(worker);
    if (this.OnWorkableEventCB != null)
      this.OnWorkableEventCB(this, Workable.WorkableEvent.WorkCompleted);
    this.workTimeRemaining = this.GetWorkTime();
    this.ShowProgressBar(false);
    EventExtensions.Trigger(((Component) this).gameObject, -2011693419, (object) this);
  }

  public void SetReportType(ReportManager.ReportType report_type) => this.reportType = report_type;

  public ReportManager.ReportType GetReportType() => this.reportType;

  protected virtual void OnStartWork(Worker worker)
  {
  }

  protected virtual void OnStopWork(Worker worker)
  {
  }

  protected virtual void OnCompleteWork(Worker worker)
  {
  }

  protected virtual void OnAbortWork(Worker worker)
  {
  }

  public virtual void OnPendingCompleteWork(Worker worker)
  {
  }

  public void SetOffsets(CellOffset[] offsets)
  {
    if (this.offsetTracker != null)
      this.offsetTracker.Clear();
    this.offsetTracker = (OffsetTracker) new StandardOffsetTracker(offsets);
  }

  public void SetOffsetTable(CellOffset[][] offset_table)
  {
    if (this.offsetTracker != null)
      this.offsetTracker.Clear();
    this.offsetTracker = (OffsetTracker) new OffsetTableTracker(offset_table, (KMonoBehaviour) this);
  }

  public virtual CellOffset[] GetOffsets(int cell)
  {
    if (this.offsetTracker == null)
      this.offsetTracker = (OffsetTracker) new StandardOffsetTracker(new CellOffset[1]);
    return this.offsetTracker.GetOffsets(cell);
  }

  public CellOffset[] GetOffsets() => this.GetOffsets(Grid.PosToCell((KMonoBehaviour) this));

  public void SetWorkTime(float work_time)
  {
    this.workTime = work_time;
    this.workTimeRemaining = work_time;
  }

  public bool ShouldFaceTargetWhenWorking() => this.faceTargetWhenWorking;

  public virtual Vector3 GetFacingTarget() => TransformExtensions.GetPosition(this.transform);

  public void ShowProgressBar(bool show)
  {
    if (show)
    {
      if (Object.op_Equality((Object) this.progressBar, (Object) null))
        this.progressBar = ProgressBar.CreateProgressBar(((Component) this).gameObject, new Func<float>(this.GetPercentComplete));
      ((Component) this.progressBar).gameObject.SetActive(true);
    }
    else
    {
      if (!Object.op_Inequality((Object) this.progressBar, (Object) null))
        return;
      TracesExtesions.DeleteObject(((Component) this.progressBar).gameObject);
      this.progressBar = (ProgressBar) null;
    }
  }

  protected virtual void OnCleanUp()
  {
    this.ShowProgressBar(false);
    if (this.offsetTracker != null)
      this.offsetTracker.Clear();
    if (this.skillsUpdateHandle != -1)
      Game.Instance.Unsubscribe(this.skillsUpdateHandle);
    if (this.minionUpdateHandle != -1)
      Game.Instance.Unsubscribe(this.minionUpdateHandle);
    base.OnCleanUp();
    this.OnWorkableEventCB = (Action<Workable, Workable.WorkableEvent>) null;
  }

  public virtual Vector3 GetTargetPoint()
  {
    Vector3 targetPoint = TransformExtensions.GetPosition(this.transform);
    float num = targetPoint.y + 0.65f;
    KBoxCollider2D component = ((Component) this).GetComponent<KBoxCollider2D>();
    if (Object.op_Inequality((Object) component, (Object) null))
    {
      Bounds bounds = component.bounds;
      targetPoint = ((Bounds) ref bounds).center;
    }
    targetPoint.y = num;
    targetPoint.z = 0.0f;
    return targetPoint;
  }

  public int GetNavigationCost(Navigator navigator, int cell) => navigator.GetNavigationCost(cell, this.GetOffsets(cell));

  public int GetNavigationCost(Navigator navigator) => this.GetNavigationCost(navigator, Grid.PosToCell((KMonoBehaviour) this));

  private void TransferDiseaseWithWorker(Worker worker)
  {
    if (Object.op_Equality((Object) this, (Object) null) || Object.op_Equality((Object) worker, (Object) null))
      return;
    Workable.TransferDiseaseWithWorker(((Component) this).gameObject, ((Component) worker).gameObject);
  }

  public static void TransferDiseaseWithWorker(GameObject workable, GameObject worker)
  {
    if (Object.op_Equality((Object) workable, (Object) null) || Object.op_Equality((Object) worker, (Object) null))
      return;
    PrimaryElement component1 = workable.GetComponent<PrimaryElement>();
    if (Object.op_Equality((Object) component1, (Object) null))
      return;
    PrimaryElement component2 = worker.GetComponent<PrimaryElement>();
    if (Object.op_Equality((Object) component2, (Object) null))
      return;
    SimUtil.DiseaseInfo invalid1 = SimUtil.DiseaseInfo.Invalid with
    {
      idx = component2.DiseaseIdx,
      count = (int) ((double) component2.DiseaseCount * 0.33000001311302185)
    };
    SimUtil.DiseaseInfo invalid2 = SimUtil.DiseaseInfo.Invalid with
    {
      idx = component1.DiseaseIdx,
      count = (int) ((double) component1.DiseaseCount * 0.33000001311302185)
    };
    component2.ModifyDiseaseCount(-invalid1.count, "Workable.TransferDiseaseWithWorker");
    component1.ModifyDiseaseCount(-invalid2.count, "Workable.TransferDiseaseWithWorker");
    if (invalid1.count > 0)
      component1.AddDisease(invalid1.idx, invalid1.count, "Workable.TransferDiseaseWithWorker");
    if (invalid2.count <= 0)
      return;
    component2.AddDisease(invalid2.idx, invalid2.count, "Workable.TransferDiseaseWithWorker");
  }

  public virtual bool InstantlyFinish(Worker worker)
  {
    float workTimeRemaining = worker.workable.WorkTimeRemaining;
    if (!float.IsInfinity(workTimeRemaining))
    {
      int num = (int) worker.Work(workTimeRemaining);
      return true;
    }
    DebugUtil.DevAssert(false, ((object) this).ToString() + " was asked to instantly finish but it has infinite work time! Override InstantlyFinish in your workable!", (Object) null);
    return false;
  }

  public virtual List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    if (this.trackUses)
    {
      Descriptor descriptor;
      // ISSUE: explicit constructor call
      ((Descriptor) ref descriptor).\u002Ector(string.Format((string) BUILDING.DETAILS.USE_COUNT, (object) this.numberOfUses), string.Format((string) BUILDING.DETAILS.USE_COUNT_TOOLTIP, (object) this.numberOfUses), (Descriptor.DescriptorType) 5, false);
      descriptors.Add(descriptor);
    }
    return descriptors;
  }

  public virtual BuildingFacade GetBuildingFacade() => ((Component) this).GetComponent<BuildingFacade>();

  [ContextMenu("Refresh Reachability")]
  public void RefreshReachability()
  {
    if (this.offsetTracker == null)
      return;
    this.offsetTracker.ForceRefresh();
  }

  public enum WorkableEvent
  {
    WorkStarted,
    WorkCompleted,
    WorkStopped,
  }

  public struct AnimInfo
  {
    public KAnimFile[] overrideAnims;
    public StateMachine.Instance smi;
  }
}
