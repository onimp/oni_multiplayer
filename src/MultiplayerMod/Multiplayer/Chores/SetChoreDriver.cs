using System;
using System.Collections.Generic;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Multiplayer.Commands;
using MultiplayerMod.Multiplayer.Objects.Reference;

namespace MultiplayerMod.Multiplayer.Chores;

[Serializable]
public class SetChoreDriver : MultiplayerCommand {

    private static Core.Logging.Logger log = LoggerFactory.GetLogger<SetChoreDriver>();

    private ComponentReference<ChoreDriver> choreDriverRef;
    private Type choreInstanceType;
    private string choreTypeId;
    private ComponentReference<ChoreProvider> providerRef;
    private int worldId;
    private GameObjectReference targetRef;
    private bool preemtable;

    public SetChoreDriver(
        ComponentReference<ChoreDriver> choreDriverRef,
        Type choreInstanceType,
        string choreTypeId,
        ComponentReference<ChoreProvider> providerRef,
        int worldId,
        GameObjectReference targetRef,
        bool preemtable
    ) {
        this.choreDriverRef = choreDriverRef;
        this.choreInstanceType = choreInstanceType;
        this.choreTypeId = choreTypeId;
        this.providerRef = providerRef;
        this.worldId = worldId;
        this.targetRef = targetRef;
        this.preemtable = preemtable;
    }

    public override void Execute(MultiplayerCommandContext context) {
        var mChore = new MultiplayerHostChore(
            worldId,
            providerRef.GetComponent(),
            choreInstanceType,
            Db.Get().ChoreTypes.Get(choreTypeId),
            targetRef.GetGameObject(),
            choreDriverRef.GetComponent(),
            preemtable
        );

        var pool = MultiplayerHostChores.HostDriversPool;
        if (!pool.TryGetValue(mChore.WorldId, out var worldChores)) {
            worldChores = new Dictionary<ChoreProvider, List<MultiplayerHostChore>>();
            pool[mChore.WorldId] = worldChores;
        }
        if (!worldChores.TryGetValue(mChore.Provider, out var providerChores)) {
            providerChores = new List<MultiplayerHostChore>();
            worldChores[mChore.Provider] = providerChores;
        }
        providerChores.Add(mChore);

        log.Debug($"New pooled host chore {mChore}");
    }

}
