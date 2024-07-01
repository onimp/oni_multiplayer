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

    public override void Execute(MultiplayerCommandContext context) {
        var telepad = args.Target.Resolve();
        var capture = LocalCaptor.Capture<TelepadAcceptDeliveryCapture>(
            () => telepad.OnAcceptDelivery(args.Deliverable)
        );

        var minionMultiplayer = capture.Instance.GetComponent<MultiplayerInstance>();
        minionMultiplayer.Register(args.GameObjectId);

        var minionIdentity = capture.Instance.GetComponent<MinionIdentity>();
        if (minionIdentity != null) {
            var proxyMultiplayer = minionIdentity.GetMultiplayerInstance();
            proxyMultiplayer.Register(args.ProxyId);
        }

        ImmigrantScreenPatch.Deliverables = null;
    }

}
