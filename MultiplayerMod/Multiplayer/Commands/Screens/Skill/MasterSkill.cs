using System;
using MultiplayerMod.Multiplayer.Objects;

namespace MultiplayerMod.Multiplayer.Commands.Screens.Skill;

[Serializable]
public class MasterSkill : IMultiplayerCommand {

    private readonly MultiplayerReference minionIdentityReference;
    private readonly string skillId;

    public MasterSkill(MinionIdentity minionIdentity, string skillId) {
        minionIdentityReference = minionIdentity.GetMultiplayerReference();
        this.skillId = skillId;
    }

    public void Execute() {
        var component = minionIdentityReference.GetComponent<MinionResume>();
        if (component == null) return;

        if (DebugHandler.InstantBuildMode && component.AvailableSkillpoints < 1)
            component.ForceAddSkillPoint();
        var masteryConditions = component.GetSkillMasteryConditions(skillId);
        if (!component.CanMasterSkill(masteryConditions)) return;
        if (component.HasMasteredSkill(skillId)) return;

        component.MasterSkill(skillId);

        ManagementMenu.Instance.skillsScreen.RefreshAll();
    }
}
