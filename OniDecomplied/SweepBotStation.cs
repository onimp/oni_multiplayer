// Decompiled with JetBrains decompiler
// Type: SweepBotStation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using STRINGS;
using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/SweepBotStation")]
public class SweepBotStation : KMonoBehaviour
{
  [Serialize]
  public Ref<KSelectable> sweepBot;
  [Serialize]
  public string storedName;
  private Operational.Flag dockedRobot = new Operational.Flag(nameof (dockedRobot), Operational.Flag.Type.Functional);
  private MeterController meter;
  [SerializeField]
  private Storage botMaterialStorage;
  [SerializeField]
  private Storage sweepStorage;
  private SchedulerHandle newSweepyHandle;
  private static readonly EventSystem.IntraObjectHandler<SweepBotStation> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<SweepBotStation>((Action<SweepBotStation, object>) ((component, data) => component.OnOperationalChanged(data)));
  private int refreshSweepbotHandle = -1;
  private int sweepBotNameChangeHandle = -1;

  public void SetStorages(Storage botMaterialStorage, Storage sweepStorage)
  {
    this.botMaterialStorage = botMaterialStorage;
    this.sweepStorage = sweepStorage;
  }

  protected virtual void OnPrefabInit()
  {
    this.Initialize(false);
    this.Subscribe<SweepBotStation>(-592767678, SweepBotStation.OnOperationalChangedDelegate);
  }

  protected void Initialize(bool use_logic_meter)
  {
    base.OnPrefabInit();
    ((Component) this).GetComponent<Operational>().SetFlag(this.dockedRobot, false);
  }

  protected virtual void OnSpawn()
  {
    this.Subscribe(-1697596308, new Action<object>(this.OnStorageChanged));
    this.meter = new MeterController((KAnimControllerBase) ((Component) this).gameObject.GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[2]
    {
      "meter_frame",
      "meter_level"
    });
    if (this.sweepBot == null || Object.op_Equality((Object) this.sweepBot.Get(), (Object) null))
    {
      this.RequestNewSweepBot();
    }
    else
    {
      StorageUnloadMonitor.Instance smi = ((Component) this.sweepBot.Get()).GetSMI<StorageUnloadMonitor.Instance>();
      smi.sm.sweepLocker.Set(this.sweepStorage, smi);
      this.RefreshSweepBotSubscription();
    }
    this.UpdateMeter();
    this.UpdateNameDisplay();
  }

  private void RequestNewSweepBot(object data = null)
  {
    if (Object.op_Equality((Object) this.botMaterialStorage.FindFirstWithMass(GameTags.RefinedMetal, SweepBotConfig.MASS), (Object) null))
    {
      FetchList2 fetchList2 = new FetchList2(this.botMaterialStorage, Db.Get().ChoreTypes.Fetch);
      fetchList2.Add(GameTags.RefinedMetal, amount: SweepBotConfig.MASS);
      fetchList2.Submit((System.Action) null, true);
    }
    else
      this.MakeNewSweepBot();
  }

  private void MakeNewSweepBot(object data = null)
  {
    if (this.newSweepyHandle.IsValid || (double) this.botMaterialStorage.GetAmountAvailable(GameTags.RefinedMetal) < (double) SweepBotConfig.MASS)
      return;
    PrimaryElement firstWithMass = this.botMaterialStorage.FindFirstWithMass(GameTags.RefinedMetal, SweepBotConfig.MASS);
    if (Object.op_Equality((Object) firstWithMass, (Object) null))
      return;
    SimHashes sweepBotMaterial = firstWithMass.ElementID;
    firstWithMass.Mass -= SweepBotConfig.MASS;
    this.UpdateMeter();
    this.newSweepyHandle = GameScheduler.Instance.Schedule("MakeSweepy", 2f, (Action<object>) (obj =>
    {
      GameObject go = GameUtil.KInstantiate(Assets.GetPrefab(Tag.op_Implicit("SweepBot")), Grid.CellToPos(Grid.CellRight(Grid.PosToCell(((Component) this).gameObject))), Grid.SceneLayer.Creatures);
      go.SetActive(true);
      this.sweepBot = new Ref<KSelectable>(go.GetComponent<KSelectable>());
      if (!string.IsNullOrEmpty(this.storedName))
        ((Component) this.sweepBot.Get()).GetComponent<UserNameable>().SetName(this.storedName);
      this.UpdateNameDisplay();
      StorageUnloadMonitor.Instance smi = go.GetSMI<StorageUnloadMonitor.Instance>();
      smi.sm.sweepLocker.Set(this.sweepStorage, smi);
      ((Component) this.sweepBot.Get()).GetComponent<PrimaryElement>().ElementID = sweepBotMaterial;
      this.RefreshSweepBotSubscription();
      this.newSweepyHandle.ClearScheduler();
    }), (object) null, (SchedulerGroup) null);
    ((Component) this).GetComponent<KBatchedAnimController>().Play(HashedString.op_Implicit("newsweepy"));
  }

