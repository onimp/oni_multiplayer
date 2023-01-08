// Decompiled with JetBrains decompiler
// Type: PressureVulnerable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
public class PressureVulnerable : 
  StateMachineComponent<PressureVulnerable.StatesInstance>,
  IGameObjectEffectDescriptor,
  IWiltCause,
  ISlicedSim1000ms
{
  private HandleVector<int>.Handle pressureAccumulator = HandleVector<int>.InvalidHandle;
  private HandleVector<int>.Handle elementAccumulator = HandleVector<int>.InvalidHandle;
  private OccupyArea _occupyArea;
  public float pressureLethal_Low;
  public float pressureWarning_Low;
  public float pressureWarning_High;
  public float pressureLethal_High;
  private static float testAreaPressure;
  private static int testAreaCount;
  public bool testAreaElementSafe = true;
  public Element currentAtmoElement;
  private static Func<int, object, bool> testAreaCB = (Func<int, object, bool>) ((test_cell, data) =>
  {
    PressureVulnerable pressureVulnerable = (PressureVulnerable) data;
    if (!Grid.IsSolidCell(test_cell))
    {
      Element element = Grid.Element[test_cell];
      if (pressureVulnerable.IsSafeElement(element))
      {
        PressureVulnerable.testAreaPressure += Grid.Mass[test_cell];
        ++PressureVulnerable.testAreaCount;
        pressureVulnerable.testAreaElementSafe = true;
        pressureVulnerable.currentAtmoElement = element;
      }
      if (pressureVulnerable.currentAtmoElement == null)
        pressureVulnerable.currentAtmoElement = element;
    }
    return true;
  });
  private AmountInstance displayPressureAmount;
  public bool pressure_sensitive = true;
  public HashSet<Element> safe_atmospheres = new HashSet<Element>();
  private int cell;
  private PressureVulnerable.PressureState pressureState = PressureVulnerable.PressureState.Normal;

  private OccupyArea occupyArea
  {
    get
    {
      if (Object.op_Equality((Object) this._occupyArea, (Object) null))
        this._occupyArea = ((Component) this).GetComponent<OccupyArea>();
      return this._occupyArea;
    }
  }

  public bool IsSafeElement(Element element) => this.safe_atmospheres == null || this.safe_atmospheres.Count == 0 || this.safe_atmospheres.Contains(element);

  public PressureVulnerable.PressureState ExternalPressureState => this.pressureState;

  public bool IsLethal => this.pressureState == PressureVulnerable.PressureState.LethalHigh || this.pressureState == PressureVulnerable.PressureState.LethalLow || !this.testAreaElementSafe;

  public bool IsNormal => this.testAreaElementSafe && this.pressureState == PressureVulnerable.PressureState.Normal;

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.displayPressureAmount = ((Component) this).gameObject.GetAmounts().Add(new AmountInstance(Db.Get().Amounts.AirPressure, ((Component) this).gameObject));
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    SlicedUpdaterSim1000ms<PressureVulnerable>.instance.RegisterUpdate1000ms(this);
    this.cell = Grid.PosToCell((KMonoBehaviour) this);
    double num = (double) this.smi.sm.pressure.Set(1f, this.smi);
    this.smi.sm.safe_element.Set(this.testAreaElementSafe, this.smi);
    this.smi.master.pressureAccumulator = Game.Instance.accumulators.Add("pressureAccumulator", (KMonoBehaviour) this);
    this.smi.master.elementAccumulator = Game.Instance.accumulators.Add("elementAccumulator", (KMonoBehaviour) this);
    this.smi.StartSM();
  }

  protected override void OnCleanUp()
  {
    SlicedUpdaterSim1000ms<PressureVulnerable>.instance.UnregisterUpdate1000ms(this);
    base.OnCleanUp();
  }

  public void Configure(SimHashes[] safeAtmospheres = null)
  {
    this.pressure_sensitive = false;
    this.pressureWarning_Low = float.MinValue;
    this.pressureLethal_Low = float.MinValue;
    this.pressureLethal_High = float.MaxValue;
    this.pressureWarning_High = float.MaxValue;
    this.safe_atmospheres = new HashSet<Element>();
    if (safeAtmospheres == null)
      return;
    foreach (SimHashes safeAtmosphere in safeAtmospheres)
      this.safe_atmospheres.Add(ElementLoader.FindElementByHash(safeAtmosphere));
  }

  public void Configure(
    float pressureWarningLow = 0.25f,
    float pressureLethalLow = 0.01f,
    float pressureWarningHigh = 10f,
    float pressureLethalHigh = 30f,
    SimHashes[] safeAtmospheres = null)
  {
    this.pressure_sensitive = true;
    this.pressureWarning_Low = pressureWarningLow;
    this.pressureLethal_Low = pressureLethalLow;
    this.pressureLethal_High = pressureLethalHigh;
    this.pressureWarning_High = pressureWarningHigh;
    this.safe_atmospheres = new HashSet<Element>();
    if (safeAtmospheres == null)
      return;
    foreach (SimHashes safeAtmosphere in safeAtmospheres)
      this.safe_atmospheres.Add(ElementLoader.FindElementByHash(safeAtmosphere));
  }

  WiltCondition.Condition[] IWiltCause.Conditions => new WiltCondition.Condition[2]
  {
    WiltCondition.Condition.Pressure,
    WiltCondition.Condition.AtmosphereElement
  };

  public string WiltStateString
  {
    get
    {
      string wiltStateString = "";
      if (this.smi.IsInsideState((StateMachine.BaseState) this.smi.sm.warningLow) || this.smi.IsInsideState((StateMachine.BaseState) this.smi.sm.lethalLow))
        wiltStateString += Db.Get().CreatureStatusItems.AtmosphericPressureTooLow.resolveStringCallback((string) CREATURES.STATUSITEMS.ATMOSPHERICPRESSURETOOLOW.NAME, (object) this);
      else if (this.smi.IsInsideState((StateMachine.BaseState) this.smi.sm.warningHigh) || this.smi.IsInsideState((StateMachine.BaseState) this.smi.sm.lethalHigh))
        wiltStateString += Db.Get().CreatureStatusItems.AtmosphericPressureTooHigh.resolveStringCallback((string) CREATURES.STATUSITEMS.ATMOSPHERICPRESSURETOOHIGH.NAME, (object) this);
      else if (this.smi.IsInsideState((StateMachine.BaseState) this.smi.sm.unsafeElement))
        wiltStateString += Db.Get().CreatureStatusItems.WrongAtmosphere.resolveStringCallback((string) CREATURES.STATUSITEMS.WRONGATMOSPHERE.NAME, (object) this);
      return wiltStateString;
    }
  }

  public bool IsSafePressure(float pressure)
  {
    if (!this.pressure_sensitive)
      return true;
    return (double) pressure > (double) this.pressureLethal_Low && (double) pressure < (double) this.pressureLethal_High;
  }

  public void SlicedSim1000ms(float dt)
  {
    Game.Instance.accumulators.Accumulate(this.smi.master.pressureAccumulator, this.GetPressureOverArea(this.cell));
    float averageRate = Game.Instance.accumulators.GetAverageRate(this.smi.master.pressureAccumulator);
    this.displayPressureAmount.value = averageRate;
    Game.Instance.accumulators.Accumulate(this.smi.master.elementAccumulator, this.testAreaElementSafe ? 1f : 0.0f);
    this.smi.sm.safe_element.Set((double) Game.Instance.accumulators.GetAverageRate(this.smi.master.elementAccumulator) > 0.0, this.smi);
    double num = (double) this.smi.sm.pressure.Set(averageRate, this.smi);
  }

  public float GetExternalPressure() => this.GetPressureOverArea(this.cell);

  private float GetPressureOverArea(int cell)
  {
    bool testAreaElementSafe = this.testAreaElementSafe;
    PressureVulnerable.testAreaPressure = 0.0f;
    PressureVulnerable.testAreaCount = 0;
    this.testAreaElementSafe = false;
    this.currentAtmoElement = (Element) null;
    this.occupyArea.TestArea(cell, (object) this, PressureVulnerable.testAreaCB);
    this.occupyArea.TestAreaAbove(cell, (object) this, PressureVulnerable.testAreaCB);
    PressureVulnerable.testAreaPressure = PressureVulnerable.testAreaCount > 0 ? PressureVulnerable.testAreaPressure / (float) PressureVulnerable.testAreaCount : 0.0f;
    if (this.testAreaElementSafe != testAreaElementSafe)
      this.Trigger(-2023773544, (object) null);
    return PressureVulnerable.testAreaPressure;
  }

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    if (this.pressure_sensitive)
      descriptors.Add(new Descriptor(string.Format((string) UI.GAMEOBJECTEFFECTS.REQUIRES_PRESSURE, (object) GameUtil.GetFormattedMass(this.pressureWarning_Low)), string.Format((string) UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_PRESSURE, (object) GameUtil.GetFormattedMass(this.pressureWarning_Low)), (Descriptor.DescriptorType) 0, false));
    if (this.safe_atmospheres != null && this.safe_atmospheres.Count > 0)
    {
      string str = "";
      bool flag1 = false;
      bool flag2 = false;
      foreach (Element safeAtmosphere in this.safe_atmospheres)
      {
        flag1 |= safeAtmosphere.IsGas;
        flag2 |= safeAtmosphere.IsLiquid;
        str = str + "\n        • " + safeAtmosphere.name;
      }
      if (flag1 & flag2)
        descriptors.Add(new Descriptor(string.Format((string) UI.GAMEOBJECTEFFECTS.REQUIRES_ATMOSPHERE, (object) str), string.Format((string) UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_ATMOSPHERE_MIXED, (object) str), (Descriptor.DescriptorType) 0, false));
      if (flag1)
        descriptors.Add(new Descriptor(string.Format((string) UI.GAMEOBJECTEFFECTS.REQUIRES_ATMOSPHERE, (object) str), string.Format((string) UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_ATMOSPHERE, (object) str), (Descriptor.DescriptorType) 0, false));
      else
        descriptors.Add(new Descriptor(string.Format((string) UI.GAMEOBJECTEFFECTS.REQUIRES_ATMOSPHERE, (object) str), string.Format((string) UI.GAMEOBJECTEFFECTS.TOOLTIPS.REQUIRES_ATMOSPHERE_LIQUID, (object) str), (Descriptor.DescriptorType) 0, false));
    }
    return descriptors;
  }

  public class StatesInstance : 
    GameStateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.GameInstance
  {
    public bool hasMaturity;

    public StatesInstance(PressureVulnerable master)
      : base(master)
    {
      if (Db.Get().Amounts.Maturity.Lookup(this.gameObject) == null)
        return;
      this.hasMaturity = true;
    }
  }

  public class States : 
    GameStateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable>
  {
    public StateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.FloatParameter pressure;
    public StateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.BoolParameter safe_element;
    public GameStateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.State unsafeElement;
    public GameStateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.State lethalLow;
    public GameStateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.State lethalHigh;
    public GameStateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.State warningLow;
    public GameStateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.State warningHigh;
    public GameStateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.State normal;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.normal;
      this.lethalLow.ParamTransition<float>((StateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.Parameter<float>) this.pressure, this.warningLow, (StateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.Parameter<float>.Callback) ((smi, p) => (double) p > (double) smi.master.pressureLethal_Low)).ParamTransition<bool>((StateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.Parameter<bool>) this.safe_element, this.unsafeElement, GameStateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.IsFalse).Enter((StateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.State.Callback) (smi => smi.master.pressureState = PressureVulnerable.PressureState.LethalLow)).TriggerOnEnter(GameHashes.LowPressureFatal);
      this.lethalHigh.ParamTransition<float>((StateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.Parameter<float>) this.pressure, this.warningHigh, (StateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.Parameter<float>.Callback) ((smi, p) => (double) p < (double) smi.master.pressureLethal_High)).ParamTransition<bool>((StateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.Parameter<bool>) this.safe_element, this.unsafeElement, GameStateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.IsFalse).Enter((StateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.State.Callback) (smi => smi.master.pressureState = PressureVulnerable.PressureState.LethalHigh)).TriggerOnEnter(GameHashes.HighPressureFatal);
      this.warningLow.ParamTransition<float>((StateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.Parameter<float>) this.pressure, this.lethalLow, (StateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.Parameter<float>.Callback) ((smi, p) => (double) p < (double) smi.master.pressureLethal_Low)).ParamTransition<float>((StateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.Parameter<float>) this.pressure, this.normal, (StateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.Parameter<float>.Callback) ((smi, p) => (double) p > (double) smi.master.pressureWarning_Low)).ParamTransition<bool>((StateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.Parameter<bool>) this.safe_element, this.unsafeElement, GameStateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.IsFalse).Enter((StateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.State.Callback) (smi => smi.master.pressureState = PressureVulnerable.PressureState.WarningLow)).TriggerOnEnter(GameHashes.LowPressureWarning);
      this.unsafeElement.ParamTransition<bool>((StateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.Parameter<bool>) this.safe_element, this.normal, GameStateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.IsTrue).TriggerOnExit(GameHashes.CorrectAtmosphere).TriggerOnEnter(GameHashes.WrongAtmosphere);
      this.warningHigh.ParamTransition<float>((StateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.Parameter<float>) this.pressure, this.lethalHigh, (StateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.Parameter<float>.Callback) ((smi, p) => (double) p > (double) smi.master.pressureLethal_High)).ParamTransition<float>((StateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.Parameter<float>) this.pressure, this.normal, (StateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.Parameter<float>.Callback) ((smi, p) => (double) p < (double) smi.master.pressureWarning_High)).ParamTransition<bool>((StateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.Parameter<bool>) this.safe_element, this.unsafeElement, GameStateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.IsFalse).Enter((StateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.State.Callback) (smi => smi.master.pressureState = PressureVulnerable.PressureState.WarningHigh)).TriggerOnEnter(GameHashes.HighPressureWarning);
      this.normal.ParamTransition<float>((StateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.Parameter<float>) this.pressure, this.warningHigh, (StateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.Parameter<float>.Callback) ((smi, p) => (double) p > (double) smi.master.pressureWarning_High)).ParamTransition<float>((StateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.Parameter<float>) this.pressure, this.warningLow, (StateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.Parameter<float>.Callback) ((smi, p) => (double) p < (double) smi.master.pressureWarning_Low)).ParamTransition<bool>((StateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.Parameter<bool>) this.safe_element, this.unsafeElement, GameStateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.IsFalse).Enter((StateMachine<PressureVulnerable.States, PressureVulnerable.StatesInstance, PressureVulnerable, object>.State.Callback) (smi => smi.master.pressureState = PressureVulnerable.PressureState.Normal)).TriggerOnEnter(GameHashes.OptimalPressureAchieved);
    }
  }

  public enum PressureState
  {
    LethalLow,
    WarningLow,
    Normal,
    WarningHigh,
    LethalHigh,
  }
}
