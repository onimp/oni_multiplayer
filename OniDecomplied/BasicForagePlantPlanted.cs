// Decompiled with JetBrains decompiler
// Type: BasicForagePlantPlanted
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using UnityEngine;

public class BasicForagePlantPlanted : StateMachineComponent<BasicForagePlantPlanted.StatesInstance>
{
  [MyCmpReq]
  private Harvestable harvestable;
  [MyCmpReq]
  private SeedProducer seedProducer;
  [MyCmpReq]
  private KBatchedAnimController animController;

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
    GameStateMachine<BasicForagePlantPlanted.States, BasicForagePlantPlanted.StatesInstance, BasicForagePlantPlanted, object>.GameInstance
  {
    public StatesInstance(BasicForagePlantPlanted smi)
      : base(smi)
    {
    }
  }

  public class States : 
    GameStateMachine<BasicForagePlantPlanted.States, BasicForagePlantPlanted.StatesInstance, BasicForagePlantPlanted>
  {
    public GameStateMachine<BasicForagePlantPlanted.States, BasicForagePlantPlanted.StatesInstance, BasicForagePlantPlanted, object>.State seed_grow;
    public BasicForagePlantPlanted.States.AliveStates alive;
    public GameStateMachine<BasicForagePlantPlanted.States, BasicForagePlantPlanted.StatesInstance, BasicForagePlantPlanted, object>.State dead;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.seed_grow;
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      this.seed_grow.PlayAnim("idle", (KAnim.PlayMode) 1).EventTransition(GameHashes.AnimQueueComplete, this.alive.idle);
      this.alive.InitializeStates(this.masterTarget, this.dead);
      this.alive.idle.PlayAnim("idle").EventTransition(GameHashes.Harvest, this.alive.harvest).Enter((StateMachine<BasicForagePlantPlanted.States, BasicForagePlantPlanted.StatesInstance, BasicForagePlantPlanted, object>.State.Callback) (smi => smi.master.harvestable.SetCanBeHarvested(true)));
      this.alive.harvest.Enter((StateMachine<BasicForagePlantPlanted.States, BasicForagePlantPlanted.StatesInstance, BasicForagePlantPlanted, object>.State.Callback) (smi => smi.master.seedProducer.DropSeed())).GoTo(this.dead);
      this.dead.Enter((StateMachine<BasicForagePlantPlanted.States, BasicForagePlantPlanted.StatesInstance, BasicForagePlantPlanted, object>.State.Callback) (smi =>
      {
        GameUtil.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(EffectConfigs.PlantDeathId)), TransformExtensions.GetPosition(smi.master.transform), Grid.SceneLayer.FXFront).SetActive(true);
        smi.master.Trigger(1623392196, (object) null);
        smi.master.animController.StopAndClear();
        Object.Destroy((Object) smi.master.animController);
        smi.master.DestroySelf((object) null);
      }));
    }

    public class AliveStates : 
      GameStateMachine<BasicForagePlantPlanted.States, BasicForagePlantPlanted.StatesInstance, BasicForagePlantPlanted, object>.PlantAliveSubState
    {
      public GameStateMachine<BasicForagePlantPlanted.States, BasicForagePlantPlanted.StatesInstance, BasicForagePlantPlanted, object>.State idle;
      public GameStateMachine<BasicForagePlantPlanted.States, BasicForagePlantPlanted.StatesInstance, BasicForagePlantPlanted, object>.State harvest;
    }
  }
}
