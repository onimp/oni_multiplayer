// Decompiled with JetBrains decompiler
// Type: SimMessages
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Database;
using STRINGS;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

public static class SimMessages
{
  public const int InvalidCallback = -1;
  public const float STATE_TRANSITION_TEMPERATURE_BUFER = 3f;

  public static unsafe void AddElementConsumer(
    int gameCell,
    ElementConsumer.Configuration configuration,
    SimHashes element,
    byte radius,
    int cb_handle)
  {
    Debug.Assert(Grid.IsValidCell(gameCell));
    if (!Grid.IsValidCell(gameCell))
      return;
    ushort elementIndex = ElementLoader.GetElementIndex(element);
    SimMessages.AddElementConsumerMessage* msg = stackalloc SimMessages.AddElementConsumerMessage[1];
    msg->cellIdx = gameCell;
    msg->configuration = (byte) configuration;
    msg->elementIdx = elementIndex;
    msg->radius = radius;
    msg->callbackIdx = cb_handle;
    Sim.SIM_HandleMessage(2024405073, sizeof (SimMessages.AddElementConsumerMessage), (byte*) msg);
  }

  public static unsafe void SetElementConsumerData(int sim_handle, int cell, float consumptionRate)
  {
    if (!Sim.IsValidHandle(sim_handle))
      return;
    SimMessages.SetElementConsumerDataMessage* msg = stackalloc SimMessages.SetElementConsumerDataMessage[1];
    msg->handle = sim_handle;
    msg->cell = cell;
    msg->consumptionRate = consumptionRate;
    Sim.SIM_HandleMessage(1575539738, sizeof (SimMessages.SetElementConsumerDataMessage), (byte*) msg);
  }

  public static unsafe void RemoveElementConsumer(int cb_handle, int sim_handle)
  {
    if (!Sim.IsValidHandle(sim_handle))
    {
      Debug.Assert(false, (object) "Invalid handle");
    }
    else
    {
      SimMessages.RemoveElementConsumerMessage* msg = stackalloc SimMessages.RemoveElementConsumerMessage[1];
      msg->callbackIdx = cb_handle;
      msg->handle = sim_handle;
      Sim.SIM_HandleMessage(894417742, sizeof (SimMessages.RemoveElementConsumerMessage), (byte*) msg);
    }
  }

  public static unsafe void AddElementEmitter(
    float max_pressure,
    int on_registered,
    int on_blocked = -1,
    int on_unblocked = -1)
  {
    SimMessages.AddElementEmitterMessage* msg = stackalloc SimMessages.AddElementEmitterMessage[1];
    msg->maxPressure = max_pressure;
    msg->callbackIdx = on_registered;
    msg->onBlockedCB = on_blocked;
    msg->onUnblockedCB = on_unblocked;
    Sim.SIM_HandleMessage(-505471181, sizeof (SimMessages.AddElementEmitterMessage), (byte*) msg);
  }

  public static unsafe void ModifyElementEmitter(
    int sim_handle,
    int game_cell,
    int max_depth,
    SimHashes element,
    float emit_interval,
    float emit_mass,
    float emit_temperature,
    float max_pressure,
    byte disease_idx,
    int disease_count)
  {
    Debug.Assert(Grid.IsValidCell(game_cell));
    if (!Grid.IsValidCell(game_cell))
      return;
    ushort elementIndex = ElementLoader.GetElementIndex(element);
    SimMessages.ModifyElementEmitterMessage* msg = stackalloc SimMessages.ModifyElementEmitterMessage[1];
    msg->handle = sim_handle;
    msg->cellIdx = game_cell;
    msg->emitInterval = emit_interval;
    msg->emitMass = emit_mass;
    msg->emitTemperature = emit_temperature;
    msg->maxPressure = max_pressure;
    msg->elementIdx = elementIndex;
    msg->maxDepth = (byte) max_depth;
    msg->diseaseIdx = disease_idx;
    msg->diseaseCount = disease_count;
    Sim.SIM_HandleMessage(403589164, sizeof (SimMessages.ModifyElementEmitterMessage), (byte*) msg);
  }

  public static unsafe void RemoveElementEmitter(int cb_handle, int sim_handle)
  {
    if (!Sim.IsValidHandle(sim_handle))
    {
      Debug.Assert(false, (object) "Invalid handle");
    }
    else
    {
      SimMessages.RemoveElementEmitterMessage* msg = stackalloc SimMessages.RemoveElementEmitterMessage[1];
      msg->callbackIdx = cb_handle;
      msg->handle = sim_handle;
      Sim.SIM_HandleMessage(-1524118282, sizeof (SimMessages.RemoveElementEmitterMessage), (byte*) msg);
    }
  }

  public static unsafe void AddRadiationEmitter(
    int on_registered,
    int game_cell,
    short emitRadiusX,
    short emitRadiusY,
    float emitRads,
    float emitRate,
    float emitSpeed,
    float emitDirection,
    float emitAngle,
    RadiationEmitter.RadiationEmitterType emitType)
  {
    SimMessages.AddRadiationEmitterMessage* msg = stackalloc SimMessages.AddRadiationEmitterMessage[1];
    msg->callbackIdx = on_registered;
    msg->cell = game_cell;
    msg->emitRadiusX = emitRadiusX;
    msg->emitRadiusY = emitRadiusY;
    msg->emitRads = emitRads;
    msg->emitRate = emitRate;
    msg->emitSpeed = emitSpeed;
    msg->emitDirection = emitDirection;
    msg->emitAngle = emitAngle;
    msg->emitType = (int) emitType;
    Sim.SIM_HandleMessage(-1505895314, sizeof (SimMessages.AddRadiationEmitterMessage), (byte*) msg);
  }

  public static unsafe void ModifyRadiationEmitter(
    int sim_handle,
    int game_cell,
    short emitRadiusX,
    short emitRadiusY,
    float emitRads,
    float emitRate,
    float emitSpeed,
    float emitDirection,
    float emitAngle,
    RadiationEmitter.RadiationEmitterType emitType)
  {
    if (!Grid.IsValidCell(game_cell))
      return;
    SimMessages.ModifyRadiationEmitterMessage* msg = stackalloc SimMessages.ModifyRadiationEmitterMessage[1];
    msg->handle = sim_handle;
    msg->cell = game_cell;
    msg->callbackIdx = -1;
    msg->emitRadiusX = emitRadiusX;
    msg->emitRadiusY = emitRadiusY;
    msg->emitRads = emitRads;
    msg->emitRate = emitRate;
    msg->emitSpeed = emitSpeed;
    msg->emitDirection = emitDirection;
    msg->emitAngle = emitAngle;
    msg->emitType = (int) emitType;
    Sim.SIM_HandleMessage(-503965465, sizeof (SimMessages.ModifyRadiationEmitterMessage), (byte*) msg);
  }

