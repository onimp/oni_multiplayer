using System;
using MultiplayerMod.Core.Logging;
using MultiplayerMod.Game.Context;
using MultiplayerMod.Game.Effects;
using MultiplayerMod.Game.Tools.Events;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class CopySettings : IMultiplayerCommand {

    private static readonly Core.Logging.Logger log = LoggerFactory.GetLogger<CopySettings>();

    private CopySettingsEventArgs arguments;

    public CopySettings(CopySettingsEventArgs arguments) {
        this.arguments = arguments;
    }

    public void Execute() {
        GameContext.Override(new DisablePopUpEffects(), DoExecute);
    }

    // ReSharper disable once Unity.IncorrectMonoBehaviourInstantiation
    private void DoExecute() {
        var tool = new CopySettingsTool();
        var source = Grid.Objects[arguments.SourceCell, (int) arguments.SourceLayer];
        if (source == null) {
            log.Warning($"Unable to locate source at cell #{arguments.SourceCell} on {arguments.SourceLayer} layer");
            return;
        }
        tool.SetSourceObject(source);
        arguments.DragEvent.Cells.ForEach(it => tool.OnDragTool(it, 0));
    }

}
