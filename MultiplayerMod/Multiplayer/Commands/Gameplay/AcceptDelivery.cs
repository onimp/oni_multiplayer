using System;
using MultiplayerMod.Core.Patch.Capture;
using MultiplayerMod.Game.Mechanics.Minions;
using MultiplayerMod.Game.Mechanics.Printing;
using MultiplayerMod.Game.UI.Screens;
using MultiplayerMod.Multiplayer.Objects;
using static MultiplayerMod.Game.Mechanics.Printing.TelepadEvents;

namespace MultiplayerMod.Multiplayer.Commands.Gameplay;

[Serializable]
public class AcceptDelivery : MultiplayerCommand {

    private AcceptDeliveryEventArgs args;

    public AcceptDelivery(AcceptDeliveryEventArgs args) {
        this.args = args;
    }

    public override void Execute() {
        var telepad = args.Target.GetComponent();
        var capture = LocalCaptor.Capture<TelepadAcceptDeliveryCapture>(
            () => telepad.OnAcceptDelivery(args.Deliverable)
        );

        var minionMultiplayer = capture.Instance.GetComponent<MultiplayerInstance>();
        minionMultiplayer.Id = args.GameObjectId;
        minionMultiplayer.Register();

        var minionIdentity = capture.Instance.GetComponent<MinionIdentity>();
        if (minionIdentity != null) {
            var proxyMultiplayer = minionIdentity.GetMultiplayerInstance();
            proxyMultiplayer.Id = args.ProxyId;
            proxyMultiplayer.Register();
        }

        ImmigrantScreenPatch.Deliverables = null;
    }

}
