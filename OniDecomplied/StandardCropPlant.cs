// Decompiled with JetBrains decompiler
// Type: StandardCropPlant
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StandardCropPlant : StateMachineComponent<StandardCropPlant.StatesInstance>
{
  private const int WILT_LEVELS = 3;
  [MyCmpReq]
  private Crop crop;
  [MyCmpReq]
  private WiltCondition wiltCondition;
  [MyCmpReq]
  private ReceptacleMonitor rm;
  [MyCmpReq]
  private Growing growing;
  [MyCmpReq]
  private KAnimControllerBase animController;
  [MyCmpGet]
  private Harvestable harvestable;
  public static StandardCropPlant.AnimSet defaultAnimSet = new StandardCropPlant.AnimSet()
  {
    grow = "grow",
    grow_pst = "grow_pst",
    idle_full = "idle_full",
    wilt_base = "wilt",
    harvest = "harvest",
    waning = "waning"
  };
  public StandardCropPlant.AnimSet anims = StandardCropPlant.defaultAnimSet;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
  }

  protected void DestroySelf(object callbackParam)
  {
    CreatureHelpers.DeselectCreature(((Component) this).gameObject);
    Util.KDestroyGameObject(((Component) this).gameObject);
  }

  public Notification CreateDeathNotification() => new Notification((string) CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION, NotificationType.Bad, (Func<List<Notification>, object, string>) ((notificationList, data) => (string) CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION_TOOLTIP + notificationList.ReduceMessages(false)), (object) ("/t• " + ((Component) this).gameObject.GetProperName()));

  public void RefreshPositionPercent() => this.animController.SetPositionPercent(this.growing.PercentOfCurrentHarvest());

  private static string ToolTipResolver(List<Notification> notificationList, object data)
  {
    string str = "";
    for (int index = 0; index < notificationList.Count; ++index)
    {
      Notification notification = notificationList[index];
      str += (string) notification.tooltipData;
      if (index < notificationList.Count - 1)
        str += "\n";
    }
    return string.Format((string) CREATURES.STATUSITEMS.PLANTDEATH.NOTIFICATION_TOOLTIP, (object) str);
  }

  public class AnimSet
  {
    public string grow;
    public string grow_pst;
    public string idle_full;
    public string wilt_base;
    public string harvest;
    public string waning;
    private string[] m_wilt;

    public string GetWiltLevel(int level)
    {
      if (this.m_wilt == null)
      {
        this.m_wilt = new string[3];
        for (int index = 0; index < 3; ++index)
          this.m_wilt[index] = this.wilt_base + (index + 1).ToString();
      }
      return this.m_wilt[level - 1];
    }
  }

  public class States : 
    GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant>
  {
    public StandardCropPlant.States.AliveStates alive;
    public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State dead;
    public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.PlantAliveSubState blighted;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      default_state = (StateMachine.BaseState) this.alive;
      this.dead.ToggleMainStatusItem(Db.Get().CreatureStatusItems.Dead).Enter((StateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State.Callback) (smi =>
      {
        if (smi.master.rm.Replanted && !((Component) smi.master).GetComponent<KPrefabID>().HasTag(GameTags.Uprooted))
          ((Component) smi.master).gameObject.AddOrGet<Notifier>().Add(smi.master.CreateDeathNotification());
        GameUtil.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(EffectConfigs.PlantDeathId)), TransformExtensions.GetPosition(smi.master.transform), Grid.SceneLayer.FXFront).SetActive(true);
        Harvestable component = ((Component) smi.master).GetComponent<Harvestable>();
        if (Object.op_Inequality((Object) component, (Object) null) && component.CanBeHarvested && Object.op_Inequality((Object) GameScheduler.Instance, (Object) null))
          GameScheduler.Instance.Schedule("SpawnFruit", 0.2f, new Action<object>(smi.master.crop.SpawnConfiguredFruit), (object) null, (SchedulerGroup) null);
        smi.master.Trigger(1623392196, (object) null);
        ((Component) smi.master).GetComponent<KBatchedAnimController>().StopAndClear();
        Object.Destroy((Object) ((Component) smi.master).GetComponent<KBatchedAnimController>());
        smi.Schedule(0.5f, new Action<object>(smi.master.DestroySelf), (object) null);
      }));
      this.blighted.InitializeStates(this.masterTarget, this.dead).PlayAnim((Func<StandardCropPlant.StatesInstance, string>) (smi => smi.master.anims.waning)).ToggleMainStatusItem(Db.Get().CreatureStatusItems.Crop_Blighted).TagTransition(GameTags.Blighted, (GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State) this.alive, true);
      this.alive.InitializeStates(this.masterTarget, this.dead).DefaultState(this.alive.idle).ToggleComponent<Growing>().TagTransition(GameTags.Blighted, (GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State) this.blighted);
      this.alive.idle.EventTransition(GameHashes.Wilt, this.alive.wilting, (StateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.Transition.ConditionCallback) (smi => smi.master.wiltCondition.IsWilting())).EventTransition(GameHashes.Grow, this.alive.pre_fruiting, (StateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.Transition.ConditionCallback) (smi => smi.master.growing.ReachedNextHarvest())).EventTransition(GameHashes.CropSleep, this.alive.sleeping, new StateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.Transition.ConditionCallback(this.IsSleeping)).PlayAnim((Func<StandardCropPlant.StatesInstance, string>) (smi => smi.master.anims.grow), (KAnim.PlayMode) 2).Enter(new StateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State.Callback(StandardCropPlant.States.RefreshPositionPercent)).Update(new Action<StandardCropPlant.StatesInstance, float>(StandardCropPlant.States.RefreshPositionPercent), (UpdateRate) 7).EventHandler(GameHashes.ConsumePlant, new StateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State.Callback(StandardCropPlant.States.RefreshPositionPercent));
      this.alive.pre_fruiting.PlayAnim((Func<StandardCropPlant.StatesInstance, string>) (smi => smi.master.anims.grow_pst)).TriggerOnEnter(GameHashes.BurstEmitDisease).EventTransition(GameHashes.AnimQueueComplete, (GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State) this.alive.fruiting);
      this.alive.fruiting_lost.Enter((StateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State.Callback) (smi =>
      {
        if (!Object.op_Inequality((Object) smi.master.harvestable, (Object) null))
          return;
        smi.master.harvestable.SetCanBeHarvested(false);
      })).GoTo(this.alive.idle);
      this.alive.wilting.PlayAnim(new Func<StandardCropPlant.StatesInstance, string>(StandardCropPlant.States.GetWiltAnim), (KAnim.PlayMode) 0).EventTransition(GameHashes.WiltRecover, this.alive.idle, (StateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.Transition.ConditionCallback) (smi => !smi.master.wiltCondition.IsWilting())).EventTransition(GameHashes.Harvest, this.alive.harvest);
      this.alive.sleeping.PlayAnim((Func<StandardCropPlant.StatesInstance, string>) (smi => smi.master.anims.grow)).EventTransition(GameHashes.CropWakeUp, this.alive.idle, GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.Not(new StateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.Transition.ConditionCallback(this.IsSleeping))).EventTransition(GameHashes.Harvest, this.alive.harvest).EventTransition(GameHashes.Wilt, this.alive.wilting);
      this.alive.fruiting.DefaultState(this.alive.fruiting.fruiting_idle).EventTransition(GameHashes.Wilt, this.alive.wilting).EventTransition(GameHashes.Harvest, this.alive.harvest).EventTransition(GameHashes.Grow, this.alive.fruiting_lost, (StateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.Transition.ConditionCallback) (smi => !smi.master.growing.ReachedNextHarvest()));
      this.alive.fruiting.fruiting_idle.PlayAnim((Func<StandardCropPlant.StatesInstance, string>) (smi => smi.master.anims.idle_full), (KAnim.PlayMode) 0).Enter((StateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State.Callback) (smi =>
      {
        if (!Object.op_Inequality((Object) smi.master.harvestable, (Object) null))
          return;
        smi.master.harvestable.SetCanBeHarvested(true);
      })).Transition(this.alive.fruiting.fruiting_old, new StateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.Transition.ConditionCallback(this.IsOld), (UpdateRate) 7);
      this.alive.fruiting.fruiting_old.PlayAnim(new Func<StandardCropPlant.StatesInstance, string>(StandardCropPlant.States.GetWiltAnim), (KAnim.PlayMode) 0).Enter((StateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State.Callback) (smi =>
      {
        if (!Object.op_Inequality((Object) smi.master.harvestable, (Object) null))
          return;
        smi.master.harvestable.SetCanBeHarvested(true);
      })).Transition(this.alive.fruiting.fruiting_idle, GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.Not(new StateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.Transition.ConditionCallback(this.IsOld)), (UpdateRate) 7);
      this.alive.harvest.PlayAnim((Func<StandardCropPlant.StatesInstance, string>) (smi => smi.master.anims.harvest)).Enter((StateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State.Callback) (smi =>
      {
        if (Object.op_Inequality((Object) GameScheduler.Instance, (Object) null) && Object.op_Inequality((Object) smi.master, (Object) null))
          GameScheduler.Instance.Schedule("SpawnFruit", 0.2f, new Action<object>(smi.master.crop.SpawnConfiguredFruit), (object) null, (SchedulerGroup) null);
        if (!Object.op_Inequality((Object) smi.master.harvestable, (Object) null))
          return;
        smi.master.harvestable.SetCanBeHarvested(false);
      })).Exit((StateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State.Callback) (smi => smi.Trigger(113170146))).OnAnimQueueComplete(this.alive.idle);
    }

    private static string GetWiltAnim(StandardCropPlant.StatesInstance smi)
    {
      float num = smi.master.growing.PercentOfCurrentHarvest();
      int level = (double) num >= 0.75 ? ((double) num >= 1.0 ? 3 : 2) : 1;
      return smi.master.anims.GetWiltLevel(level);
    }

    private static void RefreshPositionPercent(StandardCropPlant.StatesInstance smi, float dt) => smi.master.RefreshPositionPercent();

    private static void RefreshPositionPercent(StandardCropPlant.StatesInstance smi) => smi.master.RefreshPositionPercent();

    public bool IsOld(StandardCropPlant.StatesInstance smi) => (double) smi.master.growing.PercentOldAge() > 0.5;

    public bool IsSleeping(StandardCropPlant.StatesInstance smi)
    {
      CropSleepingMonitor.Instance smi1 = ((Component) smi.master).GetSMI<CropSleepingMonitor.Instance>();
      return smi1 != null && smi1.IsSleeping();
    }

    public class AliveStates : 
      GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.PlantAliveSubState
    {
      public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State idle;
      public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State pre_fruiting;
      public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State fruiting_lost;
      public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State barren;
      public StandardCropPlant.States.FruitingState fruiting;
      public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State wilting;
      public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State destroy;
      public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State harvest;
      public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State sleeping;
    }

    public class FruitingState : 
      GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State
    {
      public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State fruiting_idle;
      public GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.State fruiting_old;
    }
  }

  public class StatesInstance : 
    GameStateMachine<StandardCropPlant.States, StandardCropPlant.StatesInstance, StandardCropPlant, object>.GameInstance
  {
    public StatesInstance(StandardCropPlant master)
      : base(master)
    {
    }
  }
}
