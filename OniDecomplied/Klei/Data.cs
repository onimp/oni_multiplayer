// Decompiled with JetBrains decompiler
// Type: Klei.Data
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using ProcGen;
using ProcGenGame;
using System.Collections.Generic;
using VoronoiTree;

namespace Klei
{
  public class Data
  {
    public int globalWorldSeed;
    public int globalWorldLayoutSeed;
    public int globalTerrainSeed;
    public int globalNoiseSeed;
    public int chunkEdgeSize = 32;
    public WorldLayout worldLayout;
    public List<TerrainCell> terrainCells;
    public List<TerrainCell> overworldCells;
    public List<River> rivers;
    public GameSpawnData gameSpawnData;
    public Chunk world;
    public Tree voronoiTree;
    public AxialI clusterLocation;

    public Data()
    {
      this.worldLayout = new WorldLayout((WorldGen) null, 0);
      this.terrainCells = new List<TerrainCell>();
      this.overworldCells = new List<TerrainCell>();
      this.rivers = new List<River>();
      this.gameSpawnData = new GameSpawnData();
      this.world = new Chunk();
      this.voronoiTree = new Tree(0);
    }
  }
}
