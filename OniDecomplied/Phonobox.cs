// Decompiled with JetBrains decompiler
// Type: Phonobox
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
public class Phonobox : StateMachineComponent<Phonobox.StatesInstance>, IGameObjectEffectDescriptor
{
  public const string SPECIFIC_EFFECT = "Danced";
  public const string TRACKING_EFFECT = "RecentlyDanced";
  public CellOffset[] choreOffsets = new CellOffset[5]
  {
    new CellOffset(0, 0),
    new CellOffset(-1, 0),
    new CellOffset(1, 0),
    new CellOffset(-2, 0),
    new CellOffset(2, 0)
  };
  private PhonoboxWorkable[] workables;
  private Chore[] chores;
  private HashSet<Worker> players = new HashSet<Worker>();
  private static string[] building_anims = new string[3]
  {
    "working_loop",
    "working_loop2",
    "working_loop3"
  };

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
    GameScheduler.Instance.Schedule("Scheduling Tutorial", 2f, (Action<object>) (obj => Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Schedule)), (object) null, (SchedulerGroup) null);
    this.workables = new PhonoboxWorkable[this.choreOffsets.Length];
    this.chores = new Chore[this.choreOffsets.Length];
    for (int index = 0; index < this.workables.Length; ++index)
    {
      GameObject locator = ChoreHelpers.CreateLocator("PhonoboxWorkable", Grid.CellToPosCBC(Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) this), this.choreOffsets[index]), Grid.SceneLayer.Move));
      KSelectable kselectable = locator.AddOrGet<KSelectable>();
      kselectable.SetName(((Component) this).GetProperName());
      kselectable.IsSelectable = false;
      PhonoboxWorkable phonoboxWorkable = locator.AddOrGet<PhonoboxWorkable>();
      phonoboxWorkable.owner = this;
      this.workables[index] = phonoboxWorkable;
    }
  }

  protected override void OnCleanUp()
  {
    this.UpdateChores(false);
    for (int index = 0; index < this.workables.Length; ++index)
    {
      if (Object.op_Implicit((Object) this.workables[index]))
      {
        Util.KDestroyGameObject((Component) this.workables[index]);
        this.workables[index] = (PhonoboxWorkable) null;
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
    WorkChore<PhonoboxWorkable> chore = new WorkChore<PhonoboxWorkable>(relax, (IStateMachineTarget) target, on_end: on_end, allow_in_red_alert: false, schedule_block: schedule_block, allow_prioritization: false, priority_class: PriorityScreen.PriorityClass.high);
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

  public void AddWorker(Worker player)
  {
    this.players.Add(player);
    this.smi.sm.playerCount.Set(this.players.Count, this.smi);
  }

  public void RemoveWorker(Worker player)
  {
    this.players.Remove(player);
    this.smi.sm.playerCount.Set(this.players.Count, this.smi);
  }

  List<Descriptor> IGameObjectEffectDescriptor.GetDescriptors(GameObject go)
  {
    List<Descriptor> descs = new List<Descriptor>();
    Descriptor descriptor = new Descriptor();
    ((Descriptor) ref descriptor).SetupDescriptor((string) UI.BUILDINGEFFECTS.RECREATION, (string) UI.BUILDINGEFFECTS.TOOLTIPS.RECREATION, (Descriptor.DescriptorType) 1);
    descs.Add(descriptor);
    Effect.AddModifierDescriptions(((Component) this).gameObject, descs, "Danced", true);
    return descs;
  }

  public class States : GameStateMachine<Phonobox.States, Phonobox.StatesInstance, Phonobox>
  {
    public StateMachine<Phonobox.States, Phonobox.StatesInstance, Phonobox, object>.IntParameter playerCount;
    public GameStateMachine<Phonobox.States, Phonobox.StatesInstance, Phonobox, object>.State unoperational;
    public Phonobox.States.OperationalStates operational;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.unoperational;
      this.unoperational.Enter((StateMachine<Phonobox.States, Phonobox.StatesInstance, Phonobox, object>.State.Callback) (smi => smi.SetActive(false))).TagTransition(GameTags.Operational, (GameStateMachine<Phonobox.States, Phonobox.StatesInstance, Phonobox, object>.State) this.operational).PlayAnim("off");
      this.operational.TagTransition(GameTags.Operational, this.unoperational, true).Enter("CreateChore", (StateMachine<Phonobox.States, Phonobox.StatesInstance, Phonobox, object>.State.Callback) (smi => smi.master.UpdateChores())).Exit("CancelChore", (StateMachine<Phonobox.States, Phonobox.StatesInstance, Phonobox, object>.State.Callback) (smi => smi.master.UpdateChores(false))).DefaultState(this.operational.stopped);
      this.operational.stopped.Enter((StateMachine<Phonobox.States, Phonobox.StatesInstance, Phonobox, object>.State.Callback) (smi => smi.SetActive(false))).ParamTransition<int>((StateMachine<Phonobox.States, Phonobox.StatesInstance, Phonobox, object>.Parameter<int>) this.playerCount, this.operational.pre, (StateMachine<Phonobox.States, Phonobox.StatesInstance, Phonobox, object>.Parameter<int>.Callback) ((smi, p) => p > 0)).PlayAnim("on");
      this.operational.pre.PlayAnim("working_pre").OnAnimQueueComplete(this.operational.playing);
      this.operational.playing.Enter((StateMachine<Phonobox.States, Phonobox.StatesInstance, Phonobox, object>.State.Callback) (smi => smi.SetActive(true))).ScheduleGoTo(25f, (StateMachine.BaseState) this.operational.song_end).ParamTransition<int>((StateMachine<Phonobox.States, Phonobox.StatesInstance, Phonobox, object>.Parameter<int>) this.playerCount, this.operational.post, (StateMachine<Phonobox.States, Phonobox.StatesInstance, Phonobox, object>.Parameter<int>.Callback) ((smi, p) => p == 0)).PlayAnim(new Func<Phonobox.StatesInstance, string>(Phonobox.States.GetPlayAnim), (KAnim.PlayMode) 0);
      this.operational.song_end.ParamTransition<int>((StateMachine<Phonobox.States, Phonobox.StatesInstance, Phonobox, object>.Parameter<int>) this.playerCount, this.operational.bridge, (StateMachine<Phonobox.States, Phonobox.StatesInstance, Phonobox, object>.Parameter<int>.Callback) ((smi, p) => p > 0)).ParamTransition<int>((StateMachine<Phonobox.States, Phonobox.StatesInstance, Phonobox, object>.Parameter<int>) this.playerCount, this.operational.post, (StateMachine<Phonobox.States, Phonobox.StatesInstance, Phonobox, object>.Parameter<int>.Callback) ((smi, p) => p == 0));
      this.operational.bridge.PlayAnim("working_trans").OnAnimQueueComplete(this.operational.playing);
      this.operational.post.PlayAnim("working_pst").OnAnimQueueComplete(this.operational.stopped);
    }

    public static string GetPlayAnim(Phonobox.StatesInstance smi)
    {
      int index = Random.Range(0, Phonobox.building_anims.Length);
      return Phonobox.building_anims[index];
    }

    public class OperationalStates : 
      GameStateMachine<Phonobox.States, Phonobox.StatesInstance, Phonobox, object>.State
    {
      public GameStateMachine<Phonobox.States, Phonobox.StatesInstance, Phonobox, object>.State stopped;
      public GameStateMachine<Phonobox.States, Phonobox.StatesInstance, Phonobox, object>.State pre;
      public GameStateMachine<Phonobox.States, Phonobox.StatesInstance, Phonobox, object>.State bridge;
      public GameStateMachine<Phonobox.States, Phonobox.StatesInstance, Phonobox, object>.State playing;
      public GameStateMachine<Phonobox.States, Phonobox.StatesInstance, Phonobox, object>.State song_end;
      public GameStateMachine<Phonobox.States, Phonobox.StatesInstance, Phonobox, object>.State post;
    }
  }

  public class StatesInstance : 
    GameStateMachine<Phonobox.States, Phonobox.StatesInstance, Phonobox, object>.GameInstance
  {
    private FetchChore chore;
    private Operational operational;

    public StatesInstance(Phonobox smi)
      : base(smi)
    {
      this.operational = ((Component) this.master).GetComponent<Operational>();
    }

    public void SetActive(bool active) => this.operational.SetActive(this.operational.IsOperational & active);
  }
}
