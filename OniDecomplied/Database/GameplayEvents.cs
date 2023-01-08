// Decompiled with JetBrains decompiler
// Type: Database.GameplayEvents
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D13CBB0B-55A9-4EF0-9BB5-5C2436A6B8EE
// Assembly location: D:\dev\OniMod\Assembly-CSharp.dll

using Klei.AI;
using STRINGS;
using UnityEngine;

namespace Database
{
  public class GameplayEvents : ResourceSet<GameplayEvent>
  {
    public GameplayEvent HatchSpawnEvent;
    public GameplayEvent PartyEvent;
    public GameplayEvent EclipseEvent;
    public GameplayEvent SatelliteCrashEvent;
    public GameplayEvent FoodFightEvent;
    public GameplayEvent MeteorShowerIronEvent;
    public GameplayEvent MeteorShowerGoldEvent;
    public GameplayEvent MeteorShowerCopperEvent;
    public GameplayEvent MeteorShowerFullereneEvent;
    public GameplayEvent MeteorShowerDustEvent;
    public GameplayEvent GassyMooteorEvent;
    public GameplayEvent PrickleFlowerBlightEvent;
    public GameplayEvent BonusDream1;
    public GameplayEvent BonusDream2;
    public GameplayEvent BonusDream3;
    public GameplayEvent BonusDream4;
    public GameplayEvent BonusToilet1;
    public GameplayEvent BonusToilet2;
    public GameplayEvent BonusToilet3;
    public GameplayEvent BonusToilet4;
    public GameplayEvent BonusResearch;
    public GameplayEvent BonusDigging1;
    public GameplayEvent BonusStorage;
    public GameplayEvent BonusBuilder;
    public GameplayEvent BonusOxygen;
    public GameplayEvent BonusAlgae;
    public GameplayEvent BonusGenerator;
    public GameplayEvent BonusDoor;
    public GameplayEvent BonusHitTheBooks;
    public GameplayEvent BonusLitWorkspace;
    public GameplayEvent BonusTalker;
    public GameplayEvent CryoFriend;
    public GameplayEvent WarpWorldReveal;
    public GameplayEvent ArtifactReveal;

