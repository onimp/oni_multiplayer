// Decompiled with JetBrains decompiler
// Type: ColdBreather
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
public class ColdBreather : 
  StateMachineComponent<ColdBreather.StatesInstance>,
  IGameObjectEffectDescriptor
{
  [MyCmpReq]
  private WiltCondition wiltCondition;
  [MyCmpReq]
  private KAnimControllerBase animController;
  [MyCmpReq]
  private Storage storage;
  [MyCmpReq]
  private ElementConsumer elementConsumer;
  [MyCmpGet]
  private RadiationEmitter radiationEmitter;
  [MyCmpReq]
  private ReceptacleMonitor receptacleMonitor;
  private const float EXHALE_PERIOD = 1f;
  public float consumptionRate;
  public float deltaEmitTemperature = -5f;
  public Vector3 emitOffsetCell = new Vector3(0.0f, 0.0f);
  private List<GameObject> gases = new List<GameObject>();
  private Tag lastEmitTag;
  private int nextGasEmitIndex;
  private HandleVector<Game.ComplexCallbackInfo<Sim.MassEmittedCallback>>.Handle simEmitCBHandle = HandleVector<Game.ComplexCallbackInfo<Sim.MassEmittedCallback>>.InvalidHandle;
  private static readonly EventSystem.IntraObjectHandler<ColdBreather> OnReplantedDelegate = new EventSystem.IntraObjectHandler<ColdBreather>((Action<ColdBreather, object>) ((component, data) => component.OnReplanted(data)));

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.simEmitCBHandle = Game.Instance.massEmitCallbackManager.Add(new Action<Sim.MassEmittedCallback, object>(ColdBreather.OnSimEmittedCallback), (object) this, nameof (ColdBreather));
    this.smi.StartSM();
  }

  protected virtual void OnPrefabInit()
  {
    this.elementConsumer.EnableConsumption(false);
    this.Subscribe<ColdBreather>(1309017699, ColdBreather.OnReplantedDelegate);
    base.OnPrefabInit();
  }

  private void OnReplanted(object data = null)
  {
    ReceptacleMonitor component = ((Component) this).GetComponent<ReceptacleMonitor>();
    if (Object.op_Equality((Object) component, (Object) null))
      return;
    ((Component) this).GetComponent<ElementConsumer>().consumptionRate = !component.Replanted ? this.consumptionRate * 0.25f : this.consumptionRate;
    if (!Object.op_Inequality((Object) this.radiationEmitter, (Object) null))
      return;
    this.radiationEmitter.emitRads = 480f;
    this.radiationEmitter.Refresh();
  }

  protected override void OnCleanUp()
  {
    Game.Instance.massEmitCallbackManager.Release(this.simEmitCBHandle, "coldbreather");
    this.simEmitCBHandle.Clear();
    if (Object.op_Implicit((Object) this.storage))
      this.storage.DropAll(true, offset: new Vector3());
    base.OnCleanUp();
  }

  protected void DestroySelf(object callbackParam)
  {
    CreatureHelpers.DeselectCreature(((Component) this).gameObject);
    Util.KDestroyGameObject(((Component) this).gameObject);
  }

  public List<Descriptor> GetDescriptors(GameObject go)
  {
    List<Descriptor> descriptors = new List<Descriptor>();
    descriptors.Add(new Descriptor((string) UI.GAMEOBJECTEFFECTS.COLDBREATHER, (string) UI.GAMEOBJECTEFFECTS.TOOLTIPS.COLDBREATHER, (Descriptor.DescriptorType) 1, false));
    return descriptors;
  }

  private void SetEmitting(bool emitting)
  {
    if (!Object.op_Inequality((Object) this.radiationEmitter, (Object) null))
      return;
    this.radiationEmitter.SetEmitting(emitting);
  }

  private void Exhale()
  {
    if (Tag.op_Inequality(this.lastEmitTag, Tag.Invalid))
      return;
    this.gases.Clear();
    this.storage.Find(GameTags.Gas, this.gases);
    if (this.nextGasEmitIndex >= this.gases.Count)
      this.nextGasEmitIndex = 0;
    while (this.nextGasEmitIndex < this.gases.Count)
    {
      PrimaryElement component = this.gases[this.nextGasEmitIndex++].GetComponent<PrimaryElement>();
      if (Object.op_Inequality((Object) component, (Object) null) && (double) component.Mass > 0.0 && this.simEmitCBHandle.IsValid())
      {
        float num = Mathf.Max(component.Element.lowTemp + 5f, component.Temperature + this.deltaEmitTemperature);
        int cell = Grid.PosToCell(Vector3.op_Addition(TransformExtensions.GetPosition(this.transform), this.emitOffsetCell));
        ushort idx = component.Element.idx;
        Game.Instance.massEmitCallbackManager.GetItem(this.simEmitCBHandle);
        int element_idx = (int) idx;
        double mass = (double) component.Mass;
        double temperature = (double) num;
        int diseaseIdx = (int) component.DiseaseIdx;
        int diseaseCount = component.DiseaseCount;
        int index = this.simEmitCBHandle.index;
        SimMessages.EmitMass(cell, (ushort) element_idx, (float) mass, (float) temperature, (byte) diseaseIdx, diseaseCount, index);
        this.lastEmitTag = component.Element.tag;
        break;
      }
    }
  }

  private static void OnSimEmittedCallback(Sim.MassEmittedCallback info, object data) => ((ColdBreather) data).OnSimEmitted(info);

  private void OnSimEmitted(Sim.MassEmittedCallback info)
  {
    if (info.suceeded == (byte) 1 && Object.op_Implicit((Object) this.storage) && ((Tag) ref this.lastEmitTag).IsValid)
      this.storage.ConsumeIgnoringDisease(this.lastEmitTag, info.mass);
    this.lastEmitTag = Tag.Invalid;
  }

  public class StatesInstance : 
    GameStateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather, object>.GameInstance
  {
    public StatesInstance(ColdBreather master)
      : base(master)
    {
    }
  }

  public class States : 
    GameStateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather>
  {
    public GameStateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather, object>.State grow;
    public GameStateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather, object>.State blocked_from_growing;
    public ColdBreather.States.AliveStates alive;
    public GameStateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather, object>.State dead;
    private StatusItem statusItemCooling;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      default_state = (StateMachine.BaseState) this.grow;
      this.statusItemCooling = new StatusItem("cooling", (string) CREATURES.STATUSITEMS.COOLING.NAME, (string) CREATURES.STATUSITEMS.COOLING.TOOLTIP, "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.dead.ToggleStatusItem((string) CREATURES.STATUSITEMS.DEAD.NAME, (string) CREATURES.STATUSITEMS.DEAD.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main).Enter((StateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather, object>.State.Callback) (smi =>
      {
        GameUtil.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(EffectConfigs.PlantDeathId)), TransformExtensions.GetPosition(smi.master.transform), Grid.SceneLayer.FXFront).SetActive(true);
        smi.master.Trigger(1623392196, (object) null);
        ((Component) smi.master).GetComponent<KBatchedAnimController>().StopAndClear();
        Object.Destroy((Object) ((Component) smi.master).GetComponent<KBatchedAnimController>());
        smi.Schedule(0.5f, new Action<object>(smi.master.DestroySelf), (object) null);
      }));
      this.blocked_from_growing.ToggleStatusItem(Db.Get().MiscStatusItems.RegionIsBlocked).EventTransition(GameHashes.EntombedChanged, (GameStateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather, object>.State) this.alive, (StateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather, object>.Transition.ConditionCallback) (smi => this.alive.ForceUpdateStatus(((Component) smi.master).gameObject))).EventTransition(GameHashes.TooColdWarning, (GameStateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather, object>.State) this.alive, (StateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather, object>.Transition.ConditionCallback) (smi => this.alive.ForceUpdateStatus(((Component) smi.master).gameObject))).EventTransition(GameHashes.TooHotWarning, (GameStateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather, object>.State) this.alive, (StateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather, object>.Transition.ConditionCallback) (smi => this.alive.ForceUpdateStatus(((Component) smi.master).gameObject))).TagTransition(GameTags.Uprooted, this.dead);
      this.grow.Enter((StateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather, object>.State.Callback) (smi =>
      {
        if (!smi.master.receptacleMonitor.HasReceptacle() || this.alive.ForceUpdateStatus(((Component) smi.master).gameObject))
          return;
        smi.GoTo((StateMachine.BaseState) this.blocked_from_growing);
      })).PlayAnim("grow_seed", (KAnim.PlayMode) 1).EventTransition(GameHashes.AnimQueueComplete, (GameStateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather, object>.State) this.alive);
      this.alive.InitializeStates(this.masterTarget, this.dead).DefaultState(this.alive.mature).Update((Action<ColdBreather.StatesInstance, float>) ((smi, dt) => smi.master.Exhale()));
      this.alive.mature.EventTransition(GameHashes.Wilt, this.alive.wilting, (StateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather, object>.Transition.ConditionCallback) (smi => smi.master.wiltCondition.IsWilting())).PlayAnim("idle", (KAnim.PlayMode) 0).ToggleMainStatusItem(this.statusItemCooling).Enter((StateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather, object>.State.Callback) (smi =>
      {
        smi.master.elementConsumer.EnableConsumption(true);
        smi.master.SetEmitting(true);
      })).Exit((StateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather, object>.State.Callback) (smi =>
      {
        smi.master.elementConsumer.EnableConsumption(false);
        smi.master.SetEmitting(false);
      }));
      this.alive.wilting.PlayAnim("wilt1").EventTransition(GameHashes.WiltRecover, this.alive.mature, (StateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather, object>.Transition.ConditionCallback) (smi => !smi.master.wiltCondition.IsWilting())).Enter((StateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather, object>.State.Callback) (smi => smi.master.SetEmitting(false)));
    }

    public class AliveStates : 
      GameStateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather, object>.PlantAliveSubState
    {
      public GameStateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather, object>.State mature;
      public GameStateMachine<ColdBreather.States, ColdBreather.StatesInstance, ColdBreather, object>.State wilting;
    }
  }
}
