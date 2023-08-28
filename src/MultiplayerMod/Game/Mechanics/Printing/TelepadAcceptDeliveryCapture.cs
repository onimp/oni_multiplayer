using MultiplayerMod.Core.Patch.Capture;
using UnityEngine;

namespace MultiplayerMod.Game.Mechanics.Printing;

[LocalVariableCapture(typeof(Telepad), nameof(Telepad.OnAcceptDelivery))]
public class TelepadAcceptDeliveryCapture : ILocalCapture {

    [LocalVariable(1)]
    public GameObject Instance { get; private set; } = null!;

}