    public GameplayEvents(ResourceSet parent)
      : base(nameof (GameplayEvents), parent)
    {
      this.HatchSpawnEvent = this.Add((GameplayEvent) new CreatureSpawnEvent());
      this.PartyEvent = this.Add((GameplayEvent) new Klei.AI.PartyEvent());
      this.EclipseEvent = this.Add((GameplayEvent) new Klei.AI.EclipseEvent());
      this.SatelliteCrashEvent = this.Add((GameplayEvent) new Klei.AI.SatelliteCrashEvent());
      this.FoodFightEvent = this.Add((GameplayEvent) new Klei.AI.FoodFightEvent());
      MathUtil.MinMax secondsBombardmentOn;
      // ISSUE: explicit constructor call
      ((MathUtil.MinMax) ref secondsBombardmentOn).\u002Ector(100f, 400f);
      this.MeteorShowerIronEvent = this.Add((GameplayEvent) new MeteorShowerEvent(nameof (MeteorShowerIronEvent), 6000f, 1.25f, new MathUtil.MinMax(300f, 1200f), secondsBombardmentOn).AddMeteor(IronCometConfig.ID, 1f).AddMeteor(RockCometConfig.ID, 2f).AddMeteor(DustCometConfig.ID, 5f));
      // ISSUE: explicit constructor call
      ((MathUtil.MinMax) ref secondsBombardmentOn).\u002Ector(50f, 100f);
      this.MeteorShowerGoldEvent = this.Add((GameplayEvent) new MeteorShowerEvent(nameof (MeteorShowerGoldEvent), 3000f, 0.4f, new MathUtil.MinMax(800f, 1200f), secondsBombardmentOn).AddMeteor(GoldCometConfig.ID, 2f).AddMeteor(RockCometConfig.ID, 0.5f).AddMeteor(DustCometConfig.ID, 5f));
      // ISSUE: explicit constructor call
      ((MathUtil.MinMax) ref secondsBombardmentOn).\u002Ector(100f, 400f);
      this.MeteorShowerCopperEvent = this.Add((GameplayEvent) new MeteorShowerEvent(nameof (MeteorShowerCopperEvent), 4200f, 5.5f, new MathUtil.MinMax(300f, 1200f), secondsBombardmentOn).AddMeteor(CopperCometConfig.ID, 1f).AddMeteor(RockCometConfig.ID, 1f));
      // ISSUE: explicit constructor call
      ((MathUtil.MinMax) ref secondsBombardmentOn).\u002Ector(80f, 80f);
      this.MeteorShowerFullereneEvent = this.Add((GameplayEvent) new MeteorShowerEvent(nameof (MeteorShowerFullereneEvent), 30f, 0.66f, new MathUtil.MinMax(1f, 1f), secondsBombardmentOn).AddMeteor(FullereneCometConfig.ID, 6f).AddMeteor(RockCometConfig.ID, 1f).AddMeteor(DustCometConfig.ID, 1f));
      // ISSUE: explicit constructor call
      ((MathUtil.MinMax) ref secondsBombardmentOn).\u002Ector(100f, 400f);
      this.MeteorShowerDustEvent = this.Add((GameplayEvent) new MeteorShowerEvent(nameof (MeteorShowerDustEvent), 9000f, 2f, new MathUtil.MinMax(300f, 1200f), secondsBombardmentOn).AddMeteor(RockCometConfig.ID, 1f).AddMeteor(DustCometConfig.ID, 5f));
      // ISSUE: explicit constructor call
      ((MathUtil.MinMax) ref secondsBombardmentOn).\u002Ector(15f, 15f);
      this.GassyMooteorEvent = this.Add((GameplayEvent) new MeteorShowerEvent(nameof (GassyMooteorEvent), 15f, 5f, new MathUtil.MinMax(1f, 1f), secondsBombardmentOn).AddMeteor(GassyMooCometConfig.ID, 1f));
      this.PrickleFlowerBlightEvent = this.Add((GameplayEvent) new PlantBlightEvent(nameof (PrickleFlowerBlightEvent), "PrickleFlower", 3600f, 30f));
      this.CryoFriend = this.Add((GameplayEvent) new SimpleEvent(nameof (CryoFriend), (string) GAMEPLAY_EVENTS.EVENT_TYPES.CRYOFRIEND.NAME, (string) GAMEPLAY_EVENTS.EVENT_TYPES.CRYOFRIEND.DESCRIPTION, "cryofriend_kanim", (string) GAMEPLAY_EVENTS.EVENT_TYPES.CRYOFRIEND.BUTTON));
      this.WarpWorldReveal = this.Add((GameplayEvent) new SimpleEvent(nameof (WarpWorldReveal), (string) GAMEPLAY_EVENTS.EVENT_TYPES.WARPWORLDREVEAL.NAME, (string) GAMEPLAY_EVENTS.EVENT_TYPES.WARPWORLDREVEAL.DESCRIPTION, "warpworldreveal_kanim", (string) GAMEPLAY_EVENTS.EVENT_TYPES.WARPWORLDREVEAL.BUTTON));
      this.ArtifactReveal = this.Add((GameplayEvent) new SimpleEvent(nameof (ArtifactReveal), (string) GAMEPLAY_EVENTS.EVENT_TYPES.ARTIFACT_REVEAL.NAME, (string) GAMEPLAY_EVENTS.EVENT_TYPES.ARTIFACT_REVEAL.DESCRIPTION, "analyzeartifact_kanim", (string) GAMEPLAY_EVENTS.EVENT_TYPES.ARTIFACT_REVEAL.BUTTON));
    }

