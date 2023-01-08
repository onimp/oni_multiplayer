// Decompiled with JetBrains decompiler
// Type: SapTree
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class SapTree : 
  GameStateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>
{
  public SapTree.AliveStates alive;
  public GameStateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.State dead;
  private StateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.TargetParameter foodItem;
  private StateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.BoolParameter hasNearbyEnemy;
  private StateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.FloatParameter storedSap;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.alive;
    this.serializable = StateMachine.SerializeType.ParamsOnly;
    this.dead.ToggleStatusItem((string) CREATURES.STATUSITEMS.DEAD.NAME, (string) CREATURES.STATUSITEMS.DEAD.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main).ToggleTag(GameTags.PreventEmittingDisease).Enter((StateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.State.Callback) (smi =>
    {
      GameUtil.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(EffectConfigs.PlantDeathId)), TransformExtensions.GetPosition(smi.master.transform), Grid.SceneLayer.FXFront).SetActive(true);
      smi.master.Trigger(1623392196, (object) null);
      smi.master.GetComponent<KBatchedAnimController>().StopAndClear();
    }));
    this.alive.InitializeStates(this.masterTarget, this.dead).DefaultState((GameStateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.State) this.alive.normal);
    this.alive.normal.DefaultState(this.alive.normal.idle).EventTransition(GameHashes.Wilt, (GameStateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.State) this.alive.wilting, (StateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.Transition.ConditionCallback) (smi => smi.wiltCondition.IsWilting())).Update((System.Action<SapTree.StatesInstance, float>) ((smi, dt) => smi.CheckForFood()), (UpdateRate) 6);
    this.alive.normal.idle.PlayAnim("idle", (KAnim.PlayMode) 0).ToggleStatusItem((string) CREATURES.STATUSITEMS.IDLE.NAME, (string) CREATURES.STATUSITEMS.IDLE.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main).ParamTransition<bool>((StateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.Parameter<bool>) this.hasNearbyEnemy, this.alive.normal.attacking_pre, GameStateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.IsTrue).ParamTransition<float>((StateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.Parameter<float>) this.storedSap, this.alive.normal.oozing, (StateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.Parameter<float>.Callback) ((smi, p) => (double) p >= (double) smi.def.stomachSize)).ParamTransition<GameObject>((StateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.Parameter<GameObject>) this.foodItem, this.alive.normal.eating, GameStateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.IsNotNull);
    this.alive.normal.eating.PlayAnim("eat_pre", (KAnim.PlayMode) 1).QueueAnim("eat_loop", true).Update((System.Action<SapTree.StatesInstance, float>) ((smi, dt) => smi.EatFoodItem(dt)), (UpdateRate) 6).ParamTransition<GameObject>((StateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.Parameter<GameObject>) this.foodItem, this.alive.normal.eating_pst, GameStateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.IsNull).ParamTransition<float>((StateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.Parameter<float>) this.storedSap, this.alive.normal.eating_pst, (StateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.Parameter<float>.Callback) ((smi, p) => (double) p >= (double) smi.def.stomachSize));
    this.alive.normal.eating_pst.PlayAnim("eat_pst", (KAnim.PlayMode) 1).OnAnimQueueComplete(this.alive.normal.idle);
    this.alive.normal.oozing.PlayAnim("ooze_pre", (KAnim.PlayMode) 1).QueueAnim("ooze_loop", true).Update((System.Action<SapTree.StatesInstance, float>) ((smi, dt) => smi.Ooze(dt))).ParamTransition<float>((StateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.Parameter<float>) this.storedSap, this.alive.normal.oozing_pst, (StateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.Parameter<float>.Callback) ((smi, p) => (double) p <= 0.0)).ParamTransition<bool>((StateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.Parameter<bool>) this.hasNearbyEnemy, this.alive.normal.oozing_pst, GameStateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.IsTrue);
    this.alive.normal.oozing_pst.PlayAnim("ooze_pst", (KAnim.PlayMode) 1).OnAnimQueueComplete(this.alive.normal.idle);
    this.alive.normal.attacking_pre.PlayAnim("attacking_pre", (KAnim.PlayMode) 1).OnAnimQueueComplete(this.alive.normal.attacking);
    this.alive.normal.attacking.PlayAnim("attacking_loop", (KAnim.PlayMode) 1).Enter((StateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.State.Callback) (smi => smi.DoAttack())).OnAnimQueueComplete(this.alive.normal.attacking_cooldown);
    this.alive.normal.attacking_cooldown.PlayAnim("attacking_pst", (KAnim.PlayMode) 1).QueueAnim("attack_cooldown", true).ParamTransition<bool>((StateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.Parameter<bool>) this.hasNearbyEnemy, this.alive.normal.attacking_done, GameStateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.IsFalse).ScheduleGoTo((Func<SapTree.StatesInstance, float>) (smi => smi.def.attackCooldown), (StateMachine.BaseState) this.alive.normal.attacking);
    this.alive.normal.attacking_done.PlayAnim("attack_to_idle", (KAnim.PlayMode) 1).OnAnimQueueComplete(this.alive.normal.idle);
    this.alive.wilting.PlayAnim("withered", (KAnim.PlayMode) 0).EventTransition(GameHashes.WiltRecover, (GameStateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.State) this.alive.normal).ToggleTag(GameTags.PreventEmittingDisease);
  }

  public class Def : StateMachine.BaseDef
  {
    public Vector2I foodSenseArea;
    public float massEatRate;
    public float kcalorieToKGConversionRatio;
    public float stomachSize;
    public float oozeRate;
    public List<Vector3> oozeOffsets;
    public Vector2I attackSenseArea;
    public float attackCooldown;
  }

  public class AliveStates : 
    GameStateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.PlantAliveSubState
  {
    public SapTree.NormalStates normal;
    public SapTree.WiltingState wilting;
  }

  public class NormalStates : 
    GameStateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.State
  {
    public GameStateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.State idle;
    public GameStateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.State eating;
    public GameStateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.State eating_pst;
    public GameStateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.State oozing;
    public GameStateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.State oozing_pst;
    public GameStateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.State attacking_pre;
    public GameStateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.State attacking;
    public GameStateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.State attacking_cooldown;
    public GameStateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.State attacking_done;
  }

  public class WiltingState : 
    GameStateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.State
  {
    public GameStateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.State wilting_pre;
    public GameStateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.State wilting;
    public GameStateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.State wilting_pst;
  }

  public class StatesInstance : 
    GameStateMachine<SapTree, SapTree.StatesInstance, IStateMachineTarget, SapTree.Def>.GameInstance
  {
    [MyCmpReq]
    public WiltCondition wiltCondition;
    [MyCmpReq]
    public EntombVulnerable entombVulnerable;
    [MyCmpReq]
    private Storage storage;
    [MyCmpReq]
    private Weapon weapon;
    private HandleVector<int>.Handle partitionerEntry;
    private Extents feedExtents;
    private Extents attackExtents;

    public StatesInstance(IStateMachineTarget master, SapTree.Def def)
      : base(master, def)
    {
      Vector2I xy = Grid.PosToXY(TransformExtensions.GetPosition(this.gameObject.transform));
      Vector2I vector2I1;
      // ISSUE: explicit constructor call
      ((Vector2I) ref vector2I1).\u002Ector(xy.x - def.attackSenseArea.x / 2, xy.y);
      this.attackExtents = new Extents(vector2I1.x, vector2I1.y, def.attackSenseArea.x, def.attackSenseArea.y);
      this.partitionerEntry = GameScenePartitioner.Instance.Add("SapTreeAttacker", (object) this, this.attackExtents, GameScenePartitioner.Instance.objectLayers[0], new System.Action<object>(this.OnMinionChanged));
      Vector2I vector2I2;
      // ISSUE: explicit constructor call
      ((Vector2I) ref vector2I2).\u002Ector(xy.x - def.foodSenseArea.x / 2, xy.y);
      this.feedExtents = new Extents(vector2I2.x, vector2I2.y, def.foodSenseArea.x, def.foodSenseArea.y);
    }

    protected override void OnCleanUp() => GameScenePartitioner.Instance.Free(ref this.partitionerEntry);

    public void EatFoodItem(float dt)
    {
      Pickupable pickupable = this.sm.foodItem.Get(this).GetComponent<Pickupable>().Take(this.def.massEatRate * dt);
      if (!Object.op_Inequality((Object) pickupable, (Object) null))
        return;
      float mass = ((Component) pickupable).GetComponent<Edible>().Calories * (1f / 1000f) * this.def.kcalorieToKGConversionRatio;
      Util.KDestroyGameObject(((Component) pickupable).gameObject);
      PrimaryElement component = this.GetComponent<PrimaryElement>();
      this.storage.AddLiquid(SimHashes.Resin, mass, component.Temperature, byte.MaxValue, 0, true, false);
      double num = (double) this.sm.storedSap.Set(this.storage.GetMassAvailable(SimHashes.Resin.CreateTag()), this);
    }

    public void Ooze(float dt)
    {
      float amount = Mathf.Min(this.sm.storedSap.Get(this), dt * this.def.oozeRate);
      if ((double) amount <= 0.0)
        return;
      int index = Mathf.FloorToInt(GameClock.Instance.GetTime() % (float) this.def.oozeOffsets.Count);
      this.storage.DropSome(SimHashes.Resin.CreateTag(), amount, dumpLiquid: true, offset: this.def.oozeOffsets[index]);
      double num = (double) this.sm.storedSap.Set(this.storage.GetMassAvailable(SimHashes.Resin.CreateTag()), this);
    }

    public void CheckForFood()
    {
      ListPool<ScenePartitionerEntry, SapTree>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, SapTree>.Allocate();
      GameScenePartitioner.Instance.GatherEntries(this.feedExtents, GameScenePartitioner.Instance.pickupablesLayer, (List<ScenePartitionerEntry>) gathered_entries);
      foreach (ScenePartitionerEntry partitionerEntry in (List<ScenePartitionerEntry>) gathered_entries)
      {
        Pickupable pickupable = partitionerEntry.obj as Pickupable;
        if (Object.op_Inequality((Object) ((Component) pickupable).GetComponent<Edible>(), (Object) null))
        {
          this.sm.foodItem.Set(((Component) pickupable).gameObject, this);
          return;
        }
      }
      this.sm.foodItem.Set((KMonoBehaviour) null, this);
    }

    public bool DoAttack()
    {
      this.sm.hasNearbyEnemy.Set(this.weapon.AttackArea(TransformExtensions.GetPosition(this.transform)) > 0, this);
      return true;
    }

    private void OnMinionChanged(object obj)
    {
      if (!Object.op_Inequality((Object) (obj as GameObject), (Object) null))
        return;
      this.sm.hasNearbyEnemy.Set(true, this);
    }
  }
}
