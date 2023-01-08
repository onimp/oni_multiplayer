// Decompiled with JetBrains decompiler
// Type: ToiletWorkableUse
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using KSerialization;
using STRINGS;
using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/ToiletWorkableUse")]
public class ToiletWorkableUse : Workable, IGameObjectEffectDescriptor
{
  [Serialize]
  public int timesUsed;

  private ToiletWorkableUse() => this.SetReportType(ReportManager.ReportType.PersonalTime);

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.showProgressBar = true;
    this.resetProgressOnStop = true;
    this.attributeConverter = Db.Get().AttributeConverters.ToiletSpeed;
    this.SetWorkTime(8.5f);
  }

  protected override void OnStartWork(Worker worker)
  {
    base.OnStartWork(worker);
    if (Sim.IsRadiationEnabled() && (double) worker.GetAmounts().Get(Db.Get().Amounts.RadiationBalance).value > 0.0)
      ((Component) worker).gameObject.GetComponent<KSelectable>().AddStatusItem(Db.Get().DuplicantStatusItems.ExpellingRads);
    Game.Instance.roomProber.GetRoomOfGameObject(((Component) this).gameObject)?.roomType.TriggerRoomEffects(((Component) this).GetComponent<KPrefabID>(), ((Component) worker).GetComponent<Effects>());
  }

  protected override void OnStopWork(Worker worker)
  {
    if (Sim.IsRadiationEnabled())
      ((Component) worker).gameObject.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().DuplicantStatusItems.ExpellingRads);
    base.OnStopWork(worker);
  }

  protected override void OnAbortWork(Worker worker)
  {
    if (Sim.IsRadiationEnabled())
      ((Component) worker).gameObject.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().DuplicantStatusItems.ExpellingRads);
    base.OnAbortWork(worker);
  }

  protected override void OnCompleteWork(Worker worker)
  {
    double num1 = (double) Db.Get().Amounts.Bladder.Lookup((Component) worker).SetValue(0.0f);
    if (Sim.IsRadiationEnabled())
    {
      ((Component) worker).gameObject.GetComponent<KSelectable>().RemoveStatusItem(Db.Get().DuplicantStatusItems.ExpellingRads);
      AmountInstance amountInstance = Db.Get().Amounts.RadiationBalance.Lookup((Component) worker);
      float d = Math.Min(amountInstance.value, 100f * ((Component) worker).GetSMI<RadiationMonitor.Instance>().difficultySettingMod);
      if ((double) d >= 1.0)
        PopFXManager.Instance.SpawnFX(PopFXManager.Instance.sprite_Negative, Math.Floor((double) d).ToString() + (string) UI.UNITSUFFIXES.RADIATION.RADS, worker.transform, Vector3.op_Multiply(Vector3.up, 2f));
      double num2 = (double) amountInstance.ApplyDelta(-d);
    }
    ++this.timesUsed;
    this.Trigger(-350347868, (object) worker);
    base.OnCompleteWork(worker);
  }
}
