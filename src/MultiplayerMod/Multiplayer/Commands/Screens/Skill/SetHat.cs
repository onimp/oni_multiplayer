using System;
using MultiplayerMod.ModRuntime;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Commands.Screens.Skill;

[Serializable]
public class SetHat : MultiplayerCommand {

    private readonly GameObjectReference minionIdentityReference;
    private readonly string? targetHat;

    public SetHat(MinionIdentity minionIdentity, string? targetHat) {
        minionIdentityReference = minionIdentity.gameObject.GetMultiplayerReference();
        this.targetHat = targetHat;
    }

    public override void Execute(Runtime runtime) {
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
