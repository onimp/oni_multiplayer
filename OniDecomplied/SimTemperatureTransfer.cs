// Decompiled with JetBrains decompiler
// Type: SimTemperatureTransfer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

[SkipSaveFileSerialization]
[AddComponentMenu("KMonoBehaviour/scripts/SimTemperatureTransfer")]
public class SimTemperatureTransfer : KMonoBehaviour
{
  private const float SIM_FREEZE_SPAWN_ORE_PERCENT = 0.8f;
  public const float MIN_MASS_FOR_TEMPERATURE_TRANSFER = 0.01f;
  public float deltaKJ;
  public Action<SimTemperatureTransfer> onSimRegistered;
  protected int simHandle = -1;
  private float pendingEnergyModifications;
  [SerializeField]
  protected float surfaceArea = 10f;
  [SerializeField]
  protected float thickness = 0.01f;
  [SerializeField]
  protected float groundTransferScale = 1f / 16f;
  private static Dictionary<int, SimTemperatureTransfer> handleInstanceMap = new Dictionary<int, SimTemperatureTransfer>();

  public float SurfaceArea
  {
    get => this.surfaceArea;
    set => this.surfaceArea = value;
  }

  public float Thickness
  {
    get => this.thickness;
    set => this.thickness = value;
  }

  public float GroundTransferScale
  {
    get => this.GroundTransferScale;
    set => this.groundTransferScale = value;
  }

  public int SimHandle => this.simHandle;

  public static void ClearInstanceMap() => SimTemperatureTransfer.handleInstanceMap.Clear();

  public static void DoOreMeltTransition(int sim_handle)
  {
    SimTemperatureTransfer cmp = (SimTemperatureTransfer) null;
    if (!SimTemperatureTransfer.handleInstanceMap.TryGetValue(sim_handle, out cmp) || Object.op_Equality((Object) cmp, (Object) null) || ((Component) cmp).HasTag(GameTags.Sealed))
      return;
    PrimaryElement component = ((Component) cmp).GetComponent<PrimaryElement>();
    Element element = component.Element;
    bool flag1 = (double) component.Temperature >= (double) element.highTemp;
    bool flag2 = (double) component.Temperature <= (double) element.lowTemp;
    DebugUtil.DevAssert(flag1 | flag2, "An ore got a melt message from the sim but it's still the correct temperature for its state!", (Object) component);
    if (flag1 && element.highTempTransitionTarget == SimHashes.Unobtanium || flag2 && element.lowTempTransitionTarget == SimHashes.Unobtanium)
      return;
    if ((double) component.Mass > 0.0)
    {
      int cell = Grid.PosToCell(TransformExtensions.GetPosition(cmp.transform));
      float mass1 = component.Mass;
      int diseaseCount = component.DiseaseCount;
      SimHashes new_element = flag1 ? element.highTempTransitionTarget : element.lowTempTransitionTarget;
      SimHashes hash = flag1 ? element.highTempTransitionOreID : element.lowTempTransitionOreID;
      float num = flag1 ? element.highTempTransitionOreMassConversion : element.lowTempTransitionOreMassConversion;
      if (hash != (SimHashes) 0)
      {
        float mass2 = mass1 * num;
        int disease_count = (int) ((double) diseaseCount * (double) num);
        if ((double) mass2 > 1.0 / 1000.0)
        {
          mass1 -= mass2;
          diseaseCount -= disease_count;
          Element elementByHash = ElementLoader.FindElementByHash(hash);
          if (elementByHash.IsSolid)
          {
            GameObject gameObject = elementByHash.substance.SpawnResource(TransformExtensions.GetPosition(cmp.transform), mass2, component.Temperature, component.DiseaseIdx, disease_count, true, manual_activation: true);
            elementByHash.substance.ActivateSubstanceGameObject(gameObject, component.DiseaseIdx, disease_count);
          }
          else
            SimMessages.AddRemoveSubstance(cell, elementByHash.id, CellEventLogger.Instance.OreMelted, mass2, component.Temperature, component.DiseaseIdx, disease_count);
        }
      }
      SimMessages.AddRemoveSubstance(cell, new_element, CellEventLogger.Instance.OreMelted, mass1, component.Temperature, component.DiseaseIdx, diseaseCount);
    }
    ((KMonoBehaviour) cmp).OnCleanUp();
    Util.KDestroyGameObject(((Component) cmp).gameObject);
  }

