// Decompiled with JetBrains decompiler
// Type: SolarPanel
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using UnityEngine;

[SerializationConfig]
public class SolarPanel : Generator
{
  private MeterController meter;
  private HandleVector<int>.Handle accumulator = HandleVector<int>.InvalidHandle;
  private SolarPanel.StatesInstance smi;
  private Guid statusHandle;
  private CellOffset[] solarCellOffsets = new CellOffset[14]
  {
    new CellOffset(-3, 2),
    new CellOffset(-2, 2),
    new CellOffset(-1, 2),
    new CellOffset(0, 2),
    new CellOffset(1, 2),
    new CellOffset(2, 2),
    new CellOffset(3, 2),
    new CellOffset(-3, 1),
    new CellOffset(-2, 1),
    new CellOffset(-1, 1),
    new CellOffset(0, 1),
    new CellOffset(1, 1),
    new CellOffset(2, 1),
    new CellOffset(3, 1)
  };
  private static readonly EventSystem.IntraObjectHandler<SolarPanel> OnActiveChangedDelegate = new EventSystem.IntraObjectHandler<SolarPanel>((Action<SolarPanel, object>) ((component, data) => component.OnActiveChanged(data)));

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.Subscribe<SolarPanel>(824508782, SolarPanel.OnActiveChangedDelegate);
    this.smi = new SolarPanel.StatesInstance(this);
    this.smi.StartSM();
    this.accumulator = Game.Instance.accumulators.Add("Element", (KMonoBehaviour) this);
    this.meter = new MeterController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Infront, Grid.SceneLayer.NoLayer, new string[4]
    {
      "meter_target",
      "meter_fill",
      "meter_frame",
      "meter_OL"
    });
  }

  protected override void OnCleanUp()
  {
    this.smi.StopSM("cleanup");
    Game.Instance.accumulators.Remove(this.accumulator);
    base.OnCleanUp();
  }

  protected void OnActiveChanged(object data)
  {
    StatusItem status_item = ((Operational) data).IsActive ? Db.Get().BuildingStatusItems.Wattage : Db.Get().BuildingStatusItems.GeneratorOffline;
    ((Component) this).GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Power, status_item, (object) this);
  }

  private void UpdateStatusItem()
  {
    this.selectable.RemoveStatusItem(Db.Get().BuildingStatusItems.Wattage);
    if (this.statusHandle == Guid.Empty)
    {
      this.statusHandle = this.selectable.AddStatusItem(Db.Get().BuildingStatusItems.SolarPanelWattage, (object) this);
    }
    else
    {
      if (!(this.statusHandle != Guid.Empty))
        return;
      ((Component) this).GetComponent<KSelectable>().ReplaceStatusItem(this.statusHandle, Db.Get().BuildingStatusItems.SolarPanelWattage, (object) this);
    }
  }

  public override void EnergySim200ms(float dt)
  {
    base.EnergySim200ms(dt);
    ushort circuitId = this.CircuitID;
    this.operational.SetFlag(Generator.wireConnectedFlag, circuitId != ushort.MaxValue);
    if (!this.operational.IsOperational)
      return;
    float num1 = 0.0f;
    foreach (CellOffset solarCellOffset in this.solarCellOffsets)
    {
      int num2 = Grid.LightIntensity[Grid.OffsetCell(Grid.PosToCell((KMonoBehaviour) this), solarCellOffset)];
      num1 += (float) num2 * 0.00053f;
    }
    this.operational.SetActive((double) num1 > 0.0);
    float num3 = Mathf.Clamp(num1, 0.0f, 380f);
    Game.Instance.accumulators.Accumulate(this.accumulator, num3 * dt);
    if ((double) num3 > 0.0)
      this.GenerateJoules(Mathf.Max(num3 * dt, 1f * dt));
    this.meter.SetPositionPercent(Game.Instance.accumulators.GetAverageRate(this.accumulator) / 380f);
    this.UpdateStatusItem();
  }

  public float CurrentWattage => Game.Instance.accumulators.GetAverageRate(this.accumulator);

  public class StatesInstance : 
    GameStateMachine<SolarPanel.States, SolarPanel.StatesInstance, SolarPanel, object>.GameInstance
  {
    public StatesInstance(SolarPanel master)
      : base(master)
    {
    }
  }

  public class States : GameStateMachine<SolarPanel.States, SolarPanel.StatesInstance, SolarPanel>
  {
    public GameStateMachine<SolarPanel.States, SolarPanel.StatesInstance, SolarPanel, object>.State idle;

    public override void InitializeStates(out StateMachine.BaseState default_state)
    {
      default_state = (StateMachine.BaseState) this.idle;
      this.idle.DoNothing();
    }
  }
}
