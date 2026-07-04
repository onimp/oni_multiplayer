using MultiplayerMod.Core.Dependency;
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
// This traffic shares the single reliable, in-order network stream with latency-sensitive gameplay
// commands (movement, chores), so it must stay light or it becomes head-of-line blocking: bulk cell
// data queued ahead of a movement command delays that command, and if the stream can't drain fast
// enough the backlog (and lag) grows. Hence the deliberately low defaults below (~1 small chunk / 2 s,
// full map every ~2 min). The daily hard-sync and the manual force-sync button are the backstops for
// the wider drift this allows. (A proper fix for tight-sync-without-lag is a lower-priority network
// lane for bulk traffic; not done yet.)
public class SimStateSynchronizer : MultiplayerKMonoBehaviour, IRenderEveryTick {

    // Grid is split into this many contiguous chunks; one chunk is sent per send tick. More chunks =
    // smaller per-send payload = lower peak bandwidth (but a slower full-map refresh).
    private const int chunkCount = 64;
    private const float sendPeriod = 2.0f;

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
