using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Game.Events;
using MultiplayerMod.Game.Screens;
using MultiplayerMod.Game.Tools.Events;
using MultiplayerMod.Multiplayer.Commands.Screens.Consumable;
using MultiplayerMod.Multiplayer.Commands.Screens.Research;
using MultiplayerMod.Multiplayer.Commands.Screens.Schedule;
using MultiplayerMod.Multiplayer.Commands.Speed;
using MultiplayerMod.Multiplayer.Commands.State;
using MultiplayerMod.Multiplayer.Commands.Tools;
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

        BindSpeedControl();
        BindMouse();
        BingColonyControls();
        BindTools();

        bound = true;
    }

    private void BindSpeedControl() {
        GameSpeedControlEvents.GamePaused += () => client.Send(new PauseGame());
        GameSpeedControlEvents.GameResumed += () => client.Send(new ResumeGame());
        GameSpeedControlEvents.SpeedChanged += speed => client.Send(new ChangeGameSpeed(speed));
    }

    private void BindMouse() {
        // TODO: Cursor update may be ignored if MouseMoved isn't triggered after the rate period.
        // TODO: Will be changed later (probably with current / last sent positions check).
        InterfaceToolEvents.MouseMoved += position => throttle10Hz.Run<UpdateCursorPosition>(
            () => client.Send(new UpdateCursorPosition(client.Player, position))
        );
    }

    private void BingColonyControls() {
        ResearchEvents.ResearchCanceled += techId => client.Send(new CancelResearch(techId));
        ResearchEvents.ResearchSelected += techId => client.Send(new SelectResearch(techId));

        ConsumableEvents.PermittedByDefault +=
            permittedList => client.Send(new PermitConsumableByDefault(permittedList));
        ConsumableEvents.PermittedToMinion += (properName, consumableId, isAllowed) =>
            client.Send(new PermitConsumableToMinion(properName, consumableId, isAllowed));

        ScheduleEvents.SchedulesChanged += schedules => client.Send(new ChangeSchedulesList(schedules));
    }

    private void BindTools() {
        DragToolEvents.DragComplete += (sender, args) => {
            // @formatter:off
            switch (sender) {
                case DigTool: client.Send(new Dig(args)); break;
                case CancelTool: client.Send(new Cancel(args)); break;
                case DeconstructTool: client.Send(new Deconstruct(args)); break;
                case PrioritizeTool: client.Send(new Prioritize(args)); break;
                case DisinfectTool: client.Send(new Disinfect(args)); break;
                case ClearTool: client.Send(new Sweep(args)); break;
                case AttackTool: client.Send(new Commands.Tools.Attack(args)); break;
                case MopTool: client.Send(new Mop(args)); break;
                case CaptureTool: client.Send(new Wrangle(args)); break;
                case HarvestTool: client.Send(new Harvest(args)); break;
                case EmptyPipeTool: client.Send(new EmptyPipe(args)); break;
                case DisconnectTool: client.Send(new Disconnect(args)); break;
            }
            // @formatter:on
        };

        UtilityBuildEvents.Build += (sender, args) => {
            // @formatter:off
            switch (sender) {
                case UtilityBuildTool: client.Send(new BuildUtility(args)); break;
                case WireBuildTool: client.Send(new BuildWire(args)); break;
            }
            // @formatter:on
        };

        BuildEvents.Build += (_, args) => client.Send(new Build(args));
        CopySettingsEvents.Copy += (_, args) => client.Send(new CopySettings(args));
        DebugToolEvents.Modify += (_, args) => client.Send(new Modify(args));
        StampToolEvents.Stamp += (_, args) => client.Send(new Stamp(args));
    }

}
