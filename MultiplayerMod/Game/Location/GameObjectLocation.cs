using System;
using MultiplayerMod.Game.Extension;
using UnityEngine;

namespace MultiplayerMod.Game.Location;

[Serializable]
public class GameObjectLocation {

    public int Cell { get; }
    public int Layer { get; }

    public GameObjectLocation(int cell, int layer) {
        Cell = cell;
        Layer = layer;
    }

    public GameObjectLocation(GameObject gameObject) {
        Cell = Grid.PosToCell(gameObject);
        Layer = GetLayer(gameObject);
    }

    private int GetLayer(GameObject gameObject) => gameObject.GetComponent<GameObjectExtension>()?.GridLayer ?? -1;

    public GameObject GetGameObject() => Grid.Objects[Cell, Layer];

    public override string ToString() => $"{Cell}:{Layer}";

}
