// Decompiled with JetBrains decompiler
// Type: Sim
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;

public static class Sim
{
  public const int InvalidHandle = -1;
  public const int QueuedRegisterHandle = -2;
  public const byte InvalidDiseaseIdx = 255;
  public const ushort InvalidElementIdx = 65535;
  public const byte SpaceZoneID = 255;
  public const byte SolidZoneID = 0;
  public const int ChunkEdgeSize = 32;
  public const float StateTransitionEnergy = 3f;
  public const float ZeroDegreesCentigrade = 273.15f;
  public const float StandardTemperature = 293.15f;
  public const float StandardPressure = 101.3f;
  public const float Epsilon = 0.0001f;
  public const float MaxTemperature = 10000f;
  public const float MinTemperature = 0.0f;
  public const float MaxRadiation = 9000000f;
  public const float MinRadiation = 0.0f;
  public const float MaxMass = 10000f;
  public const float MinMass = 1.0001f;
  private const int PressureUpdateInterval = 1;
  private const int TemperatureUpdateInterval = 1;
  private const int LiquidUpdateInterval = 1;
  private const int LifeUpdateInterval = 1;
  public const byte ClearSkyGridValue = 253;
  public const int PACKING_ALIGNMENT = 4;

  public static bool IsRadiationEnabled() => DlcManager.FeatureRadiationEnabled();

  public static bool IsValidHandle(int h) => h != -1 && h != -2;

  public static int GetHandleIndex(int h) => h & 16777215;

  [DllImport("SimDLL")]
  public static extern void SIM_Initialize(Sim.GAME_MessageHandler callback);

  [DllImport("SimDLL")]
  public static extern void SIM_Shutdown();

  [DllImport("SimDLL")]
  public static extern unsafe IntPtr SIM_HandleMessage(int sim_msg_id, int msg_length, byte* msg);

  [DllImport("SimDLL")]
  private static extern unsafe byte* SIM_BeginSave(int* size, int x, int y);

  [DllImport("SimDLL")]
  private static extern void SIM_EndSave();

  [DllImport("SimDLL")]
  public static extern void SIM_DebugCrash();

  public static unsafe IntPtr HandleMessage(
    SimMessageHashes sim_msg_id,
    int msg_length,
    byte[] msg)
  {
    IntPtr num;
    fixed (byte* msg1 = msg)
      num = Sim.SIM_HandleMessage((int) sim_msg_id, msg_length, msg1);
    return num;
  }

  public static unsafe void Save(BinaryWriter writer, int x, int y)
  {
    int length;
    byte* source = Sim.SIM_BeginSave(&length, x, y);
    byte[] numArray = new byte[length];
    Marshal.Copy((IntPtr) (void*) source, numArray, 0, length);
    Sim.SIM_EndSave();
    writer.Write(length);
    writer.Write(numArray);
  }

  public static unsafe int LoadWorld(IReader reader)
  {
    int msg_length = reader.ReadInt32();
    IntPtr num;
    fixed (byte* msg = reader.ReadBytes(msg_length))
      num = Sim.SIM_HandleMessage(-672538170, msg_length, msg);
    IntPtr zero = IntPtr.Zero;
    return num == zero ? -1 : 0;
  }

  public static void AllocateCells(int width, int height, bool headless = false)
  {
    using (MemoryStream output = new MemoryStream(8))
    {
      using (BinaryWriter binaryWriter = new BinaryWriter((Stream) output))
      {
        binaryWriter.Write(width);
        binaryWriter.Write(height);
        bool flag = Sim.IsRadiationEnabled();
        binaryWriter.Write(flag);
        binaryWriter.Write(headless);
        binaryWriter.Flush();
        Sim.HandleMessage(SimMessageHashes.AllocateCells, (int) output.Length, output.GetBuffer());
      }
    }
  }

  public static unsafe int Load(IReader reader)
  {
    int msg_length = reader.ReadInt32();
    IntPtr num;
    fixed (byte* msg = reader.ReadBytes(msg_length))
      num = Sim.SIM_HandleMessage(-672538170, msg_length, msg);
    IntPtr zero = IntPtr.Zero;
    return num == zero ? -1 : 0;
  }

