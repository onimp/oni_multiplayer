namespace MultiplayerMod.Game.Chores.Types;

public record ChoreSyncConfig(
    CreationStatusEnum CreationSync,
    GlobalChoreProviderStatusEnum GlobalChoreProviderSync,
    StatesTransitionConfig StatesTransitionSync
) {
    public static ChoreSyncConfig FullyDeterminedByInput(
        GlobalChoreProviderStatusEnum globalChoreStatus = GlobalChoreProviderStatusEnum.Off
    ) {
        return new ChoreSyncConfig(
            CreationStatusEnum.On,
            globalChoreStatus,
            StatesTransitionConfig.Disabled()
        );
    }

    public static ChoreSyncConfig Dynamic(StatesTransitionConfig statesTransitionConfig) {
        return new ChoreSyncConfig(
            CreationStatusEnum.On,
            GlobalChoreProviderStatusEnum.Off,
            statesTransitionConfig
        );
    }
}
