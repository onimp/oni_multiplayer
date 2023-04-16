using System;
using System.Runtime.CompilerServices;
using MultiplayerMod.Core.Unity;
using MultiplayerMod.Game.Building;
using MultiplayerMod.Game.Events.Tools;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Commands.Tools;

[Serializable]
public class Build : IMultiplayerCommand {

    private BuildEventArgs @event;

    public Build(BuildEventArgs @event) {
        this.@event = @event;
    }

    public void Execute() {
        var definition = Assets.GetBuildingDef(@event.PrefabId);
        var cbcPosition = Grid.CellToPosCBC(@event.Cell, Grid.SceneLayer.Building);
        try {
            BuildValidation.Enabled = false;
            Execute(definition, cbcPosition);
        } finally {
            BuildValidation.Enabled = true;
        }
    }

    private void Execute(BuildingDef definition, Vector3 cbcPosition) {
        var building = @event.Upgrade ? DoUpgrade(definition, cbcPosition) : DoBuild(definition, cbcPosition);
        if (building != null)
            ConfigureBuilding(building);
    }

    private GameObject DoBuild(BuildingDef definition, Vector3 cbcPosition) {
        return @event.InstantBuild
            ? DoInstantBuild(definition)
            : definition.TryPlace(null, cbcPosition, @event.Orientation, @event.Materials, @event.FacadeId);
    }

    private GameObject DoUpgrade(BuildingDef definition, Vector3 cbcPosition) {
        if (@event.InstantBuild) {
            var candidate = definition.GetReplacementCandidate(@event.Cell);
            return InstantUpgrade(definition, candidate);
        }
        var item = definition.TryReplaceTile(null, cbcPosition, @event.Orientation, @event.Materials, @event.FacadeId);
        Grid.Objects[@event.Cell, (int) definition.ReplacementLayer] = item;
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
        if (!@event.InstantBuild) {
            var prioritizable = builtItem.GetComponent<Prioritizable>();
            if (prioritizable != null)
                prioritizable.SetMasterPriority(@event.Priority);
        }
        var rotatable = builtItem.GetComponent<Rotatable>();
        if (rotatable != null)
            rotatable.SetOrientation(@event.Orientation);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private GameObject DoInstantBuild(BuildingDef definition) => definition.Build(
        @event.Cell,
        @event.Orientation,
        null,
        @event.Materials,
        293.15f,
        @event.FacadeId,
        false,
        GameClock.Instance.GetTime()
    );

}
