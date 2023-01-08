// Decompiled with JetBrains decompiler
// Type: HarvestablePOIConfig
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using UnityEngine;

public class HarvestablePOIConfig : IMultiEntityConfig
{
  public const string CarbonAsteroidField = "CarbonAsteroidField";
  public const string MetallicAsteroidField = "MetallicAsteroidField";
  public const string SatelliteField = "SatelliteField";
  public const string RockyAsteroidField = "RockyAsteroidField";
  public const string InterstellarIceField = "InterstellarIceField";
  public const string OrganicMassField = "OrganicMassField";
  public const string IceAsteroidField = "IceAsteroidField";
  public const string GasGiantCloud = "GasGiantCloud";
  public const string ChlorineCloud = "ChlorineCloud";
  public const string GildedAsteroidField = "GildedAsteroidField";
  public const string GlimmeringAsteroidField = "GlimmeringAsteroidField";
  public const string HeliumCloud = "HeliumCloud";
  public const string OilyAsteroidField = "OilyAsteroidField";
  public const string OxidizedAsteroidField = "OxidizedAsteroidField";
  public const string SaltyAsteroidField = "SaltyAsteroidField";
  public const string FrozenOreField = "FrozenOreField";
  public const string ForestyOreField = "ForestyOreField";
  public const string SwampyOreField = "SwampyOreField";
  public const string SandyOreField = "SandyOreField";
  public const string RadioactiveGasCloud = "RadioactiveGasCloud";
  public const string RadioactiveAsteroidField = "RadioactiveAsteroidField";
  public const string OxygenRichAsteroidField = "OxygenRichAsteroidField";
  public const string InterstellarOcean = "InterstellarOcean";
  private static readonly List<string> GasFieldOrbit = new List<string>()
  {
    Db.Get().OrbitalTypeCategories.iceCloud.Id,
    Db.Get().OrbitalTypeCategories.heliumCloud.Id,
    Db.Get().OrbitalTypeCategories.purpleGas.Id,
    Db.Get().OrbitalTypeCategories.radioactiveGas.Id
  };
  private static readonly List<string> AsteroidFieldOrbit = new List<string>()
  {
    Db.Get().OrbitalTypeCategories.iceRock.Id,
    Db.Get().OrbitalTypeCategories.frozenOre.Id,
    Db.Get().OrbitalTypeCategories.rocky.Id
  };

  public List<GameObject> CreatePrefabs()
  {
    List<GameObject> prefabs = new List<GameObject>();
    foreach (HarvestablePOIConfig.HarvestablePOIParams config in this.GenerateConfigs())
      prefabs.Add(HarvestablePOIConfig.CreateHarvestablePOI(config.id, config.anim, StringEntry.op_Implicit(Strings.Get(config.nameStringKey)), config.descStringKey, config.poiType.idHash, config.poiType.canProvideArtifacts));
    return prefabs;
  }

  public static GameObject CreateHarvestablePOI(
    string id,
    string anim,
    string name,
    StringKey descStringKey,
    HashedString poiType,
    bool canProvideArtifacts = false)
  {
    GameObject entity = EntityTemplates.CreateEntity(id, id);
    entity.AddOrGet<SaveLoadRoot>();
    entity.AddOrGet<HarvestablePOIConfigurator>().presetType = poiType;
    HarvestablePOIClusterGridEntity clusterGridEntity = entity.AddOrGet<HarvestablePOIClusterGridEntity>();
    clusterGridEntity.m_name = name;
    clusterGridEntity.m_Anim = anim;
    entity.AddOrGetDef<HarvestablePOIStates.Def>();
    if (canProvideArtifacts)
    {
      entity.AddOrGetDef<ArtifactPOIStates.Def>();
      entity.AddOrGet<ArtifactPOIConfigurator>().presetType = ArtifactPOIConfigurator.defaultArtifactPoiType.idHash;
    }
    entity.AddOrGet<InfoDescription>().description = StringEntry.op_Implicit(Strings.Get(descStringKey));
    return entity;
  }

  public void OnPrefabInit(GameObject inst)
  {
  }

  public void OnSpawn(GameObject inst)
  {
  }

