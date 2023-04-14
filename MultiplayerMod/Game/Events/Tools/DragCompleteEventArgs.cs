using System;
using System.Collections.Generic;
using UnityEngine;

namespace MultiplayerMod.Game.Events.Tools;

[Serializable]
public class DragCompleteEventArgs : EventArgs {
    public List<int> Cells { get; set; }
    public Vector3 CursorDown { get; set; }
    public Vector3 CursorUp { get; set; }
    public PrioritySetting Priority { get; set; }
    public string[] Parameters { get; set; }
}
