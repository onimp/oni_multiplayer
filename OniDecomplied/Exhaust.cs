// Decompiled with JetBrains decompiler
// Type: Exhaust
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/Exhaust")]
public class Exhaust : KMonoBehaviour, ISim200ms
{
  [MyCmpGet]
  private Vent vent;
  [MyCmpGet]
  private Storage storage;
  [MyCmpGet]
  private Operational operational;
  [MyCmpGet]
  private ConduitConsumer consumer;
  [MyCmpGet]
  private PrimaryElement exhaustPE;
  private static readonly Operational.Flag canExhaust = new Operational.Flag(nameof (canExhaust), Operational.Flag.Type.Requirement);
  private bool isAnimating;
  private bool recentlyExhausted;
  private const float MinSwitchTime = 1f;
  private float elapsedSwitchTime;
  private static readonly EventSystem.IntraObjectHandler<Exhaust> OnConduitStateChangedDelegate = new EventSystem.IntraObjectHandler<Exhaust>((Action<Exhaust, object>) ((component, data) => component.OnConduitStateChanged(data)));
  private static Exhaust.EmitDelegate emit_element = (Exhaust.EmitDelegate) ((cell, primary_element) => SimMessages.AddRemoveSubstance(cell, primary_element.ElementID, CellEventLogger.Instance.ExhaustSimUpdate, primary_element.Mass, primary_element.Temperature, primary_element.DiseaseIdx, primary_element.DiseaseCount));
  private static Exhaust.EmitDelegate emit_particle = (Exhaust.EmitDelegate) ((cell, primary_element) => FallingWater.instance.AddParticle(cell, primary_element.Element.idx, primary_element.Mass, primary_element.Temperature, primary_element.DiseaseIdx, primary_element.DiseaseCount, true, debug_track: true));

  protected virtual void OnPrefabInit()
  {
    base.OnPrefabInit();
    this.Subscribe<Exhaust>(-592767678, Exhaust.OnConduitStateChangedDelegate);
    this.Subscribe<Exhaust>(-111137758, Exhaust.OnConduitStateChangedDelegate);
    ((Component) this).GetComponent<RequireInputs>().visualizeRequirements = RequireInputs.Requirements.NoWire;
    this.simRenderLoadBalance = true;
  }

  protected virtual void OnSpawn() => this.OnConduitStateChanged((object) null);

  private void OnConduitStateChanged(object data) => this.operational.SetActive(this.operational.IsOperational && !this.vent.IsBlocked);

  private void CalculateDiseaseTransfer(
    PrimaryElement item1,
    PrimaryElement item2,
    float transfer_rate,
    out int disease_to_item1,
    out int disease_to_item2)
  {
    disease_to_item1 = (int) ((double) item2.DiseaseCount * (double) transfer_rate);
    disease_to_item2 = (int) ((double) item1.DiseaseCount * (double) transfer_rate);
  }

  public void Sim200ms(float dt)
  {
    this.operational.SetFlag(Exhaust.canExhaust, !this.vent.IsBlocked);
    if (!this.operational.IsOperational)
    {
      if (!this.isAnimating)
        return;
      this.isAnimating = false;
      this.recentlyExhausted = false;
      this.Trigger(-793429877, (object) null);
    }
    else
    {
      this.UpdateEmission();
      this.elapsedSwitchTime -= dt;
      if ((double) this.elapsedSwitchTime > 0.0)
        return;
      this.elapsedSwitchTime = 1f;
      if (this.recentlyExhausted != this.isAnimating)
      {
        this.isAnimating = this.recentlyExhausted;
        this.Trigger(-793429877, (object) null);
      }
      this.recentlyExhausted = false;
    }
  }

  public bool IsAnimating() => this.isAnimating;

  private void UpdateEmission()
  {
    if ((double) this.consumer.ConsumptionRate == 0.0 || this.storage.items.Count == 0)
      return;
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
    if (Grid.Solid[cell])
      return;
    switch (this.consumer.TypeOfConduit)
    {
      case ConduitType.Gas:
        this.EmitGas(cell);
        break;
      case ConduitType.Liquid:
        this.EmitLiquid(cell);
        break;
    }
  }

  private bool EmitCommon(int cell, PrimaryElement primary_element, Exhaust.EmitDelegate emit)
  {
    if ((double) primary_element.Mass <= 0.0)
      return false;
    int disease_to_item1;
    int disease_to_item2;
    this.CalculateDiseaseTransfer(this.exhaustPE, primary_element, 0.05f, out disease_to_item1, out disease_to_item2);
    primary_element.ModifyDiseaseCount(-disease_to_item1, "Exhaust transfer");
    primary_element.AddDisease(this.exhaustPE.DiseaseIdx, disease_to_item2, "Exhaust transfer");
    this.exhaustPE.ModifyDiseaseCount(-disease_to_item2, "Exhaust transfer");
    this.exhaustPE.AddDisease(primary_element.DiseaseIdx, disease_to_item1, "Exhaust transfer");
    emit(cell, primary_element);
    if (Object.op_Inequality((Object) this.vent, (Object) null))
      this.vent.UpdateVentedMass(primary_element.ElementID, primary_element.Mass);
    primary_element.KeepZeroMassObject = true;
    primary_element.Mass = 0.0f;
    primary_element.ModifyDiseaseCount(int.MinValue, "Exhaust.SimUpdate");
    this.recentlyExhausted = true;
    return true;
  }

  private void EmitLiquid(int cell)
  {
    int num = Grid.CellBelow(cell);
    Exhaust.EmitDelegate emit = !Grid.IsValidCell(num) || Grid.Solid[num] ? Exhaust.emit_element : Exhaust.emit_particle;
    foreach (GameObject gameObject in this.storage.items)
    {
      PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
      if (component.Element.IsLiquid && this.EmitCommon(cell, component, emit))
        break;
    }
  }

  private void EmitGas(int cell)
  {
    foreach (GameObject gameObject in this.storage.items)
    {
      PrimaryElement component = gameObject.GetComponent<PrimaryElement>();
      if (component.Element.IsGas && this.EmitCommon(cell, component, Exhaust.emit_element))
        break;
    }
  }

  private delegate void EmitDelegate(int cell, PrimaryElement primary_element);
}