  public static unsafe void RemoveRadiationEmitter(int cb_handle, int sim_handle)
  {
    if (!Sim.IsValidHandle(sim_handle))
    {
      Debug.Assert(false, (object) "Invalid handle");
    }
    else
    {
      SimMessages.RemoveRadiationEmitterMessage* msg = stackalloc SimMessages.RemoveRadiationEmitterMessage[1];
      msg->callbackIdx = cb_handle;
      msg->handle = sim_handle;
      Sim.SIM_HandleMessage(-704259919, sizeof (SimMessages.RemoveRadiationEmitterMessage), (byte*) msg);
    }
  }

  public static unsafe void AddElementChunk(
    int gameCell,
    SimHashes element,
    float mass,
    float temperature,
    float surface_area,
    float thickness,
    float ground_transfer_scale,
    int cb_handle)
  {
    Debug.Assert(Grid.IsValidCell(gameCell));
    if (!Grid.IsValidCell(gameCell) || (double) mass * (double) temperature <= 0.0)
      return;
    ushort elementIndex = ElementLoader.GetElementIndex(element);
    SimMessages.AddElementChunkMessage* msg = stackalloc SimMessages.AddElementChunkMessage[1];
    msg->gameCell = gameCell;
    msg->callbackIdx = cb_handle;
    msg->mass = mass;
    msg->temperature = temperature;
    msg->surfaceArea = surface_area;
    msg->thickness = thickness;
    msg->groundTransferScale = ground_transfer_scale;
    msg->elementIdx = elementIndex;
    Sim.SIM_HandleMessage(1445724082, sizeof (SimMessages.AddElementChunkMessage), (byte*) msg);
  }

  public static unsafe void RemoveElementChunk(int sim_handle, int cb_handle)
  {
    if (!Sim.IsValidHandle(sim_handle))
    {
      Debug.Assert(false, (object) "Invalid handle");
    }
    else
    {
      SimMessages.RemoveElementChunkMessage* msg = stackalloc SimMessages.RemoveElementChunkMessage[1];
      msg->callbackIdx = cb_handle;
      msg->handle = sim_handle;
      Sim.SIM_HandleMessage(-912908555, sizeof (SimMessages.RemoveElementChunkMessage), (byte*) msg);
    }
  }

  public static unsafe void SetElementChunkData(
    int sim_handle,
    float temperature,
    float heat_capacity)
  {
    if (!Sim.IsValidHandle(sim_handle))
      return;
    SimMessages.SetElementChunkDataMessage* msg = stackalloc SimMessages.SetElementChunkDataMessage[1];
    msg->handle = sim_handle;
    msg->temperature = temperature;
    msg->heatCapacity = heat_capacity;
    Sim.SIM_HandleMessage(-435115907, sizeof (SimMessages.SetElementChunkDataMessage), (byte*) msg);
  }

  public static unsafe void MoveElementChunk(int sim_handle, int cell)
  {
    if (!Sim.IsValidHandle(sim_handle))
    {
      Debug.Assert(false, (object) "Invalid handle");
    }
    else
    {
      SimMessages.MoveElementChunkMessage* msg = stackalloc SimMessages.MoveElementChunkMessage[1];
      msg->handle = sim_handle;
      msg->gameCell = cell;
      Sim.SIM_HandleMessage(-374911358, sizeof (SimMessages.MoveElementChunkMessage), (byte*) msg);
    }
  }

  public static unsafe void ModifyElementChunkEnergy(int sim_handle, float delta_kj)
  {
    if (!Sim.IsValidHandle(sim_handle))
    {
      Debug.Assert(false, (object) "Invalid handle");
    }
    else
    {
      SimMessages.ModifyElementChunkEnergyMessage* msg = stackalloc SimMessages.ModifyElementChunkEnergyMessage[1];
      msg->handle = sim_handle;
      msg->deltaKJ = delta_kj;
      Sim.SIM_HandleMessage(1020555667, sizeof (SimMessages.ModifyElementChunkEnergyMessage), (byte*) msg);
    }
  }

  public static unsafe void ModifyElementChunkTemperatureAdjuster(
    int sim_handle,
    float temperature,
    float heat_capacity,
    float thermal_conductivity)
  {
    if (!Sim.IsValidHandle(sim_handle))
    {
      Debug.Assert(false, (object) "Invalid handle");
    }
    else
    {
      SimMessages.ModifyElementChunkAdjusterMessage* msg = stackalloc SimMessages.ModifyElementChunkAdjusterMessage[1];
      msg->handle = sim_handle;
      msg->temperature = temperature;
      msg->heatCapacity = heat_capacity;
      msg->thermalConductivity = thermal_conductivity;
      Sim.SIM_HandleMessage(-1387601379, sizeof (SimMessages.ModifyElementChunkAdjusterMessage), (byte*) msg);
    }
  }

  public static unsafe void AddBuildingHeatExchange(
    Extents extents,
    float mass,
    float temperature,
    float thermal_conductivity,
    float operating_kw,
    ushort elem_idx,
    int callbackIdx = -1)
  {
    if (!Grid.IsValidCell(Grid.XYToCell(extents.x, extents.y)))
      return;
    int cell = Grid.XYToCell(extents.x + extents.width, extents.y + extents.height);
    if (!Grid.IsValidCell(cell))
      Debug.LogErrorFormat("Invalid Cell [{0}] Extents [{1},{2}] [{3},{4}]", new object[5]
      {
        (object) cell,
        (object) extents.x,
        (object) extents.y,
        (object) extents.width,
        (object) extents.height
      });
    if (!Grid.IsValidCell(cell))
      return;
    SimMessages.AddBuildingHeatExchangeMessage* msg = stackalloc SimMessages.AddBuildingHeatExchangeMessage[1];
    msg->callbackIdx = callbackIdx;
    msg->elemIdx = elem_idx;
    msg->mass = mass;
    msg->temperature = temperature;
    msg->thermalConductivity = thermal_conductivity;
    msg->overheatTemperature = float.MaxValue;
    msg->operatingKilowatts = operating_kw;
    msg->minX = extents.x;
    msg->minY = extents.y;
    msg->maxX = extents.x + extents.width;
    msg->maxY = extents.y + extents.height;
    Sim.SIM_HandleMessage(1739021608, sizeof (SimMessages.AddBuildingHeatExchangeMessage), (byte*) msg);
  }

