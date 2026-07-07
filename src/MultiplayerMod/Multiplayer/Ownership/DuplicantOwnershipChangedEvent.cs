using MultiplayerMod.Core.Events;

namespace MultiplayerMod.Multiplayer.Ownership;

/// <summary>
/// Raised on every machine whenever duplicant ownership or the feature-enabled flag changes, so UI can
/// refresh. Not a network command - the commands that mutate the registry dispatch this locally after
/// applying their change.
/// </summary>
public record DuplicantOwnershipChangedEvent : IDispatchableEvent;
