// Decompiled with JetBrains decompiler
// Type: ChoreConsumerState
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using UnityEngine;

public class ChoreConsumerState
{
  public KPrefabID prefabid;
  public GameObject gameObject;
  public ChoreConsumer consumer;
  public ChoreProvider choreProvider;
  public Navigator navigator;
  public Ownable ownable;
  public Assignables assignables;
  public MinionResume resume;
  public ChoreDriver choreDriver;
  public Schedulable schedulable;
  public Traits traits;
  public Equipment equipment;
  public Storage storage;
  public ConsumableConsumer consumableConsumer;
  public KSelectable selectable;
  public Worker worker;
  public SolidTransferArm solidTransferArm;
  public bool hasSolidTransferArm;
  public ScheduleBlock scheduleBlock;

  public ChoreConsumerState(ChoreConsumer consumer)
  {
    this.consumer = consumer;
    this.navigator = ((Component) consumer).GetComponent<Navigator>();
    this.prefabid = ((Component) consumer).GetComponent<KPrefabID>();
    this.ownable = ((Component) consumer).GetComponent<Ownable>();
    this.gameObject = ((Component) consumer).gameObject;
    this.solidTransferArm = ((Component) consumer).GetComponent<SolidTransferArm>();
    this.hasSolidTransferArm = Object.op_Inequality((Object) this.solidTransferArm, (Object) null);
    this.resume = ((Component) consumer).GetComponent<MinionResume>();
    this.choreDriver = ((Component) consumer).GetComponent<ChoreDriver>();
    this.schedulable = ((Component) consumer).GetComponent<Schedulable>();
    this.traits = ((Component) consumer).GetComponent<Traits>();
    this.choreProvider = ((Component) consumer).GetComponent<ChoreProvider>();
    MinionIdentity component = ((Component) consumer).GetComponent<MinionIdentity>();
    if (Object.op_Inequality((Object) component, (Object) null))
    {
      if (component.assignableProxy == null)
        component.assignableProxy = MinionAssignablesProxy.InitAssignableProxy(component.assignableProxy, (IAssignableIdentity) component);
      this.assignables = (Assignables) component.GetSoleOwner();
      this.equipment = component.GetEquipment();
    }
    else
    {
      this.assignables = ((Component) consumer).GetComponent<Assignables>();
      this.equipment = ((Component) consumer).GetComponent<Equipment>();
    }
    this.storage = ((Component) consumer).GetComponent<Storage>();
    this.consumableConsumer = ((Component) consumer).GetComponent<ConsumableConsumer>();
    this.worker = ((Component) consumer).GetComponent<Worker>();
    this.selectable = ((Component) consumer).GetComponent<KSelectable>();
    if (!Object.op_Inequality((Object) this.schedulable, (Object) null))
      return;
    int blockIdx = Schedule.GetBlockIdx();
    this.scheduleBlock = this.schedulable.GetSchedule().GetBlock(blockIdx);
  }

  public void Refresh()
  {
    if (!Object.op_Inequality((Object) this.schedulable, (Object) null))
      return;
    int blockIdx = Schedule.GetBlockIdx();
    Schedule schedule = this.schedulable.GetSchedule();
    if (schedule == null)
      return;
    this.scheduleBlock = schedule.GetBlock(blockIdx);
  }
}
