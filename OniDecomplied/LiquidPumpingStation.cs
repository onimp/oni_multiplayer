// Decompiled with JetBrains decompiler
// Type: LiquidPumpingStation
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using System;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/Workable/LiquidPumpingStation")]
public class LiquidPumpingStation : Workable, ISim200ms
{
  private static readonly CellOffset[] liquidOffsets = new CellOffset[10]
  {
    new CellOffset(0, 0),
    new CellOffset(1, 0),
    new CellOffset(0, -1),
    new CellOffset(1, -1),
    new CellOffset(0, -2),
    new CellOffset(1, -2),
    new CellOffset(0, -3),
    new CellOffset(1, -3),
    new CellOffset(0, -4),
    new CellOffset(1, -4)
  };
  private LiquidPumpingStation.LiquidInfo[] infos;
  private int infoCount;
  private int depthAvailable = -1;
  private LiquidPumpingStation.WorkSession session;
  private MeterController meter;

  protected override void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.resetProgressOnStop = true;
    this.showProgressBar = false;
  }

  protected override void OnSpawn()
  {
    base.OnSpawn();
    this.infos = new LiquidPumpingStation.LiquidInfo[LiquidPumpingStation.liquidOffsets.Length * 2];
    this.RefreshStatusItem();
    this.Sim200ms(0.0f);
    this.SetWorkTime(10f);
    this.RefreshDepthAvailable();
    this.meter = new MeterController((KAnimControllerBase) ((Component) this).GetComponent<KBatchedAnimController>(), "meter_target", "meter", Meter.Offset.Behind, Grid.SceneLayer.NoLayer, new string[3]
    {
      "meter_target",
      "meter_arrow",
      "meter_scale"
    });
    foreach (GameObject gameObject in ((Component) this).GetComponent<Storage>().items)
    {
      if (!Object.op_Equality((Object) gameObject, (Object) null) && Object.op_Inequality((Object) gameObject, (Object) null))
        TracesExtesions.DeleteObject(gameObject);
    }
  }

  private void RefreshDepthAvailable()
  {
    int depthAvailable = PumpingStationGuide.GetDepthAvailable(Grid.PosToCell((KMonoBehaviour) this), ((Component) this).gameObject);
    int num = 4;
    if (depthAvailable <= this.depthAvailable)
      return;
    KAnimControllerBase component = ((Component) this).GetComponent<KAnimControllerBase>();
    for (int index = 1; index <= num; ++index)
      component.SetSymbolVisiblity(KAnimHashedString.op_Implicit("pipe" + index.ToString()), index <= depthAvailable);
    PumpingStationGuide.OccupyArea(((Component) this).gameObject, depthAvailable);
    this.depthAvailable = depthAvailable;
  }

  public void Sim200ms(float dt)
  {
    if (this.session != null)
      return;
    int infoCount = this.infoCount;
    for (int index = 0; index < this.infoCount; ++index)
      this.infos[index].amount = 0.0f;
    if (((Component) this).GetComponent<Operational>().IsOperational)
    {
      int cell = Grid.PosToCell((KMonoBehaviour) this);
      for (int index1 = 0; index1 < LiquidPumpingStation.liquidOffsets.Length; ++index1)
      {
        if (this.depthAvailable >= Math.Abs(LiquidPumpingStation.liquidOffsets[index1].y))
        {
          int i = Grid.OffsetCell(cell, LiquidPumpingStation.liquidOffsets[index1]);
          bool flag = false;
          Element element = Grid.Element[i];
          if (element.IsLiquid)
          {
            float num = Grid.Mass[i];
            for (int index2 = 0; index2 < this.infoCount; ++index2)
            {
              if (this.infos[index2].element == element)
              {
                this.infos[index2].amount += num;
                flag = true;
                break;
              }
            }
            if (!flag)
            {
              this.infos[this.infoCount].amount = num;
              this.infos[this.infoCount].element = element;
              ++this.infoCount;
            }
          }
        }
      }
    }
    int index3 = 0;
    while (index3 < this.infoCount)
    {
      LiquidPumpingStation.LiquidInfo info = this.infos[index3];
      if ((double) info.amount <= 1.0)
      {
        if (Object.op_Inequality((Object) info.source, (Object) null))
          TracesExtesions.DeleteObject((Component) info.source);
        this.infos[index3] = this.infos[this.infoCount - 1];
        --this.infoCount;
      }
      else
      {
        if (Object.op_Equality((Object) info.source, (Object) null))
        {
          info.source = ((Component) ((Component) this).GetComponent<Storage>().AddLiquid(info.element.id, info.amount, info.element.defaultValues.temperature, byte.MaxValue, 0)).GetComponent<SubstanceChunk>();
          Pickupable component = ((Component) info.source).GetComponent<Pickupable>();
          ((Component) component).GetComponent<KPrefabID>().AddTag(GameTags.LiquidSource, false);
          component.SetOffsets(new CellOffset[1]
          {
            new CellOffset(0, 1)
          });
          component.targetWorkable = (Workable) this;
          component.OnReservationsChanged += new System.Action(this.OnReservationsChanged);
        }
        ((Component) info.source).GetComponent<Pickupable>().TotalAmount = info.amount;
        this.infos[index3] = info;
        ++index3;
      }
    }
    if (infoCount != this.infoCount)
      this.RefreshStatusItem();
    this.RefreshDepthAvailable();
  }

  private void RefreshStatusItem()
  {
    if (this.infoCount > 0)
      ((Component) this).GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.PumpingStation, (object) this);
    else
      ((Component) this).GetComponent<KSelectable>().SetStatusItem(Db.Get().StatusItemCategories.Main, Db.Get().BuildingStatusItems.EmptyPumpingStation, (object) this);
  }

  public string ResolveString(string base_string)
  {
    string newValue = "";
    for (int index = 0; index < this.infoCount; ++index)
    {
      if (Object.op_Inequality((Object) this.infos[index].source, (Object) null))
        newValue = newValue + "\n" + this.infos[index].element.name + ": " + GameUtil.GetFormattedMass(this.infos[index].amount);
    }
    return base_string.Replace("{Liquids}", newValue);
  }

  public static bool IsLiquidAccessible(Element element) => true;

  public override float GetPercentComplete() => this.session != null ? this.session.GetPercentComplete() : 0.0f;

  protected override void OnStartWork(Worker worker)
  {
    base.OnStartWork(worker);
    Pickupable.PickupableStartWorkInfo startWorkInfo = (Pickupable.PickupableStartWorkInfo) worker.startWorkInfo;
    float amount = startWorkInfo.amount;
    Element element = ((Component) startWorkInfo.originalPickupable).GetComponent<PrimaryElement>().Element;
    this.session = new LiquidPumpingStation.WorkSession(Grid.PosToCell((KMonoBehaviour) this), element.id, ((Component) startWorkInfo.originalPickupable).GetComponent<SubstanceChunk>(), amount, ((Component) this).gameObject);
    this.meter.SetPositionPercent(0.0f);
    this.meter.SetSymbolTint(new KAnimHashedString("meter_target"), element.substance.colour);
  }

  protected override void OnStopWork(Worker worker)
  {
    base.OnStopWork(worker);
    if (this.session != null)
    {
      this.session.Cleanup();
      this.session = (LiquidPumpingStation.WorkSession) null;
    }
    ((Component) this).GetComponent<KAnimControllerBase>().Play(HashedString.op_Implicit("on"));
  }

  private void OnReservationsChanged()
  {
    bool is_unfetchable = false;
    for (int index = 0; index < this.infoCount; ++index)
    {
      if (Object.op_Inequality((Object) this.infos[index].source, (Object) null) && (double) ((Component) this.infos[index].source).GetComponent<Pickupable>().ReservedAmount > 0.0)
      {
        is_unfetchable = true;
        break;
      }
    }
    for (int index = 0; index < this.infoCount; ++index)
    {
      if (Object.op_Inequality((Object) this.infos[index].source, (Object) null))
        ((Component) this.infos[index].source).GetSMI<FetchableMonitor.Instance>()?.SetForceUnfetchable(is_unfetchable);
    }
  }

  protected override void OnCompleteWork(Worker worker)
  {
    base.OnCompleteWork(worker);
    if (this.session == null)
      return;
    Storage component1 = ((Component) worker).GetComponent<Storage>();
    float consumedAmount = this.session.GetConsumedAmount();
    if ((double) consumedAmount > 0.0)
    {
      SubstanceChunk source = this.session.GetSource();
      SimUtil.DiseaseInfo diseaseInfo = this.session != null ? this.session.GetDiseaseInfo() : SimUtil.DiseaseInfo.Invalid;
      PrimaryElement component2 = ((Component) source).GetComponent<PrimaryElement>();
      Pickupable component3 = ((Component) LiquidSourceManager.Instance.CreateChunk(component2.Element, consumedAmount, this.session.GetTemperature(), diseaseInfo.idx, diseaseInfo.count, TransformExtensions.GetPosition(this.transform))).GetComponent<Pickupable>();
      component3.TotalAmount = consumedAmount;
      component3.Trigger(1335436905, (object) ((Component) source).GetComponent<Pickupable>());
      worker.workCompleteData = (object) component3;
      this.Sim200ms(0.0f);
      if (Object.op_Inequality((Object) component3, (Object) null))
        component1.Store(((Component) component3).gameObject);
    }
    this.session.Cleanup();
    this.session = (LiquidPumpingStation.WorkSession) null;
  }

  protected override bool OnWorkTick(Worker worker, float dt)
  {
    if (this.session != null)
    {
      this.meter.SetPositionPercent(this.session.GetPercentComplete());
      if ((double) this.session.GetLastTickAmount() <= 0.0)
        return true;
    }
    return false;
  }

  protected override void OnCleanUp()
  {
    base.OnCleanUp();
    if (this.session != null)
    {
      this.session.Cleanup();
      this.session = (LiquidPumpingStation.WorkSession) null;
    }
    for (int index = 0; index < this.infoCount; ++index)
    {
      if (Object.op_Inequality((Object) this.infos[index].source, (Object) null))
        TracesExtesions.DeleteObject((Component) this.infos[index].source);
    }
  }

  private class WorkSession
  {
    private int cell;
    private float amountToPickup;
    private float consumedAmount;
    private float temperature;
    private float amountPerTick;
    private SimHashes element;
    private float lastTickAmount;
    private SubstanceChunk source;
    private SimUtil.DiseaseInfo diseaseInfo;
    private GameObject pump;

    public WorkSession(
      int cell,
      SimHashes element,
      SubstanceChunk source,
      float amount_to_pickup,
      GameObject pump)
    {
      this.cell = cell;
      this.element = element;
      this.source = source;
      this.amountToPickup = amount_to_pickup;
      this.temperature = ElementLoader.FindElementByHash(element).defaultValues.temperature;
      this.diseaseInfo = SimUtil.DiseaseInfo.Invalid;
      this.amountPerTick = 40f;
      this.pump = pump;
      this.lastTickAmount = this.amountPerTick;
      this.ConsumeMass();
    }

    private void OnSimConsumeCallback(Sim.MassConsumedCallback mass_cb_info, object data) => ((LiquidPumpingStation.WorkSession) data).OnSimConsume(mass_cb_info);

    private void OnSimConsume(Sim.MassConsumedCallback mass_cb_info)
    {
      this.temperature = (double) this.consumedAmount != 0.0 ? GameUtil.GetFinalTemperature(this.temperature, this.consumedAmount, mass_cb_info.temperature, mass_cb_info.mass) : mass_cb_info.temperature;
      this.consumedAmount += mass_cb_info.mass;
      this.lastTickAmount = mass_cb_info.mass;
      this.diseaseInfo = SimUtil.CalculateFinalDiseaseInfo(this.diseaseInfo.idx, this.diseaseInfo.count, mass_cb_info.diseaseIdx, mass_cb_info.diseaseCount);
      if ((double) this.consumedAmount >= (double) this.amountToPickup)
      {
        this.amountPerTick = 0.0f;
        this.lastTickAmount = 0.0f;
      }
      this.ConsumeMass();
    }

    private void ConsumeMass()
    {
      if ((double) this.amountPerTick <= 0.0)
        return;
      float mass = Mathf.Max(Mathf.Min(this.amountPerTick, this.amountToPickup - this.consumedAmount), 1f);
      HandleVector<Game.ComplexCallbackInfo<Sim.MassConsumedCallback>>.Handle handle = Game.Instance.massConsumedCallbackManager.Add(new Action<Sim.MassConsumedCallback, object>(this.OnSimConsumeCallback), (object) this, nameof (LiquidPumpingStation));
      int depthAvailable = PumpingStationGuide.GetDepthAvailable(this.cell, this.pump);
      SimMessages.ConsumeMass(Grid.OffsetCell(this.cell, new CellOffset(0, -depthAvailable)), this.element, mass, (byte) (depthAvailable + 1), handle.index);
    }

    public float GetPercentComplete() => this.consumedAmount / this.amountToPickup;

    public float GetLastTickAmount() => this.lastTickAmount;

    public SimUtil.DiseaseInfo GetDiseaseInfo() => this.diseaseInfo;

    public SubstanceChunk GetSource() => this.source;

    public float GetConsumedAmount() => this.consumedAmount;

    public float GetTemperature()
    {
      if ((double) this.temperature > 0.0)
        return this.temperature;
      Debug.LogWarning((object) "TODO(YOG): Fix bad temperature in liquid pumping station.");
      return ElementLoader.FindElementByHash(this.element).defaultValues.temperature;
    }

    public void Cleanup()
    {
      this.amountPerTick = 0.0f;
      this.diseaseInfo = SimUtil.DiseaseInfo.Invalid;
    }
  }

  private struct LiquidInfo
  {
    public float amount;
    public Element element;
    public SubstanceChunk source;
  }
}