  public static unsafe void ModifyBuildingHeatExchange(
    int sim_handle,
    Extents extents,
    float mass,
    float temperature,
    float thermal_conductivity,
    float overheat_temperature,
    float operating_kw,
    ushort element_idx)
  {
    int cell1 = Grid.XYToCell(extents.x, extents.y);
    Debug.Assert(Grid.IsValidCell(cell1));
    if (!Grid.IsValidCell(cell1))
      return;
    int cell2 = Grid.XYToCell(extents.x + extents.width, extents.y + extents.height);
    Debug.Assert(Grid.IsValidCell(cell2));
    if (!Grid.IsValidCell(cell2))
      return;
    SimMessages.ModifyBuildingHeatExchangeMessage* msg = stackalloc SimMessages.ModifyBuildingHeatExchangeMessage[1];
    msg->callbackIdx = sim_handle;
    msg->elemIdx = element_idx;
    msg->mass = mass;
    msg->temperature = temperature;
    msg->thermalConductivity = thermal_conductivity;
    msg->overheatTemperature = overheat_temperature;
    msg->operatingKilowatts = operating_kw;
    msg->minX = extents.x;
    msg->minY = extents.y;
    msg->maxX = extents.x + extents.width;
    msg->maxY = extents.y + extents.height;
    Sim.SIM_HandleMessage(1818001569, sizeof (SimMessages.ModifyBuildingHeatExchangeMessage), (byte*) msg);
  }

  public static unsafe void RemoveBuildingHeatExchange(int sim_handle, int callbackIdx = -1)
  {
    SimMessages.RemoveBuildingHeatExchangeMessage* msg = stackalloc SimMessages.RemoveBuildingHeatExchangeMessage[1];
    Debug.Assert(Sim.IsValidHandle(sim_handle));
    msg->handle = sim_handle;
    msg->callbackIdx = callbackIdx;
    Sim.SIM_HandleMessage(-456116629, sizeof (SimMessages.RemoveBuildingHeatExchangeMessage), (byte*) msg);
  }

  public static unsafe void ModifyBuildingEnergy(
    int sim_handle,
    float delta_kj,
    float min_temperature,
    float max_temperature)
  {
    SimMessages.ModifyBuildingEnergyMessage* msg = stackalloc SimMessages.ModifyBuildingEnergyMessage[1];
    Debug.Assert(Sim.IsValidHandle(sim_handle));
    msg->handle = sim_handle;
    msg->deltaKJ = delta_kj;
    msg->minTemperature = min_temperature;
    msg->maxTemperature = max_temperature;
    Sim.SIM_HandleMessage(-1348791658, sizeof (SimMessages.ModifyBuildingEnergyMessage), (byte*) msg);
  }

  public static unsafe void RegisterBuildingToBuildingHeatExchange(
    int structureTemperatureHandler,
    int callbackIdx = -1)
  {
    SimMessages.RegisterBuildingToBuildingHeatExchangeMessage* msg = stackalloc SimMessages.RegisterBuildingToBuildingHeatExchangeMessage[1];
    msg->structureTemperatureHandler = structureTemperatureHandler;
    msg->callbackIdx = callbackIdx;
    Sim.SIM_HandleMessage(-1338718217, sizeof (SimMessages.RegisterBuildingToBuildingHeatExchangeMessage), (byte*) msg);
  }

  public static unsafe void AddBuildingToBuildingHeatExchange(
    int selfHandler,
    int buildingInContact,
    int cellsInContact)
  {
    SimMessages.AddBuildingToBuildingHeatExchangeMessage* msg = stackalloc SimMessages.AddBuildingToBuildingHeatExchangeMessage[1];
    msg->selfHandler = selfHandler;
    msg->buildingInContactHandle = buildingInContact;
    msg->cellsInContact = cellsInContact;
    Sim.SIM_HandleMessage(-1586724321, sizeof (SimMessages.AddBuildingToBuildingHeatExchangeMessage), (byte*) msg);
  }

  public static unsafe void RemoveBuildingInContactFromBuildingToBuildingHeatExchange(
    int selfHandler,
    int buildingToRemove)
  {
    SimMessages.RemoveBuildingInContactFromBuildingToBuildingHeatExchangeMessage* msg = stackalloc SimMessages.RemoveBuildingInContactFromBuildingToBuildingHeatExchangeMessage[1];
    msg->selfHandler = selfHandler;
    msg->buildingNoLongerInContactHandler = buildingToRemove;
    Sim.SIM_HandleMessage(-1993857213, sizeof (SimMessages.RemoveBuildingInContactFromBuildingToBuildingHeatExchangeMessage), (byte*) msg);
  }

  public static unsafe void RemoveBuildingToBuildingHeatExchange(int selfHandler, int callback = -1)
  {
    SimMessages.RemoveBuildingToBuildingHeatExchangeMessage* msg = stackalloc SimMessages.RemoveBuildingToBuildingHeatExchangeMessage[1];
    msg->callbackIdx = callback;
    msg->selfHandler = selfHandler;
    Sim.SIM_HandleMessage(697100730, sizeof (SimMessages.RemoveBuildingToBuildingHeatExchangeMessage), (byte*) msg);
  }

  public static unsafe void AddDiseaseEmitter(int callbackIdx)
  {
    SimMessages.AddDiseaseEmitterMessage* msg = stackalloc SimMessages.AddDiseaseEmitterMessage[1];
    msg->callbackIdx = callbackIdx;
    Sim.SIM_HandleMessage(1486783027, sizeof (SimMessages.AddDiseaseEmitterMessage), (byte*) msg);
  }

  public static unsafe void ModifyDiseaseEmitter(
    int sim_handle,
    int cell,
    byte range,
    byte disease_idx,
    float emit_interval,
    int emit_count)
  {
    Debug.Assert(Sim.IsValidHandle(sim_handle));
    SimMessages.ModifyDiseaseEmitterMessage* msg = stackalloc SimMessages.ModifyDiseaseEmitterMessage[1];
    msg->handle = sim_handle;
    msg->gameCell = cell;
    msg->maxDepth = range;
    msg->diseaseIdx = disease_idx;
    msg->emitInterval = emit_interval;
    msg->emitCount = emit_count;
    Sim.SIM_HandleMessage(-1899123924, sizeof (SimMessages.ModifyDiseaseEmitterMessage), (byte*) msg);
  }

