// Decompiled with JetBrains decompiler
// Type: ProcGenGame.WorldGenSimUtil
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei;
using KSerialization;
using ProcGen;
using STRINGS;
using System;
using System.Collections.Generic;
using System.IO;

namespace ProcGenGame
{
  public static class WorldGenSimUtil
  {
    private const int STEPS = 500;

    public static unsafe bool DoSettleSim(
      WorldGenSettings settings,
      ref Sim.Cell[] cells,
      ref float[] bgTemp,
      ref Sim.DiseaseCell[] dcs,
      WorldGen.OfflineCallbackFunction updateProgressFn,
      Klei.Data data,
      List<KeyValuePair<Vector2I, TemplateContainer>> templateSpawnTargets,
      Action<OfflineWorldGen.ErrorInfo> error_cb,
      int baseId)
    {
      Sim.SIM_Initialize(new Sim.GAME_MessageHandler(Sim.DLL_MessageHandler));
      SimMessages.CreateSimElementsTable(ElementLoader.elements);
      SimMessages.CreateDiseaseTable(WorldGen.diseaseStats);
      SimMessages.SimDataInitializeFromCells(Grid.WidthInCells, Grid.HeightInCells, cells, bgTemp, dcs, true);
      int num1 = updateProgressFn(UI.WORLDGEN.SETTLESIM.key, 0.0f, WorldGenProgressStages.Stages.SettleSim) ? 1 : 0;
      Sim.Start();
      byte[] msg = new byte[Grid.CellCount];
      for (int index = 0; index < Grid.CellCount; ++index)
        msg[index] = byte.MaxValue;
      Vector2I vector2I;
      // ISSUE: explicit constructor call
      ((Vector2I) ref vector2I).\u002Ector(0, 0);
      Vector2I size = data.world.size;
      List<Game.SimActiveRegion> activeRegions = new List<Game.SimActiveRegion>();
      activeRegions.Add(new Game.SimActiveRegion()
      {
        region = new Pair<Vector2I, Vector2I>(vector2I, size)
      });
      for (int index1 = 0; index1 < 500; ++index1)
      {
        if (index1 == 498)
        {
          HashSet<int> intSet = new HashSet<int>();
          foreach (KeyValuePair<Vector2I, TemplateContainer> templateSpawnTarget in templateSpawnTargets)
          {
            if (templateSpawnTarget.Value.cells != null)
            {
              for (int index2 = 0; index2 < templateSpawnTarget.Value.cells.Count; ++index2)
              {
                TemplateClasses.Cell cell = templateSpawnTarget.Value.cells[index2];
                int num2 = Grid.OffsetCell(Grid.XYToCell(templateSpawnTarget.Key.x, templateSpawnTarget.Key.y), cell.location_x, cell.location_y);
                if (Grid.IsValidCell(num2) && !intSet.Contains(num2))
                {
                  intSet.Add(num2);
                  ushort elementIndex = ElementLoader.GetElementIndex(cell.element);
                  float temperature = cell.temperature;
                  float mass = cell.mass;
                  byte index3 = WorldGen.diseaseStats.GetIndex(HashedString.op_Implicit(cell.diseaseName));
                  int diseaseCount = cell.diseaseCount;
                  SimMessages.ModifyCell(num2, elementIndex, temperature, mass, index3, diseaseCount, SimMessages.ReplaceType.Replace);
                }
              }
            }
          }
        }
        SimMessages.NewGameFrame(0.2f, activeRegions);
        IntPtr num3 = Sim.HandleMessage(SimMessageHashes.PrepareGameData, msg.Length, msg);
        int num4 = updateProgressFn(UI.WORLDGEN.SETTLESIM.key, (float) index1 / 500f, WorldGenProgressStages.Stages.SettleSim) ? 1 : 0;
        if (num3 == IntPtr.Zero)
        {
          DebugUtil.LogWarningArgs(new object[1]
          {
            (object) "Unexpected"
          });
        }
        else
        {
          Sim.GameDataUpdate* gameDataUpdatePtr = (Sim.GameDataUpdate*) (void*) num3;
          Grid.elementIdx = gameDataUpdatePtr->elementIdx;
          Grid.temperature = gameDataUpdatePtr->temperature;
          Grid.mass = gameDataUpdatePtr->mass;
          Grid.radiation = gameDataUpdatePtr->radiation;
          Grid.properties = gameDataUpdatePtr->properties;
          Grid.strengthInfo = gameDataUpdatePtr->strengthInfo;
          Grid.insulation = gameDataUpdatePtr->insulation;
          Grid.diseaseIdx = gameDataUpdatePtr->diseaseIdx;
          Grid.diseaseCount = gameDataUpdatePtr->diseaseCount;
          Grid.AccumulatedFlowValues = gameDataUpdatePtr->accumulatedFlow;
          Grid.exposedToSunlight = (byte*) (void*) gameDataUpdatePtr->propertyTextureExposedToSunlight;
          for (int index4 = 0; index4 < gameDataUpdatePtr->numSubstanceChangeInfo; ++index4)
          {
            Sim.SubstanceChangeInfo substanceChangeInfo = gameDataUpdatePtr->substanceChangeInfo[index4];
            int cellIdx = substanceChangeInfo.cellIdx;
            cells[cellIdx].elementIdx = gameDataUpdatePtr->elementIdx[cellIdx];
            cells[cellIdx].insulation = gameDataUpdatePtr->insulation[cellIdx];
            cells[cellIdx].properties = gameDataUpdatePtr->properties[cellIdx];
            cells[cellIdx].temperature = gameDataUpdatePtr->temperature[cellIdx];
            cells[cellIdx].mass = gameDataUpdatePtr->mass[cellIdx];
            cells[cellIdx].strengthInfo = gameDataUpdatePtr->strengthInfo[cellIdx];
            dcs[cellIdx].diseaseIdx = gameDataUpdatePtr->diseaseIdx[cellIdx];
            dcs[cellIdx].elementCount = gameDataUpdatePtr->diseaseCount[cellIdx];
            Grid.Element[cellIdx] = ElementLoader.elements[(int) substanceChangeInfo.newElemIdx];
          }
          for (int index5 = 0; index5 < gameDataUpdatePtr->numSolidInfo; ++index5)
          {
            Sim.SolidInfo solidInfo = gameDataUpdatePtr->solidInfo[index5];
            Grid.SetSolid(solidInfo.cellIdx, solidInfo.isSolid != 0, (CellSolidEvent) null);
          }
        }
      }
      int num5 = WorldGenSimUtil.SaveSim(settings, data, baseId, error_cb) ? 1 : 0;
      Sim.Shutdown();
      return num5 != 0;
    }

