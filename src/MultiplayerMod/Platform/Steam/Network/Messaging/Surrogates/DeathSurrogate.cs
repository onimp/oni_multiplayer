using System;
using System.Runtime.Serialization;

namespace MultiplayerMod.Platform.Steam.Network.Messaging.Surrogates;

public class DeathSurrogate : ISerializationSurrogate, ISurrogateType {

    public Type Type => typeof(Death);

    public void GetObjectData(object obj, SerializationInfo info, StreamingContext context) {
        var death = (Death) obj;
        info.AddValue("id", death.Id);
    }

    public object SetObjectData(
        object obj,
        SerializationInfo info,
        StreamingContext context,
        ISurrogateSelector selector
    ) {
        var id = info.GetString("id");
        return Db.Get().Deaths.Get(id);
    }
}
