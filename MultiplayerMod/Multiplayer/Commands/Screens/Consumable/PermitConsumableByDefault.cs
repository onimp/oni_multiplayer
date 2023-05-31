using System;
using System.Collections.Generic;
using System.Linq;

namespace MultiplayerMod.Multiplayer.Commands.Screens.Consumable;

[Serializable]
public class PermitConsumableByDefault : IMultiplayerCommand {
    private readonly List<Tag> permittedList;

    public PermitConsumableByDefault(List<Tag> permittedList) {
        this.permittedList = permittedList;
    }

    public void Execute() {
        ConsumerManager.instance.DefaultForbiddenTagsList.Clear();
        ConsumerManager.instance.DefaultForbiddenTagsList.AddRange(permittedList);
        var screen = ManagementMenu.Instance.consumablesScreen;
        foreach (var row in screen.rows.Where(row => row.rowType == TableRow.RowType.Default)) {
            foreach (var widget in row.widgets.Where(entry => entry.Key is ConsumableInfoTableColumn)
                         .Select(entry => entry.Value)) {
                screen.on_load_consumable_info(null, widget);
            }
        }
    }
}