  public static unsafe void RemoveDiseaseEmitter(int cb_handle, int sim_handle)
  {
    Debug.Assert(Sim.IsValidHandle(sim_handle));
    SimMessages.RemoveDiseaseEmitterMessage* msg = stackalloc SimMessages.RemoveDiseaseEmitterMessage[1];
    msg->handle = sim_handle;
    msg->callbackIdx = cb_handle;
    Sim.SIM_HandleMessage(468135926, sizeof (SimMessages.RemoveDiseaseEmitterMessage), (byte*) msg);
  }

  public static unsafe void SetSavedOptionValue(SimMessages.SimSavedOptions option, int zero_or_one)
  {
    SimMessages.SetSavedOptionsMessage* msg = stackalloc SimMessages.SetSavedOptionsMessage[1];
    if (zero_or_one == 0)
    {
      ref byte local = ref msg->clearBits;
      local = (byte) ((SimMessages.SimSavedOptions) local | option);
      msg->setBits = (byte) 0;
    }
    else
    {
      msg->clearBits = (byte) 0;
      ref byte local = ref msg->setBits;
      local = (byte) ((SimMessages.SimSavedOptions) local | option);
    }
    Sim.SIM_HandleMessage(1154135737, sizeof (SimMessages.SetSavedOptionsMessage), (byte*) msg);
  }

  private static void WriteKleiString(this BinaryWriter writer, string str)
  {
    byte[] bytes = Encoding.UTF8.GetBytes(str);
    writer.Write(bytes.Length);
    if (bytes.Length == 0)
      return;
    writer.Write(bytes);
  }

  public static unsafe void CreateSimElementsTable(List<Element> elements)
  {
    MemoryStream output = new MemoryStream(Marshal.SizeOf(typeof (int)) + Marshal.SizeOf(typeof (Sim.Element)) * elements.Count);
    BinaryWriter writer = new BinaryWriter((Stream) output);
    Debug.Assert(elements.Count < (int) ushort.MaxValue, (object) "SimDLL internals assume there are fewer than 65535 elements");
    writer.Write(elements.Count);
    for (int index = 0; index < elements.Count; ++index)
      new Sim.Element(elements[index], elements).Write(writer);
    for (int index = 0; index < elements.Count; ++index)
      writer.WriteKleiString(UI.StripLinkFormatting(elements[index].name));
    byte[] buffer = output.GetBuffer();
    fixed (byte* msg = buffer)
      Sim.SIM_HandleMessage(1108437482, buffer.Length, msg);
  }

  public static unsafe void CreateDiseaseTable(Diseases diseases)
  {
    MemoryStream output = new MemoryStream(1024);
    BinaryWriter writer = new BinaryWriter((Stream) output);
    writer.Write(((ResourceSet) diseases).Count);
    List<Element> elements = ElementLoader.elements;
    writer.Write(elements.Count);
    for (int index1 = 0; index1 < ((ResourceSet) diseases).Count; ++index1)
    {
      Klei.AI.Disease disease = diseases[index1];
      writer.WriteKleiString(UI.StripLinkFormatting(disease.Name));
      writer.Write(disease.id.GetHashCode());
      writer.Write(disease.strength);
      disease.temperatureRange.Write(writer);
      disease.temperatureHalfLives.Write(writer);
      disease.pressureRange.Write(writer);
      disease.pressureHalfLives.Write(writer);
      writer.Write(disease.radiationKillRate);
      for (int index2 = 0; index2 < elements.Count; ++index2)
        disease.elemGrowthInfo[index2].Write(writer);
    }
    fixed (byte* msg = output.GetBuffer())
      Sim.SIM_HandleMessage(825301935, (int) output.Length, msg);
  }

  public static unsafe void DefineWorldOffsets(List<SimMessages.WorldOffsetData> worldOffsets)
  {
    MemoryStream output = new MemoryStream(1024);
    BinaryWriter binaryWriter = new BinaryWriter((Stream) output);
    binaryWriter.Write(worldOffsets.Count);
    foreach (SimMessages.WorldOffsetData worldOffset in worldOffsets)
    {
      binaryWriter.Write(worldOffset.worldOffsetX);
      binaryWriter.Write(worldOffset.worldOffsetY);
      binaryWriter.Write(worldOffset.worldSizeX);
      binaryWriter.Write(worldOffset.worldSizeY);
    }
    fixed (byte* msg = output.GetBuffer())
      Sim.SIM_HandleMessage(-895846551, (int) output.Length, msg);
  }

  public static void SimDataInitializeFromCells(
    int width,
    int height,
    Sim.Cell[] cells,
    float[] bgTemp,
    Sim.DiseaseCell[] dc,
    bool headless)
  {
    MemoryStream output = new MemoryStream(Marshal.SizeOf(typeof (int)) + Marshal.SizeOf(typeof (int)) + Marshal.SizeOf(typeof (Sim.Cell)) * width * height + Marshal.SizeOf(typeof (float)) * width * height + Marshal.SizeOf(typeof (Sim.DiseaseCell)) * width * height);
    BinaryWriter writer = new BinaryWriter((Stream) output);
    writer.Write(width);
    writer.Write(height);
    bool flag = Sim.IsRadiationEnabled();
    writer.Write(flag);
    writer.Write(headless);
    int num = width * height;
    for (int index = 0; index < num; ++index)
      cells[index].Write(writer);
    for (int index = 0; index < num; ++index)
      writer.Write(bgTemp[index]);
    for (int index = 0; index < num; ++index)
      dc[index].Write(writer);
    byte[] buffer = output.GetBuffer();
    Sim.HandleMessage(SimMessageHashes.SimData_InitializeFromCells, buffer.Length, buffer);
  }

  public static void SimDataResizeGridAndInitializeVacuumCells(
    Vector2I grid_size,
    int width,
    int height,
    int x_offset,
    int y_offset)
  {
    MemoryStream output = new MemoryStream(Marshal.SizeOf(typeof (int)) + Marshal.SizeOf(typeof (int)));
    BinaryWriter binaryWriter = new BinaryWriter((Stream) output);
    binaryWriter.Write(grid_size.x);
    binaryWriter.Write(grid_size.y);
    binaryWriter.Write(width);
    binaryWriter.Write(height);
    binaryWriter.Write(x_offset);
    binaryWriter.Write(y_offset);
    byte[] buffer = output.GetBuffer();
    Sim.HandleMessage(SimMessageHashes.SimData_ResizeAndInitializeVacuumCells, buffer.Length, buffer);
  }

