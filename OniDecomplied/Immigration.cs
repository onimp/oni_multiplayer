// Decompiled with JetBrains decompiler
// Type: Immigration
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using KSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("KMonoBehaviour/scripts/Immigration")]
public class Immigration : KMonoBehaviour, ISaveLoadable, ISim200ms, IPersonalPriorityManager
{
  public float[] spawnInterval;
  public int[] spawnTable;
  [Serialize]
  private Dictionary<HashedString, int> defaultPersonalPriorities = new Dictionary<HashedString, int>();
  [Serialize]
  public float timeBeforeSpawn = float.PositiveInfinity;
  [Serialize]
  private bool bImmigrantAvailable;
  [Serialize]
  private int spawnIdx;
  private CarePackageInfo[] carePackages;
  public static Immigration Instance;
  private const int CYCLE_THRESHOLD_A = 6;
  private const int CYCLE_THRESHOLD_B = 12;
  private const int CYCLE_THRESHOLD_C = 24;
  private const int CYCLE_THRESHOLD_D = 48;
  public const string FACADE_SELECT_RANDOM = "SELECTRANDOM";

  public static void DestroyInstance() => Immigration.Instance = (Immigration) null;

  protected virtual void OnPrefabInit()
  {
    this.bImmigrantAvailable = false;
    Immigration.Instance = this;
    this.timeBeforeSpawn = this.spawnInterval[Math.Min(this.spawnIdx, this.spawnInterval.Length - 1)];
    this.ResetPersonalPriorities();
    this.ConfigureCarePackages();
  }

  private void ConfigureCarePackages()
  {
    if (DlcManager.FeatureClusterSpaceEnabled())
      this.ConfigureMultiWorldCarePackages();
    else
      this.ConfigureBaseGameCarePackages();
  }

