// Decompiled with JetBrains decompiler
// Type: Klei.AI.MeteorShowerEvent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Klei.AI
{
  public class MeteorShowerEvent : GameplayEvent<MeteorShowerEvent.StatesInstance>
  {
    private List<MeteorShowerEvent.BombardmentInfo> bombardmentInfo;
    private MathUtil.MinMax secondsBombardmentOff;
    private MathUtil.MinMax secondsBombardmentOn;
    private float secondsPerMeteor = 0.33f;
    private float duration;

    public MeteorShowerEvent(
      string id,
      float duration,
      float secondsPerMeteor,
      MathUtil.MinMax secondsBombardmentOff = default (MathUtil.MinMax),
      MathUtil.MinMax secondsBombardmentOn = default (MathUtil.MinMax))
      : base(id)
    {
      this.duration = duration;
      this.secondsPerMeteor = secondsPerMeteor;
      this.secondsBombardmentOff = secondsBombardmentOff;
      this.secondsBombardmentOn = secondsBombardmentOn;
      this.bombardmentInfo = new List<MeteorShowerEvent.BombardmentInfo>();
      this.tags.Add(GameTags.SpaceDanger);
    }

    public MeteorShowerEvent AddMeteor(string prefab, float weight)
    {
      this.bombardmentInfo.Add(new MeteorShowerEvent.BombardmentInfo()
      {
        prefab = prefab,
        weight = weight
      });
      return this;
    }

    public override StateMachine.Instance GetSMI(
      GameplayEventManager manager,
      GameplayEventInstance eventInstance)
    {
      return (StateMachine.Instance) new MeteorShowerEvent.StatesInstance(manager, eventInstance, this);
    }

    private struct BombardmentInfo
    {
      public string prefab;
      public float weight;
    }

    public class States : 
      GameplayEventStateMachine<MeteorShowerEvent.States, MeteorShowerEvent.StatesInstance, GameplayEventManager, MeteorShowerEvent>
    {
      public GameStateMachine<MeteorShowerEvent.States, MeteorShowerEvent.StatesInstance, GameplayEventManager, object>.State planning;
      public MeteorShowerEvent.States.RunningStates running;
      public GameStateMachine<MeteorShowerEvent.States, MeteorShowerEvent.StatesInstance, GameplayEventManager, object>.State finished;
      public StateMachine<MeteorShowerEvent.States, MeteorShowerEvent.StatesInstance, GameplayEventManager, object>.FloatParameter runTimeRemaining;
      public StateMachine<MeteorShowerEvent.States, MeteorShowerEvent.StatesInstance, GameplayEventManager, object>.FloatParameter bombardTimeRemaining;
      public StateMachine<MeteorShowerEvent.States, MeteorShowerEvent.StatesInstance, GameplayEventManager, object>.FloatParameter snoozeTimeRemaining;

      public override void InitializeStates(out StateMachine.BaseState default_state)
      {
        base.InitializeStates(out default_state);
        default_state = (StateMachine.BaseState) this.planning;
        this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
        this.planning.Enter((StateMachine<MeteorShowerEvent.States, MeteorShowerEvent.StatesInstance, GameplayEventManager, object>.State.Callback) (smi =>
        {
          double num1 = (double) this.runTimeRemaining.Set(smi.gameplayEvent.duration, smi);
          double num2 = (double) this.bombardTimeRemaining.Set(((MathUtil.MinMax) ref smi.gameplayEvent.secondsBombardmentOn).Get(), smi);
          double num3 = (double) this.snoozeTimeRemaining.Set(((MathUtil.MinMax) ref smi.gameplayEvent.secondsBombardmentOff).Get(), smi);
        })).GoTo((GameStateMachine<MeteorShowerEvent.States, MeteorShowerEvent.StatesInstance, GameplayEventManager, object>.State) this.running);
        double num4;
        this.running.DefaultState(this.running.snoozing).Update((Action<MeteorShowerEvent.StatesInstance, float>) ((smi, dt) => num4 = (double) this.runTimeRemaining.Delta(-dt, smi))).ParamTransition<float>((StateMachine<MeteorShowerEvent.States, MeteorShowerEvent.StatesInstance, GameplayEventManager, object>.Parameter<float>) this.runTimeRemaining, this.finished, GameStateMachine<MeteorShowerEvent.States, MeteorShowerEvent.StatesInstance, GameplayEventManager, object>.IsLTEZero);
        double num5;
        double num6;
        this.running.bombarding.Enter((StateMachine<MeteorShowerEvent.States, MeteorShowerEvent.StatesInstance, GameplayEventManager, object>.State.Callback) (smi => smi.StartBackgroundEffects())).Exit((StateMachine<MeteorShowerEvent.States, MeteorShowerEvent.StatesInstance, GameplayEventManager, object>.State.Callback) (smi => smi.StopBackgroundEffects())).Exit((StateMachine<MeteorShowerEvent.States, MeteorShowerEvent.StatesInstance, GameplayEventManager, object>.State.Callback) (smi => num5 = (double) this.bombardTimeRemaining.Set(((MathUtil.MinMax) ref smi.gameplayEvent.secondsBombardmentOn).Get(), smi))).Update((Action<MeteorShowerEvent.StatesInstance, float>) ((smi, dt) => num6 = (double) this.bombardTimeRemaining.Delta(-dt, smi))).ParamTransition<float>((StateMachine<MeteorShowerEvent.States, MeteorShowerEvent.StatesInstance, GameplayEventManager, object>.Parameter<float>) this.bombardTimeRemaining, this.running.snoozing, GameStateMachine<MeteorShowerEvent.States, MeteorShowerEvent.StatesInstance, GameplayEventManager, object>.IsLTEZero).Update((Action<MeteorShowerEvent.StatesInstance, float>) ((smi, dt) => smi.Bombarding(dt)));
        double num7;
        double num8;
        this.running.snoozing.Exit((StateMachine<MeteorShowerEvent.States, MeteorShowerEvent.StatesInstance, GameplayEventManager, object>.State.Callback) (smi => num7 = (double) this.snoozeTimeRemaining.Set(((MathUtil.MinMax) ref smi.gameplayEvent.secondsBombardmentOff).Get(), smi))).Update((Action<MeteorShowerEvent.StatesInstance, float>) ((smi, dt) => num8 = (double) this.snoozeTimeRemaining.Delta(-dt, smi))).ParamTransition<float>((StateMachine<MeteorShowerEvent.States, MeteorShowerEvent.StatesInstance, GameplayEventManager, object>.Parameter<float>) this.snoozeTimeRemaining, this.running.bombarding, GameStateMachine<MeteorShowerEvent.States, MeteorShowerEvent.StatesInstance, GameplayEventManager, object>.IsLTEZero);
        this.finished.ReturnSuccess();
      }

      public class RunningStates : 
        GameStateMachine<MeteorShowerEvent.States, MeteorShowerEvent.StatesInstance, GameplayEventManager, object>.State
      {
        public GameStateMachine<MeteorShowerEvent.States, MeteorShowerEvent.StatesInstance, GameplayEventManager, object>.State bombarding;
        public GameStateMachine<MeteorShowerEvent.States, MeteorShowerEvent.StatesInstance, GameplayEventManager, object>.State snoozing;
      }
    }

    public class StatesInstance : 
      GameplayEventStateMachine<MeteorShowerEvent.States, MeteorShowerEvent.StatesInstance, GameplayEventManager, MeteorShowerEvent>.GameplayEventStateMachineInstance
    {
      public GameObject activeMeteorBackground;
      [Serialize]
      private float nextMeteorTime;
      [Serialize]
      private float timeRemaining;
      [Serialize]
      private float timeBetweenMeteors;
      [Serialize]
      private int m_worldId;

      public StatesInstance(
        GameplayEventManager master,
        GameplayEventInstance eventInstance,
        MeteorShowerEvent meteorShowerEvent)
        : base(master, eventInstance, meteorShowerEvent)
      {
        this.timeRemaining = this.gameplayEvent.duration;
        this.timeBetweenMeteors = this.gameplayEvent.secondsPerMeteor;
        this.m_worldId = eventInstance.worldId;
      }

      public override void StopSM(string reason)
      {
        this.StopBackgroundEffects();
        base.StopSM(reason);
      }

      public void StartBackgroundEffects()
      {
        if (!Object.op_Equality((Object) this.activeMeteorBackground, (Object) null))
          return;
        this.activeMeteorBackground = Util.KInstantiate(EffectPrefabs.Instance.MeteorBackground, (GameObject) null, (string) null);
        WorldContainer world = ClusterManager.Instance.GetWorld(this.m_worldId);
        TransformExtensions.SetPosition(this.activeMeteorBackground.transform, new Vector3((float) (((double) world.maximumBounds.x + (double) world.minimumBounds.x) / 2.0), world.maximumBounds.y, 25f));
        this.activeMeteorBackground.transform.rotation = Quaternion.Euler(90f, 0.0f, 0.0f);
      }

      public void StopBackgroundEffects()
      {
        if (!Object.op_Inequality((Object) this.activeMeteorBackground, (Object) null))
          return;
        ParticleSystem component = this.activeMeteorBackground.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule main = component.main;
        ((ParticleSystem.MainModule) ref main).stopAction = (ParticleSystemStopAction) 2;
        component.Stop();
        if (!component.IsAlive())
          Object.Destroy((Object) this.activeMeteorBackground);
        this.activeMeteorBackground = (GameObject) null;
      }

      public float TimeUntilNextShower() => this.IsInsideState((StateMachine.BaseState) this.sm.running.bombarding) ? 0.0f : this.sm.snoozeTimeRemaining.Get(this);

      public void Bombarding(float dt)
      {
        for (this.nextMeteorTime -= dt; (double) this.nextMeteorTime < 0.0; this.nextMeteorTime += this.timeBetweenMeteors)
          this.DoBombardment(this.gameplayEvent.bombardmentInfo);
      }

      private void DoBombardment(
        List<MeteorShowerEvent.BombardmentInfo> bombardment_info)
      {
        float num1 = 0.0f;
        foreach (MeteorShowerEvent.BombardmentInfo bombardmentInfo in bombardment_info)
          num1 += bombardmentInfo.weight;
        float num2 = Random.Range(0.0f, num1);
        MeteorShowerEvent.BombardmentInfo bombardmentInfo1 = bombardment_info[0];
        int num3 = 0;
        for (; (double) num2 - (double) bombardmentInfo1.weight > 0.0; bombardmentInfo1 = bombardment_info[++num3])
          num2 -= bombardmentInfo1.weight;
        Game.Instance.Trigger(-84771526, (object) null);
        this.SpawnBombard(bombardmentInfo1.prefab);
      }

      private GameObject SpawnBombard(string prefab)
      {
        WorldContainer world = ClusterManager.Instance.GetWorld(this.m_worldId);
        float num1 = (float) world.Width * Random.value + (float) world.WorldOffset.x;
        float num2 = (float) (world.Height + world.WorldOffset.y - 1);
        float layerZ = Grid.GetLayerZ(Grid.SceneLayer.FXFront);
        Vector3 vector3;
        // ISSUE: explicit constructor call
        ((Vector3) ref vector3).\u002Ector(num1, num2, layerZ);
        GameObject gameObject = Util.KInstantiate(Assets.GetPrefab(Tag.op_Implicit(prefab)), vector3, Quaternion.identity, (GameObject) null, (string) null, true, 0);
        gameObject.SetActive(true);
        return gameObject;
      }
    }
  }
}
