// Decompiled with JetBrains decompiler
// Type: ElementDropperMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class ElementDropperMonitor : 
  GameStateMachine<ElementDropperMonitor, ElementDropperMonitor.Instance, IStateMachineTarget, ElementDropperMonitor.Def>
{
  public GameStateMachine<ElementDropperMonitor, ElementDropperMonitor.Instance, IStateMachineTarget, ElementDropperMonitor.Def>.State satisfied;
  public GameStateMachine<ElementDropperMonitor, ElementDropperMonitor.Instance, IStateMachineTarget, ElementDropperMonitor.Def>.State readytodrop;
  public StateMachine<ElementDropperMonitor, ElementDropperMonitor.Instance, IStateMachineTarget, ElementDropperMonitor.Def>.Signal cellChangedSignal;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.satisfied;
    this.root.EventHandler(GameHashes.DeathAnimComplete, (StateMachine<ElementDropperMonitor, ElementDropperMonitor.Instance, IStateMachineTarget, ElementDropperMonitor.Def>.State.Callback) (smi => smi.DropDeathElement()));
    this.satisfied.OnSignal(this.cellChangedSignal, this.readytodrop, (Func<ElementDropperMonitor.Instance, bool>) (smi => smi.ShouldDropElement()));
    this.readytodrop.ToggleBehaviour(GameTags.Creatures.WantsToDropElements, (StateMachine<ElementDropperMonitor, ElementDropperMonitor.Instance, IStateMachineTarget, ElementDropperMonitor.Def>.Transition.ConditionCallback) (smi => true), (System.Action<ElementDropperMonitor.Instance>) (smi => smi.GoTo((StateMachine.BaseState) this.satisfied))).EventHandler(GameHashes.ObjectMovementStateChanged, (GameStateMachine<ElementDropperMonitor, ElementDropperMonitor.Instance, IStateMachineTarget, ElementDropperMonitor.Def>.GameEvent.Callback) ((smi, d) =>
    {
      if ((GameHashes) d != GameHashes.ObjectMovementWakeUp)
        return;
      smi.GoTo((StateMachine.BaseState) this.satisfied);
    }));
  }

  public class Def : StateMachine.BaseDef
  {
    public SimHashes dirtyEmitElement;
    public float dirtyProbabilityPercent;
    public float dirtyCellToTargetMass;
    public float dirtyMassPerDirty;
    public float dirtyMassReleaseOnDeath;
    public byte emitDiseaseIdx = byte.MaxValue;
    public float emitDiseasePerKg;
  }

  public new class Instance : 
    GameStateMachine<ElementDropperMonitor, ElementDropperMonitor.Instance, IStateMachineTarget, ElementDropperMonitor.Def>.GameInstance
  {
    public Instance(IStateMachineTarget master, ElementDropperMonitor.Def def)
      : base(master, def)
    {
      Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(this.transform, new System.Action(this.OnCellChange), "ElementDropperMonitor.Instance");
    }

    public override void StopSM(string reason)
    {
      base.StopSM(reason);
      Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(this.transform, new System.Action(this.OnCellChange));
    }

    private void OnCellChange() => this.sm.cellChangedSignal.Trigger(this);

    public bool ShouldDropElement() => this.IsValidDropCell() && (double) Random.Range(0.0f, 100f) < (double) this.def.dirtyProbabilityPercent;

    public void DropDeathElement() => this.DropElement(this.def.dirtyMassReleaseOnDeath, this.def.dirtyEmitElement, this.def.emitDiseaseIdx, Mathf.RoundToInt(this.def.dirtyMassReleaseOnDeath * this.def.dirtyMassPerDirty));

    public void DropPeriodicElement() => this.DropElement(this.def.dirtyMassPerDirty, this.def.dirtyEmitElement, this.def.emitDiseaseIdx, Mathf.RoundToInt(this.def.emitDiseasePerKg * this.def.dirtyMassPerDirty));

    public void DropElement(float mass, SimHashes element_id, byte disease_idx, int disease_count)
    {
      if ((double) mass <= 0.0)
        return;
      Element elementByHash = ElementLoader.FindElementByHash(element_id);
      float temperature = this.GetComponent<PrimaryElement>().Temperature;
      if (elementByHash.IsGas || elementByHash.IsLiquid)
        SimMessages.AddRemoveSubstance(Grid.PosToCell(TransformExtensions.GetPosition(this.transform)), element_id, CellEventLogger.Instance.ElementConsumerSimUpdate, mass, temperature, disease_idx, disease_count);
      else if (elementByHash.IsSolid)
        elementByHash.substance.SpawnResource(Vector3.op_Addition(TransformExtensions.GetPosition(this.transform), new Vector3(0.0f, 0.5f, 0.0f)), mass, temperature, disease_idx, disease_count, forceTemperature: true);
      PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Resource, elementByHash.name, this.gameObject.transform);
    }

    public bool IsValidDropCell()
    {
      int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
      return Grid.IsValidCell(cell) && Grid.IsGas(cell) && (double) Grid.Mass[cell] <= 1.0;
    }
  }
}