  private List<HarvestablePOIConfig.HarvestablePOIParams> GenerateConfigs()
  {
    List<HarvestablePOIConfig.HarvestablePOIParams> configs = new List<HarvestablePOIConfig.HarvestablePOIParams>();
    configs.Add(new HarvestablePOIConfig.HarvestablePOIParams("cloud", new HarvestablePOIConfigurator.HarvestablePOIType("CarbonAsteroidField", new Dictionary<SimHashes, float>()
    {
      {
        SimHashes.RefinedCarbon,
        1.5f
      },
      {
        SimHashes.Carbon,
        5.5f
      }
    }, 30000f, 45000f, orbitalObject: HarvestablePOIConfig.AsteroidFieldOrbit)));
    configs.Add(new HarvestablePOIConfig.HarvestablePOIParams("metallic_asteroid_field", new HarvestablePOIConfigurator.HarvestablePOIType("MetallicAsteroidField", new Dictionary<SimHashes, float>()
    {
      {
        SimHashes.MoltenIron,
        1.25f
      },
      {
        SimHashes.Cuprite,
        1.75f
      },
      {
        SimHashes.Obsidian,
        7f
      }
    }, orbitalObject: HarvestablePOIConfig.AsteroidFieldOrbit)));
    configs.Add(new HarvestablePOIConfig.HarvestablePOIParams("satellite_field", new HarvestablePOIConfigurator.HarvestablePOIType("SatelliteField", new Dictionary<SimHashes, float>()
    {
      {
        SimHashes.Sand,
        3f
      },
      {
        SimHashes.IronOre,
        3f
      },
      {
        SimHashes.MoltenCopper,
        2.67f
      },
      {
        SimHashes.Glass,
        1.33f
      }
    }, 30000f, 45000f, orbitalObject: HarvestablePOIConfig.AsteroidFieldOrbit)));
    configs.Add(new HarvestablePOIConfig.HarvestablePOIParams("rocky_asteroid_field", new HarvestablePOIConfigurator.HarvestablePOIType("RockyAsteroidField", new Dictionary<SimHashes, float>()
    {
      {
        SimHashes.Cuprite,
        2f
      },
      {
        SimHashes.SedimentaryRock,
        4f
      },
      {
        SimHashes.IgneousRock,
        4f
      }
    }, orbitalObject: HarvestablePOIConfig.AsteroidFieldOrbit)));
    configs.Add(new HarvestablePOIConfig.HarvestablePOIParams("interstellar_ice_field", new HarvestablePOIConfigurator.HarvestablePOIType("InterstellarIceField", new Dictionary<SimHashes, float>()
    {
      {
        SimHashes.Ice,
        2.5f
      },
      {
        SimHashes.SolidCarbonDioxide,
        7f
      },
      {
        SimHashes.SolidOxygen,
        0.5f
      }
    }, orbitalObject: new List<string>()
    {
      Db.Get().OrbitalTypeCategories.iceCloud.Id,
      Db.Get().OrbitalTypeCategories.iceRock.Id
    })));
    configs.Add(new HarvestablePOIConfig.HarvestablePOIParams("organic_mass_field", new HarvestablePOIConfigurator.HarvestablePOIType("OrganicMassField", new Dictionary<SimHashes, float>()
    {
      {
        SimHashes.SlimeMold,
        3f
      },
      {
        SimHashes.Algae,
        3f
      },
      {
        SimHashes.ContaminatedOxygen,
        1f
      },
      {
        SimHashes.Dirt,
        3f
      }
    }, orbitalObject: HarvestablePOIConfig.AsteroidFieldOrbit)));
    configs.Add(new HarvestablePOIConfig.HarvestablePOIParams("ice_asteroid_field", new HarvestablePOIConfigurator.HarvestablePOIType("IceAsteroidField", new Dictionary<SimHashes, float>()
    {
      {
        SimHashes.Ice,
        6f
      },
      {
        SimHashes.SolidCarbonDioxide,
        2f
      },
      {
        SimHashes.Oxygen,
        1.5f
      },
      {
        SimHashes.SolidMethane,
        0.5f
      }
    }, orbitalObject: new List<string>()
    {
      Db.Get().OrbitalTypeCategories.iceCloud.Id,
      Db.Get().OrbitalTypeCategories.iceRock.Id
    })));
    configs.Add(new HarvestablePOIConfig.HarvestablePOIParams("gas_giant_cloud", new HarvestablePOIConfigurator.HarvestablePOIType("GasGiantCloud", new Dictionary<SimHashes, float>()
    {
      {
        SimHashes.Methane,
        1f
      },
      {
        SimHashes.LiquidMethane,
        1f
      },
      {
        SimHashes.SolidMethane,
        1f
      },
      {
        SimHashes.Hydrogen,
        7f
      }
    }, 15000f, 20000f, orbitalObject: HarvestablePOIConfig.GasFieldOrbit)));
    configs.Add(new HarvestablePOIConfig.HarvestablePOIParams("chlorine_cloud", new HarvestablePOIConfigurator.HarvestablePOIType("ChlorineCloud", new Dictionary<SimHashes, float>()
    {
      {
        SimHashes.Chlorine,
        2.5f
      },
      {
        SimHashes.BleachStone,
        7.5f
      }
    }, orbitalObject: HarvestablePOIConfig.GasFieldOrbit)));
    configs.Add(new HarvestablePOIConfig.HarvestablePOIParams("gilded_asteroid_field", new HarvestablePOIConfigurator.HarvestablePOIType("GildedAsteroidField", new Dictionary<SimHashes, float>()
    {
      {
        SimHashes.Gold,
        2.5f
      },
      {
        SimHashes.Fullerene,
        1f
      },
      {
        SimHashes.RefinedCarbon,
        1f
      },
      {
        SimHashes.SedimentaryRock,
        4.5f
      },
      {
        SimHashes.Regolith,
        1f
      }
    }, 30000f, 45000f, orbitalObject: HarvestablePOIConfig.AsteroidFieldOrbit)));
    configs.Add(new HarvestablePOIConfig.HarvestablePOIParams("glimmering_asteroid_field", new HarvestablePOIConfigurator.HarvestablePOIType("GlimmeringAsteroidField", new Dictionary<SimHashes, float>()
    {
      {
        SimHashes.MoltenTungsten,
        2f
      },
      {
        SimHashes.Wolframite,
        6f
      },
      {
        SimHashes.Carbon,
        1f
      },
      {
        SimHashes.CarbonDioxide,
        1f
      }
    }, 30000f, 45000f, orbitalObject: HarvestablePOIConfig.AsteroidFieldOrbit)));
    configs.Add(new HarvestablePOIConfig.HarvestablePOIParams("helium_cloud", new HarvestablePOIConfigurator.HarvestablePOIType("HeliumCloud", new Dictionary<SimHashes, float>()
    {
      {
        SimHashes.Hydrogen,
        2f
      },
      {
        SimHashes.Water,
        8f
      }
    }, 30000f, 45000f, orbitalObject: HarvestablePOIConfig.GasFieldOrbit)));
    configs.Add(new HarvestablePOIConfig.HarvestablePOIParams("oily_asteroid_field", new HarvestablePOIConfigurator.HarvestablePOIType("OilyAsteroidField", new Dictionary<SimHashes, float>()
    {
      {
        SimHashes.SolidCarbonDioxide,
        7.75f
      },
      {
        SimHashes.SolidMethane,
        1.125f
      },
      {
        SimHashes.CrudeOil,
        1.125f
      }
    }, 15000f, 25000f, orbitalObject: HarvestablePOIConfig.AsteroidFieldOrbit)));
    configs.Add(new HarvestablePOIConfig.HarvestablePOIParams("oxidized_asteroid_field", new HarvestablePOIConfigurator.HarvestablePOIType("OxidizedAsteroidField", new Dictionary<SimHashes, float>()
    {
      {
        SimHashes.Rust,
        8f
      },
      {
        SimHashes.SolidCarbonDioxide,
        2f
      }
    }, orbitalObject: HarvestablePOIConfig.AsteroidFieldOrbit)));
    configs.Add(new HarvestablePOIConfig.HarvestablePOIParams("salty_asteroid_field", new HarvestablePOIConfigurator.HarvestablePOIType("SaltyAsteroidField", new Dictionary<SimHashes, float>()
    {
      {
        SimHashes.SaltWater,
        5f
      },
      {
        SimHashes.Brine,
        4f
      },
      {
        SimHashes.SolidCarbonDioxide,
        1f
      }
    }, orbitalObject: HarvestablePOIConfig.AsteroidFieldOrbit)));
    configs.Add(new HarvestablePOIConfig.HarvestablePOIParams("frozen_ore_field", new HarvestablePOIConfigurator.HarvestablePOIType("FrozenOreField", new Dictionary<SimHashes, float>()
    {
      {
        SimHashes.Ice,
        2.33f
      },
      {
        SimHashes.DirtyIce,
        2.33f
      },
      {
        SimHashes.Snow,
        1.83f
      },
      {
        SimHashes.AluminumOre,
        2f
      }
    }, orbitalObject: HarvestablePOIConfig.AsteroidFieldOrbit)));
    configs.Add(new HarvestablePOIConfig.HarvestablePOIParams("foresty_ore_field", new HarvestablePOIConfigurator.HarvestablePOIType("ForestyOreField", new Dictionary<SimHashes, float>()
    {
      {
        SimHashes.IgneousRock,
        7f
      },
      {
        SimHashes.AluminumOre,
        1f
      },
      {
        SimHashes.CarbonDioxide,
        2f
      }
    }, orbitalObject: HarvestablePOIConfig.AsteroidFieldOrbit)));
    configs.Add(new HarvestablePOIConfig.HarvestablePOIParams("swampy_ore_field", new HarvestablePOIConfigurator.HarvestablePOIType("SwampyOreField", new Dictionary<SimHashes, float>()
    {
      {
        SimHashes.Mud,
        2f
      },
      {
        SimHashes.ToxicSand,
        7f
      },
      {
        SimHashes.Cobaltite,
        1f
      }
    }, orbitalObject: HarvestablePOIConfig.AsteroidFieldOrbit)));
    configs.Add(new HarvestablePOIConfig.HarvestablePOIParams("sandy_ore_field", new HarvestablePOIConfigurator.HarvestablePOIType("SandyOreField", new Dictionary<SimHashes, float>()
    {
      {
        SimHashes.SandStone,
        4f
      },
      {
        SimHashes.Algae,
        2f
      },
      {
        SimHashes.Cuprite,
        1f
      },
      {
        SimHashes.Sand,
        3f
      }
    }, orbitalObject: HarvestablePOIConfig.AsteroidFieldOrbit)));
    configs.Add(new HarvestablePOIConfig.HarvestablePOIParams("radioactive_gas_cloud", new HarvestablePOIConfigurator.HarvestablePOIType("RadioactiveGasCloud", new Dictionary<SimHashes, float>()
    {
      {
        SimHashes.UraniumOre,
        2f
      },
      {
        SimHashes.Chlorine,
        2f
      },
      {
        SimHashes.CarbonDioxide,
        7f
      }
    }, 5000f, 10000f, orbitalObject: HarvestablePOIConfig.AsteroidFieldOrbit)));
    configs.Add(new HarvestablePOIConfig.HarvestablePOIParams("radioactive_asteroid_field", new HarvestablePOIConfigurator.HarvestablePOIType("RadioactiveAsteroidField", new Dictionary<SimHashes, float>()
    {
      {
        SimHashes.UraniumOre,
        2f
      },
      {
        SimHashes.Sulfur,
        3f
      },
      {
        SimHashes.BleachStone,
        2f
      },
      {
        SimHashes.Rust,
        4f
      }
    }, 5000f, 10000f, orbitalObject: HarvestablePOIConfig.AsteroidFieldOrbit)));
    configs.Add(new HarvestablePOIConfig.HarvestablePOIParams("oxygen_rich_asteroid_field", new HarvestablePOIConfigurator.HarvestablePOIType("OxygenRichAsteroidField", new Dictionary<SimHashes, float>()
    {
      {
        SimHashes.Water,
        4f
      },
      {
        SimHashes.ContaminatedOxygen,
        2f
      },
      {
        SimHashes.Ice,
        4f
      }
    }, 15000f, 25000f, orbitalObject: HarvestablePOIConfig.AsteroidFieldOrbit)));
    configs.Add(new HarvestablePOIConfig.HarvestablePOIParams("interstellar_ocean", new HarvestablePOIConfigurator.HarvestablePOIType("InterstellarOcean", new Dictionary<SimHashes, float>()
    {
      {
        SimHashes.SaltWater,
        2.5f
      },
      {
        SimHashes.Brine,
        2.5f
      },
      {
        SimHashes.Salt,
        2.5f
      },
      {
        SimHashes.Ice,
        2.5f
      }
    }, 15000f, 25000f, orbitalObject: HarvestablePOIConfig.AsteroidFieldOrbit)));
    configs.RemoveAll((Predicate<HarvestablePOIConfig.HarvestablePOIParams>) (poi => !Util.IsNullOrWhiteSpace(poi.poiType.dlcID) && !DlcManager.IsContentActive(poi.poiType.dlcID)));
    return configs;
  }

  public struct HarvestablePOIParams
  {
    public string id;
    public string anim;
    public StringKey nameStringKey;
    public StringKey descStringKey;
    public HarvestablePOIConfigurator.HarvestablePOIType poiType;

    public HarvestablePOIParams(
      string anim,
      HarvestablePOIConfigurator.HarvestablePOIType poiType)
    {
      this.id = "HarvestableSpacePOI_" + poiType.id;
      this.anim = anim;
      this.nameStringKey = new StringKey("STRINGS.UI.SPACEDESTINATIONS.HARVESTABLE_POI." + poiType.id.ToUpper() + ".NAME");
      this.descStringKey = new StringKey("STRINGS.UI.SPACEDESTINATIONS.HARVESTABLE_POI." + poiType.id.ToUpper() + ".DESC");
      this.poiType = poiType;
    }
  }
}
