// Decompiled with JetBrains decompiler
// Type: OxygenBreather
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using KSerialization;
using System;
using UnityEngine;

[RequireComponent(typeof (Health))]
[AddComponentMenu("KMonoBehaviour/scripts/OxygenBreather")]
public class OxygenBreather : KMonoBehaviour, ISim200ms
{
  public static CellOffset[] DEFAULT_BREATHABLE_OFFSETS = new CellOffset[6]
  {
    new CellOffset(0, 0),
    new CellOffset(0, 1),
    new CellOffset(1, 1),
    new CellOffset(-1, 1),
    new CellOffset(1, 0),
    new CellOffset(-1, 0)
  };
  public float O2toCO2conversion = 0.5f;
  public float lowOxygenThreshold;
  public float noOxygenThreshold;
  public Vector2 mouthOffset;
  [Serialize]
  public float accumulatedCO2;
  [SerializeField]
  public float minCO2ToEmit = 0.3f;
  private bool hasAir = true;
  private Timer hasAirTimer = new Timer();
  [MyCmpAdd]
  private Notifier notifier;
  [MyCmpGet]
  private Facing facing;
  private HandleVector<int>.Handle o2Accumulator = HandleVector<int>.InvalidHandle;
  private HandleVector<int>.Handle co2Accumulator = HandleVector<int>.InvalidHandle;
  private AmountInstance temperature;
  private AttributeInstance airConsumptionRate;
  public CellOffset[] breathableCells;
  public Action<Sim.MassConsumedCallback> onSimConsume;
  private OxygenBreather.IGasProvider gasProvider;
  private static readonly EventSystem.IntraObjectHandler<OxygenBreather> OnDeadTagAddedDelegate = GameUtil.CreateHasTagHandler<OxygenBreather>(GameTags.Dead, (Action<OxygenBreather, object>) ((component, data) => component.OnDeath(data)));

  public float CO2EmitRate => Game.Instance.accumulators.GetAverageRate(this.co2Accumulator);

  public HandleVector<int>.Handle O2Accumulator => this.o2Accumulator;

  protected virtual void OnPrefabInit() => GameUtil.SubscribeToTags<OxygenBreather>(this, OxygenBreather.OnDeadTagAddedDelegate, true);

  public bool IsLowOxygen() => (double) this.GetOxygenPressure(this.mouthCell) < (double) this.lowOxygenThreshold;

  protected virtual void OnSpawn()
  {
    this.airConsumptionRate = Db.Get().Attributes.AirConsumptionRate.Lookup((Component) this);
    this.o2Accumulator = Game.Instance.accumulators.Add("O2", (KMonoBehaviour) this);
    this.co2Accumulator = Game.Instance.accumulators.Add("CO2", (KMonoBehaviour) this);
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    component.AddStatusItem(Db.Get().DuplicantStatusItems.BreathingO2, (object) this);
    component.AddStatusItem(Db.Get().DuplicantStatusItems.EmittingCO2, (object) this);
    this.temperature = Db.Get().Amounts.Temperature.Lookup((Component) this);
    NameDisplayScreen.Instance.RegisterComponent(((Component) this).gameObject, (object) this);
  }

  protected virtual void OnCleanUp()
  {
    Game.Instance.accumulators.Remove(this.o2Accumulator);
    Game.Instance.accumulators.Remove(this.co2Accumulator);
    this.SetGasProvider((OxygenBreather.IGasProvider) null);
    base.OnCleanUp();
  }

  public void Consume(Sim.MassConsumedCallback mass_consumed)
  {
    if (this.onSimConsume == null)
      return;
    this.onSimConsume(mass_consumed);
  }

