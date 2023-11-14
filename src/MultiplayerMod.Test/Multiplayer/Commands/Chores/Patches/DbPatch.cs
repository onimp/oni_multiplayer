using Database;
using HarmonyLib;
using JetBrains.Annotations;
using Klei.AI;
using Amounts = Database.Amounts;
using AttributeConverters = Database.AttributeConverters;
using Attributes = Database.Attributes;

namespace MultiplayerMod.Test.Multiplayer.Commands.Chores.Patches;

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
        __instance.AttributeConverters = new AttributeConverters();
        return false;
    }
}
