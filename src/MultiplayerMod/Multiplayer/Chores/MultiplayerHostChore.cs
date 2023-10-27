using System;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Chores;

public class MultiplayerHostChore {

    public int WorldId { get; }
    public ChoreProvider Provider { get; }
    public Type ChoreInstanceType { get; }
    public ChoreType ChoreType { get; }
    public GameObject Target { get; }
    public ChoreDriver Driver { get; }
    public bool Preemptable { get; }

    public MultiplayerHostChore(
        int worldId,
        ChoreProvider provider,
        Type choreInstanceType,
        ChoreType choreType,
        GameObject target,
        ChoreDriver driver,
        bool preemptable
    ) {
        WorldId = worldId;
        Provider = provider;
        ChoreInstanceType = choreInstanceType;
        ChoreType = choreType;
        Target = target;
        Driver = driver;
        Preemptable = preemptable;
    }

    public override string ToString() =>
        $"MultiplayerHostChore(World={WorldId}, Provider={Provider.GetProperName()}, ChoreTypeId={ChoreType.Id}, InstanceType={ChoreInstanceType.Name}, Target={Target.GetProperName()}, Driver={Driver.GetProperName()}, Preemptable={Preemptable})";

}