  public static void SimDataFreeCells(int width, int height, int x_offset, int y_offset)
  {
    MemoryStream output = new MemoryStream(Marshal.SizeOf(typeof (int)) + Marshal.SizeOf(typeof (int)));
    BinaryWriter binaryWriter = new BinaryWriter((Stream) output);
    binaryWriter.Write(width);
    binaryWriter.Write(height);
    binaryWriter.Write(x_offset);
    binaryWriter.Write(y_offset);
    byte[] buffer = output.GetBuffer();
    Sim.HandleMessage(SimMessageHashes.SimData_FreeCells, buffer.Length, buffer);
  }

  public static unsafe void Dig(int gameCell, int callbackIdx = -1, bool skipEvent = false)
  {
    if (!Grid.IsValidCell(gameCell))
      return;
    SimMessages.DigMessage* msg = stackalloc SimMessages.DigMessage[1];
    msg->cellIdx = gameCell;
    msg->callbackIdx = callbackIdx;
    msg->skipEvent = skipEvent;
    Sim.SIM_HandleMessage(833038498, sizeof (SimMessages.DigMessage), (byte*) msg);
  }

  public static unsafe void SetInsulation(int gameCell, float value)
  {
    if (!Grid.IsValidCell(gameCell))
      return;
    SimMessages.SetCellFloatValueMessage* msg = stackalloc SimMessages.SetCellFloatValueMessage[1];
    msg->cellIdx = gameCell;
    msg->value = value;
    Sim.SIM_HandleMessage(-898773121, sizeof (SimMessages.SetCellFloatValueMessage), (byte*) msg);
  }

  public static unsafe void SetStrength(int gameCell, int weight, float strengthMultiplier)
  {
    if (!Grid.IsValidCell(gameCell))
      return;
    SimMessages.SetCellFloatValueMessage* msg = stackalloc SimMessages.SetCellFloatValueMessage[1];
    msg->cellIdx = gameCell;
    int num1 = (int) ((double) strengthMultiplier * 4.0) & (int) sbyte.MaxValue;
    int num2 = (weight & 1) << 7 | num1;
    msg->value = (float) (byte) num2;
    Sim.SIM_HandleMessage(1593243982, sizeof (SimMessages.SetCellFloatValueMessage), (byte*) msg);
  }

  public static unsafe void SetCellProperties(int gameCell, byte properties)
  {
    if (!Grid.IsValidCell(gameCell))
      return;
    SimMessages.CellPropertiesMessage* msg = stackalloc SimMessages.CellPropertiesMessage[1];
    msg->cellIdx = gameCell;
    msg->properties = properties;
    msg->set = (byte) 1;
    Sim.SIM_HandleMessage(-469311643, sizeof (SimMessages.CellPropertiesMessage), (byte*) msg);
  }

  public static unsafe void ClearCellProperties(int gameCell, byte properties)
  {
    if (!Grid.IsValidCell(gameCell))
      return;
    SimMessages.CellPropertiesMessage* msg = stackalloc SimMessages.CellPropertiesMessage[1];
    msg->cellIdx = gameCell;
    msg->properties = properties;
    msg->set = (byte) 0;
    Sim.SIM_HandleMessage(-469311643, sizeof (SimMessages.CellPropertiesMessage), (byte*) msg);
  }

  public static unsafe void ModifyCell(
    int gameCell,
    ushort elementIdx,
    float temperature,
    float mass,
    byte disease_idx,
    int disease_count,
    SimMessages.ReplaceType replace_type = SimMessages.ReplaceType.None,
    bool do_vertical_solid_displacement = false,
    int callbackIdx = -1)
  {
    if (!Grid.IsValidCell(gameCell))
      return;
    Element element = ElementLoader.elements[(int) elementIdx];
    if ((double) element.maxMass == 0.0 && (double) mass > (double) element.maxMass)
    {
      Debug.LogWarningFormat("Invalid cell modification (mass greater than element maximum): Cell={0}, EIdx={1}, T={2}, M={3}, {4} max mass = {5}", new object[6]
      {
        (object) gameCell,
        (object) elementIdx,
        (object) temperature,
        (object) mass,
        (object) element.id,
        (object) element.maxMass
      });
      mass = element.maxMass;
    }
    if ((double) temperature < 0.0 || (double) temperature > 10000.0)
    {
      Debug.LogWarningFormat("Invalid cell modification (temp out of bounds): Cell={0}, EIdx={1}, T={2}, M={3}, {4} default temp = {5}", new object[6]
      {
        (object) gameCell,
        (object) elementIdx,
        (object) temperature,
        (object) mass,
        (object) element.id,
        (object) element.defaultValues.temperature
      });
      temperature = element.defaultValues.temperature;
    }
    if ((double) temperature == 0.0 && (double) mass > 0.0)
    {
      Debug.LogWarningFormat("Invalid cell modification (zero temp with non-zero mass): Cell={0}, EIdx={1}, T={2}, M={3}, {4} default temp = {5}", new object[6]
      {
        (object) gameCell,
        (object) elementIdx,
        (object) temperature,
        (object) mass,
        (object) element.id,
        (object) element.defaultValues.temperature
      });
      temperature = element.defaultValues.temperature;
    }
    SimMessages.ModifyCellMessage* msg = stackalloc SimMessages.ModifyCellMessage[1];
    msg->cellIdx = gameCell;
    msg->callbackIdx = callbackIdx;
    msg->temperature = temperature;
    msg->mass = mass;
    msg->elementIdx = elementIdx;
    msg->replaceType = (byte) replace_type;
    msg->diseaseIdx = disease_idx;
    msg->diseaseCount = disease_count;
    msg->addSubType = do_vertical_solid_displacement ? (byte) 0 : (byte) 1;
    Sim.SIM_HandleMessage(-1252920804, sizeof (SimMessages.ModifyCellMessage), (byte*) msg);
  }

  public static unsafe void ModifyDiseaseOnCell(int gameCell, byte disease_idx, int disease_delta)
  {
    SimMessages.CellDiseaseModification* msg = stackalloc SimMessages.CellDiseaseModification[1];
    msg->cellIdx = gameCell;
    msg->diseaseIdx = disease_idx;
    msg->diseaseCount = disease_delta;
    Sim.SIM_HandleMessage(-1853671274, sizeof (SimMessages.CellDiseaseModification), (byte*) msg);
  }

