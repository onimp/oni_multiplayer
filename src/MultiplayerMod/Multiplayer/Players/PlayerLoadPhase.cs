namespace MultiplayerMod.Multiplayer.Players;

/// <summary>
/// Cosmetic, fine-grained progress of a player through a hard-sync (world save transfer + reload).
/// Purely for the loading-status UI — it is deliberately separate from <see cref="PlayerState"/>, which
/// remains the authoritative readiness gate (<c>Players.Ready</c>). A player whose <see cref="PlayerState"/>
/// is <see cref="PlayerState.Ready"/> has finished regardless of the last phase reported here.
/// </summary>
public enum PlayerLoadPhase {
    None,
    Preparing,    // host is saving + packaging the world
    Transferring, // client received the save and is writing it to disk
    Loading,      // client is running the full ONI world reload (LoadScreen.DoLoad)
    Reconciling   // client reload finished, applying the multiplayer world state
}
