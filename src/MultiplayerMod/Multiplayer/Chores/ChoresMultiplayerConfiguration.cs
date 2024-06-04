using static MultiplayerMod.Multiplayer.Chores.Dsl.ChoresConfigurationDsl;

namespace MultiplayerMod.Multiplayer.Chores;

public static class ChoresMultiplayerConfiguration {

    public static readonly ChoreConfiguration[] Configuration = [
        Synchronize<AttackChore>()
    ];

}
