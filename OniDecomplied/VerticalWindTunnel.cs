// Decompiled with JetBrains decompiler
// Type: VerticalWindTunnel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class VerticalWindTunnel : 
  StateMachineComponent<VerticalWindTunnel.StatesInstance>,
  IGameObjectEffectDescriptor,
  ISim200ms
{
  public string specificEffect;
  public string trackingEffect;
  public int basePriority;
  public float displacementAmount_DescriptorOnly;
  public static Operational.Flag validIntakeFlag = new Operational.Flag("valid_intake", Operational.Flag.Type.Requirement);
  private bool invalidIntake;
  private float avgGasAccumTop;
  private float avgGasAccumBottom;
  private int avgGasCounter;
  public CellOffset[] choreOffsets = new CellOffset[3]
  {
    new CellOffset(0, 0),
    new CellOffset(-1, 0),
    new CellOffset(1, 0)
  };
  private VerticalWindTunnelWorkable[] workables;
  private Chore[] chores;
  private ElementConsumer bottomConsumer;
  private ElementConsumer topConsumer;
  private Operational operational;
  public HashSet<int> players = new HashSet<int>();
  public HashedString[] overrideAnims = new HashedString[3]
  {
    HashedString.op_Implicit("anim_interacts_windtunnel_center_kanim"),
    HashedString.op_Implicit("anim_interacts_windtunnel_left_kanim"),
    HashedString.op_Implicit("anim_interacts_windtunnel_right_kanim")
  };
  public string[][] workPreAnims = new string[3][]
  {
    new string[2]
    {
      "weak_working_front_pre",
      "weak_working_back_pre"
    },
    new string[2]
    {
      "medium_working_front_pre",
      "medium_working_back_pre"
    },
    new string[2]
    {
      "strong_working_front_pre",
      "strong_working_back_pre"
    }
  };
  public string[] workAnims = new string[3]
  {
    "weak_working_loop",
    "medium_working_loop",
    "strong_working_loop"
  };
  public string[][] workPstAnims = new string[3][]
  {
    new string[2]
    {
      "weak_working_back_pst",
      "weak_working_front_pst"
    },
    new string[2]
    {
      "medium_working_back_pst",
      "medium_working_front_pst"
    },
    new string[2]
    {
      "strong_working_back_pst",
      "strong_working_front_pst"
    }
  };

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    ElementConsumer[] components = ((Component) this).GetComponents<ElementConsumer>();
    this.bottomConsumer = components[0];
    this.bottomConsumer.EnableConsumption(false);
    this.bottomConsumer.OnElementConsumed += (Action<Sim.ConsumedMassInfo>) (info => this.OnElementConsumed(false, info));
    this.topConsumer = components[1];
    this.topConsumer.EnableConsumption(false);
    this.topConsumer.OnElementConsumed += (Action<Sim.ConsumedMassInfo>) (info => this.OnElementConsumed(true, info));
    this.operational = ((Component) this).GetComponent<Operational>();
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.invalidIntake = this.HasInvalidIntake();
    ((Component) this).GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.WindTunnelIntake, this.invalidIntake, (object) this);
    this.operational.SetFlag(VerticalWindTunnel.validIntakeFlag, !this.invalidIntake);
    GameScheduler.Instance.Schedule("Scheduling Tutorial", 2f, (Action<object>) (obj => Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Schedule)), (object) null, (SchedulerGroup) null);
    this.workables = new VerticalWindTunnelWorkable[this.choreOffsets.Length];
    this.chores = new Chore[this.choreOffsets.Length];
    for (int index = 0; index < this.workables.Length; ++index)
    {
      GameObject locator = ChoreHelpers.CreateLocator("VerticalWindTunnelWorkable", Grid.CellToPosCBC(Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) this), this.choreOffsets[index]), Grid.SceneLayer.Move));
      KSelectable kselectable = locator.AddOrGet<KSelectable>();
      kselectable.SetName(((Component) this).GetProperName());
      kselectable.IsSelectable = false;
      VerticalWindTunnelWorkable windTunnelWorkable1 = locator.AddOrGet<VerticalWindTunnelWorkable>();
      int player_index = index;
      VerticalWindTunnelWorkable windTunnelWorkable2 = windTunnelWorkable1;
      windTunnelWorkable2.OnWorkableEventCB = windTunnelWorkable2.OnWorkableEventCB + (Action<Workable, Workable.WorkableEvent>) ((workable, ev) => this.OnWorkableEvent(player_index, ev));
      windTunnelWorkable1.overrideAnim = this.overrideAnims[index];
      windTunnelWorkable1.preAnims = this.workPreAnims[index];
      windTunnelWorkable1.loopAnim = this.workAnims[index];
      windTunnelWorkable1.pstAnims = this.workPstAnims[index];
      this.workables[index] = windTunnelWorkable1;
      this.workables[index].windTunnel = this;
    }
    this.smi.StartSM();
  }

  protected override void OnCleanUp()
  {
    this.UpdateChores(false);
    for (int index = 0; index < this.workables.Length; ++index)
    {
      if (Object.op_Implicit((Object) this.workables[index]))
      {
        Util.KDestroyGameObject((Component) this.workables[index]);
        this.workables[index] = (VerticalWindTunnelWorkable) null;
      }
    }
    base.OnCleanUp();
  }

  private Chore CreateChore(int i)
  {
    Workable workable = (Workable) this.workables[i];
    ChoreType relax = Db.Get().ChoreTypes.Relax;
    Workable target = workable;
    ScheduleBlockType recreation = Db.Get().ScheduleBlockTypes.Recreation;
    Action<Chore> on_end = new Action<Chore>(this.OnSocialChoreEnd);
    ScheduleBlockType schedule_block = recreation;
    WorkChore<VerticalWindTunnelWorkable> chore = new WorkChore<VerticalWindTunnelWorkable>(relax, (IStateMachineTarget) target, on_end: on_end, allow_in_red_alert: false, schedule_block: schedule_block, allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.high);
    chore.AddPrecondition(ChorePreconditions.instance.CanDoWorkerPrioritizable, (object) workable);
    return (Chore) chore;
  }

  private void OnSocialChoreEnd(Chore chore)
  {
    if (!((Component) this).gameObject.HasTag(GameTags.Operational))
      return;
    this.UpdateChores();
  }

  public void UpdateChores(bool update = true)
  {
    for (int i = 0; i < this.choreOffsets.Length; ++i)
    {
      Chore chore = this.chores[i];
      if (update)
      {
        if (chore == null || chore.isComplete)
          this.chores[i] = this.CreateChore(i);
      }
      else if (chore != null)
      {
        chore.Cancel("locator invalidated");
        this.chores[i] = (Chore) null;
      }
    }
  }

  public void Sim200ms(float dt)
  {
    bool flag = this.HasInvalidIntake();
    if (flag == this.invalidIntake)
      return;
    this.invalidIntake = flag;
    ((Component) this).GetComponent<KSelectable>().ToggleStatusItem(Db.Get().BuildingStatusItems.WindTunnelIntake, this.invalidIntake, (object) this);
    this.operational.SetFlag(VerticalWindTunnel.validIntakeFlag, !this.invalidIntake);
  }

  private float GetIntakeRatio(int fromCell, int radius)
  {
    float num1 = 0.0f;
    float num2 = 0.0f;
    for (int y = -radius; y < radius; ++y)
    {
      for (int x = -radius; x < radius; ++x)
      {
        int cell = Grid.OffsetCell(fromCell, x, y);
        if (!Grid.IsSolidCell(cell))
        {
          if (Grid.IsGas(cell))
            ++num2;
          ++num1;
        }
      }
    }
    return num2 / num1;
  }

  private bool HasInvalidIntake()
  {
    Vector3 position = TransformExtensions.GetPosition(this.transform);
    int cell = Grid.XYToCell((int) position.x, (int) position.y);
    int fromCell1 = Grid.OffsetCell(cell, (int) this.topConsumer.sampleCellOffset.x, (int) this.topConsumer.sampleCellOffset.y);
    int fromCell2 = Grid.OffsetCell(cell, (int) this.bottomConsumer.sampleCellOffset.x, (int) this.bottomConsumer.sampleCellOffset.y);
    this.avgGasAccumTop += this.GetIntakeRatio(fromCell1, (int) this.topConsumer.consumptionRadius);
    this.avgGasAccumBottom += this.GetIntakeRatio(fromCell2, (int) this.bottomConsumer.consumptionRadius);
    int num1 = 5;
    this.avgGasCounter = (this.avgGasCounter + 1) % num1;
    if (this.avgGasCounter != 0)
      return this.invalidIntake;
    double num2 = (double) this.avgGasAccumTop / (double) num1;
    float num3 = this.avgGasAccumBottom / (float) num1;
    this.avgGasAccumBottom = 0.0f;
    this.avgGasAccumTop = 0.0f;
    return num2 < 0.5 || (double) num3 < 0.5;
  }

  public void SetGasWalls(bool set)
  {
    Building component = ((Component) this).GetComponent<Building>();
    Sim.Cell.Properties properties = Sim.Cell.Properties.GasImpermeable | Sim.Cell.Properties.LiquidImpermeable;
    Vector3 position = TransformExtensions.GetPosition(this.transform);
    for (int index = 0; index < component.Def.HeightInCells; ++index)
    {
      int cell1 = Grid.XYToCell(Mathf.FloorToInt(position.x) - 2, Mathf.FloorToInt(position.y) + index);
      int cell2 = Grid.XYToCell(Mathf.FloorToInt(position.x) + 2, Mathf.FloorToInt(position.y) + index);
      if (set)
      {
        SimMessages.SetCellProperties(cell1, (byte) properties);
        SimMessages.SetCellProperties(cell2, (byte) properties);
      }
      else
      {
        SimMessages.ClearCellProperties(cell1, (byte) properties);
        SimMessages.ClearCellProperties(cell2, (byte) properties);
      }
    }
  }

  private void OnElementConsumed(bool isTop, Sim.ConsumedMassInfo info)
  {
    Building component = ((Component) this).GetComponent<Building>();
    Vector3 position = TransformExtensions.GetPosition(this.transform);
    CellOffset offset = isTop ? new CellOffset(0, component.Def.HeightInCells + 1) : new CellOffset(0, 0);
    SimMessages.AddRemoveSubstance(Grid.OffsetCell(Grid.XYToCell((int) position.x, (int) position.y), offset), info.removedElemIdx, CellEventLogger.Instance.ElementEmitted, info.mass, info.temperature, info.diseaseIdx, info.diseaseCount);
  }

  public void OnWorkableEvent(int player, Workable.WorkableEvent ev)
  {
    if (ev == Workable.WorkableEvent.WorkStarted)
      this.players.Add(player);
    else
      this.players.Remove(player);
    this.smi.sm.playerCount.Set(this.players.Count, this.smi);
  }

  List<Descriptor> IGameObjectEffectDescriptor.GetDescriptors(GameObject go)
  {
    List<Descriptor> descs = new List<Descriptor>();
    descs.Add(new Descriptor(BUILDINGS.PREFABS.VERTICALWINDTUNNEL.DISPLACEMENTEFFECT.Replace("{amount}", GameUtil.GetFormattedMass(this.displacementAmount_DescriptorOnly, GameUtil.TimeSlice.PerSecond)), BUILDINGS.PREFABS.VERTICALWINDTUNNEL.DISPLACEMENTEFFECT_TOOLTIP.Replace("{amount}", GameUtil.GetFormattedMass(this.displacementAmount_DescriptorOnly, GameUtil.TimeSlice.PerSecond)), (Descriptor.DescriptorType) 0, false));
    descs.Add(new Descriptor((string) UI.BUILDINGEFFECTS.RECREATION, (string) UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION, (Descriptor.DescriptorType) 1, false));
    Effect.AddModifierDescriptions(((Component) this).gameObject, descs, this.specificEffect, true);
    return descs;
  }

  public class States : 
    GameStateMachine<VerticalWindTunnel.States, VerticalWindTunnel.StatesInstance, VerticalWindTunnel>
  {
    public StateMachine<VerticalWindTunnel.States, VerticalWindTunnel.StatesInstance, VerticalWindTunnel, object>.IntParameter playerCount;
    public GameStateMachine<VerticalWindTunnel.States, VerticalWindTunnel.StatesInstance, VerticalWindTunnel, object>.State unoperational;
    public VerticalWindTunnel.States.OperationalStates operational;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.unoperational;
      this.unoperational.Enter((StateMachine<VerticalWindTunnel.States, VerticalWindTunnel.StatesInstance, VerticalWindTunnel, object>.State.Callback) (smi => smi.SetActive(false))).TagTransition(GameTags.Operational, (GameStateMachine<VerticalWindTunnel.States, VerticalWindTunnel.StatesInstance, VerticalWindTunnel, object>.State) this.operational).PlayAnim("off");
      this.operational.TagTransition(GameTags.Operational, this.unoperational, true).Enter("CreateChore", (StateMachine<VerticalWindTunnel.States, VerticalWindTunnel.StatesInstance, VerticalWindTunnel, object>.State.Callback) (smi => smi.master.UpdateChores())).Exit("CancelChore", (StateMachine<VerticalWindTunnel.States, VerticalWindTunnel.StatesInstance, VerticalWindTunnel, object>.State.Callback) (smi => smi.master.UpdateChores(false))).DefaultState(this.operational.stopped);
      this.operational.stopped.PlayAnim("off").ParamTransition<int>((StateMachine<VerticalWindTunnel.States, VerticalWindTunnel.StatesInstance, VerticalWindTunnel, object>.Parameter<int>) this.playerCount, this.operational.pre, (StateMachine<VerticalWindTunnel.States, VerticalWindTunnel.StatesInstance, VerticalWindTunnel, object>.Parameter<int>.Callback) ((smi, p) => p > 0));
      this.operational.pre.PlayAnim("working_pre").OnAnimQueueComplete(this.operational.playing);
      this.operational.playing.PlayAnim("working_loop", (KAnim.PlayMode) 0).Enter((StateMachine<VerticalWindTunnel.States, VerticalWindTunnel.StatesInstance, VerticalWindTunnel, object>.State.Callback) (smi => smi.SetActive(true))).Exit((StateMachine<VerticalWindTunnel.States, VerticalWindTunnel.StatesInstance, VerticalWindTunnel, object>.State.Callback) (smi => smi.SetActive(false))).ParamTransition<int>((StateMachine<VerticalWindTunnel.States, VerticalWindTunnel.StatesInstance, VerticalWindTunnel, object>.Parameter<int>) this.playerCount, this.operational.post, (StateMachine<VerticalWindTunnel.States, VerticalWindTunnel.StatesInstance, VerticalWindTunnel, object>.Parameter<int>.Callback) ((smi, p) => p == 0)).Enter("GasWalls", (StateMachine<VerticalWindTunnel.States, VerticalWindTunnel.StatesInstance, VerticalWindTunnel, object>.State.Callback) (smi => smi.master.SetGasWalls(true))).Exit("GasWalls", (StateMachine<VerticalWindTunnel.States, VerticalWindTunnel.StatesInstance, VerticalWindTunnel, object>.State.Callback) (smi => smi.master.SetGasWalls(false)));
      this.operational.post.PlayAnim("working_pst").QueueAnim("off_pre").OnAnimQueueComplete(this.operational.stopped);
    }

    public class OperationalStates : 
      GameStateMachine<VerticalWindTunnel.States, VerticalWindTunnel.StatesInstance, VerticalWindTunnel, object>.State
    {
      public GameStateMachine<VerticalWindTunnel.States, VerticalWindTunnel.StatesInstance, VerticalWindTunnel, object>.State stopped;
      public GameStateMachine<VerticalWindTunnel.States, VerticalWindTunnel.StatesInstance, VerticalWindTunnel, object>.State pre;
      public GameStateMachine<VerticalWindTunnel.States, VerticalWindTunnel.StatesInstance, VerticalWindTunnel, object>.State playing;
      public GameStateMachine<VerticalWindTunnel.States, VerticalWindTunnel.StatesInstance, VerticalWindTunnel, object>.State post;
    }
  }

  public class StatesInstance : 
    GameStateMachine<VerticalWindTunnel.States, VerticalWindTunnel.StatesInstance, VerticalWindTunnel, object>.GameInstance
  {
    private Operational operational;

    public StatesInstance(VerticalWindTunnel smi)
      : base(smi)
    {
      this.operational = ((Component) this.master).GetComponent<Operational>();
    }

    public void SetActive(bool active) => this.operational.SetActive(this.operational.IsOperational & active);
  }
}
