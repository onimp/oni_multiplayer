// Decompiled with JetBrains decompiler
// Type: EnergySim
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

public class EnergySim
{
  private HashSet<Generator> generators = new HashSet<Generator>();
  private HashSet<ManualGenerator> manualGenerators = new HashSet<ManualGenerator>();
  private HashSet<Battery> batteries = new HashSet<Battery>();
  private HashSet<EnergyConsumer> energyConsumers = new HashSet<EnergyConsumer>();

  public HashSet<Generator> Generators => this.generators;

  public void AddGenerator(Generator generator) => this.generators.Add(generator);

  public void RemoveGenerator(Generator generator) => this.generators.Remove(generator);

  public void AddManualGenerator(ManualGenerator manual_generator) => this.manualGenerators.Add(manual_generator);

  public void RemoveManualGenerator(ManualGenerator manual_generator) => this.manualGenerators.Remove(manual_generator);

  public void AddBattery(Battery battery) => this.batteries.Add(battery);

  public void RemoveBattery(Battery battery) => this.batteries.Remove(battery);

  public void AddEnergyConsumer(EnergyConsumer energy_consumer) => this.energyConsumers.Add(energy_consumer);

  public void RemoveEnergyConsumer(EnergyConsumer energy_consumer) => this.energyConsumers.Remove(energy_consumer);

  public void EnergySim200ms(float dt)
  {
    foreach (Generator generator in this.generators)
      generator.EnergySim200ms(dt);
    foreach (ManualGenerator manualGenerator in this.manualGenerators)
      manualGenerator.EnergySim200ms(dt);
    foreach (Battery battery in this.batteries)
      battery.EnergySim200ms(dt);
    foreach (EnergyConsumer energyConsumer in this.energyConsumers)
      energyConsumer.EnergySim200ms(dt);
  }
}
