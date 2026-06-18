using System;
using MultiplayerMod.Game.Extension;
using MultiplayerMod.Game.World;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Objects.Reference;

[Serializable]
public class GridReference : GameObjectReference {

    public int Cell { get; }
    public int Layer { get; }
    public int? WorldId { get; }

    public GridReference(int cell, int layer, int? worldId = null) {
        Cell = cell;
        Layer = layer;
        WorldId = worldId;
    }

    public GridReference(GameObject gameObject) {
        var extension = gameObject.GetComponent<GameObjectExtension>();
        Cell = Grid.PosToCell(gameObject);
        Layer = extension != null ? extension.GridLayer : 0;
        WorldId = WorldIdentity.GetCellWorldId(Cell);
    }

    protected override GameObject? ResolveGameObject() {
        if (WorldId.HasValue && WorldIdentity.GetCellWorldId(Cell) is { } currentWorldId && currentWorldId != WorldId)
            return null;
        return Grid.Objects[Cell, Layer];
    }

    public override string ToString() => $"{{ Cell = {Cell}, Layer = {Layer}, WorldId = {WorldId?.ToString() ?? "unknown"} }}";

    protected bool Equals(GridReference other) =>
        Cell == other.Cell
        && Layer == other.Layer
        && (!WorldId.HasValue || !other.WorldId.HasValue || WorldId == other.WorldId);

    public override bool Equals(object? obj) {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        return obj.GetType() == GetType() && Equals((GridReference) obj);
    }

    public override int GetHashCode() => Cell * 397 ^ Layer;

}
