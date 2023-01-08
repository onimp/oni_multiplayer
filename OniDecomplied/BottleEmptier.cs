// Decompiled with JetBrains decompiler
// Type: BottleEmptier
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using KSerialization;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

[SerializationConfig]
public class BottleEmptier : 
  StateMachineComponent<BottleEmptier.StatesInstance>,
  IGameObjectEffectDescriptor
{
  public float emptyRate = 10f;
  [Serialize]
  public bool allowManualPumpingStationFetching;
  public bool isGasEmptier;
  private static readonly EventSystem.IntraObjectHandler<BottleEmptier> OnRefreshUserMenuDelegate = new EventSystem.IntraObjectHandler<BottleEmptier>((Action<BottleEmptier, object>) ((component, data) => component.OnRefreshUserMenu(data)));
  private static readonly EventSystem.IntraObjectHandler<BottleEmptier> OnCopySettingsDelegate = new EventSystem.IntraObjectHandler<BottleEmptier>((Action<BottleEmptier, object>) ((component, data) => component.OnCopySettings(data)));

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    this.smi.StartSM();
    this.Subscribe<BottleEmptier>(493375141, BottleEmptier.OnRefreshUserMenuDelegate);
    this.Subscribe<BottleEmptier>(-905833192, BottleEmptier.OnCopySettingsDelegate);
  }

  public List<Descriptor> GetDescriptors(GameObject go) => (List<Descriptor>) null;

  private void OnChangeAllowManualPumpingStationFetching()
  {
    this.allowManualPumpingStationFetching = !this.allowManualPumpingStationFetching;
    this.smi.RefreshChore();
  }

  private void OnRefreshUserMenu(object data)
  {
    if (this.isGasEmptier)
      Game.Instance.userMenu.AddButton(((Component) this).gameObject, this.allowManualPumpingStationFetching ? new KIconButtonMenu.ButtonInfo("action_bottler_delivery", (string) UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.DENIED_GAS.NAME, new System.Action(this.OnChangeAllowManualPumpingStationFetching), tooltipText: ((string) UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.DENIED_GAS.TOOLTIP)) : new KIconButtonMenu.ButtonInfo("action_bottler_delivery", (string) UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.ALLOWED_GAS.NAME, new System.Action(this.OnChangeAllowManualPumpingStationFetching), tooltipText: ((string) UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.ALLOWED_GAS.TOOLTIP)), 0.4f);
    else
      Game.Instance.userMenu.AddButton(((Component) this).gameObject, this.allowManualPumpingStationFetching ? new KIconButtonMenu.ButtonInfo("action_bottler_delivery", (string) UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.DENIED.NAME, new System.Action(this.OnChangeAllowManualPumpingStationFetching), tooltipText: ((string) UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.DENIED.TOOLTIP)) : new KIconButtonMenu.ButtonInfo("action_bottler_delivery", (string) UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.ALLOWED.NAME, new System.Action(this.OnChangeAllowManualPumpingStationFetching), tooltipText: ((string) UI.USERMENUACTIONS.MANUAL_PUMP_DELIVERY.ALLOWED.TOOLTIP)), 0.4f);
  }

  private void OnCopySettings(object data)
  {
    this.allowManualPumpingStationFetching = ((GameObject) data).GetComponent<BottleEmptier>().allowManualPumpingStationFetching;
    this.smi.RefreshChore();
  }

  public class StatesInstance : 
    GameStateMachine<BottleEmptier.States, BottleEmptier.StatesInstance, BottleEmptier, object>.GameInstance
  {
    private FetchChore chore;

    public MeterController meter { get; private set; }

    public StatesInstance(BottleEmptier smi)
      : base(smi)
    {
      ((Component) this.master).GetComponent<TreeFilterable>().OnFilterChanged += new Action<HashSet<Tag>>(this.OnFilterChanged);
      this.meter = new MeterController((KAnimControllerBase) this.GetComponent<KBatchedAnimController>(), "meter_target", nameof (meter), Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[3]
      {
        "meter_target",
        "meter_arrow",
        "meter_scale"
      });
      this.Subscribe(-1697596308, new Action<object>(this.OnStorageChange));
      this.Subscribe(644822890, new Action<object>(this.OnOnlyFetchMarkedItemsSettingChanged));
    }

    public void CreateChore()
    {
      HashSet<Tag> tags = this.GetComponent<TreeFilterable>().GetTags();
      Tag[] forbidden_tags;
      if (!this.master.allowManualPumpingStationFetching)
        forbidden_tags = new Tag[2]
        {
          GameTags.LiquidSource,
          GameTags.GasSource
        };
      else
        forbidden_tags = new Tag[0];
      Storage component = this.GetComponent<Storage>();
      this.chore = new FetchChore(Db.Get().ChoreTypes.StorageFetch, component, component.Capacity(), tags, FetchChore.MatchCriteria.MatchID, Tag.Invalid, forbidden_tags);
    }

    public void CancelChore()
    {
      if (this.chore == null)
        return;
      this.chore.Cancel("Storage Changed");
      this.chore = (FetchChore) null;
    }

    public void RefreshChore() => this.GoTo((StateMachine.BaseState) this.sm.unoperational);

    private void OnFilterChanged(HashSet<Tag> tags) => this.RefreshChore();

    private void OnStorageChange(object data)
    {
      Storage component = this.GetComponent<Storage>();
      this.meter.SetPositionPercent(Mathf.Clamp01(component.RemainingCapacity() / component.capacityKg));
    }

    private void OnOnlyFetchMarkedItemsSettingChanged(object data) => this.RefreshChore();

    public void StartMeter()
    {
      PrimaryElement firstPrimaryElement = this.GetFirstPrimaryElement();
      if (Object.op_Equality((Object) firstPrimaryElement, (Object) null))
        return;
      this.meter.SetSymbolTint(new KAnimHashedString("meter_fill"), firstPrimaryElement.Element.substance.colour);
      this.meter.SetSymbolTint(new KAnimHashedString("water1"), firstPrimaryElement.Element.substance.colour);
      this.GetComponent<KBatchedAnimController>().SetSymbolTint(new KAnimHashedString("leak_ceiling"), Color32.op_Implicit(firstPrimaryElement.Element.substance.colour));
    }

    private PrimaryElement GetFirstPrimaryElement()
    {
      Storage component1 = this.GetComponent<Storage>();
      for (int idx = 0; idx < component1.Count; ++idx)
      {
        GameObject gameObject = component1[idx];
        if (!Object.op_Equality((Object) gameObject, (Object) null))
        {
          PrimaryElement component2 = gameObject.GetComponent<PrimaryElement>();
          if (!Object.op_Equality((Object) component2, (Object) null))
            return component2;
        }
      }
      return (PrimaryElement) null;
    }

    public void Emit(float dt)
    {
      PrimaryElement firstPrimaryElement = this.GetFirstPrimaryElement();
      if (Object.op_Equality((Object) firstPrimaryElement, (Object) null))
        return;
      Storage component = this.GetComponent<Storage>();
      float amount = Mathf.Min(firstPrimaryElement.Mass, this.master.emptyRate * dt);
      if ((double) amount <= 0.0)
        return;
      Tag prefabTag = ((Component) firstPrimaryElement).GetComponent<KPrefabID>().PrefabTag;
      float amount_consumed;
      SimUtil.DiseaseInfo disease_info;
      float aggregate_temperature;
      component.ConsumeAndGetDisease(prefabTag, amount, out amount_consumed, out disease_info, out aggregate_temperature);
      Vector3 position = TransformExtensions.GetPosition(this.transform);
      position.y += 1.8f;
      bool flag = this.GetComponent<Rotatable>().GetOrientation() == Orientation.FlipH;
      position.x += flag ? -0.2f : 0.2f;
      int num = Grid.PosToCell(position) + (flag ? -1 : 1);
      if (Grid.Solid[num])
        num += flag ? 1 : -1;
      Element element = firstPrimaryElement.Element;
      ushort idx = element.idx;
      if (element.IsLiquid)
        FallingWater.instance.AddParticle(num, idx, amount_consumed, aggregate_temperature, disease_info.idx, disease_info.count, true);
      else
        SimMessages.ModifyCell(num, idx, aggregate_temperature, amount_consumed, disease_info.idx, disease_info.count);
    }
  }

  public class States : 
    GameStateMachine<BottleEmptier.States, BottleEmptier.StatesInstance, BottleEmptier>
  {
    private StatusItem statusItem;
    public GameStateMachine<BottleEmptier.States, BottleEmptier.StatesInstance, BottleEmptier, object>.State unoperational;
    public GameStateMachine<BottleEmptier.States, BottleEmptier.StatesInstance, BottleEmptier, object>.State waitingfordelivery;
    public GameStateMachine<BottleEmptier.States, BottleEmptier.StatesInstance, BottleEmptier, object>.State emptying;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.waitingfordelivery;
      this.statusItem = new StatusItem(nameof (BottleEmptier), "", "", "", StatusItem.IconType.Info, NotificationType.Neutral, false, OverlayModes.None.ID);
      this.statusItem.resolveStringCallback = (Func<string, object, string>) ((str, data) =>
      {
        BottleEmptier bottleEmptier = (BottleEmptier) data;
        if (Object.op_Equality((Object) bottleEmptier, (Object) null))
          return str;
        return bottleEmptier.allowManualPumpingStationFetching ? (string) (bottleEmptier.isGasEmptier ? BUILDING.STATUSITEMS.CANISTER_EMPTIER.ALLOWED.NAME : BUILDING.STATUSITEMS.BOTTLE_EMPTIER.ALLOWED.NAME) : (string) (bottleEmptier.isGasEmptier ? BUILDING.STATUSITEMS.CANISTER_EMPTIER.DENIED.NAME : BUILDING.STATUSITEMS.BOTTLE_EMPTIER.DENIED.NAME);
      });
      this.statusItem.resolveTooltipCallback = (Func<string, object, string>) ((str, data) =>
      {
        BottleEmptier bottleEmptier = (BottleEmptier) data;
        if (Object.op_Equality((Object) bottleEmptier, (Object) null))
          return str;
        return bottleEmptier.allowManualPumpingStationFetching ? (bottleEmptier.isGasEmptier ? (string) BUILDING.STATUSITEMS.CANISTER_EMPTIER.ALLOWED.TOOLTIP : (string) BUILDING.STATUSITEMS.BOTTLE_EMPTIER.ALLOWED.TOOLTIP) : (bottleEmptier.isGasEmptier ? (string) BUILDING.STATUSITEMS.CANISTER_EMPTIER.DENIED.TOOLTIP : (string) BUILDING.STATUSITEMS.BOTTLE_EMPTIER.DENIED.TOOLTIP);
      });
      this.root.ToggleStatusItem(this.statusItem, (Func<BottleEmptier.StatesInstance, object>) (smi => (object) smi.master));
      this.unoperational.TagTransition(GameTags.Operational, this.waitingfordelivery).PlayAnim("off");
      this.waitingfordelivery.TagTransition(GameTags.Operational, this.unoperational, true).EventTransition(GameHashes.OnStorageChange, this.emptying, (StateMachine<BottleEmptier.States, BottleEmptier.StatesInstance, BottleEmptier, object>.Transition.ConditionCallback) (smi => !smi.GetComponent<Storage>().IsEmpty())).Enter("CreateChore", (StateMachine<BottleEmptier.States, BottleEmptier.StatesInstance, BottleEmptier, object>.State.Callback) (smi => smi.CreateChore())).Exit("CancelChore", (StateMachine<BottleEmptier.States, BottleEmptier.StatesInstance, BottleEmptier, object>.State.Callback) (smi => smi.CancelChore())).PlayAnim("on");
      this.emptying.TagTransition(GameTags.Operational, this.unoperational, true).EventTransition(GameHashes.OnStorageChange, this.waitingfordelivery, (StateMachine<BottleEmptier.States, BottleEmptier.StatesInstance, BottleEmptier, object>.Transition.ConditionCallback) (smi => smi.GetComponent<Storage>().IsEmpty())).Enter("StartMeter", (StateMachine<BottleEmptier.States, BottleEmptier.StatesInstance, BottleEmptier, object>.State.Callback) (smi => smi.StartMeter())).Update("Emit", (Action<BottleEmptier.StatesInstance, float>) ((smi, dt) => smi.Emit(dt))).PlayAnim("working_loop", (KAnim.PlayMode) 0);
    }
  }
}
