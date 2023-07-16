using System;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerMod.Game.UI.Tools.Events;

[Serializable]
public class DragCompleteEventArgs : EventArgs {
    public List<int> Cells { get; }
    public Vector3 CursorDown { get; }
    public Vector3 CursorUp { get; }
    public PrioritySetting Priority { get; }
    public string[]? Parameters { get; }

    public DragCompleteEventArgs(
        List<int> cells,
        Vector3 cursorDown,
        Vector3 cursorUp,
        PrioritySetting priority,
        string[]? parameters
    ) {
        Cells = cells;
        CursorDown = cursorDown;
        CursorUp = cursorUp;
        Priority = priority;
        Parameters = parameters;
    }
}