  public static unsafe void ModifyRadiationOnCell(
    int gameCell,
    float radiationDelta,
    int callbackIdx = -1)
  {
    SimMessages.CellRadiationModification* msg = stackalloc SimMessages.CellRadiationModification[1];
    msg->cellIdx = gameCell;
    msg->radiationDelta = radiationDelta;
    msg->callbackIdx = callbackIdx;
    Sim.SIM_HandleMessage(-1914877797, sizeof (SimMessages.CellRadiationModification), (byte*) msg);
  }

  public static unsafe void ModifyRadiationParams(RadiationParams type, float value)
  {
    SimMessages.RadiationParamsModification* msg = stackalloc SimMessages.RadiationParamsModification[1];
    msg->RadiationParamsType = (int) type;
    msg->value = value;
    Sim.SIM_HandleMessage(377112707, sizeof (SimMessages.RadiationParamsModification), (byte*) msg);
  }

  public static ushort GetElementIndex(SimHashes element) => ElementLoader.GetElementIndex(element);

  public static unsafe void ConsumeMass(
    int gameCell,
    SimHashes element,
    float mass,
    byte radius,
    int callbackIdx = -1)
  {
    if (!Grid.IsValidCell(gameCell))
      return;
    ushort elementIndex = ElementLoader.GetElementIndex(element);
    SimMessages.MassConsumptionMessage* msg = stackalloc SimMessages.MassConsumptionMessage[1];
    msg->cellIdx = gameCell;
    msg->callbackIdx = callbackIdx;
    msg->mass = mass;
    msg->elementIdx = elementIndex;
    msg->radius = radius;
    Sim.SIM_HandleMessage(1727657959, sizeof (SimMessages.MassConsumptionMessage), (byte*) msg);
  }

  public static unsafe void EmitMass(
    int gameCell,
    ushort element_idx,
    float mass,
    float temperature,
    byte disease_idx,
    int disease_count,
    int callbackIdx = -1)
  {
    if (!Grid.IsValidCell(gameCell))
      return;
    SimMessages.MassEmissionMessage* msg = stackalloc SimMessages.MassEmissionMessage[1];
    msg->cellIdx = gameCell;
    msg->callbackIdx = callbackIdx;
    msg->mass = mass;
    msg->temperature = temperature;
    msg->elementIdx = element_idx;
    msg->diseaseIdx = disease_idx;
    msg->diseaseCount = disease_count;
    Sim.SIM_HandleMessage(797274363, sizeof (SimMessages.MassEmissionMessage), (byte*) msg);
  }

  public static unsafe void ConsumeDisease(
    int game_cell,
    float percent_to_consume,
    int max_to_consume,
    int callback_idx)
  {
    if (!Grid.IsValidCell(game_cell))
      return;
    SimMessages.ConsumeDiseaseMessage* msg = stackalloc SimMessages.ConsumeDiseaseMessage[1];
    msg->callbackIdx = callback_idx;
    msg->gameCell = game_cell;
    msg->percentToConsume = percent_to_consume;
    msg->maxToConsume = max_to_consume;
    Sim.SIM_HandleMessage(-1019841536, sizeof (SimMessages.ConsumeDiseaseMessage), (byte*) msg);
  }

  public static void AddRemoveSubstance(
    int gameCell,
    SimHashes new_element,
    CellAddRemoveSubstanceEvent ev,
    float mass,
    float temperature,
    byte disease_idx,
    int disease_count,
    bool do_vertical_solid_displacement = true,
    int callbackIdx = -1)
  {
    ushort elementIndex = SimMessages.GetElementIndex(new_element);
    SimMessages.AddRemoveSubstance(gameCell, elementIndex, ev, mass, temperature, disease_idx, disease_count, do_vertical_solid_displacement, callbackIdx);
  }

  public static void AddRemoveSubstance(
    int gameCell,
    ushort elementIdx,
    CellAddRemoveSubstanceEvent ev,
    float mass,
    float temperature,
    byte disease_idx,
    int disease_count,
    bool do_vertical_solid_displacement = true,
    int callbackIdx = -1)
  {
    if (elementIdx == ushort.MaxValue)
      return;
    Element element = ElementLoader.elements[(int) elementIdx];
    float temperature1 = (double) temperature != -1.0 ? temperature : element.defaultValues.temperature;
    SimMessages.ModifyCell(gameCell, elementIdx, temperature1, mass, disease_idx, disease_count, do_vertical_solid_displacement: do_vertical_solid_displacement, callbackIdx: callbackIdx);
  }

  public static void ReplaceElement(
    int gameCell,
    SimHashes new_element,
    CellElementEvent ev,
    float mass,
    float temperature = -1f,
    byte diseaseIdx = 255,
    int diseaseCount = 0,
    int callbackIdx = -1)
  {
    ushort elementIndex = SimMessages.GetElementIndex(new_element);
    if (elementIndex == ushort.MaxValue)
      return;
    Element element = ElementLoader.elements[(int) elementIndex];
    float temperature1 = (double) temperature != -1.0 ? temperature : element.defaultValues.temperature;
    SimMessages.ModifyCell(gameCell, elementIndex, temperature1, mass, diseaseIdx, diseaseCount, SimMessages.ReplaceType.Replace, callbackIdx: callbackIdx);
  }

  public static void ReplaceAndDisplaceElement(
    int gameCell,
    SimHashes new_element,
    CellElementEvent ev,
    float mass,
    float temperature = -1f,
    byte disease_idx = 255,
    int disease_count = 0,
    int callbackIdx = -1)
  {
    ushort elementIndex = SimMessages.GetElementIndex(new_element);
    if (elementIndex == ushort.MaxValue)
      return;
    Element element = ElementLoader.elements[(int) elementIndex];
    float temperature1 = (double) temperature != -1.0 ? temperature : element.defaultValues.temperature;
    SimMessages.ModifyCell(gameCell, elementIndex, temperature1, mass, disease_idx, disease_count, SimMessages.ReplaceType.ReplaceAndDisplace, callbackIdx: callbackIdx);
  }

  public static unsafe void ModifyEnergy(
    int gameCell,
    float kilojoules,
    float max_temperature,
    SimMessages.EnergySourceID id)
  {
    if (!Grid.IsValidCell(gameCell))
      return;
    if ((double) max_temperature <= 0.0)
    {
      Debug.LogError((object) "invalid max temperature for cell energy modification");
    }
    else
    {
      SimMessages.ModifyCellEnergyMessage* msg = stackalloc SimMessages.ModifyCellEnergyMessage[1];
      msg->cellIdx = gameCell;
      msg->kilojoules = kilojoules;
      msg->maxTemperature = max_temperature;
      msg->id = (int) id;
      Sim.SIM_HandleMessage(818320644, sizeof (SimMessages.ModifyCellEnergyMessage), (byte*) msg);
    }
  }

