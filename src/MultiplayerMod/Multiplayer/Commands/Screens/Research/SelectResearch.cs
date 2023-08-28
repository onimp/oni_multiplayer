using System;

namespace MultiplayerMod.Multiplayer.Commands.Screens.Research;

[Serializable]
public class SelectResearch : AbstractResearchEntryCommand {

    public SelectResearch(string techId) : base(techId) { }

    protected override void Execute(ResearchEntry researchEntry) {
        researchEntry.OnResearchClicked();
    }
}
