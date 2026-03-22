using Database;
using HarmonyLib;
using JetBrains.Annotations;
using Klei.AI;
using Amounts = Database.Amounts;
using AttributeConverters = Database.AttributeConverters;
using Attributes = Database.Attributes;

namespace MultiplayerMod.Test.GameRuntime.Patches;

[UsedImplicitly]
[HarmonyPatch(typeof(Db))]
public class DbPatch {

    [UsedImplicitly]
    [HarmonyPrefix]
    [HarmonyPatch(nameof(Db.Initialize))]
    private static bool DbInitialize(Db __instance) {
        var root = __instance.Root;

        __instance.DuplicantStatusItems = new DuplicantStatusItems(root);
        __instance.Attributes = new Attributes(root);
        __instance.ChoreGroups = new ChoreGroups(root);
        __instance.Urges = new Urges();
        __instance.ChoreTypes = new ChoreTypes(root);
        __instance.Amounts = new Amounts();
        __instance.Amounts.Load();
        __instance.Thoughts = new Thoughts(root);
        __instance.Faces = new Faces();
        __instance.Expressions = new Expressions(root);
        __instance.ScheduleBlockTypes = new ScheduleBlockTypes(root);
        __instance.SkillPerks = new SkillPerks(root);
        __instance.Emotes = new Emotes(root);
        __instance.Deaths = new Deaths(root);
        __instance.MiscStatusItems = new MiscStatusItems(root);
        __instance.BuildingStatusItems = new BuildingStatusItems(root);
        __instance.effects = new ResourceSet<Effect>();
        // Minimal Effect stubs required by SM ctors that call Db.Get().effects.Get(...).
        // Normal game loads these from modifierInfos JSON (ModifierSet.LoadEffects) or
        // explicitly in ModifierSet/Db.Initialize — both skipped by this patch.
        // ExternalTemperatureMonitor field-initializers (factory [14]):
        __instance.effects.Add(new Effect("WarmAir",                "WarmAir",                "", 0f, false, false, false));
        __instance.effects.Add(new Effect("ColdAir",                "ColdAir",                "", 0f, false, false, false));
        __instance.effects.Add(new Effect("WarmTouch",              "WarmTouch",              "", 0f, false, false, false));
        __instance.effects.Add(new Effect("WarmTouchFood",          "WarmTouchFood",          "", 0f, false, false, false));
        // TubeTraveller.InitModifiers called from InitializeStates (factory [29]):
        __instance.effects.Add(new Effect("SoakingWet",             "SoakingWet",             "", 0f, false, false, false));
        __instance.effects.Add(new Effect("WetFeet",                "WetFeet",                "", 0f, false, false, false));
        __instance.effects.Add(new Effect("PoppedEarDrums",         "PoppedEarDrums",         "", 0f, false, false, false));
        __instance.effects.Add(new Effect("MinorIrritation",        "MinorIrritation",        "", 0f, false, false, false));
        __instance.effects.Add(new Effect("MajorIrritation",        "MajorIrritation",        "", 0f, false, false, false));
        // SlipperyMonitor.Instance ctor (factory [39]):
        __instance.effects.Add(new Effect("RecentlySlippedTracker", "RecentlySlippedTracker", "", 0f, false, false, false));
        __instance.AttributeConverters = new AttributeConverters();
        __instance.StatusItemCategories = new StatusItemCategories(root);
        __instance.Personalities = new Personalities();
        __instance.TechItems = new TechItems(root);
        __instance.RoomTypeCategories = new RoomTypeCategories(root);
        __instance.RoomTypes = new RoomTypes(root);
        __instance.AssignableSlots = new AssignableSlots();
        __instance.ArtableStatuses = new ArtableStatuses(root);
        __instance.Accessories = new Accessories(root);
        __instance.AccessorySlots = new AccessorySlots(root);
        __instance.Permits = new PermitResources(root);
        __instance.Stories = new Stories(root);
        // Additional fields needed for building configs
        __instance.CreatureStatusItems = new CreatureStatusItems(root);
        __instance.CritterAttributes = new CritterAttributes(root);
        __instance.Spices = new Spices(root);
        __instance.SkillGroups = new SkillGroups(root);
        __instance.Skills = new Skills(root);
        __instance.TechTreeTitles = new TechTreeTitles(root);
        __instance.Techs = new Techs(root);
        try { __instance.Techs.Init(); } catch {}
        try { __instance.TechItems.Init(); } catch {}
        return false;
    }
}
