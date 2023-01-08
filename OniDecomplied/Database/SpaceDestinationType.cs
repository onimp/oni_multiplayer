// Decompiled with JetBrains decompiler
// Type: Database.SpaceDestinationType
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using System.Collections.Generic;
using System.Diagnostics;

namespace Database
{
  [DebuggerDisplay("{Id}")]
  public class SpaceDestinationType : Resource
  {
    public const float MASS_TO_RECOVER = 1000f;
    public string typeName;
    public string description;
    public int iconSize = 128;
    public string spriteName;
    public Dictionary<SimHashes, MathUtil.MinMax> elementTable;
    public Dictionary<string, int> recoverableEntities;
    public ArtifactDropRate artifactDropTable;
    public bool visitable;
    public int cyclesToRecover;

    public int maxiumMass { get; private set; }

    public int minimumMass { get; private set; }

    public float replishmentPerCycle => 1000f / (float) this.cyclesToRecover;

    public float replishmentPerSim1000ms => (float) (1000.0 / ((double) this.cyclesToRecover * 600.0));

    public SpaceDestinationType(
      string id,
      ResourceSet parent,
      string name,
      string description,
      int iconSize,
      string spriteName,
      Dictionary<SimHashes, MathUtil.MinMax> elementTable,
      Dictionary<string, int> recoverableEntities = null,
      ArtifactDropRate artifactDropRate = null,
      int max = 64000000,
      int min = 63994000,
      int cycles = 6,
      bool visitable = true)
      : base(id, parent, name)
    {
      this.typeName = name;
      this.description = description;
      this.iconSize = iconSize;
      this.spriteName = spriteName;
      this.elementTable = elementTable;
      this.recoverableEntities = recoverableEntities;
      this.artifactDropTable = artifactDropRate;
      this.maxiumMass = max;
      this.minimumMass = min;
      this.cyclesToRecover = cycles;
      this.visitable = visitable;
    }
  }
}
