// Decompiled with JetBrains decompiler
// Type: Database.GameplaySeasons
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;

namespace Database
{
  public class GameplaySeasons : ResourceSet<GameplaySeason>
  {
    public GameplaySeason MeteorShowers;
    public GameplaySeason GassyMooteorShowers;
    public GameplaySeason TemporalTearMeteorShowers;
    public GameplaySeason NaturalRandomEvents;
    public GameplaySeason DupeRandomEvents;
    public GameplaySeason PrickleCropSeason;
    public GameplaySeason BonusEvents;
    public GameplaySeason RegolithMoonMeteorShowers;

    public GameplaySeasons(ResourceSet parent)
      : base(nameof (GameplaySeasons), parent)
    {
      this.MeteorShowers = this.Add(new GameplaySeason(nameof (MeteorShowers), GameplaySeason.Type.World, "", 14f, false, startActive: true).AddEvent(Db.Get().GameplayEvents.MeteorShowerIronEvent).AddEvent(Db.Get().GameplayEvents.MeteorShowerGoldEvent).AddEvent(Db.Get().GameplayEvents.MeteorShowerCopperEvent));
      this.RegolithMoonMeteorShowers = this.Add(new GameplaySeason(nameof (RegolithMoonMeteorShowers), GameplaySeason.Type.World, "EXPANSION1_ID", 20f, false, startActive: true).AddEvent(Db.Get().GameplayEvents.MeteorShowerDustEvent));
      this.TemporalTearMeteorShowers = this.Add(new GameplaySeason(nameof (TemporalTearMeteorShowers), GameplaySeason.Type.World, "EXPANSION1_ID", 1f, false, 0.0f).AddEvent(Db.Get().GameplayEvents.MeteorShowerFullereneEvent));
      this.GassyMooteorShowers = this.Add(new GameplaySeason(nameof (GassyMooteorShowers), GameplaySeason.Type.World, "EXPANSION1_ID", 20f, false, startActive: true).AddEvent(Db.Get().GameplayEvents.GassyMooteorEvent));
    }
  }
}