  private void RefreshSweepBotSubscription()
  {
    if (this.refreshSweepbotHandle != -1)
    {
      this.sweepBot.Get().Unsubscribe(this.refreshSweepbotHandle);
      this.sweepBot.Get().Unsubscribe(this.sweepBotNameChangeHandle);
    }
    this.refreshSweepbotHandle = this.sweepBot.Get().Subscribe(1969584890, new Action<object>(this.RequestNewSweepBot));
    this.sweepBotNameChangeHandle = this.sweepBot.Get().Subscribe(1102426921, new Action<object>(this.UpdateStoredName));
  }

  private void UpdateStoredName(object data)
  {
    this.storedName = (string) data;
    this.UpdateNameDisplay();
  }

  private void UpdateNameDisplay()
  {
    if (string.IsNullOrEmpty(this.storedName))
      ((Component) this).GetComponent<KSelectable>().SetName(string.Format((string) BUILDINGS.PREFABS.SWEEPBOTSTATION.NAMEDSTATION, (object) ROBOTS.MODELS.SWEEPBOT.NAME));
    else
      ((Component) this).GetComponent<KSelectable>().SetName(string.Format((string) BUILDINGS.PREFABS.SWEEPBOTSTATION.NAMEDSTATION, (object) this.storedName));
    NameDisplayScreen.Instance.UpdateName(((Component) this).gameObject);
  }

  public void DockRobot(bool docked) => ((Component) this).GetComponent<Operational>().SetFlag(this.dockedRobot, docked);

  public void StartCharging()
  {
    ((Component) this).GetComponent<KBatchedAnimController>().Queue(HashedString.op_Implicit("sleep_pre"));
    ((Component) this).GetComponent<KBatchedAnimController>().Queue(HashedString.op_Implicit("sleep_idle"), (KAnim.PlayMode) 0);
  }

  public void StopCharging()
  {
    ((Component) this).GetComponent<KBatchedAnimController>().Play(HashedString.op_Implicit("sleep_pst"));
    this.UpdateNameDisplay();
  }

  protected virtual void OnCleanUp()
  {
    if (this.newSweepyHandle.IsValid)
      this.newSweepyHandle.ClearScheduler();
    if (this.refreshSweepbotHandle == -1 || !Object.op_Inequality((Object) this.sweepBot.Get(), (Object) null))
      return;
    this.sweepBot.Get().Unsubscribe(this.refreshSweepbotHandle);
  }

  private void UpdateMeter()
  {
    float minusStorageMargin = this.GetMaxCapacityMinusStorageMargin();
    float percent_full = Mathf.Clamp01(this.GetAmountStored() / minusStorageMargin);
    if (this.meter == null)
      return;
    this.meter.SetPositionPercent(percent_full);
  }

  private void OnStorageChanged(object data)
  {
    this.UpdateMeter();
    if (this.sweepBot == null || Object.op_Equality((Object) this.sweepBot.Get(), (Object) null))
      this.RequestNewSweepBot();
    KBatchedAnimController component = ((Component) this).GetComponent<KBatchedAnimController>();
    if (component.currentFrame >= component.GetCurrentNumFrames())
      ((Component) this).GetComponent<KBatchedAnimController>().Play(HashedString.op_Implicit("remove"));
    for (int idx = 0; idx < this.sweepStorage.Count; ++idx)
      this.sweepStorage[idx].GetComponent<Clearable>().MarkForClear(allowWhenStored: true);
  }

  private void OnOperationalChanged(object data)
  {
    Operational component = ((Component) this).GetComponent<Operational>();
    if (component.Flags.ContainsValue(false))
      component.SetActive(false);
    else
      component.SetActive(true);
    if (this.sweepBot != null && !Object.op_Equality((Object) this.sweepBot.Get(), (Object) null))
      return;
    this.RequestNewSweepBot();
  }

  private float GetMaxCapacityMinusStorageMargin() => this.sweepStorage.Capacity() - this.sweepStorage.storageFullMargin;

  private float GetAmountStored() => this.sweepStorage.MassStored();
}
