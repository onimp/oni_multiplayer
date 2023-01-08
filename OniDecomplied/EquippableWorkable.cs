// Decompiled with JetBrains decompiler
// Type: EquippableWorkable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using UnityEngine;

[SerializationConfig]
[AddComponentMenu("KMonoBehaviour/Workable/EquippableWorkable")]
public class EquippableWorkable : Workable, ISaveLoadable
{
  [MyCmpReq]
  private Equippable equippable;
  private Chore chore;
  private QualityLevel quality;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.workerStatusItem = Db.Get().DuplicantStatusItems.Equipping;
    this.overrideAnims = new KAnimFile[1]
    {
      Assets.GetAnim(HashedString.op_Implicit("anim_equip_clothing_kanim"))
    };
    this.synchronizeAnims = false;
  }

  public QualityLevel GetQuality() => this.quality;

  public void SetQuality(QualityLevel level) => this.quality = level;

  protected override void OnSpawn()
  {
    this.SetWorkTime(1.5f);
    this.equippable.OnAssign += new Action<IAssignableIdentity>(this.RefreshChore);
  }

  private void CreateChore()
  {
    Debug.Assert(this.chore == null, (object) "chore should be null");
    this.chore = (Chore) new EquipChore((IStateMachineTarget) this);
  }

  public void CancelChore(string reason = "")
  {
    if (this.chore == null)
      return;
    this.chore.Cancel(reason);
    Prioritizable.RemoveRef(((Component) this.equippable).gameObject);
    this.chore = (Chore) null;
  }

  private void RefreshChore(IAssignableIdentity target)
  {
    if (this.chore != null)
      this.CancelChore("Equipment Reassigned");
    if (target == null || ((Component) target.GetSoleOwner()).GetComponent<Equipment>().IsEquipped(this.equippable))
      return;
    this.CreateChore();
  }

  protected override void OnCompleteWork(Worker worker)
  {
    if (this.equippable.assignee == null)
      return;
    Ownables soleOwner = this.equippable.assignee.GetSoleOwner();
    if (!Object.op_Implicit((Object) soleOwner))
      return;
    ((Component) soleOwner).GetComponent<Equipment>().Equip(this.equippable);
  }

  protected override void OnStopWork(Worker worker)
  {
    this.workTimeRemaining = this.GetWorkTime();
    base.OnStopWork(worker);
  }
}
