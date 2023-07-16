using System;
using UnityEngine;

namespace MultiplayerMod.Game.UI.Tools.Events;

[Serializable]
public class StampEventArgs {
    public TemplateContainer Template { get; }
    public Vector2 Location { get; }

    public StampEventArgs(TemplateContainer template, Vector2 location) {
        Template = template;
        Location = location;
    }
}
