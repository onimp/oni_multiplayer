// Decompiled with JetBrains decompiler
// Type: Telepad
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Telepad : StateMachineComponent<Telepad.StatesInstance>
{
  [MyCmpReq]
  private KSelectable selectable;
  private MeterController meter;
  private const float MAX_IMMIGRATION_TIME = 120f;
  private const int NUM_METER_NOTCHES = 8;
  private List<MinionStartingStats> minionStats;
  public float startingSkillPoints;
  public static readonly HashedString[] PortalBirthAnim = new HashedString[1]
  {
    HashedString.op_Implicit("portalbirth")
  };

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    ((Component) this).GetComponent<Deconstructable>().allowDeconstruction = false;
    int x = 0;
    int y = 0;
    Grid.CellToXY(Grid.PosToCell((KMonoBehaviour) this), out x, out y);
    if (x != 0)
      return;
    Debug.LogError((object) ("Headquarters spawned at: (" + x.ToString() + "," + y.ToString() + ")"));
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    Components.Telepads.Add(this);
    this.meter = new MeterController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, new string[4]
    {
      "meter_target",
      "meter_fill",
      "meter_frame",
      "meter_OL"
    });
    this.meter.gameObject.GetComponent<KBatchedAnimController>().SetDirty();
    this.smi.StartSM();
  }

  protected override void OnCleanUp()
  {
    Components.Telepads.Remove(this);
    base.OnCleanUp();
  }

  public void Update()
  {
    if (this.smi.IsColonyLost())
      return;
    if (Immigration.Instance.ImmigrantsAvailable && ((Component) this).GetComponent<Operational>().IsOperational)
    {
      this.smi.sm.openPortal.Trigger(this.smi);
      this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.NewDuplicantsAvailable, (object) this);
    }
    else
    {
      this.smi.sm.closePortal.Trigger(this.smi);
      this.selectable.SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.Wattson, (object) this);
    }
    if ((double) this.GetTimeRemaining() >= -120.0)
      return;
    Messenger.Instance.QueueMessage((Message) new DuplicantsLeftMessage());
    Immigration.Instance.EndImmigration();
  }

  public void RejectAll()
  {
    Immigration.Instance.EndImmigration();
    this.smi.sm.closePortal.Trigger(this.smi);
  }

  public void OnAcceptDelivery(ITelepadDeliverable delivery)
  {
    int cell = Grid.PosToCell((KMonoBehaviour) this);
    Immigration.Instance.EndImmigration();
    GameObject go = delivery.Deliver(Grid.CellToPosCBC(cell, Grid.SceneLayer.Move));
    MinionIdentity component1 = go.GetComponent<MinionIdentity>();
    if (Object.op_Inequality((Object) component1, (Object) null))
    {
      ReportManager.Instance.ReportValue(ReportManager.ReportType.PersonalTime, GameClock.Instance.GetTimeSinceStartOfReport(), string.Format((string) UI.ENDOFDAYREPORT.NOTES.PERSONAL_TIME, (object) DUPLICANTS.CHORES.NOT_EXISTING_TASK), go.GetProperName());
      foreach (Component worldItem in Components.LiveMinionIdentities.GetWorldItems(((Component) this).gameObject.GetComponent<KSelectable>().GetMyWorldId()))
        worldItem.GetComponent<Effects>().Add("NewCrewArrival", true);
      MinionResume component2 = ((Component) component1).GetComponent<MinionResume>();
      for (int index = 0; (double) index < (double) this.startingSkillPoints; ++index)
        component2.ForceAddSkillPoint();
    }
    this.smi.sm.closePortal.Trigger(this.smi);
  }

  public float GetTimeRemaining() => Immigration.Instance.GetTimeRemaining();

  public class StatesInstance : 
    GameStateMachine<Telepad.States, Telepad.StatesInstance, Telepad, object>.GameInstance
  {
    public StatesInstance(Telepad master)
      : base(master)
    {
    }

    public bool IsColonyLost() => Object.op_Inequality((Object) GameFlowManager.Instance, (Object) null) && GameFlowManager.Instance.IsGameOver();

    public void UpdateMeter() => this.master.meter.SetPositionPercent(Mathf.Clamp01((float) (1.0 - (double) Immigration.Instance.GetTimeRemaining() / (double) Immigration.Instance.GetTotalWaitTime())));
  }

  public class States : GameStateMachine<Telepad.States, Telepad.StatesInstance, Telepad>
  {
    public StateMachine<Telepad.States, Telepad.StatesInstance, Telepad, object>.Signal openPortal;
    public StateMachine<Telepad.States, Telepad.StatesInstance, Telepad, object>.Signal closePortal;
    public StateMachine<Telepad.States, Telepad.StatesInstance, Telepad, object>.Signal idlePortal;
    public GameStateMachine<Telepad.States, Telepad.StatesInstance, Telepad, object>.State idle;
    public GameStateMachine<Telepad.States, Telepad.StatesInstance, Telepad, object>.State resetToIdle;
    public GameStateMachine<Telepad.States, Telepad.StatesInstance, Telepad, object>.State opening;
    public GameStateMachine<Telepad.States, Telepad.StatesInstance, Telepad, object>.State open;
    public GameStateMachine<Telepad.States, Telepad.StatesInstance, Telepad, object>.State close;
    public GameStateMachine<Telepad.States, Telepad.StatesInstance, Telepad, object>.State unoperational;
    private static readonly HashedString[] workingAnims = new HashedString[2]
    {
      HashedString.op_Implicit("working_loop"),
      HashedString.op_Implicit("working_pst")
    };

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.idle;
      this.serializable = StateMachine.SerializeType.Both_DEPRECATED;
      this.root.OnSignal(this.idlePortal, this.resetToIdle);
      this.resetToIdle.GoTo(this.idle);
      this.idle.Enter((StateMachine<Telepad.States, Telepad.StatesInstance, Telepad, object>.State.Callback) (smi => smi.UpdateMeter())).Update("TelepadMeter", (Action<Telepad.StatesInstance, float>) ((smi, dt) => smi.UpdateMeter()), (UpdateRate) 7).EventTransition(GameHashes.OperationalChanged, this.unoperational, (StateMachine<Telepad.States, Telepad.StatesInstance, Telepad, object>.Transition.ConditionCallback) (smi => !smi.GetComponent<Operational>().IsOperational)).PlayAnim("idle").OnSignal(this.openPortal, this.opening);
      this.unoperational.PlayAnim("idle").Enter("StopImmigration", (StateMachine<Telepad.States, Telepad.StatesInstance, Telepad, object>.State.Callback) (smi => smi.master.meter.SetPositionPercent(0.0f))).EventTransition(GameHashes.OperationalChanged, this.idle, (StateMachine<Telepad.States, Telepad.StatesInstance, Telepad, object>.Transition.ConditionCallback) (smi => smi.GetComponent<Operational>().IsOperational));
      this.opening.Enter((StateMachine<Telepad.States, Telepad.StatesInstance, Telepad, object>.State.Callback) (smi => smi.master.meter.SetPositionPercent(1f))).PlayAnim("working_pre").OnAnimQueueComplete(this.open);
      this.open.OnSignal(this.closePortal, this.close).Enter((StateMachine<Telepad.States, Telepad.StatesInstance, Telepad, object>.State.Callback) (smi => smi.master.meter.SetPositionPercent(1f))).PlayAnim("working_loop", (KAnim.PlayMode) 0).Transition(this.close, (StateMachine<Telepad.States, Telepad.StatesInstance, Telepad, object>.Transition.ConditionCallback) (smi => smi.IsColonyLost())).EventTransition(GameHashes.OperationalChanged, this.close, (StateMachine<Telepad.States, Telepad.StatesInstance, Telepad, object>.Transition.ConditionCallback) (smi => !smi.GetComponent<Operational>().IsOperational));
      this.close.Enter((StateMachine<Telepad.States, Telepad.StatesInstance, Telepad, object>.State.Callback) (smi => smi.master.meter.SetPositionPercent(0.0f))).PlayAnims((Func<Telepad.StatesInstance, HashedString[]>) (smi => Telepad.States.workingAnims)).OnAnimQueueComplete(this.idle);
    }
  }
}
