// Decompiled with JetBrains decompiler
// Type: HighEnergyParticleRedirector
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using UnityEngine;

[SerializationConfig]
public class HighEnergyParticleRedirector : 
  StateMachineComponent<HighEnergyParticleRedirector.StatesInstance>,
  IHighEnergyParticleDirection
{
  public static readonly HashedString PORT_ID = HashedString.op_Implicit("HEPRedirector");
  [MyCmpReq]
  private KSelectable selectable;
  [MyCmpReq]
  private HighEnergyParticleStorage storage;
  [MyCmpGet]
  private HighEnergyParticlePort port;
  public float directorDelay;
  public bool directionControllable = true;
  [Serialize]
  private EightDirection _direction;
  private EightDirectionController directionController;
  [MyCmpAdd]
  private CopyBuildingSettings copyBuildingSettings;
  private static readonly EventSystem.IntraObjectHandler<HighEnergyParticleRedirector> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<HighEnergyParticleRedirector>((Action<HighEnergyParticleRedirector, object>) ((component, data) => component.OnCopySettings(data)));
  private static readonly EventSystem.IntraObjectHandler<HighEnergyParticleRedirector> OnLogicValueChangedDelegate = new EventSystem.IntraObjectHandler<HighEnergyParticleRedirector>((Action<HighEnergyParticleRedirector, object>) ((component, data) => component.OnLogicValueChanged(data)));
  private bool hasLogicWire;
  private bool isLogicActive;
  private static StatusItem infoStatusItem_Logic;

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
    HighEnergyParticleRedirector component = ((GameObject) data).GetComponent<HighEnergyParticleRedirector>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    this.Direction = component.Direction;
  }

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<HighEnergyParticleRedirector>(-905833192, HighEnergyParticleRedirector.OnCopySettingsDelegate);
    this.Subscribe<HighEnergyParticleRedirector>(-801688580, HighEnergyParticleRedirector.OnLogicValueChangedDelegate);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    HighEnergyParticlePort component = ((Component) this).GetComponent<HighEnergyParticlePort>();
    if (Object.op_Implicit((Object) component))
      component.onParticleCaptureAllowed += new HighEnergyParticlePort.OnParticleCaptureAllowed(this.OnParticleCaptureAllowed);
    if (HighEnergyParticleRedirector.infoStatusItem_Logic == null)
    {
      HighEnergyParticleRedirector.infoStatusItem_Logic = new StatusItem("HEPRedirectorLogic", "BUILDING", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      HighEnergyParticleRedirector.infoStatusItem_Logic.resolveStringCallback = new Func<string, object, string>(HighEnergyParticleRedirector.ResolveInfoStatusItem);
      HighEnergyParticleRedirector.infoStatusItem_Logic.resolveTooltipCallback = new Func<string, object, string>(HighEnergyParticleRedirector.ResolveInfoStatusItemTooltip);
    }
    this.selectable.AddStatusItem(HighEnergyParticleRedirector.infoStatusItem_Logic, (object) this);
    this.directionController = new EightDirectionController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "redirector_target", "redirector", EightDirectionController.Offset.Infront);
    this.Direction = this.Direction;
    this.smi.StartSM();
    Tutorial.Instance.TutorialMessage(Tutorial.TutorialMessages.TM_Radiation);
  }

  private bool OnParticleCaptureAllowed(HighEnergyParticle particle) => this.AllowIncomingParticles;

  private void LaunchParticle()
  {
    if ((double) this.smi.master.storage.Particles < 0.10000000149011612)
    {
      double num = (double) this.smi.master.storage.ConsumeAll();
    }
    else
    {
      int particleOutputCell = ((Component) this).GetComponent<Building>().GetHighEnergyParticleOutputCell();
      GameObject gameObject = GameUtil.KInstantiate(Assets.GetPrefab(Tag.op_Implicit("HighEnergyParticle")), Grid.CellToPosCCC(particleOutputCell, Grid.SceneLayer.FXFront2), Grid.SceneLayer.FXFront2);
      gameObject.SetActive(true);
      if (!Object.op_Inequality((Object) gameObject, (Object) null))
        return;
      HighEnergyParticle component = gameObject.GetComponent<HighEnergyParticle>();
      component.payload = this.smi.master.storage.ConsumeAll();
      component.payload -= 0.1f;
      component.capturedBy = this.port;
      component.SetDirection(this.Direction);
      this.directionController.PlayAnim("redirector_send");
      this.directionController.controller.Queue(HashedString.op_Implicit("redirector"));
    }
  }

  protected override void OnCleanUp()
  {
    base.OnCleanUp();
    HighEnergyParticlePort component = ((Component) this).GetComponent<HighEnergyParticlePort>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.onParticleCaptureAllowed -= new HighEnergyParticlePort.OnParticleCaptureAllowed(this.OnParticleCaptureAllowed);
  }

  public bool AllowIncomingParticles
  {
    get
    {
      if (!this.hasLogicWire)
        return true;
      return this.hasLogicWire && this.isLogicActive;
    }
  }

  public bool HasLogicWire => this.hasLogicWire;

  public bool IsLogicActive => this.isLogicActive;

  private LogicCircuitNetwork GetNetwork() => Game.Instance.logicCircuitManager.GetNetworkForCell(((Component) this).GetComponent<LogicPorts>().GetPortCell(HighEnergyParticleRedirector.PORT_ID));

  private void OnLogicValueChanged(object data)
  {
    LogicValueChanged logicValueChanged = (LogicValueChanged) data;
    if (!HashedString.op_Equality(logicValueChanged.portID, HighEnergyParticleRedirector.PORT_ID))
      return;
    this.isLogicActive = logicValueChanged.newValue > 0;
    this.hasLogicWire = this.GetNetwork() != null;
  }

  private static string ResolveInfoStatusItem(string format_str, object data)
  {
    HighEnergyParticleRedirector particleRedirector = (HighEnergyParticleRedirector) data;
    if (!particleRedirector.HasLogicWire)
      return (string) BUILDING.STATUSITEMS.HIGHENERGYPARTICLEREDIRECTOR.NORMAL;
    return particleRedirector.IsLogicActive ? (string) BUILDING.STATUSITEMS.HIGHENERGYPARTICLEREDIRECTOR.LOGIC_CONTROLLED_ACTIVE : (string) BUILDING.STATUSITEMS.HIGHENERGYPARTICLEREDIRECTOR.LOGIC_CONTROLLED_STANDBY;
  }

  private static string ResolveInfoStatusItemTooltip(string format_str, object data)
  {
    HighEnergyParticleRedirector particleRedirector = (HighEnergyParticleRedirector) data;
    if (!particleRedirector.HasLogicWire)
      return (string) BUILDING.STATUSITEMS.HIGHENERGYPARTICLEREDIRECTOR.TOOLTIPS.NORMAL;
    return particleRedirector.IsLogicActive ? (string) BUILDING.STATUSITEMS.HIGHENERGYPARTICLEREDIRECTOR.TOOLTIPS.LOGIC_CONTROLLED_ACTIVE : (string) BUILDING.STATUSITEMS.HIGHENERGYPARTICLEREDIRECTOR.TOOLTIPS.LOGIC_CONTROLLED_STANDBY;
  }

  public class StatesInstance : 
    GameStateMachine<HighEnergyParticleRedirector.States, HighEnergyParticleRedirector.StatesInstance, HighEnergyParticleRedirector, object>.GameInstance
  {
    public StatesInstance(HighEnergyParticleRedirector smi)
      : base(smi)
    {
    }
  }

  public class States : 
    GameStateMachine<HighEnergyParticleRedirector.States, HighEnergyParticleRedirector.StatesInstance, HighEnergyParticleRedirector>
  {
    public GameStateMachine<HighEnergyParticleRedirector.States, HighEnergyParticleRedirector.StatesInstance, HighEnergyParticleRedirector, object>.State inoperational;
    public GameStateMachine<HighEnergyParticleRedirector.States, HighEnergyParticleRedirector.StatesInstance, HighEnergyParticleRedirector, object>.State ready;
    public GameStateMachine<HighEnergyParticleRedirector.States, HighEnergyParticleRedirector.StatesInstance, HighEnergyParticleRedirector, object>.State redirect;
    public GameStateMachine<HighEnergyParticleRedirector.States, HighEnergyParticleRedirector.StatesInstance, HighEnergyParticleRedirector, object>.State launch;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.inoperational;
      this.inoperational.PlayAnim("off").TagTransition(GameTags.Operational, this.ready);
      this.ready.PlayAnim("on").TagTransition(GameTags.Operational, this.inoperational, true).EventTransition(GameHashes.OnParticleStorageChanged, this.redirect);
      this.redirect.PlayAnim("working_pre").QueueAnim("working_loop").QueueAnim("working_pst").ScheduleGoTo((Func<HighEnergyParticleRedirector.StatesInstance, float>) (smi => smi.master.directorDelay), (StateMachine.BaseState) this.ready).Exit((StateMachine<HighEnergyParticleRedirector.States, HighEnergyParticleRedirector.StatesInstance, HighEnergyParticleRedirector, object>.State.Callback) (smi => smi.master.LaunchParticle()));
    }
  }
}
