using System;
using System.Linq;

namespace MultiplayerMod.Multiplayer.Commands.Screens.Priorities;

[Serializable]
public class SetDefaultPriority : IMultiplayerCommand {

    private readonly string choreGroupId;
    private readonly int value;

    private ChoreGroup ChoreGroup =>
        Db.Get().ChoreGroups.resources.FirstOrDefault(resource => resource.Id == choreGroupId);

    public SetDefaultPriority(ChoreGroup choreGroup, int value) {
        choreGroupId = choreGroup.Id;
        this.value = value;
    }

    public void Execute() {
        global::Immigration.Instance.SetPersonalPriority(ChoreGroup, value);
        RefreshTable();
    }

    private void RefreshTable() {
        var screen = ManagementMenu.Instance.jobsScreen;
        foreach (var row in screen.rows) {
            var minion = row.GetIdentity();
            foreach (var widget in row.widgets.Where(entry => entry.Key is PrioritizationGroupTableColumn)
                         .Select(entry => entry.Value)) {
                screen.LoadValue(minion, widget);
            }
        }
    }

}