  protected virtual void OnPrefabInit()
  {
    PrimaryElement component = ((Component) this).GetComponent<PrimaryElement>();
    component.getTemperatureCallback = new PrimaryElement.GetTemperatureCallback(SimTemperatureTransfer.OnGetTemperature);
    component.setTemperatureCallback = new PrimaryElement.SetTemperatureCallback(SimTemperatureTransfer.OnSetTemperature);
    component.onDataChanged += new Action<PrimaryElement>(this.OnDataChanged);
  }

  protected virtual void OnSpawn()
  {
    base.OnSpawn();
    PrimaryElement component = ((Component) this).GetComponent<PrimaryElement>();
    Element element = component.Element;
    Singleton<CellChangeMonitor>.Instance.RegisterCellChangedHandler(this.transform, new System.Action(this.OnCellChanged), "SimTemperatureTransfer.OnSpawn");
    if (!Grid.IsValidCell(Grid.PosToCell((KMonoBehaviour) this)) || component.Element.HasTag(GameTags.Special) || (double) element.specificHeatCapacity == 0.0)
      ((Behaviour) this).enabled = false;
    this.SimRegister();
  }

  protected virtual void OnCmpEnable()
  {
    base.OnCmpEnable();
    this.SimRegister();
    if (!Sim.IsValidHandle(this.simHandle))
      return;
    PrimaryElement component = ((Component) this).GetComponent<PrimaryElement>();
    SimTemperatureTransfer.OnSetTemperature(component, component.Temperature);
  }

  protected virtual void OnCmpDisable()
  {
    if (Sim.IsValidHandle(this.simHandle))
    {
      PrimaryElement component = ((Component) this).GetComponent<PrimaryElement>();
      float temperature = component.Temperature;
      component.InternalTemperature = component.Temperature;
      SimMessages.SetElementChunkData(this.simHandle, temperature, 0.0f);
    }
    base.OnCmpDisable();
  }

  private void OnCellChanged()
  {
    int cell = Grid.PosToCell((KMonoBehaviour) this);
    if (!Grid.IsValidCell(cell))
    {
      ((Behaviour) this).enabled = false;
    }
    else
    {
      this.SimRegister();
      if (!Sim.IsValidHandle(this.simHandle))
        return;
      SimMessages.MoveElementChunk(this.simHandle, cell);
    }
  }

  protected virtual void OnCleanUp()
  {
    Singleton<CellChangeMonitor>.Instance.UnregisterCellChangedHandler(this.transform, new System.Action(this.OnCellChanged));
    this.SimUnregister();
    this.OnForcedCleanUp();
  }

  public void ModifyEnergy(float delta_kilojoules)
  {
    if (Sim.IsValidHandle(this.simHandle))
      SimMessages.ModifyElementChunkEnergy(this.simHandle, delta_kilojoules);
    else
      this.pendingEnergyModifications += delta_kilojoules;
  }

  private static unsafe float OnGetTemperature(PrimaryElement primary_element)
  {
    SimTemperatureTransfer component = ((Component) primary_element).GetComponent<SimTemperatureTransfer>();
    float temperature;
    if (Sim.IsValidHandle(component.simHandle))
    {
      int handleIndex = Sim.GetHandleIndex(component.simHandle);
      temperature = Game.Instance.simData.elementChunks[handleIndex].temperature;
      component.deltaKJ = Game.Instance.simData.elementChunks[handleIndex].deltaKJ;
    }
    else
      temperature = primary_element.InternalTemperature;
    return temperature;
  }

