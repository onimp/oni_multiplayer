using System.Runtime.Serialization;

namespace MultiplayerMod.Platform.Steam.Network.Messaging.Surrogates;

public static class SerializationSurrogates {

    public static readonly SurrogateSelector Selector = new();

    static SerializationSurrogates() {
        Selector.Add(new Vector2SerializationSurrogate());
        Selector.Add(new Vector3SerializationSurrogate());
        Selector.Add(new PrioritySettingSurrogate());
        Selector.Add(new TagSurrogate());
        Selector.Add(new PathNodeSurrogate());
    }

    private static void Add<T>(this SurrogateSelector selector, T surrogate) where T : ISerializationSurrogate, ISurrogateType {
        selector.AddSurrogate(surrogate.Type, new StreamingContext(StreamingContextStates.All), surrogate);
    }

}
