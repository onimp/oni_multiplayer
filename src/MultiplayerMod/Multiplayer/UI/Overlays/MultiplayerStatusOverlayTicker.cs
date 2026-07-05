using System;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.UI.Overlays;

/// <summary>
/// Rebuilds <see cref="MultiplayerStatusOverlay"/> text every frame from a supplier, so live content
/// (e.g. the per-player hard-sync elapsed timers) animates smoothly. Attached to the loading overlay
/// GameObject by <see cref="MultiplayerStatusOverlay"/>.
/// </summary>
public class MultiplayerStatusOverlayTicker : MonoBehaviour {

    public Func<string>? Supplier;
    public Action<string>? Apply;

    private void Update() {
        if (Supplier != null && Apply != null)
            Apply(Supplier());
    }

}
