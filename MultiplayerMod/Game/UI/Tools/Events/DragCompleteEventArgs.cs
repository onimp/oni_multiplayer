using System;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerMod.Game.UI.Tools.Events;

[Serializable]
public record DragCompleteEventArgs(
    List<int> Cells,
    Vector3 CursorDown,
    Vector3 CursorUp,
    PrioritySetting Priority,
    string[]? Parameters
);
