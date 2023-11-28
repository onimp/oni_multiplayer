using System;
using System.Runtime.Serialization;
using Klei.AI;

namespace MultiplayerMod.Platform.Steam.Network.Messaging.Surrogates;

public class EmoteSurrogate : ISerializationSurrogate, ISurrogateType {

    public Type Type => typeof(Emote);

    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
        var emote = (Emote) obj;
        info.AddValue("id", emote.Id);
    }

    public object SetObjectData(
        object obj,
        SerializationInfo info,
        StreamingContext context,
        ISurrogateSelector selector
    ) {
        var id = info.GetString("id");
        return Db.Get().Emotes.Get(id);
    }
}
