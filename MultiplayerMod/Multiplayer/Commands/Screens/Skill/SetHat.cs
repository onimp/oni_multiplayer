using System;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Commands.Screens.Skill;

[Serializable]
public class SetHat : IMultiplayerCommand {

    private readonly GameObjectReference minionIdentityReference;
    private readonly string? targetHat;

    public SetHat(MinionIdentity minionIdentity, string? targetHat) {
        minionIdentityReference = minionIdentity.GetMultiplayerReference();
        this.targetHat = targetHat;
    }

    public void Execute() {
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
