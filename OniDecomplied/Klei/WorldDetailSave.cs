// Decompiled with JetBrains decompiler
// Type: Klei.WorldDetailSave
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Delaunay.Geo;
using KSerialization;
using ProcGen;
using ProcGenGame;
using System.Collections.Generic;

namespace Klei
{
  public class WorldDetailSave
  {
    public List<WorldDetailSave.OverworldCell> overworldCells;
    public int globalWorldSeed;
    public int globalWorldLayoutSeed;
    public int globalTerrainSeed;
    public int globalNoiseSeed;

    public WorldDetailSave() => this.overworldCells = new List<WorldDetailSave.OverworldCell>();

    [SerializationConfig]
    public class OverworldCell
    {
      public Polygon poly;
      public TagSet tags;
      public SubWorld.ZoneType zoneType;

      public OverworldCell()
      {
      }

      public OverworldCell(SubWorld.ZoneType zoneType, TerrainCell tc)
      {
        this.poly = tc.poly;
        this.tags = ((ProcGen.Node) tc.node).tags;
        this.zoneType = zoneType;
      }
    }
  }
}
