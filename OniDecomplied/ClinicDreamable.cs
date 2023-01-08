// Decompiled with JetBrains decompiler
// Type: ClinicDreamable
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/Clinic Dreamable")]
public class ClinicDreamable : Workable
{
  private static GameObject dreamJournalPrefab;
  private static Effect sleepClinic;
  public bool HasStartedThoughts_Dreaming;
  private ChoreDriver dreamer;
  private Equippable equippable;
  private Effects effects;
  private Sleepable sleepable;
  private KSelectable selectable;
  private HashedString dreamAnimName = HashedString.op_Implicit("portal rocket comp");

  public bool DreamIsDisturbed { get; private set; }

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.resetProgressOnStop = false;
    this.workerStatusItem = Db.Get().DuplicantStatusItems.Dreaming;
    this.workingStatusItem = (StatusItem) null;
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    if (Object.op_Equality((Object) ClinicDreamable.dreamJournalPrefab, (Object) null))
    {
      ClinicDreamable.dreamJournalPrefab = Assets.GetPrefab(DreamJournalConfig.ID);
      ClinicDreamable.sleepClinic = Db.Get().effects.Get("SleepClinic");
    }
    this.equippable = ((Component) this).GetComponent<Equippable>();
    Debug.Assert(Object.op_Inequality((Object) this.equippable, (Object) null));
    this.equippable.def.OnEquipCallBack += new Action<Equippable>(this.OnEquipPajamas);
    this.equippable.def.OnUnequipCallBack += new Action<Equippable>(this.OnUnequipPajamas);
    this.OnEquipPajamas(this.equippable);
  }

  protected override void OnCleanUp()
  {
    base.OnCleanUp();
    if (Object.op_Equality((Object) this.equippable, (Object) null))
      return;
    this.equippable.def.OnEquipCallBack -= new Action<Equippable>(this.OnEquipPajamas);
    this.equippable.def.OnUnequipCallBack -= new Action<Equippable>(this.OnUnequipPajamas);
  }

  protected override bool OnWorkTick(Worker worker, float dt)
  {
    if ((double) this.GetPercentComplete() >= 1.0)
    {
      Vector3 position = this.dreamer.transform.position;
      ++position.y;
      position.z = Grid.GetLayerZ(Grid.SceneLayer.Ore);
      Util.KInstantiate(ClinicDreamable.dreamJournalPrefab, position, Quaternion.identity, (GameObject) null, (string) null, true, 0).SetActive(true);
      this.workTimeRemaining = this.GetWorkTime();
    }
    return false;
  }

  public void OnEquipPajamas(Equippable eq)
  {
    if (Object.op_Equality((Object) this.equippable, (Object) null) || Object.op_Inequality((Object) this.equippable, (Object) eq))
      return;
    MinionAssignablesProxy assignee = this.equippable.assignee as MinionAssignablesProxy;
    if (Object.op_Equality((Object) assignee, (Object) null) || assignee.target is StoredMinionIdentity)
      return;
    GameObject targetGameObject = assignee.GetTargetGameObject();
    this.effects = targetGameObject.GetComponent<Effects>();
    this.dreamer = targetGameObject.GetComponent<ChoreDriver>();
    this.selectable = targetGameObject.GetComponent<KSelectable>();
    this.dreamer.Subscribe(-1283701846, new Action<object>(this.WorkerStartedSleeping));
    this.dreamer.Subscribe(-2090444759, new Action<object>(this.WorkerStoppedSleeping));
    this.effects.Add(ClinicDreamable.sleepClinic, true);
    this.selectable.AddStatusItem(Db.Get().DuplicantStatusItems.MegaBrainTank_Pajamas_Wearing);
  }

  public void OnUnequipPajamas(Equippable eq)
  {
    if (Object.op_Equality((Object) this.dreamer, (Object) null) || Object.op_Equality((Object) this.equippable, (Object) null) || Object.op_Inequality((Object) this.equippable, (Object) eq))
      return;
    this.dreamer.Unsubscribe(-1283701846, new Action<object>(this.WorkerStartedSleeping));
    this.dreamer.Unsubscribe(-2090444759, new Action<object>(this.WorkerStoppedSleeping));
    this.selectable.RemoveStatusItem(Db.Get().DuplicantStatusItems.MegaBrainTank_Pajamas_Wearing);
    this.selectable.RemoveStatusItem(Db.Get().DuplicantStatusItems.MegaBrainTank_Pajamas_Sleeping);
    this.effects.Remove(ClinicDreamable.sleepClinic.Id);
    this.StopDreamingThought();
    this.dreamer = (ChoreDriver) null;
    this.selectable = (KSelectable) null;
    this.effects = (Effects) null;
  }

  public void WorkerStartedSleeping(object data)
  {
    SleepChore currentChore = this.dreamer.GetCurrentChore() as SleepChore;
    currentChore.smi.sm.isDisturbedByLight.GetContext(currentChore.smi).onDirty += new Action<SleepChore.StatesInstance>(this.OnSleepDisturbed);
    currentChore.smi.sm.isDisturbedByMovement.GetContext(currentChore.smi).onDirty += new Action<SleepChore.StatesInstance>(this.OnSleepDisturbed);
    currentChore.smi.sm.isDisturbedByNoise.GetContext(currentChore.smi).onDirty += new Action<SleepChore.StatesInstance>(this.OnSleepDisturbed);
    currentChore.smi.sm.isScaredOfDark.GetContext(currentChore.smi).onDirty += new Action<SleepChore.StatesInstance>(this.OnSleepDisturbed);
    this.sleepable = data as Sleepable;
    this.sleepable.Dreamable = this;
    this.StartWork(this.sleepable.worker);
    this.progressBar.Retarget(((Component) this.sleepable).gameObject);
    this.selectable.AddStatusItem(Db.Get().DuplicantStatusItems.MegaBrainTank_Pajamas_Sleeping, (object) this);
    this.StartDreamingThought();
  }

  public void WorkerStoppedSleeping(object data)
  {
    this.selectable.RemoveStatusItem(Db.Get().DuplicantStatusItems.MegaBrainTank_Pajamas_Sleeping);
    SleepChore currentChore = this.dreamer.GetCurrentChore() as SleepChore;
    if (!Util.IsNullOrDestroyed((object) currentChore) && !Util.IsNullOrDestroyed((object) currentChore.smi) && !Util.IsNullOrDestroyed((object) currentChore.smi.sm))
    {
      currentChore.smi.sm.isDisturbedByLight.GetContext(currentChore.smi).onDirty -= new Action<SleepChore.StatesInstance>(this.OnSleepDisturbed);
      currentChore.smi.sm.isDisturbedByMovement.GetContext(currentChore.smi).onDirty -= new Action<SleepChore.StatesInstance>(this.OnSleepDisturbed);
      currentChore.smi.sm.isDisturbedByNoise.GetContext(currentChore.smi).onDirty -= new Action<SleepChore.StatesInstance>(this.OnSleepDisturbed);
      currentChore.smi.sm.isScaredOfDark.GetContext(currentChore.smi).onDirty -= new Action<SleepChore.StatesInstance>(this.OnSleepDisturbed);
    }
    this.StopDreamingThought();
    this.DreamIsDisturbed = false;
    if (Object.op_Inequality((Object) this.worker, (Object) null))
      this.StopWork(this.worker, false);
    if (!Object.op_Inequality((Object) this.sleepable, (Object) null))
      return;
    this.sleepable.Dreamable = (ClinicDreamable) null;
    this.sleepable = (Sleepable) null;
  }

  private void OnSleepDisturbed(SleepChore.StatesInstance smi)
  {
    SleepChore currentChore = this.dreamer.GetCurrentChore() as SleepChore;
    bool flag = currentChore.smi.sm.isDisturbedByLight.Get(currentChore.smi) | currentChore.smi.sm.isDisturbedByMovement.Get(currentChore.smi) | currentChore.smi.sm.isDisturbedByNoise.Get(currentChore.smi) | currentChore.smi.sm.isScaredOfDark.Get(currentChore.smi);
    this.DreamIsDisturbed = flag;
    if (!flag)
      return;
    this.StopDreamingThought();
  }

  private void StartDreamingThought()
  {
    if (!Object.op_Inequality((Object) this.dreamer, (Object) null) || this.HasStartedThoughts_Dreaming)
      return;
    ((Component) this.dreamer).GetSMI<Dreamer.Instance>().SetDream(Db.Get().Dreams.CommonDream);
    ((Component) this.dreamer).GetSMI<Dreamer.Instance>().StartDreaming();
    this.HasStartedThoughts_Dreaming = true;
  }

  private void StopDreamingThought()
  {
    if (!Object.op_Inequality((Object) this.dreamer, (Object) null) || !this.HasStartedThoughts_Dreaming)
      return;
    ((Component) this.dreamer).GetSMI<Dreamer.Instance>().StopDreaming();
    this.HasStartedThoughts_Dreaming = false;
  }
}
