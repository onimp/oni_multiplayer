// Decompiled with JetBrains decompiler
// Type: DecompositionMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DecompositionMonitor : 
  GameStateMachine<DecompositionMonitor, DecompositionMonitor.Instance>
{
  public StateMachine<DecompositionMonitor, DecompositionMonitor.Instance, IStateMachineTarget, object>.FloatParameter decomposition;
  [SerializeField]
  public int remainingRotMonsters = 3;
  public GameStateMachine<DecompositionMonitor, DecompositionMonitor.Instance, IStateMachineTarget, object>.State satisfied;
  public DecompositionMonitor.RottenState rotten;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.satisfied;
    this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
    this.satisfied.Update("UpdateDecomposition", (System.Action<DecompositionMonitor.Instance, float>) ((smi, dt) => smi.UpdateDecomposition(dt))).ParamTransition<float>((StateMachine<DecompositionMonitor, DecompositionMonitor.Instance, IStateMachineTarget, object>.Parameter<float>) this.decomposition, (GameStateMachine<DecompositionMonitor, DecompositionMonitor.Instance, IStateMachineTarget, object>.State) this.rotten, GameStateMachine<DecompositionMonitor, DecompositionMonitor.Instance, IStateMachineTarget, object>.IsGTEOne).ToggleAttributeModifier("Dead", (Func<DecompositionMonitor.Instance, AttributeModifier>) (smi => smi.satisfiedDecorModifier)).ToggleAttributeModifier("Dead", (Func<DecompositionMonitor.Instance, AttributeModifier>) (smi => smi.satisfiedDecorRadiusModifier));
    this.rotten.DefaultState((GameStateMachine<DecompositionMonitor, DecompositionMonitor.Instance, IStateMachineTarget, object>.State) this.rotten.exposed).ToggleStatusItem(Db.Get().DuplicantStatusItems.Rotten).ToggleAttributeModifier("Rotten", (Func<DecompositionMonitor.Instance, AttributeModifier>) (smi => smi.rottenDecorModifier)).ToggleAttributeModifier("Rotten", (Func<DecompositionMonitor.Instance, AttributeModifier>) (smi => smi.rottenDecorRadiusModifier));
    this.rotten.exposed.DefaultState(this.rotten.exposed.openair).EventTransition(GameHashes.OnStore, this.rotten.stored, (StateMachine<DecompositionMonitor, DecompositionMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => !smi.IsExposed()));
    this.rotten.exposed.openair.Enter((StateMachine<DecompositionMonitor, DecompositionMonitor.Instance, IStateMachineTarget, object>.State.Callback) (smi =>
    {
      if (!smi.spawnsRotMonsters)
        return;
      smi.ScheduleGoTo(Random.Range(150f, 300f), (StateMachine.BaseState) this.rotten.spawningmonster);
    })).Transition((GameStateMachine<DecompositionMonitor, DecompositionMonitor.Instance, IStateMachineTarget, object>.State) this.rotten.exposed.submerged, (StateMachine<DecompositionMonitor, DecompositionMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.IsSubmerged())).ToggleFX((Func<DecompositionMonitor.Instance, StateMachine.Instance>) (smi => (StateMachine.Instance) this.CreateFX(smi)));
    this.rotten.exposed.submerged.DefaultState(this.rotten.exposed.submerged.idle).Transition(this.rotten.exposed.openair, (StateMachine<DecompositionMonitor, DecompositionMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => !smi.IsSubmerged()));
    this.rotten.exposed.submerged.idle.ScheduleGoTo(0.25f, (StateMachine.BaseState) this.rotten.exposed.submerged.dirtywater);
    this.rotten.exposed.submerged.dirtywater.Enter("DirtyWater", (StateMachine<DecompositionMonitor, DecompositionMonitor.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.DirtyWater(smi.dirtyWaterMaxRange))).GoTo(this.rotten.exposed.submerged.idle);
    this.rotten.spawningmonster.Enter((StateMachine<DecompositionMonitor, DecompositionMonitor.Instance, IStateMachineTarget, object>.State.Callback) (smi =>
    {
      if (this.remainingRotMonsters > 0)
      {
        --this.remainingRotMonsters;
        GameUtil.KInstantiate(Assets.GetPrefab(new Tag("Glom")), TransformExtensions.GetPosition(smi.transform), Grid.SceneLayer.Creatures).SetActive(true);
      }
      smi.GoTo((StateMachine.BaseState) this.rotten.exposed);
    }));
    this.rotten.stored.EventTransition(GameHashes.OnStore, (GameStateMachine<DecompositionMonitor, DecompositionMonitor.Instance, IStateMachineTarget, object>.State) this.rotten.exposed, (StateMachine<DecompositionMonitor, DecompositionMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.IsExposed()));
  }

  private FliesFX.Instance CreateFX(DecompositionMonitor.Instance smi) => !smi.isMasterNull ? new FliesFX.Instance(smi.master, new Vector3(0.0f, 0.0f, -0.1f)) : (FliesFX.Instance) null;

  public class SubmergedState : 
    GameStateMachine<DecompositionMonitor, DecompositionMonitor.Instance, IStateMachineTarget, object>.State
  {
    public GameStateMachine<DecompositionMonitor, DecompositionMonitor.Instance, IStateMachineTarget, object>.State idle;
    public GameStateMachine<DecompositionMonitor, DecompositionMonitor.Instance, IStateMachineTarget, object>.State dirtywater;
  }

  public class ExposedState : 
    GameStateMachine<DecompositionMonitor, DecompositionMonitor.Instance, IStateMachineTarget, object>.State
  {
    public DecompositionMonitor.SubmergedState submerged;
    public GameStateMachine<DecompositionMonitor, DecompositionMonitor.Instance, IStateMachineTarget, object>.State openair;
  }

  public class RottenState : 
    GameStateMachine<DecompositionMonitor, DecompositionMonitor.Instance, IStateMachineTarget, object>.State
  {
    public DecompositionMonitor.ExposedState exposed;
    public GameStateMachine<DecompositionMonitor, DecompositionMonitor.Instance, IStateMachineTarget, object>.State stored;
    public GameStateMachine<DecompositionMonitor, DecompositionMonitor.Instance, IStateMachineTarget, object>.State spawningmonster;
  }

  public new class Instance : 
    GameStateMachine<DecompositionMonitor, DecompositionMonitor.Instance, IStateMachineTarget, object>.GameInstance
  {
    public float decompositionRate;
    public Klei.AI.Disease disease;
    public int dirtyWaterMaxRange = 3;
    public bool spawnsRotMonsters = true;
    public AttributeModifier satisfiedDecorModifier = new AttributeModifier(Db.Get().BuildingAttributes.Decor.Id, -65f, (string) DUPLICANTS.MODIFIERS.DEAD.NAME);
    public AttributeModifier satisfiedDecorRadiusModifier = new AttributeModifier(Db.Get().BuildingAttributes.DecorRadius.Id, 4f, (string) DUPLICANTS.MODIFIERS.DEAD.NAME);
    public AttributeModifier rottenDecorModifier = new AttributeModifier(Db.Get().BuildingAttributes.Decor.Id, -100f, (string) DUPLICANTS.MODIFIERS.ROTTING.NAME);
    public AttributeModifier rottenDecorRadiusModifier = new AttributeModifier(Db.Get().BuildingAttributes.DecorRadius.Id, 4f, (string) DUPLICANTS.MODIFIERS.ROTTING.NAME);

    public Instance(
      IStateMachineTarget master,
      Klei.AI.Disease disease,
      float decompositionRate = 0.000833333354f,
      bool spawnRotMonsters = true)
      : base(master)
    {
      this.gameObject.AddComponent<DecorProvider>();
      this.decompositionRate = decompositionRate;
      this.disease = disease;
      this.spawnsRotMonsters = spawnRotMonsters;
    }

    public void UpdateDecomposition(float dt)
    {
      double num = (double) this.sm.decomposition.Delta(dt * this.decompositionRate, this.smi);
    }

    public bool IsExposed()
    {
      KPrefabID component = this.smi.GetComponent<KPrefabID>();
      return Object.op_Equality((Object) component, (Object) null) || !component.HasTag(GameTags.Preserved);
    }

    public bool IsRotten() => this.IsInsideState((StateMachine.BaseState) this.sm.rotten);

    public bool IsSubmerged() => PathFinder.IsSubmerged(Grid.PosToCell(TransformExtensions.GetPosition(this.master.transform)));

    public void DirtyWater(int maxCellRange = 3)
    {
      int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.master.transform));
      if (Grid.Element[cell].id == SimHashes.Water)
      {
        SimMessages.ReplaceElement(cell, SimHashes.DirtyWater, CellEventLogger.Instance.DecompositionDirtyWater, Grid.Mass[cell], Grid.Temperature[cell], Grid.DiseaseIdx[cell], Grid.DiseaseCount[cell]);
      }
      else
      {
        if (Grid.Element[cell].id != SimHashes.DirtyWater)
          return;
        int[] numArray = new int[4];
        for (int index1 = 0; index1 < maxCellRange; ++index1)
        {
          for (int index2 = 0; index2 < maxCellRange; ++index2)
          {
            numArray[0] = Grid.OffsetCell(cell, new CellOffset(-index1, index2));
            numArray[1] = Grid.OffsetCell(cell, new CellOffset(index1, index2));
            numArray[2] = Grid.OffsetCell(cell, new CellOffset(-index1, -index2));
            numArray[3] = Grid.OffsetCell(cell, new CellOffset(index1, -index2));
            Util.Shuffle<int>((IList<int>) numArray);
            foreach (int index3 in numArray)
            {
              if (Grid.GetCellDistance(cell, index3) < maxCellRange - 1 && Grid.IsValidCell(index3) && Grid.Element[index3].id == SimHashes.Water)
              {
                SimMessages.ReplaceElement(index3, SimHashes.DirtyWater, CellEventLogger.Instance.DecompositionDirtyWater, Grid.Mass[index3], Grid.Temperature[index3], Grid.DiseaseIdx[index3], Grid.DiseaseCount[index3]);
                return;
              }
            }
          }
        }
      }
    }
  }
}
