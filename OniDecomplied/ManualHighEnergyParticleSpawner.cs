// Decompiled with JetBrains decompiler
// Type: ManualHighEnergyParticleSpawner
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using UnityEngine;

[SerializationConfig]
public class ManualHighEnergyParticleSpawner : 
  StateMachineComponent<ManualHighEnergyParticleSpawner.StatesInstance>,
  IHighEnergyParticleDirection
{
  [MyCmpReq]
  private HighEnergyParticleStorage particleStorage;
  [MyCmpGet]
  private RadiationEmitter radiationEmitter;
  [Serialize]
  private EightDirection _direction;
  private EightDirectionController directionController;
  [MyCmpAdd]
  private CopyBuildingSettings copyBuildingSettings;
  private static readonly EventSystem.IntraObjectHandler<ManualHighEnergyParticleSpawner> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<ManualHighEnergyParticleSpawner>((Action<ManualHighEnergyParticleSpawner, object>) ((component, data) => component.OnCopySettings(data)));

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

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<ManualHighEnergyParticleSpawner>(-905833192, ManualHighEnergyParticleSpawner.OnCopySettingsDelegate);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
    this.radiationEmitter.SetEmitting(false);
    this.directionController = new EightDirectionController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "redirector_target", "redirect", EightDirectionController.Offset.Infront);
    this.Direction = this.Direction;
    Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Radiation);
  }

  private void OnCopySettings(object data)
  {
    ManualHighEnergyParticleSpawner component = ((GameObject) data).GetComponent<ManualHighEnergyParticleSpawner>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    this.Direction = component.Direction;
  }

  public void LauncherUpdate()
  {
    if ((double) this.particleStorage.Particles <= 0.0)
      return;
    int particleOutputCell = ((Component) this).GetComponent<Building>().GetHighEnergyParticleOutputCell();
    GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(Tag.op_Implicit("HighEnergyParticle")), Grid.CellToPosCCC(particleOutputCell, Grid.SceneLayer.FXFront2), Grid.SceneLayer.FXFront2);
    gameObject.SetActive(true);
    if (!Object.op_Inequality((Object) gameObject, (Object) null))
      return;
    HighEnergyParticle component = gameObject.GetComponent<HighEnergyParticle>();
    component.payload = this.particleStorage.ConsumeAndGet(this.particleStorage.Particles);
    component.SetDirection(this.Direction);
    this.directionController.PlayAnim("redirect_send");
    this.directionController.controller.Queue(HashedString.op_Implicit("redirect"));
  }

  public class StatesInstance : 
    GameStateMachine<ManualHighEnergyParticleSpawner.States, ManualHighEnergyParticleSpawner.StatesInstance, ManualHighEnergyParticleSpawner, object>.GameInstance
  {
    public StatesInstance(ManualHighEnergyParticleSpawner smi)
      : base(smi)
    {
    }

    public bool IsComplexFabricatorWorkable(object data) => Object.op_Inequality((Object) (data as ComplexFabricatorWorkable), (Object) null);
  }

  public class States : 
    GameStateMachine<ManualHighEnergyParticleSpawner.States, ManualHighEnergyParticleSpawner.StatesInstance, ManualHighEnergyParticleSpawner>
  {
    public GameStateMachine<ManualHighEnergyParticleSpawner.States, ManualHighEnergyParticleSpawner.StatesInstance, ManualHighEnergyParticleSpawner, object>.State inoperational;
    public ManualHighEnergyParticleSpawner.States.ReadyStates ready;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.inoperational;
      this.inoperational.Enter((StateMachine<ManualHighEnergyParticleSpawner.States, ManualHighEnergyParticleSpawner.StatesInstance, ManualHighEnergyParticleSpawner, object>.State.Callback) (smi => smi.master.radiationEmitter.SetEmitting(false))).TagTransition(GameTags.Operational, (GameStateMachine<ManualHighEnergyParticleSpawner.States, ManualHighEnergyParticleSpawner.StatesInstance, ManualHighEnergyParticleSpawner, object>.State) this.ready);
      this.ready.DefaultState(this.ready.idle).TagTransition(GameTags.Operational, this.inoperational, true).Update((Action<ManualHighEnergyParticleSpawner.StatesInstance, float>) ((smi, dt) => smi.master.LauncherUpdate()));
      this.ready.idle.EventHandlerTransition(GameHashes.WorkableStartWork, this.ready.working, (Func<ManualHighEnergyParticleSpawner.StatesInstance, object, bool>) ((smi, data) => smi.IsComplexFabricatorWorkable(data)));
      this.ready.working.Enter((StateMachine<ManualHighEnergyParticleSpawner.States, ManualHighEnergyParticleSpawner.StatesInstance, ManualHighEnergyParticleSpawner, object>.State.Callback) (smi => smi.master.radiationEmitter.SetEmitting(true))).EventHandlerTransition(GameHashes.WorkableCompleteWork, this.ready.idle, (Func<ManualHighEnergyParticleSpawner.StatesInstance, object, bool>) ((smi, data) => smi.IsComplexFabricatorWorkable(data))).EventHandlerTransition(GameHashes.WorkableStopWork, this.ready.idle, (Func<ManualHighEnergyParticleSpawner.StatesInstance, object, bool>) ((smi, data) => smi.IsComplexFabricatorWorkable(data))).Exit((StateMachine<ManualHighEnergyParticleSpawner.States, ManualHighEnergyParticleSpawner.StatesInstance, ManualHighEnergyParticleSpawner, object>.State.Callback) (smi => smi.master.radiationEmitter.SetEmitting(false)));
    }

    public class ReadyStates : 
      GameStateMachine<ManualHighEnergyParticleSpawner.States, ManualHighEnergyParticleSpawner.StatesInstance, ManualHighEnergyParticleSpawner, object>.State
    {
      public GameStateMachine<ManualHighEnergyParticleSpawner.States, ManualHighEnergyParticleSpawner.StatesInstance, ManualHighEnergyParticleSpawner, object>.State idle;
      public GameStateMachine<ManualHighEnergyParticleSpawner.States, ManualHighEnergyParticleSpawner.StatesInstance, ManualHighEnergyParticleSpawner, object>.State working;
    }
  }
}
