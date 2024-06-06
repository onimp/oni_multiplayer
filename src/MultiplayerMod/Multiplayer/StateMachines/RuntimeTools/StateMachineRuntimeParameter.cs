using System.Reflection;
using MultiplayerMod.Game.NameOf;

namespace MultiplayerMod.Multiplayer.StateMachines.RuntimeTools;

public class StateMachineRuntimeParameter<T>(StateMachine.Instance instance, StateMachine.Parameter parameter) {

    private readonly MethodBase setter = parameter.GetType()
        .GetMethod(nameof(StateMachineMemberReference.Parameter.Set))!;

    public void Set(T value, bool silenceEvents = false) => setter.Invoke(parameter, [value, instance, silenceEvents]);

}
