using System;

namespace MultiplayerMod.Game.Chores.Types;

public record StateTransitionConfig(
    TransitionTypeEnum TransitionType,
    string StateToMonitorName,
    string[] ParameterName
) {

    /// TODO: Adjust client logic to wait until receives args from the host.
    ///
    /// TODO: Execute command on the client
    public static StateTransitionConfig OnEnter(string stateName, params string[] parameterName) =>
        new(TransitionTypeEnum.Enter, stateName, parameterName);

    /// Host:
    ///  - Sends command to all clients upon exit handler call.
    /// Client:
    ///  - Prevents transition to specified state.
    ///  - Transits to WaitHostState instead.
    ///  - Transits to specified state by host upon command.
    ///  - Sets values specified by host upon command.
    public static StateTransitionConfig OnExit(string stateName, params string[] parameterName) =>
        new(TransitionTypeEnum.Exit, stateName, parameterName);

    /// TODO: Adjust client logic (prevent / postpone execution of original handler)
    /// TODO: Execute command on the client
    public static StateTransitionConfig OnMove(string stateName) =>
        new(TransitionTypeEnum.MoveTo, stateName, Array.Empty<string>());

    /// Host:
    ///  - send command to all clients upon update handler call.
    /// Client:
    ///  - prevents update handler call.
    ///  - Receives values from host.
    /// TODO: Trigger command sent on the host
    /// TODO: Execute command on the client
    public static StateTransitionConfig OnUpdate(string stateName, params string[] parameterName) =>
        new(TransitionTypeEnum.Update, stateName, parameterName);

    /// TODO: Adjust client logic (prevent / postpone execution of original handler)
    /// TODO: Execute command on the client
    public static StateTransitionConfig OnEventHandler(string stateName, params string[] parameterName) =>
        new(TransitionTypeEnum.EventHandler, stateName, parameterName);

    public StateMachine.BaseState GetMonitoredState(StateMachine sm) {
        var stateName = StateToMonitorName;
        object findInObject = sm;
        while (stateName.Contains(".")) {
            var firstSplit = StateToMonitorName.IndexOf('.');
            findInObject = findInObject.GetType().GetField(stateName.Substring(0, firstSplit))
                .GetValue(findInObject);
            stateName = stateName.Substring(firstSplit + 1);
        }
        return (StateMachine.BaseState) findInObject.GetType().GetField(stateName).GetValue(findInObject);
    }
}
