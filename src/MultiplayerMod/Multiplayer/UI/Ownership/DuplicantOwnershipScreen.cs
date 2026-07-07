using System;
using System.Linq;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Game.Mechanics.Minions;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Commands.Player;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Ownership;
using MultiplayerMod.Multiplayer.Players;
using MultiplayerMod.Network;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MultiplayerMod.Multiplayer.UI.Ownership;

/// <summary>
/// The top-bar "Duplicant Ownership" management screen (opened by the button added in
/// <see cref="ManagementMenuOwnershipPatcher"/>). A self-built <see cref="KScreen"/>: one row per live
/// duplicant showing its owner with a click-to-cycle button, plus a host-only enable/disable toggle.
///
/// The whole thing is defensive - any failure while (re)building the UI is caught and logged so a UI bug can
/// never take down the game's HUD. Content is rebuilt on every show and whenever ownership changes, so the
/// list stays live without reopening.
/// </summary>
public class DuplicantOwnershipScreen : KScreen {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger<DuplicantOwnershipScreen>();

    private static readonly Color mineColor = new(0.55f, 0.85f, 0.55f);
    private static readonly Color unownedColor = new(0.75f, 0.75f, 0.75f);

    private DuplicantOwnershipRegistry registry = null!;
    private DuplicantOwnershipController controller = null!;
    private MultiplayerGame multiplayer = null!;
    private IMultiplayerClient client = null!;
    private ExecutionLevelManager executionLevel = null!;
    private EventDispatcher events = null!;
    private TMP_FontAsset font = null!;
    private float baseSize = 14f;
    private System.Action onClose = null!;

    private EventSubscription? changedSubscription;
    private RectTransform rowsContainer = null!;
    private LocText statusLabel = null!;
    private bool built;

    /// <summary>Wires services and the shared font. Must be called by the patcher before the screen is shown.</summary>
    public void Configure(
        DuplicantOwnershipRegistry registry,
        DuplicantOwnershipController controller,
        MultiplayerGame multiplayer,
        IMultiplayerClient client,
        ExecutionLevelManager executionLevel,
        EventDispatcher events,
        TMP_FontAsset font,
        float baseFontSize,
        System.Action onClose
    ) {
        this.registry = registry;
        this.controller = controller;
        this.multiplayer = multiplayer;
        this.client = client;
        this.executionLevel = executionLevel;
        this.events = events;
        this.font = font;
        this.baseSize = baseFontSize > 0f ? baseFontSize : 14f;
        this.onClose = onClose;
    }

    protected override void OnActivate() {
        EnsureBuilt();
        changedSubscription ??= events.Subscribe<DuplicantOwnershipChangedEvent>(_ => Refresh());
        Refresh();
    }

    protected override void OnShow(bool show) {
        base.OnShow(show);
        if (show) {
            EnsureBuilt();
            Refresh();
        }
    }

    protected override void OnDeactivate() {
        changedSubscription?.Cancel();
        changedSubscription = null;
    }

    private void EnsureBuilt() {
        if (built)
            return;
        try {
            Build();
            built = true;
        } catch (Exception e) {
            log.Error($"Failed to build duplicant ownership screen: {e}");
        }
    }

    private void Build() {
        // Root panel: fixed width, height driven by its content, anchored below the top bar.
        var root = gameObject.GetComponent<RectTransform>() ?? gameObject.AddComponent<RectTransform>();
        root.anchorMin = root.anchorMax = new Vector2(0.5f, 1f);
        root.pivot = new Vector2(0.5f, 1f);
        root.anchoredPosition = new Vector2(0f, -72f);
        root.sizeDelta = new Vector2(360f, 0f);

        var background = gameObject.AddComponent<Image>();
        background.color = new Color(0.09f, 0.10f, 0.12f, 0.96f);
        background.raycastTarget = true;

        var layout = gameObject.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(16, 16, 14, 14);
        layout.spacing = 6f;
        layout.childControlWidth = layout.childControlHeight = true;
        layout.childForceExpandWidth = true;
        layout.childForceExpandHeight = false;

        var fitter = gameObject.AddComponent<ContentSizeFitter>();
        fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        // Header row: title on the left, close (X) on the right.
        var header = CreateRow(root);
        var title = CreateText(header.transform, "Duplicant Ownership", baseSize * 1.15f, FontStyles.Bold);
        title.alignment = TextAlignmentOptions.Left;
        MakeFlexible(title.gameObject);
        CreateButton(header.transform, "X", () => onClose(), 30f);

        statusLabel = CreateText(root, "", baseSize * 0.95f, FontStyles.Normal);
        statusLabel.alignment = TextAlignmentOptions.Left;

        rowsContainer = CreateColumn(root);
    }

    private void Refresh() {
        if (!built)
            return;
        try {
            RefreshInternal();
        } catch (Exception e) {
            log.Error($"Failed to refresh duplicant ownership screen: {e}");
        }
    }

