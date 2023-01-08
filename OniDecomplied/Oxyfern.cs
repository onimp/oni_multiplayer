// Decompiled with JetBrains decompiler
// Type: Oxyfern
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

public class Oxyfern : StateMachineComponent<Oxyfern.StatesInstance>
{
  [MyCmpReq]
  private WiltCondition wiltCondition;
  [MyCmpReq]
  private ElementConsumer elementConsumer;
  [MyCmpReq]
  private ElementConverter elementConverter;
  [MyCmpReq]
  private ReceptacleMonitor receptacleMonitor;
  private static readonly EventSystem.IntraObjectHandler<Oxyfern> OnReplantedDelegate = new EventSystem.IntraObjectHandler<Oxyfern>((Action<Oxyfern, object>) ((component, data) => component.OnReplanted(data)));

  protected void DestroySelf(object callbackParam)
  {
    CreatureHelpers.DeselectCreature(((Component) this).gameObject);
    Util.KDestroyGameObject(((Component) this).gameObject);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
  }

  protected override void OnCleanUp()
  {
    base.OnCleanUp();
    if (!Tutorial.Instance.oxygenGenerators.Contains(((Component) this).gameObject))
      return;
    Tutorial.Instance.oxygenGenerators.Remove(((Component) this).gameObject);
  }

  protected virtual void OnPrefabInit()
  {
    this.Subscribe<Oxyfern>(1309017699, Oxyfern.OnReplantedDelegate);
    base.OnPrefabInit();
  }

  private void OnReplanted(object data = null)
  {
    this.SetConsumptionRate();
    if (!this.receptacleMonitor.Replanted)
      return;
    Tutorial.Instance.oxygenGenerators.Add(((Component) this).gameObject);
  }

  public void SetConsumptionRate()
  {
    if (this.receptacleMonitor.Replanted)
      this.elementConsumer.consumptionRate = 0.000625000044f;
    else
      this.elementConsumer.consumptionRate = 0.000156250011f;
  }

  public class StatesInstance : 
    GameStateMachine<Oxyfern.States, Oxyfern.StatesInstance, Oxyfern, object>.GameInstance
  {
    public StatesInstance(Oxyfern master)
      : base(master)
    {
    }
  }

  public class States : GameStateMachine<Oxyfern.States, Oxyfern.StatesInstance, Oxyfern>
  {
    public GameStateMachine<Oxyfern.States, Oxyfern.StatesInstance, Oxyfern, object>.State grow;
    public GameStateMachine<Oxyfern.States, Oxyfern.StatesInstance, Oxyfern, object>.State blocked_from_growing;
    public Oxyfern.States.AliveStates alive;
    public GameStateMachine<Oxyfern.States, Oxyfern.StatesInstance, Oxyfern, object>.State dead;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      default_state = (StateMachine.BaseState) this.grow;
      this.dead.ToggleStatusItem((string) CREATURES.STATUSITEMS.DEAD.NAME, (string) CREATURES.STATUSITEMS.DEAD.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main).Enter((StateMachine<Oxyfern.States, Oxyfern.StatesInstance, Oxyfern, object>.State.Callback) (smi =>
      {
        GameUtil.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(EffectConfigs.PlantDeathId)), TransformExtensions.GetPosition(smi.master.transform), Grid.SceneLayer.FXFront).SetActive(true);
        smi.master.Trigger(1623392196, (object) null);
        ((Component) smi.master).GetComponent<KBatchedAnimController>().StopAndClear();
        Object.Destroy((Object) ((Component) smi.master).GetComponent<KBatchedAnimController>());
        smi.Schedule(0.5f, new Action<object>(smi.master.DestroySelf), (object) null);
      }));
      this.blocked_from_growing.ToggleStatusItem(Db.Get().MiscStatusItems.RegionIsBlocked).EventTransition(GameHashes.EntombedChanged, (GameStateMachine<Oxyfern.States, Oxyfern.StatesInstance, Oxyfern, object>.State) this.alive, (StateMachine<Oxyfern.States, Oxyfern.StatesInstance, Oxyfern, object>.Transition.ConditionCallback) (smi => this.alive.ForceUpdateStatus(((Component) smi.master).gameObject))).EventTransition(GameHashes.TooColdWarning, (GameStateMachine<Oxyfern.States, Oxyfern.StatesInstance, Oxyfern, object>.State) this.alive, (StateMachine<Oxyfern.States, Oxyfern.StatesInstance, Oxyfern, object>.Transition.ConditionCallback) (smi => this.alive.ForceUpdateStatus(((Component) smi.master).gameObject))).EventTransition(GameHashes.TooHotWarning, (GameStateMachine<Oxyfern.States, Oxyfern.StatesInstance, Oxyfern, object>.State) this.alive, (StateMachine<Oxyfern.States, Oxyfern.StatesInstance, Oxyfern, object>.Transition.ConditionCallback) (smi => this.alive.ForceUpdateStatus(((Component) smi.master).gameObject))).TagTransition(GameTags.Uprooted, this.dead);
      this.grow.Enter((StateMachine<Oxyfern.States, Oxyfern.StatesInstance, Oxyfern, object>.State.Callback) (smi =>
      {
        if (!smi.master.receptacleMonitor.HasReceptacle() || this.alive.ForceUpdateStatus(((Component) smi.master).gameObject))
          return;
        smi.GoTo((StateMachine.BaseState) this.blocked_from_growing);
      })).PlayAnim("grow_pst", (KAnim.PlayMode) 1).EventTransition(GameHashes.AnimQueueComplete, (GameStateMachine<Oxyfern.States, Oxyfern.StatesInstance, Oxyfern, object>.State) this.alive);
      this.alive.InitializeStates(this.masterTarget, this.dead).DefaultState(this.alive.mature);
      this.alive.mature.EventTransition(GameHashes.Wilt, this.alive.wilting, (StateMachine<Oxyfern.States, Oxyfern.StatesInstance, Oxyfern, object>.Transition.ConditionCallback) (smi => smi.master.wiltCondition.IsWilting())).PlayAnim("idle_full", (KAnim.PlayMode) 0).Enter((StateMachine<Oxyfern.States, Oxyfern.StatesInstance, Oxyfern, object>.State.Callback) (smi => smi.master.elementConsumer.EnableConsumption(true))).Exit((StateMachine<Oxyfern.States, Oxyfern.StatesInstance, Oxyfern, object>.State.Callback) (smi => smi.master.elementConsumer.EnableConsumption(false)));
      this.alive.wilting.PlayAnim("wilt3").EventTransition(GameHashes.WiltRecover, this.alive.mature, (StateMachine<Oxyfern.States, Oxyfern.StatesInstance, Oxyfern, object>.Transition.ConditionCallback) (smi => !smi.master.wiltCondition.IsWilting()));
    }

    public class AliveStates : 
      GameStateMachine<Oxyfern.States, Oxyfern.StatesInstance, Oxyfern, object>.PlantAliveSubState
    {
      public GameStateMachine<Oxyfern.States, Oxyfern.StatesInstance, Oxyfern, object>.State mature;
      public GameStateMachine<Oxyfern.States, Oxyfern.StatesInstance, Oxyfern, object>.State wilting;
    }
  }
}