  public void Sim200ms(float dt)
  {
    if (((Component) this).gameObject.HasTag(GameTags.Dead))
      return;
    float amount1 = this.airConsumptionRate.GetTotalValue() * dt;
    bool flag = this.gasProvider.ConsumeGas(this, amount1);
    if (flag)
    {
      if (this.gasProvider.ShouldEmitCO2())
      {
        float amount2 = amount1 * this.O2toCO2conversion;
        Game.Instance.accumulators.Accumulate(this.co2Accumulator, amount2);
        this.accumulatedCO2 += amount2;
        if ((double) this.accumulatedCO2 >= (double) this.minCO2ToEmit)
        {
          this.accumulatedCO2 -= this.minCO2ToEmit;
          Vector3 position1 = TransformExtensions.GetPosition(this.transform);
          Vector3 position2 = position1;
          position2.x += this.facing.GetFacing() ? -this.mouthOffset.x : this.mouthOffset.x;
          position2.y += this.mouthOffset.y;
          position2.z -= 0.5f;
          if (Mathf.FloorToInt(position2.x) != Mathf.FloorToInt(position1.x))
            position2.x = Mathf.Floor(position1.x) + (this.facing.GetFacing() ? 0.01f : 0.99f);
          CO2Manager.instance.SpawnBreath(position2, this.minCO2ToEmit, this.temperature.value, this.facing.GetFacing());
        }
      }
      else if (this.gasProvider.ShouldStoreCO2())
      {
        Equippable equippable = ((Component) this).GetComponent<SuitEquipper>().IsWearingAirtightSuit();
        if (Object.op_Inequality((Object) equippable, (Object) null))
        {
          float amount3 = amount1 * this.O2toCO2conversion;
          Game.Instance.accumulators.Accumulate(this.co2Accumulator, amount3);
          this.accumulatedCO2 += amount3;
          if ((double) this.accumulatedCO2 >= (double) this.minCO2ToEmit)
          {
            this.accumulatedCO2 -= this.minCO2ToEmit;
            ((Component) equippable).GetComponent<Storage>().AddGasChunk(SimHashes.CarbonDioxide, this.minCO2ToEmit, this.temperature.value, byte.MaxValue, 0, false);
          }
        }
      }
    }
    if (flag != this.hasAir)
    {
      this.hasAirTimer.Start();
      if (!this.hasAirTimer.TryStop(2f))
        return;
      this.hasAir = flag;
    }
    else
      this.hasAirTimer.Stop();
  }

  private void OnDeath(object data)
  {
    ((Behaviour) this).enabled = false;
    KSelectable component = ((Component) this).GetComponent<KSelectable>();
    component.RemoveStatusItem(Db.Get().DuplicantStatusItems.BreathingO2);
    component.RemoveStatusItem(Db.Get().DuplicantStatusItems.EmittingCO2);
  }

  private int GetMouthCellAtCell(int cell, CellOffset[] offsets)
  {
    float num = 0.0f;
    int mouthCellAtCell = cell;
    foreach (CellOffset offset in offsets)
    {
      int cell1 = Grid.OffsetCell(cell, offset);
      float oxygenPressure = this.GetOxygenPressure(cell1);
      if ((double) oxygenPressure > (double) num && (double) oxygenPressure > (double) this.noOxygenThreshold)
      {
        num = oxygenPressure;
        mouthCellAtCell = cell1;
      }
    }
    return mouthCellAtCell;
  }

  public int mouthCell => this.GetMouthCellAtCell(Grid.PosToCell((KMonoBehaviour) this), this.breathableCells);

  public bool IsBreathableElementAtCell(int cell, CellOffset[] offsets = null) => this.GetBreathableElementAtCell(cell, offsets) != SimHashes.Vacuum;

  public SimHashes GetBreathableElementAtCell(int cell, CellOffset[] offsets = null)
  {
    if (offsets == null)
      offsets = this.breathableCells;
    int mouthCellAtCell = this.GetMouthCellAtCell(cell, offsets);
    if (!Grid.IsValidCell(mouthCellAtCell))
      return SimHashes.Vacuum;
    Element element = Grid.Element[mouthCellAtCell];
    return (!element.IsGas || !element.HasTag(GameTags.Breathable) ? 0 : ((double) Grid.Mass[mouthCellAtCell] > (double) this.noOxygenThreshold ? 1 : 0)) == 0 ? SimHashes.Vacuum : element.id;
  }

  public bool IsUnderLiquid => Grid.Element[this.mouthCell].IsLiquid;

  public bool IsSuffocating => !this.hasAir;

  public SimHashes GetBreathableElement => this.GetBreathableElementAtCell(Grid.PosToCell((KMonoBehaviour) this));

  public bool IsBreathableElement => this.IsBreathableElementAtCell(Grid.PosToCell((KMonoBehaviour) this));

  private float GetOxygenPressure(int cell) => Grid.IsValidCell(cell) && Grid.Element[cell].HasTag(GameTags.Breathable) ? Grid.Mass[cell] : 0.0f;

  public OxygenBreather.IGasProvider GetGasProvider() => this.gasProvider;

  public void SetGasProvider(OxygenBreather.IGasProvider gas_provider)
  {
    if (this.gasProvider != null)
      this.gasProvider.OnClearOxygenBreather(this);
    this.gasProvider = gas_provider;
    if (this.gasProvider == null)
      return;
    this.gasProvider.OnSetOxygenBreather(this);
  }

  public interface IGasProvider
  {
    void OnSetOxygenBreather(OxygenBreather oxygen_breather);

    void OnClearOxygenBreather(OxygenBreather oxygen_breather);

    bool ConsumeGas(OxygenBreather oxygen_breather, float amount);

    bool ShouldEmitCO2();

    bool ShouldStoreCO2();
  }
}
