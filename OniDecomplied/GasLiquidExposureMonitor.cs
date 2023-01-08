// Decompiled with JetBrains decompiler
// Type: GasLiquidExposureMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GasLiquidExposureMonitor : 
  GameStateMachine<GasLiquidExposureMonitor, GasLiquidExposureMonitor.Instance, IStateMachineTarget, GasLiquidExposureMonitor.Def>
{
  public const float MIN_REACT_INTERVAL = 60f;
  private static Dictionary<SimHashes, float> customExposureRates;
  private static Effect minorIrritationEffect;
  private static Effect majorIrritationEffect;
  public StateMachine<GasLiquidExposureMonitor, GasLiquidExposureMonitor.Instance, IStateMachineTarget, GasLiquidExposureMonitor.Def>.BoolParameter isIrritated;
  public StateMachine<GasLiquidExposureMonitor, GasLiquidExposureMonitor.Instance, IStateMachineTarget, GasLiquidExposureMonitor.Def>.Signal reactFinished;
  public GameStateMachine<GasLiquidExposureMonitor, GasLiquidExposureMonitor.Instance, IStateMachineTarget, GasLiquidExposureMonitor.Def>.State normal;
  public GasLiquidExposureMonitor.IrritatedStates irritated;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.normal;
    this.root.Update(new System.Action<GasLiquidExposureMonitor.Instance, float>(this.UpdateExposure), (UpdateRate) 4);
    this.normal.ParamTransition<bool>((StateMachine<GasLiquidExposureMonitor, GasLiquidExposureMonitor.Instance, IStateMachineTarget, GasLiquidExposureMonitor.Def>.Parameter<bool>) this.isIrritated, (GameStateMachine<GasLiquidExposureMonitor, GasLiquidExposureMonitor.Instance, IStateMachineTarget, GasLiquidExposureMonitor.Def>.State) this.irritated, (StateMachine<GasLiquidExposureMonitor, GasLiquidExposureMonitor.Instance, IStateMachineTarget, GasLiquidExposureMonitor.Def>.Parameter<bool>.Callback) ((smi, p) => this.isIrritated.Get(smi)));
    this.irritated.ParamTransition<bool>((StateMachine<GasLiquidExposureMonitor, GasLiquidExposureMonitor.Instance, IStateMachineTarget, GasLiquidExposureMonitor.Def>.Parameter<bool>) this.isIrritated, this.normal, (StateMachine<GasLiquidExposureMonitor, GasLiquidExposureMonitor.Instance, IStateMachineTarget, GasLiquidExposureMonitor.Def>.Parameter<bool>.Callback) ((smi, p) => !this.isIrritated.Get(smi))).ToggleStatusItem(Db.Get().DuplicantStatusItems.GasLiquidIrritation, (Func<GasLiquidExposureMonitor.Instance, object>) (smi => (object) smi)).DefaultState(this.irritated.irritated);
    this.irritated.irritated.Transition(this.irritated.rubbingEyes, new StateMachine<GasLiquidExposureMonitor, GasLiquidExposureMonitor.Instance, IStateMachineTarget, GasLiquidExposureMonitor.Def>.Transition.ConditionCallback(GasLiquidExposureMonitor.CanReact));
    this.irritated.rubbingEyes.Exit((StateMachine<GasLiquidExposureMonitor, GasLiquidExposureMonitor.Instance, IStateMachineTarget, GasLiquidExposureMonitor.Def>.State.Callback) (smi => smi.lastReactTime = GameClock.Instance.GetTime())).ToggleReactable((Func<GasLiquidExposureMonitor.Instance, Reactable>) (smi => smi.GetReactable())).OnSignal(this.reactFinished, this.irritated.irritated);
  }

  private static bool CanReact(GasLiquidExposureMonitor.Instance smi) => (double) GameClock.Instance.GetTime() > (double) smi.lastReactTime + 60.0;

  private static void InitializeCustomRates()
  {
    if (GasLiquidExposureMonitor.customExposureRates != null)
      return;
    GasLiquidExposureMonitor.minorIrritationEffect = Db.Get().effects.Get("MinorIrritation");
    GasLiquidExposureMonitor.majorIrritationEffect = Db.Get().effects.Get("MajorIrritation");
    GasLiquidExposureMonitor.customExposureRates = new Dictionary<SimHashes, float>();
    float num1 = -1f;
    GasLiquidExposureMonitor.customExposureRates[SimHashes.Water] = num1;
    float num2 = -0.25f;
    GasLiquidExposureMonitor.customExposureRates[SimHashes.CarbonDioxide] = num2;
    GasLiquidExposureMonitor.customExposureRates[SimHashes.Oxygen] = num2;
    float num3 = 0.0f;
    GasLiquidExposureMonitor.customExposureRates[SimHashes.ContaminatedOxygen] = num3;
    GasLiquidExposureMonitor.customExposureRates[SimHashes.DirtyWater] = num3;
    GasLiquidExposureMonitor.customExposureRates[SimHashes.ViscoGel] = num3;
    float num4 = 0.5f;
    GasLiquidExposureMonitor.customExposureRates[SimHashes.Hydrogen] = num4;
    GasLiquidExposureMonitor.customExposureRates[SimHashes.SaltWater] = num4;
    float num5 = 1f;
    GasLiquidExposureMonitor.customExposureRates[SimHashes.ChlorineGas] = num5;
    GasLiquidExposureMonitor.customExposureRates[SimHashes.EthanolGas] = num5;
    float num6 = 3f;
    GasLiquidExposureMonitor.customExposureRates[SimHashes.Chlorine] = num6;
    GasLiquidExposureMonitor.customExposureRates[SimHashes.SourGas] = num6;
    GasLiquidExposureMonitor.customExposureRates[SimHashes.Brine] = num6;
    GasLiquidExposureMonitor.customExposureRates[SimHashes.Ethanol] = num6;
    GasLiquidExposureMonitor.customExposureRates[SimHashes.SuperCoolant] = num6;
    GasLiquidExposureMonitor.customExposureRates[SimHashes.CrudeOil] = num6;
    GasLiquidExposureMonitor.customExposureRates[SimHashes.Naphtha] = num6;
    GasLiquidExposureMonitor.customExposureRates[SimHashes.Petroleum] = num6;
  }

  public float GetCurrentExposure(GasLiquidExposureMonitor.Instance smi)
  {
    float num;
    return GasLiquidExposureMonitor.customExposureRates.TryGetValue(smi.CurrentlyExposedToElement().id, out num) ? num : 0.0f;
  }

  private void UpdateExposure(GasLiquidExposureMonitor.Instance smi, float dt)
  {
    GasLiquidExposureMonitor.InitializeCustomRates();
    float num1 = 0.0f;
    smi.isInAirtightEnvironment = false;
    smi.isImmuneToIrritability = false;
    int index = Grid.CellAbove(Grid.PosToCell(smi.gameObject));
    if (Grid.IsValidCell(index))
    {
      Element element = Grid.Element[index];
      float num2;
      if (!GasLiquidExposureMonitor.customExposureRates.TryGetValue(element.id, out num2))
        num2 = (double) Grid.Temperature[index] < -13657.5 || (double) Grid.Temperature[index] > 27315.0 ? 2f : 1f;
      if (smi.effects.HasImmunityTo(GasLiquidExposureMonitor.minorIrritationEffect) || smi.effects.HasImmunityTo(GasLiquidExposureMonitor.majorIrritationEffect))
      {
        smi.isImmuneToIrritability = true;
        num1 = GasLiquidExposureMonitor.customExposureRates[SimHashes.Oxygen];
      }
      if (smi.master.gameObject.HasTag(GameTags.HasSuitTank) && Object.op_Implicit((Object) smi.gameObject.GetComponent<SuitEquipper>().IsWearingAirtightSuit()) || smi.master.gameObject.HasTag(GameTags.InTransitTube))
      {
        smi.isInAirtightEnvironment = true;
        num1 = GasLiquidExposureMonitor.customExposureRates[SimHashes.Oxygen];
      }
      if (!smi.isInAirtightEnvironment && !smi.isImmuneToIrritability)
      {
        if (element.IsGas)
          num1 = (float) ((double) num2 * (double) Grid.Mass[index] / 1.0);
        else if (element.IsLiquid)
          num1 = (float) ((double) num2 * (double) Grid.Mass[index] / 1000.0);
      }
    }
    smi.exposureRate = num1;
    smi.exposure += smi.exposureRate * dt;
    smi.exposure = MathUtil.Clamp(0.0f, 30f, smi.exposure);
    this.ApplyEffects(smi);
  }

  private void ApplyEffects(GasLiquidExposureMonitor.Instance smi)
  {
    if (smi.IsMinorIrritation())
    {
      if (smi.effects.Add(GasLiquidExposureMonitor.minorIrritationEffect, true) == null)
        return;
      this.isIrritated.Set(true, smi);
    }
    else if (smi.IsMajorIrritation())
    {
      if (smi.effects.Add(GasLiquidExposureMonitor.majorIrritationEffect, true) == null)
        return;
      this.isIrritated.Set(true, smi);
    }
    else
    {
      smi.effects.Remove(GasLiquidExposureMonitor.minorIrritationEffect);
      smi.effects.Remove(GasLiquidExposureMonitor.majorIrritationEffect);
      this.isIrritated.Set(false, smi);
    }
  }

  public Effect GetAppliedEffect(GasLiquidExposureMonitor.Instance smi)
  {
    if (smi.IsMinorIrritation())
      return GasLiquidExposureMonitor.minorIrritationEffect;
    return smi.IsMajorIrritation() ? GasLiquidExposureMonitor.majorIrritationEffect : (Effect) null;
  }

  public class Def : StateMachine.BaseDef
  {
  }

  public class TUNING
  {
    public const float MINOR_IRRITATION_THRESHOLD = 8f;
    public const float MAJOR_IRRITATION_THRESHOLD = 15f;
    public const float MAX_EXPOSURE = 30f;
    public const float GAS_UNITS = 1f;
    public const float LIQUID_UNITS = 1000f;
    public const float REDUCE_EXPOSURE_RATE_FAST = -1f;
    public const float REDUCE_EXPOSURE_RATE_SLOW = -0.25f;
    public const float NO_CHANGE = 0.0f;
    public const float SLOW_EXPOSURE_RATE = 0.5f;
    public const float NORMAL_EXPOSURE_RATE = 1f;
    public const float QUICK_EXPOSURE_RATE = 3f;
    public const float DEFAULT_MIN_TEMPERATURE = -13657.5f;
    public const float DEFAULT_MAX_TEMPERATURE = 27315f;
    public const float DEFAULT_LOW_RATE = 1f;
    public const float DEFAULT_HIGH_RATE = 2f;
  }

  public class IrritatedStates : 
    GameStateMachine<GasLiquidExposureMonitor, GasLiquidExposureMonitor.Instance, IStateMachineTarget, GasLiquidExposureMonitor.Def>.State
  {
    public GameStateMachine<GasLiquidExposureMonitor, GasLiquidExposureMonitor.Instance, IStateMachineTarget, GasLiquidExposureMonitor.Def>.State irritated;
    public GameStateMachine<GasLiquidExposureMonitor, GasLiquidExposureMonitor.Instance, IStateMachineTarget, GasLiquidExposureMonitor.Def>.State rubbingEyes;
  }

  public new class Instance : 
    GameStateMachine<GasLiquidExposureMonitor, GasLiquidExposureMonitor.Instance, IStateMachineTarget, GasLiquidExposureMonitor.Def>.GameInstance
  {
    [Serialize]
    public float exposure;
    [Serialize]
    public float lastReactTime;
    [Serialize]
    public float exposureRate;
    public Effects effects;
    public bool isInAirtightEnvironment;
    public bool isImmuneToIrritability;

    public float minorIrritationThreshold => 8f;

    public Instance(IStateMachineTarget master, GasLiquidExposureMonitor.Def def)
      : base(master, def)
    {
      this.effects = master.GetComponent<Effects>();
    }

    public Reactable GetReactable()
    {
      Emote iritatedEyes = Db.Get().Emotes.Minion.IritatedEyes;
      SelfEmoteReactable reactable = new SelfEmoteReactable(this.master.gameObject, HashedString.op_Implicit("IrritatedEyes"), Db.Get().ChoreTypes.Cough, localCooldown: 0.0f);
      reactable.SetEmote(iritatedEyes);
      reactable.preventChoreInterruption = true;
      reactable.RegisterEmoteStepCallbacks(HashedString.op_Implicit("irritated_eyes"), (System.Action<GameObject>) null, (System.Action<GameObject>) (go => this.sm.reactFinished.Trigger(this)));
      return (Reactable) reactable;
    }

    public bool IsMinorIrritation() => (double) this.exposure >= 8.0 && (double) this.exposure < 15.0;

    public bool IsMajorIrritation() => (double) this.exposure >= 15.0;

    public Element CurrentlyExposedToElement()
    {
      if (this.isInAirtightEnvironment)
        return ElementLoader.GetElement(SimHashes.Oxygen.CreateTag());
      int index = Grid.CellAbove(Grid.PosToCell(this.smi.gameObject));
      return Grid.Element[index];
    }

    public void ResetExposure() => this.exposure = 0.0f;
  }
}