  private static unsafe void OnSetTemperature(PrimaryElement primary_element, float temperature)
  {
    if ((double) temperature <= 0.0)
    {
      KCrashReporter.Assert(false, "STT.OnSetTemperature - Tried to set <= 0 degree temperature");
      temperature = 293f;
    }
    SimTemperatureTransfer component = ((Component) primary_element).GetComponent<SimTemperatureTransfer>();
    if (Sim.IsValidHandle(component.simHandle))
    {
      float mass = primary_element.Mass;
      float heat_capacity = (double) mass >= 0.0099999997764825821 ? mass * primary_element.Element.specificHeatCapacity : 0.0f;
      SimMessages.SetElementChunkData(component.simHandle, temperature, heat_capacity);
      Game.Instance.simData.elementChunks[Sim.GetHandleIndex(component.simHandle)].temperature = temperature;
    }
    else
      primary_element.InternalTemperature = temperature;
  }

  private void OnDataChanged(PrimaryElement primary_element)
  {
    if (!Sim.IsValidHandle(this.simHandle))
      return;
    float heat_capacity = (double) primary_element.Mass >= 0.0099999997764825821 ? primary_element.Mass * primary_element.Element.specificHeatCapacity : 0.0f;
    SimMessages.SetElementChunkData(this.simHandle, primary_element.Temperature, heat_capacity);
  }

  protected void SimRegister()
  {
    if (!this.isSpawned || this.simHandle != -1 || !((Behaviour) this).enabled)
      return;
    PrimaryElement component = ((Component) this).GetComponent<PrimaryElement>();
    if ((double) component.Mass <= 0.0 || component.Element.IsTemperatureInsulated)
      return;
    int cell = Grid.PosToCell(TransformExtensions.GetPosition(this.transform));
    this.simHandle = -2;
    HandleVector<Game.ComplexCallbackInfo<int>>.Handle handle = Game.Instance.simComponentCallbackManager.Add(new Action<int, object>(SimTemperatureTransfer.OnSimRegisteredCallback), (object) this, "SimTemperatureTransfer.SimRegister");
    float num = component.InternalTemperature;
    if ((double) num <= 0.0)
    {
      component.InternalTemperature = 293f;
      num = 293f;
    }
    int elementId = (int) component.ElementID;
    double mass = (double) component.Mass;
    double temperature = (double) num;
    double surfaceArea = (double) this.surfaceArea;
    double thickness = (double) this.thickness;
    double groundTransferScale = (double) this.groundTransferScale;
    int index = handle.index;
    SimMessages.AddElementChunk(cell, (SimHashes) elementId, (float) mass, (float) temperature, (float) surfaceArea, (float) thickness, (float) groundTransferScale, index);
  }

  protected unsafe void SimUnregister()
  {
    if (this.simHandle == -1 || KMonoBehaviour.isLoadingScene)
      return;
    PrimaryElement component = ((Component) this).GetComponent<PrimaryElement>();
    if (Sim.IsValidHandle(this.simHandle))
    {
      int handleIndex = Sim.GetHandleIndex(this.simHandle);
      component.InternalTemperature = Game.Instance.simData.elementChunks[handleIndex].temperature;
      SimMessages.RemoveElementChunk(this.simHandle, -1);
      SimTemperatureTransfer.handleInstanceMap.Remove(this.simHandle);
    }
    this.simHandle = -1;
  }

  private static void OnSimRegisteredCallback(int handle, object data) => ((SimTemperatureTransfer) data).OnSimRegistered(handle);

  private unsafe void OnSimRegistered(int handle)
  {
    if (Object.op_Inequality((Object) this, (Object) null) && this.simHandle == -2)
    {
      this.simHandle = handle;
      if ((double) Game.Instance.simData.elementChunks[Sim.GetHandleIndex(handle)].temperature <= 0.0)
        KCrashReporter.Assert(false, "Bad temperature");
      SimTemperatureTransfer.handleInstanceMap[this.simHandle] = this;
      if ((double) this.pendingEnergyModifications > 0.0)
      {
        this.ModifyEnergy(this.pendingEnergyModifications);
        this.pendingEnergyModifications = 0.0f;
      }
      if (this.onSimRegistered != null)
        this.onSimRegistered(this);
      if (((Behaviour) this).enabled)
        return;
      base.OnCmpDisable();
    }
    else
      SimMessages.RemoveElementChunk(handle, -1);
  }
}
