using System;
using System.Linq;
using MultiplayerMod.Core.Extensions;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Commands.Screens.Priorities;

[Serializable]
public class SetPersonalPriority : IMultiplayerCommand {

    private readonly GameObjectReference choreConsumerReference;
    private readonly string choreGroupId;
    private readonly int value;

    private ChoreGroup? ChoreGroup =>
        Db.Get().ChoreGroups.resources.FirstOrDefault(resource => resource.Id == choreGroupId);

    public SetPersonalPriority(ChoreConsumer choreConsumer, ChoreGroup choreGroup, int value) {
        choreConsumerReference = choreConsumer.gameObject.GetMultiplayerReference();
        choreGroupId = choreGroup.Id;
        this.value = value;
    }

    public void Execute() {
        var choreConsumer = choreConsumerReference.GetComponent<ChoreConsumer>();
        if (choreConsumer == null) return;

        choreConsumer.SetPersonalPriority(ChoreGroup, value);
        RefreshTable();
    }

    private void RefreshTable() {
        var screen = ManagementMenu.Instance.jobsScreen;
        foreach (var row in screen.rows) {
            var minion = row.GetIdentity();
            row.widgets.Where(entry => entry.Key is PrioritizationGroupTableColumn)
                .Select(entry => entry.Value).ForEach(
                    widget => screen.LoadValue(minion, widget)
                );
        }
    }
}
