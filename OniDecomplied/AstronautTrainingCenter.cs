// Decompiled with JetBrains decompiler
// Type: AstronautTrainingCenter
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using STRINGS;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/AstronautTrainingCenter")]
public class AstronautTrainingCenter : Workable
{
  public float daysToMasterRole;
  private Chore chore;
  public Chore.Precondition IsNotMarkedForDeconstruction = new Chore.Precondition()
  {
    id = nameof (IsNotMarkedForDeconstruction),
    description = (string) DUPLICANTS.CHORES.PRECONDITIONS.IS_MARKED_FOR_DECONSTRUCTION,
    fn = (Chore.PreconditionFn) ((ref Chore.Precondition.Context context, object data) =>
    {
      Deconstructable deconstructable = data as Deconstructable;
      return Object.op_Equality((Object) deconstructable, (Object) null) || !deconstructable.IsMarkedForDeconstruction();
    })
  };

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.chore = this.CreateChore();
  }

  private Chore CreateChore() => (Chore) new WorkChore<AstronautTrainingCenter>(Db.Get().ChoreTypes.Train, (IStateMachineTarget) this, allow_in_red_alert: false);

  protected override void OnStartWork(Worker worker)
  {
    base.OnStartWork(worker);
    ((Component) this).GetComponent<Operational>().SetActive(true);
  }

  protected override bool OnWorkTick(Worker worker, float dt)
  {
    Object.op_Equality((Object) worker, (Object) null);
    return true;
  }

  protected override void OnCompleteWork(Worker worker)
  {
    base.OnCompleteWork(worker);
    if (this.chore != null && !this.chore.isComplete)
      this.chore.Cancel("completed but not complete??");
    this.chore = this.CreateChore();
  }

  protected override void OnStopWork(Worker worker)
  {
    base.OnStopWork(worker);
    ((Component) this).GetComponent<Operational>().SetActive(false);
  }

  public override float GetPercentComplete()
  {
    Object.op_Equality((Object) this.worker, (Object) null);
    return 0.0f;
  }
}
