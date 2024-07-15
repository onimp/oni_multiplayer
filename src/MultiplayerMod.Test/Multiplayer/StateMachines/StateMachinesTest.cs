using System;
using MultiplayerMod.Test.GameRuntime;
using NUnit.Framework;
using UnityEngine;

namespace MultiplayerMod.Test.Multiplayer.StateMachines;

[TestFixture]
public class StateMachinesTest : PlayableGameTest {

    [TearDown]
    private void StateMachinesTearDown() {
        StateMachineManager.Instance.stateMachines.Clear();
    }

    protected static T CreateStateMachineInstance<T>() where T : StateMachine.Instance {
        var go = new GameObject();
        var target = go.AddComponent<TestTarget>();
        target.Awake();
        return (T) Activator.CreateInstance(typeof(T), [target]);
    }

    public class TestTarget : KMonoBehaviour;

}
