using System;
using MultiplayerMod.Multiplayer.Objects.Extensions;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Commands.Screens.Skill;

[Serializable]
public class SetHat : MultiplayerCommand {

    private readonly GameObjectReference minionIdentityReference;
    private readonly string? targetHat;

    public SetHat(MinionIdentity minionIdentity, string? targetHat) {
        minionIdentityReference = minionIdentity.gameObject.GetReference();
        this.targetHat = targetHat;
    }

    public override void Execute(MultiplayerCommandContext context) {
        var resume = minionIdentityReference.GetComponent<MinionResume>();
        if (resume == null)
            return;

        resume.SetHats(resume.currentHat, targetHat);
        if (targetHat != null) {
            if (resume.OwnsHat(targetHat)) {
                var unused = new PutOnHatChore(resume, Db.Get().ChoreTypes.SwitchHat);
            }
        } else {
            resume.ApplyTargetHat();
        }

        ManagementMenu.Instance.skillsScreen.RefreshAll();
    }
}
