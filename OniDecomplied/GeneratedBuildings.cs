// Decompiled with JetBrains decompiler
// Type: GeneratedBuildings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class GeneratedBuildings
{
  public static void LoadGeneratedBuildings(List<System.Type> types)
  {
    System.Type type1 = typeof (IBuildingConfig);
    List<System.Type> typeList = new List<System.Type>();
    foreach (System.Type type2 in types)
    {
      if (type1.IsAssignableFrom(type2) && !type2.IsAbstract && !type2.IsInterface)
        typeList.Add(type2);
    }
    foreach (System.Type type3 in typeList)
    {
      object instance = Activator.CreateInstance(type3);
      try
      {
        BuildingConfigManager.Instance.RegisterBuilding(instance as IBuildingConfig);
      }
      catch (Exception ex)
      {
        DebugUtil.LogException((Object) null, "Exception in RegisterBuilding for type " + type3.FullName + " from " + type3.Assembly.GetName().Name, ex);
      }
    }
    foreach (PlanScreen.PlanInfo planInfo in BUILDINGS.PLANORDER)
    {
      List<string> stringList1 = new List<string>();
      foreach (KeyValuePair<string, string> keyValuePair in planInfo.buildingAndSubcategoryData)
      {
        if (Object.op_Equality((Object) Assets.GetBuildingDef(keyValuePair.Key), (Object) null))
          stringList1.Add(keyValuePair.Key);
      }
      foreach (string str in stringList1)
      {
        string entry = str;
        planInfo.buildingAndSubcategoryData.RemoveAll((Predicate<KeyValuePair<string, string>>) (match => match.Key == entry));
      }
      List<string> stringList2 = new List<string>();
      foreach (string str in planInfo.data)
      {
        string entry = str;
        if (planInfo.buildingAndSubcategoryData.FindIndex((Predicate<KeyValuePair<string, string>>) (x => x.Key == entry)) == -1 && Object.op_Inequality((Object) Assets.GetBuildingDef(entry), (Object) null))
        {
          Debug.LogWarning((object) ("Mod: Building '" + entry + "' was not added properly to PlanInfo, use ModUtil.AddBuildingToPlanScreen instead."));
          stringList2.Add(entry);
        }
      }
      foreach (string building_id in stringList2)
        ModUtil.AddBuildingToPlanScreen(planInfo.category, building_id, "uncategorized");
    }
  }

  public static void MakeBuildingAlwaysOperational(GameObject go)
  {
    BuildingDef def = go.GetComponent<BuildingComplete>().Def;
    if (def.LogicInputPorts != null || def.LogicOutputPorts != null)
      Debug.LogWarning((object) "Do not call MakeBuildingAlwaysOperational directly if LogicInputPorts or LogicOutputPorts are defined. Instead set BuildingDef.AlwaysOperational = true");
    GeneratedBuildings.MakeBuildingAlwaysOperationalImpl(go);
  }

  public static void RemoveLoopingSounds(GameObject go) => Object.DestroyImmediate((Object) go.GetComponent<LoopingSounds>());

  public static void RemoveDefaultLogicPorts(GameObject go) => Object.DestroyImmediate((Object) go.GetComponent<LogicPorts>());

  public static void RegisterWithOverlay(HashSet<Tag> overlay_tags, string id)
  {
    overlay_tags.Add(new Tag(id));
    overlay_tags.Add(new Tag(id + "UnderConstruction"));
  }

  public static void RegisterSingleLogicInputPort(GameObject go)
  {
    LogicPorts logicPorts = go.AddOrGet<LogicPorts>();
    logicPorts.inputPortInfo = LogicOperationalController.CreateSingleInputPortList(new CellOffset(0, 0)).ToArray();
    logicPorts.outputPortInfo = (LogicPorts.Port[]) null;
  }

  private static void MakeBuildingAlwaysOperationalImpl(GameObject go)
  {
    Object.DestroyImmediate((Object) go.GetComponent<BuildingEnabledButton>());
    Object.DestroyImmediate((Object) go.GetComponent<Operational>());
    Object.DestroyImmediate((Object) go.GetComponent<LogicPorts>());
  }

  public static void InitializeLogicPorts(GameObject go, BuildingDef def)
  {
    if (def.AlwaysOperational)
      GeneratedBuildings.MakeBuildingAlwaysOperationalImpl(go);
    if (def.LogicInputPorts != null)
      go.AddOrGet<LogicPorts>().inputPortInfo = def.LogicInputPorts.ToArray();
    if (def.LogicOutputPorts == null)
      return;
    go.AddOrGet<LogicPorts>().outputPortInfo = def.LogicOutputPorts.ToArray();
  }

  public static void InitializeHighEnergyParticlePorts(GameObject go, BuildingDef def)
  {
    if (!def.UseHighEnergyParticleInputPort && !def.UseHighEnergyParticleOutputPort)
      return;
    HighEnergyParticlePort energyParticlePort = go.AddOrGet<HighEnergyParticlePort>();
    energyParticlePort.particleInputOffset = def.HighEnergyParticleInputOffset;
    energyParticlePort.particleOutputOffset = def.HighEnergyParticleOutputOffset;
    energyParticlePort.particleInputEnabled = def.UseHighEnergyParticleInputPort;
    energyParticlePort.particleOutputEnabled = def.UseHighEnergyParticleOutputPort;
  }
}
