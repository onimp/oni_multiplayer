// Decompiled with JetBrains decompiler
// Type: ConduitSleepStates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using System;
using UnityEngine;

public class ConduitSleepStates : 
  GameStateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>
{
  public ConduitSleepStates.DrowsyStates drowsy;
  public ConduitSleepStates.HasConnectorStates connector;
  public GameStateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.State behaviourcomplete;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.connector.moveToSleepLocation;
    this.root.EventTransition(GameHashes.NewDay, (Func<ConduitSleepStates.Instance, KMonoBehaviour>) (smi => (KMonoBehaviour) GameClock.Instance), this.behaviourcomplete).Exit(new StateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.State.Callback(ConduitSleepStates.CleanUp));
    this.connector.moveToSleepLocation.ToggleStatusItem((string) CREATURES.STATUSITEMS.DROWSY.NAME, (string) CREATURES.STATUSITEMS.DROWSY.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main).MoveTo((Func<ConduitSleepStates.Instance, int>) (smi =>
    {
      ConduitSleepMonitor.Instance smi1 = smi.GetSMI<ConduitSleepMonitor.Instance>();
      return smi1.sm.targetSleepCell.Get(smi1);
    }), (GameStateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.State) this.drowsy, this.behaviourcomplete);
    this.drowsy.ToggleStatusItem((string) CREATURES.STATUSITEMS.DROWSY.NAME, (string) CREATURES.STATUSITEMS.DROWSY.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main).Enter((StateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.State.Callback) (smi => smi.GetComponent<Navigator>().SetCurrentNavType(NavType.Ceiling))).Enter((StateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.State.Callback) (smi =>
    {
      if (!GameClock.Instance.IsNighttime())
        return;
      smi.GoTo((StateMachine.BaseState) this.connector.sleep);
    })).DefaultState(this.drowsy.loop);
    this.drowsy.loop.PlayAnim("drowsy_pre").QueueAnim("drowsy_loop", true).EventTransition(GameHashes.Nighttime, (Func<ConduitSleepStates.Instance, KMonoBehaviour>) (smi => (KMonoBehaviour) GameClock.Instance), this.drowsy.pst, (StateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.Transition.ConditionCallback) (smi => GameClock.Instance.IsNighttime()));
    this.drowsy.pst.PlayAnim("drowsy_pst").OnAnimQueueComplete((GameStateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.State) this.connector.sleep);
    this.connector.sleep.ToggleStatusItem((string) CREATURES.STATUSITEMS.SLEEPING.NAME, (string) CREATURES.STATUSITEMS.SLEEPING.TOOLTIP, render_overlay: new HashedString(), category: Db.Get().StatusItemCategories.Main).Enter((StateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.State.Callback) (smi =>
    {
      if (!smi.staterpillar.IsConnectorBuildingSpawned())
      {
        smi.GoTo((StateMachine.BaseState) this.behaviourcomplete);
      }
      else
      {
        smi.GetComponent<Navigator>().SetCurrentNavType(NavType.Ceiling);
        smi.staterpillar.EnableConnector();
        if (smi.staterpillar.IsConnected())
          smi.GoTo((StateMachine.BaseState) this.connector.sleep.connected);
        else
          smi.GoTo((StateMachine.BaseState) this.connector.sleep.noConnection);
      }
    }));
    this.connector.sleep.connected.Enter((StateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.State.Callback) (smi => smi.animController.SetSceneLayer(ConduitSleepStates.GetSleepingLayer(smi)))).Exit((StateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.State.Callback) (smi => smi.animController.SetSceneLayer(Grid.SceneLayer.Creatures))).EventTransition(GameHashes.NewDay, (Func<ConduitSleepStates.Instance, KMonoBehaviour>) (smi => (KMonoBehaviour) GameClock.Instance), this.connector.connectedWake).Transition(this.connector.sleep.noConnection, (StateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.Transition.ConditionCallback) (smi => !smi.staterpillar.IsConnected())).PlayAnim("sleep_charging_pre").QueueAnim("sleep_charging_loop", true).Update(new System.Action<ConduitSleepStates.Instance, float>(ConduitSleepStates.UpdateGulpSymbol), (UpdateRate) 6).EventHandler(GameHashes.OnStorageChange, new GameStateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.GameEvent.Callback(ConduitSleepStates.OnStorageChanged));
    this.connector.sleep.noConnection.PlayAnim("sleep_pre").QueueAnim("sleep_loop", true).ToggleStatusItem(new Func<ConduitSleepStates.Instance, StatusItem>(ConduitSleepStates.GetStatusItem)).EventTransition(GameHashes.NewDay, (Func<ConduitSleepStates.Instance, KMonoBehaviour>) (smi => (KMonoBehaviour) GameClock.Instance), this.connector.noConnectionWake).Transition(this.connector.sleep.connected, (StateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.Transition.ConditionCallback) (smi => smi.staterpillar.IsConnected()));
    this.connector.connectedWake.QueueAnim("sleep_charging_pst").OnAnimQueueComplete(this.behaviourcomplete);
    this.connector.noConnectionWake.QueueAnim("sleep_pst").OnAnimQueueComplete(this.behaviourcomplete);
    this.behaviourcomplete.BehaviourComplete(GameTags.Creatures.WantsConduitConnection);
  }

  private static Grid.SceneLayer GetSleepingLayer(ConduitSleepStates.Instance smi)
  {
    Grid.SceneLayer sleepingLayer;
    switch (smi.staterpillar.conduitLayer)
    {
      case ObjectLayer.GasConduit:
        sleepingLayer = Grid.SceneLayer.Gas;
        break;
      case ObjectLayer.LiquidConduit:
        sleepingLayer = Grid.SceneLayer.GasConduitBridges;
        break;
      case ObjectLayer.Wire:
        sleepingLayer = Grid.SceneLayer.SolidConduitBridges;
        break;
      default:
        sleepingLayer = Grid.SceneLayer.SolidConduitBridges;
        break;
    }
    return sleepingLayer;
  }

  private static StatusItem GetStatusItem(ConduitSleepStates.Instance smi)
  {
    StatusItem statusItem;
    switch (smi.staterpillar.conduitLayer)
    {
      case ObjectLayer.GasConduit:
        statusItem = Db.Get().BuildingStatusItems.NeedGasOut;
        break;
      case ObjectLayer.LiquidConduit:
        statusItem = Db.Get().BuildingStatusItems.NeedLiquidOut;
        break;
      case ObjectLayer.Wire:
        statusItem = Db.Get().BuildingStatusItems.NoWireConnected;
        break;
      default:
        statusItem = Db.Get().BuildingStatusItems.Normal;
        break;
    }
    return statusItem;
  }

  private static void OnStorageChanged(ConduitSleepStates.Instance smi, object obj)
  {
    GameObject gameObject = obj as GameObject;
    if (!Object.op_Inequality((Object) gameObject, (Object) null))
      return;
    smi.amountDeposited += gameObject.GetComponent<PrimaryElement>().Mass;
  }

  private static void UpdateGulpSymbol(ConduitSleepStates.Instance smi, float dt)
  {
    smi.SetGulpSymbolVisibility((double) smi.amountDeposited > 0.0);
    smi.amountDeposited = 0.0f;
  }

  private static void CleanUp(ConduitSleepStates.Instance smi)
  {
    ConduitSleepMonitor.Instance smi1 = smi.GetSMI<ConduitSleepMonitor.Instance>();
    smi1?.sm.targetSleepCell.Set(Grid.InvalidCell, smi1);
    smi.staterpillar.DestroyOrphanedConnectorBuilding();
  }

  public class Def : StateMachine.BaseDef
  {
    public HashedString gulpSymbol = HashedString.op_Implicit("gulp");
  }

  public new class Instance : 
    GameStateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.GameInstance
  {
    [MyCmpReq]
    public KBatchedAnimController animController;
    [MyCmpReq]
    public Staterpillar staterpillar;
    [MyCmpAdd]
    private LoopingSounds loopingSounds;
    public bool gulpSymbolVisible;
    public float amountDeposited;

    public Instance(Chore<ConduitSleepStates.Instance> chore, ConduitSleepStates.Def def)
      : base((IStateMachineTarget) chore, def)
    {
      chore.AddPrecondition(ChorePreconditions.instance.CheckBehaviourPrecondition, (object) GameTags.Creatures.WantsConduitConnection);
    }

    public void SetGulpSymbolVisibility(bool state)
    {
      string sound = GlobalAssets.GetSound("PlugSlug_Charging_Gulp_LP");
      if (this.gulpSymbolVisible == state)
        return;
      this.gulpSymbolVisible = state;
      this.animController.SetSymbolVisiblity(KAnimHashedString.op_Implicit(this.def.gulpSymbol), state);
      if (state)
        this.loopingSounds.StartSound(sound);
      else
        this.loopingSounds.StopSound(sound);
    }
  }

  public class SleepStates : 
    GameStateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.State
  {
    public GameStateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.State connected;
    public GameStateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.State noConnection;
  }

  public class DrowsyStates : 
    GameStateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.State
  {
    public GameStateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.State loop;
    public GameStateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.State pst;
  }

  public class HasConnectorStates : 
    GameStateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.State
  {
    public GameStateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.State moveToSleepLocation;
    public ConduitSleepStates.SleepStates sleep;
    public GameStateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.State noConnectionWake;
    public GameStateMachine<ConduitSleepStates, ConduitSleepStates.Instance, IStateMachineTarget, ConduitSleepStates.Def>.State connectedWake;
  }
}
