﻿using System;
using HarmonyLib;
using MultiplayerMod.Core.Patch;
using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Game.Mechanics;

[HarmonyPatch]
public static class FilterEvents {

    public static event Action<MultiplayerReference, Tag> SetFilter;
    public static event Action<MultiplayerReference, Tag> AddTagToFilter;
    public static event Action<MultiplayerReference, Tag> RemoveTagFromFilter;

    [HarmonyPostfix]
    [HarmonyPatch(typeof(Filterable), nameof(Filterable.OnFilterChanged))]
    private static void OnFilterChangedPostfix(Filterable __instance) => PatchControl.RunIfEnabled(
        () => SetFilter?.Invoke(__instance.GetMultiplayerReference(), __instance.selectedTag)
    );

    [HarmonyPostfix]
    [HarmonyPatch(typeof(TreeFilterable), nameof(TreeFilterable.AddTagToFilter))]
    private static void AddTagToFilterPostfix(TreeFilterable __instance, Tag t) => PatchControl.RunIfEnabled(
        () => AddTagToFilter?.Invoke(__instance.GetMultiplayerReference(), t)
    );

    [HarmonyPostfix]
    [HarmonyPatch(typeof(TreeFilterable), nameof(TreeFilterable.RemoveTagFromFilter))]
    private static void RemoveTagFromFilterPostfix(TreeFilterable __instance, Tag t) => PatchControl.RunIfEnabled(
        () => RemoveTagFromFilter?.Invoke(__instance.GetMultiplayerReference(), t)
    );
}