// Decompiled with JetBrains decompiler
// Type: IlluminationVulnerable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
public class IlluminationVulnerable : 
  StateMachineComponent<IlluminationVulnerable.StatesInstance>,
  IGameObjectEffectDescriptor,
  IWiltCause
{
  private OccupyArea _occupyArea;
  private SchedulerHandle handle;
  public bool prefersDarkness;
  private AttributeInstance minLuxAttributeInstance;

  public int LightIntensityThreshold => this.minLuxAttributeInstance != null ? Mathf.RoundToInt(this.minLuxAttributeInstance.GetTotalValue()) : Mathf.RoundToInt(((Component) this).GetComponent<Modifiers>().GetPreModifiedAttributeValue(Db.Get().PlantAttributes.MinLightLux));

  private OccupyArea occupyArea
  {
    get
    {
      if (Object.op_Equality((Object) this._occupyArea, (Object) null))
        this._occupyArea = ((Component) this).GetComponent<OccupyArea>();
      return this._occupyArea;
    }
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    ((Component) this).gameObject.GetAmounts().Add(new AmountInstance(Db.Get().Amounts.Illumination, ((Component) this).gameObject));
    this.minLuxAttributeInstance = ((Component) this).gameObject.GetAttributes().Add(Db.Get().PlantAttributes.MinLightLux);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
  }

  public void SetPrefersDarkness(bool prefersDarkness = false) => this.prefersDarkness = prefersDarkness;

  protected override void OnCleanUp()
  {
    this.handle.ClearScheduler();
    base.OnCleanUp();
  }

  public bool IsCellSafe(int cell)
  {
    if (!Grid.IsValidCell(cell))
      return false;
    return this.prefersDarkness ? Grid.LightIntensity[cell] == 0 : Grid.LightIntensity[cell] > this.LightIntensityThreshold;
  }

  WiltCondition.Condition[] IWiltCause.Conditions => new WiltCondition.Condition[2]
  {
    WiltCondition.Condition.Darkness,
    WiltCondition.Condition.IlluminationComfort
  };

  public string WiltStateString
  {
    get
    {
      if (this.smi.IsInsideState((StateMachine.BaseState) this.smi.sm.too_bright))
        return Db.Get().CreatureStatusItems.Crop_Too_Bright.GetName((object) this);
      return this.smi.IsInsideState((StateMachine.BaseState) this.smi.sm.too_dark) ? Db.Get().CreatureStatusItems.Crop_Too_Dark.GetName((object) this) : "";
    }
  }

  public bool IsComfortable() => this.smi.IsInsideState((StateMachine.BaseState) this.smi.sm.comfortable);

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    if (this.prefersDarkness)
    {
      List<Descriptor> descriptors = new List<Descriptor>();
      descriptors.Add(new Descriptor((string) UI.GAMEOBJECTEFFECTS.REQUIRES_DARKNESS, (string) UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_DARKNESS, (Descriptor.DescriptorType) 0, false));
      return descriptors;
    }
    List<Descriptor> descriptors1 = new List<Descriptor>();
    descriptors1.Add(new Descriptor(UI.GAMEOBJECTEFFECTS.REQUIRES_LIGHT.Replace("{Lux}", GameUtil.GetFormattedLux(this.LightIntensityThreshold)), UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_LIGHT.Replace("{Lux}", GameUtil.GetFormattedLux(this.LightIntensityThreshold)), (Descriptor.DescriptorType) 0, false));
    return descriptors1;
  }

  public class StatesInstance : 
    GameStateMachine<IlluminationVulnerable.States, IlluminationVulnerable.StatesInstance, IlluminationVulnerable, object>.GameInstance
  {
    public bool hasMaturity;

    public StatesInstance(IlluminationVulnerable master)
      : base(master)
    {
    }
  }

  public class States : 
    GameStateMachine<IlluminationVulnerable.States, IlluminationVulnerable.StatesInstance, IlluminationVulnerable>
  {
    public StateMachine<IlluminationVulnerable.States, IlluminationVulnerable.StatesInstance, IlluminationVulnerable, object>.BoolParameter illuminated;
    public GameStateMachine<IlluminationVulnerable.States, IlluminationVulnerable.StatesInstance, IlluminationVulnerable, object>.State comfortable;
    public GameStateMachine<IlluminationVulnerable.States, IlluminationVulnerable.StatesInstance, IlluminationVulnerable, object>.State too_dark;
    public GameStateMachine<IlluminationVulnerable.States, IlluminationVulnerable.StatesInstance, IlluminationVulnerable, object>.State too_bright;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.comfortable;
      this.root.Update("Illumination", (Action<IlluminationVulnerable.StatesInstance, float>) ((smi, dt) =>
      {
        int cell = Grid.PosToCell(((Component) smi.master).gameObject);
        if (Grid.IsValidCell(cell))
        {
          double num1 = (double) smi.master.GetAmounts().Get(Db.Get().Amounts.Illumination).SetValue((float) Grid.LightCount[cell]);
        }
        else
        {
          double num2 = (double) smi.master.GetAmounts().Get(Db.Get().Amounts.Illumination).SetValue(0.0f);
        }
      }), (UpdateRate) 6);
      this.comfortable.Update("Illumination.Comfortable", (Action<IlluminationVulnerable.StatesInstance, float>) ((smi, dt) =>
      {
        int cell = Grid.PosToCell(((Component) smi.master).gameObject);
        if (smi.master.IsCellSafe(cell))
          return;
        GameStateMachine<IlluminationVulnerable.States, IlluminationVulnerable.StatesInstance, IlluminationVulnerable, object>.State state = smi.master.prefersDarkness ? this.too_bright : this.too_dark;
        smi.GoTo((StateMachine.BaseState) state);
      }), (UpdateRate) 6).Enter((StateMachine<IlluminationVulnerable.States, IlluminationVulnerable.StatesInstance, IlluminationVulnerable, object>.State.Callback) (smi => smi.master.Trigger(1113102781, (object) null)));
      this.too_dark.TriggerOnEnter(GameHashes.IlluminationDiscomfort).Update("Illumination.too_dark", (Action<IlluminationVulnerable.StatesInstance, float>) ((smi, dt) =>
      {
        int cell = Grid.PosToCell(((Component) smi.master).gameObject);
        if (!smi.master.IsCellSafe(cell))
          return;
        smi.GoTo((StateMachine.BaseState) this.comfortable);
      }), (UpdateRate) 6);
      this.too_bright.TriggerOnEnter(GameHashes.IlluminationDiscomfort).Update("Illumination.too_bright", (Action<IlluminationVulnerable.StatesInstance, float>) ((smi, dt) =>
      {
        int cell = Grid.PosToCell(((Component) smi.master).gameObject);
        if (!smi.master.IsCellSafe(cell))
          return;
        smi.GoTo((StateMachine.BaseState) this.comfortable);
      }), (UpdateRate) 6);
    }
  }
}
