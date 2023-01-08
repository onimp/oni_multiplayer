// Decompiled with JetBrains decompiler
// Type: RadiationVulnerable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System.Collections.Generic;
using UnityEngine;

public class RadiationVulnerable : 
  GameStateMachine<RadiationVulnerable, RadiationVulnerable.StatesInstance, IStateMachineTarget, RadiationVulnerable.Def>
{
  public GameStateMachine<RadiationVulnerable, RadiationVulnerable.StatesInstance, IStateMachineTarget, RadiationVulnerable.Def>.State comfortable;
  public GameStateMachine<RadiationVulnerable, RadiationVulnerable.StatesInstance, IStateMachineTarget, RadiationVulnerable.Def>.State too_dark;
  public GameStateMachine<RadiationVulnerable, RadiationVulnerable.StatesInstance, IStateMachineTarget, RadiationVulnerable.Def>.State too_bright;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.comfortable;
    this.comfortable.Transition(this.too_dark, (StateMachine<RadiationVulnerable, RadiationVulnerable.StatesInstance, IStateMachineTarget, RadiationVulnerable.Def>.Transition.ConditionCallback) (smi => smi.GetRadiationThresholdCrossed() == -1), (UpdateRate) 6).Transition(this.too_bright, (StateMachine<RadiationVulnerable, RadiationVulnerable.StatesInstance, IStateMachineTarget, RadiationVulnerable.Def>.Transition.ConditionCallback) (smi => smi.GetRadiationThresholdCrossed() == 1), (UpdateRate) 6).TriggerOnEnter(GameHashes.RadiationComfort);
    this.too_dark.Transition(this.comfortable, (StateMachine<RadiationVulnerable, RadiationVulnerable.StatesInstance, IStateMachineTarget, RadiationVulnerable.Def>.Transition.ConditionCallback) (smi => smi.GetRadiationThresholdCrossed() != -1), (UpdateRate) 6).TriggerOnEnter(GameHashes.RadiationDiscomfort);
    this.too_bright.Transition(this.comfortable, (StateMachine<RadiationVulnerable, RadiationVulnerable.StatesInstance, IStateMachineTarget, RadiationVulnerable.Def>.Transition.ConditionCallback) (smi => smi.GetRadiationThresholdCrossed() != 1), (UpdateRate) 6).TriggerOnEnter(GameHashes.RadiationDiscomfort);
  }

  public class Def : StateMachine.BaseDef, IGameObjectEffectDescriptor
  {
    public List<Descriptor> GetDescriptors(GameObject go)
    {
      Modifiers component1 = go.GetComponent<Modifiers>();
      float modifiedAttributeValue = component1.GetPreModifiedAttributeValue(Db.Get().PlantAttributes.MinRadiationThreshold);
      string attributeFormattedValue1 = component1.GetPreModifiedAttributeFormattedValue(Db.Get().PlantAttributes.MinRadiationThreshold);
      string attributeFormattedValue2 = component1.GetPreModifiedAttributeFormattedValue(Db.Get().PlantAttributes.MaxRadiationThreshold);
      MutantPlant component2 = go.GetComponent<MutantPlant>();
      bool flag = Object.op_Inequality((Object) component2, (Object) null) && component2.IsOriginal;
      if ((double) modifiedAttributeValue <= 0.0)
      {
        List<Descriptor> descriptors = new List<Descriptor>();
        descriptors.Add(new Descriptor(UI.GAMEOBJECTEFFECTS.REQUIRES_NO_MIN_RADIATION.Replace("{MaxRads}", attributeFormattedValue2), UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_NO_MIN_RADIATION.Replace("{MaxRads}", attributeFormattedValue2) + (flag ? UI.GAMEOBJECTEFFECTS.TOOLTIPS.MUTANT_SEED_TOOLTIP.ToString() : ""), (Descriptor.DescriptorType) 0, false));
        return descriptors;
      }
      List<Descriptor> descriptors1 = new List<Descriptor>();
      descriptors1.Add(new Descriptor(UI.GAMEOBJECTEFFECTS.REQUIRES_RADIATION.Replace("{MinRads}", attributeFormattedValue1).Replace("{MaxRads}", attributeFormattedValue2), UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_RADIATION.Replace("{MinRads}", attributeFormattedValue1).Replace("{MaxRads}", attributeFormattedValue2) + (flag ? UI.GAMEOBJECTEFFECTS.TOOLTIPS.MUTANT_SEED_TOOLTIP.ToString() : ""), (Descriptor.DescriptorType) 0, false));
      return descriptors1;
    }
  }

  public class StatesInstance : 
    GameStateMachine<RadiationVulnerable, RadiationVulnerable.StatesInstance, IStateMachineTarget, RadiationVulnerable.Def>.GameInstance,
    IWiltCause
  {
    private AttributeInstance minRadiationAttributeInstance;
    private AttributeInstance maxRadiationAttributeInstance;

    public StatesInstance(IStateMachineTarget master, RadiationVulnerable.Def def)
      : base(master, def)
    {
      this.minRadiationAttributeInstance = Db.Get().PlantAttributes.MinRadiationThreshold.Lookup(this.gameObject);
      this.maxRadiationAttributeInstance = Db.Get().PlantAttributes.MaxRadiationThreshold.Lookup(this.gameObject);
    }

    public int GetRadiationThresholdCrossed()
    {
      int cell = Grid.PosToCell(this.master.gameObject);
      if (!Grid.IsValidCell(cell))
        return 0;
      if ((double) Grid.Radiation[cell] < (double) this.minRadiationAttributeInstance.GetTotalValue())
        return -1;
      return (double) Grid.Radiation[cell] <= (double) this.maxRadiationAttributeInstance.GetTotalValue() ? 0 : 1;
    }

    public WiltCondition.Condition[] Conditions => new WiltCondition.Condition[1]
    {
      WiltCondition.Condition.Radiation
    };

    public string WiltStateString
    {
      get
      {
        if (this.smi.IsInsideState((StateMachine.BaseState) this.smi.sm.too_dark))
          return Db.Get().CreatureStatusItems.Crop_Too_NonRadiated.GetName((object) this);
        return this.smi.IsInsideState((StateMachine.BaseState) this.smi.sm.too_bright) ? Db.Get().CreatureStatusItems.Crop_Too_Radiated.GetName((object) this) : "";
      }
    }
  }
}
