// Decompiled with JetBrains decompiler
// Type: Chore`1
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

public class Chore<StateMachineInstanceType> : Chore, IStateMachineTarget where StateMachineInstanceType : StateMachine.Instance
{
  public StateMachineInstanceType smi { get; protected set; }

  protected override StateMachine.Instance GetSMI() => (StateMachine.Instance) this.smi;

  public int Subscribe(int hash, Action<object> handler) => ((KMonoBehaviour) this.GetComponent<KPrefabID>()).Subscribe(hash, handler);

  public void Unsubscribe(int hash, Action<object> handler) => ((KMonoBehaviour) this.GetComponent<KPrefabID>()).Unsubscribe(hash, handler);

  public void Unsubscribe(int id) => ((KMonoBehaviour) this.GetComponent<KPrefabID>()).Unsubscribe(id);

  public void Trigger(int hash, object data = null) => ((KMonoBehaviour) this.GetComponent<KPrefabID>()).Trigger(hash, data);

  public ComponentType GetComponent<ComponentType>() => this.target.GetComponent<ComponentType>();

  public override GameObject gameObject => this.target.gameObject;

  public Transform transform => this.target.gameObject.transform;

  public string name => ((Object) this.gameObject).name;

  public override bool isNull => this.target.isNull;

  public Chore(
    ChoreType chore_type,
    IStateMachineTarget target,
    ChoreProvider chore_provider,
    bool run_until_complete = true,
    Action<Chore> on_complete = null,
    Action<Chore> on_begin = null,
    Action<Chore> on_end = null,
    PriorityScreen.PriorityClass master_priority_class = PriorityScreen.PriorityClass.basic,
    int master_priority_value = 5,
    bool is_preemptable = false,
    bool allow_in_context_menu = true,
    int priority_mod = 0,
    bool add_to_daily_report = false,
    ReportManager.ReportType report_type = ReportManager.ReportType.WorkTime)
    : base(chore_type, target, chore_provider, run_until_complete, on_complete, on_begin, on_end, master_priority_class, master_priority_value, is_preemptable, allow_in_context_menu, priority_mod, add_to_daily_report, report_type)
  {
    target.Subscribe(1969584890, new Action<object>(this.OnTargetDestroyed));
    this.reportType = report_type;
    this.addToDailyReport = add_to_daily_report;
    if (!this.addToDailyReport)
      return;
    ReportManager.Instance.ReportValue(ReportManager.ReportType.ChoreStatus, 1f, chore_type.Name, GameUtil.GetChoreName((Chore) this, (object) null));
  }

  public override string ResolveString(string str)
  {
    if (!this.target.isNull)
      str = str.Replace("{Target}", this.target.gameObject.GetProperName());
    return base.ResolveString(str);
  }

  public override void Cleanup()
  {
    base.Cleanup();
    if (this.target != null)
      this.target.Unsubscribe(1969584890, new Action<object>(this.OnTargetDestroyed));
    if (this.onCleanup == null)
      return;
    this.onCleanup((Chore) this);
  }

  private void OnTargetDestroyed(object data) => this.Cancel("Target Destroyed");

  public override bool CanPreempt(Chore.Precondition.Context context) => base.CanPreempt(context);
}
