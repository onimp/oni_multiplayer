using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Core.Scheduling;
using MultiplayerMod.Multiplayer.Players.Events;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Players;

// todo: Should be replaced with something more visual effective
[UsedImplicitly]
public class GameLeaveRequestGuard {

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<GameLeaveRequestGuard>();
    private const int guardIntervalMs = 2000;

    private readonly IMultiplayerClient client;
    private readonly UnityTaskScheduler scheduler;

    private CancellationTokenSource? cancellationTokenSource;

    public GameLeaveRequestGuard(EventDispatcher dispatcher, IMultiplayerClient client, UnityTaskScheduler scheduler) {
        this.client = client;
        this.scheduler = scheduler;
        dispatcher.Subscribe<GameLeaveRequestedEvent>(OnGameLeaveRequested);
        client.StateChanged += OnClientStateChanged;
    }

    private void OnClientStateChanged(MultiplayerClientState state) {
        if (state != MultiplayerClientState.Disconnected)
            return;

        cancellationTokenSource?.Cancel();
        cancellationTokenSource?.Dispose();
        cancellationTokenSource = null;
    }

    private void OnGameLeaveRequested(GameLeaveRequestedEvent _) {
        cancellationTokenSource = new CancellationTokenSource();
        Task.Delay(guardIntervalMs, cancellationTokenSource.Token).ContinueWith(
            _ => OnRequestTimeout(),
            cancellationTokenSource.Token,
            TaskContinuationOptions.None,
            scheduler
        );
    }

    private void OnRequestTimeout() {
        cancellationTokenSource?.Dispose();
        cancellationTokenSource = null;

        log.Info("Game leave guard triggered: no answer from the server, disconnecting manually.");
        client.Disconnect();
    }

}
