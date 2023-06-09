using System;
using MultiplayerMod.Multiplayer.Extensions;

namespace MultiplayerMod.Multiplayer.Commands.Screens.Skill;

[Serializable]
public class SetHat : IMultiplayerCommand {

    private string properName;
    private string targetHat;

    public SetHat(string properName, string targetHat) {
        this.properName = properName;
        this.targetHat = targetHat;
    }

    public void Execute() {
        var minion = MinionIdentityUtils.GetLiveMinion(properName);
        var resume = minion.GetComponent<MinionResume>();
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
