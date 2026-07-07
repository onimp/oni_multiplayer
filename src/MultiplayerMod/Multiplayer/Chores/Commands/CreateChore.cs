using System;
using System.Linq;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Multiplayer.Chores.Serialization;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Objects;
using MultiplayerMod.Multiplayer.Objects.Extensions;

namespace MultiplayerMod.Multiplayer.Chores.Commands;

[Serializable]
public class CreateChore : MultiplayerCommand {

    private static Core.Logging.Logger log = LoggerFactory.GetLogger<CreateChore>();

    public new readonly MultiplayerId Id;
    public readonly Type ChoreType;
    public readonly object?[] Arguments;

    public CreateChore(MultiplayerId id, Type choreType, object?[] arguments) {
        Id = id;
        ChoreType = choreType;
        Arguments = ArgumentUtils.WrapObjects(ChoreArgumentsWrapper.Wrap(ChoreType, arguments));
    }

    public override void Execute(MultiplayerCommandContext context) {
        // Reconstructing a host chore can fail on the client (an argument references an object not present
        // here, or a chore ctor touches state the client hasn't set up). Guard it like the other
        // reconstruction sites (RuntimeChoresStateManager / SetDriverChore) so one bad chore is logged and
        // skipped instead of aborting command processing - the dupe reconciles at the next host drive / hard-sync.
        try {
            var args = ChoreArgumentsWrapper.Reconstruct(ChoreType, Arguments);
            EnsureRequiredSensors(ChoreType, args);
            log.Debug($"Create chore {ChoreType} [id={Id}]");
            var chore = (Chore) ChoreType.GetConstructors()[0].Invoke(args);
            chore.Register(Id);
        } catch (Exception exception) {
            log.Warning($"Unable to create chore {ChoreType} [id={Id}]: {exception.Message}");
        }
    }

    // IdleChore.StatesInstance's constructor immediately does GetComponent<Sensors>().GetSensor<IdleCellSensor>().
    // Sensors.GetSensor logs "Missing sensor of type: IdleCellSensor" and returns null when the sensor isn't in
    // the target dupe's list yet - error spam plus a half-built idle chore. The sensor is normally added in the
    // minion's BaseOnSpawn; when the host broadcasts an IdleChore before that ran on this client, add it here
    // (checked against the public sensor list, which does NOT log, unlike GetSensor) so construction resolves a
    // real sensor. A later BaseOnSpawn Add is harmless - GetSensor returns the first match.
    private static void EnsureRequiredSensors(Type choreType, object?[] args) {
        if (choreType != typeof(IdleChore))
            return;

        var target = args.OfType<IStateMachineTarget>().FirstOrDefault();
        var sensors = target?.gameObject != null ? target.gameObject.GetComponent<Sensors>() : null;
        if (sensors == null)
            return;

        if (!sensors.sensors.Any(sensor => sensor is IdleCellSensor))
            sensors.Add(new IdleCellSensor(sensors));
    }

}
