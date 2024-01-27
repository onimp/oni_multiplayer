﻿using System;

namespace MultiplayerMod.Game.Chores.Types;

public record StateTransitionConfig(
    TransitionTypeEnum TransitionType,
    string StateToMonitorName,
    GameHashes? EventGameHash,
    string[] ParameterName
) {

    public static StateTransitionConfig OnEnter(string stateName, params string[] parameterName) =>
        new(TransitionTypeEnum.Enter, stateName, null, parameterName);

    /// Host:
    ///  - Sends command to all clients upon exit handler call.
    /// Client:
    ///  - Prevents transition to specified state.
    ///  - Transits to WaitHostState instead.
    ///  - Transits to specified state by host upon command.
    ///  - Sets values specified by host upon command.
    public static StateTransitionConfig OnExit(string stateName, params string[] parameterName) =>
        new(TransitionTypeEnum.Exit, stateName, null, parameterName);

    /// TODO: Adjust client logic (prevent / postpone execution of original handler)
    /// TODO: Execute command on the client
    public static StateTransitionConfig OnMove(string stateName) =>
        new(TransitionTypeEnum.MoveTo, stateName, null, Array.Empty<string>());

    /// Host:
    ///  - send command to all clients upon update handler call.
    /// Client:
    ///  - prevents update handler call.
    ///  - Receives values from host.
    public static StateTransitionConfig OnUpdate(string stateName, params string[] parameterName) =>
        new(TransitionTypeEnum.Update, stateName, null, parameterName);

    /// Host:
    ///  - send command to all clients upon event.
    /// Client:
    ///  - prevents event handler call.
    ///  - Receives values from host.
    public static StateTransitionConfig OnEventHandler(
        string stateName,
        GameHashes eventGameHash,
        params string[] parameterName
    ) => new(TransitionTypeEnum.EventHandler, stateName, eventGameHash, parameterName);

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