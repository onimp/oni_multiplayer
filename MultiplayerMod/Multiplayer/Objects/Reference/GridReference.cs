using System;
using MultiplayerMod.Game.Extension;
using UnityEngine;

namespace MultiplayerMod.Multiplayer.Objects.Reference;

[Serializable]
public class GridReference : GameObjectReference {

    public int Cell { get; }
    public int Layer { get; }

    public GridReference(int cell, int layer) {
        Cell = cell;
        Layer = layer;
    }

    public GridReference(GameObject gameObject) {
        var extension = gameObject.GetComponent<GameObjectExtension>();
        Cell = Grid.PosToCell(gameObject);
        Layer = extension?.GridLayer ?? 0;
    }

    protected override GameObject Resolve() => Grid.Objects[Cell, Layer];

    public override string ToString() => $"{{ Cell = {Cell}, Layer = {Layer} }}";

}
