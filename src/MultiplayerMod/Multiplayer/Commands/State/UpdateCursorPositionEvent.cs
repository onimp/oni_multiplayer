using System;
using MultiplayerMod.Network;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Commands.State;

[Serializable]
public record UpdateCursorPositionEvent(IMultiplayerClientId Player, Vector2 Position);
