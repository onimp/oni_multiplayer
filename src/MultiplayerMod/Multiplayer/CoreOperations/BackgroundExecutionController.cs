using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Multiplayer.CoreOperations.Events;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.CoreOperations;

// The Steam network pump rides the Unity main thread (SteamClientComponent.Update -> client.Tick ->
// RunCallbacks). When ONI loses focus and Application.runInBackground is false, Unity throttles/pauses the
// update loop, so the pump stops and any in-flight transfer (notably the multi-MB hard-sync save) stalls
// until the window is focused again — the "tabbing out randomly slows the transfer" symptom. Force
// run-in-background on for the duration of a multiplayer session and restore the previous value on stop.
[Dependency, UsedImplicitly]
public class BackgroundExecutionController {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger<BackgroundExecutionController>();

    private bool previousRunInBackground;
    private bool applied;

    public BackgroundExecutionController(EventDispatcher eventDispatcher) {
        eventDispatcher.Subscribe<MultiplayerModeSelectedEvent>(_ => Apply());
        eventDispatcher.Subscribe<StopMultiplayerEvent>(_ => Restore());
    }

    private void Apply() {
        if (applied)
            return;
        previousRunInBackground = Application.runInBackground;
        Application.runInBackground = true;
        applied = true;
        log.Debug("Forced Application.runInBackground = true for the multiplayer session");
    }

    private void Restore() {
        if (!applied)
            return;
        Application.runInBackground = previousRunInBackground;
        applied = false;
        log.Debug($"Restored Application.runInBackground = {previousRunInBackground}");
    }

}
