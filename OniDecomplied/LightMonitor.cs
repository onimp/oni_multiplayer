// Decompiled with JetBrains decompiler
// Type: LightMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using UnityEngine;

public class LightMonitor : GameStateMachine<LightMonitor, LightMonitor.Instance>
{
  public const float BURN_RESIST_RECOVERY_FACTOR = 0.25f;
  public StateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.FloatParameter lightLevel;
  public StateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.FloatParameter burnResistance = new StateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.FloatParameter(120f);
  public LightMonitor.UnburntStates unburnt;
  public GameStateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.State get_burnt;
  public GameStateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.State burnt;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.unburnt;
    this.root.EventTransition(GameHashes.SicknessAdded, this.burnt, (StateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => smi.gameObject.GetSicknesses().Has(Db.Get().Sicknesses.Sunburn))).Update(new System.Action<LightMonitor.Instance, float>(LightMonitor.CheckLightLevel), (UpdateRate) 6);
    this.unburnt.DefaultState((GameStateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.State) this.unburnt.safe).ParamTransition<float>((StateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.Parameter<float>) this.burnResistance, this.get_burnt, GameStateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.IsLTEZero);
    double num1;
    this.unburnt.safe.DefaultState(this.unburnt.safe.unlit).Update((System.Action<LightMonitor.Instance, float>) ((smi, dt) => num1 = (double) smi.sm.burnResistance.DeltaClamp(dt * 0.25f, 0.0f, 120f, smi)));
    this.unburnt.safe.unlit.ParamTransition<float>((StateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.Parameter<float>) this.lightLevel, this.unburnt.safe.normal_light, GameStateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.IsGTZero);
    this.unburnt.safe.normal_light.ParamTransition<float>((StateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.Parameter<float>) this.lightLevel, this.unburnt.safe.unlit, GameStateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.IsLTEZero).ParamTransition<float>((StateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.Parameter<float>) this.lightLevel, this.unburnt.safe.sunlight, (StateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.Parameter<float>.Callback) ((smi, p) => (double) p >= 40000.0));
    this.unburnt.safe.sunlight.ParamTransition<float>((StateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.Parameter<float>) this.lightLevel, this.unburnt.safe.normal_light, (StateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.Parameter<float>.Callback) ((smi, p) => (double) p < 40000.0)).ParamTransition<float>((StateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.Parameter<float>) this.lightLevel, this.unburnt.burning, (StateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.Parameter<float>.Callback) ((smi, p) => (double) p >= 72000.0)).ToggleEffect("Sunlight_Pleasant");
    double num2;
    this.unburnt.burning.ParamTransition<float>((StateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.Parameter<float>) this.lightLevel, this.unburnt.safe.sunlight, (StateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.Parameter<float>.Callback) ((smi, p) => (double) p < 72000.0)).Update((System.Action<LightMonitor.Instance, float>) ((smi, dt) => num2 = (double) smi.sm.burnResistance.DeltaClamp(-dt, 0.0f, 120f, smi))).ToggleEffect("Sunlight_Burning");
    this.get_burnt.Enter((StateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.gameObject.GetSicknesses().Infect(new SicknessExposureInfo(Db.Get().Sicknesses.Sunburn.Id, (string) DUPLICANTS.DISEASES.SUNBURNSICKNESS.SUNEXPOSURE)))).GoTo(this.burnt);
    double num3;
    this.burnt.EventTransition(GameHashes.SicknessCured, (GameStateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.State) this.unburnt, (StateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.Transition.ConditionCallback) (smi => !smi.gameObject.GetSicknesses().Has(Db.Get().Sicknesses.Sunburn))).Exit((StateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.State.Callback) (smi => num3 = (double) smi.sm.burnResistance.Set(120f, smi)));
  }

  private static void CheckLightLevel(LightMonitor.Instance smi, float dt)
  {
    KPrefabID component = smi.GetComponent<KPrefabID>();
    if (Object.op_Inequality((Object) component, (Object) null) && component.HasTag(GameTags.Shaded))
    {
      double num1 = (double) smi.sm.lightLevel.Set(0.0f, smi);
    }
    else
    {
      int cell = Grid.PosToCell(smi.gameObject);
      if (!Grid.IsValidCell(cell))
        return;
      double num2 = (double) smi.sm.lightLevel.Set((float) Grid.LightIntensity[cell], smi);
    }
  }

  public class UnburntStates : 
    GameStateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.State
  {
    public LightMonitor.SafeStates safe;
    public GameStateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.State burning;
  }

  public class SafeStates : 
    GameStateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.State
  {
    public GameStateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.State unlit;
    public GameStateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.State normal_light;
    public GameStateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.State sunlight;
  }

  public new class Instance : 
    GameStateMachine<LightMonitor, LightMonitor.Instance, IStateMachineTarget, object>.GameInstance
  {
    public Effects effects;

    public Instance(IStateMachineTarget master)
      : base(master)
    {
      this.effects = this.GetComponent<Effects>();
    }
  }
}