    private static bool SaveSim(
      WorldGenSettings settings,
      Klei.Data data,
      int baseId,
      Action<OfflineWorldGen.ErrorInfo> error_cb)
    {
      try
      {
        Manager.Clear();
        SimSaveFileStructure saveFileStructure = new SimSaveFileStructure();
        for (int index = 0; index < data.overworldCells.Count; ++index)
          saveFileStructure.worldDetail.overworldCells.Add(new WorldDetailSave.OverworldCell(SettingsCache.GetCachedSubWorld(((ProcGen.Node) data.overworldCells[index].node).type).zoneType, data.overworldCells[index]));
        saveFileStructure.worldDetail.globalWorldSeed = data.globalWorldSeed;
        saveFileStructure.worldDetail.globalWorldLayoutSeed = data.globalWorldLayoutSeed;
        saveFileStructure.worldDetail.globalTerrainSeed = data.globalTerrainSeed;
        saveFileStructure.worldDetail.globalNoiseSeed = data.globalNoiseSeed;
        saveFileStructure.WidthInCells = Grid.WidthInCells;
        saveFileStructure.HeightInCells = Grid.HeightInCells;
        saveFileStructure.x = data.world.offset.x;
        saveFileStructure.y = data.world.offset.y;
        using (MemoryStream output = new MemoryStream())
        {
          using (BinaryWriter writer = new BinaryWriter((Stream) output))
            Sim.Save(writer, saveFileStructure.x, saveFileStructure.y);
          saveFileStructure.Sim = output.ToArray();
        }
        using (MemoryStream output = new MemoryStream())
        {
          using (BinaryWriter binaryWriter = new BinaryWriter((Stream) output))
          {
            try
            {
              Serializer.Serialize((object) saveFileStructure, binaryWriter);
            }
            catch (Exception ex)
            {
              DebugUtil.LogErrorArgs(new object[3]
              {
                (object) "Couldn't serialize",
                (object) ex.Message,
                (object) ex.StackTrace
              });
            }
          }
          using (BinaryWriter binaryWriter = new BinaryWriter((Stream) File.Open(WorldGen.GetSIMSaveFilename(baseId), FileMode.Create)))
          {
            Manager.SerializeDirectory(binaryWriter);
            binaryWriter.Write(output.ToArray());
          }
        }
        return true;
      }
      catch (Exception ex)
      {
        error_cb(new OfflineWorldGen.ErrorInfo()
        {
          errorDesc = string.Format((string) UI.FRONTEND.SUPPORTWARNINGS.SAVE_DIRECTORY_READ_ONLY, (object) WorldGen.GetSIMSaveFilename(baseId)),
          exception = ex
        });
        DebugUtil.LogErrorArgs(new object[3]
        {
          (object) "Couldn't write",
          (object) ex.Message,
          (object) ex.StackTrace
        });
        return false;
      }
    }

    public static void LoadSim(int baseCount, List<SimSaveFileStructure> loadedWorlds)
    {
      for (int baseID = 0; baseID != baseCount; ++baseID)
      {
        SimSaveFileStructure saveFileStructure = new SimSaveFileStructure();
        try
        {
          FastReader fastReader = new FastReader(File.ReadAllBytes(WorldGen.GetSIMSaveFilename(baseID)));
          Manager.DeserializeDirectory((IReader) fastReader);
          Deserializer.Deserialize((object) saveFileStructure, (IReader) fastReader);
        }
        catch (Exception ex)
        {
          DebugUtil.LogErrorArgs(new object[3]
          {
            (object) "LoadSim Error!\n",
            (object) ex.Message,
            (object) ex.StackTrace
          });
          break;
        }
        if (saveFileStructure.worldDetail == null)
          Debug.LogError((object) ("Detail is null for world " + baseID.ToString()));
        else
          loadedWorlds.Add(saveFileStructure);
      }
    }
  }
}
