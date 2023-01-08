// Decompiled with JetBrains decompiler
// Type: GeneticAnalysisStation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;

public class GeneticAnalysisStation : 
  GameStateMachine<GeneticAnalysisStation, GeneticAnalysisStation.StatesInstance, IStateMachineTarget, GeneticAnalysisStation.Def>
{
  public GameStateMachine<GeneticAnalysisStation, GeneticAnalysisStation.StatesInstance, IStateMachineTarget, GeneticAnalysisStation.Def>.State inoperational;
  public GameStateMachine<GeneticAnalysisStation, GeneticAnalysisStation.StatesInstance, IStateMachineTarget, GeneticAnalysisStation.Def>.State operational;
  public GameStateMachine<GeneticAnalysisStation, GeneticAnalysisStation.StatesInstance, IStateMachineTarget, GeneticAnalysisStation.Def>.State ready;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.inoperational;
    this.inoperational.EventTransition(GameHashes.OperationalChanged, this.ready, new StateMachine<GeneticAnalysisStation, GeneticAnalysisStation.StatesInstance, IStateMachineTarget, GeneticAnalysisStation.Def>.Transition.ConditionCallback(this.IsOperational));
    this.operational.EventTransition(GameHashes.OperationalChanged, this.inoperational, GameStateMachine<GeneticAnalysisStation, GeneticAnalysisStation.StatesInstance, IStateMachineTarget, GeneticAnalysisStation.Def>.Not(new StateMachine<GeneticAnalysisStation, GeneticAnalysisStation.StatesInstance, IStateMachineTarget, GeneticAnalysisStation.Def>.Transition.ConditionCallback(this.IsOperational))).EventTransition(GameHashes.OnStorageChange, this.ready, new StateMachine<GeneticAnalysisStation, GeneticAnalysisStation.StatesInstance, IStateMachineTarget, GeneticAnalysisStation.Def>.Transition.ConditionCallback(this.HasSeedToStudy));
    this.ready.EventTransition(GameHashes.OperationalChanged, this.inoperational, GameStateMachine<GeneticAnalysisStation, GeneticAnalysisStation.StatesInstance, IStateMachineTarget, GeneticAnalysisStation.Def>.Not(new StateMachine<GeneticAnalysisStation, GeneticAnalysisStation.StatesInstance, IStateMachineTarget, GeneticAnalysisStation.Def>.Transition.ConditionCallback(this.IsOperational))).EventTransition(GameHashes.OnStorageChange, this.operational, GameStateMachine<GeneticAnalysisStation, GeneticAnalysisStation.StatesInstance, IStateMachineTarget, GeneticAnalysisStation.Def>.Not(new StateMachine<GeneticAnalysisStation, GeneticAnalysisStation.StatesInstance, IStateMachineTarget, GeneticAnalysisStation.Def>.Transition.ConditionCallback(this.HasSeedToStudy))).ToggleChore(new Func<GeneticAnalysisStation.StatesInstance, Chore>(this.CreateChore), this.operational);
  }

  private bool HasSeedToStudy(GeneticAnalysisStation.StatesInstance smi) => (double) smi.storage.GetMassAvailable(GameTags.UnidentifiedSeed) >= 1.0;

  private bool IsOperational(GeneticAnalysisStation.StatesInstance smi) => smi.GetComponent<Operational>().IsOperational;

  private Chore CreateChore(GeneticAnalysisStation.StatesInstance smi) => (Chore) new WorkChore<GeneticAnalysisStationWorkable>(Db.Get().ChoreTypes.AnalyzeSeed, (IStateMachineTarget) smi.workable);

  public class Def : StateMachine.BaseDef
  {
  }

  public class StatesInstance : 
    GameStateMachine<GeneticAnalysisStation, GeneticAnalysisStation.StatesInstance, IStateMachineTarget, GeneticAnalysisStation.Def>.GameInstance
  {
    [MyCmpReq]
    public Storage storage;
    [MyCmpReq]
    public ManualDeliveryKG manualDelivery;
    [MyCmpReq]
    public GeneticAnalysisStationWorkable workable;
    [Serialize]
    private HashSet<Tag> forbiddenSeeds;

    public StatesInstance(IStateMachineTarget master, GeneticAnalysisStation.Def def)
      : base(master, def)
    {
      this.workable.statesInstance = this;
    }

    public override void StartSM()
    {
      base.StartSM();
      this.RefreshFetchTags();
    }

    public void SetSeedForbidden(Tag seedID, bool forbidden)
    {
      if (this.forbiddenSeeds == null)
        this.forbiddenSeeds = new HashSet<Tag>();
      if (!(!forbidden ? this.forbiddenSeeds.Remove(seedID) : this.forbiddenSeeds.Add(seedID)))
        return;
      this.RefreshFetchTags();
    }

    public bool GetSeedForbidden(Tag seedID)
    {
      if (this.forbiddenSeeds == null)
        this.forbiddenSeeds = new HashSet<Tag>();
      return this.forbiddenSeeds.Contains(seedID);
    }

    private void RefreshFetchTags()
    {
      if (this.forbiddenSeeds == null)
      {
        this.manualDelivery.ForbiddenTags = (Tag[]) null;
      }
      else
      {
        Tag[] tagArray = new Tag[this.forbiddenSeeds.Count];
        int num = 0;
        foreach (Tag forbiddenSeed in this.forbiddenSeeds)
        {
          tagArray[num++] = forbiddenSeed;
          this.storage.Drop(forbiddenSeed);
        }
        this.manualDelivery.ForbiddenTags = tagArray;
      }
    }
  }
}