  private void ConfigureBaseGameCarePackages() => this.carePackages = new CarePackageInfo[59]
  {
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.SandStone).tag.ToString(), 1000f, (Func<bool>) null),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Dirt).tag.ToString(), 500f, (Func<bool>) null),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Algae).tag.ToString(), 500f, (Func<bool>) null),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.OxyRock).tag.ToString(), 100f, (Func<bool>) null),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Water).tag.ToString(), 2000f, (Func<bool>) null),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Sand).tag.ToString(), 3000f, (Func<bool>) null),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Carbon).tag.ToString(), 3000f, (Func<bool>) null),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Fertilizer).tag.ToString(), 3000f, (Func<bool>) null),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Ice).tag.ToString(), 4000f, (Func<bool>) (() => this.CycleCondition(12))),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Brine).tag.ToString(), 2000f, (Func<bool>) null),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.SaltWater).tag.ToString(), 2000f, (Func<bool>) null),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Rust).tag.ToString(), 1000f, (Func<bool>) null),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Cuprite).tag.ToString(), 2000f, (Func<bool>) (() => this.CycleCondition(12) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Cuprite).tag))),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.GoldAmalgam).tag.ToString(), 2000f, (Func<bool>) (() => this.CycleCondition(12) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.GoldAmalgam).tag))),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Copper).tag.ToString(), 400f, (Func<bool>) (() => this.CycleCondition(24) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Copper).tag))),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Iron).tag.ToString(), 400f, (Func<bool>) (() => this.CycleCondition(24) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Iron).tag))),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Lime).tag.ToString(), 150f, (Func<bool>) (() => this.CycleCondition(48) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Lime).tag))),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Polypropylene).tag.ToString(), 500f, (Func<bool>) (() => this.CycleCondition(48) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Polypropylene).tag))),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Glass).tag.ToString(), 200f, (Func<bool>) (() => this.CycleCondition(48) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Glass).tag))),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Steel).tag.ToString(), 100f, (Func<bool>) (() => this.CycleCondition(48) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Steel).tag))),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Ethanol).tag.ToString(), 100f, (Func<bool>) (() => this.CycleCondition(48) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Ethanol).tag))),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.AluminumOre).tag.ToString(), 100f, (Func<bool>) (() => this.CycleCondition(48) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.AluminumOre).tag))),
    new CarePackageInfo("PrickleGrassSeed", 3f, (Func<bool>) null),
    new CarePackageInfo("LeafyPlantSeed", 3f, (Func<bool>) null),
    new CarePackageInfo("CactusPlantSeed", 3f, (Func<bool>) null),
    new CarePackageInfo("MushroomSeed", 1f, (Func<bool>) null),
    new CarePackageInfo("PrickleFlowerSeed", 2f, (Func<bool>) null),
    new CarePackageInfo("OxyfernSeed", 1f, (Func<bool>) null),
    new CarePackageInfo("ForestTreeSeed", 1f, (Func<bool>) null),
    new CarePackageInfo(BasicFabricMaterialPlantConfig.SEED_ID, 3f, (Func<bool>) (() => this.CycleCondition(24))),
    new CarePackageInfo("SwampLilySeed", 1f, (Func<bool>) (() => this.CycleCondition(24))),
    new CarePackageInfo("ColdBreatherSeed", 1f, (Func<bool>) (() => this.CycleCondition(24))),
    new CarePackageInfo("SpiceVineSeed", 1f, (Func<bool>) (() => this.CycleCondition(24))),
    new CarePackageInfo("FieldRation", 5f, (Func<bool>) null),
    new CarePackageInfo("BasicForagePlant", 6f, (Func<bool>) null),
    new CarePackageInfo("CookedEgg", 3f, (Func<bool>) (() => this.CycleCondition(6))),
    new CarePackageInfo(PrickleFruitConfig.ID, 3f, (Func<bool>) (() => this.CycleCondition(12))),
    new CarePackageInfo("FriedMushroom", 3f, (Func<bool>) (() => this.CycleCondition(24))),
    new CarePackageInfo("CookedMeat", 3f, (Func<bool>) (() => this.CycleCondition(48))),
    new CarePackageInfo("SpicyTofu", 3f, (Func<bool>) (() => this.CycleCondition(48))),
    new CarePackageInfo("LightBugBaby", 1f, (Func<bool>) null),
    new CarePackageInfo("HatchBaby", 1f, (Func<bool>) null),
    new CarePackageInfo("PuftBaby", 1f, (Func<bool>) null),
    new CarePackageInfo("SquirrelBaby", 1f, (Func<bool>) null),
    new CarePackageInfo("CrabBaby", 1f, (Func<bool>) null),
    new CarePackageInfo("DreckoBaby", 1f, (Func<bool>) (() => this.CycleCondition(24))),
    new CarePackageInfo("Pacu", 8f, (Func<bool>) (() => this.CycleCondition(24))),
    new CarePackageInfo("MoleBaby", 1f, (Func<bool>) (() => this.CycleCondition(48))),
    new CarePackageInfo("OilfloaterBaby", 1f, (Func<bool>) (() => this.CycleCondition(48))),
    new CarePackageInfo("LightBugEgg", 3f, (Func<bool>) null),
    new CarePackageInfo("HatchEgg", 3f, (Func<bool>) null),
    new CarePackageInfo("PuftEgg", 3f, (Func<bool>) null),
    new CarePackageInfo("OilfloaterEgg", 3f, (Func<bool>) (() => this.CycleCondition(12))),
    new CarePackageInfo("MoleEgg", 3f, (Func<bool>) (() => this.CycleCondition(24))),
    new CarePackageInfo("DreckoEgg", 3f, (Func<bool>) (() => this.CycleCondition(24))),
    new CarePackageInfo("SquirrelEgg", 2f, (Func<bool>) null),
    new CarePackageInfo("BasicCure", 3f, (Func<bool>) null),
    new CarePackageInfo("CustomClothing", 1f, (Func<bool>) null, "SELECTRANDOM"),
    new CarePackageInfo("Funky_Vest", 1f, (Func<bool>) null)
  };

  private void ConfigureMultiWorldCarePackages() => this.carePackages = new CarePackageInfo[66]
  {
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.SandStone).tag.ToString(), 1000f, (Func<bool>) null),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Dirt).tag.ToString(), 500f, (Func<bool>) null),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Algae).tag.ToString(), 500f, (Func<bool>) null),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.OxyRock).tag.ToString(), 100f, (Func<bool>) null),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Water).tag.ToString(), 2000f, (Func<bool>) null),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Sand).tag.ToString(), 3000f, (Func<bool>) null),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Carbon).tag.ToString(), 3000f, (Func<bool>) null),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Fertilizer).tag.ToString(), 3000f, (Func<bool>) null),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Ice).tag.ToString(), 4000f, (Func<bool>) (() => this.CycleCondition(12))),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Brine).tag.ToString(), 2000f, (Func<bool>) null),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.SaltWater).tag.ToString(), 2000f, (Func<bool>) null),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Rust).tag.ToString(), 1000f, (Func<bool>) null),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Cuprite).tag.ToString(), 2000f, (Func<bool>) (() => this.CycleCondition(12) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Cuprite).tag))),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.GoldAmalgam).tag.ToString(), 2000f, (Func<bool>) (() => this.CycleCondition(12) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.GoldAmalgam).tag))),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Copper).tag.ToString(), 400f, (Func<bool>) (() => this.CycleCondition(24) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Copper).tag))),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Iron).tag.ToString(), 400f, (Func<bool>) (() => this.CycleCondition(24) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Iron).tag))),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Lime).tag.ToString(), 150f, (Func<bool>) (() => this.CycleCondition(48) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Lime).tag))),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Polypropylene).tag.ToString(), 500f, (Func<bool>) (() => this.CycleCondition(48) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Polypropylene).tag))),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Glass).tag.ToString(), 200f, (Func<bool>) (() => this.CycleCondition(48) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Glass).tag))),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Steel).tag.ToString(), 100f, (Func<bool>) (() => this.CycleCondition(48) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Steel).tag))),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.Ethanol).tag.ToString(), 100f, (Func<bool>) (() => this.CycleCondition(48) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.Ethanol).tag))),
    new CarePackageInfo(ElementLoader.FindElementByHash(SimHashes.AluminumOre).tag.ToString(), 100f, (Func<bool>) (() => this.CycleCondition(48) && this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.AluminumOre).tag))),
    new CarePackageInfo("PrickleGrassSeed", 3f, (Func<bool>) null),
    new CarePackageInfo("LeafyPlantSeed", 3f, (Func<bool>) null),
    new CarePackageInfo("CactusPlantSeed", 3f, (Func<bool>) null),
    new CarePackageInfo("MushroomSeed", 1f, (Func<bool>) (() => this.DiscoveredCondition(ElementLoader.FindElementByHash(SimHashes.SlimeMold).tag))),
    new CarePackageInfo("PrickleFlowerSeed", 2f, (Func<bool>) (() => this.DiscoveredCondition(Tag.op_Implicit("PrickleFlowerSeed")))),
    new CarePackageInfo("OxyfernSeed", 1f, (Func<bool>) null),
    new CarePackageInfo("ForestTreeSeed", 1f, (Func<bool>) (() => this.DiscoveredCondition(Tag.op_Implicit("ForestTreeSeed")))),
    new CarePackageInfo(BasicFabricMaterialPlantConfig.SEED_ID, 3f, (Func<bool>) (() => this.CycleCondition(24) && this.DiscoveredCondition(Tag.op_Implicit(BasicFabricMaterialPlantConfig.SEED_ID)))),
    new CarePackageInfo("SwampLilySeed", 1f, (Func<bool>) (() => this.CycleCondition(24) && this.DiscoveredCondition(Tag.op_Implicit("SwampLilySeed")))),
    new CarePackageInfo("ColdBreatherSeed", 1f, (Func<bool>) (() => this.CycleCondition(24) && this.DiscoveredCondition(Tag.op_Implicit("ColdBreatherSeed")))),
    new CarePackageInfo("SpiceVineSeed", 1f, (Func<bool>) (() => this.CycleCondition(24) && this.DiscoveredCondition(Tag.op_Implicit("SpiceVineSeed")))),
    new CarePackageInfo("FieldRation", 5f, (Func<bool>) null),
    new CarePackageInfo("BasicForagePlant", 6f, (Func<bool>) (() => this.DiscoveredCondition(Tag.op_Implicit("BasicForagePlant")))),
    new CarePackageInfo("ForestForagePlant", 2f, (Func<bool>) (() => this.DiscoveredCondition(Tag.op_Implicit("ForestForagePlant")))),
    new CarePackageInfo("SwampForagePlant", 2f, (Func<bool>) (() => this.DiscoveredCondition(Tag.op_Implicit("SwampForagePlant")))),
    new CarePackageInfo("CookedEgg", 3f, (Func<bool>) (() => this.CycleCondition(6))),
    new CarePackageInfo(PrickleFruitConfig.ID, 3f, (Func<bool>) (() => this.CycleCondition(12) && this.DiscoveredCondition(Tag.op_Implicit(PrickleFruitConfig.ID)))),
    new CarePackageInfo("FriedMushroom", 3f, (Func<bool>) (() => this.CycleCondition(24) && this.DiscoveredCondition(Tag.op_Implicit("FriedMushroom")))),
    new CarePackageInfo("CookedMeat", 3f, (Func<bool>) (() => this.CycleCondition(48))),
    new CarePackageInfo("SpicyTofu", 3f, (Func<bool>) (() => this.CycleCondition(48) && this.DiscoveredCondition(Tag.op_Implicit("SpicyTofu")))),
    new CarePackageInfo("WormSuperFood", 2f, (Func<bool>) (() => this.DiscoveredCondition(Tag.op_Implicit("WormPlantSeed")))),
    new CarePackageInfo("LightBugBaby", 1f, (Func<bool>) (() => this.DiscoveredCondition(Tag.op_Implicit("LightBugEgg")))),
    new CarePackageInfo("HatchBaby", 1f, (Func<bool>) (() => this.DiscoveredCondition(Tag.op_Implicit("HatchEgg")))),
    new CarePackageInfo("PuftBaby", 1f, (Func<bool>) (() => this.DiscoveredCondition(Tag.op_Implicit("PuftEgg")))),
    new CarePackageInfo("SquirrelBaby", 1f, (Func<bool>) (() => this.DiscoveredCondition(Tag.op_Implicit("SquirrelEgg")) || this.CycleCondition(24))),
    new CarePackageInfo("CrabBaby", 1f, (Func<bool>) (() => this.DiscoveredCondition(Tag.op_Implicit("CrabEgg")))),
    new CarePackageInfo("DreckoBaby", 1f, (Func<bool>) (() => this.CycleCondition(24) && this.DiscoveredCondition(Tag.op_Implicit("DreckoEgg")))),
    new CarePackageInfo("Pacu", 8f, (Func<bool>) (() => this.CycleCondition(24) && this.DiscoveredCondition(Tag.op_Implicit("PacuEgg")))),
    new CarePackageInfo("MoleBaby", 1f, (Func<bool>) (() => this.CycleCondition(48) && this.DiscoveredCondition(Tag.op_Implicit("MoleEgg")))),
    new CarePackageInfo("OilfloaterBaby", 1f, (Func<bool>) (() => this.CycleCondition(48) && this.DiscoveredCondition(Tag.op_Implicit("OilfloaterEgg")))),
    new CarePackageInfo("DivergentBeetleBaby", 1f, (Func<bool>) (() => this.CycleCondition(48) && this.DiscoveredCondition(Tag.op_Implicit("DivergentBeetleEgg")))),
    new CarePackageInfo("StaterpillarBaby", 1f, (Func<bool>) (() => this.CycleCondition(48) && this.DiscoveredCondition(Tag.op_Implicit("StaterpillarEgg")))),
    new CarePackageInfo("LightBugEgg", 3f, (Func<bool>) (() => this.DiscoveredCondition(Tag.op_Implicit("LightBugEgg")))),
    new CarePackageInfo("HatchEgg", 3f, (Func<bool>) (() => this.DiscoveredCondition(Tag.op_Implicit("HatchEgg")))),
    new CarePackageInfo("PuftEgg", 3f, (Func<bool>) (() => this.DiscoveredCondition(Tag.op_Implicit("PuftEgg")))),
    new CarePackageInfo("OilfloaterEgg", 3f, (Func<bool>) (() => this.CycleCondition(12) && this.DiscoveredCondition(Tag.op_Implicit("OilfloaterEgg")))),
    new CarePackageInfo("MoleEgg", 3f, (Func<bool>) (() => this.CycleCondition(24) && this.DiscoveredCondition(Tag.op_Implicit("MoleEgg")))),
    new CarePackageInfo("DreckoEgg", 3f, (Func<bool>) (() => this.CycleCondition(24) && this.DiscoveredCondition(Tag.op_Implicit("DreckoEgg")))),
    new CarePackageInfo("SquirrelEgg", 2f, (Func<bool>) (() => this.DiscoveredCondition(Tag.op_Implicit("SquirrelEgg")) || this.CycleCondition(24))),
    new CarePackageInfo("DivergentBeetleEgg", 2f, (Func<bool>) (() => this.CycleCondition(48) && this.DiscoveredCondition(Tag.op_Implicit("DivergentBeetleEgg")))),
    new CarePackageInfo("StaterpillarEgg", 2f, (Func<bool>) (() => this.CycleCondition(48) && this.DiscoveredCondition(Tag.op_Implicit("StaterpillarEgg")))),
    new CarePackageInfo("BasicCure", 3f, (Func<bool>) null),
    new CarePackageInfo("CustomClothing", 1f, (Func<bool>) null, "SELECTRANDOM"),
    new CarePackageInfo("Funky_Vest", 1f, (Func<bool>) null)
  };

  private bool CycleCondition(int cycle) => GameClock.Instance.GetCycle() >= cycle;

  private bool DiscoveredCondition(Tag tag) => DiscoveredResources.Instance.IsDiscovered(tag);

  public bool ImmigrantsAvailable => this.bImmigrantAvailable;

  public int EndImmigration()
  {
    this.bImmigrantAvailable = false;
    ++this.spawnIdx;
    int index = Math.Min(this.spawnIdx, this.spawnInterval.Length - 1);
    this.timeBeforeSpawn = this.spawnInterval[index];
    return this.spawnTable[index];
  }

  public float GetTimeRemaining() => this.timeBeforeSpawn;

  public float GetTotalWaitTime() => this.spawnInterval[Math.Min(this.spawnIdx, this.spawnInterval.Length - 1)];

  public void Sim200ms(float dt)
  {
    if (this.IsHalted() || this.bImmigrantAvailable)
      return;
    this.timeBeforeSpawn -= dt;
    this.timeBeforeSpawn = Math.Max(this.timeBeforeSpawn, 0.0f);
    if ((double) this.timeBeforeSpawn > 0.0)
      return;
    this.bImmigrantAvailable = true;
  }

  private bool IsHalted()
  {
    foreach (Component component1 in Components.Telepads.Items)
    {
      Operational component2 = component1.GetComponent<Operational>();
      if (Object.op_Inequality((Object) component2, (Object) null) && component2.IsOperational)
        return false;
    }
    return true;
  }

  public int GetPersonalPriority(ChoreGroup group)
  {
    int personalPriority;
    if (!this.defaultPersonalPriorities.TryGetValue(group.IdHash, out personalPriority))
      personalPriority = 3;
    return personalPriority;
  }

  public CarePackageInfo RandomCarePackage()
  {
    List<CarePackageInfo> carePackageInfoList = new List<CarePackageInfo>();
    foreach (CarePackageInfo carePackage in this.carePackages)
    {
      if (carePackage.requirement == null || carePackage.requirement())
        carePackageInfoList.Add(carePackage);
    }
    return carePackageInfoList[Random.Range(0, carePackageInfoList.Count)];
  }

  public void SetPersonalPriority(ChoreGroup group, int value) => this.defaultPersonalPriorities[group.IdHash] = value;

  public int GetAssociatedSkillLevel(ChoreGroup group) => 0;

  public void ApplyDefaultPersonalPriorities(GameObject minion)
  {
    IPersonalPriorityManager instance = (IPersonalPriorityManager) Immigration.Instance;
    IPersonalPriorityManager component = (IPersonalPriorityManager) minion.GetComponent<ChoreConsumer>();
    foreach (ChoreGroup resource in Db.Get().ChoreGroups.resources)
    {
      int personalPriority = instance.GetPersonalPriority(resource);
      component.SetPersonalPriority(resource, personalPriority);
    }
  }

  public void ResetPersonalPriorities()
  {
    bool personalPriorities = Game.Instance.advancedPersonalPriorities;
    foreach (ChoreGroup resource in Db.Get().ChoreGroups.resources)
      this.defaultPersonalPriorities[resource.IdHash] = personalPriorities ? resource.DefaultPersonalPriority : 3;
  }

  public bool IsChoreGroupDisabled(ChoreGroup g) => false;
}
