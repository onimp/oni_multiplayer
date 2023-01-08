// Decompiled with JetBrains decompiler
// Type: Klei.ClusterLayoutSave
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;

namespace Klei
{
  public class ClusterLayoutSave
  {
    public string ID;
    public Vector2I version;
    public List<ClusterLayoutSave.World> worlds;
    public Vector2I size;
    public int currentWorldIdx;
    public int numRings;
    public Dictionary<ClusterLayoutSave.POIType, List<AxialI>> poiLocations = new Dictionary<ClusterLayoutSave.POIType, List<AxialI>>();
    public Dictionary<AxialI, string> poiPlacements = new Dictionary<AxialI, string>();

    public ClusterLayoutSave() => this.worlds = new List<ClusterLayoutSave.World>();

    public class World
    {
      public Dictionary<string, object> stats = new Dictionary<string, object>();
      public Data data = new Data();
      public string name = string.Empty;
      public bool isDiscovered;
      public List<string> traits = new List<string>();
      public List<string> storyTraits = new List<string>();
    }

    public enum POIType
    {
      TemporalTear,
      ResearchDestination,
    }
  }
}
