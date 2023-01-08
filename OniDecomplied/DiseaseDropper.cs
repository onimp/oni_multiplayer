// Decompiled with JetBrains decompiler
// Type: DiseaseDropper
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class DiseaseDropper : 
  GameStateMachine<DiseaseDropper, DiseaseDropper.Instance, IStateMachineTarget, DiseaseDropper.Def>
{
  public GameStateMachine<DiseaseDropper, DiseaseDropper.Instance, IStateMachineTarget, DiseaseDropper.Def>.State working;
  public GameStateMachine<DiseaseDropper, DiseaseDropper.Instance, IStateMachineTarget, DiseaseDropper.Def>.State stopped;
  public StateMachine<DiseaseDropper, DiseaseDropper.Instance, IStateMachineTarget, DiseaseDropper.Def>.Signal cellChangedSignal;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.stopped;
    this.root.EventHandler(GameHashes.BurstEmitDisease, (StateMachine<DiseaseDropper, DiseaseDropper.Instance, IStateMachineTarget, DiseaseDropper.Def>.State.Callback) (smi => smi.DropSingleEmit()));
    this.working.TagTransition(GameTags.PreventEmittingDisease, this.stopped).Update((System.Action<DiseaseDropper.Instance, float>) ((smi, dt) => smi.DropPeriodic(dt)));
    this.stopped.TagTransition(GameTags.PreventEmittingDisease, this.working, true);
  }

  public class Def : StateMachine.BaseDef, IGameObjectEffectDescriptor
  {
    public byte diseaseIdx = byte.MaxValue;
    public int singleEmitQuantity;
    public int averageEmitPerSecond;
    public float emitFrequency = 1f;

    public List<Descriptor> GetDescriptors(GameObject go)
    {
      List<Descriptor> descriptors = new List<Descriptor>();
      if (this.singleEmitQuantity > 0)
        descriptors.Add(new Descriptor(UI.UISIDESCREENS.PLANTERSIDESCREEN.DISEASE_DROPPER_BURST.Replace("{Disease}", GameUtil.GetFormattedDiseaseName(this.diseaseIdx)).Replace("{DiseaseAmount}", GameUtil.GetFormattedDiseaseAmount(this.singleEmitQuantity)), UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.DISEASE_DROPPER_BURST.Replace("{Disease}", GameUtil.GetFormattedDiseaseName(this.diseaseIdx)).Replace("{DiseaseAmount}", GameUtil.GetFormattedDiseaseAmount(this.singleEmitQuantity)), (Descriptor.DescriptorType) 1, false));
      if (this.averageEmitPerSecond > 0)
        descriptors.Add(new Descriptor(UI.UISIDESCREENS.PLANTERSIDESCREEN.DISEASE_DROPPER_CONSTANT.Replace("{Disease}", GameUtil.GetFormattedDiseaseName(this.diseaseIdx)).Replace("{DiseaseAmount}", GameUtil.GetFormattedDiseaseAmount(this.averageEmitPerSecond, GameUtil.TimeSlice.PerSecond)), UI.UISIDESCREENS.PLANTERSIDESCREEN.TOOLTIPS.DISEASE_DROPPER_CONSTANT.Replace("{Disease}", GameUtil.GetFormattedDiseaseName(this.diseaseIdx)).Replace("{DiseaseAmount}", GameUtil.GetFormattedDiseaseAmount(this.averageEmitPerSecond, GameUtil.TimeSlice.PerSecond)), (Descriptor.DescriptorType) 1, false));
      return descriptors;
    }
  }

  public new class Instance : 
    GameStateMachine<DiseaseDropper, DiseaseDropper.Instance, IStateMachineTarget, DiseaseDropper.Def>.GameInstance
  {
    private float timeSinceLastDrop;

    public Instance(IStateMachineTarget master, DiseaseDropper.Def def)
      : base(master, def)
    {
    }

    public bool ShouldDropDisease() => true;

    public void DropSingleEmit() => this.DropDisease(this.def.diseaseIdx, this.def.singleEmitQuantity);

    public void DropPeriodic(float dt)
    {
      this.timeSinceLastDrop += dt;
      if (this.def.averageEmitPerSecond <= 0 || (double) this.def.emitFrequency <= 0.0)
        return;
      for (; (double) this.timeSinceLastDrop > (double) this.def.emitFrequency; this.timeSinceLastDrop -= this.def.emitFrequency)
        this.DropDisease(this.def.diseaseIdx, (int) ((double) this.def.averageEmitPerSecond * (double) this.def.emitFrequency));
    }

    public void DropDisease(byte disease_idx, int disease_count)
    {
      if (disease_count <= 0 || disease_idx == byte.MaxValue)
        return;
      int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
      if (!Grid.IsValidCell(cell))
        return;
      SimMessages.ModifyDiseaseOnCell(cell, disease_idx, disease_count);
    }

    public bool IsValidDropCell()
    {
      int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
      return Grid.IsValidCell(cell) && Grid.IsGas(cell) && (double) Grid.Mass[cell] <= 1.0;
    }
  }
}
