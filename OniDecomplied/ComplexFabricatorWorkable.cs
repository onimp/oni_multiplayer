// Decompiled with JetBrains decompiler
// Type: ComplexFabricatorWorkable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using TUNING;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/ComplexFabricatorWorkable")]
public class ComplexFabricatorWorkable : Workable
{
  [MyCmpReq]
  private Operational operational;
  [MyCmpReq]
  private ComplexFabricator fabricator;
  public Action<Worker, float> OnWorkTickActions;
  public MeterController meter;
  protected GameObject visualizer;
  protected KAnimLink visualizerLink;

  public StatusItem WorkerStatusItem
  {
    get => this.workerStatusItem;
    set => this.workerStatusItem = value;
  }

  public AttributeConverter AttributeConverter
  {
    get => this.attributeConverter;
    set => this.attributeConverter = value;
  }

  public float AttributeExperienceMultiplier
  {
    get => this.attributeExperienceMultiplier;
    set => this.attributeExperienceMultiplier = value;
  }

  public string SkillExperienceSkillGroup
  {
    set => this.skillExperienceSkillGroup = value;
  }

  public float SkillExperienceMultiplier
  {
    set => this.skillExperienceMultiplier = value;
  }

  public ComplexRecipe CurrentWorkingOrder => !Object.op_Inequality((Object) this.fabricator, (Object) null) ? (ComplexRecipe) null : this.fabricator.CurrentWorkingOrder;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.workerStatusItem = Db.Get().DuplicantStatusItems.Fabricating;
    this.attributeConverter = Db.Get().AttributeConverters.MachinerySpeed;
    this.attributeExperienceMultiplier = DUPLICANTSTATS.ATTRIBUTE_LEVELING.PART_DAY_EXPERIENCE;
    this.skillExperienceSkillGroup = Db.Get().SkillGroups.Technicals.Id;
    this.skillExperienceMultiplier = SKILLS.PART_DAY_EXPERIENCE;
  }

  public override string GetConversationTopic() => this.fabricator.GetConversationTopic() ?? base.GetConversationTopic();

  protected override void OnStartWork(Worker worker)
  {
    base.OnStartWork(worker);
    if (!this.operational.IsOperational)
      return;
    if (this.fabricator.CurrentWorkingOrder != null)
      this.InstantiateVisualizer(this.fabricator.CurrentWorkingOrder);
    else
      DebugUtil.DevAssertArgs(false, new object[2]
      {
        (object) "ComplexFabricatorWorkable.OnStartWork called but CurrentMachineOrder is null",
        (object) ((Component) this).gameObject
      });
  }

  protected override bool OnWorkTick(Worker worker, float dt)
  {
    if (this.OnWorkTickActions != null)
      this.OnWorkTickActions(worker, dt);
    this.UpdateOrderProgress(worker, dt);
    return base.OnWorkTick(worker, dt);
  }

  public override float GetWorkTime()
  {
    ComplexRecipe currentWorkingOrder = this.fabricator.CurrentWorkingOrder;
    if (currentWorkingOrder == null)
      return -1f;
    this.workTime = currentWorkingOrder.time;
    return this.workTime;
  }

  public Chore CreateWorkChore(ChoreType choreType, float order_progress)
  {
    WorkChore<ComplexFabricatorWorkable> workChore = new WorkChore<ComplexFabricatorWorkable>(choreType, (IStateMachineTarget) this);
    this.workTimeRemaining = this.GetWorkTime() * (1f - order_progress);
    return (Chore) workChore;
  }

  protected override void OnCompleteWork(Worker worker)
  {
    base.OnCompleteWork(worker);
    this.fabricator.CompleteWorkingOrder();
    this.DestroyVisualizer();
  }

  private void InstantiateVisualizer(ComplexRecipe recipe)
  {
    if (Object.op_Inequality((Object) this.visualizer, (Object) null))
      this.DestroyVisualizer();
    if (this.visualizerLink != null)
    {
      this.visualizerLink.Unregister();
      this.visualizerLink = (KAnimLink) null;
    }
    if (Object.op_Equality((Object) recipe.FabricationVisualizer, (Object) null))
      return;
    this.visualizer = Util.KInstantiate(recipe.FabricationVisualizer, (GameObject) null, (string) null);
    this.visualizer.transform.parent = ((Component) this.meter.meterController).transform;
    TransformExtensions.SetLocalPosition(this.visualizer.transform, new Vector3(0.0f, 0.0f, 1f));
    this.visualizer.SetActive(true);
    this.visualizerLink = new KAnimLink((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), (KAnimControllerBase) this.visualizer.GetComponent<KBatchedAnimController>());
  }

  private void UpdateOrderProgress(Worker worker, float dt)
  {
    float workTime = this.GetWorkTime();
    float percent_full = Mathf.Clamp01((workTime - this.WorkTimeRemaining) / workTime);
    if (Object.op_Implicit((Object) this.fabricator))
      this.fabricator.OrderProgress = percent_full;
    if (this.meter == null)
      return;
    this.meter.SetPositionPercent(percent_full);
  }

  private void DestroyVisualizer()
  {
    if (!Object.op_Inequality((Object) this.visualizer, (Object) null))
      return;
    if (this.visualizerLink != null)
    {
      this.visualizerLink.Unregister();
      this.visualizerLink = (KAnimLink) null;
    }
    Util.KDestroyGameObject(this.visualizer);
    this.visualizer = (GameObject) null;
  }
}
