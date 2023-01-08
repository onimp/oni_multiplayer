// Decompiled with JetBrains decompiler
// Type: JungleGasPlant
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

public class JungleGasPlant : StateMachineComponent<JungleGasPlant.StatesInstance>
{
  [MyCmpReq]
  private ReceptacleMonitor rm;
  [MyCmpReq]
  private Growing growing;
  [MyCmpReq]
  private WiltCondition wiltCondition;
  [MyCmpReq]
  private ElementEmitter elementEmitter;

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
  }

  protected void DestroySelf(object callbackParam)
  {
    CreatureHelpers.DeselectCreature(((Component) this).gameObject);
    Util.KDestroyGameObject(((Component) this).gameObject);
  }

  public class StatesInstance : 
    GameStateMachine<JungleGasPlant.States, JungleGasPlant.StatesInstance, JungleGasPlant, object>.GameInstance
  {
    public StatesInstance(JungleGasPlant master)
      : base(master)
    {
    }
  }

  public class States : 
    GameStateMachine<JungleGasPlant.States, JungleGasPlant.StatesInstance, JungleGasPlant>
  {
    public GameStateMachine<JungleGasPlant.States, JungleGasPlant.StatesInstance, JungleGasPlant, object>.State blocked_from_growing;
    public JungleGasPlant.States.AliveStates alive;
    public GameStateMachine<JungleGasPlant.States, JungleGasPlant.StatesInstance, JungleGasPlant, object>.State dead;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.alive.seed_grow;
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      this.root.Enter((StateMachine<JungleGasPlant.States, JungleGasPlant.StatesInstance, JungleGasPlant, object>.State.Callback) (smi =>
      {
        if (smi.master.rm.Replanted && !this.alive.ForceUpdateStatus(((Component) smi.master).gameObject))
          smi.GoTo((StateMachine.BaseState) this.blocked_from_growing);
        else
          smi.GoTo((StateMachine.BaseState) this.alive.seed_grow);
      }));
      this.dead.ToggleStatusItem((string) CREATURES.STATUSITEMS.DEAD.NAME, (string) CREATURES.STATUSITEMS.DEAD.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main).Enter((StateMachine<JungleGasPlant.States, JungleGasPlant.StatesInstance, JungleGasPlant, object>.State.Callback) (smi =>
      {
        GameUtil.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(EffectConfigs.PlantDeathId)), TransformExtensions.GetPosition(smi.master.transform), Grid.SceneLayer.FXFront).SetActive(true);
        smi.master.Trigger(1623392196, (object) null);
        ((Component) smi.master).GetComponent<KBatchedAnimController>().StopAndClear();
        Object.Destroy((Object) ((Component) smi.master).GetComponent<KBatchedAnimController>());
        smi.Schedule(0.5f, new Action<object>(smi.master.DestroySelf), (object) null);
      }));
      this.blocked_from_growing.ToggleStatusItem(Db.Get().MiscStatusItems.RegionIsBlocked).TagTransition(GameTags.Entombed, this.alive.seed_grow, true).EventTransition(GameHashes.TooColdWarning, this.alive.seed_grow).EventTransition(GameHashes.TooHotWarning, this.alive.seed_grow).TagTransition(GameTags.Uprooted, this.dead);
      this.alive.InitializeStates(this.masterTarget, this.dead);
      this.alive.seed_grow.QueueAnim("seed_grow").EventTransition(GameHashes.AnimQueueComplete, this.alive.idle).EventTransition(GameHashes.Wilt, (GameStateMachine<JungleGasPlant.States, JungleGasPlant.StatesInstance, JungleGasPlant, object>.State) this.alive.wilting, (StateMachine<JungleGasPlant.States, JungleGasPlant.StatesInstance, JungleGasPlant, object>.Transition.ConditionCallback) (smi => smi.master.wiltCondition.IsWilting()));
      this.alive.idle.EventTransition(GameHashes.Wilt, (GameStateMachine<JungleGasPlant.States, JungleGasPlant.StatesInstance, JungleGasPlant, object>.State) this.alive.wilting, (StateMachine<JungleGasPlant.States, JungleGasPlant.StatesInstance, JungleGasPlant, object>.Transition.ConditionCallback) (smi => smi.master.wiltCondition.IsWilting())).EventTransition(GameHashes.Grow, (GameStateMachine<JungleGasPlant.States, JungleGasPlant.StatesInstance, JungleGasPlant, object>.State) this.alive.grown, (StateMachine<JungleGasPlant.States, JungleGasPlant.StatesInstance, JungleGasPlant, object>.Transition.ConditionCallback) (smi => smi.master.growing.IsGrown())).PlayAnim("idle_loop", (KAnim.PlayMode) 0);
      this.alive.grown.DefaultState(this.alive.grown.pre).EventTransition(GameHashes.Wilt, (GameStateMachine<JungleGasPlant.States, JungleGasPlant.StatesInstance, JungleGasPlant, object>.State) this.alive.wilting, (StateMachine<JungleGasPlant.States, JungleGasPlant.StatesInstance, JungleGasPlant, object>.Transition.ConditionCallback) (smi => smi.master.wiltCondition.IsWilting())).Enter((StateMachine<JungleGasPlant.States, JungleGasPlant.StatesInstance, JungleGasPlant, object>.State.Callback) (smi => smi.master.elementEmitter.SetEmitting(true))).Exit((StateMachine<JungleGasPlant.States, JungleGasPlant.StatesInstance, JungleGasPlant, object>.State.Callback) (smi => smi.master.elementEmitter.SetEmitting(false)));
      this.alive.grown.pre.PlayAnim("grow", (KAnim.PlayMode) 1).OnAnimQueueComplete(this.alive.grown.idle);
      this.alive.grown.idle.PlayAnim("idle_bloom_loop", (KAnim.PlayMode) 0);
      this.alive.wilting.pre.DefaultState(this.alive.wilting.pre).PlayAnim("wilt_pre", (KAnim.PlayMode) 1).OnAnimQueueComplete(this.alive.wilting.idle).EventTransition(GameHashes.WiltRecover, this.alive.wilting.pst, (StateMachine<JungleGasPlant.States, JungleGasPlant.StatesInstance, JungleGasPlant, object>.Transition.ConditionCallback) (smi => !smi.master.wiltCondition.IsWilting()));
      this.alive.wilting.idle.PlayAnim("idle_wilt_loop", (KAnim.PlayMode) 0).EventTransition(GameHashes.WiltRecover, this.alive.wilting.pst, (StateMachine<JungleGasPlant.States, JungleGasPlant.StatesInstance, JungleGasPlant, object>.Transition.ConditionCallback) (smi => !smi.master.wiltCondition.IsWilting()));
      this.alive.wilting.pst.PlayAnim("wilt_pst", (KAnim.PlayMode) 1).OnAnimQueueComplete(this.alive.idle);
    }

    public class AliveStates : 
      GameStateMachine<JungleGasPlant.States, JungleGasPlant.StatesInstance, JungleGasPlant, object>.PlantAliveSubState
    {
      public GameStateMachine<JungleGasPlant.States, JungleGasPlant.StatesInstance, JungleGasPlant, object>.State seed_grow;
      public GameStateMachine<JungleGasPlant.States, JungleGasPlant.StatesInstance, JungleGasPlant, object>.State idle;
      public JungleGasPlant.States.WiltingState wilting;
      public JungleGasPlant.States.GrownState grown;
      public GameStateMachine<JungleGasPlant.States, JungleGasPlant.StatesInstance, JungleGasPlant, object>.State destroy;
    }

    public class GrownState : 
      GameStateMachine<JungleGasPlant.States, JungleGasPlant.StatesInstance, JungleGasPlant, object>.State
    {
      public GameStateMachine<JungleGasPlant.States, JungleGasPlant.StatesInstance, JungleGasPlant, object>.State pre;
      public GameStateMachine<JungleGasPlant.States, JungleGasPlant.StatesInstance, JungleGasPlant, object>.State idle;
    }

    public class WiltingState : 
      GameStateMachine<JungleGasPlant.States, JungleGasPlant.StatesInstance, JungleGasPlant, object>.State
    {
      public GameStateMachine<JungleGasPlant.States, JungleGasPlant.StatesInstance, JungleGasPlant, object>.State pre;
      public GameStateMachine<JungleGasPlant.States, JungleGasPlant.StatesInstance, JungleGasPlant, object>.State idle;
      public GameStateMachine<JungleGasPlant.States, JungleGasPlant.StatesInstance, JungleGasPlant, object>.State pst;
    }
  }
}
