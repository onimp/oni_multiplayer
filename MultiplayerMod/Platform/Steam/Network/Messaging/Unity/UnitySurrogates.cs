using System.Runtime.Serialization;
using UnityEngine;

namespace MultiplayerMod.Platform.Steam.Network.Messaging.Unity;

public static class UnitySurrogates {

    public static readonly SurrogateSelector Selector = new();

    static UnitySurrogates() {
        Selector.AddSurrogate(
            typeof(Vector2),
            new StreamingContext(StreamingContextStates.All),
            new Vector2SerializationSurrogate()
        );
        Selector.AddSurrogate(
            typeof(Vector3),
            new StreamingContext(StreamingContextStates.All),
            new Vector3SerializationSurrogate()
        );
    }

}
