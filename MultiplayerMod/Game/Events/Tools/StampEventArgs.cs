using System;
using UnityEngine;

namespace MultiplayerMod.Game.Events.Tools;

[Serializable]
public class StampEventArgs {
    public TemplateContainer Template { set; get; }
    public Vector2 Location { set; get; }
}
