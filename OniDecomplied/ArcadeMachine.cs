// Decompiled with JetBrains decompiler
// Type: ArcadeMachine
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
public class ArcadeMachine : 
  StateMachineComponent<ArcadeMachine.StatesInstance>,
  IGameObjectEffectDescriptor
{
  public CellOffset[] choreOffsets = new CellOffset[2]
  {
    new CellOffset(-1, 0),
    new CellOffset(1, 0)
  };
  private ArcadeMachineWorkable[] workables;
  private Chore[] chores;
  public HashSet<int> players = new HashSet<int>();
  public KAnimFile[][] overrideAnims = new KAnimFile[2][]
  {
    new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_arcade_cabinet_playerone_kanim"))
    },
    new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_interacts_arcade_cabinet_playertwo_kanim"))
    }
  };
  public HashedString[][] workAnims = new HashedString[2][]
  {
    new HashedString[2]
    {
      HashedString.op_Implicit("working_pre"),
      HashedString.op_Implicit("working_loop_one_p")
    },
    new HashedString[2]
    {
      HashedString.op_Implicit("working_pre"),
      HashedString.op_Implicit("working_loop_two_p")
    }
  };

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    GameScheduler.Instance.Schedule("Scheduling Tutorial", 2f, (Action<object>) (obj => Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Schedule)), (object) null, (SchedulerGroup) null);
    this.workables = new ArcadeMachineWorkable[this.choreOffsets.Length];
    this.chores = new Chore[this.choreOffsets.Length];
    for (int index = 0; index < this.workables.Length; ++index)
    {
      GameObject locator = ChoreHelpers.CreateLocator("ArcadeMachineWorkable", Grid.CellToPosCBC(Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) this), this.choreOffsets[index]), Grid.SceneLayer.Move));
      ArcadeMachineWorkable arcadeMachineWorkable1 = locator.AddOrGet<ArcadeMachineWorkable>();
      KSelectable kselectable = locator.AddOrGet<KSelectable>();
      kselectable.SetName(((Component) this).GetProperName());
      kselectable.IsSelectable = false;
      int player_index = index;
      ArcadeMachineWorkable arcadeMachineWorkable2 = arcadeMachineWorkable1;
      arcadeMachineWorkable2.OnWorkableEventCB = arcadeMachineWorkable2.OnWorkableEventCB + (Action<Workable, Workable.WorkableEvent>) ((workable, ev) => this.OnWorkableEvent(player_index, ev));
      arcadeMachineWorkable1.overrideAnims = this.overrideAnims[index];
      arcadeMachineWorkable1.workAnims = this.workAnims[index];
      this.workables[index] = arcadeMachineWorkable1;
      this.workables[index].owner = this;
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
        this.workables[index] = (ArcadeMachineWorkable) null;
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
    WorkChore<ArcadeMachineWorkable> chore = new WorkChore<ArcadeMachineWorkable>(relax, (IStateMachineTarget) target, on_end: on_end, allow_in_red_alert: false, schedule_block: schedule_block, allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.high);
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
    Descriptor descriptor = new Descriptor();
    ((Descriptor) ref descriptor).SetupDescriptor((string) UI.BUILDINGEFFECTS.RECREATION, (string) UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION, (Descriptor.DescriptorType) 1);
    descs.Add(descriptor);
    Effect.AddModifierDescriptions(((Component) this).gameObject, descs, "PlayedArcade", true);
    return descs;
  }

  public class States : 
    GameStateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine>
  {
    public StateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.IntParameter playerCount;
    public GameStateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.State unoperational;
    public ArcadeMachine.States.OperationalStates operational;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.unoperational;
      this.unoperational.Enter((StateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.State.Callback) (smi => smi.SetActive(false))).TagTransition(GameTags.Operational, (GameStateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.State) this.operational).PlayAnim("off");
      this.operational.TagTransition(GameTags.Operational, this.unoperational, true).Enter("CreateChore", (StateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.State.Callback) (smi => smi.master.UpdateChores())).Exit("CancelChore", (StateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.State.Callback) (smi => smi.master.UpdateChores(false))).DefaultState(this.operational.stopped);
      this.operational.stopped.Enter((StateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.State.Callback) (smi => smi.SetActive(false))).PlayAnim("on").ParamTransition<int>((StateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.Parameter<int>) this.playerCount, this.operational.pre, (StateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.Parameter<int>.Callback) ((smi, p) => p > 0));
      this.operational.pre.Enter((StateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.State.Callback) (smi => smi.SetActive(true))).PlayAnim("working_pre").OnAnimQueueComplete(this.operational.playing);
      this.operational.playing.PlayAnim(new Func<ArcadeMachine.StatesInstance, string>(this.GetPlayingAnim), (KAnim.PlayMode) 0).ParamTransition<int>((StateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.Parameter<int>) this.playerCount, this.operational.post, (StateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.Parameter<int>.Callback) ((smi, p) => p == 0)).ParamTransition<int>((StateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.Parameter<int>) this.playerCount, this.operational.playing_coop, (StateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.Parameter<int>.Callback) ((smi, p) => p > 1));
      this.operational.playing_coop.PlayAnim(new Func<ArcadeMachine.StatesInstance, string>(this.GetPlayingAnim), (KAnim.PlayMode) 0).ParamTransition<int>((StateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.Parameter<int>) this.playerCount, this.operational.post, (StateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.Parameter<int>.Callback) ((smi, p) => p == 0)).ParamTransition<int>((StateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.Parameter<int>) this.playerCount, this.operational.playing, (StateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.Parameter<int>.Callback) ((smi, p) => p == 1));
      this.operational.post.PlayAnim("working_pst").OnAnimQueueComplete(this.operational.stopped);
    }

    private string GetPlayingAnim(ArcadeMachine.StatesInstance smi)
    {
      bool flag1 = smi.master.players.Contains(0);
      bool flag2 = smi.master.players.Contains(1);
      if (flag1 && !flag2)
        return "working_loop_one_p";
      return flag2 && !flag1 ? "working_loop_two_p" : "working_loop_coop_p";
    }

    public class OperationalStates : 
      GameStateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.State
    {
      public GameStateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.State stopped;
      public GameStateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.State pre;
      public GameStateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.State playing;
      public GameStateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.State playing_coop;
      public GameStateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.State post;
    }
  }

  public class StatesInstance : 
    GameStateMachine<ArcadeMachine.States, ArcadeMachine.StatesInstance, ArcadeMachine, object>.GameInstance
  {
    private Operational operational;

    public StatesInstance(ArcadeMachine smi)
      : base(smi)
    {
      this.operational = ((Component) this.master).GetComponent<Operational>();
    }

    public void SetActive(bool active) => this.operational.SetActive(this.operational.IsOperational & active);
  }
}
