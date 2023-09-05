using System;
using System.Linq;
using MultiplayerMod.Core.Logging;

namespace MultiplayerMod.Multiplayer.Commands.Screens.Research;

[Serializable]
public abstract class AbstractResearchEntryCommand : MultiplayerCommand {

    private static Core.Logging.Logger log = LoggerFactory.GetLogger<AbstractResearchEntryCommand>();

    private string techId;

    protected AbstractResearchEntryCommand(string techId) {
        this.techId = techId;
    }

    protected abstract void Execute(ResearchEntry researchEntry);

    public override void Execute(MultiplayerCommandContext context) {
        var screen = ManagementMenu.Instance.researchScreen;
        var entry = screen.entryMap.Values.FirstOrDefault(entry => entry.targetTech.Id == techId);
        if (entry == null) {
            log.Warning($"Tech {techId} is not found.");
            return;
        }
        Execute(entry);
    }
}
