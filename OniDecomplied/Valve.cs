// Decompiled with JetBrains decompiler
// Type: Valve
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/Workable/Valve")]
public class Valve : Workable, ISaveLoadable
{
  [MyCmpReq]
  private ValveBase valveBase;
  [Serialize]
  private float desiredFlow = 0.5f;
  private Chore chore;
  [MyCmpAdd]
  private CopyBuildingSettings copyBuildingSettings;
  private static readonly EventSystem.IntraObjectHandler<Valve> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<Valve>((Action<Valve, object>) ((component, data) => component.OnCopySettings(data)));

  public float QueuedMaxFlow => this.chore == null ? -1f : this.desiredFlow;

  public float DesiredFlow => this.desiredFlow;

  public float MaxFlow => this.valveBase.MaxFlow;

  private void OnCopySettings(object data)
  {
    Valve component = ((GameObject) data).GetComponent<Valve>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    this.ChangeFlow(component.desiredFlow);
  }

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.SetOffsetTable(OffsetGroups.InvertedStandardTable);
    this.synchronizeAnims = false;
    this.valveBase.CurrentFlow = this.valveBase.MaxFlow;
    this.desiredFlow = this.valveBase.MaxFlow;
    this.Subscribe<Valve>(-905833192, Valve.OnCopySettingsDelegate);
  }

  protected override void OnSpawn()
  {
    this.ChangeFlow(this.desiredFlow);
    base.OnSpawn();
    Prioritizable.AddRef(((Component) this).gameObject);
  }

  protected override void OnCleanUp()
  {
    Prioritizable.RemoveRef(((Component) this).gameObject);
    base.OnCleanUp();
  }

  public void ChangeFlow(float amount)
  {
    this.desiredFlow = Mathf.Clamp(amount, 0.0f, this.valveBase.MaxFlow);
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    component.ToggleStatusItem(Db.Get().BuildingStatusItems.PumpingLiquidOrGas, (double) this.desiredFlow >= 0.0, (object) this.valveBase.AccumulatorHandle);
    if (DebugHandler.InstantBuildMode)
      this.UpdateFlow();
    else if ((double) this.desiredFlow != (double) this.valveBase.CurrentFlow)
    {
      if (this.chore != null)
        return;
      component.AddStatusItem(Db.Get().BuildingStatusItems.ValveRequest, (object) this);
      component.AddStatusItem(Db.Get().BuildingStatusItems.PendingWork, (object) this);
      this.chore = (Chore) new WorkChore<Valve>(Db.Get().ChoreTypes.Toggle, (IStateMachineTarget) this, only_when_operational: false);
    }
    else
    {
      if (this.chore != null)
      {
        this.chore.Cancel("desiredFlow == currentFlow");
        this.chore = (Chore) null;
      }
      component.RemoveStatusItem(Db.Get().BuildingStatusItems.ValveRequest);
      component.RemoveStatusItem(Db.Get().BuildingStatusItems.PendingWork);
    }
  }

  protected override void OnCompleteWork(Worker worker)
  {
    base.OnCompleteWork(worker);
    this.UpdateFlow();
  }

  public void UpdateFlow()
  {
    this.valveBase.CurrentFlow = this.desiredFlow;
    this.valveBase.UpdateAnim();
    if (this.chore != null)
      this.chore.Cancel("forced complete");
    this.chore = (Chore) null;
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    component.RemoveStatusItem(Db.Get().BuildingStatusItems.ValveRequest);
    component.RemoveStatusItem(Db.Get().BuildingStatusItems.PendingWork);
  }
}
