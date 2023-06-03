using System;
using MultiplayerMod.Multiplayer.Extensions;

namespace MultiplayerMod.Multiplayer.Commands.Screens.Skill;

[Serializable]
public class MasterSkill : IMultiplayerCommand {

    private readonly string properName;
    private readonly string skillId;

    public MasterSkill(string properName, string skillId) {
        this.properName = properName;
        this.skillId = skillId;
    }

    public void Execute() {
        var minionIdentity = MinionIdentityUtils.GetLiveMinion(properName);

        var component = minionIdentity.GetComponent<MinionResume>();
        if (DebugHandler.InstantBuildMode && component.AvailableSkillpoints < 1)
            component.ForceAddSkillPoint();
        var masteryConditions = component.GetSkillMasteryConditions(skillId);
        if (!component.CanMasterSkill(masteryConditions)) return;
        if (component.HasMasteredSkill(skillId)) return;

        component.MasterSkill(skillId);

        ManagementMenu.Instance.skillsScreen.RefreshAll();
    }
}