    private void RefreshInternal() {
        var active = global::Game.Instance != null && executionLevel.LevelIsActive(ExecutionLevel.Multiplayer);
        statusLabel.SetText(!active
            ? "Multiplayer isn't active."
            : registry.Enabled
                ? "Per-player assignment is <b>ON</b>. Click an owner to reassign."
                : "Per-player assignment is <b>OFF</b> (vanilla behaviour).");

        // Rebuild the duplicant rows from scratch each refresh - simplest way to stay in sync. Detach before
        // destroying: Destroy() is deferred to end-of-frame, so unparenting keeps the (still-alive) old rows
        // from being double-counted by the layout group alongside the ones we build below this frame.
        for (var i = rowsContainer.childCount - 1; i >= 0; i--) {
            var child = rowsContainer.GetChild(i);
            child.SetParent(null, false);
            Destroy(child.gameObject);
        }

        if (!active)
            return;

        // Host-only global toggle.
        if (multiplayer.Mode == MultiplayerMode.Host) {
            var enabled = registry.Enabled;
            var toggleRow = CreateRow(rowsContainer);
            var label = CreateText(toggleRow.transform,
                enabled ? "Assignment enabled (host)" : "Assignment disabled (host)", baseSize, FontStyles.Normal);
            label.alignment = TextAlignmentOptions.Left;
            MakeFlexible(label.gameObject);
            CreateButton(toggleRow.transform, enabled ? "Disable" : "Enable",
                () => controller.SetEnabled(!enabled), 76f);
        }

        foreach (var identity in global::Components.LiveMinionIdentities.Items) {
            if (identity == null)
                continue;
            var proxyId = TryGetProxyId(identity);
            var owner = proxyId == null ? null : registry.GetOwner(proxyId);

            var row = CreateRow(rowsContainer);
            var name = CreateText(row.transform, identity.GetProperName(), baseSize, FontStyles.Normal);
            name.alignment = TextAlignmentOptions.Left;
            MakeFlexible(name.gameObject);

            var ownerButton = CreateButton(row.transform, OwnerName(owner),
                () => { if (proxyId != null) CycleOwner(proxyId, owner); }, 130f);
            ownerButton.color = owner == null ? unownedColor : mineColor;
        }
    }

    private void CycleOwner(MultiplayerId proxyId, PlayerIdentity? current) {
        var options = multiplayer.Players
            .OrderByDescending(player => player.Role == PlayerRole.Host)
            .Select(player => (PlayerIdentity?) player.Id)
            .ToList();
        options.Add(null); // Unassigned

        var index = options.FindIndex(option => Equals(option, current));
        if (index < 0)
            index = options.Count - 1;
        var next = options[(index + 1) % options.Count];

        if (multiplayer.Mode == MultiplayerMode.Host)
            controller.Assign(proxyId, next);
        else
            client.Send(new RequestAssignDuplicantOwner(proxyId, next));
    }

    private string OwnerName(PlayerIdentity? owner) {
        if (owner == null)
            return "Unassigned";
        var player = multiplayer.Players.FirstOrDefault(it => it.Id.Equals(owner));
        return player?.Profile.PlayerName ?? "Unknown";
    }

    // --- Tiny UI construction helpers (no PLib in this project) --------------------------------------------

    private RectTransform CreateRow(Transform parent) {
        var go = new GameObject("Row", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var group = go.AddComponent<HorizontalLayoutGroup>();
        group.spacing = 8f;
        group.childControlWidth = group.childControlHeight = true;
        group.childForceExpandWidth = false;
        group.childForceExpandHeight = false;
        var element = go.AddComponent<LayoutElement>();
        element.minHeight = 30f;
        return (RectTransform) go.transform;
    }

    private RectTransform CreateColumn(Transform parent) {
        var go = new GameObject("Column", typeof(RectTransform));
        go.transform.SetParent(parent, false);
        var group = go.AddComponent<VerticalLayoutGroup>();
        group.spacing = 4f;
        group.childControlWidth = group.childControlHeight = true;
        group.childForceExpandWidth = true;
        group.childForceExpandHeight = false;
        return (RectTransform) go.transform;
    }

    private LocText CreateText(Transform parent, string text, float size, FontStyles style) {
        // Create inactive first: LocText.Awake() runs on AddComponent for an active object and does
        // `if (key != "")` with a null default key -> `new StringKey(null)` NREs. Deferring Awake until after
        // we set key = "" (and allowOverride = true to skip its null textStyleSetting path) avoids the crash.
        var go = new GameObject("Text", typeof(RectTransform));
        go.SetActive(false);
        go.transform.SetParent(parent, false);

        var loc = go.AddComponent<LocText>();
        loc.key = "";
        loc.allowOverride = true;
        loc.font = font;
        loc.enableAutoSizing = false; // otherwise TMP ignores fontSize and fits to the (large) default range
        loc.fontSize = size;
        loc.fontStyle = style;
        loc.color = Color.white;
        loc.enableWordWrapping = false;
        loc.overflowMode = TextOverflowModes.Ellipsis;
        loc.alignment = TextAlignmentOptions.Center;
        loc.raycastTarget = false;
        loc.SetText(text);

        go.SetActive(true);
        loc.fontSize = size; // re-assert after Awake/OnEnable, which can reset it back to the default
        return loc;
    }

    private LocText CreateButton(Transform parent, string label, System.Action onClick, float width) {
        var go = new GameObject("Button", typeof(RectTransform));
        go.transform.SetParent(parent, false);

        var image = go.AddComponent<Image>();
        image.color = new Color(0.20f, 0.22f, 0.26f, 1f);

        var button = go.AddComponent<Button>();
        button.targetGraphic = image;
        button.onClick.AddListener(() => {
            try {
                onClick();
            } catch (Exception e) {
                log.Error($"Ownership screen button action failed: {e}");
            }
        });

        var element = go.AddComponent<LayoutElement>();
        element.minWidth = element.preferredWidth = width;
        element.minHeight = 28f;

        var text = CreateText(go.transform, label, baseSize * 0.95f, FontStyles.Normal);
        var textRect = (RectTransform) text.transform;
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(6f, 0f);
        textRect.offsetMax = new Vector2(-6f, 0f);
        return text;
    }

    private static void MakeFlexible(GameObject go) {
        var element = go.GetComponent<LayoutElement>() ?? go.AddComponent<LayoutElement>();
        element.flexibleWidth = 1f;
    }

    private static MultiplayerId? TryGetProxyId(MinionIdentity identity) {
        try {
            return identity.GetMultiplayerInstance().Id;
        } catch {
            return null;
        }
    }

}