  public static void ModifyMass(
    int gameCell,
    float mass,
    byte disease_idx,
    int disease_count,
    CellModifyMassEvent ev,
    float temperature = -1f,
    SimHashes element = SimHashes.Vacuum)
  {
    if (element != SimHashes.Vacuum)
    {
      ushort elementIndex = SimMessages.GetElementIndex(element);
      if (elementIndex == ushort.MaxValue)
        return;
      if ((double) temperature == -1.0)
        temperature = ElementLoader.elements[(int) elementIndex].defaultValues.temperature;
      SimMessages.ModifyCell(gameCell, elementIndex, temperature, mass, disease_idx, disease_count);
    }
    else
      SimMessages.ModifyCell(gameCell, (ushort) 0, temperature, mass, disease_idx, disease_count);
  }

  public static unsafe void CreateElementInteractions(SimMessages.ElementInteraction[] interactions)
  {
    fixed (SimMessages.ElementInteraction* elementInteractionPtr = interactions)
    {
      SimMessages.CreateElementInteractionsMsg* msg = stackalloc SimMessages.CreateElementInteractionsMsg[1];
      msg->numInteractions = interactions.Length;
      msg->interactions = elementInteractionPtr;
      Sim.SIM_HandleMessage(-930289787, sizeof (SimMessages.CreateElementInteractionsMsg), (byte*) msg);
    }
  }

  public static unsafe void NewGameFrame(
    float elapsed_seconds,
    List<Game.SimActiveRegion> activeRegions)
  {
    Debug.Assert(activeRegions.Count > 0, (object) "NewGameFrame cannot be called with zero activeRegions");
    Sim.NewGameFrame* msg = stackalloc Sim.NewGameFrame[activeRegions.Count];
    Sim.NewGameFrame* newGameFramePtr = msg;
    foreach (Game.SimActiveRegion activeRegion in activeRegions)
    {
      Pair<Vector2I, Vector2I> region = activeRegion.region;
      region.first = new Vector2I(MathUtil.Clamp(0, Grid.WidthInCells - 1, activeRegion.region.first.x), MathUtil.Clamp(0, Grid.HeightInCells - 1, activeRegion.region.first.y));
      region.second = new Vector2I(MathUtil.Clamp(0, Grid.WidthInCells - 1, activeRegion.region.second.x), MathUtil.Clamp(0, Grid.HeightInCells - 1, activeRegion.region.second.y));
      newGameFramePtr->elapsedSeconds = elapsed_seconds;
      newGameFramePtr->minX = region.first.x;
      newGameFramePtr->minY = region.first.y;
      newGameFramePtr->maxX = region.second.x;
      newGameFramePtr->maxY = region.second.y;
      newGameFramePtr->currentSunlightIntensity = activeRegion.currentSunlightIntensity;
      newGameFramePtr->currentCosmicRadiationIntensity = activeRegion.currentCosmicRadiationIntensity;
      ++newGameFramePtr;
    }
    Sim.SIM_HandleMessage(-775326397, sizeof (Sim.NewGameFrame) * activeRegions.Count, (byte*) msg);
  }

  public static unsafe void SetDebugProperties(Sim.DebugProperties properties)
  {
    Sim.DebugProperties* msg = stackalloc Sim.DebugProperties[1];
    msg[0] = properties;
    msg->buildingTemperatureScale = properties.buildingTemperatureScale;
    msg->buildingToBuildingTemperatureScale = properties.buildingToBuildingTemperatureScale;
    Sim.SIM_HandleMessage(-1683118492, sizeof (Sim.DebugProperties), (byte*) msg);
  }