  public static unsafe void Start()
  {
    Sim.GameDataUpdate* gameDataUpdatePtr = (Sim.GameDataUpdate*) (void*) Sim.SIM_HandleMessage(-931446686, 0, (byte*) null);
    Grid.elementIdx = gameDataUpdatePtr->elementIdx;
    Grid.temperature = gameDataUpdatePtr->temperature;
    Grid.radiation = gameDataUpdatePtr->radiation;
    Grid.mass = gameDataUpdatePtr->mass;
    Grid.properties = gameDataUpdatePtr->properties;
    Grid.strengthInfo = gameDataUpdatePtr->strengthInfo;
    Grid.insulation = gameDataUpdatePtr->insulation;
    Grid.diseaseIdx = gameDataUpdatePtr->diseaseIdx;
    Grid.diseaseCount = gameDataUpdatePtr->diseaseCount;
    Grid.AccumulatedFlowValues = gameDataUpdatePtr->accumulatedFlow;
    PropertyTextures.externalFlowTex = gameDataUpdatePtr->propertyTextureFlow;
    PropertyTextures.externalLiquidTex = gameDataUpdatePtr->propertyTextureLiquid;
    PropertyTextures.externalExposedToSunlight = gameDataUpdatePtr->propertyTextureExposedToSunlight;
    Grid.InitializeCells();
  }

  public static unsafe void Shutdown()
  {
    Sim.SIM_Shutdown();
    Grid.mass = (float*) null;
  }

  [DllImport("SimDLL")]
  public static extern unsafe char* SYSINFO_Acquire();

  [DllImport("SimDLL")]
  public static extern void SYSINFO_Release();

  public static unsafe int DLL_MessageHandler(int message_id, IntPtr data)
  {
    switch ((Sim.GameHandledMessages) message_id)
    {
      case Sim.GameHandledMessages.ExceptionHandler:
        Sim.DLLExceptionHandlerMessage* exceptionHandlerMessagePtr = (Sim.DLLExceptionHandlerMessage*) (void*) data;
        KCrashReporter.ReportSimDLLCrash("SimDLL Crash Dump", Marshal.PtrToStringAnsi(exceptionHandlerMessagePtr->callstack), Marshal.PtrToStringAnsi(exceptionHandlerMessagePtr->dmpFilename));
        return 0;
      case Sim.GameHandledMessages.ReportMessage:
        Sim.DLLReportMessageMessage* reportMessageMessagePtr = (Sim.DLLReportMessageMessage*) (void*) data;
        string msg = "SimMessage: " + Marshal.PtrToStringAnsi(reportMessageMessagePtr->message);
        string str;
        if (reportMessageMessagePtr->callstack != IntPtr.Zero)
          str = Marshal.PtrToStringAnsi(reportMessageMessagePtr->callstack);
        else
          str = Marshal.PtrToStringAnsi(reportMessageMessagePtr->file) + ":" + reportMessageMessagePtr->line.ToString();
        string stack_trace = str;
        KCrashReporter.ReportSimDLLCrash(msg, stack_trace, (string) null);
        return 0;
      default:
        return -1;
    }
  }