    private void BonusEvents()
    {
      GameplayEventMinionFilters instance1 = GameplayEventMinionFilters.Instance;
      GameplayEventPreconditions instance2 = GameplayEventPreconditions.Instance;
      Skills skills = Db.Get().Skills;
      RoomTypes roomTypes = Db.Get().RoomTypes;
      this.BonusDream1 = this.Add(new BonusEvent("BonusDream1").TriggerOnUseBuilding(1, BedConfig.ID, LuxuryBedConfig.ID).SetRoomConstraints(false, roomTypes.Barracks).AddPrecondition(instance2.BuildingExists(BedConfig.ID, 2)).AddPriorityBoost(instance2.BuildingExists(BedConfig.ID, 5), 1).AddPriorityBoost(instance2.BuildingExists(LuxuryBedConfig.ID), 5).TrySpawnEventOnSuccess(HashedString.op_Implicit("BonusDream2")));
      this.BonusDream2 = this.Add(new BonusEvent("BonusDream2", priority: 10).TriggerOnUseBuilding(10, BedConfig.ID, LuxuryBedConfig.ID).AddPrecondition(instance2.PastEventCountAndNotActive(this.BonusDream1)).AddPrecondition(instance2.Or(instance2.RoomBuilt(roomTypes.Barracks), instance2.RoomBuilt(roomTypes.Bedroom))).AddPriorityBoost(instance2.BuildingExists(LuxuryBedConfig.ID), 5).TrySpawnEventOnSuccess(HashedString.op_Implicit("BonusDream3")));
      this.BonusDream3 = this.Add(new BonusEvent("BonusDream3", priority: 20).TriggerOnUseBuilding(10, BedConfig.ID, LuxuryBedConfig.ID).AddPrecondition(instance2.PastEventCountAndNotActive(this.BonusDream2)).AddPrecondition(instance2.Or(instance2.RoomBuilt(roomTypes.Barracks), instance2.RoomBuilt(roomTypes.Bedroom))).TrySpawnEventOnSuccess(HashedString.op_Implicit("BonusDream4")));
      this.BonusDream4 = this.Add(new BonusEvent("BonusDream4", priority: 30).TriggerOnUseBuilding(10, LuxuryBedConfig.ID).AddPrecondition(instance2.PastEventCountAndNotActive(this.BonusDream2)).AddPrecondition(instance2.Or(instance2.RoomBuilt(roomTypes.Barracks), instance2.RoomBuilt(roomTypes.Bedroom))));
      this.BonusToilet1 = this.Add(new BonusEvent("BonusToilet1").TriggerOnUseBuilding(1, "Outhouse", "FlushToilet").AddPrecondition(instance2.Or(instance2.BuildingExists("Outhouse", 2), instance2.BuildingExists("FlushToilet"))).AddPrecondition(instance2.Or(instance2.BuildingExists("WashBasin", 2), instance2.BuildingExists("WashSink"))).AddPriorityBoost(instance2.BuildingExists("FlushToilet"), 1).TrySpawnEventOnSuccess(HashedString.op_Implicit("BonusToilet2")));
      this.BonusToilet2 = this.Add(new BonusEvent("BonusToilet2", priority: 10).TriggerOnUseBuilding(5, "FlushToilet").AddPrecondition(instance2.BuildingExists("FlushToilet")).AddPrecondition(instance2.PastEventCountAndNotActive(this.BonusToilet1)).AddPriorityBoost(instance2.BuildingExists("FlushToilet", 2), 5).TrySpawnEventOnSuccess(HashedString.op_Implicit("BonusToilet3")));
      this.BonusToilet3 = this.Add(new BonusEvent("BonusToilet3", priority: 20).TriggerOnUseBuilding(5, "FlushToilet").SetRoomConstraints(false, roomTypes.Latrine, roomTypes.PlumbedBathroom).AddPrecondition(instance2.PastEventCountAndNotActive(this.BonusToilet2)).AddPrecondition(instance2.Or(instance2.RoomBuilt(roomTypes.Latrine), instance2.RoomBuilt(roomTypes.PlumbedBathroom))).AddPriorityBoost(instance2.BuildingExists("FlushToilet", 2), 10).TrySpawnEventOnSuccess(HashedString.op_Implicit("BonusToilet4")));
      this.BonusToilet4 = this.Add(new BonusEvent("BonusToilet4", priority: 30).TriggerOnUseBuilding(5, "FlushToilet").SetRoomConstraints(false, roomTypes.PlumbedBathroom).AddPrecondition(instance2.PastEventCountAndNotActive(this.BonusToilet3)).AddPrecondition(instance2.RoomBuilt(roomTypes.PlumbedBathroom)));
      this.BonusResearch = this.Add(new BonusEvent("BonusResearch").AddPrecondition(instance2.BuildingExists("ResearchCenter")).AddPrecondition(instance2.ResearchCompleted("FarmingTech")).AddMinionFilter(instance1.HasSkillAptitude(skills.Researching1)));
      this.BonusDigging1 = this.Add(new BonusEvent("BonusDigging1", preSelectMinion: true).TriggerOnWorkableComplete(30, typeof (Diggable)).AddMinionFilter(instance1.Or(instance1.HasChoreGroupPriorityOrHigher(Db.Get().ChoreGroups.Dig, 4), instance1.HasSkillAptitude(skills.Mining1))).AddPriorityBoost(instance2.MinionsWithChoreGroupPriorityOrGreater(Db.Get().ChoreGroups.Dig, 1, 4), 1));
      this.BonusStorage = this.Add(new BonusEvent("BonusStorage", preSelectMinion: true).TriggerOnUseBuilding(10, "StorageLocker").AddMinionFilter(instance1.Or(instance1.HasChoreGroupPriorityOrHigher(Db.Get().ChoreGroups.Hauling, 4), instance1.HasSkillAptitude(skills.Hauling1))).AddPrecondition(instance2.BuildingExists("StorageLocker")));
      this.BonusBuilder = this.Add(new BonusEvent("BonusBuilder", preSelectMinion: true).TriggerOnNewBuilding(10).AddMinionFilter(instance1.Or(instance1.HasChoreGroupPriorityOrHigher(Db.Get().ChoreGroups.Build, 4), instance1.HasSkillAptitude(skills.Building1))));
      this.BonusOxygen = this.Add(new BonusEvent("BonusOxygen").TriggerOnUseBuilding(1, "MineralDeoxidizer").AddPrecondition(instance2.BuildingExists("MineralDeoxidizer")).AddPrecondition(instance2.Not(instance2.PastEventCount("BonusAlgae"))));
      this.BonusAlgae = this.Add(new BonusEvent("BonusAlgae", "BonusOxygen").TriggerOnUseBuilding(1, "AlgaeHabitat").AddPrecondition(instance2.BuildingExists("AlgaeHabitat")).AddPrecondition(instance2.Not(instance2.PastEventCount("BonusOxygen"))));
      this.BonusGenerator = this.Add(new BonusEvent("BonusGenerator").TriggerOnUseBuilding(1, "ManualGenerator").AddPrecondition(instance2.BuildingExists("ManualGenerator")));
      this.BonusDoor = this.Add(new BonusEvent("BonusDoor").TriggerOnUseBuilding(1, "Door").SetExtraCondition((BonusEvent.ConditionFn) (data => ((Component) data.building).GetComponent<Door>().RequestedState == Door.ControlState.Locked)).AddPrecondition(instance2.RoomBuilt(roomTypes.Barracks)));
      this.BonusHitTheBooks = this.Add(new BonusEvent("BonusHitTheBooks", preSelectMinion: true).TriggerOnWorkableComplete(1, typeof (ResearchCenter), typeof (NuclearResearchCenterWorkable)).AddPrecondition(instance2.BuildingExists("ResearchCenter")).AddMinionFilter(instance1.HasSkillAptitude(skills.Researching1)));
      this.BonusLitWorkspace = this.Add(new BonusEvent("BonusLitWorkspace").TriggerOnWorkableComplete(1).SetExtraCondition((BonusEvent.ConditionFn) (data => data.workable.currentlyLit)).AddPrecondition(instance2.CycleRestriction(10f)));
      this.BonusTalker = this.Add(new BonusEvent("BonusTalker", preSelectMinion: true).TriggerOnWorkableComplete(3, typeof (SocialGatheringPointWorkable)).SetExtraCondition((BonusEvent.ConditionFn) (data => (data.workable as SocialGatheringPointWorkable).timesConversed > 0)).AddPrecondition(instance2.CycleRestriction(10f)));
    }

