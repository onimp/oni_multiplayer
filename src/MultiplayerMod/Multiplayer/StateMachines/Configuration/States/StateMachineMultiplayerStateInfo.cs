namespace MultiplayerMod.Multiplayer.StateMachines.Configuration.States;

public class StateMachineMultiplayerStateInfo(string name) {
    public string Name { get; } = name;
    public string ReferenceName { get; } = $"root.{name}";
}
