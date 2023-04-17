using System;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Game.Effects;
using MultiplayerMod.Game.Events.Tools;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class CopySettings : IMultiplayerCommand {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger<CopySettings>();

    private CopySettingsEventArgs @event;

    public CopySettings(CopySettingsEventArgs @event) {
        this.@event = @event;
    }

    public void Execute() {
        try {
            PopUpEffects.Enabled = false;
            DoExecute();
        } finally {
            PopUpEffects.Enabled = true;
        }
    }

    // ReSharper disable once Unity.IncorrectMonoBehaviourInstantiation
    private void DoExecute() {
        var tool = new CopySettingsTool();
        var source = Grid.Objects[@event.SourceCell, (int) @event.SourceLayer];
        if (source == null) {
            log.Warning($"Unable to locate source at cell #{@event.SourceCell} on {@event.SourceLayer} layer");
            return;
        }
        tool.SetSourceObject(source);
        @event.DragEvent.Cells.ForEach(it => tool.OnDragTool(it, 0));
    }

}
