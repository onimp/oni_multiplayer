using System;
using UnityEngine;

namespace MultiplayerMod.Game.UI.Tools.Events;

[Serializable]
public record StampEventArgs(
    TemplateContainer Template,
    Vector2 Location
);
