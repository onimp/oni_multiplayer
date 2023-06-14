using System;
using System.Linq;
using MultiplayerMod.Multiplayer.Extensions;

namespace MultiplayerMod.Multiplayer.Commands.Screens.Priorities;

[Serializable]
public class SetPersonalPriority : IMultiplayerCommand {

    private readonly string properName;
    private readonly string choreGroup;

    private readonly int value;

    private ChoreGroup ChoreGroup =>
        Db.Get().ChoreGroups.resources.FirstOrDefault(resource => resource.Id == choreGroup);

    public SetPersonalPriority(string properName, string choreGroup, int value) {
        this.properName = properName;
        this.choreGroup = choreGroup;
        this.value = value;
    }

    public void Execute() {
        var identity = MinionIdentityUtils.GetLiveMinion(properName);
        if (identity == null) return;
        identity.GetComponent<ChoreConsumer>().SetPersonalPriority(ChoreGroup, value);
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
