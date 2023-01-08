// Decompiled with JetBrains decompiler
// Type: HugMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class HugMonitor : 
  GameStateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>
{
  private static string soundPath = GlobalAssets.GetSound("Squirrel_hug_frenzyFX");
  private StateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.FloatParameter hugFrenzyTimer;
  private StateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.FloatParameter wantsHugCooldownTimer;
  private StateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.FloatParameter hugEggCooldownTimer;
  public HugMonitor.NormalStates normal;
  public GameStateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.State hugFrenzy;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.normal;
    this.serializable = StateMachine.SerializeType.ParamsOnly;
    this.root.Update(new System.Action<HugMonitor.Instance, float>(this.UpdateHugEggCooldownTimer), (UpdateRate) 6).ToggleBehaviour(GameTags.Creatures.WantsToTendEgg, (StateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.Transition.ConditionCallback) (smi => smi.UpdateHasTarget()), (System.Action<HugMonitor.Instance>) (smi => smi.hugTarget = (GameObject) null));
    this.normal.DefaultState(this.normal.idle).ParamTransition<float>((StateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.Parameter<float>) this.hugFrenzyTimer, this.hugFrenzy, GameStateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.IsGTZero);
    this.normal.idle.ParamTransition<float>((StateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.Parameter<float>) this.wantsHugCooldownTimer, this.normal.hugReady.seekingHug, GameStateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.IsLTEZero).Update(new System.Action<HugMonitor.Instance, float>(this.UpdateWantsHugCooldownTimer), (UpdateRate) 6);
    this.normal.hugReady.ToggleReactable(new Func<HugMonitor.Instance, Reactable>(this.GetHugReactable));
    this.normal.hugReady.passiveHug.ParamTransition<float>((StateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.Parameter<float>) this.wantsHugCooldownTimer, this.normal.hugReady.seekingHug, GameStateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.IsLTEZero).Update(new System.Action<HugMonitor.Instance, float>(this.UpdateWantsHugCooldownTimer), (UpdateRate) 6).ToggleStatusItem((string) CREATURES.STATUSITEMS.HUGMINIONWAITING.NAME, (string) CREATURES.STATUSITEMS.HUGMINIONWAITING.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main);
    this.normal.hugReady.seekingHug.ToggleBehaviour(GameTags.Creatures.WantsAHug, (StateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.Transition.ConditionCallback) (smi => true), (System.Action<HugMonitor.Instance>) (smi =>
    {
      double num = (double) this.wantsHugCooldownTimer.Set(smi.def.hugFrenzyCooldownFailed, smi);
      smi.GoTo((StateMachine.BaseState) this.normal.hugReady.passiveHug);
    }));
    this.hugFrenzy.ParamTransition<float>((StateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.Parameter<float>) this.hugFrenzyTimer, (GameStateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.State) this.normal, (StateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.Parameter<float>.Callback) ((smi, p) => (double) p <= 0.0 && !smi.IsHugging())).Update(new System.Action<HugMonitor.Instance, float>(this.UpdateHugFrenzyTimer), (UpdateRate) 6).ToggleEffect((Func<HugMonitor.Instance, Effect>) (smi => smi.frenzyEffect)).ToggleLoopingSound(HugMonitor.soundPath).Enter((StateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.State.Callback) (smi =>
    {
      smi.hugParticleFx = Util.KInstantiate(EffectPrefabs.Instance.HugFrenzyFX, Vector3.op_Addition(TransformExtensions.GetPosition(smi.master.transform), smi.hugParticleOffset));
      smi.hugParticleFx.transform.SetParent(smi.master.transform);
      smi.hugParticleFx.SetActive(true);
    })).Exit((StateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.State.Callback) (smi =>
    {
      Util.KDestroyGameObject(smi.hugParticleFx);
      double num = (double) this.wantsHugCooldownTimer.Set(smi.def.hugFrenzyCooldown, smi);
    }));
  }

  private Reactable GetHugReactable(HugMonitor.Instance smi) => (Reactable) new HugMinionReactable(smi.gameObject);

  private void UpdateWantsHugCooldownTimer(HugMonitor.Instance smi, float dt)
  {
    double num = (double) this.wantsHugCooldownTimer.DeltaClamp(-dt, 0.0f, float.MaxValue, smi);
  }

  private void UpdateHugEggCooldownTimer(HugMonitor.Instance smi, float dt)
  {
    double num = (double) this.hugEggCooldownTimer.DeltaClamp(-dt, 0.0f, float.MaxValue, smi);
  }

  private void UpdateHugFrenzyTimer(HugMonitor.Instance smi, float dt)
  {
    double num = (double) this.hugFrenzyTimer.DeltaClamp(-dt, 0.0f, float.MaxValue, smi);
  }

  public class HUGTUNING
  {
    public const float HUG_EGG_TIME = 15f;
    public const float HUG_DUPE_WAIT = 60f;
    public const float FRENZY_EGGS_PER_CYCLE = 6f;
    public const float FRENZY_EGG_TRAVEL_TIME_BUFFER = 5f;
    public const float HUG_FRENZY_DURATION = 120f;
  }

  public class Def : StateMachine.BaseDef
  {
    public float hugsPerCycle = 2f;
    public float scanningInterval = 30f;
    public float hugFrenzyDuration = 120f;
    public float hugFrenzyCooldown = 480f;
    public float hugFrenzyCooldownFailed = 120f;
    public float scanningIntervalFrenzy = 15f;
  }

  public class HugReadyStates : 
    GameStateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.State
  {
    public GameStateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.State passiveHug;
    public GameStateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.State seekingHug;
  }

  public class NormalStates : 
    GameStateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.State
  {
    public GameStateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.State idle;
    public HugMonitor.HugReadyStates hugReady;
  }

  public new class Instance : 
    GameStateMachine<HugMonitor, HugMonitor.Instance, IStateMachineTarget, HugMonitor.Def>.GameInstance
  {
    public GameObject hugParticleFx;
    public Vector3 hugParticleOffset;
    public GameObject hugTarget;
    [MyCmpGet]
    private Effects effects;
    public Effect frenzyEffect;

    public Instance(IStateMachineTarget master, HugMonitor.Def def)
      : base(master, def)
    {
      this.frenzyEffect = Db.Get().effects.Get("HuggingFrenzy");
      this.RefreshSearchTime();
      double num = (double) this.smi.sm.wantsHugCooldownTimer.Set(Random.Range(this.smi.def.hugFrenzyCooldownFailed, this.smi.def.hugFrenzyCooldown), this.smi);
    }

    private void RefreshSearchTime()
    {
      if (Object.op_Equality((Object) this.hugTarget, (Object) null))
      {
        double num1 = (double) this.smi.sm.hugEggCooldownTimer.Set(this.GetScanningInterval(), this.smi);
      }
      else
      {
        double num2 = (double) this.smi.sm.hugEggCooldownTimer.Set(this.GetHugInterval(), this.smi);
      }
    }

    private float GetScanningInterval() => !this.IsHuggingFrenzy() ? this.def.scanningInterval : this.def.scanningIntervalFrenzy;

    private float GetHugInterval() => this.IsHuggingFrenzy() ? 0.0f : 600f / this.def.hugsPerCycle;

    public bool IsHuggingFrenzy() => this.smi.GetCurrentState() == this.smi.sm.hugFrenzy;

    public bool IsHugging() => this.smi.GetSMI<AnimInterruptMonitor.Instance>().anims != null;

    public bool UpdateHasTarget()
    {
      if (Object.op_Equality((Object) this.hugTarget, (Object) null))
      {
        if ((double) this.smi.sm.hugEggCooldownTimer.Get(this.smi) > 0.0)
          return false;
        this.FindEgg();
        this.RefreshSearchTime();
      }
      return Object.op_Inequality((Object) this.hugTarget, (Object) null);
    }

    public void EnterHuggingFrenzy()
    {
      double num1 = (double) this.smi.sm.hugFrenzyTimer.Set(this.smi.def.hugFrenzyDuration, this.smi);
      double num2 = (double) this.smi.sm.hugEggCooldownTimer.Set(0.0f, this.smi);
    }

    private void FindEgg()
    {
      this.hugTarget = (GameObject) null;
      ListPool<ScenePartitionerEntry, GameScenePartitioner>.PooledList gathered_entries = ListPool<ScenePartitionerEntry, GameScenePartitioner>.Allocate();
      ListPool<KMonoBehaviour, SquirrelHugConfig>.PooledList pooledList = ListPool<KMonoBehaviour, SquirrelHugConfig>.Allocate();
      Extents extents = new Extents(Grid.PosToCell(TransformExtensions.GetPosition(this.master.transform)), 10);
      GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.completeBuildings, (List<ScenePartitionerEntry>) gathered_entries);
      GameScenePartitioner.Instance.GatherEntries(extents, GameScenePartitioner.Instance.pickupablesLayer, (List<ScenePartitionerEntry>) gathered_entries);
      Navigator component1 = this.GetComponent<Navigator>();
      foreach (ScenePartitionerEntry partitionerEntry in (List<ScenePartitionerEntry>) gathered_entries)
      {
        KMonoBehaviour cmp = partitionerEntry.obj as KMonoBehaviour;
        KPrefabID component2 = ((Component) cmp).GetComponent<KPrefabID>();
        if (!component2.HasTag(GameTags.Creatures.ReservedByCreature))
        {
          int cell = Grid.PosToCell(cmp);
          if (component1.CanReach(cell))
          {
            EggIncubator component3 = ((Component) cmp).GetComponent<EggIncubator>();
            if (Object.op_Inequality((Object) component3, (Object) null))
            {
              if (Object.op_Equality((Object) component3.Occupant, (Object) null) || component3.Occupant.HasTag(GameTags.Creatures.ReservedByCreature) || !component3.Occupant.HasTag(GameTags.Egg) || component3.Occupant.GetComponent<Effects>().HasEffect("EggHug"))
                continue;
            }
            else if (!component2.HasTag(GameTags.Egg) || ((Component) cmp).GetComponent<Effects>().HasEffect("EggHug"))
              continue;
            ((List<KMonoBehaviour>) pooledList).Add(cmp);
          }
        }
      }
      if (((List<KMonoBehaviour>) pooledList).Count > 0)
      {
        int index = Random.Range(0, ((List<KMonoBehaviour>) pooledList).Count);
        this.hugTarget = ((Component) ((List<KMonoBehaviour>) pooledList)[index]).gameObject;
      }
      gathered_entries.Recycle();
      pooledList.Recycle();
    }
  }
}
