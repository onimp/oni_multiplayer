using System;
using System.Linq;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Commands.Screens.Consumable;

[Serializable]
public class PermitConsumableToMinion : MultiplayerCommand {

    private readonly GameObjectReference consumableConsumerReference;
    private readonly string consumableId;
    private readonly bool isAllowed;

    public PermitConsumableToMinion(ConsumableConsumer consumableConsumer, string consumableId, bool isAllowed) {
        consumableConsumerReference = consumableConsumer.gameObject.GetMultiplayerReference();
        this.consumableId = consumableId;
        this.isAllowed = isAllowed;
    }

    public override void Execute(Runtime runtime) {
        var consumableConsumer = consumableConsumerReference.GetComponent<ConsumableConsumer>();
        if (consumableConsumer == null) return;

        consumableConsumer.SetPermitted(consumableId, isAllowed);
        RefreshTable();
    }

    private void RefreshTable() {
        var screen = ManagementMenu.Instance.consumablesScreen;
        foreach (var row in screen.rows) {
            var minion = row.GetIdentity();
            row.widgets.Where(entry => entry.Key is ConsumableInfoTableColumn)
                .Select(entry => entry.Value).ForEach(
                    widget => screen.on_load_consumable_info(minion, widget)
                );
        }
    }
}