  public delegate int GAME_MessageHandler(int message_id, IntPtr data);

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct DLLExceptionHandlerMessage
  {
    public IntPtr callstack;
    public IntPtr dmpFilename;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct DLLReportMessageMessage
  {
    public IntPtr callstack;
    public IntPtr message;
    public IntPtr file;
    public int line;
  }

  private enum GameHandledMessages
  {
    ExceptionHandler,
    ReportMessage,
  }

  [Serializable]
  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct PhysicsData
  {
    public float temperature;
    public float mass;
    public float pressure;

    public void Write(BinaryWriter writer)
    {
      writer.Write(this.temperature);
      writer.Write(this.mass);
      writer.Write(this.pressure);
    }
  }

  [StructLayout(LayoutKind.Sequential, Pack = 1)]
  public struct Cell
  {
    public ushort elementIdx;
    public byte properties;
    public byte insulation;
    public byte strengthInfo;
    public byte pad0;
    public byte pad1;
    public byte pad2;
    public float temperature;
    public float mass;

    public void Write(BinaryWriter writer)
    {
      writer.Write(this.elementIdx);
      writer.Write((byte) 0);
      writer.Write(this.insulation);
      writer.Write((byte) 0);
      writer.Write(this.pad0);
      writer.Write(this.pad1);
      writer.Write(this.pad2);
      writer.Write(this.temperature);
      writer.Write(this.mass);
    }

    public void SetValues(global::Element elem, List<global::Element> elements) => this.SetValues(elem, elem.defaultValues, elements);

    public void SetValues(global::Element elem, Sim.PhysicsData pd, List<global::Element> elements)
    {
      this.elementIdx = (ushort) elements.IndexOf(elem);
      this.temperature = pd.temperature;
      this.mass = pd.mass;
      this.insulation = byte.MaxValue;
      DebugUtil.Assert((double) this.temperature > 0.0 || (double) this.mass == 0.0, "A non-zero mass cannot have a <= 0 temperature");
    }

    public void SetValues(ushort new_elem_idx, float new_temperature, float new_mass)
    {
      this.elementIdx = new_elem_idx;
      this.temperature = new_temperature;
      this.mass = new_mass;
      this.insulation = byte.MaxValue;
      DebugUtil.Assert((double) this.temperature > 0.0 || (double) this.mass == 0.0, "A non-zero mass cannot have a <= 0 temperature");
    }

    public enum Properties
    {
      GasImpermeable = 1,
      LiquidImpermeable = 2,
      SolidImpermeable = 4,
      Unbreakable = 8,
      Transparent = 16, // 0x00000010
      Opaque = 32, // 0x00000020
      NotifyOnMelt = 64, // 0x00000040
      ConstructedTile = 128, // 0x00000080
    }
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct Element
  {
    public SimHashes id;
    public ushort lowTempTransitionIdx;
    public ushort highTempTransitionIdx;
    public ushort elementsTableIdx;
    public byte state;
    public byte pack0;
    public float specificHeatCapacity;
    public float thermalConductivity;
    public float molarMass;
    public float solidSurfaceAreaMultiplier;
    public float liquidSurfaceAreaMultiplier;
    public float gasSurfaceAreaMultiplier;
    public float flow;
    public float viscosity;
    public float minHorizontalFlow;
    public float minVerticalFlow;
    public float maxMass;
    public float lowTemp;
    public float highTemp;
    public float strength;
    public SimHashes lowTempTransitionOreID;
    public float lowTempTransitionOreMassConversion;
    public SimHashes highTempTransitionOreID;
    public float highTempTransitionOreMassConversion;
    public ushort sublimateIndex;
    public ushort convertIndex;
    public uint colour;
    public SpawnFXHashes sublimateFX;
    public float sublimateRate;
    public float sublimateEfficiency;
    public float sublimateProbability;
    public float offGasProbability;
    public float lightAbsorptionFactor;
    public float radiationAbsorptionFactor;
    public float radiationPer1000Mass;
    public Sim.PhysicsData defaultValues;

    public Element(global::Element e, List<global::Element> elements)
    {
      this.id = e.id;
      this.state = (byte) e.state;
      if (e.HasTag(GameTags.Unstable))
        this.state |= (byte) 8;
      int index1 = elements.FindIndex((Predicate<global::Element>) (ele => ele.id == e.lowTempTransitionTarget));
      int index2 = elements.FindIndex((Predicate<global::Element>) (ele => ele.id == e.highTempTransitionTarget));
      this.lowTempTransitionIdx = index1 >= 0 ? (ushort) index1 : ushort.MaxValue;
      this.highTempTransitionIdx = index2 >= 0 ? (ushort) index2 : ushort.MaxValue;
      this.elementsTableIdx = (ushort) elements.IndexOf(e);
      this.specificHeatCapacity = e.specificHeatCapacity;
      this.thermalConductivity = e.thermalConductivity;
      this.solidSurfaceAreaMultiplier = e.solidSurfaceAreaMultiplier;
      this.liquidSurfaceAreaMultiplier = e.liquidSurfaceAreaMultiplier;
      this.gasSurfaceAreaMultiplier = e.gasSurfaceAreaMultiplier;
      this.molarMass = e.molarMass;
      this.strength = e.strength;
      this.flow = e.flow;
      this.viscosity = e.viscosity;
      this.minHorizontalFlow = e.minHorizontalFlow;
      this.minVerticalFlow = e.minVerticalFlow;
      this.maxMass = e.maxMass;
      this.lowTemp = e.lowTemp;
      this.highTemp = e.highTemp;
      this.highTempTransitionOreID = e.highTempTransitionOreID;
      this.highTempTransitionOreMassConversion = e.highTempTransitionOreMassConversion;
      this.lowTempTransitionOreID = e.lowTempTransitionOreID;
      this.lowTempTransitionOreMassConversion = e.lowTempTransitionOreMassConversion;
      this.sublimateIndex = (ushort) elements.FindIndex((Predicate<global::Element>) (ele => ele.id == e.sublimateId));
      this.convertIndex = (ushort) elements.FindIndex((Predicate<global::Element>) (ele => ele.id == e.convertId));
      this.pack0 = (byte) 0;
      if (e.substance == null)
      {
        this.colour = 0U;
      }
      else
      {
        Color32 colour = e.substance.colour;
        this.colour = (uint) ((int) colour.a << 24 | (int) colour.b << 16 | (int) colour.g << 8) | (uint) colour.r;
      }
      this.sublimateFX = e.sublimateFX;
      this.sublimateRate = e.sublimateRate;
      this.sublimateEfficiency = e.sublimateEfficiency;
      this.sublimateProbability = e.sublimateProbability;
      this.offGasProbability = e.offGasPercentage;
      this.lightAbsorptionFactor = e.lightAbsorptionFactor;
      this.radiationAbsorptionFactor = e.radiationAbsorptionFactor;
      this.radiationPer1000Mass = e.radiationPer1000Mass;
      this.defaultValues = e.defaultValues;
    }

    public void Write(BinaryWriter writer)
    {
      writer.Write((int) this.id);
      writer.Write(this.lowTempTransitionIdx);
      writer.Write(this.highTempTransitionIdx);
      writer.Write(this.elementsTableIdx);
      writer.Write(this.state);
      writer.Write(this.pack0);
      writer.Write(this.specificHeatCapacity);
      writer.Write(this.thermalConductivity);
      writer.Write(this.molarMass);
      writer.Write(this.solidSurfaceAreaMultiplier);
      writer.Write(this.liquidSurfaceAreaMultiplier);
      writer.Write(this.gasSurfaceAreaMultiplier);
      writer.Write(this.flow);
      writer.Write(this.viscosity);
      writer.Write(this.minHorizontalFlow);
      writer.Write(this.minVerticalFlow);
      writer.Write(this.maxMass);
      writer.Write(this.lowTemp);
      writer.Write(this.highTemp);
      writer.Write(this.strength);
      writer.Write((int) this.lowTempTransitionOreID);
      writer.Write(this.lowTempTransitionOreMassConversion);
      writer.Write((int) this.highTempTransitionOreID);
      writer.Write(this.highTempTransitionOreMassConversion);
      writer.Write(this.sublimateIndex);
      writer.Write(this.convertIndex);
      writer.Write(this.colour);
      writer.Write((int) this.sublimateFX);
      writer.Write(this.sublimateRate);
      writer.Write(this.sublimateEfficiency);
      writer.Write(this.sublimateProbability);
      writer.Write(this.offGasProbability);
      writer.Write(this.lightAbsorptionFactor);
      writer.Write(this.radiationAbsorptionFactor);
      writer.Write(this.radiationPer1000Mass);
      this.defaultValues.Write(writer);
    }
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct DiseaseCell
  {
    public byte diseaseIdx;
    private byte reservedInfestationTickCount;
    private byte pad1;
    private byte pad2;
    public int elementCount;
    private float reservedAccumulatedError;
    public static readonly Sim.DiseaseCell Invalid = new Sim.DiseaseCell()
    {
      diseaseIdx = byte.MaxValue,
      elementCount = 0
    };

    public void Write(BinaryWriter writer)
    {
      writer.Write(this.diseaseIdx);
      writer.Write(this.reservedInfestationTickCount);
      writer.Write(this.pad1);
      writer.Write(this.pad2);
      writer.Write(this.elementCount);
      writer.Write(this.reservedAccumulatedError);
    }
  }

  public delegate void GAME_Callback();

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct SolidInfo
  {
    public int cellIdx;
    public int isSolid;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct LiquidChangeInfo
  {
    public int cellIdx;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct SolidSubstanceChangeInfo
  {
    public int cellIdx;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct SubstanceChangeInfo
  {
    public int cellIdx;
    public ushort oldElemIdx;
    public ushort newElemIdx;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct CallbackInfo
  {
    public int callbackIdx;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct GameDataUpdate
  {
    public int numFramesProcessed;
    public unsafe ushort* elementIdx;
    public unsafe float* temperature;
    public unsafe float* mass;
    public unsafe byte* properties;
    public unsafe byte* insulation;
    public unsafe byte* strengthInfo;
    public unsafe float* radiation;
    public unsafe byte* diseaseIdx;
    public unsafe int* diseaseCount;
    public int numSolidInfo;
    public unsafe Sim.SolidInfo* solidInfo;
    public int numLiquidChangeInfo;
    public unsafe Sim.LiquidChangeInfo* liquidChangeInfo;
    public int numSolidSubstanceChangeInfo;
    public unsafe Sim.SolidSubstanceChangeInfo* solidSubstanceChangeInfo;
    public int numSubstanceChangeInfo;
    public unsafe Sim.SubstanceChangeInfo* substanceChangeInfo;
    public int numCallbackInfo;
    public unsafe Sim.CallbackInfo* callbackInfo;
    public int numSpawnFallingLiquidInfo;
    public unsafe Sim.SpawnFallingLiquidInfo* spawnFallingLiquidInfo;
    public int numDigInfo;
    public unsafe Sim.SpawnOreInfo* digInfo;
    public int numSpawnOreInfo;
    public unsafe Sim.SpawnOreInfo* spawnOreInfo;
    public int numSpawnFXInfo;
    public unsafe Sim.SpawnFXInfo* spawnFXInfo;
    public int numUnstableCellInfo;
    public unsafe Sim.UnstableCellInfo* unstableCellInfo;
    public int numWorldDamageInfo;
    public unsafe Sim.WorldDamageInfo* worldDamageInfo;
    public int numBuildingTemperatures;
    public unsafe Sim.BuildingTemperatureInfo* buildingTemperatures;
    public int numMassConsumedCallbacks;
    public unsafe Sim.MassConsumedCallback* massConsumedCallbacks;
    public int numMassEmittedCallbacks;
    public unsafe Sim.MassEmittedCallback* massEmittedCallbacks;
    public int numDiseaseConsumptionCallbacks;
    public unsafe Sim.DiseaseConsumptionCallback* diseaseConsumptionCallbacks;
    public int numComponentStateChangedMessages;
    public unsafe Sim.ComponentStateChangedMessage* componentStateChangedMessages;
    public int numRemovedMassEntries;
    public unsafe Sim.ConsumedMassInfo* removedMassEntries;
    public int numEmittedMassEntries;
    public unsafe Sim.EmittedMassInfo* emittedMassEntries;
    public int numElementChunkInfos;
    public unsafe Sim.ElementChunkInfo* elementChunkInfos;
    public int numElementChunkMeltedInfos;
    public unsafe Sim.MeltedInfo* elementChunkMeltedInfos;
    public int numBuildingOverheatInfos;
    public unsafe Sim.MeltedInfo* buildingOverheatInfos;
    public int numBuildingNoLongerOverheatedInfos;
    public unsafe Sim.MeltedInfo* buildingNoLongerOverheatedInfos;
    public int numBuildingMeltedInfos;
    public unsafe Sim.MeltedInfo* buildingMeltedInfos;
    public int numCellMeltedInfos;
    public unsafe Sim.CellMeltedInfo* cellMeltedInfos;
    public int numDiseaseEmittedInfos;
    public unsafe Sim.DiseaseEmittedInfo* diseaseEmittedInfos;
    public int numDiseaseConsumedInfos;
    public unsafe Sim.DiseaseConsumedInfo* diseaseConsumedInfos;
    public int numRadiationConsumedCallbacks;
    public unsafe Sim.ConsumedRadiationCallback* radiationConsumedCallbacks;
    public unsafe float* accumulatedFlow;
    public IntPtr propertyTextureFlow;
    public IntPtr propertyTextureLiquid;
    public IntPtr propertyTextureExposedToSunlight;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct SpawnFallingLiquidInfo
  {
    public int cellIdx;
    public ushort elemIdx;
    public byte diseaseIdx;
    public byte pad0;
    public float mass;
    public float temperature;
    public int diseaseCount;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct SpawnOreInfo
  {
    public int cellIdx;
    public ushort elemIdx;
    public byte diseaseIdx;
    private byte pad0;
    public float mass;
    public float temperature;
    public int diseaseCount;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct SpawnFXInfo
  {
    public int cellIdx;
    public int fxHash;
    public float rotation;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct UnstableCellInfo
  {
    public int cellIdx;
    public ushort elemIdx;
    public byte fallingInfo;
    public byte diseaseIdx;
    public float mass;
    public float temperature;
    public int diseaseCount;

    public enum FallingInfo
    {
      StartedFalling,
      StoppedFalling,
    }
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct NewGameFrame
  {
    public float elapsedSeconds;
    public int minX;
    public int minY;
    public int maxX;
    public int maxY;
    public float currentSunlightIntensity;
    public float currentCosmicRadiationIntensity;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct WorldDamageInfo
  {
    public int gameCell;
    public int damageSourceOffset;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct PipeTemperatureChange
  {
    public int cellIdx;
    public float temperature;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct MassConsumedCallback
  {
    public int callbackIdx;
    public ushort elemIdx;
    public byte diseaseIdx;
    private byte pad0;
    public float mass;
    public float temperature;
    public int diseaseCount;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct MassEmittedCallback
  {
    public int callbackIdx;
    public ushort elemIdx;
    public byte suceeded;
    public byte diseaseIdx;
    public float mass;
    public float temperature;
    public int diseaseCount;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct DiseaseConsumptionCallback
  {
    public int callbackIdx;
    public byte diseaseIdx;
    private byte pad0;
    private byte pad1;
    private byte pad2;
    public int diseaseCount;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct ComponentStateChangedMessage
  {
    public int callbackIdx;
    public int simHandle;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct DebugProperties
  {
    public float buildingTemperatureScale;
    public float buildingToBuildingTemperatureScale;
    public float contaminatedOxygenEmitProbability;
    public float contaminatedOxygenConversionPercent;
    public float biomeTemperatureLerpRate;
    public byte isDebugEditing;
    public byte pad0;
    public byte pad1;
    public byte pad2;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct EmittedMassInfo
  {
    public ushort elemIdx;
    public byte diseaseIdx;
    public byte pad0;
    public float mass;
    public float temperature;
    public int diseaseCount;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct ConsumedMassInfo
  {
    public int simHandle;
    public ushort removedElemIdx;
    public byte diseaseIdx;
    private byte pad0;
    public float mass;
    public float temperature;
    public int diseaseCount;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct ConsumedDiseaseInfo
  {
    public int simHandle;
    public byte diseaseIdx;
    private byte pad0;
    private byte pad1;
    private byte pad2;
    public int diseaseCount;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct ElementChunkInfo
  {
    public float temperature;
    public float deltaKJ;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct MeltedInfo
  {
    public int handle;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct CellMeltedInfo
  {
    public int gameCell;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct BuildingTemperatureInfo
  {
    public int handle;
    public float temperature;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct BuildingConductivityData
  {
    public float temperature;
    public float heatCapacity;
    public float thermalConductivity;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct DiseaseEmittedInfo
  {
    public byte diseaseIdx;
    private byte pad0;
    private byte pad1;
    private byte pad2;
    public int count;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct DiseaseConsumedInfo
  {
    public byte diseaseIdx;
    private byte pad0;
    private byte pad1;
    private byte pad2;
    public int count;
  }

  [StructLayout(LayoutKind.Sequential, Pack = 4)]
  public struct ConsumedRadiationCallback
  {
    public int callbackIdx;
    public int gameCell;
    public float radiation;
  }
}
