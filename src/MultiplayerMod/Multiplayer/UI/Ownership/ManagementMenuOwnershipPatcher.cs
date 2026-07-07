using System;
using System.Collections;
using System.Collections.Generic;
using HarmonyLib;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Ownership;
using MultiplayerMod.Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerMod.Multiplayer.UI.Ownership;

/// <summary>
/// Adds a native "MP Owners" button to the top-bar management menu (next to Vitals / Consumables / Schedule)
/// that opens the <see cref="DuplicantOwnershipScreen"/>. Done by post-patching <c>ManagementMenu.OnPrefabInit</c>
/// and hand-appending one <c>KToggle</c> + <c>ScreenData</c> - we deliberately do NOT re-run the menu's
/// <c>Setup()</c>, because ManagementMenu wires several specific toggles after Setup and rebuilding them all
/// would invalidate those references.
///
/// The whole thing is gated to multiplayer sessions and wrapped defensively so a UI failure can never keep the
/// HUD from initialising.
/// </summary>
[Dependency, UsedImplicitly]
public class ManagementMenuOwnershipPatcher {

    private const string IconName = "OverviewUI_jobs_icon";

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger<ManagementMenuOwnershipPatcher>();

    private static DuplicantOwnershipRegistry registry = null!;
    private static DuplicantOwnershipController controller = null!;
    private static MultiplayerGame multiplayer = null!;
    private static IMultiplayerClient client = null!;
    private static ExecutionLevelManager executionLevel = null!;
    private static EventDispatcher events = null!;

    private readonly Harmony harmony;

    public ManagementMenuOwnershipPatcher(
        EventDispatcher events,
        Harmony harmony,
        DuplicantOwnershipRegistry registry,
        DuplicantOwnershipController controller,
        MultiplayerGame multiplayer,
        IMultiplayerClient client,
        ExecutionLevelManager executionLevel
    ) {
        this.harmony = harmony;
        ManagementMenuOwnershipPatcher.events = events;
        ManagementMenuOwnershipPatcher.registry = registry;
        ManagementMenuOwnershipPatcher.controller = controller;
        ManagementMenuOwnershipPatcher.multiplayer = multiplayer;
        ManagementMenuOwnershipPatcher.client = client;
        ManagementMenuOwnershipPatcher.executionLevel = executionLevel;

        events.Subscribe<RuntimeReadyEvent>(_ => OnRuntimeReady());
    }

    private void OnRuntimeReady() {
        var target = AccessTools.Method(typeof(ManagementMenu), "OnPrefabInit");
        var postfix = new HarmonyMethod(SymbolExtensions.GetMethodInfo(() => OnManagementMenuInit(null!)));
        harmony.CreateProcessor(target).AddPostfix(postfix).Patch();
    }

    [UsedImplicitly]
    [RequireExecutionLevel(ExecutionLevel.Multiplayer)]
    private static void OnManagementMenuInit(ManagementMenu __instance) {
        try {
            AddOwnershipTab(__instance);
        } catch (Exception e) {
            log.Error($"Failed to add duplicant ownership tab to management menu: {e}");
        }
    }

    private static void AddOwnershipTab(ManagementMenu menu) {
        var fontSource = FindFontSource(menu);
        if (fontSource == null || fontSource.font == null) {
            log.Warning("Could not resolve a font for the ownership screen; skipping the management tab.");
            return;
        }
        var font = fontSource.font;
        // Match vanilla text scale: the screen hardcoded sizes rendered ~2x too big because LocText with
        // allowOverride skips its style and falls back to TMP's large default point size. Deriving from a real
        // menu label keeps us in the canvas' actual units.
        var baseFontSize = fontSource.fontSize > 0f ? fontSource.fontSize : 14f;

        // Parent under the full-screen screen-space-overlay canvas so top-centre anchoring is predictable. The
        // vitals screen's own parent is a right-aligned "top right screens" container, which pushed the panel
        // off the right edge.
        var overlay = GameScreenManager.Instance != null ? GameScreenManager.Instance.ssOverlayCanvas : null;
        var container = overlay != null ? overlay.transform
            : menu.vitalsScreen != null ? menu.vitalsScreen.transform.parent : menu.transform;
        var screenGo = new GameObject("MultiplayerOwnershipScreen", typeof(RectTransform));
        screenGo.transform.SetParent(container, false);
        var screen = screenGo.AddComponent<DuplicantOwnershipScreen>();
        screenGo.SetActive(false);

        var info = new ManagementMenu.ManagementMenuToggleInfo(
            "MP Owners", IconName, null, global::Action.NumActions,
            "Per-player duplicant ownership: see and change who owns each duplicant."
        );
        var screenData = new ManagementMenu.ScreenData {
            screen = screen,
            toggleInfo = info,
            cancelHandler = null,
            tabIdx = 0
        };

        var screenInfoMatch = (IDictionary) AccessTools.Field(typeof(ManagementMenu), "ScreenInfoMatch").GetValue(menu);
        screenInfoMatch.Add(info, screenData);

        screen.Configure(registry, controller, multiplayer, client, executionLevel, events, font, baseFontSize,
            () => menu.ToggleScreen(screenData));

        AppendToggle(menu, info, () => menu.ToggleScreen(screenData));
    }

    // Instantiate a single toggle from the menu's own prefab and wire it directly to ToggleScreen, without
    // disturbing the toggles the menu built (and post-wired) during OnPrefabInit.
    private static void AppendToggle(ManagementMenu menu, ManagementMenu.ManagementMenuToggleInfo info, System.Action onClick) {
        var toggleParent = (Transform) AccessTools.Field(typeof(KIconToggleMenu), "toggleParent").GetValue(menu);
        var prefab = (KToggle) AccessTools.Field(typeof(KIconToggleMenu), "prefab").GetValue(menu);
        var group = (ToggleGroup) AccessTools.Field(typeof(KIconToggleMenu), "group").GetValue(menu);
        var toggles = (List<KToggle>) AccessTools.Field(typeof(KIconToggleMenu), "toggles").GetValue(menu);
        var toggleInfos = (IList) AccessTools.Field(typeof(KIconToggleMenu), "toggleInfo").GetValue(menu);

        var parent = toggleParent != null ? toggleParent : menu.transform;
        var toggle = Util.KInstantiateUI<KToggle>(prefab.gameObject, parent.gameObject, force_active: true);
        toggle.Deselect();
        toggle.gameObject.name = "Toggle:MP Owners";
        toggle.group = group;

        var label = toggle.GetComponentInChildren<LocText>();
        if (label != null)
            label.SetText("MP Owners");
        if (toggle.fgImage != null)
            toggle.fgImage.sprite = Assets.GetSprite(IconName);

        info.SetToggle(toggle);
        toggles.Add(toggle);
        toggleInfos.Add(info);
        toggle.onClick += () => onClick();
    }

    // Returns a real menu LocText we can copy both the font asset and its (canvas-correct) font size from.
    private static LocText? FindFontSource(ManagementMenu menu) {
        var source = menu.GetComponentInChildren<LocText>(true);
        if (source == null && menu.vitalsScreen != null)
            source = menu.vitalsScreen.GetComponentInChildren<LocText>(true);
        return source;
    }

}
