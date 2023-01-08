// Decompiled with JetBrains decompiler
// Type: ToiletMonitor
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

public class ToiletMonitor : GameStateMachine<ToiletMonitor, ToiletMonitor.Instance>
{
  public GameStateMachine<ToiletMonitor, ToiletMonitor.Instance, IStateMachineTarget, object>.State satisfied;
  public GameStateMachine<ToiletMonitor, ToiletMonitor.Instance, IStateMachineTarget, object>.State unsatisfied;

  public override void InitializeStates(out StateMachine.BaseState default_state)
  {
    default_state = (StateMachine.BaseState) this.satisfied;
    this.satisfied.EventHandler(GameHashes.ToiletSensorChanged, (StateMachine<ToiletMonitor, ToiletMonitor.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.RefreshStatusItem())).Exit("ClearStatusItem", (StateMachine<ToiletMonitor, ToiletMonitor.Instance, IStateMachineTarget, object>.State.Callback) (smi => smi.ClearStatusItem()));
  }

  public new class Instance : 
    GameStateMachine<ToiletMonitor, ToiletMonitor.Instance, IStateMachineTarget, object>.GameInstance
  {
    private ToiletSensor toiletSensor;

    public Instance(IStateMachineTarget master)
      : base(master)
    {
      this.toiletSensor = this.GetComponent<Sensors>().GetSensor<ToiletSensor>();
    }

    public void RefreshStatusItem()
    {
      StatusItem status_item = (StatusItem) null;
      if (!this.toiletSensor.AreThereAnyToilets())
        status_item = Db.Get().DuplicantStatusItems.NoToilets;
      else if (!this.toiletSensor.AreThereAnyUsableToilets())
        status_item = Db.Get().DuplicantStatusItems.NoUsableToilets;
      else if (this.toiletSensor.GetNearestUsableToilet() == null)
        status_item = Db.Get().DuplicantStatusItems.ToiletUnreachable;
      this.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Toilet, status_item);
    }

    public void ClearStatusItem() => this.GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Toilet, (StatusItem) null);
  }
}
