using System;

namespace MultiplayerMod.Multiplayer.Commands.Screens.Research;

[Serializable]
public class CancelResearch : AbstractResearchEntryCommand {
    public CancelResearch(string techId) : base(techId) { }

    protected override void Execute(ResearchEntry researchEntry) {
        researchEntry.OnResearchCanceled();
    }
}
