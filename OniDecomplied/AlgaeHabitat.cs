// Decompiled with JetBrains decompiler
// Type: AlgaeHabitat
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class AlgaeHabitat : StateMachineComponent<AlgaeHabitat.SMInstance>
{
  [MyCmpGet]
  private Operational operational;
  private Storage pollutedWaterStorage;
  [SerializeField]
  public float lightBonusMultiplier = 1.1f;
  public CellOffset pressureSampleOffset = CellOffset.none;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
    GameScheduler.Instance.Schedule("WaterFetchingTutorial", 2f, (Action<object>) (obj => Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_FetchingWater)), (object) null, (SchedulerGroup) null);
    this.ConfigurePollutedWaterOutput();
    Tutorial.Instance.oxygenGenerators.Add(((Component) this).gameObject);
  }

  protected override void OnCleanUp()
  {
    Tutorial.Instance.oxygenGenerators.Remove(((Component) this).gameObject);
    base.OnCleanUp();
  }

  private void ConfigurePollutedWaterOutput()
  {
    Storage storage = (Storage) null;
    Tag tag = ElementLoader.FindElementByHash(SimHashes.DirtyWater).tag;
    foreach (Storage component in ((Component) this).GetComponents<Storage>())
    {
      if (component.storageFilters.Contains(tag))
      {
        storage = component;
        break;
      }
    }
    foreach (ElementConverter component in ((Component) this).GetComponents<ElementConverter>())
    {
      foreach (ElementConverter.OutputElement outputElement in component.outputElements)
      {
        if (outputElement.elementHash == SimHashes.DirtyWater)
        {
          component.SetStorage(storage);
          break;
        }
      }
    }
    this.pollutedWaterStorage = storage;
  }

  public class SMInstance : 
    GameStateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat, object>.GameInstance
  {
    public ElementConverter converter;
    public Chore emptyChore;

    public SMInstance(AlgaeHabitat master)
      : base(master)
    {
      this.converter = ((Component) master).GetComponent<ElementConverter>();
    }

    public bool HasEnoughMass(Tag tag) => this.converter.HasEnoughMass(tag);

    public bool NeedsEmptying() => (double) this.smi.master.pollutedWaterStorage.RemainingCapacity() <= 0.0;

    public void CreateEmptyChore()
    {
      if (this.emptyChore != null)
        this.emptyChore.Cancel("dupe");
      AlgaeHabitatEmpty component = ((Component) this.master).GetComponent<AlgaeHabitatEmpty>();
      this.emptyChore = (Chore) new WorkChore<AlgaeHabitatEmpty>(Db.Get().ChoreTypes.EmptyStorage, (IStateMachineTarget) component, on_complete: new Action<Chore>(this.OnEmptyComplete), ignore_building_assignment: true);
    }

    public void CancelEmptyChore()
    {
      if (this.emptyChore == null)
        return;
      this.emptyChore.Cancel("Cancelled");
      this.emptyChore = (Chore) null;
    }

    private void OnEmptyComplete(Chore chore)
    {
      this.emptyChore = (Chore) null;
      this.master.pollutedWaterStorage.DropAll(true, offset: new Vector3());
    }
  }

  public class States : GameStateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat>
  {
    public GameStateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat, object>.State generatingOxygen;
    public GameStateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat, object>.State stoppedGeneratingOxygen;
    public GameStateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat, object>.State stoppedGeneratingOxygenTransition;
    public GameStateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat, object>.State noWater;
    public GameStateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat, object>.State noAlgae;
    public GameStateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat, object>.State needsEmptying;
    public GameStateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat, object>.State gotAlgae;
    public GameStateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat, object>.State gotWater;
    public GameStateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat, object>.State gotEmptied;
    public GameStateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat, object>.State lostAlgae;
    public GameStateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat, object>.State notoperational;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.noAlgae;
      this.root.EventTransition(GameHashes.OperationalChanged, this.notoperational, (StateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat, object>.Transition.ConditionCallback) (smi => !smi.master.operational.IsOperational)).EventTransition(GameHashes.OperationalChanged, this.noAlgae, (StateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat, object>.Transition.ConditionCallback) (smi => smi.master.operational.IsOperational));
      this.notoperational.QueueAnim("off");
      this.gotAlgae.PlayAnim("on_pre").OnAnimQueueComplete(this.noWater);
      this.gotEmptied.PlayAnim("on_pre").OnAnimQueueComplete(this.generatingOxygen);
      this.lostAlgae.PlayAnim("on_pst").OnAnimQueueComplete(this.noAlgae);
      this.noAlgae.QueueAnim("off").EventTransition(GameHashes.OnStorageChange, this.gotAlgae, (StateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat, object>.Transition.ConditionCallback) (smi => smi.HasEnoughMass(GameTags.Algae))).Enter((StateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat, object>.State.Callback) (smi => smi.master.operational.SetActive(false)));
      this.noWater.QueueAnim("on").Enter((StateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat, object>.State.Callback) (smi => ((Component) smi.master).GetComponent<PassiveElementConsumer>().EnableConsumption(true))).EventTransition(GameHashes.OnStorageChange, this.lostAlgae, (StateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat, object>.Transition.ConditionCallback) (smi => !smi.HasEnoughMass(GameTags.Algae))).EventTransition(GameHashes.OnStorageChange, this.gotWater, (StateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat, object>.Transition.ConditionCallback) (smi => smi.HasEnoughMass(GameTags.Algae) && smi.HasEnoughMass(GameTags.Water)));
      this.needsEmptying.QueueAnim("off").Enter((StateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat, object>.State.Callback) (smi => smi.CreateEmptyChore())).Exit((StateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat, object>.State.Callback) (smi => smi.CancelEmptyChore())).ToggleStatusItem(Db.Get().BuildingStatusItems.HabitatNeedsEmptying).EventTransition(GameHashes.OnStorageChange, this.noAlgae, (StateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat, object>.Transition.ConditionCallback) (smi => !smi.HasEnoughMass(GameTags.Algae) || !smi.HasEnoughMass(GameTags.Water))).EventTransition(GameHashes.OnStorageChange, this.gotEmptied, (StateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat, object>.Transition.ConditionCallback) (smi => smi.HasEnoughMass(GameTags.Algae) && smi.HasEnoughMass(GameTags.Water) && !smi.NeedsEmptying()));
      this.gotWater.PlayAnim("working_pre").OnAnimQueueComplete(this.needsEmptying);
      this.generatingOxygen.Enter((StateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat, object>.State.Callback) (smi => smi.master.operational.SetActive(true))).Exit((StateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat, object>.State.Callback) (smi => smi.master.operational.SetActive(false))).Update("GeneratingOxygen", (Action<AlgaeHabitat.SMInstance, float>) ((smi, dt) =>
      {
        int cell = Grid.PosToCell(TransformExtensions.GetPosition(smi.master.transform));
        smi.converter.OutputMultiplier = Grid.LightCount[cell] > 0 ? smi.master.lightBonusMultiplier : 1f;
      })).QueueAnim("working_loop", true).EventTransition(GameHashes.OnStorageChange, this.stoppedGeneratingOxygen, (StateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat, object>.Transition.ConditionCallback) (smi => !smi.HasEnoughMass(GameTags.Water) || !smi.HasEnoughMass(GameTags.Algae) || smi.NeedsEmptying()));
      this.stoppedGeneratingOxygen.PlayAnim("working_pst").OnAnimQueueComplete(this.stoppedGeneratingOxygenTransition);
      this.stoppedGeneratingOxygenTransition.EventTransition(GameHashes.OnStorageChange, this.needsEmptying, (StateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat, object>.Transition.ConditionCallback) (smi => smi.NeedsEmptying())).EventTransition(GameHashes.OnStorageChange, this.noWater, (StateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat, object>.Transition.ConditionCallback) (smi => !smi.HasEnoughMass(GameTags.Water))).EventTransition(GameHashes.OnStorageChange, this.lostAlgae, (StateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat, object>.Transition.ConditionCallback) (smi => !smi.HasEnoughMass(GameTags.Algae))).EventTransition(GameHashes.OnStorageChange, this.gotWater, (StateMachine<AlgaeHabitat.States, AlgaeHabitat.SMInstance, AlgaeHabitat, object>.Transition.ConditionCallback) (smi => smi.HasEnoughMass(GameTags.Water) && smi.HasEnoughMass(GameTags.Algae)));
    }
  }
}
