using System;
using JetBrains.Annotations;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Events;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.ModRuntime;

namespace MultiplayerMod.Multiplayer.UI.Dev;

[UsedImplicitly]
[Dependency]
public class DevToolsConfigurer {

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<DevToolsConfigurer>();

    public DevToolsConfigurer(EventDispatcher events) {
        if (Type.GetType("DevTool, Assembly-CSharp") == null) {
            log.Info("Dev tools are unavailable on this platform");
            return;
        }

        events.Subscribe<RuntimeReadyEvent>(OnRuntimeReady);
    }

    private void OnRuntimeReady(RuntimeReadyEvent @event) {
        DevToolManager.Instance.RegisterDevTool<DevToolMultiplayerObjects>("Multiplayer/Objects");
    }

}
