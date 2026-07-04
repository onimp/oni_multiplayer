using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.ModRuntime.Context;
using MultiplayerMod.Multiplayer.Commands.Gameplay;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.World;

// Host-authoritative core-simulation replication. Each machine runs ONI's native cell Sim on its own,
// so element / temperature / mass / disease drift apart across the whole map (the dominant source of
// the "N errors" divergence metric, and why gas/oxygen desyncs). The daily hard-reload corrects it but
// only once a cycle. This streams the host's Sim buffers to clients continuously instead.
//
// Bandwidth is bounded by refreshing only one contiguous chunk of cells per send tick and round-robin
// cycling through the whole grid. A cell keeps simulating locally between refreshes; drift is gradual
// so a full-map refresh every (chunkCount * sendPeriod) seconds keeps the two sims tightly aligned.
//
// SyncSimCells rides the dedicated low-priority, unreliable Sim network lane (see NetworkLane.Sim), so
// this bulk traffic no longer head-of-line-blocks latency-sensitive gameplay the way it did on the old
// single reliable stream. That lets us run a much tighter cadence than before (full map every ~16 s vs
// the previous ~2 min) for far better gas/oxygen convergence. The unreliable lane requires each chunk
// to fit in a single network message (no fragmentation) - the round-robin chunk count keeps payloads
// small, and SendChunk guards against an oversized chunk on unusually large maps.
public class SimStateSynchronizer : MultiplayerKMonoBehaviour, IRenderEveryTick {

    // Grid is split into this many contiguous chunks; one chunk is sent per send tick. Full-map refresh
    // takes chunkCount * sendPeriod seconds. Tuning knobs: raise chunkCount for smaller payloads/slower
    // sweep, lower sendPeriod for a faster sweep at higher bandwidth.
    private const int chunkCount = 16;
    private const float sendPeriod = 1.0f;

    // Bytes per cell on the wire (elementIdx 2 + temperature 4 + mass 4 + diseaseIdx 1 + diseaseCount 4).
    // Used to keep a chunk under a single unreliable network message (must not fragment).
    private const int bytesPerCell = 15;
    private const int maxSafeChunkBytes = 400 * 1024; // headroom under the 512 KiB message cap

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<SimStateSynchronizer>();

    [InjectDependency]
    private readonly IMultiplayerServer server = null!;

    [InjectDependency]
    private readonly MultiplayerGame multiplayer = null!;

    [InjectDependency]
    private readonly ExecutionLevelManager manager = null!;

    private float lastTime;
    private int nextChunk;

    public void RenderEveryTick(float dt) {
        if (!manager.LevelIsActive(ExecutionLevel.Multiplayer) || multiplayer.Mode != MultiplayerMode.Host)
            return;
        if (Grid.CellCount <= 0 || server.Clients.Count == 0)
            return;
        if (GameClock.Instance.GetTime() - lastTime < sendPeriod)
            return;

        lastTime = GameClock.Instance.GetTime();
        SendChunk(nextChunk);
        nextChunk = (nextChunk + 1) % chunkCount;
    }

    private unsafe void SendChunk(int chunk) {
        var cellCount = Grid.CellCount;
        var chunkSize = (cellCount + chunkCount - 1) / chunkCount;
        var startCell = chunk * chunkSize;
        if (startCell >= cellCount)
            return;
        var count = chunkSize;
        if (startCell + count > cellCount)
            count = cellCount - startCell;

        // The Sim lane is unreliable, so the chunk must fit in one (unfragmented) network message. This
        // holds for all standard maps at the current chunkCount; warn (rather than silently fragment and
        // drop under unreliable delivery) if an unusually large map pushes a chunk over the limit.
        if (count * bytesPerCell > maxSafeChunkBytes)
            log.Warning(
                $"Sim chunk of {count} cells (~{count * bytesPerCell / 1024} KiB) may fragment on the " +
                "unreliable lane; raise SimStateSynchronizer.chunkCount"
            );

        var elementIdx = new ushort[count];
        var temperature = new float[count];
        var mass = new float[count];
        var diseaseIdx = new byte[count];
        var diseaseCount = new int[count];
        for (var i = 0; i < count; i++) {
            var cell = startCell + i;
            elementIdx[i] = Grid.elementIdx[cell];
            temperature[i] = Grid.temperature[cell];
            mass[i] = Grid.mass[cell];
            diseaseIdx[i] = Grid.diseaseIdx[cell];
            diseaseCount[i] = Grid.diseaseCount[cell];
        }

        server.Send(new SyncSimCells(startCell, elementIdx, temperature, mass, diseaseIdx, diseaseCount));
    }

}
