// Decompiled with JetBrains decompiler
// Type: PajamaDispenser
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using UnityEngine;

public class PajamaDispenser : Workable, IDispenser
{
  private static GameObject pajamaPrefab = (GameObject) null;
  public bool didCompleteChore;
  private WorkChore<PajamaDispenser> chore;
  private static List<Tag> PajamaList;

  public event System.Action OnStopWorkEvent;

  private WorkChore<PajamaDispenser> Chore
  {
    get => this.chore;
    set
    {
      this.chore = value;
      if (this.chore != null)
        ((Component) this).gameObject.GetComponent<KSelectable>().AddStatusItem(Db.Get().BuildingStatusItems.DispenseRequested);
      else
        ((Component) this).gameObject.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.DispenseRequested, true);
    }
  }

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    if (Object.op_Inequality((Object) PajamaDispenser.pajamaPrefab, (Object) null))
      return;
    PajamaDispenser.pajamaPrefab = Assets.GetPrefab(new Tag("SleepClinicPajamas"));
  }

  protected override void OnCompleteWork(Worker worker)
  {
    Vector3 targetPoint = this.GetTargetPoint();
    targetPoint.z = Grid.GetLayerZ(Grid.SceneLayer.BuildingFront);
    Util.KInstantiate(PajamaDispenser.pajamaPrefab, targetPoint, Quaternion.identity, (GameObject) null, (string) null, true, 0).SetActive(true);
    this.didCompleteChore = true;
  }

  protected override void OnStopWork(Worker worker)
  {
    base.OnStopWork(worker);
    if (this.Chore.smi.IsRunning())
      this.Chore.Cancel("work interrupted");
    this.Chore = (WorkChore<PajamaDispenser>) null;
    if (!this.didCompleteChore)
      this.FetchPajamas();
    this.didCompleteChore = false;
    if (this.OnStopWorkEvent == null)
      return;
    this.OnStopWorkEvent();
  }

  [ContextMenu("fetch")]
  public void FetchPajamas()
  {
    if (this.Chore != null)
      return;
    this.didCompleteChore = false;
    this.Chore = new WorkChore<PajamaDispenser>(Db.Get().ChoreTypes.EquipmentFetch, (IStateMachineTarget) this, only_when_operational: false, add_to_daily_report: false);
  }

  public void CancelFetch()
  {
    if (this.Chore == null)
      return;
    this.Chore.Cancel("User Cancelled");
    this.Chore = (WorkChore<PajamaDispenser>) null;
    this.didCompleteChore = false;
    ((Component) this).gameObject.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().BuildingStatusItems.DispenseRequested);
  }

  public List<Tag> DispensedItems() => PajamaDispenser.PajamaList;

  public Tag SelectedItem() => PajamaDispenser.PajamaList[0];

  public void SelectItem(Tag tag)
  {
  }

  public void OnOrderDispense() => this.FetchPajamas();

  public void OnCancelDispense() => this.CancelFetch();

  public bool HasOpenChore() => this.Chore != null;

  static PajamaDispenser()
  {
    List<Tag> tagList = new List<Tag>();
    tagList.Add(Tag.op_Implicit("SleepClinicPajamas"));
    PajamaDispenser.PajamaList = tagList;
  }
}
