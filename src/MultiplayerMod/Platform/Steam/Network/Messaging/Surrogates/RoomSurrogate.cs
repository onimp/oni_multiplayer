using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace MultiplayerMod.Platform.Steam.Network.Messaging.Surrogates;

public class RoomSurrogate : ISerializationSurrogate, ISurrogateType {

    public Type Type => typeof(Room);

    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
        var room = obj as Room;
        var allIds = new List<KPrefabID>(room!.primary_buildings);
        allIds.AddRange(room.buildings);
        allIds.AddRange(room.plants);
        var firstGo = allIds.First().gameObject!;
        var cell = Grid.PosToCell(firstGo);
        info.AddValue("cell", cell);
    }

    public object? SetObjectData(
        object obj,
        SerializationInfo info,
        StreamingContext context,
        ISurrogateSelector selector
    ) {
        var cell = info.GetInt32("cell");
        return global::Game.Instance.roomProber.GetCavityForCell(cell)?.room;
    }
}
