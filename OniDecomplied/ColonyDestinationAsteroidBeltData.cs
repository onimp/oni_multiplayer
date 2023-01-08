// Decompiled with JetBrains decompiler
// Type: ColonyDestinationAsteroidBeltData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ProcGen;
using STRINGS;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ColonyDestinationAsteroidBeltData
{
  private World startWorld;
  private ClusterLayout cluster;
  private List<AsteroidDescriptor> paramDescriptors = new List<AsteroidDescriptor>();
  private List<AsteroidDescriptor> traitDescriptors = new List<AsteroidDescriptor>();
  public static List<Tuple<string, string, string>> survivalOptions;

  public float TargetScale { get; set; }

  public float Scale { get; set; }

  public int seed { get; private set; }

  public string startWorldPath => this.startWorld.filePath;

  public Sprite sprite { get; private set; }

  public int difficulty { get; private set; }

  public string startWorldName => StringEntry.op_Implicit(Strings.Get(this.startWorld.name));

  public string properName => this.cluster == null ? "" : this.cluster.name;

  public string beltPath => this.cluster == null ? WorldGenSettings.ClusterDefaultName : this.cluster.filePath;

  public List<World> worlds { get; private set; }

  public ClusterLayout Layout => this.cluster;

  public World GetStartWorld => this.startWorld;

  public ColonyDestinationAsteroidBeltData(string staringWorldName, int seed, string clusterPath)
  {
    this.startWorld = SettingsCache.worlds.GetWorldData(staringWorldName);
    this.Scale = this.TargetScale = this.startWorld.iconScale;
    this.worlds = new List<World>();
    if (clusterPath != null)
    {
      this.cluster = SettingsCache.clusterLayouts.GetClusterData(clusterPath);
      for (int index = 0; index < this.cluster.worldPlacements.Count; ++index)
      {
        if (index != this.cluster.startWorldIndex)
          this.worlds.Add(SettingsCache.worlds.GetWorldData(this.cluster.worldPlacements[index].world));
      }
    }
    this.ReInitialize(seed);
  }

  public static Sprite GetUISprite(string filename)
  {
    if (Util.IsNullOrWhiteSpace(filename))
      filename = DlcManager.FeatureClusterSpaceEnabled() ? "asteroid_sandstone_start_kanim" : "Asteroid_sandstone";
    KAnimFile anim;
    Assets.TryGetAnim(HashedString.op_Implicit(filename), out anim);
    return Object.op_Inequality((Object) anim, (Object) null) ? Def.GetUISpriteFromMultiObjectAnim(anim) : Assets.GetSprite(HashedString.op_Implicit(filename));
  }

  public void ReInitialize(int seed)
  {
    this.seed = seed;
    this.paramDescriptors.Clear();
    this.traitDescriptors.Clear();
    this.sprite = ColonyDestinationAsteroidBeltData.GetUISprite(this.startWorld.asteroidIcon);
    this.difficulty = this.cluster.difficulty;
  }

  public List<AsteroidDescriptor> GetParamDescriptors()
  {
    if (this.paramDescriptors.Count == 0)
      this.paramDescriptors = this.GenerateParamDescriptors();
    return this.paramDescriptors;
  }

  public List<AsteroidDescriptor> GetTraitDescriptors()
  {
    if (this.traitDescriptors.Count == 0)
      this.traitDescriptors = this.GenerateTraitDescriptors();
    return this.traitDescriptors;
  }

  private List<AsteroidDescriptor> GenerateParamDescriptors()
  {
    List<AsteroidDescriptor> paramDescriptors = new List<AsteroidDescriptor>();
    if (this.cluster != null && DlcManager.FeatureClusterSpaceEnabled())
      paramDescriptors.Add(new AsteroidDescriptor(string.Format((string) WORLDS.SURVIVAL_CHANCE.CLUSTERNAME, (object) Strings.Get(this.cluster.name)), StringEntry.op_Implicit(Strings.Get(this.cluster.description)), Color.white));
    paramDescriptors.Add(new AsteroidDescriptor(string.Format((string) WORLDS.SURVIVAL_CHANCE.PLANETNAME, (object) this.startWorldName), (string) null, Color.white));
    paramDescriptors.Add(new AsteroidDescriptor(StringEntry.op_Implicit(Strings.Get(this.startWorld.description)), (string) null, Color.white));
    if (DlcManager.FeatureClusterSpaceEnabled())
    {
      paramDescriptors.Add(new AsteroidDescriptor(string.Format((string) WORLDS.SURVIVAL_CHANCE.MOONNAMES), (string) null, Color.white));
      foreach (World world in this.worlds)
        paramDescriptors.Add(new AsteroidDescriptor(string.Format("{0}", (object) Strings.Get(world.name)), StringEntry.op_Implicit(Strings.Get(world.description)), Color.white));
    }
    int index = Mathf.Clamp(this.difficulty, 0, ColonyDestinationAsteroidBeltData.survivalOptions.Count - 1);
    Tuple<string, string, string> survivalOption = ColonyDestinationAsteroidBeltData.survivalOptions[index];
    paramDescriptors.Add(new AsteroidDescriptor(string.Format((string) WORLDS.SURVIVAL_CHANCE.TITLE, (object) survivalOption.first, (object) survivalOption.third), (string) null, Color.white));
    return paramDescriptors;
  }

  private List<AsteroidDescriptor> GenerateTraitDescriptors()
  {
    List<AsteroidDescriptor> traitDescriptors = new List<AsteroidDescriptor>();
    List<World> worldList = new List<World>();
    worldList.Add(this.startWorld);
    worldList.AddRange((IEnumerable<World>) this.worlds);
    for (int index = 0; index < worldList.Count; ++index)
    {
      World singleWorld = worldList[index];
      if (DlcManager.IsExpansion1Active())
      {
        traitDescriptors.Add(new AsteroidDescriptor("", (string) null, Color.white));
        traitDescriptors.Add(new AsteroidDescriptor(string.Format("<b>{0}</b>", (object) Strings.Get(singleWorld.name)), (string) null, Color.white));
      }
      List<WorldTrait> worldTraits = this.GetWorldTraits(singleWorld);
      foreach (WorldTrait worldTrait in worldTraits)
      {
        string associatedIcon = worldTrait.filePath.Substring(worldTrait.filePath.LastIndexOf("/") + 1);
        traitDescriptors.Add(new AsteroidDescriptor(string.Format("<color=#{1}>{0}</color>", (object) Strings.Get(worldTrait.name), (object) worldTrait.colorHex), StringEntry.op_Implicit(Strings.Get(worldTrait.description)), Util.ColorFromHex(worldTrait.colorHex), associatedIcon: associatedIcon));
      }
      if (worldTraits.Count == 0)
        traitDescriptors.Add(new AsteroidDescriptor((string) WORLD_TRAITS.NO_TRAITS.NAME, (string) WORLD_TRAITS.NO_TRAITS.DESCRIPTION, Color.white, associatedIcon: "NoTraits"));
    }
    return traitDescriptors;
  }

  public List<AsteroidDescriptor> GenerateTraitDescriptors(
    World singleWorld,
    bool includeDefaultTrait = true)
  {
    List<AsteroidDescriptor> traitDescriptors = new List<AsteroidDescriptor>();
    List<World> worldList = new List<World>();
    worldList.Add(this.startWorld);
    worldList.AddRange((IEnumerable<World>) this.worlds);
    for (int index = 0; index < worldList.Count; ++index)
    {
      if (worldList[index] == singleWorld)
      {
        List<WorldTrait> worldTraits = this.GetWorldTraits(worldList[index]);
        foreach (WorldTrait worldTrait in worldTraits)
        {
          string associatedIcon = worldTrait.filePath.Substring(worldTrait.filePath.LastIndexOf("/") + 1);
          traitDescriptors.Add(new AsteroidDescriptor(string.Format("<color=#{1}>{0}</color>", (object) Strings.Get(worldTrait.name), (object) worldTrait.colorHex), StringEntry.op_Implicit(Strings.Get(worldTrait.description)), Util.ColorFromHex(worldTrait.colorHex), associatedIcon: associatedIcon));
        }
        if (worldTraits.Count == 0 & includeDefaultTrait)
          traitDescriptors.Add(new AsteroidDescriptor((string) WORLD_TRAITS.NO_TRAITS.NAME, (string) WORLD_TRAITS.NO_TRAITS.DESCRIPTION, Color.white, associatedIcon: "NoTraits"));
      }
    }
    return traitDescriptors;
  }

  public List<WorldTrait> GetWorldTraits(World singleWorld)
  {
    List<WorldTrait> worldTraits = new List<WorldTrait>();
    List<World> worldList = new List<World>();
    worldList.Add(this.startWorld);
    worldList.AddRange((IEnumerable<World>) this.worlds);
    for (int index = 0; index < worldList.Count; ++index)
    {
      if (worldList[index] == singleWorld)
      {
        World world = worldList[index];
        int seed = this.seed;
        if (seed > 0)
          seed += this.cluster.worldPlacements.FindIndex((Predicate<WorldPlacement>) (x => x.world == world.filePath));
        foreach (string randomTrait in SettingsCache.GetRandomTraits(seed, world))
        {
          WorldTrait cachedWorldTrait = SettingsCache.GetCachedWorldTrait(randomTrait, true);
          worldTraits.Add(cachedWorldTrait);
        }
      }
    }
    return worldTraits;
  }

  static ColonyDestinationAsteroidBeltData()
  {
    List<Tuple<string, string, string>> tupleList = new List<Tuple<string, string, string>>();
    tupleList.Add(new Tuple<string, string, string>((string) WORLDS.SURVIVAL_CHANCE.MOSTHOSPITABLE, "", "D2F40C"));
    tupleList.Add(new Tuple<string, string, string>((string) WORLDS.SURVIVAL_CHANCE.VERYHIGH, "", "7DE419"));
    tupleList.Add(new Tuple<string, string, string>((string) WORLDS.SURVIVAL_CHANCE.HIGH, "", "36D246"));
    tupleList.Add(new Tuple<string, string, string>((string) WORLDS.SURVIVAL_CHANCE.NEUTRAL, "", "63C2B7"));
    tupleList.Add(new Tuple<string, string, string>((string) WORLDS.SURVIVAL_CHANCE.LOW, "", "6A8EB1"));
    tupleList.Add(new Tuple<string, string, string>((string) WORLDS.SURVIVAL_CHANCE.VERYLOW, "", "937890"));
    tupleList.Add(new Tuple<string, string, string>((string) WORLDS.SURVIVAL_CHANCE.LEASTHOSPITABLE, "", "9636DF"));
    ColonyDestinationAsteroidBeltData.survivalOptions = tupleList;
  }
}
