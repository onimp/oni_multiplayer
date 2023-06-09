using System;
using System.Runtime.CompilerServices;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.Game.Context;
using MultiplayerMod.Game.UI.Tools.Context;
using MultiplayerMod.Game.UI.Tools.Events;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class Build : IMultiplayerCommand {

    private BuildEventArgs arguments;

    public Build(BuildEventArgs arguments) {
        this.arguments = arguments;
    }

    public void Execute() {
        var definition = Assets.GetBuildingDef(arguments.PrefabId);
        var cbcPosition = Grid.CellToPosCBC(arguments.Cell, Grid.SceneLayer.Building);
        GameContext.Override(new DisableBuildingValidation(), () => Execute(definition, cbcPosition));
    }

    private void Execute(BuildingDef definition, Vector3 cbcPosition) {
        var building = arguments.Upgrade ? DoUpgrade(definition, cbcPosition) : DoBuild(definition, cbcPosition);
        if (building != null)
            ConfigureBuilding(building);
    }

    private GameObject DoBuild(BuildingDef definition, Vector3 cbcPosition) {
        return arguments.InstantBuild
            ? DoInstantBuild(definition)
            : definition.TryPlace(null, cbcPosition, arguments.Orientation, arguments.Materials, arguments.FacadeId);
    }

    private GameObject DoUpgrade(BuildingDef definition, Vector3 cbcPosition) {
        if (arguments.InstantBuild) {
            var candidate = definition.GetReplacementCandidate(arguments.Cell);
            return InstantUpgrade(definition, candidate);
        }
        var item = definition.TryReplaceTile(null, cbcPosition, arguments.Orientation, arguments.Materials, arguments.FacadeId);
        Grid.Objects[arguments.Cell, (int) definition.ReplacementLayer] = item;
        return item;
    }

    public GameObject InstantUpgrade(BuildingDef definition, GameObject candidate) {
        if (candidate.GetComponent<SimCellOccupier>() == null) {
            UnityObject.Destroy(candidate);
            return DoInstantBuild(definition);
        }
        candidate.GetComponent<SimCellOccupier>().DestroySelf(
            () => {
                UnityObject.Destroy(candidate);
                ConfigureBuilding(DoInstantBuild(definition));
            }
        );
        return null;
    }

    private void ConfigureBuilding(GameObject builtItem) {
        if (!arguments.InstantBuild) {
            var prioritizable = builtItem.GetComponent<Prioritizable>();
            if (prioritizable != null)
                prioritizable.SetMasterPriority(arguments.Priority);
        }
        var rotatable = builtItem.GetComponent<Rotatable>();
        if (rotatable != null)
            rotatable.SetOrientation(arguments.Orientation);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private GameObject DoInstantBuild(BuildingDef definition) => definition.Build(
        arguments.Cell,
        arguments.Orientation,
        null,
        arguments.Materials,
        293.15f,
        arguments.FacadeId,
        false,
        GameClock.Instance.GetTime()
    );

}
