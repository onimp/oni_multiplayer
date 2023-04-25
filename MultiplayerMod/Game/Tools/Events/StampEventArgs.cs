using System;
using UnityEngine;

namespace MultiplayerMod.Game.Tools.Events;

[Serializable]
public class StampEventArgs {
    public TemplateContainer Template { set; get; }
    public Vector2 Location { set; get; }
}
