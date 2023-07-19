using MultiplayerMod.Core.Dependency;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Game.Mechanics.Objects;
using MultiplayerMod.Game.UI;
using MultiplayerMod.Game.UI.Screens.Events;
using MultiplayerMod.Game.UI.Tools.Events;
using MultiplayerMod.Multiplayer.Commands.Gameplay;
using MultiplayerMod.Multiplayer.Commands.Overlay;
using MultiplayerMod.Multiplayer.Commands.Screens.Consumable;
using MultiplayerMod.Multiplayer.Commands.Screens.Immigration;
using MultiplayerMod.Multiplayer.Commands.Screens.Priorities;
using MultiplayerMod.Multiplayer.Commands.Screens.Research;
using MultiplayerMod.Multiplayer.Commands.Screens.Schedule;
using MultiplayerMod.Multiplayer.Commands.Screens.Skill;
using MultiplayerMod.Multiplayer.Commands.Screens.UserMenu;
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
        BindScreens();
        BindTools();
        BindOverlays();
        BindMechanics();

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

    private void BindScreens() {
        ResearchScreenEvents.Cancel += techId => client.Send(new CancelResearch(techId));
        ResearchScreenEvents.Select += techId => client.Send(new SelectResearch(techId));

        ConsumableScreenEvents.PermitByDefault +=
            permittedList => client.Send(new PermitConsumableByDefault(permittedList));
        ConsumableScreenEvents.PermitToMinion += (consumableConsumer, consumableId, isAllowed) =>
            client.Send(new PermitConsumableToMinion(consumableConsumer, consumableId, isAllowed));

        ScheduleScreenEvents.Changed += schedules => client.Send(new ChangeSchedulesList(schedules));

        PrioritiesScreenEvents.Set += (choreConsumer, choreGroup, value) =>
            client.Send(new SetPersonalPriority(choreConsumer, choreGroup, value));
        PrioritiesScreenEvents.DefaultSet +=
            (choreGroup, value) => client.Send(new SetDefaultPriority(choreGroup, value));
        PrioritiesScreenEvents.AdvancedSet += value => client.Send(new SetPersonalPrioritiesAdvanced(value));

        SkillScreenEvents.SetHat += (minionIdentity, hat) => client.Send(new SetHat(minionIdentity, hat));
        SkillScreenEvents.MasterSkill += (minionIdentity, skillId) =>
            client.Send(new MasterSkill(minionIdentity, skillId));

        UserMenuButtonEvents.Click += (gameObject, action) => client.Send(new ClickUserMenuButton(gameObject, action));

        ImmigrantScreenEvents.Initialize +=
            containers => client.Send(new InitializeImmigration(containers));
        ImmigrantScreenEvents.Proceed += deliverable => client.Send(new ProceedImmigration(deliverable));
        ImmigrantScreenEvents.Reject += () => client.Send(new RejectImmigration());
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

    private void BindOverlays() {
        DiseaseOverlayEvents.DiseaseSettingsChanged += (minGerm, enableAutoDisinfect) =>
            client.Send(new SetDisinfectSettings(minGerm, enableAutoDisinfect));
    }

    private void BindMechanics() {
        ObjectEvents.MethodCalled += args => client.Send(new CallMethod(args));
    }

}
