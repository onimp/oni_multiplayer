using System;
using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Game.Events;
using MultiplayerMod.Multiplayer.Commands.GameTools;
using MultiplayerMod.Multiplayer.Commands.Speed;
using MultiplayerMod.Multiplayer.Commands.State;
using MultiplayerMod.Multiplayer.Patches;
using MultiplayerMod.Multiplayer.Tools;
using MultiplayerMod.Network;

namespace MultiplayerMod.Multiplayer.Configuration;

public class GameEventBindings {

    private readonly Core.Logging.Logger log = LoggerFactory.GetLogger<GameEventBindings>();

    private readonly IMultiplayerClient client = Container.Get<IMultiplayerClient>();

    private readonly CommandRateThrottle throttle10Hz = new(rate: 10);

    private bool bound;

    public void Bind() {
        if (bound)
            return;

        log.Debug("Binding game events");

        GameSpeedControlEvents.GamePaused += () => client.Send(new PauseGame());
        GameSpeedControlEvents.GameResumed += () => client.Send(new ResumeGame());
        GameSpeedControlEvents.SpeedChanged += speed => client.Send(new ChangeGameSpeed(speed));

        // TODO: Cursor update may be ignored if MouseMoved isn't triggered after the rate period.
        // TODO: Will be changed later (probably with current / last sent positions check).
        InterfaceToolEvents.MouseMoved += position => throttle10Hz.Run<UpdateCursorPosition>(
            () => client.Send(new UpdateCursorPosition(client.Player, position))
        );

        BindTools();

        bound = true;
    }

    [Obsolete("For payload-based compatibility")]
    private void BindTools() {
        DragToolPatches.BaseUtilityBuildToolPatch.OnUtilityTool +=
            p => client.Send(new UseTool(GameToolType.UtilityBuild, p));
        DragToolPatches.BaseUtilityBuildToolPatch.OnWireTool +=
            p => client.Send(new UseTool(GameToolType.WireBuild, p));
        DragToolPatches.BuildToolPatch.OnDragTool += p => client.Send(new UseTool(GameToolType.Build, p));
        DragToolPatches.CopySettingsToolPatch.OnDragTool += p => client.Send(new UseTool(GameToolType.CopySettings, p));
        DragToolPatches.DebugToolPatch.OnDragTool += p => client.Send(new UseTool(GameToolType.Debug, p));
        DragToolPatches.PlaceToolPatch.OnDragTool += p => client.Send(new UseTool(GameToolType.Place, p));
    }

}
