// Decompiled with JetBrains decompiler
// Type: RocketStats
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using TUNING;
using UnityEngine;

public class RocketStats
{
  private CommandModule commandModule;
  public static Dictionary<Tag, float> oxidizerEfficiencies;

  public RocketStats(CommandModule commandModule) => this.commandModule = commandModule;

  public float GetRocketMaxDistance()
  {
    double totalMass = (double) this.GetTotalMass();
    return Mathf.Max(0.0f, this.GetTotalThrust() - ROCKETRY.CalculateMassWithPenalty((float) totalMass));
  }

  public float GetTotalMass() => this.GetDryMass() + this.GetWetMass();

  public float GetDryMass()
  {
    float dryMass = 0.0f;
    foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(((Component) this.commandModule).GetComponent<AttachableBuilding>()))
    {
      RocketModule component = gameObject.GetComponent<RocketModule>();
      if (Object.op_Inequality((Object) component, (Object) null))
        dryMass += ((Component) component).GetComponent<PrimaryElement>().Mass;
    }
    return dryMass;
  }

  public float GetWetMass()
  {
    float wetMass = 0.0f;
    foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(((Component) this.commandModule).GetComponent<AttachableBuilding>()))
    {
      RocketModule component1 = gameObject.GetComponent<RocketModule>();
      if (Object.op_Inequality((Object) component1, (Object) null))
      {
        FuelTank component2 = ((Component) component1).GetComponent<FuelTank>();
        OxidizerTank component3 = ((Component) component1).GetComponent<OxidizerTank>();
        SolidBooster component4 = ((Component) component1).GetComponent<SolidBooster>();
        if (Object.op_Inequality((Object) component2, (Object) null))
          wetMass += component2.storage.MassStored();
        if (Object.op_Inequality((Object) component3, (Object) null))
          wetMass += component3.storage.MassStored();
        if (Object.op_Inequality((Object) component4, (Object) null))
          wetMass += component4.fuelStorage.MassStored();
      }
    }
    return wetMass;
  }

  public Tag GetEngineFuelTag()
  {
    RocketEngine mainEngine = this.GetMainEngine();
    return Object.op_Inequality((Object) mainEngine, (Object) null) ? mainEngine.fuelTag : Tag.op_Implicit((string) null);
  }

  public float GetTotalFuel(bool includeBoosters = false)
  {
    float totalFuel = 0.0f;
    foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(((Component) this.commandModule).GetComponent<AttachableBuilding>()))
    {
      FuelTank component1 = gameObject.GetComponent<FuelTank>();
      Tag engineFuelTag = this.GetEngineFuelTag();
      if (Object.op_Inequality((Object) component1, (Object) null))
        totalFuel += component1.storage.GetAmountAvailable(engineFuelTag);
      if (includeBoosters)
      {
        SolidBooster component2 = gameObject.GetComponent<SolidBooster>();
        if (Object.op_Inequality((Object) component2, (Object) null))
          totalFuel += component2.fuelStorage.GetAmountAvailable(component2.fuelTag);
      }
    }
    return totalFuel;
  }

  public float GetTotalOxidizer(bool includeBoosters = false)
  {
    float totalOxidizer = 0.0f;
    foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(((Component) this.commandModule).GetComponent<AttachableBuilding>()))
    {
      OxidizerTank component1 = gameObject.GetComponent<OxidizerTank>();
      if (Object.op_Inequality((Object) component1, (Object) null))
        totalOxidizer += component1.GetTotalOxidizerAvailable();
      if (includeBoosters)
      {
        SolidBooster component2 = gameObject.GetComponent<SolidBooster>();
        if (Object.op_Inequality((Object) component2, (Object) null))
          totalOxidizer += component2.fuelStorage.GetAmountAvailable(GameTags.OxyRock);
      }
    }
    return totalOxidizer;
  }

  public float GetAverageOxidizerEfficiency()
  {
    Dictionary<Tag, float> dictionary = new Dictionary<Tag, float>();
    dictionary[SimHashes.LiquidOxygen.CreateTag()] = 0.0f;
    dictionary[SimHashes.OxyRock.CreateTag()] = 0.0f;
    foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(((Component) this.commandModule).GetComponent<AttachableBuilding>()))
    {
      OxidizerTank component = gameObject.GetComponent<OxidizerTank>();
      if (Object.op_Inequality((Object) component, (Object) null))
      {
        foreach (KeyValuePair<Tag, float> keyValuePair in component.GetOxidizersAvailable())
        {
          if (dictionary.ContainsKey(keyValuePair.Key))
            dictionary[keyValuePair.Key] += keyValuePair.Value;
        }
      }
    }
    float num1 = 0.0f;
    float num2 = 0.0f;
    foreach (KeyValuePair<Tag, float> keyValuePair in dictionary)
    {
      num1 += keyValuePair.Value * RocketStats.oxidizerEfficiencies[keyValuePair.Key];
      num2 += keyValuePair.Value;
    }
    return (double) num2 == 0.0 ? 0.0f : (float) ((double) num1 / (double) num2 * 100.0);
  }

  public float GetTotalThrust()
  {
    float totalFuel = this.GetTotalFuel();
    float totalOxidizer = this.GetTotalOxidizer();
    float oxidizerEfficiency = this.GetAverageOxidizerEfficiency();
    RocketEngine mainEngine = this.GetMainEngine();
    return Object.op_Equality((Object) mainEngine, (Object) null) ? 0.0f : (mainEngine.requireOxidizer ? Mathf.Min(totalFuel, totalOxidizer) * (mainEngine.efficiency * (oxidizerEfficiency / 100f)) : totalFuel * mainEngine.efficiency) + this.GetBoosterThrust();
  }

  public float GetBoosterThrust()
  {
    float boosterThrust = 0.0f;
    foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(((Component) this.commandModule).GetComponent<AttachableBuilding>()))
    {
      SolidBooster component = gameObject.GetComponent<SolidBooster>();
      if (Object.op_Inequality((Object) component, (Object) null))
      {
        float amountAvailable1 = component.fuelStorage.GetAmountAvailable(ElementLoader.FindElementByHash(SimHashes.OxyRock).tag);
        float amountAvailable2 = component.fuelStorage.GetAmountAvailable(ElementLoader.FindElementByHash(SimHashes.Iron).tag);
        boosterThrust += component.efficiency * Mathf.Min(amountAvailable1, amountAvailable2);
      }
    }
    return boosterThrust;
  }

  public float GetEngineEfficiency()
  {
    RocketEngine mainEngine = this.GetMainEngine();
    return Object.op_Inequality((Object) mainEngine, (Object) null) ? mainEngine.efficiency : 0.0f;
  }

  public RocketEngine GetMainEngine()
  {
    RocketEngine mainEngine = (RocketEngine) null;
    foreach (GameObject gameObject in AttachableBuilding.GetAttachedNetwork(((Component) this.commandModule).GetComponent<AttachableBuilding>()))
    {
      mainEngine = gameObject.GetComponent<RocketEngine>();
      if (Object.op_Inequality((Object) mainEngine, (Object) null))
      {
        if (mainEngine.mainEngine)
          break;
      }
    }
    return mainEngine;
  }

  public float GetTotalOxidizableFuel() => Mathf.Min(this.GetTotalFuel(), this.GetTotalOxidizer());

  static RocketStats()
  {
    Dictionary<Tag, float> dictionary = new Dictionary<Tag, float>();
    dictionary.Add(SimHashes.OxyRock.CreateTag(), ROCKETRY.OXIDIZER_EFFICIENCY.LOW);
    dictionary.Add(SimHashes.LiquidOxygen.CreateTag(), ROCKETRY.OXIDIZER_EFFICIENCY.HIGH);
    RocketStats.oxidizerEfficiencies = dictionary;
  }
}