    private void VerifyEvents()
    {
      foreach (GameplayEvent resource in this.resources)
      {
        if (HashedString.op_Equality(resource.animFileName, HashedString.op_Implicit((string) null)))
          DebugUtil.LogWarningArgs(new object[1]
          {
            (object) ("Gameplay event anim missing: " + resource.Id)
          });
        if (resource is BonusEvent)
          this.VerifyBonusEvent(resource as BonusEvent);
      }
    }

    private void VerifyBonusEvent(BonusEvent e)
    {
      StringEntry stringEntry;
      if (!Strings.TryGet("STRINGS.GAMEPLAY_EVENTS.BONUS." + e.Id.ToUpper() + ".NAME", ref stringEntry))
        DebugUtil.DevLogError("Event [" + e.Id + "]: STRINGS.GAMEPLAY_EVENTS.BONUS." + e.Id.ToUpper() + " is missing");
      Effect effect = Db.Get().effects.TryGet(e.effect);
      if (effect == null)
      {
        DebugUtil.DevLogError("Effect " + e.effect + "[" + e.Id + "]: Missing from spreadsheet");
      }
      else
      {
        if (!Strings.TryGet("STRINGS.DUPLICANTS.MODIFIERS." + effect.Id.ToUpper() + ".NAME", ref stringEntry))
          DebugUtil.DevLogError("Effect " + e.effect + "[" + e.Id + "]: STRINGS.DUPLICANTS.MODIFIERS." + effect.Id.ToUpper() + ".NAME is missing");
        if (Strings.TryGet("STRINGS.DUPLICANTS.MODIFIERS." + effect.Id.ToUpper() + ".TOOLTIP", ref stringEntry))
          return;
        DebugUtil.DevLogError("Effect " + e.effect + "[" + e.Id + "]: STRINGS.DUPLICANTS.MODIFIERS." + effect.Id.ToUpper() + ".TOOLTIP is missing");
      }
    }
  }
}
