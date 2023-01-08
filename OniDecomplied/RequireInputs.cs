// Decompiled with JetBrains decompiler
// Type: RequireInputs
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/RequireInputs")]
public class RequireInputs : KMonoBehaviour, ISim200ms
{
  [SerializeField]
  private bool requirePower = true;
  [SerializeField]
  private bool requireConduit;
  public bool requireConduitHasMass = true;
  public RequireInputs.Requirements visualizeRequirements = RequireInputs.Requirements.All;
  private static readonly Operational.Flag inputConnectedFlag = new Operational.Flag("inputConnected", Operational.Flag.Type.Requirement);
  private static readonly Operational.Flag pipesHaveMass = new Operational.Flag(nameof (pipesHaveMass), Operational.Flag.Type.Requirement);
  private Guid noWireStatusGuid;
  private Guid needPowerStatusGuid;
  private bool requirementsMet;
  private BuildingEnabledButton button;
  private IEnergyConsumer energy;
  public ConduitConsumer conduitConsumer;
  [MyCmpReq]
  private KSelectable selectable;
  [MyCmpGet]
  private Operational operational;
  private bool previouslyConnected = true;
  private bool previouslySatisfied = true;

  public bool RequiresPower => this.requirePower;

  public bool RequiresInputConduit => this.requireConduit;

  public void SetRequirements(bool power, bool conduit)
  {
    this.requirePower = power;
    this.requireConduit = conduit;
  }

  public bool RequirementsMet => this.requirementsMet;

  protected virtual void OnPrefabInit() => this.Bind();

  protected virtual void OnSpawn()
  {
    this.CheckRequirements(true);
    this.Bind();
  }

  [ContextMenu("Bind")]
  private void Bind()
  {
    if (this.requirePower)
    {
      this.energy = ((Component) this).GetComponent<IEnergyConsumer>();
      this.button = ((Component) this).GetComponent<BuildingEnabledButton>();
    }
    if (!this.requireConduit || Object.op_Implicit((Object) this.conduitConsumer))
      return;
    this.conduitConsumer = ((Component) this).GetComponent<ConduitConsumer>();
  }

  public void Sim200ms(float dt) => this.CheckRequirements(false);

  private void CheckRequirements(bool forceEvent)
  {
    bool flag1 = true;
    bool flag2 = false;
    if (this.requirePower)
    {
      bool isConnected = this.energy.IsConnected;
      bool isPowered = this.energy.IsPowered;
      flag1 = flag1 & isPowered & isConnected;
      bool show1 = this.VisualizeRequirement(RequireInputs.Requirements.NeedPower) & isConnected && !isPowered && (Object.op_Equality((Object) this.button, (Object) null) || this.button.IsEnabled);
      bool show2 = this.VisualizeRequirement(RequireInputs.Requirements.NoWire) && !isConnected;
      this.needPowerStatusGuid = this.selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.NeedPower, this.needPowerStatusGuid, show1, (object) this);
      this.noWireStatusGuid = this.selectable.ToggleStatusItem(Db.Get().BuildingStatusItems.NoWireConnected, this.noWireStatusGuid, show2, (object) this);
      flag2 = flag1 != this.RequirementsMet && Object.op_Inequality((Object) ((Component) this).GetComponent<Light2D>(), (Object) null);
    }
    if (this.requireConduit)
    {
      bool flag3 = !((Behaviour) this.conduitConsumer).enabled || this.conduitConsumer.IsConnected;
      bool flag4 = !((Behaviour) this.conduitConsumer).enabled || this.conduitConsumer.IsSatisfied;
      if (this.VisualizeRequirement(RequireInputs.Requirements.ConduitConnected) && this.previouslyConnected != flag3)
      {
        this.previouslyConnected = flag3;
        StatusItem status_item = (StatusItem) null;
        switch (this.conduitConsumer.TypeOfConduit)
        {
          case ConduitType.Gas:
            status_item = Db.Get().BuildingStatusItems.NeedGasIn;
            break;
          case ConduitType.Liquid:
            status_item = Db.Get().BuildingStatusItems.NeedLiquidIn;
            break;
        }
        if (status_item != null)
          this.selectable.ToggleStatusItem(status_item, !flag3, (object) new Tuple<ConduitType, Tag>(this.conduitConsumer.TypeOfConduit, this.conduitConsumer.capacityTag));
        this.operational.SetFlag(RequireInputs.inputConnectedFlag, flag3);
      }
      flag1 &= flag3;
      if (this.VisualizeRequirement(RequireInputs.Requirements.ConduitEmpty) && this.previouslySatisfied != flag4)
      {
        this.previouslySatisfied = flag4;
        StatusItem status_item = (StatusItem) null;
        switch (this.conduitConsumer.TypeOfConduit)
        {
          case ConduitType.Gas:
            status_item = Db.Get().BuildingStatusItems.GasPipeEmpty;
            break;
          case ConduitType.Liquid:
            status_item = Db.Get().BuildingStatusItems.LiquidPipeEmpty;
            break;
        }
        if (this.requireConduitHasMass)
        {
          if (status_item != null)
            this.selectable.ToggleStatusItem(status_item, !flag4, (object) this);
          this.operational.SetFlag(RequireInputs.pipesHaveMass, flag4);
        }
      }
    }
    this.requirementsMet = flag1;
    if (!flag2)
      return;
    Room roomOfGameObject = Game.Instance.roomProber.GetRoomOfGameObject(((Component) this).gameObject);
    if (roomOfGameObject == null)
      return;
    Game.Instance.roomProber.UpdateRoom(roomOfGameObject.cavity);
  }

  public bool VisualizeRequirement(RequireInputs.Requirements r) => (this.visualizeRequirements & r) == r;

  [Flags]
  public enum Requirements
  {
    None = 0,
    NoWire = 1,
    NeedPower = 2,
    ConduitConnected = 4,
    ConduitEmpty = 8,
    AllPower = NeedPower | NoWire, // 0x00000003
    AllConduit = ConduitEmpty | ConduitConnected, // 0x0000000C
    All = AllConduit | AllPower, // 0x0000000F
  }
}
