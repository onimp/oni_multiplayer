// Decompiled with JetBrains decompiler
// Type: HighEnergyParticleSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using UnityEngine;

[SerializationConfig]
public class HighEnergyParticleSpawner : 
  StateMachineComponent<HighEnergyParticleSpawner.StatesInstance>,
  IHighEnergyParticleDirection,
  IProgressBarSideScreen,
  ISingleSliderControl,
  ISliderControl
{
  [MyCmpReq]
  private HighEnergyParticleStorage particleStorage;
  [MyCmpGet]
  private Operational operational;
  private float recentPerSecondConsumptionRate;
  public int minSlider;
  public int maxSlider;
  [Serialize]
  private EightDirection _direction;
  public float minLaunchInterval;
  public float radiationSampleRate;
  [Serialize]
  public float particleThreshold = 50f;
  private EightDirectionController directionController;
  private float launcherTimer;
  private float radiationSampleTimer;
  private MeterController particleController;
  private bool particleVisualPlaying;
  private MeterController progressMeterController;
  [Serialize]
  public Ref<HighEnergyParticlePort> capturedByRef = new Ref<HighEnergyParticlePort>();
  [MyCmpAdd]
  private CopyBuildingSettings copyBuildingSettings;
  private static readonly EventSystem.IntraObjectHandler<HighEnergyParticleSpawner> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<HighEnergyParticleSpawner>((Action<HighEnergyParticleSpawner, object>) ((component, data) => component.OnCopySettings(data)));

  public float PredictedPerCycleConsumptionRate => (float) Mathf.FloorToInt((float) ((double) this.recentPerSecondConsumptionRate * 0.10000000149011612 * 600.0));

  public EightDirection Direction
  {
    get => this._direction;
    set
    {
      this._direction = value;
      if (this.directionController == null)
        return;
      this.directionController.SetRotation((float) (45 * EightDirectionUtil.GetDirectionIndex(this._direction)));
      this.directionController.controller.enabled = false;
      this.directionController.controller.enabled = true;
    }
  }

  private void OnCopySettings(object data)
  {
    HighEnergyParticleSpawner component = ((GameObject) data).GetComponent<HighEnergyParticleSpawner>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    this.Direction = component.Direction;
    this.particleThreshold = component.particleThreshold;
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<HighEnergyParticleSpawner>(-905833192, HighEnergyParticleSpawner.OnCopySettingsDelegate);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
    this.directionController = new EightDirectionController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "redirector_target", "redirect", EightDirectionController.Offset.Infront);
    this.Direction = this.Direction;
    this.particleController = new MeterController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "orb_target", "orb_off", Meter.Offset.NoChange, Grid.SceneLayer.NoLayer, Array.Empty<string>());
    this.particleController.gameObject.AddOrGet<LoopingSounds>();
    this.progressMeterController = new MeterController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
    Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Radiation);
  }

  public float GetProgressBarMaxValue() => this.particleThreshold;

  public float GetProgressBarFillPercentage() => this.particleStorage.Particles / this.particleThreshold;

  public string GetProgressBarTitleLabel() => (string) UI.UISIDESCREENS.RADBOLTTHRESHOLDSIDESCREEN.PROGRESS_BAR_LABEL;

  public string GetProgressBarLabel()
  {
    int num = Mathf.FloorToInt(this.particleStorage.Particles);
    string str1 = num.ToString();
    num = Mathf.FloorToInt(this.particleThreshold);
    string str2 = num.ToString();
    return str1 + "/" + str2;
  }

  public string GetProgressBarTooltip() => (string) UI.UISIDESCREENS.RADBOLTTHRESHOLDSIDESCREEN.PROGRESS_BAR_TOOLTIP;

  public void DoConsumeParticlesWhileDisabled(float dt)
  {
    double num = (double) this.particleStorage.ConsumeAndGet(dt * 1f);
    this.progressMeterController.SetPositionPercent(this.GetProgressBarFillPercentage());
  }

  public void LauncherUpdate(float dt)
  {
    this.radiationSampleTimer += dt;
    if ((double) this.radiationSampleTimer >= (double) this.radiationSampleRate)
    {
      this.radiationSampleTimer -= this.radiationSampleRate;
      int cell = Grid.PosToCell((KMonoBehaviour) this);
      float num1 = Grid.Radiation[cell];
      if ((double) num1 != 0.0 && (double) this.particleStorage.RemainingCapacity() > 0.0)
      {
        this.smi.sm.isAbsorbingRadiation.Set(true, this.smi);
        this.recentPerSecondConsumptionRate = num1 / 600f;
        double num2 = (double) this.particleStorage.Store((float) ((double) this.recentPerSecondConsumptionRate * (double) this.radiationSampleRate * 0.10000000149011612));
      }
      else
      {
        this.recentPerSecondConsumptionRate = 0.0f;
        this.smi.sm.isAbsorbingRadiation.Set(false, this.smi);
      }
    }
    this.progressMeterController.SetPositionPercent(this.GetProgressBarFillPercentage());
    if (!this.particleVisualPlaying && (double) this.particleStorage.Particles > (double) this.particleThreshold / 2.0)
    {
      this.particleController.meterController.Play(HashedString.op_Implicit("orb_pre"));
      this.particleController.meterController.Queue(HashedString.op_Implicit("orb_idle"), (KAnim.PlayMode) 0);
      this.particleVisualPlaying = true;
    }
    this.launcherTimer += dt;
    if ((double) this.launcherTimer < (double) this.minLaunchInterval || (double) this.particleStorage.Particles < (double) this.particleThreshold)
      return;
    this.launcherTimer = 0.0f;
    int particleOutputCell = ((Component) this).GetComponent<Building>().GetHighEnergyParticleOutputCell();
    GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(Tag.op_Implicit("HighEnergyParticle")), Grid.CellToPosCCC(particleOutputCell, Grid.SceneLayer.FXFront2), Grid.SceneLayer.FXFront2);
    gameObject.SetActive(true);
    if (!Object.op_Inequality((Object) gameObject, (Object) null))
      return;
    HighEnergyParticle component = gameObject.GetComponent<HighEnergyParticle>();
    component.payload = this.particleStorage.ConsumeAndGet(this.particleThreshold);
    component.SetDirection(this.Direction);
    this.directionController.PlayAnim("redirect_send");
    this.directionController.controller.Queue(HashedString.op_Implicit("redirect"));
    this.particleController.meterController.Play(HashedString.op_Implicit("orb_send"));
    this.particleController.meterController.Queue(HashedString.op_Implicit("orb_off"));
    this.particleVisualPlaying = false;
  }

  public string SliderTitleKey => "STRINGS.UI.UISIDESCREENS.RADBOLTTHRESHOLDSIDESCREEN.TITLE";

  public string SliderUnits => (string) UI.UNITSUFFIXES.HIGHENERGYPARTICLES.PARTRICLES;

  public int SliderDecimalPlaces(int index) => 0;

  public float GetSliderMin(int index) => (float) this.minSlider;

  public float GetSliderMax(int index) => (float) this.maxSlider;

  public float GetSliderValue(int index) => this.particleThreshold;

  public void SetSliderValue(float value, int index) => this.particleThreshold = value;

  public string GetSliderTooltipKey(int index) => "STRINGS.UI.UISIDESCREENS.RADBOLTTHRESHOLDSIDESCREEN.TOOLTIP";

  string ISliderControl.GetSliderTooltip() => string.Format(StringEntry.op_Implicit(Strings.Get("STRINGS.UI.UISIDESCREENS.RADBOLTTHRESHOLDSIDESCREEN.TOOLTIP")), (object) this.particleThreshold);

  public class StatesInstance : 
    GameStateMachine<HighEnergyParticleSpawner.States, HighEnergyParticleSpawner.StatesInstance, HighEnergyParticleSpawner, object>.GameInstance
  {
    public StatesInstance(HighEnergyParticleSpawner smi)
      : base(smi)
    {
    }
  }

  public class States : 
    GameStateMachine<HighEnergyParticleSpawner.States, HighEnergyParticleSpawner.StatesInstance, HighEnergyParticleSpawner>
  {
    public StateMachine<HighEnergyParticleSpawner.States, HighEnergyParticleSpawner.StatesInstance, HighEnergyParticleSpawner, object>.BoolParameter isAbsorbingRadiation;
    public HighEnergyParticleSpawner.States.ReadyStates ready;
    public HighEnergyParticleSpawner.States.InoperationalStates inoperational;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.inoperational;
      this.inoperational.PlayAnim("off").TagTransition(GameTags.Operational, (GameStateMachine<HighEnergyParticleSpawner.States, HighEnergyParticleSpawner.StatesInstance, HighEnergyParticleSpawner, object>.State) this.ready).DefaultState(this.inoperational.empty);
      this.inoperational.empty.EventTransition(GameHashes.OnParticleStorageChanged, this.inoperational.losing, (StateMachine<HighEnergyParticleSpawner.States, HighEnergyParticleSpawner.StatesInstance, HighEnergyParticleSpawner, object>.Transition.ConditionCallback) (smi => !smi.GetComponent<HighEnergyParticleStorage>().IsEmpty()));
      this.inoperational.losing.ToggleStatusItem(Db.Get().BuildingStatusItems.LosingRadbolts).Update((Action<HighEnergyParticleSpawner.StatesInstance, float>) ((smi, dt) => smi.master.DoConsumeParticlesWhileDisabled(dt)), (UpdateRate) 6).EventTransition(GameHashes.OnParticleStorageChanged, this.inoperational.empty, (StateMachine<HighEnergyParticleSpawner.States, HighEnergyParticleSpawner.StatesInstance, HighEnergyParticleSpawner, object>.Transition.ConditionCallback) (smi => smi.GetComponent<HighEnergyParticleStorage>().IsEmpty()));
      this.ready.TagTransition(GameTags.Operational, (GameStateMachine<HighEnergyParticleSpawner.States, HighEnergyParticleSpawner.StatesInstance, HighEnergyParticleSpawner, object>.State) this.inoperational, true).DefaultState(this.ready.idle).Update((Action<HighEnergyParticleSpawner.StatesInstance, float>) ((smi, dt) => smi.master.LauncherUpdate(dt)), (UpdateRate) 3);
      this.ready.idle.ParamTransition<bool>((StateMachine<HighEnergyParticleSpawner.States, HighEnergyParticleSpawner.StatesInstance, HighEnergyParticleSpawner, object>.Parameter<bool>) this.isAbsorbingRadiation, this.ready.absorbing, GameStateMachine<HighEnergyParticleSpawner.States, HighEnergyParticleSpawner.StatesInstance, HighEnergyParticleSpawner, object>.IsTrue).PlayAnim("on");
      this.ready.absorbing.Enter("SetActive(true)", (StateMachine<HighEnergyParticleSpawner.States, HighEnergyParticleSpawner.StatesInstance, HighEnergyParticleSpawner, object>.State.Callback) (smi => smi.master.operational.SetActive(true))).Exit("SetActive(false)", (StateMachine<HighEnergyParticleSpawner.States, HighEnergyParticleSpawner.StatesInstance, HighEnergyParticleSpawner, object>.State.Callback) (smi => smi.master.operational.SetActive(false))).ParamTransition<bool>((StateMachine<HighEnergyParticleSpawner.States, HighEnergyParticleSpawner.StatesInstance, HighEnergyParticleSpawner, object>.Parameter<bool>) this.isAbsorbingRadiation, this.ready.idle, GameStateMachine<HighEnergyParticleSpawner.States, HighEnergyParticleSpawner.StatesInstance, HighEnergyParticleSpawner, object>.IsFalse).ToggleStatusItem(Db.Get().BuildingStatusItems.CollectingHEP, (Func<HighEnergyParticleSpawner.StatesInstance, object>) (smi => (object) smi.master)).PlayAnim("working_loop", (KAnim.PlayMode) 0);
    }

    public class InoperationalStates : 
      GameStateMachine<HighEnergyParticleSpawner.States, HighEnergyParticleSpawner.StatesInstance, HighEnergyParticleSpawner, object>.State
    {
      public GameStateMachine<HighEnergyParticleSpawner.States, HighEnergyParticleSpawner.StatesInstance, HighEnergyParticleSpawner, object>.State empty;
      public GameStateMachine<HighEnergyParticleSpawner.States, HighEnergyParticleSpawner.StatesInstance, HighEnergyParticleSpawner, object>.State losing;
    }

    public class ReadyStates : 
      GameStateMachine<HighEnergyParticleSpawner.States, HighEnergyParticleSpawner.StatesInstance, HighEnergyParticleSpawner, object>.State
    {
      public GameStateMachine<HighEnergyParticleSpawner.States, HighEnergyParticleSpawner.StatesInstance, HighEnergyParticleSpawner, object>.State idle;
      public GameStateMachine<HighEnergyParticleSpawner.States, HighEnergyParticleSpawner.StatesInstance, HighEnergyParticleSpawner, object>.State absorbing;
    }
  }
}
