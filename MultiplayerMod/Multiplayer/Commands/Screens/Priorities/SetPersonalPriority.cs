using System;
using System.Linq;
using MultiplayerMod.Core.Logging;

namespace MultiplayerMod.Multiplayer.Commands.Screens.Priorities;

[Serializable]
public class SetPersonalPriority : IMultiplayerCommand {
    private static Core.Logging.Logger log = LoggerFactory.GetLogger<SetPersonalPriority>();

    private readonly string properName;
    private readonly string choreGroup;

    private readonly int value;

    private IPersonalPriorityManager PersonalPriorityManager {
        get {
            if (properName == null) return Immigration.Instance;

            var minionIdentity =
                global::Components.LiveMinionIdentities.Items.FirstOrDefault(
                    minion => minion.GetProperName() == properName
                );
            if (minionIdentity != null) return minionIdentity.GetComponent<ChoreConsumer>();

            log.Warning($"Minion {properName} is not found.");
            return null;
        }
    }

    private ChoreGroup ChoreGroup =>
        Db.Get().ChoreGroups.resources.FirstOrDefault(resource => resource.Id == choreGroup);

    public SetPersonalPriority(string properName, string choreGroup, int value) {
        this.properName = properName;
        this.choreGroup = choreGroup;
        this.value = value;
    }

    public void Execute() {
        PersonalPriorityManager?.SetPersonalPriority(ChoreGroup, value);
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
