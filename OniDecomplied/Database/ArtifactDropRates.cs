// Decompiled with JetBrains decompiler
// Type: Database.ArtifactDropRates
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using TUNING;

namespace Database
{
  public class ArtifactDropRates : ResourceSet<ArtifactDropRate>
  {
    public ArtifactDropRate None;
    public ArtifactDropRate Bad;
    public ArtifactDropRate Mediocre;
    public ArtifactDropRate Good;
    public ArtifactDropRate Great;
    public ArtifactDropRate Amazing;
    public ArtifactDropRate Perfect;

    public ArtifactDropRates(ResourceSet parent)
      : base(nameof (ArtifactDropRates), parent)
    {
      this.CreateDropRates();
    }

    private void CreateDropRates()
    {
      this.None = new ArtifactDropRate();
      this.None.AddItem(DECOR.SPACEARTIFACT.TIER_NONE, 1f);
      this.Add(this.None);
      this.Bad = new ArtifactDropRate();
      this.Bad.AddItem(DECOR.SPACEARTIFACT.TIER_NONE, 10f);
      this.Bad.AddItem(DECOR.SPACEARTIFACT.TIER0, 5f);
      this.Bad.AddItem(DECOR.SPACEARTIFACT.TIER1, 3f);
      this.Bad.AddItem(DECOR.SPACEARTIFACT.TIER2, 2f);
      this.Add(this.Bad);
      this.Mediocre = new ArtifactDropRate();
      this.Mediocre.AddItem(DECOR.SPACEARTIFACT.TIER_NONE, 10f);
      this.Mediocre.AddItem(DECOR.SPACEARTIFACT.TIER1, 5f);
      this.Mediocre.AddItem(DECOR.SPACEARTIFACT.TIER2, 3f);
      this.Mediocre.AddItem(DECOR.SPACEARTIFACT.TIER3, 2f);
      this.Add(this.Mediocre);
      this.Good = new ArtifactDropRate();
      this.Good.AddItem(DECOR.SPACEARTIFACT.TIER_NONE, 10f);
      this.Good.AddItem(DECOR.SPACEARTIFACT.TIER2, 5f);
      this.Good.AddItem(DECOR.SPACEARTIFACT.TIER3, 3f);
      this.Good.AddItem(DECOR.SPACEARTIFACT.TIER4, 2f);
      this.Add(this.Good);
      this.Great = new ArtifactDropRate();
      this.Great.AddItem(DECOR.SPACEARTIFACT.TIER_NONE, 10f);
      this.Great.AddItem(DECOR.SPACEARTIFACT.TIER3, 5f);
      this.Great.AddItem(DECOR.SPACEARTIFACT.TIER4, 3f);
      this.Great.AddItem(DECOR.SPACEARTIFACT.TIER5, 2f);
      this.Add(this.Great);
      this.Amazing = new ArtifactDropRate();
      this.Amazing.AddItem(DECOR.SPACEARTIFACT.TIER_NONE, 10f);
      this.Amazing.AddItem(DECOR.SPACEARTIFACT.TIER3, 3f);
      this.Amazing.AddItem(DECOR.SPACEARTIFACT.TIER4, 5f);
      this.Amazing.AddItem(DECOR.SPACEARTIFACT.TIER5, 2f);
      this.Add(this.Amazing);
      this.Perfect = new ArtifactDropRate();
      this.Perfect.AddItem(DECOR.SPACEARTIFACT.TIER_NONE, 10f);
      this.Perfect.AddItem(DECOR.SPACEARTIFACT.TIER4, 6f);
      this.Perfect.AddItem(DECOR.SPACEARTIFACT.TIER5, 4f);
      this.Add(this.Perfect);
    }
  }
}