  public static unsafe void ModifyCellWorldZone(int cell, byte zone_id)
  {
    SimMessages.CellWorldZoneModification* msg = stackalloc SimMessages.CellWorldZoneModification[1];
    msg->cell = cell;
    msg->zoneID = zone_id;
    Sim.SIM_HandleMessage(-449718014, sizeof (SimMessages.CellWorldZoneModification), (byte*) msg);
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  private struct AddElementConsumerMessage
  {
    public int cellIdx;
    public int callbackIdx;
    public byte radius;
    public byte configuration;
    public ushort elementIdx;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  private struct SetElementConsumerDataMessage
  {
    public int handle;
    public int cell;
    public float consumptionRate;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  private struct RemoveElementConsumerMessage
  {
    public int handle;
    public int callbackIdx;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  private struct AddElementEmitterMessage
  {
    public float maxPressure;
    public int callbackIdx;
    public int onBlockedCB;
    public int onUnblockedCB;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  private struct ModifyElementEmitterMessage
  {
    public int handle;
    public int cellIdx;
    public float emitInterval;
    public float emitMass;
    public float emitTemperature;
    public float maxPressure;
    public int diseaseCount;
    public ushort elementIdx;
    public byte maxDepth;
    public byte diseaseIdx;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  private struct RemoveElementEmitterMessage
  {
    public int handle;
    public int callbackIdx;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  private struct AddRadiationEmitterMessage
  {
    public int callbackIdx;
    public int cell;
    public short emitRadiusX;
    public short emitRadiusY;
    public float emitRads;
    public float emitRate;
    public float emitSpeed;
    public float emitDirection;
    public float emitAngle;
    public int emitType;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  private struct ModifyRadiationEmitterMessage
  {
    public int handle;
    public int cell;
    public int callbackIdx;
    public short emitRadiusX;
    public short emitRadiusY;
    public float emitRads;
    public float emitRate;
    public float emitSpeed;
    public float emitDirection;
    public float emitAngle;
    public int emitType;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  private struct RemoveRadiationEmitterMessage
  {
    public int handle;
    public int callbackIdx;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  private struct AddElementChunkMessage
  {
    public int gameCell;
    public int callbackIdx;
    public float mass;
    public float temperature;
    public float surfaceArea;
    public float thickness;
    public float groundTransferScale;
    public ushort elementIdx;
    public byte pad0;
    public byte pad1;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  private struct RemoveElementChunkMessage
  {
    public int handle;
    public int callbackIdx;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  private struct SetElementChunkDataMessage
  {
    public int handle;
    public float temperature;
    public float heatCapacity;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  private struct MoveElementChunkMessage
  {
    public int handle;
    public int gameCell;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  private struct ModifyElementChunkEnergyMessage
  {
    public int handle;
    public float deltaKJ;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  private struct ModifyElementChunkAdjusterMessage
  {
    public int handle;
    public float temperature;
    public float heatCapacity;
    public float thermalConductivity;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct AddBuildingHeatExchangeMessage
  {
    public int callbackIdx;
    public ushort elemIdx;
    public byte pad0;
    public byte pad1;
    public float mass;
    public float temperature;
    public float thermalConductivity;
    public float overheatTemperature;
    public float operatingKilowatts;
    public int minX;
    public int minY;
    public int maxX;
    public int maxY;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct ModifyBuildingHeatExchangeMessage
  {
    public int callbackIdx;
    public ushort elemIdx;
    public byte pad0;
    public byte pad1;
    public float mass;
    public float temperature;
    public float thermalConductivity;
    public float overheatTemperature;
    public float operatingKilowatts;
    public int minX;
    public int minY;
    public int maxX;
    public int maxY;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct ModifyBuildingEnergyMessage
  {
    public int handle;
    public float deltaKJ;
    public float minTemperature;
    public float maxTemperature;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct RemoveBuildingHeatExchangeMessage
  {
    public int handle;
    public int callbackIdx;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct RegisterBuildingToBuildingHeatExchangeMessage
  {
    public int callbackIdx;
    public int structureTemperatureHandler;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct AddBuildingToBuildingHeatExchangeMessage
  {
    public int selfHandler;
    public int buildingInContactHandle;
    public int cellsInContact;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct RemoveBuildingInContactFromBuildingToBuildingHeatExchangeMessage
  {
    public int selfHandler;
    public int buildingNoLongerInContactHandler;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct RemoveBuildingToBuildingHeatExchangeMessage
  {
    public int callbackIdx;
    public int selfHandler;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct AddDiseaseEmitterMessage
  {
    public int callbackIdx;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct ModifyDiseaseEmitterMessage
  {
    public int handle;
    public int gameCell;
    public byte diseaseIdx;
    public byte maxDepth;
    private byte pad0;
    private byte pad1;
    public float emitInterval;
    public int emitCount;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct RemoveDiseaseEmitterMessage
  {
    public int handle;
    public int callbackIdx;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  private struct SetSavedOptionsMessage
  {
    public byte clearBits;
    public byte setBits;
  }

  public enum SimSavedOptions : byte
  {
    ENABLE_DIAGONAL_FALLING_SAND = 1,
  }

  public struct WorldOffsetData
  {
    public int worldOffsetX;
    public int worldOffsetY;
    public int worldSizeX;
    public int worldSizeY;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  private struct DigMessage
  {
    public int cellIdx;
    public int callbackIdx;
    public bool skipEvent;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  private struct SetCellFloatValueMessage
  {
    public int cellIdx;
    public float value;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  private struct CellPropertiesMessage
  {
    public int cellIdx;
    public byte properties;
    public byte set;
    public byte pad0;
    public byte pad1;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  private struct SetInsulationValueMessage
  {
    public int cellIdx;
    public float value;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  private struct ModifyCellMessage
  {
    public int cellIdx;
    public int callbackIdx;
    public float temperature;
    public float mass;
    public int diseaseCount;
    public ushort elementIdx;
    public byte replaceType;
    public byte diseaseIdx;
    public byte addSubType;
  }

  public enum ReplaceType
  {
    None,
    Replace,
    ReplaceAndDisplace,
  }

  private enum AddSolidMassSubType
  {
    DoVerticalDisplacement,
    OnlyIfSameElement,
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  private struct CellDiseaseModification
  {
    public int cellIdx;
    public byte diseaseIdx;
    public byte pad0;
    public byte pad1;
    public byte pad2;
    public int diseaseCount;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  private struct RadiationParamsModification
  {
    public int RadiationParamsType;
    public float value;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  private struct CellRadiationModification
  {
    public int cellIdx;
    public float radiationDelta;
    public int callbackIdx;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  private struct MassConsumptionMessage
  {
    public int cellIdx;
    public int callbackIdx;
    public float mass;
    public ushort elementIdx;
    public byte radius;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  private struct MassEmissionMessage
  {
    public int cellIdx;
    public int callbackIdx;
    public float mass;
    public float temperature;
    public int diseaseCount;
    public ushort elementIdx;
    public byte diseaseIdx;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  private struct ConsumeDiseaseMessage
  {
    public int gameCell;
    public int callbackIdx;
    public float percentToConsume;
    public int maxToConsume;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  private struct ModifyCellEnergyMessage
  {
    public int cellIdx;
    public float kilojoules;
    public float maxTemperature;
    public int id;
  }

  public enum EnergySourceID
  {
    DebugHeat = 1000, // 0x000003E8
    DebugCool = 1001, // 0x000003E9
    FierySkin = 1002, // 0x000003EA
    Overheatable = 1003, // 0x000003EB
    LiquidCooledFan = 1004, // 0x000003EC
    ConduitTemperatureManager = 1005, // 0x000003ED
    Excavator = 1006, // 0x000003EE
    HeatBulb = 1007, // 0x000003EF
    WarmBlooded = 1008, // 0x000003F0
    StructureTemperature = 1009, // 0x000003F1
    Burner = 1010, // 0x000003F2
    VacuumRadiator = 1011, // 0x000003F3
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  private struct VisibleCells
  {
    public Vector2I min;
    public Vector2I max;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  private struct WakeCellMessage
  {
    public int gameCell;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct ElementInteraction
  {
    public uint interactionType;
    public ushort elemIdx1;
    public ushort elemIdx2;
    public ushort elemResultIdx;
    public byte pad0;
    public byte pad1;
    public float minMass;
    public float interactionProbability;
    public float elem1MassDestructionPercent;
    public float elem2MassRequiredMultiplier;
    public float elemResultMassCreationMultiplier;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  private struct CreateElementInteractionsMsg
  {
    public int numInteractions;
    public unsafe SimMessages.ElementInteraction* interactions;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct PipeChange
  {
    public int cell;
    public byte layer;
    public byte pad0;
    public byte pad1;
    public byte pad2;
    public float mass;
    public float temperature;
    public int elementHash;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct CellWorldZoneModification
  {
    public int cell;
    public byte zoneID;
  }
}
