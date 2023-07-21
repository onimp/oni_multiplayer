using System;
using MultiplayerMod.Core.Patch.Capture;
using MultiplayerMod.Game.Mechanics.Minions;
using MultiplayerMod.Game.Mechanics.Printing;
using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay;

[Serializable]
public class AcceptDelivery : IMultiplayerCommand {

    private AcceptDeliveryEventArgs args;

    public AcceptDelivery(AcceptDeliveryEventArgs args) {
        this.args = args;
    }

    public void Execute() {
        var telepad = args.Target.GetComponent<Telepad>();
        var capture = LocalCaptor.Capture<TelepadAcceptDeliveryCapture>(
            () => telepad.OnAcceptDelivery(args.Deliverable)
        );

        var minionMultiplayer = capture.Instance.GetComponent<MultiplayerInstance>();
        minionMultiplayer.Id = args.MinionId;
        minionMultiplayer.Register();

        var proxyMultiplayer = capture.Instance.GetComponent<MinionIdentity>().GetMultiplayerInstance();
        proxyMultiplayer.Id = args.ProxyId;
        proxyMultiplayer.Register();
    }

}
