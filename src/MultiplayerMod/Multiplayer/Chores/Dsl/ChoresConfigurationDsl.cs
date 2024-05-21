namespace MultiplayerMod.Multiplayer.Chores.Dsl;

public static class ChoresConfigurationDsl {

    public static ChoreConfiguration Synchronize<TChore>() => new(typeof(TChore), null);

}
