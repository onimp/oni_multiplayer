// Decompiled with JetBrains decompiler
// Type: EggIncubator
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class EggIncubator : SingleEntityReceptacle, ISaveLoadable, ISim1000ms
{
  [MyCmpAdd]
  private EggIncubatorWorkable workable;
  [MyCmpAdd]
  private CopyBuildingSettings copySettings;
  private Chore chore;
  private EggIncubatorStates.Instance smi;
  private KBatchedAnimTracker tracker;
  private MeterController meter;
  private static readonly EventSystem.IntraObjectHandler<EggIncubator> OnOperationalChangedDelegate = new EventSystem.IntraObjectHandler<EggIncubator>((Action<EggIncubator, object>) ((component, data) => component.OnOperationalChanged(data)));
  private static readonly EventSystem.IntraObjectHandler<EggIncubator> OnOccupantChangedDelegate = new EventSystem.IntraObjectHandler<EggIncubator>((Action<EggIncubator, object>) ((component, data) => component.OnOccupantChanged(data)));
  private static readonly EventSystem.IntraObjectHandler<EggIncubator> OnStorageChangeDelegate = new EventSystem.IntraObjectHandler<EggIncubator>((Action<EggIncubator, object>) ((component, data) => component.OnStorageChange(data)));
  private static readonly EventSystem.IntraObjectHandler<EggIncubator> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<EggIncubator>((Action<EggIncubator, object>) ((component, data) => component.OnCopySettings(data)));

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.autoReplaceEntity = true;
    this.choreType = Db.Get().ChoreTypes.RanchingFetch;
    this.statusItemNeed = Db.Get().BuildingStatusItems.NeedEgg;
    this.statusItemNoneAvailable = Db.Get().BuildingStatusItems.NoAvailableEgg;
    this.statusItemAwaitingDelivery = Db.Get().BuildingStatusItems.AwaitingEggDelivery;
    this.requiredSkillPerk = Db.Get().SkillPerks.CanWrangleCreatures.Id;
    this.occupyingObjectRelativePosition = new Vector3(0.5f, 1f, -1f);
    this.synchronizeAnims = false;
    ((Component) this).GetComponent<KBatchedAnimController>().SetSymbolVisiblity(KAnimHashedString.op_Implicit("egg_target"), false);
    this.meter = new MeterController((KMonoBehaviour) this, Meter.Offset.Infront, Grid.SceneLayer.NoLayer, Array.Empty<string>());
    this.Subscribe<EggIncubator>(-905833192, EggIncubator.OnCopySettingsDelegate);
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    if (Object.op_Implicit((Object) this.occupyingObject))
    {
      if (this.occupyingObject.HasTag(GameTags.Creature))
        this.storage.allowItemRemoval = true;
      this.storage.RenotifyAll();
      this.PositionOccupyingObject();
    }
    this.Subscribe<EggIncubator>(-592767678, EggIncubator.OnOperationalChangedDelegate);
    this.Subscribe<EggIncubator>(-731304873, EggIncubator.OnOccupantChangedDelegate);
    this.Subscribe<EggIncubator>(-1697596308, EggIncubator.OnStorageChangeDelegate);
    this.smi = new EggIncubatorStates.Instance((IStateMachineTarget) this);
    this.smi.StartSM();
  }

  private void OnCopySettings(object data)
  {
    EggIncubator component1 = ((GameObject) data).GetComponent<EggIncubator>();
    if (!Object.op_Inequality((Object) component1, (Object) null))
      return;
    this.autoReplaceEntity = component1.autoReplaceEntity;
    if (Object.op_Equality((Object) this.occupyingObject, (Object) null))
    {
      if ((!Tag.op_Equality(this.requestedEntityTag, component1.requestedEntityTag) ? 0 : (Tag.op_Equality(this.requestedEntityAdditionalFilterTag, component1.requestedEntityAdditionalFilterTag) ? 1 : 0)) == 0)
        this.CancelActiveRequest();
      if (this.fetchChore == null)
        this.CreateOrder(component1.requestedEntityTag, component1.requestedEntityAdditionalFilterTag);
    }
    if (!Object.op_Inequality((Object) this.occupyingObject, (Object) null))
      return;
    Prioritizable component2 = ((Component) this).GetComponent<Prioritizable>();
    if (!Object.op_Inequality((Object) component2, (Object) null))
      return;
    Prioritizable component3 = this.occupyingObject.GetComponent<Prioritizable>();
    if (!Object.op_Inequality((Object) component3, (Object) null))
      return;
    component3.SetMasterPriority(component2.GetMasterPriority());
  }

  protected override void OnCleanUp()
  {
    this.smi.StopSM("cleanup");
    base.OnCleanUp();
  }

  protected override void SubscribeToOccupant()
  {
    base.SubscribeToOccupant();
    if (Object.op_Inequality((Object) this.occupyingObject, (Object) null))
    {
      this.tracker = this.occupyingObject.AddComponent<KBatchedAnimTracker>();
      this.tracker.symbol = HashedString.op_Implicit("egg_target");
      this.tracker.forceAlwaysVisible = true;
    }
    this.UpdateProgress();
  }

  protected override void UnsubscribeFromOccupant()
  {
    base.UnsubscribeFromOccupant();
    Object.Destroy((Object) this.tracker);
    this.tracker = (KBatchedAnimTracker) null;
    this.UpdateProgress();
  }

  private void OnOperationalChanged(object data = null)
  {
    if (Object.op_Implicit((Object) this.occupyingObject))
      return;
    this.storage.DropAll(false, false, new Vector3(), true, (List<GameObject>) null);
  }

  private void OnOccupantChanged(object data = null)
  {
    if (Object.op_Implicit((Object) this.occupyingObject))
      return;
    this.storage.allowItemRemoval = false;
  }

  private void OnStorageChange(object data = null)
  {
    if (!Object.op_Implicit((Object) this.occupyingObject) || this.storage.items.Contains(this.occupyingObject))
      return;
    this.UnsubscribeFromOccupant();
    this.ClearOccupant();
  }

  protected override void ClearOccupant()
  {
    bool flag = false;
    if (Object.op_Inequality((Object) this.occupyingObject, (Object) null))
      flag = !this.occupyingObject.HasTag(GameTags.Egg);
    base.ClearOccupant();
    if (!(this.autoReplaceEntity & flag) || !((Tag) ref this.requestedEntityTag).IsValid)
      return;
    this.CreateOrder(this.requestedEntityTag, Tag.Invalid);
  }

  protected override void PositionOccupyingObject()
  {
    base.PositionOccupyingObject();
    this.occupyingObject.GetComponent<KBatchedAnimController>().SetSceneLayer(Grid.SceneLayer.BuildingUse);
    KSelectable component = this.occupyingObject.GetComponent<KSelectable>();
    if (!Object.op_Inequality((Object) component, (Object) null))
      return;
    component.IsSelectable = true;
  }

  public override void OrderRemoveOccupant()
  {
    Object.Destroy((Object) this.tracker);
    this.tracker = (KBatchedAnimTracker) null;
    this.storage.DropAll(false, false, new Vector3(), true, (List<GameObject>) null);
    this.occupyingObject = (GameObject) null;
    this.ClearOccupant();
  }

  public float GetProgress()
  {
    float progress = 0.0f;
    if (Object.op_Implicit((Object) this.occupyingObject))
    {
      AmountInstance amountInstance = this.occupyingObject.GetAmounts().Get(Db.Get().Amounts.Incubation);
      progress = amountInstance == null ? 1f : amountInstance.value / amountInstance.GetMax();
    }
    return progress;
  }

  private void UpdateProgress() => this.meter.SetPositionPercent(this.GetProgress());

  public void Sim1000ms(float dt)
  {
    this.UpdateProgress();
    this.UpdateChore();
  }

  public void StoreBaby(GameObject baby)
  {
    this.UnsubscribeFromOccupant();
    this.storage.DropAll(false, false, new Vector3(), true, (List<GameObject>) null);
    this.storage.allowItemRemoval = true;
    this.storage.Store(baby);
    this.occupyingObject = baby;
    this.SubscribeToOccupant();
    this.Trigger(-731304873, (object) this.occupyingObject);
  }

  private void UpdateChore()
  {
    if (this.operational.IsOperational && this.EggNeedsAttention())
    {
      if (this.chore != null)
        return;
      this.chore = (Chore) new WorkChore<EggIncubatorWorkable>(Db.Get().ChoreTypes.EggSing, (IStateMachineTarget) this.workable);
    }
    else
    {
      if (this.chore == null)
        return;
      this.chore.Cancel("now is not the time for song");
      this.chore = (Chore) null;
    }
  }

  private bool EggNeedsAttention()
  {
    if (!Object.op_Implicit((Object) this.Occupant))
      return false;
    IncubationMonitor.Instance smi = this.Occupant.GetSMI<IncubationMonitor.Instance>();
    return smi != null && !smi.HasSongBuff();
  }
}
