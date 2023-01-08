// Decompiled with JetBrains decompiler
// Type: CropSleepingMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CropSleepingMonitor : 
  GameStateMachine<CropSleepingMonitor, CropSleepingMonitor.Instance, IStateMachineTarget, CropSleepingMonitor.Def>
{
  public GameStateMachine<CropSleepingMonitor, CropSleepingMonitor.Instance, IStateMachineTarget, CropSleepingMonitor.Def>.State sleeping;
  public GameStateMachine<CropSleepingMonitor, CropSleepingMonitor.Instance, IStateMachineTarget, CropSleepingMonitor.Def>.State awake;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.awake;
    this.serializable = StateMachine.SerializeType.Never;
    this.root.Update("CropSleepingMonitor.root", (System.Action<CropSleepingMonitor.Instance, float>) ((smi, dt) =>
    {
      int cell = Grid.PosToCell(smi.master.gameObject);
      GameStateMachine<CropSleepingMonitor, CropSleepingMonitor.Instance, IStateMachineTarget, CropSleepingMonitor.Def>.State state = smi.IsCellSafe(cell) ? this.awake : this.sleeping;
      smi.GoTo((StateMachine.BaseState) state);
    }), (UpdateRate) 6);
    this.sleeping.TriggerOnEnter(GameHashes.CropSleep).ToggleStatusItem(Db.Get().CreatureStatusItems.CropSleeping, (Func<CropSleepingMonitor.Instance, object>) (smi => (object) smi));
    this.awake.TriggerOnEnter(GameHashes.CropWakeUp);
  }

  public class Def : StateMachine.BaseDef, IGameObjectEffectDescriptor
  {
    public bool prefersDarkness;

    public List<Descriptor> GetDescriptors(GameObject obj)
    {
      if (this.prefersDarkness)
      {
        List<Descriptor> descriptors = new List<Descriptor>();
        descriptors.Add(new Descriptor((string) UI.GAMEOBJECTEFFECTS.REQUIRES_DARKNESS, (string) UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_DARKNESS, (Descriptor.DescriptorType) 0, false));
        return descriptors;
      }
      Klei.AI.Attribute minLightLux = Db.Get().PlantAttributes.MinLightLux;
      AttributeInstance attributeInstance = minLightLux.Lookup(obj);
      int lux = Mathf.RoundToInt(attributeInstance != null ? attributeInstance.GetTotalValue() : obj.GetComponent<Modifiers>().GetPreModifiedAttributeValue(minLightLux));
      List<Descriptor> descriptors1 = new List<Descriptor>();
      descriptors1.Add(new Descriptor(UI.GAMEOBJECTEFFECTS.REQUIRES_LIGHT.Replace("{Lux}", GameUtil.GetFormattedLux(lux)), UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_LIGHT.Replace("{Lux}", GameUtil.GetFormattedLux(lux)), (Descriptor.DescriptorType) 0, false));
      return descriptors1;
    }
  }

  public new class Instance : 
    GameStateMachine<CropSleepingMonitor, CropSleepingMonitor.Instance, IStateMachineTarget, CropSleepingMonitor.Def>.GameInstance
  {
    public Instance(IStateMachineTarget master, CropSleepingMonitor.Def def)
      : base(master, def)
    {
    }

    public bool IsSleeping() => this.GetCurrentState() == this.smi.sm.sleeping;

    public bool IsCellSafe(int cell)
    {
      AttributeInstance attributeInstance = Db.Get().PlantAttributes.MinLightLux.Lookup(this.gameObject);
      int num = Grid.LightIntensity[cell];
      return !this.def.prefersDarkness ? (double) num >= (double) attributeInstance.GetTotalValue() : num == 0;
    }
  }
}
